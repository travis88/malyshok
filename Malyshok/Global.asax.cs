using Disly.Controllers;
using Portal.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Disly
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error()
        {
            //Exception exception = Server.GetLastError();
            //Response.Clear();

            //HttpException httpException = exception as HttpException;
            //string httpCode = httpException.GetHttpCode().ToString();

            //// clear error on server
            //Server.ClearError();

            //Response.Redirect(String.Format("~/Error/Custom/?httpCode={0}", httpCode));

            //var exception = Server.GetLastError();

            //var httpException = exception as HttpException;
            //var httpCode = httpException != null ? httpException.GetHttpCode() : 404;
            //var errMassege = exception.Message.ToString();
            //AppLogger.Fatal(errMassege, exception);

            //ExecuteError(httpCode, exception.Message.ToString());
        }


        private void ExecuteError(Int32? httpCode, String message)
        {
            var routeData = new RouteData();
            routeData.Values.Add("controller", ErrorController.Name);
            routeData.Values.Add("action", ErrorController.ActionName_Custom);
            if (httpCode.HasValue)
                routeData.Values.Add("httpCode", httpCode.Value);

            try
            {
                Response.Clear();
                Server.ClearError();
                Response.ContentType = "text/html; charset=utf-8";
                Response.TrySkipIisCustomErrors = true;

                IController errorController = new ErrorController();
                errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
            }
            catch (Exception exception)
            {
                //SiteLogger.Fatal("Global.ExecuteError", exception, Context.Request.RequestContext.HttpContext);
                AppLogger.Fatal("Не предвиденная ошибка", exception);
            }
        }
    }
}
