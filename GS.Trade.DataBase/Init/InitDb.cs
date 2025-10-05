using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Trade.DataBase.Dal;

namespace GS.Trade.DataBase.Init
{
    //public class InitDb : DropCreateDatabaseAlways<DbTradeContext>
    public class InitDb : DropCreateDatabaseIfModelChanges<DbTradeContext>
    // public class InitDb : CreateDatabaseIfNotExists<DbTradeContext>
    { 
        protected override void Seed(DbTradeContext context)
        {
            var seed = MethodBase.GetCurrentMethod()?.Name + "()";
            base.Seed(context);
            ConsoleSync.WriteLineT($"HelloFromSeed Type: {GetType()} Method: {seed}");
        }
    }
}
