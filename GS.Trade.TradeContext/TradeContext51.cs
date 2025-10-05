using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Accounts;
using GS.Elements;
using GS.Events;
using GS.Exceptions;
using GS.Extension;
using GS.Interfaces;
using GS.Interfaces.Dde;
using GS.Process;
using GS.Serialization;
using GS.Time.TimePlan;
//using GS.Trade.Data;
using GS.Trade.DataBase.Storage;

//using GS.Trade.Dde;
using GS.Trade.EntityWebClient;
using GS.Trade.Interfaces;
// using GS.Trade.Strategies;
// using GS.Trade.Strategies.Portfolio;
using GS.Trade.Trades;
using GS.Trade.Trades.Deals;
using GS.Trade.Trades.Orders3;
using GS.Trade.TradeTerminals64.Simulate;
using GS.Trade.Windows;
using GS.Trade.Windows.Charts;
using GS.Xml;

namespace GS.Trade.TradeContext
{
    public  class TradeContext51 : TradeContext5 // Element1<string>, ITradeContext
    {
        public TradeContext51()
        {
            Code = "TradeContext51";
            Name = Code;
            Alias = Code;
        }
        public override void Init()
        {
            try
            {
                //var assLoaded = AppDomain.CurrentDomain.GetAssemblies();

                base.Init();

                // assLoaded = AppDomain.CurrentDomain.GetAssemblies();

                //TradeWebClient = new TradeWebClient2(@"http://62.173.154.142/positionsMvc_02/", @"application/xml",
                //        @"api/apiTrades") { QueuePostTimeInterval = 15 };

                //var xdoc = ConfigurationResourse.Get("DataBase");
                //if (xdoc != null)
                //{
                //    TradeStorage = Builder.Build2<ITradeStorage, string, ITradeStorage>(xdoc, "TradeStorage21");
                //    if (TradeStorage == null)
                //    {
                //        Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, Code, "TradeStorage", "Buld()",
                //            "TradeStorage Build Failure", "");
                //        return;
                //    }
                //    TradeStorage.ExceptionEvent += EventHub.FireEvent;
                //    TradeStorage.ChangedEvent += EventHub.FireEvent;
                //    TradeStorage.Init(EventLog);
                //}
                //else
                //{
                //    SendExceptionMessage3(Code, "TradeStorage", "Configuration", "TradeStorage", new FileNotFoundException());
                //}
                // 15.11.30
                /*
                var xdoc = ConfigurationResourse.Get("Dde");
                if (xdoc != null)
                {
                    //Dde2 = Builder.Build2<IDde,string,TopicItem>(xdoc, "Dde21");
                    //Dde2 = Builder.Build2<IDde,string, IElement1<string>>(xdoc, "Dde21");
                    Dde2 = Builder.Build2<IDde, string, ITopicItem>(xdoc, "Dde21");
                    if (Dde2 == null)
                    {
                        Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, Code, "Dde", "Build()", "Dde Build Failure", "");
                        return;
                    }
                    Dde2.Parent = this;
                    Dde2.Init(EventLog);
                }
                */
                // Dde25
                // 2018.05.12
                // Dde2 = ConfigurationResourse1.Build<IDde, string, ITopicItem>("Dde", "Dde21");
                Dde = ConfigurationResourse1.Build<IDde, string, ITopicItem>("Dde", "Dde");
                if (Dde == null)
                {
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, Code, "Dde", "Init()", "Dde Init Failure", "");
                    SendExceptionMessage3(Code, "Dde", "Configuration", "Dde", new FileNotFoundException());
                    return;
                }
                Dde.Parent = this;
                Dde.Init();
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.Init()", ToString(), e);
                // e.Message, e.Source, e.GetType().ToString(), e.TargetSite.ToString());
                // throw;
            }
        }
        public override void Open()
        {
            try
            {
                //var assLoaded = AppDomain.CurrentDomain.GetAssemblies();
                var timeInterval = 1*1000;
                StrategyProcess = new ProcessManager2("Strategy_Process", timeInterval, 0, EventLog);
                StrategyProcess.CloseAutoEvent = StraCloseAutoEvent;
                //StrategyProcess.CloseAutoEvent = (AutoResetEvent)WaitHandles[0];

                UIProcess = new ProcessManager2("UI_Process", 1000, 0, EventLog);
                UIProcess.CloseAutoEvent = UiCloseAutoEvent;
                //UIProcess.CloseAutoEvent = (AutoResetEvent)WaitHandles[1];

                DiagnosticProcess = new ProcessManager2("Diagnostic_Process", 1000, 0, EventLog);
                DiagnosticProcess.CloseAutoEvent = DiaCloseAutoEvent;
                //DiagnosticProcess.CloseAutoEvent = (AutoResetEvent)WaitHandles[2];

                // FastProcess = new ProcessManager2("Fast_Process", 100, 0, EventLog);

                // 2018.05.12 ProcessTask
                //StrategyProcess.RegisterProcess("Tickers.DdeQuotes DeQueue Process1",
                //    "Tickers.QuotesDdeQueueProcess1()",
                //    (int) (1*1000), 0,
                //    null, Tickers.DeQueueDdeQuoteStrProcess1, null);

                Thread.Sleep(1000);

                try
                {
                    // Strategies = new Strategies.Strategies("MyStrategies", "xStyle", this);
                    // Strategies = Builder.Build2<IStrategies>(@"Init\736Z_M5_210921.xml", "Strategies");
                    //15.11.30
                    // Strategies = Builder.Build3<IStrategies>(@"Init\736Z_M5_210921.xml", "Strategies");

                    // var typesstr = Builder.GetTypeStrListEnumerable(@"Init\736Z_M5_210921.xml", "Strategies", "GS.Trade.Strategies");
                    Strategies = ConfigurationResourse1.Build<IStrategies>("Strats", "Strategies");
                    // var typesstr = 
                    //    Builder.GetTypeStrListEnumerable(@"Init\736Z_M5_210921.xml", "Strategies", "GS.Trade.Strategies");
                    //var strs = XDocExtensions.DeSerialize<IStrategy>(@"Init\736Z_M5_210921.xml",
                    //    "Strategies", "GS.Trade.Strategies", "GS.Trade.Strategies")

                    //if (Strategies == null)
                    //{ 
                    //    throw new NullReferenceException("Stategies Build Failure");
                    //}
                    // Should be realize without 
                    // 23.09.28 without Configurations
                    // Strategies = Builder.DeSerialization<Strategies.Strategies>(@"Init\StrategiesType.xml", "Strategies");
                    // 
                    Strategies.Parent = this;
                    Strategies.TradeContext = this;

                    Strategies.Init();
                    // Strategies.Init(@"Init\736Z_M5_210921.xml","Strategies");

                    // TradeStorage

                    foreach (var s in Strategies.StrategyCollection)
                    {
                        Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, s.Name, "Create", s.FullInfo, "");
                        try
                        {
                            TradeStorage.Register(s);
                            TradeStorage.Register(s.Position);
                            TradeStorage.RegisterTotal(s.PositionTotal);
                        }
                        catch (Exception e)
                        {
                            SendExceptionMessage3(FullName, "TradeContext", "Strategy.Register:" + s.StrategyTickerString,
                                "", e);
                             throw;
                        }
                    }

                    Thread.Sleep(1000);

                    Strategies.StrategyTradeEntityChangedEvent += EventHub.FireEvent;
                    Strategies.ExceptionEvent += EventHub.FireEvent;

                    // Portfolios = BuildPortfolios();
                    // 15.09.24
                    // Portfolio with TimeInts
                    Portfolios = BuildPortfolios3();
                    Portfolios.ChangedEvent += EventHub.FireEvent;

                    // DeQueue Process SendTransactions from Transactions Queue
                    // 2018.05.14
                    //StrategyProcess.RegisterProcess("Orders DeQueue Process", "OrdersDeQueueProcess", (int) (1*500), 0,
                    //    null, TradeTerminals.DeQueueProcess, null);

                    // Orders and Trades RESOLVE PROCESS
                    // 2018.05.16 shut down my friends
                    //StrategyProcess.RegisterProcess("Trade Resolve Process", "TradeResolveProcess", (int) (1*1000), 0,
                    //    null, TradeTerminals.TradeResolveProcess, null);

                    //UIProcess.RegisterProcess("Order Resolve Process", "OrderResolveProcess", (int) (1*1000), 0, null,
                    //    TradeTerminals.OrderResolveProcess, null);

                    TradeTerminals.ChangedEvent += EventHub.FireEvent;
                    TradeTerminals.ExceptionEvent += EventHub.FireEvent;
                    TradeTerminals.WhoAreYou();

                    SimulateTerminal = TradeTerminals.GetSimulateTerminal();

                    if (SimulateTerminal != null)
                        Tickers.NewTickEvent += SimulateTerminal.ExecuteTick;
                    
                    // 2018.05.11 
                    //UIProcess.RegisterProcess("Event Hub Dequeue Process", "EventHub.DeQueue()", (int) (1*1000), 0, null,
                    //    EventHub.DeQueueProcess, null);


                    UIProcess.RegisterProcess("TradeStorages DeQueue Process", "TradeStoragesDeQueueProcess",
                        (int) (1*1000), 0, null, TradeStorage.DeQueueProcess, null);
                    //UIProcess.RegisterProcess(Portfolios.Name, Portfolios.Code, (int) (15*1000), 0, null,
                    // 2019.10.01
                    UIProcess.RegisterProcess(Portfolios.Name, Portfolios.Code, (int)(1 * 1000), 0, null,
                        Portfolios.Refresh, null);

                    Thread.Sleep(1000);

                   Tickers.LoadBarsFromArch();
                }
                catch (Exception e)
                {
                    SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.StrategyInit()", ToString(), e);
                    throw new Exception(e.Message);
                }

                // StrategyProcess.NewTickEvent += Tickers.UpdateTimeSeries;
                StrategyProcess.NewTickEvent += TimePlans.NewTickEventHandler;

                //Tickers.NewTickEvent += SimulateOrders.ExecuteTick;

                DiagnosticProcess.RegisterProcess("Check Connections Process", 15000, 5000, null,
                    TradeTerminals.CheckConnection, null);

                UIProcess.RegisterProcess("Update Last Price", "UpdateLastPriceProcess",
                    5000, 0, null, Strategies.UpdateLastPrice, null);

                // 2018.05.11 
                // UIProcess.RegisterProcess("EventLog Dequeue Process", "EventLog.DeQueue",
                //    1000, 0, null, ((IEventLogs) EventLog).DeQueueProcess, null);

                //Dde2.RegisterDefaultCallBack(Tickers.PutDdeQuote3);
                // 2018.05.14
                // Dde2?.RegisterDefaultCallBack(Tickers.PushDdeQuoteStr1);
              

               if (Dde.Mode == GS.Interfaces.Dde.ChangesSendMode.Table)
                    Dde.TableChangesSendAction = Tickers.PushDdeQuoteListStr;
               else
                    Dde.LineChangesSendAction = Tickers.PushDdeQuoteStr1;
                
                WindowsInit();
                ShowWindows();

                //assLoaded = AppDomain.CurrentDomain.GetAssemblies();

                Strategies.WhoAreYou();
            }
            catch (Exception ex)
            {
                var em = ex.Message;
            }
        }
        public override void OpenChart()
        {
            UIProcess.RegisterProcess("Chart Window Refresh", "ChartWindowRefreshProcess",
                                                    1000, 0, null, ChartWindow.TickRefreshEventHandler, null);
            ChartWindow.Show();
        }
        public override void Stop()
        {
            try
            {
                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Begin Stop Process", "", "");

                //foreach (var quik in TradeTerminals.TradeTerminalCollection.Values.OfType<IQuikTradeTerminal>())
                //{
                //    //quik.TransactionEvent -= EventHub.FireEvent;
                //    //quik.OrderEvent -= EventHub.FireEvent;
                //    //quik.TradeEvent -= EventHub.FireEvent;

                //    //quik.TradeEntityChangedEvent -= EventHub.FireEvent;
                //}
                TradeTerminals.ChangedEvent -= EventHub.FireEvent;
                
                if(Strategies != null)
                    Strategies.StrategyTradeEntityChangedEvent -= EventHub.FireEvent;
                EventLog.EventLogChangedEvent -= EventHub.FireEvent;

                if (StrategyProcess != null)
                {
                    StrategyProcess.NewTickEvent -= Tickers.UpdateTimeSeries;
                    //     StrategyProcess.NewTickEvent -= TimePlans.NewTickEventHandler;

                    //    StrategyProcess.NewTickEvent -= OrderPlaneWindow.UpdateQuote;

                    StrategyProcess.NewTickEvent -= TimePlans.NewTickEventHandler;

                    StrategyProcess.Stop();
                    StraCloseAutoEvent.WaitOne();
                }
               // 15.11.11 
              //  Rand.NewTickEvent -= Orders.ExecuteTick;
              //  Rand.NewTickStrEvent -= Tickers.PutDdeQuote3;

                Tickers.NewTickEvent -= Orders.ExecuteTick;
                Tickers.NewTickEvent -= SimulateOrders.ExecuteTick;

                //   Dde.Stop();
                Dde?.Stop();

                TimePlans?.Stop();
                // 15.11.11
               // Rand.Stop();

                TradeTerminals?.DisConnect();

                CloseWindows();

                //StrategyProcess.Stop();
                //StraCloseAutoEvent.WaitOne();
                if (DiagnosticProcess != null)
                {
                    DiagnosticProcess.Stop();
                    DiaCloseAutoEvent.WaitOne();
                }
                if (UIProcess != null)
                {
                    UIProcess.Stop();
                    UiCloseAutoEvent.WaitOne();
                }
                // WaitHandle.WaitAll(WaitHandles);

                Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Finish Stop Process", "", "");
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.Stop()", ToString(), e);
                throw;
            }
        }
        public override void Close()
        {
            try
            {
                //CloseWindows();

                // Dde.Close();
                // Dde2?.Close();

                StrategyProcess.Close();
                DiagnosticProcess.Close();
                UIProcess.Close();

                // 19.08.26 Finish Startegies
                //try
                //{
                //    Strategies.Close();
                //}
                //catch (Exception e)
                //{
                //    SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.Strategies.Close()", ToString(), e);
                //    throw;
                //}

                // CloseWindows();

                // EventLogWindow.Close();

                TradeTerminals.Stop();
                Tickers.Stop();
                EventHub.Stop();
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.Close()", ToString(), e);
                throw;
            }
        }
        public override void Start()
        {
            Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Begin Start Process", "", "");

            UIProcess.Start();
            StrategyProcess.Start();
            DiagnosticProcess.Start();

            //  TimePlans.Start();

            //  Dde.Start();
            Dde?.Start();

            TradeTerminals.Start();

            // Rand.Start();

            Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Finish Start Process", "", "");

            ((IEventLogs)EventLog).SetMode(EvlModeEnum.Nominal);

        }
        public override void StartRand()
        {
            // 15.11.11
           // Rand.Start();
        }

        protected override void WindowsInit()
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
            //PositionsWindow2.Init(this, EventLog);
            PositionsWindow3.Init(this, EventLog);
            //PositionTotalsWindow2.Init(this, EventLog);
            PositionTotalsWindow3.Init(this, EventLog);
            PositionTotalsWindow4.Init(this, EventLog);

            PortfolioWindow.Init(this, EventLog);

            TransactionsWindow.Init(EventLog, this);

            ChartWindow = new ChartWindow();
            ChartWindow.Init(this);

            //     QuotesWindow.Init(EventLog, Quotes);

            #endregion
        }
        public override void ShowWindows()
        {
            //EventLogWindow.Show();

            //TradesWindow.Show();
            TradesWindow2?.Show();

            //OrdersActiveWindow.Show();
            OrdersActiveWindow2?.Show();

            //OrdersFilledWindow.Show();
            OrdersFilledWindow2?.Show();

            //PositionsOpenedWindow.Show();
            //PositionsClosedWindow.Show();
            //PositionTotalsWindow.Show();
            //PositionsWindow.Show();

            DealsWindow?.Show();

            PositionsWindow3?.Show();
            PositionTotalsWindow3?.Show();
            PositionTotalsWindow4?.Show();

            PortfolioWindow?.Show();

            //ExceptionsWindow.Show();

            TransactionsWindow?.Show();

            //OrderPlaneWindow.Show();

            // QuotesWindow.Show();

            // ChartWindow.Show();
        }
        public override void CloseWindows()
        {
            //EventLogWindow.Close();

            ChartWindow?.Close();

            TradesWindow2?.Close();
            //TradesWindow.Close();

            //OrdersActiveWindow.Close();
            OrdersActiveWindow2?.Close();

            //OrdersFilledWindow.Close();
            OrdersFilledWindow2.Close();

            //PositionsOpenedWindow.Close();
            //PositionsClosedWindow.Close();
            //PositionTotalsWindow.Close();
            //PositionsWindow.Close();

            DealsWindow?.Close();

            PositionsWindow3?.Close();
            PositionTotalsWindow3?.Close();
            PositionTotalsWindow4?.Close();

            PortfolioWindow?.Close();

            TransactionsWindow?.Close();

            // OrderPlaneWindow.Close();

            // QuotesWindow.Show();
        }
        public override void ShowEventLogWindow()
        {
            EventLogWindow.Show();
        }
    }
}

