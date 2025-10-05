using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.Trades
{
    //public class OrderFiller
    //{
    //    private Orders _orders;
    //    private Trades _trades;

    //    public OrderFiller( Orders orders, Trades trades)
    //    {
    //        _orders = orders;
    //        _trades = trades;
    //    }
    //    public void NewTick( DateTime dt, string tickerkey, double price)
    //    {

    //        // ***********   Cancell Expired
    //        foreach (var o in _orders.OrderCollection.Where(o => o.IsActive && o.ExpireDate.CompareTo(dt) <= 0))
    //        {
    //            _orders.CancelOrder(o.Number);
    //        }

    //        // ***************************** Fill Limit *********************************

    //        var oo = from o in _orders.OrderCollection
    //                 where o.Ticker == tickerkey &&
    //                       o.IsActive &&
    //                       o.IsLimit
    //                 select o;

    //        foreach (var order in oo)
    //        {
    //            if (order.IsBuy &&
    //                order.LimitPrice.CompareTo(price) >= 0)
    //            {
    //                order.Status = OrderStatusEnum.Filled;
    //                order.CancelTime = DateTime.Now.TimeOfDay;
    //                _orders.SetNeedToObserver();

    //                _trades.NewTrade(order.Number, DateTime.Now,
    //                                 order.Account, order.Strategy, order.Ticker,
    //                                 TradeOperationEnum.Buy, (int) order.Quantity, price, "", order.Number, 0.01);

    //            }
    //            else if (order.IsSell &&
    //                     order.LimitPrice.CompareTo(price) <= 0)
    //            {
    //                order.Status = OrderStatusEnum.Filled;
    //                order.CancelTime = DateTime.Now.TimeOfDay;
    //                _orders.SetNeedToObserver();

    //                _trades.NewTrade(order.Number, DateTime.Now,
    //                                 order.Account, order.Strategy, order.Ticker,
    //                                 TradeOperationEnum.Sell, (int) order.Quantity, price, "", order.Number, 0.01);
    //            }
    //        }

    //        // ****************** Fill Stop Orders ***********************

    //        foreach (var o in _orders.OrderCollection.Where(o =>
    //                                                        o.Ticker == tickerkey &&
    //                                                        o.IsActive &&
    //                                                        o.IsStopLimit))
    //        {
    //            if( o.IsBuy && o.StopPrice.CompareTo(price) <= 0)
    //            {
    //                //_orders.CancelOrder(o.Number);
    //                o.SetStatus(OrderStatusEnum.Canceled, "BuyStop Filled="+ price);

    //                _orders.RegisterOrder(0, DateTime.Now, 0, o.Account, o.Strategy, o.Ticker, o.Operation, OrderTypeEnum.Limit, 0,
    //                          o.LimitPrice, o.Quantity, o.Quantity, OrderStatusEnum.Activated,
    //                          dt.TimeOfDay, TimeSpan.MinValue, DateTime.Today.Add(new TimeSpan(23, 49, 15)), "");
    //            }
    //            else if ( o.IsSell && o.StopPrice.CompareTo(price) >= 0 )
    //            {
    //                //_orders.CancelOrder(o.Number);

    //                _orders.RegisterOrder(0, DateTime.Now, 0, o.Account, o.Strategy, o.Ticker, o.Operation, OrderTypeEnum.Limit, 0,
    //                          o.LimitPrice, o.Quantity, o.Quantity, OrderStatusEnum.Activated,
    //                          dt.TimeOfDay, TimeSpan.MinValue, DateTime.Today.Add(new TimeSpan(23, 49, 15)), "");
    //            }
    //        }

    //    }
    //}
}
