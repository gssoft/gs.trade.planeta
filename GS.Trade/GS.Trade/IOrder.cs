using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GS.Interfaces;
using GS.Status;

namespace GS.Trade
{
    //public enum OrderStatusEnum : short
    //{
    //    All = 100, Unknown = 0, Registered = 1, Activated = 2, Filled = 3, PartlyFilled = 5, Canceled = 6,
    //    Confirmed = 7, Sended = 8, Completed = 9, InUse = 10, ReadyToUse = 11,
    //    NotRegistered = -2,
    //    Rejected = -7,  NotSended = -8,
    //    PendingToActivate = 12, PendingToCancel = 13, NeedSend = 14, NeedReSend = 15
    //}
    public enum OrderStatusEnum : short
    {
        Unknown = 0, All,  Registered, Activated, Filled, PartlyFilled, Canceled,
        Confirmed, Sended, Completed, InUse, ReadyToUse,
        NotRegistered,
        Rejected, NotSended,
        PendingToActivate, PendingToCancel, NeedSend, NeedReSend
    }

    public enum OrderErrorMsg : short
    {
        Unknown = 0, Ok = 1, Empty = 2,
        NoMargin = -1, CannotCancel = -2, NotSended = -3, NotConnected = -4,
        AnyReason = -5, OrderNumberIsEmpty = -6, UnknownReplyCode = -7, TransactionSendFailure = -8,
        UnknownSendTransFailure = -9, AddToListFailure = -10
    }
    public enum OrderTypeEnum : short { Unknown = 0, Limit = 1, Stop = 2, StopLimit = 3, Market = 4, All = 5 }
    //public enum OperationEnum : short { Unknown = 0, Buy = +1, Sell = -1, All = 2 }

    public enum OrderTransactionActionEnum : short { Unknown = 0, SetOrder, CancelOrder }

    public enum OrderOperationEnum : short { Unknown = 0, Buy = +1, Sell = -1, All = 2 }

    public enum BackTestOrderExecutionMode { Optimistic = 1, Pessimistic = 2, Medium = 3 }

    public interface IOrders
    {
        IEnumerable<IOrder> ActiveOrders { get; }
        IEnumerable<IOrder> ClosedOrders { get; }

        IOrder CreateOrder(IStrategy strategy, ulong number, DateTime dt,
                                OrderOperationEnum operation, OrderTypeEnum ordertype, OrderStatusEnum status,
                                double stopprice, double limitprice, long quantity, string comment);

        IOrder RegisterOrder(IStrategy strategy,
            OrderOperationEnum operation, OrderTypeEnum ordertype,
            double stopprice, double limitprice, long quantity, string comment);

        //IEnumerable<Containers3.IContainerItem<string>> ActiveOrders { get; }
        //IEnumerable<Containers3.IContainerItem<string>> ClosedOrders { get; }

        void GetOrders(OrderTypeEnum orderType, OrderOperationEnum operation, OrderStatusEnum orderStatus, string tradeKey,
            IList<IOrder> ol);

        void Init(IEventLog evl, IPositions ps, ITrades ts);
        BackTestOrderExecutionMode BackOrderExecMode { get; set; }
        void ExecuteTick(DateTime dt, string tickerkey, double price, double bid, double ask);
        long MaxIndex { get; }
        void GetActiveOrders(long index, List<IOrder> lst); // for UI_Windows
        void GetAllFilledOrCancelled(List<IOrder> os);
        void GetFilledOrCancelOrders(long index, List<IOrder> os);

        void CancelOrder(ulong number);

        // BackTest
        void NewTick(DateTime dt, string tickerkey, double price);
        void ClearSomeData(int howMany);

        event EventHandler<INewOrderStatusEventArgs> NewOrderStatusEvent; 
    }
    public interface IOrders3
    {
        IEnumerable<IOrder3> ActiveOrders { get; }
        IEnumerable<IOrder3> ClosedOrders { get; } 
        IEnumerable<IOrder3> ValidOrders { get; }

        IEnumerable<IOrder3> ActiveOrdersSoft { get; }
        IEnumerable<IOrder3> ClosedOrdersSoft { get; }
        IEnumerable<IOrder3> ValidOrdersSoft { get; }

        bool IsAnyAciveOrders { get; }
        bool IsAnyValidOrders { get; }

        IOrder3 CreateOrder(IStrategy strategy, ulong number, DateTime dt,
                                OrderOperationEnum operation, OrderTypeEnum ordertype, OrderStatusEnum status,
                                double stopprice, double limitprice, long quantity, string comment);

        IOrder3 RegisterOrder(IStrategy strategy,
                                OrderOperationEnum operation, OrderTypeEnum ordertype,
                                double stopprice, double limitprice, long quantity, string comment);
        IOrder3 CreateOrder(IStrategy strategy,
                                OrderOperationEnum operation, OrderTypeEnum ordertype,
                                double stopprice, double limitprice, long quantity, string comment);

        //IEnumerable<Containers3.IContainerItem<string>> ActiveOrders { get; }
        //IEnumerable<Containers3.IContainerItem<string>> ClosedOrders { get; }

        void GetOrders(OrderTypeEnum orderType, OrderOperationEnum operation, OrderStatusEnum orderStatus, string tradeKey,
            IList<IOrder3> ol);

        bool Add(IOrder3 o);
        bool RemoveNoKey(IOrder3 o);
    }
    public interface ISimulateOrders : IOrders3
    {
        void ExecuteTick(DateTime dt, string tickerkey, double price, double bid, double ask);
    }

    public interface IOrder
    {
        long MyIndex { get; }

        IStrategy StrategyStra { get; set; }
        string Strategy { get; set; }
        ulong Number { get; set; }

        OrderOperationEnum Operation { get; }
        OrderTypeEnum OrderType { get; }

        double StopPrice { get; }
        double LimitPrice { get; }

        long Quantity { get; }
        long Rest { get; set; }

        OrderStatusEnum Status { get; set; }

        bool IsLimit { get; }
        bool IsStopLimit { get; }

        bool IsValid { get; }
    }

    public interface IOrderBase : Containers5.IHaveKey<string>, IHaveId<long>
    {
        // ulong Number { get; set; }

        OrderStatusEnum Status { get; set; }

        OrderOperationEnum Operation { get; set; }
        OrderTypeEnum OrderType { get; set; }

        double StopPrice { get; }
        double LimitPrice { get; }
        double FilledPrice { get; }

        long Quantity { get; }
        long Rest { get; set; }
        double Amount { get; }

        string TrMessage { get; set; }
        // int TrReplyCode { get; set; }
    }

    public interface IOrderDb : IOrderBase
    {
        decimal Number { get; set; }
        string StrategyKey { get; }
    }
    public interface IOrder3 : IOrderBase //Containers5.IHaveKey<string>  //GS.Containers3.IContainerItem<string>
    {
        ulong Number { get; set; }
        IStrategy Strategy { get; set; }
        ITicker Ticker { get; }

        string Suid { get; }

        UInt32 TransId { get; set; }

        string AccountCode { get; }
        string TickerBoard { get; }
        string TickerCode { get; }

        string TickerKey { get; }

        //OrderOperationEnum Operation { get; }
        //OrderTypeEnum OrderType { get; }

        //double StopPrice { get; }
        //double LimitPrice { get; }
        //double FilledPrice { get; }

        //long Quantity { get;  }
        //long Rest { get; set; }
        //double Amount { get; }
        
        //OrderStatusEnum Status { get; set; }
        BusyStatusEnum BusyStatus { get; set; }
        OrderErrorMsg ErrorMsg { get; set; }
        OrderTransactionActionEnum TransactionAction { get; set; }
        OrderStatusEnum TransactionStatus { get; set; }
        int TrReplyCode { get; set; }
        int AttemptsToSend { get; set; }

        string LimitPriceString { get; }
        string LimitPriceStringF { get; }

        string StopPriceString { get; }
        string StopPriceStringF { get; }

        DateTime Created { get; set; }
        DateTime Registered { get; set; }
        DateTime Sended { get; set; }
        DateTime Confirmed { get; set; }
        DateTime Activated { get; set; }
        DateTime Filled { get; set; }
        DateTime Canceled { get; set; }
        DateTime SendTimeOutDT { get; }
        string Comment { get; set; }

        TimeSpan ExActivateTime { get; set; }
        TimeSpan ExCancelTime { get; set; }
        TimeSpan FilledTime { get; set; }

        long MyIndex { get; set; }

        bool IsRegistered { get;}
        bool IsActive { get; }
        bool IsActiveSoft { get; }

        bool IsBuy { get; }
        bool IsSell { get; }

        bool IsClosed { get; }
        bool IsClosedSoft { get; }
        bool IsValid { get; }
        bool IsValidSoft { get; }
        bool IsLimit { get; }
        bool IsStopLimit { get; }

        string TimeDateString { get; }

        OrderOperationEnum OperationString { get; }
        OrderTypeEnum OrderTypeString { get; }

        void SetStatus(OrderStatusEnum status, string comment);

        string StratTicker { get; }
        string ShortInfo { get; }
        string ShortDescription { get; }

        string StrategyTimeIntTickerString { get; }

        bool IsRegisteredTimeOutExpired { get; }
        bool IsSendTimeOutExpire(DateTime dt);
        void SetSendTimeOut(DateTime dt);
        void ClearSendTimeOut();

        IOrder3 Clone();
        string ToString();

    }

    public interface INewOrderStatusEventArgs
    {
        IOrder Order { get;  }
    }
}
