using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.Interfaces;

namespace GS.Trade
{
    public enum PosStatusEnum : short { Closed = 0, Opened = 1 }
    public enum PosOperationEnum : short { Neutral = 0, Long = +1, Short = -1 }

    public delegate void PositionChangedEventHandler(long oldPosition, long newPosition);
    public delegate void PositionChangedEventHandler2(IPosition oldPosition, IPosition newPosition, PositionChangedEnum changedResult);
    public delegate void PositionChangedEventHandler3(IPosition2 oldPosition, IPosition2 newPosition, PositionChangedEnum changedResult);

    public interface IPosition : Containers5.IHaveKey<string>
    {
        IEventLog EventLog { get; set; }

        event PositionChangedEventHandler PositionChangedEvent;
        event PositionChangedEventHandler2 PositionChangedEvent2;

        ITicker Ticker { get; }

        DateTime FirstTradeDT { get; }
        ulong FirstTradeNumber { get; }
        DateTime LastTradeDT { get; }
        ulong LastTradeNumber { get; }

        PosOperationEnum Operation { get; }

        long Quantity { get; }
        long Pos { get; }

        bool IsLong { get; }
        bool IsShort { get; }
        bool IsNeutral { get; }

        bool IsOpened { get; }

        PosOperationEnum PosOperation { get; }
        PosStatusEnum PosStatus { get; }

        IPositionTotal PositionTotal { get; }

        decimal Price1 { get; }
        decimal Price2 { get; }

        float Profit { get; }
        decimal PnL { get; }

        string OperationString { get; }
        string PositionInfo { get; }

        float LastTradeSellPrice { get; }
        float LastTradeBuyPrice { get; }

        void NewTrade(ITrade3 t);

    }

    public interface IPositionTotal
    {
        long Quantity { get; }

        string TotalPnLString { get; }
        string TotalPointPnLString { get; }

        decimal PointPnL { get; }
        decimal CurrencyPnL { get; }

        void Update(IPosition p);

        DateTime FirstTradeDT { get; }
        ulong FirstTradeNumber { get; }
        DateTime LastTradeDT { get; }
        ulong LastTradeNumber { get; }
    }
}
