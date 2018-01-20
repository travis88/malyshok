using System.Web.Mvc;
using Disly.Areas.Admin.Models;
using System;
using System.Web;
using cms.dbModel.entity;
using System.IO;
using System.Linq;

namespace Disly.Areas.Admin.Controllers
{
    public class BannersController : CoreController
    {
        // Модель для вывода в представления
        private BannersViewModel model;

        // Фильтр
        private FilterParams filter;

        // Кол-во эл-тов на странице 100
        int pageSize = 100;

        /// <summary>
        /// Обрабатывается до вызова экшенов
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new BannersViewModel
            {
                DomainName = Domain,
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ControllerName = ControllerName,
                ActionName = ActionName
            };

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.Keywords = "";
            #endregion
        }

        /// <summary>
        /// Список секций для баннеров
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(Guid? id)
        {
            if (!id.HasValue)
            {
                // Наполняем модель списком секций
                var sections = _cmsRepository.getSections();
                if (sections != null)
                {
                    foreach (var section in sections)
                    {
                        section.CountBanners = _cmsRepository.getCountBanners(section.Id);
                    }
                }
                model.Sections = sections;
            }
            else
            {
                // наполняем фильтр
                filter = getFilter(pageSize);

                var sectionItem = _cmsRepository.getSectionItem(id.Value);
                if (sectionItem != null)
                {
                    // наполняем модель списка баннеров
                    var bannersList = _cmsRepository.getBanners(id.Value, filter);
                    sectionItem.BannerList = bannersList;
                    sectionItem.CountBanners = (bannersList != null) ? bannersList.Pager.items_count : 0;
                }
                model.SectionItem = sectionItem;
            }

            return View(model);
        }

        /// <summary>
        /// Конкретный баннер
        /// </summary>
        /// <param name="id">Id-баннера</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Item(Guid id)
        {
            var banner = _cmsRepository.getBannerItem(id);
            ViewBag.DataPath = Settings.UserFiles + Domain + Settings.BannersDir+id.ToString()+"/";

            if (banner == null)
            {
                var section = Guid.NewGuid();

                if (string.IsNullOrEmpty(Request.Params["section"]) || !Guid.TryParse(Request.Params["section"], out section))
                    throw new Exception("BannerController: Невозможно создать баннер, секция не определена");

                var getSection = _cmsRepository.getSections();
                if (getSection == null || (getSection != null && !getSection.Any(p => p.Id == section)))
                    throw new Exception("BannerController: Невозможно создать баннер, секция не определена");

                model.Item = new BannersModel()
                {
                    Id = id,
                    Site = Domain,
                    Date = DateTime.Now,
                };
                ViewBag.Section = section.ToString();
            }
            else
                model.Item = banner;

            // файл изображения
            if (model.Item != null)
            {
                if (model.Item.Photo != null && !string.IsNullOrEmpty(model.Item.Photo.Url))
                {
                    model.Item.Photo = Files.getInfoImage(model.Item.Photo.Url);
                }

                //Заполняем для модели связи с другими объектами
                filter = getFilter();
                var sitefilter = FilterParams.Extend<SiteFilter>(filter);
                sitefilter.RelId = id;
                sitefilter.RelType = ContentType.BANNER;
                var sitessList = _cmsRepository.getSiteList(sitefilter);

                model.Item.Links = new ObjectLinks()
                {
                    Sites = (sitessList != null) ? sitessList.Data : null
                };
            }

            return View(model);
        }

        /// <summary>
        /// Создание или редактирование баннера
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Item(Guid id, BannersViewModel back_model, HttpPostedFileBase upload)
        {
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";

            #region Данные, необходимые для сохранения
            back_model.Item.Section = back_model.Item.Section != null ? back_model.Item.Section : Guid.Parse(Request.Form["Item_Section"]);
            back_model.Item.Site = Domain;
            #endregion

            if (ModelState.IsValid)
            {
                back_model.Item.Id = id;
                #region Сохранение изображения
                // путь для сохранения изображения
                string savePath = Settings.UserFiles + Domain + Settings.BannersDir;

                // секция
                if (!back_model.Item.Section.HasValue)
                    throw new Exception("BannerController: В back_model не определена секция");

                var _section = _cmsRepository.getSectionItem(back_model.Item.Section.Value);
                int width = _section.Width; // ширина
                int height = _section.Height; // высота

                if (upload != null && upload.ContentLength > 0)
                {
                    string fileExtension = upload.FileName.Substring(upload.FileName.LastIndexOf(".")).ToLower();

                    var validExtension = (!string.IsNullOrEmpty(Settings.PicTypes)) ? Settings.PicTypes.Split(',') : "jpg,jpeg,png,gif".Split(',');
                    if (!validExtension.Contains(fileExtension.Replace(".", "")))
                    {
                        model.Item = _cmsRepository.getBannerItem(id);
                        model.ErrorInfo = new ErrorMessage()
                        {
                            title = "Ошибка",
                            info = "Вы не можете загружать файлы данного формата",
                            buttons = new ErrorMassegeBtn[]
                            {
                                //Без перезагрузки, просто отменяем
                                new ErrorMassegeBtn { url = "#", text = "ок", action = "false", style="primary" }
                            }
                        };

                        return View("Item", model);
                    }

                    Photo photoNew = new Photo()
                    {
                        Name = id.ToString() + fileExtension,
                        Size = Files.FileAnliz.SizeFromUpload(upload),
                        Url = Files.SaveImageResizeRename(upload, savePath, id.ToString(), width, height)
                    };

                    back_model.Item.Photo = photoNew;
                }
                #endregion
                var res = false;
                if (!_cmsRepository.checkBannerExist(id))
                {
                    userMessage.info = "Запись добавлена";
                    res = _cmsRepository.createBanner(id, back_model.Item);
                }
                else
                {
                    userMessage.info = "Запись обновлена";
                    res = _cmsRepository.updateBanner(id, back_model.Item);
                }

                string backUrl = back_model.Item.Section != null ? "index/" + back_model.Item.Section : string.Empty;
                if (res)
                {
                    string currentUrl = Request.Url.PathAndQuery;
                    userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = StartUrl + backUrl, text = "Вернуться в список" },
                     new ErrorMassegeBtn { url = currentUrl, text = "ок"}
                    };

                }
                else
                {
                    userMessage.info = "Произошла ошибка";
                    userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = StartUrl + backUrl, text = "Вернуться в список" },
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false"}
                    };
                }

            }
            else
            {
                userMessage.info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом.";
                userMessage.buttons = new ErrorMassegeBtn[]{
                    //Без перезагрузки action = "false" 
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }

            model.Item = _cmsRepository.getBannerItem(id);
            model.ErrorInfo = userMessage;

            if (model.Item != null && model.Item.Photo != null && !string.IsNullOrEmpty(model.Item.Photo.Url))
            {
                model.Item.Photo = Files.getInfoImage(model.Item.Photo.Url);
            }


            return View(model);
        }

        /// <summary>
        /// Событие по кнопке "Отмена"
        /// </summary>
        /// <param name="id">Id-баннера</param>
        /// <param name="section">Id-секции</param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel(Guid id, Guid? section)
        {
            model.Item = _cmsRepository.getBannerItem(id);
            var _section = model.Item != null ? model.Item.Section : Guid.Parse(Request.Form["Item_Section"]);
            //var _section = (section.Equals(null) && model.Item != null) ? model.Item.Section : section;

            if (_section != null)
            {
                return RedirectToAction("Index", new { id = _section });
            }
            return Redirect(StartUrl + Request.Url.Query);
        }

        /// <summary>
        /// Событие по кнопке "Удалить"
        /// </summary>
        /// <param name="id">id-баннера</param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            // записываем информацию о результатах
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";
            //В случае ошибки
            userMessage.info = "Ошибка, Запись не удалена";
            userMessage.buttons = new ErrorMassegeBtn[]{
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };


            var data = model.Item = _cmsRepository.getBannerItem(id);

            // удаляем файл изображения и запись
            if (model.Item != null)
            {
                var image = (data.Photo != null) ? data.Photo.Url : null;
                var res = _cmsRepository.deleteBanner(id);
                if (res)
                {
                    if (!string.IsNullOrEmpty(image))
                        Files.deleteImage(image);

                    // записываем информацию о результатах
                    userMessage.title = "Информация";
                    userMessage.info = "Запись удалена";

                    var section = (model.Item.Section.HasValue) ? "/admin/banners/index/" + model.Item.Section.Value.ToString() : "/admin/banners/";
                    userMessage.buttons = new ErrorMassegeBtn[]
                    {
                        new ErrorMassegeBtn { url = section, text = "Перейти в список" }
                    };
                    //return RedirectToAction("Index", new { id = model.Item.Section });
                }
            }

            model.ErrorInfo = userMessage;

            return View(model);
        }
    }
}
