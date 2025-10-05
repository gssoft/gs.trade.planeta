using System;

namespace GS.Trade.Data
{
    public class Tick : TimeSeriesItem, ITick, IBarSimple
    {
        public ITicker Ticker { get; set; }

        public double Last { get; set; }
        public double Volume { get; set; }

        public uint Ticks
        {
            get { return 1; }
        }

        public override DateTime LastDT { get {return DT;}}
        public override DateTime SyncDT { get { return DT; } }

        public double Open
        {
            get { return Last; }
        }
        public double High
        {
            get { return Last; }
        }
        public double Low
        {
            get { return Last; }
        }
        public double Close
        {
            get { return Last; }
        }

        public override string ToString()
        {
            return String.Format("[Type={0};Ticker={1};DT={2};Last={3};Volume={4};Sync={5}]",
                GetType(), Ticker, DT, Last, Volume, SyncDT);
        }
    }
    public class TickQuik : Tick
    {
        //Tick without Ticks, MiliSeconds. Only Seconds
 
        public override DateTime GetTimeSeriesItemDateTime(DateTime dt, int timeIntSeconds)
        {
            return TimeSeries.GetBarDateTime(dt, timeIntSeconds);
        }
    }

    public class TickSimple : ITickSimple
    {
        public DateTime DT { get; set; }

        public double Last { get; set; }
        public double Volume { get; set; }

        public double Open
        {
            get { return Last; }
        }
        public double High
        {
            get { return Last; }
        }
        public double Low
        {
            get { return Last; }
        }
        public double Close
        {
            get { return Last; }
        }

        public uint Ticks
        {
            get { return 1; }
        }

        public string Key { get; private set; }
    }
}
