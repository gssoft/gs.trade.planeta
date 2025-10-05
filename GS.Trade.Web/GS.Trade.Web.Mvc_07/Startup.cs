using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GS.Trade.Web.Mvc_07.Startup))]
namespace GS.Trade.Web.Mvc_07
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
