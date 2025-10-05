using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.EventLog.DataBase.Model;
using GS.EventLog.Dto;
using GS.Extension;

namespace GS.EventLog.DataBase1.Dal
{
    using DbEventLog = GS.EventLog.DataBase1.Model.DbEventLog;
    using DbEventLogItem = GS.EventLog.DataBase1.Model.DbEventLogItem;

    public partial class EvlContext1
    {
        public DbEventLog GetEventLog(string appCode, string evlCode)
        {
            return EventLogs.FirstOrDefault(evl => evl.Code == evlCode);   
        }
        public DbEventLog Register(string appName, DbEventLog evl)
        {
            var e = GetEventLog(appName, evl.Code);
            if (e != null)
                return e;  // Already Exist
            
            EventLogs.Add(evl);
            SaveChanges();
            return evl;
        }

        public DbEventLog RegisterEvl(string appName, string evlName)
        {
            evlName = evlName.Trim();
            var e = GetEventLog(appName, evlName);
            if (e != null)
                return e;  // Already Exist
            e = new DbEventLog
            {
                Code = evlName,
                Name = evlName,
                Description = evlName,
                Alias = evlName
            };
            EventLogs.Add(e);
            SaveChanges();
            return e;
        }

        public DbEventLog Register(string appName, EventLogDto evl)
        {
            var e = GetEventLog(appName, evl.Code);
            if (e != null)
                return e;  // Already Exist

            var dbevl = EventLogs.Add(CreateDbEventLog(evl));
            SaveChanges();
            return dbevl;
        }

        public DbEventLogItem Add(EventLogItemDto evli)
        {
            var evl = EventLogs.Find(evli.EventLogID);
            if (evl == null)
                return null;
            var i = CreateDbEventLogItem(evli);
            var ei = EventLogItems.Add(i);
            SaveChanges();
            return ei;
        }

        public DbEventLogItem AddWithCheck(DbEventLogItem evli)
        {
            var evl = EventLogs.Find(evli.EventLogID);
            if (evl == null)
            return null;
           
            var ei = EventLogItems.Add(evli);
            SaveChanges();
            return ei;
        }
        public DbEventLogItem Add(DbEventLogItem evli)
        {
            var ei = EventLogItems.Add(evli);
            SaveChanges();
            return ei;
        }

        public DbEventLog GetUnknown()
        {
            var evl = EventLogs.FirstOrDefault(e => e.Code == "Unknown");
            if (evl != null)
                return evl;
            var u = new DbEventLog
            {
                Code = "Unknown",
                Name = "Unknown",
                Description = "Unknown EventLog for Unknown EventLogItems"
            };
            u = EventLogs.Add(u);
            return u;
        }

        public bool IsEvlExist(int id)
        {
            return EventLogs.Find(id) != null;
        }
    }
}
