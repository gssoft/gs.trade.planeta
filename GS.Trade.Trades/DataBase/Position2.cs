using System;
using System.Reflection;
using System.Xml.Serialization;
using GS.Extension;
using GS.Extensions.DateTime;
using GS.ICharts;
using GS.Interfaces;
using GS.Trade.Trades.Deals;
using GS.Trade.Trades.Equity;

namespace GS.Trade.Trades.DataBase
{
    public class PositionDb  : TradeEntity // , ILineXY,  IPosition2
    {
        private IEventLog _eventLog;

        public string Symbol { get; set; }


        public Positions2 Positions { get; set; }

        public event PositionChangedEventHandler PositionChangedEvent;
        public event PositionChangedEventHandler2 PositionChangedEvent2;
        public event PositionChangedEventHandler3 PositionChangedEvent3;

        //public string Key {
        //    get { return String.Format("{0}@{1}", GetType(), StrategyKey); }
        //}
        public  string Key => StrategyKey;

        public int StrategyKeyHash { get; set; }

        public ulong FirstTradeNumber { get; set; }
        public DateTime FirstTradeDT { get; set; }

        public ulong LastTradeNumber { get; set; }
        public DateTime LastTradeDT { get; set; }

        public decimal PosPnLFixed { get; set; }
        public decimal DailyPnLFixed { get; set; }

        public IBar LastBar { get; private set; }
        public void SetLastBar(IBar b){LastBar = b;}
        public DateTime LocalBarDateTime => LastBar?.DT ?? DateTime.Now;

        public int TotalDays => (LastTradeDT.Date - FirstTradeDT.Date).Days + 1;
        public virtual decimal DailyPnLAvg => TotalDays != 0 ? PnL / TotalDays : 0;
        public string DailyPnLAvgFormated => DailyPnLAvg.ToString(FormatAvgStr);
        public decimal TradeAvg1 => Quantity != 0 ? PnL / Quantity : 0;
        public string TradeAvg1Formated => TradeAvg1.ToString(FormatAvgStr);
        public float LastTradePrice { get; set; }
        
        public decimal Price1 { get; set; }
        public decimal Price2 { get; set; }
        public decimal Price3 => Price1 - (!IsNeutral && Quantity!=0 ? (short)Operation * PnL3 / Quantity : 0);

        public decimal LastPrice { get; set; }
        

        public decimal Amount1 => Quantity * Price1;
        public decimal Amount2 => Quantity * Price2;
        public PosOperationEnum Operation { get; set; }
        public PosOperationEnum PosOperation => Operation;
        public long Quantity { get; set; }
        public decimal Rate { get; set; }
        public int TimeInt { get; set; }
        public string TimeIntKey { get; set; }
        public long Pos => Quantity * (short)Operation;
        public string OperationString => Operation.ToString();
        public string OperationStringUpper => Operation.ToString().ToUpper();
        // public string StrategyTickerString => Strategy.Code + "." + TickerCode;
        public string StrategyTickerString => Strategy != null ? Strategy.StrategyTickerString : "Unknown";
        // public string StrategyTimeIntTickerString => Strategy != null ? Strategy.StrategyTimeIntTickerString : "Unknown";

        [XmlIgnore]
        public OmegaReport Report { get; set; }
        public string PositionInfo => $"Position: {OperationString}: [{(Operation > 0 ? "+" : Operation < 0 ? "-" : "")}{Quantity}] @ {Price1:N2}" +
                                      $" P&L: {Profit:N2} P&L2: {PnL3:N2}";
        public string ShortInfo => 
            $"Postn:[{LastTradeNumber}] {LastTradeDT.DateTimeTickStrTodayCnd()} " +
            $"{Operation}[{(Operation > 0 ? "+" : Operation < 0 ? "-" : "")}{Quantity}] {TickerCode}" +
            $"@{Price1:N2} {Status}";
        public string ShortDescription =>
            $"Postn:[{LastTradeNumber}] {LastTradeDT.DateTimeTickStrTodayCnd()} " +
            $"{Operation}[{(Operation > 0 ? "+" : Operation < 0 ? "-" : "")}{Quantity}] {TickerCode}" +
            $"@{Price1:N2} {Status} {AccountCode} {StrategyCode}";
        public string PnLFormated => PnL.ToString(FormatStr);
        public string PnLFormatedAvg => PnL.ToString(FormatAvgStr);

        public decimal PnL { get; set; }

        public decimal PnL1 => (Price2 - Price1) * Pos;

        public decimal PnL2 => PnL1;

        public decimal PnL3 { get; set; }
        public decimal Costs { get; set; }
        public string Price1Formated => Price1.ToString(FormatStr);
        public string Price1FormatedAvg => Price1.ToString(FormatAvgStr);
        public string Price2Formated => Price2.ToString(FormatStr);
        public string Price2FormatedAvg => Price2.ToString(FormatAvgStr);

        public long Count { get; set; }
        public PosStatusEnum Status { get; set; }

        public PosStatusEnum PosStatus => Status;

        public PositionChangedEnum LastChangedResult { get; set; }

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

        public float Profit => Ticker != null ? (float)(Ticker.LastPrice - (double)Price1) * Pos : 0f;
        public string StatusStringUpper => Status.ToString().ToUpper();
        public string Comment { get; set; }
        public long Index { get; set; }

        public string TradeKey => StrategyKey + "@" + AccountKey + "@" + TickerKey;
        public string FirstTimeDateString => FirstTradeDT.ToString("T") + ' ' + FirstTradeDT.ToString("d");
        public string LastTimeDateString => LastTradeDT.ToString("T") + ' ' + LastTradeDT.ToString("d");

        /// <summary>
        /// Daily PnL Limits
        /// </summary>
        /// 
        public decimal DailyProfitLimit { get; set; }
        public decimal DailyLossLimit { get; set; }

        public decimal DailyCurrentPnL => PnL1 + DailyPnLFixed;

        public bool IsDailyProfitLimitReached => 
            Account != null && IsOpened && DailyCurrentPnL.IsGreaterThan(Account.DailyProfitLimit);

        public bool IsDailyLossLimitReached => 
            Account != null && IsOpened && DailyCurrentPnL.IsLessThan(Account.DailyLossLimit);


        public string TickerFormat => Ticker != null ? Ticker.Format : "N2";
        public string TickerFormatAvg => Ticker != null ? Ticker.FormatAvg : "N4";
        public string TickerFormatM => Ticker != null ? Ticker.FormatM : "N2";

        public string LastTradeDTString => LastTradeDT.ToString("G");
        public string FirstTradeDTString => FirstTradeDT.ToString("G");

        public string PositionString => (Pos > 0 ? "+" : "") + Pos.ToString("N0");
        public string PositionString2 => (Pos > 0 ? "+" : (Pos < 0 ? "-" : string.Empty)) + Pos.ToString("N0");
        public string PositionString3 => TickerCode + " " + OperationStringUpper + PositionString2.WithSqBrackets();

        public string PositionString5 => $"{PositionString3} {EntryPriceString} {StatusStringUpper} at {FirstTradeDTString} Trade:[{FirstTradeNumber}]";

        public string PositionString6 => PositionString3 + "P&L: " + ProfitTotalString;

        public string EntryPriceString => "EntryPrice: " + AvgPriceString;

        public string AmountString => Amount1.ToString(TickerFormatAvg);

        public string AvgPriceString => Price1.ToString(TickerFormatAvg);
        public string AvgPrice2String => Price2.ToString(TickerFormatAvg);

        public string LastTradePriceString => Price2.ToString(TickerFormat);

        public string PnLString => PnL.ToString(TickerFormatM);
        public string PnL1String => PnL1.ToString(TickerFormatAvg);
        public string PnL2String => PnL2.ToString(TickerFormatM);
        //public decimal TotalPnL { get { return (Price2 - Price1) * Quantity; } }
        //public string TotalPnLString { get { return TotalPnL.ToString(TickerFormatM); } }
        //public string PnLString { get { return ((Price2 - Price1)*Pos).ToString(TickerFormatAvg); } }
        public string ProfitTotalString => ((Price2 - Price1) * Pos).ToString(TickerFormatAvg);
        //public string StrategyKey { get { return AccountKey + Strategy + TickerCode; } }
        // public double Delta { get; set; }
        public double Delta => Ticker?.Delta*Pos ?? 0;
        public decimal TotalDailyMaxProfit { get; set; }
        public DateTime TotalDailyMaxProfitDT { get; set; }
        public decimal TotalDailyMaxLoss { get; set; }
        public DateTime TotalDailyMaxLossDT { get; set; }

        public PositionDb() { }
     
        
    
        
    }
}
