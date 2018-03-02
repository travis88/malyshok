using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class UserController : RootController
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
        public ActionResult Login()
        {
            //string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";


            return View(model);
        }

        public ActionResult Reg()
        {
            //string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";


            return View(model);
        }

        [HttpPost]
        public ActionResult Reg(regModel backModel)
        {
            //ErrorMassege userMassege = new ErrorMassege();
            //userMassege.title = "Информация";

            if (ModelState.IsValid)
            {
                string PrivateKey = Settings.SecretKey;
                string EncodedResponse = Request["g-Recaptcha-Response"];
                bool IsCaptchaValid = (ReCaptchaClass.Validate(PrivateKey, EncodedResponse).ToLower() == "true" ? true : false);

                if (IsCaptchaValid)
                {
                    char[] _pass = backModel.Pass.Password.ToCharArray();
                    Cripto password = new Cripto(_pass);
                    string NewSalt = password.Salt;
                    string NewHash = password.Hash;

                    UsersModel User = new UsersModel();
                    User.Id = Guid.NewGuid();
                    User.FIO = backModel.UserName;
                    User.Phone = backModel.Phone;
                    User.EMail = backModel.Mail;
                    User.Address = backModel.Adress;
                    User.Salt = NewSalt;
                    User.Hash = NewHash;
                    User.Disabled = true;

                    ModelState.AddModelError("", "Всё супер.");
                    //return RedirectToAction("SendMail", "Page");
                }
                else
                {
                    //userMassege.info = "Не пройдена проверка \"Я не робот\".";
                    ModelState.AddModelError("", "Не пройдена проверка \"Я не робот\".");
                }
            }
            else
            {
                //userMassege.info = "Произошла ошибка, попробуйте снова.";
                ModelState.AddModelError("", "Произошла ошибка, попробуйте снова.");
            }

            //model.ErrorInfo = userMassege;

            return View(model);
        }
    }
}

