using System;
using System.Xml.Serialization;
using GS.ICharts;
using GS.Interfaces;
using GS.Trade.Data.Studies.GS.xMa018;
using GS.Trade.Interfaces;
using GS.Trade.Data;
using GS.Trade.Data.Studies;
using GS.Trade.Data.Studies.GS;


using GS.Trade.Trades;

namespace GS.Trade.Strategies
{
    
    public class X113 : Strategy
    {
        [XmlIgnore]
        public override IBars Bars { get; protected set; }
        [XmlIgnore]
        public override int MaxBarsBack { get; protected set; }

        private Xma018 _xma018;
        private Xma018 _xTrend;
        private Xma018 _xMainTrend;

        private Atr _xAtr;

        private int _xMainTrendValue;

       // private BarSeries _bars;

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

        public int Mode1 { get; set; }
        public int Mode2 { get; set; }

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

        private bool _waitFlat;

        private float _xProfit;


        private float _kProfitAtr1 = 30f;
        private float _kAtrZone1 = 2f;
        private float _kAtrStopZone2 = 3f;

        public X113()
        {
            // need for Serialization

        }
        public X113(ITradeContext tx, string name, string code, Ticker ticker, uint timeInt)
            : base(tx, name, code, ticker, timeInt)
        {
        }
        public override void Init()
        {
            base.Init(); if (IsWrong) return;
            Position.PositionChangedEvent += PositionIsChangedEventHandler;

            _xma018 = (Xma018)TradeContext.RegisterTimeSeries(new Xma018("Xma18", Ticker, (int)TimeInt, Ma1Length, Ma1AtrLength, Ma1KAtr, Ma1Mode));
            _xma018.Init();

            _xTrend = _xma018;
          //  Bars = _xTrend.Bars;
            
            //_xAtr = (Atr)Ticker.RegisterTimeSeries(new Atr("MyAtr", Ticker, (int)TimeInt, Ma1AtrLength));
            //_xAtr.Init();

            _xAtr = _xTrend.Atr;
            MaxBarsBack = _xAtr.Length;

            _xMainTrend = (Xma018)TradeContext.RegisterTimeSeries(new Xma018("Xma18", Ticker, (int)TimeInt2, Ma2Length, Ma2AtrLength, Ma2KAtr, Ma2Mode));
            _xMainTrend.Init();

            //_random = new Random(Code.GetHashCode());
            //_randCount = 1;

            TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategy", "Strategy", "Init " + Code, ToString(), "");
        }

        private void PositionIsChangedEventHandler(long oldposition, long newposition)
        {
            if (oldposition < 0)
                TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                                    OrderTypeEnum.All, OrderOperationEnum.Buy);
            else if (oldposition > 0)
                TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                                    OrderTypeEnum.All, OrderOperationEnum.Sell);

            TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.TRADING, Code, Code + "." + Ticker.Code, "PositionChanged",
                                         "New: " + newposition + "; Old: " + oldposition, "");

            if (Math.Sign(oldposition) != Math.Sign(newposition) ||     // reverse
                (oldposition == 0 && newposition != 0)                 // open new from flat
                )

                _swingCount = 0;

            if (oldposition != 0 && newposition == 0)
            {
                _waitFlat = true;
            }

            SetStopOrderFilledStatus(false);
        }

        public override void Main()
        {
          //  if (TimePlan.IsTimeToRest) return;

            if (_xma018.Count < 2) return;
            if (_xMainTrend.Count < 1) return;

            if (_xma018.LastItemCompletedDT.CompareTo(_lastDT) <= 0) return;
            _lastDT = _xma018.LastItemCompletedDT;

            if (ExitInEmergencyWhenStopUnFilled()) return;

            //TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.PROGRAMMING, Name, "Main",
            //                                    _xma018.LastItemCompleted.ToString(), _xma018.ToString());

            if (_waitFlat && _xTrend.IsFlat) _waitFlat = false;

            _xProfit = Position.Profit;

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

            // ***************************** Short Exit Long Entry ***********************
            if (Position.IsShort)
            {
                // Reverse
                if (Mode1 == 2)
                {
                    if (_xTrend.IsFlat2 && _xTrend.IsDown && _swingCount >= _swingCountShort)
                    {
                        _currentLimitPrice = _xma018.GetPrice(+1, 21, out _comment);
                        _comment = "Reverse in Range. " + _comment;

                        if (_currentLimitPrice.Equals(0.0)) return;

                        if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                        _lastLimitBuyPrice = _currentLimitPrice;

                        SetOrder(OrderOperationEnum.Buy);
                        return;
                    }
                }
                if (_xTrend.IsUp2 && _xTrend.IsUp)
                {
                    _currentLimitPrice = _xma018.GetPrice(+1, EntryMode, out _comment);
                    
                    if (_currentLimitPrice.Equals(0.0)) return;

                    if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitBuyPrice = _currentLimitPrice;

                    if (_xTrend.Ma.CompareTo(_xTrend.High3) > 0)
                    {
                        _comment = "Reverse to UpTrend. " + _comment;
                        SetOrder(OrderOperationEnum.Buy);
                    }
                    else
                    {
                        _comment = "Short Exit. " + _comment;
                        SetOrder2(OrderOperationEnum.Buy, 1);
                    }
                    return;
                }

                // ********************* Short Exit **********************************
                // Exit After Trend
                if (_xTrend.IsDown2 && _xTrend.IsUp && _swingCount >= _swingCountLong
                    && _xTrend.High.CompareTo(_xTrend.High2 - Ma1KAtr * _kAtrZone1 * _xAtr.LastAtrCompleted) < 0
                    && _xTrend.High.CompareTo(_xTrend.Low2 + Ma1KAtr * _kAtrStopZone2 * _xAtr.LastAtrCompleted) > 0)
                {
                    _currentLimitPrice = _xma018.GetPrice(+1, EntryMode, out _comment);
                    _comment = "Short Trend Exit. " + _comment;

                    if (_currentLimitPrice.Equals(0.0)) return;

                    if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitBuyPrice = _currentLimitPrice;

                //    if (_xTrend.High2.CompareTo(_xTrend.High3) > 0)
                //        SetOrder(OperationEnum.Buy);
                 //   else
                    SetOrder2(OrderOperationEnum.Buy, 1);
                    return;
                }
                // Exit
                /*
                if (_xTrend.Ma.CompareTo(_xTrend.High2) > 0 && _swingCount >= _swingCountLong * 3)
                {
                    _currentLimitPrice = _xma018.GetPrice(+1, 1, out _comment);
                    _comment = "Short Exit. " + _comment;

                    if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitBuyPrice = _currentLimitPrice;

                    SetOrder2(OperationEnum.Buy,1);
                }
                */
                if (_xTrend.IsDown && _xProfit.CompareTo((float)(_xAtr.LastAtrCompleted * _kProfitAtr1)) > 0)
                {
                    _currentLimitPrice = _xma018.GetPrice(+1, EntryMode, out _comment);
                    _comment = "Short Profit Exit. " + _comment;

                    if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitBuyPrice = _currentLimitPrice;

                    SetOrder2(OrderOperationEnum.Buy, 1);
                }
            }
            // ******************************** Long Exit Short Entry *****************
            else if (Position.IsLong)
            {
                if (Mode1 == 2)
                {
                    if (_xTrend.IsFlat2 && _xTrend.IsUp && _swingCount >= _swingCountLong)
                    {
                        _currentLimitPrice = _xma018.GetPrice(-1, 21, out _comment);
                        _comment = "Reverse in Range. " + _comment;

                        if (_currentLimitPrice.Equals(0.0)) return;

                        if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                        _lastLimitSellPrice = _currentLimitPrice;

                        SetOrder(OrderOperationEnum.Sell);
                        return;
                    }
                }
                if (_xTrend.IsDown2 && _xTrend.IsDown)
                {
                    _currentLimitPrice = _xma018.GetPrice(-1, EntryMode, out _comment);
                    
                    if (_currentLimitPrice.Equals(0.0)) return;

                    if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitSellPrice = _currentLimitPrice;

                    if (_xTrend.Ma.CompareTo(_xTrend.Low3) < 0)
                    {
                        _comment = "Reverse to DownTrend. " + _comment;
                        SetOrder(OrderOperationEnum.Sell);
                    }
                    else
                    {
                        _comment = "Long Exit. " + _comment;
                        SetOrder2(OrderOperationEnum.Sell, 1);
                    }
                    return;
                }

                // *****************************  Long Exit ********************************
                if (_xTrend.IsUp2 && _xTrend.IsDown && _swingCount >= _swingCountLong
                    && _xTrend.Low.CompareTo(_xTrend.Low2 + Ma1KAtr * _kAtrZone1 * _xAtr.LastAtrCompleted) > 0
                    && _xTrend.Low.CompareTo(_xTrend.High2 - Ma1KAtr * _kAtrStopZone2 * _xAtr.LastAtrCompleted) < 0)
                {
                    _currentLimitPrice = _xma018.GetPrice(-1, EntryMode, out _comment);
                    _comment = "Long Trend Exit. " + _comment;

                    if (_currentLimitPrice.Equals(0.0)) return;

                    if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitSellPrice = _currentLimitPrice;

                  //  if (_xTrend.Low2.CompareTo(_xTrend.Low3) < 0)
                  //      SetOrder(OperationEnum.Sell);
                  //  else
                    SetOrder2(OrderOperationEnum.Sell, 1);
                    return;
                }
                /*
                // Exit Only
                if (_xTrend.Ma.CompareTo(_xTrend.Low2) < 0 && _swingCount >= _swingCountLong * 3)
                {
                    _currentLimitPrice = _xTrend.GetPrice(-1, 1, out _comment);
                    _comment = "Exit. " + _comment;

                    if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitSellPrice = _currentLimitPrice;

                    SetOrder2(OperationEnum.Sell,1);
                 }
                */
                if (_xTrend.IsUp && _xProfit.CompareTo((float)(_xAtr.LastAtrCompleted * _kProfitAtr1)) > 0)
                {
                    _currentLimitPrice = _xTrend.GetPrice(-1, EntryMode, out _comment);
                    _comment = "Long Profit Exit. " + _comment;

                    if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitSellPrice = _currentLimitPrice;

                    SetOrder2(OrderOperationEnum.Sell, 1);
                }
            }
            else // Position == 0
            {
                /*
                    if (_xTrend.IsFlat2 && _xTrend.IsDown)
                    {
                        _currentLimitPrice = _xma018.GetPrice(+1, 21, out _comment);
                        _comment = "Long Entry Range. " + _comment;

                        if (_currentLimitPrice.Equals(0.0)) return;

                        if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                        _lastLimitBuyPrice = _currentLimitPrice;

                        SetOrder(OperationEnum.Buy);
                        return;
                    }
                    if (_xTrend.IsFlat2 && _xTrend.IsUp)
                    {
                        _currentLimitPrice = _xma018.GetPrice(-1, 21, out _comment);
                        _comment = "Short Entry Range. " + _comment;

                        if (_currentLimitPrice.Equals(0.0)) return;

                        if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                        _lastLimitSellPrice = _currentLimitPrice;

                        SetOrder(OperationEnum.Sell);
                        return;
                    }
                */
                if (_xTrend.IsUp2 && _xTrend.IsUp && _xTrend.Ma.CompareTo(_xTrend.High3) > 0)
                {
                    _currentLimitPrice = _xma018.GetPrice(+1, EntryMode, out _comment);
                    _comment = "Long Entry. Break High. " + _comment;

                    if (_currentLimitPrice.Equals(0.0)) return;

                    if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitBuyPrice = _currentLimitPrice;

                    SetOrder2(OrderOperationEnum.Buy, 1);

                    return;
                }
                if (_xTrend.IsDown2 && _xTrend.IsDown && _xTrend.Ma.CompareTo(_xTrend.Low3) < 0)
                {
                    _currentLimitPrice = _xma018.GetPrice(-1, EntryMode, out _comment);
                    _comment = "Short Entry. Break Low. " + _comment;

                    if (_currentLimitPrice.Equals(0.0)) return;

                    if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitSellPrice = _currentLimitPrice;

                    SetOrder2(OrderOperationEnum.Sell, 1);

                    return;
                }
                
                if (_xTrend.IsUp && _xTrend.IsFlat2)
                {
                    _currentLimitPrice = _xma018.GetPrice(+1, EntryMode, out _comment);
                    _comment = "Long Entry. " + _comment;

                    if (_currentLimitPrice.Equals(0.0)) return;

                    if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitBuyPrice = _currentLimitPrice;

                    SetOrder(OrderOperationEnum.Buy);
                }
                else if (_xTrend.IsDown && _xTrend.IsFlat2)
                {
                    _currentLimitPrice = _xma018.GetPrice(-1, EntryMode, out _comment);
                    _comment = "Short Entry. " + _comment;

                    if (_currentLimitPrice.Equals(0.0)) return;

                    if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitSellPrice = _currentLimitPrice;

                    SetOrder(OrderOperationEnum.Sell);
                }
                
                // Long Entry After UpTrend Exit
                if (_xTrend.IsUp2 && _xTrend.IsDown &&
                    ( 
                    (_xTrend.Low.CompareTo(_xTrend.Low2 + _kAtrZone1 * _xAtr.LastAtrCompleted) <= 0) ||
                    ( _xTrend.High3.CompareTo((_xTrend.High2 + _xTrend.Low2)/2) <= 0 &&  _xTrend.Low.CompareTo(_xTrend.High3) <= 0)
                    )
                   )  //  First was <=
                {
                    var v = _xTrend.Low2 + _kAtrZone1*_xAtr.LastAtrCompleted;
                    _currentLimitPrice = _xma018.GetPrice(+1, 0, out _comment);
                    _comment = "Long Entry After UpTrend Exit. L=" + _xTrend.Low + " < L2="+ v.ToString("N2") + " " + _comment;

                    if (_currentLimitPrice.Equals(0.0)) return;

                    if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitBuyPrice = _currentLimitPrice;

                    SetOrder(OrderOperationEnum.Buy);
                    return;
                }
                // Short Entry After DownTrend Exit
                if (_xTrend.IsDown2 && _xTrend.IsUp &&
                    (
                    ( _xTrend.High.CompareTo(_xTrend.High2 - _kAtrZone1 * _xAtr.LastAtrCompleted) >= 0) ||
                    ( _xTrend.Low3.CompareTo((_xTrend.High2 + _xTrend.Low2)/2) >= 0 && _xTrend.High.CompareTo(_xTrend.Low3) >= 0)
                    )
                    )
                {
                    var v = _xTrend.High2 - _kAtrZone1 * _xAtr.LastAtrCompleted;
                    _currentLimitPrice = _xma018.GetPrice(-1, 0, out _comment);
                    //_comment = "Short Entry After DownTrend Exit. " + _comment;
                    _comment = "Short Entry After DownTrend Exit. H=" + _xTrend.High + " > H2="+ v.ToString("N2") + " " + _comment;

                    if (_currentLimitPrice.Equals(0.0)) return;

                    if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitSellPrice = _currentLimitPrice;

                    SetOrder(OrderOperationEnum.Sell);
                    return;
                }
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

            var stopprice = _currentLimitPrice + stopDirection * KAtrStop * _xBandAtr / Ma1KAtr;

            stopprice = Ticker.ToMinMove(stopprice, stopDirection);

            TradeTerminal.SetStopLimitOrder(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
                                            flipOperation,
                                            stopprice, stopprice, 1,
                                            DateTime.Today.Add(new TimeSpan(23, 49, 15)), "");

            TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.TRADING, Code, Code, "New " + operation,
                                          "Price=" + _currentLimitPrice, "");
        }
        private void SetOrder2(OrderOperationEnum operation, int contract)
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

            //var contract = Position.Pos == 0 ? 1 : 2;

            TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                        OrderTypeEnum.Limit, OrderOperationEnum.All);

            TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                        OrderTypeEnum.StopLimit, flipOperation);

            TradeTerminal.SetLimitOrder(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
                                        operation,
                                        _currentLimitPrice, contract,
                                        DateTime.Today.Add(new TimeSpan(23, 49, 15)),
                                        _comment);

            /*
            var stopprice = _currentLimitPrice + stopDirection * KAtrStop * _xBandAtr / Ma1KAtr;

            stopprice = Ticker.ToMinMove(stopprice, stopDirection);

            TradeTerminal.SetStopLimitOrder(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
                                            flipOperation,
                                            stopprice, stopprice, contract,
                                            DateTime.Today.Add(new TimeSpan(23, 49, 15)), "");
            */
            TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.TRADING, Code, Code, "Close: " + Position.OperationString,
                                          "Price=" + _currentLimitPrice, "");
        }

        public override void Finish()
        {
            CloseAll();
            TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Name, "Finish",
                                         ToString(), "");
        }
        public void AskRandom()
        {
            if (_randCount <= 0) return;
            _randCount = 0;

            _randValue = _random.Next(1, 101);
            _randAnswer = _randValue > RandMode ? true : false;

            TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, Code, Code, "Ask Random", "Value=" + _randValue + ", Answer=" + _randAnswer, "");
        }
        //public override IBandSeries Band { get { return _xTrend; } }
        // public override IBandSeries Band2 { get { return XMainTrend; } }
        protected override void PositionIsChangedEventHandler2(IPosition2 oldpos, IPosition2 newpos, PositionChangedEnum changedResult)
        {
            throw new NotImplementedException();
        }

        public override ILevelCollection Levels { get { return _xTrend; } }
        public override ILevelCollection Levels2 { get { return _xMainTrend; } }

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