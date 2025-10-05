using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade
{
    public interface IReportItem
    {
        DateTime DT { get; }
        DateTime FirstTradeDT { get; }
        DateTime LastTradeDT { get; }
        decimal PnL { get; }
        decimal Costs { get; }
    }
}
