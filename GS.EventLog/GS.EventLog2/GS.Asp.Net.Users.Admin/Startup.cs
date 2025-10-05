using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GS.Asp.Net.Users.Admin.Startup))]
namespace GS.Asp.Net.Users.Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
