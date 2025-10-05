using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.Tickers
{
    public class Tick2 : ITimeSeriesItem, ITick, IBarSimple
    {
        public ITicker Ticker { get; set; }

        public DateTime DT { get; set; }
        public double Last { get; set; }
        public double Volume { get; set; }

        public uint Ticks
        {
            get { return 1; }
        }
        public DateTime LastDT { get; set; }
        public DateTime SyncDT { get; set; }

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
        public virtual DateTime GetTimeSeriesItemDateTime(DateTime dt, int timeIntSeconds)
        {
            return GetBarDateTime2(dt, timeIntSeconds);
        }
        private static DateTime GetBarDateTime2(DateTime dt, int timeIntSeconds)
        {
            var sec = dt.Hour * 3600 + dt.Minute * 60 + dt.Second;
            var hsec = (int)(Math.Ceiling((double)sec / timeIntSeconds)) * timeIntSeconds;
            return (dt.AddSeconds(hsec - sec)).AddMilliseconds(-dt.Millisecond);
        }
    }
    public class TickQuik2 : Tick2
    {
        //Tick without Ticks, MiliSeconds. Only Seconds

        public override DateTime GetTimeSeriesItemDateTime(DateTime dt, int timeIntSeconds)
        {
            return GetBarDateTime(dt, timeIntSeconds);
        }
        static private DateTime GetBarDateTime(DateTime dt, int timeIntSeconds)
        {
            var sec = dt.Hour * 3600 + dt.Minute * 60 + dt.Second;
            var hsec = (sec / timeIntSeconds + 1) * timeIntSeconds;
            return (dt.AddSeconds(hsec - sec)).AddMilliseconds(-dt.Millisecond);
        }
    }

    public class TickSimple2 : ITickSimple
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
    }
}
