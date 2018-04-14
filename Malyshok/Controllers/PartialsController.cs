using cms.dbase;
using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using Disly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class PartialsController : Controller
    {
        /// <summary>
        /// Контекст доступа к базе данных
        /// </summary>
        protected FrontRepository _repository { get; private set; }


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            _repository = new FrontRepository("cmsdbConnection");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public ActionResult AdminMenu(string viewName = "Menu/Default")
        {
            SiteMapModel[] Menu = _repository.getMenu("main");

            return View(viewName, Menu);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public ActionResult Catalog(string viewName = "Prod/Catalog")
        {
            CategoryModel[] Model = _repository.getProdCatalogModule();

            return View(viewName, Model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public ActionResult Basket(string viewName = "Basket/Default")
        {
            Guid OrderId;
            OrderModel Model = null;

            if (User.Identity.IsAuthenticated)
            {
                OrderId = _repository.getOrderId(new Guid(User.Identity.Name));
            }
            else
            {
                HttpCookie MyCookie = Request.Cookies["order-id"];

                OrderId = (MyCookie != null) ? Guid.Parse(HttpUtility.UrlDecode(MyCookie.Value, Encoding.UTF8)) : Guid.Empty;
            }

            Model = _repository.getBasketInfo(OrderId);

            return View(viewName, Model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public ActionResult BreadCrumb(string viewName = "services/BreadCrumbs")
        {
            string path = HttpContext.Request.Url.PathAndQuery;
            if (path.IndexOf("?") > -1)
                path = path.Substring(0, path.IndexOf("?"));

            Breadcrumbs[] Model = _repository.getBreadCrumb(path);

            return View(viewName, Model);
        }
        public ActionResult BreadCrumb_Prod(string Path, string viewName = "services/BreadCrumbs_prod")
        {
            Breadcrumbs[] Model = _repository.getCatalogBreadCrumb(Path);

            return View(viewName, Model);
        }


        public ActionResult Novelties(int Count = 10, string viewName = "Prod/Novelties" )
        {

            Random rnd = new Random();
            var filter = getFilter();
            filter.Date =  DateTime.Now.AddDays(-14);
            filter.Size = 100;
            var List = _repository.getProdList(filter);

            Count = (List.Data.Length < Count) ? List.Data.Length : Count;

            ProductModel[] Model = new ProductModel[Count];
            int[] Selected = new int[Count];

            for (int i = 0; i < Count; i++) {
                int Num = new Random().Next(0, List.Data.Length - 1);
                while (Selected.Contains(Num)) {
                    Num = new Random().Next(0, List.Data.Length - 1);
                }
                Selected[i] = Num;
                Model[i] = (List.Data.Length > 0) ? List.Data[Num] : null;
            }
                        
            return View(viewName, Model);
        }


        public FilterParams getFilter(int defaultPageSize = 12)
        {
            string return_url = HttpUtility.UrlDecode(Request.Url.Query);
            // если в URL номер страницы равен значению по умолчанию - удаляем его из URL
            try
            {
                return_url = (Convert.ToInt32(Request.QueryString["page"]) == 1) ? addFiltrParam(return_url, "page", String.Empty) : return_url;
            }
            catch
            {
                return_url = addFiltrParam(return_url, "page", String.Empty);
            }
            try
            {
                return_url = (Convert.ToInt32(Request.QueryString["size"]) == defaultPageSize) ? addFiltrParam(return_url, "size", String.Empty) : return_url;
            }
            catch
            {
                return_url = addFiltrParam(return_url, "size", String.Empty);
            }
            return_url = (!Convert.ToBoolean(Request.QueryString["disabled"])) ? addFiltrParam(return_url, "disabled", String.Empty) : return_url;
            return_url = String.IsNullOrEmpty(Request.QueryString["tab"]) ? addFiltrParam(return_url, "tab", String.Empty) : return_url;
            return_url = String.IsNullOrEmpty(Request.QueryString["searchtext"]) ? addFiltrParam(return_url, "searchtext", String.Empty) : return_url;

            // Если парамметры из адресной строки равны значениям по умолчанию - удаляем их из URL
            if (return_url.ToLower() != HttpUtility.UrlDecode(Request.Url.Query).ToLower())
                Response.Redirect(Request.Path + return_url);

            DateTime? DateNull = new DateTime?();

            FilterParams result = new FilterParams()
            {
                Domain = "main",
                Page = (Convert.ToInt32(Request.QueryString["page"]) > 0) ? Convert.ToInt32(Request.QueryString["page"]) : 1,
                Size = (Convert.ToInt32(Request.QueryString["size"]) > 0) ? Convert.ToInt32(Request.QueryString["size"]) : defaultPageSize,
                Type = (String.IsNullOrEmpty(Request.QueryString["type"])) ? String.Empty : Request.QueryString["type"],
                Category = (String.IsNullOrEmpty(Request.QueryString["category"])) ? String.Empty : Request.QueryString["category"],
                Group = (String.IsNullOrEmpty(Request.QueryString["group"])) ? String.Empty : Request.QueryString["group"],
                Lang = (String.IsNullOrEmpty(Request.QueryString["lang"])) ? String.Empty : Request.QueryString["lang"],
                Date = (String.IsNullOrEmpty(Request.QueryString["date"])) ? DateNull : DateTime.Parse(Request.QueryString["date"]),
                DateEnd = (String.IsNullOrEmpty(Request.QueryString["dateend"])) ? DateNull : DateTime.Parse(Request.QueryString["dateend"]),
                SearchText = (String.IsNullOrEmpty(Request.QueryString["searchtext"])) ? String.Empty : Request.QueryString["searchtext"],
                Disabled = (String.IsNullOrEmpty(Request.QueryString["disabled"])) ? false : Convert.ToBoolean(Request.QueryString["disabled"])
            };

            return result;
        }

        public string addFiltrParam(string query, string name, string val)
        {
            //string search_Param = @"\b" + name + @"=[\w]*[\b]*&?";
            string search_Param = @"\b" + name + @"=(.*?)(&|$)";
            string normal_Query = @"&$";

            Regex delParam = new Regex(search_Param, RegexOptions.CultureInvariant);
            Regex normalQuery = new Regex(normal_Query);
            query = delParam.Replace(query, String.Empty);
            query = normalQuery.Replace(query, String.Empty);

            if (val != String.Empty)
            {
                if (query.IndexOf("?") > -1) query += "&" + name + "=" + val;
                else query += "?" + name + "=" + val;
            }

            query = query.Replace("?&", "?").Replace("&&", "&");

            return query;
        }

    }
}