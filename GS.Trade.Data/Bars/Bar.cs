using System;

namespace GS.Trade.Data.Bars
{
    public class Bar : TimeSeriesItem, IBar
    {
     //   public long ID { get; set; }
     //   public long SeriesID { get; set; }

      //  public DateTime LastDT { get; set; }
     //   public DateTime SyncDT { get; set; }
        
    //    public DateTime DT { get; set; }

        public ITicker Ticker { get; set; }

        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }

        public double Last {get { return Close; }}

        public double Volume { get; set; }

        public UInt32 Ticks { get; set; }

        public UInt32 UpTicks { get; set; }
        public UInt32 DownTicks { get; set; }

        public double MedianPrice => (High + Low) / 2;
        public double TypicalPrice => (High + Low + Close) / 3;

        public Bar()
        {
            // need for Serialization
        }
        public Bar(DateTime dt, double open, double high, double low, double close, double volume)
        {
            //DataSeriesID = seriesID;
            DT = dt;
            Open = open;
            High = high;
            Low = low;
            Close = close;

            Volume = volume;
        }
        public bool IsBlack => Close.CompareTo(Open) < 0;
        public bool IsBlackLight => Close.CompareTo(Open) <= 0;
        public bool IsWhite => Close.CompareTo(Open) > 0;
        public bool IsWhiteLight => Close.CompareTo(Open) >= 0;
        public bool IsDoj => Close.CompareTo(Open) == 0;
        public bool IsFaded => Close.CompareTo(High) == 0 && Close.CompareTo(Low) == 0;

        public override string ToString()
        {
            return String.Format(
                "[Type={0}; D: {10:yy-MM-dd}; T: {1:HH:mm:ss:fff}; {2:N3}; {3:N3}; {4:N3}; {5:N3}; {6:N0}; SyncDT={7:HH:mm:ss:fff}; LastDT={8:HH:mm:ss:fff}; SrcCnt={9}]",
                GetType(), DT, Open, High, Low, Close, Volume, SyncDT, LastDT, Series!=null?Series.Count:0, DT.Date);
        }
    }
}