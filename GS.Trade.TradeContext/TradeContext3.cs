using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using GS.Process;
using GS.Serialization;
using GS.Time.TimePlan;
//using GS.Trade.Data;
using GS.Trade.Interfaces;
//using GS.Trade.Strategies;

using GS.Trade.Trades;

using GS.Trade.Trades.Orders3;
using GS.Trade.TradeTerminals64;
using GS.Trade.TradeTerminals64.Quik;
using GS.Trade.Windows;
using GS.Trade.Windows.Charts;

namespace GS.Trade.TradeContext
{
 //   public class TradeContext3 : ITradeContext
 //   {
 //       private  IEventLog _eventLog;
 //       public IEventLog EventLog
 //       {
 //           get { return _eventLog; }
 //           set { _eventLog =  value as IEventLog; }
 //       }
 //       public TradeContextInit TradeContextInit { get; set; }
 //   //    public TradeContextInit2 TradeContextInit2 { get; set; }

 //       public Trades.Trades Trades { get; set; }
 //       public Orders Orders { get; set; }

 //       public Positions Positions { get; set; }
 //       public Strategies.Strategies Strategies { get; set; }
 //       public Portfolios Portfolios { get; set; }
 //       public EntryManagers EntryManagers { get; set; }

 //       public Orders3 ActiveOrders  { get; set; }

 //       public readonly QuikTransactions Transactions = new QuikTransactions();

 //       public  IEventHub EventHub { get; set; }

 //       public Orders SimulateOrders { get; set; }

 //       public IAccounts Accounts { get; private set; }

 //       public Dde.Dde Dde;
 //       public Quotes.Quotes Quotes;
 //       public Tickers Tickers { get; set;}
 //       public Tickers TickersAll;

 //       public IEnumerable<Ticker> TickerCollection { get { return Tickers.TickerCollection; } }
 //       public IEnumerable<Strategy> StrategyCollection { get { return Strategies.StrategyCollection; } }

 //   //    public GS.Trade.Trades.Time.TimePlans TimePlans { get; set; }
 //       public TimePlans TimePlans { get; set; }

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

 //       public ProcessManager2 StrategyProcess { get; set; }
 //       public ProcessManager2 UIProcess { get; set; }
 //       public ProcessManager2 DiagnosticProcess { get; set; }

 //       public ProcessManager2 FastProcess { get; set; }

 //       public EventLogWindow EventLogWindow;

 //       public TradesWindow TradesWindow;
 //       public OrdersActiveWindow OrdersActiveWindow;
 //       public OrdersActiveWindow2 OrdersActiveWindow2;

 //       public OrdersFilledWindow OrdersFilledWindow;

 //       public PositionsOpenedWindow PositionsOpenedWindow;
 //       public PositionsClosedWindow PositionsClosedWindow;
 //       public PositionTotalsWindow PositionTotalsWindow;
 //       public PositionsWindow PositionsWindow;

 //       public TransactionsWindow TransactionsWindow;
 //       // public QuotesWindow QuotesWindow;
 //      // public OrderPlaneWindow OrderPlaneWindow;

 //       public ChartWindow ChartWindow { get; set; }

 //       public TradeTerminals.TradeTerminals TradeTerminals;

 //       public  TradeContext3()
 //       {
 //          // if(_eventLog == null)
 //          //     _eventLog = new ConsoleEventLog();

 //     //      WebEventLogClient = new WebEventLog();

 //           Trades = new Trades.Trades();
 //           Orders = new Orders();

 //           ActiveOrders = new Orders3();

 //           Positions = new Positions();
 //           Portfolios = new Portfolios();
 //           EntryManagers = new EntryManagers();

 //           EventHub = new EventHub();

 //           SimulateOrders = new Orders();

 //           Accounts = new Accounts.Accounts();

 //           Tickers = new Tickers();
 //           TickersAll = new Tickers();

 //           TimePlans = new TimePlans();
            
 //           Rand = new BarRandom(1000, 100);

 //           //OrderFiller = new OrderFiller(Orders, Trades);

 //      //     TradeContextInit2 = new TradeContextInit2();
 //      //     SerializeInitData();

 //           if(!DeserializeInitData())
 //               throw new NullReferenceException("TradeContextData XMLDesrialization Failure");

 //           Dde = new Dde.Dde(TradeContextInit.DdeInit.ServerName);

 //           if (TradeContextInit.OrderInit.ClassCodeToRemoveLogin.HasValue() &&
 //               TradeContextInit.OrderInit.LoginToRemove.HasValue())
 //           {
 //               Orders.ClassCodeToRemoveLogin = TradeContextInit.OrderInit.ClassCodeToRemoveLogin;
 //               Orders.LoginToRemove = TradeContextInit.OrderInit.LoginToRemove;

 //               Trades.ClassCodeToRemoveLogin = TradeContextInit.OrderInit.ClassCodeToRemoveLogin;
 //               Trades.LoginToRemove = TradeContextInit.OrderInit.LoginToRemove;
 //           }
 //           //Dde = new Dde("QUIKInfo");

 //            Quotes = new Quotes.Quotes();

 //            TradeTerminals = new TradeTerminals.TradeTerminals();

            

 //       //    QuotesWindow = new QuotesWindow();
            
 //       }
 //       public void Init()
 //       {

 //           //var p = new ProcessManager3();
 //           //p.Init();
 //           //p.Serialize();

 //           var p = Builder.Build<ProcessManager3>(@"Init\ProcessManager3.xml", "ProcessManager3");

 //           EventLogWindow = new EventLogWindow();
 //           EventLogWindow.Init(_eventLog.GetPrimary());
 //           ShowEventLogWindow();

 //           StrategyProcess = new ProcessManager2("Strategy_Process", 500, 0, EventLog);
 //           UIProcess = new ProcessManager2("UI_Process", 1000, 0, EventLog);
 //           DiagnosticProcess = new ProcessManager2("Diagnostic_Process", 15000, 0, EventLog);

 //          // FastProcess = new ProcessManager2("Fast_Process", 100, 0, EventLog);

 //           Positions.Init(EventLog);
 //           TimePlans.Init(EventLog);

 //           Orders.Init(EventLog, Positions, Trades);
 //           Trades.Init(EventLog, Positions, Orders);

 //           SimulateOrders.Init(EventLog, Positions, Trades);

 //           Accounts.Load();

 //           //var wc = new WebClient();
 //           //wc.Init();

 //           //foreach (var a in Accounts.GetAccounts)
 //           //{
 //           //    wc.Register(a);
 //           //    var b = a;
 //           //}

 //           TickersAll.Init(EventLog);
 //           TickersAll.Load();
 //           Tickers.Init(EventLog);

 //           //foreach (var t in TickersAll.GetTickers)
 //           //{
 //           //    wc.Register(t);
 //           //    var b = t;
 //           //}

 //           TradeTerminals.Evl = EventLog;

 //           Orders.BackOrderExecMode = BackTestOrderExecutionMode.Optimistic;
 //           SimulateOrders.BackOrderExecMode = BackTestOrderExecutionMode.Optimistic;

 //           Dde.Init(EventLog);
 //    //       Quotes.Init(EventLog);

 //           EventHub.Init(EventLog);
 //           ActiveOrders.Init(EventLog);

 //           try
 //           {
 //               Strategies = new Strategies.Strategies("MyStrategies", "xStyle", this);
 //               Strategies.Init();
            

 //           foreach (var t in TradeTerminals.TradeTerminalCollection.Values)
 //           {
 //               StrategyProcess.RegisterProcess(t.Code, t.Name, (int)(1 * 1000), 0, null, t.SendOrdersFromQueue, null);
 //               var quik = t as IQuikTradeTerminal;
 //               if (quik == null)
 //                   continue;
 //               quik.TransactionEvent += EventHub.FireEvent;
 //               quik.OrderEvent += EventHub.FireEvent;
 //           }
            
 //           EventHub.Register(new Events.EventHubItem
 //           {
 //               Category = "Transactions",
 //               Entity = "Transaction"
 //           });
 //           EventHub.Register(new Events.EventHubItem
 //           {
 //               Category = "Orders",
 //               Entity = "Order"
 //           });
 //           EventHub.Register(new Events.EventHubItem
 //           {
 //               Category = "Order",
 //               Entity = "Status"
 //           });
 //           EventHub.Register(new Events.EventHubItem
 //           {
 //               Category = "UI.Orders",
 //               Entity = "Order"
 //           });
 //           EventHub.Subscribe("Transactions", "Transaction", Transactions.GetFireEvent);
 //           EventHub.Subscribe("Order", "Status", ActiveOrders.GetFireEvent);

 //           ActiveOrders.ContainerEvent += EventHub.FireEvent;  // for UI Windows

 //           EntryManagers.Init();

 //           //foreach (var t in Tickers.GetTickers)
 //           //{
 //           //    wc.Register(t);
 //           //    var b = t;
 //           //}

 //           Tickers.LoadBarsFromArch();
 //           }
 //           catch (Exception e)
 //           {
 //               throw new Exception(e.Message);
 //           }
            
            
 //#region Windows Init

 //           //EventLogWindow = new EventLogWindow();
 //           TradesWindow = new TradesWindow();

 //           OrdersActiveWindow = new OrdersActiveWindow();
 //           OrdersActiveWindow2 = new OrdersActiveWindow2();
 //           OrdersFilledWindow = new OrdersFilledWindow();

 //           PositionsOpenedWindow = new PositionsOpenedWindow();
 //           PositionsClosedWindow = new PositionsClosedWindow();
 //           PositionTotalsWindow = new PositionTotalsWindow();
 //           PositionsWindow = new PositionsWindow();
 //           TransactionsWindow = new TransactionsWindow();

 //           //OrderPlaneWindow = new OrderPlaneWindow();
 //           //OrderPlaneWindow.Init(this);
        
 //           TradesWindow.Init(EventLog,Trades);

 //           OrdersActiveWindow.Init(this, EventLog, Orders);
 //           OrdersActiveWindow2.Init(this, EventLog);
 //           OrdersFilledWindow.Init(this, EventLog, Orders);

 //           PositionsOpenedWindow.Init(this, EventLog, Positions);
 //           PositionsClosedWindow.Init(EventLog, Positions);
 //           PositionTotalsWindow.Init(this, EventLog, Positions.PositionTotals);
 //           PositionsWindow.Init(this, EventLog, Positions);
 //           TransactionsWindow.Init(EventLog, this);

 //           ChartWindow = new ChartWindow();
 //           ChartWindow.Init(this);

 //           //     QuotesWindow.Init(EventLog, Quotes);

 //           #endregion
 //       }
 //       public void Open()
 //       {
 //           StrategyProcess.NewTickEvent += Tickers.UpdateTimeSeries;
 //     //      StrategyProcess.NewTickEvent += TimePlans.NewTickEventHandler;

 //     //      StrategyProcess.NewTickEvent += OrderPlaneWindow.UpdateQuote;

 //           StrategyProcess.NewTickEvent += TimePlans.NewTickEventHandler;

 //           Tickers.NewTickEvent += SimulateOrders.ExecuteTick;
            
 //         //  Rand.NewTickEvent += Orders.ExecuteTick;
 //         //  Rand.NewTickStrEvent += Tickers.PutDdeQuote3;

 //           DiagnosticProcess.RegisterProcess("Check Connections Process", 15000, 5000, null, TradeTerminals.CheckConnection, null);

 //           var topic = Dde.RegisterTopic("Forts", "Main Quotes", Tickers.PutDdeQuote3);
 //           topic.RegisterChannel(1, "Rtsi", Tickers.PutDdeQuote3);

 //           ShowWindows();
 //       }
 //       public void OpenChart()
 //       {
 //           UIProcess.RegisterProcess("Chart Window Refresh", "ChartWindowRefreshProcess",
 //                                                   1000, 0, null, ChartWindow.TickRefreshEventHandler, null);
 //           ChartWindow.Show();
 //       }

 //       public void Close()
 //       {
 //           Dde.Close();

 //           StrategyProcess.NewTickEvent -= Tickers.UpdateTimeSeries;
 //      //     StrategyProcess.NewTickEvent -= TimePlans.NewTickEventHandler;

 //       //    StrategyProcess.NewTickEvent -= OrderPlaneWindow.UpdateQuote;
            
 //           StrategyProcess.NewTickEvent -= TimePlans.NewTickEventHandler;

 //           Rand.NewTickEvent -= Orders.ExecuteTick;
 //           Rand.NewTickStrEvent -= Tickers.PutDdeQuote3;

 //           Tickers.NewTickEvent -= Orders.ExecuteTick;
 //           Tickers.NewTickEvent -= SimulateOrders.ExecuteTick;

 //           StrategyProcess.Close();
 //           DiagnosticProcess.Close();
 //           UIProcess.Close();

 //           Strategies.Close();

 //           CloseWindows();

 //          // EventLogWindow.Close();
 //       }
 //       public void Start()
 //       {
 //           Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "Begin Start Process", "", "");

 //           UIProcess.Start();
 //           StrategyProcess.Start();
 //           DiagnosticProcess.Start();

 //         //  TimePlans.Start();

 //           Dde.Start();

 //           // Rand.Start();
            
 //           Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "Finish Start Process", "", "");
 //       }
 //       public void StartRand()
 //       {
 //           Rand.Start();
 //       }

 //       public void Stop()
 //       {
 //           Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "Begin Stop Process","","");
            
 //           Dde.Stop();
 //           TimePlans.Stop();

 //           Rand.Stop();

 //           TradeTerminals.DisConnect();

 //           StrategyProcess.Stop();
 //           DiagnosticProcess.Stop();
 //           UIProcess.Stop();

 //           Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "Finish Stop Process", "", "");
 //       }
 //       public void ShowWindows()
 //       {
 //           //EventLogWindow.Show();

 //           TradesWindow.Show();

 //           OrdersActiveWindow.Show();
 //           OrdersActiveWindow2.Show();

 //           OrdersFilledWindow.Show();

 //           PositionsOpenedWindow.Show();
 //           PositionsClosedWindow.Show();
 //           PositionTotalsWindow.Show();
 //           PositionsWindow.Show();

 //           TransactionsWindow.Show();

 //           //OrderPlaneWindow.Show();

 //          // QuotesWindow.Show();

 //           // ChartWindow.Show();
 //       }
 //       public void CloseWindows()
 //       {
 //           //EventLogWindow.Close();

 //           ChartWindow.Close();

 //           TradesWindow.Close();

 //           OrdersActiveWindow.Close();
 //           OrdersActiveWindow2.Close();

 //           OrdersFilledWindow.Close();

 //           PositionsOpenedWindow.Close();
 //           PositionsClosedWindow.Close();
 //           PositionTotalsWindow.Close();
 //           PositionsWindow.Close();
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
 //           Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, code, "RegisterTicker", "Ticker: " + code + "Is NOT Found", "");
 //           return null;
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
 //       public Position RegisterPosition( string account, string strategy, ITicker ticker)
 //       {
 //           return Positions.Register2(account, strategy, ticker);
 //       }

 //       public IAccount GetAccount(string key)
 //       {
 //           return Accounts.GetAccount(key);
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
 //           throw new NotImplementedException();
 //       }

 //       public IOrder Resolve(IOrder o)
 //       {
 //           throw new NotImplementedException();
 //       }

 //       public IStrategy GetStrategyByKey(string key)
 //       {
 //           throw new NotImplementedException();
 //       }

 //       public IStrategy RegisterDefaultStrategy(string name, string code, string accountKey, string tickerKey, uint timeInt,
 //           string terminalType, string terminalKey)
 //       {
 //           throw new NotImplementedException();
 //       }

 //       public void Save(IOrder o)
 //       {
 //           throw new NotImplementedException();
 //       }

 //       public void Save(ITrade3 t)
 //       {
 //           throw new NotImplementedException();
 //       }

 //       public TimePlan RegisterTimePlanEventHandler(string timePlanKey,
 //                                                                   EventHandler<TimePlanEventArgs> action)
 //       {
 //           return TimePlans.RegisterTimePlanEventHandler(timePlanKey, action);
 //       }

 //       public IPortfolio RegisterPortfolio(string code, string name)
 //       {
 //           var p = new Portfolio { Code = code, Name = name };
 //           IPortfolio ip;
 //           if ((ip = Portfolios.AddNew(p)) != null)
 //               return ip;
 //           throw new NullReferenceException("Portfolio Register Failure:" + p.ToString());
 //       }
 //       public IEntryManager RegisterEntryManager(string code, string name)
 //       {
 //           var p = new EntryManager { Code = code, Name = name };
 //           IEntryManager ip;
 //           if ((ip = EntryManagers.AddNew(p)) != null)
 //               return ip;
 //           throw new NullReferenceException("EntryManager Register Failure:" + p.ToString());
 //       }

 //       public void Evlm(EvlResult result, EvlSubject subject,
 //                                   string source, string operation, string description, string obj)
 //       {
 //           if (_eventLog != null)
 //               _eventLog.AddItem(result, subject, source, operation, description, obj);
 //           /*
 //           try
 //           {
 //               if (WebEventLogClient != null)
 //                   WebEventLogClient.AddItem(result, subject, source, "entity", operation, description, obj)
 //                   ;
 //           }
 //           catch (Exception e)
 //           {
 //               throw new Exception( e.Message);
 //           }
 //            */ 
 //       }
 //       public void Evlm(EvlResult result, EvlSubject subject,
 //                                   string source, string entity, string operation, string description, string obj)
 //       {
 //           if (_eventLog != null)
 //               _eventLog.AddItem(result, subject, source, entity, operation, description, obj);
 //           /*
 //           try
 //           {
 //               if (WebEventLogClient != null)
 //                   WebEventLogClient.AddItem(result, subject, source, entity, operation, description, obj)
 //                   ;
 //           }
 //           catch (Exception e)
 //           {
 //               throw new Exception(e.Message);
 //           }
 //            */ 
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

 //               Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TxInitData", "Serialization",
 //               String.Format("FileName={0}", xmlfname), "");

 //               return true;
 //           }
 //           catch (Exception e)
 //           {
 //               Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "TxInitData", "Serialization",
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
 //                   (s1, s2) => Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
 //                                   "TradeContextInit", s1, String.Format("FileName={0}", xmlfname), s2)
 //                                   );
                
 //               Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TradeContextInit", "DeSerialization",
 //               String.Format("FileName={0}", xmlfname), "");

 //           }
 //           catch (Exception e)
 //           {
 //               Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "TradeContextInit", "DeSerialization",
 //               String.Format("FileName={0}", xmlfname), e.ToString());

 //               throw new SerializationException("TradeContextInit.Deserialization Failure " + xmlfname);
 //           }
 //           return true;
 //       }
 //        public int RegisterNewTrade(ITrade t)
 //       {
 //           var ret = Strategies.RegisterNewTrade(t);
 //           return +1;
 //       }
 //   }
}
