using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.EventLog.DataBase1.Dal
{
    using GS.EventLog.DataBase1.Model;
    public class Initializer : DropCreateDatabaseIfModelChanges<EvlContext1>
    //public class Initializer : DropCreateDatabaseAlways<EvlContext1>
    {
        protected override void Seed(EvlContext1 db)
        {
            Console.WriteLine("Initialize DataBase");
            var evl = new DbEventLog
            {
                Name = "GS.Trade.EventLog",
                Alias = "GS.Trade.EventLog",
                Code = "GS.Trade.EventLog",
                Description = "GS.Trade.EventLog",
            };
            db.EventLogs.Add(evl);
            db.SaveChanges();
        }
    }
}
