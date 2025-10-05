using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace GS.EventLog.DataBase.Dal
{
    public class EvlContext : DbContext
    {
        public EvlContext(string db)
            : base(nameOrConnectionString: db) //base("EvlContext")
        {         
        }

        public EvlContext() : base("EventLog")
        {
        }

        public DbSet<Model.DbEventLog> EventLogs { get; set; }
        public DbSet<Model.DbEventLogItem> EventLogItems { get; set; }
    }
}
