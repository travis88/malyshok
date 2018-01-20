using cms.dbase;
using cms.dbModel.entity;
using System;
using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;
using Portal.Code;
using System.Drawing.Imaging;

namespace Disly.Areas.Admin.Controllers
{
    [Authorize]
    public class CoreController : Controller
    {
        /// <summary>
        /// Контекст доступа к базе данных
        /// </summary>
        protected AccountRepository _accountRepository { get; private set; }
        protected cmsRepository _cmsRepository { get; private set; }
              
        public string Domain;
        public string StartUrl;
        public AccountModel AccountInfo;
        public SettingsModel SettingsInfo;
        public ResolutionsModel UserResolutionInfo;//права пользователей
        public cmsLogModel[] LogInfo;
        public SitesModel SiteInfo;
        public string ControllerName;
        public string ActionName;

        public int page_size = 40;
        public int last_items = 10;

        protected Guid? orgId;
        protected Guid? mainSpecialist;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            cmsRepository.DislyEvent += CmsRepository_DislyEvent;

            base.OnActionExecuting(filterContext);
            
            ControllerName = filterContext.RouteData.Values["Controller"].ToString().ToLower();
            ActionName = filterContext.RouteData.Values["Action"].ToString().ToLower();
            Guid _PageId;

            try {
                Domain = _cmsRepository.getSiteId(Request.Url.Host.ToLower().Replace("www.", ""));
            }
            catch(Exception ex)
            {
                if (Request.Url.Host.ToLower().Replace("www.", "") != ConfigurationManager.AppSettings["BaseURL"])
                    filterContext.Result = Redirect("/Error/");
                else Domain = String.Empty;

                AppLogger.Debug("CoreController: Не получилось определить Domain", ex);
            }
            ViewBag.Domain = Domain;            

            StartUrl = "/Admin/" + (String)RouteData.Values["controller"] + "/";

            #region Настройки сайта            
                
            //SettingsInfo = _cmsRepository.getCmsSettings();
            // Сайт, на котором находимся
            //if (Domain != String.Empty) SettingsInfo.ThisSite = _cmsRepository.getSite(Domain);
            #endregion
            
            #region Данные об авторизованном пользователе
            Guid _userId = new Guid();
            try { _userId = new Guid(System.Web.HttpContext.Current.User.Identity.Name); }
            catch { FormsAuthentication.SignOut(); }
            AccountInfo = _accountRepository.getCmsAccount(_userId);

            // Список доменов, доступных пользователю
            AccountInfo.Domains = _accountRepository.getUserDomains(_userId);

            #endregion
            
            #region Права пользователя
            UserResolutionInfo = _accountRepository.getCmsUserResolutioInfo(_userId, ControllerName);
            // Если нет прав на проссмотр, то направляем на главную
            try { if (!UserResolutionInfo.Read) filterContext.Result = Redirect("/Admin/"); }
            catch { }
            #endregion
            
            #region Ограничение пользователя (не администратора портала и не разработчика) на доступ только к своим сайтам (доменам)
            int IsRedirect = 0;

            SiteInfo = _cmsRepository.getSite(Domain); // информация по сайту

            if (AccountInfo.Group.ToLower() != "developer" && AccountInfo.Group.ToLower() != "administrator")
            {
                if (AccountInfo.Domains != null)
                {
                    foreach (DomainList domain in AccountInfo.Domains)
                    {
                        if (domain.SiteId == Domain) { IsRedirect++; }
                    }
                }
                

                //перенаправляем на первый из своих доменов
                if(IsRedirect==0)
                {
                    string url = "http://" + AccountInfo.Domains[0].DomainName + "/Admin/";
                    Response.Redirect(url, true);
                }
            }
            #endregion
            
            #region  Логи
            try
            {
                _PageId = Guid.Parse(filterContext.RouteData.Values["id"].ToString());
                LogInfo = _cmsRepository.getCmsPageLog(_PageId);
            }
            catch { }
            #endregion
        }

        private void CmsRepository_DislyEvent(object sender, DislyEventArgs e)
        {
            switch (e.EventLevel)
            {
                case LogLevelEnum.Debug:
                    AppLogger.Debug(e.Message, e.Exception);
                    break;
                case LogLevelEnum.Error:
                    AppLogger.Error(e.Message, e.Exception);
                    break;
                case LogLevelEnum.Warn:
                    AppLogger.Warn(e.Message, e.Exception);
                    break;
                case LogLevelEnum.Info:
                    AppLogger.Info(e.Message, e.Exception);
                    break;
            }
        }

        public CoreController()
        {
                _accountRepository = new AccountRepository("cmsdbConnection");

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

        public static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            foreach (var enc in ImageCodecInfo.GetImageEncoders())
                if (enc.MimeType.ToLower() == mimeType.ToLower())
                    return enc;
            return null;
        }
        
        public FilterParams getFilter(int defaultPageSize = 20)
        {
            string return_url = HttpUtility.UrlDecode(Request.Url.Query);
            // если в URL номер страницы равен значению по умолчанию - удаляем его из URL
            try
            {
                return_url = (Convert.ToInt32(Request.QueryString["page"]) == 1) ? addFiltrParam(return_url, "page", String.Empty) : return_url;
            }
            catch {
                return_url = addFiltrParam(return_url, "page", String.Empty);
            }
            try {
                return_url = (Convert.ToInt32(Request.QueryString["size"]) == defaultPageSize) ? addFiltrParam(return_url, "size", String.Empty) : return_url;
            }
            catch {
                return_url = addFiltrParam(return_url, "size", String.Empty);
            }                
            return_url = (!Convert.ToBoolean(Request.QueryString["disabled"])) ? addFiltrParam(return_url, "disabled", String.Empty) : return_url;
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
                SearchText = (String.IsNullOrEmpty(Request.QueryString["searchtext"])) ? String.Empty : Request.QueryString["searchtext"]
                //,
                //Disabled = (String.IsNullOrEmpty(Request.QueryString["disabled"])) ? Nullable : Convert.ToBoolean(Request.QueryString["disabled"])
            };
            if (!String.IsNullOrEmpty(Request.QueryString["disabled"]))
            {
                result.Disabled = Convert.ToBoolean(Request.QueryString["disabled"]);
            }
            
            //if (result.Date != DateNull && result.DateEnd == DateNull)
            //{
            //    result.DateEnd = ((DateTime)result.Date).AddDays(1);
            //}

            return result;
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
    }
}