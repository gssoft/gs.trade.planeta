using System;
using System.Globalization;
using GS.Cloning;
using GS.Extension;
using GS.Status;

namespace GS.Trade.Trades.Orders3
{
    public class Order3 : IOrder3
    {
         /*
        public enum OrderStatusEnum : short
        {
            All = 0, Unknown = -1, Registered = 1, Active = 2, Filled = 3, PartlyFilled = 5, Cancel = 6
        }
        public enum OrderTypeEnum : short { Unknown = 0, Limit = 1, Stop = 2, StopLimit = 3, Market = 4, All = 5 }
        public enum OperationEnum : short { Unknown = 0, Buy = +1, Sell = -1, All = 2 }
        */
        public static OrderOperationEnum OperationToEnum( short operation )
        {
            return  operation == +1 
                        ? OrderOperationEnum.Buy
                        : (operation == -1 
                            ? OrderOperationEnum.Sell 
                            : OrderOperationEnum.Unknown);
        }
        //public event PropertyChangedEventHandler PropertyChanged;
        //public void OnPropertyChanged(PropertyChangedEventArgs e)
        //{
        //    if (PropertyChanged != null)
        //        PropertyChanged(this, e);
        //}
        public IOrder3 Clone()
        {
            var o = new Order3
            {
                Orders = Orders,
                Id = Id,
                Suid = Suid,
                Strategy = Strategy,
                // Account = Account,
                AccountKey = AccountKey,
                TickerBoardKey = TickerBoardKey,
                TickerKey = TickerKey,
                Number = Number,
                DateTime = DateTime,
                Created = Created,
                Registered = Sended,
                Confirmed = Confirmed,
                Activated = Activated,
                Filled = Filled,
                Canceled = Canceled,
                Operation = Operation,
                OrderType = OrderType,
                StopPrice = StopPrice,
                LimitPrice = LimitPrice,
                FilledPrice = FilledPrice,
                Quantity = Quantity,
                Rest = Rest,
                BusyStatus = BusyStatus,
                Status = Status,
                ErrorMsg = ErrorMsg,
                TransactionAction = TransactionAction,
                TransactionStatus = TransactionStatus,
                TrMessage = TrMessage,
                TrReplyCode = TrReplyCode,
                TransId = TransId,
                Comment = Comment,
                Mode = Mode,
                LastStatusChangedTime = LastStatusChangedTime,
                LastStatusChangedDateTime = LastStatusChangedDateTime,
                ExpireDate = ExpireDate,
                SendTimeOutDT = SendTimeOutDT
            };
            return o;
        }
        public Orders Orders { get; set; }
        public string Suid { get; set; }
        public long Id { get; set; }
        public IStrategy Strategy { get; set; }
        public IAccount Account => Strategy?.Account;
        public ITicker Ticker => Strategy?.Ticker;
        public string StrategyCode => Strategy != null ? Strategy.Code : "Unknown";
        public string TickerBoard => Ticker != null ? Ticker.ClassCode : (TickerBoardKey.HasValue() ? TickerBoardKey : "Unknown");
        public string TickerCode => Ticker != null ? Ticker.Code : (TickerKey.HasValue() ? TickerKey : "Unknown");
        public string AccountCode => Account != null ? Account.Code : (AccountKey.HasValue() ? AccountKey : "Unknown");
        public string AccountKey { get; set; }
        public string TickerBoardKey { get; set; }
        public string TickerKey { get; set; }
        public ulong Number { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime Created { get; set; }
        public DateTime Registered { get; set; }
        public DateTime Sended { get; set; }
        public DateTime Confirmed { get; set; }

        private DateTime _activated;
        public DateTime Activated
        {
            get { return _activated; }
            set
            {
                _activated = value;
                LastStatusChangedDateTime = value;
            }
        }
        private DateTime _filled;
        public DateTime Filled
        {
            get { return _filled; }
            set
            {
                _filled = value;
                LastStatusChangedDateTime = value;
            }
        }
        private DateTime _canceled;
        public DateTime Canceled
        {
            get { return _canceled; }
            set
            {
                _canceled = value;
                LastStatusChangedDateTime = value;
            }
        }
        public DateTime SendTimeOutDT { get;  set; }
        public OrderOperationEnum Operation { get; set; }
        public OrderTypeEnum OrderType { get; set; }  
        
        public double StopPrice { get; set; }
        public double LimitPrice { get; set; }
        public double FilledPrice { get; set; }

        public long Quantity { get; set; }
        public long Rest { get; set; }
        public double Amount => Quantity * LimitPrice;

        public BusyStatusEnum BusyStatus { get; set; }

        private OrderStatusEnum _status;
        public OrderStatusEnum Status
        {
            get { return _status; }
            set
            {
                if( Orders != null) MyIndex = ++Orders.MaxIndex;

                _status = value;
                UpdateStatusDateTime(_status);
            }
        }
        public OrderErrorMsg ErrorMsg { get; set; }
        public OrderTransactionActionEnum TransactionAction { get; set; }
        public OrderStatusEnum TransactionStatus { get; set; }
        public string TrMessage { get; set; }
        public int TrReplyCode { get; set; }
        public UInt32 TransId { get; set; }
        public string Comment { get; set; }
        public int Mode { get; set; }
        public TimeSpan ExActivateTime
        {
            get { return _exActivateTime; }
            set
            {
                _exActivateTime = value;
                ActivateTime = value;
            }
        }
        public TimeSpan ExCancelTime
        {
            get { return _exCancelTime; }
            set
            {
                _exCancelTime = value;
                CancelTime = value;
            }
        }
        public TimeSpan RegisterTime { get; set; }
        public TimeSpan SendTime { get; set; }
        public TimeSpan ConfirmTime { get; set; }
        public TimeSpan ActivateTime
        {
            get { return _activateTime; }
            set
            {
                _activateTime = value;
                LastStatusChangedTime = value;
            }
        }
        public TimeSpan CancelTime
        {
            get { return _cancelTime; }
            set
            {
                _cancelTime = value;
                LastStatusChangedTime = value;
            }
        }
        private TimeSpan _filledTime;
        public TimeSpan FilledTime
        {
            get { return _filledTime; }
            set
            {
                _filledTime = value;
                LastStatusChangedTime = value;
            }
        }
        public int AttemptsToSend { get; set; }
        public TimeSpan LastStatusChangedTime { get; set; }
        public DateTime LastStatusChangedDateTime { get; set; }
        public DateTime ExpireDate { get; set; }
        public long MyIndex { get; set; }
        public bool IsRegistered => Status == OrderStatusEnum.Registered;
        public bool IsSended => Status == OrderStatusEnum.Sended;
        public bool IsNotSended => Status == OrderStatusEnum.NotSended;
        public bool IsActive => Status == OrderStatusEnum.Activated;
        public bool IsActiveSoft => Status == OrderStatusEnum.Activated ||
                                    Status == OrderStatusEnum.PendingToActivate;

        public bool IsValid => Status == OrderStatusEnum.Activated ||
                               Status == OrderStatusEnum.Sended ||
                               Status == OrderStatusEnum.Registered ||
                               Status == OrderStatusEnum.Confirmed ||
                               Status == OrderStatusEnum.PartlyFilled;

        public bool IsValidSoft =>  Status == OrderStatusEnum.Activated ||
                                    Status == OrderStatusEnum.Sended ||
                                    Status == OrderStatusEnum.Registered ||
                                    Status == OrderStatusEnum.Confirmed ||
                                    Status == OrderStatusEnum.PartlyFilled ||
                                    Status == OrderStatusEnum.PendingToActivate;

        public bool IsClosed => Status == OrderStatusEnum.Filled ||
                                Status == OrderStatusEnum.Canceled ||
                                Status == OrderStatusEnum.Rejected ||
                                Status == OrderStatusEnum.NotSended;

        public bool IsClosedSoft => Status == OrderStatusEnum.Filled ||
                                    Status == OrderStatusEnum.Canceled ||
                                    Status == OrderStatusEnum.PendingToCancel ||
                                    Status == OrderStatusEnum.Rejected ||
                                    Status == OrderStatusEnum.NotSended;

        public bool IsWaitForActivate => Status == OrderStatusEnum.Sended ||
                                         Status == OrderStatusEnum.Registered ||
                                         Status == OrderStatusEnum.Confirmed;

        public bool IsLimit => OrderType == OrderTypeEnum.Limit;

        public bool IsStopLimit => OrderType == OrderTypeEnum.StopLimit || OrderType == OrderTypeEnum.Stop;

        public bool IsBuy => Operation == OrderOperationEnum.Buy;

        public bool IsSell => Operation == OrderOperationEnum.Sell;

        // Order Field frm FIle
        // public string StrategyKey => General.StrategyKey(Account.Code, Strategy.Code, Ticker.Code);
        public string Key => (Number + "." + AccountCode).TrimUpper();
        // public string Key => Suid;
        public string TradeKey => Strategy.Code + Account.Code + Ticker.Code;

        public long PositionQuantity => Quantity * (short)Operation;
        public long PositionRest => Rest * (short)Operation;

        //public string OperationString { get { return Operation > 0 ? "BUY" : "SELL"; } }
        public OrderOperationEnum OperationString => Operation;
        public OrderTypeEnum OrderTypeString => OrderType;
        /*
        public string AmountString { get { return String.Format("{0:N2}", Amount); } }
        public string LimitPriceString { get { return String.Format("{0:N2}", LimitPrice); } }
        public string StopPriceString { get { return String.Format("{0:N2}", StopPrice); } }
        */
        public string DateTimeString => DateTime.ToString("G");
        public string TimeDateString => DateTime.ToString("T") + ' ' + DateTime.ToString("d");

        public string PositionQuantityString => PositionQuantity.ToString("N0");
        public string PositionRestString => PositionRest.ToString("N0");

        public string AmountString => Amount.ToString(Ticker!=null?Ticker.FormatM:"N2");

        // Price Strings Formatted
        public string OrderPriceStr => OrderType != OrderTypeEnum.StopLimit
            ? LimitPriceString
            : $"StPr: {StopPriceString}, LmtPr: {LimitPriceString}";
        public string LimitPriceString => LimitPrice.ToString(Ticker!=null?Ticker.Format:"N2");
        public string LimitPriceStringF => LimitPrice.ToString(Ticker!=null?Ticker.FormatF:"N2");

        public string StopPriceString => StopPrice.ToString(Ticker!=null?Ticker.Format:"N2");
        public string StopPriceStringF => StopPrice.ToString(Ticker!=null?Ticker.FormatF:"N2");

        public OrderStatusEnum StatusString => Status;
        public string ModeString => (Mode == 0) ? "New" : (Mode == 1) ? "Init" : "End Init";

        //public string ActivateTimeString { get { return String.Format("{0:HH:mm:ss}", ActivateTime);/* ActivateTime.ToString("HH:mm:ss:fff");*/ } }
        //public string CancelTimeString { get { return String.Format("{0:HH:mm:ss}", CancelTime);/*CancelTime.ToString("HH:mm:ss:fff");*/ } }

        public string ActivateTimeString => ActivateTime.ToString("T");
        public string CancelTimeString => CancelTime.ToString("T");

        public string ExpireDateString => ExpireDate.ToString("G");

        // public string PnLString { get { return String.Format("{0:N2}", Pnl); } }

        //public string ShortOrderInfo 
        //{
        //    get
        //    {
        //        return OrderType == OrderTypeEnum.Limit
        //                ? "Lim @ " + LimitPriceString
        //                : OrderType == OrderTypeEnum.StopLimit
        //                    ? "Limit @ " + LimitPriceString + " Stop @ " + StopPriceString
        //                    : "";
        //    }
        //}
        public string ShortOrderInfo => Operation + " " + 
                                        Ticker.Code + Quantity.ToString(CultureInfo.InvariantCulture).WithSqBrackets() + 
                                        OrderType + 
                                        (OrderType == OrderTypeEnum.Limit
                                            ? " @ " + LimitPriceString
                                            : OrderType == OrderTypeEnum.StopLimit
                                                ? $" @ {LimitPriceString} Stop @ {StopPriceString}"
                                                : "");

        //public string ShortInfo => string.Format("{1} {0}",
        //    ShortOrderInfo, Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets());

        // public string ShortInfo => $"Order:[{Number}] Rg:{DateTimeTickStrTodayCondition(Registered)} {Status} {OrderStatusDateTimeString(Status)} {TimeTickStr(LastStatusChangedDateTime - Registered)}" +
       //                            $" Tr:{TransactionAction} {TransactionStatus} {ErrorMsg}"; // {DateTimeTickStrTodayCondition(LastStatusChangedDateTime)}";
        public string ShortInfo => $"Order:[{Number}] {OrderStatusDateTimeString(Status)} {Status} {BusyStatus} {TransactionAction} {TransactionStatus} {ErrorMsg}"; // {DateTimeTickStrTodayCondition(LastStatusChangedDateTime)}";
        public string ShortDescription =>
            $"{OperationString} {Quantity} {TickerCode} @ {PriceString} {OrderType} " +
            $"Rg:{DateTimeTickStrTodayCondition(Registered)} {Status} " +
            $"{DateTimeTickStrTodayCondition(LastStatusChangedDateTime)} " +
            $"LT:{TimeTickStr(LastStatusChangedDateTime - Registered)}";

        //private bool IsLimit => OrderType == OrderTypeEnum.Limit;

        protected string PriceString => IsLimit 
            ? LimitPriceString 
            : (IsStopLimit ? $"{StopPriceString} {LimitPriceString}" : StopPriceString); 

        protected string RegisteredTimeStr => Registered.TimeOfDay.ToString(@"hh\:mm\:ss\.fff");
        protected string RegisteredDate => Registered.Date.ToString("yy-MM-dd");

        protected string OrderStatusDateTimeString(OrderStatusEnum status)
        {
            string str;
            switch (status)
            {
                case OrderStatusEnum.Registered:
                    str = DateTimeTickStrTodayCondition(Registered);
                    break;
                case OrderStatusEnum.NotSended:
                case OrderStatusEnum.Sended:
                    str = DateTimeTickStrTodayCondition(Sended);
                    break;
                case OrderStatusEnum.PendingToActivate:
                case OrderStatusEnum.Activated:
                case OrderStatusEnum.Rejected:
                    str = DateTimeTickStrTodayCondition(Activated);
                    break;
                case OrderStatusEnum.PendingToCancel:
                case OrderStatusEnum.Canceled:
                    str = DateTimeTickStrTodayCondition(Canceled);
                    break;
                case OrderStatusEnum.Filled:
                case OrderStatusEnum.PartlyFilled:
                    str = DateTimeTickStrTodayCondition(Filled);
                    break;
                case OrderStatusEnum.Confirmed:
                    str = DateTimeTickStrTodayCondition(Confirmed);
                    break;
                case OrderStatusEnum.All:
                    str = DateTimeTickStrTodayCondition(LastStatusChangedDateTime);
                    break;
                case OrderStatusEnum.Unknown:
                    str = DateTimeTickStrTodayCondition(LastStatusChangedDateTime);
                    break;
                case OrderStatusEnum.NeedSend:
                    str = DateTimeTickStrTodayCondition(LastStatusChangedDateTime);
                    break;
                case OrderStatusEnum.NeedReSend:
                    str = DateTimeTickStrTodayCondition(LastStatusChangedDateTime);
                    break;
                default:
                    str = DateTimeTickStrTodayCondition(LastStatusChangedDateTime);
                    break;
            }
            return str;
        }
        protected string TimeTickStr(TimeSpan tms)
        {
            return tms.ToString(@"hh\:mm\:ss\.fff");
        }
        protected string TimeTickStr(DateTime dt)
        {
            return dt.TimeOfDay.ToString(@"hh\:mm\:ss\.fff");
        }
        protected string DateTimeTickStr(DateTime dt)
        {
            return $"{dt.Date.ToString("yy-MM-dd")} {dt.TimeOfDay.ToString(@"hh\:mm\:ss\.fff")}";
        }
        protected string DateTimeTickStrTodayCondition(DateTime dt)
        {
            return DateTime.Now.Date == dt.Date
                ? TimeTickStr(dt)
                : DateTimeTickStr(dt);
        }
        public string StrategyTimeIntTickerString => Strategy != null ? Strategy.StrategyTimeIntTickerString : "Strategy Unknown";
        public string StratTicker => Strategy != null ? Strategy.StrategyTickerString : "Strategy Unknown";
        private void UpdateStatusDateTime(OrderStatusEnum status)
        {
            var dt = DateTime.Now;
            DateTime = dt;
            switch (status)
            {       
                case OrderStatusEnum.Canceled:
                    Canceled = dt;
                    LastStatusChangedDateTime = dt;
                    // BusyStatus = BusyStatusEnum.ReadyToUse;
                    break;
                case OrderStatusEnum.Filled:
                    Filled = dt;
                    LastStatusChangedDateTime = dt;
                    // BusyStatus = BusyStatusEnum.ReadyToUse;
                    break;
                case OrderStatusEnum.Activated:
                    Activated = dt;
                    LastStatusChangedDateTime = dt;
                    // BusyStatus = BusyStatusEnum.ReadyToUse;
                    break;
                case OrderStatusEnum.Registered:
                    Registered = dt;
                    LastStatusChangedDateTime = dt;
                    break;
                case OrderStatusEnum.NotSended:
                case OrderStatusEnum.Sended:
                    Sended = dt;
                    LastStatusChangedDateTime = dt;
                    break;
                case OrderStatusEnum.Confirmed:
                    Confirmed = dt;
                    LastStatusChangedDateTime = dt;
                    break;
                case OrderStatusEnum.Rejected:
                    Activated = dt;
                    LastStatusChangedDateTime = dt;
                    // BusyStatus = BusyStatusEnum.ReadyToUse;
                    break;
            }
        }
        public void SetStatus(OrderStatusEnum status, string comment)
        {
            Status = status;
            if (comment.HasValue()) Comment +=  comment + "; ";
            switch (Status)
            {
                case OrderStatusEnum.Canceled:
                case OrderStatusEnum.Filled:
                    CancelTime = DateTime.Now.TimeOfDay;
                    LastStatusChangedTime = CancelTime;
                    //if(Orders != null) 
                    //    Orders.RemoveFilled(this);
                    break;
                case OrderStatusEnum.Activated:
                    ActivateTime = DateTime.Now.TimeOfDay;
                    LastStatusChangedTime = ActivateTime;
                    break;
                case OrderStatusEnum.Registered:
                    RegisterTime = DateTime.Now.TimeOfDay;
                    LastStatusChangedTime = RegisterTime;
                    break;
                case OrderStatusEnum.Sended:
                    SendTime = DateTime.Now.TimeOfDay;
                    LastStatusChangedTime = SendTime;
                    break;
                case OrderStatusEnum.Confirmed:
                    ConfirmTime = DateTime.Now.TimeOfDay;
                    LastStatusChangedTime = ConfirmTime;
                    break;
            }
            //if( Orders != null) Orders.SetNeedToObserver();
        }
        public TimeSpan RegisteredAndActvateOrCancelTimeOut = new TimeSpan(0, 5, 0);
        private TimeSpan _exActivateTime;
        private TimeSpan _activateTime;
        private TimeSpan _cancelTime;
        private TimeSpan _exCancelTime;

        public bool IsRegisteredTimeOutExpired =>
            (IsRegistered || IsSended || IsNotSended) && 
            DateTime.Now - Registered > RegisteredAndActvateOrCancelTimeOut;

        protected TimeSpan TimeInWaitingForReply => DateTime.Now - Registered;

        public bool IsSendTimeOutExpire(DateTime dt)
        {
            return dt.IsGreaterThan(SendTimeOutDT);
        }
        public void SetSendTimeOut(DateTime dt)
        {
            SendTimeOutDT = dt;
        }
        public void ClearSendTimeOut()
        {
            SendTimeOutDT = DateTime.MaxValue;
        }

        public override string ToString()
        {
            return
                $"{DateTime:G} {Number} {AccountCode} {StrategyTimeIntTickerString} {Operation} {OrderType}" +
                $" {OrderPriceStr} {PositionQuantity} {PositionRest}" +
                $" {Status} {DateTimeTickStrTodayCondition(LastStatusChangedDateTime)} {Comment} TrId:{TransId} {Suid}";
        }
    }
}
