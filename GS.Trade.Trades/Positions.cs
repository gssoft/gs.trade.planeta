using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using GS.Interfaces;
//using GS.Trade.Data;
using GS.Trade;

namespace GS.Trade.Trades
{
    public delegate void GetPositionsClosedToObserve();
    public delegate void GetPositionsOpenedToObserve();

    [System.Xml.Serialization.XmlRootAttribute()]
    public partial class Positions : IPositions
    {
        [XmlIgnore]
        IEventLog _eventLog;
        // public ITradeContext TradeContext { get; set; }

        public event EventHandler<IPositionsEventArgs> PositionsEvent;

        public delegate void NeedToOserverEventHandler();
        public event NeedToOserverEventHandler NeedToObserverEvent;
        public event EventHandler<IPosition> NewPositionClosed; 

        private long _lastObserveGetRequestNumber;

        public GetPositionsClosedToObserve CallbackGetPositionsClosedToObserve;
        public GetPositionsOpenedToObserve CallbackGetPositionsOpenedToObserve;

        public List<Position> PositionCollection;
        public List<Position> PositionCurrentCollection;
        // public ObservableCollection<Position> PositionCurrentCollection;
        public List<Position> PositionOpenedCollection;
        public List<Position> PositionClosedCollection;

        public ObservableCollection<Position> PositionObserveCollection;
        public ObservableCollection<Position> PositionOpenedObserveCollection;
        public ObservableCollection<Position> PositionClosedObserveCollection;

        private long _maxIndex;
        private volatile int _needOpenedToObserve;
        private volatile int _needClosedToObserve;

        private object _lockPosition;
        private object _lockOpenPosition;
        private object _lockCurrentPosition;

        private int _positionCurrentCount;
        private int _positionClosedCount;
        //private int _positionCurrentObserveCount;

        public Positions()
        {
            PositionCollection = new List<Position>();
            PositionCurrentCollection = new List<Position>();
            // PositionCurrentCollection = new ObservableCollection<Position>();
            PositionOpenedCollection = new List<Position>();
            PositionClosedCollection = new List<Position>();

            PositionObserveCollection = new ObservableCollection<Position>();
            PositionOpenedObserveCollection = new ObservableCollection<Position>();
            PositionClosedObserveCollection = new ObservableCollection<Position>();

            PositionTotals = new Totals();

            _maxIndex = 0;

            _lockPosition = new object();
            _lockOpenPosition = new object();
            _lockCurrentPosition = new object();
        }
        public void Init(IEventLog evl)
        {
            _eventLog = evl;
            _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "Positions", "Positions", "Initialization", "", "");
        }
        public int PositionCount { get { return PositionCollection.Count; } }

        public void AddPosition(Position p)
        {
            lock(_lockPosition)
            {
                p.Index = ++_maxIndex;
                PositionCollection.Add(p);

                _positionClosedCount = PositionCollection.Count;

                _needOpenedToObserve = +1;
                _needClosedToObserve = +1;
            }
            if (NewPositionClosed != null)
                NewPositionClosed(this, p);

            _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, p.StrategyTickerString, 
                                "Position", "New Closed", p.PositionString6 , p.ToString());
        }
        private void AddCurrentPosition(Position p)
        {
            lock (_lockCurrentPosition)
            {
                p.Index = ++_maxIndex;
                PositionCurrentCollection.Add(p);

                _positionCurrentCount = PositionCurrentCollection.Count;

                _needOpenedToObserve = +1;
                _needClosedToObserve = +1;
            }
            RisePositionsEvent("NewCurrent", p);

            _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, 
                        p.StrategyTickerString, "Position","New Current", p.PositionString3, p.ToString());
        }
        private void RisePositionsEvent(string whats, Position p)
        {
            if(PositionsEvent != null)
                PositionsEvent(this, new PositionsEventArgs(whats, p));
        }
        /*
        public void GetAllCurrents()
        {
            lock (_lockCurrentPosition)
            {
                foreach(var p in PositionCurrentCollection)
                    RisePositionsEvent("NewCurrent",p);
            }
        }
        */
        public void GetAllCurrents(List<IPosition> lst)
        {
            lock (_lockCurrentPosition)
            {
                foreach (var p in PositionCurrentCollection)
                    lst.Add(p);
            }
        }

        public IPosition Register(string account, string strategy, ITicker ticker )
        {
            if (ticker == null) return null;
            Position po;

            lock (_lockCurrentPosition)
            {
                po = (from p in PositionCurrentCollection
                          where p.Account == account && p.Strategy == strategy && p.TickerStr == ticker.Code
                          //  && (p.IsOpened || p.Operation == Position.OperationEnum.Neutral)
                          select p).FirstOrDefault();
            }

            if (po != null) return po;
            /*
            po = new Position
            {
                    Account = account,
                    Strategy = strategy,
                    Ticker = ticker,
                    TickerStr = ticker.Code,
                    Operation = Position.OperationEnum.Neutral,
                    Status = Position.StatusEnum.Closed,
                    //Index = ++ _maxIndex
            };
             */
            po = new PositionNpc
            {
                Account = account,
                Strategy = strategy,
                Ticker = ticker,
                TickerStr = ticker.Code,
                Operation = PosOperationEnum.Neutral,
                Status = PosStatusEnum.Closed,
                //Index = ++ _maxIndex
            };
            po.Clear();
            AddCurrentPosition(po);
            //_eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Positions", "Register New Current","", po.ToString());
            return po;
        }
        public IPosition Register2(string account, string strategy, ITicker ticker)
        {
            if (ticker == null) return null;
            Position po;

            lock (_lockCurrentPosition)
            {
                po = (from p in PositionCurrentCollection
                      where p.Account == account && p.Strategy == strategy && p.TickerStr == ticker.Code
                      //  && (p.IsOpened || p.Operation == Position.OperationEnum.Neutral)
                      select p).FirstOrDefault();
            }

            if (po != null) //&& po.PositionTotal != null)
            {
                po.Positions = this;
                return po;
            }
            //else
            //    throw new NullReferenceException("Position Register Failure ");

            po = new PositionNpc
            {
                Positions = this,
                Account = account,
                Strategy = strategy,
                Ticker = ticker,
                TickerStr = ticker.Code,
                Operation = PosOperationEnum.Neutral,
                Status = PosStatusEnum.Closed,
                //Index = ++ _maxIndex
            };
            po.Clear();
            
            //_eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Positions", "Register New Current","", po.ToString());
            var pt = PositionTotals.Register(po);
            if (pt == null)
                throw new NullReferenceException("PositionTotal - Invalid Registration");
            po.PositionTotal = pt;

            AddCurrentPosition(po);
            return po;
        }

        public void Calculate(ObservableCollection<Trade> Trades, int i)
        {
            int j; j = i;
            List<string> lStrat = new List<string>();
            lStrat.Clear();
            foreach (Trade t in Trades)
            {               
               lStrat.Add(t.Account + t.Strategy + t.Ticker);
            }
            IEnumerable<string> astrat = lStrat.Distinct();
            PositionCollection.Clear();
            foreach (string s in astrat)
            {
                long sum = Trades.Where(t => s == t.Account + t.Strategy + t.Ticker).Sum(t => t.Position);
                //PositionCollection.Add(new Position(s, sum));
                
                if( _eventLog != null )
                _eventLog.AddItem(EvlResult.SUCCESS, "Create Position", s  + " " + sum);
            }
        }
        public void Calculate(List<Trade> Trades)
        {
           PositionCollection.Clear();
           var AccStraTicker = 
           (from s in
                from t in Trades
                group t by t.Account + t.Strategy + t.Ticker into s
                select new
                {
                    // Key = s.Key,
                    Account = (from r1 in s select r1.Account).First(),
                    Strategy = (from r1 in s select r1.Strategy).First(),
                    Ticker = (from r1 in s select r1.Ticker).First(),
                    LastTradePrice = (from r1 in s select r1.Price).Last(),
                    Pos = (from r1 in s select r1.Position).Sum(),
                    Pnl = (from r1 in s select r1.Position * r1.Price).Sum(),
                    MaxNumber = (from r1 in s select r1.Number).Max(),
                    MinNumber = (from r1 in s select r1.Number).Min(),
                    MaxDate = (from r1 in s select r1.DT).Max(),
                    MinDate = (from r1 in s select r1.DT).Min(),
                    Count = (from r1 in s select r1.DT).Count()
                }
            orderby s.Account, s.Strategy, s.Ticker
            select s);

            foreach (var str in AccStraTicker)
            {
                try
                {
                    decimal pnl = 0;
                    if (str.Pos != 0)
                        pnl = (-1) * (str.Pnl - str.Pos * str.LastTradePrice);
                    else
                        pnl = (-1) * str.Pnl;

                    var p = new Position(str.Account, str.Strategy, str.Ticker, 
                        (PosOperationEnum)Math.Sign(str.Pos), Math.Abs(str.Pos), 
                        str.LastTradePrice, str.LastTradePrice,
                        pnl, str.Pos == 0 ? PosStatusEnum.Closed : PosStatusEnum.Opened,
                        str.MinDate, str.MinNumber, str.MaxDate, str.MaxNumber);

                    PositionCollection.Add(p);
                    //if (_EventLog != null)
                    //    _EventLog.Add(new EventLogItem(EvlResult.SUCCESS, "Create Position", p.ToString()));
                }
                catch
                {
                    if (_eventLog != null)
                        _eventLog.AddItem(EvlResult.FATAL, "Create Position", "Create Position Failure");
                }
            }
            if (PositionCollection.Count > 0)
            {
                if (_eventLog != null)
                    _eventLog.AddItem(EvlResult.SUCCESS, "Create Position", 
                        "Create Position complete. Position count = " + PositionCollection.Count);
            }
            else
                if (_eventLog != null)
                    _eventLog.AddItem(EvlResult.WARNING, "Create Position", "Try toCreate Position but Position Count = 0");
 
        }
        public void PositionCalculate(Trade t)
        {
            lock (this)
            {
                var pp = PositionOpenedCollection.FirstOrDefault(
                    p => p.Opened && t.Account == p.Account && t.Strategy == p.Strategy && t.Ticker == p.TickerStr);
                if ((pp != null))
                {
                    pp.LastTradeDT = t.DT;
                    pp.LastTradeNumber = t.Number;
                    pp.Price2 = t.Price;

                    if ((short)pp.Operation == (short)t.Operation)
                    {
                        // PositionOpenedCollection.Remove(pp);

                        pp.Price1 = (pp.Price1*pp.Quantity + t.Price*t.Quantity)/(pp.Quantity + t.Quantity);
                        pp.Quantity += t.Quantity;
                        pp.Count += t.Quantity;
                        pp.Comment += String.Format("Rise.{0} {1:C} {2} {3} {4}. ", pp.Pos, pp.Price1, t.Number,
                                                    t.Position, t.Price);

                        PositionOpenedCollection.Add(pp);

                        _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                                       "Position", "Rise " + pp.OperationString, pp.ToString(),
                                                       t.ToString());
                    }
                    else
                    {
                        // Something Should be Close   Close  3 -3  ReSize 3 -1  Reverse 3 -5
                        //pp.Quantity = 0;                

                        if (pp.Pos + t.Position == 0)
                        {
                            //   Delete(pp, PositionOpenedCollection);

                         //   pp.Pnl = (t.Price - pp.Price1)*pp.Pos;
                            pp.Price2 = t.Price;
                            pp.Count *= 2;
                            pp.Status = PosStatusEnum.Closed;
                            pp.LastTradeDT = t.DT;
                            pp.LastTradeNumber = t.Number;
                            pp.Comment += String.Format("Close {0} {1:C} {2} {3} {4}. ", pp.Pos, pp.Price1, t.Number,
                                                        t.Position, t.Price);

                            PositionClosedCollection.Add(pp); //Insert(0, pp);

                            _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                                           "Position", "Close " + pp.OperationString, pp.ToString(),
                                                           t.ToString());
                        }
                        else //(pp.Pos + t.Position != 0)
                        {
                            if (pp.Quantity > t.Quantity) // Reduce
                            {
                                // Open new "Open Position with Old LastTradeNumber
                                var p = new Position(t.Account, t.Strategy, t.Ticker, pp.Operation,
                                                     pp.Quantity - t.Quantity,
                                                     pp.Price1, pp.Price1, 0, PosStatusEnum.Opened, 
                                                     pp.FirstTradeDT, pp.FirstTradeNumber, pp.LastTradeDT,
                                                     pp.LastTradeNumber);

                                p.Comment += String.Format("Open.Reduce {0} {1:C} {2} {3} {4}. ", pp.Pos, pp.Price2,
                                                           t.Number, t.Position, t.Price);
                                PositionOpenedCollection.Add(p);

                               // Delete(pp, PositionOpenedCollection);

                                pp.Quantity = t.Quantity;
                                pp.Pnl = (t.Price - pp.Price1)*pp.Pos;
                                pp.Count += t.Quantity;
                                pp.Status = PosStatusEnum.Closed;
                                pp.LastTradeDT = t.DT;
                                pp.LastTradeNumber = t.Number;
                                pp.Comment += String.Format("Close.Reduce {0} {1:C} {2} {3} {4}. ", pp.Pos, pp.Price1,
                                                            t.Number, t.Position, t.Price);

                                PositionClosedCollection.Add(pp); // Insert(0, pp);

                                _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                                               "Position", "Reduce.Close " + pp.OperationString,
                                                               pp.ToString(), t.ToString());

                                _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                                               "Position", "Reduce.Open " + p.OperationString,
                                                               p.ToString(), t.ToString());
                            }
                            else // Reverse
                            {
                                //  Delete(pp, PositionOpenedCollection);

                                pp.Pnl = (t.Price - pp.Price1)*pp.Pos;
                                pp.Count += pp.Quantity;
                                pp.Status = PosStatusEnum.Closed;
                                pp.LastTradeDT = t.DT;
                                pp.LastTradeNumber = t.Number;
                                pp.Comment += String.Format("Close.Reverse {0} {1:C} {2} {3}. ", pp.Pos, pp.Price1,
                                                            t.Number, t.Price);
                                PositionClosedCollection.Add(pp); //  Insert(0, pp);

                                var p = new Position(t.Account, t.Strategy, t.Ticker, (PosOperationEnum)Math.Sign(pp.Pos*(-1)),
                                                     t.Quantity - pp.Quantity,
                                                     t.Price, t.Price, 0, PosStatusEnum.Opened,
                                                     t.DT, t.Number, t.DT, t.Number);
                                p.Comment += String.Format("Open.Reverse {0} {1:C} {2} {3} {4}. ", pp.Pos, pp.Price1,
                                                           t.Number, t.Position, t.Price);
                                PositionOpenedCollection.Add(p);

                                _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                                               "Position", "Reverse. Close " + pp.OperationString,
                                                               pp.ToString(), t.ToString());
                                _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                                               "Position", "Reverse. Open " + p.OperationString,
                                                               p.ToString(), t.ToString());
                            }
                        }
                    }
                }
                else
                {
                    /*
                        if (t.Quantity == 0 || t.Operation == 0)
                        {
                            _EventLog.Add(new EventLogItem(EvlResult.FATAL, "New Position",
                                                           String.Format("{0} {1} {2} {3} {4}",
                                                                          t.Number, t.DT, t.Operation, t.Quantity, t.TradePrice)));
                        }
                    */
                    var p1 = new Position(t.Account, t.Strategy, t.Ticker, (PosOperationEnum)t.Operation, t.Quantity, t.Price, t.Price, 0, 
                                                PosStatusEnum.Opened, t.DT, t.Number, t.DT, t.Number);

                    p1.Comment += String.Format("Open {0} {1} {2}. ", p1.Pos, t.Number, t.Price);

                    PositionOpenedCollection.Add(p1);
                    _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position", "Position",
                                                   "Open " + p1.OperationString, p1.ToString(), t.ToString());
                }
            }
        }
        public void PositionCalculate2(Trade t)
        {
           lock ( _lockPosition )
            {
                //_needToObserve = +1;

                var pp = PositionCollection.FirstOrDefault( p => p.Opened && p.StrategyKey == t.StrategyKey);
                if ((pp != null))
                {
                    lock (pp)
                    {
                        var posPosition = pp.Pos;
                        var posQuantity = pp.Quantity;
                        var posOperation = pp.Operation;

                        pp.LastTradeDT = t.DT;
                        pp.LastTradeNumber = t.Number;
                        pp.Price2 = t.Price;

                        if ((short)posOperation == (short)t.Operation)  // Rise Position Operation the Same example: Buy & Buy
                        {
                            // PositionOpenedCollection.Remove(pp);

                            pp.Price1 = (pp.Price1*pp.Quantity + t.Price*t.Quantity)/(pp.Quantity + t.Quantity);
                            pp.Quantity += t.Quantity;
                            //pp.Count += t.Quantity;
                            pp.Comment += String.Format("Rise.{0} {1:C} {2} {3} {4}. ", pp.Pos, pp.Price1, t.Number,
                                                        t.Position, t.Price);

                            // PositionCollection.Add(pp);

                            _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                                           "Position", "Rise " + pp.OperationString, pp.ToString(), t.ToString());                          
                        }
                        else
                        {
                            // Close Flag Shoud Be S E E E E T
                            // Something Should be Close   Close  3 -3  ReSize 3 -1  Reverse 3 -5
                            //pp.Quantity = 0;                

                            _needClosedToObserve = +1;
                            _needOpenedToObserve = +1;

                            if (posPosition + t.Position == 0)   // ONLY Only Close
                            {
                                //   Delete(pp, PositionOpenedCollection);

                                _needClosedToObserve = +1;
                                _needOpenedToObserve = +1;

                                pp.Status = PosStatusEnum.Closed;
                                pp.Pnl = (t.Price - pp.Price1)*pp.Pos;
                                pp.Price2 = t.Price; // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!11
                                pp.LastTradeDT = t.DT;
                                pp.LastTradeNumber = t.Number;
                                pp.Comment += String.Format("Close {0} {1:C} {2} {3} {4}. ", pp.Pos, pp.Price1,
                                                            t.Number,
                                                            t.Position, t.Price);
                                //pp.Index = ++_maxIndex;
                                // ********************************* Total ***************************
                                PositionTotals.UpdateTotalOrNew(pp);

                                // PositionClosedCollection.Add(pp); //Insert(0, pp);

                                _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                                               "Position", "Close " + pp.OperationString, pp.ToString(),
                                                               t.ToString());
                            }
                            else        //(pp.Pos + t.Position != 0)  Reduce  or Reverse Position 
                            {
                                if (posQuantity > t.Quantity) // Reduce
                                {
                                    // Open new "Open Position with Old LastTradeNumber
                                    var p = new Position(t.Account, t.Strategy, t.Ticker, posOperation,
                                                         pp.Quantity - t.Quantity,
                                                         pp.Price1, t.Price, 0, PosStatusEnum.Opened, 
                                                         pp.FirstTradeDT, pp.FirstTradeNumber, pp.LastTradeDT,
                                                         pp.LastTradeNumber);

                                    p.Comment += String.Format("Open.Reduce {0} {1:C} {2} {3} {4}. ", pp.Pos,
                                                               pp.Price1,
                                                               t.Number, t.Position, t.Price);
                                    //p.Index = ++_maxIndex;

                                    //PositionCollection.Add(p);
                                    AddPosition(p);

                                    // Delete(pp, PositionOpenedCollection);

                                    pp.Status = PosStatusEnum.Closed;
                                    pp.Quantity = t.Quantity;
                                    pp.Price2 = t.Price; // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
                                    pp.Pnl = (t.Price - pp.Price1)*pp.Pos;

                                    pp.LastTradeDT = t.DT;
                                    pp.LastTradeNumber = t.Number;
                                    pp.Comment += String.Format("Close.Reduce {0} {1:C} {2} {3} {4}. ", pp.Pos,
                                                                pp.Price1,
                                                                t.Number, t.Position, t.Price);
                                 //   pp.Index = ++_maxIndex;
                     // ************************************** Total *****************************
                                    PositionTotals.UpdateTotalOrNew(pp);

                                    // PositionClosedCollection.Add(pp); // Insert(0, pp);

                                    _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                                                   "Position", "Reduce.Close " + pp.OperationString,
                                                                   pp.ToString(), t.ToString());
                                    _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                                                   "Position", "Reduce.Open " + p.OperationString,
                                                                   p.ToString(), t.ToString());
                                }
                                else if (posQuantity < t.Quantity) // REVERSE 
                                {
                                    //  Delete(pp, PositionOpenedCollection);

                                    pp.Status = PosStatusEnum.Closed;
                                    pp.Pnl = (t.Price - pp.Price1)*posPosition;
                                    pp.Price2 = t.Price;

                                    pp.LastTradeDT = t.DT;
                                    pp.LastTradeNumber = t.Number;
                                    pp.Comment += String.Format("Close.Reverse {0} {1:C} {2} {3} {4}.", posPosition,
                                                                pp.Price1,
                                                                t.Number, t.Position, t.Price);
                                   // pp.Index = ++_maxIndex;
                             // *************** Total ******************************************
                                    PositionTotals.UpdateTotalOrNew(pp);

                                    //PositionClosedCollection.Add(pp); //  Insert(0, pp);

                                    var p = new Position(t.Account, t.Strategy, t.Ticker, Position.Reverse(posOperation),
                                                         t.Quantity - pp.Quantity,
                                                         t.Price, t.Price, 0, PosStatusEnum.Opened,
                                                         t.DT, t.Number, t.DT, t.Number);
                                    p.Comment += String.Format("Open.Reverse {0} {1:C} {2} {3} {4}. ", posPosition,
                                                               pp.Price1,
                                                               t.Number, t.Position, t.Price);
                                  //  p.Index = ++_maxIndex;

                                    //PositionCollection.Add(p);
                                    AddPosition(p);

                                    _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                                      "Position", "Reverse. Close " + pp.OperationString,
                                                      pp.ToString(), t.ToString());
                                    _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                                                   "Position", "Reverse. Open " + p.OperationString,
                                                                   p.ToString(), t.ToString());
                                }
                                else
                                {
                                    _eventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING, "Position",
                                                                   "Position", "Nobody's Fools - ???",
                                                                   String.Format("{0} {1} {2} {3}", posPosition,
                                                                                 posQuantity, t.Position, t.Quantity),
                                                                   t.ToString());

                                    _eventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING, "Position",
                                                                   "Position", "Nobody's Fools - ???",
                                                                   pp.ToString(), t.ToString());
                                }
                            }
                        }
                    }
                }
                else
                {
                    /*
                        if (t.Quantity == 0 || t.Operation == 0)
                        {
                            _EventLog.Add(new EventLogItem(EvlResult.FATAL, "New Position",
                                                           String.Format("{0} {1} {2} {3} {4}",
                                                                          t.Number, t.DT, t.Operation, t.Quantity, t.TradePrice)));
                        }
                    */

                    _needOpenedToObserve = +1;
                    var p1 = new Position(t.Account, t.Strategy, t.Ticker, (PosOperationEnum)t.Operation, t.Quantity, t.Price, t.Price, 0, 
                                                PosStatusEnum.Opened, t.DT, t.Number, t.DT, t.Number);

                    p1.Comment += String.Format("Open {0} {1} {2}. ", p1.Pos, t.Number, t.Price);
                   // p1.Index = ++_maxIndex;

                    //PositionCollection.Add(p1);

                    AddPosition(p1);
                    _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position", "Position",
                                                   "Open " + p1.OperationString, p1.ToString(), t.ToString());

                    //PositionTotals.GetTotalOrNew(t);
                }
            }
        }
        public void PositionCalculate3(Trade t)
        {
            lock (_lockPosition)
            {
                //_needToObserve = +1;

                var pp = PositionCollection.FirstOrDefault( p => p.StrategyKey == t.StrategyKey && p.Opened);
                if ((pp != null))
                {
                    var oldPosition = (Position)pp.Clone(); // Make Clone
                    var oldposition = pp.Pos;
                        //var posPosition = pp.Pos;
                        //var posQuantity = pp.Quantity;
                        //var posOperation = pp.Operation;

                        pp.LastTradeDT = t.DT;
                        pp.LastTradeNumber = t.Number;
                        pp.Price2 = t.Price;

                        if ((short)pp.Operation == (short)t.Operation)  // Rise Size !!! Position Operation the Same. For example: Buy & Buy
                        {

                            pp.Price1 = (pp.Price1 * pp.Quantity + t.Price * t.Quantity) / (pp.Quantity + t.Quantity);
                            pp.Quantity += t.Quantity;
                            //pp.Count += t.Quantity;
                            pp.Comment += String.Format("Rise.{0} {1:C} {2} {3} {4}. ", pp.Pos, pp.Price1, t.Number,
                                                        t.Position, t.Price);

                            pp.InvokePositionChangedEvent(oldposition, pp.Pos);
                            pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.ReSizedUp);
                            // PositionCollection.Add(pp);

                            _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                                           "Position", "Rise Size" + pp.OperationString, pp.ToString(), t.ToString());
                        }
                        else
                        {   // Position Opened and Position Status Not the Same with Trade Operation
                            // Close Flag Shoud Be S E E E E T

                            // Something Should be Close   Close  3 -3  ReSize 3 -1  Reverse 3 -5      

                            _needClosedToObserve = +1;
                            _needOpenedToObserve = +1;

                            if (pp.Pos + t.Position == 0)   // ONLY Only Close Position -> addToClose and Clear Current Position
                            {
                                //   Delete(pp, PositionOpenedCollection);

                                _needClosedToObserve = +1;
                                _needOpenedToObserve = +1;

                                var p = new Position(pp.Account, pp.Strategy, pp.TickerStr, pp.Operation,
                                                     pp.Quantity,
                                                     pp.Price1, t.Price, (t.Price - pp.Price1)*pp.Pos,
                                                     PosStatusEnum.Closed,
                                                     pp.FirstTradeDT, pp.FirstTradeNumber, t.DT, t.Number)
                                            {
                                                Ticker = pp.Ticker,
                                                Comment = pp.Comment + String.Format("Close {0} {1:C} {2} {3} {4}. ",
                                                                                     pp.Pos, pp.Price1, t.Number,
                                                                                     t.Position, t.Price)
                                            };
                                
                                AddPosition(p);

                                pp.Clear();
                             //   pp.Index = ++_maxIndex;

                                pp.InvokePositionChangedEvent(oldposition, pp.Pos);
                                pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.Closed);

                                // ********************************* Total ***************************
                                PositionTotals.UpdateTotalOrNew(p);

                                // PositionClosedCollection.Add(pp); //Insert(0, pp);

                                _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                                               "Position", "Close " + p.OperationString, p.ToString(),
                                                               t.ToString());
                            }
                            else        //(pp.Pos + t.Position != 0)  Reduce  or Reverse Position 
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
                                                    Comment =
                                                        pp.Comment +
                                                        String.Format("Close.Reduce Size{0} {1:C} {2} {3} {4}. ",
                                                                      pp.Pos, pp.Price1, t.Number, t.Position, t.Price)
                                                };
                                    AddPosition(p);

                                    pp.Quantity = pp.Quantity - t.Quantity;
                                    pp.Price2 = t.Price; // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
                                    pp.Pnl = (t.Price - pp.Price1) * pp.Pos;

                                    pp.LastTradeDT = t.DT;
                                    pp.LastTradeNumber = t.Number;

                                    pp.Comment += String.Format("Open.Reduce {0} {1:C} {2} {3} {4}. ",
                                                            pp.Pos, pp.Price1, t.Number, t.Position, t.Price);
                              //      pp.Index = ++_maxIndex;

                                    pp.InvokePositionChangedEvent(oldposition, pp.Pos);
                                    pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.ReSizedDown);

                                    // ************************************** Total *****************************
                                    PositionTotals.UpdateTotalOrNew(p);

                                    // PositionClosedCollection.Add(pp); // Insert(0, pp);

                                    _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                                                   "Position", "Reduce.Close " + p.OperationString,
                                                                   p.ToString(), t.ToString());
                                    _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                                                   "Position", "Reduce.Open " + pp.OperationString,
                                                                   pp.ToString(), t.ToString());
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
                                    AddPosition(p);

                                    pp.Operation = Position.Reverse(pp.Operation);
                                    pp.Quantity = t.Quantity - pp.Quantity;

                                    pp.Price1 = t.Price;
                                    pp.Price2 = t.Price;

                                    pp.Pnl = (t.Price - pp.Price1) * pp.Pos;
                                    
                                    pp.FirstTradeDT = t.DT;
                                    pp.FirstTradeNumber = t.Number;
                                    pp.LastTradeDT = t.DT;
                                    pp.LastTradeNumber = t.Number;

                                    // !!!! OUT of Range pp.Comment += String.Format("Open.Reverse {0} {1:C} {2} {3} {4}. ", pp.Pos,
                                                             //  pp.Price1,
                                                             //  t.Number, t.Position, t.Price);
                                  //  pp.Index = ++_maxIndex;

                                    pp.InvokePositionChangedEvent(oldposition, pp.Pos);
                                    pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.Reversed);

                                    // *************** Total ******************************************
                                    PositionTotals.UpdateTotalOrNew(p);


                                    _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                                      "Position", "Reverse. Close " + p.OperationString,
                                                      p.ToString(), t.ToString());
                                    _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position",
                                                                   "Position", "Reverse. Open " + pp.OperationString,
                                                                   pp.ToString(), t.ToString());
                                }
                                else
                                {
                                    _eventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING, "Position",
                                                                   "Position", "Nobody's Fools - ???",
                                                                   String.Format("{0} {1} {2} {3}", pp.Pos,
                                                                                 pp.Quantity, t.Position, t.Quantity),
                                                                   t.ToString());

                                    _eventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING, "Position",
                                                                   "Position", "Nobody's Fools - ???",
                                                                   pp.ToString(), t.ToString());
                                }
                            }
                        }
                }
                else // Position not IsOpened
                {
                    _needOpenedToObserve = +1;
                    pp = PositionCollection.FirstOrDefault(p => p.StrategyKey == t.StrategyKey && p.IsNeutral);
                    if (pp != null)
                    {
                        var oldPosition = (Position)pp.Clone(); // Make Clone

                        pp.Operation = (PosOperationEnum) t.Operation;
                        pp.Quantity = t.Quantity;
                        pp.Price1 = t.Price;
                        pp.Price2 = t.Price;
                        pp.Status = PosStatusEnum.Opened;

                        pp.FirstTradeDT = t.DT;
                        pp.FirstTradeNumber = t.Number;

                        pp.LastTradeDT = t.DT;
                        pp.LastTradeNumber = t.Number;

                      //  pp.Index = ++ _maxIndex;

                        pp.InvokePositionChangedEvent(0, pp.Pos);
                        pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.Opened);

                        _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position", "Position",
                                          "Open " + pp.OperationString, pp.ToString(), t.ToString());
                    }
                    else
                    {
                        // Position Neutral is Not Exist

                        // !!!!!!!!!!!!!! -- throw new NullReferenceException("Position Neutral is Not Exist");

                        var pOld = new Position(t.Account, t.Strategy, t.Ticker, PosOperationEnum.Neutral,
                                             0, 0, 0, 0,
                                             PosStatusEnum.Closed, t.DT, 0, t.DT, 0);
                        pOld.Clear();

                        var p = new Position(t.Account, t.Strategy, t.Ticker, (PosOperationEnum) t.Operation,
                                             t.Quantity, t.Price, t.Price, 0,
                                             PosStatusEnum.Opened, t.DT, t.Number, t.DT, t.Number);

                        p.Comment += String.Format("Open {0} {1} {2}. ", p.Pos, t.Number, t.Price);
                      //  p.Index = ++_maxIndex;

                        //PositionCollection.Add(p1);

                        AddPosition(p);

                        p.InvokePositionChangedEvent(0, p.Pos);
                        p.InvokePositionChangedEvent(pOld, PositionChangedEnum.Opened);

                        _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position Unknown", "Position Unknown",
                                          "New Current" + p.OperationString, p.ToString(), t.ToString());
                    }
                }
            }
           // if (_needClosedToObserve > 0 && NeedToObserverEvent != null) NeedToObserverEvent();
            if ( NeedToObserverEvent != null ) NeedToObserverEvent();
        }

        public void PositionCalculate5(ITrade t)
        {
            Position pp;
            lock (_lockCurrentPosition)
            {
                pp = PositionCurrentCollection.FirstOrDefault(p => p.StrategyKey == t.StrategyKey);
            }
            if (pp != null && pp.IsOpened)
            {
                    var oldPosition = (Position)pp.Clone(); // Make Clone
                    var oldposition = pp.Pos;
                    //var posPosition = pp.Pos;
                    //var posQuantity = pp.Quantity;
                    //var posOperation = pp.Operation;

                    pp.LastTradeDT = t.DT;
                    pp.LastTradeNumber = t.Number;
                    pp.Price2 = t.Price;

                    if (t.Operation == TradeOperationEnum.Buy)
                        pp.LastTradeBuyPrice = (float)t.Price;
                    else if (t.Operation == TradeOperationEnum.Sell)
                        pp.LastTradeSellPrice = (float)t.Price;

                try
                {
                    if ((short) pp.Operation == (short) t.Operation)
                        // Rise Size !!! Position Operation the Same. For example: Buy & Buy
                    {

                        pp.Price1 = (pp.Price1*pp.Quantity + t.Price*t.Quantity)/(pp.Quantity + t.Quantity);
                        pp.Quantity += t.Quantity;
                        //pp.Count += t.Quantity;
                        //pp.Comment += String.Format("Rise.{0} {1:C} {2} {3} {4}. ", pp.Pos, pp.Price1, t.Number,
                        //    t.Position, t.Price);

                        pp.InvokePositionChangedEvent(oldposition, pp.Pos);
                        pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.ReSizedUp);
                        // PositionCollection.Add(pp);

                        _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, pp.StrategyTickerString, "Position",
                            "ReSized Up",
                            pp.PositionString5 + " ReSized Up from " + oldPosition.PositionString5, pp.ToString());
                    }
                    else
                    {
                        // Position Opened and Position Status Not the Same with Trade Operation
                        // Close Flag Shoud Be S E E E E T

                        // Something Should be Close   Close  3 -3  ReSize 3 -1  Reverse 3 -5      

                        _needClosedToObserve = +1;
                        _needOpenedToObserve = +1;

                        if (pp.Pos + t.Position == 0)
                            // ONLY Only Close Position -> addToClose and Clear Current Position
                        {
                            //   Delete(pp, PositionOpenedCollection);

                            _needClosedToObserve = +1;
                            _needOpenedToObserve = +1;

                            var p = new Position(pp.Account, pp.Strategy, pp.TickerStr, pp.Operation,
                                pp.Quantity,
                                pp.Price1, t.Price, (t.Price - pp.Price1)*pp.Pos,
                                PosStatusEnum.Closed,
                                pp.FirstTradeDT, pp.FirstTradeNumber, t.DT, t.Number)
                            {
                                Ticker = pp.Ticker,
                                //Comment = pp.Comment + String.Format("Close {0} {1:C} {2} {3} {4}. ",
                                //    pp.Pos, pp.Price1, t.Number,
                                //    t.Position, t.Price)
                            };

                            pp.Clear();

                            AddPosition(p);

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
                                    pp.Price1, t.Price, (t.Price - pp.Price1)*pp.Pos, PosStatusEnum.Closed,
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
                                pp.Pnl = (t.Price - pp.Price1)*pp.Pos;

                                pp.LastTradeDT = t.DT;
                                pp.LastTradeNumber = t.Number;

                              //  pp.Comment += String.Format("Open.Reduce {0} {1:C} {2} {3} {4}. ",
                              //      pp.Pos, pp.Price1, t.Number, t.Position, t.Price);
                                //      pp.Index = ++_maxIndex;

                                AddPosition(p); // need to refresh

                                pp.InvokePositionChangedEvent(oldposition, pp.Pos);
                                pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.ReSizedDown);

                                // ************************************** Total *****************************
                                // PositionTotals.UpdateTotalOrNew(p);
                                pp.PositionTotal.Update(p); // 15.11.2012

                                // PositionClosedCollection.Add(pp); // Insert(0, pp);

                                _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, pp.StrategyTickerString,
                                    "Position", "ReSized Down",
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
                                    pp.Price1, t.Price, (t.Price - pp.Price1)*pp.Pos, PosStatusEnum.Closed,  // 
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
                                AddPosition(p);

                                pp.Operation = Position.Reverse(pp.Operation);
                                pp.Quantity = t.Quantity - pp.Quantity;

                                pp.Price1 = t.Price;
                                pp.Price2 = t.Price;

                                pp.Pnl = (t.Price - pp.Price1)*pp.Pos;

                                pp.FirstTradeDT = t.DT;
                                pp.FirstTradeNumber = t.Number;
                                pp.LastTradeDT = t.DT;
                                pp.LastTradeNumber = t.Number;

                                _needOpenedToObserve = +1;

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
                                    "Position", "Reversed",
                                    pp.PositionString5 + " Reversed from " + oldPosition.PositionString5, pp.ToString());
                            }
                            else
                            {
                                _eventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING, "Position",
                                    "Position", "Nobody's Fools - ???",
                                    String.Format("{0} {1} {2} {3}", pp.Pos,
                                        pp.Quantity, t.Position, t.Quantity),
                                    t.ToString());

                                _eventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING, "Position",
                                    "Position", "Nobody's Fools - ???",
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
                else if (pp != null && pp.IsNeutral) // Position not IsOpened
                {
                    _needOpenedToObserve = +1;
                    // pp = PositionCurrentCollection.FirstOrDefault(p => p.StrategyKey == t.StrategyKey && p.IsNeutral);

                    var oldPosition = (Position) pp.Clone(); // Make Clone

                    pp.Operation = (PosOperationEnum) t.Operation;
                    pp.Quantity = t.Quantity;
                    pp.Price1 = t.Price;
                    pp.Price2 = t.Price;
                    pp.Status = PosStatusEnum.Opened;

                    pp.FirstTradeDT = t.DT;
                    pp.FirstTradeNumber = t.Number;

                    pp.LastTradeDT = t.DT;
                    pp.LastTradeNumber = t.Number;

                    if (t.Operation == TradeOperationEnum.Buy)
                    {
                        pp.LastTradeBuyPrice = (float) t.Price;
                        pp.LastTradeSellPrice = 0f;
                    }
                    else if (t.Operation == TradeOperationEnum.Sell)
                    {
                        pp.LastTradeSellPrice = (float) t.Price;
                        pp.LastTradeBuyPrice = 0f;
                    }
                    //  pp.Index = ++ _maxIndex;

                    pp.InvokePositionChangedEvent(0, pp.Pos);
                    pp.InvokePositionChangedEvent(oldPosition, PositionChangedEnum.Opened);

                    _needOpenedToObserve = +1;

                    _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, pp.StrategyTickerString, "Position" ,
                                      "New" , pp.PositionString5, pp.ToString());
                }
                else if ( pp == null)
                {
                        // Position Neutral is Not Exist

                        // !!!!!!!!!!!!!! -- throw new NullReferenceException("Position Neutral is Not Exist");

                       // _tx
                    /*   
                    var pOld = new Position(t.Account, t.Strategy, t.Ticker, Position.OperationEnum.Neutral,
                                             0, 0, 0, 0,
                                             Position.StatusEnum.Closed, t.DT, 0, t.DT, 0);
                    */
                        var pOld = new PositionNpc {Account = t.Account, Strategy = t.Strategy, TickerStr = t.Ticker};
                        pOld.Clear();
                       /* 
                        var p = new Position(t.Account, t.Strategy, t.Ticker, (Position.OperationEnum)t.Operation,
                                             t.Quantity, t.Price, t.Price, 0,
                                             Position.StatusEnum.Opened, t.DT, t.Number, t.DT, t.Number);
                        */
                    var p = new PositionNpc
                                {
                                    Account = t.Account,
                                    Strategy = t.Strategy,
                                    TickerStr = t.Ticker,
                                    Operation = (PosOperationEnum) t.Operation,
                                    Quantity = t.Quantity,
                                    Price1 = t.Price,
                                    Price2 = t.Price,
                                    Pnl = 0,
                                    Status = PosStatusEnum.Opened,
                                    FirstTradeDT = t.DT,
                                    FirstTradeNumber = t.Number,
                                    LastTradeDT = t.DT,
                                    LastTradeNumber = t.Number
                                };

                        p.Comment += String.Format("Open {0} {1} {2}. ", p.Pos, t.Number, t.Price);
                        //  p.Index = ++_maxIndex;

                        //PositionCollection.Add(p1);

                        var pt = PositionTotals.Register(p);
                        if (pt == null)
                            throw new NullReferenceException("PositionTotal - Invalid Registration");
                        p.PositionTotal = pt;
                        pOld.PositionTotal = pt;

                        AddCurrentPosition(p);

                        _needOpenedToObserve = +1;

                        p.InvokePositionChangedEvent(0, p.Pos);
                        p.InvokePositionChangedEvent(pOld, PositionChangedEnum.Opened);

                        _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Position","Unknown",
                                          "New Current" + p.PositionString5, p.ToString(), t.ToString());
                
                }
            
            // if (_needClosedToObserve > 0 && NeedToObserverEvent != null) NeedToObserverEvent();
            if (NeedToObserverEvent != null) NeedToObserverEvent();
        }

        public void FireObserverEvent()
        {
            if (NeedToObserverEvent != null) NeedToObserverEvent();
        }

        public void GetPositionClosed(int index, IList<IPosition> pcList)
        {
            lock (_lockPosition)
            {
                foreach(var p in PositionCollection.Where(p => (p.Index > index && p.IsClosed && !p.IsNeutral)))
                    pcList.Add(p);
            }
        }

        public void GetPositionsClosedToObserve()
        {
          // if (_needClosedToObserve <= 0) return;
            int cnt1, cnt2;
            lock (_lockPosition)
            {
                var i = _lastObserveGetRequestNumber;
                foreach (var p in PositionCollection.Where(p => (p.Index > i && p.IsClosed && !p.IsNeutral)))
                {
                    PositionClosedObserveCollection.Insert(0, p);
                    if (_lastObserveGetRequestNumber < p.Index) _lastObserveGetRequestNumber = p.Index;

                  //  _eventLog.AddItem(EvlResult.SUCCESS,EvlSubject.TECHNOLOGY,"Positions","GetClosedToObserver",
                  //      p.ToString(), _lastObserveGetRequestNumber.ToString());
                }
            
              //  _needClosedToObserve = 0; 
                cnt1 = PositionCollection.Where(p => p.IsClosed && !p.IsNeutral).Count();
                cnt2 = PositionClosedObserveCollection.Count;
            }
            if( cnt1 != cnt2 )
                _eventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING, "PositionObserver", "PositionObserver",
                                  "PositionClosedCount = " + cnt1 + " is Not Equal with ClosedObserverCount = " + cnt2, "","");
            /*
            lock (_lockPosition)
            {
                PositionClosedObserveCollection.Clear();
                foreach (
                    var p in PositionCollection.Where(p => p.Closed)
                    )
                {
                    PositionClosedObserveCollection.Insert(0, p);
                   // _lastObserveGetRequestNumber = p.LastTradeNumber;
                }
            }
            */
            /*
                if ( (PositionCollection.Where(p => p.Closed)).Count() > 0)
                {
                    var p = (PositionCollection.Where(po => po.Closed)).Last();
                    _lastObserveGetRequestNumber = p.MyIndex;
                }
                else
                    _lastObserveGetRequestNumber = 0;
                */
// WORK !!!!    
                /*
                PositionOpenedObserveCollection.Clear();
                foreach(var p in  PositionCollection.Where(p => p.Opened ))
                {
                    PositionOpenedObserveCollection.Add(p); //Insert(0, p);
                }

                PositionTotals.PositionTotalsObserveCollection.Clear();
                var valueCollection = PositionTotals.PositionTotalCollection.Values;
                foreach (Totals.PositionTotal p in valueCollection)
                {
                    PositionTotals.PositionTotalsObserveCollection.Add(p);
                }
                */
            
        }
        public void GetPositionsOpenedToObserve()
        {
            foreach (var p in PositionOpenedObserveCollection.Where(p => p.Opened && p.Ticker != null))
            {
                //p.Price2 = new decimal(p.Ticker.LastPrice);
                p.Price2 = (decimal)p.Ticker.LastPrice;
            }

            // if (_needOpenedToObserve <= 0) return;

            if (PositionOpenedObserveCollection.Count == _positionCurrentCount)
                return;

            PositionOpenedObserveCollection.Clear();
            lock (_lockCurrentPosition)
            {
                    foreach (var p in PositionCurrentCollection) //.Where(p => p.Opened || p.Operation == Position.OperationEnum.Neutral))
                    {
                        PositionOpenedObserveCollection.Add(p); //Insert(0, p);
                       // if( p.IsOpened && p.Ticker != null)
                       //     p.Price2 = (decimal)p.Ticker.LastPrice;
                    }

                _needOpenedToObserve = 0;

                /*
                foreach(var p in PositionOpenedObserveCollection.Where( p=>p.Closed) )
                {
                    PositionOpenedObserveCollection.Remove(p);
                }
                var except2 = (PositionCollection.Where (p => p.Opened)).Except(PositionOpenedObserveCollection);
                foreach (var p in except2)
                    PositionOpenedObserveCollection.Add(p);
                 */
            }
            
        }
        private void Delete(Position position, ICollection<Position> pcollection)
        {
            lock (this)
            {
                //foreach (var p in pcollection.Where(p => p.PositionId == position.PositionId))
                //{
                    pcollection.Remove(position);
                //}
            }
        }      

    }
}
