using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Interfaces;
using GS.Trade.Interfaces;

namespace GS.Trade
{
    public interface ITradeTerminals : IElement1<string>
    {
        ITradeTerminal RegisterTradeTerminal(string type, string key,
                            IOrders ord, IOrders sim, ITrades trs, IEventLog evl, ITradeContext txt);

        void CheckConnection();
        void DisConnect();

        ISimulateTerminal GetSimulateTerminal();
        IEnumerable<ISimulateTerminal> GetSimulateTerminals();

        void OrderResolveProcess();
        void TradeResolveProcess();

        void DeQueueProcess();

        void Start();
        void Stop();

    }

    public interface ITradeTerminals64 : IElement1<string>
    {
        ITradeTerminal RegisterTradeTerminal(string type, string key,
            IOrders ord, IOrders sim, ITrades trs, IEventLog evl, ITradeContext txt);

        void CheckConnection();
        void DisConnect();

        ISimulateTerminal GetSimulateTerminal();
        IEnumerable<ISimulateTerminal> GetSimulateTerminals();

        void OrderResolveProcess();
        void TradeResolveProcess();

        void DeQueueProcess();

        void Start();
        void Stop();

    }
}
