using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using Disly.Areas.Admin.Service;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Disly.Areas.Admin.Controllers
{
    public class PhotoAlbumsController : CoreController
    {
        PhotoViewModel model;
        FilterParams filter;

        string[] allowedExtensions = new string[] { ".jpg", ".jpeg", ".gif", ".png" };

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            ViewBag.HttpKeys = Request.QueryString.AllKeys;
            ViewBag.Query = Request.QueryString;

            filter = getFilter(page_size);

            model = new PhotoViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ControllerName = ControllerName,
                ActionName = ActionName
            };

            //Справочник всех доступных категорий
            MaterialsGroup[] GroupsValues = _cmsRepository.getAllMaterialGroups();
            ViewBag.AllGroups = GroupsValues;

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: Materials
        public ActionResult Index(string category, string type)
        {
            model.List = _cmsRepository.getPhotoAlbum(filter);
            return View(model);
        }

        /// <summary>
        /// Форма редактирования записи
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid id)
        {
            model.Album = _cmsRepository.getPhotoAlbumItem(id);
            if (model.Album == null)
            {
                model.Album = new PhotoAlbum();
                model.Album.Date = DateTime.Now;

            }
            ViewBag.DataPath = Settings.UserFiles + Domain + Settings.PhotoDir+id.ToString()+"/";
            return View("Item", model);
        }

        /// <summary>
        /// Формируем строку фильтра
        /// </summary>
        /// <param name="title_serch">Поиск по названию</param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string searchtext, bool disabled, string size, DateTime? date, DateTime? dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = addFiltrParam(query, "searchtext", searchtext);
            query = addFiltrParam(query, "disabled", disabled.ToString().ToLower());
            query = (date == null) ? addFiltrParam(query, "date", String.Empty) : addFiltrParam(query, "date", ((DateTime)date).ToString("dd.MM.yyyy").ToLower());
            query = (dateend == null) ? addFiltrParam(query, "dateend", String.Empty) : addFiltrParam(query, "dateend", ((DateTime)dateend).ToString("dd.MM.yyyy").ToString().ToLower());
            query = addFiltrParam(query, "page", String.Empty);
            query = addFiltrParam(query, "size", size);

            return Redirect(StartUrl + query);
        }

        /// <summary>
        /// Очищаем фильтр
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        public ActionResult ClearFiltr()
        {
            return Redirect(StartUrl);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "insert-btn")]
        public ActionResult Insert()
        {
            //  При создании записи сбрасываем номер страницы
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = addFiltrParam(query, "page", String.Empty);

            return Redirect(StartUrl + "Item/" + Guid.NewGuid() + "/" + query);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, PhotoViewModel bindData, HttpPostedFileBase upload, IEnumerable<HttpPostedFileBase> uploadPhoto)
        {
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";

            string savePath = Settings.UserFiles + Domain + Settings.PhotoDir + bindData.Album.Date.ToString("yyyy") + "_" + bindData.Album.Date.ToString("MM") + "/" + bindData.Album.Date.ToString("dd") + "/" + id + "/";
            bindData.Album.Path = savePath;
            if (ModelState.IsValid)
            {
                //превью
                #region Сохранение изображения
                if (upload == null && uploadPhoto != null)
                {
                    if (uploadPhoto.Count() > 0)
                    {
                        upload = uploadPhoto.ToArray()[0];
                    }
                }
                var width = 0;
                var height = 0;
                var defaultPreviewSizes = new string[] { "540", "360" };
                if (upload != null && upload.ContentLength > 0)
                {
                    string fileExtension = upload.FileName.Substring(upload.FileName.LastIndexOf(".")).ToLower();

                    var validExtension = (!string.IsNullOrEmpty(Settings.PicTypes)) ? Settings.PicTypes.Split(',') : "jpg,jpeg,png,gif".Split(',');
                    if (!validExtension.Contains(fileExtension.Replace(".", "")))
                    {
                        model.ErrorInfo = new ErrorMessage()
                        {
                            title = "Ошибка",
                            info = "Вы не можете загружать файлы данного формата",
                            buttons = new ErrorMassegeBtn[]
                            {
                             new ErrorMassegeBtn { url = "#", text = "ок", action = "false", style="primary" }
                            }
                        };
                        return View("Item", model);
                    }

                    var sizes = (!string.IsNullOrEmpty(Settings.MaterialPreviewImgSize)) ? Settings.MaterialPreviewImgSize.Split(',') : defaultPreviewSizes;
                    int.TryParse(sizes[0], out width);
                    int.TryParse(sizes[1], out height);
                    bindData.Album.PreviewImage = new Photo()
                    {
                        Name = id.ToString() + fileExtension,
                        Size = Files.FileAnliz.SizeFromUpload(upload),
                        Url = Files.SaveImageResizeRename(upload, savePath, "logo", width, height)
                    };
                }

                #endregion


                var getAlbum = _cmsRepository.getPhotoAlbumItem(id);
                bindData.Album.Id = id;
                var status = false;//если trueдобавляем в фотоальбом фотографии иначе - не добавляем

                //Определяем Insert или Update
                if (getAlbum != null)
                    if (_cmsRepository.updPhotoAlbum(id, bindData.Album))
                    {
                        status = true;
                        userMessage.info = "Запись обновлена";
                    }
                    else
                    { userMessage.info = "Произошла ошибка"; }
                else
                {
                    //превью альбома


                    if (_cmsRepository.insPhotoAlbum(id, bindData.Album))
                    {
                        userMessage.info = "Запись добавлена";
                        status = true;
                    }
                    else { userMessage.info = "Произошла ошибка"; }
                }
                if (status)
                {
                    #region save-photos                    
                    int counter = 0;
                    string serverPath = savePath;

                    PhotoModel[] photoList = new PhotoModel[uploadPhoto.Count()];

                    foreach (HttpPostedFileBase photos in uploadPhoto)
                    {
                        if (photos != null && photos.ContentLength > 0)
                        {
                            //try
                            //{
                            if (!allowedExtensions.Contains(Path.GetExtension(photos.FileName.ToLower())))
                            {
                                Exception ex = new Exception("неверный формат файла \"" + Path.GetFileName(photos.FileName) + "\". Доступные расширения: .jpg, .jpeg, .png, .gif");
                                throw ex;
                            }
                            if (!Directory.Exists(Server.MapPath(serverPath))) { DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(serverPath)); }

                            double filesCount = Directory.EnumerateFiles(Server.MapPath(serverPath)).Count();
                            double newFilenameInt = Math.Ceiling(filesCount / 2) + 1;
                            string newFilename = newFilenameInt.ToString() + ".jpg";

                            while (System.IO.File.Exists(Server.MapPath(Path.Combine(serverPath, newFilename))))
                            {
                                newFilenameInt++;
                                newFilename = newFilenameInt.ToString() + ".jpg";
                            }

                            //сохраняем оригинал
                            photos.SaveAs(Server.MapPath(Path.Combine(serverPath, newFilename)));

                            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                            EncoderParameters myEncoderParameters = new EncoderParameters(1);
                            myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L); //cжатие 90


                            Bitmap _File = (Bitmap)Bitmap.FromStream(photos.InputStream);
                            //оригинал
                            Bitmap _FileOrigin = Imaging.Resize(_File, 4000, "width");
                            _FileOrigin.Save(Server.MapPath(serverPath + "/" + newFilename), myImageCodecInfo, myEncoderParameters);

                            //сохраняем full hd
                            Bitmap _FileHd = Imaging.Resize(_File, 2000, "width");
                            _FileHd.Save(Server.MapPath(serverPath + "/hd_" + newFilename), myImageCodecInfo, myEncoderParameters);

                            //сохраняем превью
                            Bitmap _FilePrev = Imaging.Resize(_File, 120, 120, "center", "center");
                            _FilePrev.Save(Server.MapPath(serverPath + "/prev_" + newFilename), myImageCodecInfo, myEncoderParameters);


                            photoList[counter] = new PhotoModel()
                            {
                                Id = Guid.NewGuid(),
                                AlbumId = id,
                                Title = Path.GetFileName(photos.FileName),
                                Date = DateTime.Now,
                                PreviewImage = new Photo { Url = serverPath + "/prev_" + newFilename },
                                PhotoImage = new Photo { Url = serverPath + "/" + newFilename }
                            };
                            counter++;

                            //записываем обложку фотоальбома


                            //}
                            //catch (Exception ex)
                            //{
                            //    ViewBag.Message = "Произошла ошибка: " + ex.Message.ToString();
                            //    break;
                            //}
                        }
                        else
                        {
                            ModelState.AddModelError("", "Фотоальбом должен содержать хотя бы одну фотографию.");
                            break;
                        }
                    }
                    #endregion
                    //model.Album.PreviewImage = new Photo { Url = photoList[0].PreviewImage.Url };
                    _cmsRepository.insertPhotos(id, photoList);
                }
                userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                 };
            }
            else
            {
                userMessage.info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом.";
                userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                 };
            }

            model.Album = _cmsRepository.getPhotoAlbumItem(id);
            if (model.Album != null && model.Album.PreviewImage != null && !string.IsNullOrEmpty(model.Album.PreviewImage.Url))
            {
                model.Album.PreviewImage = Files.getInfoImage(model.Album.PreviewImage.Url);
            }
            model.ErrorInfo = userMessage;

            return View("Item", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return Redirect(StartUrl + Request.Url.Query);
        }
        /// <summary>
        /// удаляем фотоальбом и входящие в него фотографии
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid Id)
        {

            // записываем информацию о результатах
            ErrorMessage userMassege = new ErrorMessage();
            userMassege.title = "Информация";

            var data = _cmsRepository.getPhotoAlbumItem(Id);
            if (data != null)
            {
                var delpath = data.Path;

                if (_cmsRepository.delPhotoAlbum(Id))
                {
                    userMassege.info = "Запись Удалена";
                    #region удаление файлов
                    try
                    {
                        try
                        {
                            Directory.Delete(Server.MapPath(delpath), true);
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(0);
                            Directory.Delete(Server.MapPath(delpath), true);
                        }
                    }
                    catch
                    {
                        //на случай когда в базе есть - а физически изображений не существует
                    }
                    #endregion
                }
                else
                {
                    userMassege.info = "Произошла ошибка";
                }
            }
            userMassege.buttons = new ErrorMassegeBtn[]{
                new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };
            model.ErrorInfo = userMassege;
            //return RedirectToAction("Index");
            return View("Item", model);
        }


        // Удаление одной фотографии из фотоальбома
        [HttpPost]
        public string DeletePhoto(string id)
        {
            PhotoModel photo = _cmsRepository.getPhotoItem(Guid.Parse(id));
            if (photo != null)
            {
                string photoPath = photo.PhotoImage.Url.ToString();
                string previewPath = photo.PreviewImage.Url.ToString();
                string hdPath = previewPath.Replace("prev_", "hd_");
                if (_cmsRepository.delPhotoItem(Guid.Parse(id)))
                {
                    try
                    {
                        if (System.IO.File.Exists(Server.MapPath(photoPath)))
                        {
                            System.IO.File.Delete(Server.MapPath(photoPath));
                            System.IO.File.Delete(Server.MapPath(previewPath));
                            System.IO.File.Delete(Server.MapPath(hdPath));
                            return "true";
                        }
                        else return "Не удалось удалить фотографию.";
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(0);
                        if (System.IO.File.Exists(Server.MapPath(photoPath)))
                        {
                            System.IO.File.Delete(Server.MapPath(photoPath));
                            System.IO.File.Delete(Server.MapPath(previewPath));
                            System.IO.File.Delete(Server.MapPath(hdPath));
                            return "true";
                        }
                        else return "Не удалось удалить фотографию.";
                    }
                }
                else return "Не удалось удалить фотографию.";


            }
            return "Не удалось удалить фотографию.";



            //return (_repository.deletePhoto(Guid.Parse(id))) ? "" : "Не удалось удалить фотографию.";
        }

        /// <summary>
        /// Получаем альбомы в формате JSON
        /// </summary>
        /// <returns></returns>
        public JsonResult Api(int page = 1, int size = 30)
        {
            filter.Page = page;
            filter.Size = size;

            PhotoAlbumList list = _cmsRepository.getPhotoAlbum(filter);
            model.List = _cmsRepository.getPhotoAlbum(filter);


            if (model.List == null)
                model.List = new PhotoAlbumList()
                {
                    Data = null,
                    Pager = new Pager()
                    {
                        items_count = 0,
                        page_count = 1,
                        page = 1,
                        size = 30
                    }
                };

            var json = Json(model.List, JsonRequestBehavior.AllowGet);

            return json;
        }
    }
}