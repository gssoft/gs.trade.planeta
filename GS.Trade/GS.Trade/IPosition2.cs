using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.Interfaces;

namespace GS.Trade
{
    public interface IPositionBase : Containers5.IHaveKey<string>, IHaveId<long>
    {
        DateTime FirstTradeDT { get; set; }
        // ulong FirstTradeNumber { get; set; }
        DateTime LastTradeDT { get; set; }
        // ulong LastTradeNumber { get; set; }

        PosOperationEnum Operation { get; set; }
        PosStatusEnum Status { get; set; }

        long Quantity { get; set; }
        long Pos { get;  }

        decimal Price1 { get; set; }
        decimal Price2 { get; set; }
        decimal Price3 { get; }

        decimal PnL3 { get; set; }
    }

    public interface IPosition2 : IPositionBase
    {
        IEventLog EventLog { get; set; }

        event PositionChangedEventHandler PositionChangedEvent;
        event PositionChangedEventHandler2 PositionChangedEvent2;
        event PositionChangedEventHandler3 PositionChangedEvent3;

        ulong FirstTradeNumber { get; set; }
        ulong LastTradeNumber { get; set; }

        IStrategy Strategy { get; }
        ITicker Ticker { get; }
        IAccount Account { get; }

        PosOperationEnum PosOperation { get; }
        PosStatusEnum PosStatus { get; }

        bool IsLong { get; }
        bool IsShort { get; }
        bool IsNeutral { get; }

        bool IsOpened { get; }

        PositionChangedEnum LastChangedResult { get; set; }

        IPositionTotal2 PositionTotal2 { get; set; }
        IPosition2 PositionTotal { get; set; }

        decimal LastPrice { get; set; }

        float Profit { get; }

        decimal PnL { get; set; }
        decimal PnL1 { get; }
        decimal PnL2 { get; }
        decimal PosPnLFixed { get; set; }
        decimal DailyPnLFixed { get; set;}

        string OperationString { get; }
        string PositionInfo { get; }

        float LastTradeSellPrice { get; }
        float LastTradeBuyPrice { get; }

        IBar LastBar { get; }
        void SetLastBar(IBar b);

        DateTime LocalBarDateTime { get; }

        IDeal NewTrade(ITrade3 t);
        void Update(IPosition2 p);

        IPosition2 Clone();

        void Clear();

        string StrategyCodeEx { get; set; }
        string StrategyKeyEx { get; set; }
        string AccountCodeEx { get; set; }
        string TickerCodeEx { get; set; }

        long TotalQuantity { get; }

        string PositionString3 { get; }
        bool IsDailyProfitLimitReached { get; }
        bool IsDailyLossLimitReached { get; }

        decimal DailyCurrentPnL { get; }
        decimal DailyProfitLimit { get; set; }

        string Comment { get; set; }

        string StrategyTickerString { get; }
        string StrategyTimeIntTickerString { get; }

        double Delta { get;}
        decimal TotalDailyMaxProfit { get; }
        decimal TotalDailyMaxLoss { get; }
        DateTime TotalDailyMaxProfitDT { get; }
        DateTime TotalDailyMaxLossDT { get; }
        void CreateStat();
    }

    public interface IPositionDb : IPositionBase
    {
        decimal FirstTradeNumber { get; set; }
        decimal LastTradeNumber { get; set; }
    }

    //public interface IPositionTotal2
    //{
    //    long Quantity { get; }

    //    string TotalPnLString { get; }
    //    string TotalPointPnLString { get; }

    //    decimal PointPnL { get; }
    //    decimal CurrencyPnL { get; }

    //    void Update(IPosition2 p);

    //    DateTime FirstTradeDT { get; }
    //    long FirstTradeNumber { get; }
    //    DateTime LastTradeDT { get; }
    //    long LastTradeNumber { get; }
    //}
    public interface IPositionTotal2 : IPosition2
    {
    }
    public interface IDealBase : Containers5.IHaveKey<string>, IHaveId<long>
    {
        DateTime DT { get; set; }
        // ulong Number { get; set; }
        DateTime FirstTradeDT { get; set; }
        // ulong FirstTradeNumber { get; set; }
        DateTime LastTradeDT { get; set; }
        // ulong LastTradeNumber { get; set; }

        PosOperationEnum Operation { get; set; }
        PosStatusEnum Status { get; set; }

        long Quantity { get; set; }
        long Pos { get; }

        decimal Price1 { get; set; }
        decimal Price2 { get; set; }
    }

    public interface IDealDb : IDealBase
    {
        //decimal Number { get; set; }
        //decimal FirstTradeNumber { get; set; }
        //decimal LastTradeNumber { get; set; }

        decimal Number { get; set; }
        decimal FirstTradeNumber { get; set; }
        decimal LastTradeNumber { get; set; }
    }

    public interface IDeal : IDealBase
    {
        ulong Number { get; set; }
        ulong FirstTradeNumber { get; set; }
        ulong LastTradeNumber { get; set; }

        IStrategy Strategy { get; }

        string Amount1String { get; }
        string Amount2String { get;}

        string Price1String { get;}
        string Price2String { get;}
        string LastPriceString { get;}

        decimal PnL { get; }

        bool IsFinResultPositive { get; }
        bool IsFinResultNegative { get; }

        bool IsLong { get; }
        bool IsShort { get; }
    }
}
