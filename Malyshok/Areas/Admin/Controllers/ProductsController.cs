using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class ProductsController : CoreController
    {
        ProductViewModel model;
        FilterParams filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            ViewBag.HttpKeys = Request.QueryString.AllKeys;
            ViewBag.Query = Request.QueryString;

            filter = getFilter(page_size);

            model = new ProductViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Categories = _cmsRepository.getAllCategories()
            };

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: Admin/Products
        public ActionResult Index()
        {
            model.List = _cmsRepository.getProducts(filter);
            model.CategoryFilters = _cmsRepository.getCategoryFilters(Guid.Empty);
            ViewBag.Category = Request.Params["category"];
            return View(model);
        }

        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.getProduct(id);

            if (model.Item == null)
            {
                model.Item = new ProductModel
                {
                    Id = id,
                    Date = DateTime.Now
                };
            }
            
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, ProductViewModel bindData)
        {
            ErrorMessage userMessage = new ErrorMessage()
            {
                title = "Информация"
            };

            if (ModelState.IsValid)
            {
                bool result = false;
                var item = _cmsRepository.getProduct(id);

                // добавление необходимых полей перед сохранением модели
                bindData.Item.Id = id;

                #region Сохранение изображения
                //var width = 0;
                //var height = 0;
                //var defaultPreviewSizes = new string[] { "540", "360" };

                // путь для сохранения изображения //Preview image
                //string savePath = Settings.UserFiles + Domain + Settings.ProductsDir;
                //if (upload != null && upload.ContentLength > 0)
                //{
                //    string fileExtension = upload.FileName.Substring(upload.FileName.LastIndexOf(".")).ToLower();

                //    var validExtension = (!string.IsNullOrEmpty(Settings.PicTypes)) ? Settings.PicTypes.Split(',') : "jpg,jpeg,png,gif".Split(',');
                //    if (!validExtension.Contains(fileExtension.Replace(".", "")))
                //    {
                //        model.Item = _cmsRepository.getProduct(id);

                //        model.ErrorInfo = new ErrorMessage()
                //        {
                //            title = "Ошибка",
                //            info = "Вы не можете загружать файлы данного формата",
                //            buttons = new ErrorMassegeBtn[]
                //            {
                //             new ErrorMassegeBtn { url = "#", text = "ок", action = "false", style="primary" }
                //            }
                //        };
                //        return View("Item", model);
                //    }

                //    var sizes = (!string.IsNullOrEmpty(Settings.MaterialPreviewImgSize)) ? Settings.MaterialPreviewImgSize.Split(',') : defaultPreviewSizes;
                //    int.TryParse(sizes[0], out width);
                //    int.TryParse(sizes[1], out height);
                //}
                #endregion

                //Определяем Insert или Update
                if (item != null)
                {
                    userMessage.info = "Запись обновлена";
                    result = _cmsRepository.updateProduct(bindData.Item);
                }

                else
                {
                    userMessage.info = "Запись добавлена";
                    result = _cmsRepository.insertProduct(bindData.Item);
                }
                //Сообщение пользователю
                if (result)
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

            model.Item = _cmsRepository.getProduct(id);
            model.ErrorInfo = userMessage;

            return View("Item", model);
        }

        /// <summary>
        /// Формируем строку фильтра
        /// </summary>
        /// <param name="title_serch">Поиск по названию</param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string searchtext, string size, DateTime? date, DateTime? dateend, string category, bool disabled = false)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = addFiltrParam(query, "searchtext", searchtext);
            query = addFiltrParam(query, "disabled", disabled.ToString().ToLower());
            query = (date == null) ? addFiltrParam(query, "date", String.Empty) : addFiltrParam(query, "date", ((DateTime)date).ToString("dd.MM.yyyy").ToLower());
            query = (dateend == null) ? addFiltrParam(query, "dateend", String.Empty) : addFiltrParam(query, "dateend", ((DateTime)dateend).ToString("dd.MM.yyyy").ToString().ToLower());
            query = addFiltrParam(query, "page", String.Empty);
            query = addFiltrParam(query, "size", size);
            query = addFiltrParam(query, "category", category);

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
            ErrorMessage userMessage = new ErrorMessage()
            {
                title = "Информация"
            };

            userMessage.info = "Ошибка, Запись не удалена";
            userMessage.buttons = new ErrorMassegeBtn[]{
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };

            model.Item = _cmsRepository.getProduct(Id);
            if (model.Item != null)
            {
                var image = model.Item.Photo;
                var res = _cmsRepository.deleteProduct(Id);
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

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-all-btn")]
        public ActionResult DeleteAll()
        {
            _cmsRepository.deleteAllProducts();

            //DirectoryInfo[] dirsToDrop = {
            //    new DirectoryInfo(Server.MapPath(Settings.ProdContent)),
            //    new DirectoryInfo(Server.MapPath(Settings.Certificates))
            //};

            //foreach (var dirToDrop in dirsToDrop)
            //{
            //    foreach (FileInfo file in dirToDrop.GetFiles())
            //    {
            //        file.Delete();
            //    }
            //    foreach (DirectoryInfo dir in dirToDrop.GetDirectories())
            //    {
            //        dir.Delete(true);
            //    }
            //}

            return RedirectToAction("Index");
        }
    }
}