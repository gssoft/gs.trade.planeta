using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using GS.Trade.DataBase.Dal;
using GS.Trade.DataBase.Model;
using Newtonsoft.Json;

namespace WebApi_01
{
    // Примечание: Инструкции по включению классического режима IIS6 или IIS7 
    // см. по ссылке http://go.microsoft.com/?LinkId=9394801

    public class InitDb : DropCreateDatabaseIfModelChanges<DbTradeContext>
    {
        protected override void Seed(DbTradeContext db)
        {
            var a = new Account
            {
                Name = "Acc_001",
                Alias = "OpenAcc-001",
                Code = "12345",
                TradePlace = "Forts"
            };
            db.Accounts.Add(a);

            a = new Account
            {
                Name = "Acc_02",
                Alias = "Vtb24-001",
                Code = "54321",
                TradePlace = "Micex"
            };
            db.Accounts.Add(a);

            var t = new Ticker
            {
                Name = "RIZ3",
                Alias = "RI1",
                Code = "RTS-12.13",
                TradeBoard = "SPBFUT"
            };
            db.Tickers.Add(t);

            var s = new Strategy
            {
                Name = "MyStratName",
                Alias = "MyAtrat",
                Code = "MyCode"
            };
            a.Strategies.Add(s);
            t.Strategies.Add(s);

            db.SaveChanges();
        }
    }

    public class WebApiApplication : System.Web.HttpApplication
    {
       
        protected void Application_Start()
        {
          //  GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            //var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            //json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.All;

            //var xml = GlobalConfiguration.Configuration.Formatters.XmlFormatter;
            //var dcs = new DataContractSerializer(typeof(Strategy), null, int.MaxValue,
            //    false, /* preserveObjectReferences: */ true, null);
            //xml.SetSerializer<Strategy>(dcs);

            //var dcs1 = new DataContractSerializer(typeof(Account), null, int.MaxValue,
            //    false, /* preserveObjectReferences: */ true, null);
            //xml.SetSerializer<Account>(dcs1);

            //var dcs2 = new DataContractSerializer(typeof(Ticker), null, int.MaxValue,
            //   false, /* preserveObjectReferences: */ true, null);
            //xml.SetSerializer<Ticker>(dcs2);

            //////////var formatters = GlobalConfiguration.Configuration.Formatters;
            //////////formatters.Remove(formatters.XmlFormatter);

          //  GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            ////////GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            Database.SetInitializer(new InitDb());

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // ConfigureApi();
        }

        void ConfigureApi(HttpConfiguration config)
        {
            // Remove the JSON formatter
            config.Formatters.Remove(config.Formatters.JsonFormatter);

            // or

            // Remove the XML formatter
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}