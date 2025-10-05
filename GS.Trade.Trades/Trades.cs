using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;
using GS.Extension;
using GS.Interfaces;
//using GS.Trade.TradeTerminals.Quik;

//namespace sg_Trades
namespace GS.Trade.Trades
{
    //public delegate void AddTradeToList(Trade trade);
    public delegate void GetTradesToObserve();

    public class Trades : ITradeStatusReceiver, ITrades
    {
        //public delegate void NeedToOserverEventHandler();
        public event NeedToOserverEventHandler NeedToObserverEvent;

        private long _lastObserveGetRequestNumber;
        public bool bNewTrade { get; set; }

        public ulong LastTradeNumber { get; set; }
        public DateTime LastTradeDT{get;set;}
        public string LastTradeStr { get; set; }
        public string LastTradeStrError { get; set; }

        public IEventLog EventLog;
        private IOrders _orders;

        //public AddTradeToList CallbackAddTradeToList;
        public GetTradesToObserve CallbackGetTradesToObserve;

        private readonly Queue<string> qTradeStr;

        public ObservableCollection<Trade> TradeObserveCollection { get; set; }
        public List<Trade> TradeCollection {get; set;}
        public Dictionary<string, Trade> TradeDictionary { get; set; }

        public Thread GetTradeThread;

        public IPositions _Positions;

        private DateTime Get_Next_Trades_Request_DT = DateTime.Now;
        private int Get_Trades_From_Queue_Delay = 5;

        private  volatile Int32 _myIndex;
        private object _tradeLocker;

        private Random r = null;

        private bool _needToObserver = false;

      //  private long _tradeId = 0;

        public string ClassCodeToRemoveLogin { get; set; }
        public string LoginToRemove { get; set; }

        public Trades()
        {
            TradeObserveCollection = new ObservableCollection<Trade>();
            TradeCollection = new List<Trade>(); 
            TradeDictionary = new Dictionary<string, Trade>();

            qTradeStr = new Queue<string>();
            LastTradeNumber = 0;

            _myIndex = 0;

            _tradeLocker = new object();
        }
        public void Init(IEventLog evl, IPositions positions, IOrders orders)
        {
            EventLog = evl;
            _orders = orders;
            // _Positions = new Positions();
            _Positions = positions;
            // DeserializePositions();

            //_Positions.Init(evl);
            /*
            foreach (var p in _Positions.PositionCollection.Where(p => p.Opened && p.Quantity !=0))
            {
                var t = new Trade(p.LastTradeNumber, p.LastTradeDT, p.Ticker,
                                    p.Operation, p.Quantity, p.LastTradePrice, 0,
                                    p.Account, p.Strategy, 1); 
                Add(t); 
                //_Positions.PositionCalculate(t);
            }
            */
            bNewTrade = true;
            LastTradeNumber = 0;
            LastTradeDT = DateTime.Now.AddYears(-10);

            r = new Random();

            EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "Trades", "Trades", "Initialization", "", "");

        }
        /*
        public void SetCallBack_TradeToList(AddTradeToList cb)
        {
            CallbackAddTradeToList = cb;
        }
        */
        /*
        private static string Key(Trade t)
        {
            return t.Number + t.Account + t.Strategy + t.Ticker;
        }
        */
        private bool AddTrade(Trade trade)
        {
            var key = trade.Key;
            if ( ! TradeDictionary.ContainsKey(key) )
            {
                lock (_tradeLocker)
                {
                    trade._Trades = this;
                    trade.MyIndex = ++_myIndex;

                    TradeDictionary.Add(key, trade);

                    _needToObserver = true;
                }
                EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, trade.StratTicker,
                    "Trade", "New", trade.ShortInfo, trade.ToString());
                                 //String.Format("TradeCount: {0}", TradeCollection.Count));
                
                return true;
            }
            EventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, trade.StratTicker,
                                 "Trade", "New", trade.ShortInfo + "Trade already Registered", trade.ToString());
            return false;

        }
        /*
        private void AddTrade(Trade trade)
        {           
                lock (_tradeLocker)
                {
                    trade._Trades = this;
                    trade.MyIndex = ++_myIndex;

                    TradeCollection.Add(trade);

                    LastTradeNumber = trade.Number;
                    LastTradeDT = trade.DT;
                }
                EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
                                  "Trades", "Add New", trade.ToString(),
                                  String.Format("TradeCount: {0}", TradeCollection.Count));
           
        }
         */ 
        public void Add1(Trade trade)
        {         
            trade._Trades = this;
            trade.MyIndex = ++_myIndex;

            TradeCollection.Add(trade);

            LastTradeNumber = trade.Number;
            LastTradeDT = trade.DT;

               // _Positions.Calculate(TradeCollection);

            EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "Trades",
                    "Trades", "Add New Trade", trade.ToString(), String.Format("TradeCount: {0}", TradeCollection.Count));
        }
        public void Clear()
        {
            TradeCollection.Clear();
        }
        
        public void Put_DDE_Trade_ToQueue(string _ddestr)
        {
            // DateTime _dt = DateTime.Now;
            lock (this)
            {
                    qTradeStr.Enqueue(_ddestr);
            }
        }
        public void Put_DDE_Trade_To_Queue_Test()
        {
           // DateTime _dt = DateTime.Now;
            //int j = _dt.Minute*100 + _dt.Second;
            
            string operation;
            string ticker;
            string strategy;
            string account;

            
            for (int i = 100; i < 200; i++)
            {
                string s;
                var ri = r.Next(0, 10);
                operation = ri % 2 == 0 ? "Продажа" : "Купля";
                ri = r.Next(0, 10);
                ticker = ri % 2 == 0 ? "RIZ0" : "RIM0";  // "RIM0" : "RIZ0";
                ri = r.Next(0, 10);
                strategy = ri % 2 == 0 ? "G123" : "S999"; // "S999" : "S999"; // "S123" : "S999";
                ri = r.Next(0, 10);
                account = ri % 2 == 0 ? "ITInvest" : "VTB24"; //  "VTB24" : "VTB24"; // "ITInvest" : "VTB24";
                ri = r.Next(0, 10);
                int j = ri % 2 == 0 ? 1 : 2;

                s = String.Format("{0};{1:d};{2:t};{3};{4};{5};{6};{7};{8};{9};",
                                   ++LastTradeNumber, DateTime.Now, DateTime.Now.TimeOfDay, ticker, operation, i * j * 10, (i - 99) * j, i * j * 100, account, strategy);
                Put_DDE_Trade_ToQueue(s);
            }
        }
        //public void GetTradesFromQueueTest()
        //{
        //    string s = null; int cnt = 0; int cnt2 = 0;

        //    lock (this)
        //    {
        //        cnt = qTradeStr.Count; cnt2 = cnt;
        //        while (cnt-- > 0)
        //        {
        //            s = qTradeStr.Dequeue();
        //            Trade t = null;

        //            if ((t = Parse_Trade(s)) != null)
        //            {
                       
        //                bool b_newtrade = true; long n = 0;
                        
        //                foreach (Position p in _Positions.PositionCollection.Where(p=>p.Opened))
        //                {
        //                    if (t.Number <= p.LastTradeNumber && t.Account == p.Account && t.Strategy == p.Strategy && t.Ticker == p.TickerStr)
        //                    {
        //                        b_newtrade = false; n = p.LastTradeNumber; break;
        //                    }
        //                }
                        
        //                if (b_newtrade) 
        //                {
        //                    //if (callbackAddTradeToList != null)
        //                    //{
        //                        //callbackAddTradeToList(t);
        //                      //  Add(t);
        //                      //  bNewTrade = true;
        //                        //_Positions.PositionCalculate(t);
        //                    //}
        //                    //else
        //                    //{
        //                    if ( AddTrade(t) )
        //                    {
        //                        bNewTrade = true;
        //                        //_Positions.PositionCalculate2(t);
        //                        _Positions.PositionCalculate3(t);
        //                    }
        //                    //}
        //                }
        //                else
        //                    EventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING,
        //                        "Trades", "Add New Trade",
        //                        String.Format("New Trade number: {0} less then Positions.LastTradeNumber {1}. Acc {2}, Strat {3}, Tick {4} Trades count: {5}",
        //                        t.Number, n, t.Account, t.Strategy, t.Ticker, TradeCollection.Count),"");
        //            }
        //        }
        //        //if (cnt2 > 0) { _Positions.Calculate(TradeCollection); }
        //    }

        //}

        //public void Get_Trades_From_Queue()
        //{
        //    DateTime _dt = DateTime.Now;
        //    if (_dt >= Get_Next_Trades_Request_DT)
        //    {
        //        Get_Next_Trades_Request_DT = _dt.AddSeconds(Get_Trades_From_Queue_Delay);

        //     //   _EventLog.Add(new EventLogItem(EvlResult.SUCCESS, "Trades. Get Trade From Queue",
        //     //          String.Format("Pooling Delay: {0}. NextRequest: {1}", 
        //     //          Get_Trades_Polling_Delay_Seconds, Get_Next_Trades_Request_DT)));

        //        string s = null; int cnt = 0; int cnt2 = 0;

        //        lock (this)
        //        {
        //            cnt = qTradeStr.Count; cnt2 = cnt;
        //            while (cnt-- > 0)
        //            {
        //                s = qTradeStr.Dequeue();
        //                Trade t = null;

        //                if ((t = Parse_Trade(s)) != null)
        //                {
        //                    /*
        //                    bool any = _Positions.PositionCollection.Any(
        //                        p => p.Account.Equals(t.Account) && p.Strategy.Equals(t.Strategy) && p.Ticker.Equals(t.Ticker)
        //                            && p.MaxTradeNumber >= t.Number);
        //                    bNewTrade = !any;
        //                    */
        //                    bool b_newtrade = true; long n = 0;
        //                    foreach (Position p in _Positions.PositionCollection)
        //                    {
        //                        if (t.Number <= p.LastTradeNumber && t.Account == p.Account && t.Strategy == p.Strategy && t.Ticker == p.TickerStr)
        //                        {
        //                            b_newtrade = false; n = p.LastTradeNumber; break; 
        //                        }
        //                    }
        //                    if (b_newtrade)
        //                    {
        //                        if( AddTrade(t))
        //                        {
        //                            bNewTrade = true; _Positions.PositionCalculate(t);
        //                        }
        //                    }
        //                    else
        //                        EventLog.AddItem(EvlResult.FATAL,EvlSubject.TRADING,
        //                            "Trades","Add New Trade",
        //                            String.Format("New Trade number: {0} less then Positions.LastTradeNumber {1}. Acc {2}, Strat {3}, Tick {4} Trades count: {5}",
        //                            t.Number, n, t.Account, t.Strategy, t.Ticker, TradeCollection.Count),"");                        }
        //            }
        //            if(cnt2 > 0)  { _Positions.Calculate(TradeCollection);  }
        //        }
        //    }
        //}
        public Trade Parse_Trade(string Trade_String)
        {
             string sError=null;
             try
             {
                 string[] split = Trade_String.Split(new Char[] { ';' });
                 sError = "Split_sNumber";
                 string sNumber = (split[0]).Trim();
                 sError = "Split_sDT";
                 string sD = (split[1]).Trim();
                 string sT = (split[2]).Trim(); 
                 sError = "Split_ticker";
                 string ticker = (split[3]).Trim();
                 sError = "Split_sDirection";
                 string sDirection = (split[4]).Trim();
                 sError = "Split_sTradePlace";
                 string sTradePrice = (split[5]).Trim();
                 sError = "Split_sAmount";
                 string sAmount = (split[6]).Trim();
                 sError = "Split_sOrderNumber";
                 string sOrderNumber = (split[7]).Trim();
                 sError = "Split_account";
                 string account = (split[8]).Trim();
                 sError = "Split_strategy";
                 string strategy = (split[9]).Trim();

                 ulong tradenumber; DateTime dt; TradeOperationEnum direction; decimal tradeprice; int amount;
                 ulong ordernumber; int idtradeowner;

                 sError = "Parse_number";
                 tradenumber = ulong.Parse(sNumber);
                 sError = "Parse_dt";
                 dt = DateTime.Parse(sD + ' ' + sT);
                 direction = (sDirection == "Купля") ? TradeOperationEnum.Buy : TradeOperationEnum.Sell;
                 sError = "Parse_tradeprice";
                 tradeprice = Decimal.Parse(sTradePrice);
                 sError = "Parse_amount";
                 amount = int.Parse(sAmount);
                 sError = "Parse_ordernumber";
                 ordernumber = ulong.Parse(sOrderNumber);
                 if (strategy == null || strategy == "")
                     strategy = "999";
                 idtradeowner = 1;
                 sError = "";

                 LastTradeStr = Trade_String;

                 //_EventLog.Add(new EventLogItem(EvlResult.SUCCESS, "Trades Parse",
                 //      String.Format("{0} {1} {2}.", tradenumber, dt, ticker)));

                 return new Trade(tradenumber, dt, ticker, direction, amount, tradeprice, ordernumber, account, strategy, idtradeowner);
             }
             catch
             {
                 LastTradeStrError = Trade_String;
                 EventLog.AddItem(EvlResult.FATAL, "TradeDdeChannel.OnPoked",
                                    String.Format("DdeDataStr Trade Parse Error - {0} DdeStr {1}", sError, Trade_String));
                 return null;
             }
        }
        public void Set_Get_Trades_Delay(int Get_Trades_From_Queue_Delay_In_Seconds)
        {
            Get_Trades_From_Queue_Delay = Get_Trades_From_Queue_Delay_In_Seconds;
            EventLog.AddItem(EvlResult.WARNING, "Trades",
                String.Format("New Value for Get_Trades_From_Queue_Delay is set to {0}", Get_Trades_From_Queue_Delay)); 
        }
        
        public void Start_GetTradeThread()
        {
            if(GetTradeThread == null)
            {
                GetTradeThread = new Thread(ThreadTradesUpdate);
                GetTradeThread.Start();
            }
            else if( ! GetTradeThread.IsAlive) GetTradeThread.Start();
        }
        public void Stop_GetTradeThread()
        {
            if (GetTradeThread != null)
                if (GetTradeThread.IsAlive)
                {
                    GetTradeThread.Abort(); GetTradeThread.Join();
                    GetTradeThread = null;
                }
        }
        public void ThreadTradesUpdate() //AddTradeToList cb)
        {
            //callbackAddTradeToList = cb;

            DateTime dt;
            int iDelayInSeconds;
            ulong i = 200;

            dt = DateTime.Now;
            iDelayInSeconds = (65 - dt.Second) % 60;
            while (true)
            {
                dt = DateTime.Now;
                iDelayInSeconds = (65 - dt.Second) % 60;
                //Thread.Sleep(iDelayInSeconds * 1000);
                Thread.Sleep(TimeSpan.FromSeconds(5));
                Trade tr = new Trade(++i, DateTime.Now, "RIU0", TradeOperationEnum.Sell, i * 10);
            /*
                if (CallbackAddTradeToList != null) CallbackAddTradeToList(tr);
                else
                {
                    TradeCollection.Add(tr);
                   // SystemSounds.Beep.Play();
                }
                Thread.Sleep(2 * 1000);
             */ 
            }
        }
        public void GetTradesToObserve()
        {
            lock (_tradeLocker)
            {
                var i = _lastObserveGetRequestNumber;
                foreach (var trade in TradeDictionary.Values.Where(trade => trade.MyIndex > i))
                {
                    TradeObserveCollection.Insert(0, trade);
                    _lastObserveGetRequestNumber = trade.MyIndex;
                }
                _needToObserver = false;
            }
        }
        /*
        public void GetTradesToObserve()
        {
            lock (_tradeLocker)
            {
                var i = _lastObserveGetRequestNumber;
                foreach (var trade in TradeCollection.Where(trade => trade.MyIndex > i))
                {
                    TradeObserveCollection.Insert(0, trade);
                    _lastObserveGetRequestNumber = trade.MyIndex;
                }
               
            }
        }
        */
        public bool SerializePositions()
        {
            try
            {
                string _xmlfname;
                //XDocument xDoc = XDocument.Load(@"D:\Mts\Mts1\SpsInit.xml");
                XDocument xDoc = XDocument.Load(@"SpsInit.xml");
                XElement xe = xDoc.Descendants("Positions_XmlFileName").First();
                _xmlfname = xe.Value;

                TextWriter tr = new StreamWriter(_xmlfname);
                XmlSerializer sr = new XmlSerializer(typeof(Positions));
                sr.Serialize(tr, _Positions);
                tr.Close();

                EventLog.AddItem(EvlResult.SUCCESS, "Positions",
                String.Format("Serialization to file {0}", _xmlfname));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeserializePositions()
        {
            try
            {
                //XDocument xDoc = XDocument.Load(@"D:\Mts\Mts1\SpsInit.xml");
                var xDoc = XDocument.Load(@"SpsInit.xml");
                var xe = xDoc.Descendants("Positions_XmlFileName").First();
                var xmlfname = xe.Value;

                var f = new FileStream(xmlfname, FileMode.Open);
                var newSr = new XmlSerializer(typeof(Positions));
                _Positions = (Positions)newSr.Deserialize(f);
                f.Close();

                // sps.Init(_EventLog, _Trades, _Quotes, new SendToView(SendStrToView));

                EventLog.AddItem(EvlResult.SUCCESS, "Positions", String.Format("DeSerialization from file {0}", xmlfname));

                return true;
            }
            catch (Exception)
            { return false; }
        }

        public void NewTrade(string trade)
        {
            throw new NotImplementedException();
        }
        public void NewTrade(   ulong number, DateTime dt,
                                string account, string strategy, string ticker,
                                TradeOperationEnum operation, int quantity, double price, string comment,
                                ulong orderNumber, double commission)
        {
            var t = new Trade( number, dt, (int)0, account, strategy, ticker, ticker, operation, quantity, price, comment, orderNumber, commission);
            
            if( AddTrade(t))
                //_Positions.PositionCalculate3(t);
                _Positions.PositionCalculate5(t);
            if (_needToObserver) NeedToObserverEvent?.Invoke();
        }
        // Current 
        public void NewTrade(   double dNumber, int iDate, int iTime, int nMode,
                                string account, string strategy, string classCode, string ticker,
                                int iIsSell, int quantity, double price, string comment, double orderNumber, double commissionTs)
        {
            // var number = Convert.ToInt64(dNumber);
            var number = Convert.ToUInt64(dNumber);
            var sDt = iDate.ToString("D8");
            var sTm = iTime.ToString("D6");
            var dt = new DateTime(
                int.Parse(sDt.Substring(0, 4)), int.Parse(sDt.Substring(4, 2)), int.Parse(sDt.Substring(6, 2)),
                int.Parse(sTm.Substring(0, 2)), int.Parse(sTm.Substring(2, 2)), int.Parse(sTm.Substring(4, 2)));

            var operation = iIsSell == 0 ? TradeOperationEnum.Buy : TradeOperationEnum.Sell;
            var ordNumber = Convert.ToUInt64(orderNumber);
            if (classCode != "FUTEVN")
            {
                if (ClassCodeToRemoveLogin.HasValue())
                    if (classCode.Contains(ClassCodeToRemoveLogin))
                        strategy = strategy.Replace(LoginToRemove, "");

                    var t = new Trade(number, dt, nMode, account, strategy, classCode, ticker, operation, quantity,
                                      price, strategy,
                                      ordNumber, commissionTs);
                    if ( AddTrade(t) )
                    {
                        //_Positions.PositionCalculate2(t);
                        //_Positions.PositionCalculate3(t);
                        _Positions.PositionCalculate5(t);
                    }
            }
            else
            {
              var t = new Trade(number, dt, nMode, account, strategy, classCode, ticker, operation, quantity,
                                      price, comment,
                                      ordNumber, commissionTs);

              EventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Trades",
                                     "Trades", "Evening Session Trade", t.ToString(), "Ignore Positions Calculation");
                
            }
            if (_needToObserver) NeedToObserverEvent?.Invoke();
        }
        //public void NewTradeOld(double dNumber, int iDate, int iTime, int nMode, 
        //    string account, string strategy, string classCode, string ticker, 
        //    int iIsSell, int quantity, double price, string comment, double orderNumber, double commissionTs)
        //{
        //    long number = Convert.ToInt64(dNumber);
        //    var sDt = iDate.ToString("D8");
        //    var sTm = iTime.ToString("D6");
        //    var dt = new DateTime(
        //        int.Parse(sDt.Substring(0, 4)), int.Parse(sDt.Substring(4, 2)), int.Parse(sDt.Substring(6, 2)),
        //        int.Parse(sTm.Substring(0, 2)), int.Parse(sTm.Substring(2, 2)), int.Parse(sTm.Substring(4, 2)));

        //    var operation = iIsSell == 0 ? Trade.OperationEnum.Buy : Trade.OperationEnum.Sell;
        //    long ordNumber = Convert.ToInt64(orderNumber);
        //    if (classCode != "FUTEVN")
        //    //if(classCode != null)
        //    {
        //        var order = _orders.OrderCollection.
        //            Where(o => Equals(o.Account, account) && Equals(o.Ticker, ticker) && o.Number == orderNumber).
        //            FirstOrDefault();
        //        if (order != null)
        //        {
        //            var t = new Trade(number, dt, nMode, account, order.Strategy, classCode, ticker, operation, quantity,
        //                              price, strategy,
        //                              ordNumber, commissionTs);
        //            if (AddTrade(t))
        //            {
        //                _Positions.PositionCalculate2(t);
        //            }
        //            /*
        //            _EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
        //                        "Trades", "New Trade","Order with My Strategy Found", t.ToString());
        //             */
        //        }
        //        else
        //        {
        //            var t = new Trade(number, dt, nMode, account, "000", classCode, ticker, operation, quantity, price,
        //                              strategy,
        //                              ordNumber, commissionTs) {MyStatus = -1};
        //            AddTrade(t);
        //            EventLog.AddItem(EvlResult.SOS, EvlSubject.TRADING,
        //                              "Trades", "New Unknown Trade", "Order with Trade Strategy NOT FOUND", t.ToString());
        //        }
        //    }
        //    else
        //    {
        //        var t = new Trade(number, dt, nMode, account, strategy, classCode, ticker, operation, quantity,
        //                              price, comment,
        //                              ordNumber, commissionTs);
        //        EventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING,
        //                              "Trades", "Evening Session Trade", t.ToString(),"");
        //    }

        //}
        
        public void NewTradeProcess(double number, DateTime dt, int mode,
            string account, string strategy, string classCode, string ticker,
            short operation, int quantity, double price, string comment, long orderNumber, double commissionTs)
        {
            
        }
        public void ClearSomeData(int count)
        {
            lock (_tradeLocker)
            {
                var tt = TradeDictionary.Values.ToList();
                for(var i = 0; TradeDictionary.Count > count; i++)
                    TradeDictionary.Remove(tt[i].Key);
            }
        }
    }


}

