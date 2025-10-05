using System.Data.Entity;
using GS.EventLog.DataBase1.Model;
using GS.EventLog.Dto;

namespace GS.EventLog.DataBase1.Dal
{
    using GS.EventLog.DataBase1.Model;

    public partial class EvlContext1 : DbContext
    {

        public EvlContext1() : base("EventLog1")
        {
            Database.CommandTimeout = 900;
        }

         public EvlContext1(string db)
            : base(nameOrConnectionString: db)
        {
            Database.CommandTimeout = 900;
        }

        public DbSet<DbEventLog> EventLogs { get; set; }
        public DbSet<DbEventLogItem> EventLogItems { get; set; }

        public DbEventLog CreateDbEventLog(EventLogDto dto)
        {
            return new DbEventLog
            {
                EventLogID = dto.ID,
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description
            };
        }
        public DbEventLog CreateDbEventLog(EventLog evl)
        {
            return new DbEventLog
            {
                Code = evl.Code,
                Name = evl.Name,
                Description = evl.Description
            };
        }
        public EventLogDto CreateEventLogDto(DbEventLog evl)
        {
            return new EventLogDto
            {
                ID = evl.EventLogID,
                Code = evl.Code,
                Name = evl.Name,
                Description = evl.Description
            };
        }

        public EventLogItemDto CreateEventLogItemDto(DbEventLogItem i)
        {
            return new EventLogItemDto
            {
                ID = i.EventLogItemID,
                DT = i.DT,
                ResultCode = i.ResultCode,
                Subject = i.Subject,
                Source = i.Source,
                Entity = i.Entity,
                Operation = i.Operation, 
                Description = i.Description,
                Object = i.Object,
                Index = i.Index
            };
        }
        public DbEventLogItem CreateDbEventLogItem(EventLogItemDto i)
        {
            return new DbEventLogItem
            {
                // EventLogItemID = i.ID,
                EventLogID = i.EventLogID,
                DT = i.DT,
                ResultCode = i.ResultCode,
                Subject = i.Subject,
                Source = i.Source,
                Entity = i.Entity,
                Operation = i.Operation,
                Description = i.Description,
                Object = i.Object,
                Index = i.Index
            };
        }
    }
}
