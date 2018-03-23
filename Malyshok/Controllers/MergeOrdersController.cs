using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Disly.Controllers
{
    public class MergeOrdersController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private BasketViewModel model;


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new BasketViewModel
            {
                SitesInfo = siteModel,
                UserInfo = UserInfo
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
        public ActionResult Merge(Guid id)
        {
            Response.ContentType = "application/json; charset=utf-8";
            int Count = Convert.ToInt32(Request.QueryString["count"]);

            if (id != Guid.Empty && Count > 0)
            {
                if (OrderId == Guid.Empty)
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        OrderId = _repository.CreateOrder(Guid.Parse(User.Identity.Name));
                    }
                    else
                    {
                        OrderId = _repository.CreateOrder();

                        HttpCookie MyCookie = new HttpCookie("order-id");
                        MyCookie.Value = HttpUtility.UrlEncode(OrderId.ToString(), Encoding.UTF8);
                        Response.Cookies.Add(MyCookie);
                    }
                }

                _repository.addInBasket(OrderId, id, (int)Count);

                OrderModel basketInfo = _repository.getBasketInfo(OrderId);

                string ProdCount = "<span>" + basketInfo.ProdCount.ToString() + "</span> ";
                string LL = basketInfo.ProdCount.ToString().Substring(basketInfo.ProdCount.ToString().Length - 1);
                if (LL == "1" && basketInfo.ProdCount != 11) ProdCount += "товар";
                else if ((LL == "2" || LL == "3" || LL == "4") && basketInfo.ProdCount != 12 && basketInfo.ProdCount != 13 && basketInfo.ProdCount != 14) ProdCount += "товара";
                else ProdCount += "товаров";

                return Json(new { Result = "Товар успешно добавлен в корзину", Count = ProdCount, Cost = "<span>" + basketInfo.Total.ToString("# ###.00#") + "</span> руб." }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { Result = "Ошибка. Товар не идентифицирован." }, JsonRequestBehavior.AllowGet);
        }
    }
}

