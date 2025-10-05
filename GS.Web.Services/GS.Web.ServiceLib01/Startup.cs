using System.Web.Http;
using GS.Web.Api.Service01;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace GS.Web.Api.Service01
{
   
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
#if DEBUG
            app.UseErrorPage();
#endif
            app.UseWelcomePage("/");

            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            var webApiConfiguration = ConfigureWebApi();

            // Use the extension method provided by the WebApi.Owin library:
            app.UseWebApi(webApiConfiguration);

        }
        private HttpConfiguration ConfigureWebApi()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional});

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            return config;
        }
    }
}
