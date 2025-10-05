using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade
{
    public enum QuikTransactionActionEnum
    {
        SetLimit = 1, SetStop = 2, SetStopLimit = 3,
        KillLimit = -1, KillStop = -2, KillStopLimit = -3
    }
}
