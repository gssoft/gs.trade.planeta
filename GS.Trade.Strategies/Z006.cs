using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GS.Interfaces;
//using GS.Time.TimePlan;
using GS.Trade.Data;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Studies;
using GS.Trade.Data.Studies.Averages;
using GS.Trade.Data.Studies.GS.xMa018;
using GS.Trade.Interfaces;
using GS.Trade.Strategies.PositionMM;
using GS.Triggers;

namespace GS.Trade.Strategies
{
    public partial class Z006 : Strategy
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
        private Atr _xAtr1;
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

        public float KAtrStop { get; set; }
        public float KAtrStop1 { get; set; }
        public float KAtrStop2 { get; set; }

        public int SwingCount { get; set; }

        public int SwingCountStartEntry { get; set; }
        public int SwingCountEntry { get; set; }
        public int SwingCountReverse { get; set; }
        public int SwingCountExit { get; set; }

        public int KAtrHookOrder { get; set; }

        [XmlIgnore]
        protected int LastSignalId;

        public int StartEntryMode { get; set; }
        public int EntryMode { get; set; }

        public int EntrySignal1 { get; set; }
        public int EntrySignal2 { get; set; }

        public int ExitSignal1 { get; set; }
        public int ExitSignal2 { get; set; }

        public int ReverseSignal1 { get; set; }
        public int ReverseSignal2 { get; set; }

        public int ReverseLossSignal { get; set; }

        public int EntryPriceId { get; set; }
        public int ReversePriceId { get; set; }
        public int ExitPriceId { get; set; }

        public int StopExitSignal1 { get; set; }
        public int StopPriceId { get; set; }

        public int RandMode { get; set; }

        private DateTime _lastDT = DateTime.MinValue;

        private int _trend;
        private bool _trendWasChanged;

        private long _tradeNumber;
        private int _position;

        protected int MyEntrySignal1;
        protected int MyReverseSignal1;
        protected int MyExitSignal1;

        protected double _currentLimitPrice;
        private double _lastLimitBuyPrice;
        private double _lastLimitSellPrice;
        private double _lastStopPrice;

        protected int LastExitMode;

        private string _comment;

        // private float _kAtrStop = 1.5f;

        protected int _swingCountEntry;
        protected int _swingCountReverse;
        protected int _swingCountExit;

        protected const int KMinFarFromHere = 3;

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

        protected Trigger TrMaxContracts;

        [XmlIgnore]
        protected Trigger TrEntryEnabled;

        protected float VolatilityUnit
        {
            get { return (float)(Ma1KAtr * (_xAtr == null ? 0f : _xAtr.LastAtrCompleted)); }
        }
        protected float StopValue
        {
            get { return KAtrStop * VolatilityUnit; }
        }
        public override float Volatility
        {
            get { return Ma1KAtr * VolatilityUnit2; }
        }
        protected float VolatilityUnit2
        {
            get { return (float)(XTrend.High - XTrend.Low) / 2f; }
        }
        protected float StopValue2
        {
            get { return KAtrStop * VolatilityUnit2; }
        }
        protected float StopValue1
        {
            get { return KAtrStop1 * VolatilityUnit2; }
        }

        protected float OrderHookValue
        {
            get { return KAtrHookOrder * VolatilityUnit2; }
        }

        protected bool IsPositionRiskLow
        {
            get
            {
                return (
                           (Position.IsLong && PositionMax.IsValid && PositionMax.IsGreaterThan((float)XTrend.High2))
                           ||
                           (Position.IsShort && PositionMin.IsValid && PositionMin.IsLessThan((float)XTrend.Low2))
                       );
            }
        }
        protected bool IsLongTermBreakUp
        {
            get
            {
                double high;
                return !XTrend.HaveHigher((int)TimeInt * 60 * KMinFarFromHere, XTrend.Ma, out high);
            }
        }
        protected bool IsLongTermBreakDown
        {
            get
            {
                double low;
                return !XTrend.HaveLower((int)TimeInt * 60 * KMinFarFromHere, XTrend.Ma, out low);
            }
        }
        public override string PositionInfo
        {
            get
            {
                return String.Format("PosMax: {0:N2} PosMed: {1:N2}  PosMin: {2:N2} PosStop: {3:N2}",
                    PositionMax.GetValidValue, PositionMedian, PositionMin.GetValidValue,
                     PositionStop.GetValidValue);
            }
        }
        protected PositionMinMax PositionStop = new PositionMinMax();

        protected PositionMin PositionMin = new PositionMin();
        protected PositionMax PositionMax = new PositionMax();

        public float PositionMedian
        {
            get
            {
                return PositionMax.IsValid && PositionMin.IsValid
                          ? 0.5f * (PositionMax.GetValidValue + PositionMin.GetValidValue)
                          : 0f;
            }
        }

        protected float PositionMin2;
        protected float PositionMax2;


        //protected bool PositionStopValid
        //{
        //    get { return PositionStop.CompareTo(Single.MaxValue) != 0; }
        //}

        protected int Mode;
        protected int ModeSafe;

        public int ReverseLossCnt;
        protected int RealReverseLossCnt;

        public int RichTarget { get; set; }

        public Z006()
        {
            // need for Serialization

        }
        public Z006(ITradeContext tx, string name, string code, Ticker ticker, uint timeInt)
            : base(tx, name, code, ticker, timeInt)
        {
        }
        public override void Init()
        {
            base.Init(); if (IsWrong) return;
            // Position.PositionChangedEvent += PositionIsChangedEventHandler;
            //Position2.PositionChangedEvent3 += PositionIsChangedEventHandler2;
            try
            {
                _xma018 = TradeContext.RegisterTimeSeries(
                    new Xma018("Xma18", Ticker, (int)TimeInt, Ma1Length, Ma1AtrLength, Ma1KAtr, Ma1Mode)) as Xma018;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            if (_xma018 == null)
                throw new Exception(StrategyTickerString + ": Xma016 Initialization Error");

            XTrend = _xma018;

            //_xAtr = XTrend.Atr;
            //_xAtr2 = (Atr)TradeContext.RegisterTimeSeries(new Atr("Atr2", Ticker, (int)TimeInt, 500));

            _xAtr1 = XTrend.Atr1;
            _xAtr2 = XTrend.Atr2;
            _xAtr = XTrend.Atr;

            MaxBarsBack = _xAtr.Length;

            _xMainTrend = _xma018;

            if (_xma018 != null)
            {
                _bars = (Bars)(_xma018.SyncSeries);
                Bars = _bars;
            }

            MyReverseSignal1 = ReverseSignal1;
            MyExitSignal1 = ExitSignal1;
            MyEntrySignal1 = EntrySignal1;

            TrMaxContracts = new Trigger();
            TrEntryEnabled = new Trigger();

            Mode = 1;


            TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, StrategyTickerString, StrategyTickerString, "Init " + Code, ToString(), "");
        }


        protected override void PositionIsChangedEventHandler2(IPosition2 oldposition, IPosition2 newposition, PositionChangedEnum changedResult)
        {
            CalcLastTradeOperation(oldposition, newposition, changedResult);

            if (oldposition.IsShort)
                TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                                    OrderTypeEnum.All, OrderOperationEnum.Buy);
            else if (oldposition.IsLong)
                TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                                    OrderTypeEnum.All, OrderOperationEnum.Sell);

            //     TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.TRADING, StrategyTickerString, "PositionChanged",
            //                                  "New: " + newposition.StatusString + "; Old: " + oldposition.StatusString, "");
            _swingCountEntry = 0;
            TrEntryEnabled.Reset();

            if (changedResult == PositionChangedEnum.Reversed ||     // reverse
                (changedResult == PositionChangedEnum.Opened)                 // open new from flat
                )
            {
                //  TrMaxContracts.Reset();
                TrEntryEnabled.Reset();

                _swingCountExit = 0;
                _swingCountReverse = 0;
                _swingCountEntry = 0;

                MyReverseSignal1 = ReverseSignal1;
                MyExitSignal1 = ExitSignal1;
                MyEntrySignal1 = EntrySignal1;

                PositionMin.Clear();
                PositionMax.Clear();
                PositionStop.Clear();

            }

            if (changedResult == PositionChangedEnum.Closed)
            {
                _swingCountEntry = 0;

                MyReverseSignal1 = ReverseSignal1;
                MyExitSignal1 = ExitSignal1;
                MyEntrySignal1 = EntrySignal1;

                TrMaxContracts.Reset();
                PositionStop.Clear();
            }
            if (changedResult == PositionChangedEnum.ReSizedUp ||
                changedResult == PositionChangedEnum.ReSizedDown)
            {
                _swingCountReverse = 0;
                _swingCountEntry = 0;
            }
            if (newposition.IsOpened && oldposition.IsNeutral)
            {
                _swingCountEntry = 0;
                TrMaxContracts.Reset();
                TrEntryEnabled.Reset();
            }

            SetStopOrderFilledStatus(false);

            PositionChanged(oldposition, newposition, changedResult);
        }

        public override void Main()
        {
            // if (TimePlan.IsTimeToRest) return;
            if (!TimePlan.Enabled) return;

            if (_xma018.Count < 2) return;
            if (_xMainTrend.Count < 1) return;

            //   HookOrder();

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

            if (Position.IsShort)
            {
                var pmax = PositionMax;
                var pmin = PositionMin;

                if (XTrend.IsDown)
                {
                    if (PositionMax.SetIfLessThan((float)XTrend.High2))
                            PositionStop.Value = PositionMax.GetValidValue;
                }
                else if (XTrend.IsUp)
                {
                    PositionMin.SetIfGreaterThan((float) XTrend.Low2);
                }


            }
            else if (Position.IsLong)
            {
                var pmax = PositionMax;
                var pmin = PositionMin;

                if (XTrend.IsUp)
                {
                    if (PositionMin.SetIfGreaterThan((float)XTrend.Low2))
                        PositionStop.Value = PositionMin.GetValidValue;
                }
                else if (XTrend.IsDown)
                {
                    PositionMax.SetIfLessThan((float) XTrend.High2);
                }
            }

            TrMaxContracts.Value = Position.Quantity >= Contracts;
            TrMaxContracts.Value = true;

            if (_swingCountEntry >= SwingCountEntry)
            {
                TrEntryEnabled.SetValue(TakeLong(19) || TakeShort(19));

                //TradeContext.Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, StrategyTickerString,
                //       "TrEntryEnabled", TrEntryEnabled.Value.ToString(), "swCntEntry=" + _swingCountEntry, "");
            }
            Prefix();



            // ***************************** Short Exit Long Entry ***********************

            if (Position.IsShort)
            {
                // ******************** Reverse **************************
                #region Reverse

                if (EntryEnabled && ShortReverse())
                {
                    if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitBuyPrice = _currentLimitPrice;

                    SetOrder2(OrderOperationEnum.Buy, Contract);
                    return;
                }

                #endregion

                // ********************* Short Exit **********************************
                #region Short Exit

                //if (ShortExit())
                //{
                //    if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                //    _lastLimitBuyPrice = _currentLimitPrice;

                //    SetOrder2(OperationEnum.Buy, Position.Quantity);
                //    return;
                //}

                #region StopExit

                //if (ShortStopExit())
                //{
                //    SetOrder2(OperationEnum.Buy, Position.Quantity);
                //    return;
                //}

                #endregion

                #region SignalExit

                int nContracts;
                if ((nContracts = ShortExit2()) > 0)
                {
                    if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitBuyPrice = _currentLimitPrice;

                    SetOrder2(OrderOperationEnum.Buy, nContracts);
                    return;
                }
                #endregion

                #region StopExit
                /*
                if (ShortStopExit())
                {
                    SetOrder2(OperationEnum.Buy, Position.Quantity);
                    return;
                }
                */
                #endregion

                #region TakeProfit
                #endregion
                #endregion
            }
            // ******************************** Long Exit Short Entry *****************
            else if (Position.IsLong)
            {
                // ************************** Reverse *******************************
                #region Reverse

                if (EntryEnabled && LongReverse())
                {
                    if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitSellPrice = _currentLimitPrice;

                    SetOrder2(OrderOperationEnum.Sell, Contract);
                    return;
                }

                #endregion
                // *****************************  Long Exit ********************************
                #region Long Exit

                //if (LongExit())
                //{
                //    if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                //    _lastLimitSellPrice = _currentLimitPrice;

                //    SetOrder2(OperationEnum.Sell, Position.Quantity);
                //    return;
                //}

                //if (LongStopExit())
                //{
                //    SetOrder2(OperationEnum.Sell, Position.Quantity);
                //    return;
                //}

                #endregion

                #region TakeProfit
                #endregion

                #region Long Exit2

                int nContracts;
                if ((nContracts = LongExit2()) > 0)
                {
                    if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitSellPrice = _currentLimitPrice;

                    SetOrder2(OrderOperationEnum.Sell, nContracts);
                    return;
                }

                /*
                if (LongStopExit())
                {
                    SetOrder2(OperationEnum.Sell, Position.Quantity);
                    return;
                }
                */

                #endregion


            }
            // ****************************  Position is Newtral *******************************
            #region Postion is Neutral or Contracts is Not Enought

            if (!EntryEnabled) return;

            //  if (Position.IsNeutral || (Position.IsOpened && Position.Quantity < Contracts))
            //   {
            var oper = Entry();
            if (oper < 0)
            {
                if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                _lastLimitSellPrice = _currentLimitPrice;

                SetOrder2(OrderOperationEnum.Sell, Contract);
                return;
            }
            if (oper > 0)
            {
                if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                _lastLimitBuyPrice = _currentLimitPrice;

                SetOrder2(OrderOperationEnum.Buy, Contract);
                return;
            }
            //   }

            //if (Position.IsNeutral || Position.IsLong ||
            //    (Position.IsShort && Position.Quantity < Contracts)
            //    )
            //{
            //    if (ShortEntry())
            //    {
            //        if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
            //        _lastLimitSellPrice = _currentLimitPrice;

            //        SetOrder2(OperationEnum.Sell, Contract);
            //        return;
            //    }
            //}
            //if (Position.IsNeutral || Position.IsShort ||
            //    (Position.IsLong && Position.Quantity < Contracts)
            //    )
            //{
            //    if (LongEntry())
            //    {
            //        if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
            //        _lastLimitBuyPrice = _currentLimitPrice;

            //        SetOrder2(OperationEnum.Buy, Contract);
            //        return;
            //    }
            //}

            /*
            else
            {
                if (StartShortEntry())
                {
                    if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitSellPrice = _currentLimitPrice;

                    SetOrder2(OperationEnum.Sell, Contract);
                    return;
                }

                if (StartLongEntry())
                {
                    if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitBuyPrice = _currentLimitPrice;

                    SetOrder2(OperationEnum.Buy, Contract);
                    return;
                }
            }
            */
            #endregion
        }
        protected void SetOrder2(OrderOperationEnum operation, long contract)
        {
            if (_currentLimitPrice.CompareTo(0d) == 0) return;

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

            /*
            TradeTerminal.KillAllOrders(Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                        OrderTypeEnum.Limit, OperationEnum.All);

            TradeTerminal.KillAllOrders(Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                        OrderTypeEnum.StopLimit, flipOperation);
            */
            KillAllActiveOrders2();

            if (string.IsNullOrWhiteSpace(Ticker.FormatF))
                throw new NullReferenceException(Ticker.Code + ": Ticker Format is Null");

            var priceStr = _currentLimitPrice.ToString(Ticker.FormatF);

            //TradeTerminal.SetLimitOrder(this, TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
            //                            operation,
            //                            _currentLimitPrice, priceStr, contract,
            //                            DateTime.Today.Add(new TimeSpan(23, 49, 15)),
            //                            _comment);

            var o = ActiveOrderCollection.RegisterOrder(this, operation, OrderTypeEnum.Limit, 0, _currentLimitPrice, contract, "");
            TradeTerminal.SetLimitOrder(o);


            //   TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.TRADING, Code, "Close: " + Position.OperationString,
            //                                 "Price=" + _currentLimitPrice, "");
        }
        protected void SetOrder3(OrderOperationEnum operation, long contract)
        {
            if (_currentLimitPrice.CompareTo(0d) == 0) return;

            if (string.IsNullOrWhiteSpace(Ticker.FormatF))
                throw new NullReferenceException(Ticker.Code + ": Ticker Format is Null");

            var priceStr = _currentLimitPrice.ToString(Ticker.FormatF);

            TradeTerminal.SetLimitOrder(this, TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
                                        operation,
                                        _currentLimitPrice, priceStr, contract,
                                        DateTime.Today.Add(new TimeSpan(23, 49, 15)),
                                        _comment);


            //   TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.TRADING, Code, "Close: " + Position.OperationString,
            //                                 "Price=" + _currentLimitPrice, "");
        }

      


        protected virtual void Prefix()
        {
        }
        protected virtual int Entry()
        {
            return 0;
        }
        protected virtual bool StartShortEntry()
        {
            return false;
        }
        protected virtual bool StartLongEntry()
        {
            return false;
        }
        protected virtual bool ShortEntry()
        {
            return false;
        }
        protected virtual bool LongEntry()
        {
            return false;
        }

        protected virtual bool ShortReverse()
        {
            return false;
        }
        protected virtual bool LongReverse()
        {
            return false;
        }

        protected virtual bool ShortExit()
        {
            if (ExitSignal1 > 0 && XTrend.TakeLong(ExitSignal1) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Short Exit:[" + ExitSignal1 + "] " + _comment;
                    return true;
                }
            }
            if (PositionMax.IsValid && XTrend.Ma.CompareTo(PositionMax) > 0)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                    return true;
            }
            return false;
        }

        protected virtual int ShortExit2()
        {
            return 0;
        }

        protected virtual bool LongExit()
        {
            if (ExitSignal1 > 0 && XTrend.TakeShort(ExitSignal1) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Exit:[" + ExitSignal1 + "] " + _comment;
                    return true;
                }
            }
            if (PositionMin.IsValid && XTrend.Ma.CompareTo(PositionMin) < 0)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                    return true;
            }
            return false;
        }
        protected virtual int LongExit2()
        {
            return 0;
        }

        protected virtual bool ShortStopExit()
        {
            if (XTrend.Ma.CompareTo((double)(Position.Price1 + (decimal)StopValue2)) > 0)
            {
                _currentLimitPrice = _xma018.GetPrice3(+1, StopPriceId, out _comment);
                return true;
            }
            return false;
        }
        protected virtual bool LongStopExit()
        {
            if (XTrend.Ma.CompareTo((double)(Position.Price1 - (decimal)StopValue2)) < 0)
            {
                _currentLimitPrice = _xma018.GetPrice3(-1, StopPriceId, out _comment);
                return true;
            }
            return false;
        }

        protected bool TakeLong(int index)
        {
            if (index == 0) return false;

            switch (index)
            {
                case 1:
                    return true;
                case 1106:
                    if (XTrend.TakeLong(106) && ((double)PositionMin2).CompareTo(XTrend.High) > 0)
                        return true;
                    break;
                default: if (XTrend.TakeLong(index))
                        return true;
                    break;
            }
            return false;
        }
        protected bool TakeShort(int index)
        {
            if (index == 0) return false;

            switch (index)
            {
                case 1:
                    return true;

                case 1106:
                    if (XTrend.TakeShort(106) && ((double)PositionMax2).CompareTo(XTrend.Low) < 0)
                        return true;
                    break;

                default: if (XTrend.TakeShort(index))
                        return true;
                    break;

            }
            return false;
        }

        protected void HookOrder()
        {
            if (Position.IsLong)
            {
                if (OrderHookValue.CompareTo((float)(Ticker.Bid - XTrend.Ma)) <= 0)
                {
                    if (XTrend.GetPrice5(-1, 2, out _currentLimitPrice, out _comment))
                    {
                        _comment = "Hook LongExit:" + _comment;
                        SetOrder3(OrderOperationEnum.Sell, Position.Quantity);
                    }
                }
            }
            else if (Position.IsShort)
            {
                if (OrderHookValue.CompareTo((float)(-Ticker.Ask + XTrend.Ma)) <= 0)
                {
                    if (XTrend.GetPrice5(+1, 2, out _currentLimitPrice, out _comment))
                    {
                        _comment = "Hook ShortExit:" + _comment;
                        SetOrder3(OrderOperationEnum.Buy, Position.Quantity);
                    }
                }
            }
        }
        protected bool HaveHigher()
        {
            if (!XTrend.IsUp2) return false;
            var h = XTrend.IsUp ? XTrend.High : XTrend.High2;

            double high2;
            return XTrend.HaveHigher((int)TimeInt * 60 * KMinFarFromHere, h, out high2);
        }

        protected bool HaveLower()
        {
            if (!XTrend.IsDown2) return false;

            var l = XTrend.IsDown ? XTrend.Low : XTrend.Low2;
            double low2;
            return XTrend.HaveLower((int)TimeInt * 60 * KMinFarFromHere, l, out low2);
        }
        /*
        public override void TimePlanEventHandler(object sender, TimePlanEventArgs args)
        {
            var tpie = (TimePlanItemEvent)sender;
            switch(args.TimePlanItemCode.Trim().ToUpper())
            {
                case "MORNING":
                    switch (tpie.Category.Trim().ToUpper())
                    {
                        case "TOEND":
                            switch (tpie.Code.Trim().ToUpper())
                            {
                                case "15_SEC":
                                    SetExitMode(11, args.ToString());
                                    break;
                                case "1_MIN":
                                    SetExitMode(12, args.ToString());
                                    break;
                            }
                            break;
                        case "AFTER":
                            switch (tpie.Code.Trim().ToUpper())
                            {
                                case "0_SEC":
                                case "0_MIN":
                                    SetExitMode(0, args.ToString());
                                    break;
                            }
                            break;
                    }
                    break;
                case "EVENING":
                    switch (tpie.Category.Trim().ToUpper())
                    {
                        case "TOEND":
                            switch (tpie.Code.Trim().ToUpper())
                            {
                                case "15_SEC":
                                    break;
                                case "30_SEC":
                                    SetExitMode(1, args.ToString());
                                    break;
                                case "1_MIN":
                                    break;
                                case "2_MIN":
                                    SetExitMode(2, args.ToString());
                                    break;
                                case "3_MIN":
                                    break;
                                case "5_MIN":
                                    SetExitMode(3, args.ToString());
                                    break;
                            }
                            break;
                        case "AFTER":
                            switch (tpie.Code.Trim().ToUpper())
                            {
                                case "0_SEC":
                                case "0_MIN":
                                    SetExitMode(0, args.ToString());
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
        */
        public override void TimePlanEventHandler(object sender, ITimePlanEventArgs args)
        {
            // var tpie = (TimePlanItemEvent) sender;

            //TradeContext.Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, StrategyTickerString, tpie.Key,
            //    args.Description, args.ToString());
            switch (args.EventType)
            {
                case TimePlanEventType.TimePlanItem:
                    switch (args.TimePlanItemCode)
                    {
                        case "MORNING":
                            switch (args.Msg)
                            {
                                case "START":
                                    SetExitMode(0, args.ToString());
                                    break;
                                case "FINISH":
                                    break;
                            }
                            break;
                        case "EVENING":
                            switch (args.Msg)
                            {
                                case "START":
                                    SetExitMode(0, args.ToString());
                                    break;
                                case "FINISH":
                                    break;
                            }
                            break;
                    }
                    break;
                case TimePlanEventType.TimePlanItemEvent:
                    switch (args.TimePlanItemCode)
                    {
                        case "MORNING":
                            switch (args.TimePlanItemEventCode)
                            {
                                case "TOEND":
                                    switch (args.Msg)
                                    {
                                        case "15_SEC":
                                            SetExitMode(11, args.ToString());
                                            break;
                                        case "1_MIN":
                                            SetExitMode(12, args.ToString());
                                            break;
                                        case "3_MIN":
                                            SetExitMode(13, args.ToString());
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case "EVENING":
                            switch (args.TimePlanItemEventCode)
                            {
                                case "TOEND":
                                    switch (args.Msg)
                                    {
                                        case "15_SEC":
                                            break;
                                        case "30_SEC":
                                            SetExitMode(1, args.ToString());
                                            break;
                                        case "1_MIN":
                                            break;
                                        case "2_MIN":
                                            SetExitMode(2, args.ToString());
                                            break;
                                        case "3_MIN":
                                            break;
                                        case "5_MIN":
                                            SetExitMode(3, args.ToString());
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
        public void AskRandom()
        {
            if (_randCount <= 0) return;
            _randCount = 0;

            _randValue = _random.Next(1, 101);
            _randAnswer = _randValue > RandMode ? true : false;

            TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, Code, Code, "Ask Random", "Value=" + _randValue + ", Answer=" + _randAnswer, "");
        }

        //public override string Key
        //{
        //    get
        //    {
        //        return String.Format("Type={0};Name={1};Code={2};TradeAccountKey={3};TickerKey={4};TimeInt={5}",
        //              GetType(), Name, Code, TradeAccountKey, TickerKey, TimeInt);
        //    }
        //}
        public override string ToString()
        {
            return String.Format("Name={0};Code={1};Account={2};Ticker={3};TimeInt={4}",
                        Name, Code, TradeAccountKey, TickerKey, TimeInt);
        }

        protected void ClearValue(out float v, int maxmin)
        {
            if (maxmin > 0)
                v = float.MaxValue;
            else if (maxmin < 0)
                v = float.MinValue;
            else
                v = 0;
        }

    }
}
