using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GS.Interfaces;
using GS.Trade.Data;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Studies;
using GS.Trade.Data.Studies.Averages;
using GS.Trade.Data.Studies.GS.xMa018;
using GS.Trade.Interfaces;
using GS.Trade.Trades;

namespace GS.Trade.Strategies
{
    public partial class X118 : Strategy
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

        protected float VolatilityUnit
        {
            get { return (float)( Ma1KAtr * (_xAtr == null ? 0f : _xAtr.LastAtrCompleted)); }
        }
        protected float StopValue
        {
            get { return KAtrStop * VolatilityUnit; }
        }
        public override float Volatility
        {
            get { return /*Ma1KAtr * */ VolatilityUnit2; }
        }
        protected float VolatilityUnit2
        {
            get { return (float) (XTrend.High - XTrend.Low)/2f; }
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
                           (Position.IsLong && PositionMaxValid && ((double)PositionMax).CompareTo(XTrend.High2) > 0)
                           ||
                           (Position.IsShort && PositionMinValid && ((double)PositionMin).CompareTo(XTrend.Low2) < 0)
                       );
            }
        }
        protected bool IsLongTermBreakUp
        {
            get
            {
                double high;
                return ! XTrend.HaveHigher((int)TimeInt * 60 * KMinFarFromHere , XTrend.Ma, out high);
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
                return String.Format("PosMax: {0:N2} PosMin: {1:N2} PosStop: {2:N2} StopVol: {3:N2}",
                                     PositionMaxValid ? PositionMax : 0,
                                     PositionMinValid ? PositionMin : 0,
                                     PositionStopValid ? PositionStop : 0,
                                     VolatilityUnit2.CompareTo(0f) != 0
                                         ? (int)Position.PosOperation * ((float) Position.Price1 - PositionStop)/VolatilityUnit2
                                         : 0);
            }
        }
        protected float PositionStop;

        protected float PositionMin;
        protected float PositionMax;
        
        protected float PositionMin2;
        protected float PositionMax2;

        protected bool PositionMaxValid
        {
            get { return PositionMax.CompareTo(Single.MinValue) != 0; }
        }
        protected bool PositionMinValid
        {
            get { return PositionMin.CompareTo(Single.MaxValue) != 0; }
        }
        protected bool PositionStopValid
        {
            get { return PositionStop.CompareTo(Single.MaxValue) != 0; }
        }

        protected int Mode;
        protected int ModeSafe;

        public int ReverseLossCnt;
        protected int RealReverseLossCnt;

        public int RichTarget { get; set; }

        public X118()
        {
            // need for Serialization

        }
        public X118(ITradeContext tx, string name, string code, Ticker ticker, uint timeInt)
            : base(tx, name, code, ticker, timeInt)
        {
        }
        public override void Init()
        {
            base.Init(); if (IsWrong) return;
            Position.PositionChangedEvent += PositionIsChangedEventHandler;
            if (EntryManager != null)
            {
                Position.PositionChangedEvent2 += EntryManager.PositionChanged;
                ShortEnabled = false;
                LongEnabled = false;
            }
            else
            {
                ShortEnabled = true;
                LongEnabled = true;
            }
            _xma018 = TradeContext.RegisterTimeSeries(
                new Xma018("Xma18", Ticker, (int)TimeInt, Ma1Length, Ma1AtrLength, Ma1KAtr, Ma1Mode)) as Xma018;

            XTrend = _xma018;

            _xAtr = XTrend.Atr;
            _xAtr2 = (Atr)TradeContext.RegisterTimeSeries(new Atr("Atr2", Ticker, (int)TimeInt, 500));
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

            TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, StrategyTickerString, StrategyTickerString, "Init " + Code, ToString(), "");
        }

        private void PositionIsChangedEventHandler(long oldposition, long newposition)
        {
            if (oldposition < 0)
                TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                                    OrderTypeEnum.All, OrderOperationEnum.Buy);
            else if (oldposition > 0)
                TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                                    OrderTypeEnum.All, OrderOperationEnum.Sell);

         //   TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.TRADING, StrategyTickerString, "PositionChanged",
         //                                "New: " + newposition + "; Old: " + oldposition, "");

            if (Math.Sign(oldposition) != Math.Sign(newposition) ||     // reverse
                (oldposition == 0 && newposition != 0)                 // open new from flat
                )
            {
                _swingCountExit = 0;
                _swingCountReverse = 0;
                _swingCountEntry = 0;

                MyReverseSignal1 = ReverseSignal1;
                MyExitSignal1 = ExitSignal1;
                MyEntrySignal1 = EntrySignal1;

                PositionMin = Single.MaxValue;
                PositionMax = Single.MinValue;

                PositionMin2 = PositionMin;
                PositionMax2 = PositionMax;

                PositionStop = PositionMin;
            }
            if (oldposition != 0 && newposition == 0)
            {
                _waitFlat = true;
                // _swingCount = 0;
            }
            if (newposition == 0)
            {
                _swingCountEntry = 0;

                MyReverseSignal1 = ReverseSignal1;
                MyExitSignal1 = ExitSignal1;
                MyEntrySignal1 = EntrySignal1;

                PositionStop = Single.MaxValue;
            }

            SetStopOrderFilledStatus(false);

            PositionChanged(oldposition, newposition);
        }
        public override void Main()
        {
          //  if (TimePlan.IsTimeToRest) return;
            if ( ! TimePlan.Enabled) return;
            
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
                    PositionMax = Math.Max(PositionMax, (float) XTrend.High2);
                else if (XTrend.IsUp)
                    PositionMin = Math.Min(PositionMin, (float) XTrend.Low2);

                if (PositionMaxValid && pmax.CompareTo(PositionMax) != 0)
                    PositionMax2 = pmax;
                if ( PositionMinValid && pmin.CompareTo(PositionMin) != 0)
                    PositionMin2 = pmin;
                /*
                if( ! PositionMaxValid )
                {
                    double m;
                    if (XTrend.Higher((int)TimeInt * 60, XTrend.High, out m))
                        PositionMax = (float)m;
                }
                */

            }
            else if (Position.IsLong)
            {
                var pmax = PositionMax;
                var pmin = PositionMin;

                if (XTrend.IsUp)
                    PositionMin = Math.Min(PositionMin, (float) XTrend.Low2);
                else if (XTrend.IsDown)
                    PositionMax = Math.Max(PositionMax, (float) XTrend.High2);

                if (pmax.CompareTo(PositionMax) != 0)
                    PositionMax2 = pmax;
                if (pmin.CompareTo(PositionMin) != 0)
                    PositionMin2 = pmin;
                /*
                if (!PositionMinValid)
                {
                    double m;
                    if (XTrend.Lower((int)TimeInt * 60, XTrend.Low, out m))
                        PositionMin = (float)m;
                }
                */
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

                    SetOrder2(OrderOperationEnum.Buy, 2);
                    return;
                }

                #endregion

                // ********************* Short Exit **********************************
                #region Short Exit

                #region SignalExit

                if ( ShortExit() )
                {
                    if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitBuyPrice = _currentLimitPrice;

                    SetOrder2(OrderOperationEnum.Buy, 1);
                    return;
                }
                #endregion
                
                #region StopExit

                if ( ShortStopExit() )
                {
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
                // ************************** Reverse *******************************
                #region Reverse

                if (EntryEnabled && LongReverse())
                {
                    if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitSellPrice = _currentLimitPrice;

                    SetOrder2(OrderOperationEnum.Sell, 2);
                    return;
                }

                #endregion
                // *****************************  Long Exit ********************************
                #region Long Exit

                if ( LongExit())
                {
                    if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                    _lastLimitSellPrice = _currentLimitPrice;

                    SetOrder2(OrderOperationEnum.Sell, 1);
                    return;
                }

                if ( LongStopExit() )
                {
                    SetOrder2(OrderOperationEnum.Sell, 1);
                    return;
                }
                
                #endregion

                #region TakeProfit
                #endregion
            }
            // ****************************  Position is Newtral *******************************
            #region Postion is Neutral
            else if (EntryEnabled)// Position is Neutral 10000000100000101
            {
                if (PositionTotal.Quantity != 0)
                {
                    if( Entry() < 0)
                    {
                        if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                        _lastLimitSellPrice = _currentLimitPrice;

                        SetOrder2(OrderOperationEnum.Sell, 1);
                        return;
                    }
                    if (Entry() > 0)
                    {
                        if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                        _lastLimitBuyPrice = _currentLimitPrice;

                        SetOrder2(OrderOperationEnum.Buy, 1);
                        return;
                    }
                    if (ShortEntry())
                    {
                        if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                        _lastLimitSellPrice = _currentLimitPrice;

                        SetOrder2(OrderOperationEnum.Sell, 1);
                        return;
                    }

                    if (LongEntry())
                    {
                        if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                        _lastLimitBuyPrice = _currentLimitPrice;

                        SetOrder2(OrderOperationEnum.Buy, 1);
                        return;
                    }
                }
                else
                {
                    if (Entry() < 0)
                    {
                        if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                        _lastLimitSellPrice = _currentLimitPrice;

                        SetOrder2(OrderOperationEnum.Sell, 1);
                        return;
                    }
                    if (Entry() > 0)
                    {
                        if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                        _lastLimitBuyPrice = _currentLimitPrice;

                        SetOrder2(OrderOperationEnum.Buy, 1);
                        return;
                    }

                    if (StartShortEntry())
                    {
                        if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                        _lastLimitSellPrice = _currentLimitPrice;

                        SetOrder2(OrderOperationEnum.Sell, 1);
                        return;
                    }

                    if (StartLongEntry())
                    {
                        if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                        _lastLimitBuyPrice = _currentLimitPrice;

                        SetOrder2(OrderOperationEnum.Buy, 1);
                        return;
                    }
                }
            }
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

            

            KillAllOrders();

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
        
        protected virtual void PositionChanged(long op, long np)
        {
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
            if (PositionMax.CompareTo(Single.MinValue) != 0 && XTrend.Ma.CompareTo(PositionMax) > 0)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                    return true;
            }
            return false;
        }
        protected virtual bool LongExit()
        {
            if (ExitSignal1 > 0 && XTrend.TakeShort(ExitSignal1)&& _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Exit:[" + ExitSignal1 + "] " + _comment;
                    return true;
                }
            }
            if (PositionMin.CompareTo(Single.MaxValue) != 0 && XTrend.Ma.CompareTo(PositionMin) < 0)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                    return true;
            }
            return false;
        }

        protected virtual bool ShortStopExit()
        {
            //if (XTrend.Ma.CompareTo((double)(Position.Price1 + (decimal)StopValue2)) > 0 )
            //{
            //    _currentLimitPrice = _xma018.GetPrice3(+1, StopPriceId, out _comment);
            //    return true;
            //}
            return false;
        }
        protected virtual bool LongStopExit()
        {
            //if (XTrend.Ma.CompareTo((double)(Position.Price1 - (decimal)StopValue2)) < 0 ) 
            //{
            //    _currentLimitPrice = _xma018.GetPrice3(-1, StopPriceId, out _comment);
            //    return true;
            //}
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
                    if ( XTrend.TakeLong(106) && ((double)PositionMin2).CompareTo(XTrend.High) > 0)
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
                    if ( XTrend.TakeShort(106) && ((double)PositionMax2).CompareTo(XTrend.Low) < 0)
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
            if( Position.IsLong)
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
            else if ( Position.IsShort)
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

        protected void ClearValue( out float v, int maxmin)
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
