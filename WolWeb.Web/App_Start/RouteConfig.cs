using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace WolWeb {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapHttpRoute(
                name: "Ping",
                routeTemplate: "Ping/{id}",
                defaults: new { controller = "Ping", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapHttpRoute(
                name: "Wake",
                routeTemplate: "Wake/{id}",
                defaults: new { controller = "Wake", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapHttpRoute(
                name: "Shutdown",
                routeTemplate: "Shutdown/{id}",
                defaults: new { controller = "Shutdown", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}