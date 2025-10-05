using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.Strategies.StrategyManager
{
    public class TradeStrategyManager : StrategyManager
    {
        public List<ITradeStrategy> Strategies;
        public TradeStrategyManager()
        {
            Strategies = new List<ITradeStrategy>();
        }
        public ITradeStrategy Register( ITradeStrategy startegy)
        {
            return (ITradeStrategy)(new object());
        }
    }
}