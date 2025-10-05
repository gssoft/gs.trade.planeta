using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Ca_OwinSelfHost_Auth_01.Models;
using GS.ConsoleAS;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;



namespace Ca_OwinSelfHost_Auth_01
{
    
    public class Startup
    {
        private const int AccessTokenExpireTimeSpanMinute = 5;

        public void Configuration(IAppBuilder app)
        {
#if DEBUG
            app.UseErrorPage();
#endif
            // app.UseWelcomePage("/");

            ConsoleSync.WriteReadLineT(
                $"Start Method: {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            ConfigureOAuth(app);

            var webApiConfiguration = ConfigureWebApi();

            // Use the extension method provided by the WebApi.Owin library:
            app.UseWebApi(webApiConfiguration);

            // ConfigureOAuth(app);

            ConsoleSync.WriteReadLineT(
                $"Finish Method: {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }
        private HttpConfiguration ConfigureWebApi()
        {
            ConsoleSync.WriteReadLineT(
                $"Start Method: {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            ConsoleSync.WriteReadLineT(
                $"Finish Method: {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            return config;
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            ConsoleSync.WriteReadLineT(
               $"Start Method: {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            app.CreatePerOwinContext<OwinAuthDbContext>(() => new OwinAuthDbContext());
            app.CreatePerOwinContext<UserManager<IdentityUser>>(CreateManager);
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/oauth/token"),
                Provider = new AuthorizationServerProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(AccessTokenExpireTimeSpanMinute),
                AllowInsecureHttp = true,

            });
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            ConsoleSync.WriteReadLineT(
               $"Finish Method: {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }

        public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
        {
            public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
            {
                context.Validated();
                //string clientId;
                //string clientSecret;

                //if (context.TryGetBasicCredentials(out clientId, out clientSecret))
                //{
                //    // validate the client Id and secret 
                //    context.Validated();
                //}
                //else
                //{
                //    context.SetError("invalid_client", "Client credentials could not be retrieved from the Authorization header");
                //    context.Rejected();
                //}
            }
            public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
            {
                UserManager<IdentityUser> userManager = context.OwinContext.GetUserManager<UserManager<IdentityUser>>();
                IdentityUser user;
                try
                {
                    user = await userManager.FindAsync(context.UserName, context.Password);

                    ConsoleSync.WriteLineT(
                        $"Method: {System.Reflection.MethodBase.GetCurrentMethod().Name}, User: {user.Email}");
                
                }
                catch
                {
                    // Could not retrieve the user due to error.
                    context.SetError("server_error");
                    context.Rejected();
                    return;
                }
                if (user != null)
                {
                    ClaimsIdentity identity = await userManager.CreateIdentityAsync(
                                                            user,
                                                            DefaultAuthenticationTypes.ExternalBearer);
                    context.Validated(identity);

                    ConsoleSync.WriteLineT(
                        $"Method: {System.Reflection.MethodBase.GetCurrentMethod().Name}, UserID: {identity.GetUserId()}");

                    ConsoleSync.WriteLineT(
                        $"Method: {System.Reflection.MethodBase.GetCurrentMethod().Name}, UserName: {identity.GetUserName()}");

                    ConsoleSync.WriteLineT(
                        $"Method: {System.Reflection.MethodBase.GetCurrentMethod().Name}, IsAuthenticated: {identity.IsAuthenticated}" + Environment.NewLine);

                }
                else
                {
                    context.SetError("invalid_grant", "Invalid UserId or password'");
                    context.Rejected();
                }
            }
        }
        private static UserManager<IdentityUser> CreateManager(IdentityFactoryOptions<UserManager<IdentityUser>> options, IOwinContext context)
        {
            var userStore = new UserStore<IdentityUser>(context.Get<OwinAuthDbContext>());
            var owinManager = new UserManager<IdentityUser>(userStore);
            return owinManager;
        }
    }
}
   