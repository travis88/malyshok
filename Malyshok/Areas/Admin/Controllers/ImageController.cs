using Disly.Areas.Admin.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class ImageController : CoreController
    {
        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            MainViewModel model = new MainViewModel()
            {
                DomainName = Domain,
                Account = AccountInfo,
                Settings = SettingsInfo,
                AccountLog = _cmsRepository.getCmsUserLog(AccountInfo.id)
            };

            #region Метатеги
            ViewBag.Title = "Редактор изображений";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }
    }
}