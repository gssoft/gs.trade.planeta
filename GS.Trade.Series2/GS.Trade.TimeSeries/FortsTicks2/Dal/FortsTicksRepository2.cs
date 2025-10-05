using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.TimeSeries.FortsTicks2.Dal
{
    public class FortsTicksRepository2 : IDisposable
    {
        // Flag: Has Dispose already been called?
        private bool _disposed = false;

        private readonly DbContext _db;
        public FortsTicksRepository2() : this(new FortsTicksContext2())
        {
        }

        public FortsTicksRepository2(DbContext context)
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
        ~FortsTicksRepository2()
        {
            Dispose(false);
        }
    }
}
