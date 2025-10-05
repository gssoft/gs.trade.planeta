using System;
using System.Globalization;
using GS.Extension;

namespace GS.Trade.Trades.Orders2
{
    //public class Order2 : IOrder2
    //{
    //     /*
    //    public enum OrderStatusEnum : short
    //    {
    //        All = 0, Unknown = -1, Registered = 1, Active = 2, Filled = 3, PartlyFilled = 5, Cancel = 6
    //    }
    //    public enum OrderTypeEnum : short { Unknown = 0, Limit = 1, Stop = 2, StopLimit = 3, Market = 4, All = 5 }
    //    public enum OperationEnum : short { Unknown = 0, Buy = +1, Sell = -1, All = 2 }
    //    */
    //    public static OperationEnum OperationToEnum( short operation )
    //    {
    //        return  operation == +1 ? OperationEnum.Buy: operation == -1 ? OperationEnum.Sell : OperationEnum.Unknown;
    //    }
    //    //public event PropertyChangedEventHandler PropertyChanged;
    //    //public void OnPropertyChanged(PropertyChangedEventArgs e)
    //    //{
    //    //    if (PropertyChanged != null)
    //    //        PropertyChanged(this, e);
    //    //}

    //    public Orders Orders { get; set; }

    //    public IStrategy Strategy { get; set; }
    //    public IAccount Account { get { return Strategy != null ? Strategy.Account : null; } }
    //    public ITicker Ticker { get { return Strategy != null ? Strategy.Ticker : null; } }

    //    public string StrategyCode { get { return Strategy != null ? Strategy.Code : "Unknown"; } }

    //    public string TickerBoard { get { return Ticker != null ? Ticker.ClassCode : (TickerBoardKey.HasValue() ? TickerBoardKey : "Unknown"); } }
    //    public string TickerCode { get { return Ticker != null ? Ticker.Code : (TickerKey.HasValue() ? TickerKey : "Unknown"); } }

    //    public string AccountCode { get { return Account != null ? Account.Code : (AccountKey.HasValue() ? AccountKey : "Unknown"); } }
    //    public string AccountKey { get; set; }

    //    public string TickerBoardKey { get; set; }
    //    public string TickerKey { get; set; }

    //    public long Number { get; set; }

    //    public DateTime DateTime { get; set; }
    //    public DateTime Created { get; set; }
    //    public DateTime Registered { get; set; }
    //    public DateTime Sended { get; set; }
    //    public DateTime Confirmed { get; set; }

    //    public DateTime Activated { get; set; }
    //    public DateTime Filled { get; set; }
    //    public DateTime Canceled { get; set; }

    //    public OperationEnum Operation { get; set; }
    //    public OrderTypeEnum OrderType { get; set; }
        
    //    public double StopPrice { get; set; }
    //    public double LimitPrice { get; set; }

    //    public long Quantity { get; set; }
    //    public long Rest { get; set; }
    //    public double Amount { get { return Quantity * LimitPrice; } }
    //    private OrderStatusEnum _status;

    //    public OrderStatusEnum Status
    //    {
    //        get { return _status; }
    //        set
    //        {
    //            if( Orders != null) MyIndex = ++Orders.MaxIndex;
    //            _status = value;
    //            UpdateStatusDateTime(_status);
    //        }
    //    }

    //    public OrderErrorMsg ErrorMsg { get; set; }

    //    public UInt32 TransId { get; set; }
    //    public string Comment { get; set; }
    //    public int Mode { get; set; }

    //    public TimeSpan ExActivateTime { get; set; }
    //    public TimeSpan ExCancelTime { get; set; }

    //    public TimeSpan RegisterTime { get; set; }
    //    public TimeSpan SendTime { get; set; }
    //    public TimeSpan ConfirmTime { get; set; }
    //    public TimeSpan ActivateTime { get; set; }
    //    public TimeSpan CancelTime { get; set; }
    //    public DateTime ExpireDate { get; set; }

    //    public long MyIndex { get; set; }

    //    public bool IsActive
    //    {
    //        get { return Status == OrderStatusEnum.Activated; }
    //    }
    //    public bool IsValid
    //    {
    //        get 
    //        { 
    //            return      Status == OrderStatusEnum.Activated ||
    //                        Status == OrderStatusEnum.Sended ||
    //                        Status == OrderStatusEnum.Registered ||
    //                        Status == OrderStatusEnum.Confirmed ||
    //                        Status == OrderStatusEnum.PartlyFilled
    //                        ;
    //        }
    //    }

    //    public bool IsClosed {
    //        get
    //        {
    //            return Status == OrderStatusEnum.Filled ||
    //                   Status == OrderStatusEnum.Canceled ||
    //                   Status == OrderStatusEnum.Rejected
    //                   ;
    //        }
    //    }

    //    public bool IsLimit
    //    {
    //        get { return OrderType == OrderTypeEnum.Limit; }
    //    }
    //    public bool IsStopLimit
    //    {
    //        get { return OrderType == OrderTypeEnum.StopLimit || OrderType == OrderTypeEnum.Stop; }
    //    }

    //    public bool IsBuy
    //    {
    //        get { return Operation == OperationEnum.Buy; }
    //    }
    //    public bool IsSell
    //    {
    //        get { return Operation == OperationEnum.Sell; }
    //    }

    //    // Order Field frm FIle
        
    //    public string StrategyKey { get { return General.StrategyKey(Account.Code,Strategy.Code,Ticker.Code); } }
    //    public string Key { get { return (Number + "." + AccountCode).TrimUpper(); } }
    //    public string TradeKey  {  get { return Strategy.Code + Account.Code + Ticker.Code; } }

    //    public long PositionQuantity { get { return Quantity * (short)Operation; } }
    //    public long PositionRest { get { return Rest * (short)Operation; } }

    //    //public string OperationString { get { return Operation > 0 ? "BUY" : "SELL"; } }
    //    public OperationEnum OperationString { get { return Operation; } }
    //    public OrderTypeEnum OrderTypeString { get { return OrderType; } }
    //    /*
    //    public string AmountString { get { return String.Format("{0:N2}", Amount); } }
    //    public string LimitPriceString { get { return String.Format("{0:N2}", LimitPrice); } }
    //    public string StopPriceString { get { return String.Format("{0:N2}", StopPrice); } }
    //    */
    //    public string DateTimeString { get { return DateTime.ToString("G"); } }
    //    public string TimeDateString
    //    {
    //        get { return DateTime.ToString("T") + ' ' + DateTime.ToString("d"); }
    //    }

    //    public string PositionQuantityString { get { return PositionQuantity.ToString("N0"); } }
    //    public string PositionRestString { get { return PositionRest.ToString("N0"); } }

    //    public string AmountString { get { return Amount.ToString(Ticker!=null?Ticker.FormatM:"N2"); } }

    //    public string LimitPriceString { get { return LimitPrice.ToString(Ticker!=null?Ticker.Format:"N2"); } }
    //    public string LimitPriceStringF { get { return LimitPrice.ToString(Ticker!=null?Ticker.FormatF:"N2"); } }

    //    public string StopPriceString { get { return StopPrice.ToString(Ticker!=null?Ticker.Format:"N2"); } }
    //    public string StopPriceStringF { get { return StopPrice.ToString(Ticker!=null?Ticker.FormatF:"N2"); } }

    //    public OrderStatusEnum StatusString { get { return Status; } }
    //    public string ModeString { get { return (Mode == 0) ? "New" : (Mode == 1) ? "Init" : "End Init"; } }

    //    //public string ActivateTimeString { get { return String.Format("{0:HH:mm:ss}", ActivateTime);/* ActivateTime.ToString("HH:mm:ss:fff");*/ } }
    //    //public string CancelTimeString { get { return String.Format("{0:HH:mm:ss}", CancelTime);/*CancelTime.ToString("HH:mm:ss:fff");*/ } }

    //    public string ActivateTimeString { get { return ActivateTime.ToString("T"); } }
    //    public string CancelTimeString { get { return CancelTime.ToString("T"); } }

    //    public string ExpireDateString { get { return ExpireDate.ToString("G"); } }

    //    // public string PnLString { get { return String.Format("{0:N2}", Pnl); } }

    //    //public string ShortOrderInfo 
    //    //{
    //    //    get
    //    //    {
    //    //        return OrderType == OrderTypeEnum.Limit
    //    //                ? "Lim @ " + LimitPriceString
    //    //                : OrderType == OrderTypeEnum.StopLimit
    //    //                    ? "Limit @ " + LimitPriceString + " Stop @ " + StopPriceString
    //    //                    : "";
    //    //    }
    //    //}
    //    public string ShortOrderInfo
    //    {
    //        get
    //        {
    //            return Operation + " " + 
    //                    Ticker + Quantity.ToString(CultureInfo.InvariantCulture).WithSqBrackets() + 
    //                    OrderType + 
    //                    (OrderType == OrderTypeEnum.Limit
    //                    ? " @ " + LimitPriceString
    //                    : OrderType == OrderTypeEnum.StopLimit
    //                        ? string.Format(" @ {0} Stop @ {1}", LimitPriceString, StopPriceString)
    //                        : "");
    //        }
    //    }

    //    public string ShortInfo
    //    {
    //        //get { return String.Format("{0}: {1} {2} {3} {4} {5}",
    //        //         Ticker, Operation, OrderType, Quantity, ShortOrderInfo, 
    //        //                            Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets()); }
    //        get { return String.Format("{0} {1}",
    //                 ShortOrderInfo, Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets()); }
    //    }

    //    public string StratTicker {
    //        get { return Strategy != null ? Strategy.StrategyTickerString : "Unknown" ; }
    //    }

    //    private void UpdateStatusDateTime(OrderStatusEnum status)
    //    {
    //        switch (status)
    //        {       
    //            case OrderStatusEnum.Canceled:
    //                Canceled = DateTime.Now;
    //                break;
    //            case OrderStatusEnum.Filled:
    //                Filled = DateTime.Now;
    //                break;
    //            case OrderStatusEnum.Activated:
    //                Activated = DateTime.Now;
    //                break;
    //            case OrderStatusEnum.Registered:
    //                Registered = DateTime.Now;
    //                break;
    //            case OrderStatusEnum.NotSended:
    //            case OrderStatusEnum.Sended:
    //                Sended = DateTime.Now;
    //                break;
    //            case OrderStatusEnum.Confirmed:
    //                Confirmed = DateTime.Now;
    //                break;
    //        }
    //        DateTime = DateTime.Now;
    //    }

    //    public void SetStatus(OrderStatusEnum status, string comment)
    //    {
    //        Status = status;
    //        if (!string.IsNullOrWhiteSpace(comment)) Comment +=  comment + "; ";
    //        switch (status)
    //        {
    //            case OrderStatusEnum.Canceled:
    //            case OrderStatusEnum.Filled:
    //                CancelTime = DateTime.Now.TimeOfDay;
    //                //if(Orders != null) 
    //                //    Orders.RemoveFilled(this);
    //                break;
    //            case OrderStatusEnum.Activated:
    //                ActivateTime = DateTime.Now.TimeOfDay;
    //                break;
    //            case OrderStatusEnum.Registered:
    //                RegisterTime = DateTime.Now.TimeOfDay;
    //                break;
    //            case OrderStatusEnum.Sended:
    //                SendTime = DateTime.Now.TimeOfDay;
    //                break;
    //            case OrderStatusEnum.Confirmed:
    //                ConfirmTime = DateTime.Now.TimeOfDay;
    //                break;
    //        }
    //        //if( Orders != null) Orders.SetNeedToObserver();
    //    }

    //    public override string ToString()
    //    {
    //        return String.Format("{0:G} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} TrId={13} {14:T}",
    //                             DateTime, Number, AccountCode, StrategyCode, 
    //                             TickerCode, Operation, OrderType, StopPrice, LimitPrice, PositionQuantity, PositionRest, 
    //                             StatusString, Comment, TransId, CancelTime);
    //    }
    //}
}
