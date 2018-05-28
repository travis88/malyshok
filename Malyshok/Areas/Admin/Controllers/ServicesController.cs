using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using System;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class ServicesController : CoreController
    {

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();
        }

        /// <summary>
        /// Проссмотр журнала
        /// </summary>
        /// <param name="id">ID записи, для которой получаем журнал</param>
        /// <returns></returns>
        public ActionResult Log(Guid id)
        {
            cmsLogModel[] model = _cmsRepository.getCmsPageLog(id);

            return PartialView("Log", model);
        }
        
        #region Смена пароля
        /// <summary>
        /// Смена пароля
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ChangePass(Guid id, string type)
        {
            if (!String.IsNullOrWhiteSpace(type) && type == "site")
            {
                PortalUsersViewModel model = new PortalUsersViewModel()
                {
                    UserResolution = UserResolutionInfo,
                    Item = _cmsRepository.getCustomer(id)
                };
                return PartialView("ChangePass", model);
            }
            else
            {
                PortalUsersViewModel model = new PortalUsersViewModel()
                {
                    UserResolution = UserResolutionInfo,
                    Item = _cmsRepository.getUser(id)
                };
                return PartialView("ChangePass", model);
            }
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "password-update")]
        public ActionResult ChangePass(Guid id, PortalUsersViewModel model, string type)
        {
            if (ModelState.IsValid)
            {
                string NewPass = model.Password.Password;
                Cripto pass = new Cripto(NewPass.ToCharArray());
                string NewSalt = pass.Salt;
                string NewHash = pass.Hash;

                if (!String.IsNullOrWhiteSpace(type) && type == "site")
                {
                    _cmsRepository.ChangePasswordUserSite(id, NewSalt, NewHash);
                    model = new PortalUsersViewModel()
                    {
                        UserResolution = UserResolutionInfo,
                        Item = _cmsRepository.getCustomer(id)
                    };
                }
                else
                {
                    _cmsRepository.changePassword(id, NewSalt, NewHash);
                    model = new PortalUsersViewModel()
                    {
                        UserResolution = UserResolutionInfo,
                        Item = _cmsRepository.getUser(id)
                
                    };
                }
                ViewBag.SuccesAlert = "Пароль изменен";
            }


            return PartialView("ChangePass", model);
        }
        #endregion

        #region Настройки доступа пользователя
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public ActionResult UserResolutions(Guid id)
        //{
        //    UsersViewModel model = new UsersViewModel()
        //    {
        //        User = _repository.getUser(id),
        //        ResolutionsList = _repository.getResolutionsPerson(id)
        //    };
        //    return PartialView("UserResolutions", model);
        //}

        //[HttpPost]
        //public ActionResult UserResolutions(Guid id, Guid url, string action, int val)
        //{   
        //    bool Result = false;

        //    _repository.appointResolutionsUser(id, url, action, val);

        //    //UsersViewModel model = new UsersViewModel()
        //    //{
        //    //    User = _repository.getUser(id),
        //    //    ResolutionsList = _repository.getResolutionsPerson(id)
        //    //};
        //    //return PartialView("UserResolutions", model);
        //    return Content(Result.ToString());
        //}
        #endregion

        #region Список доступных сайтов
        public ActionResult UserSiteListModal(Guid id)
        {
            PortalUsersViewModel userInfo = new PortalUsersViewModel()
            {
                UserResolution = UserResolutionInfo,
                Item = _cmsRepository.getUser(id)
            };

            var filtr = getFilter();
            var sfilter = FilterParams.Extend<SiteFilter>(filtr);
            sfilter.UserId = id;
            var sitesList = _cmsRepository.getSiteListWithCheckedForUser(sfilter);
            
            var model = new UserSiteLinkModel()
            {
                UserId = id,
                Sites = sitesList,
            };

            return PartialView("UserSites", model);
        }

        [HttpPost]
        public ActionResult UpdateUserLinkToSite(ContentLinkModel linkData)
        {
            if(linkData != null)
            {
                var res = _cmsRepository.updateUserSiteLinks(linkData);
                if (res)
                    return Json("Success");

            }
            //return Response.Status = "OK";
            return Json("An Error Has occourred"); //Ne
        }
        #endregion

        #region Группы пользователей
        [HttpGet]
        public ActionResult GroupClaims(string id)
        {
            GroupModel model = _cmsRepository.getGroup(id);
            ViewBag.Group = id;
            return PartialView("GroupClaims", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "create-group-btn")]
        public ActionResult GroupClaims(GroupModel bindData)
        {
            if (bindData != null)
            {
                if (ModelState.IsValid)
                {
                    var res = _cmsRepository.updateGroup(bindData);
                    if (res)
                        return PartialView("Modal/Success");
                }
            }

            return PartialView("Modal/Error");
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-group-btn")]
        public ActionResult SaveGroup(GroupModel bindData)
        {
            if (bindData != null)
            {
                var res = _cmsRepository.updateGroup(bindData);
                if (res)
                    return PartialView("Modal/Success");
            }

            return PartialView("Modal/Error");
        }

        [HttpPost]
        public ActionResult UpdateGroupClaims(GroupClaims data)
        {
            //var model = Newtonsoft.Json.JsonConvert.DeserializeObject<GroupClaims>(data);

            var res = _cmsRepository.updateGroupClaims(data);
            if (res)
                return Json("Success");
            //return Response.Status = "OK";  //AsJson(new { status = true, reason = "OK", data = "" });
            return Json("An Error Has occourred"); //Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-group-btn")]
        public ActionResult DeleteGroup(string group)
        {
            var res = _cmsRepository.deleteGroup(group);
            if (res)
                return PartialView("Modal/Success");

            return PartialView("Modal/Error");
        }

       

        //public ActionResult GroupCreate()
        //{
        //    UsersViewModel model = new UsersViewModel();
        //    return PartialView("Group", model);
        //}

        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "create-btn-group-user")]
        //public ActionResult GroupCreate(UsersViewModel model)
        //{
        //    Guid PageId = Guid.NewGuid();
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _repository.createUsersGroup(PageId, model.GroupItem.C_Alias, model.GroupItem.C_GroupName);
        //            _repository.insertLog(PageId, AccountInfo.id, "insert", RequestUserInfo.IP);
        //            ViewBag.SuccesAlert = "Запись добавлена";
        //            ViewBag.ActionName = "groupedit";
        //        }
        //        catch
        //        {
        //            ViewBag.DankerAlert = "Группа с таким названием уже существует";
        //            UsersViewModel newmodel = new UsersViewModel();
        //            return PartialView("Group", newmodel);
        //        }

        //    }
        //    model = new UsersViewModel()
        //    {
        //        GroupItem = _repository.getUserGroup(model.GroupItem.C_Alias),
        //        ResolutionsTemplatesList = _repository.getResolutions(model.GroupItem.C_Alias)
        //    };
        //    return PartialView("Group", model);
        //}


        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "update-btn-group-user")]
        //public ActionResult GroupEdit(UsersViewModel model)
        //{
        //    Guid PageId;
        //    UsersViewModel model_dop = new UsersViewModel()
        //    {
        //        GroupItem = _repository.getUserGroup(model.GroupItem.C_Alias),
        //        ResolutionsTemplatesList = _repository.getResolutions(model.GroupItem.C_Alias)
        //    };
        //    PageId = model_dop.GroupItem.id;
        //    if (ModelState.IsValid)
        //    {
        //        _repository.updateUsersGroup(model.GroupItem.C_Alias, model.GroupItem.C_GroupName);
        //        _repository.insertLog(PageId, AccountInfo.id, "update", RequestUserInfo.IP);
        //        ViewBag.SuccesAlert = "Запись обновлена";
        //    }
        //    return PartialView("Group", model_dop);
        //}


        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn-group-user")]
        //public ActionResult GroupDelete()
        //{
        //    string Alias = Request.Params["alias"];
        //    Guid PageId;
        //    UsersViewModel model_dop = new UsersViewModel()
        //    {
        //        GroupItem = _repository.getUserGroup(Alias)
        //    };
        //    _repository.deleteUsersGroup(Alias);//удаление группы
        //    _repository.delResolutionsTemplates(Alias);//удаление прав            

        //    PageId = model_dop.GroupItem.id;
        //    _repository.insertLog(PageId, AccountInfo.id, "delete", RequestUserInfo.IP);
        //    return RedirectToAction("Index");

        //}

        //[HttpPost]
        //public ActionResult GroupResolut(string user, Guid url, string action, int val)
        //{
        //    string Alias = Request.Params["alias"];
        //    _repository.appointResolutionsTemplates(user, url, action, val);
        //    UsersViewModel model = new UsersViewModel()
        //    {
        //        ResolutionsTemplatesList = _repository.getResolutions(Alias)
        //    };
        //    return PartialView("Group", model);
        //}
        #endregion

        #region Карта сайта
        [HttpGet]
        public ActionResult SitemapMenu(Guid id)
        {
            SiteMapMenu model = new SiteMapMenu();
            model = _cmsRepository.getSiteMapMenu(id);

            return PartialView("SiteMapMenuGroup", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-menugroup-btn")]
        public ActionResult SaveSitemapMenu(SiteMapMenu model)
        {
            if (ModelState.IsValid)
            {
               var res = _cmsRepository.createOrUpdateSiteMapMenu(model);
               if (res)
                    return PartialView("Modal/Success");
            }
           return PartialView("Modal/Error");
        }
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-menugroup-btn")]
        public ActionResult DeleteSitemapMenu(Guid id)
        {
            if (ModelState.IsValid)
            {
                var res = _cmsRepository.deleteSiteMapMenu(id);
                if (res)
                    return PartialView("Modal/Success");
            }
            return PartialView("Modal/Error");
        }
        #endregion

        /// <summary>
        /// Изменение позиции в списке
        /// </summary>
        /// <param name="group">Раздел, в котором производятся изменения</param>
        /// <param name="id">ID записи, у которой меняется позиция</param>
        /// <param name="permit">Номер новой позиции</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangePermit(string group, string menuSort, Guid id, int permit)
        {
            bool Result = false;

            switch (group.ToLower())
            {
                case "cmsmenu":
                    Result = _cmsRepository.permit_cmsMenu(id, permit);
                    break;
                case "sitemap":
                    Result = _cmsRepository.permit_SiteMap(id, permit, menuSort);
                    break;
                case "category":
                    Result = _cmsRepository.permit_Category(id, permit);
                    break;
                case "documents":
                    Result = _cmsRepository.permit_Documents(id, permit);
                    break;
                case "photos":
                    Result = _cmsRepository.sortingPhotos(id, permit);
                    break;
            }
            return Content(Result.ToString());
        }
        
        [HttpPost]
        public ActionResult GetFile(string dir)
        {
            Response.ContentType = "application/json";
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (String.IsNullOrEmpty(dir))
                {
                    dir = Settings.UserFiles + Domain + "/temp/";
                }
                if (!Directory.Exists(dir)) { Directory.CreateDirectory(Server.MapPath(dir)); }
                string savePath = dir + file.FileName;
                file.SaveAs(Server.MapPath(savePath));
                //using (Image image = Image.FromStream(file.InputStream))
                //{
                //    using (MemoryStream m = new MemoryStream())
                //    {
                //        image.Save(m, image.RawFormat);
                //        byte[] imageBytes = m.ToArray();

                //        // Convert byte[] to Base64 String
                //        string base64String = "data:image/png;base64," + Convert.ToBase64String(imageBytes);
                //        return Content("{ \"location\": \"" + base64String + "\" }");
                //    }
                //}
                return Content("{ \"location\": \"" + savePath + "\" }");
            }

            return Content("");
        }
    }
}