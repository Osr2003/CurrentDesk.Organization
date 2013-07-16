using System.Web.Mvc;

namespace CurrentDesk.BackOffice.Areas.AssetManager
{
    public class AssetManagerAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AssetManager";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AssetManager_default",
                "AssetManager/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "CurrentDesk.BackOffice.Areas.AssetManager.Controllers" }
            );
        }
    }
}
