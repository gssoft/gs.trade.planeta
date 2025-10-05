using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Elements;
using GS.Events;
using GS.Exceptions;
using GS.Extension;
using GS.Interfaces;
using GS.Serialization;
// using GS.Trade.Data.Bars;
using GS.Trade.DataBase;
using GS.Trade.DataBase.Storage;
using GS.Trade.Interfaces;
using GS.Process;
//using GS.Trade.Strategies.Managers;
//using GS.Trade.Strategies.Portfolio;
// using GS.Trade.Strategies.Portfolio;
using GS.Trade.Trades.Deals;
using GS.Trade.Trades.Orders3;
using GS.Trade.Trades.Time;
using GS.Trade.TradeTerminals64;
using GS.Trade.TradeTerminals64.Quik;
using GS.Trade.TradeTerminals64.Simulate;
//using GS.Trade.Web.Clients;
using GS.Trade.Windows;
// using GS.Trade.Data;
// using GS.Trade.Strategies;
using GS.Trade.Windows.Charts;
// using GS.Trade.Windows.OrderPlane;
using GS.Time.TimePlan;


using TimePlanEventArgs = GS.Time.TimePlan.TimePlanEventArgs;

namespace GS.Trade.TradeContext
{
    using Trades;
 //   using Dde;
    using Quotes;
    using EventLog;
 //   using Strategies;
    using Accounts;

 //   public partial class TradeContext : Element1<string>, ITradeContext
 //   {
 //       //private  IEventLog _eventLog;

 //       public override string Key
 //       {
 //           get { return Code; }
 //       }

 //       //public IEventLog EventLog
 //       //{
 //       //    get { return _eventLog; }
 //       //    set { _eventLog =  value as IEventLog; }
 //       //}

 //       //public event EventHandler<GS.Events.IEventArgs> ExceptionEvent;
 //       //public virtual void OnExceptionEvent(IEventArgs e)
 //       //{
 //       //    EventHandler<IEventArgs> handler = ExceptionEvent;
 //       //    if (handler != null) handler(this, e);
 //       //}
 //       public int GetFromDbMode { get; set; }
 //       public bool NeedToReCreateDb { get; set; }

 //       [XmlIgnore]
 //       public AutoResetEvent StraCloseAutoEvent { get; set; }
 //       [XmlIgnore]
 //       public AutoResetEvent DiaCloseAutoEvent { get; set; }
 //       [XmlIgnore]
 //       public AutoResetEvent UiCloseAutoEvent { get; set; }
 //       [XmlIgnore]
 //       protected WaitHandle[] WaitHandles;
 //       [XmlIgnore]
 //       public TradeContextInit TradeContextInit { get; set; }
 //   //    public TradeContextInit2 TradeContextInit2 { get; set; }
 //       [XmlIgnore]
 //       public ITrades Trades { get; set; }
 //       [XmlIgnore]
 //       public IOrders Orders { get; set; }
 //       [XmlIgnore]
 //       public IPositions Positions { get; set; }
 //       [XmlIgnore]
 //       public Strategies Strategies { get; set; }
 //       [XmlIgnore]
 //       public IPortfolios Portfolios { get; set; }
 //       //public EntryManagers EntryManagers { get; set; }
 //       [XmlIgnore]
 //       public ITradeStorage TradeStorage { get; set; }
 //       //public ITradeStorage Storage { get; set; }
 //       [XmlIgnore]
 //       public Orders3 ActiveOrders  { get; set; }
 //      //[XmlIgnore]
 //       //public  IEventHub2 EventHub2 { get; set; }
 //       [XmlIgnore]
 //       public IEventHub EventHub { get; set; }
 //       [XmlIgnore]
 //       public IOrders SimulateOrders { get; set; }
 //       [XmlIgnore]
 //       public IAccounts Accounts { get; private set; }
 //       [XmlIgnore]
 //       public Dde Dde;
 //       [XmlIgnore]
 //       public IDde Dde2;
 //       [XmlIgnore]
 //       public Quotes Quotes;
 //       [XmlIgnore]
 //       public ITickers Tickers { get; set;}
 //       [XmlIgnore]
 //       public Tickers TickersAll;
 //       [XmlIgnore]
 //       public IEnumerable<ITicker> TickerCollection { get { return Tickers.TickerCollection; } }
 //       [XmlIgnore]
 //       public IEnumerable<IStrategy> StrategyCollection { get { return Strategies.StrategyCollection; } }

 //   //    public GS.Trade.Trades.Time.TimePlans TimePlans { get; set; }
 //       [XmlIgnore]
 //       public TimePlans TimePlans { get; set; }
 //       [XmlIgnore]
 //       public BarRandom Rand;

 //       public ITickers GetTickers
 //       {
 //           get { return Tickers; }
 //       }
 //       public IStrategies GetStrategies
 //       {
 //           get { return Strategies; }
 //       }

 //       //public IEventHub EventHub { get; private set; }

 //       //public OrderFiller OrderFiller;
 //       [XmlIgnore]
 //       public ProcessManager2 StrategyProcess { get; set; }
 //       [XmlIgnore]
 //       public ProcessManager2 UIProcess { get; set; }
 //       [XmlIgnore]
 //       public ProcessManager2 DiagnosticProcess { get; set; }
 //       [XmlIgnore]
 //       public ProcessManager2 FastProcess { get; set; }
 //       [XmlIgnore]
 //       public EventLogWindow2 EventLogWindow;

 //       //public TradesWindow TradesWindow;
 //       [XmlIgnore]
 //       public TradesWindow2 TradesWindow2;

 //       //public OrdersActiveWindow OrdersActiveWindow;
 //       [XmlIgnore]
 //       public OrdersActiveWindow2 OrdersActiveWindow2;

 //       //public OrdersFilledWindow OrdersFilledWindow;
 //       [XmlIgnore]
 //       public OrdersFilledWindow2 OrdersFilledWindow2;

 //       //public PositionsOpenedWindow PositionsOpenedWindow;
 //       //public PositionsClosedWindow PositionsClosedWindow;
 //       //public PositionTotalsWindow PositionTotalsWindow;
 //       [XmlIgnore]
 //       public DealsWindow DealsWindow;
 //       [XmlIgnore]
 //       public PositionsWindow2 PositionsWindow2;
 //       [XmlIgnore]
 //       public PositionTotalsWindow2 PositionTotalsWindow2;

 //       [XmlIgnore]
 //       public PortfolioWindow PortfolioWindow;
 //       [XmlIgnore]
 //       public ExceptionsWindow ExceptionsWindow;

 //       //public PositionsWindow PositionsWindow;

 //       [XmlIgnore]
 //       public TransactionsWindow TransactionsWindow;
 //       // public QuotesWindow QuotesWindow;
 //      // public OrderPlaneWindow OrderPlaneWindow;
 //       [XmlIgnore]
 //       public ChartWindow ChartWindow { get; set; }
 //       [XmlIgnore]
 //       public TradeTerminals.TradeTerminals TradeTerminals;
 //       [XmlIgnore]
 //       public DbStorage32 DbStorageTemp;

 //       public  TradeContext()
 //       {
 //          // if(_eventLog == null)
 //          //     _eventLog = new ConsoleEventLog();

 //     //      WebEventLogClient = new WebEventLog();
 //           //var db = new DbStorage();

 //           Code = "TradeContext";
 //           Name = Code;
 //           Alias = Code;

 //           StraCloseAutoEvent = new AutoResetEvent(false);
 //           DiaCloseAutoEvent = new AutoResetEvent(false);
 //           UiCloseAutoEvent = new AutoResetEvent(false);

 //           //WaitHandles = new WaitHandle[] 
 //           //{
 //           //    new AutoResetEvent(false),
 //           //    new AutoResetEvent(false),
 //           //    new AutoResetEvent(false)
 //           //};

 //           Trades = new Trades();
 //           Orders = new Orders();

 //           ActiveOrders = new Orders3();

 //           Positions = new Positions();

 //           //Portfolios = new Portfolios();
 //           //EntryManagers = new EntryManagers();

 //           //Storage = new DbStorage();

 //         //  EventHub = new EventHub();

 //           SimulateOrders = new Orders();

 //           Accounts = new Accounts {Parent = this};

 //           Tickers = new Tickers();
 //           TickersAll = new Tickers();

 //           TimePlans = new TimePlans();
            
 //           Rand = new BarRandom(1000, 100);

 //           //OrderFiller = new OrderFiller(Orders, Trades);

 //      //     TradeContextInit2 = new TradeContextInit2();
 //      //     SerializeInitData();

 //           //if(!DeserializeInitData())
 //           //    throw new NullReferenceException("TradeContextData XMLDesrialization Failure");

 //           //Dde = new Dde(TradeContextInit.DdeInit.ServerName);

 //           //if (TradeContextInit.OrderInit.ClassCodeToRemoveLogin.HasValue() &&
 //           //    TradeContextInit.OrderInit.LoginToRemove.HasValue())
 //           //{
 //           //    Orders.ClassCodeToRemoveLogin = TradeContextInit.OrderInit.ClassCodeToRemoveLogin;
 //           //    Orders.LoginToRemove = TradeContextInit.OrderInit.LoginToRemove;

 //           //    Trades.ClassCodeToRemoveLogin = TradeContextInit.OrderInit.ClassCodeToRemoveLogin;
 //           //    Trades.LoginToRemove = TradeContextInit.OrderInit.LoginToRemove;
 //           //}

 //           //Dde = new Dde("QUIKInfo");

 //            Quotes = new Quotes();

             
            
 //       //    QuotesWindow = new QuotesWindow();
            
 //       }
 //       public virtual void Init()
 //       {
 //           try
 //           {
 //           //((IEventLogs)EventLog).SetMode(EvlModeEnum.Init);

 //           //EventHub = Builder.Build<EventHub>(@"Init\EventHub.xml", "EventHub");
 //           //EventHub.Init(EventLog);

 //           EventHub = Builder.Build<EventHub>(@"Init\EventHub.xml", "EventHub");
 //           //EventHub.Init(EventLog);
 //           //EventHub.ExceptionEvent += ExceptionRegister;
            
 //           this.ExceptionEvent += EventHub.FireEvent;
 //           EventLog.ExceptionEvent += EventHub.FireEvent;

 //           ExceptionsWindow = new ExceptionsWindow();
 //           ExceptionsWindow.Init(this);
 //           ExceptionsWindow.Show();

 //           EventLogWindow = new EventLogWindow2();
 //           EventLogWindow.Init(EventLog);
 //           ShowEventLogWindow();

 //           EventHub.Init(EventLog);
 //           EventHub.ExceptionEvent += ExceptionRegister;

 //           //var TradeStore = Builder.Build<GS.Trade.TradeStorage.TradeStore>(@"Init\TradeStorage.xml", "TradeStore");
 //           //TradeStore.Init(EventLog);

 //           TradeStorage = Builder.Build2<ITradeStorage>(@"Init\TradeStorages.xml", "TradeStorage");
 //           TradeStorage.ExceptionEvent += EventHub.FireEvent;
 //           TradeStorage.ChangedEvent += EventHub.FireEvent;
 //           TradeStorage.Init(EventLog);

 //           EventHub.Subscribe("UI.EXCEPTIONS","EXCEPTION", ExceptionRegister);

 //           TradeTerminals = new TradeTerminals.TradeTerminals
 //           {
 //               Name = "Trade Terminals",
 //               Code = "TradeTerminals",
 //               //Parent = this,
 //               EventLog = EventLog
 //           };

 //           var p = Builder.Build<ProcessManager3>(@"Init\ProcessManager3.xml", "ProcessManager3");

 //           StrategyProcess = new ProcessManager2("Strategy_Process", 500, 0, EventLog);
 //           StrategyProcess.CloseAutoEvent = StraCloseAutoEvent;
 //           //StrategyProcess.CloseAutoEvent = (AutoResetEvent)WaitHandles[0];

 //           UIProcess = new ProcessManager2("UI_Process", 1000, 0, EventLog);
 //           UIProcess.CloseAutoEvent = UiCloseAutoEvent;
 //           //UIProcess.CloseAutoEvent = (AutoResetEvent)WaitHandles[1];

 //           DiagnosticProcess = new ProcessManager2("Diagnostic_Process", 1000, 0, EventLog);
 //           DiagnosticProcess.CloseAutoEvent = DiaCloseAutoEvent;
 //           //DiagnosticProcess.CloseAutoEvent = (AutoResetEvent)WaitHandles[2];

 //          // FastProcess = new ProcessManager2("Fast_Process", 100, 0, EventLog);

 //           Positions.Init(EventLog);
 //           try
 //           {
 //               TimePlans.Init(EventLog);
 //           }
 //           catch (Exception e)
 //           {
 //               SendExceptionMessage("TradeContext"," TimePlan.Init()",e.Message,e.Source);
 //               throw ;
 //           }
 //           Orders.Init(EventLog, Positions, Trades);
 //           Trades.Init(EventLog, Positions, Orders);

 //           SimulateOrders.Init(EventLog, Positions, Trades);

 //           // throw new NullReferenceException("Test Exceptions");
 //           // Need to ReCreate DataBase
 //           if (NeedToReCreateDb)
 //           {
 //               Accounts.Load();
 //               foreach (var a in Accounts.GetAccounts)
 //               {
 //                   try
 //                   {
 //                       TradeStorage.Register(a);
 //                   }
 //                   catch (Exception e)
 //                   {
 //                       SendExceptionMessage3("TradeContex", a.GetType().ToString(),
 //                           "Account.Register:" + a.Key, a.ToString(), e);
 //                       throw;
 //                   }
 //               }
 //               TickersAll.Init(EventLog);
 //               TickersAll.Load();

 //               foreach (var t in TickersAll.GetTickers)
 //               {
 //                   try
 //                   {
 //                       TradeStorage.Register(t);
 //                   }
 //                   catch (Exception e)
 //                   {
 //                       SendExceptionMessage3("TradeContex",
 //                           t.GetType().ToString(), "Ticker.Register:" + t.Code, t.ToString(), e);
 //                       throw;
 //                   }
 //               }
 //           }

 //           Tickers.Init(EventLog);
 //           Tickers.Parent = this;

 //           //foreach (var t in TickersAll.GetTickers)
 //           //{
 //           //    wc.Register(t);
 //           //    var b = t;
 //           //}

 //           //TradeTerminals.Evl = EventLog;

 //           Orders.BackOrderExecMode = BackTestOrderExecutionMode.Optimistic;
 //           SimulateOrders.BackOrderExecMode = BackTestOrderExecutionMode.Optimistic;

 //           if (!DeserializeInitData())
 //               throw new NullReferenceException("TradeContextData XMLDesrialization Failure");

 //         //  Dde = new Dde(TradeContextInit.DdeInit.ServerName);
 //         //  Dde.Init(EventLog);

 //           Dde2 = Builder.Build2<IDde>(@"Init\Dde.xml", "Dde2");
 //           Dde2.Init(EventLog);

 //    //       Quotes.Init(EventLog);

 //           //
 //           // throw new NullReferenceException();

 //           ActiveOrders.Init(EventLog);

 //           try
 //           {
 //               Strategies = new Strategies("MyStrategies", "xStyle", this);
 //               Strategies.Init();

 //               foreach (var s in Strategies.StrategyCollection)
 //               {
 //                   try
 //                   {
 //                       TradeStorage.Register(s);
 //                       TradeStorage.Register(s.Position);
 //                       TradeStorage.RegisterTotal(s.PositionTotal);
 //                   }
 //                   catch (Exception e)
 //                   {
 //                       SendExceptionMessage("TradeContex", "Strategy.Register:" + s.StrategyTickerString, e.Message, e.Source );
 //                       throw;
 //                   }
 //               }

 //               Strategies.StrategyTradeEntityChangedEvent += EventHub.FireEvent;
 //               Strategies.ExceptionEvent += EventHub.FireEvent;

 //               Portfolios = BuildPortfolios();
 //               Portfolios.ChangedEvent += EventHub.FireEvent;

 //               //TradeTerminals.DeQueueProcess();
 //               //TradeTerminals.OrderResolveProcess();
 //               //TradeTerminals.TradeResolveProcess();

 //               //foreach (var t in TradeTerminals.TradeTerminalCollection.Values)
 //               //{
 //               //    //StrategyProcess.RegisterProcess(t.Code, t.Name, (int)(1 * 1000), 0, null, t.SendOrdersFromQueue, null);
 //               //    //StrategyProcess.RegisterProcess(t.Code + ".2", t.Name + ".2", (int)(1 * 500), 0, null, t.SendOrderTransactionsFromBlQueue, null);
 //               //    //StrategyProcess.RegisterProcess(t.Code + ".TradesPro3", t.Name + ".TradesPro3", (int)(1 * 1000), 0, null, t.TradeProcess3, null);
 //               //    var quik = t as IQuikTradeTerminal;
 //               //    if (quik == null)
 //               //        continue;
 //               //    //UIProcess.RegisterProcess(t.Code + ".TradesPro2", t.Name + ".TradesPro2", (int)(1 * 1000), 0, null, t.OrderProcess2, null);

 //               //    //quik.TransactionEvent += EventHub.FireEvent;
 //               //    //quik.OrderEvent += EventHub.FireEvent;
 //               //    //quik.TradeEvent += EventHub.FireEvent;

 //               //    //quik.TradeEntityChangedEvent += EventHub.FireEvent;
 //               //}
 //               StrategyProcess.RegisterProcess("Orders DeQueue Process", "OrdersDeQueueProcess", (int)(1 * 500), 0, null, TradeTerminals.DeQueueProcess, null);
 //               StrategyProcess.RegisterProcess("Trade Resolve Process", "TradeResolveProcess", (int)(1 * 1000), 0, null, TradeTerminals.TradeResolveProcess, null);
 //               UIProcess.RegisterProcess("Order Resolve Process", "OrderResolveProcess", (int)(1 * 1000), 0, null, TradeTerminals.OrderResolveProcess, null);
                
 //               TradeTerminals.ChangedEvent += EventHub.FireEvent;

 //               UIProcess.RegisterProcess("Event Hub Dequeue Process", "EventHib.DeQueue()" , (int)(1 * 1000), 0, null, EventHub.DeQueueProcess, null);
                
 //               //int i = 0;
 //               //foreach (var a in TradeStorage.RepoItems.Where(r=>r.IsEnabled && r.IsQueueEnabled))
 //               //{
 //               //    ++i;
 //               //    UIProcess.RegisterProcess(a.Name + i, a.Code + i, (int)(1 * 1000), 0, null, a.DeQueueProcess, null);
 //               //}
 //               // TradeStorage.DeQueueProcess();
 //               UIProcess.RegisterProcess("TradeStorages DeQueue Process", "TradeStoragesDeQueueProcess",
 //                                                       (int)(1 * 1000), 0, null, TradeStorage.DeQueueProcess, null);
 //               UIProcess.RegisterProcess(Portfolios.Name, Portfolios.Code, (int)(15 * 1000), 0, null, Portfolios.Refresh, null);

 //               // EventHub.Subscribe("Transactions", "Transaction", Transactions.GetFireEvent);

 //               //EventHub.Subscribe("Order", "Status", ActiveOrders.GetFireEvent);

 //               // ActiveOrders.ContainerEvent += EventHub.FireEvent;  // Common Orders2 for UI Windows

 //               //EntryManagers.Init();

 //               //foreach (var t in Tickers.GetTickers)
 //               //{
 //               //    wc.Register(t);
 //               //    var b = t;
 //               //}

 //               //throw new NullReferenceException();

 //               Tickers.LoadBarsFromArch();
 //           }
 //           catch (Exception e)
 //           {
 //               SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.StrategyInit()", ToString(), e);
 //               throw new Exception(e.Message);
 //           }

 //           }
 //           catch (Exception e)
 //           {
 //               SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.Init()", ToString(), e);
 //               //e.Message, e.Source, e.GetType().ToString(), e.TargetSite.ToString());
 //               throw ;
 //           }
            
            
 //#region Windows Init

 //           //EventLogWindow = new EventLogWindow();
 //           //TradesWindow = new TradesWindow();
 //           TradesWindow2 = new TradesWindow2();

 //           //OrdersActiveWindow = new OrdersActiveWindow();
 //           OrdersActiveWindow2 = new OrdersActiveWindow2();

 //           OrdersFilledWindow2 = new OrdersFilledWindow2();
 //           //OrdersFilledWindow = new OrdersFilledWindow();

 //           //PositionsOpenedWindow = new PositionsOpenedWindow();
 //           //PositionsClosedWindow = new PositionsClosedWindow();
 //           //PositionTotalsWindow = new PositionTotalsWindow();
 //           //PositionsWindow = new PositionsWindow();

 //           DealsWindow = new DealsWindow();
 //           PositionsWindow2 = new PositionsWindow2();
 //           PositionTotalsWindow2 = new PositionTotalsWindow2();

 //           PortfolioWindow = new PortfolioWindow();

 //           //ExceptionsWindow = new ExceptionsWindow();
 //           //ExceptionsWindow.Init(this);

 //           TransactionsWindow = new TransactionsWindow();

 //           //OrderPlaneWindow = new OrderPlaneWindow();
 //           //OrderPlaneWindow.Init(this);
        
 //           //TradesWindow.Init(EventLog,Trades);
 //           TradesWindow2.Init(this);

 //           //OrdersActiveWindow.Init(this, EventLog, Orders);
 //           OrdersActiveWindow2.Init(this, EventLog);

 //           //OrdersFilledWindow.Init(this, EventLog, Orders);
 //           OrdersFilledWindow2.Init(this);

 //           //PositionsOpenedWindow.Init(this, EventLog, Positions);
 //           //PositionsClosedWindow.Init(EventLog, Positions);
 //           //PositionTotalsWindow.Init(this, EventLog, Positions.PositionTotals);

 //           //PositionsWindow.Init(this, EventLog, Positions);
 //           //PositionsWindow2.Init(this, EventLog);

 //           DealsWindow.Init(this, EventLog);
 //           PositionsWindow2.Init(this, EventLog);
 //           PositionTotalsWindow2.Init(this, EventLog);

 //           PortfolioWindow.Init(this, EventLog);

 //           TransactionsWindow.Init(EventLog, this);

 //           ChartWindow = new ChartWindow();
 //           ChartWindow.Init(this);

 //           //     QuotesWindow.Init(EventLog, Quotes);

 //           #endregion
 //       }
 //       public virtual void Open()
 //       {
 //           StrategyProcess.NewTickEvent += Tickers.UpdateTimeSeries;
 //     //      StrategyProcess.NewTickEvent += TimePlans.NewTickEventHandler;

 //     //      StrategyProcess.NewTickEvent += OrderPlaneWindow.UpdateQuote;

 //           StrategyProcess.NewTickEvent += TimePlans.NewTickEventHandler;

 //           Tickers.NewTickEvent += SimulateOrders.ExecuteTick;
            
 //         //  Rand.NewTickEvent += Orders.ExecuteTick;
 //         //  Rand.NewTickStrEvent += Tickers.PutDdeQuote3;

 //           DiagnosticProcess.RegisterProcess("Check Connections Process", 15000, 5000, null, TradeTerminals.CheckConnection, null);

 //           UIProcess.RegisterProcess("Update Last Price", "UpdateLastPriceProcess",
 //                                                   5000, 0, null, Strategies.UpdateLastPrice, null);

 //           UIProcess.RegisterProcess("EventLog Dequeue Process", "EventLog.DeQueue",
 //                                                   1000, 0, null, ((IEventLogs)EventLog).DeQueueProcess, null);

 //           //var topic = Dde.RegisterTopic("Forts", "Main Quotes", Tickers.PutDdeQuote3);
 //           //topic.RegisterChannel(1, "Rtsi", Tickers.PutDdeQuote3);

 //           //var topic1 = Dde.RegisterTopic("Micex", "Micex Quotes", Tickers.PutDdeQuote3);
 //           //topic1.RegisterChannel(1, "Micex", Tickers.PutDdeQuote3);

 //           //Dde2.RegisterTopic("Forts","1", Tickers.PutDdeQuote3);
 //           Dde2.RegisterDefaultCallBack(Tickers.PutDdeQuote3);


 //           ShowWindows();
 //           //((IEventLogs)EventLog).SetMode(EvlModeEnum.Nominal);
 //       }
 //       public void OpenChart()
 //       {
 //           UIProcess.RegisterProcess("Chart Window Refresh", "ChartWindowRefreshProcess",
 //                                                   1000, 0, null, ChartWindow.TickRefreshEventHandler, null);
 //           ChartWindow.Show();
 //       }
 //       public virtual void Stop()
 //       {
 //           try
 //           {
 //               Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Begin Stop Process", "", "");

 //               //foreach (var quik in TradeTerminals.TradeTerminalCollection.Values.OfType<IQuikTradeTerminal>())
 //               //{
 //               //    //quik.TransactionEvent -= EventHub.FireEvent;
 //               //    //quik.OrderEvent -= EventHub.FireEvent;
 //               //    //quik.TradeEvent -= EventHub.FireEvent;

 //               //    //quik.TradeEntityChangedEvent -= EventHub.FireEvent;
 //               //}
 //               TradeTerminals.ChangedEvent -= EventHub.FireEvent;

 //               Strategies.StrategyTradeEntityChangedEvent -= EventHub.FireEvent;
 //               EventLog.EventLogChangedEvent -= EventHub.FireEvent;

 //               StrategyProcess.NewTickEvent -= Tickers.UpdateTimeSeries;
 //               //     StrategyProcess.NewTickEvent -= TimePlans.NewTickEventHandler;

 //               //    StrategyProcess.NewTickEvent -= OrderPlaneWindow.UpdateQuote;

 //               StrategyProcess.NewTickEvent -= TimePlans.NewTickEventHandler;

 //               Rand.NewTickEvent -= Orders.ExecuteTick;
 //               Rand.NewTickStrEvent -= Tickers.PutDdeQuote3;

 //               Tickers.NewTickEvent -= Orders.ExecuteTick;
 //               Tickers.NewTickEvent -= SimulateOrders.ExecuteTick;

 //            //   Dde.Stop();
 //               Dde2.Stop();

 //               TimePlans.Stop();

 //               Rand.Stop();

 //               TradeTerminals.DisConnect();

 //               CloseWindows();

 //               StrategyProcess.Stop();
 //               StraCloseAutoEvent.WaitOne();

 //               DiagnosticProcess.Stop();
 //               DiaCloseAutoEvent.WaitOne();

 //               UIProcess.Stop();
 //               UiCloseAutoEvent.WaitOne();

 //               // WaitHandle.WaitAll(WaitHandles);

 //               Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Finish Stop Process", "", "");
 //           }
 //           catch (Exception e)
 //           {
 //               SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.Stop()", ToString(), e);
 //               throw;
 //           }
 //       }
 //       public virtual void Close()
 //       {
 //           try
 //           {

 //           //CloseWindows();

 //          // Dde.Close();
 //           Dde2.Close();

 //           StrategyProcess.Close();
 //           DiagnosticProcess.Close();
 //           UIProcess.Close();

 //               try
 //               {
 //                   Strategies.Close();
 //               }
 //               catch (Exception e)
 //               {
 //                   SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.Strategies.Close()", ToString(), e);
 //                   throw;

 //               }
 //               // CloseWindows();

 //          // EventLogWindow.Close();
 //           }
 //           catch (Exception e)
 //           {
 //               SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.Close()", ToString(), e);
 //               throw;
 //           }
 //       }
 //       public void Start()
 //       {
 //           Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Begin Start Process", "", "");

 //           UIProcess.Start();
 //           StrategyProcess.Start();
 //           DiagnosticProcess.Start();

 //         //  TimePlans.Start();

 //         //  Dde.Start();
 //           Dde2.Start();

 //           // Rand.Start();

 //           Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Finish Start Process", "", "");

 //           ((IEventLogs)EventLog).SetMode(EvlModeEnum.Nominal);

 //       }
 //       public void StartRand()
 //       {
 //           Rand.Start();
 //       }

       
 //       public void ShowWindows()
 //       {
 //           //EventLogWindow.Show();

 //           //TradesWindow.Show();
 //           TradesWindow2.Show();

 //           //OrdersActiveWindow.Show();
 //           OrdersActiveWindow2.Show();

 //           //OrdersFilledWindow.Show();
 //           OrdersFilledWindow2.Show();

 //           //PositionsOpenedWindow.Show();
 //           //PositionsClosedWindow.Show();
 //           //PositionTotalsWindow.Show();
 //           //PositionsWindow.Show();

 //           DealsWindow.Show();
 //           PositionsWindow2.Show();
 //           PositionTotalsWindow2.Show();

 //           PortfolioWindow.Show();

 //           //ExceptionsWindow.Show();

 //           TransactionsWindow.Show();

 //           //OrderPlaneWindow.Show();

 //          // QuotesWindow.Show();

 //           // ChartWindow.Show();
 //       }
 //       public void CloseWindows()
 //       {
 //           //EventLogWindow.Close();

 //           ChartWindow.Close();

 //           TradesWindow2.Close();
 //           //TradesWindow.Close();

 //           //OrdersActiveWindow.Close();
 //           OrdersActiveWindow2.Close();

 //           //OrdersFilledWindow.Close();
 //           OrdersFilledWindow2.Close();

 //           //PositionsOpenedWindow.Close();
 //           //PositionsClosedWindow.Close();
 //           //PositionTotalsWindow.Close();
 //           //PositionsWindow.Close();

 //           DealsWindow.Close();
 //           PositionsWindow2.Close();
 //           PositionTotalsWindow2.Close();

 //           PortfolioWindow.Close();

 //           TransactionsWindow.Close();

 //          // OrderPlaneWindow.Close();

 //           // QuotesWindow.Show();
 //       }

 //       public void ShowEventLogWindow()
 //       {
 //           EventLogWindow.Show();
 //       }

 //       public ITicker RegisterTicker( string code )
 //       {
 //           var t = TickersAll.GetTicker(code);
 //           if( t != null)
 //           {
 //           //t.RegisterAsyncSeries(new Bars001("1_Min","PrimaryBars", t, 60, 0));
 //           Tickers.AddTicker(t);
 //           return t;
 //           }
 //           Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, code, code, "RegisterTicker", "Ticker: " + code + "Is NOT Found", "");
 //           return null;
 //       }
 //       //public ITicker RegisterTicker2(string board, string code)
 //       //{
 //       //    var t = Tickers.Register(board, code);
 //       //    return t = TradeStorage.Register(t);

 //       //    //t = Tickers.CreateTicker(board, code); 

 //       //    //Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, code, code, "RegisterTicker", "Ticker: " + code + "Is NOT Found", "");
 //       //    //return null;
 //       //}
 //       public ITicker GetTicker(string key)
 //       {
 //           var ti = default (ITicker);
 //           try
 //           {
 //               ti = Tickers.GetTicker(key);
 //               if (ti != null)
 //               {
 //                   if (ti.Id == 0)
 //                       TradeStorage.Register(ti);
 //                   return ti;
 //               }
 //               var t = GetFromDbMode > 0 ? TradeStorage.GetTickerByKey(key) : TradeStorage.GetTicker(key);
 //               if (t != null)
 //               {
 //                   ti = new Ticker
 //                   {
 //                       Id = t.Id,
                        
 //                       Code = t.Code,
 //                       Name = t.Name,
 //                       Alias = t.Alias,
                        
 //                       ClassCode = t.TradeBoard,
 //                       TradeBoard = t.TradeBoard,
 //                       BaseContract = t.BaseContract,
 //                       Decimals = t.Decimals,
 //                       Margin = t.Margin,
 //                       MinMove = t.MinMove,
                        
 //                       PriceLimit = t.PriceLimit,
 //                   };
 //                   Tickers.Add(ti);
 //                   return ti;
 //               }
 //           }
 //           catch (Exception e)
 //           {
 //               SendExceptionMessage3("TradeContext",
 //                           ti == null ? "ITicker" : ti.GetType().ToString(), "GetTicker()",
 //                           ti == null ? "Ticker" : ti.ToString(), e);
 //               throw;
 //           }
 //           return null;
 //       }
 //       public IAccount RegisterTicker3(string key)
 //       {
 //           var a = default(IAccount);
 //           try
 //           {
 //               a = GetAccount2(key);
 //               if (a != null)
 //               {
 //                   //Accounts.Register(a);
 //                   return a;
 //               }

 //               a = new Account
 //               {
 //                   //Key = key,
 //                   Code = key,
 //                   Name = key,
 //                   Alias = key,
 //                   TradePlace = key
 //               };
 //               Accounts.Register(a);
 //               a = TradeStorage.Register(a);
 //               Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Code, "Account",
 //                           "RegisterAccount(): " + a.Key.WithSqBrackets0(),
 //                           "RegisterAccount(): " + a.Key.WithSqBrackets0(), a.ToString());
 //           }
 //           catch (Exception e)
 //           {
 //               SendExceptionMessage3("TradeContext",
 //                           a == null ? "IAccountBase" : a.GetType().ToString(), "GetAccount2()",
 //                           a == null ? "Account" : a.ToString(), e);
 //           }
 //           return a;
 //       }
        
 //       public ITimeSeries RegisterTimeSeries(ITimeSeries ts)
 //       {
 //           //var t = ts as TimeSeries;
 //           //return t.Ticker.RegisterTimeSeries(t);
 //           return ts.Ticker.RegisterTimeSeries(ts);
 //       }
 //       public ITimeSeries GetTimeSeriesOrNull(string key)
 //       {
 //           foreach (Ticker t in Tickers.TickerCollection)
 //           {
 //               ITimeSeries ts;
 //               if ((ts = t.GetTimeSeries(key)) != null) return ts;
 //           }
 //           return null;
 //       }

 //       public ITradeTerminal RegisterTradeTerminal(string type, string key)
 //       {
 //           var tt = TradeTerminals.RegisterTradeTerminal(type, key, Orders, SimulateOrders, Trades, EventLog, this);
 //           if( tt == null)
 //               throw new NullReferenceException(string.Format("TradeTerminal {0} {1} is Not Found", type, key) );

 //         //  Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TradeTerminal", "Register", type + " " + key, "");
 //           return tt;
 //       }
 //       public IPosition RegisterPosition( string account, string strategy, ITicker ticker)
 //       {
 //           var p = Positions.Register2(account, strategy, ticker);
 //           p.EventLog = EventLog;
 //           return p;
 //       }

 //       public IPosition2 RegisterPosition(IStrategy s)
 //       {
 //           //var p = Positions.Register2(account, strategy, ticker);
 //           //p.EventLog = EventLog;
 //           //return p;
 //           return new Position2
 //           {
 //               Strategy = s,
 //               Status = PosStatusEnum.Closed,
 //               Operation = PosOperationEnum.Neutral,
 //               EventLog = EventLog,
 //               PositionTotal = new PositionTotal2
 //               {
 //                   Strategy = s,
 //                   Status = PosStatusEnum.Closed,
 //                   Operation = PosOperationEnum.Neutral,
 //                   EventLog = EventLog
 //               }
 //           };
 //       }
 //       public IEnumerable<IPosition2> GetPositionCurrents()
 //       {
 //           return Strategies.GetPositionCurrents();
 //       }

 //       public IEnumerable<IPosition2> GetPositionTotals()
 //       {
 //           return Strategies.GetPositionTotals();
 //       }

 //       public IEnumerable<IPosition2> GetDeals()
 //       {
 //           return Strategies.GetDeals();
 //       }

 //       public IAccount GetAccount(string key)
 //       {
 //           return Accounts.GetAccount(key);
 //           //try
 //           //{
 //           //    var a = TradeStorage.GetAccountByKey(key);
 //           //    return new Account
 //           //    {
 //           //        Id = a.Id,
 //           //        Key = a.Key,
 //           //        Code = a.Code,
 //           //        Name = a.Name,
 //           //        Alias = a.Alias,
 //           //        TradePlace = a.TradePlace,

 //           //        Balance = a.Balance
 //           //    };
 //           //}
 //           //catch (Exception e)
 //           //{
 //           //    throw new NullReferenceException("");
 //           //}
            
 //       }
 //       public IAccount GetAccount2(string key)
 //       {
 //           var ac = default(IAccount);
 //           try
 //           {
 //               ac = Accounts.GetAccount(key);
 //               if (ac != null)
 //               {
 //                   if (ac.Id == 0)
 //                       TradeStorage.Register(ac);
 //                   return ac;
 //               }
 //               var a = GetFromDbMode > 0 ? TradeStorage.GetAccountByKey(key) : TradeStorage.GetAccount(key);
 //               if (a != null)
 //               {
 //                   ac = new Account
 //                   {
 //                       Id = a.Id,
 //                       //Key = a.Key,
 //                       Code = a.Code,
 //                       Name = a.Name,
 //                       Alias = a.Alias,
 //                       TradePlace = a.TradePlace,

 //                       Balance = a.Balance
 //                   };
 //                   Accounts.Register(ac);
 //                   return ac;
 //               }
 //           }
 //           catch (Exception e)
 //           {
 //               SendExceptionMessage3("TradeContext", 
 //                           ac == null ? "IAccountBase" : ac.GetType().ToString(), "GetAccount2()", 
 //                           ac == null ? "Account" : ac.ToString(), e);
 //               throw;
 //           }
 //           return null;
 //       }
 //       public IAccount RegisterAccount(string key)
 //       {
 //           var a = default(IAccount);
 //           try
 //           {
 //               a = GetAccount2(key);
 //               if (a != null)
 //               {
 //                   //Accounts.Register(a);
 //                   return a;
 //               }

 //               a = new Account
 //               {
 //                   //Key = key,
 //                   Code = key,
 //                   Name = key,
 //                   Alias = key,
 //                   TradePlace = key
 //               };
 //               Accounts.Register(a);
 //               a = TradeStorage.Register(a);
 //               Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Code, "Account",
 //                           "RegisterAccount(): " + a.Key.WithSqBrackets0(),
 //                           "RegisterAccount(): " + a.Key.WithSqBrackets0(), a.ToString());
 //           }
 //           catch (Exception e)
 //           {
 //               SendExceptionMessage3("TradeContext",
 //                           a == null ? "IAccountBase" : a.GetType().ToString(), "GetAccount2()",
 //                           a == null ? "Account" : a.ToString(), e);
 //           }
 //           return a;
 //       }

        

 //       /*
 //       public TimePlan RegisterTimePlanStatusChangedEventHandler(string timePlanKey,
 //                                                                   TimePlan.TimeStatusIsChangedEventHandler action)
 //       {
 //           return TimePlans.RegisterTimeStatusChangedEventHandler(timePlanKey, action );
 //       }
 //        */
 //       public ITrade3 Resolve(ITrade3 t)
 //       {
 //           var stratKey = TradeStorage.GetStrategyKeyFromOrder(t.OrderKey);
 //           if (stratKey.HasNoValue())
 //               return null;
 //           var s = Strategies.GetByKey(stratKey);
 //           if (s == null)
 //               return null;
 //           t.Strategy = s;
 //           return t;
 //           //throw new NotImplementedException();
 //       }

 //       public IOrder Resolve(IOrder o)
 //       {
 //           throw new NotImplementedException();
 //       }

 //       public IOrder3 Resolve(IOrder3 o)
 //       {
 //           var stratKey = TradeStorage.GetStrategyKeyFromOrder(o.Key);
 //           if (stratKey.HasNoValue())
 //               return null;
 //           var s = Strategies.GetByKey(stratKey);
 //           if (s == null)
 //               return null;
 //           o.Strategy = s;
 //           return o;
 //           //throw new NotImplementedException();
 //       }

 //       public IStrategy GetStrategyByKey(string key)
 //       {
 //           var stratKey = TradeStorage.GetStrategyKeyFromOrder(key);
 //           if (stratKey.HasNoValue())
 //               return null;
 //           return Strategies.GetByKey(stratKey);
 //       }

 //       public IStrategy RegisterDefaultStrategy(string name, string code, string accountKey,
 //                                                   string tickerBoard ,string tickerCode, uint timeInt,
 //                                                   string terminalType, string terminalKey)
 //       {
 //           var t = GetTicker(tickerBoard + "@"+ tickerCode);
 //           var a = GetAccount2(accountKey);

 //           if (t == null || a == null)
 //           {
 //               Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, FullName, "DefaultStrategy",
 //                           "Failure To Create DeafultStrategy from Order ",
 //                           "Ticker=" + (tickerBoard + "@" + tickerCode).WithSqBrackets0() + " Acc=" + accountKey.WithSqBrackets0() , "");
 //               return null;
 //           }
 //           //var s = Strategies.RegisterDefaultStrategy(name,code,accountKey, tickerBoard, tickerCode,timeInt,terminalType,terminalKey);
 //           var s = Strategies.RegisterDefaultStrategy(name, code, a, t, timeInt, terminalType, terminalKey);
 //           //s.Parent = this;
 //           if (s == null)
 //           {
 //               Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, FullName,
 //                   "DefaultStrategy", "Failure: RegisterDefaultStrategy()",
 //                                       String.Format("Account={0}; Ticker={1}; TimeInt={2};", a, t, timeInt), "" );
 //               return null;
 //           }
 //           try
 //           {
 //               TradeStorage.Register(s);
 //               //TradeStorage.Register(s.Position);
 //               //TradeStorage.RegisterTotal(s.PositionTotal);
 //           }
 //           catch (Exception e)
 //           {
 //               SendExceptionMessage3("TradeContext", "DefaultStrategy",
 //                                       "RegisterDefaultStrategy().TradeStorage.Register() Failure",
 //                                       s.ToString() ,e);
 //               throw;
 //           }
            
 //           return s;
 //       }

 //       public void Save(IOrder3 o)
 //       {
 //           // TradeStorage.Add(o);
 //           TradeStorage.SaveChanges(StorageOperationEnum.Add, o);
 //       }

 //       public void Save(ITrade3 t)
 //       {
 //           //TradeStorage.Add(t);
 //           TradeStorage.SaveChanges(StorageOperationEnum.Add, t);
 //       }

 //       public IDeals BuildDeals()
 //       {
 //           return new Deals();
 //       }

 //       public TimePlan RegisterTimePlanEventHandler(string timePlanKey,
 //                                                                   EventHandler<TimePlanEventArgs> action)
 //       {
 //           return TimePlans.RegisterTimePlanEventHandler(timePlanKey, action);
 //       }

 //       public void ExecuteTick(DateTime dt, string tickerkey, double price, double bid, double ask)
 //       {
 //           throw new NotImplementedException();
 //       }

 //       private IPortfolios BuildPortfolios()
 //       {
 //           //var d = 0.123456789012345678901234567890123456789012345678901234567890012345678901234567890m;
 //           var ps = new Portfolios();
 //           var v = (from s in Strategies.StrategyCollection
 //                    group s by new { s.TradeAccountCode, s.TickerKey }
 //                        into g
 //                        select new
 //                        {
 //                            Acc = g.Key.TradeAccountCode,
 //                            Ticker = g.Key.TickerKey,
 //                            Str = g
 //                        }).ToList();

 //           foreach (var i in v.ToList())
 //           {
 //               var p = new Portfolio();
 //               var k = i.Acc + "@" + i.Ticker;
 //               p.Code = k;
 //               p.Name = "Portfolio" + k;
 //               p.Position.StrategyKeyEx = k;

 //               foreach (var s in i.Str.ToList())
 //               {
 //                   p.Register(s);
 //               }
 //               ps.Register(p);
 //           }

 //           foreach (var p in ps.Collection.Items)
 //           {
 //               var s = p.Key;
 //               foreach (var i in p.Items)
 //               {
 //                   var j = i.ToString();
 //               }
 //           }
 //           return ps;
 //       }

 //       public IPortfolio RegisterPortfolio(string code, string name)
 //       {
 //           //var p = new Portfolio { Code = code, Name = name };
 //           //IPortfolio ip;
 //           //if ((ip = Portfolios.AddNew(p)) != null)
 //           //    return ip;
 //           //throw new NullReferenceException("Portfolio Register Failure:" + p.ToString());

 //           return null;
 //       }
 //       public IEntryManager RegisterEntryManager(string code, string name)
 //       {
 //           //var p = new EntryManager { Code = code, Name = name };
 //           //IEntryManager ip;
 //           //if ((ip = EntryManagers.AddNew(p)) != null)
 //           //    return ip;
 //           //throw new NullReferenceException("EntryManager Register Failure:" + p.ToString());

 //           return null;
 //       }

     
 //       public bool SerializeInitData()
 //       {
 //           string xmlfname = null;
 //           TextWriter tr = null;
 //           try
 //           {
 //               var xDoc = XDocument.Load(@"SpsInit.xml");
 //               var xe = xDoc.Descendants("TradeContextInit_XmlFileName2").First();
 //               xmlfname = xe.Value;


 //               tr = new StreamWriter(xmlfname);
 //               // var sr = new XmlSerializer(typeof(Dictionary<string,Ticker>));  // !!! Not Support !!!!!
 //               var sr = new XmlSerializer(typeof(TradeContextInit));
 //               sr.Serialize(tr, TradeContextInit);
 //               tr.Close();

 //               Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TxInitData", "TxInitData", "Serialization",
 //               String.Format("FileName={0}", xmlfname), "");

 //               return true;
 //           }
 //           catch (Exception e)
 //           {
 //               Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "TxInitData", "TxInitData", "Serialization",
 //               String.Format("FileName={0}", xmlfname), e.ToString());

 //               if (tr != null) tr.Close();
 //               return false;
 //           }
 //       }
 //       private bool DeserializeInitData()
 //       {
 //           string xmlfname = null;
 //           try
 //           {
 //               var xDoc = XDocument.Load(@"SpsInit.xml");

 //               var xe = xDoc.Descendants("TradeContextInit_XmlFileName").First();
 //               xmlfname = xe.Value;

 //               var x = XElement.Load(xmlfname);

 //               TradeContextInit = Serialization.Do.DeSerialize<TradeContextInit>(x,
 //                   (s1, s2) => Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "TradeContextInit",
 //                                   "TradeContextInit", s1, String.Format("FileName={0}", xmlfname), s2)
 //                                   );

 //               Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TradeContextInit", "TradeContextInit", "DeSerialization",
 //               String.Format("FileName={0}", xmlfname), "");

 //           }
 //           catch (Exception e)
 //           {
 //               Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "TradeContextInit", "TradeContextInit", "DeSerialization",
 //               String.Format("FileName={0}", xmlfname), e.ToString());

 //               throw new SerializationException("TradeContextInit.Deserialization Failure " + xmlfname);
 //           }
 //           return true;
 //       }


 //       private void ExceptionRegister(object sender, Events.IEventArgs ea)
 //       {
 //           if (ea.OperationKey != "UI.Exceptions.Exception.Add".TrimUpper())
 //               return;
 //           TradeStorage.Add((IGSException)ea.Object);
 //       }
 //   }
}
