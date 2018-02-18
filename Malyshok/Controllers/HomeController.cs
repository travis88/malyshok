using cms.dbase;
using Disly.Models;
using System;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class HomeController : RootController
    {
        private HomePageViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new HomePageViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArray = bannerArray,
                ProdCatalog = category_list
            };

            IsSpecVersion = HttpContext.Request.Cookies["spec_version"] != null;
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns ></returns>
        public ActionResult Index()
        {
            #region Создаем переменные (значения по умолчанию)
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            string PageTitle = "Главная страница";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion

            model.Materials = _repository.getMaterialsModule(); //Domain
            
            // версия для слабовидящих
            if (IsSpecVersion)
            {
                _ViewName = _ViewName.ToLower().Replace("views/", "views/_spec/");
            }

            return View(_ViewName, model);
        }
    }
}

