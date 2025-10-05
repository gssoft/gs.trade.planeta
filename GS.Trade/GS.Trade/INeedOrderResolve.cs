using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade
{
    public interface INeedOrderResolve
    {
        void OrderResolveProcess();
        void TradeResolveProcess();
    }
}
