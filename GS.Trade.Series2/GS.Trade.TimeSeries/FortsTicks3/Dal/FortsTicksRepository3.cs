using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.TimeSeries.FortsTicks3.Dal
{
    public class FortsTicksRepository3 : IDisposable
    {
        // Flag: Has Dispose already been called?
        private bool _disposed = false;

        private readonly DbContext _db;
        public FortsTicksRepository3() : this(new FortsTicksContext3())
        {
        }

        public FortsTicksRepository3(DbContext context)
        {
            _db = context;
        }
        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                if(_db != null)
                    _db.Dispose();
                // Free any other managed objects here.
                //
            }
            // Free any unmanaged objects here.
            _disposed = true;
        }
        ~FortsTicksRepository3()
        {
            Dispose(false);
        }
    }
}
