using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GS.Counters
{
    public class LongCounter
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
    public class IntCounter
    {
        protected int Value;

        public int Next()
        {
            return Interlocked.Increment(ref Value);
        }

        public void Clear()
        {
            Interlocked.Exchange(ref Value, 0);
        }
    }
}
