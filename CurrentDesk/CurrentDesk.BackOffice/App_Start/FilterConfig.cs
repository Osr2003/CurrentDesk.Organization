using CurrentDesk.BackOffice.Filters;
using System.Web;
using System.Web.Mvc;

namespace CurrentDesk.BackOffice
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new EnableCompressionAttribute());
        }
    }
}