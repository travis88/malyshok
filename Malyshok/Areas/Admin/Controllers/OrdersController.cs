using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace Disly.Areas.Admin.Controllers
{
    public class OrdersController : CoreController
    {
        OrderViewModel model;
        FilterParams filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            ViewBag.HttpKeys = Request.QueryString.AllKeys;
            ViewBag.Query = Request.QueryString;

            filter = getFilter(page_size);

            model = new OrderViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ControllerName = ControllerName,
                ActionName = ActionName
            };

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: Admin/Orders
        public ActionResult Index()
        {
            model.List = _cmsRepository.getOrders(filter);
            model.Statuses = _cmsRepository.getStatuses();
            ViewBag.Status = Request.Params["category"];
            return View(model);
        }

        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.getOrder(id);
            model.Statuses = _cmsRepository.getStatuses();
            ViewBag.Sum = model.Item.Details.Sum(g => g.Price * g.Count);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Item(Guid id, OrderViewModel binData)
        {
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";
            binData.Item.Id = id;
            bool result = false;

            if (ModelState.IsValid)
            {
                int status = Int32.Parse(Request.Form["Item.StatusId"]);
                binData.Item.Status = new cms.dbModel.entity.OrderStatus
                {
                    Id = status
                };
                userMessage.info = "Запись обновлена";
                result = _cmsRepository.updateOrder(binData.Item);

                if (result)
                {
                    string currentUrl = Request.Url.PathAndQuery;

                    userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                     new ErrorMassegeBtn { url = currentUrl, text = "ок" }
                 };
                }
                else
                {
                    userMessage.info = "Произошла ошибка";

                    userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false"  }
                 };
                }
            }
            else
            {
                userMessage.info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом.";

                userMessage.buttons = new ErrorMassegeBtn[]
                {
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }

            model.Item = _cmsRepository.getOrder(id);
            model.Statuses = _cmsRepository.getStatuses();
            ViewBag.Sum = model.Item.Details.Sum(g => g.Price * g.Count);

            model.ErrorInfo = userMessage;

            return View("Item", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string searchtext, string size, DateTime? date, DateTime? dateend, string status)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = addFiltrParam(query, "searchtext", searchtext);
            query = (date == null) ? addFiltrParam(query, "date", String.Empty) : addFiltrParam(query, "date", ((DateTime)date).ToString("dd.MM.yyyy").ToLower());
            query = (dateend == null) ? addFiltrParam(query, "dateend", String.Empty) : addFiltrParam(query, "dateend", ((DateTime)dateend).ToString("dd.MM.yyyy").ToString().ToLower());
            query = addFiltrParam(query, "page", String.Empty);
            query = addFiltrParam(query, "size", size);
            query = addFiltrParam(query, "category", status);

            return Redirect(StartUrl + query);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return Redirect(StartUrl + Request.Url.Query);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "xml-btn")]
        public ActionResult GetXml(Guid id)
        {
            var order = _cmsRepository.getOrder(id);
            if (order != null)
            {
                XmlSerializer xsSubmit = new XmlSerializer(typeof(OrderModel));
                byte[] xml;

                using (var sww = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(sww))
                    {
                        xsSubmit.Serialize(writer, order);
                        xml = Encoding.UTF8.GetBytes(sww.ToString());
                    }
                }
                return File(xml, "application/xml", order.Num.ToString() + ".xml");
            }

            model.Item = order;
            return View("Item", model);
        }
    }
}