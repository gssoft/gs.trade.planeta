using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.DataBase
{
    public static class Extensions
    {
        public static ulong ToUint64(this long value)
        {
            return Convert.ToUInt64(value);
        }
        public static ulong ToUint64(this decimal value)
        {
            return Convert.ToUInt64(value);
        }
        public static decimal ToDecimal(this long value)
        {
            return Convert.ToDecimal(value);
        }
        public static decimal ToDecimal(this ulong value)
        {
            return Convert.ToDecimal(value);
        }
    }
}
