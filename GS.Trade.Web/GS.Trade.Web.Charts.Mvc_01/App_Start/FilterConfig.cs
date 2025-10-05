using System.Web;
using System.Web.Mvc;

namespace GS.Trade.Web.Charts.Mvc_01
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
