using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GS.EventLog
{
    //public class Counter
    //{
    //    public long Value { get; private set; } 
    //    private readonly object _locker;
    
    //    public Counter()
    //    {
    //        _locker = new object();
    //    }

    //    public long Next()
    //    {
    //        lock (_locker)
    //        {
    //            return ++ Value;
    //        }
    //    }
    //    public void Clear()
    //    {
    //        lock (_locker)
    //            Value = 0;
    //    }

    //}
    public class Counter
    {
        protected long Value;

        public long Next()
        {
            return Interlocked.Increment(ref Value);
        }

        public void Clear()
        {
            Interlocked.Exchange(ref Value, 0);
        }
    }
    //public class Counter<T>
 
    //{
    //    protected T Value;

    //    public T Next()
    //    {
    //        return Interlocked.Increment(ref Value);
    //    }

    //    public void Clear()
    //    {
    //        Interlocked.Exchange(ref Value, 0);
    //    }

    //}

    //public class Counter<T>
    //    where T : ValueType
    //{
    //    public T Value;
    //    private readonly object _locker;

    //    public Counter()
    //    {
    //        _locker = new object();
    //    }

    //    public long Next()
    //    {
    //        T a = T(1);
    //        lock (_locker)
    //        {
    //            return Value += 1;
    //        }
    //    }
    //    public void Clear()
    //    {
    //        lock (_locker)
    //            Value = 0;
    //    }

    //}
}
