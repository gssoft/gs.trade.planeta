using System;
using System.Xml.Serialization;
using GS.Interfaces;
using GS.Trade.Data.Studies.GS.xMa018;
using GS.Trade.Interfaces;
using GS.Trade.Data;
using GS.Trade.Data.Studies.GS;
using GS.Trade.Trades;

namespace GS.Trade.Strategies
{
    public class X302 : Strategy
    {
        [XmlIgnore]
        public override IBars Bars { get; protected set; }
        [XmlIgnore]
        public override int MaxBarsBack { get; protected set; }

        private Xma018 _xma018;
        private Xma018 _xMainTrend;
        private int _xMainTrendValue;

  //      private BarSeries _bars;

        public uint TimeInt2 { get; set; }

        public int Ma1Length { get; set; }
        public int Ma1AtrLength { get; set; }
        public float Ma1KAtr { get; set; }
        public int Ma1Mode { get; set; }

        public int Ma2Length { get; set; }
        public int Ma2AtrLength { get; set; }
        public float Ma2KAtr { get; set; }
        public int Ma2Mode { get; set; }

        public float KAtrStop { get; set; }
        public int SwingCount { get; set; }
        public int EntryMode { get; set; }
        public int RandMode { get; set; }

        private DateTime _lastDT = DateTime.MinValue;

        private int _trend;
        private bool _trendWasChanged;

        private long _tradeNumber;
        private int _position;

        private double _currentLimitPrice;
        private double _lastLimitBuyPrice;
        private double _lastLimitSellPrice;
        private double _lastStopPrice;

        private string _comment;

        // private float _kAtrStop = 1.5f;
        private int _swingCount;

        private int _swingCountLong;
        private int _swingCountShort;

        //private double _xBand;
        private double _xBandAtr;

        private Random _random;
        private bool _randAnswer;
        private int _randValue;
        private int _randCount;

        public X302()
        {
            // need for Serialization

        }
        public X302(ITradeContext tx, string name, string code, Ticker ticker, uint timeInt)
            : base(tx, name, code, ticker, timeInt)
        {
        }
        public override void Init()
        {
            base.Init(); if (IsWrong) return;
            Position.PositionChangedEvent += PositionIsChangedEventHandler;

            _xma018 = (Xma018)TradeContext.RegisterTimeSeries(new Xma018("Xma18", Ticker, (int)TimeInt, Ma1Length, Ma1AtrLength, Ma1KAtr, Ma1Mode));
            _xma018.Init();

            _xMainTrend = (Xma018)TradeContext.RegisterTimeSeries(new Xma018("Xma18", Ticker, (int)TimeInt2, Ma2Length, Ma2AtrLength, Ma2KAtr, Ma2Mode));
            _xMainTrend.Init();

            _random = new Random( Code.GetHashCode() );
            _randCount = 1;

            TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategy", "Strategy", "Init " + Code, ToString(), "");
        }

        private void PositionIsChangedEventHandler(long oldposition, long newposition)
        {
            if (newposition > 0)
                TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                                    OrderTypeEnum.StopLimit, OrderOperationEnum.Buy);
            else if (newposition < 0)
                TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                                    OrderTypeEnum.StopLimit, OrderOperationEnum.Sell);

            TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.TRADING, Code, Code + "." + Ticker.Code, "PositionChanged",
                                         "New: " + newposition + "; Old: " + oldposition, "");

            if (Math.Sign(oldposition) != Math.Sign(newposition) ||     // reverse
                (oldposition == 0 && newposition != 0)                 // open new from flat
                )
            
                _swingCount = 0;

            SetStopOrderFilledStatus(false);
        }

        public override void Main()
        {
            if (_xma018.Count < 2) return;
            if (_xMainTrend.Count < 1) return;

            if (_xma018.LastItemCompletedDT.CompareTo(_lastDT) <= 0) return;

            //TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.PROGRAMMING, Name, "Main",
            //                                    _xma018.LastItemCompleted.ToString(), _xma018.ToString());

            _lastDT = _xma018.LastItemCompletedDT;

            if (ExitInEmergencyWhenStopUnFilled()) return;

            if (_xMainTrendValue != _xMainTrend.Trend)
            {
                _xMainTrendValue = _xMainTrend.Trend;
                _randCount++;
            }

            if (_xma018.TrendChanged)
            {
                _lastLimitBuyPrice = 0.0;
                _lastLimitSellPrice = 0.0;

                _swingCount++;
                _randCount++;

                // if (RandMode > 0) AskRandom();             
            }
         //   if (XMainTrend.TrendChanged) _randCount++;

            var contract = Position.Pos == 0 ? 1 : 2;

            if (_xMainTrend.Trend > 0)
            {
                _swingCountLong = SwingCount;
                _swingCountShort = SwingCount;
               // _swingCountShort = 1;
            }
            else if (_xMainTrend.Trend < 0)
            {
                // _swingCountLong = 1;
                _swingCountLong = SwingCount;
                _swingCountShort = SwingCount;
            }

            // ***************************** Long Entry ***********************
            if (Position.Pos <= 0 && _xma018.Trend < 0)
            {
            #region EntryMode == 1 First Impulse Buy Ask
                if ( EntryMode == 1 &&
                    _xma018.IsImpulse &&
                    Ticker.Ask.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) < 0 &&
                    (
                        (Position.Pos < 0 && _swingCount >= _swingCountShort) ||
                        (Position.Pos == 0 && _xMainTrend.Trend > 0)
                    )
                    )
                {
                    _comment = "Buy BestAsk.Impulse";
                    _currentLimitPrice = Ticker.BestAsk;
                }
            #endregion
            #region EntryMode == 11 First Impulse Buy Ma
                else if (EntryMode == 11 &&_xma018.IsImpulse &&
                    // &&  Ticker.Ask.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) < 0 &&
                    (
                        (Position.Pos < 0 && _swingCount >= _swingCountShort) ||
                        (Position.Pos == 0 && _xMainTrend.Trend > 0)
                    )
                    )
                {
                    _comment = "Buy Ma.Impulse";

                    _currentLimitPrice = ((Xma018.Item)(_xma018.LastItemCompleted)).Ma;
                    _currentLimitPrice = Ticker.ToMinMove(_currentLimitPrice, +1);

                    if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitBuyPrice = _currentLimitPrice;
                }
            #endregion
            #region EntryMode == 2 First Flat && Ask < Ma without Random
                //*************************************************************************************
                
                else if (EntryMode == 2 &&
                    _xma018.Trend < 0 && _xma018.IsFlat &&
                    Ticker.Ask.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) < 0 &&
                    (
                        (Position.Pos < 0 && _swingCount >= _swingCountShort) ||
                        (Position.Pos == 0 && _xMainTrend.Trend > 0)
                    )
                    )
                {
                    _comment = "BuyFlat";
                    _currentLimitPrice = ((Xma018.Item)(_xma018.LastItemCompleted)).Ma;
                    _currentLimitPrice = Ticker.ToMinMove(_currentLimitPrice, +1);

                    if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitBuyPrice = _currentLimitPrice;
                }
                #endregion
            #region EntryMode == 3 First Ask < Ma + Ask Random
                //*************************************************************************************
                else if (   EntryMode == 3 &&
                            _xma018.Trend < 0 && _xma018.IsFlat &&
                            Ticker.Bid.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) < 0
                        )
                {
                    if ( Position.Pos < 0) AskRandom();
                    if (
                        (Position.Pos < 0 && _xMainTrend.Trend > 0 && _randAnswer) ||
                        (Position.Pos < 0 && _xMainTrend.Trend < 0 && !_randAnswer) ||
                        (Position.Pos == 0 && _xMainTrend.Trend > 0)
                        )
                    {

                        _comment = String.Format("Buy Low.Flat: Pos={0},MainTrend={1},Trend={2},RandAnswer={3},Value={4}",
                                                            Position.Pos, _xMainTrend.Trend, _xma018.Trend, _randAnswer, _randValue);

                        _currentLimitPrice = ((Xma018.Item)(_xma018.LastItemCompleted)).Low;
                        _currentLimitPrice = Ticker.ToMinMove(_currentLimitPrice, +1);

                        if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                        _lastLimitBuyPrice = _currentLimitPrice;

                        TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, Code, Code, "Long Entry",
                                              String.Format("Buy Flat: Pos={0},MainTrend={1},Trend={2},RandAnswer={3},Value={4}",
                                                            Position.Pos, _xMainTrend.Trend, _xma018.Trend, _randAnswer, _randValue), "");

                    }
                    else
                    {
                        TradeContext.EventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, Code, Code, "Long Entry Reject",
                                              String.Format("Buy Flat: Pos={0},MainTrend={1},Trend={2},RandAnswer={3},Value={4}",
                                                            Position.Pos, _xMainTrend.Trend, _xma018.Trend, _randAnswer, _randValue), "");
                        return;
                    }
                }
                #endregion
            #region EntryMode == 5 First Impulse + AskRandom() Random
                else if (EntryMode == 5 && _xma018.IsImpulse
                        // && Ticker.Ask.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) < 0
                    )
                {
                    if (Position.Pos < 0) AskRandom();
                    if (
                        (Position.Pos < 0 && _xMainTrend.Trend > 0 && _randAnswer) ||
                        (Position.Pos < 0 && _xMainTrend.Trend < 0 && !_randAnswer) ||
                        (Position.Pos == 0 && _xMainTrend.Trend > 0)
                        )
                    {

                        _comment = String.Format("Buy Ma.Impulse: Pos={0},MainTrend={1},Trend={2},RandAnswer={3},Value={4}",
                                                            Position.Pos, _xMainTrend.Trend, _xma018.Trend, _randAnswer, _randValue);

                        _currentLimitPrice = ((Xma018.Item)(_xma018.LastItemCompleted)).Ma;
                        _currentLimitPrice = Ticker.ToMinMove(_currentLimitPrice, +1);

                        if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                        _lastLimitBuyPrice = _currentLimitPrice;

                        TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, Code, Code, "Long Entry",
                                              String.Format("Buy Impulse: Pos={0},MainTrend={1},Trend={2},RandAnswer={3},Value={4}",
                                                            Position.Pos, _xMainTrend.Trend, _xma018.Trend, _randAnswer, _randValue), "");

                    }
                    else
                    {
                        TradeContext.EventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, Code, Code, "Long Entry Reject",
                                              String.Format("Buy Impulse: Pos={0},MainTrend={1},Trend={2},RandAnswer={3},Value={4}",
                                                            Position.Pos, _xMainTrend.Trend, _xma018.Trend, _randAnswer, _randValue), "");
                        return;
                    }

                }
            #endregion
            #region EntryMode == 8 First Impulse + AskRandom() Random
                else if (EntryMode == 8 && _xma018.IsFlat 
                      //  && Ticker.Ask.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) < 0
                    )
                {
                    if (Position.Pos < 0) AskRandom();
                    if (
                        (Position.Pos < 0 && _xMainTrend.Trend > 0 && _randAnswer) ||
                        (Position.Pos < 0 && _xMainTrend.Trend < 0 && !_randAnswer) ||
                        (Position.Pos == 0 && _xMainTrend.Trend > 0)
                        )
                    {

                        _comment = String.Format("Buy Ma.Flat: Pos={0},MainTrend={1},Trend={2},RandAnswer={3},Value={4}",
                                                            Position.Pos, _xMainTrend.Trend, _xma018.Trend, _randAnswer, _randValue);

                        _currentLimitPrice = ((Xma018.Item)(_xma018.LastItemCompleted)).Ma;
                        _currentLimitPrice = Ticker.ToMinMove(_currentLimitPrice, +1);

                        if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                        _lastLimitBuyPrice = _currentLimitPrice;

                        TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, Code, Code, "Long Entry",
                                              String.Format("Buy Flat: Pos={0},MainTrend={1},Trend={2},RandAnswer={3},Value={4}",
                                                            Position.Pos, _xMainTrend.Trend, _xma018.Trend, _randAnswer, _randValue), "");

                    }
                    else
                    {
                        TradeContext.EventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, Code, Code, "Long Entry Reject",
                                              String.Format("Buy Flat: Pos={0},MainTrend={1},Trend={2},RandAnswer={3},Value={4}",
                                                            Position.Pos, _xMainTrend.Trend, _xma018.Trend, _randAnswer, _randValue), "");
                        return;
                    }

                }
            #endregion
                else return;

                SetOrder(OrderOperationEnum.Buy);

            }

            // ******************************** Short Entry *****************
            else if (Position.Pos >= 0 && _xma018.Trend > 0)
            {
                #region EntryMode == 1 First Impulse without Rand
                if ( EntryMode == 1 &&
                        _xma018.IsImpulse &&
                        Ticker.Bid.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) > 0 &&
                        (
                        (Position.Pos > 0 && _swingCount >= _swingCountLong) ||
                        (Position.Pos == 0 && _xMainTrend.Trend < 0)
                        )                         
                    )
                {
                    _comment = "Sell BestBid.Impulse";
                    _currentLimitPrice = Ticker.BestBid;
                }
                #endregion
                #region EntryMode == 11 First Impulse without Rand
                else if (EntryMode == 11 && _xma018.IsImpulse &&
                        // Ticker.Bid.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) > 0 &&
                        (
                        (Position.Pos > 0 && _swingCount >= _swingCountLong) ||
                        (Position.Pos == 0 && _xMainTrend.Trend < 0)
                        )
                    )
                {
                    _comment = "Sell Ma.Impulse";
                    _currentLimitPrice = ((Xma018.Item)(_xma018.LastItemCompleted)).Ma;
                    _currentLimitPrice = Ticker.ToMinMove(_currentLimitPrice, -1);

                    if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitSellPrice = _currentLimitPrice;
                }
                #endregion
                #region EntryMode == 2 First Flat && Ask < Ma without Random
                // ************************************************************************       
                else if (EntryMode == 2 &&
                           _xma018.Trend > 0 && _xma018.IsFlat &&
                           Ticker.Bid.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) > 0 &&
                           (
                           (Position.Pos > 0 && _swingCount >= _swingCountLong) ||
                           (Position.Pos == 0 && _xMainTrend.Trend < 0)
                           )
                   )
                {
                    _comment = "Sell Flat";
                    _currentLimitPrice = ((Xma018.Item)(_xma018.LastItemCompleted)).Ma;
                    _currentLimitPrice = Ticker.ToMinMove(_currentLimitPrice, -1);

                    if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitSellPrice = _currentLimitPrice;
                }
                #endregion
                #region EntryMode == 3 First Ask < Ma + Ask Random
                // ************************************************************************       
                else if (   EntryMode == 3 &&
                           _xma018.Trend > 0 && _xma018.IsFlat &&
                           Ticker.Ask.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) > 0 
                        )
                {
                    if ( Position.Pos > 0) AskRandom();
                    if (
                        (Position.Pos > 0 && _xMainTrend.Trend < 0 && _randAnswer) ||
                        (Position.Pos > 0 && _xMainTrend.Trend > 0 && !_randAnswer) ||
                        (Position.Pos == 0 && _xMainTrend.Trend < 0)
                        )
                    {
                        _comment = String.Format("Sell High.Flat: Pos={0},MainTrend={1},Trend={2},RandAnswer={3},Value={4}",
                                                            Position.Pos, _xMainTrend.Trend, _xma018.Trend, _randAnswer, _randValue);

                        _currentLimitPrice = ((Xma018.Item)(_xma018.LastItemCompleted)).High;
                        _currentLimitPrice = Ticker.ToMinMove(_currentLimitPrice, -1);

                        if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                        _lastLimitSellPrice = _currentLimitPrice;

                        TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, Code, Code, "Short Entry",
                                              String.Format("Sell Flat: Pos={0},MainTrend={1},Trend={2},RandAnswer={3},Value={4}",
                                                            Position.Pos, _xMainTrend.Trend, _xma018.Trend, _randAnswer, _randValue), "");
                    }
                    else
                    {
                        TradeContext.EventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, Code, Code, "Short Entry Reject",
                                              String.Format("Sell Flat: Pos={0},MainTrend={1},Trend={2},RandAnswer={3},Value={4}",
                                                            Position.Pos, _xMainTrend.Trend, _xma018.Trend, _randAnswer, _randValue), "");
                        return;
                    }
                }
                #endregion
                #region EntryMode == 5 First Impulse + AskRandom() Random
                else if (EntryMode == 5 && _xma018.IsImpulse
                       // && Ticker.Bid.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) > 0
                    )
                {
                    if (Position.Pos > 0) AskRandom();
                    if (
                        (Position.Pos > 0 && _xMainTrend.Trend < 0 && _randAnswer) ||
                        (Position.Pos > 0 && _xMainTrend.Trend > 0 && !_randAnswer) ||
                        (Position.Pos == 0 && _xMainTrend.Trend < 0)
                        )
                    {
                        _comment = String.Format("Sell Ma.Impulse: Pos={0},MainTrend={1},Trend={2},RandAnswer={3},Value={4}",
                                                            Position.Pos, _xMainTrend.Trend, _xma018.Trend, _randAnswer, _randValue);

                        _currentLimitPrice = ((Xma018.Item)(_xma018.LastItemCompleted)).Ma;
                        _currentLimitPrice = Ticker.ToMinMove(_currentLimitPrice, -1);

                        if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                        _lastLimitSellPrice = _currentLimitPrice;

                        TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, Code, Code, "Short Entry",
                                              String.Format("Sell Impulse: Pos={0},MainTrend={1},Trend={2},RandAnswer={3},Value={4}",
                                                            Position.Pos, _xMainTrend.Trend, _xma018.Trend, _randAnswer, _randValue), "");
                    }
                    else
                    {
                        TradeContext.EventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, Code, Code, "Short Entry Reject",
                                              String.Format("Sell Impulse: Pos={0},MainTrend={1},Trend={2},RandAnswer={3},Value={4}",
                                                            Position.Pos, _xMainTrend.Trend, _xma018.Trend, _randAnswer, _randValue), "");
                        return;
                    }
                }
                #endregion
                #region EntryMode == 8 First Impulse + AskRandom() Random
                else if (EntryMode == 8 && _xma018.IsFlat
                    // && Ticker.Bid.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) > 0
                    )
                {
                    if (Position.Pos > 0) AskRandom();
                    if (
                        (Position.Pos > 0 && _xMainTrend.Trend < 0 && _randAnswer) ||
                        (Position.Pos > 0 && _xMainTrend.Trend > 0 && !_randAnswer) ||
                        (Position.Pos == 0 && _xMainTrend.Trend < 0)
                        )
                    {
                        _comment = String.Format("Sell Ma.Flat: Pos={0},MainTrend={1},Trend={2},RandAnswer={3},Value={4}",
                                                            Position.Pos, _xMainTrend.Trend, _xma018.Trend, _randAnswer, _randValue);

                        _currentLimitPrice = ((Xma018.Item)(_xma018.LastItemCompleted)).Ma;
                        _currentLimitPrice = Ticker.ToMinMove(_currentLimitPrice, -1);

                        if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                        _lastLimitSellPrice = _currentLimitPrice;

                        TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, Code, Code, "Short Entry",
                                              String.Format("Sell Flat: Pos={0},MainTrend={1},Trend={2},RandAnswer={3},Value={4}",
                                                            Position.Pos, _xMainTrend.Trend, _xma018.Trend, _randAnswer, _randValue), "");
                    }
                    else
                    {
                        TradeContext.EventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, Code, Code, "Short Entry Reject",
                                              String.Format("Sell Flat: Pos={0},MainTrend={1},Trend={2},RandAnswer={3},Value={4}",
                                                            Position.Pos, _xMainTrend.Trend, _xma018.Trend, _randAnswer, _randValue), "");
                        return;
                    }
                }
                #endregion
                else return;

                SetOrder(OrderOperationEnum.Sell);
                
            }
            else // Position == 0
            {
                /*
                if ( EntryMode == 1)
                {
                    if (    XMainTrend.Trend > 0 && 
                            _xma018.Trend < 0 && _xma018.IsImpulse &&
                            Ticker.Ask.CompareTo(((Xma018.Item) (_xma018.LastItemCompleted)).Ma) < 0 )
                    {
                        _comment = "Buy Impulse";
                        _currentLimitPrice = Ticker.BestAsk;
                        SetOrder(OperationEnum.Buy);
                    }
                    else if (XMainTrend.Trend < 0 &&
                            _xma018.Trend > 0 && _xma018.IsImpulse &&
                            Ticker.Bid.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) > 0)
                    {
                        _comment = "Sell Impulse";
                        _currentLimitPrice = Ticker.BestBid;
                        SetOrder(OperationEnum.Sell);
                    }
                }
                else if ( EntryMode == 3)
                {

                }
                */
            }
        }

        private void SetOrder(OrderOperationEnum operation)
        {

            OrderOperationEnum flipOperation;
            int stopDirection;

            if (operation == OrderOperationEnum.Buy)
            {
                flipOperation = OrderOperationEnum.Sell;
                stopDirection = -1;
            }
            else // Sell
            {
                flipOperation = OrderOperationEnum.Buy;
                stopDirection = +1;
            }

            _xBandAtr = ((Xma018.Item)(_xma018.LastItemCompleted)).High -
                        ((Xma018.Item)(_xma018.LastItemCompleted)).Ma;

            var contract = Position.Pos == 0 ? 1 : 2;

            TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                        OrderTypeEnum.Limit, OrderOperationEnum.All);

            TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                        OrderTypeEnum.StopLimit, flipOperation);

            TradeTerminal.SetLimitOrder(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
                                        operation,
                                        _currentLimitPrice, contract,
                                        DateTime.Today.Add(new TimeSpan(23, 49, 15)),
                                        _comment);

            var stopprice = _currentLimitPrice + stopDirection * KAtrStop * _xBandAtr / Ma1KAtr ;

            stopprice = Ticker.ToMinMove(stopprice, stopDirection);

            TradeTerminal.SetStopLimitOrder(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
                                            flipOperation,
                                            stopprice, stopprice, 1,
                                            DateTime.Today.Add(new TimeSpan(23, 49, 15)), "");

            TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.TRADING, Code, Code, "New " + operation,
                                          "Price=" + _currentLimitPrice, "");
        }

        public override void Finish()
        {
            CloseAll();
            TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Name, "Finish",
                                         ToString(), "");
        }

        protected override void PositionIsChangedEventHandler2(IPosition2 oldpos, IPosition2 newpos, PositionChangedEnum changedResult)
        {
            throw new NotImplementedException();
        }

        public  void AskRandom()
        {
            if (_randCount <= 0) return;
            _randCount = 0;

            _randValue = _random.Next(1, 101);
            _randAnswer = _randValue > RandMode ? true : false;

            TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, Code, Code, "Ask Random", "Value=" + _randValue + ", Answer=" + _randAnswer, "");
        }

        public override string Key
        {
            get
            {
                return String.Format("Type={0};Name={1};Code={2};TradeAccountKey={3};TickerKey={4};TimeInt={5}",
                      GetType(), Name, Code, TradeAccountKey, TickerKey, TimeInt);
            }
        }
        public override string ToString()
        {
            return String.Format("Name={0};Code={1};Account={2};Ticker={3};TimeInt={4}",
                        Name, Code, TradeAccountKey, TickerKey, TimeInt);
        }
    }
}
