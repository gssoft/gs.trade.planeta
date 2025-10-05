using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Elements;
using GS.Interfaces;
using GS.Extension;


namespace GS.Trade.Tickers
{
    public class Ticker2 : ITicker
    {
        [XmlIgnore]
        public IEventLog EventLog { get;  set; }

        [XmlIgnore]
        public ITickers Tickers { get; set; }

        [XmlIgnore]
        public ITimeSeries PrimarySeries { get; private set; }

        public int Id { get; set; }

        public string Code { get; set; }
        public string TradeBoard { get; set; }
        //public string TradeBoard { get { return ClassCode; }}

        public string ClassCode { get; set; }
        public string BaseContract { get; set; }

        public string Name { get; set; }

        public string Key
        {
            get { return (TradeBoard.HasValue() ? TradeBoard + "@" : "") + Code; }
        }
        public string Symbol { get; set; }
        public string Alias { get; set; }

        public float MinMove { get; set; }
        public int Decimals { get; set; }
        public decimal Margin { get; set; }
        public float PriceLimit { get; set; }

        private string _format;
        public string Format
        {
            get
            {
                if (_format.HasValue())
                    return _format;
                _format = "N" + Decimals;
                return _format;
            }
        }
        private string _formatAvg;
        public string FormatAvg
        {
            get
            {
                if (_formatAvg.HasValue())
                    return _formatAvg;
                _formatAvg = "N" + (Decimals + 2);
                return _formatAvg;
            }
        }

        public string FormatM { get { return "N2"; } }
        public string FormMAvg { get { return "N4"; } }

        private string _formatF;
        public string FormatF
        {
            get
            {
                if (_formatF.HasValue())
                    return _formatF;
                _formatF = "F" + Decimals;
                return _formatF;
            }
        }

        public double MarketPricePrcnt { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public bool IsNeedLoadFromDataBase { get; set; }
        public int LoadMode { get; set; }

        //private FinamDownLoader _finamDownLoader;
        
        [XmlIgnore]
        public Quote Last { get; set; }

       // [XmlIgnore]
        //public List<BarSeries> BarSeriesCollection;
        [XmlIgnore]
        public Dictionary<string, ITimeSeries> AsyncSeriesCollection;
        [XmlIgnore]
        public List<ITimeSeries> BarSeriesCollection;

        [XmlIgnore]
        public IEnumerable<ITimeSeries> AsyncSeriesCollectionValues {get { return AsyncSeriesCollection.Values; }}
        [XmlIgnore]
        public Dictionary<string, ITimeSeries> TimeSeriesCollection;

        private readonly object _lockAddSeries = new object();

        public double MarketPriceBuy { get { return MinMove * Math.Ceiling((Last.Last * (1.0 + MarketPricePrcnt)) / MinMove); } }
        public double MarketPriceSell { get { return MinMove * Math.Floor((Last.Last * (1.0 - MarketPricePrcnt)) / MinMove); } }

        public string MarketPriceBuyStr { get { return MarketPriceBuy.ToString(Format); } } //
        public string MarketPriceSellStr { get { return MarketPriceSell.ToString(Format); } } // for visualization in well format

        public double LastPrice { get { return Last.Last; }}
        public double Bid { get { return Last.Bid; } }
        public double Ask { get { return Last.Ask; } }

        public double BestBid { get { return Last.Bid + MinMove; } }
        public double BestAsk { get { return Last.Ask - MinMove; } }

        public Ticker2()
        {
            Last = new Quote("", DateTime.Now, 0, 0);
            AsyncSeriesCollection = new Dictionary<string, ITimeSeries>();
            BarSeriesCollection = new List<ITimeSeries>();
            TimeSeriesCollection = new Dictionary<string, ITimeSeries>();
            //MarketPricePrcnt = 0.5 / 100;
            MarketPricePrcnt = 0.5 * 0.01;
        }
        public void SetEventLog(IEventLog evl)
        {
            if(evl == null) throw new NullReferenceException("TickerCode=" + Code + "; EventLog == null");
            EventLog = evl;
        }

        public Ticker2(int id, string name, string code, string classcode, string basecontract, string key, string symbol, string format,
            DateTime from, DateTime to)
        {
            Id = id;
            Name = name;
            Code = code;
            ClassCode = classcode;
            BaseContract = basecontract;
            //Key = key;
            Symbol = symbol;
            //Format = format;
            From = from;
            To = to;

            MarketPricePrcnt = 0.5 / 100;

            Last = new Quote(Code, DateTime.Now, 0, 0);

            BarSeriesCollection = new List<ITimeSeries>();
        }
        public void Tick(DateTime dt, double last)
        {
            Last.DT = dt;
            Last.Last = last;
        }
        public void Tick(DateTime dt, double bid, double ask, double last)
        {
            Last.DT = dt;
            Last.Bid = bid;
            Last.Ask = ask;
            //Last.Bid = last;
            //Last.Ask = last;
            Last.Last = last;
        }

        public void AddBarSeries(ITimeSeries bs)
        {
            lock( _lockAddSeries)
            {
                BarSeriesCollection.Add(bs);
            }
        }
        public void AddSeries(ITimeSeries ts, IDictionary<string,ITimeSeries> tss)
        {
            lock (_lockAddSeries)
            {
                tss.Add(ts.Key.Trim().ToUpper(), ts);
            }
        }

        public void RegisterBarSeries(string name, int timeInt, int shift)
        {
            var s = BarSeriesCollection.FirstOrDefault(
                bs => bs.Name == name && bs.TimeIntSeconds == timeInt && bs.ShiftIntSecond == shift);
            if (s != null)
                return;
            //AddBarSeries(
            //    new BarSeries(0, name, this, timeInt, shift)
            //    );
        }

        //public TimeSeries RegisterTimeSeries( TimeSeries ts)
        //{
        //    /*
        //    TimeSeries myts;
        //    var key = ts.Key.Trim().ToUpper();
        //    if (!TimeSeriesCollection.TryGetValue(key, out myts))
        //    {
        //        ts.SetEventLog(EventLog);
        //        TimeSeriesCollection.Add(key, ts);
        //        return ts;
        //    }
        //    return myts;
        //     */
        //    return ts == null ? ts : RegisterSeries(ts, TimeSeriesCollection);
        //}
        public ITimeSeries RegisterTimeSeries(ITimeSeries ts)
        {
            /*
            TimeSeries myts;
            var key = ts.Key.Trim().ToUpper();
            if (!TimeSeriesCollection.TryGetValue(key, out myts))
            {
                ts.SetEventLog(EventLog);
                TimeSeriesCollection.Add(key, ts);
                return ts;
            }
            return myts;
             */
            return ts == null ? null : RegisterSeries(ts, TimeSeriesCollection);
        }

        public ITimeSeries RegisterAsyncSeries(ITimeSeries ts)
        {
            return RegisterSeries(ts, AsyncSeriesCollection);
        }
        private ITimeSeries RegisterSeries(ITimeSeries ts, IDictionary<string, ITimeSeries> tss)
        {
            ITimeSeries myts;
                var key = ts.Key.Trim().ToUpper();
                if (!tss.TryGetValue(key, out myts))
                {
                    ts.SetEventLog(EventLog);
                    ts.Init();

                    AddSeries(ts, tss);
                    return ts;
                }
            return myts;
        }

        public ITimeSeries GetTimeSeries( ITimeSeries ts)
        {
            ITimeSeries myts;
            return TimeSeriesCollection.TryGetValue(ts.Key.Trim().ToUpper(), out myts) ? myts : null;
        }
        public ITimeSeries GetTimeSeries(string key)
        {
            ITimeSeries myts;
            return TimeSeriesCollection.TryGetValue(key.Trim().ToUpper(), out myts) ? myts : null;
        }

        public ITimeSeries GetBarSeries(string key)
        {
            return BarSeriesCollection.FirstOrDefault(bs => bs.Key == key);
        }

        //private ITimeSeries LookForPrimarySeries()
        //{
        //    if ( PrimarySeries != null) return PrimarySeries;
        //    //PrimarySeries = (from bs in BarSeriesCollection where bs.Code == "1_Min" select bs).FirstOrDefault();
        //    //PrimarySeries = (from ts in AsyncSeriesCollection.Values where ts.Name == "PrimaryBars" select ts).FirstOrDefault();
        //    PrimarySeries = RegisterAsyncSeries(new Bars001("1_Min","PrimaryBars", this, 60, 0)) as Bars001;
        //    return PrimarySeries;
        //}

        //public void LoadBarsFromArch(QuoteDataBase db)
        //{
        //    if (LoadMode == 1)
        //    {

        //    }
        //    if ( LoadMode == 2)
        //    {
        //        if (_finamDownLoader == null) _finamDownLoader = new FinamDownLoader();

        //        var cnt = _finamDownLoader.DownLoad(Symbol, "1_Min", DateTime.Today.AddDays(-10), DateTime.Now);
        //        if (cnt <= -2)
        //        {
        //            EventLog.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Ticker." + Code, "Ticker." + Code,
        //                             "LoadFromRemoteHost", 
        //                             "Unable to connect to the remote host IP=" + _finamDownLoader.RemoteHostIP + ", HostName=" + _finamDownLoader.RemoteHostName,
        //                             "");
        //            return;
        //       }
        //    }

        //    UpdateSeries(_finamDownLoader.Bars);
        //}
        //public void UpdateSeries(IEnumerable<IBarFromFinam> bars)
        //{
        //    var atss = from ts in AsyncSeriesCollection.Values.OfType<Bars001>().Where(s => s.TimeIntSeconds >= 60)
        //              select ts;
        //    var stss = from ts in TimeSeriesCollection.Values select ts; // No Bars .Where(s => s.TimeIntSeconds > primary.TimeIntSecond


        //    var ba = new Bar();
        //    foreach (var b in bars.OrderBy(d => d.DT))
        //    {
        //        ba.DT = b.DT;
        //        ba.LastDT = b.DT;
        //        ba.SyncDT = b.DT;

        //        ba.Open = b.Open;
        //        ba.High = b.High;
        //        ba.Low = b.Low;
        //        ba.Close = b.Close;

        //        ba.Volume = (double) b.Volume;
            
        //        foreach (var ts in atss)
        //            ts.Update(ba);
        //        foreach (var ts in stss)
        //            ts.Update(ba.DT);
        //    }
        //}

        public void UpToDate()
        {
            foreach (var ts in TimeSeriesCollection.Values)
                ts.UpToDate();
        }
        public double ToMinMove(double price, int iUpDown)
        {
            return iUpDown > 0
                       ? MinMove * (int)Math.Ceiling(price/MinMove)
                       : MinMove * (int)Math.Floor(price/MinMove);
        }

        //public void InitAsync()
        //{
        //    foreach (var bs in BarSeriesCollection)
        //    {
        //        var b = new Bars001
        //                    {
        //                        Name = bs.Name,
        //                        Ticker = bs.Ticker,
        //                        TimeIntSeconds = bs.TimeIntSeconds,
        //                        ShiftIntSecond = bs.ShiftIntSeconds
        //                    };
        //        b.SetEventLog(Tickers.EventLog);
        //        AsyncSeriesCollection.Add(b.Key,b);
        //    }
        //}
        public void ClearSomeData(int count)
        {
            foreach (var bs in BarSeriesCollection.Where(bs => bs.Count > count))
                bs.ClearSomeData(count);

            foreach (var ts in TimeSeriesCollection.Values.Where(ts => ts.Count > count))
                ts.ClearSomeData(count);
        }
        public override string ToString()
        {
            return string.Format("[Id={0}; Key={5}; Code={1}; Name={2}; TradeBoard={10}; ClassCode={3}; BaseContract={4};" +
                                 " Symbol={6}; Format={7}; DateFrom={8}; DateTo={9}]",
                                 Id, Code, Name, ClassCode, BaseContract, Key, Symbol, Format, From, To, TradeBoard);
        }
        // ************* Quote *****************************************
        public class Quote
        {
            public string Name { get; set; }
            public string Ticker { get; set; }
            // private DateTime _dt;
            public DateTime DT { get; set; }
            // private double _bid;
            public double Bid { get; set; }
            // private double _ask;
            public double Ask { get; set; }
            // private double _last;
            public double Last { get; set; }        
            //private double _volume;
            public double Volume { get; set; }
           
            public string DateTimeString
            {
                get { return DT.ToString("G"); }
            }
            public string TimeDateString
            {
                get { return DT.ToString("T") + ' ' + DT.ToString("d"); }
            }
            public Quote()
            {
            }
            public Quote(string ticker, DateTime dt, double close, double volume)
            {
                Name = ticker;
                Ticker = ticker;
                DT = dt;
                Last = close;
                Volume = volume;
            }
            public Quote(string ticker, DateTime dt, double close, double bid, double ask, double volume)
            {
                Name = ticker;
                Ticker = ticker;
                DT = dt;
                Bid = bid;
                Ask = ask;
                Last = close;
                Volume = volume;

                //var m = DT.Millisecond;
            }
            public override string ToString()
            {
                return String.Format("Ticker={0}, DateTime={1}, Last={2} Bid={3} Ask={4} Volume={5}", Ticker, DT, Last, Bid, Ask, Volume);
            }
        }
        public class QuoteNotify : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public string Name { get; set; }
            public string Ticker { get; set; }

            private DateTime _dt;
            public DateTime DT
            {
                get { return _dt; }
                set 
                { 
                    _dt = value; 
                    //OnPropertyChanged(new PropertyChangedEventArgs("TimeDateString"));
                    OnPropertyChanged(new PropertyChangedEventArgs("TimeString"));
                }
            }

            private double _bid;
            public double Bid 
            { 
                get { return _bid; }
                set
                {
                    _bid = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Bid"));
                }
            }
            private double _ask;
            public double Ask
            {
                get { return _ask; }
                set
                {
                    _ask = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Ask"));
                }
            }

            private double _last;
            public double Last
            {
            get { return _last; }
            set
                {
                    _last = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Last"));
                }
            }
            private double _volume;
            public double Volume
            {
                get { return _volume;  }
                set
                {
                    _volume = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Volume"));
                }
            }

            public string DateTimeString
            {
                get { return _dt.ToString("G"); }
               // set { value = value; }
            }
            public string TimeDateString
            {
                get { return _dt.ToString("T") + ' ' + _dt.ToString("d") ; }
            }
            public string TimeString
            {
                get { return _dt.ToString("T"); }
            }
            public QuoteNotify()
            {
            }
            public QuoteNotify(string ticker, DateTime dt, double close, double volume)
            {
                Name = ticker;
                Ticker = ticker;
                _dt = dt;
                _last = close;
                Volume = volume;
            }
            public QuoteNotify(string ticker, DateTime dt, double close, double bid, double ask, double volume)
            {
                Name = ticker;
                Ticker = ticker;
                _dt = dt;
                Bid = bid;
                Ask = ask;
                _last = close;
                Volume = volume;
            }
            public override string ToString()
            {
                return String.Format("[Ticker={0}; DateTime={1}; Last={2}; Bid={3}; Ask={4}; Volume={5}]", Ticker, DT, Last, Bid, Ask, Volume);
            }
            public void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                if (PropertyChanged != null) PropertyChanged(this, e);
            }
        }
    }
    
}