using cms.dbase;
using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class ProdController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private ProdViewModel model;
        private SelectList sortList;
        public int PageSize = Settings.ProdListSize;


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new ProdViewModel
            {
                SitesInfo = siteModel,
                UserInfo = UserInfo
            };
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string path)
        {
            #region Заголовок страницы
            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;
            }
            else
            {
                ViewBag.Title = "Каталог продукции";
                ViewBag.Description = "";
                ViewBag.KeyWords = "";
            }
            #endregion
            
            var filter = getFilter();
            if (OrderId != null)
            {
                filter.Order = (Guid)OrderId;
            }
            ViewBag.Category = filter.Category = ("/" + path + "/").Replace("//", "/");

            model.Categorys = _repository.getProdCatalog(("/" + path + "/").Replace("//", "/"));
            model.List = _repository.getProdList(filter);

            model.sortParams = _repository.getfiltrParams("sort");
            model.availableParams = _repository.getfiltrParams("available");

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(string size, string sort, string available)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = addFiltrParam(query, "page", String.Empty);
            query = addFiltrParam(query, "size", size);
            query = String.IsNullOrEmpty(sort) ? addFiltrParam(query, "sort", String.Empty) : addFiltrParam(query, "sort", sort);
            query = String.IsNullOrEmpty(available) ? addFiltrParam(query, "available", String.Empty) :addFiltrParam(query, "available", available);

            return Redirect(Request.Path + query);
        }

        public ActionResult Novelties()
        {
            int daysCount = (Convert.ToInt32(Request.QueryString["days"]) > 0) ? Convert.ToInt32(Request.QueryString["days"]) : Settings.NoveltiesDay;

            string viewName = "Novelties";
            #region Заголовок страницы
            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;
            }
            else
            {
                ViewBag.Title = "Новинки";
                ViewBag.Description = "";
                ViewBag.KeyWords = "";
            }
            #endregion

            var filter = getFilter();
            filter.Date = DateTime.Now.AddDays((-1) * daysCount);
            if (OrderId != null) filter.Order = (Guid)OrderId;
            
            model.List = _repository.getProdList(filter);

            model.sortParams = _repository.getfiltrParams("sort");
            model.availableParams = _repository.getfiltrParams("available");

            return View(viewName, model);
        }

        [HttpPost]
        public ActionResult Novelties(string size, string sort, string days, string availability)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = addFiltrParam(query, "page", String.Empty);
            query = addFiltrParam(query, "size", size);
            query = addFiltrParam(query, "days", days.ToString());

            return Redirect(Request.Path + query);
        }
        
        public ActionResult Certificates(Guid Id)
        {
            Response.ContentType = "application/json; charset=utf-8";

            if (Id != Guid.Empty)
            {
                Catalog_list[] certList = _repository.getCertificates(Id);

                for (int i = 0; i < certList.Length; i++)
                {
                    if (System.IO.File.Exists(Server.MapPath("/Certificates/" + certList[i].text + ".jpg")))
                    {
                        certList[i].selected = "true";
                    }
                    else
                    {
                        certList[i].selected = "false";
                    }
                }

                //Response.Write("stop");

                //Response.End();
                //certList = certList
                //    .Where(w => w.selected == true.ToString())
                //    .Select(s => new Catalog_list
                //    {
                //        text = s.text,
                //        value = s.value,
                //        available = s.available
                //    })
                //    .ToArray();

                return Json(new { Result = "Список сертификатов", Certificates = certList }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { Result = "Ошибка. Товар не идентифицирован." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult List(string path, string size, string page, string sort, string days, string available)
        {
            FilterParams filter = new FilterParams();
            filter.Page = (Convert.ToInt32(Request.QueryString["page"]) > 0) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            filter.Size = (Convert.ToInt32(Request.QueryString["size"]) > 0) ? Convert.ToInt32(Request.QueryString["size"]) : PageSize;
            if (OrderId != null)
            {
                filter.Order = (Guid)OrderId;
            }
            filter.Category = ("/" + path + "/").Replace("//", "/");
            filter.Sort = sort;
            filter.Available = available;
            if (!String.IsNullOrEmpty(days))
            {
                var daysCount = (Convert.ToInt32(days) > 0) ? Convert.ToInt32(days) : Settings.NoveltiesDay;
                filter.Date = DateTime.Now.AddDays((-1) * daysCount);
            }

            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = (String.IsNullOrEmpty(page) || page == 1.ToString()) ? addFiltrParam(query, "page", String.Empty) : addFiltrParam(query, "page", page); 
            query = (String.IsNullOrEmpty(size) || size == PageSize.ToString())? addFiltrParam(query, "size", String.Empty): addFiltrParam(query, "size", size);
            query = String.IsNullOrEmpty(sort) ? addFiltrParam(query, "sort", String.Empty) : addFiltrParam(query, "sort", sort);
            query = String.IsNullOrEmpty(available) ? addFiltrParam(query, "available", String.Empty) : addFiltrParam(query, "available", available);
            query = String.IsNullOrEmpty(days) ? addFiltrParam(query, "days", String.Empty) : addFiltrParam(query, "days", days);
            ViewBag.NewUrl = query;

            model.List = _repository.getProdList(filter);

            return View(model);
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid id)
        {
            model.Item = _repository.getProdItem(id, (Guid)OrderId);
            ViewBag.Category = model.Item.Catalog;

            #region Заголовок страницы
            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;
            }
            else
            {
                ViewBag.Title = model.Item.Title;
                ViewBag.Description = "";
                ViewBag.KeyWords = "";
            }
            #endregion

            return View(model);
        }
    }
}

