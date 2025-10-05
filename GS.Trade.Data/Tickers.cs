// #define StringQuotes
 #define ListOfStringQuotes

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using GS.ConsoleAS;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;
using GS.Queues;
using GS.Serialization;
using GS.Trade.Data.Options;
//using GS.Trade.DB.Q;
using GS.Trade.Data.Studies;
using GS.Trade.Dto;
using Moex;
using GS.Moex.Interfaces;
using WebClients;
using Bar = GS.Trade.Data.Bars.Bar;


namespace GS.Trade.Data
{
    public partial class Tickers : Element1<string>, ITickers //, IHaveQueue<string>
    {
        protected CultureInfo CultureInfoProvider { get; private set; }

        //public delegate void NewTickEventHandler(DateTime dt, string tickerkey, double last, double bid, double ask);
        public event NewTickEventHandler NewTickEvent;

        //[XmlIgnore]
        //private IEventLog _evl;
        protected QueueFifo<Ticker.Quote> DdeQuoteStrQueue;
        protected QueueFifo<string> DdeQuoteStrQueue2;
        protected QueueFifo<IEnumerable<string>> DdeListOfQuoteStrQueue;
        private readonly Dictionary<string, Ticker> _tickerDictionary;
        private readonly Queue<string> _quoteStrQueue;
        //private readonly QuoteDataBase _barsDataBase = new QuoteDataBase();

        private BarWebClient _barWebClient;
        private TimeSeriesWebClient01 _timeSeriesWebClient;

        //public List<Ticker> _tickerList;

        public IEnumerable<ITicker> TickerCollection => _tickerDictionary.Values;
        //public ICollection<ITicker> GetTickers { get { return _tickerDictionary.Values.ToList() as ICollection<ITicker>; } }
        //public IEventLog EventLog { get { return EventLog; } } 

        private readonly object _lockPutQuote;
        private readonly object _lockAddTicker;
        private readonly object _lockQuoteProcess;

        public override string Key => Code;

        public Tickers()
        {
            Code = "Tickers.Core.Storage";
            Name = "Tickers Core.Storage";

            _tickerDictionary = new Dictionary<string, Ticker>();
            _quoteStrQueue = new Queue<string>();

            DdeQuoteStrQueue = new QueueFifo<Ticker.Quote>();
            DdeQuoteStrQueue2 = new QueueFifo<string>();

            _lockPutQuote = new object();
            _lockAddTicker = new object();
            _lockQuoteProcess = new object();

            CultureInfoProvider = CultureInfo.InvariantCulture;

        }

        public override void Init(IEventLog evl)
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

            _barWebClient = Builder.Build<BarWebClient>(@"Init\WebClients.xml", "BarWebClient");
            if (_barWebClient == null)
                throw new NullReferenceException("BarWeb After Build is null");
            _barWebClient.Init();
            _barWebClient.Parent = this;

            _timeSeriesWebClient = new TimeSeriesWebClient01 {Parent = this};
            _timeSeriesWebClient.Init(EventLog);

            SetupProcessTask();


            Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, Code, Code, "Initialization()", "Complete", "");
        }

        public void Load()
        {
            DeserializeTickers();
        }

        public void LoadBarsFromArch()
        {
            foreach (var t in _tickerDictionary.Values)
            {
                // if( t.IsNeedLoadFromDataBase)

                if (t.LoadMode > 0)
                    t.LoadBarsFromArch();
            }
            /*
            foreach (var t in _tickerDictionary.Values)
            {
                // if( t.IsNeedLoadFromDataBase)
                if (t.LoadMode > 0)
                    t.UpToDate();
            }
            */
        }

        public ITicker CreateInstance(ITickerBase t)
        {
            return new Ticker
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

                LoadMode = 2
            };
        }

        public ITicker CreateInstanceFromMoex(IMoexTicker t)
        {
            if (t.IsFutures)
            {
                return new Ticker
                {
                    Code = t.SecId,
                    Name = t.SecName,
                    Alias = t.ShortName,
                    ShortName = t.ShortName,

                    TickerTradeType = TickerTradeTypeEnum.Futures,

                    ClassCode = "SPBFUT",
                    TradeBoard = "SPBFUT",

                    //TradeBoard = t.BoardId,
                    BaseContract = t.AssetCode,
                    Symbol = t.BoardId,

                    Decimals = t.Decimals,
                    Margin = (decimal) t.InitialMargin,
                    MinMove = (float) t.MinStep,
                    From = t.FirstTradeDate,
                    To = t.LastTradeDate,

                    LoadMode = 2 // for DownLoad Bars
                };
            }
            if (t.IsOption)
            {
                return new OptionTicker
                {
                    Code = t.SecId,
                    Name = t.SecName,
                    Alias = t.ShortName,
                    ShortName = t.ShortName,

                    TickerTradeType = TickerTradeTypeEnum.Option,

                    ClassCode = "SPBOPT",
                    TradeBoard = "SPBOPT",

                    //TradeBoard = t.BoardId,
                    BaseContract = t.AssetCode,
                    Symbol = t.BoardId,

                    Decimals = t.Decimals,
                    Margin = (decimal) t.InitialMargin,
                    MinMove = (float) t.MinStep,
                    From = t.FirstTradeDate,
                    To = t.LastTradeDate,

                    LoadMode = 2 // for DownLoad Bars
                };
            }
            return null;
        }

        public ITicker CreateInstance1(IMoexTicker t)
        {
            return new Ticker
            {
                Code = t.SecId,
                Name = t.SecName,
                Alias = t.ShortName,
                ShortName = t.ShortName,

                ClassCode = t.IsFutures ? "SPBFUT" : (t.IsOption ? "SPBOPT" : "EQBR"),
                TradeBoard = t.IsFutures ? "SPBFUT" : (t.IsOption ? "SPBOPT" : "EQBR"),
                TickerTradeType = t.IsFutures
                    ? TickerTradeTypeEnum.Futures
                    : (t.IsOption ? TickerTradeTypeEnum.Option : TickerTradeTypeEnum.Stock),

                //TradeBoard = t.BoardId,
                BaseContract = t.AssetCode,
                Symbol = t.BoardId,

                Decimals = t.Decimals,
                Margin = (decimal) t.InitialMargin,
                MinMove = (float) t.MinStep,
                From = t.FirstTradeDate,
                To = t.LastTradeDate,

                LoadMode = 2 // for DownLoad Bars
            };
        }

        public ITicker Register(ITicker ti)
        {
            var t = GetTicker(ti.Key);
            if (t != null)
                return t;
            ti.Parent = this;
            ti.Tickers = this;
            ti.TradeBoard = ti.ClassCode;
            ti.EventLog = EventLog;
            AddTicker((Ticker) ti);
            return ti;
        }

        public void Add(ITicker t)
        {
            var key = t.Key.Trim();
            lock (_lockAddTicker)
            {
                if (_tickerDictionary.ContainsKey(key)) return;

                t.Tickers = this;
                t.TradeBoard = t.ClassCode;
                t.EventLog = EventLog;
                _tickerDictionary.Add(key, (Ticker) t);

                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName, "Ticker",
                    "AddNew: " + key.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                    "Count=" + _tickerDictionary.Count, t.ToString());
            }
        }

        public void AddTicker(ITicker t)
        {
            var res = false;
            var key = t.Key.Trim();
            lock (_lockAddTicker)
            {
                if (!_tickerDictionary.ContainsKey(key))
                {
                    _tickerDictionary.Add(key, (Ticker) t);
                    res = true;
                }
            }
            if (res)
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName, key, "AddNew " + key,
                    t.ToString(), "Count=" + _tickerDictionary.Count);
            // else
            //    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Tickers", "Add New " + key,
            //                     "Ticker Already Exist", t.ToString());
        }

        public ITicker GetTicker(string code)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                Ticker t;
                lock (_lockAddTicker)
                {
                    var boo = _tickerDictionary.TryGetValue(code.Trim(), out t);
                    // var t = _tickerDictionary.OfType<ITicker>().FirstOrDefault(tk=>tk.Code == code.Trim());
                    if (boo)
                        return t;
                    t =
                        _tickerDictionary.Values.ToList()
                            .FirstOrDefault(ac => ac.Code.HasValue() && ac.Code.Trim() == code.Trim()) ??
                        _tickerDictionary.Values.ToList()
                            .FirstOrDefault(ac => ac.Alias.HasValue() && ac.Alias.Trim() == code.Trim());
                }
                if (t != null)
                    return t;

                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m, $"Ticker: {code} not Found",
                    "");
                // return null;
            }
            catch (Exception e)
            {
                //SendExceptionMessage3(FullName, "Ticker", "GetTicker()", code, e);
                //throw;
                SendException(e);
            }
            return null;
        }

        public ITicker GetTickerByKey(string key)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            if (string.IsNullOrWhiteSpace(key))
                return null;
            try
            {
                lock (_lockAddTicker)
                {
                    var boo = _tickerDictionary.TryGetValue(key.Trim(), out var t);
                    if (boo)
                        return t;
                }
                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m,
                    $"Ticker: {key} not Found", "");
                // return null;
            }
            catch (Exception e)
            {
                // SendExceptionMessage3(FullName, "Ticker", "GetTickerByKey()", key, e);
                // throw;
                SendException(e);
            }
            return null;
        }

        public ITicker this[string index] => _tickerDictionary.Keys.Contains(index.Trim().ToUpper())
            ? _tickerDictionary[index]
            : null;

        public IEnumerable<ITicker> GetTickers => _tickerDictionary.Values;

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

                NewTickEvent?.Invoke(q.DT, q.Ticker, q.Last, q.Bid, q.Ask);

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
#if StringQuotes
            if (IsProcessTaskInUse)
                ProcessTask?.EnQueue(s);
            else
#endif
            DdeQuoteStrQueue2.Push(s);
        }

        public void PushDdeQuoteListStr(List<string> list)
        {
            if (!list.Any())
                return;
#if ListOfStringQuotes

            if (IsProcessTaskInUse)
                ProcessTask?.EnQueue(list);
            else
#endif
                DdeListOfQuoteStrQueue.Push(list);
        }

        // Current version of Quotes Receiving
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

                    NewTickEvent?.Invoke(q.DT, q.Ticker, q.Last, q.Bid, q.Ask);

                    UpdateQuoteOrNew3(q);
                }
                UpdateTimeSeries(DateTime.Now);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "DdeStr", "DeQueueDdeQuoteStrProcess1()", "", e);
                throw;
            }
        }

        public void QuoteStringsProcessing(IEnumerable<string> quotestrings)
        {
            try
            {
                foreach (var s in quotestrings)
                {
                    var q = ParseQuoteStr(s);
                    if (q == null)
                        continue;

                    NewTickEvent?.Invoke(q.DT, q.Ticker, q.Last, q.Bid, q.Ask);

                    UpdateQuoteOrNew3(q);
                }
                UpdateTimeSeries(DateTime.Now);
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }

        // Dde can to push IEnumerable<List<string>> quotes string;
        // Every one List<quotes> contains updates in one Table at the on time moment
        // If we have some updates in second, then we have two or more Lists of quotes. 
        public void QuoteListsOfStringsProcessing(IEnumerable<IEnumerable<string>> listsOfstringsQoutes)
        {
            try
            {
                foreach (var list in listsOfstringsQoutes)
                {
                    foreach (var s in list)
                    {
                        var q = ParseQuoteStr(s);
                        if (q == null)
                            continue;

                        NewTickEvent?.Invoke(q.DT, q.Ticker, q.Last, q.Bid, q.Ask);

                        UpdateQuoteOrNew3(q);
                    }
                    //UpdateTimeSeries(DateTime.Now);
                }
                UpdateTimeSeries(DateTime.Now);
            }
            catch (Exception e)
            {
                SendException(e);
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
                    NewTickEvent?.Invoke(q.DT, q.Ticker, q.Last, q.Bid, q.Ask);

                    UpdateQuoteOrNew3(q); // Update TickerLast and Async (BarSeries)
                    i++;
                }
                if (i > 1)
                    Evlm(EvlResult.WARNING, EvlSubject.DIAGNOSTIC, "Tickers", "DdeStringList", "ParseDdeStr2()",
                        "ListCount=" + i, "");
            }
            catch (Exception e)
            {
                //_eventLog.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "QuoteDdeChannel.OnPoked", "Parse",
                //String.Format("Error in {0} {1}", sError, s), "");
                //SendExceptionMessage3("Tickers", "DdeStringList", "ParseQuoteStr2", ss, e);
                //throw;
                SendException(e);
            }
        }

        public void GetDdeQuote2()
        {
            string s;
            Ticker.Quote q;
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

                NewTickEvent?.Invoke(q.DT, q.Ticker, q.Last, q.Bid, q.Ask);

                UpdateQuoteOrNew3(q);

                // Evlm2(EvlResult.SUCCESS, "PutQuote", q.ToString());
            }
        }

        public void PutDdeQuote5(string ddeStr)
        {
            if (ddeStr.HasNoValue())
                return;

            lock (_lockPutQuote)
            {
                var q = ParseQuoteStr5(ddeStr);
                if (q == null) return;

                NewTickEvent?.Invoke(q.DT, q.Ticker, q.Last, q.Bid, q.Ask);

                UpdateQuoteOrNew3(q);

                // Evlm2(EvlResult.SUCCESS, "PutQuote", q.ToString());
            }
        }
        // 2021.04.06 DoubleInvariantParse
        public Ticker.Quote ParseQuoteStr(string s)
        {
            if (s.HasNoValue())
                return null;
            Ticker.Quote r;
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

                if (sClose == "0"
                    || sBid == "0"
                    || sAsk == "0"
                    )
                    return null;

                sTicker = sTradeBoard + "@" + sTicker;

                var close = sClose.DoubleInvariantParse();
                double bid = sBid.DoubleInvariantParse();
                double ask = sAsk.DoubleInvariantParse();

                if (close.IsLessOrEqualsThan(0.0d)
                    || ask.IsLessOrEqualsThan(0.0d)
                    || bid.IsLessOrEqualsThan(0.0d)
                    )
                    return null;

                double vol = sVol.DoubleInvariantParse();

                if (!DateTime.TryParse(sD + " " + sT, out var dt))
                {
                    SendExceptionMessage3("Tickers.ParseQuoteStr()", "DateTime", "Parse", s,
                        new Exception("DateTime ParseError"));
                    return null;
                }
                return new Ticker.Quote(sTicker, dt, close, bid, ask, vol);
            }
            catch (Exception e)
            {
                //_eventLog.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "QuoteDdeChannel.OnPoked", "Parse",
                //String.Format("Error in {0} {1}", sError, s), "");
                SendExceptionMessage3("Tickers", "DdeString", "ParseQuoteStr", s, e);
                // 2018.05.13
                // throw
                return null;
            }
        }
        // Old
        public Ticker.Quote ParseQuoteStr1(string s)
        {
            if (s.HasNoValue())
                return null;
            Ticker.Quote r;
            try
            {
                var split = s.Split(new Char[] {';'});
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

                if (sClose == "0"
                    || sBid == "0"
                    || sAsk == "0"
                    )
                    return null;

                sTicker = sTradeBoard + "@" + sTicker;

                if (!double.TryParse(sClose, out var close))
                    return null;

                if (!double.TryParse(sBid, out var bid))
                    return null;

                if (!double.TryParse(sAsk, out var ask))
                    return null;

                if (close.IsLessOrEqualsThan(0.0d)
                    || ask.IsLessOrEqualsThan(0.0d)
                    || bid.IsLessOrEqualsThan(0.0d)
                    )
                    return null;

                if (!double.TryParse(sVol, out var vol))
                    return null;

                //var vol = double.Parse(sVol);

                if (!DateTime.TryParse(sD + " " + sT, out var dt))
                {
                    SendExceptionMessage3("Tickers.ParseQuoteStr()", "DateTime", "Parse", s,
                        new Exception("DateTime ParseError"));
                    return null;
                }
                //var dt = DateTime.Parse(sD);
                //var t = TimeSpan.Parse(sT);

                //  var dtt = dt.Add(t);
                //  var mili = dtt.Millisecond;

                //  r = new Ticker.Quote(sTicker, dt.Add(t), close, bid, ask, vol );
                //r = new Ticker.Quote(sTicker, dt, close, bid, ask, vol);
                return new Ticker.Quote(sTicker, dt, close, bid, ask, vol);
            }
            catch (Exception e)
            {
                //_eventLog.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "QuoteDdeChannel.OnPoked", "Parse",
                //String.Format("Error in {0} {1}", sError, s), "");
                SendExceptionMessage3("Tickers", "DdeString", "ParseQuoteStr", s, e);
                // 2018.05.13
                // throw
                return null;
            }
        }

        public OptionDeskItem ParseOptionDeskItemStr(string s)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            if (s.HasNoValue())
                return null;
            string[] split = {};
            OptionDeskItemParseFieldPlaceEnum lastSplitIndexToProcessing = 0;
            try
            {
                split = s.Split(';');
                if (split.Any(sp => sp.HasNoValue()))
                    return null;
                // OptionDeskItemParseFieldPlaceEnum.PutCode - MAX Split Index in Input string
                if (split.Length != (int) OptionDeskItemParseFieldPlaceEnum.PutCode + 1)
                    return null;

                var optiondeskitem = new OptionDeskItem();

                var call = new CallOptionInfo {OptionType = OptionTypeEnum.Call};

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.ExpirationDate;
                call.ExpirationDate = DateTime
                    .ParseExact(split[(int) lastSplitIndexToProcessing], "dd.MM.yyyy", CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.BaseAssetPrice;
                call.BaseAssetPrice = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.Volatility;
                call.Volatility = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallCode;
                call.Code = split[(int) lastSplitIndexToProcessing];

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallDelta;
                call.Delta = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallGamma;
                call.Gamma = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallTetta;
                call.Theta = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallVega;
                call.Vega = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallPo;
                call.Rho = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallBid;
                call.Bid = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallOffer;
                call.Offer = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallTheoryPrice;
                call.TheoryPrice = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallOpenInerest;
                call.OpenInterest = long.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallTradesAmount;
                call.TradesCount = int.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.Strike;
                call.Strike = int.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                optiondeskitem.CallInfo = call;

                var put = new PutOptionInfo {OptionType = OptionTypeEnum.Put};

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.ExpirationDate;
                put.ExpirationDate = DateTime
                    .ParseExact(split[(int) lastSplitIndexToProcessing], "dd.MM.yyyy", CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.BaseAssetPrice;
                put.BaseAssetPrice = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.Volatility;
                put.Volatility = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutCode;
                put.Code = split[(int) lastSplitIndexToProcessing];

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutDelta;
                put.Delta = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutGamma;
                put.Gamma = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutTetta;
                put.Theta = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutVega;
                put.Vega = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutPo;
                put.Rho = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutBid;
                put.Bid = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutOffer;
                put.Offer = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutTheoryPrice;
                put.TheoryPrice = double.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutOpenInerest;
                put.OpenInterest = long.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutTradesAmount;
                put.TradesCount = int.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.Strike;
                put.Strike = int.Parse(split[(int) lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                optiondeskitem.PutInfo = put;

                return optiondeskitem;
            }
            catch (Exception e)
            {
                SendException(e);
                var invalidcontent = (int) lastSplitIndexToProcessing < split.Length
                    ? split[(int) lastSplitIndexToProcessing]
                    : "Unknown";
                Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, e.GetType().Name, $"{TypeName}.{m}",
                    $"Failure in Parse input string on SplitIndex:{lastSplitIndexToProcessing} InvalidValue:{invalidcontent}",
                    $"{e.Message} Str: {s}");
            }
            return null;
        }

        public Ticker.Quote ParseQuoteStr5(string s)
        {
            if (s.HasNoValue())
                return null;
            Ticker.Quote r;
            try
            {
                var split = s.Split(new Char[] {';'});
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

                if (sClose == "0"
                    || sBid == "0"
                    || sAsk == "0"
                    )
                    return null;

                sTicker = sTradeBoard + "@" + sTicker;

                if (!double.TryParse(sClose, out var close))
                    return null;

                if (!double.TryParse(sBid, out var bid))
                    return null;

                if (!double.TryParse(sAsk, out var ask))
                    return null;

                if (close.IsLessOrEqualsThan(0.0d)
                    || ask.IsLessOrEqualsThan(0.0d)
                    || bid.IsLessOrEqualsThan(0.0d)
                    )
                    return null;

                if (!double.TryParse(sVol, out var vol))
                    return null;

                //var vol = double.Parse(sVol);

                DateTime dt;
                //if (!DateTime.TryParse(sD + " " + sT, out dt))
                //    return null;
                const string format = "dd.MM.yyyy H:mm:ss.fff";
                try
                {
                    dt = DateTime.ParseExact(sD + " " + sT, format, CultureInfoProvider);
                }
                catch (Exception e)
                {
                    SendExceptionMessage3("Tickers.ParseQuoteStr5()", "DateTime", "Parse", s, e);
                    return null;
                }
                //var dt = DateTime.Parse(sD);
                //var t = TimeSpan.Parse(sT);

                //  var dtt = dt.Add(t);
                //  var mili = dtt.Millisecond;

                //  r = new Ticker.Quote(sTicker, dt.Add(t), close, bid, ask, vol );
                //r = new Ticker.Quote(sTicker, dt, close, bid, ask, vol);
                return new Ticker.Quote(sTicker, dt, close, bid, ask, vol);
            }
            catch (Exception e)
            {
                //_eventLog.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "QuoteDdeChannel.OnPoked", "Parse",
                //String.Format("Error in {0} {1}", sError, s), "");
                SendExceptionMessage3("Tickers", "DdeString", "ParseQuoteStr", s, e);
                throw;
            }
        }

        private void UpdateQuoteOrNew3(Ticker.Quote q)
        {
            if (_tickerDictionary.TryGetValue(q.Ticker, out var ticker))
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
                var t = new Tick
                {
                    Ticker = ticker,
                    DT = q.DT,
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
            if (!_tickerDictionary.TryGetValue(tickerKey, out var ticker)) return;

            ticker.Last.Bid = b.Close;
            ticker.Last.Ask = b.Close;
            ticker.Last.Last = b.Close;
            ticker.Last.DT = b.DT;
            ticker.Last.Volume = b.Volume;

            // Evlm2(EvlResult.SUCCESS,EvlSubject.PROGRAMMING,"Tickers","NewQuote",ticker.Last.ToString(),"");

            foreach (var ass in ticker.AsyncSeriesCollection.Values)
            {
                ass.Update((TimeSeriesItem) b);
            }
            /*
                foreach( var s in ticker.TimeSeriesCollection.Values.OfType<Bars>() )
                {
                    s.Tick(DateTime.Now, 0, 0, _evl);
                }
                 */
        }

        public void UpdateBarSeries(string tickerKey, Bar b)
        {
            if (_tickerDictionary.TryGetValue(tickerKey, out var ticker))
            {
                ticker.Last.Bid = b.Close;
                ticker.Last.Ask = b.Close;
                ticker.Last.Last = b.Close;
                ticker.Last.DT = b.LastDT;
                ticker.Last.Volume = b.Volume;

                // Evlm2(EvlResult.SUCCESS,EvlSubject.PROGRAMMING,"Tickers","NewQuote",ticker.Last.ToString(),"");

                foreach (var bs in ticker.BarSeriesCollection)
                {
                    bs.Update(b);
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
            foreach (TimeSeries ts in
                from Ticker t in TickerCollection from ts in t.TimeSeriesCollection.Values select ts)
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
                var sr = new XmlSerializer(typeof (List<Ticker>));
                sr.Serialize(tr, tl);
                tr.Close();

                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Tickers", "Tickers", "Serialization",
                    String.Format("FileName={0}", xmlfname), "Count=" + _tickerDictionary.Count);

                return true;
            }
            catch (Exception e)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Tickers", "Tickers", "Serialization",
                    String.Format("FileName={0}", xmlfname), e.ToString());

                if (tr != null) tr.Close();
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

                var tl = Serialization.Do.DeSerialize<List<Ticker>>(x, (s1, s2) =>
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Tickers",
                        "Tickers", s1, $"FileName={xmlfname}", s2));

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
                    $"FileName={xmlfname}", "Count=" + _tickerDictionary.Count);
                /*
                foreach( var t in _tickerDictionary.Values)
                {
                    t.InitAsync();
                }
                 */
            }
            catch (Exception e)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Tickers", "Tickers", "DeSerialization",
                    $"FileName={xmlfname}", e.ToString());

                throw new SerializationException("Tickers.Deserialization Failure " + xmlfname);
            }
            return true;
        }

        private bool CheckTicker(Ticker t)
        {
            var decStr = t.Decimals.ToString();
            var fn = "N" + decStr;
            var ff = "F" + decStr;

            if (fn != t.Format ||
                ff != t.FormatF)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Tickers", "Tickers", "DeSerialization",
                    "Ticker: " + t.Code + " FormatString is Invalid", "");
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

        public IEnumerable<IBarSimple> GetSeries(string queryString)
        {
            return null;
        }

        public IEnumerable<IBarSimple> GetSeries(string ticker, int timeInt)
        {
            // return _barWebClient.GetSeries(ticker, timeInt);
            return _timeSeriesWebClient?.GetSeries(ticker, timeInt);

        }

        public IEnumerable<IBarSimple> GetSeries(string ticker, int timeInt, DateTime dt)
        {
            // return _barWebClient.GetSeries(ticker, timeInt, dt);

            // 21.07.2017
            //return _timeSeriesWebClient != null
            //    ? _timeSeriesWebClient.GetSeries(ticker, timeInt, dt)
            //    : null;

            try
            {
                var resp = _timeSeriesWebClient?.GetSeries(ticker, timeInt, dt);
                if (resp == null)
                {
                    //throw new Exception(
                    //    _timeSeriesWebClient?.HttpReasonPhrase + Environment.NewLine +
                    //    _timeSeriesWebClient?.ErrorMessage);
                    var e = new Exception(
                        _timeSeriesWebClient?.HttpReasonPhrase + Environment.NewLine +
                        _timeSeriesWebClient?.ErrorMessage);
                    // TODO Exception Handling
                }
                return resp;
            }
            catch (Exception ex)
            {
                SendException(ex);
            }
            return null;
        }

        public IEnumerable<IBarSimple> GetSeries(long seriesId, DateTime dt)
        {
            var m = MethodBase.GetCurrentMethod() + "()";
            // return _barWebClient.GetSeries(seriesId, dt);
            var r = _barWebClient.GetSeries(seriesId, dt);
            if (r == null)
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                    m, $"Failure in Get SeriesId:{seriesId}, DT:{dt.Date:d}", ToString());
            return r;
        }
        public TimeSeriesStat GetTimeSeriesStat(string ticker, int timeInt)
        {
            var m = MethodBase.GetCurrentMethod() + "()";
            var r = _timeSeriesWebClient?.GetTimeSeriesStat(ticker, timeInt);
            if(r == null)
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                    m, $"Failure in Get Ticker:{ticker}, TimeInt:{timeInt}", ToString());
            return r;
        }
    }
}
