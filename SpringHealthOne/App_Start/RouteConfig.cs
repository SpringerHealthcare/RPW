using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SpringHealthOne
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("benefits", "benefits", new { controller = "Pages", action = "Details", id = "1" });
            routes.MapRoute("home", "", new { controller = "Pages", action = "Details", id = "2" });
            routes.MapRoute("features", "features", new { controller = "Pages", action = "Details", id = "3" });
            routes.MapRoute("for-publishers", "for-publishers", new { controller = "Pages", action = "Details", id = "4" });
            routes.MapRoute("contact-us", "contact-us", new { controller = "Pages", action = "Details", id = "5" });
            routes.MapRoute("europe", "europe", new { controller = "Pages", action = "Details", id = "6" });
            routes.MapRoute("latin-america", "latin-america", new { controller = "Pages", action = "Details", id = "7" });
            routes.MapRoute("north-america", "north-america", new { controller = "Pages", action = "Details", id = "8" });
            routes.MapRoute("asia-pacific-rim", "asia-pacific-rim", new { controller = "Pages", action = "Details", id = "9" });
            routes.MapRoute("copyright-notice", "copyright-notice", new { controller = "Pages", action = "Details", id = "10" });
            routes.MapRoute("privacy-policy", "privacy-policy", new { controller = "Pages", action = "Details", id = "11" });
            routes.MapRoute("website-disclaimer", "website-disclaimer", new { controller = "Pages", action = "Details", id = "12" });
            routes.MapRoute("cookie-policy", "cookie-policy", new { controller = "Pages", action = "Details", id = "13" });

            
            //routes.MapRoute("ID", "Pages/{id}", new { controller = "Pages", action = "Details" });
           
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
           
            //routes.MapRoute("About", "About", new { controller = "About", action = "Index" });
            //routes.MapRoute("ID", "{id}", new { controller = "Home", action = "Details" });
            //routes.MapRoute("Default", "", new { controller = "Home", action = "Index" });
        }
    }
}