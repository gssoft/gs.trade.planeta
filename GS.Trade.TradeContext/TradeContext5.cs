using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Assemblies;
using GS.Configurations;
// using GS.Interfaces.Configurations;
using GS.Interfaces.Dde;
using GS.Trade.EntityWebClient;
using GS.Accounts;
using GS.Elements;
using GS.EventLog;
using GS.Events;
using GS.Exceptions;
using GS.Extension;
using GS.Interfaces;
using GS.Process;
using GS.Serialization;
using GS.Time.TimePlan;
//using GS.Trade.Data;

//using GS.Trade.Strategies;
//using GS.Trade.Dde;
using GS.Trade.Interfaces;
// 
// using GS.Trade.Strategies.Portfolio;
using GS.Trade.Trades;
using GS.Trade.Trades.Deals;
using GS.Trade.Trades.Orders3;
using GS.Trade.TradeTerminals64.Simulate;
using GS.Trade.Windows;
using GS.Trade.Windows.Charts;
using GS.Trade.WebClients;

// Unvisable but Need for Build Serialization
using GS.Trade.DataBase.Storage;
using GS.Moex.Interfaces;
using Moex;

// 15.11.11
using WebClients;

// using IMoexTicker = Moex.IMoexTicker;

namespace GS.Trade.TradeContext
{
    public partial class TradeContext5 : Element1<string>, ITradeContext
    {
        //[XmlIgnore]
        //public Tickers TickersForLoad { get; set; }

        //[XmlIgnore]
        //public Strategies.Strategies  StrategiesForLoad { get; set; }

        [XmlIgnore]
        public IEventLog TradeStorageEventLog { get; set; }
        [XmlIgnore]
        public IEventLog TradeTerminalsEventLog { get; set; }

        public override string Key => Code;
        public int GetFromDbMode { get; set; }
        public bool NeedToReCreateDb { get; set; }

        [XmlIgnore]
        public MoexTest Moex { get; set; }

        [XmlIgnore]
        public AutoResetEvent StraCloseAutoEvent { get; set; }
        [XmlIgnore]
        public AutoResetEvent DiaCloseAutoEvent { get; set; }
        [XmlIgnore]
        public AutoResetEvent UiCloseAutoEvent { get; set; }
        [XmlIgnore]
        protected WaitHandle[] WaitHandles;
        [XmlIgnore]
        public TradeContextInit TradeContextInit { get; set; }
        //    public TradeContextInit2 TradeContextInit2 { get; set; }
        [XmlIgnore]
        public ITrades Trades { get; set; }
        [XmlIgnore]
        public IOrders Orders { get; set; }
        [XmlIgnore]
        public IPositions Positions { get; set; }
        [XmlIgnore]
        //public Strategies.Strategies Strategies { get; set; }
        public IStrategies Strategies { get; set; }
        [XmlIgnore]
        public IPortfolios Portfolios { get; set; }
        //public EntryManagers EntryManagers { get; set; }
        [XmlIgnore]
        public ITradeStorage TradeStorage { get; set; }
        //public ITradeStorage Storage { get; set; }
        [XmlIgnore]
        public Orders3 ActiveOrders { get; set; }
        //[XmlIgnore]
        //public  IEventHub2 EventHub2 { get; set; }
        [XmlIgnore]
        public IEventHub EventHub { get; set; }
        [XmlIgnore]
        public IOrders SimulateOrders { get; set; }
        [XmlIgnore]
        public IAccounts Accounts { get; private set; }
        [XmlIgnore]
        public IAccounts AccountsAll { get; private set; }
        [XmlIgnore]
        public IDde Dde;
        [XmlIgnore]
        public Quotes.Quotes Quotes;
        [XmlIgnore]
        public ITickers Tickers { get; set; }
        [XmlIgnore]
        public ITickers TickersAll;
        [XmlIgnore]
        public IEnumerable<ITicker> TickerCollection => Tickers.TickerCollection;

        [XmlIgnore]
        public IEnumerable<IStrategy> StrategyCollection => Strategies.StrategyCollection;

        //    public GS.Trade.Trades.Time.TimePlans TimePlans { get; set; }
        [XmlIgnore]
        public TimePlans TimePlans { get; set; }
        //[XmlIgnore]
        //public BarRandom Rand;

        [XmlIgnore]
        public ITradeWebClient TradeWebClient { get; set; }

        [XmlIgnore]
        public IConfigurationResourse2 ConfigurationResourse { get; set; }
        [XmlIgnore]
        public IConfigurationResourse21 ConfigurationResourse1 { get; set; }

        public ITickers GetTickers => Tickers;
        public IStrategies GetStrategies => Strategies;

        [XmlIgnore]
        public ProcessManager2 StrategyProcess { get; set; }
        [XmlIgnore]
        public ProcessManager2 UIProcess { get; set; }
        [XmlIgnore]
        public ProcessManager2 DiagnosticProcess { get; set; }
        [XmlIgnore]
        public ProcessManager2 FastProcess { get; set; }
        [XmlIgnore]
        public EventLogWindow3 EventLogWindow;
        [XmlIgnore]
        public EventLogWindow3 TradeStorageEventLogWindow;
        [XmlIgnore]
        public EventLogWindow3 TradeTerminalsEventLogWindow;

        [XmlIgnore]
        public TradesWindow2 TradesWindow2;

        [XmlIgnore]
        public OrdersActiveWindow2 OrdersActiveWindow2;

        [XmlIgnore]
        public OrdersFilledWindow2 OrdersFilledWindow2;

        [XmlIgnore]
        public DealsWindow DealsWindow;
        [XmlIgnore]
        public PositionsWindow2 PositionsWindow2;
        [XmlIgnore]
        public PositionsWindow3 PositionsWindow3;
        [XmlIgnore]
        public PositionTotalsWindow2 PositionTotalsWindow2;
        [XmlIgnore]
        public PositionTotalsWindow3 PositionTotalsWindow3;
        [XmlIgnore]
        public PositionTotalsWindow4 PositionTotalsWindow4;

        [XmlIgnore]
        public PortfolioWindow PortfolioWindow;
        [XmlIgnore]
        public ExceptionsWindow ExceptionsWindow;

        [XmlIgnore]
        public TransactionsWindow TransactionsWindow;

        [XmlIgnore]
        public ChartWindow ChartWindow { get; set; }

        [XmlIgnore]
        public ITradeTerminals TradeTerminals { get; set; }

        //[XmlIgnore]
        //public ITradeTerminals64 TradeTerminals64 { get; set; }

        [XmlIgnore]
        public DbStorage32 DbStorageTemp;

        [XmlIgnore]
        public ISimulateTerminal SimulateTerminal;

        //public bool IsLongEnabled {
        //    get { return Portfolios != null && Portfolios.IsEnabled; }
        //}

        public TradeContext5()
        {
            StraCloseAutoEvent = new AutoResetEvent(false);
            DiaCloseAutoEvent = new AutoResetEvent(false);
            UiCloseAutoEvent = new AutoResetEvent(false);

            Trades = new Trades.Trades();
            Orders = new Orders();

            ActiveOrders = new Orders3();

            Positions = new Positions();

            SimulateOrders = new Orders();

            TimePlans = new TimePlans();

            // 15.11.11
            // Rand = new BarRandom(1000, 100);

            Quotes = new Quotes.Quotes();
            //TradeWebClient = new TradeWebClient2(@"http://62.173.154.142/positionsMvc_02/", @"application/xml",
            //        @"api/apiTrades") {QueuePostTimeInterval = 5};

        }
        public override void Init()
        {
            try
            {
                ExceptionEvent += ExceptionRegister;
                // 15.11.16
                // 15.11.29
                //var cnf1 = Configurations.ConfigurationResourse1.Instance;
                //var s = cnf1.Build<IStrategies>("Strats", "Strategies");
                //var s = cnf1.Build<IEventLog>("EventLog", "EventLogs");

                /* 15.11.30
                ConfigurationResourse = Configurations.ConfigurationResourse.Instance;
                // While EventHub does not work
                ConfigurationResourse.Parent = this;
                ConfigurationResourse.ExceptionEvent += ExceptionRegister;

                var xdoc = ConfigurationResourse.Get("EventLog");
                if (xdoc != null)
                {
                    EventLog = Builder.Build2<IEventLog>(xdoc, "EventLogs");
                    if (EventLog == null)
                        throw new NullReferenceException("TradeContext5.Init(): Failure in EventLog Building");

                    EventLog.Parent = this;
                    EventLog.ExceptionEvent += ExceptionRegister;
                    EventLog.Init();
                    EventLog.WhoAreYou(); // Exceptions to File
                }
                else
                    throw new NullReferenceException("TTradeContext5.Init(): Failure in EventLog Configuration");

                ConfigurationResourse.EventLog = EventLog;
                ConfigurationResourse.WhoAreYou();  // Exceptions to File
                */
                // 15.11.30
                #region 15.11.30
                ConfigurationResourse1 = Configurations.ConfigurationResourse1.Instance;
                // While EventHub does not work
                ConfigurationResourse1.Parent = this;
                ConfigurationResourse1.ExceptionEvent += ExceptionRegister;

                #region EventLog
                // EventLog = ConfigurationResourse1.Build<IEventLog>("EventLog", "EventLogs");
                EventLog = Builder.Build<EventLogs>(@"Init\EventLog.xml", "EventLogs");
                if (EventLog == null)
                    throw new NullReferenceException("TradeContext5.Init(): Failure in EventLog Building");
                EventLog.Parent = this;
                EventLog.ExceptionEvent += ExceptionRegister;
                EventLog.Init();
                EventLog.WhoAreYou(); // Exceptions to File

                // 2018.05.11 new EventLog with Processor Inside ProcessTask<IEvlItem>
                ((IEventLogs)EventLog)?.Start();
                // ((IEventLogs)EventLog)?.Stop();  // Don't forget insert this at the end
                #endregion
                #endregion

                // ExceptionEvent -= ExceptionRegister;

                EventHub = Builder.Build<EventHub>(@"Init\EventHub.xml", "EventHub");
                EventHub.Init(EventLog);
                EventHub.ExceptionEvent += ExceptionRegister;   // Internal EventHub Exception to TradeContext
                EventHub.Subscribe("UI.EXCEPTIONS", "EXCEPTION", ExceptionRegister);  // Exception from Others to TradeContext
                // EventHub.WhoAreYou();

                EventHub.Start();

                ExceptionsWindow = new ExceptionsWindow();
                ExceptionsWindow.Init(this);
                ExceptionsWindow.Show();

                EventLogWindow = new EventLogWindow3();
                EventLogWindow.Init(EventLog);
                ShowEventLogWindow();

                TradeStorageEventLog = Builder.Build2<IEventLog>(@"Init\EventLogMemProcTask.xml", "EventLogs");
                TradeStorageEventLog.Init();
                ((IEventLogs)TradeStorageEventLog)?.Start();

                TradeStorageEventLogWindow = new EventLogWindow3("TradeStorageEventLog");
                TradeStorageEventLogWindow.Init(TradeStorageEventLog);
                TradeStorageEventLogWindow.Show();

                // Only EventHub Internal Exceptions
                // EventHub.ExceptionEvent -= ExceptionRegister;   // Internal EventHub Exception to TradeContext
                EventHub.UnSubscribe("UI.EXCEPTIONS", "EXCEPTION", ExceptionRegister);

                // ******* UnSubscribe From this.ExceptionRegister ******************
                // No Need Handling Exceptions via ExceptionRegister ( Save Files with Exceptions )
                ConfigurationResourse1.ExceptionEvent -= ExceptionRegister;
                EventLog.ExceptionEvent -= ExceptionRegister;

                // No Need this.Exception  ExceptionRegister, while EVENTHUB is Live, 
                // and because Exception Window do that via EventHub
                this.ExceptionEvent -= ExceptionRegister;

                this.ExceptionEvent += EventHub.FireEvent;
                this.ChangedEvent += EventHub.FireEvent;

                ConfigurationResourse1.ExceptionEvent += EventHub.FireEvent;
                EventLog.ExceptionEvent += EventHub.FireEvent;

                EventHub.WhoAreYou(); // EventHub Shuold be appear in File Exceptions and in Window Exceptions
                ConfigurationResourse1.WhoAreYou();
                EventLog.WhoAreYou();

                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, FullName, "Environment:",
                                "Computer: " + Environment.MachineName,
                                "Domain: " + Environment.UserDomainName,
                                "User: " + Environment.UserName);

                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, FullName, "Configuration",ConfigurationResourse1.ConfigurationKey, "", "");
                
                // 15.11.30
                // ConfigurationResourse.LoadAssemblies();

                //EventLogWindow = new EventLogWindow3();
                //EventLogWindow.Init(EventLog);
                //ShowEventLogWindow();

                //EventHub.Init(EventLog);
                //EventHub.ExceptionEvent += ExceptionRegister;
                //EventHub.Subscribe("UI.EXCEPTIONS", "EXCEPTION", ExceptionRegister);

                //var xdoc = ConfigurationResourse.Get("DataBase");
                //if (xdoc == null)
                //{
                //    SendExceptionMessage3(Code, typeof (ConfigurationResourse).ToString(), "Init()", "",
                //        new NullReferenceException("ConfigurationResourse is null"));
                //    return;
                //}

                // 15.11.05 -> TradeStorage21
                // TradeStorage = Builder.Build2<ITradeStorage>(@"Init\TradeStorages.xml", "TradeStorage");

                //var xdoc = XDocument.Load(@"Init\TradeStorages21.xml");
                //TradeStorage = Builder.Build2<ITradeStorage, string, ITradeStorage>(xdoc, "TradeStorage21");

                //TradeStorage = Builder.Build2<ITradeStorage>(xdoc, "TradeStorage");

                // 15.11.30
                /*
                var xdoc = ConfigurationResourse.Get("DataBase");
                if (xdoc != null)
                {
                    TradeStorage = Builder.Build2<ITradeStorage, string, ITradeStorage>(xdoc, "TradeStorage21");
                    if (TradeStorage == null)
                    {
                        Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, Code, "TradeStorage", "Buld()",
                            "TradeStorage Build Failure", "");
                        return;
                    }
                    TradeStorage.Parent = this;
                    TradeStorage.ExceptionEvent += EventHub.FireEvent;
                    TradeStorage.ChangedEvent += EventHub.FireEvent;
                    TradeStorage.Init(EventLog);
                   // TradeStorage.WhoAreYou();
                }
                else
                {
                    SendExceptionMessage3(Code, "TradeStorage", "Configuration", "TradeStorage", new FileNotFoundException());
                    return;
                }
                */
                TradeStorage = ConfigurationResourse1.Build<ITradeStorage, string, ITradeStorage>("DataBase", "TradeStorage21");
                if (TradeStorage == null)
                {
                    SendExceptionMessage3(Code, "TradeStorage", "Configuration", "TradeStorage", new FileNotFoundException());
                    return;
                }
                TradeStorage.Parent = this;
                TradeStorage.ExceptionEvent += EventHub.FireEvent;
                TradeStorage.ChangedEvent += EventHub.FireEvent;

                //TradeStorageEventLogWindow = new EventLogWindow3();
                //TradeStorageEventLogWindow.Init(TradeStorageEventLog);
                //TradeStorageEventLogWindow.Show();

                // ShowEventLogWindow();

                // Thread.Sleep(1000);

                TradeStorage.Init(TradeStorageEventLog);

                //  Thread.Sleep(1000);

                TradeTerminalsEventLog = Builder.Build2<IEventLog>(@"Init\EventLogMemProcTask.xml", "EventLogs");
                TradeTerminalsEventLog.Init();
                ((IEventLogs)TradeTerminalsEventLog)?.Start();

                TradeTerminalsEventLogWindow = new EventLogWindow3("TradeTerminals EventLog");
                TradeTerminalsEventLogWindow.Init(TradeTerminalsEventLog);
                TradeTerminalsEventLogWindow.Show();

                
                    //TradeTerminals = new TradeTerminals.TradeTerminals()
                    //{
                    //    Name = "Trade Terminals",
                    //    Code = "TradeTerminals",
                    //    //Parent = this
                    //    //EventLog = EventLog
                    //    EventLog = TradeTerminalsEventLog
                    //};

                //TradeTerminals = new TradeTerminals.TradeTerminals
                //{
                //    Name = "Trade Terminals",
                //    Code = "TradeTerminals",
                //    //Parent = this
                //    //EventLog = EventLog
                //    EventLog = TradeTerminalsEventLog
                //};

                // TradeTerminals.ExceptionEvent += EventHub.FireEvent;
                // TradeTerminals.ExceptionEvent += FireEvent;

                TradeTerminals = new TradeTerminals64.TradeTerminals
                {
                    Name = "Trade Terminals",
                    Code = "TradeTerminals",
                    //Parent = this
                    //EventLog = EventLog
                    EventLog = TradeTerminalsEventLog
                };
                TradeTerminals.ExceptionEvent += FireEvent;

                // var p = Builder.Build<ProcessManager3>(@"Init\ProcessManager3.xml", "ProcessManager3");

                Positions.Init(EventLog);
                try
                {
                    TimePlans.Init(EventLog);
                }
                catch (Exception e)
                {
                    SendExceptionMessage3("TradeContext", "TimePlan", "Init()","", e);
                   // throw ;
                }
                Orders.Init(EventLog, Positions, Trades);
                Trades.Init(EventLog, Positions, Orders);

                SimulateOrders.Init(EventLog, Positions, Trades);

                // throw new NullReferenceException("Test Exceptions");

                Accounts = new Accounts.Accounts { Parent = this, Code = "Accounts", Name="Accounts" };
                AccountsAll = new Accounts.Accounts { Parent = this, Code = "AccountsAll", Name = "AccountsAll" };
                AccountsAll.Load();

                //Tickers = new Tickers { Parent = this, Code = "Tickers", Name = "Tickers" };
                //TickersAll = new Tickers { Parent = this, Code = "TickersAll", Name = "TickersAll" };

                //Tickers = Builder.Build2<ITickers>(@"Init\Tickers.xml", "Tickers");
                Tickers = Builder.Build3<ITickers>(@"Init\Tickers.xml", "Tickers");
                if (Tickers == null)
                    throw new NullReferenceException("Tickers is Null");
                    
                Tickers.Parent = this;
                Tickers.Init(EventLog);

                Tickers.Start();

                //TickersAll = Builder.Build2<ITickers>(@"Init\TickersAll.xml", "Tickers");
                TickersAll = Builder.Build3<ITickers>(@"Init\TickersAll.xml", "Tickers");
                if (TickersAll == null)
                    throw new NullReferenceException("TickersAll is Null");
                
                TickersAll.Parent = this;
                TickersAll.Init(EventLog);
                TickersAll.Load();

                Moex = Builder.Build3<MoexTest>(@"Init\Moex.xml", "MoexTest");
                Moex.Init();

                // Thread.Sleep(1000);

                // Need to ReCreate DataBase
                // ReCreatedDb();

                Orders.BackOrderExecMode = BackTestOrderExecutionMode.Optimistic;
                SimulateOrders.BackOrderExecMode = BackTestOrderExecutionMode.Optimistic;

                ActiveOrders.Init(EventLog);

                Accounts.WhoAreYou();
                Tickers.WhoAreYou();
                TradeTerminals.WhoAreYou();
                TradeStorage.WhoAreYou();
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.Init()", ToString(), e);
                // throw;
            }
        }

        private void FireEvent(object sender, IEventArgs args)
        {
            EventHub?.FireEvent(sender, args);
        }

        private void ReCreatedDb()
        {
            if (NeedToReCreateDb)
            {
                foreach (var a in AccountsAll.GetAccounts)
                {
                    try
                    {
                        TradeStorage.Register(a);
                    }
                    catch (Exception e)
                    {
                        SendExceptionMessage3("TradeContex", a.GetType().ToString(),
                            "Account.Register:" + a.Key, a.ToString(), e);
                        throw;
                    }
                }
                foreach (var t in TickersAll.GetTickers)
                {
                    try
                    {
                        TradeStorage.Register(t);
                    }
                    catch (Exception e)
                    {
                        SendExceptionMessage3(FullName,
                            t.GetType().ToString(), "Ticker.Register:" + t.Code, t.ToString(), e);
                        throw;
                    }
                }
            }
        }

        protected virtual void WindowsInit()
        {
            #region Windows Init

            //EventLogWindow = new EventLogWindow();

            TradesWindow2 = new TradesWindow2();

            //OrdersActiveWindow = new OrdersActiveWindow();
            OrdersActiveWindow2 = new OrdersActiveWindow2();

            OrdersFilledWindow2 = new OrdersFilledWindow2();


            DealsWindow = new DealsWindow();
            // PositionsWindow2 = new PositionsWindow2();
            PositionsWindow3 = new PositionsWindow3();
            // PositionTotalsWindow2 = new PositionTotalsWindow2();
            PositionTotalsWindow3 = new PositionTotalsWindow3();
            PositionTotalsWindow4 = new PositionTotalsWindow4();

            PortfolioWindow = new PortfolioWindow();

            //ExceptionsWindow = new ExceptionsWindow();
            //ExceptionsWindow.Init(this);

            TransactionsWindow = new TransactionsWindow();

            //OrderPlaneWindow = new OrderPlaneWindow();
            //OrderPlaneWindow.Init(this);

            //TradesWindow.Init(EventLog,Trades);
            TradesWindow2.Init(this);

            //OrdersActiveWindow.Init(this, EventLog, Orders);
            OrdersActiveWindow2.Init(this, EventLog);

            //OrdersFilledWindow.Init(this, EventLog, Orders);
            OrdersFilledWindow2.Init(this);

            DealsWindow.Init(this, EventLog);
            // PositionsWindow2.Init(this, EventLog);
            PositionsWindow3.Init(this, EventLog);
            // PositionTotalsWindow2.Init(this, EventLog);
            PositionTotalsWindow3.Init(this, EventLog);
            PositionTotalsWindow4.Init(this, EventLog);

            PortfolioWindow.Init(this, EventLog);

            TransactionsWindow.Init(EventLog, this);

            ChartWindow = new ChartWindow();
            ChartWindow.Init(this);

            //     QuotesWindow.Init(EventLog, Quotes);

            #endregion
        }
        public virtual void Open()
        {
            /*
            StrategyProcess = new ProcessManager2("Strategy_Process", 500, 0, EventLog);
            StrategyProcess.CloseAutoEvent = StraCloseAutoEvent;
            //StrategyProcess.CloseAutoEvent = (AutoResetEvent)WaitHandles[0];

            UIProcess = new ProcessManager2("UI_Process", 1000, 0, EventLog);
            UIProcess.CloseAutoEvent = UiCloseAutoEvent;
            //UIProcess.CloseAutoEvent = (AutoResetEvent)WaitHandles[1];

            DiagnosticProcess = new ProcessManager2("Diagnostic_Process", 1000, 0, EventLog);
            DiagnosticProcess.CloseAutoEvent = DiaCloseAutoEvent;
            //DiagnosticProcess.CloseAutoEvent = (AutoResetEvent)WaitHandles[2];

            // FastProcess = new ProcessManager2("Fast_Process", 100, 0, EventLog);
            try
            {
                Strategies = new Strategies.Strategies("MyStrategies", "xStyle", this);
                Strategies.Init();

                foreach (var s in Strategies.StrategyCollection)
                {
                    try
                    {
                        TradeStorage.Register(s);
                        TradeStorage.Register(s.Position);
                        TradeStorage.RegisterTotal(s.PositionTotal);
                    }
                    catch (Exception e)
                    {
                        SendExceptionMessage3("TradeContext", "Strategy","Register:" + s.StrategyTickerString,"", e);
                        throw;
                    }
                }

                Strategies.StrategyTradeEntityChangedEvent += EventHub.FireEvent;
                Strategies.ExceptionEvent += EventHub.FireEvent;

                //Portfolios = BuildPortfolios();
                // Portfolios with TimeInts
                Portfolios = BuildPortfolios2();
                Portfolios.ChangedEvent += EventHub.FireEvent;


                StrategyProcess.RegisterProcess("Orders DeQueue Process", "OrdersDeQueueProcess", (int)(1 * 500), 0,
                    null, TradeTerminals.DeQueueProcess, null);
                StrategyProcess.RegisterProcess("Trade Resolve Process", "TradeResolveProcess", (int)(1 * 1000), 0,
                    null, TradeTerminals.TradeResolveProcess, null);
                UIProcess.RegisterProcess("Order Resolve Process", "OrderResolveProcess", (int)(1 * 1000), 0, null,
                    TradeTerminals.OrderResolveProcess, null);

                TradeTerminals.ChangedEvent += EventHub.FireEvent;

                UIProcess.RegisterProcess("Event Hub Dequeue Process", "EventHib.DeQueue()", (int)(1 * 1000), 0, null,
                    EventHub.DeQueueProcess, null);


                UIProcess.RegisterProcess("TradeStorages DeQueue Process", "TradeStoragesDeQueueProcess",
                    (int)(1 * 1000), 0, null, TradeStorage.DeQueueProcess, null);
                UIProcess.RegisterProcess(Portfolios.Name, Portfolios.Code, (int)(15 * 1000), 0, null,
                    Portfolios.Refresh, null);

                Tickers.LoadBarsFromArch();
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.StrategyInit()", ToString(), e);
                throw new Exception(e.Message);
            }

            StrategyProcess.NewTickEvent += Tickers.UpdateTimeSeries;
            StrategyProcess.NewTickEvent += TimePlans.NewTickEventHandler;

            Tickers.NewTickEvent += SimulateOrders.ExecuteTick;

            DiagnosticProcess.RegisterProcess("Check Connections Process", 15000, 5000, null, TradeTerminals.CheckConnection, null);

            UIProcess.RegisterProcess("Update Last Price", "UpdateLastPriceProcess",
                                                    5000, 0, null, Strategies.UpdateLastPrice, null);
            UIProcess.RegisterProcess("EventLog Dequeue Process", "EventLog.DeQueue",
                                                    1000, 0, null, ((IEventLogs)EventLog).DeQueueProcess, null);
            // 15.09.24 Not need
            //UIProcess.RegisterProcess("Portfolio Refresh Process", "Portfolio.Refresh",
            //                                       1000, 0, null, Portfolios.Refresh, null);
            if(Dde2 != null)
                Dde2.RegisterDefaultCallBack(Tickers.PutDdeQuote3);

            WindowsInit();
            ShowWindows();
            //((IEventLogs)EventLog).SetMode(EvlModeEnum.Nominal);
             */ 
        }
        
        public virtual void OpenChart()
        {
            UIProcess.RegisterProcess("Chart Window Refresh", "ChartWindowRefreshProcess",
                                                    1000, 0, null, ChartWindow.TickRefreshEventHandler, null);
            ChartWindow.Show();
        }

        //public ITradeTerminals TradeTerminals { get; }

        public virtual void Stop()
        {
            try
            {
                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Stop Process Started", "", "");

                //foreach (var quik in TradeTerminals.TradeTerminalCollection.Values.OfType<IQuikTradeTerminal>())
                //{
                //    //quik.TransactionEvent -= EventHub.FireEvent;
                //    //quik.OrderEvent -= EventHub.FireEvent;
                //    //quik.TradeEvent -= EventHub.FireEvent;

                //    //quik.TradeEntityChangedEvent -= EventHub.FireEvent;
                //}
                TradeTerminals.ChangedEvent -= EventHub.FireEvent;

                Strategies.StrategyTradeEntityChangedEvent -= EventHub.FireEvent;
                EventLog.EventLogChangedEvent -= EventHub.FireEvent;

                StrategyProcess.NewTickEvent -= Tickers.UpdateTimeSeries;
                //     StrategyProcess.NewTickEvent -= TimePlans.NewTickEventHandler;

                //    StrategyProcess.NewTickEvent -= OrderPlaneWindow.UpdateQuote;

                StrategyProcess.NewTickEvent -= TimePlans.NewTickEventHandler;

                // 15.11.11
                //Rand.NewTickEvent -= Orders.ExecuteTick;
                //Rand.NewTickStrEvent -= Tickers.PutDdeQuote3;

                Tickers.NewTickEvent -= Orders.ExecuteTick;
                Tickers.NewTickEvent -= SimulateOrders.ExecuteTick;

                //   Dde.Stop();
                Dde?.Stop();

                TimePlans.Stop();
                
                // 15.11.11
                // Rand.Stop();

                TradeTerminals.DisConnect();

                TradeTerminals.ExceptionEvent -= EventHub.FireEvent;

                CloseWindows();

                StrategyProcess.Stop();
                StraCloseAutoEvent.WaitOne();

                DiagnosticProcess.Stop();
                DiaCloseAutoEvent.WaitOne();

                UIProcess.Stop();
                UiCloseAutoEvent.WaitOne();

                // WaitHandle.WaitAll(WaitHandles);

                Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Stop Process Finished", "", "");
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.Stop()", ToString(), e);
                // throw;
            }          
        }
        public virtual void Close()
        {
            try
            {
                //CloseWindows();

                // Dde.Close();
                //if(Dde2!=null)
                //    Dde2.Close();

                StrategyProcess.Close();
                DiagnosticProcess.Close();
                UIProcess.Close();

                // 19.08.06
                //try
                //{
                //    Strategies.Close();
                //}
                //catch (Exception e)
                //{
                //    SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.Strategies.Close()", ToString(), e);
                //}

                // CloseWindows();

                // EventLogWindow.Close();
                ((IEventLogs)TradeTerminalsEventLog)?.Stop();
                ((IEventLogs)TradeStorageEventLog)?.Stop();
                ((IEventLogs)EventLog)?.Stop();
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.Close()", ToString(), e);
            }
        }
        public virtual void Start()
        {
            Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Begin Start Process", "", "");

            UIProcess.Start();
            StrategyProcess.Start();
            DiagnosticProcess.Start();

           Dde?.Start();

            // Rand.Start();

            Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Finish Start Process", "", "");

            ((IEventLogs)EventLog).SetMode(EvlModeEnum.Nominal);

        }
        public virtual void StartRand()
        {
            // 15.11.11
            //Rand.Start();
        }

        public virtual void ShowWindows()
        {
            //EventLogWindow.Show();

            //TradesWindow.Show();
            TradesWindow2.Show();

            //OrdersActiveWindow.Show();
            OrdersActiveWindow2.Show();

            //OrdersFilledWindow.Show();
            OrdersFilledWindow2.Show();

            //PositionsOpenedWindow.Show();
            //PositionsClosedWindow.Show();
            //PositionTotalsWindow.Show();
            //PositionsWindow.Show();

            DealsWindow.Show();
            //PositionsWindow2.Show();
            //PositionTotalsWindow2.Show();

            PositionsWindow3.Show();
            PositionTotalsWindow3?.Show();
            PositionTotalsWindow4?.Show();

            PortfolioWindow.Show();

            //ExceptionsWindow.Show();

            TransactionsWindow.Show();

            //OrderPlaneWindow.Show();

            // QuotesWindow.Show();

            // ChartWindow.Show();
        }
        public virtual void CloseWindows()
        {
            //EventLogWindow.Close();

            ChartWindow.Close();

            TradesWindow2.Close();
            OrdersActiveWindow2.Close();
            OrdersFilledWindow2.Close();

            //PositionsOpenedWindow.Close();
            //PositionsClosedWindow.Close();
            //PositionTotalsWindow.Close();
            //PositionsWindow.Close();

            DealsWindow.Close();

            // PositionsWindow2.Close();
            PositionsWindow3?.Close();
            // PositionTotalsWindow2?.Close();
            PositionTotalsWindow3?.Close();
            PositionTotalsWindow4?.Close();

            PortfolioWindow?.Close();
            TransactionsWindow?.Close();

            // OrderPlaneWindow.Close();

            // QuotesWindow.Show();
        }

        public virtual void ShowEventLogWindow()
        {
            EventLogWindow.Show();
        }

        public virtual IAccount GetAccount(string key)
        {
            return Accounts.GetAccount(key);
        }
        public virtual IAccount RegisterAccount(string key)
        {
            var ac = default(IAccount);
            try
            {
                if (Accounts == null)
                    throw new NullReferenceException("RegisterAccount(): Accounts==null");

                ac = Accounts.GetAccount(key);
                if (ac != null)
                    return ac;

                if (AccountsAll == null)
                    throw new NullReferenceException("RegisterAccount(): AccountsAll==null");

                ac = AccountsAll.GetAccount(key);
                if (ac != null)
                {
                    Accounts.Register(ac);
                    return ac;
                }
                var accountBase = GetFromDbMode > 0 ? TradeStorage.GetAccountByKey(key) : TradeStorage.GetAccount(key);
                if (accountBase == null)
                {
                    var a = Accounts.CreateInstanceSimple(key);
                    return a;
                }
                ac = Accounts.CreateInstance(accountBase); 
                Accounts.Register(ac);
                return ac;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName,
                            ac?.GetType().ToString() ?? "IAccount", "RegisterAccount()",
                            ac?.ToString() ?? "IAccount", e);
                throw;
            }
        }
        public virtual ITicker GetTicker(string key)
        {
            return Tickers.GetTicker(key);
        }
        public virtual ITicker RegisterTicker(string key)
        {
            var ti = default(ITicker);
            try
            {
                if (Tickers == null)
                    throw new NullReferenceException("RegisterTicker(): Ticker==null");

                ti = Tickers.GetTicker(key);
                if (ti != null)
                {
                    //if (ti.Id == 0)
                    //    TradeStorage.Register(ti);
                    return ti;
                }
                if (TickersAll == null)
                    throw new NullReferenceException("RegisterTicker(): TickerAll==null");

                ti = TickersAll.GetTicker(key);
                if (ti != null)
                {
                    Tickers.Add(ti);
                    return ti;
                }
                var t = GetFromDbMode > 0 ? TradeStorage.GetTickerByKey(key) : TradeStorage.GetTicker(key);
                if (t != null)
                {
                    ti = Tickers.CreateInstance(t);
                    Tickers.Add(ti);
                    return ti;
                }
                return null;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName,
                            ti?.GetType().ToString() ?? "ITicker", "RegisterTicker()",
                            ti?.ToString() ?? "Ticker", e);
               // throw;
            }
            return null;
        }
        public virtual ITicker RegisterTicker(string tradeboard, string tickercode)
        {
            var ti = default(ITicker);
            try
            {
                if (Tickers == null)
                    throw new NullReferenceException("RegisterTicker(): Ticker==null");

                ti = Tickers.GetTicker(tickercode);
                if (ti != null)
                {
                    //if (ti.Id == 0)
                    //    TradeStorage.Register(ti);
                    return ti;
                }
                var moexTicker = default(IMoexTicker);
                if (tradeboard.HasValue() && tickercode.HasValue())
                {
                    moexTicker = Moex?.GetTicker(tradeboard, tickercode);
                }
                else if (tradeboard.HasNoValue() && tickercode.HasValue())
                {
                    moexTicker = Moex?.GetTicker(tickercode);
                }
                if (moexTicker != null)
                {
                    ti = Tickers.CreateInstanceFromMoex(moexTicker);
                    Tickers.Add(ti);
                    return ti;
                }

                //if (TickersAll == null)
                //    throw new NullReferenceException("RegisterTicker(): TickerAll==null");

                ti = TickersAll.GetTicker(tickercode);
                if (ti != null)
                {
                    Tickers.Add(ti);
                    return ti;
                }
                var t = GetFromDbMode > 0 ? TradeStorage.GetTickerByKey(tickercode) : TradeStorage.GetTicker(tickercode);
                if (t != null)
                {
                    ti = Tickers.CreateInstance(t);
                    Tickers.Add(ti);
                    return ti;
                }
                return null;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName,
                            ti?.GetType().ToString() ?? "ITicker", "RegisterTicker()",
                            ti?.ToString() ?? "Ticker", e);
                // throw;
            }
            return null;
        }
        public virtual ITimeSeries RegisterTimeSeries(ITimeSeries ts)
        {
            //var t = ts as TimeSeries;
            //return t.Ticker.RegisterTimeSeries(t);
            return ts.Ticker.RegisterTimeSeries(ts);
        }
        public virtual ITimeSeries GetTimeSeriesOrNull(string key)
        {
            foreach (ITicker t in Tickers.TickerCollection)
            {
                ITimeSeries ts;
                if ((ts = t.GetTimeSeries(key)) != null) return ts;
            }
            return null;
        }

        public virtual ITradeTerminal RegisterTradeTerminal(string type, string key)
        {
            var tt = TradeTerminals.RegisterTradeTerminal(type, key, Orders, SimulateOrders, Trades, EventLog, this);
            if (tt == null)
                throw new NullReferenceException($"TradeTerminal {type} {key} is Not Found");

            //  Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TradeTerminal", "Register", type + " " + key, "");
            return tt;
        }
        public virtual IPosition RegisterPosition(string account, string strategy, ITicker ticker)
        {
            var p = Positions.Register2(account, strategy, ticker);
            p.EventLog = EventLog;
            return p;
        }

        public virtual IPosition2 RegisterPosition(IStrategy s)
        {
            //var p = Positions.Register2(account, strategy, ticker);
            //p.EventLog = EventLog;
            //return p;
            return new Position2
            {
                Strategy = s,
                Status = PosStatusEnum.Closed,
                Operation = PosOperationEnum.Neutral,
                EventLog = EventLog,
                PositionTotal = new PositionTotal2
                {
                    Strategy = s,
                    Status = PosStatusEnum.Closed,
                    Operation = PosOperationEnum.Neutral,
                    EventLog = EventLog
                }
            };
        }
        public virtual IEnumerable<IPosition2> GetPositionCurrents()
        {
            return Strategies.GetPositionCurrents();
        }

        public virtual IEnumerable<IPosition2> GetPositionTotals()
        {
            return Strategies.GetPositionTotals();
        }

        public virtual IEnumerable<IPosition2> GetDeals()
        {
            return Strategies.GetDeals();
        }

        public TimePlan RegisterTimePlanEventHandler(string timePlanKey, EventHandler a)
        {
            // throw new NotImplementedException();
            return null;
        }

        //public TimePlan RegisterTimePlanEventHandler(string timePlanKey, EventHandler a)
        //{
        //    throw new NotImplementedException();
        //}

        public virtual ITrade3 Resolve(ITrade3 t)
        {
            var stratKey = TradeStorage.GetStrategyKeyFromOrder(t.OrderKey);
            if (stratKey.HasNoValue())
                return null;
            var s = Strategies.GetByKey(stratKey);
            if (s == null)
                return null;
            t.Strategy = s;
            return t;
            //throw new NotImplementedException();
        }

        public virtual IOrder Resolve(IOrder o)
        {
            throw new NotImplementedException();
        }

        public virtual IOrder3 Resolve(IOrder3 o)
        {
            var stratKey = TradeStorage.GetStrategyKeyFromOrder(o.Key);
            if (stratKey.HasNoValue())
                return null;
            var s = Strategies.GetByKey(stratKey);
            if (s == null)
                return null;
            o.Strategy = s;
            return o;
            //throw new NotImplementedException();
        }

        public void CloseStrategies()
        {
            Strategies?.CloseAll();
        }

        public void EnableEntries()
        {
            Strategies?.EnableEntry();
        }

        public void DisableEntries()
        {
            Strategies?.DisableEntry();
        }

        public void SetWorkingStatus(bool status)
        {
            Strategies?.SetWorkingStatus(status);
        }

        public virtual IStrategy GetStrategyByKey(string key)
        {
            var stratKey = TradeStorage.GetStrategyKeyFromOrder(key);
            if (stratKey.HasNoValue())
                return null;
            return Strategies.GetByKey(stratKey);
        }

        public virtual IStrategy GetDefaultStrategy(IOrder3 order)
        {
            return Strategies?.GetDefaultStrategy(order);
        }

        public virtual IStrategy RegisterDefaultStrategy(string name, string code, string accountKey,
                                                    string tickerBoard, string tickerCode, uint timeInt,
                                                    string terminalType, string terminalKey)
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";
            //var t = GetTicker(tickerBoard + "@" + tickerCode);
            //var a = GetAccount(accountKey);

            //if (t == null || a == null)
            //{
            //    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, FullName, "DefaultStrategy",
            //                "Failure To Create DeafultStrategy from Order", "NOT FOUND: " +
            //                "Ticker=" + (tickerBoard + "@" + tickerCode).WithSqBrackets0() + " Acc=" + accountKey.WithSqBrackets0(), "");
            //    return null;
            //}
            //var s = Strategies.RegisterDefaultStrategy(name,code,accountKey, tickerBoard, tickerCode,timeInt,terminalType,terminalKey);
            // var s = Strategies.RegisterDefaultStrategy(name, code, a, t, timeInt, terminalType, terminalKey);
            var s = Strategies.GetStrategy(code, accountKey, tickerBoard, tickerCode, (int) timeInt);
            if (s != null)
            {
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Code, s.StrategyTimeIntTickerString,
                        methodname, "Strategy Already Registered", s.ToString());
                return s;
            }

            s = Strategies.RegisterDefaultStrategy(name, code,
                        accountKey, tickerBoard, tickerCode, timeInt, terminalType, terminalKey);
            //s.Parent = this;
            if (s == null)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, FullName,
                    "DefaultStrategy", "Failure: RegisterDefaultStrategy()",
                    $"Account={accountKey}; Ticker={tickerCode}; TimeInt={timeInt};", "");
                return null;
            }
            try
            {
                TradeStorage.Register(s);
                //TradeStorage.Register(s.Position);
                //TradeStorage.RegisterTotal(s.PositionTotal);
            }
            catch (Exception e)
            {
                SendExceptionMessage3("TradeContext", "DefaultStrategy",
                                        "RegisterDefaultStrategy().TradeStorage.Register() Failure",
                                        s.ToString(), e);
                // throw;
            }

            return s;
        }

        public virtual void Save(IOrder3 o)
        {
            // TradeStorage.Add(o);
            TradeStorage.SaveChanges(StorageOperationEnum.Add, o);
        }

        public virtual void Save(ITrade3 t)
        {
            //TradeStorage.Add(t);
            TradeStorage.SaveChanges(StorageOperationEnum.Add, t);
        }

        #region Publish Entities
        public virtual void Publish(IOrder3 o)
        {
        }
        public virtual void Publish(ITrade3 t)
        {
        }
        public virtual void Publish(IPosition2 p)
        {
        }
        public virtual void Publish(IPositionTotal2 p)
        {
        }
        public virtual void Publish(IDeal d)
        {
        }
        #endregion

        public virtual IDeals BuildDeals()
        {
            return new Deals();
        }

        public virtual ITimePlan RegisterTimePlanEventHandler(string timePlanKey,
                                                                    EventHandler<ITimePlanEventArgs> action)
        {
            return TimePlans.RegisterTimePlanEventHandler(timePlanKey, action);
        }

        public virtual void ExecuteTick(DateTime dt, string tickerkey, double price, double bid, double ask)
        {
            SimulateTerminal?.ExecuteTick(dt, tickerkey, price, bid, ask);
        }

        //protected IPortfolios BuildPortfolios()
        //{
        //    //var d = 0.123456789012345678901234567890123456789012345678901234567890012345678901234567890m;
        //    var ps = new Portfolios {Parent = this};
        //    var v = (from s in Strategies.StrategyCollection
        //             group s by new { s.TradeAccountCode, s.TickerKey }
        //                 into g
        //                 select new
        //                 {
        //                     Acc = g.Key.TradeAccountCode,
        //                     Ticker = g.Key.TickerKey,
        //                     Str = g
        //                 }).ToList();

        //    foreach (var i in v.ToList())
        //    {
        //        var p = new Portfolio { Parent = ps };
        //        var k = i.Acc + "@" + i.Ticker;
        //        p.Code = k;
        //        p.Name = "Portfolio" + k;
        //        p.Position.StrategyKeyEx = k;

        //        foreach (var s in i.Str.ToList())
        //        {
        //            p.Register(s);
        //            s.PortfolioRisk = p;
        //        }
        //        p.Init();
        //        ps.Register(p);
        //    }
        //    Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Portfolios", ps.Code, "Portfolio List","", "");
        //    foreach (var p in ps.Collection.Items)
        //    {
        //        var pKey = p.Key;
        //        Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Portfolio", pKey, "StrategyList", "Porfolio StrategyList Below","");
        //        foreach (var s in p.Items)
        //        {
        //            var j = s.ToString();
        //            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Portfolio", pKey, "Strategy", s.ToString(), "");
        //        }
        //    }
        //    return ps;
        //}
       
        //// Portfolio with TimeInts
        //protected IPortfolios BuildPortfolios2()
        //{
        //    //var d = 0.123456789012345678901234567890123456789012345678901234567890012345678901234567890m;
        //    var ps = new Portfolios { Parent = this };
        //    var v = (from s in Strategies.StrategyCollection
        //             group s by new { s.TradeAccountCode, s.TickerKey, s.TimeInt }
        //                 into g
        //                 select new
        //                 {
        //                     Acc = g.Key.TradeAccountCode,
        //                     Ticker = g.Key.TickerKey,
        //                     g.Key.TimeInt,
        //                     Str = g
        //                 }).ToList();

        //    foreach (var i in v.ToList())
        //    {
        //        var p = new Portfolio { Parent = ps };
        //        var k = i.Acc + "@" + i.Ticker + "@" + i.TimeInt;
        //        p.Code = k;
        //        p.Name = "Portfolio" + k;
        //        p.Position.StrategyKeyEx = k;

        //        foreach (var s in i.Str.ToList())
        //        {
        //            p.Register(s);
        //            s.PortfolioRisk = p;
        //        }
        //        p.Init();
        //        ps.Register(p);
        //    }
        //    Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Portfolios", ps.Code, "Portfolio List", "", "");
        //    foreach (var p in ps.Collection.Items)
        //    {
        //        var pKey = p.Key;
        //        Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Portfolio", pKey, "StrategyList", "Porfolio StrategyList Below", "");
        //        foreach (var s in p.Items)
        //        {
        //            //var j = s.ToString();
        //            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Portfolio", pKey, "Strategy", s.ToString(), "");
        //        }
        //    }
        //    return ps;
        //}
        protected IPortfolios BuildPortfolios3()
        {
            //var d = 0.123456789012345678901234567890123456789012345678901234567890012345678901234567890m;
            //var ps = new Portfolios { Parent = this };
            // var ps = Builder.Build2<IPortfolios>(@"Init\Portfolios.xml", "Portfolios");
            var ps = Builder.Build3<IPortfolios>(@"Init\Portfolios.xml", "Portfolios");
            if (ps == null)
                return null;

            ps.Parent = this;
            var ats = (from s in Strategies.StrategyCollection
                     group s by new { s.TradeAccountCode, s.TickerKey }
                         into g
                         select new
                         {
                             Acc = g.Key.TradeAccountCode,
                             Ticker = g.Key.TickerKey,
                             Str = g
                         }).ToList();

            foreach (var i in ats)
            {
                // var p = new Portfolio { Parent = ps, MaxSideInitValue = 1};
                // var p = Builder.Build2<IPortfolioRisk>(@"Init\Portfolios.xml", "Portfolio");
                var p = Builder.Build3<IPortfolioRisk>(@"Init\Portfolios.xml", "Portfolio");
                if (p == null)
                    continue;
                p.Parent = this;
                // p.MaxSideInitValue = 1;

                var k = i.Acc + "@" + i.Ticker;
                p.Code = k;
                //p.Name = "Portfolio.AT" + k;
                p.Name = "Portfolio.AT";
                p.Position.StrategyKeyEx = k;

                foreach (var s in i.Str.ToList())
                {
                    p.Register(s);
                    s.PortfolioRisk = p;
                    s.EntryPortfolioEnabled = true;
                }
                p.Init();
                ps.Register(p);
            }
            /*
            var atts = (from s in Strategies.StrategyCollection.Where(s=>s.PortfolioEnable)
                     group s by new { s.TradeAccountCode, s.TickerKey, s.TimeInt, s.PortfolioKey }
                         into g
                         select new
                         {
                             Acc = g.Key.TradeAccountCode,
                             Ticker = g.Key.TickerKey,
                             g.Key.TimeInt,
                             g.Key.PortfolioKey,
                             Str = g
                         }).ToList();

            foreach (var i in atts.ToList())
            {
                //var p = new Portfolio { Parent = ps, MaxSideInitValue = 2f/3f};
                //var p = Builder.Build2<IPortfolioRisk>(@"Init\Portfolios.xml", "Portfolio");
                var p = Builder.Build3<IPortfolioRisk>(@"Init\Portfolios.xml", "Portfolio");
                if (p == null)
                    continue;
                p.Parent = this;
                // p.MaxSideInitValue = 2f / 3f;

                var k = i.Acc + "@" + i.Ticker + "@" + i.TimeInt + (i.PortfolioKey.HasValue() ? "@" + i.PortfolioKey : "");
                p.Code = k;
                //p.Name = "Portfolio.ATT@" + k;
                p.Name = "Portfolio.ATT";
                p.Position.StrategyKeyEx = k;

                foreach (var s in i.Str.ToList())
                {
                    p.Register(s);
                    s.PortfolioRisk = p;
                    s.EntryPortfolioEnabled = true;
                }
                p.Init();
                ps.Register(p);
            }
            */
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Portfolios", ps.Code, "Portfolio List", "", "");
            foreach (var p in ps.Items)
            {
                var pKey = p.Key;
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Portfolio", pKey, "StrategyList", "Porfolio StrategyList Below", "");
                foreach (var s in p.Items)
                {
                    //var j = s.ToString();
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Portfolio", pKey, "Strategy", s.ToString(), "");
                }
            }
            ps.WhoAreYou();
            return ps;
        }

        public virtual IPortfolio RegisterPortfolio(string code, string name)
        {
            //var p = new Portfolio { Code = code, Name = name };
            //IPortfolio ip;
            //if ((ip = Portfolios.AddNew(p)) != null)
            //    return ip;
            //throw new NullReferenceException("Portfolio Register Failure:" + p.ToString());

            return null;
        }
        public virtual IEntryManager RegisterEntryManager(string code, string name)
        {
            //var p = new EntryManager { Code = code, Name = name };
            //IEntryManager ip;
            //if ((ip = EntryManagers.AddNew(p)) != null)
            //    return ip;
            //throw new NullReferenceException("EntryManager Register Failure:" + p.ToString());

            return null;
        }

        public virtual bool SerializeInitData()
        {
            string xmlfname = null;
            TextWriter tr = null;
            try
            {
                var xDoc = XDocument.Load(@"SpsInit.xml");
                var xe = xDoc.Descendants("TradeContextInit_XmlFileName2").First();
                xmlfname = xe.Value;


                tr = new StreamWriter(xmlfname);
                // var sr = new XmlSerializer(typeof(Dictionary<string,Ticker>));  // !!! Not Support !!!!!
                var sr = new XmlSerializer(typeof(TradeContextInit));
                sr.Serialize(tr, TradeContextInit);
                tr.Close();

                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TxInitData", "TxInitData", "Serialization",
                    $"FileName={xmlfname}", "");

                return true;
            }
            catch (Exception e)
            {
                Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "TxInitData", "TxInitData", "Serialization",
                    $"FileName={xmlfname}", e.ToString());

                tr?.Close();
                return false;
            }
        }
        private bool DeserializeInitData()
        {
            string xmlfname = null;
            try
            {
                var xDoc = XDocument.Load(@"SpsInit.xml");

                var xe = xDoc.Descendants("TradeContextInit_XmlFileName").First();
                xmlfname = xe.Value;

                var x = XElement.Load(xmlfname);

                TradeContextInit = Serialization.Do.DeSerialize<TradeContextInit>(x,
                    (s1, s2) => Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "TradeContextInit",
                                    "TradeContextInit", s1, $"FileName={xmlfname}", s2)
                                    );

                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TradeContextInit", "TradeContextInit", "DeSerialization",
                    $"FileName={xmlfname}", "");

            }
            catch (Exception e)
            {
                Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "TradeContextInit", "TradeContextInit", "DeSerialization",
                    $"FileName={xmlfname}", e.ToString());

                throw new SerializationException("TradeContextInit.Deserialization Failure " + xmlfname);
            }
            return true;
        }

        public virtual void ExceptionRegister(object sender, Events.IEventArgs ea)
        {         
            GSException.SaveInFile(@"Exceptions\", ea);       
        }

        public void ClearOrderCollection()
        {
            Strategies.ClearOrderCollection();
        }
        public void SetStrategiesLongShortEnabled(bool longenabled, bool shortenabled)
        {
            Strategies.SetStrategiesLongShortEnabled(longenabled, shortenabled);
        }
        public void SetPortfolioMaxSideSize(int maxsidesize)
        {
            Portfolios?.SetMaxSideSize(maxsidesize);
        }
    }
}
