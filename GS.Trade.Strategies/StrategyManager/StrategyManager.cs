using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.Strategies.StrategyManager
{
    public abstract class StrategyManager : IStrategyManager
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Key { get; set; }
    }
}
