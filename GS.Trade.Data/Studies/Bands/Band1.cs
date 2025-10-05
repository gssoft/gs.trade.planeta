using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.ICharts;
using GS.Trade.Data.Chart;

namespace GS.Trade.Data.Studies.Bands
{
    public abstract class Band1 : Band
    {
        protected Band1(string name, Ticker ticker, int timeIntSeconds)
            : base(name, ticker, timeIntSeconds)
        {
        }

        public float Ma
        {
            get { return LastItemCompleted != null ? ((Item)LastItemCompleted).Ma : 0f; }
        }
        public float High
        {
            get { return LastItemCompleted != null ? ((Item)LastItemCompleted).High : 0f; }
        }
        public float Low
        {
            get { return LastItemCompleted != null ? ((Item)LastItemCompleted).Low : 0f; }
        }

        public float Deviation
        {
            get { return (High - Low) /2f; }
        }
        public float DeviationUp
        {
            get { return High - Ma; }
        }
        public float DeviationDown
        {
            get { return Ma - Low; }
        }

        public override double GetMa(int i)
        {
            return ((Item)Items[i]).Ma;
        }
        public override double GetHigh(int i)
        {
            return ((Item)Items[i]).High;
        }
        public override double GetLow(int i)
        {
            return ((Item)Items[i]).Low;
        }

        public override string Key
        {
            get
            {
                return String.Format("[Type={0};Ticker={1};TimeIntSeconds={2};ShiftIntSecond={3};" +
                                     "MaType={4};MaLength={5};MaSmooth={6};MaBarValue={7};" +
                                     "DevBarValue={8};KStdDevUp={9};KStdDevDown={10}]",
                                     GetType(), Ticker.Code, TimeIntSeconds, ShiftIntSecond,
                                     MaType, MaLength, MaSmoothLength, MaBarValue,
                                     DevBarValue, KDevUp, KDevDown);
            }
        }
        public override string ToString()
        {
            return String.Format("[Key={0};Count={1}]", Key, Count);
        }
        
        public  class Item : TimeSeriesItem
        {
            public float High { get; set; }
            public float Low { get; set; }
            public float Ma { get; set; }

            public override string ToString()
            {
                return String.Format("[Type={0};DT={1:T};Ma={2:N3};High={3:N3};Low={4:N3};" +
                                            "SyncTime={5:HH:mm:ss:fff};LastDT={6:HH:mm:ss:fff};Cnt={7}",
                                        GetType(), DT,  Ma, High, Low, SyncDT, LastDT, Series.Count);
            }
        }
    }
}
