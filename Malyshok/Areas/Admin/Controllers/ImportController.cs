using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using Import.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml;
using System.Xml.Serialization;

namespace Disly.Areas.Admin.Controllers
{
    public class ImportController : CoreController
    {
        /// <summary>
        /// Модель для вьюхи
        /// </summary>
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
            if (upload != null && upload.ContentLength > 0)
            {
                using (Stream stream = upload.InputStream)
                {
                    Importer.DoImport(stream);
                }
            }
            return View(model);
        }
        
        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult ImportProcessed()
        {
            var result = new
            {
                count = Importer.CountProducts,
                percent = Importer.Percent,
                step = Importer.Step,
                time = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}