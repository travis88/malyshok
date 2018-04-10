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
            Importer.IsCompleted = false;
            Importer.Percent = 0;
            Importer.Step = 0;
            Importer.CountProducts = 0;
            Importer.Log = new List<string>();

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
                time = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                isCompleted = Importer.IsCompleted,
                steps = Importer.Steps.ToArray(),
                total = Importer.Total,
                log = Importer.Log.ToArray()
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void FileUpload(IEnumerable<HttpPostedFileBase> upload)
        {
            if (upload != null)
            {
                string importDir = Server.MapPath(Settings.UserFiles + Settings.ImportDir);

                // чистим папку от предыдущих файлов
                DirectoryInfo di = new DirectoryInfo(importDir);
                if (di.Exists)
                {
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                }
                else
                {
                    di.Create();
                }

                // сохраняем файлы для импорта
                foreach (var file in upload)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string savePath = importDir + file.FileName;
                        file.SaveAs(savePath);
                    }
                }

                #region comments
                //FileInfo[] files = { di.GetFiles("*.xml")
                //                        .Where(w => w.FullName.ToLower()
                //                        .Contains("cat"))
                //                        .OrderByDescending(p => p.LastWriteTime)
                //                        .FirstOrDefault(),

                //                     di.GetFiles("*.xml")
                //                        .Where(w => w.FullName.ToLower()
                //                        .Contains("prod"))
                //                        .OrderByDescending(p => p.LastWriteTime)
                //                        .FirstOrDefault(),

                //                     di.GetFiles("*.zip")
                //                        .OrderByDescending(p => p.LastWriteTime)
                //                        .FirstOrDefault() };
                #endregion

                FileInfo[] files = di.GetFiles("*.zip")
                                     .OrderByDescending(p => p.LastWriteTime)
                                     .Take(2)
                                     .ToArray();
                
                Importer.DoImport(files);
            }
        }
    }
}