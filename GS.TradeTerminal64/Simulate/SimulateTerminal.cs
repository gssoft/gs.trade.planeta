using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GS.Elements;
using GS.Events;
using GS.Interfaces;
using GS.Extension;
using GS.Trade.Interfaces;
using GS.Trade.Trades;
using GS.Trade.Trades.Orders3;
using GS.Trade.Trades.Trades3;
using EventArgs = GS.Events.EventArgs;

namespace GS.Trade.TradeTerminals64.Simulate
{
    public class SimulateTerminal : Element1<string>, ITradeTerminal, ISimulateTerminal
    {
        public override string Key => Type + "@" + Code;
        public event EventHandler<Events.IEventArgs> TradeEntityChangedEvent;

        //protected virtual void OnTradeEntityChangedEvent(IEventArgs e)
        //{
        //    EventHandler<IEventArgs> handler = TradeEntityChangedEvent;
        //    if (handler != null) handler(this, e);
        //}

        public delegate void NewOrderStatusEventHandler();
        public event NewOrderStatusEventHandler NewOrderStatusEvent;

        private void InvokeNewOrderStatusEvent()
        {
            NewOrderStatusEvent?.Invoke();
        }

        //public delegate void NewTradeEventHandler();
        //public event NewTradeEventHandler NewTradeEvent;

        private readonly ITradeContext _tx;

        public TradeTerminalType Type { get; private set; }

        private readonly IOrders _orders;
        //private Trades.Trades _trades;

        private static int _number;

        public Orders3 OrdersActivated { get; set; }
        public Orders3 OrdersCompleted { get; set; }

        public List<IOrder3> FillerCollection;
        private readonly object _fillerLocker;

        public BackTestOrderExecutionMode BackOrderExecMode { get; set; }

        public SimulateTerminal(ITradeContext tx)
        {
            _tx = tx;

            FillerCollection = new List<IOrder3>();
            _fillerLocker = new object();

           // 15.04.2018
           // BackOrderExecMode = BackTestOrderExecutionMode.Optimistic;
           BackOrderExecMode = BackTestOrderExecutionMode.Pessimistic;

            //if (TradeEntityChangedEvent != null)
            //    OrdersActivated = new Orders3
            //    {
            //        ExternalContainerEvent = TradeEntityChangedEvent
            //    };
            OrdersActivated = new Orders3();
            OrdersCompleted = new Orders3();
        }

        public SimulateTerminal(string code, string name, IOrders oo, ITrades tt, ITradeContext tx)
        {
            Name = name;
            Code = code;
            IsConnectedNow = true;
            _orders = oo;
            Type = TradeTerminalType.Simulator;
            // _trades = tt;

            FillerCollection = new List<IOrder3>();
            _fillerLocker = new object();

            BackOrderExecMode = BackTestOrderExecutionMode.Optimistic;

            OrdersActivated = new Orders3();
            OrdersCompleted = new Orders3();

            _tx = tx;
        }
        public override void Init()
        {
        }
        public void SetEventLog(IEventLog evl)
        {
        }

        public void Start()
        {
        }
        public void Stop()
        {
        }

        public bool IsConnectedNow { get; private set; }

        public bool Connect()
        {
            return true;
        }
        public bool DisConnect()
        {
            return true;
        }

        public bool IsWellConnected => true;

        public int IsConnected()
        {
            return +1;
        }
        public long SetMarketOrder(string account, string strategy, string tickercategory, string tickercode,
            OrderOperationEnum operation, double price, long quantity)
        {
            var n = (ulong)GetUnicID();
            var dt = DateTime.Now;
            var o = new Order(n, dt, 1, account, strategy, tickercode,
                operation, OrderTypeEnum.Market, 0,
                price, (int)quantity, (int)quantity, OrderStatusEnum.Activated,
                n, "", DateTime.Now.TimeOfDay, DateTime.Now.TimeOfDay);
            return (long)n;
        }

        public long SetLimitOrder(IOrder3 o)
        {
            if (o == null) throw new ArgumentNullException("o", "SimulateTerminal.SetLimitOrder(o)");

            var n = (ulong)GetUnicID();
            //_orders.RegisterOrder(o.Number, dt, o.Number, o.Strategy.Account.Code, o.Strategy.Code, o.Strategy.Ticker.Code,
            //                  o.Operation, OrderTypeEnum.Limit, 0,
            //                  o.LimitPrice, o.Quantity, o.Quantity, OrderStatusEnum.Activated,
            //                  dt.TimeOfDay, TimeSpan.MinValue, DateTime.Today.Add(new TimeSpan(23, 49, 15)), "");
            
            o.SetStatus(OrderStatusEnum.Activated,"");
            o.TransId = (uint)n;
            o.Number = n;

            OrdersActivated.Add(o);
            NotifyActiveOrder(o, "ADD");
            return +1;
        }

        public long SetLimitOrder(string account, string strategy, string tickercategory, string tickercode,
                                    OrderOperationEnum operation, double price, long quantity,
                                        DateTime expireDateTime, string comment)
        {
            var n = (long)GetUnicID();
            //_orders.RegisterOrder(n, dt, n, account, strategy, tickercode, operation, OrderTypeEnum.Limit, 0,
            //                  price, quantity, quantity, OrderStatusEnum.Activated, 
            //                  dt.TimeOfDay, TimeSpan.MinValue, expireDateTime, comment);
            return n;
        }
        // Current
        public long SetLimitOrder(IStrategy stra, string account, string strategy, string tickercategory, string tickercode,
                                    OrderOperationEnum operation, double price, string priceStr, long quantity,
                                        DateTime expireDateTime, string comment)
        {
            var n = (long)GetUnicID();
            //_orders.RegisterOrder(n, dt, n, account, strategy, tickercode, operation, OrderTypeEnum.Limit, 0,
            //                  price, quantity, quantity, OrderStatusEnum.Activated,
            //                  dt.TimeOfDay, TimeSpan.MinValue, expireDateTime, comment);
            return n;
        }

        public long SetLimitOrderInQueue(string account, string strategy, string tickercategory, string tickercode,
                                         OrderOperationEnum operation, double price, string priceStr, long quantity, DateTime expireDateTime,
                                         string comment)
        {
            return SetLimitOrder(null, account, strategy, tickercategory, tickercode,
                                 operation, price, priceStr, quantity,
                                 expireDateTime, comment);
        }

        public long SetMarketOrLimitOrder(string account, string strategy, string tickercategory, string tickercode,
                                            OrderTypeEnum ordertype, OrderOperationEnum operation, double price, long quantity,
                                             DateTime expireDateTime, string comment)
        {
            var n = (long)GetUnicID();
            // var dt = DateTime.Now;

            //_orders.RegisterOrder(n, dt, n, account, strategy, tickercode, operation, ordertype, 0,
            //                  price, quantity, quantity, OrderStatusEnum.Activated,
            //                  dt.TimeOfDay, TimeSpan.MinValue, expireDateTime, "");

            //var o = new Order(n, dt, 1, account, strategy, tickercode, operation, ordertype, 0,
            //                  price, (int)quantity, (int)quantity, Order.OrderStatusEnum.Active, n, "", DateTime.Now.TimeOfDay, DateTime.Now.TimeOfDay);
            return n;
        }

        public long SetStopLimitOrder(string account, string strategy, string tickercategory, string tickercode,
                                        OrderOperationEnum operation, double stopprice, double price, long quantity,
                                        DateTime expireDateTime, string comment)
        {
            var n = (long)GetUnicID();
            // var dt = DateTime.Now;
            
            //_orders.RegisterOrder(n, dt, n, account, strategy, tickercode, operation, OrderTypeEnum.StopLimit, stopprice,
            //                  price, quantity, quantity, OrderStatusEnum.Activated,
            //                  dt.TimeOfDay, TimeSpan.MinValue, expireDateTime, comment);
            // var o = new Order(n, dt, 1, account, strategy, tickercode, operation, Order.OrderTypeEnum.StopLimit, stopprice,
            //                  price, (int)quantity, (int)quantity, Order.OrderStatusEnum.Active, n, "", DateTime.Now.TimeOfDay, DateTime.Now.TimeOfDay);
            return n;
        }

        public long SetMarketOrLimitWithStopLimit(string Account, string StratName, string ClassCode, string Code, OrderTypeEnum ordertype,
            OrderOperationEnum operation, double entryPrice, double stopPrice, double limitPrice, long Quantity, ref long limitNumber, ref long stopLimitNumber)
        {
            throw new NotImplementedException();
        }

        public long SetStopLimitWithLinkedProfitOrder(string Account, string StratName, string ClassCode, string Code,
            OrderOperationEnum operation, double stopPrice, double limitPrice, double takeProfit, long Quantity, DateTime expire, string comment)
        {
            throw new NotImplementedException();
        }

        public int SetKillLimitOrderInQueue(string classcode, string seccode, ulong orderkey)
        {
            return KillLimitOrder(null, classcode, seccode, orderkey);
        }

        public int KillLimitOrder(IOrder3 o)
        {
            //_orders.CancelOrder(o.Number);
            o.SetStatus(OrderStatusEnum.Canceled, "");
            
            OrdersActivated.Remove(o);
            NotifyActiveOrder(o, "DELETE");

            _tx.Save(o);
            return 0;
        }

        public int KillLimitOrder(IOrder3 o, IStrategy stra, string tickercategory, string tickercode, ulong orderkey)
        {
            //_orders.CancelOrder(orderkey);
            return 0;
        }

        public int KillLimitOrder(IStrategy stra, string tickercategory, string tickercode, ulong orderkey)
        {
            //_orders.CancelOrder(orderkey);
            return 0;
        }
        public int KillLimitOrderByTransID(string tickercategory, string tickercode, ulong orderTransID)
        {
            //_orders.CancelOrderByTransID(orderTransID);
            return 0;
        }

        public bool KillStopOrder(string tickercategory, string tickercode, ulong orderkey)
        {
            //_orders.CancelOrder(orderkey);
            return true;
        }

        public bool KillAllOrders(IStrategy stra, string strategy, string account, string tickercategory, string tickercode,
                                    OrderTypeEnum ordtype, OrderOperationEnum operation)
        {
            //_orders.CancelOrders(strategy, account, tickercategory, tickercode, ordtype, operation);
            return true;
        }

        public bool KillAllFuturesLimitOrders(string account, string tickercategory, string tickercode, string basecontract)
        {
            throw new NotImplementedException();
        }

        public void NewTick( DateTime dt, string tickerkey, double quote )
        {
            //var mydt = dt;
            //var ticker = tickerkey;
            //var q = quote;
        }

        /*
        private static long GetUnicID2()
        {
            var dt = DateTime.Now;
            if( _number < 999) ++ _number;
            else _number = 0;
            var lo = (long)(_number) + (long)(dt.Millisecond) * 1000 + (long)(dt.Second) * 1000 * 1000 + (long)(dt.Minute) * 1000 * 1000 * 100 + (long)(dt.Hour) * 1000 * 1000 * 100 * 100;
            return lo;
        }
         */
        private static long GetUnicID()
        {
            var dt = DateTime.Now;
            if (_number < 999) ++_number;
            else _number = 0;

            var s = dt.Month.ToString("00") +
                    dt.Day.ToString("00") +
                    dt.Hour.ToString("00") +
                    dt.Minute.ToString("00") +
                    dt.Second.ToString("00") +
                    dt.Millisecond.ToString("000") +
                    _number.ToString("000");

            return long.Parse(s);
        }
        public override string ToString()
        {
            return $"[Type: {GetType().FullName}; Code: {Code}; Name: {Name}]";
        }

        public void SendOrdersFromQueue()
        {
            
        }

        public void SendOrderTransactionsFromQueue()
        {
            throw new NotImplementedException();
        }

        public void SendOrderTransactionsFromQueue3()
        {
            throw new NotImplementedException();
        }

        public void SendOrderTransactionsFromBlQueue()
        {
           // throw new NotImplementedException();
        }

        public IOrder3 GetOrderByKey(string key)
        {
           // throw new NotImplementedException();
            return null;
        }

        public IEnumerable<IOrder3> GetActiveOrders()
        {
            return Enumerable.Empty<IOrder3>();
        }

        public IEnumerable<IOrder3> GetStartegyActiveOrders(string stratkey)
        {
            return Enumerable.Empty<IOrder3>();
        }

        public void TradeProcess2()
        {
            //throw new NotImplementedException();
        }

        public void TradeProcess3()
        {
        }

        public string ShortName => Code + ": " + Name;

        public void ExecuteTick(DateTime dt, string tickerkey, double price, double bid, double ask)
        {
            try
            {
                lock (_fillerLocker)
                {
                    FillerCollection.Clear();
                    FillerCollection.AddRange(OrdersActivated.ActiveOrders.Where(o => o.Ticker.Key == tickerkey));

                    //foreach (var o in OrderCollection.Where(o => o.Ticker == tickerkey))
                    //{
                    //    if (!o.IsActive) continue;
                    //    FillerCollection.Add(o);
                    //}
                }
                lock (_fillerLocker)
                {
                    //    }
                    // ***********   Cancell Expired *******************************
                    /*
                    foreach (var o in FillerCollection.Where(o => o.IsActive && o.ExpireDate.CompareTo(dt) <= 0))
                    {
                        o.SetStatus(OrderStatusEnum.Cancel, "Expired: " + o.ExpireDateString);
                    }
                */
                    // *************** Fill Limit Orders ****************** 

                    foreach (var order in FillerCollection.Where(o => o.IsLimit))
                    {
                        if (!order.IsActive) continue;
                        if (BackOrderExecMode == BackTestOrderExecutionMode.Optimistic)
                        {
                            if (order.IsBuy && order.LimitPrice.IsGreaterOrEqualsThan(ask))
                            {
                                order.SetStatus(OrderStatusEnum.Filled, "Filled BuyLimit=" + ask);
                                // "Filled BuyLimit=" + order.LimitPrice);
                                OrdersActivated.Remove(order);
                                NotifyActiveOrder(order, "FILLED");

                                var tr = NewTrade(order.Strategy, dt, order.Number, TradeStatusEnum.Confirmed, 
                                    TradeOperationEnum.Buy,
                                    (int)order.Quantity,
                                    (decimal)ask,
                                    // (decimal)order.LimitPrice,
                                    order.Number,
                                    (decimal)0.01);

                                TradeProcess(tr);
                                _tx.Save(order);

                                // InvokeLimitOrderFilledEvent(order);

                            }
                            else if (order.IsSell && order.LimitPrice.IsLessOrEqualsThan(bid))
                            {
                                order.SetStatus(OrderStatusEnum.Filled, "Filled SellLimit=" + bid);
                                //"Filled SellLimit=" + order.LimitPrice);
                                OrdersActivated.Remove(order);
                                NotifyActiveOrder(order, "FILLED");

                                var tr = NewTrade(order.Strategy, dt, order.Number, TradeStatusEnum.Confirmed,
                                   TradeOperationEnum.Sell, (int)order.Quantity,
                                   (decimal)bid,
                                   // (decimal)order.LimitPrice,
                                   order.Number,
                                   (decimal)0.01);

                                TradeProcess(tr);
                                _tx.Save(order);

                                //_trades.NewTrade(order.Number, dt,
                                //    order.Account, order.Strategy, order.Ticker,
                                //    TradeOperationEnum.Sell, (int)order.Quantity, bid, "", order.Number,
                                //    0.01);

                                // InvokeLimitOrderFilledEvent(order);
                            }
                        }
                        else if (BackOrderExecMode == BackTestOrderExecutionMode.Pessimistic)
                        {
                            if (order.IsBuy && order.LimitPrice.IsGreaterThan(ask))
                            {
                                order.SetStatus(OrderStatusEnum.Filled, $"Filled BuyLimit={ask}");
                                   // $"Filled BuyLimit={order.LimitPrice}");
                                OrdersActivated.Remove(order);
                                NotifyActiveOrder(order, "FILLED");

                                var tr = NewTrade(order.Strategy, dt, order.Number, TradeStatusEnum.Confirmed,
                                    TradeOperationEnum.Buy, (int)order.Quantity,
                                    // 15.04.2018
                                    (decimal)ask,
                                    // (decimal)order.LimitPrice,
                                    order.Number,
                                    (decimal)0.01);

                                TradeProcess(tr);
                                _tx.Save(order);

                                //InvokeLimitOrderFilledEvent(order);
                            }
                            else if (order.IsSell && order.LimitPrice.IsLessThan(bid))
                            {
                                order.SetStatus(OrderStatusEnum.Filled,  "Filled SellLimit: " + bid);
                                    // "Filled SellLimit: " + order.LimitPrice);
                                OrdersActivated.Remove(order);
                                NotifyActiveOrder(order, "FILLED");

                                var tr = NewTrade(order.Strategy, dt, order.Number, TradeStatusEnum.Confirmed,
                                   TradeOperationEnum.Sell, (int)order.Quantity,
                                   (decimal)bid,
                                   // (decimal) order.LimitPrice,
                                   order.Number,
                                   (decimal)0.01);

                                TradeProcess(tr);
                                _tx.Save(order);

                                //_trades.NewTrade(order.Number, dt,
                                //    order.Account, order.Strategy, order.Ticker,
                                //    TradeOperationEnum.Sell, (int)order.Quantity, bid, "", order.Number,
                                //    0.01);

                                //InvokeLimitOrderFilledEvent(order);
                            }
                        }
                    }

                    // ********************* Fill StopLimit ***********************

                    foreach (var o in FillerCollection.Where(o => o.IsStopLimit))
                    {
                        if (!o.IsActive) continue;
                        if (o.IsBuy && o.StopPrice.IsLessOrEqualsThan(price))
                        {
                            o.SetStatus(OrderStatusEnum.Filled, "Filled BuyStop=" + price);
                            OrdersActivated.Remove(o);
                            
                            //RegisterOrder(0, dt, 0, o.Account, o.Strategy, o.Ticker, o.Operation,
                            //    OrderTypeEnum.Limit, 0,
                            //    o.LimitPrice, o.Quantity, o.Quantity, OrderStatusEnum.Activated,
                            //    dt.TimeOfDay, TimeSpan.MinValue, DateTime.Today.Add(new TimeSpan(23, 49, 15)),
                            //    "Create from StopOrder:" + o.Number);

                            //InvokeStopOrderFilledEvent(o);
                        }
                        else if (o.IsSell && o.StopPrice.IsGreaterOrEqualsThan(price) )
                        {
                            o.SetStatus(OrderStatusEnum.Filled, "Filled SellStop=" + price);
                            OrdersActivated.Remove(o);

                            //RegisterOrder(0, dt, 0, o.Account, o.Strategy, o.Ticker, o.Operation,
                            //    OrderTypeEnum.Limit, 0,
                            //    o.LimitPrice, o.Quantity, o.Quantity, OrderStatusEnum.Activated,
                            //    dt.TimeOfDay, TimeSpan.MinValue, DateTime.Today.Add(new TimeSpan(23, 49, 15)),
                            //    "Create from StopOrder:" + o.Number);

                            //InvokeStopOrderFilledEvent(o);
                        }
                    }

                }
                //  FillerCollection.Clear();
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "SimulateTerminal", "ExecuteTick()","",e);
                //throw new Exception("Order Execution by Tick Failure. " + e.Message);
                throw;
            }
        }

        private ITrade3 NewTrade(IStrategy s, DateTime dt, ulong number,
            TradeStatusEnum status, TradeOperationEnum operation,
            long quantity, decimal price, ulong orderNumber, decimal comission)
        {
            return new Trade3
            {
                Strategy = s,
                DT = dt,

                Number = number,
                Registered = dt,

                Status = status,

                //AccountEx = account,
                //StrategyEx = strategy,
                //ClassCodeEx = classCode,
                //TickerEx = ticker,

                //TradeOperation = TradeOperationEnum.Buy,

                Operation = operation,
                Quantity = quantity,
                Price = price,
                OrderNumber = orderNumber,
                CommissionTs = comission
            };
        }

        private void TradeProcess(ITrade3 t)
        {
            try
            {
                t.Status = TradeStatusEnum.Confirmed;
                if (t.Strategy.NewTrade(t) <= 0)
                    return;

                OnChangedEvent(new EventArgs
                {
                    Category = "UI.Trades",
                    Entity = "Trade",
                    Operation = "Add",
                    Object = t
                });
                 _tx.Save(t);
            }
            catch (Exception e)
            {
                throw new NullReferenceException("SimulateTerminal.TradeProcess() Failure: " + e.Message );
            }
            //TradeEventFire(new EventArgs
            //{
            //    Category = "UI.Trades",
            //    Entity = "Trade",
            //    Operation = "Update",
            //    Object = t
            //});
        }

        private void NotifyActiveOrder(IOrder3 o, string operation)
        {
            //OnChangedEvent(new Events.EventArgs
            //{
            //    Category = "UI.ORDERS",
            //    Entity = "ORDER",
            //    Operation = operation,
            //    Object = o
            //});
            switch (operation.TrimUpper())
            {
                case "ADD":
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER",
                        Operation = "ADD",
                        Object = o,
                        Sender = this
                    });
                    break;
                case "FILLED":
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER",
                        Operation = "DELETE",
                        Object = o,
                        Sender = this
                    });
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER.COMPLETED",
                        Operation = "ADD",
                        Object = o,
                        Sender = this
                    });
                    break;
                case "DELETE":
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER",
                        Operation = "DELETE",
                        Object = o,
                        Sender = this
                    });
                    break;
            }
        }

    }
}
