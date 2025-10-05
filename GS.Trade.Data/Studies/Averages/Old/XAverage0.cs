using System;
using System.Collections.Generic;
using GS.ICharts;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Chart;

namespace GS.Trade.Data.Studies.Averages.Old
{
    public class XAverage0 : TimeSeries
    {
        private readonly int _length;
        private readonly int _smoothLength;

        private readonly double _k;
        private readonly BarValue _be;
        
        public int Color;

        public XAverage0(string name, Ticker ticker, BarValue be, int timeIntSeconds, int length, int smoothLength)
            : base(name, ticker, timeIntSeconds)
        {
            _be = be;
            _length = length;
            _smoothLength = smoothLength;
            _k = 2.0 / (_length + 1);
        }
        public XAverage0(string name, Ticker ticker, int timeIntSeconds, int length)
            : base(name, ticker, timeIntSeconds)
        {
            _length = length;
            _k = 2.0 / (_length + 1);
        }

        public override void Init()
        {
            if (SyncSeries != null) return;
            SyncSeries = Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, TimeIntSeconds, ShiftIntSecond));
            if (SyncSeries == null)
                throw new NullReferenceException("Bar == null");

            DebudAddITem = true;
        }
        public static float Calculate( int length, float newValue, float previousValue)
        {
            var  k = (float) (2.0/(length + 1));
            return (float) (k*newValue + (1.0 - k)*previousValue);
        }
        
        /*
        public override void Update(DateTime syncDT, IEventLog evl)
        {
            if (IsUpToSyncTime(syncDT)) return;  // If this Series already UpDate for this Sync
            if (_bars.Count < 1) return;

            SyncDT = syncDT;

            if (_bars.TickCount <= TickCount) return;
            TickCount = _bars.TickCount;

            var barLastTickDt = _bars.LastTickDT;

            LastTickDT = barLastTickDt;

            var barDt = GetBarDateTime(barLastTickDt, TimeIntSeconds);

            if (Items.Count > 0)
            {
                var nbardt = DtToLong(barDt);
                var nlastdt = DtToLong(_lastItem.DT);

                //if (barDt.CompareTo(Last.DT) > 0)
                if (nlastdt >= nbardt)
                {
                    // _lastItem.SyncDT = dt;

                    _lastItem.SyncDT = barLastTickDt;
                    if (nlastdt == nbardt)
                    {
                        _lastItem.Ma = (float)(Count > 1 ? ((Item)LastItemCompleted).Ma : _bars[0].TypicalPrice);
                    }
                    else
                    {
                        evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "XAverage", "Update",
                                    String.Format("LastBarTime={0:T} > LastTickTime={1:T}", _lastItem.DT, syncDT), "");
                    }
                }
                else // New Item Should Add
                {
                    if (Count > 1)
                        _lastItem.Ma = (float)(_k *_bars[1].TypicalPrice + (1.0 -_k) * ((Item)LastItemCompleted).Ma);
                    else
                    {
                        _lastItem.Ma = (float)_bars[1].TypicalPrice;
                    }

                    if (DtToLong(_lastItem.DT) != DtToLong(_bars[1].DT))
                        evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Xma015.New", "Sync is Lost",
                                    _lastItem.ToString(), _bars[1].ToString());
                    
                    // if (Count > 1)
                    //    evl.AddItem(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, "XAverage.Update", "New Item", "Completed:" + _lastItem + " Prev:" + LastItemCompleted,
                    //    String.Format("BarDT={0:T} k={1:N3} TR={2:N3} Ticks={3}",
                    //        _bars[1].DT, _k, _bars.TrueRange(1), TickCount));
                    
                    var ma = _lastItem.Ma;
                    _lastItem = new Item(barDt, ma) { SyncDT = LastTickDT };
                    AddItem(_lastItem);
                }
            }
            else
            {
                var ma = (float)_bars[0].TypicalPrice;
                _lastItem = new Item(barDt, ma) { SyncDT = LastTickDT };
                AddItem(_lastItem);
            }
        }
*//*
        public override void Update(DateTime syncDT)
        {
            if (IsUpToSyncTime(syncDT)) return;  // If this Series already UpDate for this Sync
            if (_bars.Count < 1) return;

            if (_bars.TickCount <= TickCount) return;
            TickCount = _bars.TickCount;

            var barLastTickDt = _bars.LastTickDT;

            LastTickDT = barLastTickDt;

            var barDt = _bars.LastItem.DT;

            if (Items.Count > 0)
            {
                var nbardt = DtToLong(barDt);
                var nlastdt = DtToLong(_lastItem.DT);

                if (nlastdt >= nbardt)
                {
                    _lastItem.SyncDT = barLastTickDt;

                    if (nlastdt == nbardt)
                    {
                        _lastItem.Ma = (float)(Count > 1 ? ((Item)LastItemCompleted).Ma : _bars[0].TypicalPrice);
                    }
                    else
                    {
                        Evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "XAverage", "Update",
                                    String.Format("LastBarTime={0:T} > LastTickTime={1:T}", _lastItem.DT, syncDT), "");
                    }
                }
                else // New Item Should Add
                {
                    if (Count > 1)
                        Calculate1(1,0,1);
                    else
                    {
                        _lastItem.Ma = (float)_bars[1].TypicalPrice;
                    }

                    if ( Count > 1  && DtToLong(_lastItem.DT) < DtToLong(_bars[1].DT))
                    {
                        //Evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Xma015.New", "Sync is Lost",
                        //            _lastItem.ToString(), _bars[1].ToString());
                        var lostItem = this[0];
                        var i = 0;
                        while (_bars[i].DT > _lastItem.DT) i++;
                        for (var j = i; j >= 0; j--)
                        {
                                Calculate1(j,0,1);
                                LastItem.DT = _bars[j].DT;
                                LastItem.SyncDT = _bars[j].DT;
                                
                                if (j > 0) AddItem(new Item());
                        }
                        _lastItem = LastItem as Item;

                        Evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, Name +".NewItem", "Lost[" + i + "]: " + syncDT.ToString("H:mm:ss.fff"),
                             "XAvr: " + lostItem.DT.ToString("H:mm:ss.fff") + " Bar[0]: " + _bars[0].DT.ToString("H:mm:ss.fff") + " Bar[1]: " + _bars[1].DT.ToString("H:mm:ss.fff"),
                                    "XAvr: " + lostItem + " Bar: " + _bars[0]);
                    }
                    else
                    {
                        var ma = _lastItem.Ma;
                        _lastItem = new Item(barDt, ma) {SyncDT = LastTickDT};
                        AddItem(_lastItem);
                    }
                }
            }
            else
            {
                var ma = (float)_bars[0].TypicalPrice;
                _lastItem = new Item(barDt, ma) { SyncDT = LastTickDT };
                AddItem(_lastItem);
            }
        }
        */

        public override void InitUpdate(DateTime dt)
        {
        }
        /*
        public override void InitItem(int ibar, int ilast)
        {
            var bar = _bars.Bar(ibar);
            var last = (Item)(Items[ilast]);

            last.DT = bar.DT;
            last.SyncDT = bar.LastDT;
            last.LastDT = bar.LastDT;

            last.Ma = (float)bar.TypicalPrice;
        }
        public override void Calculate2(int ibar, int ilast, int iprev)
        {
            if (Count > 1)
            {
                var bar = _bars.Bar(ibar);
                var last = ((Item) Items[ilast]);
                var prev = ((Item) Items[iprev]);

                last.LastDT = _bars.Bar(ibar).DT;
                last.SyncDT = _bars.Bar(ibar).LastDT;

                last.Ma = (float) (_k * bar.TypicalPrice + (1.0 - _k) * prev.Ma);
            }
            else if (Count > 0)
                InitItem(ibar, 0);
        }
        public override void CopyItem(int ibar, int ilast, int iprev)
        {
            if (Count < 2) return;

            var bar = _bars.Bar(ibar);
            var last = ((Item)Items[ilast]);
            var prev = ((Item)Items[iprev]);

            last.DT = bar.DT;
            last.SyncDT = bar.LastDT;

            last.Ma = prev.Ma;
        }
        */
        public override void AddNewItem()
        {
            AddItem(new Item());
        }

        // TimeSeries UpdateItem(TimeSeries tsi)
        /*
        public override void PreUpdate(TimeSeriesItem tsi)
        {
        }
        public override void Calculate(TimeSeriesItem tsi)
        {
            if (Count > 1)
            {
                var b = (Bar) tsi;
                var last = (Item) LastItem;
                var prev = (Item) LastItemCompleted;
                
                last.LastDT = b.DT;
                last.SyncDT = b.DT;

                last.Ma = (float) (_k*b.TypicalPrice + (1.0 - _k)*prev.Ma);
            }
            else if (Count > 0)
                InitItem(tsi);
        }
        public override void InitItem(TimeSeriesItem tsi)
        {
            var b = (Bar)tsi;
            var last = (Item)LastItem;

            last.DT = b.DT;
            last.LastDT = b.DT;
            last.SyncDT = b.DT;

            last.Ma = (float)b.TypicalPrice;
        }
        */
        public override void Calculate(ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var b = (Bar)SyncSeries[isync];
            var last = (Item)(Items[ilast]);
            var prev = (Item)(Items[iprev]);

            float ma;
            switch(_be)
            {
                case BarValue.Close:
                    ma = (float)(_k * b.Close + (1.0 - _k) * prev.Ma);
                    break;
                case BarValue.Median:
                    ma = (float)(_k * b.MedianPrice + (1.0 - _k) * prev.Ma);
                    break;
                case BarValue.Typical:
                    ma = (float)(_k * b.TypicalPrice + (1.0 - _k) * prev.Ma);
                    break;
                case BarValue.High:
                    ma = (float)(_k * b.High + (1.0 - _k) * prev.Ma);
                    break;
                case BarValue.Low:
                    ma = (float)(_k * b.Low + (1.0 - _k) * prev.Ma);
                    break;
                case BarValue.Open:
                    ma = (float)(_k * b.Open + (1.0 - _k) * prev.Ma);
                    break;
                default:
                    ma = (float)(_k * b.TypicalPrice + (1.0 - _k) * prev.Ma);
                    break;
            }
            last.Ma  = _smoothLength == 0
                              ? ma
                              : Calculate(_smoothLength, ma, prev.Ma);

         //   if (Count > 0)
         //       Evl.AddItem(EvlResult.INFO, EvlSubject.PROGRAMMING, Name, "Calculate", LastItem.ToString(), "");
        }
        public override void InitItem(ITimeSeriesItem tsi, int isync, int ilast)
        {
            var b = (Bar) SyncSeries[isync];
            var last = (Item)(Items[ilast]);

            last.Ma = (float)b.TypicalPrice;

        //    if (Count > 0)
        //        Evl.AddItem(EvlResult.INFO, EvlSubject.PROGRAMMING, Name, "Init", LastItem.ToString(), "");
        }
        public override void CopyItem(TimeSeriesItem tsi, int ibar, int ilast, int iprev)
        {
            var lastItem = ((Item)Items[ilast]);
            var prevItem = ((Item)Items[iprev]);
            
            lastItem.Ma = prevItem.Ma;
        }
        public double Ma
        {
            get { return Count > 1 ? ((Item)LastItemCompleted).Ma : 0.0; }
        }
        public int Trend
        {
            get {
                return Count > 1 
                    ? ((Item)Items[0]).Ma.CompareTo(((Item)Items[1]).Ma) > 0
                        ? +1
                        : ((Item)Items[0]).Ma.CompareTo(((Item)Items[1]).Ma) < 0 
                            ? -1 
                            : 0
                    : 0;
            }
        }

        public override string Key
        {
            get
            {
                return String.Format("[Type={0};Ticker={1};TimeIntSeconds={2};ShiftIntSecond={3};Length={4}]",
                                                  GetType(), Ticker.Code, TimeIntSeconds, ShiftIntSecond, _length);
            }
        }
        public override string ToString()
        {
            return String.Format("Type={0};Name={1};Ticker={2};TimeIntSecond={3};ShiftIntSecond={4};Length={5};ItemsCount={6}",
                GetType(), Name, Ticker.Code, TimeIntSeconds, ShiftIntSecond, _length, Count);
        }

        // Chart Support
        public double GetMa(int i)
        {
            return ((Item)this[i]).Ma;
        }

        public override IList<ILineSeries> ChartLines
        {
            get { return CreateChartLines(); }
        }
        private IList<ILineSeries> CreateChartLines()
        {
            return new List<ILineSeries>
                       {
                           new ChartLine
                               {
                                   Name = "XAvrg",
                                   Color = 0xff0000,
                                   CallGetCount = GetCount,
                                   CallGetDateTime = GetDateTime,
                                   CallGetLine = GetMa
                               }
                       };
        }
        
        public class Item : TimeSeriesItem
        {
            public float Ma { get; set; }
            
            public override string ToString()
            {
                return String.Format("[Type={0};DT={1:T};Ma={2:N3};SyncTime={3:HH:mm:ss:fff};LastDT={4:HH:mm:ss:fff};Cnt={5}",
                    GetType(), DT, Ma, SyncDT, LastDT, Series.Count);
            }
        }
    }
}
