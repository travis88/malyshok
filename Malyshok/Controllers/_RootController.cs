using cms.dbase;
using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Disly.Controllers
{
    public class RootController : Controller
    {
        /// <summary>
        /// Контекст доступа к базе данных
        /// </summary>
        protected FrontRepository _repository { get; private set; }
        
        public string Domain;
        public string ControllerName;
        public string ActionName;
        public string ViewName;
        public string StartUrl;

        public string _path;
        public string _alias;

        protected SitesModel siteModel;
        protected SiteMapModel[] siteMapArray;
        protected BannersModel[] bannerArray;
        protected List<Breadcrumbs> breadcrumb;
        protected SiteMapModel currentPage;

        protected string MedCap;
        protected string Quote;
        protected string Concept;
        protected string Coordination;

        protected bool IsSpecVersion = false;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            try {
                var domainUrl = Request.Url.Host.ToLower().Replace("www.", "");
                Domain = _repository.getSiteId(domainUrl);
            }
            catch
            {
                if (Request.Url.Host.ToLower().Replace("www.", "") != ConfigurationManager.AppSettings["BaseURL"]) filterContext.Result = Redirect("/Error/");
                else Domain = String.Empty;
            }
            //Domain = "rkod";

            #region Получаем данные из адресной строки
            //string UrlPath = Request.Path;
            //if (UrlPath.LastIndexOf("/") > 0 && UrlPath.LastIndexOf("/") == UrlPath.Length - 1)
            //    UrlPath = UrlPath.Substring(0, UrlPath.Length - 1);
            //string _path = (UrlPath.LastIndexOf("/")==0) ? UrlPath.Substring(1, UrlPath.Length-1) : UrlPath.Substring(0, UrlPath.LastIndexOf("/") + 1);
            //string _alias = UrlPath.Substring(UrlPath.LastIndexOf("/") + 1);

            //Частные случаи (model.CurrentPage = null) рассматриваем в самих контроллерах 
            _alias = "";
            _path = "/";

            var url = HttpContext.Request.Url.AbsolutePath.ToLower();

            //Сюда попадаем еще, если на странице есть картинки или файлы
            if (url.LastIndexOf(".") > -1)
                return;
            
                //Обрезаем  query string (Все, что после ? )
                if (url.LastIndexOf("?") > -1)
                    url = url.Substring(0, url.LastIndexOf("?"));
                //Сегменты пути 
                var segments = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);


                if (segments != null && segments.Count() > 0)
                {
                    _alias = segments.Last();

                    if (segments.Count() > 1)
                    {
                        _path = string.Join("/", segments.Take(segments.Length - 1));
                        _path = string.Format("/{0}/", _path);
                    }
                }
                currentPage = _repository.getSiteMap(_path, _alias);
                breadcrumb = _repository.getBreadCrumbCollection(HttpContext.Request.Url.PathAndQuery);
            

            #endregion

            ControllerName = filterContext.RouteData.Values["Controller"].ToString().ToLower();
            ActionName = filterContext.RouteData.Values["Action"].ToString().ToLower();

            ViewName = _repository.getView(ControllerName); //Domain, 
            siteModel = _repository.getSiteInfo();
            
            siteMapArray = _repository.getSiteMapList(); //Domain

            bannerArray = _repository.getBanners(); //Domain

            ViewBag.MedCap = MedCap = Settings.MedCap;
            ViewBag.Quote = Quote = Settings.Quote;
            ViewBag.Concept = Concept = Settings.Concept;
            ViewBag.Coordination = Coordination = Settings.Coordination;
            ViewBag.ControllerName = ControllerName;
        }

        public RootController()
        {
            var domainUrl = "";

            if (System.Web.HttpContext.Current != null)
            {
                var context = System.Web.HttpContext.Current;

                if (context.Request != null && context.Request.Url != null && !string.IsNullOrEmpty(context.Request.Url.Host))
                    domainUrl = context.Request.Url.Host.ToLower().Replace("www.", "");
            }
            _repository = new FrontRepository("cmsdbConnection", domainUrl);
        }
        
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class MultiButtonAttribute : ActionNameSelectorAttribute
        {
            public string MatchFormKey { get; set; }
            public string MatchFormValue { get; set; }
            public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
            {
                return controllerContext.HttpContext.Request[MatchFormKey] != null &&
                    controllerContext.HttpContext.Request[MatchFormKey] == MatchFormValue;
            }
        }
        
        public FilterParams getFilter(int defaultPageSize = 20)
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
                Response.Redirect(StartUrl + return_url);

            DateTime? DateNull = new DateTime?();

            FilterParams result = new FilterParams()
            {
                Domain = Domain,
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

