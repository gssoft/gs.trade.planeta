using System;
using System.Collections.Generic;
using System.Data.Entity;
using GS.EventLog.DataBase;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.EventLog.DataBase.Dal
{
    using GS.EventLog.DataBase.Model;
    public class Initializer : DropCreateDatabaseIfModelChanges<EvlContext>
    {
        protected override void Seed(EvlContext db)
        {
            Console.WriteLine("Initialize DataBase");
            var evl = new DbEventLog
            {
                Name = "GS.Trade.EventLog",
                Alias = "GS.Trade.EventLog",
                Code = "GS.Trade.EventLog",
                Description = "GS.Trade.EventLog",
                LongCode = "GS.Trade.EventLog"
            };
            db.EventLogs.Add(evl);
            db.SaveChanges();
        }
    }
}
