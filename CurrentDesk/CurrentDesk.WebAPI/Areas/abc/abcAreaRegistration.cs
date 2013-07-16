using System.Web.Mvc;

namespace CurrentDesk.WebAPI.Areas.abc
{
    public class abcAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "abc";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "abc_default",
                "abc/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "CurrentDesk.WebAPI.Areas.abc.Controllers" }
            );           
        }
    }
}
