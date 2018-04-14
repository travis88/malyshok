using cms.dbase;
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
        /// <summary>
        /// Контекст доступа к базе данных
        /// </summary>
        protected AccountRepository _accountRepository;

        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private TypePageViewModel model;

        protected int maxLoginError = 5;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            _accountRepository = new AccountRepository("cmsdbConnection");

            model = new TypePageViewModel
            {
                SitesInfo = siteModel,
                UserInfo = UserInfo
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
        /// Форма "Напомнить пароль"
        /// </summary>
        /// <returns></returns>
        public ActionResult RestorePass()
        {
            // Авторизованного пользователя направляем на главную страницу
            if (User.Identity.IsAuthenticated) return RedirectToAction("", "User");
            else return View(model);
        }

        /// <summary>
        /// Форма "Изменить пароль"
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePass(Guid id)
        {
            // Авторизованного пользователя направляем на главную страницу
            if (User.Identity.IsAuthenticated) return RedirectToAction("", "User");
            
            // Проверка кода востановления пароля
            if (!_repository.getCmsAccountCode(id))
                ViewName = "MsgFailRestore";
            else
                ViewName = "ChangePass";

            return View(ViewName, model);
        }

        /// <summary>
        /// Сообщение об отправке письма для смены пароля
        /// </summary>
        /// <returns></returns>
        public ActionResult MsgSendMail()
        {
            // Авторизованного пользователя направляем на главную страницу
            if (User.Identity.IsAuthenticated) return RedirectToAction("", "User");

            return View(model);
        }

        /// <summary>
        /// Сообщение о некоректности кода востановления пароля
        /// </summary>
        /// <returns></returns>
        public ActionResult MsgFailRestore()
        {
            // Авторизованного пользователя направляем на главную страницу
            if (User.Identity.IsAuthenticated) return RedirectToAction("", "User");

            return View(model);
        }

        /// <summary>
        /// Сообщение о смене пароля
        /// </summary>
        /// <returns></returns>
        public ActionResult MsgResult()
        {
            // Авторизованного пользователя направляем на главную страницу
            if (User.Identity.IsAuthenticated) return RedirectToAction("", "User");

            return View(model);
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
        public ActionResult Login(LogInModel backModel)
        {
            try
            {
                // Ошибки в форме
                if (!ModelState.IsValid) return View(model);

                string _login = backModel.Login;
                string _pass = backModel.Pass;
                bool _remember = backModel.RememberMe;

                UsersModel UserInfo = _repository.getCustomer(_login);

                // Если пользователь найден
                if (UserInfo != null)
                {
                    if (UserInfo.Disabled)
                    {
                        // Оповещение о блокировке
                        ModelState.AddModelError("", "Пользователь заблокирован и не имеет прав на доступ в систему администрирования");
                    }
                    else if (UserInfo.isBlocked && UserInfo.LockDate.Value.AddMinutes(15) >= DateTime.Now)
                    {
                        // Оповещение о блокировке
                        ModelState.AddModelError("", "После " + maxLoginError + " неудачных попыток авторизации Ваш пользователь временно заблокирован.");
                        ModelState.AddModelError("", "————");
                        ModelState.AddModelError("", "Вы можете повторить попытку через " + (UserInfo.LockDate.Value.AddMinutes(16) - DateTime.Now).Minutes + " минут");
                    }
                    else
                    {
                        // Проверка на совпадение пароля
                        Cripto password = new Cripto(UserInfo.Salt, UserInfo.Hash);
                        if (password.Verify(_pass.ToCharArray()))
                        {
                            // Удачная попытка, Авторизация
                            FormsAuthentication.SetAuthCookie(UserInfo.Id.ToString(), _remember);
                            
                            // Записываем данные об авторизации пользователя
                            _accountRepository.SuccessLogin(UserInfo.Id, RequestUserInfo.IP);

                            Guid UserOrder = _repository.getOrderId(UserInfo.Id);

                            if (OrderId != Guid.Empty && UserOrder != Guid.Empty)
                            {
                                return RedirectToAction("Index", "MergeOrders");
                            }
                            else if (OrderId != Guid.Empty)
                            {
                                _repository.transferOrder(OrderId, UserInfo.Id);

                                HttpCookie MyCookie = Request.Cookies["order-id"];
                                MyCookie.Expires = DateTime.Now.AddDays(-1);
                                Response.Cookies.Add(MyCookie);
                            }

                            return RedirectToAction("Index", "User");
                        }
                        else
                        {
                            // Неудачная попытка
                            // Записываем данные о попытке авторизации и плучаем кол-во неудавшихся попыток входа
                            int attemptNum = _repository.FailedLogin(UserInfo.Id, RequestUserInfo.IP);
                            if (attemptNum == maxLoginError)
                            {
                                #region Оповещение о блокировке
                                // Формируем код востановления пароля
                                Guid RestoreCode = Guid.NewGuid();
                                _repository.setRestorePassCode(UserInfo.Id, RestoreCode);

                                // оповещение на e-mail
                                string Massege = String.Empty;
                                Mailer Letter = new Mailer();
                                Letter.Theme = "Блокировка пользователя";
                                Massege = "<p>Уважаемый " + UserInfo.FIO + ", на сайте " + Request.Url.Host + " было 5 неудачных попыток ввода пароля.<br />В целях безопасности, ваш аккаунт заблокирован.</p>";
                                Massege += "<p>Для восстановления прав доступа мы сформировали для Вас ссылку, перейдя по которой, Вы сможете ввести новый пароль для вашего аккаунта и учетная запись будет разблокирована.</p>";
                                Massege += "<p>Если вы вспомнили пароль и хотите ещё раз пропробовать авторизоваться, то подождите 15 минут. Спустя это время, система позволит Вам сделать ещё попытку.</p>";
                                Massege += "<p><a href=\"http://" + Request.Url.Host + "/User/ChangePass/" + RestoreCode + "/\">http://" + Request.Url.Host + "/User/ChangePass/" + RestoreCode + "/</a></p>";
                                Massege += "<p>С уважением, администрация сайта!</p>";
                                Massege += "<hr><i><span style=\"font-size:11px\">Это сообщение отпралено роботом, на него не надо отвечать</i></span>";
                                Letter.MailTo = UserInfo.EMail;
                                Letter.Text = Massege;
                                string ErrorText = Letter.SendMail();
                                #endregion
                                ModelState.AddModelError("", "После " + maxLoginError + " неудачных попыток авторизации Ваш пользователь временно заблокирован.");
                                ModelState.AddModelError("", "Вам на почту отправлено сообщение с инструкцией по разблокировки и смене пароля.");
                                ModelState.AddModelError("", "---");
                                ModelState.AddModelError("", "Если вы хотите попробовать ещё раз, подождите 15 минут.");
                            }
                            else
                            {
                                // Оповещение об ошибке
                                string attemptCount = (maxLoginError - attemptNum == 1) ? "Осталась 1 попытка" : "Осталось " + (maxLoginError - attemptNum) + " попытки";
                                ModelState.AddModelError("", "Пара логин и пароль не подходят.");
                                ModelState.AddModelError("", attemptCount + " ввода пароля.");
                            }
                        }
                    }
                }
                else
                {
                    // Оповещение о неверном логине
                    ModelState.AddModelError("", "Такой пользователь не зарегистрирован в системе.");
                    ModelState.AddModelError("", "Проверьте правильность вводимых данных.");
                }


                return View(model);
            }
            catch
            {
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Reg(regModel backModel)
        {
            if (ModelState.IsValid)
            {
                string PrivateKey = Settings.SecretKey;
                string EncodedResponse = Request["g-Recaptcha-Response"];
                bool IsCaptchaValid = (ReCaptchaClass.Validate(PrivateKey, EncodedResponse).ToLower() == "true" ? true : false);
                bool IsEmailValid = _repository.CheckCustomerMail(backModel.Email);


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
                        User.EMail = backModel.Email;
                        User.Address = backModel.Address;
                        User.Organization = backModel.Organization;
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

        [HttpPost]
        public ActionResult RestorePass(RestoreModel model)
        {
            try
            {
                string _login = model.Email;
                UsersModel UserInfo = _repository.getCustomer(_login);

                // Ошибки в форме
                if (!ModelState.IsValid)
                {
                    // пустое поле
                    if (_login == null || _login == "")
                    {
                        ModelState.AddModelError("", "Поле \"E-Mail\" не заполнено. Для восстановления пароля введите адрес почты.");
                    }
                    return View(model);
                }

                // существует ли адрес
                if (UserInfo != null)
                {
                    // Формируем код востановления пароля
                    Guid RestoreCode = Guid.NewGuid();
                    _repository.setRestorePassCode(UserInfo.Id, RestoreCode);

                    #region оповещение на e-mail
                    string Massege = String.Empty;
                    Mailer Letter = new Mailer();
                    Letter.Theme = "Изменение пароля";
                    Massege = "<p>Уважаемый " + UserInfo.FIO + ", Вы отправили запрос на смену пароля на сайте " + Request.Url.Host + ".</p>";
                    Massege += "<p>Для вас сформирована ссылка, перейдя по которой, Вы сможете ввести новый пароль для вашего аккаунта.</p>";
                    Massege += "<p><a href=\"http://" + Request.Url.Host + "/user/ChangePass/" + RestoreCode + "/\">http://" + Request.Url.Host + "/user/ChangePass/" + RestoreCode + "/</a></p>";
                    Massege += "<p>С уважением, администрация сайта!</p>";
                    Massege += "<hr><i><span style=\"font-size:11px\">Это сообщение отпралено роботом, на него не надо отвечать</i></span>";
                    Letter.MailTo = UserInfo.EMail;
                    Letter.Text = Massege;
                    string ErrorText = Letter.SendMail();
                    #endregion

                    return RedirectToAction("MsgSendMail", "User");
                }
                else
                {
                    ModelState.AddModelError("", "Адрес почты заполнен неверно. Попробуйте ещё раз");
                }
                return View(model);

            }
            catch
            {
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult ChangePass(Guid id, PasswordModel BackModel)
        {
            if (ModelState.IsValid)
            {
                string NewPass = BackModel.Password;

                Cripto pass = new Cripto(NewPass.ToCharArray());
                string NewSalt = pass.Salt;
                string NewHash = pass.Hash;

                _repository.changePasByCode(id, NewSalt, NewHash);

                return RedirectToAction("MsgResult", "User");
            }
            else
            {
                ModelState.AddModelError("", "Ошибки в заполнении формы.");
            }

            return View(model);
        }

    }
}

