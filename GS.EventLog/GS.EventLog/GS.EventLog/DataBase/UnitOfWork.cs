using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.EventLog.DataBase.Dal;
using GS.EventLog.DataBase.Model;

namespace GS.EventLog.DataBase
{
    public class UnitOfWork : IDisposable
    {

        private readonly EvlContext _context = new EvlContext("EvlContext");

        private IRepository<Model.DbEventLog> _evenLogs = null;
        private IRepository<DbEventLogItem> _eventLogItems = null;

        public IRepository<Model.DbEventLog> EventLogs
        {
            get
            {
                if (this._evenLogs == null)
                {
                    this._evenLogs = new GenericRepository<Model.DbEventLog>(this._context);
                }
                return this._evenLogs;
            }
        }

        public IRepository<DbEventLogItem> EventLogItems
        {
            get
            {
                if (this._eventLogItems == null)
                {
                    this._eventLogItems = new GenericRepository<DbEventLogItem>(this._context);
                }
                return this._eventLogItems;
            }
        }

        public void SaveChanges()
        {
            this._context.SaveChanges();
        }

        public void Dispose()
        {
            if (this._context != null)
            {
                this._context.Dispose();
            }
        }
    }
}
