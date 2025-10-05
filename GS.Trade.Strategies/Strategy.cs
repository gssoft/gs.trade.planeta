using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Containers;
using GS.Elements;
using GS.Events;
using GS.Exceptions;
using GS.Extension;
using GS.Interfaces;
using GS.Serialization;
using GS.Status;
// using GS.Time.TimePlan;
using GS.Trade.Data;
using GS.Trade.Interfaces;
using GS.Trade.Data.Studies;
using GS.Trade.Strategies.Managers;
using GS.Trade.Trades;
using GS.Trade.Trades.Orders3;
using GS.Trade.Trades.Trades3;
using GS.Works;
using EventArgs = GS.Events.EventArgs;

// using TimePlanEventArgs = GS.Time.TimePlan.TimePlanEventArgs;

namespace GS.Trade.Strategies
{
    public struct OrderParameters
    {
        public double Price;
        public OrderOperationEnum TradeOperation;
        public int Size;
    }

    public abstract partial class Strategy : Element1<string>, IStrategy
    {
        [XmlIgnore]
        public IEventHub EventHub { get; set; }

        //public event EventHandler<GS.Events.IEventArgs> ExceptionEvent;
        //public virtual void OnExceptionEvent(IEventArgs e)
        //{
        //    EventHandler<IEventArgs> handler = ExceptionEvent;
        //    if (handler != null) handler(this, e);
        //}
        public void SetParent(IStrategies ss)
        {
                Parent = ss;
                // 23.09.28
                Strategies = ss;
        }

        public int Id { get; set; }

        public override string Key =>
            $"[Type={GetType()}; Name={Name}; Code={Code}; " +
            $"TradeAccountKey={RealAccountKey}; TickerKey={RealTickerKey}; TimeInt={TimeInt}]";

        protected bool Working;
        public void SetWorkingStatus(bool status, string reason)
        {
            var oldWorkingStatus = Working;
            Working = status;

            if (Working != oldWorkingStatus)
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategies",
                    StrategyTickerString, $"Set Working Status to: {Working}, Reason: {reason}",
                    "", ToString());
        }

        //public string Name { get; set; }
        //public string Code { get; set; }
        //public string Alias { get; set; }

        /*
        public bool IsLongEntryEnabled { get; protected set; }
        public bool IsShortEntryEnabled { get; protected set; }
        */
        public string TickerKey { get; set; }
        public string TickerBoard { get; set; }
        public string TradeAccountKey { get; set; }
        public string TradeTerminalType { get; set; }
        public string TradeTerminalKey { get; set; }
        public string TimePlanKey { get; set; }
        public long Contracts { get; set; }
        public long Contract { get; set; }
        public long SafeContracts => (long) Math.Round(Contracts*0.38);

        public bool PortfolioEnable { get; set; }

        public uint SignalMap { get; set; }

        [XmlIgnore] public IAccount TradeAccount;
        protected IAccount StrategyAccount;
        public string RealAccountKey => TradeAccount == null ? TradeAccountKey : TradeAccount.Key;
        public string RealTickerKey => Ticker == null ? TickerKey : Ticker.Key;

        public string TradeAccountCode => TradeAccount == null ? "" : TradeAccount.Code;
        public string TickerTradeBoard => Ticker != null ? Ticker.ClassCode : TickerBoard;
        public string TickerCode => Ticker != null ? Ticker.Code : TickerKey;

        protected ITimePlan TimePlan;

        protected List<IPosition> MyPositionCollection = new List<IPosition>();
        protected object PositionCollectionLocker = new object();
        protected List<IOrder> MyOrderCollection = new List<IOrder>();
        protected object OrderCollectionLocker = new object();

        protected IOrders Orders;
        protected Orders3 ActiveOrderCollection;

        protected IDeals Deals;

        //public IEnumerable<Containers3.IContainerItem<string>> ActiveOrders
        //{
        //    get
        //    {
        //        return (ActiveOrderCollection.Items.Where(o => ((IOrder)o).Status == OrderStatusEnum.Activated)); //as IEnumerable<IOrder>; 
        //    }
        //}
        public IEnumerable<IOrder3> ActiveOrders => ActiveOrderCollection?.ActiveOrders;
        public IEnumerable<IOrder3> ActiveOrdersSoft => ActiveOrderCollection?.ActiveOrdersSoft;
        public IEnumerable<IOrder3> ValidOrders => ActiveOrderCollection?.ValidOrders;
        public IEnumerable<IOrder3> ValidOrdersSoft => ActiveOrderCollection?.ValidOrdersSoft;

        public IEnumerable<IOrder3> ClosedOrders => ActiveOrderCollection?.ClosedOrders;
        public IEnumerable<IOrder3> ClosedOrdersSoft => ActiveOrderCollection?.ClosedOrdersSoft;
        [XmlIgnore]
        public string StrategyTickerString => Code + "@" + TimeInt + "@" + TickerKey;
        [XmlIgnore]
        public string StrategyTimeIntTickerString => Code + "@" + TimeInt + "@" + TickerKey;
        [XmlIgnore]
        public IStrategies Strategies { get;  set; }

        [XmlIgnore]
        public ITradeTerminal TradeTerminal { get; set; }
        [XmlIgnore]
        public ITicker Ticker { get; set; }
        [XmlIgnore]
        public IAccount Account => TradeAccount;
        public int TimeInt { get; set; }
        public string TimeIntKey => TimeInt.ToString(CultureInfo.InvariantCulture);
        public string PortfolioKey { get; set; }
        /// <summary>
        ///  PorfolioRisk
        ///  Get Available Longs and Contracts Quantity
        /// </summary>
        [XmlIgnore]
        public IPortfolio Portfolio { get; set; }
        [XmlIgnore]
        public IPortfolioRisk PortfolioRisk { get; set; }
        public bool IsPortfolioRiskEnable => PortfolioEnable && PortfolioRisk != null;
        [XmlIgnore]
        public int LongSideRequest { get; private set; }
        [XmlIgnore]
        public int ShortSideRequest { get; private set; }
        [XmlIgnore]
        public int LongContractsRequest { get; private set; }
        [XmlIgnore]
        public int ShortContractsRequest { get; private set; }
        public long LongRequest(int contracts)
        {
            if (!IsPortfolioRiskEnable)
                return contracts;
            LongContractsRequest = contracts;

            // 15.09.26
            if (!PortfolioRisk.IsLongEnabled)
                return 0;
            // SkipTheTick with the Same TimeInts 
            var stratToSkip = PortfolioRisk.Items
                .Where(s => s.Key != Key && s.TimeInt == TimeInt && s.Position.IsNeutral);
            foreach (var s in stratToSkip)
                s.SkipTheTick(2);
            return contracts;
            // return PortfolioRisk.IsLongEnabled ? contracts : 0;
        }
        public long ShortRequest(int contracts)
        {
            if (!IsPortfolioRiskEnable)
                return contracts;

            ShortContractsRequest = contracts;

            // 15.09.26
            if (!PortfolioRisk.IsShortEnabled)
                return 0;
            // SkipTheTick with the Same TimeInts 
            var stratToSkip = PortfolioRisk.Items
                .Where(s => s.Key != Key && s.TimeInt == TimeInt && s.Position.IsNeutral);
            foreach (var s in stratToSkip)
                s.SkipTheTick(2);
            return contracts;

            // return PortfolioRisk.IsShortEnabled ? contracts : 0;
        }
        public long BuySellRequest(long contracts)
        {
            if (!IsPortfolioRiskEnable || !PortfolioRisk.IsBuySellRequestEnabled(this))
                return 0;
            //PortfolioRisk.SkipTheTickToOthers(Key);
            PortfolioRisk.SkipTheTickToOthers(this, 2, false, false);

            return contracts;
        }
        
        
        public void ClearBuySellRequests()
        {
            BuyContractsRequest = 0; SellContractsRequest = 0;
        }
        public void ClearLongShortRequests()
        {
            LongContractsRequest = 0;
            ShortContractsRequest = 0;
        }

        /// <summary>
        /// Position Info
        /// </summary>
        public bool IsPosNeutral => Position?.IsNeutral ?? false;
        public bool IsPosOpened => Position?.IsOpened ?? false;
        public bool IsPosLong => Position?.IsLong ?? false;
        public bool IsPosShort => Position?.IsShort ?? false;

        public string EntryManagerKey { get; set; }

        [XmlIgnore]
        public IEntryManager EntryManager { get; set; }

        [XmlIgnore]
        public bool IsWrong { get; protected set; }

        [XmlIgnore]
        public IPosition2 Position { get; set; }

        //[XmlIgnore]
        //protected IPosition2 PositionLastClosed { get; set; }

        protected IDeal LastDeal { get; set; }
        public bool IsLastDealFinResultPositive => LastDeal?.IsFinResultPositive ?? false;
        public bool IsLastDealFinResultNegative => LastDeal?.IsFinResultNegative ?? false;


        //[XmlIgnore]
        //public IPosition2 Position {
        //    get { return Position2; }
        //    set { Position2 = value; }
        //}

        //[XmlIgnore]
        //public IPosition2 Position2 { get; set; }


        [XmlIgnore]
        // public Positions.Totals.PositionTotal PositionTotal { get; set; }
        public IPosition2 PositionTotal { get; set; }

        protected int PositionQuantityPrev;

        protected ITradeContext TradeContext;
        //protected Strategies Strategies;

        private bool _stopOrderFilledStatus;

        public bool IsStopOrderFilled => _stopOrderFilledStatus;

        public IList<IOrder> MyOrders
        {
            get
            {
                MyOrderCollection.Clear();
                TradeContext.Orders.GetOrders(OrderTypeEnum.All, OrderOperationEnum.All, OrderStatusEnum.All, TradeKey,
                    MyOrderCollection);
                return MyOrderCollection;
            }
        }

        public IEnumerable MyOrders2
        {
            // Proba
            get { return from o in MyOrders select o; }
        }

        protected TradeOperationEnum LastTradeOperation;
        protected float LastLongEntryPrice;
        protected float LastShortEntryPrice;
        protected PositionChangedEnum LastPositionChanged;

        [XmlIgnore]
        public abstract IBars Bars { get; protected set; }

        [XmlIgnore]
        public abstract int MaxBarsBack { get; protected set; }

        public string StrategyKey => TradeAccount.Code + Code + TickerKey;
        public string TradeKey => Code + TradeAccount.Code + TickerKey;

        public override string ToString()
        {
            return
                $"Code:{Code} Name:{Name} " +
                $"Acnt:{(TradeAccount != null ? TradeAccount.Code : TradeAccountKey)} " +
                $"Ticker:{(Ticker != null ? Ticker.Code : TickerKey)} TradeBoard:{TickerTradeBoard} Contracts:{Contracts}";
        }

        [XmlIgnore]
        public virtual Atr Atr => null;

        public virtual float Volatility => 0f;

        public virtual string PositionInfo => "";

        protected int ExitMode;
        protected int EntryMode;

        [XmlIgnore]
        public bool MaxContractsReached { get; protected set; }

        [XmlIgnore]
        public bool EntryEnabled { get; set; }

        [XmlIgnore] public DateTime EntryEnabledDT;

        [XmlIgnore]
        public bool EntryPortfolioEnabled { get; set; }

        [XmlIgnore]
        public bool ExitEnabled { get; set; }

        [XmlIgnore]
        public bool ShortEnabled { get; set; }

        [XmlIgnore]
        public bool LongEnabled { get; set; }

        [XmlIgnore]
        public float LongEntryLevel { get; set; }

        [XmlIgnore]
        public float ShortEntryLevel { get; set; }

        protected Strategy()
        {
            // TradeAccount = new Account { Name = "Quik.Open", Code = "SPBFUT00S98" };
            // TradeAccount = new Account { Name = "Quik.Junior", Code = "SPBFUT00082" };
        }

        protected Strategy(ITradeContext tx, string name, string code, ITicker ticker, uint timeInt)
        {
            if (tx == null) throw new NullReferenceException("TradeCOntext == null");
            TradeContext = tx;
            Name = name;
            Code = code + "." + timeInt;
            Ticker = ticker;
            TimeInt = (int) timeInt;

        }
        public override void Init()
        {
            try
            {
                ActiveOrderCollection = new Orders3();

                OrderRegisteredExpireTimeOutSec = new TimeSpan(0, 0, 2 * TimeInt);
                OrderSendedExpireTimeOutSec = new TimeSpan(0, 5, 0);
                OrderSendTimeOutExpireTimeSpan = new TimeSpan(0, 5, 0);

                TradeAccount = TradeContext.RegisterAccount(TradeAccountKey);
                //TradeAccount = TradeContext.RegisterAccount(TradeAccountKey);
                if (TradeAccount == null)
                {
                    // IsWrong = true; return;
                    throw new NullReferenceException("TradeAccount == null. TradeAccountKey=" + TradeAccountKey);
                }

                // Ticker = TradeContext.RegisterTicker(TickerKey) as Ticker;
                //Ticker = TradeContext.GetTicker(TickerKey);
                Ticker = TradeContext.RegisterTicker(TickerBoard, TickerKey);
                if (Ticker == null)
                {
                    //IsWrong = true; return;
                    throw new NullReferenceException("Ticker == null. TickerKey=" + TickerKey);
                }
                //Ticker.RegisterBarSeries("Bars" + TimeInt.ToString(CultureInfo.InvariantCulture), TimeInt, 0  );
                
                //PositionTotal = TradeContext.Positions.PositionTotals.Register(Position);
                //Position = TradeContext.RegisterPosition(TradeAccount.Code, Code, Ticker);
                Position = TradeContext.RegisterPosition(this);
                if (Position == null)
                    throw new NullReferenceException("Position is Null");

                PositionTotal = Position.PositionTotal ?? throw new NullReferenceException("PositionTotal is Null");
                Deals = TradeContext.BuildDeals();

                //Position2 = TradeContext.RegisterPosition(this);
                //Position2.PositionTotal = PositionTotal;

                EventHub = Builder.Build<EventHub>(@"Init\EventHubStrat.xml", "EventHub");
                // EventHub.Parent = this;
                EventHub.Init(TradeContext.EventLog);
                // EventHub.Parent = this;
                // EventHub.WhoAreYou();
                //EventHub.ExceptionEvent += ExceptionRegister;   // Internal EventHub Exception to TradeContext
                //EventHub.Subscribe("UI.EXCEPTIONS", "EXCEPTION", ExceptionRegister);  // Exception from Others to TradeContext

                // EventHub.Subscribe(StrategyTimeIntTickerString, "ORDER.REGISTER", OnOrderRegister);
                EventHub.Subscribe(StrategyTimeIntTickerString, "ORDER.TRANSSEND", OnOrderTransactionSend);
                EventHub.Subscribe(StrategyTimeIntTickerString, "ORDER.TRANSREPLY", OnOrderTransactionReply);
                EventHub.Subscribe(StrategyTimeIntTickerString, "ORDER.STATUSCHANGED", OnOrderStatusChanged);
                EventHub.Subscribe(StrategyTimeIntTickerString, "TRADEREPLY", OnTradeReply);
                // From "TradeTerminal", "ConnectionStatusChanged",
                EventHub.Subscribe("TradeTerminal", "ConnectionStatusChanged", OnTradeTerminalConnectionStatusChanged);

                //EventHub.Subscribe(StrategyTimeIntTickerString, "ORDER.TRANSREPLY", OnOrderTransReplyTest);
                //EventHub.Subscribe(StrategyTimeIntTickerString, "ORDER.STATUSCHANGED", OnOrderStatusChangedTest);
                
                //  TimePlan = TradeContext.RegisterTimePlanStatusChangedEventHandler("Forts.AllDay",
                //                                                                    TimeIsChangedEventHandler);
                if (string.IsNullOrWhiteSpace(TimePlanKey))
                    throw new NullReferenceException("TimePlanKey is Empty");

                if (TimePlanKey.HasValue())
                {
                    TimePlan = TradeContext.RegisterTimePlanEventHandler(TimePlanKey, TimePlanEventHandler);
                    if (TimePlan == null)
                        throw new NullReferenceException("TimePlan is Empty");
                }
              
                    // var totalseconds = OrderSendedExpireTimeOutSec.TotalSeconds;
                // TimePlan = TradeContext.TimePlans.GetTimePlan("Forts.Standard");
                // TimePlan.TimeStatusIsChangedEvent += TimeIsChangedEventHandler;

                //if (PortfolioKey.HasValue())
                //{
                //    var p = PortfolioKey + "." + TickerKey;
                //    Portfolio = TradeContext.RegisterPortfolio(p, "Name=" + p);
                //    if( Portfolio == null)
                //        throw new NullReferenceException("Portfolios: Register Portfolio Failure: " + p);
                //    var s = Portfolio.Register(this);
                //    if( s == null)
                //        throw new NullReferenceException("Portfolio: Register Strategy Failure: "  + p);
                //}

                //if (EntryManagerKey.HasValue())
                //{
                //    var p = EntryManagerKey + "." + TickerKey;
                //    EntryManager = TradeContext.RegisterEntryManager(p, "Name=" + p);
                //    if (EntryManager == null)
                //        throw new NullReferenceException("EntryManagers: Register EntryManager Failure: " + p);
                //    var s = EntryManager.Register(this);
                //    if (s == null)
                //        throw new NullReferenceException("EntryManager: Register Strategy Failure: " + p);
                //}

                var off = 1000;
                if (TimeInt == 1) off = 0;
                if (TradeContext.StrategyProcess != null)
                {
                    // 2016.12.06
                    // TradeContext.StrategyProcess.RegisterProcess(StrategyTickerString, Key, (int) (TimeInt*1000), off, null, MainBase, Finish);
                    const int timeInterval = 1000;
                    //TradeContext.StrategyProcess.RegisterProcess(StrategyTickerString, Key, timeInterval, off, null,
                    //    MainBase, Finish);
                    TradeContext.StrategyProcess.RegisterProcess(StrategyTickerString, Key, timeInterval, off, null,
                        MainBase, null);
                    TradeContext.StrategyProcess.RegisterProcess(StrategyTickerString + ".Exit", Key + ".Exit", 1000*15,
                        off, null, Exit, null);
                }

                TradeTerminal = TradeContext.RegisterTradeTerminal(TradeTerminalType, TradeTerminalKey);
                TradeTerminal.TradeEntityChangedEvent += EventHub.EnQueue;
                // TradeTerminal.ChangedEvent += EventHub.EnQueue;
                // TradeTerminal.ExceptionEvent += EventHub.FireEvent;

                Orders = TradeTerminal.Type == Trade.TradeTerminalType.Simulator
                   ? TradeContext.SimulateOrders
                   : TradeContext.Orders;

                EntryEnabled = true;

                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategies",
                    StrategyTimeIntTickerString, "StrategyBase.Init() Is Completed", "", ToString());

            }
            catch (Exception e)
            {
                SendExceptionMessage3(StrategyTickerString, "Strategy", "Strategy.Base.Init()", ToString(), e);
                // throw;
                //throw new NullReferenceException("Strategy.Init() Failure: " + StrategyTickerString);
                SendException(e);
            }
        }

        public void MainBase()
        {
            var method = MethodBase.GetCurrentMethod()?.Name + "()";

            //GetActiveOrders();
            //UpdateFromLastTick();
            // Two Times with KillOrders in Z007
            EventHub?.DeQueueProcess();
            // Remove Filled, Canceled, Rejected, NotSent
            RemoveClosedOrders();
            // Remove OrdersRegisteredReadyToUse When No Connection 
            // RemoveRegisteredWhenNoConnection(string.Empty);
            // ****** RemoveOrdersSendedTimeOut(DateTime.Now);
            RemoveOrdersSendTimeOut(DateTime.Now);
            // ReTry Cancel ActiveOrdersSoftReadyToUse which Not Seneded 
            ReTryToCancelActiveOrders();
            // ReTry SetOrder2 for RegisteredReadyToUse in AciveOrderCollection (PocketOrder Implementation)
            TryToSetOrderRegisteredPending();

            VerifyOrderList();

            Main();

            //FireChangedEventToStrategy(this, "ORDER.TRANSREPLY", "TestReplyOperation", "TestReplyObject");
            //FireChangedEventToStrategy(this, "ORDER.STATUSCHANGED", "TestStatusChangedOperation",
            //    "TestStatusChangedObject");
        }

        private void FireChangedEventToStrategy(IStrategy strategy, string entity, string operation, object obj)
        {
            strategy?.EnQueue(this, new Events.EventArgs
            {
                Category = strategy.StrategyTimeIntTickerString.TrimUpper(),
                Entity = entity.TrimUpper(),
                Operation = operation.TrimUpper(),
                Object = obj,
                Sender = this
            });
        }
        public virtual void SetPocketOrder()
        {
        }

        public abstract void Main();

        public virtual void Finish()
        {
            Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, StrategyTickerString, "Strategy",
                "Finish Process Started", StrategyTickerString, ToString());
            try
            {
                // CreateStat();
                CloseAll(1);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(StrategyTickerString, "Strategy.Finish(CloseAll(1))", "", "", e);
                // throw;
            }

            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, StrategyTickerString, "Strategy", "Finish Process Completed",
                ToString(), "");
        }

        public void SetParent(Strategies ss)
        {
            Strategies = ss;
            Parent = ss;
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

        public void SetStopOrderFilledStatus(bool filled)
        {
            _stopOrderFilledStatus = filled;
        }

        public bool ExitInEmergencyWhenStopUnFilled()
        {
            if (!IsStopOrderFilled) return false;

            TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
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
            TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                OrderTypeEnum.All, OrderOperationEnum.All);
        }

        protected void KillOrders(OrderTypeEnum ordType, OrderOperationEnum operation)
        {
            TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                ordType, operation);
        }

        public void CloseAll()
        {
            SetStopOrderFilledStatus(false);

            //TradeTerminal.KillAllOrders(Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
            //                                OrderTypeEnum.All, OperationEnum.All);
            GetActiveOrders();
            KillAllOrders();

            TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTickerString,
                StrategyTickerString, "Close All", "Cancel All Orders", "");

            var operation = OrderOperationEnum.Unknown;
            //var price = 0d;
            //string priceStr;

            if (Position.IsLong)
            {
                operation = OrderOperationEnum.Sell;
                //price = Ticker.MarketPriceSell;
            }
            else if (Position.IsShort)
            {
                operation = OrderOperationEnum.Buy;
                //price = Ticker.MarketPriceBuy;
            }

            if (operation == OrderOperationEnum.Unknown) return;

            if (string.IsNullOrWhiteSpace(Ticker.FormatF))
                throw new NullReferenceException(Ticker.Code + ": Ticker Format is Null");

            //priceStr = price.ToString(Ticker.FormatF);

            //TradeTerminal.SetLimitOrder(this, TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
            //                                operation,
            //                                price, priceStr, Position.Quantity,
            //                                DateTime.MaxValue,
            //                                "Close All");

            //   TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, Code + "." + Ticker.Code, "Cloase All", "Close Positions", "");

        }
        // work since 2019.09.20
        public virtual void CloseAll(int mode)
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";
            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, "Strategies", StrategyTickerString, methodname, "", "");
            try
            {
                if (Position.IsNeutral) return;

                var operation = OrderOperationEnum.Unknown;
                var price = 0d;

                long quantity = 0;

                switch (mode)
                {
                    case 2:
                    case 1:
                        if (Position.IsLong)
                        {
                            operation = OrderOperationEnum.Sell;
                            price = mode == 1 ? Ticker.MarketPriceSell : Ticker.Ask;
                            quantity = Position.Quantity;

                        }
                        else if (Position.IsShort)
                        {
                            operation = OrderOperationEnum.Buy;
                            price = mode == 1 ? Ticker.MarketPriceBuy : Ticker.Bid;
                            quantity = Position.Quantity;
                        }
                        break;
                    case 3:
                        if (Position.IsLong)
                        {
                            operation = OrderOperationEnum.Sell;
                            price = Ticker.Ask + Volatility;
                            price = Ticker.ToMinMove(price, -1);
                            quantity = Position.Quantity;
                        }
                        else if (Position.IsShort)
                        {
                            operation = OrderOperationEnum.Buy;
                            price = Ticker.Bid - Volatility;
                            price = Ticker.ToMinMove(price, +1);
                            quantity = Position.Quantity;
                        }
                        break;
                    case 12:
                    case 11:
                        if (Position.IsLong)
                        {
                            operation = OrderOperationEnum.Sell;
                            price = mode == 11 ? Ticker.MarketPriceSell : Ticker.Ask;
                            quantity = (Position.Quantity - SafeContracts) > 0 ? (Position.Quantity - SafeContracts) : 0;
                        }
                        else if (Position.IsShort)
                        {
                            operation = OrderOperationEnum.Buy;
                            price = mode == 11 ? Ticker.MarketPriceBuy : Ticker.Bid;
                            quantity = (Position.Quantity - SafeContracts) > 0 ? (Position.Quantity - SafeContracts) : 0;
                        }
                        break;
                    case 13:
                        if (Position.IsLong)
                        {
                            operation = OrderOperationEnum.Sell;
                            price = Ticker.Ask + Volatility;
                            price = Ticker.ToMinMove(price, -1);
                            quantity = (Position.Quantity - SafeContracts) > 0 ? (Position.Quantity - SafeContracts) : 0;
                        }
                        else if (Position.IsShort)
                        {
                            operation = OrderOperationEnum.Buy;
                            price = Ticker.Bid - Volatility;
                            price = Ticker.ToMinMove(price, +1);
                            quantity = (Position.Quantity - SafeContracts) > 0 ? (Position.Quantity - SafeContracts) : 0;
                        }
                        break;
                }
                if (quantity <= 0) return;
                if (operation == OrderOperationEnum.Unknown) return;

                if (string.IsNullOrWhiteSpace(Ticker.FormatF))
                    throw new NullReferenceException(Ticker.Code + ": Ticker Format is Null");

                var priceStr = price.ToString(Ticker.FormatF);

                //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTickerString, StrategyTickerString,
                //    methodname,
                //    $"ExitMode={mode}; Price={priceStr}; Pos={Position.PosOperation}; Quant = {quantity};",
                //    "Bid={Ticker.Bid}; Ask={Ticker.Ask}; Vol={Volatility.ToString(Ticker.FormatF)}");
                
                SetOrder2(operation, quantity, price);

                //var o = ActiveOrderCollection.RegisterOrder(this, operation, OrderTypeEnum.Limit, 0, price, quantity, "");
                //TradeTerminal.SetLimitOrder(o);

                // Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTickerString, "Close All", "Close Positions", "","");
            }
            catch (Exception e)
            {
                SendExceptionMessage3(StrategyTickerString, "Strategy", methodname, ToString(), e);
               // throw;
            }
        }
        // work until 2019.09.20
        public virtual void CloseAll1(int mode)
        {
            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, "Strategies", StrategyTickerString, "Close All", "", "");
            try
            {

                SetStopOrderFilledStatus(false);

                // TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTickerString, 
                //     "Start Close All", "ExitMode=" + mode, "");

                //GetActiveOrders();
                //  SetKillAllOrdersInQueue();

                KillAllActiveOrders2();

                if (Position == null)
                {
                    var e = new NullReferenceException(StrategyTickerString + ": Position is Null");
                    SendExceptionMessage3(StrategyTickerString, "Strategy.CloseAll(int)", "Position == null",
                        "CloseAll(int)", e);
                    return;
                }

                if (Position.IsNeutral) return;

                var operation = OrderOperationEnum.Unknown;
                var price = 0d;
                string priceStr;

                long quantity = 0;

                switch (mode)
                {
                    case 2:
                    case 1:
                        if (Position.IsLong)
                        {
                            operation = OrderOperationEnum.Sell;
                            price = mode == 1 ? Ticker.MarketPriceSell : Ticker.Ask;
                            quantity = Position.Quantity;

                        }
                        else if (Position.IsShort)
                        {
                            operation = OrderOperationEnum.Buy;
                            price = mode == 1 ? Ticker.MarketPriceBuy : Ticker.Bid;
                            quantity = Position.Quantity;
                        }
                        break;
                    case 3:
                        if (Position.IsLong)
                        {
                            operation = OrderOperationEnum.Sell;
                            price = Ticker.Ask + Volatility;
                            price = Ticker.ToMinMove(price, -1);
                            quantity = Position.Quantity;
                        }
                        else if (Position.IsShort)
                        {
                            operation = OrderOperationEnum.Buy;
                            price = Ticker.Bid - Volatility;
                            price = Ticker.ToMinMove(price, +1);
                            quantity = Position.Quantity;
                        }
                        break;
                    case 12:
                    case 11:
                        if (Position.IsLong)
                        {
                            operation = OrderOperationEnum.Sell;
                            price = mode == 11 ? Ticker.MarketPriceSell : Ticker.Ask;
                            quantity = (Position.Quantity - SafeContracts) > 0 ? (Position.Quantity - SafeContracts) : 0;
                        }
                        else if (Position.IsShort)
                        {
                            operation = OrderOperationEnum.Buy;
                            price = mode == 11 ? Ticker.MarketPriceBuy : Ticker.Bid;
                            quantity = (Position.Quantity - SafeContracts) > 0 ? (Position.Quantity - SafeContracts) : 0;
                        }
                        break;
                    case 13:
                        if (Position.IsLong)
                        {
                            operation = OrderOperationEnum.Sell;
                            price = Ticker.Ask + Volatility;
                            price = Ticker.ToMinMove(price, -1);
                            quantity = (Position.Quantity - SafeContracts) > 0 ? (Position.Quantity - SafeContracts) : 0;
                        }
                        else if (Position.IsShort)
                        {
                            operation = OrderOperationEnum.Buy;
                            price = Ticker.Bid - Volatility;
                            price = Ticker.ToMinMove(price, +1);
                            quantity = (Position.Quantity - SafeContracts) > 0 ? (Position.Quantity - SafeContracts) : 0;
                        }
                        break;
                }

                if (quantity <= 0) return;
                if (operation == OrderOperationEnum.Unknown) return;

                if (string.IsNullOrWhiteSpace(Ticker.FormatF))
                    throw new NullReferenceException(Ticker.Code + ": Ticker Format is Null");

                priceStr = price.ToString(Ticker.FormatF);

                Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTickerString, StrategyTickerString,
                    "Close All Process",
                    $"ExitMode={mode}; Price={priceStr}; Pos={Position.PosOperation}; Quant = {quantity}; Bid={Ticker.Bid}; Ask={Ticker.Ask}; Vol={Volatility.ToString(Ticker.FormatF)}",
                    "");

                //TradeTerminal.SetLimitOrderInQueue(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
                //                                operation,
                //                                price, priceStr, quantity,
                //                                DateTime.MaxValue,
                //                                "Close All");

                //SetOrder2(operation, quantity, price);

                var o = ActiveOrderCollection.RegisterOrder(this, operation, OrderTypeEnum.Limit, 0, price, quantity, "");
                TradeTerminal.SetLimitOrder(o);

                Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTickerString, "Close All", "Close Positions", "",
                    "");
            }
            catch (Exception e)
            {
                SendExceptionMessage3(StrategyTickerString, "Strategy", " CloseAll(int)", ToString(), e);
                throw;
            }
        }

        public virtual int CloseAllSoft()
        {
            if (Position.IsNeutral) return 0;
            if (Ticker.Bid.IsLessOrEqualsThan(0d) || Ticker.Ask.IsLessOrEqualsThan(0d)) return 0;

            var t = new Trade3
            {
                Strategy = this,
                Number = Position.LastTradeNumber++,
                DT = Position.LastTradeDT.AddSeconds(1),
                Operation = Position.IsLong ? TradeOperationEnum.Sell : TradeOperationEnum.Buy,
                Quantity = Position.Quantity,
                Price = Position.IsLong ? (decimal)Ticker.Bid : (decimal)Ticker.Ask,
            };
            var deal = Position.NewTrade(t);
            try
            {
                //TradeContext.TradeWebClient?.Post2(t.DT, t.Number, t.OrderNumber,
                //    t.AccountCode, Id, Code, t.TickerCode,
                //    t.Operation > 0 ? 1 : -1, t.Quantity, (double) t.Price, 0d, "Trade");

                t.Comment += " New.";
                OnChangedEvent(new Events.EventArgs
                {
                    Category = "UI.Trades",
                    Entity = "Trade",
                    Operation = "Update",
                    Object = t
                });

                TradeContext.Publish(t);

                var oldPosition = Position.Clone();
                // var pClosed = Position.NewTrade(t);

                LastDeal = deal;

                PositionIsChangedEventHandler2(oldPosition, Position, Position.LastChangedResult);

                TradeContext.TradeStorage.SaveChanges(StorageOperationEnum.Add, t);

                if (deal != null)
                {
                    OnChangedEvent(new Events.EventArgs
                    {
                        Category = "UI.Deals",
                        Entity = "Deal",
                        Operation = "Add",
                        Object = deal,
                        Sender = this
                    });
                    //TradeContext.Storage.SaveChanges(StorageOperationEnum.Add, t);
                    TradeContext.TradeStorage.SaveChanges(StorageOperationEnum.AddOrUpdate, Position);
                    TradeContext.TradeStorage.SaveTotalChanges(StorageOperationEnum.AddOrUpdate, PositionTotal);
                    TradeContext.TradeStorage.SaveChanges(StorageOperationEnum.Add, deal);

                    TradeContext.Publish(Position);
                    TradeContext.Publish(PositionTotal);
                    TradeContext.Publish(deal);
                }
                else
                {
                    //TradeContext.Storage.SaveChanges(StorageOperationEnum.Add, t);
                    TradeContext.TradeStorage.SaveChanges(StorageOperationEnum.AddOrUpdate, Position);
                    TradeContext.Publish(Position);
                }
                return +1;
            }
            catch (Exception e)
            {
                SendException(e);
                return -1;
            }
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

        public virtual void TimePlanEventHandler(object sender, ITimePlanEventArgs args)
        {
            // var tpie = (TimePlanItemEvent) sender;

            //TradeContext.Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, StrategyTickerString, tpie.Key,
            //    args.Description, args.ToString());
            switch (args.EventType)
            {
                case TimePlanEventType.TimePlanItem:
                    switch (args.TimePlanItemCode)
                    {
                        case "MORNING":
                            switch (args.Msg)
                            {
                                case "START":
                                    SetExitMode(0, args.ToString());
                                    break;
                                case "FINISH":
                                    SetExitMode(0, args.ToString());
                                    break;
                            }
                            break;
                        case "EVENING":
                            switch (args.Msg)
                            {
                                case "START":
                                    SetExitMode(0, args.ToString());
                                    break;
                                case "FINISH":
                                    SetExitMode(0, args.ToString());
                                    break;
                            }
                            break;
                    }
                    break;
                case TimePlanEventType.TimePlanItemEvent:
                    switch (args.TimePlanItemCode)
                    {
                        case "MORNING":
                            switch (args.TimePlanItemEventCode)
                            {
                                case "TOEND":
                                    switch (args.Msg)
                                    {
                                        case "15_SEC":
                                            break;
                                        case "30_SEC":
                                            break;
                                        case "1_MIN":
                                            break;
                                    }
                                    break;
                                case "AFTER":
                                    switch (args.Msg)
                                    {
                                        case "0_SEC":
                                        case "0_MIN":
                                            SetExitMode(0, args.ToString());
                                            break;
                                        case "15_SEC":
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case "EVENING":
                            switch (args.TimePlanItemEventCode)
                            {
                                case "TOEND":
                                    switch (args.Msg)
                                    {
                                        case "15_SEC":
                                            break;
                                        case "30_SEC":
                                            SetExitMode(1, args.ToString());
                                            break;
                                        case "1_MIN":
                                            SetExitMode(1, args.ToString());
                                            break;
                                        case "2_MIN":
                                            SetExitMode(2, args.ToString());
                                            break;
                                        case "3_MIN":
                                            break;
                                        case "5_MIN":
                                            SetExitMode(3, args.ToString());
                                            break;
                                    }
                                    break;
                                case "AFTER":
                                    switch (args.Msg)
                                    {
                                        case "0_SEC":
                                        case "0_MIN":
                                            SetExitMode(0, args.ToString());
                                            break;
                                        case "15_SEC":
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }

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
        protected virtual void SetOrder2(OrderOperationEnum tradeoperation, long contract, double price)
        {
        }
        protected void RemoveOrdersRegistered(string method, string msg1, string msg2)
        {
            var methodname = method.HasValue()
                ? method
                : System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            RemoveOrders(ActiveOrderCollection.OrdersRegisteredReadyToUse, methodname, msg1, msg2);
        }
        protected void RemoveOrdersActivateded(string method, string msg1, string msg2)
        {
            var methodname = method.HasValue()
                ? method
                : System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            RemoveOrders(ActiveOrderCollection.ActiveOrdersSoftReadyToUse, methodname, msg1, msg2);
        }
        protected void RemoveOrders(IEnumerable<IOrder3> orders, string method, string msg)
        {
            var methodname = method.HasValue()
                ? method
                : System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            foreach (var o in orders.ToList())
            {
                RemoveOrder(o, methodname, msg);
            }
        }
        protected void RemoveOrder(IOrder3 o, string methodname, string msg)
        {
            var ret = ActiveOrderCollection.RemoveNoKey(o);
            if (ret)
                PrintOrder(o, methodname, $"Removed.Ok {msg}");
                //Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, StrategyTimeIntTickerString,
                //    o.ShortInfo, $"{methodname} Removed: {msg}", o.ShortDescription, o.ToString());
            else
            {
                PrintOrder(o, methodname, $"Can't Removed {msg}");
                //Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, StrategyTimeIntTickerString,
                //    o.ShortInfo, $"{methodname} Can't Removed: {msg}", o.ShortDescription, o.ToString());
            }
        }
        protected void RemoveOrders(IEnumerable<IOrder3> orders, string method, string msg1, string msg2)
        {
            var methodname = method.HasValue()
                ? method
                : System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            foreach (var o in orders.ToList())
            {
                RemoveOrder(o, methodname, msg1, msg2);
            }
        }
        protected void RemoveOrder(IOrder3 o, string methodname, string msg1, string msg2)
        {
            var ret = ActiveOrderCollection.RemoveNoKey(o);
            if (ret)
                PrintOrder(o, methodname, $"Removed.Ok {msg1}", msg2);
           
            else
            {
                PrintOrder(o, methodname, $"Can't Remove {msg1}", msg2);
            }
        }
        private void RemoveOrder(IOrder3 ord, string methodname)
        {
            var o = ActiveOrderCollection.RemoveNoKey(ord);
            if (o)
                PrintOrder(ord, methodname, "Removed.Ok");
            else
            {
                PrintOrder(ord, methodname, "Can't Remove");
            }
        }
        protected void KillAllOrders()
        {
            lock (OrderCollectionLocker)
            {
                foreach (var o in MyOrderCollection)
                    if (o.IsLimit)
                        TradeTerminal.KillLimitOrder(null, this, Ticker.ClassCode, Ticker.Code, o.Number);
                    else if (o.IsStopLimit)
                        TradeTerminal.KillStopOrder(Ticker.ClassCode, Ticker.Code, o.Number);
            }
        }
        protected void KillOrders2(OrderTypeEnum ordType, OrderOperationEnum operation)
        {
            //TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
            //                                ordType, operation);

            //var orders =
            //        ActiveOrderCollection.Items.Where(o => ((IOrder)o).Status == OrderStatusEnum.Activated);

            foreach (IOrder3 ord in ActiveOrders.Where(o => o.OrderType == ordType && o.Operation == operation))
            {
                TradeTerminal.KillLimitOrder(ord);
            }
        }
        protected bool IsTradeTerminalConnected()
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            if (TradeTerminal == null)
            {
                Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString, TradeTerminal?.Key,
                    $"{methodname}", "TradeTerminal is Null", "");
                return false;
            }
            var ret = TradeTerminal.IsWellConnected;
            if (!ret)
                Evlm2(EvlResult.INFO, EvlSubject.DIAGNOSTIC, StrategyTimeIntTickerString, TradeTerminal?.Key,
                    $"{methodname} {OperationEnum.NotConnected}", "");
            return ret;
        }
        protected void PrintOrders(IEnumerable<IOrder3> orders, string methodname, string message)
        {
            foreach (var order in orders.ToList())
                PrintOrder(order, methodname, message);
        }
        protected void PrintOrder(IOrder3 ord, string methodname, string message)
        {
            var order = ord.Clone();
            Evlm2(EvlResult.INFO, EvlSubject.TRADING, order.StrategyTimeIntTickerString,
                order.ShortInfo, $"{methodname} {message}",
                order.ShortDescription, order.ToString());
        }
        protected void PrintOrders(IEnumerable<IOrder3> orders, string methodname, string msg1, string msg2)
        {
            foreach (var order in orders.ToList())
                PrintOrder(order, methodname, msg1, msg2);
        }
        protected void PrintOrder(IOrder3 ord, string methodname, string msg1, string msg2)
        {
            var order = ord.Clone();
            Evlm2(EvlResult.INFO, EvlSubject.TRADING, order.StrategyTimeIntTickerString,
                order.ShortInfo, $"{methodname} {msg1}", msg2, order.ToString());
        }
        protected void KillOrders(IEnumerable<IOrder3> orders, string methodname, string message)
        {
            foreach (var order in orders)
            {
                KillOrder(order, methodname, message);
            }
        }
        protected void KillOrder(IOrder3 order, string methodname, string message)
        {
            // if (!IsTradeTerminalConnected()) return;
            if (TradeTerminal.IsWellConnected) return;

            order.BusyStatus = BusyStatusEnum.InUse;

            if (order.IsLimit)
            {
                //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTimeIntTickerString,
                //    order.ShortInfo, $"{methodname}: Try to {OperationEnum.Kill} {message}",
                //    order.ShortDescription, order.ToString());
                var prorder = order.Clone();
                TradeTerminal.KillLimitOrder(order);
                PrintOrder(prorder, methodname, $"{OperationEnum.Kill} {message}");
            }
            else if (order.IsStopLimit)
            {
                //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTimeIntTickerString,
                //    order.ShortInfo, $"{methodname}: Try to {OperationEnum.Kill} {message}",
                //    order.ShortDescription, order.ToString());

                TradeTerminal.KillStopOrder(Ticker.ClassCode, Ticker.Code, order.Number);
                PrintOrder(order, methodname, $"{OperationEnum.Kill} {message}");
            }
        }
        protected void KillOrders(IEnumerable<IOrder3> orders, string methodname, string message1, string message2)
        {
            foreach (var order in orders)
            {
                KillOrder(order, methodname, message1, message2);
            }
        }
        protected void KillOrder(IOrder3 order, string methodname, string message1, string message2)
        {
            // if (!IsTradeTerminalConnected()) return;
            if (!TradeTerminal.IsWellConnected) return;

            order.BusyStatus = BusyStatusEnum.InUse;

            if (order.IsLimit)
            {
                //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTimeIntTickerString,
                //    order.ShortInfo, $"{methodname}: Try to {OperationEnum.Kill} {message1}",
                //    message2, order.ToString());
                var prorder = order.Clone();
                TradeTerminal.KillLimitOrder(order);
                PrintOrder(prorder, methodname, $"{OperationEnum.Kill} {message1}", message2);
            }
            else if (order.IsStopLimit)
            {
                //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTimeIntTickerString,
                //    order.ShortInfo, $"{methodname}: Try to {OperationEnum.Kill} {message1}",
                //    message2, order.ToString());

                TradeTerminal.KillStopOrder(Ticker.ClassCode, Ticker.Code, order.Number);
                PrintOrder(order, methodname, $"{OperationEnum.Kill} {message1}", message2);
            }
        }
        protected void KillOrdersActivated(string methodname, string msg1, string msg2)
        {
            KillOrders(ActiveOrderCollection.ActiveOrdersSoftReadyToUse, methodname, msg1, msg2);
        }
        protected void KillAllActiveOrders2()
        {
            //var orders =
            //       ActiveOrders;

            //var orders =
            //        ActiveOrderCollection.Items.Where(o => ((IOrder)o).Status == OrderStatusEnum.Activated);
            //var orders =
            //        ActiveOrderCollection.Items.Where(o => ((IOrder) o).Status == OrderStatusEnum.Registered);
            //var reg = orders.Count();

            //orders =
            //        ActiveOrderCollection.Items.Where(o => ((IOrder)o).Status == OrderStatusEnum.Confirmed);

            //var conf = orders.Count();

            //orders =
            //        ActiveOrderCollection.Items.Where(o => ((IOrder)o).Status == OrderStatusEnum.Activated);

            //var act = orders.Count();

            //var totOrd = ActiveOrderCollection.Items;
            //var tot = totOrd.Count();

            //TradeContext.Evlm(EvlResult.INFO,EvlSubject.TECHNOLOGY, StrategyTickerString, "Order2", "TryToCalcActive " +
            //      String.Format("tot: {0} reg: {1} conf = {2} act = {3}", tot, reg, conf, act),"");

            //foreach (IOrder o in totOrd)
            //{
            //    TradeContext.Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY,
            //                                StrategyTickerString, "Orders2", "ALL Orders", o.ToString(), "");
            //}

            //TradeContext.Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY,
            //                                    StrategyTickerString, "Orders", "ActiveOrdersCount: " + ActiveOrders.Count(),
            //                                    "Try To KILL ORDERS", "");
            RemoveClosedOrders();

            try
            {
                foreach (IOrder3 o in ActiveOrdersSoft)
                {
                    TradeContext.Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY,
                        StrategyTimeIntTickerString, o.ShortInfo, o.ShortDescription,
                        "Try To KILL ORDER", o.ToString());
                    if (o.IsLimit)
                    {
                        TradeTerminal.KillLimitOrder(o);
                    }
                    else if (o.IsStopLimit)
                        TradeTerminal.KillStopOrder(Ticker.ClassCode, Ticker.Code, o.Number);
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(StrategyTickerString, "Strategy.KillAllOrder2()", "", "", e);
                // throw;
            }
        }
        protected void SetKillAllOrdersInQueue()
        {
            lock (OrderCollectionLocker)
            {
                foreach (var o in MyOrderCollection)
                    if (o.IsLimit)
                        TradeTerminal.SetKillLimitOrderInQueue(Ticker.ClassCode, Ticker.Code, o.Number);
                    else if (o.IsStopLimit)
                        TradeTerminal.KillStopOrder(Ticker.ClassCode, Ticker.Code, o.Number);
            }
        }
        public void SetExitMode(int mode, string msg)
        {
            var oldMode = ExitMode;
            ExitMode = mode;
            try
            {
                switch (mode)
                {
                    case 1:
                    case 2:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                        EntryEnabled = false;
                        break;
                    case 3:
                        // case 13:
                        EntryEnabled = true;
                        break;
                    default:
                        EntryEnabled = true;
                        break;
                }
                // if( oldMode != ExitMode )
                Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, "Strategies",
                    StrategyTickerString, (mode > 0 ? "Exit Process" : "Normal Process") + $"; Working: {Working}",
                    $"ExitMode={ExitMode}; EntryMode={EntryEnabled} Position={Position.PosOperation}", msg);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(StrategyTickerString, "TimePlanMessage", "TimePlanEventHandler()", msg, e);
                // throw;
            }
        }
        public void SetExitModeFromPortfolio(int mode, string msg)
        {
            var oldMode = ExitMode;
            ExitMode = mode;

            try
            {
                switch (mode)
                {
                    case 1:
                    case 2:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                        EntryPortfolioEnabled = false;
                        break;
                    case 3:
                        EntryPortfolioEnabled = false;
                        break;
                    default:
                        EntryPortfolioEnabled = true;
                        break;
                }
                // if( oldMode != ExitMode )
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Portfolios",
                    StrategyTickerString, mode > 0 ? "Exit Process" : "Normal Process",
                    $"ExitMode={ExitMode}; EntryMode={EntryEnabled} Position={Position.PosOperation}", msg);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(StrategyTickerString, "TimePlanMessage", "TimePlanEventHandler()", msg, e);
                // throw;
            }
        }
        // Work Proccess for exition work Every 15 sec
        private void Exit()
        {
            if (ExitMode <= 0)
                return;
            SetWorkingStatus(false, $"ExitMode: {ExitMode}");
            CloseAll(ExitMode);
        }
        protected virtual void SetOrder(OrderOperationEnum operation, float price, long contract, string comment)
        {
            if (IsOrderAlreadyExist(operation, price)) return;

            if (string.IsNullOrWhiteSpace(Ticker.FormatF))
                throw new NullReferenceException(Ticker.Code + ": Ticker Format is Null");

            var priceStr = price.ToString(Ticker.FormatF);

            TradeTerminal.SetLimitOrder(this, TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
                operation,
                price, priceStr, contract,
                DateTime.Today.Add(new TimeSpan(23, 49, 15)),
                comment);


            //   TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.TRADING, Code, "Close: " + Position.OperationString,
            //                                 "Price=" + _currentLimitPrice, "");
        }
        protected bool IsOrderAlreadyExist(OrderOperationEnum operation, float price)
        {
            bool ret;
            lock (OrderCollectionLocker)
            {
                ret = MyOrderCollection.Any(o => o.IsValid &&
                                                 o.Operation == operation &&
                                                 o.LimitPrice.CompareTo((double) price) == 0);

            }
            return ret;
        }

        public long PositionTarget { get; set; }
        public bool IsPosContractsExceeded => Position != null && Position.IsOpened && Position.Quantity > Contracts;

        public virtual void SkipTheTick(int n)
        {
        }
        public virtual void StartNewDayInit()
        {
        }
        public virtual void Clear()
        {
            Position?.Clear();
            PositionTotal?.Clear();
        }
        protected void CalcLastTradeOperation(IPosition2 pold, IPosition2 pnew, PositionChangedEnum changedResult)
        {
            LastPositionChanged = changedResult;
            /*
            switch( changedResult)
            {
                case PositionChangedEnum.Opened:
                case PositionChangedEnum.Reversed:
                    LastLongEntryPrice = pnew.IsLong ? pnew.LastTradePrice : 0f;
                    LastShortEntryPrice = pnew.IsShort ? pnew.LastTradePrice : 0f;
                    LastTradeOperation = (OperationEnum)pnew.Operation;
                    break;
                case PositionChangedEnum.ReSizedUp:
                    if( pnew.IsLong )
                        LastLongEntryPrice = pnew.LastTradePrice;
                    else if ( pnew.IsShort)
                        LastShortEntryPrice =  pnew.LastTradePrice;
                    LastTradeOperation = (OperationEnum)pnew.Operation;
                    break;
                case PositionChangedEnum.ReSizedDown:
                    if( pnew.IsLong )
                        LastShortEntryPrice =  pnew.LastTradePrice;
                    else if ( pnew.IsShort)
                        LastLongEntryPrice = pnew.LastTradePrice;
                    LastTradeOperation = (OperationEnum)pnew.FlipOperation;
                    break;
                case PositionChangedEnum.Closed:
                    LastLongEntryPrice = 0f;
                    LastShortEntryPrice = 0f;
                    LastTradeOperation = (OperationEnum)pold.FlipOperation;
                    break;
            }
             */
        }
        public int RegisterNewTrade(ITrade t)
        {
            return +1;
        }
        public bool IsTradeNumberValid(ulong tradeNumber)
        {
            return tradeNumber > Position?.LastTradeNumber;
        }
        public int NewTrade(ITrade3 t)
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";

            if (Position == null)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, StrategyTickerString, t.ShortInfo,
                    $"{methodname} Position is Null", t.ShortDescription, t.ToString());
                return -1;
            }

            var lastTradeNumber = Position.LastTradeNumber;
            if (t.Number <= lastTradeNumber)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, StrategyTickerString, t.ShortInfo,
                    $"{methodname} OldTrade Detected.Last:[{lastTradeNumber}]", t.ShortDescription, t.ToString());

                t.Comment += " Old.";
                OnChangedEvent(new Events.EventArgs
                {
                    Category = "UI.Trades",
                    Entity = "Trade",
                    Operation = "Update",
                    Object = t
                });
                return -1;
            }

            //if (! t.DT.Date.IsEquals(Position.LastTradeDT.Date))
            //    Position.DailyPnLFixed = 0;

            try
            {
                //TradeContext.TradeWebClient?.Post2(t.DT, t.Number, t.OrderNumber,
                //    t.AccountCode, Id, Code, t.TickerCode,
                //    t.Operation > 0 ? 1 : -1, t.Quantity, (double) t.Price, 0d, "Trade");

                t.Comment += " New.";
                OnChangedEvent(new Events.EventArgs
                {
                    Category = "UI.Trades",
                    Entity = "Trade",
                    Operation = "Update",
                    Object = t
                });

                TradeContext.Publish(t);

                var oldPosition = Position.Clone();
                var pClosed = Position.NewTrade(t);

                LastDeal = pClosed;

                PositionIsChangedEventHandler2(oldPosition, Position, Position.LastChangedResult);

                //TradeContext.TradeStorage.SaveChanges(StorageOperationEnum.AddNew, t);  
                TradeContext.TradeStorage.SaveChanges(StorageOperationEnum.Add, t);

                if (pClosed != null)
                {
                    //Position.DailyPnLFixed += pClosed.PnL;

                    OnChangedEvent(new Events.EventArgs
                    {
                        Category = "UI.Deals",
                        Entity = "Deal",
                        Operation = "Add",
                        Object = pClosed,
                        Sender = this
                    });
                    //TradeContext.Storage.SaveChanges(StorageOperationEnum.Add, t);
                    TradeContext.TradeStorage.SaveChanges(StorageOperationEnum.AddOrUpdate, Position);
                    TradeContext.TradeStorage.SaveTotalChanges(StorageOperationEnum.AddOrUpdate, PositionTotal);
                    TradeContext.TradeStorage.SaveChanges(StorageOperationEnum.Add, pClosed);

                    TradeContext.Publish(Position);
                    TradeContext.Publish(PositionTotal);
                    TradeContext.Publish(pClosed);
                }
                else
                {
                    //TradeContext.Storage.SaveChanges(StorageOperationEnum.Add, t);
                    TradeContext.TradeStorage.SaveChanges(StorageOperationEnum.AddOrUpdate, Position);
                    TradeContext.Publish(Position);
                }
                return +1;
            }
            catch (Exception e)
            {
                SendException(e);
                //throw new NullReferenceException("Strtegy.NewTrade(t) - Something is Wrong" + e.Message);
                return -1;
            }
        }

        public IEnumerable<IPosition2> GetDeals()
        {
            return Deals.Items;
        }
        public void AddDeal(IPosition2 p)
        {
            //p.PnL = (p.Price2 - p.Price1)*p.Pos;
            Deals.AddNew(p);
        }
        public void UpdateFromLastTick()
        {
            if (Ticker.LastPrice.IsLessOrEqualsThan(0d))
                return;
            if (Position.IsLong)
                Position.Price2 = (decimal) Ticker.LastPrice; // Ticker.Bid;
            else if (Position.IsShort)
                Position.Price2 = (decimal) Ticker.LastPrice; //Ticker.Ask;

            Position.LastPrice = (decimal) Ticker.LastPrice;
            //else
            //    Position.Price2 = 0;

            if (Position.IsOpened)
                Strategies.OnStrategyTradeEntityChangedEvent(
                    new Events.EventArgs
                    {
                        Category = "UI.POSITIONS",
                        Entity = "CURRENT",
                        Operation = "UPDATE.PRICE2",
                        Object = Position
                    });
        }
        public void EnQueue(object sender, IEventArgs args)
        {
            EventHub?.EnQueue(sender, args);
        }
        public void FireOrderChangedEventToStrategy(IOrder3 order, string category, string entity, string operation)
        {
            order?.Strategy?.EnQueue(this, new Events.EventArgs
            {
                Category = order.Strategy.StrategyTimeIntTickerString.TrimUpper(),
                Entity = entity.TrimUpper(),
                Operation = operation.TrimUpper(),
                Object = order,
                Sender = this
            });
        }

        protected virtual void PositionIsChangedEventHandler2(IPosition2 oldpos, IPosition2 newpos,
            PositionChangedEnum changedResult)
        {
        }
        public void PositionChanged(IPosition2 oldpos, IPosition2 newpos, PositionChangedEnum changedResult)
        {
            //PositionIsChangedEventHandler2(oldpos, newpos, changedResult);

            //Strategies.OnStrategyTradeEntityChangedEvent(new Events.EventArgs
            //{
            //    Category = "UI.Positions",
            //    Entity = "Current",
            //    Operation = "Update",
            //    Object = newpos
            //});
        }

        //public void PositionUpdate(ITrade3 t)
        //{
        //    Position.NewTrade(t);
        //}

        //public void PositionUpdate(ITrade3 t)
        //{
        //    var pp = Position;
        //    if (pp.IsOpened)
        //    {
        //        var oldPosition = (Position2)pp.Clone(); // Make Clone
        //        var oldposition = pp.Pos;
        //        //var posPosition = pp.Pos;
        //        //var posQuantity = pp.Quantity;
        //        //var posOperation = pp.Operation;

        //        pp.LastTradeDT = t.DT;
        //        pp.LastTradeNumber = t.Number;
        //        pp.Price2 = t.Price;

        //        switch (t.Operation)
        //        {
        //            case OperationEnum.Buy:
        //                pp.LastTradeBuyPrice = (float)t.Price;
        //                break;
        //            case OperationEnum.Sell:
        //                pp.LastTradeSellPrice = (float)t.Price;
        //                break;
        //        }
        //        try
        //        {
        //            if ((short)pp.Operation == (short)t.Operation)
        //            // Rise Size !!! Position Operation the Same. For example: Buy & Buy
        //            {

        //                pp.Price1 = (pp.Price1 * pp.Quantity + t.Price * t.Quantity) / (pp.Quantity + t.Quantity);
        //                pp.Quantity += t.Quantity;
        //                //pp.Count += t.Quantity;
        //                //pp.Comment += String.Format("Rise.{0} {1:C} {2} {3} {4}. ", pp.Pos, pp.Price1, t.Number,
        //                //    t.Position, t.Price);

        //                //pp.InvokePositionChangedEvent(oldposition, pp.Pos);
        //                //pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.ReSizedUp);
        //                pp.Strategy.PositionChanged(oldPosition, pp, PositionChangedEnum.ReSizedUp);
        //                // PositionCollection.Add(pp);
        //                //pp.Strategy.Strategies.OnStrategyTradeEntityChangedEvent(new Events.EventArgs
        //                //{
        //                //    Category = "UI.Positions",
        //                //    Entity = "Current",
        //                //    Operation = "Update",
        //                //    Object = pp
        //                //});

        //                Evlm.AddItem(EvlResult.WARNING, EvlSubject.TRADING, pp.StrategyTickerString, "Position2",
        //                    "ReSized Up",
        //                    pp.PositionString5 + " ReSized Up from " + oldPosition.PositionString5, pp.ToString());
        //            }
        //            else
        //            {
        //                // Position Opened and Position Status Not the Same with Trade Operation
        //                // Close Flag Shoud Be S E E E E T

        //                // Something Should be Close   Close  3 -3  ReSize 3 -1  Reverse 3 -5      

        //                //_needClosedToObserve = +1;
        //                //_needOpenedToObserve = +1;

        //                if (pp.Pos + t.Position == 0)
        //                // ONLY  Close Position -> addToClose and Clear Current Position
        //                {
        //                    //   Delete(pp, PositionOpenedCollection);

        //                    //_needClosedToObserve = +1;
        //                    //_needOpenedToObserve = +1;

        //                    var p = new Position2
        //                    {
        //                        Strategy = pp.Strategy,
        //                        Ticker = pp.Ticker,
        //                        Account = pp.Account,

        //                        Operation = pp.Operation,
        //                        Quantity = t.Quantity,
        //                        Price1 = pp.Price1,
        //                        Price2 = t.Price,
        //                        PnL = (t.Price - pp.Price1) * t.Quantity * (short)pp.Operation,
        //                        Status = PosStatusEnum.Closed,
        //                        FirstTradeDT = pp.FirstTradeDT,
        //                        FirstTradeNumber = pp.FirstTradeNumber,
        //                        LastTradeDT = t.DT,
        //                        LastTradeNumber = t.Number,
        //                    };

        //                    pp.Strategy.AddDeal(p);
        //                    pp.Clear();

        //                    // pp.Clear();
        //                    //   pp.Index = ++_maxIndex;

        //                    //pp.InvokePositionChangedEvent(oldposition, pp.Pos);
        //                    //pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.Closed);
        //                    pp.Strategy.PositionChanged(oldPosition, pp, PositionChangedEnum.Closed);
        //                    //pp.Strategy.Strategies.OnStrategyTradeEntityChangedEvent(new Events.EventArgs
        //                    //{
        //                    //    Category = "UI.Positions",
        //                    //    Entity = "Current",
        //                    //    Operation = "Update",
        //                    //    Object = pp
        //                    //});

        //                    // ********************************* Total ***************************
        //                    // PositionTotals.UpdateTotalOrNew(p);

        //                    pp.Strategy.PositionTotal.Update(p); // 07.01.2014

        //                    // PositionClosedCollection.Add(pp); //Insert(0, pp);

        //                    //    _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, p.StrategyTickerString, "Position", "Closed",
        //                    //                                   oldPosition.PositionString3 + " -> " + pp.PositionString3,
        //                    //                                   p.ToString() + t.ToString());
        //                }
        //                else //(pp.Pos + t.Position != 0)  Reduce  or Reverse Position 
        //                {
        //                    if (pp.Quantity > t.Quantity) // Reduce Size
        //                    {
        //                        // Open new "Close Position with Old LastTradeNumber

        //                        //var p = new Position(pp.Account, pp.Strategy, pp.TickerStr, pp.Operation,
        //                        //    t.Quantity,
        //                        //    pp.Price1, t.Price, (t.Price - pp.Price1) * pp.Pos, PosStatusEnum.Closed,
        //                        //    pp.FirstTradeDT, pp.FirstTradeNumber, t.DT, t.Number)
        //                        //{
        //                        //    Ticker = pp.Ticker,

        //                        //};
        //                        // AddPosition(p);

        //                        var p = new Position2
        //                        {
        //                            Strategy = pp.Strategy,
        //                            Ticker = pp.Ticker,
        //                            Account = pp.Account,

        //                            Operation = pp.Operation,
        //                            Quantity = t.Quantity,
        //                            Price1 = pp.Price1,
        //                            Price2 = t.Price,
        //                            PnL = (t.Price - pp.Price1) * t.Quantity * (short)pp.Operation,
        //                            Status = PosStatusEnum.Closed,
        //                            FirstTradeDT = pp.FirstTradeDT,
        //                            FirstTradeNumber = pp.FirstTradeNumber,
        //                            LastTradeDT = t.DT,
        //                            LastTradeNumber = t.Number,
        //                        };

        //                        pp.Strategy.AddDeal(p);

        //                        pp.Quantity = pp.Quantity - t.Quantity;
        //                        pp.Price2 = t.Price; // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
        //                        pp.PnL = (t.Price - pp.Price1) * pp.Pos;

        //                        pp.LastTradeDT = t.DT;
        //                        pp.LastTradeNumber = t.Number;

        //                        //  pp.Comment += String.Format("Open.Reduce {0} {1:C} {2} {3} {4}. ",
        //                        //      pp.Pos, pp.Price1, t.Number, t.Position, t.Price);
        //                        //      pp.Index = ++_maxIndex;

        //                        // pp.Strategy.AddDeal(p);
        //                        //Positions.AddPosition(p); // need to refresh

        //                        //pp.InvokePositionChangedEvent(oldposition, pp.Pos);
        //                      //  pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.ReSizedDown);
        //                        pp.Strategy.PositionChanged(oldPosition, pp, PositionChangedEnum.ReSizedDown);

        //                        //pp.Strategy.Strategies.OnStrategyTradeEntityChangedEvent(new Events.EventArgs
        //                        //{
        //                        //    Category = "UI.Positions",
        //                        //    Entity = "Current",
        //                        //    Operation = "Update",
        //                        //    Object = pp
        //                        //});

        //                        // ************************************** Total *****************************
        //                        // PositionTotals.UpdateTotalOrNew(p);
        //                        //pp.PositionTotal.Update(p); // 15.11.2012
        //                        pp.Strategy.PositionTotal.Update(p);

        //                        // PositionClosedCollection.Add(pp); // Insert(0, pp);

        //                        _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, pp.StrategyTickerString,
        //                            "Position2", "ReSized Down",
        //                            pp.PositionString5 + " ReSized Down from " + oldPosition.PositionString5,
        //                            pp.ToString());
        //                        //_eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
        //                        //                               p.PositionString3, "Close" + t.PositionString.WithSqBrackets(),
        //                        //                               p.ToString(), t.ToString());
        //                    }
        //                    else if (pp.Quantity < t.Quantity) // REVERSE 
        //                    {
        //                        // Open New Close Position

        //                        //var p = new Position(pp.Account, pp.Strategy, pp.TickerStr, pp.Operation,
        //                        //    pp.Quantity,
        //                        //    pp.Price1, t.Price, (t.Price - pp.Price1) * pp.Pos, PosStatusEnum.Closed,
        //                        //    pp.FirstTradeDT, pp.FirstTradeNumber, t.DT, t.Number)
        //                        //{
        //                        //    Ticker = pp.Ticker,

        //                        //};
        //                        var p = new Position2
        //                        {
        //                            Strategy = pp.Strategy,
        //                            Ticker = pp.Ticker,
        //                            Account = pp.Account,

        //                            Operation = pp.Operation,
        //                            Quantity = pp.Quantity,
        //                            Price1 = pp.Price1,
        //                            Price2 = t.Price,
        //                            PnL = (t.Price - pp.Price1) * pp.Quantity * (short)pp.Operation,
        //                            Status = PosStatusEnum.Closed,
        //                            FirstTradeDT = pp.FirstTradeDT,
        //                            FirstTradeNumber = pp.FirstTradeNumber,
        //                            LastTradeDT = t.DT,
        //                            LastTradeNumber = t.Number,
        //                        };
        //                        //Positions.AddPosition(p);
        //                        pp.Strategy.AddDeal(p);

        //                        pp.Operation = Position.Reverse(pp.Operation);
        //                        pp.Quantity = t.Quantity - pp.Quantity;

        //                        pp.Price1 = t.Price;
        //                        pp.Price2 = t.Price;

        //                        pp.PnL = (t.Price - pp.Price1) * pp.Pos;

        //                        pp.FirstTradeDT = t.DT;
        //                        pp.FirstTradeNumber = t.Number;
        //                        pp.LastTradeDT = t.DT;
        //                        pp.LastTradeNumber = t.Number;

        //                        // ***************** _needOpenedToObserve = +1;

        //                        // !!!! OUT of Range pp.Comment += String.Format("Open.Reverse {0} {1:C} {2} {3} {4}. ", pp.Pos,
        //                        //  pp.Price1,
        //                        //  t.Number, t.Position, t.Price);
        //                        //  pp.Index = ++_maxIndex;

        //                        //pp.InvokePositionChangedEvent(oldposition, pp.Pos);
        //                        //pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.Reversed);
        //                        pp.Strategy.PositionChanged(oldPosition, pp, PositionChangedEnum.Reversed);
        //                        // *************** Total ******************************************
        //                        // PositionTotals.UpdateTotalOrNew(p);
        //                        //pp.PositionTotal.Update(p); // 15.11.2012
        //                        pp.Strategy.PositionTotal.Update(p);

        //                        //    _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, p.StrategyTickerString, "Position",
        //                        //                      p.PositionString3 + " Reverse.Close" + t.PositionString.WithSqBrackets(),
        //                        //                      p.ToString(), t.ToString());
        //                        TradeContext.EventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, pp.StrategyTickerString,
        //                            "Position2", "Reversed",
        //                            pp.PositionString5 + " Reversed from " + oldPosition.PositionString5, pp.ToString());
        //                    }
        //                    else
        //                    {
        //                        TradeContext.EventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING,
        //                            "Position2", "Nobody's Fools - ???",
        //                            String.Format("{0} {1} {2} {3}", pp.Pos,
        //                                pp.Quantity, t.Position, t.Quantity),
        //                            t.ToString());

        //                        TradeContext.EventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING,
        //                            "Position2", "Nobody's Fools - ???",
        //                            pp.ToString(), t.ToString());
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            throw new Exception("Position.NewTrade() Faillure: " + e.Message);
        //        }
        //    }
        //    else if (pp.IsNeutral) // Position not IsOpened
        //    {
        //        // **************************************** _needOpenedToObserve = +1;
        //        // pp = PositionCurrentCollection.FirstOrDefault(p => p.StrategyKey == t.StrategyKey && p.IsNeutral);

        //        var oldPosition = (Position2)pp.Clone(); // Make Clone

        //        pp.Operation = (PosOperationEnum)t.Operation;
        //        pp.Quantity = t.Quantity;
        //        pp.Price1 = t.Price;
        //        pp.Price2 = t.Price;
        //        pp.Status = PosStatusEnum.Opened;

        //        pp.FirstTradeDT = t.DT;
        //        pp.FirstTradeNumber = t.Number;

        //        pp.LastTradeDT = t.DT;
        //        pp.LastTradeNumber = t.Number;

        //        if (t.Operation == GS.Trade.OperationEnum.Buy)
        //        {
        //            pp.LastTradeBuyPrice = (float)t.Price;
        //            pp.LastTradeSellPrice = 0f;
        //        }
        //        else if (t.Operation == GS.Trade.OperationEnum.Sell)
        //        {
        //            pp.LastTradeSellPrice = (float)t.Price;
        //            pp.LastTradeBuyPrice = 0f;
        //        }
        //        //  pp.Index = ++ _maxIndex;

        //        //pp.InvokePositionChangedEvent(0, pp.Pos);
        //        //pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.Opened);
        //        pp.Strategy.PositionChanged(oldPosition, pp, PositionChangedEnum.Opened);
        //        // ****************************************** _needOpenedToObserve = +1;

        //        TradeContext.EventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, pp.StrategyTickerString, "Position2",
        //                          "New", pp.PositionString5, pp.ToString());
        //    }

        //    // *************************** 
        //    //Positions.FireObserverEvent();
        //}

        //protected virtual void SendExceptionMessage(string source, string operation, string message, string sourceExc)
        //{
        //    var ea = new Events.EventArgs
        //    {
        //        Category = "UI.Exceptions",
        //        Entity = "Exception",
        //        Operation = "Add",
        //        Object = new GSException
        //        {
        //            Source = source,
        //            Operation = operation,
        //            Message = message,
        //            SourceExc = sourceExc
        //        }
        //    };
        //    if(Strategies != null)
        //        Strategies.OnExceptionEvent(ea);
        //    else
        //        OnExceptionEvent(ea);

        //    TradeContext.Evlm(EvlResult.FATAL, EvlSubject.PROGRAMMING, source, "Exception", operation, message, sourceExc);
        //}
        public int LongShortEnabled { get; set; }
        public bool IsShortEnabled => ShortEnabled; //LongShortEnabled <= 0;
        public bool IsLongEnabled => LongEnabled; // LongShortEnabled >= 0;
        public bool IsAllEntryDisabled => !IsLongEnabled && !IsShortEnabled;
       
        protected void SetLongShortEnabled(bool enabled)
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";

            if (IsHedger)
                LongEnabled = ShortEnabled = false;
            else
                LongEnabled = ShortEnabled = enabled;

            Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString, 
                        methodname, $"LongEnabled:{LongEnabled} ShortEnabled:{ShortEnabled}","");
        }
        public void SetLongShortEnabled(bool longenabled, bool shortenabled)
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";

                LongEnabled = longenabled;
                ShortEnabled = shortenabled;

            Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString,
                        methodname, $"LongEnabled:{LongEnabled} ShortEnabled:{ShortEnabled}", "");
        }
        public void ClearOrderCollection()
        {
            var m = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString,
                "OrderCollection", m, "Clear All Orders","");
            RemoveOrders(ActiveOrderCollection.Items, m, "Clear All");           
        }

        //public DateTime DailyMaxProfitDT { get; set; }
        //public DateTime DailyMinProfitDT { get; set; }
        public decimal DailyMaxProfit => Position?.TotalDailyMaxProfit ?? 0;
        public decimal DailyMaxLoss => Position?.TotalDailyMaxLoss ?? 0;
        public DateTime DailyMaxProfitDT => Position?.TotalDailyMaxProfitDT ?? DateTime.Now;
        public DateTime DailyMaxLossDT => Position?.TotalDailyMaxLossDT ?? DateTime.Now;
        public void CreateStat()
        {
            Position?.CreateStat();
        }

        public void  Register(IStrategy s)
        {
        }
        public void SetParent(IStrategy s)
        {
        }
    }
}
