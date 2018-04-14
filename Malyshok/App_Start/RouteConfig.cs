using System.Web.Mvc;
using System.Web.Routing;

namespace Disly
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Ошибка
            routes.MapRoute(
               name: "Error",
               url: "Error/{*code}",
               defaults: new { controller = "Error", action = "Custom", code = UrlParameter.Optional }
            );

            // Главная страница
            routes.MapRoute(
               name: "Index",
               url: "",
               defaults: new { controller = "Home", action = "Index" }
            );

            // Продукция
            routes.MapRoute(
               name: "ProdCatalog",
               url: "catalog/{*path}",
               defaults: new { controller = "Prod", action = "Index", path = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "ProdItem",
               url: "prod/{id}",
               defaults: new { controller = "Prod", action = "item" }
            );
            // Новинки
            routes.MapRoute(
               name: "Novelties",
               url: "novelties/{id}",
               defaults: new { controller = "Prod", action = "Novelties", id = UrlParameter.Optional }
            );
            // Сертификаты
            routes.MapRoute(
               name: "Certificates",
               url: "certificates/{id}",
               defaults: new { controller = "Prod", action = "Certificates", id = UrlParameter.Optional }
            );
            // Список продуктов
            routes.MapRoute(
               name: "ProdList",
               url: "prod-list/{*path}",
               defaults: new { controller = "Prod", action = "List", path = UrlParameter.Optional }
            );

            // Контакты
            routes.MapRoute(
               name: "Contacts",
               url: "contacts/{id}",
               defaults: new { controller = "Contacts", action = "Index", id = UrlParameter.Optional }
            );

            // поиск
            routes.MapRoute(
               name: "Search",
               url: "Search/",
               defaults: new { controller = "Search", action = "Index" }
            );
            // 
            routes.MapRoute(
               name: "Photolist",
               url: "photolist/{id}",
               defaults: new { controller = "Service", action = "Photolist", id = UrlParameter.Optional }
            );
            
            // Материалы
            routes.MapRoute(
               name: "PressCentrItem",
               url: "Press/{year}/{month}/{day}/{alias}",
               defaults: new { controller = "Press", action = "Item", alias = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "PressCentrRss",
               url: "Press/Rss",
               defaults: new { controller = "Press", action = "Rss", alias = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "PressCentrRssSettings",
               url: "press/RssSettings",
               defaults: new { controller = "Press", action = "RssSettings", alias = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "PressCentrCategory",
               url: "Press/{category}",
               defaults: new { controller = "Press", action = "Category", alias = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "PressCentr",
               url: "Press/{category}/{*path}",
               defaults: new { controller = "Press", action = "Index", category = UrlParameter.Optional, path = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "User",
               url: "User/{action}/{id}",
               defaults: new { controller = "User", action = "Index", id = UrlParameter.Optional }
            );
            
            routes.MapRoute(
               name: "Basket",
               url: "basket/{action}/{id}",
               defaults: new { controller = "Basket", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "MergeOrders",
               url: "MergeOrders/{action}/{id}",
               defaults: new { controller = "MergeOrders", action = "Index", id = UrlParameter.Optional }
            );

            // Типовая страница (карта сайта)
            routes.MapRoute(
               name: "Page",
               url: "{*path}",
               defaults: new { controller = "Page", action = "Index", path = UrlParameter.Optional }
               //constraints: new { path = @"\d{6}" }
            );
            
            routes.MapRoute(
                name: "Service",
                url: "Service/{action}/{*id}",
                defaults: new { controller = "Service", action = "Index", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                name: "default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }//,
                //constraints: new { path = @"\d{6}" }
            );


        }
    }
}
