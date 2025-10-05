using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade
{
    public enum HedgeStatusEnum : short
    {
        Unknown = 0, LongHedge, ShortHedge, NoHedge
    }
}
