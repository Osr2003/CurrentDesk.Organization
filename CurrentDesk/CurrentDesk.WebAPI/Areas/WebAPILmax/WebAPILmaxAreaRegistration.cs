using System.Web.Mvc;

namespace CurrentDesk.WebAPI.Areas.WebAPILmax
{
    public class WebAPILmaxAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WebAPILmax";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WebAPILmax_default",
                "WebAPILmax/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "CurrentDesk.WebAPI.Areas.WebAPILmax.Controllers" }
            );
        }
    }
}
