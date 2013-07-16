using System.Web.Mvc;

namespace CurrentDesk.BackOffice.Areas.IntroducingBroker
{
    public class IntroducingBrokerAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "IntroducingBroker";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "IntroducingBroker_default",
                "IntroducingBroker/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "CurrentDesk.BackOffice.Areas.IntroducingBroker.Controllers" }
            );
        }
    }
}
