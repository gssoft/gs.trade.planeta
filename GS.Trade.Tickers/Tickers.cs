using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;
using GS.Queues;


namespace GS.Trade.Tickers
{
    public class Tickers2 : Element1<string>, ITickers2 //, IHaveQueue<string>
    {
        //public delegate void NewTickEventHandler(DateTime dt, string tickerkey, double last, double bid, double ask);
        public event NewTickEventHandler NewTickEvent;

        //[XmlIgnore]
        //private IEventLog _evl;
        protected QueueFifo<Ticker2.Quote> DdeQuoteStrQueue;
        protected QueueFifo<string> DdeQuoteStrQueue2;
        private readonly Dictionary<string, Ticker2>  _tickerDictionary;
        private readonly Queue<string> _quoteStrQueue;
        //private readonly QuoteDataBase _barsDataBase = new QuoteDataBase();

        //public List<Ticker> _tickerList;

        public IEnumerable<ITicker> TickerCollection { get { return _tickerDictionary.Values; } }
        //public ICollection<ITicker> GetTickers { get { return _tickerDictionary.Values.ToList() as ICollection<ITicker>; } }
        //public IEventLog EventLog { get { return EventLog; } } 

        private readonly object _lockPutQuote;
        private readonly object _lockAddTicker;
        private object _lockQuoteProcess;

        public override string Key 
        {
            get { return Code; }
        }

        public Tickers2()
        {
            Code = "Tickers.Core.Storage";
            Name = "Tickers Core.Storage";

            _tickerDictionary = new Dictionary<string, Ticker2>();
            _quoteStrQueue = new Queue<string>();

            DdeQuoteStrQueue = new QueueFifo<Ticker2.Quote>();
            DdeQuoteStrQueue2 = new QueueFifo<string>();

            _lockPutQuote = new object();
            _lockAddTicker = new object();
            _lockQuoteProcess = new object();

        }
        public override void Init( IEventLog evl )
        {
            try
            {
                if (evl == null)
                    throw new NullReferenceException("Tickers.Init() - Failure: EventLog == null");
                base.Init(evl);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(Code, GetType().ToString(), "Init()", ToString(), e);
                throw;
            }
            // _evl = evl;
           //// DeserializeTickers();
            Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, Code, Code, "Initialization()", "Complete", "");
        }
        public void Load()
        {
            DeserializeTickers();
        }
        public void LoadBarsFromArch()
        {
            foreach(var t in _tickerDictionary.Values)
            {
                //if( t.LoadMode > 0)
                //    t.LoadBarsFromArch(_barsDataBase);
            }
        }

        public ITicker CreateInstance(ITickerBase t)
        {
            return new Ticker2
            {
                Id = t.Id,

                Code = t.Code,
                Name = t.Name,
                Alias = t.Alias,

                ClassCode = t.TradeBoard,
                TradeBoard = t.TradeBoard,
                BaseContract = t.BaseContract,

                Decimals = t.Decimals,
                Margin = t.Margin,
                MinMove = t.MinMove,

                PriceLimit = t.PriceLimit,
            };
        }

        //public ITicker CreateTicker(string board, string code)
        //{
        //    return new Ticker
        //    {
        //        TradeBoard = board,
        //        Code = code,
        //        Name = board + "@" + code,
        //        MinMove = 1,
        //    };
        //}

        public ITicker Register(ITicker ti)
        {
            var t = GetTicker(ti.Key);
            if (t != null)
                return t;
            ti.Tickers = this;
            ti.TradeBoard = ti.ClassCode;
            ti.EventLog = EventLog;
            AddTicker((ITicker)ti);
            return ti;
        }

        //public ITicker Register(string board, string code)
        //{
        //    var t = GetTicker(code);
        //    if (t != null)
        //        return t;
        //    return CreateTicker(board, code);
        //}

        public void Add(ITicker t)
        {
            var key = t.Key.Trim();
            lock (_lockAddTicker)
            {
                if (_tickerDictionary.ContainsKey(key)) return;

                t.Tickers = this;
                t.TradeBoard = t.ClassCode;
                t.EventLog = EventLog;
                _tickerDictionary.Add(key, (Ticker2)t);

                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName, "Ticker",
                                "AddNew: " + key.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                                "Count=" + _tickerDictionary.Count, t.ToString());
            }
        }

        public void AddTicker( ITicker t )
        {
            var res = false;
            var key = t.Key.Trim();
            lock (_lockAddTicker)
            {
                if (!_tickerDictionary.ContainsKey(key))
                {
                    _tickerDictionary.Add(key, (Ticker2)t);
                    res = true;
                }
            }
            if(res)
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName, key, "AddNew " + key,
                                 t.ToString(), "Count=" + _tickerDictionary.Count);
            // else
            //    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Tickers", "Add New " + key,
            //                     "Ticker Already Exist", t.ToString());
        }

        public ITicker GetTicker( string code )
        {
            try
            {
                Ticker2 t;
                lock (_lockAddTicker)
                {
                    var boo = _tickerDictionary.TryGetValue(code.Trim(), out t);
                    if (boo)
                        return t;
                    t = _tickerDictionary.Values.ToList().FirstOrDefault(ac => ac.Code.HasValue() && ac.Code.Trim() == code.Trim()) ??
                           _tickerDictionary.Values.ToList().FirstOrDefault(ac => ac.Alias.HasValue() && ac.Alias.Trim() == code.Trim());
                }
                if (t != null)
                    return t;

                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, FullName, "Tickers", "GetTicker()",
                                 String.Format("Ticker: {0} is not Found",code) ,"");
                return null;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "Ticker", "GetTicker()", code, e);
                throw;
            }
        }
        public ITicker this[string index]
        {
            get
            {
                return _tickerDictionary.Keys.Contains(index.Trim().ToUpper()) ? _tickerDictionary[index] : null;
            }
        }

        public IEnumerable<ITicker> GetTickers
        {
            get { return _tickerDictionary.Values; }
        }

        public void PutDdeQuote2(string ddeStr)
        {
            lock (_lockPutQuote)
            {
                _quoteStrQueue.Enqueue(ddeStr);
            }
        }
        public void PushDdeQuoteStr(string s)
        {
            if (s.HasNoValue())
                return;
            try
            {
                var q = ParseQuoteStr(s);
                if (q == null) return;

                if (NewTickEvent != null) NewTickEvent(q.DT, q.Ticker, q.Last, q.Bid, q.Ask);

                DdeQuoteStrQueue.Push(q);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "DdeStr", "PushDdeQuoteStr()", s, e);
                throw;
            }
        }
        public void DeQueueDdeQuoteStrProcess()
        {
            if (DdeQuoteStrQueue.IsEmpty)
                return;

            //Evlm(EvlResult.WARNING, EvlSubject.TECHNOLOGY, FullName, "Repository", "DeQueueProcess", "Try to get Entities to Perform Operation","");

            var items = DdeQuoteStrQueue.GetItems();
            lock (_lockQuoteProcess)
            {
                foreach (var q in items)
                {
                    UpdateQuoteOrNew3(q); // Update TickerLast and Async (BarSeries)
                }

                UpdateTimeSeries(DateTime.Now);
            }
        }
        public void PushDdeQuoteStr1(string s)
        {
            if (s.HasNoValue())
                return;
            DdeQuoteStrQueue2.Push(s);          
        }
        public void DeQueueDdeQuoteStrProcess1()
        {
            if (DdeQuoteStrQueue2.IsEmpty)
                return;
            try
            {
                var items = DdeQuoteStrQueue2.GetItems();
                foreach (var s in items)
                {
                    var q = ParseQuoteStr(s);
                    if (q == null)
                        continue;

                    if (NewTickEvent != null)
                        NewTickEvent(q.DT, q.Ticker, q.Last, q.Bid, q.Ask);

                    UpdateQuoteOrNew3(q); // Calcs Bars
                }
                UpdateTimeSeries(DateTime.Now); // Calculate TimeSeries
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "DdeStr", "DeQueueDdeQuoteStrProcess1()","", e);
                throw;
            }
        }
        public void DeQueueDdeQuoteStrProcess11()  // Calculate Only Bars
        {
            if (DdeQuoteStrQueue2.IsEmpty)
                return;
            try
            {
                var items = DdeQuoteStrQueue2.GetItems();
                foreach (var s in items)
                {
                    var q = ParseQuoteStr(s);
                    if (q == null)
                        continue;

                    if (NewTickEvent != null)
                        NewTickEvent(q.DT, q.Ticker, q.Last, q.Bid, q.Ask);

                    UpdateQuoteOrNew3(q); // Calcs Bars
                }
                //UpdateTimeSeries(DateTime.Now); // Calculate TimeSeries
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "DdeStr", "DeQueueDdeQuoteStrProcess1()", "", e);
                throw;
            }
        }
        public void PushDdeQuoteStr2(string s)
        {
            if (s.HasNoValue())
                return;
            //try
            //{
            //    //var q = ParseQuoteStr(s);
            //    //if (q == null) return;

            //    //if (NewTickEvent != null) NewTickEvent(q.DT, q.Ticker, q.Last, q.Bid, q.Ask);

            DdeQuoteStrQueue2.Push(s);
            //}
            //catch (Exception e)
            //{
            //    SendExceptionMessage3(FullName, "DdeStr", "PushDdeQuoteStr()", s, e);
            //    throw;
            //}
        }
       
        public void DeQueueDdeQuoteStrProcess2()
        {
            if (DdeQuoteStrQueue2.IsEmpty)
                return;

            //Evlm(EvlResult.WARNING, EvlSubject.TECHNOLOGY, FullName, "Repository", "DeQueueProcess", "Try to get Entities to Perform Operation","");

            var items = DdeQuoteStrQueue2.GetItems();
            lock (_lockQuoteProcess)
            {
                foreach (var s in items)
                {
                    ParseQuoteStr2(s);
                }
                UpdateTimeSeries(DateTime.Now);
            }
        }
        public void ParseQuoteStr2(string ss)
        {
            if (ss.HasNoValue())
                return;
            try
            {
                var str = new StringReader(ss);
                string s;
                var i = 0;
                while ((s = str.ReadLine()) != null)
                {
                    var q = ParseQuoteStr(s);
                    if (q == null)
                        continue;
                    if (NewTickEvent != null)
                        NewTickEvent(q.DT, q.Ticker, q.Last, q.Bid, q.Ask);

                    UpdateQuoteOrNew3(q); // Update TickerLast and Async (BarSeries)
                    i++;
                }
                if( i > 1)
                    Evlm(EvlResult.WARNING, EvlSubject.DIAGNOSTIC, "Tickers", "DdeStringList", "ParseDdeStr2()", "ListCount="+i,"");
            }
            catch (Exception e)
            {
                //_eventLog.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "QuoteDdeChannel.OnPoked", "Parse",
                //String.Format("Error in {0} {1}", sError, s), "");
                SendExceptionMessage3("Tickers", "DdeStringList", "ParseQuoteStr2", ss, e);
                throw;
            }
        }

        public void GetDdeQuote2()
        {
            string s;
            Ticker2.Quote q;
            lock (_lockPutQuote)
            {
                var cnt = _quoteStrQueue.Count;
                while (cnt-- > 0)
                {
                    s = _quoteStrQueue.Dequeue();

                    if (s == null) continue;

                    q = ParseQuoteStr(s);
                    UpdateQuoteOrNew3(q);                 
                }
            }
        }
        public void PutDdeQuote3(string ddeStr)
        {
            if (ddeStr.HasNoValue())
                return;

            lock (_lockPutQuote)
            {
                var q = ParseQuoteStr(ddeStr);
                if (q == null) return;

                if (NewTickEvent != null) NewTickEvent(q.DT, q.Ticker, q.Last, q.Bid, q.Ask);

                UpdateQuoteOrNew3(q);
                
               // Evlm2(EvlResult.SUCCESS, "PutQuote", q.ToString());
            }
        }
        public Ticker2.Quote ParseQuoteStr(string s)
        {
            if (s.HasNoValue())
                return null;
            Ticker2.Quote r;
            try
            {
                var split = s.Split(new Char[] { ';' });
                if (split.Length < 8)
                    throw new ArgumentException("s", "Dde String Wrong Format: " + s);

                if (split.Any(sp => sp.HasNoValue()))
                    return null;
                // 
                //string sTicker = (split[0]).Trim();
                //string sD = (split[1]).Trim();
                //string sT = (split[2]).Trim();
                //string sClose = (split[3]).Trim();
                //var sBid = (split[4]).Trim();
                //var sAsk = (split[5]).Trim();
                //var sVol = (split[6]).Trim();

                // WithClassCode
                var sTradeBoard = (split[0]).Trim();
                string sTicker = (split[1]).Trim();
                string sD = (split[2]).Trim();
                string sT = (split[3]).Trim();
                string sClose = (split[4]).Trim();
                var sBid = (split[5]).Trim();
                var sAsk = (split[6]).Trim();
                var sVol = (split[7]).Trim();

                if (    sClose == "0"
                    ||  sBid == "0"
                    ||  sAsk == "0"
                    )
                    return null;

                sTicker = sTradeBoard + "@" + sTicker;

                double close;
                if (! double.TryParse(sClose, out close))
                    return null;

                double bid;
                if (!double.TryParse(sBid, out bid))
                    return null;

                double ask;
                if (!double.TryParse(sAsk, out ask))
                    return null;

                if (    close.IsLessOrEqualsThan(0.0d)
                    ||  ask.IsLessOrEqualsThan(0.0d)
                    ||  bid.IsLessOrEqualsThan(0.0d)
                    )
                    return null;

                double vol;
                if (!double.TryParse(sVol, out vol))
                    return null;
                
                //var vol = double.Parse(sVol);

                DateTime dt;
                if (!DateTime.TryParse(sD + " " + sT, out dt))
                    return null;
                //var dt = DateTime.Parse(sD);
                //var t = TimeSpan.Parse(sT);

              //  var dtt = dt.Add(t);
              //  var mili = dtt.Millisecond;

              //  r = new Ticker.Quote(sTicker, dt.Add(t), close, bid, ask, vol );
                //r = new Ticker.Quote(sTicker, dt, close, bid, ask, vol);
                return new Ticker2.Quote(sTicker, dt, close, bid, ask, vol);
            }
            catch(Exception e)
            {
                //_eventLog.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "QuoteDdeChannel.OnPoked", "Parse",
                //String.Format("Error in {0} {1}", sError, s), "");
                SendExceptionMessage3("Tickers", "DdeString", "ParseQuoteStr", s, e);
                throw;
            }
        }

        private void UpdateQuoteOrNew3(Ticker2.Quote q)
        {
            Ticker2 ticker;
            if ( _tickerDictionary.TryGetValue(q.Ticker, out ticker))
            {
                ticker.Last.Bid = q.Bid;
                ticker.Last.Ask = q.Ask;
                ticker.Last.Last = q.Last;
                ticker.Last.DT = q.DT;
                ticker.Last.Volume = q.Volume;

              //  var m = q.DT.Millisecond;

                // Evlm2(EvlResult.SUCCESS,EvlSubject.PROGRAMMING,"Tickers","NewQuote",ticker.Last.ToString(),"");
                /*
                foreach( var bs in ticker.BarSeriesCollection)
                {
                    bs.Tick( q.DT, q.Last, q.Volume,  _evl );
                }
                */
                /*
                foreach( var s in ticker.TimeSeriesCollection.Values.OfType<Bars>() )
                {
                    s.Tick(DateTime.Now, 0, 0, _evl);
                }
                 */
                var t = new Tick2
                {
                    Ticker = ticker,
                    DT = q.DT,
                    SyncDT = q.DT,
                    LastDT = q.DT,
                    Last = q.Last,
                    Volume = q.Volume
                };
                foreach (var asr in ticker.AsyncSeriesCollection.Values)
                {
                    //asr.Tick(q.DT, q.Last, q.Volume);
                    asr.Update(t);
                }
            }
            else
            {
                // QuoteDictionary.Add(q.Ticker, q);
                // _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Quotes", "New Ticker", q.Ticker, q.ToString());
            }
        }
        public void UpdateAsyncSeries(string tickerKey, IBarSimple b)
        {
            Ticker2 ticker;
            if (!_tickerDictionary.TryGetValue(tickerKey, out ticker)) return;

            ticker.Last.Bid = b.Close;
            ticker.Last.Ask = b.Close;
            ticker.Last.Last = b.Close;
            ticker.Last.DT = b.DT;
            ticker.Last.Volume = b.Volume;


            foreach (var ass in ticker.AsyncSeriesCollection.Values)
            {
                ass.Update((ITimeSeriesItem)b);
            }

        }

        public void UpdateBarSeries(string tickerKey, IBar b)
        {
            Ticker2 ticker;
            if (_tickerDictionary.TryGetValue(tickerKey, out ticker))
            {
                ticker.Last.Bid = b.Close;
                ticker.Last.Ask = b.Close;
                ticker.Last.Last = b.Close;
                ticker.Last.DT = b.DT;
                ticker.Last.Volume = b.Volume;

                // Evlm2(EvlResult.SUCCESS,EvlSubject.PROGRAMMING,"Tickers","NewQuote",ticker.Last.ToString(),"");

                foreach (var bs in ticker.BarSeriesCollection)
                {
                    bs.Update((ITimeSeriesItem)b);
                }
                /*
                foreach( var s in ticker.TimeSeriesCollection.Values.OfType<Bars>() )
                {
                    s.Tick(DateTime.Now, 0, 0, _evl);
                }
                 */
            }
            else
            {
                // QuoteDictionary.Add(q.Ticker, q);
                // _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Quotes", "New Ticker", q.Ticker, q.ToString());
            }
        }
        
        public void UpdateTimeSeries(DateTime syncDT)
        {
            foreach (ITimeSeries ts in
                from Ticker2 t in TickerCollection from ts in t.TimeSeriesCollection.Values select ts)
            {
                ts.Update(syncDT);
            }
        }
        /*
        public void UpdateTimeSeriesBar(DateTime syncDT)
        {
            foreach (TimeSeries ts in
                from Ticker t in TickerCollection from ts in t.TimeSeriesCollection.Values select ts)
            {
                ts.UpdateBar(syncDT);
            }
        }
        */
        public bool SerializeTickers()
        {
            string xmlfname = null;
            TextWriter tr = null;
            try
            {
                var xDoc = XDocument.Load(@"SpsInit.xml");
                var xe = xDoc.Descendants("Tickers_XmlFileName").First();
                xmlfname = xe.Value;

                var tl = _tickerDictionary.Values.ToList();

                tr = new StreamWriter(xmlfname);
                // var sr = new XmlSerializer(typeof(Dictionary<string,Ticker>));  // !!! Not Support !!!!!
                var sr = new XmlSerializer(typeof(List<Ticker2>));
                sr.Serialize(tr, tl);
                tr.Close();

                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Tickers", "Tickers", "Serialization",
                String.Format("FileName={0}", xmlfname), "Count=" + _tickerDictionary.Count);

                return true;
            }
            catch (Exception e)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Tickers", "Tickers", "Serialization",
                String.Format("FileName={0}", xmlfname), e.ToString() );

                if( tr != null) tr.Close();
                return false;
            }
        }
        public bool DeserializeTickers()
        {
            string xmlfname = null;
            try
            {
                var xDoc = XDocument.Load(@"SpsInit.xml");
                
                var xe = xDoc.Descendants("Tickers_XmlFileName").First();
                xmlfname = xe.Value;

                var x = XElement.Load(xmlfname);
                
                var tl = Serialization.Do.DeSerialize<List<Ticker2>>(x, (s1, s2) =>
                              Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Tickers",
                                    "Tickers", s1, String.Format("FileName={0}", xmlfname), s2));
              
                _tickerDictionary.Clear();
                foreach (var t in tl)
                {
                    if (!CheckTicker(t)) continue;
                    t.Tickers = this;
                    t.TradeBoard = t.ClassCode;
                    t.SetEventLog(EventLog);
                    AddTicker(t);
                }
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Tickers", "Tickers", "DeSerialization",
                String.Format("FileName={0}", xmlfname), "Count=" + _tickerDictionary.Count);
                /*
                foreach( var t in _tickerDictionary.Values)
                {
                    t.InitAsync();
                }
                 */
            }
            catch ( Exception e)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Tickers", "Tickers", "DeSerialization",
                String.Format("FileName={0}", xmlfname), e.ToString());

                throw new SerializationException("Tickers.Deserialization Failure " + xmlfname);
            }
            return true;
        }
        private bool CheckTicker(Ticker2 t)
        {
            var decStr = t.Decimals.ToString();
            var fn = "N" + decStr;
            var ff = "F" + decStr;

            if( fn != t.Format || 
                ff != t.FormatF)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Tickers", "Tickers", "DeSerialization",
                "Ticker: " + t.Code + " FormatString is Invalid","");
                return false;
            }
            //throw new NullReferenceException("Ticker: " + t.Code + " FormatString is Invalid");
            return true;
        }

        public void ClearSomeData(int count)
        {
            foreach (var t in TickerCollection)
            {
                t.ClearSomeData(count);
            }
        }
    }

}
