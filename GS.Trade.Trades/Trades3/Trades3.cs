using System;
using System.Collections.Generic;
using System.Linq;
using GS.Interfaces;

namespace GS.Trade.Trades.Trades3
{
    public class Trades3 : Containers5.ListContainer<string, ITrade3>, ITrades3
    {
        private IEventLog _evl;

        public string EventCategory = "Trades";
        public string EventEntity = "Trade";

        public string OperationAdd = "Add";
        public string OperationDel = "Delete";
        public string OperationUpd = "Update";

        public void Init(IEventLog evl)
        {
            _evl = evl;
        }

        public IEnumerable<ITrade3> TradeRegistered => Items.Where(t => t.Status == TradeStatusEnum.Registered);
        public IEnumerable<ITrade3> TradeUnknown => Items.Where(t => t.Status == TradeStatusEnum.Unknown);
        public IEnumerable<ITrade3> TradeConfirmed => Items.Where(t => t.Status == TradeStatusEnum.Confirmed);
        // public IEnumerable<ITrade3> TradeToProcess => Items.Where(t => t.Status == TradeStatusEnum.ToProcess);
        public IEnumerable<ITrade3> TradeToResolve => Items.Where(t => t.Status == TradeStatusEnum.ToResolve);

        public ITrade3 RegisterTrade(
            IStrategy strategy,
            TradeOperationEnum operation, double stopprice, double limitprice, long quantity,
            string comment)
        {
            if ((operation == TradeOperationEnum.Buy || operation == TradeOperationEnum.Sell))
            {
                var newTrade = new Trade3
                {
                    Registered = DateTime.Now,
                    Strategy = strategy,
                    Operation = operation,
                    Quantity = quantity,
                    Status = TradeStatusEnum.Registered
                };

                Add(newTrade);
                return newTrade;

                //_eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
                //                  "Order", "Register New", neworder.ToString(), "");
            }

            // Bad Syntax Operaton is Unknown
            return null;
        }

        public override bool Add(ITrade3 t)
        {
            if (!base.Add(t))
                return false;

            FireContainerEvent(new Events.EventArgs
            {
                Category = EventCategory,
                Entity = EventEntity,
                Operation = OperationAdd,
                Object = t
            });
            return true;
        }
        public override bool Remove(ITrade3 t)
        {
            if (!base.Remove(t.Key))
                return false;
            FireContainerEvent(new Events.EventArgs
            {
                Category = EventCategory,
                Entity = EventEntity,
                Operation = OperationDel,
                Object = t
            });
            return true;
        }
       
        public void GetFireEvent(object sender, Events.IEventArgs args)
        {
            _evl.AddItem(EvlResult.INFO, EvlSubject.TECHNOLOGY, "Trades3", "Trades3", "GetFireEvent()",
                args.OperationKey, args.Object.GetType().ToString());

            var t = args.Object as ITrade3;
            if (t == null)
                return;
            switch (args.OperationKey)
            {
                case "TRADE.STATUS.CONFIRMED":
                case "TRADES.TRADE.ADD":
                    Add(t);
                    break;
                case "TRADES.TRADE.DELETE":
                    Remove(t);
                    break;
                case "TRADES.TRADE.UPDATE":
                    Update(t.Key, t);
                    break;
            }
        }

    }
}