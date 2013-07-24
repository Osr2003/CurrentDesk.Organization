using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CurrentDesk.BackOffice
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "DemoAccount",
                "Account/Demo/{id}",
                new { controller = "AccountSignUp", action = "Index", id = UrlParameter.Optional },
                new[] { "CurrentDesk.BackOffice.Controllers" }
                );

            routes.MapRoute(
                "LiveAccount",
                "Account/Live/{id}",
                new { controller = "AccountSignUp", action = "LiveAccountInitial", id = UrlParameter.Optional },
                new[] { "CurrentDesk.BackOffice.Controllers" }
                );


            routes.MapRoute(
               "PartnersAccount",
               "Account/Partner/{id}",
               new { controller = "AccountSignUp", action = "PartnerAccountInitial", id = UrlParameter.Optional },
               new[] { "CurrentDesk.BackOffice.Controllers" }
               );


            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "Account", action = "Login", id = UrlParameter.Optional},
                new[] {"CurrentDesk.BackOffice.Controllers"}
                );

            routes.MapRoute(
                "404-PageNotFound",
                "{*url}",
                new {controller = "Error", action = "PageNotFound"}
                );
        }
    }
}