using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.Trades
{
    public class PositionTotals2
    {
        // public delegate void GetPositionTotalsToObserve();
        // public GetPositionTotalsToObserve CallbackGetPositionTotalsToObserve;
        public event EventHandler<PositionsEventArgs> PositionTotalsEvent;

        public Dictionary<string, PositionTotal2> PositionTotalCollection;
        //public ObservableCollection<PositionTotal2> PositionTotalsObserveCollection;

        private short _needTotalsToObserve;

        private readonly object _lockPositionTotalCollection;

        public PositionTotals2()
        {
            PositionTotalCollection = new Dictionary<string, PositionTotal2>();
            //PositionTotalsObserveCollection = new ObservableCollection<PositionTotal2>();

            _lockPositionTotalCollection = new object();
        }
      
        public Position2 GetTotalOrNull(Position p)
        {
            PositionTotal2 pos;
            return PositionTotalCollection.TryGetValue(p.StrategyKey, out pos) ? pos : null;
        }
        public void Update(Position p)
        {
            PositionTotal2 pos;

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

        public void UpdateTotalOrNew(Position2 p)
        {
            PositionTotal2 pos;
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

        //public PositionTotal2 Register(Position2 p)
        //{
        //    if (p == null) return null;
        //    PositionTotal2 post;
        //    if (PositionTotalCollection.TryGetValue(p.StrategyKey, out post))
        //    {
        //        return post;
        //    }
        //    var realP = p as PositionNpc2;
        //    if (realP == null)
        //        post = new PositionTotal2(p);
        //    else
        //    {
        //        post = new PositionNpc2
        //        {
        //            Account = p.Account,
        //            Strategy = p.Strategy,
        //            Ticker = p.Ticker,

        //            TickerStr = (p.Ticker != null) ? p.Ticker.Code : p.TickerStr,

        //            Operation = p.Operation,
        //            Quantity = p.Quantity,
        //            Pnl = 0,
        //            PnL2 = 0,
        //            FirstTradeNumber = p.FirstTradeNumber,
        //            FirstTradeDT = p.FirstTradeDT,
        //            LastTradeNumber = p.LastTradeNumber,
        //            LastTradeDT = p.LastTradeDT,

        //            Price1 = p.IsLong ? p.Price1 : (p.IsShort ? p.Price2 : 0),
        //            Price2 = p.IsLong ? p.Price2 : (p.IsShort ? p.Price1 : 0),

        //            Status = PosStatusEnum.Closed,
        //            StrategyKeyHash = p.StrategyKeyHash
        //        };
        //        ((PositionTotalNpc)post).PropertyChanged += ((PositionNpc)p).PositionTotalEventHandler;
        //    }
        //    lock (_lockPositionTotalCollection)
        //    {
        //        PositionTotalCollection.Add(post.StrategyKey, post);
        //    }
        //    RisePositionsEvent("NewPositionTotal", post);
        //    _needTotalsToObserve = 1;
        //    return post;
        //}

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
        public void GetAllTotals(IList<PositionTotal2> lst)
        {
            lock (_lockPositionTotalCollection)
            {
                foreach (var p in PositionTotalCollection.Values)
                    lst.Add(p);
            }
        }

        private void Register(PositionTotal2 p)
        {
            lock (_lockPositionTotalCollection)
            {
                PositionTotalCollection.Add(p.StrategyKey, p);

                _needTotalsToObserve = 1;
            }
        }
        //public void GetPositionTotalsToObserve()
        //{
        //    if (_needTotalsToObserve <= 0) return;

        //    lock (_lockPositionTotalCollection)
        //    {
        //        PositionTotalsObserveCollection.Clear();
        //        //  var valueCollection = PositionTotalCollection.Values;
        //        foreach (var p in PositionTotalCollection.Values)
        //        {
        //            PositionTotalsObserveCollection.Add(p);
        //        }
        //        _needTotalsToObserve = 0;
        //    }
        //}
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
}
