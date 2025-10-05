using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers1;
using GS.Containers3;
using GS.Events;
using GS.Interfaces;

namespace GS.Trade.Trades.Orders2
{
    //public class Orders2 : ListContainer<string> , IOrders
    //{
    //    private IEventLog _evl;

    //    public string EventCategory = "UI.Orders";
    //    public string EventEntity = "Order";
        
    //    public string OperationAdd = "Add";
    //    public string OperationDel = "Delete";
    //    public string OperationUpd = "Update";

    //    public void Init(IEventLog evl)
    //    {
    //        _evl = evl;
    //    }

    //    public IOrder RegisterOrder(//long number, DateTime dt, long transId,
    //        //string account, string strategy, string ticker,
    //        IStrategy strategy,
    //        OperationEnum operation, OrderTypeEnum ordertype, double stopprice, double limitprice, long quantity,
    //        string comment)
    //    {
    //        if ((operation == OperationEnum.Buy || operation == OperationEnum.Sell) && ordertype != OrderTypeEnum.Unknown)
    //        {
    //            var neworder = new Order2
    //            {
    //                Registered = DateTime.Now,
    //                Strategy = strategy,
    //                Operation = operation,
    //                OrderType = ordertype,
    //                StopPrice = stopprice,
    //                LimitPrice = limitprice,
    //                Quantity = quantity,
    //                Rest = quantity,
    //                Status = OrderStatusEnum.Registered
    //            };
    //            Add(neworder);
    //            return neworder;

    //            //_eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
    //            //                  "Order", "Register New", neworder.ToString(), "");
    //        }
    //        else
    //        {
    //            //_eventLog.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
    //            //                  "Order", "Register New", operation.ToString(), ordertype.ToString());
    //        }
    //        return null;
    //    }

    //    public override bool Add(IContainerItem<string> t)
    //    {
    //        if(!base.Add(t))
    //            return false;

    //        OnContainerEvent(new Events.EventArgs
    //        {
    //            Category = EventCategory,
    //            Entity = EventEntity,
    //            Operation = OperationAdd,
    //            Object = t as IOrder
    //        });
    //        return true;
    //    }
    //    public override bool Remove(IContainerItem<string> t)
    //    {
    //        if (!base.Remove(t.Key))
    //            return false;
    //        OnContainerEvent(new Events.EventArgs
    //        {
    //            Category = EventCategory,
    //            Entity = EventEntity,
    //            Operation = OperationDel,
    //            Object = t as IOrder
    //        });
    //        return true;
    //    }
    //    public void GetFireEvent(object sender, Events.EventArgs args)
    //    {
    //        //_evl.AddItem(EvlResult.INFO, EvlSubject.TECHNOLOGY, "Orders2", "Orders2", "GetFireEvent()",
    //        //    args.OperationKey, args.Object.GetType().ToString());

    //        var t = args.Object as IOrder;
    //        if (t == null)
    //            return;
    //        switch (args.OperationKey)
    //        {
    //            case "ORDER.STATUS.CONFIRMED":
    //            case "ORDERS.ORDER.ADD":
    //                Add(t);
    //                break;
    //            case "ORDERS.ORDER.DELETE":
    //                Remove(t);
    //                break;
    //            case "ORDERS.ORDER.UPDATE":
    //                Update(t.Key, t);
    //                break;
    //        }
    //    }

    //    public IEnumerable<Containers3.IContainerItem<string>> ActiveOrders {
    //        get
    //        {
    //            var os = Items.Where(o => ((IOrder)o).IsActive);
    //            return os;
    //        }
    //    }

    //    public IEnumerable<Containers3.IContainerItem<string>> ClosedOrders
    //    {
    //        get
    //        {
    //            return Items.Where(o => ((IOrder)o).IsClosed);
    //        }
    //    }
    //}

  
}
