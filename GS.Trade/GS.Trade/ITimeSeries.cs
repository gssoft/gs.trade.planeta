using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.Elements;
using GS.Interfaces;

namespace GS.Trade
{
    public interface ITimeSeriesBase : IHaveKey<string>, IHaveId<long>
    {
        string Code { get; set; }
        string Name { get; set; }

        int TimeInterval { get; set; }
        int TimeShift { get; set; }
    }
    

    public interface ITimeSeries : ITimeSeriesBase //, IElement1<string>
    {
        IElement1<string> Parent { get; set; }

        ITicker Ticker { get; }

        ITimeSeriesItem this[int index] { get;  }
        //string Key { get;  }

        int Count { get; }

        int ItemsDailyCount { get; }

        void SetEventLog(IEventLog evl);
        void Init();

        void Update(DateTime dt);
        void Update(ITimeSeriesItem tsi);

        void UpToDate();

        void Clear();
        void ClearSomeData(int count);

        long TickCount { get; }
        int TimeIntSeconds { get; }
        int ShiftIntSecond { get; set; }

        DateTime LastTickDT { get; }
        ITimeSeriesItem LastItem { get; }

        DateTime LastItemCompletedDT { get; }
        ITimeSeriesItem LastItemCompleted { get; }
        ITimeSeries SyncSeries { get; }
    }

    public interface ITimeSeriesBase2 : IHaveKey<string>, IHaveId<long>
    {
        string Code { get; set; }
        string Name { get; set; }
    }
    public interface ITimeSeriesBase3 : IHaveKey<string>, IHaveID<long>
    {
        string Code { get; set; }
        string Name { get; set; }
    }

    public interface ITimeSeries2 : ITimeSeriesBase //, IElement1<string>
    {
        IElement1<string> Parent { get; set; }

        ITicker Ticker { get; }

        ITimeSeriesItem this[int index] { get; }
        //string Key { get;  }

        int Count { get; }

        void SetEventLog(IEventLog evl);
        void Init();

        void Update(DateTime dt);
        void Update(ITimeSeriesItem tsi);

        void UpToDate();

        void ClearSomeData(int count);

        long TickCount { get; }
        int TimeIntSeconds { get; }
        int ShiftIntSecond { get; set; }

        DateTime LastTickDT { get; }
        ITimeSeriesItem LastItem { get; }

        DateTime LastItemCompletedDT { get; }
        ITimeSeriesItem LastItemCompleted { get; }
        ITimeSeries SyncSeries { get; }
    }

    //public interface ISyncSeries : ITimeSeries
    //{
    //    long TickCount { get; }
    //    DateTime LastTickDT { get; }
    //    ITimeSeriesItem LastItem { get; }
    //    ITimeSeries SyncSeries { get; }
    //}

    public interface ITimeSeriesItem : IHaveKey<string>
    {
        DateTime DT { get; set; }
        DateTime LastDT { get; set; }
        DateTime SyncDT { get; set; }

        DateTime GetTimeSeriesItemDateTime(DateTime dt, int timeIntSeconds);
    }
}
