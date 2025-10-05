using System;
using System.ComponentModel;
using GS.Extension;
using GS.ICharts;
// using GS.Trade.Data;
using GS.Interfaces;
using GS.Trade.Trades.Trades3;

namespace GS.Trade.Trades
{
    public class Position : ILineXY, ICloneable, IPosition
    {
      //  public delegate void PositionChangedEventHandler(long oldPosition, long newPosition);
      //  public delegate void PositionChangedEventHandler2(Position oldPosition, Position newPosition, PositionChangedEnum changedResult);

        private IEventLog _eventLog;
        public IEventLog EventLog
        {
            get { return _eventLog; }
            set { _eventLog = value; }
        }
        //public ITradeContext TradeContext { get; set; }

        public Positions Positions { get; set; }

        public event PositionChangedEventHandler PositionChangedEvent;
        public event PositionChangedEventHandler2 PositionChangedEvent2;

         //public enum StatusEnum : short { Closed = 0, Opened = 1  }
         //public enum OperationEnum : short { Neutral = 0, Long = +1, Short = -1 }

        public int StrategyKeyHash { get; set; }

        private static long _positionIdRoot;

        public long PositionId { get; set; }

        public string Account { get; set; }
        public string Strategy { get; set; }
        public string TickerStr { get; set; }

        public ITicker Ticker { get; set; }
      //  public IStrategy MyStrategy { get; set; }

        //public Positions.Totals.PositionTotal PositionTotal { get; set; }
        public IPositionTotal PositionTotal { get; set; }

        public virtual ulong FirstTradeNumber { get; set; }
        public virtual DateTime FirstTradeDT { get; set; }

        public virtual ulong LastTradeNumber { get; set; }
        //public PosOperationEnum Operation { get; set; }
        public virtual DateTime LastTradeDT { get; set; }
        //public decimal LastTradePrice { get; set; }

        public virtual decimal Price1 { get; set; }
        public virtual decimal Price2 { get; set; }
        public virtual float LastTradeBuyPrice { get; set; }
        public virtual float LastTradeSellPrice { get; set; }

        public decimal Amount1 { get { return Quantity * Price1; } }
        public decimal Amount2 { get { return Quantity * Price2; } }

        public virtual PosOperationEnum Operation { get; set; }

        public PosOperationEnum PosOperation {
            get { return (PosOperationEnum) Operation; }
        }

        public virtual long Quantity { get; set; }     
        
        public long Pos { get { return Quantity * (short)Operation; }}
        public string OperationString { get { return Operation.ToString().ToUpper(); } }
        public string StrategyTickerString { get { return Strategy + "." + TickerStr; } }

        public string PositionInfo
        {
            get
            {
                return string.Format("Position: {0}: [{1}{2}] P&L: {3:N2}",
                                     OperationString, Operation > 0 ? "+" : Operation < 0 ? "-" : "", Quantity, Profit);
            }
        }

        public decimal Pnl { get; set; }
        public virtual decimal PnL2 { get; set; }

        public long Count { get; set; }
        public virtual PosStatusEnum Status { get; set; }

        public PosStatusEnum PosStatus
        {
            get { return (PosStatusEnum) Status; }
        }

        public bool Opened { get { return Status == PosStatusEnum.Opened; } }
        public bool IsOpened { get { return Status == PosStatusEnum.Opened; } }

        public bool Closed { get { return Status == PosStatusEnum.Closed; } }
        public bool IsClosed { get { return Status == PosStatusEnum.Closed; } }

        public bool IsLong { get { return Operation == PosOperationEnum.Long; } }
        public bool IsShort { get { return Operation == PosOperationEnum.Short; } }
        public bool IsNeutral { get { return Operation == PosOperationEnum.Neutral; } }

        public PosOperationEnum FlipOperation
        {
            get
            {
                return IsLong 
                    ? PosOperationEnum.Short 
                    : ( IsShort
                        ? PosOperationEnum.Long
                        : PosOperationEnum.Neutral);
            }
        }

        public float Profit
        {
            get
            {
                return Ticker != null ? (float) (Ticker.LastPrice - (double) Price1) * Pos : 0f;

                /*
                return IsLong
                           ? (float)(Ticker.LastPrice - (double) Price1)
                           : IsShort
                                 ? (float)((double) Price1 - Ticker.LastPrice)
                                 : 0f;
                 */
            }
        }

        public string StatusString { get { return Status.ToString().ToUpper(); } }
        public string Comment { get; set; }
        public long Index { get; set; }

        
        public string TickerCode { get { return Ticker != null ? Ticker.Code : TickerStr; } }
        public string TradeKey { get { return Strategy + Account + TickerCode; } }

        public string FirstTimeDateString
        {
            get { return FirstTradeDT.ToString("T") + ' ' + FirstTradeDT.ToString("d"); }
        }
        public string LastTimeDateString
        {
            get { return LastTradeDT.ToString("T") + ' ' + LastTradeDT.ToString("d"); }
        }

        public string TickerFormat { get { return Ticker != null ? Ticker.Format : "N2"; } }
        public string TickerFormatAvg { get { return Ticker != null ? Ticker.FormatAvg : "N2"; } }
        public string TickerFormatM { get { return Ticker != null ? Ticker.FormatM : "N2"; } }

        public string LastTradeDTString { get { return LastTradeDT.ToString("G"); } }
        public string FirstTradeDTString { get { return FirstTradeDT.ToString("G"); } }

        public string PositionString { get { return (Pos > 0 ? "+" : "") + Pos.ToString("N0"); } }
        public string PositionString2 { get { return (Pos > 0 ? "+" : (Pos < 0 ? "-" : string.Empty)) + Pos.ToString("N0"); } }
        public string PositionString3 {
            get { return TickerCode + "." + OperationString + PositionString2.WithSqBrackets(); }
        }

        public string PositionString5 {
            get { return PositionString3 + EntryPriceString; }
        }
        public string PositionString6
        {
            get { return PositionString3 + "P&L: " + ProfitTotalString; }
        }

        public string EntryPriceString {
            get { return "EntryPrice: " + AvgPriceString; }
        }

        public string AmountString { get { return Amount1.ToString(TickerFormatAvg); } }

        public string AvgPriceString { get { return Price1.ToString(TickerFormatAvg); } }
        public string AvgPrice2String { get { return Price2.ToString(TickerFormatAvg); } }

        public string LastTradePriceString { get { return Price2.ToString(TickerFormat); } }
        public decimal PnL { get { return (Price2 - Price1) * Pos; } }
        public string PnLString { get { return PnL.ToString(TickerFormatM); } }

        public string PnL2String { get { return PnL2.ToString(TickerFormatM); } }
        //public decimal TotalPnL { get { return (Price2 - Price1) * Quantity; } }
        //public string TotalPnLString { get { return TotalPnL.ToString(TickerFormatM); } }
        //public string PnLString { get { return ((Price2 - Price1)*Pos).ToString(TickerFormatAvg); } }
        public string ProfitTotalString { get { return ((Price2 - Price1) * Pos).ToString(TickerFormatAvg); } }

        public string StrategyKey { get { return Account + Strategy + TickerCode; } }

        public Position() { }
        public Position(string account, string strategy, string ticker,  PosOperationEnum operation,  long quantity,
                        decimal firsttradeprice, decimal lasttradeprice, decimal pnl, PosStatusEnum status,
                        DateTime mindt, ulong minnumber, DateTime maxdt, ulong maxnumber)
        {
            Account = account; Strategy = strategy; TickerStr = ticker;

            Operation = operation;
            Quantity = quantity;       
            //Amount = amount;
            Pnl = pnl;
            FirstTradeNumber = minnumber;
            FirstTradeDT = mindt;
            // CloseTradeNumber = maxnumber; DT_Close = maxdt;
            LastTradeNumber = maxnumber; 
            LastTradeDT = maxdt;
            //LastTradePrice = lasttradeprice;
            Price1 = firsttradeprice;
            Price2 = lasttradeprice;   
            Count = quantity;
            //OpenCloseFlag = 1;

            Status = status;

            StrategyKeyHash = GetStrategyKeyHash();

            lock (this)
            {
                _positionIdRoot++;
                PositionId = _positionIdRoot;
            }

            // string s = "\"";
        }
        public Position( Trade3 t )
        {
            Account = t.AccountCode; 
            Strategy = t.StrategyCode; 
            TickerStr = t.TickerCode;

            Operation = t.Operation == TradeOperationEnum.Buy 
                        ? PosOperationEnum.Long : 
                        (t.Operation == TradeOperationEnum.Sell?PosOperationEnum.Short:PosOperationEnum.Neutral);
            Quantity = t.Quantity;
            //Amount = amount;
            Pnl = 0;
            FirstTradeNumber = t.Number;
            FirstTradeDT = t.DT;
            // CloseTradeNumber = maxnumber; DT_Close = maxdt;
            LastTradeNumber = FirstTradeNumber;
            LastTradeDT = FirstTradeDT;
            //LastTradePrice = t.Price;
            Price1 = t.Price;
            Count = t.Quantity;
            //OpenCloseFlag = 1;

            Status = PosStatusEnum.Opened;

            // StrategyKeyHash = t.StrategyKeyHash;

            lock (this)
            {
                _positionIdRoot++;
                PositionId = _positionIdRoot;
            }

        }

        public void NewTrade(ITrade3 t)
        {
            var pp = this;
            if (pp.IsOpened)
            {
                var oldPosition = (Position)pp.Clone(); // Make Clone
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

                        pp.InvokePositionChangedEvent(oldposition, pp.Pos);
                        pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.ReSizedUp);
                        // PositionCollection.Add(pp);

                        _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, pp.StrategyTickerString, "Position2",
                            "ReSized Up",
                            pp.PositionString5 + " ReSized Up from " + oldPosition.PositionString5, pp.ToString());
                    }
                    else
                    {
                        // Position Opened and Position Status Not the Same with Trade Operation
                        // Close Flag Shoud Be S E E E E T

                        // Something Should be Close   Close  3 -3  ReSize 3 -1  Reverse 3 -5      

                        //_needClosedToObserve = +1;
                        //_needOpenedToObserve = +1;

                        if (pp.Pos + t.Position == 0)
                        // ONLY Only Close Position -> addToClose and Clear Current Position
                        {
                            //   Delete(pp, PositionOpenedCollection);

                            //_needClosedToObserve = +1;
                            //_needOpenedToObserve = +1;

                            var p = new Position(pp.Account, pp.Strategy, pp.TickerStr, pp.Operation,
                                pp.Quantity,
                                pp.Price1, t.Price, (t.Price - pp.Price1) * pp.Pos,
                                PosStatusEnum.Closed,
                                pp.FirstTradeDT, pp.FirstTradeNumber, t.DT, t.Number)
                            {
                                Ticker = pp.Ticker,
                                //Comment = pp.Comment + String.Format("Close {0} {1:C} {2} {3} {4}. ",
                                //    pp.Pos, pp.Price1, t.Number,
                                //    t.Position, t.Price)
                            };

                            pp.Clear();

                            Positions.AddPosition(p);

                            // pp.Clear();
                            //   pp.Index = ++_maxIndex;

                            pp.InvokePositionChangedEvent(oldposition, pp.Pos);
                            pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.Closed);

                            // ********************************* Total ***************************
                            // PositionTotals.UpdateTotalOrNew(p);
                            pp.PositionTotal.Update(p); // 15.11.2012

                            // PositionClosedCollection.Add(pp); //Insert(0, pp);

                            //    _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, p.StrategyTickerString, "Position", "Closed",
                            //                                   oldPosition.PositionString3 + " -> " + pp.PositionString3,
                            //                                   p.ToString() + t.ToString());
                        }
                        else //(pp.Pos + t.Position != 0)  Reduce  or Reverse Position 
                        {
                            if (pp.Quantity > t.Quantity) // Reduce Size
                            {
                                // Open new "Close Position with Old LastTradeNumber

                                var p = new Position(pp.Account, pp.Strategy, pp.TickerStr, pp.Operation,
                                    t.Quantity,
                                    pp.Price1, t.Price, (t.Price - pp.Price1) * pp.Pos, PosStatusEnum.Closed,
                                    pp.FirstTradeDT, pp.FirstTradeNumber, t.DT, t.Number)
                                {
                                    Ticker = pp.Ticker,
                                    //Index = ++_maxIndex,
                                    //Comment =
                                    //    pp.Comment +
                                    //    String.Format("Close.Reduce Size{0} {1:C} {2} {3} {4}. ",
                                    //        pp.Pos, pp.Price1, t.Number, t.Position, t.Price)
                                };
                                // AddPosition(p);

                                pp.Quantity = pp.Quantity - t.Quantity;
                                pp.Price2 = t.Price; // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
                                pp.Pnl = (t.Price - pp.Price1) * pp.Pos;

                                pp.LastTradeDT = t.DT;
                                pp.LastTradeNumber = t.Number;

                                //  pp.Comment += String.Format("Open.Reduce {0} {1:C} {2} {3} {4}. ",
                                //      pp.Pos, pp.Price1, t.Number, t.Position, t.Price);
                                //      pp.Index = ++_maxIndex;

                                Positions.AddPosition(p); // need to refresh

                                pp.InvokePositionChangedEvent(oldposition, pp.Pos);
                                pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.ReSizedDown);

                                // ************************************** Total *****************************
                                // PositionTotals.UpdateTotalOrNew(p);
                                pp.PositionTotal.Update(p); // 15.11.2012

                                // PositionClosedCollection.Add(pp); // Insert(0, pp);

                                _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, pp.StrategyTickerString,
                                    "Position2", "ReSized Down",
                                    pp.PositionString5 + " ReSized Down from " + oldPosition.PositionString5,
                                    pp.ToString());
                                //_eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                //                               p.PositionString3, "Close" + t.PositionString.WithSqBrackets(),
                                //                               p.ToString(), t.ToString());
                            }
                            else if (pp.Quantity < t.Quantity) // REVERSE 
                            {
                                // Open New Close Position

                                var p = new Position(pp.Account, pp.Strategy, pp.TickerStr, pp.Operation,
                                    pp.Quantity,
                                    pp.Price1, t.Price, (t.Price - pp.Price1) * pp.Pos, PosStatusEnum.Closed,
                                    pp.FirstTradeDT, pp.FirstTradeNumber, t.DT, t.Number)
                                {
                                    Ticker = pp.Ticker,
                                    //   Index = ++_maxIndex,
                                    // !!!! OUT of Range   Comment =
                                    //   pp.Comment +
                                    //  String.Format("Close.Reverse {0} {1:C} {2} {3} {4}.", pp.Pos,
                                    //                      pp.Price1,
                                    //                      t.Number, t.Position, t.Price)
                                };
                                Positions.AddPosition(p);

                                pp.Operation = Position.Reverse(pp.Operation);
                                pp.Quantity = t.Quantity - pp.Quantity;

                                pp.Price1 = t.Price;
                                pp.Price2 = t.Price;

                                pp.Pnl = (t.Price - pp.Price1) * pp.Pos;

                                pp.FirstTradeDT = t.DT;
                                pp.FirstTradeNumber = t.Number;
                                pp.LastTradeDT = t.DT;
                                pp.LastTradeNumber = t.Number;

                               // ***************** _needOpenedToObserve = +1;

                                // !!!! OUT of Range pp.Comment += String.Format("Open.Reverse {0} {1:C} {2} {3} {4}. ", pp.Pos,
                                //  pp.Price1,
                                //  t.Number, t.Position, t.Price);
                                //  pp.Index = ++_maxIndex;

                                pp.InvokePositionChangedEvent(oldposition, pp.Pos);
                                pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.Reversed);

                                // *************** Total ******************************************
                                // PositionTotals.UpdateTotalOrNew(p);
                                pp.PositionTotal.Update(p); // 15.11.2012


                                //    _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, p.StrategyTickerString, "Position",
                                //                      p.PositionString3 + " Reverse.Close" + t.PositionString.WithSqBrackets(),
                                //                      p.ToString(), t.ToString());
                                _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, pp.StrategyTickerString,
                                    "Position2", "Reversed",
                                    pp.PositionString5 + " Reversed from " + oldPosition.PositionString5, pp.ToString());
                            }
                            else
                            {
                                _eventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING, "Position2",
                                    "Position2", "Nobody's Fools - ???",
                                    String.Format("{0} {1} {2} {3}", pp.Pos,
                                        pp.Quantity, t.Position, t.Quantity),
                                    t.ToString());

                                _eventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING, "Position2",
                                    "Position2", "Nobody's Fools - ???",
                                    pp.ToString(), t.ToString());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            else if ( pp.IsNeutral) // Position not IsOpened
            {
               // **************************************** _needOpenedToObserve = +1;
                // pp = PositionCurrentCollection.FirstOrDefault(p => p.StrategyKey == t.StrategyKey && p.IsNeutral);

                var oldPosition = (Position)pp.Clone(); // Make Clone

                pp.Operation = (PosOperationEnum)t.Operation;
                pp.Quantity = t.Quantity;
                pp.Price1 = t.Price;
                pp.Price2 = t.Price;
                pp.Status = PosStatusEnum.Opened;

                pp.FirstTradeDT = t.DT;
                pp.FirstTradeNumber = t.Number;

                pp.LastTradeDT = t.DT;
                pp.LastTradeNumber = t.Number;

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

                pp.InvokePositionChangedEvent(0, pp.Pos);
                pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.Opened);

                // ****************************************** _needOpenedToObserve = +1;

                _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, pp.StrategyTickerString, "Position2",
                                  "New", pp.PositionString5, pp.ToString());
            }
           
           // *************************** 
            Positions.FireObserverEvent();
        }

        public static PosOperationEnum Reverse( PosOperationEnum operation)
        {
            return   operation == PosOperationEnum.Long
                         ? PosOperationEnum.Short
                         : operation == PosOperationEnum.Short
                            ? PosOperationEnum.Long
                            : PosOperationEnum.Neutral;
        }
        private void Reverse()
        {
            //Operation = Operation == Position.OperationEnum.Long ? Position.OperationEnum.Short : Position.OperationEnum.Long;

            Operation = Operation == PosOperationEnum.Long ? PosOperationEnum.Short : Operation == PosOperationEnum.Short ? PosOperationEnum.Long : PosOperationEnum.Neutral;
        }
        public void Clear()
        {
            FirstTradeDT = DateTime.MinValue;
            FirstTradeNumber = 0;
            LastTradeDT = DateTime.MinValue;
            LastTradeNumber = 0;

            Quantity = 0;

            Operation = PosOperationEnum.Neutral;
            Status = PosStatusEnum.Closed;

            Price1 = 0;
            Price2 = 0;
            Pnl = 0;

            Count = 0;

            Comment = "";
        }
        public void InvokePositionChangedEvent( long oldp, long newp)
        {
            if (PositionChangedEvent != null) PositionChangedEvent(oldp, newp);
        }
        public void InvokePositionChangedEvent(Position old, PositionChangedEnum posChangedResult)
        {
            if (PositionChangedEvent2 != null) PositionChangedEvent2(old, this, posChangedResult);
        }

        /*
        private void Reverse(Position.OperationEnum operation)
        {
            Operation = operation == Position.OperationEnum.Long
                            ? Position.OperationEnum.Short
                            : Position.OperationEnum.Long;
        }
         */ 
        protected int GetStrategyKeyHash()
        {
            return  (Account + Strategy + Ticker).GetHashCode();

        }
        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3:N0} {4:N2} {5:N2} {6:N2} {7} {8} {9} {10} {11:G} {12:G} {13}",
                                 Account, Strategy, TickerCode, Pos, Price1, Price2, Pnl, OperationString, StatusString, 
                                 FirstTradeNumber, LastTradeNumber, FirstTradeDT, LastTradeDT, PositionId);  
        }

        public object Clone()
        {
            var p = new Position
                        {
                            EventLog = EventLog,
                            Account = Account,
                            Strategy = Strategy,
                            TickerStr = TickerStr,
                            Ticker = Ticker,
                            FirstTradeNumber = FirstTradeNumber,
                            FirstTradeDT = FirstTradeDT,
                            LastTradeNumber = LastTradeNumber,
                            LastTradeDT = LastTradeDT,
                            Price1 = Price1,
                            Price2 = Price2,
                            Operation = Operation,
                            Quantity = Quantity,
                            Pnl = Pnl,
                            Count = Count,
                            Status = Status,
                            Comment = Comment,
                            Index = Index,
                            PositionTotal = PositionTotal
                        };
            return p;
        }

        public class PositionNotify : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public string Account { get; set; }
            public string Strategy { get; set; }
            public string Ticker { get; set; }
           // public ITicker Ticker { get; }

            public  long FirstTradeNumber { get; set; }
            public  DateTime FirstTradeDT { get; set; }

            public  long LastTradeNumber { get; set; }
            public DateTime LastTradeDT { get; set; }

            public decimal Price1 { get; set; }
            public decimal Price2 { get; set; }

            public decimal Amount1 { get { return Quantity * Price1; } }
            public decimal Amount2 { get { return Quantity * Price2; } }

            private PosStatusEnum _status;
            public PosStatusEnum Status
            {
                get { return _status; }
                set 
                { 
                _status = value;
                OnPropertyChanged(new PropertyChangedEventArgs("QuantityOperationString"));
                }
            }

            private PosOperationEnum _operation;
            public PosOperationEnum Operation
            {
                get { return _operation; }
                set 
                { 
                    _operation = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("QuantityOperationString"));
                }
            }

            private long _quantity;
            public long Quantity
            {
                get { return _quantity; }
                set 
                {
                    _quantity = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("QuantityOperationString"));
                }
            }

            public long Pos { get { return Quantity * (short)Operation; } }

            public string OperationString { get { return Operation.ToString().ToUpper(); } }
            public string QuantityOperationString { get { return Pos + " " + OperationString; } }

            public decimal Pnl { get; set; }

            public bool Opened { get { return Status == PosStatusEnum.Opened ? true : false; } }
            public bool Closed { get { return Status == PosStatusEnum.Closed ? true : false; } }

            public bool IsLong { get { return Operation == PosOperationEnum.Long ? true : false; } }
            public bool IsShort { get { return Operation == PosOperationEnum.Short ? true : false; } }

            public string StatusString { get { return Status.ToString().ToUpper(); } }
            public string Comment { get; set; }
            public long MyIndex { get; set; }

            public string FirstTimeDateString
            {
                get { return FirstTradeDT.ToString("T") + ' ' + FirstTradeDT.ToString("d"); }
            }
            public string LastTimeDateString
            {
                get { return LastTradeDT.ToString("T") + ' ' + LastTradeDT.ToString("d"); }
            }

            public string LastTradeDTString { get { return LastTradeDT.ToString("G"); } }
            public string FirstTradeDTString { get { return FirstTradeDT.ToString("G"); } }

            public string PositionString { get { return Pos.ToString("N0"); } }
            public string AmountString { get { return Amount1.ToString("N2"); } }

            public string AvgPriceString { get { return Price1.ToString("N2"); } }
            public string AvgPrice2String { get { return Price2.ToString("N2"); } }

            public string LastTradePriceString { get { return Price2.ToString("N2"); } }
            public string PnLString { get { return Pnl.ToString("N2"); } }

            public PositionNotify()
            {
            }
            public void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                if (PropertyChanged != null) PropertyChanged(this, e);
            }
        }
        // ILineXY Chart Interface
        public DateTime LineX1
        {
            get { return FirstTradeDT; }
        }
        public DateTime LineX2
        {
            get { return LastTradeDT; }
        }
        public double LineY1
        {
            get { return (double) Price1; }
        }
        public double LineY2
        {
            get { return (double) Price2; }
        }
        public int LineColor
        {
            get { return Pnl > 0 ? 0x0000ff : 0xff0000; }
        }

        public int LineWidth
        {
            get { throw new NotImplementedException(); }
        }

        public string Key { get; private set; }
    }
}