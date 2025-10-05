using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers3;
using GS.Interfaces;

namespace GS.Trade.Trades.Trades2
{
    public class Trades2 : ListContainer<string>, ITrades3
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

        public ITrade2 RegisterTrade(//long number, DateTime dt, long transId,
            //string account, string strategy, string ticker,
            IStrategy strategy,
            TradeOperationEnum operation, OrderTypeEnum ordertype, double stopprice, double limitprice, long quantity,
            string comment)
        {
            if ((operation == TradeOperationEnum.Buy || operation == TradeOperationEnum.Sell) && ordertype != OrderTypeEnum.Unknown)
            {
                var newTrade = new Trade2
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
            else
            {
                //_eventLog.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                //                  "Order", "Register New", operation.ToString(), ordertype.ToString());
            }
            return null;
        }

        public bool Add(ITrade2 t)
        {
            if (!base.Add(t))
                return false;

            OnContainerEvent(new Events.EventArgs
            {
                Category = EventCategory,
                Entity = EventEntity,
                Operation = OperationAdd,
                Object = t
            });
            return true;
        }
        public override bool Remove(IContainerItem<string> t)
        {
            if (!base.Remove(t.Key))
                return false;
            OnContainerEvent(new Events.EventArgs
            {
                Category = EventCategory,
                Entity = EventEntity,
                Operation = OperationDel,
                Object = t as IOrder
            });
            return true;
        }

        public override Events.EventArgs GetEventArgs()
        {
            return new Events.EventArgs
            {
                Category = "Orders",
                Entity = "Orders"
            };
        }

        public void GetFireEvent(object sender, Events.EventArgs args)
        {
            _evl.AddItem(EvlResult.INFO, EvlSubject.TECHNOLOGY, "Trades2", "Trades2", "GetFireEvent()",
                args.OperationKey, args.Object.GetType().ToString());

            var t = args.Object as ITrade2;
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

        public bool Add(ITrade3 t)
        {
            throw new NotImplementedException();
        }
    }
}
