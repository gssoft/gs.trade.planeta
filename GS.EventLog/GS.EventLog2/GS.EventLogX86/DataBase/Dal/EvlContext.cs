using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using GS.EventLog.Dto;

namespace GS.EventLog.DataBase.Dal
{
    public partial class EvlContext : DbContext
    {
        public EvlContext(string db)
            : base(nameOrConnectionString: db) //base("EvlContext")
        {         
        }

        public EvlContext() : base("EventLog")
        {
            Database.CommandTimeout = 300;
        }

        public DbSet<Model.DbEventLog> EventLogs { get; set; }
        public DbSet<Model.DbEventLogItem> EventLogItems { get; set; }

        public Model.DbEventLog CreateDbEventLog(EventLogDto dto)
        {
            return new Model.DbEventLog
            {
                EventLogID = dto.ID,
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description
            };
        }
        public EventLogDto CreateEventLogDto(Model.DbEventLog db)
        {
            return new EventLogDto
            {
                ID = db.EventLogID,
                Code = db.Code,
                Name = db.Name,
                Description = db.Description
            };
        }
    }
}
