using cms.dbModel.entity;
using Disly.Models;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Serialization;

namespace Disly.Controllers
{
    public class BasketController : RootController
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
            model.OrderInfo = _repository.getOrder(OrderId);
            model.Items = _repository.getBasketItems(OrderId);
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(OrderModel BackModel)
        {
            model.OrderInfo = _repository.getOrder(OrderId);
            
            if (!ModelState.IsValid)
            {
                model.Items = _repository.getBasketItems(OrderId);

                return View(model);
            }

            BackModel.Id = OrderId;
            int OrderNum = _repository.sendOrder(BackModel);

            string OrderDetail = String.Empty;

            #region Описываем список заказанной продукции
            model.Items = _repository.getBasketItems(OrderId);
            BackModel.Details = _repository.getOrderDetails(OrderId);

            foreach (var item in model.Items)
            {
                OrderDetail += "<div style=\"margin-bottom: 10px; overflow: auto; border-bottom: solid 1px #dadada; padding: 10px;\">";
                if (!String.IsNullOrEmpty(item.Photo))
                    OrderDetail += "<img style=\"float: left; width: 100px; margin: 0 10px 10px; border: solid 1px grey; \" src=\"https://" + Settings.BaseURL + "/" + Settings.ProdContent + item.Barcode + "/" + item.Photo.Replace(".jpg", "_mini.jpg") + "\" />";
                else
                    OrderDetail += "<img style=\"float: left; width: 100px; margin: 0 10px 10px; border: solid 1px grey; \" src=\"\" />";

                OrderDetail += "<div style=\"overflow: auto;\">";
                OrderDetail += "<a href=\"" + Settings.BaseURL + "/prod/" + item.Id + "/\"> " + item.Title + "</a>";
                OrderDetail += "<div><span>Код:</span> " + item.Code + "</div>";
                OrderDetail += "<div><span>Цена:</span> " + item.Price.ToString("# ###.00#") + "</div>";
                OrderDetail += "<div><span>Количество:</span> " + item.Count + "шт.</div>";
                OrderDetail += "</div></div>";
            }
            #endregion

            #region Создаем XML файл заказа для 1С

            OrdersXMLModel xmlData = new OrdersXMLModel();
            xmlData.Num = OrderNum;
            xmlData.Date = DateTime.Now;
            xmlData.UserName = BackModel.UserName;
            xmlData.Organization = BackModel.Organization == null ? String.Empty : BackModel.Organization;
            xmlData.Email = BackModel.Email;
            xmlData.Phone = BackModel.Phone;
            xmlData.Address = BackModel.Delivery ? String.Empty : BackModel.Address;
            xmlData.Comment = BackModel.UserComment == null ? String.Empty : BackModel.UserComment;
            xmlData.Details = model.Items
                .Select(s => new XMLOrderDetails{
                    Code = s.Code,
                    Count = s.Count
                })
                .ToArray();

            string path = Server.MapPath(Settings.ImportDir + Settings.OrdersDir);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //DataContractSerializer serializer = new DataContractSerializer(typeof(OrdersXMLModel));
            XmlSerializer writer = new XmlSerializer(typeof(OrdersXMLModel));
            using (FileStream file = new FileStream($"{path}order_{OrderNum}.xml", FileMode.Create))
            {
                writer.Serialize(file, xmlData);
            }
            #endregion

            #region Оповещение пользователя
            string Massege = String.Empty;
            Mailer Letter = new Mailer();
            Letter.Theme = "Информация по Вашему заказу №"+ OrderNum.ToString();
            Massege = "<p>Вы оформили заказ <b>№" + OrderNum.ToString() + "</b> на сумму <b>" + model.OrderInfo.Total.ToString("# ###.00#") + " руб</b>.</p>";
            Massege += "<p>В ближайшее время наши менеджеры свяжутся с Вами для окончательного оформления покупки.</p>";
            Massege += "<p>Получить дополнительную информацию по заказу Вы можете по телефонам " + model.SitesInfo.Phone.Replace("|", " ") + " или эл. почте " + model.SitesInfo.Email.Replace("|", ", ") + ". <br/>";
            Massege += "При обращении не забывайте указывать номер заказа.</p>";
            Massege += "<p><b>Детали заказа:</b></p>";
            Massege += OrderDetail;
            Massege += "<p>&nbsp;</p><p>С уважением, администрация сайта!</p>";
            Massege += "<hr><i><span style=\"font-size:11px\">Это сообщение отпралено роботом, на него не надо отвечать</i></span>";
            Letter.MailTo = BackModel.Email;
            Letter.Text = Massege;
            string ErrorText = Letter.SendMail();
            #endregion

            #region Оповещение Администратора
            Massege = String.Empty;
            Letter = new Mailer();
            Letter.Theme = "ORDER №" + OrderNum.ToString();
            Massege = "<p>На сайте оформлен заказ <b>№" + OrderNum.ToString() + "</b>.</p>";
            Massege += "<p>Сведения о заказчике:<br/>";
            Massege += "<b>Имя:</b> <i>"+ BackModel.UserName + "</i>";
            if (!BackModel.UserType) Massege += ", является представителем ЮЛ (ИП) <b><i>" + BackModel.Organization + "</i></b>";
            Massege += "<br/><b>Телефон:</b> <i>" + BackModel.Phone + "</i><br>";
            Massege += "<b>E-Mail</b> <i>" + BackModel.Email + "</i><br>";
            if (!BackModel.Delivery) Massege += "<b>Доставка по адресу:</b>  <i>" + BackModel.Address + "</i><br>";
            else Massege += "<b>Способ доставки:</b>  <i>Самовывоз.</i></p>";

            if (!String.IsNullOrEmpty(BackModel.UserComment))
            {
                Massege += "<p><b>Комментарий:</b><br /> <i>" + BackModel.UserComment + "</i></p>";
            }

            Massege += "<p>Заказ на сумму <b>" + model.OrderInfo.Total.ToString("# ###.00#") + " руб</b>.</p>";
            Massege += "<p>Детали заказа:</p>";
            Massege += OrderDetail;

            Massege += "<hr><i><span style=\"font-size:11px\">Это сообщение отпралено роботом, на него не надо отвечать</i></span>";
            Letter.MailTo = model.SitesInfo.OrderEmail;
            Letter.Text = Massege;
            ErrorText = Letter.SendMail();
            #endregion

            ViewBag.Num = OrderNum.ToString();

            return View("Result", model);
        }

        [HttpPost]
        public ActionResult Add(Guid id)
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
                
                return Json(new { Result = "Товар успешно добавлен в корзину", Count = basketInfo.ProdCount.ToString(), Cost = basketInfo.Total.ToString("# ###.00#") }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { Result = "Ошибка. Товар не идентифицирован." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            Response.ContentType = "application/json; charset=utf-8";

            if (id != Guid.Empty && OrderId != Guid.Empty)
            {
                _repository.removeFromBasket(OrderId, id);

                OrderModel basketInfo = _repository.getBasketInfo(OrderId);

                if (basketInfo != null)
                {
                    return Json(new { Result = "Товар удален", Count = basketInfo.ProdCount.ToString(), Cost = basketInfo.Total.ToString("# ###.00#") }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = "Корзина очищена", Count = 0, Cost = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(new { Result = "Ошибка. Товар не идентифицирован." }, JsonRequestBehavior.AllowGet);
        }

    }
}

