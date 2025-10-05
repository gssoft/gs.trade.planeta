using System;
using System.Collections.Generic;
using GS.ICharts;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Chart;

namespace GS.Trade.Data.Studies.Stochastic.Old
{
    public class Stochastic0 : TimeSeries
    {
        private float _fastK;
        private float _slowK;
        private float _fastD;
        private float _slowD;

        private Int32 _length;
        private Int32 _adjustK;
        public Int32 _adjustD;

        private Int32 _overSold;
        private Int32 _overBought;

        private const float Factor = 0.5f;

        public Stochastic0(string name, Ticker ticker, int timeIntSeconds, int length, int adjustK, int adjustD, int overSold, int overBought)
            : base(name, ticker, timeIntSeconds)
        {
            _length = length;
            _overSold = overSold;
            _overBought = overBought;
            _adjustK = adjustK;
            _adjustD = adjustD;
        }
        public override void Init()
        {
            if (SyncSeries != null) return;
            SyncSeries = Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, TimeIntSeconds, ShiftIntSecond));
            if (SyncSeries == null)
                throw new NullReferenceException("Bars == null");

            DebudAddITem = true;
        }

        public override void InitUpdate(DateTime dt)
        {
        }
        public override void AddNewItem()
        {
           // if (LastItem != null)
           //     Evl.AddItem(EvlResult.INFO, EvlSubject.PROGRAMMING, "Stoh", "AddNew", LastItem.ToString(), "");

            AddItem(new Item());
        }
        private float FastK(int isync)
        {
            var bars = (Bars.Bars)SyncSeries;
            var h = bars.Highest(BarValue.High, isync, _length);
            var l = bars.Lowest(BarValue.Low, isync, _length);

            var c = ((Bar)(bars[isync])).Close;
            if (h.CompareTo(l) > 0)
                return (float)((c - l) / (h - l) * 100);
            return 0.0f;
        }
        public override void Calculate(ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            if (Count > 2)
            {
                var last = (Item) (Items[ilast]);
                var prev = (Item) (Items[iprev]);

                last.FastK = FastK(isync);
                last.FastD = prev.FastD + (Factor*(last.FastK - prev.FastD));
                last.SlowK = last.FastD;
                last.SlowD = ((prev.SlowD*2) + last.FastD)/3;
            }
            else if ( Count > 0)
                InitItem(tsi, isync, ilast);
        }
        public override void InitItem(ITimeSeriesItem tsi, int isync, int ilast)
        {
            var last = (Item)(Items[ilast]);

            last.FastK = FastK(isync);
            last.FastD = last.FastK;
            last.SlowK = last.FastD;
            last.SlowD = last.FastD;

        }
        public override void CopyItem(TimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var lastItem = ((Item)Items[ilast]);
            var prevItem = ((Item)Items[iprev]);

            lastItem.FastK = prevItem.FastK;
            lastItem.FastD = prevItem.FastD;
            lastItem.SlowK = prevItem.SlowK;
            lastItem.SlowD = prevItem.SlowD;
        }
        /*
        public override double FirstBaseValue
        {
           // get { throw new NotImplementedException(); }
            get { return SyncSeries.FirstBaseValue; }
        }
         */ 
        // Chart Support

        public double GetFastK(int index)
        {
            return ((Item)this[index]).FastK + 950;
        }
        public double GetFastD(int index)
        {
            return ((Item)this[index]).FastD + 950;
        }
        public double GetSlowK(int index)
        {
            return ((Item)this[index]).SlowK + 950;
        }
        public double GetSlowD(int index)
        {
            return ((Item)this[index]).SlowD + 950;
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
                                   Name = "FastK",
                                   Color = 0xff0000,
                                   CallGetCount = GetCount,
                                   CallGetDateTime = GetDateTime,
                                //   CallGetFirstValue = GetFirstValue,
                                   CallGetLine = GetFastK
                               },
                           new ChartLine
                               {
                                   Name = "FastD",
                                   Color = 0x0000ff,
                                   CallGetCount = GetCount,
                                   CallGetDateTime = GetDateTime,
                               //    CallGetFirstValue = GetFirstValue,
                                   CallGetLine = GetFastD
                               },
                           new ChartLine
                               {
                                   Name = "SlowK",
                                   Color = 0xf00000,
                                   CallGetCount = GetCount,
                                   CallGetDateTime = GetDateTime,
                              //     CallGetFirstValue = GetFirstValue,
                                   CallGetLine = GetSlowK
                               },
                           new ChartLine
                               {
                                   Name = "SlowD",
                                   Color = 0x00000f,
                                   CallGetCount = GetCount,
                                   CallGetDateTime = GetDateTime,
                               //    CallGetFirstValue = GetFirstValue,
                                   CallGetLine = GetSlowD
                               }
                       };
        }

        public override string Key
        {
            get
            {
                return String.Format("[Type={0};Ticker={1};TimeIntSeconds={2};ShiftIntSecond={3};"
                                                + "Length={4};AdjustK={5};AdjustD={6};OverSold={7};OverBought={8}]",
                                      GetType(), Ticker.Code, TimeIntSeconds, ShiftIntSecond,
                                                  _length, _adjustK, _adjustD, _overSold, _overBought);
            }
        }
        public override string ToString()
        {
            return String.Format("[Type={0};Ticker={1};TimeIntSeconds={2};ShiftIntSecond={3};"
                                                + "Length={4};AdjustK={5};AdjustD={6};OverSold={7};OverBought={8},Count={9}]",
                                   GetType(), Ticker.Code, TimeIntSeconds, ShiftIntSecond,
                                                  _length, _adjustK, _adjustD, _overSold, _overBought, Count);
        }

        public class Item : TimeSeriesItem
        {
            public float FastK { get; set; }
            public float FastD { get; set; }
            public float SlowK { get; set; }
            public float SlowD { get; set; }
            
            public override string ToString()
            {
                return String.Format("[Type={0};DT={1:T};" + 
                                                "FastK={2:N3};FastD={3:N3};SlowK={4:N3};SlowD={5:N3};SyncTime={6:T}",
                                        GetType(),DT,
                                                FastK, FastD, SlowK, SlowD, SyncDT);
            }
        }
    }
}
