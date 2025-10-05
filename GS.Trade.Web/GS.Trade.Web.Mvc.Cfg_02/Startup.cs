using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GS.Trade.Web.Mvc.Cfg_02.Startup))]
namespace GS.Trade.Web.Mvc.Cfg_02
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
