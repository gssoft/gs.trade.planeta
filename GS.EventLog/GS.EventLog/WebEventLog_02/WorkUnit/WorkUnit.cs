using System;
using System.Data.Entity.Infrastructure;
using GS.EventLog.DataBase;
using GS.EventLog.DataBase.Dal;
using GS.EventLog.DataBase.Model;

namespace WebEventLog_02.WorkUnit
{
    public class WorkUnit
    {
        private readonly EvlContext _context = new EvlContext("EvlContext");

        private IRepository<DbEventLog> _evenLogs = null;
        private IRepository<DbEventLogItem> _eventLogItems = null;

        public IRepository<DbEventLog> EventLogs
        {
            get { return this._evenLogs ?? (_evenLogs = new GenericRepository<DbEventLog>(_context)); }
        }

        public IRepository<DbEventLogItem> EventLogItems
        {
            get
            {
                return _eventLogItems ??
                       (_eventLogItems = new GenericRepository<DbEventLogItem>(_context));
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public int SaveChanges(out Exception exc)
        {
            try
            {
                exc = null;
                return _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                exc = ex;
                return -6;
            }
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }
    }
}