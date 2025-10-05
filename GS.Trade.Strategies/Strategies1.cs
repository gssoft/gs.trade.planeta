using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Xml;

namespace GS.Trade.Strategies
{
    public partial class Strategies
    {
        public void ClearOrderCollection()
        {
            foreach (var s in StrategyCollection)
            {
                s.ClearOrderCollection();
            }
        }
        public void Init(string xmlfile, string nodepath)
        {
            var strategies = 
                XDocExtensions.DeSerialize<IStrategy>(xmlfile, nodepath,
                    "GS.Trade.Strategies", "GS.Trade.Strategies");
            foreach (var s in strategies)
            {
                s.SetParent(this);
                s.SetTradeContext(TradeContext);
                s.Parent = this;
                ((Strategy)s).Init();
                Register(s);
            }
        }
    }
}
