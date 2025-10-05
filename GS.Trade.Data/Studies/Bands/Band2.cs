using System;
namespace GS.Trade.Data.Studies.Bands
{
    public abstract class Band2 : Band
    {
        protected Band2(string name, ITicker ticker, int timeIntSeconds)
            : base(name, ticker, timeIntSeconds)
        {
        }

        public float Ma
        {
            get { return LastItemCompleted != null ? ((Item)LastItemCompleted).Ma : 0f; }
        }
        public float High
        {
            get { return LastItemCompleted != null ? ((Item)LastItemCompleted).Ma + ((Item)LastItemCompleted).Deviation : 0f; }
        }
        public float Low
        {
            get { return LastItemCompleted != null ? ((Item)LastItemCompleted).Ma - ((Item)LastItemCompleted).Deviation : 0f; }
        }

        public float Deviation
        {
            get { return (High - Low) / 2f; }
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
            return ((Item)Items[i]).Ma + KDevUp * ((Item)Items[i]).Deviation;
        }
        public override double GetLow(int i)
        {
            return ((Item)Items[i]).Ma - KDevDown * ((Item)Items[i]).Deviation;
        }

        public override string Key
        {
            get
            {
                return String.Format("[Type={0};Ticker={1};TimeIntSeconds={2};ShiftIntSecond={3};" +
                                     "MaType={4};MaLength={5};MaSmooth={6};MaBarValue={7};" +
                                     "DevBarValue={8};DevLength={9};DevSmoothLength={10};" +
                                     "KStdDevUp={11};KStdDevDown={12}]",
                                     GetType(), Ticker.Code, TimeIntSeconds, ShiftIntSecond,
                                     MaType, MaLength, MaSmoothLength, MaBarValue,
                                     DevBarValue, DevLength, DevSmoothLength,
                                     KDevUp, KDevDown);
            }
        }
        public override string ToString()
        {
            return String.Format("[Key={0};Count={1}]", Key, Count);
        }

        public class Item : TimeSeriesItem
        {
            public float Ma { get; set; }
            public float Deviation { get; set; }
            public override string ToString()
            {
                return String.Format("[Type={0};DT={1:T};Ma={2:N3};High={3:N3};" +
                                            "SyncTime={4:HH:mm:ss:fff};LastDT={5:HH:mm:ss:fff};Cnt={6}",
                                        GetType(), DT, Ma, Deviation, SyncDT, LastDT, Series.Count);
            }
        }
    }
}
