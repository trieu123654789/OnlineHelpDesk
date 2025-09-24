using System.Web;
using System.Web.Mvc;
// using OnlineHelpDesk2.Attributes;

namespace OnlineHelpDesk2
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // TODO: Add global authorization filter later
            // filters.Add(new GlobalAuthorizationFilter());
        }
    }
}
