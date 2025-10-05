using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.ICharts;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Chart;

namespace GS.Trade.Data.Studies.Averages
{
    public abstract class Average : TimeSeries
    {
        public MaType MaType {get;set;}
        public BarValue BarValue { get; set; }
        public Int32 Length { get; set; }
        public Int32 SmoothLength { get; set; }

        public int Trend
        {
            get
            {
                return Count > 1
                    ? ((Item)Items[0]).Ma.CompareTo(((Item)Items[1]).Ma) > 0
                        ? +1
                        : ((Item)Items[0]).Ma.CompareTo(((Item)Items[1]).Ma) < 0
                            ? -1
                            : 0
                    : 0;
            }
        }
        public float Ma
        {
            get { return Count > 1 ? ((Item)LastItemCompleted).Ma : 0.0f; }
        }
        public double GetMa(int i)
        {
            return ((Item)this[i]).Ma;
        }

        protected Average(string name, MaType maType, ITicker ticker, BarValue be, int timeIntSeconds, int length, int smoothLength)
            :base(name,ticker,timeIntSeconds)
        {
            MaType = maType;
            BarValue = be;
            Length = length;
            SmoothLength = smoothLength;
        }
        public override void Init()
        {
            if (SyncSeries != null) return;
            SyncSeries = Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, TimeIntSeconds, ShiftIntSecond));
            if (SyncSeries == null)
                throw new NullReferenceException("Bar == null");

            DebudAddITem = true;
        }
        public static Average GetInstance( MaType maType, string name, ITicker ticker, int timeIntSecond, BarValue be, int length, int smLength)
        {
            switch (maType)
            {
                case MaType.Exponential:
                    return new XAverage(name, ticker, timeIntSecond, be,  length, smLength);
                case MaType.Simple:
                    return new SAverage(name, ticker, timeIntSecond, be, length, smLength);
                default:
                    return new XAverage(name, ticker, timeIntSecond, be, length, smLength);
            }
        }

        protected float GetBarValue(IBar b)
        {
            float v;
            switch (BarValue)
            {
                case BarValue.Close:
                    v = (float)(b.Close);
                    break;
                case BarValue.Median:
                    v = (float)(b.MedianPrice);
                    break;
                case BarValue.Typical:
                    v = (float)(b.TypicalPrice);
                    break;
                case BarValue.High:
                    v = (float)(b.High);
                    break;
                case BarValue.Low:
                    v = (float)(b.Low);
                    break;
                case BarValue.Open:
                    v = (float)(b.Open);
                    break;
                default:
                    v = (float)(b.TypicalPrice);
                    break;
            }
            return v;
        }
        public override void InitUpdate(DateTime dt){}
        
        public override void CopyItem(TimeSeriesItem tsi, int ibar, int ilast, int iprev)
        {
            var lastItem = ((Item)Items[ilast]);
            var prevItem = ((Item)Items[iprev]);

            lastItem.Ma = prevItem.Ma;
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
                                   Name = Name,
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
