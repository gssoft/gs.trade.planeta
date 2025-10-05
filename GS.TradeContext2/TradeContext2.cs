using System;
using System.Collections.Generic;
using GS.Interfaces;
using GS.Process;
using GS.Time.TimePlan;
using GS.Trade.Data;
using GS.Trade.Interfaces;
using GS.Trade.Strategies;
using GS.Trade.Strategies.Managers;
using GS.Trade.Strategies.Portfolio;
using GS.Trade.Trades;
using GS.Trade.Trades.Time;
using GS.Trade.Windows;
using GS.Trade.Windows.Charts;
using GS.Trade.Windows.OrderPlane;
using GS.Time.TimePlan;
using TimePlanEventArgs = GS.Time.TimePlan.TimePlanEventArgs;


namespace GS.Trade.TradeContext
{
    public class TradeContext2 : ITradeContext
    {
        private IEventLog _eventLog;
        public IEventLog EventLog
        {
            get { return _eventLog; }
            set { _eventLog = value as EventLog.EventLog; }
        }

        public Trades.Trades Trades { get; set; }
        public Orders Orders { get; set; }
        public Orders SimulateOrders { get { return Orders; } }
        public Positions Positions { get; set; }
        public Strategies.Strategies Strategies { get; set; }
        public Portfolios Portfolios { get; set; }
        public EntryManagers EntryManagers { get; set; }

        public IAccounts Accounts { get; private set; }

        public Dde.Dde Dde;
        public Quotes.Quotes Quotes;
        public Tickers Tickers { get; set; }
        public Tickers TickersAll;

        public IEnumerable<Ticker> TickerCollection { get { return Tickers.TickerCollection; } }
        public IEnumerable<Strategy> StrategyCollection { get { return Strategies.StrategyCollection; } }

     //   public GS.Trade.Trades.Time.TimePlans TimePlans { get; set; }
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

        public QuotesWindow QuotesWindow;
        public OrderPlaneWindow OrderPlaneWindow;

        public ChartWindow ChartWindow { get; set; }

        public TradeTerminals.TradeTerminals TradeTerminals;

        public TradeContext2()
        {
            // if(_eventLog == null)
            //     _eventLog = new ConsoleEventLog();

            Trades = new Trades.Trades();
            Orders = new Orders();
            Positions = new Positions();
            Portfolios = new Portfolios();
            EntryManagers = new EntryManagers();

            Accounts = new Accounts.Accounts();

            Tickers = new Tickers();
            TickersAll = new Tickers();

          //  TimePlans = new TimePlans();
            TimePlans = new TimePlans();

            Rand = new BarRandom(1000, 100);

            //OrderFiller = new OrderFiller(Orders, Trades);

            Dde = new Dde.Dde("QUIKInfo");

            Quotes = new Quotes.Quotes();

            TradeTerminals = new TradeTerminals.TradeTerminals();

            //    QuotesWindow = new QuotesWindow();

        }
        public void Init()
        {
            EventLogWindow = new EventLogWindow();
            EventLogWindow.Init(_eventLog as EventLog.EventLog);
            ShowEventLogWindow();

            StrategyProcess = new ProcessManager2("Strategy_Process", 500, 0, EventLog);
            UIProcess = new ProcessManager2("UI_Process", 1000, 0, EventLog);
            DiagnosticProcess = new ProcessManager2("Diagnostic_Process", 15000, 0, EventLog);

            FastProcess = new ProcessManager2("Fast_Process", 100, 0, EventLog);

            Positions.Init(EventLog);
            TimePlans.Init(EventLog);

            Orders.Init(EventLog, Positions, Trades);
            Trades.Init(EventLog, Positions, Orders);

            Accounts.Load();

            TickersAll.Init(EventLog);
            TickersAll.Load();
            Tickers.Init(EventLog);

            TradeTerminals.Evl = EventLog;

            Orders.BackOrderExecMode = BackTestOrderExecutionMode.Optimistic;

            Dde.Init(EventLog);
            //       Quotes.Init(EventLog);

            Strategies = new Strategies.Strategies("MyStrategies", "xStyle", this);
            Strategies.Init();

            EntryManagers.Init();

            // Tickers.LoadBarsFromArch();


            #region Windows Init

            //EventLogWindow = new EventLogWindow();
            TradesWindow = new TradesWindow();

            OrdersActiveWindow = new OrdersActiveWindow();
            OrdersFilledWindow = new OrdersFilledWindow();

            PositionsOpenedWindow = new PositionsOpenedWindow();
            PositionsClosedWindow = new PositionsClosedWindow();
            PositionTotalsWindow = new PositionTotalsWindow();

            OrderPlaneWindow = new OrderPlaneWindow();
            OrderPlaneWindow.Init(this);

            TradesWindow.Init(EventLog, Trades);

            OrdersActiveWindow.Init(this, EventLog, Orders);
            OrdersFilledWindow.Init(this, EventLog, Orders);

            PositionsOpenedWindow.Init(this, EventLog, Positions);
            PositionsClosedWindow.Init(EventLog, Positions);
            PositionTotalsWindow.Init(this, EventLog, Positions.PositionTotals);

            ChartWindow = new ChartWindow();
            ChartWindow.Init(this);

            //     QuotesWindow.Init(EventLog, Quotes);

            #endregion
        }
        public void Open()
        {
            StrategyProcess.NewTickEvent += Tickers.UpdateTimeSeries;
        //    StrategyProcess.NewTickEvent += TimePlans.NewTickEventHandler;
            StrategyProcess.NewTickEvent += OrderPlaneWindow.UpdateQuote;

            Tickers.NewTickEvent += Orders.ExecuteTick;
            Rand.NewTickEvent += Orders.ExecuteTick;
            Rand.NewTickStrEvent += Tickers.PutDdeQuote3;

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
        //    StrategyProcess.NewTickEvent -= TimePlans.NewTickEventHandler;
            StrategyProcess.NewTickEvent -= OrderPlaneWindow.UpdateQuote;

            Rand.NewTickEvent -= Orders.ExecuteTick;
            Rand.NewTickStrEvent -= Tickers.PutDdeQuote3;

            Tickers.NewTickEvent -= Orders.ExecuteTick;

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

     //       TimePlans.Start();

            //Dde.Start();

            // Rand.Start();

            Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "Finish Start Process", "", "");
        }
        public void StartRand()
        {
            Rand.Start();
        }

        public void Stop()
        {
            Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "Begin Stop Process", "", "");

            Dde.Stop();

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

            OrderPlaneWindow.Close();

            // QuotesWindow.Show();
        }

        public void ShowEventLogWindow()
        {
            EventLogWindow.Show();
        }

        public ITicker RegisterTicker(string code)
        {
            var t = TickersAll.GetTicker(code);
            if (t != null)
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
            var tt = TradeTerminals.RegisterTradeTerminal(type, key, Orders, Trades, EventLog);
            if (tt == null)
                throw new NullReferenceException(string.Format("TradeTerminal {0} {1} is Not Found", type, key));

            //  Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TradeTerminal", "Register", type + " " + key, "");
            return tt;
        }
        public Position RegisterPosition(string account, string strategy, ITicker ticker)
        {
            return Positions.Register2(account, strategy, ticker);
        }
        public IPortfolio RegisterPortfolio(string code, string name)
        {
            var p = new Portfolio { Code = code, Name = name };
            IPortfolio ip;
            if ((ip=Portfolios.AddNew(p)) != null)
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
        public IAccount GetAccount(string key)
        {
            return Accounts.GetAccount(key);
        }
        /*
        public TimePlan RegisterTimePlanStatusChangedEventHandler(string timePlanKey,
                                                                  TimePlan.TimeStatusIsChangedEventHandler action)
        {
            return TimePlans.RegisterTimeStatusChangedEventHandler(timePlanKey, action);
        }
         */
        public TimePlan RegisterTimePlanEventHandler(string timePlanKey,
                                                                    EventHandler<TimePlanEventArgs> action)
        {
            return TimePlans.RegisterTimePlanEventHandler(timePlanKey, action);
        }
        public void Evlm(EvlResult result, EvlSubject subject,
                         string source, string operation, string description, string obj)
        {
            if (_eventLog != null)
                _eventLog.AddItem(result, subject, source, operation, description, obj);
        }

        public void Evlm(EvlResult result, EvlSubject subject, string source, string entity, string operation, string description,
            string obj)
        {
            if (_eventLog != null)
                _eventLog.AddItem(result, subject, source, entity, operation, description, obj);
        }
    }
}