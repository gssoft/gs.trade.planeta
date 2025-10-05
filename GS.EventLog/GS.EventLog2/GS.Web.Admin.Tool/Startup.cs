using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GS.Web.Admin.Tool.Startup))]
namespace GS.Web.Admin.Tool
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
