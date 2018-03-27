using cms.dbase;
using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using Disly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class PartialsController : Controller
    {
        /// <summary>
        /// Контекст доступа к базе данных
        /// </summary>
        protected FrontRepository _repository { get; private set; }


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            _repository = new FrontRepository("cmsdbConnection");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public ActionResult AdminMenu(string viewName = "Menu/Default")
        {
            SiteMapModel[] Menu = _repository.getMenu("main");

            return View(viewName, Menu);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public ActionResult Catalog(string viewName = "Catalog/Default")
        {
            CategoryModel[] Model = _repository.getProdCatalogModule();

            return View(viewName, Model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public ActionResult Basket(string viewName = "Basket/Default")
        {
            Guid OrderId;
            OrderModel Model = null;

            if (User.Identity.IsAuthenticated)
            {
                OrderId = _repository.getOrderId(new Guid(User.Identity.Name));
            }
            else
            {
                HttpCookie MyCookie = Request.Cookies["order-id"];

                OrderId = (MyCookie != null) ? Guid.Parse(HttpUtility.UrlDecode(MyCookie.Value, Encoding.UTF8)) : Guid.Empty;
            }

            Model = _repository.getBasketInfo(OrderId);

            return View(viewName, Model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public ActionResult BreadCrumb(string viewName = "services/BreadCrumbs")
        {
            Breadcrumbs[] Model = _repository.getBreadCrumb(HttpContext.Request.Url.PathAndQuery);

            return View(viewName, Model);
        }
    }
}