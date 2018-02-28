using cms.dbase;
using Disly.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class UserController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private TypePageViewModel model;


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new TypePageViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                Breadcrumbs= breadcrumb,
                BannerArray = bannerArray,
                ProdCatalog = category_list,
                CurrentPage = currentPage
            };
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            //string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            

            return View(model);
        }

        public ActionResult Reg()
        {
            //string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";


            return View(model);
        }
    }
}

