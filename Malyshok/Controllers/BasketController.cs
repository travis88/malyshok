using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Disly.Controllers
{
    public class BasketController : RootController
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
                UserInfo = UserInfo,
                Breadcrumbs = breadcrumb,
                BannerArray = bannerArray,
                ProdCatalog = category_list,
                CurrentPage = currentPage
            };
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(Guid id)
        {
            Response.ContentType = "application/json; charset=utf-8";
            int Count = Convert.ToInt32(Request.QueryString["count"]);

            if (id != Guid.Empty && Count > 0)
            {
                if (OrderId == null)
                {
                    OrderId = Guid.NewGuid();

                    HttpCookie MyCookie = new HttpCookie("order-id");
                    MyCookie.Value = HttpUtility.UrlEncode(OrderId.ToString(), Encoding.UTF8);
                    Response.Cookies.Add(MyCookie);
                }
                else
                {

                }

                return Json(new { Result = "Добавлено в корзину " + Count.ToString() + " шт." }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { Result = "Ошибка. Товар не идентифицирован." }, JsonRequestBehavior.AllowGet);
        }
    }
}

