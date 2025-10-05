using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GS.ICharts;
using GS.Interfaces;
using GS.Trade.Data;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Studies;
using GS.Trade.Data.Studies.Averages;
using GS.Trade.Data.Studies.GS.xMa018;
using GS.Trade.Interfaces;

namespace GS.Trade.Strategies
{
    public partial class X117 : Strategy
    {
        [XmlIgnore]
        public override IBars Bars { get; protected set; }
        [XmlIgnore]
        public override int MaxBarsBack { get; protected set; }
        [XmlIgnore]
        public override Atr Atr { get { return _xAtr; } }

        private Xma018 _xma018;
        protected Xma018 XTrend;
        private Xma018 _xMainTrend;

        private XAverage _xAvrg;

        private Atr _xAtr;
        private Atr _xAtr2;

        private int _xMainTrendValue;

        //  private BarSeries _bars;

        public uint TimeInt2 { get; set; }

        public int Ma1Length { get; set; }
        public int Ma1AtrLength { get; set; }
        public float Ma1KAtr { get; set; }
        public int Ma1Mode { get; set; }

        public int MaAtrLength1 { get; set; }
        public int MaAtrLength2 { get; set; }
        public int AtrMode { get; set; }

        public int Ma2Length { get; set; }
        public int Ma2AtrLength { get; set; }
        public float Ma2KAtr { get; set; }
        public int Ma2Mode { get; set; }

        public float KAtrStop { get; set; }
        public float KAtrStopZone1 { get; set; }
        public float KAtrStopZone2 { get; set; }

        public int SwingCount { get; set; }

        public int SwingCountEntry { get; set; }
        public int SwingCountReverse { get; set; }
        public int SwingCountExit { get; set; }

        public int EntryId { get; set; }
        public int ReverseId { get; set; }
        public int ExitId { get; set; }

        public int EntryPriceId { get; set; }
        public int ReversePriceId { get; set; }
        public int ExitPriceId { get; set; }

        public int EntryMode { get; set; }
        public int RandMode { get; set; }

        public int Mode1 { get; set; }
        public int Mode2 { get; set; }

        public int SignalIndex { get; set; }

        private DateTime _lastDT = DateTime.MinValue;

        private int _trend;
        private bool _trendWasChanged;

        private long _tradeNumber;
        private int _position;

        protected int MyEntryId;
        protected int MyReverseId;
        protected int MyExitId;

        private double _currentLimitPrice;
        private double _lastLimitBuyPrice;
        private double _lastLimitSellPrice;
        private double _lastStopPrice;

        private string _comment;

        // private float _kAtrStop = 1.5f;

        private int _swingCountEntry;
        private int _swingCountReverse;
        private int _swingCountExit;

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
        private Bars _bars;

        protected float VolatilityUnit
        {
            get { return (float)( Ma1KAtr * (_xAtr == null ? 0f : _xAtr.LastAtrCompleted)); }
        }
        protected float StopValue
        {
            get { return KAtrStop * VolatilityUnit; }
        }
        protected float VolatilityUnit2
        {
            get { return (float) (XTrend.High - XTrend.Low)/2f; }
        }
        protected float StopValue2
        {
            get { return KAtrStop * VolatilityUnit2; }
        }

        public X117()
        {
            // need for Serialization

        }
        public X117(ITradeContext tx, string name, string code, Ticker ticker, uint timeInt)
            : base(tx, name, code, ticker, timeInt)
        {
        }
        public override void Init()
        {
            base.Init(); if (IsWrong) return;
            Position.PositionChangedEvent += PositionIsChangedEventHandler;

            _xma018 = TradeContext.RegisterTimeSeries(
                new Xma018("Xma18", Ticker, (int)TimeInt, Ma1Length, Ma1AtrLength, Ma1KAtr, Ma1Mode)) as Xma018;

            XTrend = _xma018;

            _xAtr = XTrend.Atr;
            _xAtr2 = (Atr)TradeContext.RegisterTimeSeries(new Atr("Atr2", Ticker, (int)TimeInt, 300));
            MaxBarsBack = _xAtr.Length;

            _xMainTrend = _xma018;

            if (_xma018 != null)
            {
                _bars = (Bars)(_xma018.SyncSeries);
                Bars = _bars;
            }

            MyReverseId = ReverseId;
            MyExitId = ExitId;
            MyEntryId = EntryId;

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
            {
                _swingCountExit = 0;
                _swingCountReverse = 0;
                _swingCountEntry = 0;

                MyReverseId = ReverseId;
                MyExitId = ExitId;
                MyEntryId = EntryId;
            }
            if (oldposition != 0 && newposition == 0)
            {
                _waitFlat = true;
                // _swingCount = 0;
            }
            if (newposition == 0)
            {
                _swingCountEntry = 0;

                MyReverseId = ReverseId;
                MyExitId = ExitId;
                MyEntryId = EntryId;
            }

            SetStopOrderFilledStatus(false);
        }

        public override void Main()
        {
           // if (TimePlan.IsTimeToRest) return;

            if (_xma018.Count < 2) return;
            if (_xMainTrend.Count < 1) return;

            if (_xma018.LastItemCompletedDT.CompareTo(_lastDT) <= 0) return;
            _lastDT = _xma018.LastItemCompletedDT;

            if (ExitInEmergencyWhenStopUnFilled()) return;

            if (_waitFlat && XTrend.IsFlat) _waitFlat = false;

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

                _swingCountEntry++;
                _swingCountReverse++;
                _swingCountExit++;

                _randCount++;

                // if (RandMode > 0) AskRandom();             
            }
            //   if (XMainTrend.TrendChanged) _randCount++;

            var contract = Position.Pos == 0 ? 1 : 2;

           
            // ***************************** Short Exit Long Entry ***********************
            if (Position.IsShort)
            {
                PreShort();
                // ******************** Reverse **************************
                #region Reverse

                PreShortReverse();

                if (XTrend.TakeLong(MyReverseId)
                    && _swingCountReverse >= SwingCountReverse )
                {
                    _currentLimitPrice = _xma018.GetPrice(+1, ReversePriceId, out _comment);
                    _comment = "Reverse to Long:[" + MyReverseId + "] " + _comment;

                    if (_currentLimitPrice.Equals(0.0)) return;

                    if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitBuyPrice = _currentLimitPrice;

                    SetOrder2(OrderOperationEnum.Buy, 2);
                    return;
                }

                NoShortReverse();

                #endregion

                // ********************* Short Exit **********************************
                #region Short Exit
                #region Exit

                PreShortExit();

                if (/*_xTrend.High2.CompareTo((double)Position.Price1) > 0 && */
                    XTrend.TakeLong(MyExitId) // && _xTrend.Ma.CompareTo(_xTrend.High2) > 0 
                    && _swingCountExit >= SwingCountExit)
                {
                    _currentLimitPrice = _xma018.GetPrice(+1, ExitPriceId, out _comment);
                    _comment = "Short Exit:[" + MyExitId + "] " + _comment;

                    if (_currentLimitPrice.Equals(0.0)) return;

                    if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitBuyPrice = _currentLimitPrice;

                    SetOrder2(OrderOperationEnum.Buy, 1);
                    return;
                }

                if( XTrend.Ma > (double) (Position.Price1 + (decimal) StopValue2))
                {
                    _currentLimitPrice = _xma018.GetPrice(+1, 1, out _comment);
                    SetOrder2(OrderOperationEnum.Buy, 1);
                    return;
                }

                #endregion
                #region TakeProfit
                #endregion
                #endregion
            }
            // ******************************** Long Exit Short Entry *****************
            else if (Position.IsLong)
            {
                PreLong();
                // ************************** Reverse *******************************
                #region Reverse

                PreLongReverse();

                if (XTrend.TakeShort(MyReverseId) && _swingCountReverse >= SwingCountReverse)
                {
                    _currentLimitPrice = _xma018.GetPrice(-1, ReversePriceId, out _comment);
                    _comment = "Reverse To Short:[" + MyReverseId + "] " + _comment;

                    if (_currentLimitPrice.Equals(0.0)) return;

                    if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitSellPrice = _currentLimitPrice;

                    SetOrder2(OrderOperationEnum.Sell, 2);
                    return;
                }

                NoLongReverse();

                #endregion
                // *****************************  Long Exit ********************************
                #region Long Exit

                PreLongExit();

                if (/*_xTrend.Low2.CompareTo((double)Position.Price1) < 0 && */
                    XTrend.TakeShort(MyExitId)  // && _xTrend.Ma.CompareTo(_xTrend.Low2) < 0
                    && _swingCountExit >= SwingCountExit)
                {
                    _currentLimitPrice = _xma018.GetPrice(-1, ExitPriceId, out _comment);
                    _comment = "Long Exit:[" + MyExitId + "] " + _comment;

                    if (_currentLimitPrice.Equals(0.0)) return;

                    if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitSellPrice = _currentLimitPrice;

                    SetOrder2(OrderOperationEnum.Sell, 1);
                    return;
                }

                if (XTrend.Ma < (double)(Position.Price1 - (decimal)StopValue2))
                {
                    _currentLimitPrice = _xma018.GetPrice(-1, 1, out _comment);
                    SetOrder2(OrderOperationEnum.Sell, 1);
                    return;
                }

                #endregion

                #region TakeProfit
                #endregion
            }
            // ****************************  Position is Newtral *******************************
            #region Postion is Neutral
            else // Position is Neutral 10000000100000101
            {
                PreShortEntry();
                if (XTrend.TakeShort(MyEntryId)
                    && _swingCountEntry >= SwingCountEntry
                    && XTrend.SwingCount >= SwingCount )
                {
                    _currentLimitPrice = _xma018.GetPrice(-1, EntryPriceId, out _comment);
                    _comment = "Short Entry:[" + MyEntryId + "] " + _comment;

                    if (_currentLimitPrice.Equals(0.0)) return;

                    if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitSellPrice = _currentLimitPrice;

                    SetOrder2(OrderOperationEnum.Sell, 1);
                    return;
                }
                NoShortEntry();

                PreLongEntry();
                if (XTrend.TakeLong(MyEntryId)
                    && _swingCountEntry >= SwingCountEntry
                    && XTrend.SwingCount >= SwingCount )
                {
                    _currentLimitPrice = _xma018.GetPrice(+1, EntryPriceId, out _comment);
                    _comment = "Long Entry:[" + MyEntryId + "] " + _comment;

                    if (_currentLimitPrice.Equals(0.0)) return;

                    if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitBuyPrice = _currentLimitPrice;

                    SetOrder2(OrderOperationEnum.Buy, 1);
                    return;
                }

                NoLongEntry();
            }
            #endregion
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
        private void SetOrder2(OrderOperationEnum operation, long contract)
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


            TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.TRADING, Code, Code, "Close: " + Position.OperationString,
                                          "Price=" + _currentLimitPrice, "");
        }
        protected virtual void PreShort()
        {
        }

        protected virtual void PreShortEntry()
        {
        }
        protected virtual void NoShortEntry()
        {
        }
        protected virtual void PreShortReverse()
        {
        }
        protected virtual void NoShortReverse()
        {
        }
        protected virtual void PreShortExit()
        {
        }
        protected virtual void PreLong()
        {
        }
        protected virtual void PreLongEntry()
        {
        }
        protected virtual void NoLongEntry()
        {
        }
        protected virtual void PreLongReverse()
        {
        }
        protected virtual void NoLongReverse()
        {
        }

        protected virtual void PreLongExit()
        {
        }

        public void AskRandom()
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
