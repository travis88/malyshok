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
                CurrentPage = currentPage,
                UserInfo = UserInfo
            };

            #region Создаем переменные (значения по умолчанию)
            ViewBag.Title = currentPage.Title;
            ViewBag.Description = currentPage.Desc;
            ViewBag.KeyWords = currentPage.Keyw;
            #endregion
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string catalog)
        {
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            var filter = getFilter();
            if (OrderId != null)
            {
                filter.Order = (Guid)OrderId;
            }
            filter.Size = PageSize;
            filter.Category = catalog;
            
            model.List = _repository.getProdList(filter);
            
            return View(_ViewName, model);
        }
    }
}

