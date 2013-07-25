using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CurrentDesk.BackOffice.Controllers;

namespace CurrentDesk.BackOffice
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();

            //Remove All Engine
            ViewEngines.Engines.Clear();

            //Add Razor Engine
            ViewEngines.Engines.Add(new RazorViewEngine());

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// This method catches all global exceptions and handle them accordingly
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            HttpException httpException = exception as HttpException;
            
            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            
            switch (httpException.GetHttpCode())
            {
                case 404:
                    // Page not found.
                    routeData.Values.Add("action", "PageNotFound");
                    break;
                default:
                    routeData.Values.Add("action", "ErrorMessage");
                    break;
            }

            //Pass exception details to the target error View
            routeData.Values.Add("error", exception);

            //Clear the error on server
            Server.ClearError();

            //Avoid IIS7 getting in the middle
            Response.TrySkipIisCustomErrors = true;

            //Call target Controller and pass the routeData
            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }
    }
}