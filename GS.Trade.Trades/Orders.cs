using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using GS.Extension;
using GS.Interfaces;
// using GS.Trade.Data;
// using GS.Trade.Data.Bars;

//using GS.Trade.TradeTerminals.Quik;

//namespace sg_Trades
namespace GS.Trade.Trades
{
    public delegate void GetOrdersToObserve();  
/*
    public enum EnumOrderStatus : short
    {
        Registered = 1, Active = 2, Filled = 3, PartlyFilled = 5, Cancel = 6  
    }

    public enum EnumOrderType : short { Limit = 1, Stop = 2, StopLimit = 3 }
 */  

//**************************  Orders **********************************************
    public partial class Orders : IOrderStatusReceiver, IOrders  // , ITrOrders
    {
        public delegate void NeedToOserverEventHandler();
        public event NeedToOserverEventHandler NeedToObserverEvent;

        public event EventHandler<INewOrderStatusEventArgs> NewOrderStatusEvent;

        //public delegate void StopOrderFilledEventHandler(string tradeKey);

        public delegate void StopOrderFilledEventHandler(Order o);
        public event StopOrderFilledEventHandler StopOrderFilledEvent;

        public delegate void LimitOrderFilledEventHandler(Order o);
        public event LimitOrderFilledEventHandler LimitOrderFilledEvent;

        private IEventLog _eventLog;
        private ITrades _trades;
        private IPositions _positions;

        public ObservableCollection<Order> OrderActiveObserveCollection { get; set; }
        public ObservableCollection<Order> OrderFilledObserveCollection { get; set; }

        public List<Order> FillerCollection { get; set; }
        public List<Order> TempOrderCollection { get; set; }
        public List<Order> OrderCollection { get; set; }
        public List<Order> OrderFilledCollection { get; set; }
        public Dictionary<string, Order> OrderDictionary { get; set; }

        public string ClassCodeToRemoveLogin { get; set; }
        public string LoginToRemove { get; set; }

        public IEqualityComparer<Order> GetAccountTickerComparer
        {
            get { return new AccountTickerComparer(); }
        }

        public GetOrdersToObserve CallbackGetOrdersToObserve;

        private volatile int _needToObserve;

        private long _lastObserveGetRequestNumber;

        public ulong LastOrderNumber;
        public DateTime LastOrderDateTime;
        public long MaxIndex { get; set; }

        static private long _number;

        private object _orderLocker;
        private object _orderFilledLocker;
        private object _fillerLocker;
        private bool _backTestMode;

        public BackTestOrderExecutionMode BackOrderExecMode { get; set; }

        public Orders()
        {
            OrderActiveObserveCollection = new ObservableCollection<Order>();
            OrderFilledObserveCollection = new ObservableCollection<Order>();

            FillerCollection = new List<Order>();
            TempOrderCollection = new List<Order>();
            OrderCollection = new List<Order>();
            OrderFilledCollection = new List<Order>();
            OrderDictionary = new Dictionary<string, Order>();

            LastOrderNumber = 0;
            MaxIndex = 0;

            _orderLocker = new object();
            _orderFilledLocker = new object();
            _fillerLocker = new object();
        }
        public void Init(IEventLog eventlog, IPositions positions, ITrades trades)
        {
            if (eventlog != null) _eventLog = eventlog;
            if (trades != null) _trades = trades;
            if (positions != null) _positions = positions;

            _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "Orders", "Orders", "Initialization", "", "");
        }
        //public void Init(IEventLog evl, IPositions ps, ITrades ts)
        //{
        //    throw new NotImplementedException();
        //}
        /*
               private void AddOrder( Order order)
               {
                     lock (_orderLocker)
                        {
                            Orders = this;
                            order.MyIndex = ++_myIndex;

                            OrderDictionary.Add(order.Key, order);
                            _needToObserve = +1;
                        }
                        _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
                                         "Orders", "Add New", order.ToString(),
                                         String.Format("OrderCount: {0}", OrderDictionary.Count));              
                }
         */
        private void AddOrder(Order order)
        {
            lock (_orderLocker)
            {
                order.Orders = this;
                order.MyIndex = ++MaxIndex;

                OrderCollection.Add(order);

                if (order.Status != OrderStatusEnum.Registered) LastOrderNumber = order.Number;
                _needToObserve = +1;
            }
            _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, order.StratTicker,
                             "Order", "New "/*+ order.Number.ToString().WithSqBrackets()*/, order.ShortInfo, order.ToString());
        }

        public Order GetOrderOrNull(string key)
        {
            Order order;
            return OrderDictionary.TryGetValue(key, out order) ? order : null;
        }
        public Order GetOrderOrNull(string account, ulong number)
        {
           // return
           //     (from o in OrderCollection where o.Account == account && o.Number == number select o).FirstOrDefault();
            Order ord;
            lock (_orderLocker)
            {
                ord = (from o in OrderCollection where o.Account == account && o.Number == number select o).FirstOrDefault();
            }
            return ord;
        }

        public void UpdateOrder(short mode, Order order)
        {
            Order o;
            switch (mode)
            {
                case 1:
                    o = (OrderCollection.Where(ord => ord.TransId == order.TransId)).FirstOrDefault();
                    if (o != null)
                    {
                        o.Account = order.Account;
                        o.Strategy = order.Strategy;
                        o.Quantity = order.Quantity;
                        o.Rest = order.Rest;
                        o.Status = order.Status;
                    }
                    else
                    {
                        OrderCollection.Add(order);
                    }
                    break;
                case 2:
                    o = (OrderCollection.Where(ord => ord.Number == order.Number)).FirstOrDefault();
                    if (o != null)
                    {
                        o.Quantity = order.Quantity;
                        o.Rest = order.Rest;
                        o.Status = order.Status;
                    }
                    else
                    {
                        OrderCollection.Add(order);
                    }
                    break;
            }

            /*
            foreach (var o in OrderCollection.Where(o => o.TransId == order.TransId))
            {
                o.Account = order.Account;
                o.Strategy = order.Strategy;
                o.Quantity = order.Quantity;
                o.Rest = order.Rest;
                o.Status = order.Status;
                break;
            }
            */
        }
        public void NewOrderStatus(string orderString)
        {
            _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "Orders",
                    "Orders", "New Order Status", "NewOrderStatus(string OrderString)", orderString);
        }

        public void NewOrderStatus(double dNumber, int iDate, int iTime,
           string sAcc, string sStrat, string sSecCode, int iIsSell, int iQty, int Rest, double dPrice,
           int iStatus, uint dwTransId)
        {
            ulong number = Convert.ToUInt64(dNumber);
            var sDt = iDate.ToString();
            var sTm = iTime.ToString();
            var dt = new DateTime(
                int.Parse(sDt.Substring(0, 4)), int.Parse(sDt.Substring(4, 2)), int.Parse(sDt.Substring(6, 2)),
                int.Parse(sTm.Substring(0, 2)), int.Parse(sTm.Substring(2, 2)), int.Parse(sTm.Substring(4, 2)));

            short operation = Convert.ToInt16(iIsSell & 0xff);
            ulong transId = Convert.ToUInt64(dwTransId);

            var oper = Order.OperationToEnum(operation);

            var ord = new Order(number, dt, sAcc, sStrat, sSecCode, oper, 0, dPrice, iQty, Rest, iStatus, transId, "");
            OrderCollection.Add(ord);
            _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "Orders",
                    "Orders", "New Order", "Get New Order from Quik", ord.ToString());
        }
        // Current version
        public void NewOrderStatus(double dNumber, int iDate, int iTime, int mode,
           int iActivationTime, int iCancelTime, int iExpireDate,
           string account, string strategy, string classCode, string ticker, int iIsSell, int quantity, int rest, double price,
           int iStatus, uint dwTransId, string comment)
        {
            if (classCode == "FUTEVN") return;

            if (ClassCodeToRemoveLogin.HasValue())
                if (classCode.Contains(ClassCodeToRemoveLogin))
                    strategy = strategy.Replace(LoginToRemove, "");

            var number = Convert.ToUInt64(dNumber);
            var sDt = iDate.ToString("D8");
            var sTm = iTime.ToString("D6");
            var dt = new DateTime(
                int.Parse(sDt.Substring(0, 4)), int.Parse(sDt.Substring(4, 2)), int.Parse(sDt.Substring(6, 2)),
                int.Parse(sTm.Substring(0, 2)), int.Parse(sTm.Substring(2, 2)), int.Parse(sTm.Substring(4, 2)));

            var sActTime = iActivationTime.ToString("D6");
            var activeTime = new TimeSpan(
                int.Parse(sActTime.Substring(0, 2)),
                int.Parse(sActTime.Substring(2, 2)),
                int.Parse(sActTime.Substring(4, 2)));

            var sCancTime = iCancelTime.ToString("D6");
            var cancelTime = new TimeSpan(
                int.Parse(sCancTime.Substring(0, 2)),
                int.Parse(sCancTime.Substring(2, 2)),
                int.Parse(sCancTime.Substring(4, 2)));

            var sExpireDate = iExpireDate.ToString("D8");
            var i1 = int.Parse(sExpireDate.Substring(0, 4));
            /*
                DateTime expireDate;
                if( i1 > 1800)
                {
                    var i2 = int.Parse(sExpireDate.Substring(4, 2));
                    var i3 = int.Parse(sExpireDate.Substring(6, 2));
                    expireDate = new DateTime(i1, i2, i3);    
                }
                */
            var operation = (short)(iIsSell == 0 ? +1 : -1);
            var transId = Convert.ToUInt64(dwTransId);

            OrderStatusEnum status;
            switch (iStatus)
            {
                case 1:
                    status = OrderStatusEnum.Activated;
                    break;
                case 2:
                    status = OrderStatusEnum.Canceled;
                    break;
                default:
                    status = OrderStatusEnum.Filled;
                    break;
            }

            //var ord = GetOrderOrNull(General.Key(account, number));
            var ord = GetOrderOrNull(account, number);
            if (ord != null)
            {
                /*
                NewStatusProcess(number, dt, mode,
                                 account, strategy, ticker, operation, 0, price, quantity, rest, status, transId, comment,
                                 activeTime, cancelTime);
                 */
                //NewStatusProcess(ord);

                _needToObserve = +1;
                var stOld = ord.Status;
                var trIdOld = ord.TransId;

                ord.Quantity = quantity;
                ord.Rest = rest;
                //ord.Status = status;
                ord.SetStatus(status, string.Empty);
                ord.ExCancelTime = cancelTime;
                ord.ExActivateTime = activeTime;
                ord.TransId = transId;

                if( ord.Status == OrderStatusEnum.Canceled ||
                    ord.Status == OrderStatusEnum.Filled)
                        RemoveFilled(ord);

                ord.MyIndex = ++MaxIndex;

                if (stOld == OrderStatusEnum.Activated && status == OrderStatusEnum.Canceled)
                    _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, ord.StratTicker, "Order", "Cancel Order", ord.ToString(), "");
                else if (trIdOld == 0 && transId != 0)
                    _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, ord.StratTicker, "Order", "GetTransID ", ord.ToString(),
         String.Format("numb={0} strat={1} tic={2} oper={3} pr={4} q={5} rest={6} st={7} trID={8} com={9}  actT={10} cancT={11}",
                                          number, strategy, ticker, operation, price, quantity, rest, status, transId, comment, activeTime, cancelTime));
                else
                    _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, ord.StratTicker,
                                "Order", "Update", ord.ToString(),
         String.Format("numb={0} strat={1} tic={2} oper={3} pr={4} q={5} rest={6} st={7} trID={8} com={9}  actT={10} cancT={11}",
                                          number, strategy, ticker, operation, price, quantity, rest, status, transId, comment, activeTime, cancelTime));
            }
            else
            { //New order
                var oper = Order.OperationToEnum(operation);
                var o = new Order(number, dt, mode,
                                    account, strategy, ticker,
                                    oper, OrderTypeEnum.Limit, 0, price, quantity, rest, status, transId, comment, activeTime, cancelTime);
                AddOrder(o);
            }
        }
        private void NewOrderStatusProcess(Order ord)
        {

        }
        public void RemoveFilled(Order o)
        {
            lock(_orderLocker)
            {
                OrderCollection.Remove(o);
            }
            lock (_orderFilledLocker)
            {
                OrderFilledCollection.Add(o);
            }
            RiseNewOrderStatusEvent(o);
            /*
            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 lock (_orderFilledLocker)
                                                 {
                                                     OrderFilledCollection.Add(o);
                                                 }
                                                 RiseNewOrderStatusEvent(o);
                                             });
            */
        }
        public void RemoveFilled2(Order o)
        {
            lock (_orderLocker)
            {
                OrderCollection.Remove(o);
            }
        }

        /*
        private void NewStatusProcess(long number, DateTime dt, int mode, 
            string account, string strategy, string ticker, 
            short operation, double stopprice, double limitprice, Int32 quantity, Int32 rest, OrderStatusEnum status, long transId, string comment,
            TimeSpan activeTime, TimeSpan cancelTime)
        {
            
            // _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
            //                "Orders", "New Status Order",
            //                String.Format("numb={0} strat={1} tic={2} oper={3} pr={4} q={5} rest={6} st={7} trID={8} com={9}  actT={10} cancT={11}",
            //                          number, strategy, ticker, operation, limitprice, quantity, rest, status, transId, comment, activeTime, cancelTime),
            //                          "");
            
            _needToObserve = +1;
            var key = General.Key(account, number);
            //var ord = OrderCollection.Where(o => Equals(o.Key, key)).FirstOrDefault();
            var ord = GetOrderOrNull(key);
            if ( ord != null)
            {   // Order Exist. UpdateOrder

                var stOld = ord.Status;

                ord.Quantity = quantity;
                ord.Rest = rest;
                ord.Status = status;
                ord.CancelTime = cancelTime;
                ord.ActivateTime = activeTime;
                ord.TransId = transId;

                ord.MyIndex = ++_myIndex;

                if (stOld == OrderStatusEnum.Active && status == OrderStatusEnum.Cancel)
                _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
                            "Orders", "Cancel Order",  ord.ToString(), "");
                else
                _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
                            "Orders", "Update Order", ord.ToString(),
     String.Format("numb={0} strat={1} tic={2} oper={3} pr={4} q={5} rest={6} st={7} trID={8} com={9}  actT={10} cancT={11}",
                                      number, strategy, ticker, operation, limitprice, quantity, rest, status, transId, comment, activeTime, cancelTime));

            }              
            else
            { // Order with this Number Not Found 

                    var oper = OperationToEnum(operation);
                    var o = new Order(number, dt, mode,
                                        account, strategy, ticker, 
                                        oper, OrderTypeEnum.Limit, stopprice, limitprice, quantity, rest, status, transId, comment, activeTime, cancelTime);
                    AddOrder(o);
                    _eventLog.AddItem(EvlResult.SOS, EvlSubject.TRADING,
                            "Orders", "New Order", o.ToString(),
                            String.Format("{0:G} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}",
                                            dt, number, account, strategy,
                                            ticker, operation, stopprice, limitprice, quantity, rest, status, transId));
            }
        }
        */
        /*
        private void NewStatusProcessOld(long number, DateTime dt, int mode,
            string account, string strategy, string ticker,
            short operation, double stopprice, double limitprice, int quantity, int rest, OrderStatusEnum status, long transId, string comment,
            TimeSpan activeTime, TimeSpan cancelTime)
        {
            _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
                            "Orders", "New Status Order",
                            String.Format("numb={0} strat={1} tic={2} oper={3} pr={4} q={5} rest={6} st={7} trID={8} com={9}  actT={10} cancT={11}",
                                      number, strategy, ticker, operation, limitprice, quantity, rest, status, transId, comment, activeTime, cancelTime),
                                      "");

            Order newOrder = null;
            var ord = OrderCollection.Where(o => Equals(o.Account, account) && o.Number == number).FirstOrDefault();
            if (ord != null)
            {   // Order Exist. UpdateOrder
                var stOld = ord.Status;
                ord.Quantity = quantity;
                ord.Rest = rest;
                ord.Status = status;
                ord.CancelTime = cancelTime;
                ord.ActivateTime = activeTime;
                ord.TransId = transId;

                if (stOld == OrderStatusEnum.Active && status == OrderStatusEnum.Cancel)
                    _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
                                "Orders", "Cancel Order", ord.ToString(), "");
                else
                    _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
                                "Orders", "Update Order",
                                String.Format("acc={0} strat={1} tic={2} oper={3} pr={4} q={5} st={6} com={7} trID={8} actT={9}",
                                          account, strategy, ticker, operation, limitprice, quantity, status, comment, transId, activeTime),
                                          ord.ToString());

                newOrder = ord;
            }
            else // Order does not Exist. Order with this Number is Not Found
            {
                if (transId != 0)
                {
                    var order = OrderCollection.Where(o => Equals(o.Account, account) && o.TransId == transId).FirstOrDefault();
                    if (order != null)
                    {
                        order.Number = number;
                        order.DateTime = dt;

                        order.Quantity = quantity;
                        order.Rest = rest;
                        order.Status = status;
                        order.CancelTime = cancelTime;

                        _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
                            "Orders", "New Status Update", "Update Registered Order", order.ToString());

                        newOrder = order;
                    }
                    else
                    {   //Order with this Number Not Found && Order Registered with transId Not Found Try to Take Strategy from !!!! Comment !!!!!
                        _eventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING,
                            "Orders", "New Status", String.Format("Not Found Order registered with TransID={0}", transId),
                            String.Format("{0:G} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12}",
                                            dt, number, account, strategy,
                                            ticker, operation, stopprice, limitprice, quantity, rest, status, comment, transId));

                        var oper = OperationToEnum(operation);
                        var o = new Order(number, dt, mode, account, comment, ticker,
                                        oper, stopprice, limitprice, quantity, rest, status, transId, comment, activeTime, cancelTime);
                        AddOrder(o);
                        newOrder = o;
                    }
                }
                else
                { // Order with this Number Not Found && Order TransId == 0

                    var oper = OperationToEnum(operation);
                    var o = new Order(number, dt, mode, account, strategy, ticker,
                                        oper, stopprice, limitprice, quantity, rest, status, transId, comment, activeTime, cancelTime);
                    AddOrder(o);
                    newOrder = o;

                    _eventLog.AddItem(EvlResult.SOS, EvlSubject.TRADING,
                            "Orders", "New UnRegistered", o.ToString(),
                            String.Format("{0:G} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}",
                                            dt, number, account, strategy,
                                            ticker, operation, stopprice, limitprice, quantity, rest, status, transId));
                }

            }
            if (newOrder != null)
            {
                var trade =
                    _trades.TradeCollection.
                            Where(t => t.MyStatus < 0 && newOrder.Number == t.OrderNumber && newOrder.Account == t.Account).FirstOrDefault();
                if (trade != null)
                {
                    lock (trade)
                    {
                        trade.MyStatus = +1;
                        trade.Strategy = newOrder.Strategy;
                    }
                    _positions.PositionCalculate2(trade);
                    _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
                            "Orders", "Find UnKnown Trade", newOrder.ToString(), trade.ToString());

                }
            }
        }
        */
        public void SetNeedToObserver()
        {
            _needToObserve = +1;
        }

        public void GetOrdersToObserve()
        {
            if (_needToObserve <= 0) return;
            lock (_orderLocker)
            {  /*
                foreach (var order in OrderCollection.Where(order => order.Number > _lastObserveGetRequestNumber))
                {
                    OrderActiveObserveCollection.Insert(0, order);
                }
                if (OrderCollection.Count > 0)
                {
                    var o = OrderCollection.Last();
                    _lastObserveGetRequestNumber = o.Number;
                }
                else
                    _lastObserveGetRequestNumber = 0;
                */
                OrderActiveObserveCollection.Clear();
                foreach (var order in OrderCollection.
                    Where(order =>
                        order.Status == OrderStatusEnum.Activated ||
                        order.Status == OrderStatusEnum.Registered ||
                        order.Status == OrderStatusEnum.PartlyFilled ||
                        order.Status == OrderStatusEnum.Unknown)
                        )
                {
                    OrderActiveObserveCollection.Insert(0, order); // Add(order); //Insert(0, p);
                }

                foreach (var order in OrderCollection.Where(order =>
                    (
                    order.Status == OrderStatusEnum.Filled ||
                    order.Status == OrderStatusEnum.Canceled
                    ) &&
                    order.MyIndex > _lastObserveGetRequestNumber)
                    )
                {
                    OrderFilledObserveCollection.Insert(0, order);
                    _lastObserveGetRequestNumber = order.MyIndex;
                }
                _needToObserve = 0;
            }
        }
        public void RegisterOrder(OrderCommand ordcom)
        {
            var neworder = new Order(0, DateTime.Now, ordcom.TransId,
                                     ordcom.Account, ordcom.Strategy, ordcom.Ticker,
                                     ordcom.Operation, ordcom.OrderType, ordcom.StopPrice, ordcom.LimitPrice,
                                     ordcom.Quantity, ordcom.Quantity,
                                     OrderStatusEnum.Registered, "");
            AddOrder(neworder);
    //        _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
    //                          "Order", "Register New", neworder.ToString(), "");
        }

        public void RegisterOrder(ulong number, DateTime dt, ulong transId,
            string account, string strategy, string ticker,
            OrderOperationEnum operation, OrderTypeEnum ordertype, double stopprice, double limitprice, long quantity, long rest,
            OrderStatusEnum status, string comment)
        {
            if (!CheckOrderParam(number, dt, transId, account, strategy, ticker,
                                 operation, ordertype, stopprice, limitprice, quantity, rest, status, comment))
                return;


            var neworder = new Order(number, dt, transId,
                                     account, strategy, ticker,
                                     operation, ordertype, stopprice, limitprice, quantity, rest,
                                     status, comment);
            AddOrder(neworder);
    //        _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
    //                          "Order", "Register New", neworder.ToString(), "");
        }
        public void RegisterOrder(ulong number, DateTime dt, ulong transId,
            string account, string strategy, string ticker,
            OrderOperationEnum operation, OrderTypeEnum ordertype, double stopprice, double limitprice, long quantity, long rest,
            OrderStatusEnum status, TimeSpan activatetime, TimeSpan canceltime, DateTime expire, string comment)
        {
            if (!CheckOrderParam(number, dt, transId, account, strategy, ticker,
                                 operation, ordertype, stopprice, limitprice, quantity, rest, status, comment))
                return;
            if (number == 0)
            {
                number = GetUnicID();
                transId = number;
            }

            var neworder = new Order(this, number, dt, transId,
                                     account, strategy, ticker,
                                     operation, ordertype, stopprice, limitprice, quantity, rest,
                                     status,
                                     activatetime, canceltime, expire, comment);
            AddOrder(neworder);
     //       _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
     //                         "Order", "Register New", neworder.ToString(), "");
        }
        
        public void RegisterOrder(ulong number, DateTime dt, ulong transId,
            string account, string strategy, string ticker,
            OrderOperationEnum operation, OrderTypeEnum ordertype, double stopprice, double limitprice, long quantity, long rest,
            string comment)
        {
            return;
         //   var oper = OperationEnum.Unknown;
         //   var ordtype = OrderTypeEnum.Unknown;

            /*
            switch (operation)
            {
                case QuikTradeTerminal.OrderOperation.Buy:
                    oper = OperationEnum.Buy;
                    break;
                case QuikTradeTerminal.OrderOperation.Sell:
                    oper = OperationEnum.Sell;
                    break;
            }
            switch (ordertype)
            {
                case QuikTradeTerminal.OrderType.Limit:
                    ordtype = OrderTypeEnum.Limit;
                    break;
                case QuikTradeTerminal.OrderType.StopLimit:
                    ordtype = OrderTypeEnum.StopLimit;
                    break;
                case QuikTradeTerminal.OrderType.Market:
                    ordtype = OrderTypeEnum.Market;
                    break;
                case QuikTradeTerminal.OrderType.Stop:
                    ordtype = OrderTypeEnum.StopLimit;
                    break;
            }
            */
            if ((operation == OrderOperationEnum.Buy || operation == OrderOperationEnum.Sell) && ordertype != OrderTypeEnum.Unknown)
            {
                var neworder = new Order(number, dt, transId,
                                         account, strategy, ticker,
                                         operation, ordertype, stopprice, limitprice, quantity, rest,
                                         OrderStatusEnum.Registered, comment);
                AddOrder(neworder);
                _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "Order", 
                                  "Order", "Register New", neworder.ToString(), "");
            }
            else
            {
                _eventLog.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Order",
                                  "Order", "Register New", operation.ToString(), ordertype.ToString());
            }
        }
         
        private bool CheckOrderParam(ulong number, DateTime dt, ulong transId,
            string account, string strategy, string ticker,
            OrderOperationEnum operation, OrderTypeEnum ordertype, double stopprice, double limitprice, long quantity, long rest,
            OrderStatusEnum status, string comment)
        {
            if (
                !string.IsNullOrWhiteSpace(account) && !string.IsNullOrWhiteSpace(strategy) && !string.IsNullOrWhiteSpace(ticker) &&
                (operation != OrderOperationEnum.Unknown) &&
                (
                (ordertype == OrderTypeEnum.Market && limitprice > 0) ||
                (ordertype == OrderTypeEnum.Stop && stopprice > 0) ||
                (ordertype == OrderTypeEnum.Limit && limitprice > 0) ||
                (ordertype == OrderTypeEnum.StopLimit && stopprice > 0 && limitprice > 0)) &&
                (quantity > 0)
                )
                return true;

            _eventLog.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Orders",
                              "Orders", "Invalid Order Parameters", String.Format(
                                  "Numb={0} dt={1} trId={2} acc={3} str={4} Tick={5} oper={6} ordtype={7} stop={8} lim={9} quant={10} rest={11} stat={12} comm={13}",
                                  number, dt, transId, account, strategy, ticker, operation, ordertype, stopprice, limitprice, quantity, rest, status, comment), "");
            return false;
        }
        public void TransactionReply(int result, UInt32 transId, ulong orderNumber)
        {
            if (result != 0 || transId == 0 || orderNumber == 0) return;

            var ord = OrderCollection.Where(o => o.TransId == transId).FirstOrDefault();
            if (ord != null)
            {
                ord.Number = orderNumber;
                if (ord.OrderType == OrderTypeEnum.StopLimit) ord.Status = OrderStatusEnum.Activated;
                _needToObserve = +1;
            }
        }
        /*
        public void SetStatus(Order o, OrderStatusEnum status, string comment)
        {
            if (o == null) return;

            o.Status = status;
            if (!string.IsNullOrWhiteSpace(comment)) o.Comment += "; " + comment;
            switch (status)
            {
                case OrderStatusEnum.Canceled:
                case OrderStatusEnum.Filled:
                    o.CancelTime = DateTime.Now.TimeOfDay;
 //                   RemoveFilled(o);
                    break;
                case OrderStatusEnum.Active:
                    o.ActivateTime = DateTime.Now.TimeOfDay;
                    break;
            }
            SetNeedToObserver();
        }
        */
        public void SetStatus(Order o, OrderStatusEnum status, string comment)
        {
            if (o == null) return;
            o.SetStatus(status, comment);
            SetNeedToObserver();
        }

        public void CancelOrder(ulong key)
        {
            var order = (from ord in OrderCollection
                         where ord.Status == OrderStatusEnum.Activated &&
                               ord.Number == key
                         select ord).FirstOrDefault();
            SetStatus(order, OrderStatusEnum.Canceled, "");
        }
        public void CancelOrderByTransID(ulong transID)
        {
            var order = (from ord in OrderCollection
                         where ord.Status == OrderStatusEnum.Activated &&
                               ord.TransId == transID
                         select ord).FirstOrDefault();

            if (order == null) return;

            SetStatus(order, OrderStatusEnum.Canceled, "");
        }
        public void CancelOrders(string strategy, string account, string tickercategory, string tickercode,
                                    OrderTypeEnum ordtype, OrderOperationEnum operation)
        {
            try
            {
                lock (_orderLocker)
                {
                    TempOrderCollection.Clear();

                    //foreach (var o in OrderCollection.Where(o => o.IsActive))
                    foreach (var o in OrderCollection)
                    {
                        if (!o.IsActive) continue;
                        TempOrderCollection.Add(o);
                    }
                }
                lock (_fillerLocker)
                {
                    if (ordtype == OrderTypeEnum.All)
                    {

                        if (operation == OrderOperationEnum.All)
                        {
                            foreach (var o in TempOrderCollection.Where(o => // o.Status == OrderStatusEnum.Active &&
                                o.Strategy == strategy &&
                                o.Account == account &&
                                o.Ticker == tickercode)
                                )
                            {
                                if (!o.IsActive) continue;
                                SetStatus(o, OrderStatusEnum.Canceled, "");
                                RemoveFilled2(o);
                            }
                        }
                        else
                        {
                            foreach (var o in TempOrderCollection.Where(o => // o.Status == OrderStatusEnum.Active &&
                                o.Operation == operation &&
                                o.Strategy == strategy &&
                                o.Account == account &&
                                o.Ticker == tickercode)
                                )
                            {
                                if (!o.IsActive) continue;
                                SetStatus(o, OrderStatusEnum.Canceled, "");
                                RemoveFilled2(o);
                            }
                        }
                    }
                    else
                    {
                        if (operation == OrderOperationEnum.All)
                        {
                            foreach (var o in TempOrderCollection.Where(o => // o.Status == OrderStatusEnum.Active &&
                                o.OrderType == ordtype &&
                                o.Strategy == strategy &&
                                o.Account == account &&
                                o.Ticker == tickercode)
                                )
                            {
                                if (!o.IsActive) continue;
                                SetStatus(o, OrderStatusEnum.Canceled, "");
                                RemoveFilled2(o);
                            }
                        }
                        else
                        {
                            foreach (
                                var o in TempOrderCollection.Where(o => // o.Status == OrderStatusEnum.Active &&
                                    o.OrderType == ordtype && o.Operation == operation &&
                                    o.Strategy == strategy &&
                                    o.Account == account &&
                                    o.Ticker == tickercode)
                                )
                            {
                                if (!o.IsActive) continue;
                                SetStatus(o, OrderStatusEnum.Canceled, "");
                                RemoveFilled2(o);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Cancel Orders Failure: " + e.Message);
            }
            SetNeedToObserver();
        }
        private static ulong GetUnicID()
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

            return ulong.Parse(s);
        }
        public void SetBackTestMode( bool mode)
        {
            _backTestMode = mode;
            FillerCollection = _backTestMode ? OrderCollection : new List<Order>();
        }



        public void NewTick(DateTime dt, string tickerkey, double price)
        {
            // FIll ORders Simulater
          //  if (!_backTestMode )
          //  {
            lock (_fillerLocker)
            {
                FillerCollection.Clear();
                //lock (_orderLocker)
                //{
                    // foreach (var o in OrderCollection.Where(o => o.IsActive && o.Ticker == tickerkey))
                    foreach (var o in OrderCollection.Where(o => o.Ticker == tickerkey))
                    {
                        if (!o.IsActive) continue;
                        FillerCollection.Add(o);
                    }
                //}
                //    }
                // ***********   Cancell Expired *******************************
                /*
                    foreach (var o in FillerCollection.Where(o => o.IsActive && o.ExpireDate.CompareTo(dt) <= 0))
                    {
                        o.SetStatus(OrderStatusEnum.Cancel, "Expired: " + o.ExpireDateString);
                    }
                */
                // *************** Fill Limit Orders ****************** 
                
                    foreach (var order in FillerCollection.Where(o =>
                                                                 o.Ticker == tickerkey &&
                                                                // o.IsActive &&
                                                                 o.IsLimit))
                    {
                        if (!order.IsActive) continue;
                        if (BackOrderExecMode == BackTestOrderExecutionMode.Optimistic)
                        {
                            if (order.IsBuy && order.LimitPrice.CompareTo(price) >= 0)
                            {
                                order.SetStatus(OrderStatusEnum.Filled, "Filled BuyLimit=" + price);

                                _trades.NewTrade(order.Number, dt,
                                                 order.Account, order.Strategy, order.Ticker,
                                                 TradeOperationEnum.Buy, (int) order.Quantity, price, "", order.Number,
                                                 0.01);

                                InvokeLimitOrderFilledEvent(order);

                            }
                            else if (order.IsSell && order.LimitPrice.CompareTo(price) <= 0)
                            {
                                order.SetStatus(OrderStatusEnum.Filled, "Filled SellLimit=" + price);

                                _trades.NewTrade(order.Number, dt,
                                                 order.Account, order.Strategy, order.Ticker,
                                                 TradeOperationEnum.Sell, (int) order.Quantity, price, "", order.Number,
                                                 0.01);

                                InvokeLimitOrderFilledEvent(order);
                            }
                        }
                        else if (BackOrderExecMode == BackTestOrderExecutionMode.Pessimistic)
                        {
                            if (order.IsBuy && order.LimitPrice.CompareTo(price) > 0)
                            {
                                order.SetStatus(OrderStatusEnum.Filled, "Filled BuyLimit: " + order.LimitPrice);

                                _trades.NewTrade(order.Number, dt,
                                                 order.Account, order.Strategy, order.Ticker,
                                                 TradeOperationEnum.Buy, (int)order.Quantity, order.LimitPrice, "", order.Number,
                                                 0.01);

                                InvokeLimitOrderFilledEvent(order);

                            }
                            else if (order.IsSell && order.LimitPrice.CompareTo(price) < 0)
                            {
                                order.SetStatus(OrderStatusEnum.Filled, "Filled SellLimit: " + order.LimitPrice);

                                _trades.NewTrade(order.Number, dt,
                                                 order.Account, order.Strategy, order.Ticker,
                                                 TradeOperationEnum.Sell, (int)order.Quantity, order.LimitPrice, "", order.Number,
                                                 0.01);

                                InvokeLimitOrderFilledEvent(order);
                            }
                        }
            }
                
                // ********************* Fill StopLimit ***********************
               
                    foreach (var o in FillerCollection.Where(o =>
                                                             o.Ticker == tickerkey &&
                                                             //o.IsActive &&
                                                             o.IsStopLimit))
                    {
                        if (!o.IsActive) continue;

                        if (o.IsBuy && o.StopPrice.CompareTo(price) <= 0)
                        {
                            o.SetStatus(OrderStatusEnum.Filled, "Filled BuyStop=" + price);
                            RegisterOrder(0, dt, 0, o.Account, o.Strategy, o.Ticker, o.Operation,
                                          OrderTypeEnum.Limit, 0,
                                          o.LimitPrice, o.Quantity, o.Quantity, OrderStatusEnum.Activated,
                                          dt.TimeOfDay, TimeSpan.MinValue, DateTime.Today.Add(new TimeSpan(23, 49, 15)),
                                          "Create from StopOrder:" + o.Number);

                            InvokeStopOrderFilledEvent(o);
                        }
                        else if (o.IsSell && o.StopPrice.CompareTo(price) >= 0)
                        {
                            o.SetStatus(OrderStatusEnum.Filled, "Filled SellStop=" + price);
                            RegisterOrder(0, dt, 0, o.Account, o.Strategy, o.Ticker, o.Operation,
                                          OrderTypeEnum.Limit, 0,
                                          o.LimitPrice, o.Quantity, o.Quantity, OrderStatusEnum.Activated,
                                          dt.TimeOfDay, TimeSpan.MinValue, DateTime.Today.Add(new TimeSpan(23, 49, 15)),
                                          "Create from StopOrder:" + o.Number);

                            InvokeStopOrderFilledEvent(o);
                        }
                    }
                    FillerCollection.Clear();
            }
        }
        public void ExecuteTick(DateTime dt, string tickerkey, double price, double bid, double ask)
        {
            try
            {
                // FIll ORders Simulater
                //  if (!_backTestMode )
                //  {
                //FillerCollection.Clear();
                lock (_orderLocker)
                {
                    FillerCollection.Clear();
                    //foreach (var o in OrderCollection.Where(o => o.IsActive && o.Ticker == tickerkey))
                    foreach (var o in OrderCollection.Where(o => o.Ticker == tickerkey))
                    {
                        if (!o.IsActive) continue;
                        FillerCollection.Add(o);
                    }
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

                    foreach (var order in FillerCollection.Where(o =>
                        o.Ticker == tickerkey &&
                        // o.IsActive &&
                        o.IsLimit))
                    {
                        if (!order.IsActive) continue;
                        if (BackOrderExecMode == BackTestOrderExecutionMode.Optimistic)
                        {
                            if (order.IsBuy && order.LimitPrice.CompareTo(ask) >= 0)
                            {
                                order.SetStatus(OrderStatusEnum.Filled, "Filled BuyLimit=" + ask);
                                RemoveFilled2(order);
                                _trades.NewTrade(order.Number, dt,
                                    order.Account, order.Strategy, order.Ticker,
                                    TradeOperationEnum.Buy, (int) order.Quantity, ask, "", order.Number,
                                    0.01);

                                InvokeLimitOrderFilledEvent(order);

                            }
                            else if (order.IsSell && order.LimitPrice.CompareTo(bid) <= 0)
                            {
                                order.SetStatus(OrderStatusEnum.Filled, "Filled SellLimit=" + bid);
                                RemoveFilled2(order);
                                _trades.NewTrade(order.Number, dt,
                                    order.Account, order.Strategy, order.Ticker,
                                    TradeOperationEnum.Sell, (int) order.Quantity, bid, "", order.Number,
                                    0.01);

                                InvokeLimitOrderFilledEvent(order);
                            }
                        }
                        else if (BackOrderExecMode == BackTestOrderExecutionMode.Pessimistic)
                        {
                            if (order.IsBuy && order.LimitPrice.CompareTo(ask) > 0)
                            {
                                order.SetStatus(OrderStatusEnum.Filled, "Filled BuyLimit: " + ask);
                                RemoveFilled2(order);
                                _trades.NewTrade(order.Number, dt,
                                    order.Account, order.Strategy, order.Ticker,
                                    TradeOperationEnum.Buy, (int) order.Quantity, ask, "", order.Number,
                                    0.01);

                                InvokeLimitOrderFilledEvent(order);

                            }
                            else if (order.IsSell && order.LimitPrice.CompareTo(bid) < 0)
                            {
                                order.SetStatus(OrderStatusEnum.Filled, "Filled SellLimit: " + bid);
                                RemoveFilled2(order);

                                _trades.NewTrade(order.Number, dt,
                                    order.Account, order.Strategy, order.Ticker,
                                    TradeOperationEnum.Sell, (int) order.Quantity, bid, "", order.Number,
                                    0.01);

                                InvokeLimitOrderFilledEvent(order);
                            }
                        }
                    }

                    // ********************* Fill StopLimit ***********************

                    foreach (var o in FillerCollection.Where(o =>
                        o.Ticker == tickerkey &&
                        // o.IsActive &&
                        o.IsStopLimit))
                    {
                        if (! o.IsActive) continue;
                        if (o.IsBuy && o.StopPrice.CompareTo(price) <= 0)
                        {
                            o.SetStatus(OrderStatusEnum.Filled, "Filled BuyStop=" + price);
                            RemoveFilled2(o);
                            RegisterOrder(0, dt, 0, o.Account, o.Strategy, o.Ticker, o.Operation,
                                OrderTypeEnum.Limit, 0,
                                o.LimitPrice, o.Quantity, o.Quantity, OrderStatusEnum.Activated,
                                dt.TimeOfDay, TimeSpan.MinValue, DateTime.Today.Add(new TimeSpan(23, 49, 15)),
                                "Create from StopOrder:" + o.Number);

                            InvokeStopOrderFilledEvent(o);
                        }
                        else if (o.IsSell && o.StopPrice.CompareTo(price) >= 0)
                        {
                            o.SetStatus(OrderStatusEnum.Filled, "Filled SellStop=" + price);
                            RemoveFilled2(o);
                            RegisterOrder(0, dt, 0, o.Account, o.Strategy, o.Ticker, o.Operation,
                                OrderTypeEnum.Limit, 0,
                                o.LimitPrice, o.Quantity, o.Quantity, OrderStatusEnum.Activated,
                                dt.TimeOfDay, TimeSpan.MinValue, DateTime.Today.Add(new TimeSpan(23, 49, 15)),
                                "Create from StopOrder:" + o.Number);

                            InvokeStopOrderFilledEvent(o);
                        }
                    }

                }
                //  FillerCollection.Clear();
            }
            catch (Exception e)
            {
                throw new Exception("Order Execution by Tick Failure. " + e.Message);
            }
        }
       /*
        public void NewBar(string tickerkey, Bar b)
        {
            NewTick(b.DT, tickerkey, b.Open);
            NewTick(b.DT, tickerkey, b.Low);
            NewTick(b.DT, tickerkey, b.High);
            NewTick(b.DT, tickerkey, b.Close);
        }
        */
        private void InvokeStopOrderFilledEvent(Order o)
        {
            if (StopOrderFilledEvent != null) StopOrderFilledEvent(o);
        }
        private void InvokeLimitOrderFilledEvent(Order o)
        {
            if (LimitOrderFilledEvent != null) LimitOrderFilledEvent(o);
        }
        public IList<Order> GetOrders(OrderStatusEnum orderStatus, string tradeKey, IList<Order> orderList)
        {
            orderList.Clear();
            if (orderStatus == OrderStatusEnum.All)
            {
                lock (_orderLocker)
                {
                    foreach (var o in OrderCollection.Where(o => o.TradeKey == tradeKey))
                    {
                        orderList.Add(o);
                    }
                }
            }
            else
            {
                lock (_orderLocker)
                {
                    foreach (var o in OrderCollection.Where(o => o.TradeKey == tradeKey))
                    {
                        if (o.Status != orderStatus) continue;
                        orderList.Add(o);
                    }
                }
            }
            return orderList;
        }
        public void GetOrders(OrderTypeEnum orderType, OrderOperationEnum operation, OrderStatusEnum orderStatus, string tradeKey, IList<Order> ol)
        {
            try
            {
                if (orderType == OrderTypeEnum.All)
                {
                    if (operation == OrderOperationEnum.All)
                    {
                        if (orderStatus == OrderStatusEnum.All)
                        {
                            lock (_orderLocker)
                            {
                                foreach (var order in OrderCollection.Where(o => o.TradeKey == tradeKey))
                                {
                                    ol.Add(order);
                                }
                            }
                        }
                        else // OrderStatus defined
                        {
                            lock (_orderLocker)
                            {
                                foreach (var order in OrderCollection.Where(o => o.TradeKey == tradeKey))
                                {
                                    if (order.Status != orderStatus) continue;
                                    ol.Add(order);
                                }
                            }
                        }
                    }
                    else // Operation is present
                    {
                        if (orderStatus == OrderStatusEnum.All)
                        {
                            lock (_orderLocker)
                            {
                                foreach (
                                    var order in
                                        OrderCollection.Where(o => o.Operation == operation && o.TradeKey == tradeKey))
                                {
                                    ol.Add(order);
                                }
                            }
                        }
                        else // Order.Status
                        {
                            lock (_orderLocker)
                            {
                                //foreach (var order in OrderCollection.Where(o => o.Operation == operation && o.Status == orderStatus && o.TradeKey == tradeKey))
                                foreach (
                                    var order in
                                        OrderCollection.Where(o => o.Operation == operation && o.TradeKey == tradeKey))
                                {
                                    if (order.Status != orderStatus) continue;
                                    ol.Add(order);
                                }
                            }
                        }
                    }
                }
                else // OrderType is Present
                {
                    if (operation == OrderOperationEnum.All)
                    {
                        if (orderStatus == OrderStatusEnum.All)
                        {
                            lock (_orderLocker)
                            {
                                foreach (
                                    var order in
                                        OrderCollection.Where(o => o.OrderType == orderType && o.TradeKey == tradeKey))
                                {
                                    ol.Add(order);
                                }
                            }
                        }
                        else // Order.Status
                        {
                            lock (_orderLocker)
                            {
                                //foreach (var order in OrderCollection.Where(o => o.OrderType == orderType && o.Status == orderStatus && o.TradeKey == tradeKey))
                                foreach (
                                    var order in
                                        OrderCollection.Where(o => o.OrderType == orderType && o.TradeKey == tradeKey))
                                {
                                    if (order.Status != orderStatus) continue;
                                    ol.Add(order);
                                }
                            }
                        }
                    }
                    else // Operation is present
                    {
                        if (orderStatus == OrderStatusEnum.All)
                        {
                            lock (_orderLocker)
                            {
                                foreach (
                                    var order in
                                        OrderCollection.Where(
                                            o =>
                                                o.OrderType == orderType && o.Operation == operation &&
                                                o.TradeKey == tradeKey))
                                {
                                    ol.Add(order);
                                }
                            }
                        }
                        else // Order.Status
                        {
                            lock (_orderLocker)
                            {
                                foreach (
                                    var order in
                                        OrderCollection.Where(
                                            o =>
                                                o.OrderType == orderType && o.Operation == operation &&
                                                o.TradeKey == tradeKey))
                                {
                                    if (order.Status != orderStatus) continue;
                                    ol.Add(order);
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                throw new Exception("Orders.GetOrder() Failure: " + e.Message);
            }
        }
        public void GetAllFilledOrCancelled( List<IOrder> lst)
        {
            lock (_orderFilledLocker)
            {
                lst.AddRange(from o in OrderFilledCollection
                            where   o.Status == OrderStatusEnum.Canceled ||
                                    o.Status == OrderStatusEnum.Filled
                            select o);
            }
        }

        public void GetFilledOrCancelOrders(long index, List<IOrder> oo)
        {
            lock (_orderFilledLocker)
            {
                oo.AddRange( from o in OrderFilledCollection where index < o.MyIndex &&
                             (o.Status == OrderStatusEnum.Canceled || o.Status == OrderStatusEnum.Filled)
                                select o);
            }
        }
        public void GetActiveOrders(long index, List<IOrder> oo)
        {
            lock( _orderLocker )
            {
                //oo.AddRange(from o in OrderCollection where
                //             o.Status == OrderStatusEnum.Activated ||
                //             o.Status == OrderStatusEnum.Registered ||
                //             o.Status == OrderStatusEnum.PartlyFilled ||
                //             o.Status == OrderStatusEnum.Unknown
                //         select o);
                foreach (var o in OrderCollection)
                {
                    if(      o.Status == OrderStatusEnum.Activated ||
                             o.Status == OrderStatusEnum.Registered ||
                             o.Status == OrderStatusEnum.PartlyFilled ||
                             o.Status == OrderStatusEnum.Unknown)
                    oo.Add(o);
                }
            }
        }

        public void ClearSomeData(int count)
        {
            lock( _orderLocker)
            {
               // var oo = from o in OrderCollection.Where(o => o.Status == OrderStatusEnum.Canceled) select o;
                IList<Order> oo = new List<Order>();
                foreach(var o in OrderCollection)
                {
                    if( o.Status == OrderStatusEnum.Canceled )
                        oo.Add(o);
                }
                foreach (var o in oo)
                    OrderCollection.Remove(o);
                oo.Clear();
                
            }
        }
        private void RiseNewOrderStatusEvent(Order o)
        {
            if (NewOrderStatusEvent != null)
                NewOrderStatusEvent(this, new NewOrderStatusEventArgs(o));
        }

        public IEnumerable<IOrder> ActiveOrders { get; private set; }
        public IEnumerable<IOrder> ClosedOrders { get; private set; }

        public IOrder CreateOrder(IStrategy strategy, ulong number, DateTime dt, OrderOperationEnum operation, OrderTypeEnum ordertype,
            OrderStatusEnum status, double stopprice, double limitprice, long quantity, string comment)
        {
            throw new NotImplementedException();
        }

        public IOrder RegisterOrder(IStrategy strategy, OrderOperationEnum operation, OrderTypeEnum ordertype, double stopprice,
            double limitprice, long quantity, string comment)
        {
            throw new NotImplementedException();
        }

        public void GetOrders(OrderTypeEnum orderType, OrderOperationEnum operation, OrderStatusEnum orderStatus, string tradeKey, IList<IOrder> ol)
        {
            throw new NotImplementedException();
        }

       
    }
    
    public class NewOrderStatusEventArgs : EventArgs, INewOrderStatusEventArgs
    {
        //public OrderStatusEnum Status;
        public IOrder Order { get; private set; }
    
        public NewOrderStatusEventArgs(IOrder ord)
        {
            //Status = status;
            Order = ord;
        }
    }
}
