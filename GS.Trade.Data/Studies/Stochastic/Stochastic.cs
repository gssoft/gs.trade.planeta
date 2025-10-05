using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.ICharts;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Chart;

namespace GS.Trade.Data.Studies.Stochastic
{
    public enum StochasticType { Fast = 1, Slow = 2, Classic}
    public abstract class Stochastic : TimeSeries
    {
        public StochasticType StType { get; protected set; }
        public BarValue HighValue { get; protected set; }
        public BarValue LowValue { get; protected set; }
        public BarValue CloseValue { get; protected set; }

        public Int32 Length { get; protected set; }
        public Int32 AdjustK { get; protected set; }
        public Int32 AdjustD { get; protected set; }

        public float OverSold { get; protected set; }
        public float OverBought { get; protected set; }

        protected const float Factor = 0.5f;

        protected Func<int, double> GetCloseValue;

        protected Stochastic(string name, Ticker ticker, int timeIntSeconds)
            : base(name, ticker, timeIntSeconds)
        {
        }

        protected Stochastic(string name, ITicker ticker, int timeIntSeconds,
                                    BarValue highValue, BarValue lowValue, BarValue closeValue,
                                    int length, float overSold, float overBought)
            : base(name, ticker, timeIntSeconds)
        {
            HighValue = highValue;
            LowValue = lowValue;
            CloseValue = closeValue;

            Length = length;
            OverSold = overSold;
            OverBought = overBought;
        }
        public override void Init()
        {
            if (SyncSeries != null) return;
            SyncSeries = Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, TimeIntSeconds, ShiftIntSecond));
            if (SyncSeries == null)
                throw new NullReferenceException("Bars == null");

            GetCloseValue = ((Bars001) SyncSeries).GetFuncGetItemValue(CloseValue);

            DebudAddITem = true;
        }
        protected float FastK(int isync, int len)
        {
            var bars = (Bars.Bars)SyncSeries;
            var h = bars.Highest(HighValue, isync, len);
            var l = bars.Lowest(LowValue, isync, len);

            //var c = ((Bar)(bars[isync])).Close;
            var c = GetCloseValue(isync);
            if (h.CompareTo(l) > 0)
                return (float)((c - l) / (h - l) * 100);
            return 0.0f;
        }
        public override void InitUpdate(DateTime dt)
        {
        }

        public IEnumerable<float> K()
        {
            for(var i = 0; i < Count; i++)
                yield return ((Item) Items[i]).K;
        }
        public IEnumerable<float> D()
        {
            for (var i = 0; i < Count; i++)
                yield return ((Item)Items[i]).D;
        }

        public double GetK(int index)
        {
            return ((Item)this[index]).K;
        }
        public double GetD(int index)
        {
            return ((Item)this[index]).D;
        }

        public double GetOverSold()
        {
            return OverSold;
        }
        public double GetOverBought()
        {
            return OverBought;
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
                                   Name = StType + ".K",
                                   Color = 0xff0000,
                                   CallGetCount = GetCount,
                                   CallGetDateTime = GetDateTime,
                                   CallGetLine = GetK
                               },
                           new ChartLine
                               {
                                   Name = StType + ".D",
                                   Color = 0x0000ff,
                                   CallGetCount = GetCount,
                                   CallGetDateTime = GetDateTime,
                                   CallGetLine = GetD
                               }
                       };
        }

        private IList<ILevel> _chartLevels;
        public override IList<ILevel> ChartLevels
        {
            get{ return _chartLevels ?? CreateChartLevels();}
        }
        private IList<ILevel> CreateChartLevels()
        {
            _chartLevels = new List<ILevel>
                               {
                                   new Level
                                       {
                                           Color = 0xff0000,
                                           BackGroundColor = 0xffffff,
                                           Text = "OverSold",
                                           GetValue = GetOverSold,
                                           IsValid = () => true
                                       },
                                   new Level
                                       {
                                           Color = 0x0000ff,
                                           BackGroundColor = 0xffffff,
                                           Text = "OverBought",
                                           GetValue = GetOverBought,
                                           IsValid = () => true
                                       }
                               };
            return _chartLevels;
        }

        public override string Key
        {
            get
            {
                return String.Format("[Type={0};Ticker={1};TimeIntSeconds={2};ShiftIntSecond={3};"
                                                + "Length={4};AdjustK={5};AdjustD={6};OverSold={7};OverBought={8}]",
                                      GetType(), Ticker.Code, TimeIntSeconds, ShiftIntSecond,
                                                  Length, AdjustK, AdjustD, OverSold, OverBought);
            }
        }
        public override string ToString()
        {
            return String.Format("[Type={0};Ticker={1};TimeIntSeconds={2};ShiftIntSecond={3};"
                                                + "Length={4};AdjustK={5};AdjustD={6};OverSold={7};OverBought={8},Count={9}]",
                                   GetType(), Ticker.Code, TimeIntSeconds, ShiftIntSecond,
                                                  Length, AdjustK, AdjustD, OverSold, OverBought, Count);
        }

        public class Item : TimeSeriesItem
        {
            public float K { get; set; }
            public float D { get; set; }

            public override string ToString()
            {
                return String.Format("[Type={0};DT={1:T};" +
                                                "K={2:N3};D={3:N3};SyncTime={4:T}",
                                        GetType(), DT, K, D, SyncDT);
            }
        }
    }
}
