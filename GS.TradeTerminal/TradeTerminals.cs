using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;
using GS.Queues;
using GS.Trade.Interfaces;
using GS.Trade.Trades;
using GS.Trade.TradeTerminals.Quik;
using GS.Trade.TradeTerminals.Simulate;

namespace GS.Trade.TradeTerminals
{
    public class TradeTerminals : Element1<string>, ITradeTerminals
    {
        public Dictionary<string, ITradeTerminal> TradeTerminalCollection { get; set; }
        public QuikTradeTerminalCreator QuikTerminals;

        public IEventLog Evl { get; set; }

        public override string Key
        {
            get { return FullName + "@" + (Code.HasValue() ? Code : "Terminals"); }
        }

        public TradeTerminals()
        {
            QuikTerminals =  QuikTradeTerminalCreator.GetInstance;
            TradeTerminalCollection =  new Dictionary<string, ITradeTerminal>();
        }
        private void AddTradeTerminal( string key, ITradeTerminal tt)
        {
            if (tt == null || string.IsNullOrWhiteSpace(key)) return;
            TradeTerminalCollection.Add(key.Trim().ToUpper(), tt);
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName, tt.Name, "TradeTerminal Register()",
                    tt.ShortName + "; Count()="+TradeTerminalCollection.Count, tt.ToString());
        }
        // for TradeContext2
        public ITradeTerminal RegisterTradeTerminal(string type, string key,
                                IOrders orders, ITrades trades, IEventLog evl, ITradeContext tx)
        {
            ITradeTerminal tt;
            var upKey = (type.Trim() + key.Trim()).ToUpper();
            if (TradeTerminalCollection.TryGetValue(upKey, out tt)) return tt;

            switch (type.Trim().ToUpper())
            {
                case "QUIK":
                    var qt = QuikTerminals.GetQuikTradeTerminalOrNew(key);
                    //qt.Parent = this;
                    qt.Code = type;
                    qt.Name = "TradeTerminal: " + key;
                    qt.Init(orders, trades, evl, tx, this);
                    tt = qt;
                    break;
                case "SMARTCOM":
                    break;
                case "SIMULATOR":
                    tt = new SimulateTerminal(type, key, orders, trades, tx);
                    break;
            }
            AddTradeTerminal(upKey, tt);
            return tt;
        }
        public ITradeTerminal RegisterTradeTerminal(string type, string key, IOrders orders, IOrders orders2,
                                                                    ITrades trades, IEventLog evl, ITradeContext tx)
        {
            ITradeTerminal tt;
            var upKey = (type.Trim() + key.Trim()).ToUpper();
            if (TradeTerminalCollection.TryGetValue(upKey, out tt)) return tt;

            switch (type.Trim().ToUpper())
            {
                case "QUIK":
                    var qt = QuikTerminals.GetQuikTradeTerminalOrNew(key);
                    //qt.Parent = this;
                    qt.Code = type + "@" + key;
                    qt.Name = type + "@" + key;
                    qt.Init(orders, trades, evl, tx, this);
                    tt = qt;
                    break;
                case "SMARTCOM":
                    break;
                case "SIMULATOR":
                    tt = new SimulateTerminal(type, key, orders2, trades, tx) {Parent = this};
                    break;
            }
            AddTradeTerminal(upKey, tt);
            return tt;
        }
        
        public void CheckConnection()
        {
            foreach (var t in TradeTerminalCollection.Values)
            {
                var c = t.IsConnected();
                if (c > 0) continue;
                switch (c)
                {
                    case -1:
                        Evlm(EvlResult.WARNING, EvlSubject.DIAGNOSTIC, "TradeTerminals", t.Code,
                             "Check Connection", "Trade Server is NOT Connected. IsConnectedNow: " + t.IsConnectedNow, t.ToString());
                        break;
                    case -2:
                        Evlm(EvlResult.WARNING, EvlSubject.DIAGNOSTIC, "TradeTerminals", t.Code,
                             "Check Connection", "DLL is NOT Connected. IsConnectedNow: " + t.IsConnectedNow, t.ToString());
                        t.Connect();
                        break;
                    case 0:
                        Evlm(EvlResult.FATAL, EvlSubject.DIAGNOSTIC, "TradeTerminals", t.Code,
                             "Check Connection", "Unknown Connection State. IsConnectedNow: " + t.IsConnectedNow, t.ToString());
                        t.Connect();
                        break;
                }
                //FireChangedEvent("ORDERS", "ORDER.TRANSREPLY", "Test","TestObject");
                //FireChangedEvent("ORDERS", "ORDER.STATUSCHAHGED", "Test", "TestObject");
            }
        }
        public void DisConnect()
        {
            foreach (var t in TradeTerminalCollection.Values)
                t.DisConnect();
        }

        public void Start()
        {
            foreach (var t in TradeTerminalCollection.Values)
                t.Start();
        }

        public void Stop()
        {
            foreach (var t in TradeTerminalCollection.Values)
                t.Stop();
        }


        // DeQueueOrderProcess
        public void DeQueueProcess()
        {
            foreach (var t in TradeTerminalCollection.Values.OfType<IHaveQueue>()) //.Where(i => i is IHaveQueue))
            {
                ((IHaveQueue)t).DeQueueProcess();
            }
        }
        public void OrderResolveProcess()
        {
            foreach (var t in TradeTerminalCollection.Values.OfType<INeedOrderResolve>())  //.Where(i => i is INeedOrderResolve))
            {
                ((INeedOrderResolve)t).OrderResolveProcess();
            }
        }
        public void TradeResolveProcess()
        {
            foreach (var t in TradeTerminalCollection.Values.OfType<INeedOrderResolve>())  //.Where(i => i is INeedOrderResolve))
            {
                ((INeedOrderResolve)t).TradeResolveProcess();
            }
        }

        public ISimulateTerminal GetSimulateTerminal()
        {
            return TradeTerminalCollection.Values.OfType<SimulateTerminal>().FirstOrDefault();
        }
        public IEnumerable<ISimulateTerminal> GetSimulateTerminals()
        {
            return TradeTerminalCollection.Values.OfType<SimulateTerminal>();
        } 
    }
}
