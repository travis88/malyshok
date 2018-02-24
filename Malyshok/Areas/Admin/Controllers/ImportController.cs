using Disly.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

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
            var d = new TestDate
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
    }

    class TestDate
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
    }
}