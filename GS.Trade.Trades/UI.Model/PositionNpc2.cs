using System;
using System.ComponentModel;
using GS.Containers1;
using GS.Containers5;
using GS.Extension;

namespace GS.Trade.Trades.UI.Model
{
    public class PositionNpc2 : TradeEntity, INotifyPropertyChanged //, IHaveKey<string>
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, args);
        }

        public PositionNpc2()
        {
        }
        public PositionNpc2(IPosition2 ip)
        {
            Strategy = ip.Strategy;

            FirstTradeDT = ip.FirstTradeDT;
            FirstTradeNumber = ip.FirstTradeNumber;

            LastTradeDT = ip.LastTradeDT;
            LastTradeNumber = ip.LastTradeNumber;

            Operation = ip.PosOperation;
            Status = ip.PosStatus;

            Quantity = ip.Quantity;

            Price1 = ip.Price1;
            Price2 = ip.Price2;

            DailyPnLFixed = ip.DailyPnLFixed;
            PosPnLFixed = ip.PosPnLFixed;

            DailyProfitLimit = ip.DailyProfitLimit;

            LastPrice = ip.LastPrice;

            //PnL1 = ip.PnL;
            // 17.04.11
            //PnL2 = ip.PnL2;
            PnL2 = ip.PnL;
            PnL3 = ip.PnL3;

            StrategyKeyEx = ip.StrategyKeyEx;
            TickerCodeEx = ip.TickerCodeEx;
            AccountCodeEx = ip.AccountCodeEx;

            Delta = ip.Delta;

            //TotalDailyMaxProfit = ip.TotalDailyMaxProfit;
            //TotalDailyMaxLoss = ip.TotalDailyMaxLoss;
            //TotalDailyMaxProfitDT = ip.TotalDailyMaxProfitDT;
            //TotalDailyMaxLossDT = ip.TotalDailyMaxLossDT;

            TotalDailyMaxProfit = ip.Strategy?.DailyMaxProfit ?? 0;
            TotalDailyMaxLoss = ip.Strategy?.DailyMaxLoss ?? 0;
            TotalDailyMaxProfitDT = ip.Strategy?.DailyMaxProfitDT ?? DateTime.Now;
            TotalDailyMaxLossDT = ip.Strategy?.DailyMaxLossDT ?? DateTime.Now;

            Comment = ip.Comment;
        }

        public void Update(IPosition2 ip)
        {
            if (Quantity == 0)
            {
                FirstTradeDT = ip.FirstTradeDT;
                FirstTradeNumber = ip.FirstTradeNumber;
            }

            LastTradeDT = ip.LastTradeDT;
            LastTradeNumber = ip.LastTradeNumber;

            Operation = ip.PosOperation;
            Status = ip.PosStatus;

            Quantity = ip.Quantity;

            Price1 = ip.Price1;
            Price2 = ip.Price2;

            LastPrice = ip.LastPrice;

            //PnL1 = ip.PnL;
            // 17.04.11
            //PnL2 = ip.PnL2;
            PnL2 = ip.PnL;
            PnL3 = ip.PnL3;

            DailyPnLFixed = ip.DailyPnLFixed;
            PosPnLFixed = ip.PosPnLFixed;

            DailyProfitLimit = ip.DailyProfitLimit;

            StrategyKeyEx = ip.StrategyKeyEx;
            AccountCodeEx = ip.AccountCodeEx;
            TickerCodeEx = ip.TickerCodeEx;

            Delta = ip.Delta;

            TotalDailyMaxProfit = ip.Strategy?.DailyMaxProfit ?? 0;
            TotalDailyMaxLoss = ip.Strategy?.DailyMaxLoss ?? 0;
            TotalDailyMaxProfitDT = ip.Strategy?.DailyMaxProfitDT ?? DateTime.Now;
            TotalDailyMaxLossDT = ip.Strategy?.DailyMaxLossDT ?? DateTime.Now;

            // TotalDailyMaxProfit = ip.TotalDailyMaxProfit;
            // TotalDailyMaxLoss = ip.TotalDailyMaxLoss;
            // TotalDailyMaxProfitDT = ip.TotalDailyMaxProfitDT;
            // TotalDailyMaxLossDT = ip.TotalDailyMaxLossDT;

            Comment = ip.Comment;
        }

        public decimal DailyCurrentPnL => PnL1 + DailyPnLFixed;

        private decimal _dailyProfitLimit;

        public decimal DailyProfitLimit
        {
            get { return _dailyProfitLimit; }
            set
            {
                _dailyProfitLimit = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DailyProfitLimitFormated"));
            }
        }
        public decimal DailyLossLimit { get; set; }

        public string Key => StrategyKey;

        private ulong _firstTradeNumber;

        public ulong FirstTradeNumber
        {
            get { return _firstTradeNumber; }
            set
            {
                _firstTradeNumber = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FirstTradeNumber"));
            }
        }

        private DateTime _firstTradeDT;

        public DateTime FirstTradeDT
        {
            get { return _firstTradeDT; }
            set
            {
                _firstTradeDT = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FirstTimeDateString"));
                OnPropertyChanged(new PropertyChangedEventArgs("FirstTradeDTString"));
            }
        }

        private ulong _lastTradeNumber;

        public ulong LastTradeNumber
        {
            get { return _lastTradeNumber; }
            set
            {
                _lastTradeNumber = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("LastTradeNumber"));
                //OnPropertyChanged(new PropertyChangedEventArgs("AccountCodeEx"));
                //OnPropertyChanged(new PropertyChangedEventArgs("TickerCodeEx"));
                //OnPropertyChanged(new PropertyChangedEventArgs("StrategyKeyEx"));
            }
        }

        private DateTime _lastTradeDT;

        public DateTime LastTradeDT
        {
            get { return _lastTradeDT; }
            set
            {
                _lastTradeDT = value;
                OnPropertyChanged(new PropertyChangedEventArgs("LastTradeNumber"));
                OnPropertyChanged(new PropertyChangedEventArgs("LastTradeDTString"));
                OnPropertyChanged(new PropertyChangedEventArgs("LastTimeDateString"));
                OnPropertyChanged(new PropertyChangedEventArgs("DailyPnLAvgStr"));
                OnPropertyChanged(new PropertyChangedEventArgs("TotalDays"));
                OnPropertyChanged(new PropertyChangedEventArgs("TradeAvg1Formated"));
                //OnPropertyChanged(new PropertyChangedEventArgs("DailyPnLFixedFormated"));
                // OnPropertyChanged(new PropertyChangedEventArgs("PosPnLFixedFormated"));
            }
        }

        private decimal _posPnLFixed;
        public decimal PosPnLFixed
        {
            get { return _posPnLFixed; }
            set
            {
                _posPnLFixed = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PosPnLFixedFormated"));
            }
        }

        private decimal _dailyPnLFixed;

        public decimal DailyPnLFixed
        {
            get { return _dailyPnLFixed; }
            set
            {
                _dailyPnLFixed = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DailyPnLFixedFormated"));
                OnPropertyChanged(new PropertyChangedEventArgs("DailyCurrentPnLFormated"));
            }
        }

        // public decimal DailyPnLFixed { get; set; }

        public int TotalDays => (LastTradeDT - FirstTradeDT).Days + 1;

        public decimal DailyPnLAvg => TotalDays != 0
                            ? PnL1/TotalDays
                            : 0;

        public string DailyPnLAvgStr => DailyPnLAvg.ToString("N2");

        public decimal TradeAvg1 => Quantity != 0
            ? PnL1/Quantity
            : 0;
        public string TradeAvg1Formated => TradeAvg1.ToString(FormatAvgStr);

        private decimal _price1;

        public decimal Price1
        {
            get { return _price1; }
            set
            {
                _price1 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Price1String"));
            }
        }

        private decimal _price2;

        public decimal Price2
        {
            get { return _price2; }
            set
            {
                _price2 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Price2String"));
                OnPropertyChanged(new PropertyChangedEventArgs("PnL1String"));
                OnPropertyChanged(new PropertyChangedEventArgs("DailyCurrentPnLFormated"));
                OnPropertyChanged(new PropertyChangedEventArgs("DeltaString"));

                //OnPropertyChanged(new PropertyChangedEventArgs("PnLTotalString"));
            }
        }

        private decimal _lastprice;

        public decimal LastPrice
        {
            get { return _lastprice; }
            set
            {
                _lastprice = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Price2String"));
                OnPropertyChanged(new PropertyChangedEventArgs("PnL1String"));
                OnPropertyChanged(new PropertyChangedEventArgs("LastPriceString"));
                OnPropertyChanged(new PropertyChangedEventArgs("DeltaString")); 
                //OnPropertyChanged(new PropertyChangedEventArgs("PnLTotalString"));
            }
        }
        private double _delta;
        public double Delta
        {
            get { return _delta; }
            set
            {
                _delta = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DeltaString"));
            }
        }
        public string DeltaString =>
                (Delta.IsGreaterThan(0d) 
                    ? "+" 
                    : (Delta.IsLessThan(0d) ? "-" : string.Empty)) + Delta.ToString("G");

        private PosOperationEnum _operation;
        public PosOperationEnum Operation
        {
            get { return _operation; }
            set
            {
                _operation = value;
                OnPropertyChanged(new PropertyChangedEventArgs("OperationString"));
            }
        }

        private long _quantity;
        public long Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Quantity"));
                // OnPropertyChanged(new PropertyChangedEventArgs("DeltaString"));
                OnPropertyChanged(new PropertyChangedEventArgs("PositionString2"));
                //OnPropertyChanged(new PropertyChangedEventArgs("Amount1String"));
                //OnPropertyChanged(new PropertyChangedEventArgs("Amount2String"));
                OnPropertyChanged(new PropertyChangedEventArgs("PnL1String"));
                //OnPropertyChanged(new PropertyChangedEventArgs("PnL2String"));
            }
        }
        private decimal _totalDailyMaxProfit;
        public decimal TotalDailyMaxProfit
        {
            get { return _totalDailyMaxProfit; }
            set
            {
                _totalDailyMaxProfit = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TotalDailyMaxProfitStr"));
            }
        }
        public string TotalDailyMaxProfitStr => TotalDailyMaxProfit.ToString("N");

        private DateTime _totalDailyMaxProfitDT;
        public DateTime TotalDailyMaxProfitDT
        {
            get { return _totalDailyMaxProfitDT; }
            set
            {
                _totalDailyMaxProfitDT = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TotalDailyMaxProfitDTStr"));
            }
        }
        public string TotalDailyMaxProfitDTStr => TotalDailyMaxProfitDT.ToString("d");

        private decimal _totalDailyMaxLoss;
        public decimal TotalDailyMaxLoss
        {
            get { return _totalDailyMaxLoss; }
            set
            {
                _totalDailyMaxLoss = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TotalDailyMaxLossStr"));
            }
        }
        public string TotalDailyMaxLossStr => TotalDailyMaxLoss.ToString("N");

        private DateTime _totalDailyMaxLossDT;
        public DateTime TotalDailyMaxLossDT
        {
            get { return _totalDailyMaxLossDT; }
            set
            {
                _totalDailyMaxLossDT = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TotalDailyMaxLossDTStr"));
            }
        }
        public string TotalDailyMaxLossDTStr => TotalDailyMaxLossDT.ToString("d");

        public decimal Amount1 => Quantity*Price1;
        public decimal Amount2 => Quantity*Price2;

        private PosStatusEnum _status;
        public PosStatusEnum Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged(new PropertyChangedEventArgs("StatusString"));
            }
        }

        public long Pos => Quantity*(short) Operation;

        public string OperationString => Operation.ToString().ToUpper();

        public string StrategyTickerString => StrategyCode + "." + TickerCode;

        public string PositionInfo => 
            $"Position: {OperationString}: [{(Operation > 0 ? "+" : Operation < 0 ? "-" : "")}{Quantity}] P&L: {PnL1:N2}";

        public virtual decimal PnL1 => (Price2 - Price1)*Pos;

        //private decimal _pnL1;
        //public decimal PnL1
        //{
        //    get { return _pnL1; }
        //    set
        //    {
        //        OnPropertyChanged(new PropertyChangedEventArgs("PnL1String"));
        //        _pnL1 = value;
        //    }
        //}
        private decimal _pnL2;
        public decimal PnL2
        {
            get { return _pnL2; }
            set
            {
                _pnL2 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PnL2String"));
            }
        }
        private decimal _pnL3;
        public decimal PnL3
        {
            get { return _pnL3; }
            set
            {
                _pnL3 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PnL3String"));
            }
        }

        public long Count { get; set; }
        public bool IsOpened => Status == PosStatusEnum.Opened;
        public bool IsClosed => Status == PosStatusEnum.Closed;
        public bool IsLong => Operation == PosOperationEnum.Long;
        public bool IsShort => Operation == PosOperationEnum.Short;
        public bool IsNeutral => Operation == PosOperationEnum.Neutral;

        public PosOperationEnum FlipOperation => IsLong
            ? PosOperationEnum.Short
            : (IsShort
                ? PosOperationEnum.Long
                : PosOperationEnum.Neutral);

        public string StatusString => Status.ToString().ToUpper();

        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Comment"));
            }
        }
     
        public long Index { get; set; }

        public string TradeKey => StrategyKey + "@" + AccountKey + "@" + TickerKey;
        public string FirstTimeDateString => FirstTradeDT.ToString("T") + ' ' + FirstTradeDT.ToString("d");
        public string LastTimeDateString => LastTradeDT.ToString("T") + ' ' + LastTradeDT.ToString("d");
        public string TickerFormat => Ticker != null ? Ticker.Format : "N2";
        public string TickerFormatAvg => Ticker != null ? Ticker.FormatAvg : "N4";
        public string TickerFormatM => Ticker != null ? Ticker.FormatM : "N2";
        public string LastTradeDTString => LastTradeDT.ToString("G");
        public string FirstTradeDTString => FirstTradeDT.ToString("G");
        public string PositionString => (Pos > 0 ? "+" : "") + Pos.ToString("N0");
        public string PositionString2 => (Pos > 0 ? "+" : (Pos < 0 ? "-" : string.Empty)) + Pos.ToString("N0");
        public string PositionString3 => TickerCode + "." + OperationString + PositionString2.WithSqBrackets();
        public string PositionString5 => PositionString3 + EntryPriceString;
        public string PositionString6 => PositionString3 + "P&L: " + PnL1String;
        public string EntryPriceString => "EntryPrice: " + Price1String;
        public string Amount1String => Amount1.ToString(TickerFormatAvg);
        public string Amount2String => Amount2.ToString(TickerFormatAvg);
        public string Price1String => Price1.ToString(TickerFormatAvg);
        public string Price2String => Price2.ToString(TickerFormatAvg);
        public string LastPriceString => LastPrice.ToString(TickerFormat);
        public string LastTradePriceString => Price2.ToString(TickerFormat);
        public string PnL1String => PnL1.ToString(TickerFormatAvg);
        public string PnL2String => PnL2.ToString(TickerFormatM);
        public string PnL3String => PnL3.ToString(TickerFormatAvg);
        public string DailyPnLFixedFormated => DailyPnLFixed.ToString(TickerFormat);
        public string DailyCurrentPnLFormated => DailyCurrentPnL.ToString(TickerFormat);
        public string PosPnLFixedFormated => PosPnLFixed.ToString(TickerFormat);
        public string DailyProfitLimitFormated => DailyProfitLimit.IsEquals(Decimal.MaxValue)
            ? "+ Infinity"
            : DailyProfitLimit.ToString(TickerFormat);
        public string DailyLossLimitFormated => DailyLossLimit.IsEquals(Decimal.MinValue)
            ? "-- Infinity"
            : DailyLossLimit.ToString(TickerFormat);
    }
}
