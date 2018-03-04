using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
                UserInfo = UserInfo,
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
        [Authorize]
        public ActionResult Index()
        {
            return View(model);
        }
        
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
        
        public ActionResult RegCompleted()
        {
            return View(model);
        }

        
        public ActionResult ConfirmMail(Guid id)
        {
            var Result = _repository.ConfirmMail(id);

            if (String.IsNullOrEmpty(Result))
            {
                return View(model);
            }
            else
            {
                FormsAuthentication.SetAuthCookie(Result.ToString(), false);

                return Redirect("/user/");
            }
        }


        /// <summary>
        /// Закрываем сеанс работы с личным кабинетом
        /// </summary>
        /// <returns></returns>
        public ActionResult logOut()
        {
            HttpCookie MyCookie = new HttpCookie("."+Settings.SiteTitle);
            MyCookie.Expires = DateTime.Now.AddDays(-1d);
            MyCookie.Value = HttpUtility.UrlEncode("", System.Text.Encoding.UTF8);
            MyCookie.Domain = "." + Settings.BaseURL;
            Response.Cookies.Add(MyCookie);
            FormsAuthentication.SignOut();

            return RedirectToAction("index", "Home");
        }

        [HttpPost]
        public ActionResult Reg(regModel backModel)
        {
            if (ModelState.IsValid)
            {
                string PrivateKey = Settings.SecretKey;
                string EncodedResponse = Request["g-Recaptcha-Response"];
                bool IsCaptchaValid = (ReCaptchaClass.Validate(PrivateKey, EncodedResponse).ToLower() == "true" ? true : false);
                bool IsEmailValid = _repository.CheckCustomerMail(backModel.Mail);


                if (IsCaptchaValid)
                {
                    if (!IsEmailValid)
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
                        User.Organization = backModel.OrgName;
                        User.Salt = NewSalt;
                        User.Hash = NewHash;
                        User.Disabled = true;

                        if (_repository.createCustomer(User))
                        {
                            #region Оповещение
                            string Massege = String.Empty;
                            Mailer Letter = new Mailer();
                            Letter.Theme = "Регистрация на сайте " + Settings.BaseURL;
                            Massege = "<p>Здравствуйте, " + User.FIO + "</p>";
                            Massege += "<p>Благодарим Вас за регистрацию на сайте " + Settings.SiteTitle + ". Для подтверждения регистрация и активации вашего аккаунта, пожалуйста, перейдите по ссылке.</p>";
                            Massege += "<p>Если ваша почтовая программа не поддерживает прямые переходы, Вы можете скопировать данную ссылку в адресную строку браузера.</p>";
                            Massege += "<p><a href=\"http://" + Settings.BaseURL + "/User/ConfirmMail/" + User.Id + "/\">http://" + Settings.BaseURL + "/User/ConfirmMail/" + User.Id + "/</a></p>";
                            Massege += "<p>Если Вы не проходили регистрацию на сайте " + Settings.BaseURL + " и получили это письмо случайно, пожалуйста, удалите его.</p>";
                            Massege += "<p>С уважением,<br />Администрация сайта " + Settings.BaseURL + "</p>";
                            Massege += "<hr><i><span style=\"font-size:12px\">Это сообщение отпралено роботом, на него не надо отвечать</i></span>";
                            Letter.MailTo = User.EMail;
                            Letter.Text = Massege;
                            string ErrorText = Letter.SendMail();
                            #endregion
                           
                            return RedirectToAction("RegCompleted", "user");
                        }
                        else
                            ModelState.AddModelError("", "Ошибка при регистрации пользователя. Обратитесь в службу тех. поддержки.");
                    }
                    else
                    {
                        //userMassege.info = "Не пройдена проверка \"Я не робот\".";
                        ModelState.AddModelError("", "пользователь с таким E-Mail уже зарегистрирован. Воспользуйтесь сервисом востановления пароля.");
                    }
                }
                else
                {
                    //userMassege.info = "Не пройдена проверка \"Я не робот\".";
                    ModelState.AddModelError("", "Не пройдена проверка \"Я не робот\".");
                }
            }
            else
            {
                ModelState.AddModelError("", "Произошла ошибка, попробуйте снова.");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult CheckMail(string id)
        {
            Response.ContentType = "application/json; charset=utf-8";

            if (!String.IsNullOrEmpty(id))
            {
                if (_repository.CheckCustomerMail(id))
                {
                    return Json(new { Result = "пользователь с таким E-Mail уже зарегистрирован" }, JsonRequestBehavior.AllowGet);
                } 
                else
                {
                    return Json(new { Result = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(new { Result = "ошибка проверки E-Mail" }, JsonRequestBehavior.AllowGet);
        }
    }
}

