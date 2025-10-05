using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Events;
using GS.Extension;
using GS.Extensions.DateTime;
using GS.ICharts;
using GS.Interfaces;
using GS.Trade.Trades.Deals;
using GS.Trade.Trades.Equity;
using GS.Trade.Trades.Trades3;

namespace GS.Trade.Trades
{
    public class Position2 : TradeEntity, ILineXY,  IPosition2
    {
        private IEventLog _eventLog;

        private float _lastTradeBuyPrice;
        private float _lastTradeSellPrice;

        public string Symbol { get; set; }

        public IEventLog EventLog
        {
            get { return _eventLog; }
            set { _eventLog = value; }
        }

        public Positions2 Positions { get; set; }

        public event PositionChangedEventHandler PositionChangedEvent;
        public event PositionChangedEventHandler2 PositionChangedEvent2;
        public event PositionChangedEventHandler3 PositionChangedEvent3;

        //public string Key {
        //    get { return String.Format("{0}@{1}", GetType(), StrategyKey); }
        //}
        public  string Key => StrategyKey;

        public int StrategyKeyHash { get; set; }

        //private static long _positionIdRoot;
        
        public IPositionTotal2 PositionTotal2 { get; set; }
        public IPosition2 PositionTotal { get; set; }

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
        public float LastTradeBuyPrice
        {
            get { return _lastTradeBuyPrice; }
            set
            {
                _lastTradeBuyPrice = value;
                LastTradePrice = value;
            }
        }
        public decimal Price1 { get; set; }
        public decimal Price2 { get; set; }
        public decimal Price3 => Price1 - (!IsNeutral && Quantity!=0 ? (short)Operation * PnL3 / Quantity : 0);

        public decimal LastPrice { get; set; }
        public virtual float LastTradeSellPrice{
            get { return _lastTradeSellPrice; }
            set { _lastTradeSellPrice = value; LastTradePrice = value; }
        }

        public decimal Amount1 => Quantity * Price1;
        public decimal Amount2 => Quantity * Price2;
        public PosOperationEnum Operation { get; set; }
        public PosOperationEnum PosOperation => Operation;
        public long Quantity { get; set; }
        public long TotalQuantity => PositionTotal?.Quantity ?? 0;
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

        public void CreateStat()
        {
            if (DailyPnLFixed > TotalDailyMaxProfit)
            {
                TotalDailyMaxProfit = DailyPnLFixed;
                TotalDailyMaxProfitDT = LocalBarDateTime;
            }
            if (DailyPnLFixed < TotalDailyMaxLoss)
            {
                TotalDailyMaxLoss = DailyPnLFixed;
                TotalDailyMaxLossDT = LocalBarDateTime;
            }

            //var p = Clone();
            //Strategy.OnChangedEvent(new Events.EventArgs
            //{
            //    Category = "UI.Positions",
            //    Entity = "Total.MaxMinProfit",
            //    Operation = "Update",
            //    Object = p,
            //    Sender = this
            //});
        }

        public Position2() { }
        //public Position2(string account, string strategy, string ticker, PosOperationEnum operation, long quantity,
        //                decimal firsttradeprice, decimal lasttradeprice, decimal pnl, PosStatusEnum status,
        //                DateTime mindt, long minnumber, DateTime maxdt, long maxnumber)
        //{
        //    AccountCodeEx = account; 
        //    Strategy = strategy;
        //    TickerCode = ticker;

        //    Operation = operation;
        //    Quantity = quantity;
        //    //Amount = amount;
        //    Pnl = pnl;
        //    FirstTradeNumber = minnumber;
        //    FirstTradeDT = mindt;
        //    // CloseTradeNumber = maxnumber; DT_Close = maxdt;
        //    LastTradeNumber = maxnumber;
        //    LastTradeDT = maxdt;
        //    //LastTradePrice = lasttradeprice;
        //    Price1 = firsttradeprice;
        //    Price2 = lasttradeprice;
        //    Count = quantity;
        //    //OpenCloseFlag = 1;

        //    Status = status;

        //    StrategyKeyHash = GetStrategyKeyHash();

        //    lock (this)
        //    {
        //        _positionIdRoot++;
        //        PositionId = _positionIdRoot;
        //    }

        //    // string s = "\"";
        //}
        //public Position2(Trade3 t)
        //{
        //    Account = t.AccountCode;
        //    Strategy = t.StrategyCode;
        //    TickerStr = t.TickerCode;

        //    Operation = t.Operation == OperationEnum.Buy
        //                ? PosOperationEnum.Long :
        //                (t.Operation == OperationEnum.Sell ? PosOperationEnum.Short : PosOperationEnum.Neutral);
        //    Quantity = t.Quantity;
        //    //Amount = amount;
        //    Pnl = 0;
        //    FirstTradeNumber = t.Number;
        //    FirstTradeDT = t.DT;
        //    // CloseTradeNumber = maxnumber; DT_Close = maxdt;
        //    LastTradeNumber = FirstTradeNumber;
        //    LastTradeDT = FirstTradeDT;
        //    //LastTradePrice = t.Price;
        //    Price1 = t.Price;
        //    Count = t.Quantity;
        //    //OpenCloseFlag = 1;

        //    Status = PosStatusEnum.Opened;

        //    // StrategyKeyHash = t.StrategyKeyHash;

        //    lock (this)
        //    {
        //        _positionIdRoot++;
        //        PositionId = _positionIdRoot;
        //    }
        //}

        public IDeal NewTrade(ITrade3 t)
        {
            var pp = this;

            if (!t.DT.Date.IsEquals(pp.LastTradeDT.Date))
                pp.DailyPnLFixed = 0;

            if (pp.IsOpened)
            {
                var oldPosition = (Position2)pp.Clone(); // Make Clone
                var oldposition = pp.Pos;
                //var posPosition = pp.Pos;
                //var posQuantity = pp.Quantity;
                //var posOperation = pp.Operation;

                pp.LastTradeDT = t.DT;
                pp.LastTradeNumber = t.Number;
                pp.Price2 = t.Price;

                switch (t.Operation)
                {
                    case TradeOperationEnum.Buy:
                        pp.LastTradeBuyPrice = (float)t.Price;
                        break;
                    case TradeOperationEnum.Sell:
                        pp.LastTradeSellPrice = (float)t.Price;
                        break;
                }
                try
                {
                    if ((short)pp.Operation == (short)t.Operation)
                    // Rise Size !!! Position Operation the Same. For example: Buy & Buy
                    {
                        pp.Price1 = (pp.Price1 * pp.Quantity + t.Price * t.Quantity) / (pp.Quantity + t.Quantity);
                        pp.Quantity += t.Quantity;
                        //pp.Count += t.Quantity;
                        //pp.Comment += String.Format("Rise.{0} {1:C} {2} {3} {4}. ", pp.Pos, pp.Price1, t.Number,
                        //    t.Position, t.Price);
                        pp.LastChangedResult = PositionChangedEnum.ReSizedUp;

//                      pp.Strategy.PositionChanged(oldPosition, pp, PositionChangedEnum.ReSizedUp);
                        // PositionCollection.Add(pp);
                        //pp.Strategy.Strategies.OnStrategyTradeEntityChangedEvent(new Events.EventArgs
                        //{
                        //    Category = "UI.Positions",
                        //    Entity = "Current",
                        //    Operation = "Update",
                        //    Object = pp
                        //});
                        Strategy.OnChangedEvent(new Events.EventArgs
                        {
                            Category = "UI.Positions",
                            Entity = "Current",
                            Operation = "Update",
                            Object = pp,
                            Sender = this
                        });

                        _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, pp.StrategyTickerString, "Position2",
                            "ReSized Up",
                            pp.PositionString5 + " ReSized Up from " + oldPosition.PositionString5, pp.ToString());

                        return null;
                    }
                    //else
                    //{
                        // Position Opened and Position Status Not the Same with Trade Operation
                        // Close Flag Shoud Be S E E E E T

                        // Something Should be Close   Close  3 -3  ReSize 3 -1  Reverse 3 -5      

                        //_needClosedToObserve = +1;
                        //_needOpenedToObserve = +1;

                        if (pp.Pos + t.Position == 0)
                        // ONLY  Close Position -> addToClose and Clear Current Position
                        {
                            var p = new Deal
                            {
                                Strategy = pp.Strategy,
                                //Ticker = pp.Ticker,
                                //Account = pp.Account,

                                Operation = pp.Operation,
                                Quantity = t.Quantity,
                                Price1 = pp.Price1,
                                Price2 = t.Price,
                                PnL = (t.Price - pp.Price1) * t.Quantity * (short)pp.Operation,
                                
                                Status = PosStatusEnum.Closed,
                                LastChangedResult = PositionChangedEnum.Closed,

                                FirstTradeDT = pp.FirstTradeDT,
                                FirstTradeNumber = pp.FirstTradeNumber,
                                LastTradeDT = t.DT,
                                LastTradeNumber = t.Number,
                            };

                          //  pp.Strategy.AddDeal(p);
                            pp.Clear();
                            pp.LastChangedResult = PositionChangedEnum.Closed;

                            pp.DailyPnLFixed += p.PnL;
                            pp.PosPnLFixed += p.PnL;

                            // pp.Strategy.PositionChanged(oldPosition, pp, PositionChangedEnum.Closed);
                            //pp.Strategy.Strategies.OnStrategyTradeEntityChangedEvent(new Events.EventArgs
                            //{
                            //    Category = "UI.Positions",
                            //    Entity = "Current",
                            //    Operation = "Update",
                            //    Object = pp
                            //});
                            Strategy.OnChangedEvent(new Events.EventArgs
                            {
                                Category = "UI.Positions",
                                Entity = "Current",
                                Operation = "Update",
                                Object = pp,
                                Sender = this
                            });
                            // ********************************* Total ***************************
                            // PositionTotals.UpdateTotalOrNew(p);

                            pp.Strategy.PositionTotal.Update(p); // 07.01.2014
                            CreateStat();
                            FireTotalPositionUpdateEvent(pp.Strategy.PositionTotal);
                            //Strategy.OnChangedEvent(new Events.EventArgs
                            //{
                            //    Category = "UI.Positions",
                            //    Entity = "Total",
                            //    Operation = "Update",
                            //    Object = pp.Strategy.PositionTotal,
                            //    Sender = this
                            //});
                            //CreateStat();

                            // PositionClosedCollection.Add(pp); //Insert(0, pp);

                            //    _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, p.StrategyTickerString, "Position", "Closed",
                            //                                   oldPosition.PositionString3 + " -> " + pp.PositionString3,
                            //                                   p.ToString() + t.ToString());
                            return p;
                        }
                        //else //(pp.Pos + t.Position != 0)  Reduce  or Reverse Position 
                        //{
   
                            if (pp.Quantity > t.Quantity) // Reduce Size
                            {
                                // Open new "Close Position with Old LastTradeNumber

                               var p = new Deal
                                {
                                    Strategy = pp.Strategy,
                                    //Ticker = pp.Ticker,
                                    //Account = pp.Account,

                                    Operation = pp.Operation,
                                    Quantity = t.Quantity,
                                    Price1 = pp.Price1,
                                    Price2 = t.Price,
                                    PnL = (t.Price - pp.Price1) * t.Quantity * (short)pp.Operation,

                                    Status = PosStatusEnum.Closed,
                                    LastChangedResult = PositionChangedEnum.Closed,

                                    FirstTradeDT = pp.FirstTradeDT,
                                    FirstTradeNumber = pp.FirstTradeNumber,
                                    LastTradeDT = t.DT,
                                    LastTradeNumber = t.Number,
                                };

                                //pp.Strategy.AddDeal(p);

                                pp.LastChangedResult = PositionChangedEnum.ReSizedDown;
                                pp.Quantity = pp.Quantity - t.Quantity;
                                pp.Price2 = t.Price; // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
                                pp.PnL = (t.Price - pp.Price1) * pp.Pos;

                                pp.PnL3 += p.PnL;

                                pp.DailyPnLFixed += p.PnL;
                                pp.PosPnLFixed += p.PnL;

                                pp.LastTradeDT = t.DT;
                                pp.LastTradeNumber = t.Number;

                                //  pp.Comment += String.Format("Open.Reduce {0} {1:C} {2} {3} {4}. ",
                                //      pp.Pos, pp.Price1, t.Number, t.Position, t.Price);
                                //      pp.Index = ++_maxIndex;

                                // pp.Strategy.AddDeal(p);

                              //  pp.Strategy.PositionChanged(oldPosition, pp, PositionChangedEnum.ReSizedDown);

                                //pp.Strategy.Strategies.OnStrategyTradeEntityChangedEvent(new Events.EventArgs
                                //{
                                //    Category = "UI.Positions",
                                //    Entity = "Current",
                                //    Operation = "Update",
                                //    Object = pp
                                //});
                                Strategy.OnChangedEvent(new Events.EventArgs
                                {
                                    Category = "UI.Positions",
                                    Entity = "Current",
                                    Operation = "Update",
                                    Object = pp,
                                    Sender = this
                                });
                                // ************************************** Total *****************************
                                // PositionTotals.UpdateTotalOrNew(p);
                                //pp.PositionTotal.Update(p); // 15.11.2012
                                pp.Strategy.PositionTotal.Update(p);
                                CreateStat();
                                FireTotalPositionUpdateEvent(pp.Strategy.PositionTotal);
                                //Strategy.OnChangedEvent(new Events.EventArgs
                                //{
                                //    Category = "UI.Positions",
                                //    Entity = "Total",
                                //    Operation = "Update",
                                //    Object = pp.Strategy.PositionTotal,
                                //    Sender = this
                                //});
                                // CreateStat();
                                // PositionClosedCollection.Add(pp); // Insert(0, pp);

                                _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, pp.StrategyTickerString,
                                    "Position2", "ReSized Down",
                                    pp.PositionString5 + " ReSized Down from " + oldPosition.PositionString5,
                                    pp.ToString());
                                //_eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                //                               p.PositionString3, "Close" + t.PositionString.WithSqBrackets(),
                                //                               p.ToString(), t.ToString());
                                return p;
                            }
                            if (pp.Quantity < t.Quantity) // REVERSE 
                            {
                                // Open New Close Position
                               
                                var p = new Deal
                                {
                                    Strategy = pp.Strategy,
                                    //Ticker = pp.Ticker,
                                    //Account = pp.Account,

                                    Operation = pp.Operation,
                                    Quantity = pp.Quantity,
                                    Price1 = pp.Price1,
                                    Price2 = t.Price,
                                    PnL = (t.Price - pp.Price1) * pp.Quantity * (short)pp.Operation,
                                    
                                    Status = PosStatusEnum.Closed,
                                    LastChangedResult = PositionChangedEnum.Closed,

                                    FirstTradeDT = pp.FirstTradeDT,
                                    FirstTradeNumber = pp.FirstTradeNumber,
                                    LastTradeDT = t.DT,
                                    LastTradeNumber = t.Number,
                                };

                                //pp.Strategy.AddDeal(p);

                                pp.LastChangedResult = PositionChangedEnum.Reversed;

                                pp.Operation = Position.Reverse(pp.Operation);
                                pp.Quantity = t.Quantity - pp.Quantity;

                                pp.Price1 = t.Price;
                                pp.Price2 = t.Price;

                                pp.PnL = (t.Price - pp.Price1) * pp.Pos;

                                pp.DailyPnLFixed += p.PnL;
                                pp.PosPnLFixed = 0;

                                pp.FirstTradeDT = t.DT;
                                pp.FirstTradeNumber = t.Number;
                                pp.LastTradeDT = t.DT;
                                pp.LastTradeNumber = t.Number;

                                Strategy.OnChangedEvent(new Events.EventArgs
                                {
                                    Category = "UI.Positions",
                                    Entity = "Current",
                                    Operation = "Update",
                                    Object = pp,
                                    Sender = this
                                });

                                // !!!! OUT of Range pp.Comment += String.Format("Open.Reverse {0} {1:C} {2} {3} {4}. ", pp.Pos,
                                //  pp.Price1,
                                //  t.Number, t.Position, t.Price);
                                //  pp.Index = ++_maxIndex;


                                // pp.Strategy.PositionChanged(oldPosition, pp, PositionChangedEnum.Reversed);
                                // *************** Total ******************************************
                                // PositionTotals.UpdateTotalOrNew(p);
                                //pp.PositionTotal.Update(p); // 15.11.2012
                                pp.Strategy.PositionTotal.Update(p);
                                CreateStat();
                                FireTotalPositionUpdateEvent(pp.Strategy.PositionTotal);
                                //Strategy.OnChangedEvent(new Events.EventArgs
                                //{
                                //    Category = "UI.Positions",
                                //    Entity = "Total",
                                //    Operation = "Update",
                                //    Object = pp.Strategy.PositionTotal,
                                //    Sender = this
                                //});
                                // CreateStat();
                                //    _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, p.StrategyTickerString, "Position",
                                //                      p.PositionString3 + " Reverse.Close" + t.PositionString.WithSqBrackets(),
                                //                      p.ToString(), t.ToString());
                                _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, pp.StrategyTickerString,
                                    "Position2", "Reversed",
                                    pp.PositionString5 + " Reversed from " + oldPosition.PositionString5, pp.ToString());
                                return p;
                            }

                            _eventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING, "Position2",
                                    "Position2", "Nobody's Fools - ???",
                                $"{pp.Pos} {pp.Quantity} {t.Position} {t.Quantity}",
                                    t.ToString());

                            _eventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING, "Position2",
                                    "Position2", "Nobody's Fools - ???",
                                    pp.ToString(), t.ToString());

                            throw new NullReferenceException( "Positon.NewTrade Failure: " + t );
                }
                catch (Exception e)
                {
                    throw new Exception("Position.NewTrade() Faillure: " + e.Message);
                }
            }
            if (pp.IsNeutral) // Position not IsOpened
            {
                // **************************************** _needOpenedToObserve = +1;
                // pp = PositionCurrentCollection.FirstOrDefault(p => p.StrategyKey == t.StrategyKey && p.IsNeutral);

                var oldPosition = (Position2)pp.Clone(); // Make Clone

                pp.Operation = (PosOperationEnum)t.Operation;
                pp.Quantity = t.Quantity;
                pp.Price1 = t.Price;
                pp.Price2 = t.Price;
                pp.Status = PosStatusEnum.Opened;

                pp.FirstTradeDT = t.DT;
                pp.FirstTradeNumber = t.Number;

                pp.LastTradeDT = t.DT;
                pp.LastTradeNumber = t.Number;

                PosPnLFixed = 0;

                pp.LastChangedResult = PositionChangedEnum.Opened;

                if (t.Operation == GS.Trade.TradeOperationEnum.Buy)
                {
                    pp.LastTradeBuyPrice = (float)t.Price;
                    pp.LastTradeSellPrice = 0f;
                }
                else if (t.Operation == GS.Trade.TradeOperationEnum.Sell)
                {
                    pp.LastTradeSellPrice = (float)t.Price;
                    pp.LastTradeBuyPrice = 0f;
                }
                //  pp.Index = ++ _maxIndex;
                Strategy.OnChangedEvent(new Events.EventArgs
                {
                    Category = "UI.Positions",
                    Entity = "Current",
                    Operation = "Update",
                    Object = pp,
                    Sender = this
                });
                var methodname = MethodBase.GetCurrentMethod().Name + "()";
                //pp.InvokePositionChangedEvent(0, pp.Pos);
                //pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.Opened);
                //pp.Strategy.PositionChanged(oldPosition, pp, PositionChangedEnum.Opened);
                // ****************************************** _needOpenedToObserve = +1;

                //_eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, pp.StrategyTimeIntTickerString, "Position New",
                //                               pp.PositionString5, pp.ToString(),"");
                _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, pp.StrategyTimeIntTickerString, ShortInfo,
                    methodname, ShortDescription, pp.ToString());
                return null;
            }
            return null;

            // *************************** 
            //Positions.FireObserverEvent();
        }
        public void Update(IPosition2 p)
        {
            if (p.IsLong)
            {
                Price1 = (Price1 * Quantity + p.Price1 * p.Quantity) / (Quantity + p.Quantity);
                Price2 = (Price2 * Quantity + p.Price2 * p.Quantity) / (Quantity + p.Quantity);
            }
            else if (p.IsShort)
            {
                Price1 = (Price1 * Quantity + p.Price2 * p.Quantity) / (Quantity + p.Quantity);
                Price2 = (Price2 * Quantity + p.Price1 * p.Quantity) / (Quantity + p.Quantity);
            }

            if (Quantity == 0)
            {
                FirstTradeDT = p.FirstTradeDT;
                FirstTradeNumber = p.FirstTradeNumber;
            }
            Quantity += p.Quantity;
            PnL += (p.Price2 - p.Price1) * p.Pos;

            LastTradeDT = p.LastTradeDT;
            LastTradeNumber = p.LastTradeNumber;

            // CreateStat();

            //p.Strategy.Strategies.OnStrategyTradeEntityChangedEvent(new Events.EventArgs
            //{
            //    Category = "UI.Positions",
            //    Entity = "Total",
            //    Operation = "Update",
            //    Object = this
            //});

            // Price2 = p.Price2;
        }
        public static PosOperationEnum Reverse(PosOperationEnum operation)
        {
            return operation == PosOperationEnum.Long
                         ? PosOperationEnum.Short
                         : operation == PosOperationEnum.Short
                            ? PosOperationEnum.Long
                            : PosOperationEnum.Neutral;
        }
        private void Reverse()
        {
            Operation = Operation == PosOperationEnum.Long 
                ? PosOperationEnum.Short 
                : ( Operation == PosOperationEnum.Short 
                    ? PosOperationEnum.Long 
                    : PosOperationEnum.Neutral);
        }
        public virtual void Clear()
        {
            FirstTradeDT = DateTime.MinValue;
            FirstTradeNumber = 0;
            //LastTradeDT = DateTime.MinValue;
            //LastTradeNumber = 0;

            Quantity = 0;

            Operation = PosOperationEnum.Neutral;
            Status = PosStatusEnum.Closed;

            LastChangedResult = PositionChangedEnum.Closed;

            Price1 = 0;
            Price2 = 0;
            PnL = 0;
            PnL3 = 0;

            Count = 0;

            //TotalDailyMaxProfit = 0;
            //TotalDailyMaxLoss = 0;

            Comment = "";
            Strategy.OnChangedEvent(new Events.EventArgs
            {
                Category = "UI.Positions",
                Entity = "Current",
                Operation = "Update",
                Object = this,
                Sender = this
            });
        }
        public void FireTotalPositionUpdateEvent(IPosition2 p)
        {
            if (p == null) return;
            // var pcl = p.Clone();
            Strategy.OnChangedEvent(new Events.EventArgs
            {
                Category = "UI.Positions",
                Entity = "Total",
                Operation = "Update",
                Object = p,
                Sender = this
            });
        }
        public void InvokePositionChangedEvent(long oldp, long newp)
        {
            PositionChangedEvent?.Invoke(oldp, newp);
        }
        public void InvokePositionChangedEvent(Position2 old, PositionChangedEnum posChangedResult)
        {
            PositionChangedEvent3?.Invoke(old, this, posChangedResult);
        }
        protected int GetStrategyKeyHash()
        {
            return Key.GetHashCode();
        }
        public override string ToString()
        {
            return
                $"{AccountCode} {StrategyCode} {TickerCode} {Pos:N0} {Price1:N2} {Price2:N2} {PnLString} {OperationStringUpper} {StatusStringUpper} {FirstTradeNumber} {LastTradeNumber} {FirstTradeDT:G} {LastTradeDT:G} {Id}";
        }
        public IPosition2 Clone()
        {
            var p = new Position2
            {
                Strategy = Strategy,
                //Account = Account,
                //Ticker = Ticker,

                AccountCodeEx = AccountCodeEx,
                TradeBoardEx = TradeBoardEx,
                TickerCodeEx = TickerCodeEx,
                
                FirstTradeNumber = FirstTradeNumber,
                FirstTradeDT = FirstTradeDT,
                LastTradeNumber = LastTradeNumber,
                LastTradeDT = LastTradeDT,

                Price1 = Price1,
                Price2 = Price2,

                Operation = Operation,
                Quantity = Quantity,

                Status = Status,
                LastChangedResult = LastChangedResult,

                PnL = PnL,
                //PnL2 = PnL2,
                PosPnLFixed = PosPnLFixed,
                DailyPnLFixed = DailyPnLFixed,

                Count = Count,
                
                Comment = Comment,
                Index = Index,
                PositionTotal = PositionTotal,

                TotalDailyMaxProfit = TotalDailyMaxProfit,
                TotalDailyMaxProfitDT  = TotalDailyMaxProfitDT,
                TotalDailyMaxLoss = TotalDailyMaxLoss,
                TotalDailyMaxLossDT = TotalDailyMaxLossDT
                //Delta = Delta
            };
            return p;
        }
    
        // ILineXY Chart Interface
        public DateTime LineX1 => FirstTradeDT;

        public DateTime LineX2 => LastTradeDT;
        public double LineY1 => (double)Price1;
        public double LineY2 => (double)Price2;
        public int LineColor => PnL > 0 ? 0x0000ff : 0xff0000;
        public int LineWidth => 3;
    }

    public class PositionDb2 : TradeEntity, IPositionDb
    {
        private IEventLog _eventLog;

        private float _lastTradeBuyPrice;
        private float _lastTradeSellPrice;

        public string Symbol { get; set; }

        public IEventLog EventLog
        {
            get { return _eventLog; }
            set { _eventLog = value; }
        }

        public Positions2 Positions { get; set; }

        public event PositionChangedEventHandler PositionChangedEvent;
        public event PositionChangedEventHandler2 PositionChangedEvent2;
        public event PositionChangedEventHandler3 PositionChangedEvent3;

        //public string Key {
        //    get { return String.Format("{0}@{1}", GetType(), StrategyKey); }
        //}
        public string Key => StrategyKey;

        public int StrategyKeyHash { get; set; }

        //private static long _positionIdRoot;

        public IPositionTotal2 PositionTotal2 { get; set; }
        public IPosition2 PositionTotal { get; set; }

        public decimal FirstTradeNumber { get; set; }
        public DateTime FirstTradeDT { get; set; }

        public decimal LastTradeNumber { get; set; }
        public DateTime LastTradeDT { get; set; }

        public decimal PosPnLFixed { get; set; }
        public decimal DailyPnLFixed { get; set; }

        public IBar LastBar { get; private set; }
        public void SetLastBar(IBar b) { LastBar = b; }
        public DateTime LocalBarDateTime => LastBar?.DT ?? DateTime.Now;

        public int TotalDays => (LastTradeDT.Date - FirstTradeDT.Date).Days + 1;
        public virtual decimal DailyPnLAvg => TotalDays != 0 ? PnL / TotalDays : 0;
        public string DailyPnLAvgFormated => DailyPnLAvg.ToString(FormatAvgStr);
        public decimal TradeAvg1 => Quantity != 0 ? PnL / Quantity : 0;
        public string TradeAvg1Formated => TradeAvg1.ToString(FormatAvgStr);
        public float LastTradePrice { get; set; }
        public float LastTradeBuyPrice
        {
            get { return _lastTradeBuyPrice; }
            set
            {
                _lastTradeBuyPrice = value;
                LastTradePrice = value;
            }
        }
        public decimal Price1 { get; set; }
        public decimal Price2 { get; set; }
        public decimal Price3 => Price1 - (!IsNeutral && Quantity != 0 ? (short)Operation * PnL3 / Quantity : 0);

        public decimal LastPrice { get; set; }
        public virtual float LastTradeSellPrice
        {
            get { return _lastTradeSellPrice; }
            set { _lastTradeSellPrice = value; LastTradePrice = value; }
        }

        public decimal Amount1 => Quantity * Price1;
        public decimal Amount2 => Quantity * Price2;
        public PosOperationEnum Operation { get; set; }
        public PosOperationEnum PosOperation => Operation;
        public long Quantity { get; set; }
        public long TotalQuantity => PositionTotal?.Quantity ?? 0;
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
        public double Delta => Ticker?.Delta * Pos ?? 0;
        public decimal TotalDailyMaxProfit { get; set; }
        public DateTime TotalDailyMaxProfitDT { get; set; }
        public decimal TotalDailyMaxLoss { get; set; }
        public DateTime TotalDailyMaxLossDT { get; set; }

        public void CreateStat()
        {
            if (DailyPnLFixed > TotalDailyMaxProfit)
            {
                TotalDailyMaxProfit = DailyPnLFixed;
                TotalDailyMaxProfitDT = LocalBarDateTime;
            }
            if (DailyPnLFixed < TotalDailyMaxLoss)
            {
                TotalDailyMaxLoss = DailyPnLFixed;
                TotalDailyMaxLossDT = LocalBarDateTime;
            }

            //var p = Clone();
            //Strategy.OnChangedEvent(new Events.EventArgs
            //{
            //    Category = "UI.Positions",
            //    Entity = "Total.MaxMinProfit",
            //    Operation = "Update",
            //    Object = p,
            //    Sender = this
            //});
        }

       
        public void Update(IPosition2 p)
        {
            if (p.IsLong)
            {
                Price1 = (Price1 * Quantity + p.Price1 * p.Quantity) / (Quantity + p.Quantity);
                Price2 = (Price2 * Quantity + p.Price2 * p.Quantity) / (Quantity + p.Quantity);
            }
            else if (p.IsShort)
            {
                Price1 = (Price1 * Quantity + p.Price2 * p.Quantity) / (Quantity + p.Quantity);
                Price2 = (Price2 * Quantity + p.Price1 * p.Quantity) / (Quantity + p.Quantity);
            }

            if (Quantity == 0)
            {
                FirstTradeDT = p.FirstTradeDT;
                FirstTradeNumber = p.FirstTradeNumber;
            }
            Quantity += p.Quantity;
            PnL += (p.Price2 - p.Price1) * p.Pos;

            LastTradeDT = p.LastTradeDT;
            LastTradeNumber = p.LastTradeNumber;

            // CreateStat();

            //p.Strategy.Strategies.OnStrategyTradeEntityChangedEvent(new Events.EventArgs
            //{
            //    Category = "UI.Positions",
            //    Entity = "Total",
            //    Operation = "Update",
            //    Object = this
            //});

            // Price2 = p.Price2;
        }
        public static PosOperationEnum Reverse(PosOperationEnum operation)
        {
            return operation == PosOperationEnum.Long
                         ? PosOperationEnum.Short
                         : operation == PosOperationEnum.Short
                            ? PosOperationEnum.Long
                            : PosOperationEnum.Neutral;
        }
        private void Reverse()
        {
            Operation = Operation == PosOperationEnum.Long
                ? PosOperationEnum.Short
                : (Operation == PosOperationEnum.Short
                    ? PosOperationEnum.Long
                    : PosOperationEnum.Neutral);
        }
        public virtual void Clear()
        {
            FirstTradeDT = DateTime.MinValue;
            FirstTradeNumber = 0;
            //LastTradeDT = DateTime.MinValue;
            //LastTradeNumber = 0;

            Quantity = 0;

            Operation = PosOperationEnum.Neutral;
            Status = PosStatusEnum.Closed;

            LastChangedResult = PositionChangedEnum.Closed;

            Price1 = 0;
            Price2 = 0;
            PnL = 0;
            PnL3 = 0;

            Count = 0;

            //TotalDailyMaxProfit = 0;
            //TotalDailyMaxLoss = 0;

            Comment = "";
            Strategy.OnChangedEvent(new Events.EventArgs
            {
                Category = "UI.Positions",
                Entity = "Current",
                Operation = "Update",
                Object = this,
                Sender = this
            });
        }
        public void FireTotalPositionUpdateEvent(IPosition2 p)
        {
            if (p == null) return;
            // var pcl = p.Clone();
            Strategy.OnChangedEvent(new Events.EventArgs
            {
                Category = "UI.Positions",
                Entity = "Total",
                Operation = "Update",
                Object = p,
                Sender = this
            });
        }
        public void InvokePositionChangedEvent(long oldp, long newp)
        {
            PositionChangedEvent?.Invoke(oldp, newp);
        }
        
        protected int GetStrategyKeyHash()
        {
            return Key.GetHashCode();
        }
        public override string ToString()
        {
            return
                $"{AccountCode} {StrategyCode} {TickerCode} {Pos:N0} {Price1:N2} {Price2:N2} {PnLString} {OperationStringUpper} {StatusStringUpper} {FirstTradeNumber} {LastTradeNumber} {FirstTradeDT:G} {LastTradeDT:G} {Id}";
        }
        public IPositionDb Clone()
        {
            var p = new PositionDb2
            {
                Strategy = Strategy,
                //Account = Account,
                //Ticker = Ticker,

                AccountCodeEx = AccountCodeEx,
                TradeBoardEx = TradeBoardEx,
                TickerCodeEx = TickerCodeEx,

                FirstTradeNumber = FirstTradeNumber,
                FirstTradeDT = FirstTradeDT,
                LastTradeNumber = LastTradeNumber,
                LastTradeDT = LastTradeDT,

                Price1 = Price1,
                Price2 = Price2,

                Operation = Operation,
                Quantity = Quantity,

                Status = Status,
                LastChangedResult = LastChangedResult,

                PnL = PnL,
                //PnL2 = PnL2,
                PosPnLFixed = PosPnLFixed,
                DailyPnLFixed = DailyPnLFixed,

                Count = Count,

                Comment = Comment,
                Index = Index,
                PositionTotal = PositionTotal,

                TotalDailyMaxProfit = TotalDailyMaxProfit,
                TotalDailyMaxProfitDT = TotalDailyMaxProfitDT,
                TotalDailyMaxLoss = TotalDailyMaxLoss,
                TotalDailyMaxLossDT = TotalDailyMaxLossDT
                //Delta = Delta
            };
            return p;
        }
    }
}
