using cms.dbase;
using Disly.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class PressController : RootController
    {
        private NewsViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            currentPage = _repository.getSiteMap("Press");

            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            model = new NewsViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                Breadcrumbs = breadcrumb,
                BannerArray = bannerArray,
                CurrentPage = currentPage,
                Group = _repository.getMaterialsGroup()
            };

            #region Создаем переменные (значения по умолчанию)
            string PageTitle = model.CurrentPage.Title;
            string PageDesc = model.CurrentPage.Desc;
            string PageKeyw = model.CurrentPage.Keyw;
            #endregion

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            ViewBag.Category = (RouteData.Values["category"] != null) ? RouteData.Values["category"] : String.Empty;
            var filter = getFilter();
            filter.Disabled = false;
            model.List = _repository.getMaterialsList(filter);

            ViewBag.Filter = filter;
            ViewBag.NewsSearchArea = filter.SearchText;
            ViewBag.NewsSearchDateStart = filter.Date;
            ViewBag.NewsSearchDateFin = filter.DateEnd;

            return View(_ViewName, model);
        }

        public ActionResult Item(string year, string month, string day, string alias)
        {
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            ViewBag.Day = day.ToString();
            ViewBag.Alias = (RouteData.Values["alias"] != null) ? RouteData.Values["alias"] : String.Empty;
            model.Item = _repository.getMaterialsItem(year, month, day, alias); //,Domain

            return View(_ViewName, model);
        }

        public ActionResult Category(string category)
        {
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            ViewBag.CurrentCategory = category;
            var filter = getFilter();
            filter.Disabled = false;
            filter.Category = category;
            ViewBag.Filter = filter;

            model.List = _repository.getMaterialsList(filter);

            return View(_ViewName, model);
        }

        public ActionResult RssSettings()
        {
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            ViewBag.Category = (RouteData.Values["category"] != null) ? RouteData.Values["category"] : String.Empty;
            var filter = getFilter();
            filter.Disabled = false;
            model.List = _repository.getMaterialsList(filter);

            ViewBag.Filter = filter;
            ViewBag.NewsSearchArea = filter.SearchText;
            ViewBag.NewsSearchDateStart = filter.Date;
            ViewBag.NewsSearchDateFin = filter.DateEnd;

            ViewBag.SiteUrl = _repository.getDomainSite();

            return View(model);
        }


        public ActionResult Rss()
        {
            Response.ContentType = "text/xml";
            var filter = getFilter();
            filter.Disabled = false;
            model.List = _repository.getMaterialsList(filter);
            if (model.List != null)
            {
                ViewBag.LastDatePublish = model.List.Data[0].Date;
            }
            ViewBag.Domain = _repository.getDomainSite();

            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            return View("rss", model);
        }

    }
}

