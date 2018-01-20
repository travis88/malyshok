using cms.dbase;
using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Disly.Areas.Admin.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        protected bool _IsAuthenticated = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        protected AccountRepository _accountRepository;
        protected cmsRepository _cmsRepository;
        protected int maxLoginError = 5;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            _accountRepository = new AccountRepository("cmsdbConnection");

            //_cmsRepository = new cmsRepository("cmsdbConnection");
            //SettingsModel Settings = _cmsRepository.getCmsSettings();

            Guid userId = Guid.Empty;
            var domainUrl = "";

            if (System.Web.HttpContext.Current != null)
            {
                var context = System.Web.HttpContext.Current;

                if (context.Request != null && context.Request.Url != null && !string.IsNullOrEmpty(context.Request.Url.Host))
                    domainUrl = context.Request.Url.Host.ToLower().Replace("www.", "");

                if (context.User != null && context.User.Identity != null && !string.IsNullOrEmpty(context.User.Identity.Name))
                {
                    try
                    {
                        userId = Guid.Parse(System.Web.HttpContext.Current.User.Identity.Name);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Не удалось определить идентификатор пользователя" + ex);
                    }
                }
            }

            _cmsRepository = new cmsRepository("cmsdbConnection", userId, RequestUserInfo.IP, domainUrl);

            #region Метатеги
            ViewBag.Title = "Авторизация";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        /// <summary>
        /// Сраница по умолчанию (Авторизация в CMS)
        /// </summary>
        /// <returns></returns>
        public ActionResult LogIn()
        {
            // Авторизованного пользователя направляем на главную страницу
            if (_IsAuthenticated) return RedirectToAction("", "Main");
            else
            {
                string UID = RequestUserInfo.CookiesValue(".ASPXAUTHMORE");
                
                if (UID != string.Empty)
                {
                    AccountModel AccountInfo = _accountRepository.getCmsAccount(new Guid(UID));
                    // Если пользователь найден
                    if (AccountInfo != null)
                    {
                        FormsAuthentication.SetAuthCookie(AccountInfo.id.ToString(), false);
                        return RedirectToAction("Index", "Main");
                    }
                }

                return View();
            }
        }
        
        [HttpPost]
        public ActionResult LogIn(LogInModel model)
        {
            try
            {                
                // Ошибки в форме
                if (!ModelState.IsValid) return View(model);

                string _login = model.Login;
                string _pass = model.Pass;
                bool _remember = model.RememberMe;

                AccountModel AccountInfo = _accountRepository.getCmsAccount(_login);

                // Если пользователь найден
                if (AccountInfo != null)
                {
                    if (AccountInfo.Disabled)
                    {
                        // Оповещение о блокировке
                        ModelState.AddModelError("", "Пользователь заблокирован и не имеет прав на доступ в систему администрирования");
                    }
                    else if (AccountInfo.CountError && AccountInfo.LockDate.Value.AddMinutes(15) >= DateTime.Now)
                    {
                        // Оповещение о блокировке
                        ModelState.AddModelError("", "После " + maxLoginError + " неудачных попыток авторизации Ваш пользователь временно заблокирован.");
                        ModelState.AddModelError("", "————");
                        ModelState.AddModelError("", "Вы можете повторить попытку через " + (AccountInfo.LockDate.Value.AddMinutes(16) - DateTime.Now).Minutes + " минут");
                    }
                    else
                    {
                        // Проверка на совпадение пароля
                        Cripto password = new Cripto(AccountInfo.Salt, AccountInfo.Hash);
                        if (password.Verify(_pass.ToCharArray()))
                        {
                            // Удачная попытка, Авторизация
                            FormsAuthentication.SetAuthCookie(AccountInfo.id.ToString(), _remember);

                            HttpCookie MyCookie = new HttpCookie(".ASPXAUTHMORE");
                            MyCookie.Value = HttpUtility.UrlEncode(AccountInfo.id.ToString(), System.Text.Encoding.UTF8);
                            MyCookie.Domain = "." + Settings.BaseURL;
                            Response.Cookies.Add(MyCookie);


                            // Записываем данные об авторизации пользователя
                            _accountRepository.SuccessLogin(AccountInfo.id, RequestUserInfo.IP);

                            return RedirectToAction("Index", "Main");
                        }
                        else
                        {
                            // Неудачная попытка
                            // Записываем данные о попытке авторизации и плучаем кол-во неудавшихся попыток входа
                            int attemptNum = _accountRepository.FailedLogin(AccountInfo.id, RequestUserInfo.IP);
                            if (attemptNum == maxLoginError)
                            {
                                #region Оповещение о блокировке
                                // Формируем код востановления пароля
                                Guid RestoreCode = Guid.NewGuid();
                                _accountRepository.setRestorePassCode(AccountInfo.id, RestoreCode, RequestUserInfo.IP);

                                // оповещение на e-mail
                                string Massege = String.Empty;
                                Mailer Letter = new Mailer();
                                Letter.Theme = "Блокировка пользователя";
                                Massege = "<p>Уважаемый " + AccountInfo.Surname + " " + AccountInfo.Name + ", в системе администрирования сайта " + Request.Url.Host + " было 5 неудачных попыток ввода пароля.<br />В целях безопасности, ваш аккаунт заблокирован.</p>";
                                Massege += "<p>Для восстановления прав доступа мы сформировали для Вас ссылку, перейдя по которой, Вы сможете ввести новый пароль для вашего аккаунта и учетная запись будет разблокирована.</p>";
                                Massege += "<p>Если вы вспомнили пароль и хотите ещё раз пропробовать авторизоваться, то подождите 15 минут. Спустя это время, система позволит Вам сделать ещё попытку.</p>";
                                Massege += "<p><a href=\"http://" + Request.Url.Host + "/Admin/Account/ChangePass/" + RestoreCode + "/\">http://" + Request.Url.Host + "/Admin/Account/ChangePass/" + RestoreCode + "/</a></p>";
                                Massege += "<p>С уважением, администрация сайта!</p>";
                                Massege += "<hr><i><span style=\"font-size:11px\">Это сообщение отпралено роботом, на него не надо отвечать</i></span>";
                                Letter.MailTo = AccountInfo.Mail;
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
                else {
                    // Оповещение о неверном логине
                    ModelState.AddModelError("", "Такой пользователь не зарегистрирован в системе.");
                    ModelState.AddModelError("", "Проверьте правильность вводимых данных.");
                }


                return View();
            }
            catch
            {
                return View();
            }
        }
        
        /// <summary>
        /// Форма "Напомнить пароль"
        /// </summary>
        /// <returns></returns>
        public ActionResult RestorePass()
        {
            // Авторизованного пользователя направляем на главную страницу
            if (_IsAuthenticated) return RedirectToAction("", "Main");
            else return View();
        }

        [HttpPost]
        public ActionResult RestorePass(RestoreModel model)
        {
            try
            {
                string _login = model.Email;
                AccountModel AccountInfo = _accountRepository.getCmsAccount(_login);

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
                if (AccountInfo != null)
                {
                    // Формируем код востановления пароля
                    Guid RestoreCode = Guid.NewGuid();
                    _accountRepository.setRestorePassCode(AccountInfo.id, RestoreCode, RequestUserInfo.IP);

                    #region оповещение на e-mail
                    string Massege = String.Empty;
                    Mailer Letter = new Mailer();
                    Letter.Theme = "Изменение пароля";
                    Massege = "<p>Уважаемый " + AccountInfo.Surname + " " + AccountInfo.Name + ", Вы отправили запрос на смену пароля на сайте " + Request.Url.Host + ".</p>";
                    Massege += "<p>Для вас сформирована ссылка, перейдя по которой, Вы сможете ввести новый пароль для вашего аккаунта.</p>";
                    Massege += "<p><a href=\"http://" + Request.Url.Host + "/Admin/Account/ChangePass/" + RestoreCode + "/\">http://" + Request.Url.Host + "/Admin/Account/ChangePass/" + RestoreCode + "/</a></p>";
                    Massege += "<p>С уважением, администрация сайта!</p>";
                    Massege += "<hr><i><span style=\"font-size:11px\">Это сообщение отпралено роботом, на него не надо отвечать</i></span>";
                    Letter.MailTo = AccountInfo.Mail;
                    Letter.Text = Massege;
                    string ErrorText = Letter.SendMail();
                    #endregion

                    return RedirectToAction("MsgSendMail", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Адрес почты заполнен неверно. Попробуйте ещё раз");
                }
                return View();

            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Форма "Изменить пароль"
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePass(Guid id)
        {
            // Авторизованного пользователя направляем на главную страницу
            if (_IsAuthenticated) 
                return RedirectToAction("", "Main");

            string ViewName = "~/Areas/Admin/Views/Account/ChangePass.cshtml";

            // Проверка кода востановления пароля
            if (!_accountRepository.getCmsAccountCode(id))
                ViewName = "~/Areas/Admin/Views/Account/MsgFailRestore.cshtml";


            return View(ViewName);
        }
        
        [HttpPost]
        public ActionResult ChangePass(Guid id,PasswordModel model)
        {
            if (ModelState.IsValid)
            {
                string NewPass = model.Password;

                Cripto pass = new Cripto(NewPass.ToCharArray());
                string NewSalt = pass.Salt;
                string NewHash = pass.Hash;

                _accountRepository.changePasByCode(id, NewSalt, NewHash, RequestUserInfo.IP);

                return RedirectToAction("MsgResult", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Ошибки в заполнении формы.");
            }

            return View();
        }
        
        /// <summary>
        /// Сообщение об отправке письма для смены пароля
        /// </summary>
        /// <returns></returns>
        public ActionResult MsgSendMail()
        {
            // Авторизованного пользователя направляем на главную страницу
            if (_IsAuthenticated) return RedirectToAction("", "Main");
            return View();
        }

        /// <summary>
        /// Сообщение о некоректности кода востановления пароля
        /// </summary>
        /// <returns></returns>
        public ActionResult MsgFailRestore()
        {
            // Авторизованного пользователя направляем на главную страницу
            if (_IsAuthenticated) return RedirectToAction("", "Main");
            return View();
        }
        
        /// <summary>
        /// Сообщение о смене пароля
        /// </summary>
        /// <returns></returns>
        public ActionResult MsgResult()
        {
            // Авторизованного пользователя направляем на главную страницу
            if (_IsAuthenticated) return RedirectToAction("", "Main");
            return View();
        }

        /// <summary>
        /// Закрываем сеанс работы с CMS
        /// </summary>
        /// <returns></returns>
        public ActionResult logOff()
        {
            AccountModel AccountInfo = _accountRepository.getCmsAccount(new Guid(User.Identity.Name));
            _accountRepository.insertLog(AccountInfo.id, RequestUserInfo.IP, "log_off", AccountInfo.id, "", "account","");

            HttpCookie MyCookie = new HttpCookie(".ASPXAUTHMORE");
            MyCookie.Expires = DateTime.Now.AddDays(-1d);
            MyCookie.Value = HttpUtility.UrlEncode("", System.Text.Encoding.UTF8);
            MyCookie.Domain = "." + Settings.BaseURL;
            Response.Cookies.Add(MyCookie);
            FormsAuthentication.SignOut();


            return RedirectToAction("LogIn", "Account");
        }
    }
}