using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml;
using System.Xml.Serialization;

namespace Disly.Areas.Admin.Controllers
{
    public class ImportController : CoreController
    {
        ImportViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            ViewBag.HttpKeys = Request.QueryString.AllKeys;
            ViewBag.Query = Request.QueryString;

            model = new ImportViewModel
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ControllerName = ControllerName,
                ActionName = ActionName
            };

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "Импорт продукции";
            ViewBag.KeyWords = "Импорт, интеграция";
            #endregion
        }

        // GET: Admin/Import
        public ActionResult Index()
        {
            return View(model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "xml-btn")]
        public ActionResult IndexPost(HttpPostedFileBase upload)
        {
            return View(model);
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult ImportProcessed()
        {
            var now = DateTime.Now;
            var d = new
            {
                Year = now.Year,
                Month = now.Month,
                Day = now.Day,
                Hour = now.Hour,
                Minute = now.Minute,
                Second = now.Second
            };

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "lastprod-btn")]
        public ActionResult GetLastProducts()
        {
            var filter = getFilter(200);

            var products = _cmsRepository.getProducts(filter);
            XmlSerializer xsSubmit = new XmlSerializer(typeof(ProductArray));
            byte[] xml;

            var p = new ProductArray
            {
                Products = products.Data
            };

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, p);
                    xml = Encoding.UTF8.GetBytes(sww.ToString());
                }
            }
            return File(xml, "application/xml", "last-products.xml");
        }
    }
}