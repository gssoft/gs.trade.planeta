using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GS.Web.Mvc.Evl.Server01.Startup))]
namespace GS.Web.Mvc.Evl.Server01
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
