using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using Disly.Areas.Admin.Service;
using System;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class SiteSectionController : CoreController
    {
        SiteSectionViewModel model;
        FilterParams filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            ViewBag.HttpKeys = Request.QueryString.AllKeys;
            ViewBag.Query = Request.QueryString;

            filter = getFilter(page_size);

            model = new SiteSectionViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                SiteType = new SiteMapMenu[]
                {
                    new SiteMapMenu { Value = "org", Text = "Организации" },
                    new SiteMapMenu { Value = "main", Text = "Портал" },
                    new SiteMapMenu { Value = "spec", Text = "Специалисты" }
                }
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
            var mfilter = getFilter();
            model.List = _cmsRepository.getSiteSectionList(mfilter);

            return View(model);
        }

        /// <summary>
        /// Форма редактирования записи
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid Id)
        {
            model.Item = _cmsRepository.getSiteSectionItem(Id);
            if (model.Item == null)
                model.Item = new SiteSectionModel()
                {
                    Id = Id
                };
            return View("Item", model);
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
        public ActionResult Save(Guid Id, SiteSectionViewModel bindData)
        {
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";

            if (ModelState.IsValid)
            {
                var res = false;
                var getSiteSection = _cmsRepository.getSiteSectionItem(Id);

                // добавление необходимых полей перед сохранением модели
                bindData.Item.Id = Id;

                if (String.IsNullOrEmpty(bindData.Item.Alias))
                {
                    bindData.Item.Alias = Transliteration.Translit(bindData.Item.Title);
                }
                else
                {
                    bindData.Item.Alias = Transliteration.Translit(bindData.Item.Alias);
                }

                //Определяем Insert или Update
                if (getSiteSection != null)
                {
                    res = _cmsRepository.updateSiteSection(bindData.Item);
                    userMessage.info = "Запись обновлена";
                }                    
                else
                {
                    res = _cmsRepository.insertSiteSection(bindData.Item);
                    userMessage.info = "Запись добавлена";
                }                
                if (!res)userMessage.info = "Произошла ошибка";

                userMessage.buttons = new ErrorMassegeBtn[]
                {
                     new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
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
            model.Item = _cmsRepository.getSiteSectionItem(Id);
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
            var res = _cmsRepository.deleteSiteSection(Id);

            // записываем информацию о результатах
            ErrorMessage userMassege = new ErrorMessage();
            userMassege.title = "Информация";
            userMassege.info = "Запись Удалена";
            userMassege.buttons = new ErrorMassegeBtn[]
            {
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };

            model.ErrorInfo = userMassege;

            return RedirectToAction("Index");
        }
    }
}