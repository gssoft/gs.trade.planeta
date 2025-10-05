using System;
using System.ComponentModel;
using System.Globalization;
using GS.Extension;

namespace GS.Trade.Trades
{
    public class Order : INotifyPropertyChanged, IOrder
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
            return  operation == +1 ? OrderOperationEnum.Buy: operation == -1 ? OrderOperationEnum.Sell : OrderOperationEnum.Unknown;
        }
        public static OrderOperationEnum OperationToEnum2(short operation)
        {
            return operation == +1 
                ? OrderOperationEnum.Buy 
                : (operation == -1 ? OrderOperationEnum.Sell : OrderOperationEnum.Unknown);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        public Orders Orders { get; set; }

        public IStrategy StrategyStra { get; set; }

        public string Account { get; set; }
        public string Strategy { get; set; }
        public string Ticker { get; set; }

        public ulong Number { get; set; }
        public DateTime DateTime { get; set; }
        public OrderOperationEnum Operation { get; set; }
        public OrderTypeEnum OrderType { get; set; }
        public double StopPrice { get; set; }
        public double LimitPrice { get; set; }
        public long Quantity { get; set; }
        public long Rest { get; set; }
        private double Amount { get { return Quantity * LimitPrice; } }
        private OrderStatusEnum _status;
        public OrderStatusEnum Status
        {
            get { return _status; }
            set
            {
                if( Orders != null) MyIndex = ++Orders.MaxIndex;
                _status = value;
            }
        }

        public ulong TransId { get; set; }
        public string Comment { get; set; }
        public int Mode { get; set; }

        public TimeSpan ExActivateTime { get; set; }
        public TimeSpan ExCancelTime { get; set; }

        public TimeSpan RegisterTime { get; set; }
        public TimeSpan SendTime { get; set; }
        public TimeSpan ConfirmTime { get; set; }
        public TimeSpan ActivateTime { get; set; }
        public TimeSpan CancelTime { get; set; }
        public DateTime ExpireDate { get; set; }

        

        public long MyIndex { get; set; }

        public bool IsActive
        {
            get { return Status == OrderStatusEnum.Activated; }
        }
        public bool IsValid
        {
            get { return    Status == OrderStatusEnum.Activated ||
                            Status == OrderStatusEnum.Sended ||
                            Status == OrderStatusEnum.Registered ||
                            Status == OrderStatusEnum.Confirmed ||
                            Status == OrderStatusEnum.PartlyFilled
                ; }
        }
        public bool IsLimit
        {
            get { return OrderType == OrderTypeEnum.Limit; }
        }
        public bool IsStopLimit
        {
            get { return OrderType == OrderTypeEnum.StopLimit || OrderType == OrderTypeEnum.Stop; }
        }

        public bool IsBuy
        {
            get { return Operation == OrderOperationEnum.Buy; }
        }
        public bool IsSell
        {
            get { return Operation == OrderOperationEnum.Sell; }
        }

        // Order Field frm FIle
        
        public string StrategyKey { get { return General.StrategyKey(Account,Strategy,Ticker); } }
        //public string Key { get { return General.Key(Account, Number); } }
        public string Key
        {
            get { return (Number + "." + Account).TrimUpper(); }
        }

        public string TradeKey  {  get { return Strategy + Account + Ticker; } }

        public long PositionQuantity { get { return Quantity * (short)Operation; } }
        public long PositionRest { get { return Rest * (short)Operation; } }

        //public string OperationString { get { return Operation > 0 ? "BUY" : "SELL"; } }
        public OrderOperationEnum OperationString { get { return Operation; } }
        public OrderTypeEnum OrderTypeString { get { return OrderType; } }
        /*
        public string AmountString { get { return String.Format("{0:N2}", Amount); } }
        public string LimitPriceString { get { return String.Format("{0:N2}", LimitPrice); } }
        public string StopPriceString { get { return String.Format("{0:N2}", StopPrice); } }
        */
        public string DateTimeString { get { return DateTime.ToString("G"); } }
        public string TimeDateString
        {
            get { return DateTime.ToString("T") + ' ' + DateTime.ToString("d"); }
        }

        public string PositionQuantityString { get { return ((short)Operation * Quantity).ToString("N0"); } }
        public string PositionRestString { get { return ((short)Operation * Rest).ToString("N0"); } }

        public string AmountString { get { return Amount.ToString("N2"); } }
        public string LimitPriceString { get { return LimitPrice.ToString("N2"); } }
        public string StopPriceString { get { return StopPrice.ToString("N2"); } }

        public OrderStatusEnum StatusString { get { return Status; } }
        public string ModeString { get { return (Mode == 0) ? "New" : (Mode == 1) ? "Init" : "End Init"; } }

        //public string ActivateTimeString { get { return String.Format("{0:HH:mm:ss}", ActivateTime);/* ActivateTime.ToString("HH:mm:ss:fff");*/ } }
        //public string CancelTimeString { get { return String.Format("{0:HH:mm:ss}", CancelTime);/*CancelTime.ToString("HH:mm:ss:fff");*/ } }

        public string ActivateTimeString { get { return ActivateTime.ToString("T"); } }
        public string CancelTimeString { get { return CancelTime.ToString("T"); } }

        public string ExpireDateString { get { return ExpireDate.ToString("G"); } }

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
        public string ShortOrderInfo
        {
            get
            {
                return Operation + " " + 
                        Ticker + Quantity.ToString(CultureInfo.InvariantCulture).WithSqBrackets() + 
                        OrderType + 
                        (OrderType == OrderTypeEnum.Limit
                        ? " @ " + LimitPriceString
                        : OrderType == OrderTypeEnum.StopLimit
                            ? string.Format(" @ {0} Stop @ {1}", LimitPriceString, StopPriceString)
                            : "");
            }
        }

        public string ShortInfo
        {
            //get { return String.Format("{0}: {1} {2} {3} {4} {5}",
            //         Ticker, Operation, OrderType, Quantity, ShortOrderInfo, 
            //                            Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets()); }
            get { return String.Format("{0} {1}",
                     ShortOrderInfo, Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets()); }
        }

        public string StratTicker {
            get { return Strategy + "." + Ticker; }
        }

        // *************************  Order Method  ************************************************
        public Order(
            ulong number, DateTime dt, int mode,
            string account, string strategy, string ticker, 
            OrderOperationEnum operation, OrderTypeEnum ordertype, double stopprice, double limitprice, int quantity, int rest,
            OrderStatusEnum status, ulong transId, string comment, TimeSpan activatetime, TimeSpan canceltime)
        {
            Account = account;
            Strategy = !string.IsNullOrEmpty(strategy) ? strategy : "999";

            Number = number;
            DateTime = dt;
            Ticker = ticker;

            Operation = operation;
            OrderType = ordertype;

            StopPrice = stopprice;
            LimitPrice = limitprice;

            Quantity = quantity;
            Rest = rest;

            //Status = status;
            _status = status;
            TransId = transId;
            Comment = comment;
            Mode = mode;

            ActivateTime = activatetime;
            CancelTime = canceltime;
            //ExpireDate = expiredate;
        }
        /*
        public Order(
            long number, DateTime dt, int mode,
            string account, string strategy, string ticker,
            OperationEnum operation, OrderTypeEnum ordertype, double stopprice, double limitprice, int quantity, int rest, 
            int status, long transId, string comment, TimeSpan activatetime, TimeSpan canceltime) 
        {
            Account = account;
            Strategy = !string.IsNullOrEmpty(strategy) ? strategy : "999";

            Number = number;
            DateTime = dt;
            Ticker = ticker;

            Operation = operation;
            OrderType = ordertype;

            StopPrice = stopprice;
            LimitPrice = limitprice;

            Quantity = quantity;
            Rest = rest;

            switch( status)
            {
                case 1:
                    Status = OrderStatusEnum.Active;
                    break;
                case 2:
                    Status = OrderStatusEnum.Canceled;
                    break;
                default:
                    Status = OrderStatusEnum.Filled;
                    break;
            }
            TransId = transId;
            Comment = comment;
            Mode = mode;

            ActivateTime = activatetime;
            CancelTime = canceltime;
            //ExpireDate = expiredate;
        }
        */
        public Order(
            ulong number, DateTime dt,
            string account, string strategy,
            string ticker, OrderOperationEnum operation,  double stopprice, double limitprice, int quantity, int rest,
            int status, ulong transId, string comment)
        {
            Account = account;
            Strategy = !string.IsNullOrEmpty(strategy) ? strategy : "999";

            Number = number;
            DateTime = dt;
            Ticker = ticker;

            Operation = operation;

            StopPrice = stopprice;
            LimitPrice = limitprice;

            Quantity = quantity;
            Rest = rest;
            switch (status)
            {
                case 1:
                    Status = OrderStatusEnum.Activated;
                    break;
                case 2:
                    Status = OrderStatusEnum.Canceled;
                    break;
                default:
                    Status = OrderStatusEnum.Filled;
                    break;
            }
            TransId = transId;
            Comment = comment;
        }
        
        public Order(
            ulong number, DateTime dt, ulong transId,
            string account, string strategy, string ticker,
            OrderOperationEnum operation, OrderTypeEnum ordertype, double stopprice, double limitprice, long quantity, long rest,
            OrderStatusEnum status, string comment)
        {
            Number = number;
            DateTime = dt;
            TransId = transId;

            Account = account;
            Ticker = ticker;
            Strategy = strategy;

            Operation = operation;
            OrderType = ordertype;

            StopPrice = stopprice;
            LimitPrice = limitprice;

            Quantity = quantity;
            Rest = rest;

            //Status = status;
            _status = status;
            Comment = comment;
        }
          
        public Order(
            ulong number, DateTime dt, ulong transId,
            string account, string strategy, string ticker,
            OrderOperationEnum operation, OrderTypeEnum ordertype, double stopprice, double limitprice, long quantity, long rest,
            OrderStatusEnum status, TimeSpan activate, TimeSpan cancel, DateTime expire, string comment)
        {
            Number = number;
            DateTime = dt;
            TransId = transId;

            Account = account;
            Ticker = ticker;
            Strategy = strategy;

            Operation = operation;
            OrderType = ordertype;

            StopPrice = stopprice;
            LimitPrice = limitprice;

            Quantity = quantity;
            Rest = rest;

            //Status = status;
            _status = status;
            Comment = string.IsNullOrWhiteSpace(comment) ? "" : comment + "; ";

            ActivateTime = activate;
            CancelTime = cancel;
            ExpireDate = expire;
        }
        public Order( Orders orders,
            ulong number, DateTime dt, ulong transId,
            string account, string strategy, string ticker,
            OrderOperationEnum operation, OrderTypeEnum ordertype, double stopprice, double limitprice, long quantity, long rest,
            OrderStatusEnum status, TimeSpan activate, TimeSpan cancel, DateTime expire, string comment)
        {
            Orders = orders;

            Number = number;
            DateTime = dt;
            TransId = transId;

            Account = account;
            Ticker = ticker;
            Strategy = strategy;

            Operation = operation;
            OrderType = ordertype;

            StopPrice = stopprice;
            LimitPrice = limitprice;

            Quantity = quantity;
            Rest = rest;

            Status = status;
            //_status = status;
            Comment = string.IsNullOrWhiteSpace(comment) ? "" : comment + "; ";

            ActivateTime = activate;
            CancelTime = cancel;
            ExpireDate = expire;
        }


        internal void SetStatus(OrderStatusEnum status, string comment)
        {
            Status = status;
            if (!string.IsNullOrWhiteSpace(comment)) Comment +=  comment + "; ";
            switch (status)
            {
                case OrderStatusEnum.Canceled:
                case OrderStatusEnum.Filled:
                    CancelTime = DateTime.Now.TimeOfDay;
                    //if(Orders != null) 
                    //    Orders.RemoveFilled(this);
                    break;
                case OrderStatusEnum.Activated:
                    ActivateTime = DateTime.Now.TimeOfDay;
                    break;
                case OrderStatusEnum.Registered:
                    RegisterTime = DateTime.Now.TimeOfDay;
                    break;
                case OrderStatusEnum.Sended:
                    SendTime = DateTime.Now.TimeOfDay;
                    break;
                case OrderStatusEnum.Confirmed:
                    ConfirmTime = DateTime.Now.TimeOfDay;
                    break;
            }
            if( Orders != null) Orders.SetNeedToObserver();
        }

        public override string ToString()
        {
            return String.Format("{0:G} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} TrId={13} {14:T}",
                                 DateTime, Number, Account, Strategy, 
                                 Ticker, Operation, OrderType, StopPrice, LimitPrice, PositionQuantity, PositionRest, 
                                 StatusString, Comment, TransId, CancelTime);
        }
    }
}