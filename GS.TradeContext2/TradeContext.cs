using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Interfaces;
using GS.Trade.Data.Bars;
using GS.Trade.Interfaces;
using GS.Process;
using GS.Trade.Strategies.Managers;
using GS.Trade.Strategies.Portfolio;
using GS.Trade.Trades.Time;
using GS.Trade.TradeTerminals;
using GS.Trade.TradeTerminals.Quik;
using GS.Trade.TradeTerminals.Simulate;
using GS.Trade.Windows;
using GS.Trade.Data;
using GS.Trade.Strategies;
using GS.Trade.Windows.Charts;
using GS.Trade.Windows.OrderPlane;
using GS.Time.TimePlan;
using GS.EventLog;
using WebEventLogClient;
using TimePlanEventArgs = GS.Time.TimePlan.TimePlanEventArgs;

namespace GS.Trade.TradeContext
{
    using Trades;
    using Dde;
    using Quotes;
    using EventLog;
    using Strategies;
    using Accounts;

    public partial class TradeContext : ITradeContext
    {
        private  IEventLog _eventLog;
        public IEventLog EventLog
        {
            get { return _eventLog; }
            set { _eventLog =  value as EventLog; }
        }

        public WebEventLog WebEventLogClient;

        public TradeContextInit TradeContextInit { get; set; }
    //    public TradeContextInit2 TradeContextInit2 { get; set; }

        public Trades Trades { get; set; }
        public Orders Orders { get; set; }
        public Positions Positions { get; set; }
        public Strategies Strategies { get; set; }
        public Portfolios Portfolios { get; set; }
        public EntryManagers EntryManagers { get; set; }

        public Orders SimulateOrders { get; set; }

        public IAccounts Accounts { get; private set; }

        public Dde Dde;
        public Quotes Quotes;
        public Tickers Tickers { get; set;}
        public Tickers TickersAll;

        public IEnumerable<Ticker> TickerCollection { get { return Tickers.TickerCollection; } }
        public IEnumerable<Strategy> StrategyCollection { get { return Strategies.StrategyCollection; } }

    //    public GS.Trade.Trades.Time.TimePlans TimePlans { get; set; }
        public TimePlans TimePlans { get; set; }

        public BarRandom Rand;

        public ITickers GetTickers
        {
            get { return Tickers; }
        }
        public IStrategies GetStrategies
        {
            get { return Strategies; }
        }
        
        //public OrderFiller OrderFiller;

        public ProcessManager2 StrategyProcess { get; set; }
        public ProcessManager2 UIProcess { get; set; }
        public ProcessManager2 DiagnosticProcess { get; set; }

        public ProcessManager2 FastProcess { get; set; }

        public EventLogWindow EventLogWindow;

        public TradesWindow TradesWindow;
        public OrdersActiveWindow OrdersActiveWindow;
        public OrdersFilledWindow OrdersFilledWindow;

        public PositionsOpenedWindow PositionsOpenedWindow;
        public PositionsClosedWindow PositionsClosedWindow;
        public PositionTotalsWindow PositionTotalsWindow;
        public PositionsWindow PositionsWindow;

        public QuotesWindow QuotesWindow;
        public OrderPlaneWindow OrderPlaneWindow;

        public ChartWindow ChartWindow { get; set; }

        public TradeTerminals.TradeTerminals TradeTerminals;

        public  TradeContext()
        {
           // if(_eventLog == null)
           //     _eventLog = new ConsoleEventLog();

            WebEventLogClient = new WebEventLog();

            Trades = new Trades();
            Orders = new Orders();
            Positions = new Positions();
            Portfolios = new Portfolios();
            EntryManagers = new EntryManagers();

            SimulateOrders = new Orders();

            Accounts = new Accounts();

            Tickers = new Tickers();
            TickersAll = new Tickers();

            TimePlans = new TimePlans();
            
            Rand = new BarRandom(1000, 100);

            //OrderFiller = new OrderFiller(Orders, Trades);

       //     TradeContextInit2 = new TradeContextInit2();
       //     SerializeInitData();

            if(!DeserializeInitData())
                throw new NullReferenceException("TradeContextData XMLDesrialization Failure");

            Dde = new Dde(TradeContextInit.DdeInit.ServerName);

            if (TradeContextInit.OrderInit.ClassCodeToRemoveLogin.HasValue() &&
                TradeContextInit.OrderInit.LoginToRemove.HasValue())
            {
                Orders.ClassCodeToRemoveLogin = TradeContextInit.OrderInit.ClassCodeToRemoveLogin;
                Orders.LoginToRemove = TradeContextInit.OrderInit.LoginToRemove;

                Trades.ClassCodeToRemoveLogin = TradeContextInit.OrderInit.ClassCodeToRemoveLogin;
                Trades.LoginToRemove = TradeContextInit.OrderInit.LoginToRemove;
            }
            //Dde = new Dde("QUIKInfo");

             Quotes = new Quotes();

             TradeTerminals = new TradeTerminals.TradeTerminals();

            

        //    QuotesWindow = new QuotesWindow();
            
        }
        public void Init()
        {
            EventLogWindow = new EventLogWindow();
            EventLogWindow.Init(_eventLog as EventLog);
            ShowEventLogWindow();

            StrategyProcess = new ProcessManager2("Strategy_Process", 500, 0, EventLog);
            UIProcess = new ProcessManager2("UI_Process", 1000, 0, EventLog);
            DiagnosticProcess = new ProcessManager2("Diagnostic_Process", 15000, 0, EventLog);

           // FastProcess = new ProcessManager2("Fast_Process", 100, 0, EventLog);

            Positions.Init(EventLog);
            TimePlans.Init(EventLog);

            Orders.Init(EventLog, Positions, Trades);
            Trades.Init(EventLog, Positions, Orders);

            SimulateOrders.Init(EventLog, Positions, Trades);

            Accounts.Load();

            TickersAll.Init(EventLog);
            TickersAll.Load();
            Tickers.Init(EventLog);

            TradeTerminals.Evl = EventLog;

            Orders.BackOrderExecMode = BackTestOrderExecutionMode.Optimistic;
            SimulateOrders.BackOrderExecMode = BackTestOrderExecutionMode.Optimistic;

            Dde.Init(EventLog);
     //       Quotes.Init(EventLog);

            Strategies = new Strategies("MyStrategies", "xStyle", this);
            Strategies.Init();

            foreach (var t in TradeTerminals.TradeTerminalCollection.Values)
            {
                StrategyProcess.RegisterProcess(t.Code, t.Name, (int)(1 * 1000), 0, null, t.SendOrdersFromQueue, null);
            }

            EntryManagers.Init();

            Tickers.LoadBarsFromArch();
            
            
 #region Windows Init

            //EventLogWindow = new EventLogWindow();
            TradesWindow = new TradesWindow();

            OrdersActiveWindow = new OrdersActiveWindow();
            OrdersFilledWindow = new OrdersFilledWindow();

            PositionsOpenedWindow = new PositionsOpenedWindow();
            PositionsClosedWindow = new PositionsClosedWindow();
            PositionTotalsWindow = new PositionTotalsWindow();
            PositionsWindow = new PositionsWindow();

            OrderPlaneWindow = new OrderPlaneWindow();
            OrderPlaneWindow.Init(this);
        
            TradesWindow.Init(EventLog,Trades);

            OrdersActiveWindow.Init(this, EventLog, Orders);
            OrdersFilledWindow.Init(this, EventLog, Orders);

            PositionsOpenedWindow.Init(this, EventLog, Positions);
            PositionsClosedWindow.Init(EventLog, Positions);
            PositionTotalsWindow.Init(this, EventLog, Positions.PositionTotals);
            PositionsWindow.Init(this, EventLog, Positions);

            ChartWindow = new ChartWindow();
            ChartWindow.Init(this);

            //     QuotesWindow.Init(EventLog, Quotes);

            #endregion
        }
        public void Open()
        {
            StrategyProcess.NewTickEvent += Tickers.UpdateTimeSeries;
      //      StrategyProcess.NewTickEvent += TimePlans.NewTickEventHandler;
            StrategyProcess.NewTickEvent += OrderPlaneWindow.UpdateQuote;
            StrategyProcess.NewTickEvent += TimePlans.NewTickEventHandler;

            Tickers.NewTickEvent += SimulateOrders.ExecuteTick;
            
          //  Rand.NewTickEvent += Orders.ExecuteTick;
          //  Rand.NewTickStrEvent += Tickers.PutDdeQuote3;

            DiagnosticProcess.RegisterProcess("Check Connections Process", 15000, 5000, null, TradeTerminals.CheckConnection, null);

            var topic = Dde.RegisterTopic("Forts", "Main Quotes", Tickers.PutDdeQuote3);
            topic.RegisterChannel(1, "Rtsi", Tickers.PutDdeQuote3);

            ShowWindows();
        }
        public void OpenChart()
        {
            UIProcess.RegisterProcess("Chart Window Refresh", "ChartWindowRefreshProcess",
                                                    1000, 0, null, ChartWindow.TickRefreshEventHandler, null);
            ChartWindow.Show();
        }

        public void Close()
        {
            Dde.Close();

            StrategyProcess.NewTickEvent -= Tickers.UpdateTimeSeries;
       //     StrategyProcess.NewTickEvent -= TimePlans.NewTickEventHandler;
            StrategyProcess.NewTickEvent -= OrderPlaneWindow.UpdateQuote;
            StrategyProcess.NewTickEvent -= TimePlans.NewTickEventHandler;

            Rand.NewTickEvent -= Orders.ExecuteTick;
            Rand.NewTickStrEvent -= Tickers.PutDdeQuote3;

            Tickers.NewTickEvent -= Orders.ExecuteTick;
            Tickers.NewTickEvent -= SimulateOrders.ExecuteTick;

            StrategyProcess.Close();
            DiagnosticProcess.Close();
            UIProcess.Close();

            Strategies.Close();

            CloseWindows();

           // EventLogWindow.Close();
        }
        public void Start()
        {
            Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "Begin Start Process", "", "");

            UIProcess.Start();
            StrategyProcess.Start();
            DiagnosticProcess.Start();

          //  TimePlans.Start();

            Dde.Start();

            // Rand.Start();
            
            Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "Finish Start Process", "", "");
        }
        public void StartRand()
        {
            Rand.Start();
        }

        public void Stop()
        {
            Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "Begin Stop Process","","");
            
            Dde.Stop();
            TimePlans.Stop();

            Rand.Stop();

            TradeTerminals.DisConnect();

            StrategyProcess.Stop();
            DiagnosticProcess.Stop();
            UIProcess.Stop();

            Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "Finish Stop Process", "", "");
        }
        public void ShowWindows()
        {
            //EventLogWindow.Show();

            TradesWindow.Show();

            OrdersActiveWindow.Show();
            OrdersFilledWindow.Show();

            PositionsOpenedWindow.Show();
            PositionsClosedWindow.Show();
            PositionTotalsWindow.Show();
            PositionsWindow.Show();

            OrderPlaneWindow.Show();

           // QuotesWindow.Show();

            // ChartWindow.Show();
        }
        public void CloseWindows()
        {
            //EventLogWindow.Close();

            ChartWindow.Close();

            TradesWindow.Close();

            OrdersActiveWindow.Close();
            OrdersFilledWindow.Close();

            PositionsOpenedWindow.Close();
            PositionsClosedWindow.Close();
            PositionTotalsWindow.Close();
            PositionsWindow.Close();

           OrderPlaneWindow.Close();

            // QuotesWindow.Show();
        }

        public void ShowEventLogWindow()
        {
            EventLogWindow.Show();
        }

        public ITicker RegisterTicker( string code )
        {
            var t = TickersAll.GetTicker(code);
            if( t != null)
            {
            //t.RegisterAsyncSeries(new Bars001("1_Min","PrimaryBars", t, 60, 0));
            Tickers.AddTicker(t);
            return t;
            }
            Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, code, "RegisterTicker", "Ticker: " + code + "Is NOT Found", "");
            return null;
        }
        
        public ITimeSeries RegisterTimeSeries(ITimeSeries ts)
        {
            var t = ts as TimeSeries;
            return t.Ticker.RegisterTimeSeries(t);
        }
        public TimeSeries GetTimeSeriesOrNull(string key)
        {
            foreach (Ticker t in Tickers.TickerCollection)
            {
                TimeSeries ts;
                if ((ts = t.GetTimeSeries(key)) != null) return ts;
            }
            return null;
        }

        public ITradeTerminal RegisterTradeTerminal(string type, string key)
        {
            var tt = TradeTerminals.RegisterTradeTerminal(type, key, Orders, SimulateOrders, Trades, EventLog);
            if( tt == null)
                throw new NullReferenceException(string.Format("TradeTerminal {0} {1} is Not Found", type, key) );

          //  Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TradeTerminal", "Register", type + " " + key, "");
            return tt;
        }
        public Position RegisterPosition( string account, string strategy, ITicker ticker)
        {
            return Positions.Register2(account, strategy, ticker);
        }

        public IAccount GetAccount(string key)
        {
            return Accounts.GetAccount(key);
        }
        /*
        public TimePlan RegisterTimePlanStatusChangedEventHandler(string timePlanKey,
                                                                    TimePlan.TimeStatusIsChangedEventHandler action)
        {
            return TimePlans.RegisterTimeStatusChangedEventHandler(timePlanKey, action );
        }
         */ 
        public TimePlan RegisterTimePlanEventHandler(string timePlanKey,
                                                                    EventHandler<TimePlanEventArgs> action)
        {
            return TimePlans.RegisterTimePlanEventHandler(timePlanKey, action);
        }

        public IPortfolio RegisterPortfolio(string code, string name)
        {
            var p = new Portfolio { Code = code, Name = name };
            IPortfolio ip;
            if ((ip = Portfolios.AddNew(p)) != null)
                return ip;
            throw new NullReferenceException("Portfolio Register Failure:" + p.ToString());
        }
        public IEntryManager RegisterEntryManager(string code, string name)
        {
            var p = new EntryManager { Code = code, Name = name };
            IEntryManager ip;
            if ((ip = EntryManagers.AddNew(p)) != null)
                return ip;
            throw new NullReferenceException("EntryManager Register Failure:" + p.ToString());
        }

        public void Evlm(EvlResult result, EvlSubject subject,
                                    string source, string operation, string description, string obj)
        {
            if (_eventLog != null)
                _eventLog.AddItem(result, subject, source, operation, description, obj);
            /*
            try
            {
                if (WebEventLogClient != null)
                    WebEventLogClient.AddItem(result, subject, source, "entity", operation, description, obj)
                    ;
            }
            catch (Exception e)
            {
                throw new Exception( e.Message);
            }
             */ 
        }
        public void Evlm(EvlResult result, EvlSubject subject,
                                    string source, string entity, string operation, string description, string obj)
        {
            if (_eventLog != null)
                _eventLog.AddItem(result, subject, source, entity, operation, description, obj);
            /*
            try
            {
                if (WebEventLogClient != null)
                    WebEventLogClient.AddItem(result, subject, source, entity, operation, description, obj)
                    ;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
             */ 
        }
        public bool SerializeInitData()
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

                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TxInitData", "Serialization",
                String.Format("FileName={0}", xmlfname), "");

                return true;
            }
            catch (Exception e)
            {
                Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "TxInitData", "Serialization",
                String.Format("FileName={0}", xmlfname), e.ToString());

                if (tr != null) tr.Close();
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
                    (s1, s2) => Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                                    "TradeContextInit", s1, String.Format("FileName={0}", xmlfname), s2)
                                    );
                
                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TradeContextInit", "DeSerialization",
                String.Format("FileName={0}", xmlfname), "");

            }
            catch (Exception e)
            {
                Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "TradeContextInit", "DeSerialization",
                String.Format("FileName={0}", xmlfname), e.ToString());

                throw new SerializationException("TradeContextInit.Deserialization Failure " + xmlfname);
            }
            return true;
        }
    }
}
