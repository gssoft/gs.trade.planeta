using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;
using GS.Extensions.DateTime;

namespace GS.Trade.Trades.Trades3
{
    public class Trade3 : ITrade3
    {
        //public enum OperationEnum : short { Sell = -1, Buy = +1 }
        public Trades Trades;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
        // public int StrategyKeyHash { get; set; }
        public int Id { get; set; }
        public IStrategy Strategy { get; set; }
        public int StrategyId => Strategy?.Id ?? 0;
        public IAccount Account => Strategy?.Account;
        public ITicker Ticker => Strategy?.Ticker;
        public string StrategyCode => Strategy != null ? Strategy.Code : "Unknown";
        public string StrategyKey => Strategy != null ? Strategy.StrategyTimeIntTickerString : "Unknown";
        public string TickerBoard => Ticker != null ? Ticker.ClassCode : ClassCodeEx;
        public string TickerCode => Ticker != null ? Ticker.Code : TickerEx;
        public string AccountCode => Account != null ? Account.Code : AccountEx;
        public int TimeInt => Strategy?.TimeInt ?? TimeInterval;
        public ulong Number { get; set; }
        public DateTime DT { get; set; }
        public long Quantity { get; set; }
        public TradeOperationEnum Operation { get; set; }
        //       public TradeOperationEnum TradeOperation { get { return (TradeOperationEnum) Operation; } }
        public long Position => Quantity*(short) Operation;
        public Decimal Price { get; set; }
        public Decimal Amount => Quantity*Price;
        public IOrder3 Order { get; set; }
        public Decimal OrderPrice { get; set; }
        public ulong OrderNumber { get; set; }
        private TradeStatusEnum _status;
        public TradeStatusEnum Status
        {
            get { return _status; }
            set
            {
                _status = value;
                LastStatusChangedDT = DateTime.Now;
                // UpdateStatusDateTime(_status);
            }
        }
        public string AccountEx { get; set; }
        public string StrategyEx { get; set; }
        public string ClassCodeEx { get; set; }
        public string TickerEx { get; set; }
        public int TimeInterval { get; set; }
        public long Mode { get; set; }
        public string Comment { get; set; }
        public decimal CommissionTs { get; set; }
        public int MyStatus { get; set; }
        public long MyIndex { get; set; }
        public DateTime LastStatusChangedDT { get; set; }
        public DateTime Registered { get; set; }
        public DateTime Confirmed { get; set; }
        public DateTime ToResolveDT { get; set; }
        public DateTime Resolved { get; set; }
        public TimeSpan Elapsed => LastStatusChangedDT - Registered;
        public TimeSpan Elapsed1 => Confirmed - Registered;
        // public TimeSpan Elapsed2 => ToResolveDT - Confirmed;
        public TimeSpan Elapsed2 => Resolved - Confirmed;
        public string ElapsedStr => $"EL: {Elapsed.ToString(@"ss\.fff")}";
        public string ElapsedStr1 => $"EL1: {Elapsed1.ToString(@"ss\.fff")}";
       // public string ElapsedStr2 => $"EL2: {Elapsed2.ToString(@"ss\.fff")}";
        public string ElapsedStr2 => $"EL2: {Elapsed2.ToString(@"ss\.fff")}";
        public string ElapsedAllStr => $"{ElapsedStr}, {ElapsedStr1}, {ElapsedStr2}";   
        public string DateTimeString => DT.ToString("G");
        public string TimeDateString => DT.ToString("T") + ' ' + DT.ToString("d");
        public string OperationString => Operation > 0 ? "Buy" : (Operation < 0 ? "Sell" : "Unknown");
        public string PositionString => Position.ToString("N0");
        public string AmountString => Amount.ToString("N2");
        public string PriceString => Price.ToString("N2");
        public string OrderPriceString => OrderPrice.ToString("N2");
        public string ModeString => (Mode == 0) ? "New" : (Mode == 1) ? "Init" : "End Init";
        // public string StrategyKey => General.StrategyKey(AccountEx,StrategyEx,TickerEx);
        public string OrderKey => (OrderNumber + "." + AccountCode).TrimUpper();
        public string Key => (Number + "." + AccountCode).TrimUpper();
        // public string TradeKey {get { return Key; } }
        public string StratTicker => Strategy != null ? Strategy.StrategyTickerString : "Unknown";

        public string ShortInfo =>
            $"Trade:[{Number}] {Operation} [{Quantity}] {TickerCode} @ {PriceString} {DT.DateTimeStrTodayCnd()}";
        //  $"tr#:{Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0()} " +
        //  $"ord#:{OrderNumber.ToString(CultureInfo.InvariantCulture).WithSqBrackets0()}";
        public string ShortDescription =>
            $"Trade:[{Number}] Ord:[{OrderNumber}] {Operation} [{Quantity}] {TickerCode} @ {PriceString} Acc:{AccountCode} Str:{StrategyCode}";

        private void UpdateStatusDateTime(TradeStatusEnum status)
        {
            var dt = DateTime.Now;
            switch (status)
            {
                case TradeStatusEnum.Registered:
                    Registered = DT;
                    LastStatusChangedDT = DT;
                    ToResolveDT = DT;
                    Resolved = DT;
                    break;
                case TradeStatusEnum.Confirmed:
                    // Confirmed = dt;
                    LastStatusChangedDT = dt;
                    break;
                case TradeStatusEnum.ToResolve:
                    ToResolveDT = dt;
                    LastStatusChangedDT = dt;
                    break;
                case TradeStatusEnum.Resolved:
                    Resolved = dt;
                    LastStatusChangedDT = dt;
                    break;               
            }
        }
        public string RegCnfRsvDateTimeStr => $"Rgs:{Registered}, Cnf:{Confirmed}, Rsv:{Resolved}";
        public override string ToString()
        {
            return
                $"Trade:{Number} Ord:{OrderNumber} {Status} {Operation} [{Quantity}] {TickerBoard}@{TickerCode} @ {PriceString}" +
                $" {AccountCode} {StrategyKey}" +
                $" Rgst:{Registered} Cnfm:{Confirmed} Rslv:{Resolved} Elps:{ElapsedAllStr}";
        }
    }
}
