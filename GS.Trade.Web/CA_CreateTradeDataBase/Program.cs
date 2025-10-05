using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Extension;
using GS.Trade.DataBase.Dal;
using GS.Trade.DataBase.Init;

namespace CA_CreateTradeDataBase
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var init = new InitDb();
                ConsoleSync.WriteLineT($"Start InitDb: {init.ToString()}");
                Database.SetInitializer(init);

                var db = new DbTradeContext();
                var trs = db.Trades.Where(t => t.Price > 0);
                ConsoleSync.WriteLineT($"Stop Fetch(): {trs.Count()} Trades");


            }
            catch (Exception ex)
            {
                ex.PrintException();
            }

            ConsoleSync.WriteReadLineT("Ready to use ...");
        }
    }
}
