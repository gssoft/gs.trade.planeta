using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.Containers5;

namespace GS.Trade
{
    public enum BarValue
    {
        Close = 1,
        High = 2,
        Low = 3,
        Open = 4,
        Median = 5,
        Typical = 6
    }

    public interface IBars
    {
        int Count { get; }
        IBar Bar(int i);

        int TimeIntSeconds { get; }

        long TickCount { get; }

        double Open { get; }
        double High { get; }
        double Low { get; }
        double Close { get; }
        double LastCompletedClose { get; }
        double Volume { get; }

        bool IsWhite { get; }
        bool IsBlack { get; }
        bool IsDoj { get; }
        bool IsFaded { get; }

        DateTime GetDateTime(int index);
        double GetOpen(int index);
        double GetHigh(int index);
        double GetLow(int index);
        double GetClose(int index);
        double GetTypical(int i);
        double GetMedian(int i);
        double GetLine(int i);

        double TrueRange(int index);
        // TrueRange Valid
        bool IsNoValid01(int index);
        bool IsNoValid02(int index);

        IBar LastItem { get; }
        IBar LastItemCompleted { get; }
    }
    public interface IBar : IBarSimple
    {
        long SeriesID { get; }
        
        bool IsWhite { get; }
        bool IsBlack { get; }
        bool IsDoj { get; }
        bool IsFaded { get; }

        double MedianPrice { get; }
        double TypicalPrice { get; }
    }
    public interface IBarsSimple
    {
        IBarSimple Bar(int i);
    }
    public interface IBarSimple  : IBarBase
    {
        //DateTime DT { get; }
      //  DateTime LastDT { get; }

        //double Open { get; }
        //double High { get; }
        //double Low { get;  }
        //double Close { get;  }

        //double Volume { get; }
        UInt32 Ticks { get; }
    }
    public interface IBarFromFinam
    {
        DateTime DT { get; }
        double Open { get; set; }
        double High { get; set; }
        double Low { get; set; }
        double Close { get; set; }
        decimal Volume { get; set; }
    }

    public interface IBarBase : IHaveKey<string>
    {
        DateTime DT { get; }
        double Open { get; }
        double High { get; }
        double Low { get; }
        double Close { get; }
        double Volume { get; }
    }
    //public interface IBarBase : IHaveKey<string>
    //{
    //    DateTime DT { get; set; }
    //    double Open { get; set; }
    //    double High { get; set; }
    //    double Low { get; set; }
    //    double Close { get; set; }
    //    double Volume { get; set; }
    //}
    public interface IBarBaseRW
    {
        DateTime DT { get; set; }
        double Open { get; set; }
        double High { get; set; }
        double Low { get; set; }
        double Close { get; set; }
        double Volume { get; set; }
    }

    public interface IBarDb : IBarBase
    {
        long BarSeriesId { get; }
    }
}
