using cms.dbase;
using Disly.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class SearchController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private ProdViewModel model;
        public int PageSize = 12;


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            
            model = new ProdViewModel
            {
                SitesInfo = siteModel,
                UserInfo = UserInfo
            };

            #region Создаем переменные (значения по умолчанию)
            //string PageTitle = currentPage.Title;
            //string PageDesc = currentPage.Desc;
            //string PageKeyw = currentPage.Keyw;
            #endregion

            #region Метатеги
            //ViewBag.Title = PageTitle;
            //ViewBag.Description = PageDesc;
            //ViewBag.KeyWords = PageKeyw;
            #endregion
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            string Count = Request.QueryString["searchtext"];
            //ViewBag.SearchText = (searchtext != null) ? searchtext.Replace("%20", " ") : String.Empty;


            var filter = getFilter();
            if (OrderId != null)
            {
                filter.Order = (Guid)OrderId;
                filter.Available = "all";
            }

            model.List = _repository.getProdList(filter);

            ViewBag.SearchText = Request.QueryString["searchtext"];

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(string searchtext)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = addFiltrParam(query, "searchtext", searchtext);
            query = addFiltrParam(query, "page", String.Empty);
            query = addFiltrParam(query, "size", PageSize.ToString());

            return Redirect(StartUrl + query);
        }
    }
}
