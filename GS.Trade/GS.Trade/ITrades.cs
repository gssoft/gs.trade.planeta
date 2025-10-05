using System;
using GS.Containers5;
using GS.Interfaces;

namespace GS.Trade
{
    public enum TradeStatusEnum : short
    {
        Unknown = 0, Registered = 1,  Confirmed = 2, Processed = 3, ToProcess = 4, ToResolve = 5, Resolved = 6
    }
    public enum TradeOperationEnum : short { Buy = +1, Sell = -1, Unknown = 0 }

    public delegate void NeedToOserverEventHandler();

    public interface ITrade : IHaveKey<string>
    {
        string Account { get; }
        string Strategy { get; }
        string Ticker { get; }

        DateTime DT { get; }
        ulong Number { get; }

        //TradeStatusEnum Status { get; }

        TradeOperationEnum TradeOperation { get; }
        TradeOperationEnum Operation { get; }

        long Quantity { get; }
        decimal Price { get; }

        long Position{ get; }

        DateTime Registered { get; }

        string StrategyKey { get; }
        //string Key { get; }

        
    }

    public interface ITrades
    {
        void Init(IEventLog evl, IPositions ps, IOrders os);

        void NewTrade(ulong number, DateTime dt,
            string account, string strategy, string ticker,
            TradeOperationEnum operation, int quantity, double price, string comment, ulong orderNumber,
            double commission);
        event NeedToOserverEventHandler NeedToObserverEvent;

        // BackTest
        void ClearSomeData(int howMany);
    }

    public interface ITrades3
    {
        bool Add(ITrade3 t);
    }

    public interface ITrade2 : Containers3.IContainerItem<string>
    {
        IStrategy Strategy { get; set; }

        string AccountCode { get; }
        string TickerBoard { get; }
        string TickerCode { get; }

        DateTime DT { get; }
        long Number { get; }

        TradeStatusEnum Status { get; set; }

        TradeOperationEnum TradeOperation { get; }
        TradeOperationEnum Operation { get; }
        long Quantity { get; }
        decimal Price { get; }

        long Position { get; }

        DateTime Registered { get; }

        IOrder Order { get; set; }
        long OrderNumber { get; }

        string OrderKey { get; }

        string ToString();
    }

    public interface ITradeBase : Containers5.IHaveKey<string>, IHaveId<int>
    {
      //  IStrategy Strategy { get; set; }

        //string AccountCode { get; }
        //string TickerBoard { get; }
        //string TickerCode { get; }

        DateTime DT { get; }

        // ulong Number { get; }

        // TradeStatusEnum Status { get; set; }

        TradeOperationEnum Operation { get; }

        long Quantity { get; }
        decimal Price { get; }

        //DateTime Registered { get; }

        //IOrder3 Order { get; set; }
        // ulong OrderNumber { get; }

        //string OrderKey { get; }

        //string ToString();
        //IStrategyBase Strategy { get; set; }
        int StrategyId { get; }
    }
    public interface ITradeDb : ITradeBase
    {
        decimal Number { get; set; }
        decimal OrderNumber { get; set; }
    }
    public interface ITrade3 :  ITradeBase //Containers5.IHaveKey<string> // , INotifyPropertyChanged
    {
        IStrategy Strategy { get; set; }

        string AccountCode { get; }
        string TickerBoard { get; }
        string TickerCode { get; }
        int TimeInt { get; }
        TradeStatusEnum Status { get; set; }
        ulong Number { get; }
        ulong OrderNumber { get; }
        long Position { get; }

        DateTime Registered { get; }

        IOrder3 Order { get; set; }

        string OrderKey { get; }
        string Comment { get; set; }

        string StratTicker { get; }
        string ShortInfo { get; }
        string ShortDescription { get; }

        string ElapsedAllStr { get; }
        string ToString();

        DateTime Resolved { get; set; }
        DateTime Confirmed { get; set; }
    }
}
