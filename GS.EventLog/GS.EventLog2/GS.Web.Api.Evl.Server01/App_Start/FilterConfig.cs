using System.Web;
using System.Web.Mvc;

namespace GS.Web.Api.Evl.Server01
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
