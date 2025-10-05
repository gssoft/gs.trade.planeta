using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using GS.Interfaces;
using GS.Status;
using GS.Trade.Trades.Orders2;

namespace GS.Trade.Trades.Orders3
{
    public class Orders3 : Containers5.ListContainer<string, IOrder3>, IOrders3
    {
        private IEventLog _evl;

        public string EventCategory = "UI.Orders";
        public string EventEntity = "Order";

        public string OperationAdd = "Add";
        public string OperationDel = "Delete";
        public string OperationUpd = "Update";

        public TimeSpan SendedAndActvateOrCancelTimeOut = new TimeSpan(0,5,0);

        public void Init(IEventLog evl)
        {
            _evl = evl;
        }
        public IOrder3 CreateOrder( IStrategy strategy, OrderOperationEnum operation, OrderTypeEnum ordertype,
                            double stopprice, double limitprice, long quantity, string comment)
        {
            if ((operation != OrderOperationEnum.Buy && operation != OrderOperationEnum.Sell) ||
                            ordertype == OrderTypeEnum.Unknown)
                return null;

            var cdt = DateTime.Now;
            var neworder = new Order3
            {
                Created = cdt,
                Registered = cdt,
                Strategy = strategy,
                Operation = operation,
                OrderType = ordertype,
                StopPrice = stopprice,
                LimitPrice = limitprice,
                Quantity = quantity,
                Rest = quantity,
                BusyStatus = BusyStatusEnum.ReadyToUse,
                Status = OrderStatusEnum.Registered,
                TransactionAction = OrderTransactionActionEnum.SetOrder,
                TransactionStatus = OrderStatusEnum.Registered,
                ErrorMsg = OrderErrorMsg.Empty,
                SendTimeOutDT = DateTime.MaxValue,
                Suid = Guid.NewGuid().ToString()
            };
            return neworder;
            //_eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
            //                  "Order", "Register New", neworder.ToString(), "");
        }

        public void RegisterOrder(IOrder3 ord)
        {
            if(ord != null)
                Add(ord);
        }

        public IOrder3 RegisterOrder(//long number, DateTime dt, long transId,
            //string account, string strategy, string ticker,
            IStrategy strategy,
            OrderOperationEnum operation, OrderTypeEnum ordertype, double stopprice, double limitprice, long quantity,
            string comment)
        {
            if ((operation == OrderOperationEnum.Buy || operation == OrderOperationEnum.Sell) && ordertype != OrderTypeEnum.Unknown)
            {
                var cdt = DateTime.Now;
                var neworder = new Order3
                {
                    Created = cdt,
                    Registered = cdt,
                    Strategy = strategy,
                    Operation = operation,
                    OrderType = ordertype,
                    StopPrice = stopprice,
                    LimitPrice = limitprice,
                    Quantity = quantity,
                    Rest = quantity,
                    BusyStatus = BusyStatusEnum.ReadyToUse,
                    Status = OrderStatusEnum.Registered,
                    TransactionAction = OrderTransactionActionEnum.SetOrder,
                    TransactionStatus = OrderStatusEnum.Registered,
                    ErrorMsg = OrderErrorMsg.Empty,
                    SendTimeOutDT = DateTime.MaxValue,
                    Suid = Guid.NewGuid().ToString()
                };
                var b = Add(neworder);
                if (b)
                {
                    neworder.Status = OrderStatusEnum.Registered;
                    neworder.ErrorMsg = OrderErrorMsg.Ok;
                    //var o = neworder.Clone();
                    //o?.Strategy?.FireOrderChangedEventToStrategy(o, "ORDERS", "ORDER.REGISTER", "REGISTERED");
                }
                else
                {
                    neworder.Status = OrderStatusEnum.NotRegistered;
                    neworder.ErrorMsg = OrderErrorMsg.AddToListFailure;
                    
                    //var o = neworder.Clone();
                    //o?.Strategy?.FireOrderChangedEventToStrategy(o, "ORDERS", "ORDER.REGISTER", "NOTREGISTERED");
                }
                return neworder;
            }
            else
            {
                //_eventLog.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                //                  "Order", "Register New", operation.ToString(), ordertype.ToString());
            }
            return null;
        }

        public void GetOrders(OrderTypeEnum orderType, OrderOperationEnum operation, OrderStatusEnum orderStatus,
            string tradeKey, IList<IOrder3> ol)
        {
            throw new NotImplementedException();
        }

        public bool IsOrderValidExist(OrderTypeEnum orderType, OrderOperationEnum operation, double price)
        {
            return Items.Any(o => o.IsValid
                                  && o.OrderType == orderType
                                  && o.Operation == operation
                                  && o.LimitPrice.Equals(price)
                );
        }
        public bool IsOrderValidSoftExist(OrderTypeEnum orderType, OrderOperationEnum operation, double price)
        {
            return Items.Any(o => o.IsValidSoft
                                  && o.OrderType == orderType
                                  && o.Operation == operation
                                  && o.LimitPrice.Equals(price)
                );
        }
        public IOrder3 CreateOrder( IStrategy strategy, ulong number, DateTime dt, 
                                        OrderOperationEnum operation, OrderTypeEnum ordertype, OrderStatusEnum status,
                                        double stopprice, double limitprice, long quantity, string comment)
        {
            if ((operation == OrderOperationEnum.Buy || operation == OrderOperationEnum.Sell) &&
                ordertype != OrderTypeEnum.Unknown)
            {
                var neworder = new Order3
                {
                    Number = number,
                    DateTime = dt,
                    Registered = DateTime.Now,
                    Strategy = strategy,
                    Operation = operation,
                    OrderType = ordertype,
                    StopPrice = stopprice,
                    LimitPrice = limitprice,
                    Quantity = quantity,
                    Rest = quantity,
                    Status = status
                };
                return neworder;
            }
            return null;
        }

        public override bool Add(IOrder3 o)
        {
            if (o == null) throw new ArgumentNullException(nameof(o));

            //var ord = Items.FirstOrDefault(i => i.Number == o.Number);
            //if (ord != null)
            //    return false;
            //var ord = GetByKey(o.Key);
            //if (ord != null)
            //    return false;
            if (Contains(o))
                return false;

            if (!base.Add(o))
                return false;

            FireContainerEvent(new Events.EventArgs
            {
                Category = EventCategory,
                Entity = EventEntity,
                Operation = OperationAdd,
                Object = o
            });
            return true;
        }
        public override bool Remove(IOrder3 o)
        {
            if (!base.Remove(o.Key))
                return false;
            FireContainerEvent(new Events.EventArgs
            {
                Category = EventCategory,
                Entity = EventEntity,
                Operation = OperationDel,
                Object = o
            });
            return true;
        }
        public void GetFireEvent(object sender, Events.IEventArgs args)
        {
            //_evl.AddItem(EvlResult.INFO, EvlSubject.TECHNOLOGY, "Orders2", "Orders2", "GetFireEvent()",
            //    args.OperationKey, args.Object.GetType().ToString());

            var t = args.Object as IOrder3;
            if (t == null)
                return;
            switch (args.OperationKey)
            {
                case "ORDER.STATUS.CONFIRMED":
                case "ORDERS.ORDER.ADD":
                    Add(t);
                    break;
                case "ORDERS.ORDER.DELETE":
                    Remove(t);
                    break;
                case "ORDERS.ORDER.UPDATE":
                    Update(t.Key, t);
                    break;
            }
        }
        public IEnumerable<IOrder3> ActiveOrders => Items.Where(o => o.IsActive);
        public IEnumerable<IOrder3> ActiveOrdersSoft => Items.Where(o => o.IsActiveSoft);
        public IEnumerable<IOrder3> ActiveOrdersSoftInUse => Items
                .Where(o => o.IsActiveSoft && o.BusyStatus == BusyStatusEnum.InUse);
        public IEnumerable<IOrder3> ActiveOrdersSoftReadyToUse => Items
                .Where(o => o.IsActiveSoft && o.BusyStatus == BusyStatusEnum.ReadyToUse);
        public bool IsAnyAciveOrders => ActiveOrders.Any();
        public IEnumerable<IOrder3> ClosedOrders => Items.Where(o => o.IsClosed);
        public IEnumerable<IOrder3> ClosedOrdersSoft => Items.Where(o => o.IsClosedSoft);
        public IEnumerable<IOrder3> ValidOrders => Items.Where(o => o.IsValid);
        public IEnumerable<IOrder3> ValidOrdersSoft => Items.Where(o => o.IsValidSoft);
        public IEnumerable<IOrder3> OrdersRegistered => Items.Where(o => o.IsRegistered);
        public IEnumerable<IOrder3> OrdersRegisteredInUse => Items
                .Where(o => o.IsRegistered && o.BusyStatus == BusyStatusEnum.InUse);
        public IEnumerable<IOrder3> OrdersRegisteredReadyToUse => Items
                .Where(o => o.IsRegistered && o.BusyStatus == BusyStatusEnum.ReadyToUse);

        public bool IsAnyValidOrders => ValidOrders.Any();

        //public bool Remove2(IOrder3 t)
        //{
        //    lock (((ICollection)ItemCollection).SyncRoot)
        //    {
        //        return t != null && ItemCollection.Remove(t);
        //    }
        //}
        public void Clear()
        {
            var m = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            foreach (var o in Items)
            {
                var r = RemoveNoKey(o);               
            }
        }
    }
}
