using Disly.Areas.Admin.Models;
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

            return View(model);
        }
    }
}