using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using GS.EventLog.DataBase.Dal;
using Newtonsoft.Json;

namespace WebEventLog_02
{
    // Примечание: Инструкции по включению классического режима IIS6 или IIS7 
    // см. по ссылке http://go.microsoft.com/?LinkId=9394801

    //public class InitDb : DropCreateDatabaseIfModelChanges<EvlContext>
    //{
    //    protected override void Seed(EvlContext db)
    //    {
    //        var evl = new GS.EventLog.DataBase.Model.DbEventLog
    //        {
    //            Name = "GS.Trade.EventLog",
    //            Alias = "GS.Trade.EventLog",
    //            Code = "GS.Trade.EventLog",
    //            Description = "GS.Trade.EventLog",
    //            LongCode = "GS.Trade.EventLog"
    //        };
    //        db.EventLogs.Add(evl);
    //        db.SaveChanges();
    //    }
    //}

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {



            //  GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            // GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            Database.SetInitializer(new Initializer());

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var formatters = GlobalConfiguration.Configuration.Formatters;
            formatters.Remove(formatters.XmlFormatter);

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }
        void ConfigureApi(HttpConfiguration config)
        {
            // Remove the JSON formatter
            //config.Formatters.Remove(config.Formatters.JsonFormatter);

            // or

            // Remove the XML formatter
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}