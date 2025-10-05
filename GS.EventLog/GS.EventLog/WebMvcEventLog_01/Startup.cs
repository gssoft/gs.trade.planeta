using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebMvcEventLog_01.Startup))]
namespace WebMvcEventLog_01
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
