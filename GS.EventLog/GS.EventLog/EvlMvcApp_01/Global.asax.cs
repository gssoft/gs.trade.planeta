using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using GS.EventLog;
using GS.EventLog.DataBase.Dal;
using Newtonsoft.Json;

namespace EvlMvcApp_01
{
    // Примечание: Инструкции по включению классического режима IIS6 или IIS7 
    // см. по ссылке http://go.microsoft.com/?LinkId=9394801

    public class InitDb : DropCreateDatabaseIfModelChanges<EvlContext>
    {
        protected override void Seed(EvlContext db)
        {
            var evl = new GS.EventLog.DataBase.Model.DbEventLog
            {
                Name = "GS.Trade.EventLog",
                Alias = "GS.Trade.EventLog",
                Code = "GS.Trade.EventLog",
                Description = "GS.Trade.EventLog",
                LongCode = "GS.Trade.EventLog"
            };
            db.SaveChanges();
        }
    }

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var formatters = GlobalConfiguration.Configuration.Formatters;
            formatters.Remove(formatters.XmlFormatter);

            //  GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            Database.SetInitializer(new InitDb());

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }
    }
}