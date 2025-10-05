using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GS.Trade.Web.Mvc.TimeSeries01.Startup))]
namespace GS.Trade.Web.Mvc.TimeSeries01
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
