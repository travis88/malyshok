using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using Disly.Areas.Admin.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class SiteSettingsController : CoreController
    {
        // модель для вывода в представлении
        private SitesViewModel model;


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new SitesViewModel
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ControllerName = ControllerName,
                ActionName = ActionName
            };

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: Admin/SiteSettings
        [HttpGet]
        public ActionResult Index()
        {
            model.Item = _cmsRepository.getSite();
            ViewBag.Xcoord = model.Item.CoordX;
            ViewBag.Ycoord = model.Item.CoordY;
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Index(SitesViewModel backModel, HttpPostedFileBase upload, HttpPostedFileBase uploadBack)
        {
            ErrorMessage userMassege = new ErrorMessage();
            userMassege.title = "Информация";
            
            if (ModelState.IsValid)
            {
                double MapX = 0;
                double MapY = 0;

                if (backModel.Item.CoordX != null) { MapX = (double)backModel.Item.CoordX; }
                if (backModel.Item.CoordY != null) { MapY = (double)backModel.Item.CoordY; }
                ViewBag.Xcoord = MapX;
                ViewBag.Ycoord = MapY;

                if (backModel.Item.Adress != String.Empty && (MapX == 0 || MapY == 0))
                {
                    string url = "http://geocode-maps.yandex.ru/1.x/?format=json&results=1&geocode=" + backModel.Item.Adress;
                    string html = string.Empty;
                    // Отправляем GET запрос и получаем в ответ JSON с данным об адресе
                    HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                    HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                    StreamReader myStreamReader = new StreamReader(myHttpWebResponse.GetResponseStream());
                    html = myStreamReader.ReadToEnd();

                    string coord = String.Empty;
                    Regex ReCoord = new Regex("(?<=\"Point\":{\"pos\":\")(.*)(?=\"})", RegexOptions.IgnoreCase);

                    coord = Convert.ToString(ReCoord.Match(html).Groups[1].Value);

                    coord = coord.Replace(" ", ";");
                    string[] ArrCoord = coord.Split(';');
                    foreach (string qwerty in ArrCoord)
                    {
                        MapX = double.Parse(ArrCoord[1].Replace(".", ","));
                        MapY = double.Parse(ArrCoord[0].Replace(".", ","));
                    }
                    backModel.Item.CoordX = MapX;
                    backModel.Item.CoordY = MapY;
                }

                #region Сохранение изображений

                if (upload != null && upload.ContentLength > 0)
                {

                    var photo = imageWorker(upload, 1);
                    if (photo == null)
                        return View("Item", model);

                    backModel.Item.Logo = photo;
                }

                #region Изображени под слайдером
                if (uploadBack != null && uploadBack.ContentLength > 0)
                {
                    string SavePath = Settings.UserFiles + Domain + "/logo/";
                    int idx = uploadBack.FileName.LastIndexOf('.');
                    string Title = uploadBack.FileName.Substring(0, idx);
                    string TransTitle = Transliteration.Translit(Title);
                    string FileName = TransTitle + Path.GetExtension(uploadBack.FileName);


                    string FullName = SavePath + FileName;
                    if (!Directory.Exists(Server.MapPath(SavePath)))
                    {
                        Directory.CreateDirectory(Server.MapPath(SavePath));
                    }
                    uploadBack.SaveAs(Server.MapPath(Path.Combine(SavePath, FileName)));
                    backModel.Item.BackGroundImg = new Photo { Url = FullName };
                }

                #endregion


                #endregion

                _cmsRepository.updateSiteInfo(backModel.Item);
                userMassege.info = "Запись обновлена";
                userMassege.buttons = new ErrorMassegeBtn[]
            {
                    new ErrorMassegeBtn { url = "/Admin/sitesettings", text = "ок"}
            };
            }
            else
            {

                userMassege.info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом.";

                userMassege.buttons = new ErrorMassegeBtn[]
                {
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }
            model.ErrorInfo = userMassege;
            model.Item = _cmsRepository.getSite();
            return View(model);
        }

        /// <summary>
        /// Сохранение изображений
        /// </summary>
        /// <param name="upload"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Photo imageWorker(HttpPostedFileBase upload, int type)
        {
            // путь для сохранения изображения
            string savePath = Settings.UserFiles + Domain + Settings.LogoDir;

            int width = 0; // ширина
            int height = 0; // высота

            switch (type)
            {
                case 1:
                    width = 80;
                    break;
                case 2:
                    height = 290;
                    break;
            }

            string fileExtension = upload.FileName.Substring(upload.FileName.LastIndexOf(".")).ToLower();

            var validExtension = (!string.IsNullOrEmpty(Settings.PicTypes)) ? Settings.PicTypes.Split(',') : "jpg,jpeg,png,gif".Split(',');
            if (!validExtension.Contains(fileExtension.Replace(".", "")))
            {
                return null;
            }

            string fileName = Transliteration.Translit(upload.FileName.Substring(0, upload.FileName.LastIndexOf(".")));

            Photo photoNew = new Photo()
            {
                Name = fileName + fileExtension,
                Size = Files.FileAnliz.SizeFromUpload(upload),
                Url = Files.SaveImageResizeRename(upload, savePath, fileName, width, height)
            };

            return photoNew;
        }
    }
}