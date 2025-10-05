using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using GS.Interfaces;
//using GS.Time.TimePlan;
using GS.Trade.Data;
using GS.Trade.Interfaces;
using GS.Trade.Data.Studies;
using GS.Trade.Trades;
//using TimePlanEventArgs = GS.Time.TimePlan.TimePlanEventArgs;

namespace GS.Trade.Strategies
{
    abstract public partial class TradeStrategy
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public string TickerKey { get; set; }
        public string TradeAccountKey { get; set; }

        public string TradeTerminalType { get; set; }
        public string TradeTerminalKey { get; set; }

        public string TimePlanKey { get; set; }

        public long Contracts { get; set; }
        public long Contract { get; set; }

        public uint SignalMap { get; set; }

        protected IAccount TradeAccount;
        protected IAccount StrategyAccount;

        protected ITimePlan TimePlan;

        protected List<Position> MyPositionCollection = new List<Position>();
        protected object PositionCollectionLocker = new object();
        protected List<IOrder> MyOrderCollection = new List<IOrder>();
        protected object OrderCollectionLocker = new object();
        protected Queue<OrderTemp> OrdersToSet = new Queue<OrderTemp>();

        protected IOrders Orders;

        [XmlIgnore]
        public string StrategyTickerString
        {
            get { return Code + "." + TickerKey; }
        }
        // public string StrategyTicker { get { return Code + "." + TickerKey; } }

        [XmlIgnore]
        public ITradeTerminal TradeTerminal { get; set; }

        protected Ticker Ticker { get; set; }
        public uint TimeInt { get; set; }

        [XmlIgnore]
        public bool IsWrong { get; private set; }

        [XmlIgnore]
        public IPosition Position { get; set; }
        [XmlIgnore]
        public IPositionTotal PositionTotal { get; set; }

        protected int PositionQuantityPrev;

        protected ITradeContext TradeContext;
        protected Strategies Strategies;

        private bool _stopOrderFilledStatus;

        public bool IsStopOrderFilled { get { return _stopOrderFilledStatus; } }

        public IList<IOrder> MyOrders
        {
            get
            {
                MyOrderCollection.Clear();
                //TradeContext.Orders.GetOrders(OrderTypeEnum.All, OperationEnum.All, OrderStatusEnum.All, TradeKey, MyOrderCollection);
                return MyOrderCollection;
            }
        }
        public IEnumerable MyOrders2
        {
            // Proba
            get { return from o in MyOrders select o; }
        }

        [XmlIgnore]
        abstract public IBars Bars { get; protected set; }
        [XmlIgnore]
        abstract public int MaxBarsBack { get; protected set; }
        abstract public string Key { get; }
        /*
        {
            get { return String.Format("Code={0};Name={1};Account={2};Ticker={3}", Code, Name, TradeAccount.Code, Ticker.Code); }
        }
        */
        
        public string TradeKey
        {
            get { return Code + TradeAccount.Code + TickerKey; }
        }
        public override string ToString()
        {
            return String.Format("Code={0};Name={1};Account={2};Ticker={3}", Code, Name, TradeAccount.Code, Ticker.Code);
        }
        [XmlIgnore]
        public virtual Atr Atr { get { return null; } }
        public virtual float Volatility { get { return 0f; } }

        public virtual string PositionInfo { get { return ""; } }

        protected int ExitMode;
        protected int EntryMode;

        [XmlIgnore]
        public bool EntryEnabled { get; set; }

        protected TradeStrategy()
        {
            // TradeAccount = new Account { Name = "Quik.Open", Code = "SPBFUT00S98" };
            // TradeAccount = new Account { Name = "Quik.Junior", Code = "SPBFUT00082" };
        }

        protected TradeStrategy(ITradeContext tx, string name, string code, Ticker ticker, uint timeInt)
        {
            if (tx == null) throw new NullReferenceException("TradeCOntext == null");
            TradeContext = tx;
            Name = name;
            Code = code + "." + timeInt;
            Ticker = ticker;
            TimeInt = timeInt;

            //  TradeAccount = new Account { Name = "Quik.Open", Code = "SPBFUT00S98" };
            //  StrategyAccount = new Account { Name = "StratAcc", Code = "SPBFUT00S98" };
            //  TradeAccount = new Account { Name = "Quik.Junior", Code = "SPBFUT00082" };

        }
        virtual public void Init()
        {
            Ticker = TradeContext.RegisterTicker(TickerKey) as Ticker;
            if (Ticker == null) { IsWrong = true; return; }
            TradeAccount = TradeContext.GetAccount(TradeAccountKey);
            if (TradeAccount == null) { IsWrong = true; return; }

            TradeTerminal = TradeContext.RegisterTradeTerminal(TradeTerminalType, TradeTerminalKey);
            //Position = TradeContext.Positions.Register(TradeAccount.Code, Code, Ticker);
            //PositionTotal = TradeContext.Positions.PositionTotals.Register(Position);
            Position = TradeContext.RegisterPosition(TradeAccount.Code, Code, Ticker);
            if (Position == null)
                throw new NullReferenceException("Position is Null");
            if (Position.PositionTotal == null)
                throw new NullReferenceException("PositionTotal is Null");

            PositionTotal = Position.PositionTotal;

            //  TimePlan = TradeContext.RegisterTimePlanStatusChangedEventHandler("Forts.AllDay",
            //                                                                    TimeIsChangedEventHandler);
            if (string.IsNullOrWhiteSpace(TimePlanKey))
                throw new NullReferenceException("TimePlanKey is Empty");

            //TimePlan = TradeContext.RegisterTimePlanEventHandler(TimePlanKey,TimePlanEventHandler);
            //if (TimePlan == null)
            //    throw new NullReferenceException("TimePlan is Empty");

            // TimePlan = TradeContext.TimePlans.GetTimePlan("Forts.Standard");
            // TimePlan.TimeStatusIsChangedEvent += TimeIsChangedEventHandler;
            var off = 1000;
            if (TimeInt == 1) off = 0;
            TradeContext.StrategyProcess.RegisterProcess(Code, Key, (int)(TimeInt * 1000 / 2), off, null, MainBase, Finish);
            TradeContext.StrategyProcess.RegisterProcess(Code + ".Exit", Key + ".Exit", 1000 * 15, off, null, Exit, null);

            Orders = TradeTerminal.Type == Trade.TradeTerminalType.Simulator
                ? TradeContext.SimulateOrders
                : TradeContext.Orders;

            EntryEnabled = true;
        }
        public void MainBase()
        {
            GetActiveOrders();
            Main();
        }

        abstract public void Main();
        virtual public void Finish()
        {
            CloseAll();
            TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Name, "Finish",
                                         ToString(), "");
        }

        public void SetParent(Strategies ss)
        {
            Strategies = ss;
        }
        public void SetTradeContext(ITradeContext tx)
        {
            TradeContext = tx;
        }
        /*
        protected static long GetUnicID()
        {
            var dt = DateTime.Now;
            IncTradeNumber();
            long lo =  (long) (TradeNumber) + (long)(dt.Millisecond) * 100 + (long)(dt.Second) * 100 * 1000 + (long)(dt.Minute) * 100 * 1000 * 100 + (long)(dt.Hour) * 100 * 1000 * 100 * 100;
            return lo;
        }
        */
        /*
        private static void IncTradeNumber()
        {
            if (TradeNumber < 99) ++TradeNumber;
            else TradeNumber = 0;
        }
         */
        public void SetStopOrderFilledStatus(bool filled)
        {
            _stopOrderFilledStatus = filled;
        }
        public bool ExitInEmergencyWhenStopUnFilled()
        {
            if (!IsStopOrderFilled) return false;

            TradeTerminal.KillAllOrders(null, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                            OrderTypeEnum.All, OrderOperationEnum.All);

            if (Position.Pos < 0)
            {
                /*
                TradeTerminal.KillAllOrders(Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                            OrderTypeEnum.Limit, OperationEnum.All);
                TradeTerminal.KillAllOrders(Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                            OrderTypeEnum.StopLimit, OperationEnum.Sell);
                */
                TradeTerminal.SetLimitOrder(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
                                            OrderOperationEnum.Buy,
                                            Ticker.MarketPriceBuy, Position.Quantity,
                                            DateTime.MaxValue,
                                            "Fatal: Close UnFilled BuyStop");

                TradeContext.EventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING, Name, Name, Code,
                                              "Close UnFilled BuyStop", "");
                //SetStopOrderFilledStatus(false);
                //return true;
            }
            if (Position.Pos > 0)
            {
                /*
                TradeTerminal.KillAllOrders(Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                            OrderTypeEnum.Limit, OperationEnum.All);
                TradeTerminal.KillAllOrders(Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                            OrderTypeEnum.StopLimit, OperationEnum.Buy);
                */
                TradeTerminal.SetLimitOrder(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
                                            OrderOperationEnum.Sell,
                                            Ticker.MarketPriceSell, Position.Quantity,
                                            DateTime.MaxValue,
                                            "Fatal: Close UnFilled SellStop");

                TradeContext.EventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING, Name, Name, Code,
                                              "Close UnFilled SellStop", "");
                //SetStopOrderFilledStatus(false);
                //return true;
            }
            SetStopOrderFilledStatus(false);
            return true;
        }
        protected void KillAllOrder()
        {
            TradeTerminal.KillAllOrders(null,Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                            OrderTypeEnum.All, OrderOperationEnum.All);
        }
        protected void KillOrders(OrderTypeEnum ordType, OrderOperationEnum operation)
        {
            TradeTerminal.KillAllOrders(null, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                            ordType, operation);
        }

        public void CloseAll()
        {
            SetStopOrderFilledStatus(false);

            //TradeTerminal.KillAllOrders(Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
            //                                OrderTypeEnum.All, OperationEnum.All);
            GetActiveOrders();
            KillAllOrders();

            TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTickerString, StrategyTickerString, "Close All", "Cancel All Orders", "");

            var operation = OrderOperationEnum.Unknown;
            var price = 0d;
            string priceStr;

            if (Position.IsLong)
            {
                operation = OrderOperationEnum.Sell;
                price = Ticker.MarketPriceSell;
            }
            else if (Position.IsShort)
            {
                operation = OrderOperationEnum.Buy;
                price = Ticker.MarketPriceBuy;
            }

            if (operation == OrderOperationEnum.Unknown) return;

            if (string.IsNullOrWhiteSpace(Ticker.FormatF))
                throw new NullReferenceException(Ticker.Code + ": Ticker Format is Null");

            priceStr = price.ToString(Ticker.FormatF);

            TradeTerminal.SetLimitOrder(null, TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
                                            operation,
                                            price, priceStr, Position.Quantity,
                                            DateTime.MaxValue,
                                            "Close All");

            //   TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, Code + "." + Ticker.Code, "Cloase All", "Close Positions", "");

        }
        public void CloseAll(int mode)
        {
            SetStopOrderFilledStatus(false);

            // TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTickerString, 
            //     "Start Close All", "ExitMode=" + mode, "");

            GetActiveOrders();
            KillAllOrders();

            var operation = OrderOperationEnum.Unknown;
            var price = 0d;
            string priceStr;

            if (mode == 1 || mode == 2)
            {
                if (Position.IsLong)
                {
                    operation = OrderOperationEnum.Sell;
                    price = mode == 1 ? Ticker.MarketPriceSell : Ticker.Ask;
                }
                else if (Position.IsShort)
                {
                    operation = OrderOperationEnum.Buy;
                    price = mode == 1 ? Ticker.MarketPriceBuy : Ticker.Bid;
                }
            }
            else if (mode == 3)
            {
                if (Position.IsLong)
                {
                    operation = OrderOperationEnum.Sell;
                    price = Ticker.Ask + Volatility;
                    price = Ticker.ToMinMove(price, -1);
                }
                else if (Position.IsShort)
                {
                    operation = OrderOperationEnum.Buy;
                    price = Ticker.Bid - Volatility;
                    price = Ticker.ToMinMove(price, +1);
                }
            }

            if (operation == OrderOperationEnum.Unknown) return;

            if (string.IsNullOrWhiteSpace(Ticker.FormatF))
                throw new NullReferenceException(Ticker.Code + ": Ticker Format is Null");

            priceStr = price.ToString(Ticker.FormatF);

            TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTickerString, StrategyTickerString,
                "Close All Process", String.Format("ExitMode={0}; Price={1}; Pos={2}; Bid={3}; Ask={4}; Vol={5}",
                         mode, priceStr, Position.Operation, Ticker.Bid, Ticker.Ask, Volatility.ToString(Ticker.FormatF)), "");

            TradeTerminal.SetLimitOrder(null, TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
                                            operation,
                                            price, priceStr, Position.Quantity,
                                            DateTime.MaxValue,
                                            "Close All");



            //   TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, Code + "." + Ticker.Code, "Cloase All", "Close Positions", "");

        }

        public virtual void StopOrderFilledEventHandler(Order o)
        {
        }
        public virtual void LimitOrderFilledEventHandler(Order o)
        {
        }
        /*
        public virtual void TimeIsChangedEventHandler( DateTime dt, TimePlan.Status timeStatus)
        {
            if ( TimePlan.IsTimeToRest )
                CloseAll();
        }
        */
        // 15.11.14 Remove with Plans to Remove this class
        //public virtual void TimePlanEventHandler(object sender, ITimePlanEventArgs args)
        //{
        //    var tpie = (TimePlanItemEvent)sender;

        //    //TradeContext.Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, StrategyTickerString, tpie.Key,
        //    //    args.Description, args.ToString());

        //    switch (tpie.Code.Trim().ToUpper())
        //    {
        //        case "TOEND":
        //            switch (tpie.Msg.Trim().ToUpper())
        //            {
        //                case "15_SEC":
        //                    break;
        //                case "30_SEC":
        //                    SetExitMode(1, args.ToString());
        //                    break;
        //                case "1_MIN":
        //                    break;
        //                case "2_MIN":
        //                    SetExitMode(2, args.ToString());
        //                    break;
        //                case "3_MIN":
        //                    break;
        //                case "5_MIN":
        //                    SetExitMode(3, args.ToString());
        //                    break;
        //            }
        //            break;
        //        case "AFTER":
        //            switch (tpie.Msg.Trim().ToUpper())
        //            {
        //                case "0_SEC":
        //                case "0_MIN":
        //                    SetExitMode(0, args.ToString());
        //                    break;
        //                case "15_SEC":
        //                    break;
        //                case "30_SEC":
        //                    break;
        //                case "1_MIN":
        //                    break;
        //                case "2_MIN":
        //                    break;
        //                case "3_MIN":
        //                    break;
        //                case "5_MIN":
        //                    break;
        //            }
        //            break;
        //    }
        //}

        public IList<IOrder> MyActiveOrders
        {
            get
            {
                GetActiveOrders();
                return MyOrderCollection;
            }
        }
        private void GetActiveOrders()
        {
            MyOrderCollection.Clear();
            lock (OrderCollectionLocker)
            {
                Orders.GetOrders(OrderTypeEnum.All, OrderOperationEnum.All, OrderStatusEnum.Activated, TradeKey,
                                              MyOrderCollection);
            }
        }
        protected bool KillAllOrders()
        {
            var ret = true;
            lock (OrderCollectionLocker)
            {
                foreach (var o in MyOrderCollection)
                    if (o.IsLimit)
                    {
                        TradeTerminal.KillLimitOrder(null, null, Ticker.ClassCode, Ticker.Code, o.Number);
                        ret = false;
                    }
                    else if (o.IsStopLimit)
                    {
                        TradeTerminal.KillStopOrder(Ticker.ClassCode, Ticker.Code, o.Number);
                        ret = false;
                    }
            }
            return ret;
        }
        private void SetExitMode(int mode, string msg)
        {
            ExitMode = mode;
            EntryEnabled = mode <= 0;

            TradeContext.Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategies",
                StrategyTickerString, mode > 0 ? "Exit Process" : "Normal Process",
                    String.Format("ExitMode={0}; EntryMode={1} Position={2}",
                        ExitMode, EntryEnabled, Position.Operation), msg);
        }

        private void Exit()
        {
            if (ExitMode <= 0)
                return;
            CloseAll(ExitMode);
        }
        private void Entry()
        {
            TryToEntry(EntryMode);
        }
        void TryToEntry(int entryMode)
        {
            switch (entryMode)
            {
                case -1:
                    KillAllOrders();
                    break;
                case 0:
                    break;
            }
        }
        private float _lastBuyLimitPrice;
        private float _lastSellLimitPrice;
        protected virtual void TrySetOrder(OrderOperationEnum operation, float price, long contracts, string comment)
        {
            if (IsOrderAlreadyExist(operation, price)) return;

            if (KillAllOrders())
                SetOrders();
            else
            {
                SaveToQueue(operation, price, contracts, comment);
            }
        }

        private void SetOrders()
        {
            foreach(var o in OrdersToSet)
                SetOrder(o.Operation, o.Price, o.Contracts, o.Comment);
        }
        private void SetOrder(OrderOperationEnum operation, float price, long contracts, string comment)
        {
            var priceStr = price.ToString(Ticker.FormatF);

            TradeTerminal.SetLimitOrder(null, TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
                                        operation,
                                        price, priceStr, contracts,
                                        DateTime.Today.Add(new TimeSpan(23, 49, 15)),
                                        comment);
        }

        private void SaveToQueue(OrderOperationEnum op, float price, long contracts, string comment)
        {
            OrdersToSet.Enqueue(new OrderTemp{ Operation = op, Price = price, Contracts = contracts, Comment = comment });
        }

        protected bool IsOrderAlreadyExist(OrderOperationEnum operation, float price)
        {
            bool ret;
            lock (OrderCollectionLocker)
            {
                ret = MyOrderCollection.Any(o => o.IsValid &&
                                                    o.Operation == operation &&
                                                    o.LimitPrice.CompareTo((double)price) == 0);

            }
            return ret;
        }
        public struct OrderTemp
        {
            public OrderOperationEnum Operation;
            public float Price;
            public long Contracts;
            public string Comment;
        }

    }
}
