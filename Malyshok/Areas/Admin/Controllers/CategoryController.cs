using Disly.Areas.Admin.Models;
using Disly.Areas.Admin.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class CategoryController : CoreController
    {
        private CategoryViewModel model;

        /// <summary>
        /// Обрабатывается до вызыва экшена
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            model = new CategoryViewModel
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ControllerName = ControllerName,
                ActionName = ActionName
            };

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: Admin/Category
        [HttpGet]
        public ActionResult Index()
        {
            // наполняем модель списком категорий
            model.List = _cmsRepository.getCategories();

            return View(model);
        }

        [HttpGet]
        public ActionResult Item(Guid id)
        {
            // единичная запись
            model.Item = _cmsRepository.getCategory(id);

            // список дочерних эл-тов
            model.List = _cmsRepository.getCategories(id);

            Guid? parent = null;
            string parentStr = Request.QueryString["parent"];
            if (!String.IsNullOrWhiteSpace(parentStr))
            {
                parent = Guid.Parse(parentStr);
            }
            if (model.Item != null)
            {
                parent = model.Item.Parent;
            }

            // хлебные крошки
            model.BreadCrumbs = _cmsRepository.getCategoryBreadCrumbs(parent);

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Item(Guid id, CategoryViewModel backModel)
        {
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";

            if (ModelState.IsValid)
            {
                backModel.Item.Id = id;

                if (String.IsNullOrEmpty(backModel.Item.Alias))
                {
                    backModel.Item.Alias = Transliteration.Translit(backModel.Item.Title);
                }
                else
                {
                    backModel.Item.Alias = Transliteration.Translit(backModel.Item.Alias);
                }

                if (_cmsRepository.categoryExists(id))
                {
                    _cmsRepository.updateCategory(backModel.Item);
                    userMessage.info = "Запись обновлена";
                }
                else
                {
                    _cmsRepository.createCategory(backModel.Item);
                    userMessage.info = "Запись добавлена";
                }

                string backUrl = backModel.Item.Parent != null ? "item/" + backModel.Item.Parent : string.Empty;

                userMessage.buttons = new ErrorMassegeBtn[]
                {
                    new ErrorMassegeBtn { url = StartUrl + backUrl, text = "Вернуться в список" },
                    new ErrorMassegeBtn { url = "/Admin/category/item/"+id, text = "ок" }
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
            model.Item = _cmsRepository.getCategory(id);

            // хлебные крошки
            model.BreadCrumbs = _cmsRepository.getCategoryBreadCrumbs(model.Item.Parent);

            model.ErrorInfo = userMessage;
            return View(model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel(Guid id)
        {
            var item = _cmsRepository.getCategory(id);

            Guid? parent = item != null ? item.Parent 
                : !String.IsNullOrWhiteSpace(Request.QueryString["parent"]) 
                ? Guid.Parse(Request.QueryString["parent"]) : Guid.Empty;

            if (parent != Guid.Empty && parent != null)
            {
                return RedirectToAction("Item", new { id = parent });
            }

            return Redirect(StartUrl + Request.Url.Query);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            model.Item = _cmsRepository.getCategory(id);

            _cmsRepository.deleteCategory(id);

            ErrorMessage userMassege = new ErrorMessage();
            userMassege.title = "Информация";
            userMassege.info = "Запись Удалена";
            userMassege.buttons = new ErrorMassegeBtn[]
            {
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };

            model.ErrorInfo = userMassege;

            if (model.Item.Parent.Equals(null))
            {
                return Redirect(StartUrl + Request.Url.Query);
            }
            else
            {
                return RedirectToAction("Item", new { id = model.Item.Parent });
            }
        }
    }
}