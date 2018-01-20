using Disly.Areas.Admin.Models;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class MainController : CoreController
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
                ControllerName = ControllerName,
                ActionName = ActionName,

                AccountLog = _cmsRepository.getCmsUserLog(AccountInfo.id)
            };

            #region Метатеги
            ViewBag.Title = "Главная";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }
    }
}