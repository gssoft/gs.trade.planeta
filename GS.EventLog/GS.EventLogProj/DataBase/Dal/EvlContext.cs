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
        public DbSet<Model.EventLog> EventLogs { get; set; }
        public DbSet<Model.EventLogItem> EventLogItems { get; set; }
    }
}
