using cms.dbase;
using Disly.Models;
using System;
using System.Linq;
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
        public ActionResult catalog(string catalog)
        {
            #region Заголовок страницы
            //ViewBag.Title = currentPage.Title;
            //ViewBag.Description = currentPage.Desc;
            //ViewBag.KeyWords = currentPage.Keyw;
            #endregion

            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            var filter = getFilter();
            if (OrderId != null)
            {
                filter.Order = (Guid)OrderId;
            }
            filter.Size = PageSize;
            filter.Category = catalog;

            if (!string.IsNullOrEmpty(catalog))
            {
                var catalogStart = catalog.Substring(1);
                catalogStart = (catalogStart.IndexOf("/") > 0) ? "/" + catalogStart.Substring(0, catalogStart.IndexOf("/")) : "/"+ catalogStart;

                model.Categorys = _repository.getProdCatalogModule(catalogStart);
            }

            model.List = _repository.getProdList(filter);


            return View(_ViewName, model);
        }


        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(string catalog, string id)
        {
            

            return View(model);
        }
    }
}

