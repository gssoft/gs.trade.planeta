using System;
using System.Collections.Generic;
using System.Threading;
using GS.Elements;
using GS.Events;
using GS.Exceptions;
using GS.Interfaces;
using GS.Process;
using GS.Serialization;
using GS.Time.TimePlan;
// using GS.Trade.Data;
using GS.Trade.Interfaces;
// using GS.Trade.Strategies;
//using GS.Trade.Strategies.Managers;
//using GS.Trade.Strategies.Portfolio;
using GS.Trade.Trades;
using GS.Trade.Trades.Deals;
using GS.Trade.Trades.Storage;
using GS.Trade.Trades.Time;
using GS.Trade.Trades.Trades3;
using GS.Trade.TradeTerminals64.Quik;
using GS.Trade.TradeTerminals64.Simulate;
using GS.Trade.Windows;
using GS.Trade.Windows.Charts;
// using GS.Trade.Windows.OrderPlane;
using TimePlanEventArgs = GS.Time.TimePlan.TimePlanEventArgs;


namespace GS.Trade.TradeContext
{
    //public class TradeContext2 : Element1<string>, ITradeContext
    //{
    //    public override string Key { get { return Code; }}

    //    //private IEventLog _eventLog;

    //    //public event EventHandler<IEventArgs> ExceptionEvent;
    //    //public virtual void OnExceptionEvent(IEventArgs e)
    //    //{
    //    //    EventHandler<IEventArgs> handler = ExceptionEvent;
    //    //    if (handler != null) handler(this, e);
    //    //}

    //    //public void SendExceptionMessage3(string source, string objtype, string operation, string objstr, Exception e)
    //    //{
    //    //    throw new NotImplementedException();
    //    //}

    //    //public void FireChangedEvent(string category, string entity, string operation, object o)
    //    //{
    //    //    throw new NotImplementedException();
    //    //}

    //    //public IEventLog EventLog
    //    //{
    //    //    get { return _eventLog; }
    //    //    set { _eventLog = value; }
    //    //}

    //    public Trades3 Trades3 { get; set; }
    //    public ITradeStorage Storage { get; set; }

    //    public ITrades Trades { get; set; }
    //    public IOrders Orders { get; set; }
    //    public IOrders SimulateOrders { get { return Orders; } }
    //    public IPositions Positions { get; set; }
    //    public Strategies.Strategies Strategies { get; set; }
    //    //public Portfolios Portfolios { get; set; }
    //    //public EntryManagers EntryManagers { get; set; }

    //    public IEventHub EventHub { get; set; }

    //    public ITradeStorage TradeStorage { get; set; }
    //    public ITradeStorage TradeStorage2 { get; set; }

    //    public IAccounts Accounts { get; private set; }

    //    public Dde.Dde Dde;
    //    public Quotes.Quotes Quotes;
    //    public ITickers Tickers { get; set; }
    //    public Tickers TickersAll;

    //    public IEnumerable<ITicker> TickerCollection { get { return Tickers.TickerCollection; } }
    //    public IEnumerable<IStrategy> StrategyCollection { get { return Strategies.StrategyCollection; } }

    // //   public GS.Trade.Trades.Time.TimePlans TimePlans { get; set; }
    //    public TimePlans TimePlans { get; set; }

    //    public BarRandom Rand;

    //    public ITickers GetTickers
    //    {
    //        get { return Tickers; }
    //    }

    //    public IAccount GetAccount2(string key)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IAccount RegisterAccount(string key)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public ITicker GetTicker(string key)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IStrategies GetStrategies
    //    {
    //        get { return Strategies; }
    //    }

    //    //public OrderFiller OrderFiller;

    //    public ProcessManager2 StrategyProcess { get; set; }
    //    public ProcessManager2 UIProcess { get; set; }
    //    public ProcessManager2 DiagnosticProcess { get; set; }

    //    public ProcessManager2 FastProcess { get; set; }

    //    public EventLogWindow2 EventLogWindow;

    //    public TradesWindow TradesWindow;
    //    public TradesWindow2 TradesWindow2;

    //    public OrdersActiveWindow2 OrdersActiveWindow2;
    //    //public OrdersFilledWindow OrdersFilledWindow;

    //    public OrdersFilledWindow2 OrdersFilledWindow2;

    //    //public PositionsOpenedWindow PositionsOpenedWindow;
    //    //public PositionsClosedWindow PositionsClosedWindow;

    //    //public PositionsClosedWindow2 PositionsClosedWindow2;
    //    public DealsWindow DealsWindow;
    //    public PositionsWindow2 PositionsWindow2;
    //    public PositionTotalsWindow2 PositionTotalsWindow2;

    //    public ExceptionsWindow ExceptionsWindow;

    //    public QuotesWindow QuotesWindow;
    //    //public OrderPlaneWindow OrderPlaneWindow;

    //    public ChartWindow ChartWindow { get; set; }

    //    public TradeTerminals.TradeTerminals TradeTerminals;
    //    public SimulateTerminal SimulateTerminal;

    //    public AutoResetEvent UiCloseAutoEvent { get; set; }

    //    public TradeContext2()
    //    {
    //        // if(_eventLog == null)
    //        //     _eventLog = new ConsoleEventLog();

    //        Trades3 = new Trades3();
    //        Storage = new MemStorage();
    //        TradeStorage = new MemStorage();

    //        Trades = new Trades.Trades();
    //        Orders = new Orders();
    //        Positions = new Positions();

    //        //Portfolios = new Portfolios();
    //        //EntryManagers = new EntryManagers();

    //        Accounts = new Accounts.Accounts();

    //        Tickers = new Tickers();
    //        TickersAll = new Tickers();

    //      //  TimePlans = new TimePlans();
    //        TimePlans = new TimePlans();

    //        Rand = new BarRandom(1000, 100);

    //        //OrderFiller = new OrderFiller(Orders, Trades);

    //        Dde = new Dde.Dde("QUIKInfo");

    //        Quotes = new Quotes.Quotes();

    //        TradeTerminals = new TradeTerminals.TradeTerminals();

    //        //    QuotesWindow = new QuotesWindow();

    //    }
    //    public void Init()
    //    {

    //        EventHub = Builder.Build<EventHub>(@"Init\EventHub.xml", "EventHub");
    //        EventHub.Init(EventLog);

    //        ExceptionEvent += EventHub.FireEvent;
    //        EventLog.ExceptionEvent += EventHub.FireEvent;

    //        ExceptionsWindow = new ExceptionsWindow();
    //        ExceptionsWindow.Init(this);
    //        ExceptionsWindow.Show();

    //        EventLogWindow = new EventLogWindow2();
    //        EventLogWindow.Init(EventLog);
    //        ShowEventLogWindow();

    //        TradeStorage = Builder.Build2<ITradeStorage>(@"Init\TradeStorages.xml", "TradeStorage");
    //        TradeStorage.ExceptionEvent += EventHub.FireEvent;
    //        TradeStorage.ChangedEvent += EventHub.FireEvent;
    //        TradeStorage.Init(EventLog);


    //        //EventLogWindow = new EventLogWindow2();
    //        //EventLogWindow.Init(_eventLog.Primary);
    //        //ShowEventLogWindow();

    //        StrategyProcess = new ProcessManager2("Strategy_Process", 500, 0, EventLog);

    //        UIProcess = new ProcessManager2("UI_Process", 1000, 0, EventLog);
    //        UiCloseAutoEvent = new AutoResetEvent(false);
    //        UIProcess.CloseAutoEvent = UiCloseAutoEvent;

    //        DiagnosticProcess = new ProcessManager2("Diagnostic_Process", 15000, 0, EventLog);

    //        FastProcess = new ProcessManager2("Fast_Process", 100, 0, EventLog);

    //        Positions.Init(EventLog);
    //        TimePlans.Init(EventLog);

    //        Orders.Init(EventLog, Positions, Trades);
    //        Trades.Init(EventLog, Positions, Orders);

    //        Accounts.Load();

    //        foreach (var a in Accounts.GetAccounts)
    //        {
    //            try
    //            {
    //                TradeStorage.Register(a);
    //            }
    //            catch (Exception e)
    //            {
    //                SendExceptionMessage2("TradeContext", a.GetType().ToString(),
    //                                        "Register(account)", a.ToString(),
    //                                        e.Message, e.Source,e.GetType().ToString(),e.TargetSite.ToString());
    //                throw;
    //            }
    //        }

    //        TickersAll.Init(EventLog);
    //        TickersAll.Load();
    //        Tickers.Init(EventLog);

    //        foreach (var t in TickersAll.GetTickers)
    //        {
    //            try
    //            {
    //                TradeStorage.Register(t);
    //            }
    //            catch (Exception e)
    //            {
    //                SendExceptionMessage2("TradeContext", t.GetType().ToString(),
    //                                       "Register(Ticker)", t.ToString(),
    //                                       e.Message, e.Source, e.GetType().ToString(), e.TargetSite.ToString());
    //                throw;
    //            }
    //        }

    //        TradeTerminals.Evl = EventLog;

    //        Orders.BackOrderExecMode = BackTestOrderExecutionMode.Optimistic;

    //        Dde.Init(EventLog);
    //        //       Quotes.Init(EventLog);

    //        //EventHub = Builder.Build<EventHub>(@"Init\EventHub.xml", "EventHub");
    //        //EventHub.Init(EventLog);

    //        Storage.ChangedEvent += EventHub.FireEvent;

    //        TradeStorage.ChangedEvent += EventHub.FireEvent;

    //        Strategies = new Strategies.Strategies("MyStrategies", "xStyle", this);
    //        Strategies.Init();

    //        foreach (var s in Strategies.StrategyCollection)
    //        {
    //            try
    //            {
    //                TradeStorage.Register(s);
    //                TradeStorage.Register(s.Position);
    //                TradeStorage.RegisterTotal(s.PositionTotal);
    //            }
    //            catch (Exception e)
    //            {
    //                SendExceptionMessage2("TradeContext", s.GetType().ToString(),
    //                                       "Register(Strategy)", s.ToString(),
    //                                       e.Message, e.Source, e.GetType().ToString(), e.TargetSite.ToString());
    //                //SendExceptionMessage("TradeContext", "Strategy.Register:" + s.StrategyTickerString, e.Message, e.Source);
    //                throw;
    //            }
    //        }

    //        Strategies.StrategyTradeEntityChangedEvent += EventHub.FireEvent;
            
    //        //EntryManagers.Init();

    //        foreach (var t in TradeTerminals.TradeTerminalCollection.Values)
    //        {
    //            var sim = t as SimulateTerminal;
    //            if (sim == null)
    //                continue;
    //            SimulateTerminal = sim;

    //            //Rand.NewTickEvent += SimulateTerminal.ExecuteTick;
    //            //sim.TradeEntityChangedEvent += EventHub.FireEvent;
    //            break;

    //        }

    //        // Tickers.LoadBarsFromArch();

    //        #region Windows Init

    //        //EventLogWindow = new EventLogWindow();
    //        //TradesWindow = new TradesWindow();
    //        TradesWindow2 = new TradesWindow2();

    //        OrdersActiveWindow2 = new OrdersActiveWindow2();
    //        //OrdersFilledWindow = new OrdersFilledWindow();
    //        OrdersFilledWindow2 = new OrdersFilledWindow2();


    //        //PositionsOpenedWindow = new PositionsOpenedWindow();
    //        //PositionsClosedWindow = new PositionsClosedWindow();
    //        DealsWindow = new DealsWindow();
    //        PositionsWindow2 = new PositionsWindow2();
    //        PositionTotalsWindow2 = new PositionTotalsWindow2();

    //        //ExceptionWindow = new ExceptionsWindow(); 

    //        //OrderPlaneWindow = new OrderPlaneWindow();
    //        //OrderPlaneWindow.Init(this);

    //        //TradesWindow.Init(EventLog, Trades);
    //        TradesWindow2.Init(this);

    //        OrdersActiveWindow2.Init(this, EventLog);
    //        OrdersFilledWindow2.Init(this);

    //        //PositionsOpenedWindow.Init(this, EventLog, Positions);
    //  //      PositionsClosedWindow.Init(EventLog, Positions);
    //        DealsWindow.Init(this, EventLog);
    //        PositionsWindow2.Init(this, EventLog);
    //        PositionTotalsWindow2.Init(this, EventLog);

    //        //ExceptionWindow.Init(this);

    //        ChartWindow = new ChartWindow();
    //        ChartWindow.Init(this);

    //        //     QuotesWindow.Init(EventLog, Quotes);

    //        #endregion
    //    }
    //    public void Open()
    //    {
    //        StrategyProcess.NewTickEvent += Tickers.UpdateTimeSeries;
    //    //    StrategyProcess.NewTickEvent += TimePlans.NewTickEventHandler;
    //        //StrategyProcess.NewTickEvent += OrderPlaneWindow.UpdateQuote;

    //        Tickers.NewTickEvent += Orders.ExecuteTick;

    //        Rand.NewTickEvent += Orders.ExecuteTick;
    //        Rand.NewTickStrEvent += Tickers.PutDdeQuote3;
           

    //        DiagnosticProcess.RegisterProcess("Check Connections Process", 15000, 5000, null, TradeTerminals.CheckConnection, null);

    //        var topic = Dde.RegisterTopic("Forts", "Main Quotes", Tickers.PutDdeQuote3);
    //        topic.RegisterChannel(1, "Rtsi", Tickers.PutDdeQuote3);

    //        ShowWindows();
    //    }
    //    public void OpenChart()
    //    {
    //        UIProcess.RegisterProcess("Chart Window Refresh", "ChartWindowRefreshProcess",
    //                                  1000, 0, null, ChartWindow.TickRefreshEventHandler, null);
    //        ChartWindow.Show();
    //    }

    //    public void Close()
    //    {
    //        Dde.Close();

    //        StrategyProcess.NewTickEvent -= Tickers.UpdateTimeSeries;
    //    //    StrategyProcess.NewTickEvent -= TimePlans.NewTickEventHandler;
    //        //StrategyProcess.NewTickEvent -= OrderPlaneWindow.UpdateQuote;

    //        Rand.NewTickEvent -= Orders.ExecuteTick;
    //        Rand.NewTickStrEvent -= Tickers.PutDdeQuote3;

    //        Tickers.NewTickEvent -= Orders.ExecuteTick;

    //        StrategyProcess.Close();
    //        DiagnosticProcess.Close();
    //        UIProcess.Close();

    //        Strategies.Close();

    //        CloseWindows();

    //        // EventLogWindow.Close();
    //    }
    //    public void Start()
    //    {
    //        Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Begin Start Process", "", "");

    //        UIProcess.Start();
    //        StrategyProcess.Start();
    //        //DiagnosticProcess.Start();

    // //       TimePlans.Start();

    //        //Dde.Start();

    //        // Rand.Start();

    //        Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Finish Start Process", "", "");
    //    }
    //    public void StartRand()
    //    {
    //        Rand.Start();
    //    }

    //    public void Stop()
    //    {
    //        Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Begin Stop Process", "", "");

    //        Dde.Stop();

    //        Rand.Stop();

    //        TradeTerminals.DisConnect();

    //        StrategyProcess.Stop();
    //        //DiagnosticProcess.Stop();
    //        UIProcess.Stop();
    //        //UiCloseAutoEvent.WaitOne();

    //        Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Finish Stop Process", "", "");
    //    }
    //    public void ShowWindows()
    //    {
    //        //EventLogWindow.Show();

    //        //TradesWindow.Show();
    //        TradesWindow2.Show();

    //        OrdersActiveWindow2.Show();
    //        OrdersFilledWindow2.Show();

    //        //PositionsOpenedWindow.Show();
    //      //  PositionsClosedWindow.Show();

    //        DealsWindow.Show();
    //        PositionsWindow2.Show();
    //        PositionTotalsWindow2.Show();

    //        //ExceptionWindow.Show();

    //        //OrderPlaneWindow.Show();

    //        // QuotesWindow.Show();

    //        // ChartWindow.Show();
    //    }
    //    public void CloseWindows()
    //    {
    //        //EventLogWindow.Close();

    //        ChartWindow.Close();

    //        //TradesWindow.Close();
    //        TradesWindow2.Close();

    //        OrdersActiveWindow2.Close();
    //        OrdersFilledWindow2.Close();

    //        //PositionsOpenedWindow.Close();
    //        //PositionsClosedWindow.Close();
    //        DealsWindow.Close();
    //        PositionsWindow2.Close();
    //        PositionTotalsWindow2.Close();

    //        //OrderPlaneWindow.Close();

    //        // QuotesWindow.Show();
    //    }

    //    public void ShowEventLogWindow()
    //    {
    //        EventLogWindow.Show();
    //    }

    //    public ITicker RegisterTicker(string code)
    //    {
    //        var t = TickersAll.GetTicker(code);
    //        if (t != null)
    //        {
    //            //t.RegisterAsyncSeries(new Bars001("1_Min","PrimaryBars", t, 60, 0));
    //            Tickers.AddTicker(t);
    //            return t;
    //        }
    //        Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, code, "RegisterTicker", "RegisterTicker", "Ticker: " + code + "Is NOT Found", "");
    //        return null;
    //    }

    //    public ITicker RegisterTicker2(string tickerBoard, string tickerCode)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public ITimeSeries RegisterTimeSeries(ITimeSeries ts)
    //    {
    //        //var t = ts as ITimeSeries;
    //        //return t.Ticker.RegisterTimeSeries(t);
    //        return ts.Ticker.RegisterTimeSeries(ts);
    //    }
    //    public ITimeSeries GetTimeSeriesOrNull(string key)
    //    {
    //        foreach (Ticker t in Tickers.TickerCollection)
    //        {
    //            ITimeSeries ts;
    //            if ((ts = t.GetTimeSeries(key)) != null) return ts;
    //        }
    //        return null;
    //    }
    //    public ITradeTerminal RegisterTradeTerminal(string type, string key)
    //    {
    //        var tt = TradeTerminals.RegisterTradeTerminal(type, key, Orders, Trades, EventLog, this);
    //        if (tt == null)
    //            throw new NullReferenceException(string.Format("TradeTerminal {0} {1} is Not Found", type, key));

    //        //  Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TradeTerminal", "Register", type + " " + key, "");
    //        return tt;
    //    }
    //    public IPosition RegisterPosition(string account, string strategy, ITicker ticker)
    //    {
    //        //return Positions.Register2(account, strategy, ticker);
    //        var p = Positions.Register2(account, strategy, ticker);
    //        p.EventLog = EventLog;
    //        return p;
    //    }

    //    public IPosition2 RegisterPosition(IStrategy s)
    //    {
    //        return new Position2
    //        {
    //            Strategy = s,
    //            Status = PosStatusEnum.Closed,
    //            Operation = PosOperationEnum.Neutral,
    //            EventLog = EventLog,
    //            PositionTotal = new PositionTotal2
    //            {
    //                Strategy = s,
    //                Status = PosStatusEnum.Closed,
    //                Operation = PosOperationEnum.Neutral,
    //                EventLog = EventLog
    //            }
    //        };
    //    }

    //    public IPortfolio RegisterPortfolio(string code, string name)
    //    {
    //        //var p = new Portfolio { Code = code, Name = name };
    //        //IPortfolio ip;
    //        //if ((ip = Portfolios.AddNew(p)) != null)
    //        //    return ip;
    //        //throw new NullReferenceException("Portfolio Register Failure:" + p.ToString());
    //        return null;
    //    }
    //    public IEntryManager RegisterEntryManager(string code, string name)
    //    {
    //        //var p = new EntryManager { Code = code, Name = name };
    //        //IEntryManager ip;
    //        //if ((ip = EntryManagers.AddNew(p)) != null)
    //        //    return ip;
    //        //throw new NullReferenceException("EntryManager Register Failure:" + p.ToString());
    //        return null;
    //    }

    //    public ITrade3 Resolve(ITrade3 t)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IOrder3 Resolve(IOrder3 o)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IStrategy GetStrategyByKey(string key)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IStrategy RegisterDefaultStrategy(string name, string code, string accountKey, string tickerBoard, string tickerKey,
    //        uint timeInt, string terminalType, string terminalKey)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IStrategy RegisterDefaultStrategy(string name, string code, string accountKey, string tickerKey, uint timeInt,
    //        string terminalType, string terminalKey)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Save(IOrder3 o)
    //    {
    //        // Storage.Add(o);
    //        TradeStorage.SaveChanges(StorageOperationEnum.Add, o);
    //    }

    //    public void Save(IOrder o)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Save(ITrade3 t)
    //    {
    //        TradeStorage.SaveChanges(StorageOperationEnum.Add, t);
    //    }

    //    public IDeals BuildDeals()
    //    {
    //        return new Deals();
    //    }

    //    public IEnumerable<IPosition2> GetPositionCurrents()
    //    {
    //        return Strategies.GetPositionCurrents();
    //    }
    //    public IEnumerable<IPosition2> GetPositionTotals()
    //    {
    //        return Strategies.GetPositionTotals();
    //    }
    //    public IEnumerable<IPosition2> GetDeals()
    //    {
    //        return Strategies.GetDeals();
    //    }

    //    public IAccount GetAccount(string key)
    //    {
    //        return Accounts.GetAccount(key);
    //    }
    //    /*
    //    public TimePlan RegisterTimePlanStatusChangedEventHandler(string timePlanKey,
    //                                                              TimePlan.TimeStatusIsChangedEventHandler action)
    //    {
    //        return TimePlans.RegisterTimeStatusChangedEventHandler(timePlanKey, action);
    //    }
    //     */
    //    public TimePlan RegisterTimePlanEventHandler(string timePlanKey,
    //                                                                EventHandler<TimePlanEventArgs> action)
    //    {
    //        return TimePlans.RegisterTimePlanEventHandler(timePlanKey, action);
    //    }
       
    //    public void ExecuteTick(DateTime dt, string tickerkey, double price, double bid, double ask)
    //    {
    //        SimulateTerminal.ExecuteTick(dt, tickerkey, price, bid, ask);
    //    }
    //}
}