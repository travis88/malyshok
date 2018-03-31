using cms.dbase;
using Disly.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class ProdController : RootController
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
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string path)
        {
            #region Заголовок страницы
            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;
            }
            else
            {
                ViewBag.Title = "Каталог продукции";
                ViewBag.Description = "";
                ViewBag.KeyWords = "";
            }
            #endregion
            
            var filter = getFilter();
            if (OrderId != null)
            {
                filter.Order = (Guid)OrderId;
            }
            filter.Category = ("/" + path + "/").Replace("//", "/");

            if (!string.IsNullOrEmpty(path))
            {
                model.Categorys = _repository.getProdCatalog("/" + path.Split('/').First() + "/");
            }

            model.List = _repository.getProdList(filter);

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(string size, string sort, string availability)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = addFiltrParam(query, "page", String.Empty);
            query = addFiltrParam(query, "size", size);

            return Redirect(Request.Path + query);
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(string id)
        {
            return View(model);
        }
    }
}

