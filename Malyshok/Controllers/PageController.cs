using cms.dbase;
using Disly.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class PageController : RootController
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
                UserInfo = UserInfo
            };
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            model.Item = _repository.getSiteMap(_path,_alias); //,Domain
            if (model.Item != null)
            {
                if (model.Item.FrontSection.ToLower() != "page")
                {
                    return Redirect("/" + model.Item.FrontSection);
                }
                model.Child = _repository.getSiteMapChild(model.Item.Id);
                model.Documents = _repository.getAttachDocuments(model.Item.Id);
            }
            else
            {
                return new HttpNotFoundResult();
            }

            return View(_ViewName,model);
        }
    }
}

