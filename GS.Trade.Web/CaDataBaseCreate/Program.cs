using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Trade;
using GS.Trade.DataBase.Dal;
using GS.Trade.DataBase.Init;

namespace CaDataBaseCreate
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer(new InitDb());
            var db = new DbTradeContext();
            var accounts = db.Accounts;
            var trades = db.Trades;
            var orders = db.Orders;
            var ps = db.Positions;

            
            ConsoleSync.WriteReadLineT($"Hello from Create DB. Account.Count = {accounts.Count()}");
        }
    }
}
