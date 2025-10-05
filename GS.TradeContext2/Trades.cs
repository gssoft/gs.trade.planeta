using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.TradeContext
{
    public partial class TradeContext
    {
        public int RegisterNewTrade(ITrade t)
        {
            var ret = Strategies.RegisterNewTrade(t);
            return +1;
        }
    }
}
