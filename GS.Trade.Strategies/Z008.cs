using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using GS.Extension;
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
    public partial class Z008 : Strategy
    {
        [XmlIgnore]
        public override IBars Bars { get; protected set; }

        [XmlIgnore]
        public override int MaxBarsBack { get; protected set; }

        [XmlIgnore]
        public override Atr Atr => _xAtr;


        //private Xma018 _xma018;
        protected Xma018 XTrend;
        protected Xma018 XMainTrend;

        private XAverage _xAvrg;

        private Atr _xAtr;
        private Atr _xAtr1;
        private Atr _xAtr2;

        //private int _xMainTrendValue;

        protected double LastTradeMaValue { get; set; }

        //  private BarSeries _bars;

        public uint TimeInt2 { get; set; }

        public int Ma1Length { get; set; }
        public int Ma1AtrLength { get; set; }
        public float Ma1KAtr { get; set; }
        public float Ma2KAtr { get; set; }
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

        public int SafeContractsOvn { get; set; }

        [XmlIgnore] protected int LastSignalId;

        public bool IsMorningSessionEnabled { get; set; }
        public bool IsEveningSessionEnabled { get; set; }

        public int StartEntryMode { get; set; }
        // 16.11.26
        // EntryMode on Strategy
        // public int EntryMode { get; set; }

        public bool NeedTrendChangedDelay { get; set; }
        public bool IsMode2Delayed { get; set; }
        public bool IsMode3Delayed { get; set; }

        public bool IsMode4FullExit { get; set; }
        public bool IsMode4SmartExit01 { get; set; }

        public int MaxContractsReachedMode { get; set; }

        public int StartLongSignal1 { get; set; }
        public int StartLongSignal2 { get; set; }
        public int StartShortSignal1 { get; set; }
        public int StartShortSignal2 { get; set; }

        public int EntrySignal11 { get; set; }
        public int EntrySignal12 { get; set; }
        public int EntrySignal13 { get; set; }
        public int EntrySignal14 { get; set; }

        public int EntrySignal21 { get; set; }
        public int EntrySignal22 { get; set; }
        public int EntrySignal23 { get; set; }
        public int EntrySignal24 { get; set; }

        public int EntrySignal31 { get; set; }
        public int EntrySignal32 { get; set; }
        public int EntrySignal33 { get; set; }
        public int EntrySignal34 { get; set; }

        public int EntrySignal41 { get; set; }
        public int EntrySignal42 { get; set; }
        public int EntrySignal43 { get; set; }
        public int EntrySignal44 { get; set; }
        public int EntrySignal45 { get; set; }

        //protected bool IsMode5Enable { get {
        //        return (    EntrySignal51 +
        //                    EntrySignal52 +
        //                    EntrySignal53 +
        //                    EntrySignal54
        //               ) != 0;
        //    }
        //}
        // IsMode5Enabled
        public bool IsMode5Enabled { get; set; }
        public int EntrySignal51 { get; set; }
        public int EntrySignal52 { get; set; }
        public int EntrySignal53 { get; set; }
        public int EntrySignal54 { get; set; }

        public int EntrySignal61 { get; set; }
        public int EntrySignal62 { get; set; }
        public int EntrySignal63 { get; set; }
        public int EntrySignal64 { get; set; }

        public int EntrySignal81 { get; set; }
        public int EntrySignal82 { get; set; }
        public int EntrySignal83 { get; set; }
        public int EntrySignal84 { get; set; }
        public int EntrySignal85 { get; set; }

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

        public int PosPriceMode { get; set; }

        public int RandMode { get; set; }

        private DateTime _lastDT = DateTime.MinValue;

        public bool IsMainTrendUp => XMainTrend != null && XMainTrend.Trend > 0;
        public bool IsMainTrendDown => XMainTrend != null && XMainTrend.Trend < 0;

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

        private int _swingCountStartEntry;

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
        [XmlIgnore]
        protected Trigger Trend55Changed;


        protected virtual bool IsPosPositive {
            get
            {
                return Position != null &&
                    (
                    (Position.IsLong && XTrend.High.IsGreaterOrEqualsThan((double) Position.Price1)) ||
                    (Position.IsShort && XTrend.Low.IsLessOrEqualsThan ((double) Position.Price1))
                    );
            }
        }
        // Old version
        protected bool IsPosPositive1
        {
            get
            {
                return Position != null && (
                    (Position.IsLong && XTrend.Ma > (double)Position.Price1) ||
                    (Position.IsShort && XTrend.Ma < (double)Position.Price1)
                    );
            }
        }

        protected bool IsPosNegative => !IsPosPositive;

        protected bool IsSwingNegative => TrEntryEnabled != null && TrEntryEnabled.Value && Position != null &&
                                          ((Position.IsLong && TakeShort(25)) || (Position.IsShort && TakeLong(25)));

        protected bool IsSwingPositive => TrEntryEnabled != null && TrEntryEnabled.Value && Position != null &&
                                          ((Position.IsLong && TakeLong(25)) || (Position.IsShort && TakeShort(25)));

        protected float VolatilityUnit => (float)(Ma1KAtr * (_xAtr?.LastAtrCompleted ?? 0f));

        protected float StopValue => KAtrStop * VolatilityUnit;

        public override float Volatility => Ma1KAtr * VolatilityUnit2;

        protected float VolatilityUnit2 => (float)(XTrend.High - XTrend.Low) / 2f;

        protected float StopValue2 => KAtrStop * VolatilityUnit2;

        protected float StopValue1 => KAtrStop1 * VolatilityUnit2;

        protected float OrderHookValue => KAtrHookOrder * VolatilityUnit2;

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
        public override string PositionInfo => $"PosMax: {PositionMax.GetValidValue:N2} PosMin: {PositionMin.GetValidValue:N2} PosStop: {PositionStop.GetValidValue:N2}";
        protected PositionMinMax PositionStop = new PositionMinMax();

        protected PositionMin PositionMin = new PositionMin();
        protected PositionMax PositionMax = new PositionMax();

        protected PositionMinMax TrailingTrHigh =  new PositionMinMax();
        protected PositionMinMax TrailingTrLow = new PositionMinMax();

        public float PositionMedian => PositionMax.IsValid && PositionMin.IsValid
            ? 0.5f * (PositionMax.GetValidValue + PositionMin.GetValidValue)
            : 0f;

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

        public Z008()
        {
            // need for Serialization

        }
        public Z008(ITradeContext tx, string name, string code, ITicker ticker, uint timeInt)
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
                //_xma018 = TradeContext.RegisterTimeSeries(
                //    new Xma018("Xma18", Ticker, (int)TimeInt, Ma1Length, Ma1AtrLength, Ma1KAtr, Ma1Mode)) as Xma018;
                XTrend = TradeContext.RegisterTimeSeries(
                    new Xma018("Xma18", Ticker, (int)TimeInt, Ma1Length, MaAtrLength1, MaAtrLength2, Ma1KAtr, Ma1Mode)) as Xma018;

                XMainTrend = TradeContext.RegisterTimeSeries(
                    new Xma018("Xma18Main", Ticker, (int)TimeInt2, Ma1Length, MaAtrLength1, MaAtrLength2, Ma2KAtr, Ma1Mode)) as Xma018;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            if (XTrend == null || XMainTrend == null)
                throw new Exception(StrategyTickerString + ": Xma016 Initialization Error");
            

            //_xAtr = XTrend.Atr;
            //_xAtr2 = (Atr)TradeContext.RegisterTimeSeries(new Atr("Atr2", Ticker, (int)TimeInt, 500));

            _xAtr1 = XTrend.Atr1;
            _xAtr2 = XTrend.Atr2;
            _xAtr = XTrend.Atr;

            MaxBarsBack = _xAtr.Length;
            
            _bars = (Bars)(XTrend.SyncSeries);
            Bars = _bars;

            MyReverseSignal1 = ReverseSignal1;
            MyExitSignal1 = ExitSignal1;
            MyEntrySignal1 = EntrySignal1;

            TrMaxContracts = new Trigger();
            TrEntryEnabled = new Trigger();
            Trend55Changed = new Trigger();

            Mode = 1;

            _swingCountEntry = SwingCountEntry - SwingCountStartEntry;

            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
        "Strategies", StrategyTickerString, "Signals Available", SignalsAvailable, ToString());

            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                    "Strategies", StrategyTickerString, "Init() Is Completed", "", ToString());


        }

        public string SignalsAvailable => $"EntrySignal11 {EntrySignal11}, " +
                                          $"EntrySignal12 {EntrySignal12}, " +
                                          $"EntrySignal13 {EntrySignal13}, " +
                                          $"EntrySignal14 {EntrySignal14}, " +

                                          $"EntrySignal21 {EntrySignal21}, " +
                                          $"EntrySignal22 {EntrySignal22}, " +
                                          $"EntrySignal23 {EntrySignal23}, " +
                                          $"EntrySignal24 {EntrySignal24}, " +

                                          $"EntrySignal31 {EntrySignal31}, " +
                                          $"EntrySignal32 {EntrySignal32}, " +
                                          $"EntrySignal33 {EntrySignal33}, " +
                                          $"EntrySignal34 {EntrySignal34}, " +

                                          $"EntrySignal41 {EntrySignal41}, " +
                                          $"EntrySignal42 {EntrySignal42}, " +
                                          $"EntrySignal43 {EntrySignal43}, " +
                                          $"EntrySignal44 {EntrySignal44}, " +
                                          $"EntrySignal45 {EntrySignal45}, " +

                                          $"EntrySignal51 {EntrySignal51}, " +
                                          $"EntrySignal52 {EntrySignal52}, " +
                                          $"EntrySignal53 {EntrySignal53}, " +
                                          $"EntrySignal54 {EntrySignal54}, " +

                                          $"EntrySignal81 {EntrySignal81}, " +
                                          $"EntrySignal82 {EntrySignal82}, " +
                                          $"EntrySignal83 {EntrySignal83}, " +
                                          $"EntrySignal84 {EntrySignal84}, " +
                                          $"EntrySignal85 {EntrySignal85}";

        protected override void PositionIsChangedEventHandler2(IPosition2 oldposition, IPosition2 newposition, PositionChangedEnum changedResult)
        {
            LastTradeMaValue = XTrend.Ma;

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
                changedResult == PositionChangedEnum.Opened ||                 // open new from flat
                changedResult == PositionChangedEnum.Registered
                //changedResult == PositionChangedEnum.Closed
                )
            {
                //  TrMaxContracts.Reset();
                TrEntryEnabled.Reset();
                MaxContractsReached = false;

                _swingCountExit = 0;
                _swingCountReverse = 0;
                _swingCountEntry = 0;

                MyReverseSignal1 = ReverseSignal1;
                MyExitSignal1 = ExitSignal1;
                MyEntrySignal1 = EntrySignal1;

                PositionMin.Clear();
                PositionMax.Clear();
                PositionStop.Clear();

                TrailingTrHigh.Clear();
                TrailingTrLow.Clear();

            }

            if (changedResult == PositionChangedEnum.Closed)
            {

                _swingCountEntry = 0;

                MyReverseSignal1 = ReverseSignal1;
                MyExitSignal1 = ExitSignal1;
                MyEntrySignal1 = EntrySignal1;

                MaxContractsReached = false;
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
                Trend55Changed.Reset();
            }

            SetStopOrderFilledStatus(false);

            //PositionChanged(oldposition, newposition, changedResult);
        }

        protected bool IsPosMaxReached;
        protected bool IsPosMinReached;

        protected bool IsPosAbsMaxReached => (Position.IsLong && IsPosMaxReached) || (Position.IsShort && IsPosMinReached);

        public override void Clear()
        {
            base.Clear();
            _lastDT = DateTime.MinValue;
        }
        public override void Main()
        {
            // if (TimePlan.IsTimeToRest) return;
            if (!TimePlan.Enabled) return;

            if (XTrend.Count < 2) return;
            if (XMainTrend.Count < 1) return;

            //   HookOrder();

            if (XTrend.LastItemCompletedDT.CompareTo(_lastDT) <= 0) return;
            // New Bar
            //if (_lastDT.Date.CompareTo(_xma018.LastItemCompletedDT.Date) < 0 && _xma018.TrendChanged)
            //{
            //    _swingCountEntry++;
            //}
            _lastDT = XTrend.LastItemCompletedDT;

            if (ExitInEmergencyWhenStopUnFilled()) return;

            if (_waitFlat && XTrend.IsFlat) _waitFlat = false;

            _xProfit = Position.Profit;

            //if (_xMainTrendValue != XMainTrend.Trend)
            //{
            //    _xMainTrendValue = XMainTrend.Trend;
            //    _randCount++;
            //}

            if (XTrend.TrendChanged)
            {
                _lastLimitBuyPrice = 0.0;
                _lastLimitSellPrice = 0.0;

                _swingCountEntry++;
                _swingCountReverse++;
                _swingCountExit++;

                _swingCountStartEntry--;

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
                    PositionMin.SetIfGreaterThan((float)XTrend.Low2);
                }

                IsPosMinReached = XTrend.Ma.IsLessThan(PositionMin.GetValidValue);

                TrailingTrHigh.SetIfGreaterThan((float)XTrend.TrHigh1);

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
                    PositionMax.SetIfLessThan((float)XTrend.High2);
                }

                IsPosMaxReached = XTrend.Ma.IsGreaterThan(PositionMax.GetValidValue);

                TrailingTrLow.SetIfLessThan((float)XTrend.TrLow1);
            }

            TrMaxContracts.Value = Position.Quantity >= Contracts;
            TrMaxContracts.Value = true;

            if (_swingCountEntry >= SwingCountEntry)
            {
                TrEntryEnabled.SetValue(TakeLong(190) || TakeShort(190));

                //TradeContext.Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, StrategyTickerString,
                //       "TrEntryEnabled", TrEntryEnabled.Value.ToString(), "swCntEntry=" + _swingCountEntry, "");
            }

            Trend55Changed.SetValue(XTrend.Trend55Changed);

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

            if (IsPosContractsExceeded)
            {
                var ordoper = Position.IsLong ? OrderOperationEnum.Sell : OrderOperationEnum.Buy;
                var ncntrs = Position.Quantity - Contracts;
                _currentLimitPrice = Position.IsLong ? Ticker.Ask : Ticker.Bid;
                SetOrder2(ordoper, ncntrs);

                Evlm2(EvlResult.FATAL, EvlSubject.TRADING, "Strategies", StrategyTickerString, Position.PositionString3,
                                "Contracts = " +  ncntrs + ". Contracts Numbers Exceeded." ,"");
                return;
            }
            if (!EntryEnabled || !EntryPortfolioEnabled)
            {
                //ClearLongShortRequests();
                //KillAllOrders2();
                return;
            }

            //if (PortfolioEnable && PortfolioRisk != null && PortfolioRisk.IsShortEnabled)
            //{
            //    ;
            //}
            //if (PortfolioEnable && PortfolioRisk != null && PortfolioRisk.IsLongEnabled)
            //{
            //    ;
            //}
            if (!Working || !IsSwingCountStartEntryReached)
                return;
            
            var oper = Entry();
            //if (oper != 0 && Position.IsNeutral)
            //{
            //    var shorts = ShortRequest(Contract);
            //    var longs = LongRequest(Contract);
            //}
            //15.09.24
            // if (oper < 0 && (Position.IsOpened || (Position.IsNeutral && PortfolioEnable && PortfolioRisk != null && PortfolioRisk.IsShortEnabled)))
            if (oper < 0 && 
                (Position.IsOpened ||
                (Position.IsNeutral && !IsPortfolioRiskEnable) ||
                (Position.IsNeutral && ShortRequest((int) Contract) > 0)
                ))
            {
                if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;
                _lastLimitSellPrice = _currentLimitPrice;
                
                //SetOrder2(OrderOperationEnum.Sell, Contract);
                // 15.09.18
                //SetOrder2(OrderOperationEnum.Sell, Mode == 5 || ((Mode == 6 || Mode == 4) && IsMode4FullExit) 
                //    ? Position.Quantity 
                //    : Contract);
                // 15.09.18
                var cs = GetSize(Mode, LastSignalActivated);
                SetOrder2(OrderOperationEnum.Sell, cs);
                return;
            }
            if (oper > 0 && 
                (Position.IsOpened ||
                (Position.IsNeutral && !IsPortfolioRiskEnable) ||
                (Position.IsNeutral && LongRequest((int) Contract) > 0)
                ))
            {
                if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;
                _lastLimitBuyPrice = _currentLimitPrice;

                //SetOrder2(OrderOperationEnum.Buy, Contract);
                // 15.09.18
                //SetOrder2(OrderOperationEnum.Buy, Mode == 5 || ((Mode == 6 || Mode == 4) && IsMode4FullExit) 
                //    ? Position.Quantity 
                //    : Contract);
                // 15.09.18
                var cs = GetSize(Mode, LastSignalActivated);
                SetOrder2(OrderOperationEnum.Buy, cs);
                return;
            }
            ClearLongShortRequests();
            KillAllActiveOrders2();
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

        private long GetSize(int mode, int signalId)
        {
            return  Mode == 5 ||
                    (IsMode4FullExit && (Mode == 6 || Mode == 4) && LastSignalActivated == 19) ||
                    (
                    !IsMode4FullExit && IsMode4SmartExit01 && (Mode == 6 || Mode == 4) && LastSignalActivated == 19 &&
                    ((Position.IsLong && XTrend.TrPos1 > 50d) || (Position.IsShort && XTrend.TrPos1 < 50d)) 
                    )
                    ? Position.Quantity
                    : Contract;
        }

        protected void SetOrder2(OrderOperationEnum operation, long contract)
        {
            if (contract <= 0) return;
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

            _xBandAtr = ((Xma018.Item)(XTrend.LastItemCompleted)).High -
                        ((Xma018.Item)(XTrend.LastItemCompleted)).Ma;

            //var contract = Position.Pos == 0 ? 1 : 2;

            /*
            TradeTerminal.KillAllOrders(Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                        OrderTypeEnum.Limit, OperationEnum.All);

            TradeTerminal.KillAllOrders(Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                        OrderTypeEnum.StopLimit, flipOperation);
            */



            KillAllActiveOrders2();

            //if (string.IsNullOrWhiteSpace(Ticker.FormatF))
            //    throw new NullReferenceException(Ticker.Code + ": Ticker Format is Null");

            //var priceStr = _currentLimitPrice.ToString(Ticker.FormatF);

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
        protected void SetOrder5(OrderOperationEnum operation, long contract)
        {
            if (_currentLimitPrice.CompareTo(0d) == 0) return;

            if (string.IsNullOrWhiteSpace(Ticker.FormatF))
                throw new NullReferenceException(Ticker.Code + ": Ticker Format is Null");

            var priceStr = _currentLimitPrice.ToString(Ticker.FormatF);

            var o = ActiveOrderCollection.RegisterOrder(this, operation, OrderTypeEnum.Limit, 0, _currentLimitPrice, contract, "");
            TradeTerminal.SetLimitOrder(o);

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
                _currentLimitPrice = XTrend.GetPrice3(+1, StopPriceId, out _comment);
                return true;
            }
            return false;
        }
        protected virtual bool LongStopExit()
        {
            if (XTrend.Ma.CompareTo((double)(Position.Price1 - (decimal)StopValue2)) < 0)
            {
                _currentLimitPrice = XTrend.GetPrice3(-1, StopPriceId, out _comment);
                return true;
            }
            return false;
        }

        protected int LastSignalActivated;
        protected bool TakeLong(int index)
        {
            if (index == 0) return false;

            switch (index)
            {
                case 1:
                    LastSignalActivated = 1;
                    return true;
                case 1106:
                    if (XTrend.TakeLong(106) && ((double)PositionMin2).CompareTo(XTrend.High) > 0)
                        return true;
                    break;
                case 19106:
                    return XTrend.TakeLong(19) || XTrend.TakeLong(106);
                case 819:
                    return XTrend.TakeLong(19) || XTrend.TakeLong(8);
                case 8182:
                    return XTrend.TakeLong(81) || XTrend.TakeLong(82);
                case 8119:
                    return XTrend.TakeLong(19) || XTrend.TakeLong(81);
                case 8219:
                    return XTrend.TakeLong(19) || XTrend.TakeLong(82);
                case 82106:
                    return XTrend.TakeLong(82) || XTrend.TakeLong(106);
                case 82105:
                    return XTrend.TakeLong(82) || XTrend.TakeLong(105);
                case 818219:
                    return XTrend.TakeLong(19) || XTrend.TakeLong(81) || XTrend.TakeLong(82);
                case 819106:
                    return XTrend.TakeLong(19) || XTrend.TakeLong(106) || XTrend.TakeLong(8);
                case 8106:
                    return XTrend.TakeLong(106) || XTrend.TakeLong(8);
                case 48106:
                    return XTrend.TakeLong(106) || XTrend.TakeLong(8) || XTrend.TakeLong(4);
                case 18719:
                    return XTrend.TakeLong(19) || XTrend.TakeLong(18007);
                case 18782:
                    return XTrend.TakeLong(82) || XTrend.TakeLong(18007);
                case 1878219:
                    return XTrend.TakeLong(19) || XTrend.TakeLong(82) || XTrend.TakeLong(18007);
                case 2519:
                    return (XTrend.TakeLong(25) && (XTrend.Ma - XTrend.Low2) < VolatilityUnit2 * 6f) ||
                           (XTrend.TakeLong(19) && (XTrend.High2 - XTrend.Ma) > VolatilityUnit2 * 6f);
                case 25006:
                    return (XTrend.TakeLong(25) && (XTrend.Ma - XTrend.Low2) < VolatilityUnit2 * 6f);
                default:
                    if (XTrend.TakeLong(index))
                    {
                        LastSignalActivated = index;
                        return true;
                    }
                    break;
            }
            LastSignalActivated = 0;
            return false;
        }
        protected bool TakeShort(int index)
        {
            if (index == 0)
            {
                LastSignalActivated = 0;
                return false;
            }
            switch (index)
            {
                case 1:
                    LastSignalActivated = 1;
                    return true;
                case 1106:
                    return XTrend.TakeShort(106) && ((double)PositionMax2).CompareTo(XTrend.Low) < 0;
                case 19106:
                    return XTrend.TakeShort(19) || XTrend.TakeShort(106);
                case 819:
                    return XTrend.TakeShort(19) || XTrend.TakeShort(8);
                case 8182:
                    return XTrend.TakeShort(81) || XTrend.TakeShort(82);
                case 8119:
                    return XTrend.TakeShort(19) || XTrend.TakeShort(81);
                case 8219:
                    return XTrend.TakeShort(19) || XTrend.TakeShort(82);
                case 82106:
                    return XTrend.TakeShort(82) || XTrend.TakeShort(106);
                case 82105:
                    return XTrend.TakeShort(82) || XTrend.TakeShort(105);
                case 818219:
                    return XTrend.TakeShort(19) || XTrend.TakeShort(81) || XTrend.TakeShort(82);
                case 819106:
                    return XTrend.TakeShort(19) || XTrend.TakeShort(106) || XTrend.TakeShort(8);
                case 8106:
                    return XTrend.TakeShort(106) || XTrend.TakeShort(8);
                case 48106:
                    return XTrend.TakeShort(106) || XTrend.TakeShort(8) || XTrend.TakeShort(4);
                case 18719:
                    return XTrend.TakeShort(19) || XTrend.TakeShort(18007);
                case 18782:
                    return XTrend.TakeShort(82) || XTrend.TakeShort(18007);
                case 1878219:
                    return XTrend.TakeShort(19) || XTrend.TakeShort(82) || XTrend.TakeShort(18007);
                case 2519:
                    return (XTrend.TakeShort(25) && (XTrend.High2 - XTrend.Ma) < VolatilityUnit2 * 6f) ||
                           (XTrend.TakeShort(19) && (XTrend.Ma - XTrend.Low2) > VolatilityUnit2 * 6f);
                case 25006:
                    return (XTrend.TakeShort(25) && (XTrend.High2 - XTrend.Ma) < VolatilityUnit2 * 6f);
                default:
                    if (XTrend.TakeShort(index))
                    {
                        LastSignalActivated = index;
                        return true;
                    }
                    break;
            }
            LastSignalActivated = 0;
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
                                    if (!IsMorningSessionEnabled)
                                    {
                                        Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                                        "Strategies", StrategyTickerString, $"{args.TimePlanItemCode} Session: {args.Msg} DISABLED, Working: {Working}", "", "");
                                        break;
                                    }
                                    StartNewDayInit();
                                    Working = true;
                                   // TrEntryEnabled.Reset();
                                   // _swingCountEntry = SwingCountEntry - SwingCountStartEntry;
                                    
                                    SetExitMode(0, args.ToString());
                                    Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                                        "Strategies", StrategyTickerString, $"{args.TimePlanItemCode} Session: {args.Msg}, Working: {Working}", "","");
                                    break;
                                case "FINISH":
                                    SetExitMode(0, args.ToString());
                                    Working = false;
                                    Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                                        "Strategies", StrategyTickerString, $"{args.TimePlanItemCode} Session: {args.Msg}, Working: {Working}", "", "");
                                    break;
                            }
                            break;
                        case "EVENING":
                            switch (args.Msg)
                            {
                                case "START":
                                    if (!IsEveningSessionEnabled)
                                    {
                                        Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                                        "Strategies", StrategyTickerString, $"{args.TimePlanItemCode} Session: {args.Msg} DISABLED, Working: {Working}", "", "");
                                        break;
                                    }
                                    StartNewDayInit();
                                    Working = true;
                                    SetExitMode(0, args.ToString());
                                    Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                                        "Strategies", StrategyTickerString, $"{args.TimePlanItemCode} Session: {args.Msg}, Working: {Working}", "", "");
                                    break;
                                case "FINISH":
                                    SetExitMode(0, args.ToString());
                                    Working = false;
                                    Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                                        "Strategies", StrategyTickerString, $"{args.TimePlanItemCode} Session: {args.Msg}, Working: {Working}", "", "");
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
                                        //case "15_SEC":
                                        //    SetExitMode(0, args.ToString());
                                        //    break;
                                        //case "1_MIN":
                                        //    SetExitMode(0, args.ToString());
                                        //    break;
                                        case "6_MIN":
                                            SetExitMode(11, args.ToString());
                                            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                                                "Strategies", StrategyTickerString, $"ExitMode: {ExitMode}", $"Session: {args.TimePlanItemCode} Minutes: {args.Msg} {args.TimePlanItemEventCode}", "");
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
                                        //case "15_SEC": // Market Exit
                                        //    SetExitMode(11, args.ToString());
                                        //    break;
                                        //case "30_SEC": // Market Exit
                                        //    SetExitMode(11, args.ToString());
                                        //    break;
                                        //case "45_SEC": // Market Exit
                                        //    SetExitMode(11, args.ToString());
                                        //    break;
                                        //case "1_MIN": // Market Exit
                                        //    SetExitMode(11, args.ToString());
                                        //    break;
                                        //case "2_MIN": // Volatility/2 Exit
                                        //    //SetExitMode(11, args.ToString());
                                        //    SetExitMode(12, args.ToString());
                                        //    break;
                                        case "5_MIN": // Bid/Ask
                                            SetExitMode(12, args.ToString());
                                            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                                                "Strategies", StrategyTickerString, $"ExitMode: {ExitMode}", $"Session: {args.TimePlanItemCode} Minutes: {args.Msg} {args.TimePlanItemEventCode}", "");
                                            break;
                                        //case "10_MIN": // Bid/Ask
                                        //    SetExitMode(12, args.ToString());
                                        //    break;
                                        //case "12_MIN": // Volatility Exit
                                        //    //SetExitMode(11, args.ToString());
                                        //    SetExitMode(13, args.ToString());
                                        //    break;
                                        //case "15_MIN": // Volatility Exit
                                        //    //SetExitMode(11, args.ToString());
                                        //    SetExitMode(14, args.ToString());
                                        //    break;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
        public override void CloseAll(int mode)
        {
            if (Position == null)
            {
                var e = new NullReferenceException(StrategyTickerString + ": Position is Null");
                SendExceptionMessage3(StrategyTickerString, "Strategy.CloseAll(int)", "Position == null", "CloseAll(int)", e);
                return;
                //throw new NullReferenceException();
            }
            //if (!Position.IsNeutral)
            //    Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, "Strategies" ,
            //        StrategyTickerString, "Close All(Orders & Position)", Position.PositionString3, "");
            try
            {
                SetStopOrderFilledStatus(false);

                // TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTickerString, 
                //     "Start Close All", "ExitMode=" + mode, "");

                //GetActiveOrders();
                //  SetKillAllOrdersInQueue();

                KillAllActiveOrders2();

                //if (Position == null)
                //{
                //    var e = new NullReferenceException(StrategyTickerString + ": Position is Null");
                //    SendExceptionMessage3(StrategyTickerString, "Strategy.CloseAll(int)", "Position == null", "CloseAll(int)", e);
                //    return;
                //    //throw new NullReferenceException();
                //}

                if (Position.IsNeutral) return;

                Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, "Strategies",
                       StrategyTickerString, "Close All(Orders & Position)", Position.PositionString3, "");

               var operation = OrderOperationEnum.Unknown;
                var price = 0d;
                string priceStr;

                long quantity = 0;
                var safeQv = Position.Quantity - SafeContractsOvn;
                if (safeQv <= 0)
                    return;

                switch (mode)
                {
                    case 2:
                    case 1:
                        if (Position.IsLong)
                        {
                            operation = OrderOperationEnum.Sell;
                            price = mode == 1 ? Ticker.MarketPriceSell : Ticker.Ask;
                            quantity = Position.Quantity;

                        }
                        else if (Position.IsShort)
                        {
                            operation = OrderOperationEnum.Buy;
                            price = mode == 1 ? Ticker.MarketPriceBuy : Ticker.Bid;
                            quantity = Position.Quantity;
                        }
                        break;
                    case 3:
                        if (Position.IsLong)
                        {
                            operation = OrderOperationEnum.Sell;
                            price = Ticker.Ask + Volatility;
                            price = Ticker.ToMinMove(price, -1);
                            quantity = Position.Quantity;
                        }
                        else if (Position.IsShort)
                        {
                            operation = OrderOperationEnum.Buy;
                            price = Ticker.Bid - Volatility;
                            price = Ticker.ToMinMove(price, +1);
                            quantity = Position.Quantity;
                        }
                        break;
                    // Bid Ask Market
                    case 12:
                    case 11:
                        if (Position.IsLong)
                        {
                            operation = OrderOperationEnum.Sell;
                            price = mode == 11 ? Ticker.MarketPriceSell : Ticker.Ask;
                            //quantity = (Position.Quantity - SafeContractsOvn) > 0 ? (Position.Quantity - SafeContracts) : 0;
                            quantity = safeQv;
                        }
                        else if (Position.IsShort)
                        {
                            operation = OrderOperationEnum.Buy;
                            price = mode == 11 ? Ticker.MarketPriceBuy : Ticker.Bid;
                            //quantity = (Position.Quantity - SafeContracts) > 0 ? (Position.Quantity - SafeContracts) : 0;
                            quantity = safeQv;
                        }
                        break;
                        // Bid - Volatility ; Ask + Volatility
                    case 13:
                        if (Position.IsLong)
                        {
                            operation = OrderOperationEnum.Sell;
                            price = Ticker.Ask + Volatility / 2f;
                            price = Ticker.ToMinMove(price, -1);
                            // quantity = (Position.Quantity - SafeContracts) > 0 ? (Position.Quantity - SafeContracts) : 0;
                            quantity = safeQv;
                        }
                        else if (Position.IsShort)
                        {
                            operation = OrderOperationEnum.Buy;
                            price = Ticker.Bid - Volatility / 2f;
                            price = Ticker.ToMinMove(price, +1);
                            //quantity = (Position.Quantity - SafeContracts) > 0 ? (Position.Quantity - SafeContracts) : 0;
                            quantity = safeQv;
                        }
                        break;

                    case 14:
                        if (Position.IsLong)
                        {
                            operation = OrderOperationEnum.Sell;
                            price = Ticker.Ask + Volatility;
                            price = Ticker.ToMinMove(price, -1);
                            // quantity = (Position.Quantity - SafeContracts) > 0 ? (Position.Quantity - SafeContracts) : 0;
                            quantity = safeQv;
                        }
                        else if (Position.IsShort)
                        {
                            operation = OrderOperationEnum.Buy;
                            price = Ticker.Bid - Volatility;
                            price = Ticker.ToMinMove(price, +1);
                            //quantity = (Position.Quantity - SafeContracts) > 0 ? (Position.Quantity - SafeContracts) : 0;
                            quantity = safeQv;
                        }
                        break;
                }

                if (quantity <= 0) return;
                if (operation == OrderOperationEnum.Unknown) return;

                //if (string.IsNullOrWhiteSpace(Ticker.FormatF))
                //    throw new NullReferenceException(Ticker.Code + ": Ticker Format is Null");

                //priceStr = price.ToString(Ticker.FormatF);

                //TradeContext.Evlm(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTickerString, StrategyTickerString,
                //    "Close All Process", String.Format("ExitMode={0}; Price={1}; Pos={2}; Quant = {3}; Bid={4}; Ask={5}; Vol={6}",
                //             mode, priceStr, Position.PosOperation, quantity, Ticker.Bid, Ticker.Ask, Volatility.ToString(Ticker.FormatF)), "");

                //TradeTerminal.SetLimitOrderInQueue(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
                //                                operation,
                //                                price, priceStr, quantity,
                //                                DateTime.MaxValue,
                //                                "Close All");
                var o = ActiveOrderCollection.RegisterOrder(this, operation, OrderTypeEnum.Limit, 0, price, quantity, "");
                TradeTerminal.SetLimitOrder(o);



            }
            catch (Exception e)
            {
                SendExceptionMessage3(StrategyTickerString, "Strategy", "Strategy.CloseAll(int)", ToString(),e);
                throw;
            }

        }
        public override void SkipTheTick(int n)
        {
            _swingCountEntry = n;
            TrEntryEnabled.Reset();
        }

        public override void StartNewDayInit()
        {
            _swingCountEntry = SwingCountEntry - SwingCountStartEntry;
            TrEntryEnabled.Reset();
            _swingCountStartEntry = SwingCountStartEntry;

            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, "Strategies:", StrategyTickerString,
                        System.Reflection.MethodBase.GetCurrentMethod().Name, "Init Session",
                        $"SwingCountStartEntry: {_swingCountStartEntry}");
        }
        public bool IsSwingCountStartEntryReached => _swingCountStartEntry <= 0;

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
        //        return String.Format("[Type={0};Name={1};Code={2};TradeAccountKey={3};TickerKey={4};TimeInt={5}]",
        //              GetType(), Name, Code, RealAccountKey, RealTickerKey, TimeInt);
        //    }
        //}
        public override string ToString()
        {
            return $"Name: {Name}, Code: {Code}, Account: {RealAccountKey}, Ticker: {RealTickerKey}, TimeInt:{TimeInt}";
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
