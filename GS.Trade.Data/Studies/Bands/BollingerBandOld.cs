using System;
using System.Collections.Generic;
using GS.ICharts;
using GS.Trade.Data.Chart;

namespace GS.Trade.Data.Studies.Bands
{
    public class BollingerBandOld : TimeSeries
    {
        //private StdDev _stdDev;
        private readonly float _kStdDev;
        private readonly Int32 _length;
        private readonly Int32 _smoothLength;
        private readonly BarValue _BarValue;

        public BollingerBandOld(string name, Ticker ticker, int timeIntSeconds, int length, int smoothLength, float kStdDev, BarValue BarValue)
            : base(name, ticker, timeIntSeconds)
        {
            _length = length;
            _smoothLength = smoothLength;
            _kStdDev = kStdDev;
            _BarValue = BarValue;
        }

        public override void Init()
        {
            if (SyncSeries != null) return;
            SyncSeries = Ticker.RegisterTimeSeries(new StdDev("StdDev", Ticker, TimeIntSeconds, _length, _smoothLength)) as StdDev;
            if (SyncSeries == null)
                throw new NullReferenceException("XAverage == null");
            
            DebudAddITem = true;
        }

        public override void InitUpdate(DateTime dt)
        {
            SyncSeries.Update(dt);
        }

        public override void AddNewItem()
        {
            AddItem(new Item());
        }

        public override void Calculate(ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            if (Count < 2) return;

            var stdDev = (StdDev.Item)SyncSeries[isync];
            var last = (Item)(Items[ilast]);

            last.StdDev = stdDev.StdDev;
            last.Ma = stdDev.Ma;
            last.High = last.Ma + _kStdDev * last.StdDev;
            last.Low = last.Ma - _kStdDev * last.StdDev;
        }

        public override void InitItem(ITimeSeriesItem tsi, int isync, int ilast)
        {
            var stdDev = (StdDev.Item)SyncSeries[isync];
            var last = (Item)(Items[ilast]);

            last.StdDev = stdDev.StdDev;
            last.Ma = stdDev.Ma;
            last.High = last.Ma + _kStdDev * last.StdDev;
            last.Low = last.Ma - _kStdDev * last.StdDev;
        }

        public override void CopyItem(TimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var last = (Item)(Items[ilast]);
            var prev = (Item)(Items[iprev]);

            last.StdDev = prev.StdDev;
            last.Ma = prev.Ma;
            last.High = prev.High;
            last.Low = prev.Low;
        }

        public double GetMa(int i)
        {
            return ((Item)Items[i]).Ma;
        }
        public double GetHigh(int i)
        {
            return ((Item)Items[i]).High;
        }
        public double GetLow(int i)
        {
            return ((Item)Items[i]).Low;
        }

        public override IList<ILineSeries> ChartLines { get { return CreateChartLineList(); } }
        private IList<ILineSeries> CreateChartLineList()
        {
            return new List<ILineSeries>
                       {
                           new ChartLine
                               {
                                   Name = "L.BB",
                                   Color = 0x00ff00,
                                   CallGetCount = GetCount,
                                   CallGetDateTime = GetDateTime,
                                   CallGetLine = GetMa
                               }
                       };
        }

        public override IList<IBandSeries> ChartBands { get { return CreateChartBandList(); } }
        private IList<IBandSeries> CreateChartBandList()
        {
            return new List<IBandSeries>
                       {
                           new ChartBand
                               {
                                   BandName="B.BB",
                                   BandColor = 0xff3333,
                                   BandLineColor = 0xff0000,
                                   BandFillColor = unchecked((int)0xc0ff1111),
                                   CallGetCount = GetCount,
                                   CallGetDateTime = GetDateTime,
                                   CallGetLine = GetMa,
                                   CallGetHigh = GetHigh,
                                   CallGetLow = GetLow
                               }
                       };
        }

        public override string Key
        {
            get
            {
                return String.Format("[Type={0};Ticker={1};TimeIntSeconds={2};ShiftIntSecond={3};" +
                                                   "Length={4};Smooth={5};kStdDev={6};BarValue={7}]",
                                       GetType(), Ticker.Code, TimeIntSeconds, ShiftIntSecond,
                                                  _length, _smoothLength, _kStdDev, _BarValue);
            }
        }
        public override string ToString()
        {
            return String.Format("[Key={0};Count={1}]", Key, Count);
        }

        public class Item : TimeSeriesItem
        {
            public float StdDev { get; set; }

            public float High { get; set; }
            public float Low { get; set; }
            public float Ma { get; set; }

            public override string ToString()
            {
                return String.Format("[Type={0};DT={1:T};StdDev={2:N3};Ma={3:N3};High={4:N3};Low={5:N3};" + 
                                            "SyncTime={6:HH:mm:ss:fff};LastDT={7:HH:mm:ss:fff};Cnt={8}",
                                        GetType(), DT, StdDev, Ma, High, Low, SyncDT, LastDT, Series.Count);
            }
        }
    }
}
