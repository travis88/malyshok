using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using Disly.Areas.Admin.Service;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class MaterialsController : CoreController
    {
        MaterialsViewModel model;
        FilterParams filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            ViewBag.HttpKeys = Request.QueryString.AllKeys;
            ViewBag.Query = Request.QueryString;

            filter = getFilter(page_size);

            model = new MaterialsViewModel()
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
            // Наполняем фильтр значениями
            var mfilter = FilterParams.Extend<MaterialFilter>(filter);
            model.List = _cmsRepository.getMaterialsList(mfilter);

            return View(model);
        }

        /// <summary>
        /// Форма редактирования записи
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid Id)
        {
            model.Item = _cmsRepository.getMaterial(Id);

            ViewBag.DataPath = Settings.UserFiles + Domain + Settings.MaterialsDir;
            ViewBag.DataPath = (model.Item == null)? 
                ViewBag.DataPath + DateTime.Today.ToString("yyyy") + "/" + DateTime.Today.ToString("MM") + "/" + DateTime.Today.ToString("dd") + "/"
                :
                ViewBag.DataPath + model.Item.Date.ToString("yyyy") + "/" + model.Item.Date.ToString("MM") + "/" + model.Item.Date.ToString("dd") + "/";

            if (model.Item == null)
            {
                model.Item = new MaterialsModel()
                {
                    Id = Id,
                    Date = DateTime.Now
                };
            }
            
            if (model.Item.PreviewImage != null && model.Item.PreviewImage != null && !string.IsNullOrEmpty(model.Item.PreviewImage.Url))
            {
                var photo = model.Item.PreviewImage;
                model.Item.PreviewImage = Files.getInfoImage(photo.Url);
                model.Item.PreviewImage.Source = photo.Source;
            }

            //Заполняем для модели связи с другими объектами
            var eventFilter = FilterParams.Extend<EventFilter>(filter);
            eventFilter.RelId = Id;
            eventFilter.RelType = ContentType.MATERIAL;

            var orgfilter = FilterParams.Extend<OrgFilter>(filter);
            orgfilter.RelId = Id;
            orgfilter.RelType = ContentType.MATERIAL;

            model.Item.Links = new ObjectLinks()
            {
                //Persons = null
            };

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
        public ActionResult Save(Guid Id, MaterialsViewModel bindData, HttpPostedFileBase upload)
        {
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";

            if (ModelState.IsValid)
            {
                var res = false;
                var getMaterial = _cmsRepository.getMaterial(Id);

                // добавление необходимых полей перед сохранением модели
                bindData.Item.Id = Id;

                #region Сохранение изображения
                var width = 0;
                var height = 0;
                var defaultPreviewSizes = new string[] { "540", "360" };

                // путь для сохранения изображения //Preview image
                string savePath = Settings.UserFiles + Domain + Settings.MaterialsDir; //+2017_09
                if (upload != null && upload.ContentLength > 0)
                {
                    string fileExtension = upload.FileName.Substring(upload.FileName.LastIndexOf(".")).ToLower();

                    var validExtension = (!string.IsNullOrEmpty(Settings.PicTypes)) ? Settings.PicTypes.Split(',') : "jpg,jpeg,png,gif".Split(',');
                    if (!validExtension.Contains(fileExtension.Replace(".", "")))
                    {
                        model.Item = _cmsRepository.getMaterial(Id);

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
                    bindData.Item.PreviewImage = new Photo()
                    {
                        Name = Id.ToString() + fileExtension,
                        Size = Files.FileAnliz.SizeFromUpload(upload),
                        Url = Files.SaveImageResizeRename(upload, savePath, Id.ToString(), width, height),
                        Source = bindData.Item.PreviewImage.Source
                    };
                }
                #endregion

                if (String.IsNullOrEmpty(bindData.Item.Alias))
                {
                    bindData.Item.Alias = Transliteration.Translit(bindData.Item.Title);
                }
                else
                {
                    bindData.Item.Alias = Transliteration.Translit(bindData.Item.Alias);
                }

                //Определяем Insert или Update
                if (getMaterial != null)
                {
                    userMessage.info = "Запись обновлена";
                    res = _cmsRepository.updateCmsMaterial(bindData.Item);
                }

                else
                {
                    userMessage.info = "Запись добавлена";
                    res = _cmsRepository.insertCmsMaterial(bindData.Item);
                }
                //Сообщение пользователю
                if (res)
                {
                    string currentUrl = Request.Url.PathAndQuery;

                    userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                     new ErrorMassegeBtn { url = currentUrl, text = "ок" }
                 };
                }
                else
                {
                    userMessage.info = "Произошла ошибка";

                    userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false"  }
                 };
                }
            }
            else
            {
                userMessage.info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом.";

                userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                 };
            }

            model.Item = _cmsRepository.getMaterial(Id);
            if (model.Item != null && model.Item.PreviewImage != null && !string.IsNullOrEmpty(model.Item.PreviewImage.Url))
            {
                var photo = model.Item.PreviewImage;
                model.Item.PreviewImage = Files.getInfoImage(model.Item.PreviewImage.Url);
                model.Item.PreviewImage.Source = photo.Source;
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

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid Id)
        {
            // записываем информацию о результатах
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";
            //В случае ошибки
            userMessage.info = "Ошибка, Запись не удалена";
            userMessage.buttons = new ErrorMassegeBtn[]{
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };

            model.Item = _cmsRepository.getMaterial(Id);
            if (model.Item != null)
            {
                var image = (model.Item.PreviewImage != null) ? model.Item.PreviewImage.Url : null;
                var res = _cmsRepository.deleteCmsMaterial(Id);
                if (res)
                {
                    if (!string.IsNullOrEmpty(image))
                        Files.deleteImage(image);

                    // записываем информацию о результатах
                    userMessage.title = "Информация";
                    userMessage.info = "Запись удалена";
                    userMessage.buttons = new ErrorMassegeBtn[]
                     {
                        new ErrorMassegeBtn { url = StartUrl, text = "ок" }
                     };
                }
            }
            model.ErrorInfo = userMessage;

            return View(model);
        }
    }
}