using Disly.Areas.Admin.Models;
using System;
using System.Web.Mvc;
using Disly.Areas.Admin.Service;
using System.Linq;
using cms.dbModel.entity;
using System.Web;
using System.Text.RegularExpressions;

namespace Disly.Areas.Admin.Controllers
{
    public class SiteMapController : CoreController
    {
        // Модель для вывода в представление
        private SiteMapViewModel model;

        // Фильтр
        private FilterParams filter;

        // Кол-во элементов на странице
        int pageSize = 100;
        
        /// <summary>
        /// Обрабатывается до вызыва экшена
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            model = new SiteMapViewModel
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                FrontSectionList = _cmsRepository.getSiteMapFrontSectionList(Domain),
                MenuTypes = _cmsRepository.getSiteMapMenuTypes()
            };

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        /// <summary>
        /// Список эл-тов карты сайта
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            // Наполняем фильтр значениями
            filter = getFilter(pageSize);

            // Наполняем модель списка данными
            model.List = _cmsRepository.getSiteMapList(Domain, filter);

            ViewBag.Group = filter.Group;

            return View(model);
        }

        /// <summary>
        /// Едичная запись эл-та карты сайта
        /// </summary>
        /// <param name="id">Идентификатор карты сайта</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Item(Guid id)
        {
            // текущий элемент карты сайта
            model.Item = _cmsRepository.getSiteMapItem(id);

            ViewBag.DataPath = Settings.UserFiles + Domain + Settings.SiteMapDir;
            
            if (model.Item == null)
            {
                model.Item = new SiteMapModel()
                {
                    Id = id
                };
                #region опредлим родителя чтобы построить путь в директорию
                string parent = Request.QueryString["parent"];
                if (parent != String.Empty && parent != null && parent != "")
                {
                    var parentitem = _cmsRepository.getSiteMapItem(Guid.Parse(parent));
                    ViewBag.DataPath = ViewBag.DataPath + parentitem.Path + parentitem.Alias + "/";                    
                }
                #endregion
            }
            else
            {
                ViewBag.DataPath = ViewBag.DataPath + model.Item.Path + "/" + model.Item.Alias + "/";
            }
            ViewBag.DataPath = ViewBag.DataPath.Replace("//", "/");
            var mg = new MultiSelectList(model.MenuTypes, "value", "text", model.Item?.MenuGroups);
            ViewBag.GroupMenu = mg;

            var aviable = (model.MenuTypes != null) ?
                        (model.MenuTypes.Where(p => p.available).Any()) ?
                                    model.MenuTypes.Where(p => p.available).ToArray() : new Catalog_list[] { }
                                        : new Catalog_list[] { };
            var mgAviable = new MultiSelectList(aviable, "value", "text", model.Item?.MenuGroups);
            ViewBag.GroupMenuAviable = mgAviable;


            if (model.Item != null)
                model.Item.MenuGroups = null;

            if (!string.IsNullOrEmpty(Request.QueryString["parent"]))
            {
                Guid? _parent = (string.IsNullOrEmpty(Request.QueryString["parent"]) && model.Item != null) ? model.Item.ParentId
                        : Guid.Parse(Request.QueryString["parent"]);
                // хлебные крошки
                model.BreadCrumbs = _cmsRepository.getSiteMapBreadCrumbs(_parent);
                model.Item.ParentId = _parent;
            }
            else if (model.Item != null)
            {
                model.BreadCrumbs = _cmsRepository.getSiteMapBreadCrumbs(model.Item.Id);
            }

            // список дочерних элементов
            model.Childrens = _cmsRepository.getSiteMapChildrens(id);

            return View(model);
        }

        /// <summary>
        /// POST-обработка эл-та карты сайта
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="back_model">Возвращаемая модель</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Item(Guid id, SiteMapViewModel back_model, HttpPostedFileBase upload)
        {
            ErrorMessage userMessage = new ErrorMessage
            {
                title = "Информация"
            };
            
            #region Данные необходимые для сохранения
            back_model.Item.Id = id;
            if (string.IsNullOrEmpty(Request.Form["Item_ParentId"]))
            {
                back_model.Item.ParentId = null;
            }
            else
            {
                back_model.Item.ParentId = back_model.Item.ParentId != null ? back_model.Item.ParentId : Guid.Parse(Request.Form["Item_ParentId"]); // родительский id
            }

            var p = back_model.Item.ParentId != null ? _cmsRepository.getSiteMapItem((Guid)back_model.Item.ParentId) : null;

            back_model.Item.Path = p == null ? "/" : p.Path + p.Alias + "/";


            back_model.Item.Site = Domain;

            if (String.IsNullOrEmpty(back_model.Item.Alias))
            {
                back_model.Item.Alias = Transliteration.Translit(back_model.Item.Title);
            }
            else
            {
                back_model.Item.Alias = Transliteration.Translit(back_model.Item.Alias);
            }


            if (_cmsRepository.existSiteMap(back_model.Item.Path, back_model.Item.Alias, back_model.Item.Id))// && back_model.Item.OldId!=null
            {
                model.ErrorInfo = new ErrorMessage()
                {
                    title = "Ошибка",
                    info = "Такой алиас на данном уровне уже существует, введите иное значение в поле Алиас",
                    buttons = new ErrorMassegeBtn[]
                    {
                        new ErrorMassegeBtn { url = "#", text = "ок", action = "false", style="primary" }
                    }
                };
                model.Item = back_model.Item;
                // хлебные крошки
                model.BreadCrumbs = _cmsRepository.getSiteMapBreadCrumbs(back_model.Item.ParentId);
                model.Item.ParentId = back_model.Item.ParentId;
                var _mg = new MultiSelectList(model.MenuTypes, "value", "text", model.Item?.MenuGroups);
                ViewBag.GroupMenu = _mg;
                return View("Item", model);
            }


            // хлебные крошки
            model.BreadCrumbs = _cmsRepository.getSiteMapBreadCrumbs(back_model.Item.ParentId);

            // список дочерних элементов
            model.Childrens = _cmsRepository.getSiteMapChildrens(id);
            #endregion
            //определяем занятость входного url
            if (!_cmsRepository.ckeckSiteMapAlias(back_model.Item.Alias, back_model.Item.ParentId.ToString(), id))
            {
                if (ModelState.IsValid)
                {
                    #region Сохранение изображение
                    // путь для сохранения изображения
                    string savePath = Settings.UserFiles + Domain + Settings.SiteMapDir;

                    int width = 264;
                    int height = 70;

                    if (upload != null && upload.ContentLength > 0)
                    {
                        string fileExtension = upload.FileName.Substring(upload.FileName.LastIndexOf(".")).ToLower();

                        var validExtension = (!string.IsNullOrEmpty(Settings.PicTypes)) ? Settings.PicTypes.Split(',') : "jpg,jpeg,png,gif".Split(',');
                        if (!validExtension.Contains(fileExtension.Replace(".", "")))
                        {
                            model.Item = _cmsRepository.getSiteMapItem(id);

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

                        Photo photoNew = new Photo()
                        {
                            Name = id.ToString() + fileExtension,
                            Size = Files.FileAnliz.SizeFromUpload(upload),
                            Url = Files.SaveImageResizeRename(upload, savePath, id.ToString(), width, height)
                        };

                        back_model.Item.Photo = photoNew;
                    }
                    #endregion

                    if (_cmsRepository.checkSiteMap(id))
                    {
                        //Если запись заблокирована от редактирования некоторых полей
                        var siteMapItem = _cmsRepository.getSiteMapItem(id);
                        if (siteMapItem.Blocked && !model.Account.Group.ToLower().Equals("developer") && !model.Account.Group.ToLower().Equals("administrator"))
                        {
                            siteMapItem.Disabled = back_model.Item.Disabled;
                            siteMapItem.DisabledMenu = back_model.Item.DisabledMenu;
                            siteMapItem.Keyw = back_model.Item.Keyw;
                            siteMapItem.Desc = back_model.Item.Desc;
                            siteMapItem.Text = back_model.Item.Text;
                            siteMapItem.Url = back_model.Item.Url;
                            siteMapItem.ParentId = back_model.Item.ParentId;
                            siteMapItem.MenuGroups = back_model.Item.MenuGroups;
                            siteMapItem.Path = back_model.Item.Path;

                            _cmsRepository.updateSiteMapItem(id, siteMapItem);
                        }
                        else
                        {
                            _cmsRepository.updateSiteMapItem(id, back_model.Item); //, AccountInfo.id, RequestUserInfo.IP
                        }

                        userMessage.info = "Запись обновлена";
                    }
                    else
                    {
                        _cmsRepository.createSiteMapItem(id, back_model.Item); //, AccountInfo.id, RequestUserInfo.IP
                        userMessage.info = "Запись добавлена";
                    }



                    string backUrl = back_model.Item.ParentId != null ? "item/" + back_model.Item.ParentId : string.Empty;

                    userMessage.buttons = new ErrorMassegeBtn[]
                    {
                     new ErrorMassegeBtn { url = StartUrl + backUrl, text = "Вернуться в список" },
                     new ErrorMassegeBtn { url = "/Admin/sitemap/item/"+id, text = "ок" }
                    };
                }
                else
                {
                    userMessage.info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом.";

                    userMessage.buttons = new ErrorMassegeBtn[]
                    {
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                    };
                }
            }
            else
            {
                userMessage.info = "Элемент с таким алиасом на этом уровне уже существует";

                userMessage.buttons = new ErrorMassegeBtn[]
                {
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }
            model.Item = _cmsRepository.getSiteMapItem(id);

            var mg = new MultiSelectList(model.MenuTypes, "value", "text", model.Item?.MenuGroups);
            ViewBag.GroupMenu = mg;

            var aviable = (model.MenuTypes != null) ?
                        (model.MenuTypes.Where(t => t.available).Any()) ?
                                    model.MenuTypes.Where(t => t.available).ToArray() : new Catalog_list[] { }
                                        : new Catalog_list[] { };



            var mgAviable = new MultiSelectList(aviable, "value", "text", model.Item != null ? model.Item.MenuGroups : null);
            ViewBag.GroupMenuAviable = mgAviable;


            model.Item.MenuGroups = null;
            model.ErrorInfo = userMessage;

            return View(model);
        }

        /// <summary>
        /// Обработчика кнопки "Назад"
        /// </summary>
        /// <param name="id">Идентификатор эл-та карты сайта</param>
        /// <param name="parent">Родительский идентификатор</param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel(Guid id)
        {
            var p = _cmsRepository.getSiteMapItem(id);

            // получим родительский элемент для перехода
            Guid? _parent = (string.IsNullOrEmpty(Request.Form["Item_ParentId"]) && p != null) ? p.ParentId
                : (string.IsNullOrWhiteSpace(Request.Form["Item_ParentId"]) ? Guid.Empty : Guid.Parse(Request.Form["Item_ParentId"]));

            if (_parent != Guid.Empty && _parent != null)
            {
                return RedirectToAction("Item", new { id = _parent });
            }
            return Redirect(StartUrl + Request.Url.Query);
        }

        /// <summary>
        /// Обработчик события кнопки "Удалить"
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            model.Item = _cmsRepository.getSiteMapItem(id);

            _cmsRepository.deleteSiteMapItem(id); //, AccountInfo.id, RequestUserInfo.IP

            // записываем информацию о результатах
            ErrorMessage userMassege = new ErrorMessage();
            userMassege.title = "Информация";
            userMassege.info = "Запись Удалена";
            userMassege.buttons = new ErrorMassegeBtn[]
            {
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };

            model.ErrorInfo = userMassege;

            if (model.Item.ParentId.Equals(null))
            {
                return Redirect(StartUrl + Request.Url.Query);
            }
            else
            {
                return RedirectToAction("Item", new { id = model.Item.ParentId });
            }
        }
    }
}