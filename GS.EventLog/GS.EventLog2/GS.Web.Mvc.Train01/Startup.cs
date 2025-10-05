using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GS.Web.Mvc.Train01.Startup))]
namespace GS.Web.Mvc.Train01
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
