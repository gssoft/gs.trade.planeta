using System.Web;
using System.Web.Mvc;

namespace GS.Trade.Web.Mvc_07
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
