using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CurrentDesk.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

           // config.Routes.MapHttpRoute(
           //    name: "APIRoot",
           //    routeTemplate: "apiroot/{controller}/{id}",
           //    defaults: new { id = RouteParameter.Optional }
           //);

           // config.Routes.MapHttpRoute(
           //    name: "NormalAPI",
           //    routeTemplate: "normal/root/{id}",
           //    defaults: new { controller = "DemoLead", id = RouteParameter.Optional }
           //);




        }
    }
}


