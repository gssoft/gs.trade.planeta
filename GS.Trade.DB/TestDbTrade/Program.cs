using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.DataBase.Dal;
using GS.Trade.DataBase.Model;
using GS.Trade.DataBase.Storage;

namespace TestDbTrade
{
    class Program
    {
        public class InitDb : DropCreateDatabaseIfModelChanges<DbTradeContext>
        {
            protected override void Seed(DbTradeContext context)
            {
                base.Seed(context);
            }
        }

        static void Main()
        {
            Database.SetInitializer(new InitDb());

            var db = new DbTradeContext();
            var a = new Account
            {
                Name = "12345",
                Alias = "OpenAcc-001",
                Code = "12345",
                TradePlace = "Forts",
                Key = "1"
            };
            db.Accounts.Add(a);

            a = new Account
            {
                Name = "54321",
                Alias = "Vtb24-001",
                Code = "54321",
                TradePlace = "Forts",
                Key = "2"
            };
            db.Accounts.Add(a);

            var t = new Ticker
            {
                Name = "Ticker",
                Alias = "Alias",
                Code = "Code",

                Created = DateTime.Now,
                Modified = DateTime.Now,
                LaunchDate = DateTime.Now,
                ExpireDate = DateTime.Now
            };
            db.Tickers.Add(t);

            var s = new Strategy
            {
                Name = "MyStratName",
                Alias = "MyAlias",
                Code = "MyCode"
            };
            a.Strategies.Add(s);
            t.Strategies.Add(s);

            //var d = new Deal
            //{
            //    Number = 1,
            //    Price1 = (decimal) 100.24,
            //    Price2 = (decimal) 120.25,
            //    FirstTradeNumber = 100,
            //    LastTradeNumber = 250
            //};
            //s.Deals.Add(d);
            //d = new Deal
            //{
            //    Number = 1,
            //    Price1 = (decimal)300.24,
            //    Price2 = (decimal)520.25,
            //    FirstTradeNumber = 1001,
            //    LastTradeNumber = 2501
            //};
            //s.Deals.Add(d);

            var ord = new Order
            {
                Number = 1,
                OrderKey = "OrderKey",
                StrategyKey = "StrstegyKey",
                Created = DateTime.Now,
                Modified = DateTime.Now
            };
            db.Orders.Add(ord);
            
            db.SaveChanges();

            //var st = new DbStorage();
            //var strKey = st.GetStrategyKeyFromOrder("OrderKey");

        }
    }
}
