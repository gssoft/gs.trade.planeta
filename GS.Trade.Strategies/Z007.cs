using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Xml.Serialization;
using GS.Extension;
using GS.Interfaces;
using GS.Status;
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
    public partial class Z007 : Strategy
    {
        [XmlIgnore]
        public override IBars Bars { get; protected set; }

        [XmlIgnore]
        public override int MaxBarsBack { get; protected set; }

        [XmlIgnore]
        public override Atr Atr => _xAtr;

        private Xma018 _xma018;
        protected Xma018 XTrend;
        protected Xma018 XTrend2;
        protected Xma018 XTrend3;

        private Xma018 _xMainTrend;

        protected Xma018 PrimTrend;

        private XAverage _xAvrg;

        private Atr _xAtr;
        private Atr _xAtr1;
        private Atr _xAtr2;

        private Atr _primTrendAtr;

        private int _xMainTrendValue;
        
        protected double LastTradeMaValue { get; set; }

        protected double LastTradeLongMaValue
        {
            get { return _lastTradeLongMaValue.IsGreaterThan(0d) ? _lastTradeLongMaValue : double.MaxValue; }
            set { _lastTradeLongMaValue = value; }
        }

        protected double LastTradeShortMaValue
        {
            get { return _lastTradeShortMaValue.IsGreaterThan(0d) ? _lastTradeShortMaValue : double.MinValue; }
            set { _lastTradeShortMaValue = value; }
        }

        public decimal DailyProfitLimit { get; set; }
        public decimal DailyLossLimit { get; set; }

        public bool IsDailyProfitLimitRiched =>
            DailyProfitLimit > 0 && Position != null && Position.LastTradeDT.Date.IsEquals(DateTime.Now.Date) &&
            (
                (Position.IsOpened && Position.DailyCurrentPnL >= DailyProfitLimit + DailyProfitLimit*.005m) ||
                (Position.IsNeutral && Position.DailyPnLFixed >= DailyProfitLimit + DailyProfitLimit*.005m)
                );

        public bool IsDailyLossLimitRiched =>
            DailyLossLimit > 0 &&
            Position != null && Position.IsOpened && Position.DailyCurrentPnL <= -DailyLossLimit;

        public uint TimeInt2 { get; set; }
        public uint TimeInt3 { get; set; }

        public int Ma1Length { get; set; }
        public int Ma1AtrLength { get; set; }
        public float Ma1KAtr { get; set; }
        public float Ma1KAtr2 { get; set; }
        public float Ma1KAtr3 { get; set; }
        public int Ma1Mode { get; set; }

        public float Ma2KAtr { get; set; }
        public float Ma2KAtrMltp { get; set; }

        public float Ma12KAtr { get; set; }

        public int MaAtrLength1 { get; set; }
        public int MaAtrLength2 { get; set; }

        public float KAtrStop { get; set; }
        public float KAtrStop1 { get; set; }
        public float KAtrStop2 { get; set; }

        public double KAtrTakeProfit { get; set; }

        public float KVolatilityUnitsFromPosMax { get; set; }

        public int SwingCount { get; set; }

        public int SwingCountStartEntry { get; set; }
        public int SwingCountEntry { get; set; }
        public int SwingCountReverse { get; set; }
        public int SwingCountExit { get; set; }

        public int KAtrHookOrder { get; set; }

        public bool MaxContractsPositionHold { get; set; }
        public int TrendSafeContracts { get; set; }
        public int SafeContractsOvn { get; set; }
        public int SafeContractsOvs { get; set; }

        protected int SafeContractsToRest { get; set; }

        [XmlIgnore] protected int LastSignalId;

        public bool IsMorningSessionEnabled { get; set; }
        public bool IsEveningSessionEnabled { get; set; }

        [XmlIgnore]
        public bool IsAmericanSession { get; private set; }
        public bool IsAmericanSessionDisabled { get; set; }

        protected bool IsAmericanSessionTradingEnabled =>
            (IsAmericanSession && !IsAmericanSessionDisabled) || !IsAmericanSession;

        public bool IsMorningSessionNeedClose => Contracts > SafeContractsOvs;
        public bool IsEveningSessionNeedClose => Contracts > SafeContractsOvn;

        // Condition for Mode1
        public virtual bool IsLongEntryConditionEnabled => true;
        public virtual bool IsShortEntryConditionEnabled => true;
        
        public bool IsLongOnly { get; set; }
        public bool IsShortOnly { get; set; }

        public int StartEntryMode { get; set; }
        // 16.11.26
        // EntryMode on Strategy
        // public int EntryMode { get; set; }

        public bool NeedTrendChangedDelay { get; set; }
        public bool IsMode2Delayed { get; set; }
        public bool IsMode3Delayed { get; set; }

        public bool IsMode4FullExit { get; set; }
        public bool IsMode4SmartExit01 { get; set; }

        public bool IsMaxContracts => IsOpened && Position.Quantity >= Contracts;
        public int MaxContractsReachedMode { get; set; }
        public int StartLongSignal1 { get; set; }
        public int StartLongSignal2 { get; set; }
        public int StartShortSignal1 { get; set; }
        public int StartShortSignal2 { get; set; }

        public float TakeProfitAbsValue { get; set; }
        public float TakeProfitAtrsValue { get; set; }

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

        public int EntrySignal91 { get; set; }
        public int EntrySignal92 { get; set; }
        public int EntrySignal93 { get; set; }
        public int EntrySignal94 { get; set; }

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

        private DateTime _currDT = DateTime.MinValue;
        private DateTime _lastDT = DateTime.MinValue;
        private DateTime _prevItemDT = DateTime.MinValue;

        private TimeSpan _session1StartTime = new TimeSpan(07, 0, 1);
        private TimeSpan _session1EndTime = new TimeSpan(18, 45, 0);
        private TimeSpan _session2StartTime = new TimeSpan(19, 0, 1);
        private TimeSpan _session2EndTime = new TimeSpan(23, 50, 0);

        protected TimeSpan OrderRegisteredExpireTimeOutSec;

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

        protected int _swingCountEntry2;
        protected int _swingCountEntry3;

        public bool IsSwingCountEntryReached => _swingCountEntry >= SwingCountEntry;
        public bool IsSwingCountEntry2Reached => _swingCountEntry2 >= SwingCountEntry;
        public bool IsSwingCountEntry3Reached => _swingCountEntry3 >= SwingCountEntry;

        protected int _swingPrTrendCountEntry;

        private int _swingCountStartEntry;

        protected const int KMinFarFromHere = 3;

        //private double _xBand;
        private double _xBandAtr;

        private Random _random;
        protected bool RandAnswer;
        private int _randValue;
        private int _randCount;

        private bool _waitFlat;
        private float _xProfit;
        private Bars _bars;

        protected Trigger TrMaxContracts;
        [XmlIgnore]
        protected Trigger TrEntryEnabled;
        [XmlIgnore]
        protected Trigger TrEntryEnabled2;
        [XmlIgnore]
        protected Trigger TrEntryEnabled3;
        [XmlIgnore]
        protected Trigger PrimTrendEntryEnabled;
        [XmlIgnore]
        protected Trigger Trend55Changed;

        public bool IsXTrendEntryEnable => TrEntryEnabled != null && TrEntryEnabled.Value && IsSwingCountEntryReached;
        public bool IsXTrend2EntryEnable => TrEntryEnabled2 != null && TrEntryEnabled2.Value && IsSwingCountEntry2Reached;
        public bool IsXTrend3EntryEnable => TrEntryEnabled3 != null && TrEntryEnabled3.Value && IsSwingCountEntry3Reached;

        [XmlIgnore]
        protected Trigger IsPosAbsMaxWasReached;

        public bool IsLong => Position?.IsLong ?? false;
        public bool IsShort => Position?.IsShort ?? false;
        public bool IsNeutral => Position?.IsNeutral ?? false;
        public bool IsOpened => Position?.IsOpened ?? false;

        protected bool IsPrimTrendUp => PrimTrend?.IsUp ?? false;
        protected bool IsPrimTrendDown => PrimTrend?.IsDown ?? false;
        protected bool IsPosUpToPrimTrend => (IsLong && IsPrimTrendUp) || (IsShort && IsPrimTrendDown);

        protected bool IsPrimTrendFlat => PrimTrend?.IsFlat ?? false;
        protected bool IsPrimTrendImpulseUp => PrimTrend?.IsImpulseUp ?? false;
        protected bool IsPrimTrendImpulseDown => PrimTrend?.IsImpulseDown ?? false;

        //protected virtual bool IsPosPositive => Position != null &&
        //                    (
        //                    (Position.IsLong && XTrend.High.IsGreaterOrEqualsThan((double) Position.Price1)) ||
        //                    (Position.IsShort && XTrend.Low.IsLessOrEqualsThan ((double) Position.Price1))
        //                     );
        protected virtual bool IsPosPositive => 
                            (IsLong && XTrend.Ma.IsGreaterOrEqualsThan((double) Position.Price1)) ||
                            (IsShort && XTrend.Ma.IsLessOrEqualsThan((double) Position.Price1));
        protected virtual bool IsPosPositive2 =>
                            (IsLong && XTrend.Ma.IsGreaterOrEqualsThan((double)Position.Price1 - KAtrStop * XTrend.VolatilityUnit)) ||
                            (IsShort && XTrend.Ma.IsLessOrEqualsThan((double)Position.Price1 + KAtrStop * XTrend.VolatilityUnit));
        protected virtual bool IsPosPositive3 =>
                            (IsLong && XTrend.Ma.IsGreaterOrEqualsThan((double)Position.Price1 - KAtrStop * XTrend.VolatilityUnit)) ||
                            (IsShort && XTrend.Ma.IsLessOrEqualsThan((double)Position.Price1 + KAtrStop * XTrend.VolatilityUnit));


        // Old version
        protected bool IsPosPositive1 => Position != null && (
            (Position.IsLong && XTrend.Ma > (double) Position.Price1) ||
            (Position.IsShort && XTrend.Ma < (double) Position.Price1)
            );

        protected bool IsPosNegative => !IsPosPositive;

        protected bool IsSwingNegative => TrEntryEnabled != null && TrEntryEnabled.Value && Position != null &&
                                          ((Position.IsLong && TakeShort(25)) || (Position.IsShort && TakeLong(25)));

        protected bool IsSwingPositive => TrEntryEnabled != null && TrEntryEnabled.Value && Position != null &&
                                          ((Position.IsLong && TakeLong(25)) || (Position.IsShort && TakeShort(25)));

        protected float VolatilityUnit => (float) (Ma1KAtr*(_xAtr?.LastAtrCompleted ?? 0f));
        protected float StopValue => KAtrStop*VolatilityUnit;
        protected float VolatilityUnit2 => (float) (XTrend.High - XTrend.Low)/2f;
        protected float StopValue2 => KAtrStop*VolatilityUnit2;
        protected float StopValue1 => KAtrStop1*VolatilityUnit2;
        protected float TakeProfitValue => KAtrStop2*VolatilityUnit2;
        public bool IsTakeProfitAbsValueReached => !TakeProfitAbsValue.IsEquals(0f) && (
            (IsLong && (XTrend.Ma - (double)Position.Price1).IsGreaterOrEqualsThan(TakeProfitAbsValue)) ||
            (IsShort && (-XTrend.Ma + (double)Position.Price1).IsGreaterOrEqualsThan(TakeProfitAbsValue))
            );
        public bool IsTakeProfitAtrsValueReached => !TakeProfitAtrsValue.IsEquals(0f) && (
            (IsLong && (XTrend.Ma - (double)Position.Price1).IsGreaterOrEqualsThan(TakeProfitAtrsValue * VolatilityUnit2)) ||
            (IsShort && (-XTrend.Ma + (double)Position.Price1).IsGreaterOrEqualsThan(TakeProfitAtrsValue * VolatilityUnit2))
            );

        protected bool IsProfitLevelReached => !KAtrStop2.IsEquals(0f) && (
            (IsLong && (XTrend.Ma - (double)Position.Price1).IsGreaterOrEqualsThan(TakeProfitValue)) ||
            (IsShort && (-XTrend.Ma + (double)Position.Price1).IsGreaterOrEqualsThan(TakeProfitValue))
            );

        // public override float Volatility => Ma1KAtr * VolatilityUnit2;
        public override float Volatility => VolatilityUnit2;

        protected float OrderHookValue => KAtrHookOrder*VolatilityUnit2;
        protected bool IsPositionRiskLow
        {
            get
            {
                return (
                    (Position.IsLong && PositionMax.IsValid && PositionMax.IsGreaterThan((float) XTrend.High2))
                    ||
                    (Position.IsShort && PositionMin.IsValid && PositionMin.IsLessThan((float) XTrend.Low2))
                    );
            }
        }
        protected bool IsLongTermBreakUp
        {
            get
            {
                double high;
                return !XTrend.HaveHigher((int) TimeInt*60*KMinFarFromHere, XTrend.Ma, out high);
            }
        }
        protected bool IsLongTermBreakDown
        {
            get
            {
                double low;
                return !XTrend.HaveLower((int) TimeInt*60*KMinFarFromHere, XTrend.Ma, out low);
            }
        }

        public override string PositionInfo =>
            $"PosMax: {PositionMax.GetValidValue:N2} " +
            $"PosMin: {PositionMin.GetValidValue:N2} " +
            $"PosStop: {PositionStop.GetValidValue:N2}"
            ;

        protected PositionMinMax PositionStop = new PositionMinMax();
        protected PositionMin PositionMin = new PositionMin();
        protected PositionMax PositionMax = new PositionMax();

        //protected PositionMin PosSwingHighTrailingDown = new PositionMin();
        //protected PositionMax PosSwingLowTrailingUp = new PositionMax(); 

        protected PositionMinMax TrailingTrHigh = new PositionMinMax();
        protected PositionMinMax TrailingTrLow = new PositionMinMax();

        public float PositionMedian => PositionMax.IsValid && PositionMin.IsValid
            ? 0.5f*(PositionMax.GetValidValue + PositionMin.GetValidValue)
            : 0f;

        protected float PositionMin2;
        protected float PositionMax2;

        protected int Mode;
        protected int ModeSafe;

        public int ReverseLossCnt;
        protected int RealReverseLossCnt;

        public int RichTarget { get; set; }

        public Z007()
        {
            // need for Serialization
        }
        public Z007(ITradeContext tx, string name, string code, ITicker ticker, uint timeInt)
            : base(tx, name, code, ticker, timeInt)
        {
        }
        public override void Init()
        {
            base.Init();
            if (IsWrong) return;
            // Position.PositionChangedEvent += PositionIsChangedEventHandler;
            //Position2.PositionChangedEvent3 += PositionIsChangedEventHandler2;
            try
            {
                OrderRegisteredExpireTimeOutSec = new TimeSpan(0,0,2*TimeInt);
                
                //_xma018 = TradeContext.RegisterTimeSeries(
                //    new Xma018("Xma18", Ticker, (int)TimeInt, Ma1Length, Ma1AtrLength, Ma1KAtr, Ma1Mode)) as Xma018;
                _xma018 = TradeContext.RegisterTimeSeries(
                    new Xma018("Xma18", Ticker, (int) TimeInt, Ma1Length, MaAtrLength1, MaAtrLength2, Ma1KAtr, Ma1Mode))
                    as Xma018;

                //if (TimeInt2 > 0 && TimeInt2 != TimeInt && Ma2KAtr > 0f)
                //    PrimTrend = TradeContext.RegisterTimeSeries(
                //        new Xma018("Xma18_2", Ticker, (int) TimeInt2, Ma1Length, MaAtrLength1, MaAtrLength2, Ma2KAtr,
                //            Ma1Mode)) as Xma018;
                //else if (Ma2KAtr > 0f)
                //    PrimTrend = TradeContext.RegisterTimeSeries(
                //        new Xma018("Xma18_2", Ticker, (int)TimeInt, Ma1Length, MaAtrLength1, MaAtrLength2, Ma2KAtr,
                //            Ma1Mode)) as Xma018;
                // for Test Only
                LongEnabled = true;
                ShortEnabled = true;

                if (TimeInt3 != 0)
                {

                }
                if (TimeInt2 > 0 && TimeInt2 != TimeInt)
                {
                    if (Ma2KAtrMltp.IsGreaterThan(0f))
                        PrimTrend = TradeContext.RegisterTimeSeries(
                            new Xma018("Xma18_2", Ticker, (int) TimeInt2, Ma1Length, MaAtrLength1, MaAtrLength2,  Ma1KAtr*Ma2KAtrMltp,
                                Ma1Mode)) as Xma018;
                    else if (Ma2KAtr.IsGreaterThan(0f))
                        PrimTrend = TradeContext.RegisterTimeSeries(
                            new Xma018("Xma18_2", Ticker, (int) TimeInt2, Ma1Length, MaAtrLength1, MaAtrLength2, Ma2KAtr,
                                Ma1Mode)) as Xma018;
                    //else if (Ma2KAtr > 0f)
                    //    PrimTrend = TradeContext.RegisterTimeSeries(
                    //        new Xma018("Xma18_2", Ticker, (int) TimeInt, Ma1Length, MaAtrLength1, MaAtrLength2, Ma2KAtr,
                    //            Ma1Mode)) as Xma018;
                    if (Ma1KAtr2.IsGreaterThan(0f))
                        XTrend2 = TradeContext.RegisterTimeSeries(
                            new Xma018("Xma18_2", Ticker, (int)TimeInt, Ma1Length, MaAtrLength1, MaAtrLength2, Ma1KAtr2, Ma1Mode)) as Xma018;
                    if (Ma1KAtr3.IsGreaterThan(0f))
                        XTrend3 = TradeContext.RegisterTimeSeries(
                            new Xma018("Xma18_2", Ticker, (int)TimeInt, Ma1Length, MaAtrLength1, MaAtrLength2, Ma1KAtr3, Ma1Mode)) as Xma018;
                }
                else
                {
                    if (Ma2KAtrMltp.IsGreaterThan(0f))
                    {
                        PrimTrend = TradeContext.RegisterTimeSeries(
                            new Xma018("Xma18_2", Ticker, (int)TimeInt, Ma1Length, MaAtrLength1, MaAtrLength2,
                                Ma1KAtr * Ma2KAtrMltp,
                                Ma1Mode)) as Xma018;
                        //PrimTrend = TradeContext.RegisterTimeSeries(
                        //    new Xma018("Xma18_2", Ticker, (int)68, Ma1Length, MaAtrLength1, MaAtrLength2,
                        //        // Ma1KAtr * Ma2KAtrMltp,
                        //        2.24f, Ma1Mode)) as Xma018;
                    }
                    else if (Ma2KAtr.IsGreaterThan(0f))
                        XTrend2 = TradeContext.RegisterTimeSeries(
                            new Xma018("Xma18_2", Ticker, (int) TimeInt, Ma1Length, MaAtrLength1, MaAtrLength2, Ma2KAtr,
                                Ma1Mode)) as Xma018;
                    else
                    {
                        if (Ma1KAtr2.IsGreaterThan(0f))
                            XTrend2 = TradeContext.RegisterTimeSeries(
                                new Xma018("Xma18_2", Ticker, (int)TimeInt, Ma1Length, MaAtrLength1, MaAtrLength2, Ma1KAtr2, Ma1Mode)) as Xma018;
                        if (Ma1KAtr3.IsGreaterThan(0f))
                            XTrend3 = TradeContext.RegisterTimeSeries(
                                new Xma018("Xma18_2", Ticker, (int)TimeInt, Ma1Length, MaAtrLength1, MaAtrLength2, Ma1KAtr3, Ma1Mode)) as Xma018;
                    }
                }
            }
            catch (Exception e)
            {
                // throw new Exception(e.Message);
                SendException(e);
            }
            if (_xma018 == null)
                throw new Exception(StrategyTickerString + ": Xma016 Initialization Error");

            XTrend = _xma018;

            //_xAtr = XTrend.Atr;
            //_xAtr2 = (Atr)TradeContext.RegisterTimeSeries(new Atr("Atr2", Ticker, (int)TimeInt, 500));

            _xAtr1 = XTrend.Atr1;
            _xAtr2 = XTrend.Atr2;
            _xAtr = XTrend.Atr;

            _primTrendAtr = PrimTrend?.Atr;

            MaxBarsBack = _xAtr.Length;

            _xMainTrend = _xma018;

            if (_xma018 != null)
            {
                _bars = (Bars) (_xma018.SyncSeries);
                Bars = _bars;
            }

            MyReverseSignal1 = ReverseSignal1;
            MyExitSignal1 = ExitSignal1;
            MyEntrySignal1 = EntrySignal1;

            TrMaxContracts = new Trigger();

            TrEntryEnabled = new Trigger();
            TrEntryEnabled2 = new Trigger();
            TrEntryEnabled3 = new Trigger();

            PrimTrendEntryEnabled = new Trigger();

            Trend55Changed = new Trigger();
            IsPosAbsMaxWasReached = new Trigger();

            Mode = 1;

            _random = new Random(DateTime.Now.Millisecond);

            _swingCountEntry = SwingCountEntry - SwingCountStartEntry;

            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                "Strategies", StrategyTickerString, "Signals Available", SignalsAvailable, ToString());

            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                "Strategies", StrategyTickerString, "Init() Is Completed", "", ToString());
        }

        public string SignalsAvailable => "";
        /*
                                          $"EntrySignal11 {EntrySignal11}, " +
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
               */

        protected virtual bool IsPosMinReached =>
            PositionMin != null && PositionMin.HasValue &&
            XTrend != null && XTrend.Ma.IsLessThan(PositionMin.GetValidValue);

        protected virtual bool IsPosMaxReached =>
            PositionMax != null && PositionMax.HasValue &&
            XTrend != null && XTrend.Ma.IsGreaterThan(PositionMax.GetValidValue);

        protected bool IsPosAbsMaxReached =>
            (IsLong && IsPosMaxReached) || (IsShort && IsPosMinReached);

        protected bool IsPosAbsMaxReachedNoValid =>
            (IsLong && !PositionMax.HasValue) || (IsShort && !PositionMin.HasValue);

        protected bool IsLastDealIsLong => LastDeal?.IsLong ?? false;
        protected bool IsLastDealIsShort => LastDeal?.IsShort ?? false;

        //protected double VolatilityUnitsFromPosMax => IsOpened && VolatilityUnit2.IsGreaterThan(0f)
        //        ?   (
        //                IsLong && PositionMax.HasValue
        //                    ? (PositionMax.GetValidValue - XTrend.Ma)/VolatilityUnit2
        //                    :   (
        //                            IsShort && PositionMin.HasValue 
        //                            ? (-PositionMin.GetValidValue + XTrend.Ma) / VolatilityUnit2 
        //                            : 0d
        //                        )                
        //            )
        //        : 0d;

        protected double WarningPriceLevelFromPosMax =>
            IsOpened && VolatilityUnit2.IsGreaterThan(0f) && KVolatilityUnitsFromPosMax.IsGreaterThan(0f)
                ? (
                    IsLong && PositionMax.HasValue
                    ? PositionMax.GetValidValue - KVolatilityUnitsFromPosMax * VolatilityUnit2
                    : (
                        IsShort && PositionMin.HasValue
                        ? PositionMin.GetValidValue + KVolatilityUnitsFromPosMax * VolatilityUnit2
                        : 0d
                       )
                    )
                : 0d;

        protected bool IsWarningPriceLevelFromPosMaxReached => WarningPriceLevelFromPosMax.IsGreaterThan(0d) &&
                        (
                        (IsLong && XTrend.Ma.IsLessThan(WarningPriceLevelFromPosMax)) ||
                        (IsShort && XTrend.Ma.IsGreaterThan(WarningPriceLevelFromPosMax))
                        );

        public override void Clear()
        {
            base.Clear();
            _lastDT = DateTime.MinValue;
        }
        public override void Main()
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            // if (TimePlan.IsTimeToRest) return;
            if (!TimePlan.Enabled) return;

            if (_xma018.Count < 3) return;
            if (_xMainTrend.Count < 1) return;

            //   HookOrder();

            if (_xma018.LastItemCompletedDT.CompareTo(_lastDT) <= 0) return;
            // New Bar
            //if (_lastDT.Date.CompareTo(_xma018.LastItemCompletedDT.Date) < 0 && _xma018.TrendChanged)
            //{
            //    _swingCountEntry++;
            //}
            _currDT = _xma018.LastItemDT;
            _lastDT = _xma018.LastItemCompletedDT;
            _prevItemDT = _xma018.PrevItemCompletedDT;

            Position.SetLastBar(Bars?.LastItemCompleted);

            if (_currDT.Date != _prevItemDT.Date || 
                _prevItemDT.TimeOfDay < _session1StartTime ||
                (_prevItemDT.TimeOfDay > _session1EndTime && _prevItemDT.TimeOfDay <= _session2StartTime ) ||
                // SwingCount on Evening Session last item and previouse should be in the sam sesseion
                (_lastDT.TimeOfDay > _session2StartTime && _prevItemDT.TimeOfDay <= _session2StartTime)
                ) 
                return;

            if (ExitInEmergencyWhenStopUnFilled()) return;

            if (_waitFlat && XTrend.IsFlat) _waitFlat = false;

            _xProfit = Position.Profit;

            if (_xMainTrendValue != _xMainTrend.Trend)
            {
                _xMainTrendValue = _xMainTrend.Trend;
                _randCount++;
            }
            if (PrimTrend != null && PrimTrend.TrendChanged)
            {
                _swingPrTrendCountEntry++;
            }
            if (XTrend2 != null && XTrend2.TrendChanged)
            {
                _swingCountEntry2++;
            }
            if (XTrend3 != null && XTrend3.TrendChanged)
            {
                _swingCountEntry3++;
            }
            if (_xma018.TrendChanged)
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

           // CalcPosMinMax();

            TrMaxContracts.Value = Position.Quantity >= Contracts;
            TrMaxContracts.Value = true;

            if (_swingCountEntry >= SwingCountEntry)
            {
                TrEntryEnabled.SetValue(TakeLong(190) || TakeShort(190));

                //TradeContext.Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, StrategyTickerString,
                //       "TrEntryEnabled", TrEntryEnabled.Value.ToString(), "swCntEntry=" + _swingCountEntry, "");
            }
            if (IsSwingCountEntry2Reached)
            {
                TrEntryEnabled2?.SetValue(TakeLong2(190) || TakeShort2(190));

                //TradeContext.Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, StrategyTickerString,
                //       "TrEntryEnabled", TrEntryEnabled.Value.ToString(), "swCntEntry=" + _swingCountEntry, "");
            }
            if (IsSwingCountEntry3Reached)
            {
                TrEntryEnabled3?.SetValue(TakeLong3(190) || TakeShort3(190));

                //TradeContext.Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, StrategyTickerString,
                //       "TrEntryEnabled", TrEntryEnabled.Value.ToString(), "swCntEntry=" + _swingCountEntry, "");
            }

            IsPosAbsMaxWasReached.SetValue(IsPosAbsMaxReached);

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

            if (IsDailyProfitLimitRiched)
            {
                if(Position.IsOpened)
                    CloseAll(1);
                else if(Position.IsNeutral)
                    SetWorkingStatus(false, "DailyProfitLimitRiched");
                return;
            }
            var oper = Entry();
            if (oper == 0)
            {
                KillOrdersActivated(methodname, "", "No Signals");
                RemoveOrdersRegistered(methodname, "", "No Signals");
                ClearBuySellRequests();
                return;
            }
            if (!IsHedger && IsPortfolioRiskEnable)
            {
                var op = PortfolioRiskOperate(oper);
                // if (SetLimitPriceIfDiffer(op))
                if (op == 0)
                {
                    KillOrdersActivated(methodname, "", "No Signals");
                    RemoveOrdersRegistered(methodname, "", "No Signals");
                    return;
                }
                if (op > 0)
                {
                    //if (_lastLimitBuyPrice.Equals(_currentLimitPrice))
                    //    return;
                    _lastLimitBuyPrice = _currentLimitPrice;

                    //if (ActiveOrderCollection.IsOrderValidExist(OrderTypeEnum.Limit, OrderOperationEnum.Buy,
                    //    _currentLimitPrice))
                    //    return;

                    var cs = GetSize(Mode, LastSignalActivated);
                    SetOrder2(OrderOperationEnum.Buy, cs, _currentLimitPrice);

                    //var cs = GetSize(Mode, LastSignalActivated);
                    //SetOrder2(OrderOperationEnum.Buy, cs);
                    return;
                }
                if (op < 0)
                {
                    //if (_lastLimitSellPrice.Equals(_currentLimitPrice))
                    //    return;
                    _lastLimitSellPrice = _currentLimitPrice;

                    //if (ActiveOrderCollection.IsOrderValidExist(OrderTypeEnum.Limit, OrderOperationEnum.Sell,
                    //   _currentLimitPrice))
                    //    return;

                    var cs = GetSize(Mode, LastSignalActivated);
                    SetOrder2(OrderOperationEnum.Sell, cs, _currentLimitPrice);
                    return;

                    //var cs = GetSize(Mode, LastSignalActivated);
                    //SetOrder2(OrderOperationEnum.Sell, cs);
                    //return;
                }
                if (op == 0)
                {
                    KillOrdersActivated(methodname, "", "No Signals");
                    RemoveOrdersRegistered(methodname, "", "No Signals");
                    return;
                }
                // NoPortfolio
            }
            else
            {
                if (oper == 0)
                { 
                    KillOrdersActivated(methodname, "", "No Signals");
                    RemoveOrdersRegistered(methodname,"", "No Signals");
                    return;
                }
                if (oper < 0)
                {
                    _lastLimitSellPrice = _currentLimitPrice;

                    var operStr =
                            $"Sell {Ticker.Code} @ {_currentLimitPrice} with Signal {LastSignalActivated} in Mode: {Mode}";

                    #region Exist Until 19.09.19
                    //if (ActiveOrderCollection
                    //        .IsOrderValidSoftExist(OrderTypeEnum.Limit, OrderOperationEnum.Sell, _currentLimitPrice))
                    //{
                    //    // Remove Registered in EveryBar, it Should bo Send not Registered
                    //    var orders = ActiveOrderCollection.Items.Where(o => o.IsSell && o.IsLimit && o.IsRegistered);
                    //    foreach (var o in orders)
                    //    {
                    //        if ((DateTime.Now - o.Registered).TotalSeconds <= TimeInt) continue;

                    //        TradeContext.Evlm(EvlResult.WARNING, EvlSubject.TRADING,
                    //            StrategyTickerString, o.ShortInfo + ", Registered Exist", o.ShortDescription,
                    //            operStr, o.ToString());
                    //        TradeContext.Evlm(EvlResult.WARNING, EvlSubject.TRADING,
                    //            StrategyTickerString, o.ShortInfo + " Remove()", o.ShortDescription, operStr,
                    //            o.ToString());

                    //        ActiveOrderCollection.RemoveNoKey(o);
                    //    }

                    //    if (ActiveOrderCollection
                    //        .IsOrderValidSoftExist(OrderTypeEnum.Limit, OrderOperationEnum.Sell, _currentLimitPrice))
                    //    {
                    //        // Pending or Activate
                    //        orders = ActiveOrderCollection.Items.Where(o => o.IsSell && o.IsLimit);
                    //        foreach (var o in orders)
                    //        {
                    //            TradeContext.Evlm(EvlResult.WARNING, EvlSubject.TRADING,
                    //                StrategyTickerString, o.ShortInfo + ", Activated Equals Exist: No Action", o.ShortDescription,
                    //                operStr, o.ToString());
                    //        }
                    //        return;
                    //    }
                    //}
                    #endregion

                    var cs = GetSize(Mode, LastSignalActivated);
                    SetOrder2(OrderOperationEnum.Sell, cs, _currentLimitPrice);
                    return;
                }
                if (oper > 0)
                {
                    _lastLimitBuyPrice = _currentLimitPrice;

                    var operStr =
                            $"Buy {Ticker.Code} @ {_currentLimitPrice} with Signal {LastSignalActivated} in Mode: {Mode}";
                    
                    #region Exist Until 19.09.19
                    //if (ActiveOrderCollection
                    //    .IsOrderValidSoftExist(OrderTypeEnum.Limit, OrderOperationEnum.Buy, _currentLimitPrice))
                    //{
                    //    var orders = ActiveOrderCollection.Items.Where(o => o.IsBuy && o.IsLimit && o.IsRegistered);
                    //    foreach (var o in orders)
                    //    {
                    //        if (!((DateTime.Now - o.Registered).TotalSeconds > TimeInt)) continue;
                    //        TradeContext.Evlm(EvlResult.WARNING, EvlSubject.TRADING,
                    //           StrategyTickerString, o.ShortInfo + ", Registered Exist", o.ShortDescription, operStr, o.ToString());
                    //        TradeContext.Evlm(EvlResult.WARNING, EvlSubject.TRADING,
                    //            StrategyTickerString, o.ShortInfo + " Remove()", o.ShortDescription, operStr, o.ToString());

                    //        ActiveOrderCollection.Remove(o);
                    //    }

                    //    if (ActiveOrderCollection
                    //        .IsOrderValidSoftExist(OrderTypeEnum.Limit, OrderOperationEnum.Buy, _currentLimitPrice))
                    //    {
                    //        // Pending or Activate
                    //        orders = ActiveOrderCollection.Items.Where(o => o.IsBuy && o.IsLimit);
                    //        foreach (var o in orders)
                    //        {
                    //            TradeContext.Evlm(EvlResult.WARNING, EvlSubject.TRADING,
                    //                 StrategyTickerString,  o.ShortInfo + ", Activated Exist: No Action", o.ShortDescription,
                    //                 operStr, o.ToString());
                    //        }
                    //        return;
                    //    }
                    //}
                    #endregion

                    var cs = GetSize(Mode, LastSignalActivated);
                    SetOrder2(OrderOperationEnum.Buy, cs, _currentLimitPrice);
                    return;
                }
            }
            // ClearLongShortRequests();
            // KillAllActiveOrders2();

            _lastLimitBuyPrice = 0.0;
            _lastLimitSellPrice = 0.0;
           
            #endregion
        }

        private long GetSize(int mode, int signalId)
        {
            return Contract;
            //return Mode == 5 && signalId == 19
            //    ? Position.Quantity // all size exit
            //    : Contract;         // partly exit

            //return  Mode == 5 ||
            //        (IsMode4FullExit && (Mode == 6 || Mode == 4) && LastSignalActivated == 19) ||
            //        (
            //        !IsMode4FullExit && IsMode4SmartExit01 && (Mode == 6 || Mode == 4) && LastSignalActivated == 19 &&
            //        ((Position.IsLong && XTrend.TrPos1 > 50d) || (Position.IsShort && XTrend.TrPos1 < 50d)) 
            //        )
            //        ? Position.Quantity // all size exit
            //        : Contract;         // partly exit
        }

        protected override void SetOrder2(OrderOperationEnum tradeoperation, long contract, double price)
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            if (contract <= 0) return;
            if (price.IsLessOrEqualsThan(0d)) return;

            //RemovePreviouseRegisteredOrders(tradeoperation, contract, price);
            RemoveOrdersRegistered(methodname, "", "Registered.ReadyToUse");
            RemovePreviouseActiveOrders(tradeoperation, contract, price);

            var equalOrdersActivated = EqualOrdersActivated(tradeoperation, contract, price).ToList();
            if (equalOrdersActivated.Any(o => o.BusyStatus == BusyStatusEnum.ReadyToUse))
            // if (equalOrdersActivated.Any())
            {
                //PrintOrders(equalOrdersActivated, methodname, "", "Equal Order Already Activated");
                PrintOrders(ActiveOrderCollection.ActiveOrdersSoft, methodname, "", "Equal Order Already Activated");
                return;
            }
            // if (ActiveOrderCollection.ActiveOrdersSoft.Any()) return;
            if (ActiveOrderCollection.OrdersRegisteredInUse.Any())
            {   
                PrintOrders(ActiveOrderCollection.OrdersRegistered, methodname, "", "Registered InUse Exist");
                return;
            }
            if (TradeTerminal.IsWellConnected)
            {
                var ord = ActiveOrderCollection.RegisterOrder(this, tradeoperation, OrderTypeEnum.Limit, 0, price, contract, "");
                if (ord.Status == OrderStatusEnum.NotRegistered)
                {
                    PrintOrder(ord, methodname, $"{OperationEnum.AddNew} {OperationEnum.Failure}", "Not Registered");
                    return;
                }
                if (!ActiveOrderCollection.ActiveOrdersSoft.Any())
                {
                    TradeTerminal.SetLimitOrder(ord);
                    PrintOrder(ord, methodname, "New Registered Send");
                }
                else
                    PrintOrder(ord, methodname, "Any Active Order Exist" , "Only Registered, But NotSended, Pending");
            }
            else
            {
                // No Register when No Connection
                var ord = ActiveOrderCollection.CreateOrder(this, tradeoperation, OrderTypeEnum.Limit, 0, price, contract, "");
                PrintOrder(ord, methodname, OperationEnum.NoConnection.ToString(), "Order Not Registered" );
            }
            PrintOrders(ActiveOrderCollection.Items, methodname, "OrderList");                
        }
        protected  void SetOrder21(OrderOperationEnum tradeoperation, long contract, double price)
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            if (contract <= 0) return;
            if (price.IsLessOrEqualsThan(0d)) return;

            RemovePreviouseActiveOrders(tradeoperation, contract, price);

            if (!TradeTerminal.IsWellConnected)
            {
                RemoveOrdersRegistered(methodname, "", "Registered.ReadyToUse");
                PrintOrders(ActiveOrderCollection.Items, $"{methodname} {OperationEnum.NoConnection}", "OrderList");
                return;
            }
            #region Active Orders
            var isActiveToCancelInUseExists = false;
            // the Same Orders Already Activated
            var equalOrdersActivated = EqualOrdersActivated(tradeoperation, contract, price).ToList();
            if (equalOrdersActivated.Any())
            {
                //var actToCancelInUse = equalOrdersActivated
                //        .Where( o => o.BusyStatus == BusyStatusEnum.InUse &&
                //                o.TransactionAction == OrderTransactionActionEnum.CancelOrder).ToList();
                if (equalOrdersActivated.Any(o => o.BusyStatus == BusyStatusEnum.InUse))
                {
                    isActiveToCancelInUseExists = true;
                    PrintOrders(equalOrdersActivated, methodname, "", "Equal Order Activated TryToCancel Exist");
                }
                if (equalOrdersActivated.Any(o => o.BusyStatus == BusyStatusEnum.ReadyToUse))
                {
                    PrintOrders(equalOrdersActivated, methodname, "", "Equal Order Already Activated");
                    return;
                }
            }
            #endregion
            #region Registered Orders
            // Registered the Same Orders
            var equalOrdersRegistered = EqualOrdersRegistered(tradeoperation, contract, price).ToList();
            if (equalOrdersRegistered.Any())
            {
                // Importan: At First Check ReadyToUse and then InUse
                var regReadyToUse = equalOrdersRegistered.Where(o => o.BusyStatus == BusyStatusEnum.ReadyToUse).ToList();
                if (regReadyToUse.Any())
                {
                    RemoveOrders(regReadyToUse, methodname, "Equal Order Already ReadyToUse");
                }
                var regInUse = equalOrdersRegistered.Where(o => o.BusyStatus == BusyStatusEnum.InUse).ToList();
                if (regInUse.Any())
                {
                    PrintOrders(regInUse, methodname, $"{OperationEnum.NoOperation}", "Equal Order Registered Already InUse");
                    return;
                }
            }
            #endregion
            if (TradeTerminal.IsWellConnected)
            {
                var ord = ActiveOrderCollection.RegisterOrder(this, tradeoperation, OrderTypeEnum.Limit, 0, price, contract, "");
                if (ord.Status == OrderStatusEnum.NotRegistered)
                {
                    PrintOrder(ord, methodname, $"{OperationEnum.AddNew} {OperationEnum.Failure}", "Not Registered");
                    return;
                }
                if (!isActiveToCancelInUseExists)
                {
                    TradeTerminal.SetLimitOrder(ord);
                    PrintOrder(ord, methodname, "New Registered Send");
                }
                else
                    PrintOrder(ord, methodname, $"{ErrorReason.NoConnection}", "New Registered NotSend. Pending");
            }
            else
            {
                var ord = ActiveOrderCollection.CreateOrder(this, tradeoperation, OrderTypeEnum.Limit, 0, price, contract, "");
                PrintOrder(ord, methodname, OperationEnum.NoConnection.ToString(), "Order Not Registered");
            }
            PrintOrders(ActiveOrderCollection.Items, methodname, "OrderList");
        }       
        protected void RemovePreviouseActiveOrders2(OrderOperationEnum tradeoperation, long contract, double price)
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            if (contract <= 0) return;
            if (price.IsLessOrEqualsThan(0d)) return;

            #region Active Orders *****************************************
            var theSameActivated = ActiveOrderCollection.ActiveOrdersSoft
                .Where(ord => ord.Operation == tradeoperation &&
                              ord.Quantity == contract &&
                              ord.LimitPrice.IsEquals(price) &&
                              ord.IsLimit
                              ).ToList();
      
            // ActiveOrdersSoft => Activated, PendingToActivate 
            var notTheSameActivated = ActiveOrderCollection.ActiveOrdersSoft.Except(theSameActivated).ToList();

            var notTheSameActivatedReadyToUse = notTheSameActivated
                .Where(o => o.BusyStatus == BusyStatusEnum.ReadyToUse).ToList();

            var theSameActivatedReadyToUse = theSameActivated
                .Where(o => o.BusyStatus == BusyStatusEnum.ReadyToUse).ToList();
            
            // Kill NotEqual, Remain Equal Orders

            // Clear Previouse NonEqual Active ORDERS 
            if (notTheSameActivatedReadyToUse.Any())
            {
                if (TradeTerminal.IsWellConnected)
                    KillOrders(notTheSameActivatedReadyToUse, methodname, string.Empty, "Previouse Active NonEqual Order");
                else
                    PrintOrders(theSameActivated, methodname, OperationEnum.NoConnection.ToString(),
                                      "Previous NonEqual Order Exist");
            }
            // Remain only coorect qty
            // Clear EXTRA the Equal Orders
            if (theSameActivatedReadyToUse.Count > Contracts)
            {
                if (TradeTerminal.IsWellConnected)
                {
                    var cnt = theSameActivated.Count - Contracts;
                    foreach (var ord in theSameActivated.Where(ord => cnt-- > 0))
                    {
                        KillOrder(ord, methodname, string.Empty,"Extra Previous Equal Order");
                    }
                }
                else
                    PrintOrders(theSameActivated, methodname,OperationEnum.NoConnection.ToString(),
                        "Extra Previouse Equal Active Order Exist");
            }
#endregion
            #region Registered Orders *****************************************
            // RemoveRegisteredIsNotConnected(methodname);

            var theSameRegistered = ActiveOrderCollection.OrdersRegistered
                .Where(ord => ord.Operation == tradeoperation &&
                              ord.Quantity == contract &&
                              ord.LimitPrice.IsEquals(price) &&
                              ord.IsLimit
                              ).ToList();

            var notTheSameRegistered = ActiveOrderCollection.OrdersRegistered.Except(theSameRegistered).ToList();

            var notTheSameRegisteredReadyToUse = notTheSameRegistered
                .Where(o => o.BusyStatus == BusyStatusEnum.ReadyToUse).ToList();

            var theSameRegisteredReadyToUse = theSameRegistered
                .Where(o => o.BusyStatus == BusyStatusEnum.ReadyToUse).ToList();

            // Clear Previouse NonEqual Registered ORDERS 
            if (notTheSameRegisteredReadyToUse.Any())
            {
                RemoveOrders(notTheSameRegisteredReadyToUse, methodname, string.Empty,
                                                            "Previouse NonEqual Registered.ReadyToUse");
            }
            // Clear EXTRA the Equal Orders
            if (theSameRegisteredReadyToUse.Count > Contracts)
            {
                var cnt = theSameRegisteredReadyToUse.Count - Contracts;
                foreach (var ord in theSameRegisteredReadyToUse.Where(ord => cnt-- > 0))
                {
                    RemoveOrder(ord, methodname, string.Empty,"Extra Previous Equal Order.Registered");
                }              
            }
            #endregion
        }
        protected void RemovePreviouseRegisteredOrders(OrderOperationEnum tradeoperation, long contract, double price)
        {
            // 1 Registered,  ReadyToUse, Equal,      Remove All or
            // 2 Registered,  ReadyToUse, Equal,      Remove Extra, 
            // 3 Registered,  ReadyToUse, NonEqual,   Remove
            // 4 Registered,  InUse,      Equal,      Remain, TimeOut
            // 5 Registered,  InUse,      NonEqual,   Remain, TimeOut

            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            if (contract <= 0) return;
            if (price.IsLessOrEqualsThan(0d)) return;

            // 1 Registered,  ReadyToUse, Equal,      Remove All
            // 3 Registered,  ReadyToUse, NonEqual,   Remove
            var readyToUse = ActiveOrderCollection.OrdersRegisteredReadyToUse;
            RemoveOrders(readyToUse, methodname, "", "Registered.ReadyToUse");
        }
        protected void RemovePreviouseActiveOrders(OrderOperationEnum tradeoperation, long contract, double price)
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            if (contract <= 0) return;
            if (price.IsLessOrEqualsThan(0d)) return;

            #region Active Orders *****************************************
            var theSameActivated = ActiveOrderCollection.ActiveOrdersSoft
                .Where(ord => ord.Operation == tradeoperation &&
                              ord.Quantity == contract &&
                              ord.LimitPrice.IsEquals(price) &&
                              ord.IsLimit
                              ).ToList();

            var theNotSameActivatedReadyToUse = ActiveOrderCollection.ActiveOrdersSoft
                .Where(ord => (ord.Operation != tradeoperation ||
                              ord.Quantity != contract ||
                              ord.LimitPrice.IsNoEquals(price)) &&
                              ord.IsLimit && ord.BusyStatus == BusyStatusEnum.ReadyToUse
                              ).ToList();

            var theSameActivatedReadyToUse = theSameActivated
                .Where(o => o.BusyStatus == BusyStatusEnum.ReadyToUse).ToList();

            // ActiveOrdersSoft => Activated, PendingToActivate 
            //var notTheSameActivated = ActiveOrderCollection.ActiveOrdersSoft.Except(theSameActivated).ToList();

            //var notTheSameActivatedReadyToUse = notTheSameActivated
            //    .Where(o => o.BusyStatus == BusyStatusEnum.ReadyToUse).ToList();

            // Remain       Active, ReadyToUse,     Equal Orders
            // Kill Extra   Active, ReadyToUse,     Equal Orders
            // Kill         Active, ReadyToUse,     NonEqual Orders
            // Remain       Active, ReadyToUse,     Equal Orders
            // Remain       Active, ReadyToUse,     NonEqual Orders

            // Kill         Active, ReadyToUse,     NonEqual Orders
            if (theNotSameActivatedReadyToUse.Any())
            {
                if (TradeTerminal.IsWellConnected)
                    KillOrders(theNotSameActivatedReadyToUse, methodname, string.Empty,
                        "Previouse Active NonEqual Order");
                else
                    PrintOrders(theNotSameActivatedReadyToUse, methodname, OperationEnum.NoConnection.ToString(),
                                      "Previous NonEqual Order Exist");
            }
            // Kill Extra Active, ReadyToUse,     Equal Orders
            if (theSameActivatedReadyToUse.Count > Contract)
            {
                if (TradeTerminal.IsWellConnected)
                {
                    var cnt = theSameActivatedReadyToUse.Count - Contract;
                    foreach (var ord in theSameActivatedReadyToUse.Where(ord => cnt-- > 0))
                    {
                        KillOrder(ord, methodname, string.Empty, "Extra Previous Equal Order");
                    }
                }
                else
                    PrintOrders(theSameActivatedReadyToUse, methodname, OperationEnum.NoConnection.ToString(),
                        "Extra Previouse Equal Active Order Exist");
            }
            #endregion
        }
        protected IEnumerable<IOrder3> EqualOrdersActivated(
                        OrderOperationEnum tradeoperation, long contract, double price)
        {
            return ActiveOrderCollection.ActiveOrdersSoft
                        .Where(ord => ord.Operation == tradeoperation &&
                              ord.Quantity == contract &&
                              ord.LimitPrice.IsEquals(price) &&
                              ord.IsLimit
                              );
        }
        protected IEnumerable<IOrder3> EqualOrdersRegistered(
                        OrderOperationEnum tradeoperation, long contract, double price)
        {
            return ActiveOrderCollection.OrdersRegistered
                        .Where(ord => ord.Operation == tradeoperation &&
                              ord.Quantity == contract &&
                              ord.LimitPrice.IsEquals(price) &&
                              ord.IsLimit
                              );
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

            //var contract = Position.Pos == 0 ? 1 : 2;

            /*
            TradeTerminal.KillAllOrders(Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                        OrderTypeEnum.Limit, OperationEnum.All);

            TradeTerminal.KillAllOrders(Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                        OrderTypeEnum.StopLimit, flipOperation);
            */

            RemoveOrdersRegistered("SetOrder2()", string.Empty,"");
            RemoveClosedOrders();

            KillAllActiveOrders2();

            #region ValidOrdersSoft  
            // ValidSoft Order
            //public bool IsValidSoft => Status == OrderStatusEnum.Activated ||
            //                       Status == OrderStatusEnum.Sended ||
            //                       Status == OrderStatusEnum.Registered ||
            //                       Status == OrderStatusEnum.Confirmed ||
            //                       Status == OrderStatusEnum.PartlyFilled ||
            //                       Status == OrderStatusEnum.PendingToActivate;
            #endregion
            #region Pocket Order
            //if (ValidOrdersSoft.Any())
            //{
            //    foreach (var ord in ValidOrdersSoft.ToList())
            //    {
            //        Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY,
            //            StrategyTimeIntTickerString, "Extra" + ord.ShortInfo, ord.ShortDescription,
            //            "SetOrder(Active Or Pending Orders Detected)", ord.ToString());
            //    }

            //     //PocketOrder = ActiveOrderCollection
            //     //               .CreateOrder(this, operation, OrderTypeEnum.Limit, 0, _currentLimitPrice, contract, "");

            //    Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, StrategyTimeIntTickerString,
            //            "Try to Set Pocket" + PocketOrder.ShortInfo, PocketOrder.ShortDescription,
            //             PocketOrder.ToString(), "");

            //    return;
            //}
            #endregion

            var o = ActiveOrderCollection
                        .RegisterOrder(this, operation, OrderTypeEnum.Limit, 0, _currentLimitPrice, contract, "");

            TradeTerminal.SetLimitOrder(o);
        }

        protected IOrder3 PocketOrder;
        public override void SetPocketOrder()
        {
            if (PocketOrder == null)
                return;
            //if (ValidOrdersSoft.Any())
            //    return;
            if (ActiveOrderCollection.Items.Any())
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, StrategyTimeIntTickerString, "Not Alowed Pocket" + PocketOrder.ShortInfo, PocketOrder.ShortDescription, "Not Alowed Due to Orders Exist", PocketOrder.ToString());
                foreach (var o in ActiveOrderCollection.Items)
                {
                    Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, StrategyTimeIntTickerString, "Extra" + o.ShortInfo, o.ShortDescription, o.ToString(), "");
                }
                return;
            }
            ActiveOrderCollection.RegisterOrder(PocketOrder);
            TradeTerminal.SetLimitOrder(PocketOrder);

            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, StrategyTimeIntTickerString, "Set Pocket"+PocketOrder.ShortInfo, PocketOrder.ShortDescription, PocketOrder.ToString(), "");

            PocketOrder = null;
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

        protected virtual int PortfolioRiskOperate(int oper)
        {
            if(!IsPortfolioRiskEnable)
                return 0;

            if (oper < 0 && SellRequest(Contract) > 0)
                return -1;
            /*
            IsOpened ||
            //(Position.IsOpened && SellRequest(Contract) > 0) ||
            //(Position.IsOpened && BuySellRequest(Contract) > 0) ||
            (Position.IsNeutral  && ShortRequest((int) Contract) > 0  
            // && SellRequest(Contract) > 0
            )
            */
            // return -1;

            if (oper > 0 && BuyRequest(Contract) > 0)
                return +1;
            /*
             IsOpened || // Enable Close and Resize
             // (Position.IsOpened && BuyRequest(Contract) > 0) ||
             //(Position.IsOpened && BuySellRequest(Contract) > 0) ||
            (Position.IsNeutral  && LongRequest((int) Contract) > 0  
            // && BuyRequest(Contract) > 0
            )
            */
            // return +1;

            ClearLongShortRequests();

            PortfolioRisk.ClearBuyRequest(this);
            PortfolioRisk.ClearSellRequest(this);
            PortfolioRisk.ClearBuySellRequest(this);

            return 0;
        }

        private bool SetLimitPriceIfDiffer(int oper)
        {
            if (oper > 0)
            {
                if (_lastLimitBuyPrice.Equals(_currentLimitPrice))
                    return false;
                _lastLimitBuyPrice = _currentLimitPrice;
                return true;
            }
            if (oper < 0)
            {
                if (_lastLimitSellPrice.Equals(_currentLimitPrice))
                    return false;
                _lastLimitSellPrice = _currentLimitPrice;
                return true;
            }
          //  _lastLimitBuyPrice = 0.0;
          //  _lastLimitSellPrice = 0.0;
            return false;
        }

        protected virtual void Prefix()
        {
            // CalcPosMinMax();
        }

        protected virtual void CalcPosMinMax()
        {
            //05.04.2018
            if (Position.IsNeutral)
            {
                //PosSwingHighTrailingDown.Clear();
                //PosSwingLowTrailingUp.Clear();
                PositionMax.Clear();
                PositionMin.Clear();
            }

            // 04.04.2018 to Calculate PosMinMax
            
            if (Position.IsShort)
            {
                if (XTrend.IsDown)
                {
                    if (PositionMax.SetIfLessThan((float)XTrend.High2))
                        PositionStop.Value = PositionMax.GetValidValue;
                }
                else if (XTrend.IsUp)
                {
                    PositionMin.SetIfGreaterThan((float)XTrend.Low2);

                    //PosSwingLowTrailingUp.SetIfLessThan((float) XTrend.Low2);
                }
                TrailingTrHigh.SetIfGreaterThan((float)XTrend.TrHigh1);
            }
            else if (Position.IsLong)
            {
                if (XTrend.IsUp)
                {
                    if (PositionMin.SetIfGreaterThan((float)XTrend.Low2))
                        PositionStop.Value = PositionMin.GetValidValue;
                }
                else if (XTrend.IsDown)
                {
                    PositionMax.SetIfLessThan((float)XTrend.High2);

                    //PosSwingHighTrailingDown.SetIfGreaterThan((float) XTrend.High2);
                }

                TrailingTrLow.SetIfLessThan((float)XTrend.TrLow1);
            }           
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

        protected int LastSignalActivated;

        //protected bool TakeLongPrimTrend(int index)
        //{
        //    return PrimTrend?.TakeLong(index) ?? false;
        //}
        //protected bool TakeShortPrimTrend(int index)
        //{
        //    return PrimTrend?.TakeShort(index) ?? false;
        //}
        protected bool TakeLong2(int index)
        {
            //return XTrend2?.TakeLong(index) ?? false;
            LastSignalActivated = 0;
            if (XTrend2 == null || !XTrend2.TakeLong(index)) return false;
            LastSignalActivated = index;
            return true;
        }
        protected bool TakeShort2(int index)
        {
            // return XTrend2?.TakeShort(index) ?? false;
            LastSignalActivated = 0;
            if (XTrend2 == null || !XTrend2.TakeShort(index)) return false;
            LastSignalActivated = index;
            return true;
        }
        protected bool TakeLong3(int index)
        {
            // return XTrend3?.TakeLong(index) ?? false;
            LastSignalActivated = 0;
            if (XTrend3 == null || !XTrend3.TakeLong(index)) return false;
            LastSignalActivated = index;
            return true;
        }
        protected bool TakeShort3(int index)
        {
            // return XTrend3?.TakeShort(index) ?? false;
            LastSignalActivated = 0;
            if (XTrend3 == null || !XTrend3.TakeShort(index)) return false;
            LastSignalActivated = index;
            return true;
        }
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
                var safeQv = Position.Quantity - SafeContractsToRest;
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
                            // 22.09.2017
                            price = mode == 11 ? Ticker.MarketPriceSell : Ticker.Ask;
                            // price = mode == 11 ? Ticker.MarketPriceSell : Ticker.Bid;
                            //quantity = (Position.Quantity - SafeContractsOvn) > 0 ? (Position.Quantity - SafeContracts) : 0;
                            quantity = safeQv;
                        }
                        else if (Position.IsShort)
                        {
                            operation = OrderOperationEnum.Buy;
                            // 22.09.2017
                            price = mode == 11 ? Ticker.MarketPriceBuy : Ticker.Bid;
                            // price = mode == 11 ? Ticker.MarketPriceBuy : Ticker.Ask;
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
                // throw;
            }
        }
        public override void SkipTheTick(int n)
        {
            // _swingCountEntry = n;
            if(_swingCountEntry > 0)
            // if(_swingCountEntry >= SwingCountEntry && _swingCountEntry > 0)
                    _swingCountEntry = SwingCountEntry - n;
            TrEntryEnabled.Reset();
            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, "Strategies:", StrategyTickerString,
                        System.Reflection.MethodBase.GetCurrentMethod().Name,
                        $"MA: {XTrend.Ma}, High: {XTrend.High}, Low: {XTrend.Low}","");
        }
        public void SkipTheTick1(int n)
        {
            // _swingCountEntry = n;
            _swingCountEntry = SwingCountEntry - n;
            TrEntryEnabled.Reset();
            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, "Strategies:", StrategyTickerString,
                        System.Reflection.MethodBase.GetCurrentMethod().Name,
                        $"MA: {XTrend.Ma}, High: {XTrend.High}, Low: {XTrend.Low}", "");
        }

        public void ClearPocketOrders()
        {
            PocketOrder = null;
        }
        protected bool EveningSessionClosing;
        private double _lastTradeShortMaValue;
        private double _lastTradeLongMaValue;

        public override void StartNewDayInit()
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";

            Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString,
                        methodname, "", "");

            SetLongShortEnabled(true);

            EveningSessionClosing = false;

            _swingCountEntry = SwingCountEntry - SwingCountStartEntry;
            _swingCountEntry2 = SwingCountEntry - SwingCountStartEntry;
            _swingCountEntry3 = SwingCountEntry - SwingCountStartEntry;

            // _swingCountEntry = SwingCountStartEntry;
            TrEntryEnabled?.Reset();
            TrEntryEnabled2?.Reset();
            TrEntryEnabled3?.Reset();

            PrimTrendEntryEnabled?.Reset();

            _swingCountStartEntry = SwingCountStartEntry;

            ClearPocketOrders();

            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, "Strategies:", StrategyTickerString,
                        methodname, "Init Session",
                        $"SwingCountStartEntry: {_swingCountStartEntry}");
        }
        public bool IsSwingCountStartEntryReached => _swingCountStartEntry <= 0;

        public virtual void EnterToSafeExitMode()
        {
            Mode = 9;
        }

        public void AskRandom()
        {
            if (_randCount <= 0) return;
            _randCount = 0;

            _randValue = _random.Next(1, 101);
            RandAnswer = _randValue > RandMode;

            TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, Code,  "AskRandom()","RandLevel: " + RandMode ,
                $"Value={_randValue}, Answer={RandAnswer}", "");
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
