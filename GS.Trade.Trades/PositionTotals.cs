using System;
using System.ComponentModel;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace GS.Trade.Trades
{
    public partial class Positions
    {
        public Totals PositionTotals;

        public class Totals
        {
            // public delegate void GetPositionTotalsToObserve();
            // public GetPositionTotalsToObserve CallbackGetPositionTotalsToObserve;
            public event EventHandler<PositionsEventArgs> PositionTotalsEvent;

            public  Dictionary<string, PositionTotal> PositionTotalCollection;
            public ObservableCollection<PositionTotal> PositionTotalsObserveCollection;

            private short _needTotalsToObserve;

            private readonly object _lockPositionTotalCollection;

            public Totals()
            {
                PositionTotalCollection = new Dictionary<string, PositionTotal>();
                PositionTotalsObserveCollection = new ObservableCollection<PositionTotal>();

                _lockPositionTotalCollection = new object();
            }
            /*
            public Position GetTotalOrNew(Position p)
            {
                PositionTotal pos;
                if (PositionTotalCollection.TryGetValue(p.StrategyKey, out pos))
                {
                    return pos;
                }
                pos = new PositionTotal(p);
                Register(pos);
                return pos;
            }
             */ 
            public Position GetTotalOrNull(Position p)
            {
                PositionTotal pos;
                return PositionTotalCollection.TryGetValue(p.StrategyKey, out pos) ? pos : null;
            }
            public void Update(Position p)
            {             
                PositionTotal pos;

                if (PositionTotalCollection.TryGetValue(p.StrategyKey, out pos))
                {
                    pos.Quantity = p.Quantity;
                    //pos.Status = p.Status;

                    _needTotalsToObserve = 1;
                }
                else
                {
                    // Register(p);
                }
            }
            
            public void UpdateTotalOrNew(Position p)
            {              
                PositionTotal pos;
                if (PositionTotalCollection.TryGetValue(p.StrategyKey, out pos))
                {
                    pos.Update(p);
                    _needTotalsToObserve = +1;
                }
                else
                {
                    throw new NullReferenceException("PositionTotal Not Found");
                   // pos = new PositionTotal(p);
                   // Register(pos);
                }
            }
              
            public PositionTotal Register(Position p)
            {
                if(p == null) return null;
                PositionTotal post;
                if (PositionTotalCollection.TryGetValue(p.StrategyKey, out post))
                {
                    return post;
                }
                var realP = p as PositionNpc;
                if (realP == null)
                    post = new PositionTotal(p);
                else
                {
                    post = new PositionTotalNpc
                               {
                                   Account = p.Account,
                                   Strategy = p.Strategy,
                                   Ticker = p.Ticker,
                                   TickerStr = (p.Ticker != null) ? p.Ticker.Code : p.TickerStr,

                                   Operation = p.Operation,
                                   Quantity = p.Quantity,
                                   Pnl = 0,
                                   PnL2 = 0,
                                   FirstTradeNumber = p.FirstTradeNumber,
                                   FirstTradeDT = p.FirstTradeDT,
                                   LastTradeNumber = p.LastTradeNumber,
                                   LastTradeDT = p.LastTradeDT,

                                   Price1 = p.IsLong ? p.Price1 : (p.IsShort ? p.Price2 : 0),
                                   Price2 = p.IsLong ? p.Price2 : (p.IsShort ? p.Price1 : 0),

                                   Status = PosStatusEnum.Closed,
                                   StrategyKeyHash = p.StrategyKeyHash
                               };
                    ((PositionTotalNpc)post).PropertyChanged += ((PositionNpc) p).PositionTotalEventHandler;
                }
                lock (_lockPositionTotalCollection)
                {
                    PositionTotalCollection.Add(post.StrategyKey, post);
                }
                RisePositionsEvent("NewPositionTotal", post);
                _needTotalsToObserve = 1;
                return post;
            }
            private void RisePositionsEvent(string whats, Position p)
            {
                if (PositionTotalsEvent != null)
                    PositionTotalsEvent(this, new PositionsEventArgs(whats, p));
            }
            /*
            public void GetAllTotals()
            {
                lock (_lockPositionTotalCollection)
                {
                    foreach (var p in PositionTotalCollection.Values)
                        RisePositionsEvent("NewPositionTotal", p);
                }
            }
             */ 
            public void GetAllTotals(IList<PositionTotal> lst)
            {
                lock (_lockPositionTotalCollection)
                {
                    foreach (var p in PositionTotalCollection.Values)
                        lst.Add(p);
                }
            }

            private void Register(PositionTotal p)
            {
                lock (_lockPositionTotalCollection )
                {
                    PositionTotalCollection.Add(p.StrategyKey, p);

                    _needTotalsToObserve = 1;
                }
            }
            public void GetPositionTotalsToObserve()
            {
                if (_needTotalsToObserve <= 0) return;

                lock (_lockPositionTotalCollection)
                {
                    PositionTotalsObserveCollection.Clear();
                  //  var valueCollection = PositionTotalCollection.Values;
                    foreach (var p in PositionTotalCollection.Values)
                    {
                        PositionTotalsObserveCollection.Add(p); 
                    }
                    _needTotalsToObserve = 0;
                }
            }
            public class PositionTotal : Position, IPositionTotal
            {
                public string TotalPnLString { get { return CurrencyPnL.ToString(TickerFormatM); } }
                public string TotalPointPnLString { get { return PointPnL.ToString(TickerFormatM); } }

                public decimal PointPnL { get {return (Price2 - Price1) * Quantity; }}
                public decimal CurrencyPnL { get { return (Price2 - Price1) * Quantity; } }

                public string RealPoints { get { return ((Price2 - Price1) * Quantity).ToString(TickerFormatAvg); } }
                public string RealPnL { get { return ((Price2 - Price1) * Quantity).ToString(TickerFormatM); } }

                
                public PositionTotal()
                {
                    Status = PosStatusEnum.Closed;
                }
                 
                public PositionTotal(Position p)
                {
                    Account = p.Account;
                    Strategy = p.Strategy;
                    Ticker = p.Ticker;
                    TickerStr = (Ticker != null) ? Ticker.Code : p.TickerStr;

                    Operation = p.Operation;
                    Quantity = p.Quantity;
                    //Amount = amount;
                    Pnl = 0;
                    FirstTradeNumber = p.FirstTradeNumber;
                    FirstTradeDT = p.FirstTradeDT;
                    // CloseTradeNumber = maxnumber; DT_Close = maxdt;
                    LastTradeNumber = p.LastTradeNumber;
                    LastTradeDT = p.LastTradeDT;
                    //LastTradePrice = t.Price;
                    
                    if (p.IsLong)
                    {
                        Price1 = p.Price1;
                        Price2 = p.Price2;
                    }
                    else if ( p.IsShort )
                    {
                        Price1 = p.Price2;
                        Price2 = p.Price1;
                    }

                    //Count = p.Quantity;
                    //OpenCloseFlag = 1;

                    Status = PosStatusEnum.Closed;
                    StrategyKeyHash = p.StrategyKeyHash;
                }
                
                public void Update(IPosition p)
                {

                    if (p.IsLong)
                    {
                        Price1 = (Price1 * Quantity + p.Price1 * p.Quantity)/(Quantity + p.Quantity);
                        Price2 = (Price2 * Quantity + p.Price2 * p.Quantity) / (Quantity + p.Quantity);
                    }
                    else if( p.IsShort)
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
                    PnL2 += (p.Price2 - p.Price1)* p.Pos;

                    LastTradeDT = p.LastTradeDT;
                    LastTradeNumber = p.LastTradeNumber;
                    
                    // Price2 = p.Price2;
                }
            } 
            
        
            #region PositionTotal Old
            /*
            public class PositionTotal
            {
                public int StrategyKey { get; set; }

                private static long _positionIdRoot;
                public long PositionId { get; set; }

                public string Account { get; set; }
                public string Strategy { get; set; }
                public string Ticker { get; set; }

                public long FirstTradeNumber { get; set; }
                public DateTime FirstTradeDT { get; set; }

                //  public long CloseTradeNumber { get; set; }
                //  public DateTime DT_Close { get; set; }

                //public long MaxTradeNumber { get; set; }

                public long LastTradeNumber { get; set; }
                public DateTime LastTradeDT { get; set; }
                public decimal LastTradePrice { get; set; }

                public decimal AvgPrice1 { get; set; }
                public decimal AvgPrice2 { get; set; }

                public decimal Amount1 { get { return Quantity1 * AvgPrice1; } }
                public decimal Amount2 { get { return Quantity2 * AvgPrice2; } }

                //public Position.OperationEnum Operation { get; set; }

                public Position.OperationEnum Operation { get { return (Position.OperationEnum)Math.Sign(Pos); } }

                public long Quantity1 { get; set; }
                public long Quantity2 { get; set; }

                public long Pos { get { return (Quantity1 - Quantity2); } }
                public string OperationString { get { return Operation.ToString().ToUpper(); } }

                public decimal Points1 { get; set; }
                public decimal Points2 { get; set; }

                public decimal Pnl1 { get; set; }
                public decimal Pnl2 { get; set; }

                public long Count { get; set; }
                //public Position.StatusEnum Status { get; set; }

                public Position.StatusEnum Status { get { return Pos == 0 ? Position.StatusEnum.Closed : Position.StatusEnum.Opened; } }

                public bool Opened { get { return Status == Position.StatusEnum.Opened ? true : false; } }
                public bool Closed { get { return Status == Position.StatusEnum.Closed ? true : false; } }

                public string StatusString { get { return Status.ToString().ToUpper(); } }
                public string Comment { get; set; }
                public long MyIndex { get; set; }

                public string LastTradeDTString { get { return LastTradeDT.ToString("G"); } }
                public string FirstTradeDTString { get { return FirstTradeDT.ToString("G"); } }

                public string PositionString { get { return Pos.ToString("N0"); } }

                public string Amount1String { get { return Amount1.ToString("N2"); } }
                public string Amount2String { get { return Amount2.ToString("N2"); } }

                public string AvgPrice1String { get { return AvgPrice1.ToString("N2"); } }
                public string AvgPrice2String { get { return AvgPrice2.ToString("N2"); } }

                public string LastTradePriceString { get { return LastTradePrice.ToString("N2"); } }

                public string PnL1String { get { return Pnl1.ToString("N2"); } }
                public string PnL2String { get { return Pnl2.ToString("N2"); } }


                public PositionTotal() { }
                public PositionTotal(string account, string strategy, string ticker, Position.OperationEnum operation, long quantity,
                    decimal firsttradeprice, decimal lasttradeprice, decimal pnl, Position.StatusEnum status,
                    DateTime mindt, long minnumber, DateTime maxdt, long maxnumber)
                {
                    Account = account; Strategy = strategy; Ticker = ticker;

                    //Operation = operation;
                    Quantity1 = quantity;
                    //Amount = amount;
                    Pnl1 = pnl;
                    FirstTradeNumber = minnumber;
                    FirstTradeDT = mindt;
                    // CloseTradeNumber = maxnumber; DT_Close = maxdt;
                    LastTradeNumber = maxnumber;
                    LastTradeDT = maxdt;
                    LastTradePrice = lasttradeprice;
                    AvgPrice1 = firsttradeprice;
                    Count = quantity;
                    //OpenCloseFlag = 1;

                    //Status = status;

                    SetStrategyKey();

                    lock (this)
                    {
                        _positionIdRoot++;
                        PositionId = _positionIdRoot;
                    }

                    // string s = "\"";
                }
                public PositionTotal(Trade t)
                {
                    Account = t.Account; Strategy = t.Strategy; Ticker = t.Ticker;

                    //Operation = (Position.OperationEnum)t.Operation;
                    Quantity1 = t.Quantity;
                    //Amount = amount;
                    Pnl1 = 0;
                    FirstTradeNumber = t.Number;
                    FirstTradeDT = t.DT;
                    // CloseTradeNumber = maxnumber; DT_Close = maxdt;
                    LastTradeNumber = FirstTradeNumber;
                    LastTradeDT = FirstTradeDT;
                    LastTradePrice = t.Price;
                    AvgPrice1 = LastTradePrice;
                    Count = t.Quantity;
                    //OpenCloseFlag = 1;

                    //Status = Position.StatusEnum.Opened;

                    StrategyKey = t.StrategyKey;

                    lock (this)
                    {
                        _positionIdRoot++;
                        PositionId = _positionIdRoot;
                    }

                }

                public static Position.OperationEnum Reverse(Position.OperationEnum operation)
                {
                    return operation == Position.OperationEnum.Long
                                    ? Position.OperationEnum.Short
                                    : Position.OperationEnum.Long;
                }
             */
                /*
                private void Reverse()
                {
                    Operation = Operation == Position.OperationEnum.Long
                                    ? Position.OperationEnum.Short
                                    : Position.OperationEnum.Long;
                }
                 */
            /*
            private void Reverse(Position.OperationEnum operation)
            {
                Operation = operation == Position.OperationEnum.Long
                                ? Position.OperationEnum.Short
                                : Position.OperationEnum.Long;
            }
                
            private void SetStrategyKey()
            {
                StrategyKey = (Account + Strategy + Ticker).GetHashCode();
            }
            public override string ToString()
            {
                return String.Format("{0} {1} {2} {3:N0} {4:N2} {5:N2} {6:N2} {7} {8} {9:G} {10:G} {11}",
                                        Account, Strategy, Ticker, Pos, AvgPrice1, LastTradePrice, Pnl1,
                                        FirstTradeNumber, LastTradeNumber, FirstTradeDT, LastTradeDT, PositionId);
            }
        }
        */
            #endregion

        }
        public class PositionTotalNpc : Totals.PositionTotal, INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                if (PropertyChanged != null) PropertyChanged(this, e);
            }
            private ulong _firstTradeNumber;
            public override ulong FirstTradeNumber
            {
                get { return _firstTradeNumber; }
                set
                {
                    _firstTradeNumber = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("FirstTradeNumber"));
                }
            }

            private DateTime _firstTradeDT;
            public override DateTime FirstTradeDT
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
            public override ulong LastTradeNumber
            {
                get { return _lastTradeNumber; }
                set
                {
                    _lastTradeNumber = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("LastTradeNumber"));
                }
            }

            private DateTime _lastTradeDT;
            public override DateTime LastTradeDT
            {
                get { return _lastTradeDT; }
                set
                {
                    _lastTradeDT = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("LastTradeDTString"));
                }
            }

            private decimal _price1;
            public override decimal Price1
            {
                get { return _price1; }
                set
                {
                    _price1 = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("AvgPriceString"));
                    OnPropertyChanged(new PropertyChangedEventArgs("TotalPnLString"));
                }
            }

            private decimal _price2;
            public override decimal Price2
            {
                get { return _price2; }
                set
                {
                    _price2 = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("AvgPrice2String"));
                    OnPropertyChanged(new PropertyChangedEventArgs("LastTradePriceString"));
                    OnPropertyChanged(new PropertyChangedEventArgs("TotalPnLString"));
                }
            }
            private long _quantity;
            public override long Quantity
            {
                get { return _quantity; }
                set
                {
                    _quantity = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Quantity"));
                    OnPropertyChanged(new PropertyChangedEventArgs("AmountString"));
                    OnPropertyChanged(new PropertyChangedEventArgs("TotalPnLString"));
                }
            }
            private decimal _pnl2;
            public override decimal PnL2
            {
                get { return _pnl2; }
                set
                {
                    _pnl2 = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("PnL2String"));
                }
            }
        }
    }
}