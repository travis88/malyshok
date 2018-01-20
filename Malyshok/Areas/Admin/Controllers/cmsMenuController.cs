using Disly.Areas.Admin.Models;
using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace Disly.Areas.Admin.Controllers
{
    public class cmsMenuController : CoreController
    {
        cmsMenuViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new cmsMenuViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ControllerName = ControllerName,
                ActionName = ActionName
            };

            #region Метатеги
            ViewBag.Title = "Структура CMS";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        /// <summary>
        /// Сраница по умолчанию (Список)
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            model.List = _cmsRepository.getCmsMenu(AccountInfo.id);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid Id)
        {
            model.Item = _cmsRepository.getCmsMenuItem(Id);
            model.MenuType = _cmsRepository.getCmsMenuType();

            return View("Item", model);
        }


        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "insert-btn")]
        public ActionResult Insert()
        {
            return Redirect(StartUrl + "Item/" + Guid.NewGuid() + "/" + Request.Url.Query);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid Id, cmsMenuViewModel back_model)
        {
            ErrorMessage userMassege = new ErrorMessage();
            userMassege.title = "Информация";

            if (ModelState.IsValid)
            {
                if (_cmsRepository.check_cmsMenu(Id))
                {
                    _cmsRepository.updateCmsMenu(Id, back_model.Item); //AccountInfo.id, RequestUserInfo.IP
                    userMassege.info = "Запись обновлена";
                }
                else if (!_cmsRepository.check_cmsMenu(back_model.Item.Alias))
                {
                    _cmsRepository.createCmsMenu(Id, back_model.Item); //, AccountInfo.id, RequestUserInfo.IP
                    userMassege.info = "Запись добавлена";
                }
                else
                {
                    userMassege.info = "Запись с таким псевдонимом уже существует. <br />Замените псевдоним.";
                }

                userMassege.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "вернуться в список" },
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }
            else
            {
                userMassege.info = "Ошибка в заполнении формы";
                userMassege.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }

            model.Item = _cmsRepository.getCmsMenuItem(Id);
            model.MenuType = _cmsRepository.getCmsMenuType();
            model.ErrorInfo = userMassege;

            return View("Item", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return Redirect(StartUrl + Request.Url.Query);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid Id)
        {
            _cmsRepository.deleteCmsMenu(Id);

            // записываем информацию о результатах
            ErrorMessage userMassege = new ErrorMessage();
            userMassege.title = "Информация";
            userMassege.info = "Запись Удалена";
            userMassege.buttons = new ErrorMassegeBtn[]{
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };

            model.List = _cmsRepository.getCmsMenu(AccountInfo.id);
            model.ErrorInfo = userMassege;

            return View("Index", model);
        }
    }
}