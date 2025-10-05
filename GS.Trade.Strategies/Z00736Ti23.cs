using System;
using System.Reflection;
using GS.Extension;
using GS.Interfaces;
using GS.Reflection;

namespace GS.Trade.Strategies
{
    public class Z00736Ti23 : Z007
    {
        public int TrendMode { get; set; }
        public int SafeContracts2 { get; set; }
        public int SafeContracts3 { get; set; }
        public int ExitSignal12 { get; set; }
        public int ReverseSignal12 { get; set; }

        public int FlatCount { get; set; }
        public int CancelOrderMode { get; set; }
        public int CancelReverseMode { get; set; }

        public int RichHL { get; set; }

        public int RichTargetMode11 { get; set; }
        public int RichTargetMode12 { get; set; }
        public int RichTargetMode13 { get; set; }
        public int RichTargetMode15 { get; set; }

        public int RichTargetMode21 { get; set; }
        public int RichTargetMode22 { get; set; }
        public int RichTargetMode23 { get; set; }
        public int RichTargetMode25 { get; set; }

        public int RichTargetMode31 { get; set; }
        public int RichTargetMode32 { get; set; }

        public int RichTargetMode41 { get; set; }
        public int RichTargetMode42 { get; set; }

        public int RichTargetMode51 { get; set; }

        public int RichTargetSafeCond1 { get; set; }
        public int RichTargetSafeCond2 { get; set; }

        public int ExitSignal3 { get; set; }
        public int ReverseSignal3 { get; set; }

        public int ExitSignal4 { get; set; }
        public int ReverseSignal4 { get; set; }

        public int ExitSignal51 { get; set; }
        public int ReverseSignal51 { get; set; }

        public int ExitSignal52 { get; set; }
        public int ReverseSignal52 { get; set; }

        public int ChangeMode1 { get; set; }
        public int ChangeMode2 { get; set; }
        public int ChangeMode3 { get; set; }

        public int ChangeModeCondSignal1 { get; set; }
        public int ChangeModeCondSignal2 { get; set; }
        public int ChangeModeCondSignal3 { get; set; }
        public int ChangeModeCondSignal5 { get; set; }

        public int ReverseMode1 { get; set; }
        public int ReverseMode2 { get; set; }
        public int ReverseMode3 { get; set; }
        public int ReverseMode5 { get; set; }

        public int RiskLowMap { get; set; }

        public int ZMode { get; set; }

        public int TakeExitMode { get; set; }

        private string _comment;

       

        private bool _modeSafe;
        private int _tryEnterMode;
        private int _incrLoss;

        private float _positionStop;
        private float _positionMax;
        private float _positionMin;

        
        private bool IsEntryRiskLow(int operation)
        {
            double low, high;

            switch (operation)
            {
                case +1:
                    if (XTrend.HaveLower((int)TimeInt * 60 * KMinFarFromHere, XTrend.Ma, out low))
                    {
                        var r = XTrend.Ma - low;
                        if (r.CompareTo((double)VolatilityUnit2 * KAtrStop1) < 0)
                        {
                            _positionStop = ((float)low - VolatilityUnit2 * KAtrStop2);
                            _positionMin = (float)low;
                            return true;
                        }
                    }
                    break;
                case -1:
                    if (XTrend.HaveHigher((int)TimeInt * 60 * KMinFarFromHere, XTrend.Ma, out high))
                    {
                        var r = high - XTrend.Ma;
                        if (r.CompareTo((double)VolatilityUnit2 * KAtrStop1) < 0)
                        {
                            _positionStop = ((float)high + VolatilityUnit2 * KAtrStop2);
                            _positionMax = (float)high;
                            return true;
                        }
                    }
                    break;
            }
            return false;
        }

        public bool IsUnSafeContracts => Position.IsOpened && Position.Quantity > SafeContracts2;
        public bool IsSafeContracts => !IsUnSafeContracts;
        public bool IsMaxContracts => Position.IsOpened && Position.Quantity >= Contracts;

        protected override void PositionIsChangedEventHandler2(IPosition2 op, IPosition2 np, PositionChangedEnum changedResult)
        {
            base.PositionIsChangedEventHandler2(op, np, changedResult);

          _modeSafe = false;

            RichTarget = 0;

            // if (np != 0)
            if (np.IsOpened)
            {
                if (_tryEnterMode > 0)
                {
                   

                    _incrLoss = 0;
                }
                else if (_tryEnterMode < 0)
                {
                    _tryEnterMode = 0;
                   // _reverseLossCnt = 0;
                    _incrLoss = 0;
                }

                //   if( _positionStop.CompareTo(0f) == 0)
                //       throw new NullReferenceException(
                //          String.Format("Str: {0} SigId: {1} {2}", Code, LastSignalId,"PositionStop is Null"));


            }
            else
            {
                // _reverseLossCnt = 0;
                // _incrLoss = 0;
               // _maxContracts = false;
                _positionStop = float.MaxValue;
                _positionMax = float.MinValue;
                _positionMin = float.MaxValue;
            }

            if (changedResult == PositionChangedEnum.Opened ||
                changedResult == PositionChangedEnum.ReSizedUp)
            {
                //if (np.IsLong)
                //{
                //    PositionMin.Value = ((float)Position.Price1 - StopValue2);
                //    PositionStop.Value = PositionMin.Value;
                //}
                //else if (np.IsShort)
                //{
                //    PositionMax.Value = ((float)Position.Price1 + StopValue2);
                //    PositionStop.Value = PositionMax.Value;
                //}
            }

            if (np.IsOpened)
            {
                _swingCountEntry = 0;
                if (np.Quantity >= Contracts)
                {
                    if (np.IsLong)
                    {
                        //  PositionMin.Clear();
                        PositionStop.Clear();
                    }
                    else if (np.IsShort)
                    {
                        //  PositionMax.Clear();
                        PositionStop.Clear();
                    }
                }
                else
                {
                    //  PositionMin.Clear();
                    //  PositionMax.Clear();
                    PositionStop.Clear();
                    //  _maxContracts = false;
                }
                ChangeMode123();

            }
            //RealReverseLossCnt = _reverseLossCnt;
            // Mode = _mode;
            ModeSafe = _modeSafe ? 1 : 0;

        }

        protected override void Prefix()
        {
            ChangeMode123();
        }

        protected override bool StartShortEntry()
        {
            return _swingCountEntry >= SwingCountStartEntry && ShortEntry();

            var entrySignal = 0;
            var entryMode = 0;
            switch (StartEntryMode)
            {
                case 0:
                case 1:
                    entrySignal = EntrySignal1;
                    entryMode = 1;
                    break;
                case 2:
                    entrySignal = EntrySignal2;
                    entryMode = 2;
                    break;
                case -2:

                    break;
            }

            if (entrySignal <= 0)
                throw new NullReferenceException("EntrySignal==0");

            if (TakeShort(entrySignal) && _swingCountEntry >= SwingCountStartEntry)
            {
                if (XTrend.GetPrice5(-1, EntryPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Start Short Entry:[" + entrySignal + "] " + _comment;
                    LastSignalId = entrySignal;
                    _tryEnterMode = entryMode;
                    return true;
                }
                if (CancelOrderMode > 0)
                {
                    LastSignalId = 0;
                    KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Sell);
                }
            }
            return false;

        }
        protected override bool StartLongEntry()
        {
            return _swingCountEntry >= SwingCountStartEntry && LongEntry();

            int entrySignal = 0;
            int entryMode = 0;
            switch (StartEntryMode)
            {
                case 0:
                case 1:
                    entrySignal = EntrySignal1;
                    entryMode = 1;
                    break;
                case 2:
                    entrySignal = EntrySignal2;
                    entryMode = 2;
                    break;
            }

            if (entrySignal <= 0)
                throw new NullReferenceException("EntrySignal==0");

            if (TakeLong(entrySignal) && _swingCountEntry >= SwingCountStartEntry)
            {
                if (XTrend.GetPrice5(+1, EntryPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Entry:[" + entrySignal + "] " + _comment;
                    LastSignalId = entrySignal;
                    _tryEnterMode = entryMode;
                    return true;
                }
                if (CancelOrderMode > 0)
                {
                    LastSignalId = 0;
                    KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Buy);
                }
            }
            return false;
        }

        protected override int Entry()
        {

            //if ((Position.IsOpened && Position.Quantity < Contracts) || Position.IsNeutral)
            //{
            //    var longOrShort = LongOrShort();
            //    if (longOrShort < 0)
            //    {
            //        if (ShortEntry())
            //            return -1;
            //    }
            //    else if (longOrShort > 0)
            //    {
            //        if (LongEntry())
            //            return +1;
            //    }
            //    return 0;
            //}
            if (Position.IsOpened && Position.Quantity < Contracts)
            {
                switch (Mode)
                {
                    // <= safeContract
                    case 2:
                        //if (TrendMode * XTrend.Trend55.Value > 0)
                        //{
                        //    if (Position.IsLong)
                        //    {
                        //        if (ShortEntry(0))
                        //            return -1;
                        //        if (LongEntry(0))
                        //            return +1;
                        //    }
                        //    if (Position.IsShort)
                        //    {
                        //        if (ShortEntry(0))
                        //            return -1;
                        //        if (LongEntry(0))
                        //            return +1;
                        //    }
                        //}
                        //else if (TrendMode * XTrend.Trend55.Value < 0)
                        //{
                        //    if (Position.IsLong)
                        //    {
                        //        if (LongEntry(0))
                        //            return +1;
                        //        if (ShortEntry(0))
                        //            return -1;
                        //    }
                        //    if (Position.IsShort)
                        //    {
                        //        if (LongEntry(0))
                        //            return +1;
                        //        if (ShortEntry(0))
                        //            return -1;
                        //    }
                        //}
                        //if (ShortEntry(0))
                        //    return -1;
                        //if (LongEntry(0))
                        //    return +1;
                        if (Position.IsLong)
                        {
                            if (ShortEntry(0))
                                return -1;
                            if (IsLongEnabled && LongEntry(0))
                                return +1;
                        }
                        if (Position.IsShort)
                        {
                            if (IsShortEnabled && ShortEntry(0))
                                return -1;
                            if (LongEntry(0))
                                return +1;
                        }
                        return 0;
                        // break;
                    case 3:
                        if (ShortEntry(0))
                            return -1;
                        if (LongEntry(0))
                            return +1;
                        break;
                    case 8:
                    case 4:
                        if (ShortEntry(0))
                            return -1;
                        if (LongEntry(0))
                            return +1;
                        break;
                }
            }
            else if (Position.IsOpened && Position.Quantity >= Contracts)
            {
                if (Position.IsLong)
                {
                    if (ShortEntry(0))
                        return -1;
                    // return 0;
                }
                if (Position.IsShort)
                {
                    if (LongEntry(0))
                        return +1;
                    //  return 0;
                }
            }
            else if (Position.IsNeutral)
            {
                //if (TrendMode * XTrend.Trend55.Value > 0)
                //{
                //    if (LongEntry(0))
                //        return +1;
                //}
                //else if (TrendMode * XTrend.Trend55.Value < 0)
                //{
                //    if (ShortEntry(0))
                //        return -1;
                //}
                //if (!IsHedger || (IsHedger && IsHedgerEnabled))
                //{
                   if (IsLongEnabled && LongEntry(0))
                     // if (LongEntry(0))
                        return +1;
                    if (IsShortEnabled && ShortEntry(0))
                      // if (ShortEntry(0))
                        return -1;
                //}
                //else
                //{
                //    return 0;
                //}

                //if (LongEntry(0))
                //    return +1;
                //if (ShortEntry(0))
                //    return -1;
            }
            return 0;
        }
        private int LongOrShort()
        {
            if (XTrend.IsUp2)
            {
                var h = XTrend.IsUp ? XTrend.High : XTrend.High2;
                double high2;
                if (XTrend.HaveHigher((int)TimeInt * 60 * KMinFarFromHere, h, out high2))
                {
                    var dlow = XTrend.Ma - XTrend.Low2;
                    var dhigh = high2 - XTrend.Ma;
                    if (dhigh.IsGreaterThan(dlow))
                        return EntrySignal12;
                    return -1 * EntrySignal11;
                }
                return -1 * (EntrySignal13 == 0 ? EntrySignal11 : EntrySignal13);
            }
            if (XTrend.IsDown2)
            {
                var l = XTrend.IsDown ? XTrend.Low : XTrend.Low2;
                double low2;
                if (XTrend.HaveLower((int)TimeInt * 60 * KMinFarFromHere, l, out low2))
                {
                    var dhigh = XTrend.High2 - XTrend.Ma;
                    var dlow = XTrend.Ma - low2;
                    if (dlow.IsGreaterThan(dhigh))
                        return -1 * EntrySignal12;
                    return EntrySignal11;
                }
                return EntrySignal13 == 0 ? EntrySignal11 : EntrySignal13;
            }
            return 0;
        }
        private bool ShortEntry(int signal)
        {
            int entrySignal = 0;
            int entrySignal2 = 0;
            int entryMode = 0;
            if (EntryMode == 1 || EntryMode == 0)
            {
                entrySignal = EntrySignal11;
                entrySignal2 = EntrySignal12;
                entryMode = 1;
            }


            if (entrySignal <= 0 && entrySignal2 <= 0)
                throw new NullReferenceException("EntrySignal==0");

            var swCntEntry = SwingCountEntry;
            if (EntryMode == 1 && SwingCountEntry > 2)
            {
                if (Position.IsOpened)
                {
                    if (Position.Quantity >= Contracts)
                        swCntEntry = SwingCountEntry == 3 ? 1 : 2;
                    else
                        swCntEntry = SwingCountEntry == 3 ? 2 : 1;
                }
                else
                {
                    swCntEntry = 2;
                }
            }
            int temp;
            if (signal < 0)
                temp = 1;

            //if (MorningSessionStarting)
            //{
            //    if (XTrend.Ma.IsEquals(MorningSessionStartWorkingTrendMaValue))
            //        return false;
            //} 
            //if (!TrEntryEnabled.Value && Mode != 8)
            //    return false;
            //if (XTrend.Ma.IsEquals(LastTradeMaValue))
            //    return false;
            if (
                (IsXTrendEntryEnable && signal == 0 && Mode == 1 && (TakeShort(EntrySignal11) || TakeShort(EntrySignal12))) ||
                //(TrEntryEnabled.Value && signal == 0 && Mode == 2 && Position.IsShort && (TakeShort(EntrySignal21) || TakeShort(EntrySignal22))) ||
                (IsXTrendEntryEnable && signal == 0 && Mode == 2 && Position.IsShort && (TakeShort2(EntrySignal21) || TakeShort2(EntrySignal22))) ||
                (IsXTrend3EntryEnable && signal == 0 && Mode == 2 && Position.IsLong && (TakeShort(EntrySignal23) || TakeShort(EntrySignal24))) ||
                (IsXTrendEntryEnable && signal == 0 && Mode == 3 && Position.IsShort && (TakeShort(EntrySignal31) || TakeShort(EntrySignal32))) ||
                (IsXTrend2EntryEnable && signal == 0 && Mode == 3 && Position.IsLong && (TakeShort2(EntrySignal33) || TakeShort2(EntrySignal34))) ||
                (IsXTrendEntryEnable && signal == 0 && Mode == 5 && Position.IsShort && (TakeShort(EntrySignal51) || TakeShort(EntrySignal52))) ||
                (IsXTrend3EntryEnable && signal == 0 && Mode == 5 && Position.IsLong && (TakeShort(EntrySignal53) || TakeShort(EntrySignal54))) ||
                (IsXTrendEntryEnable && signal == 0 && Mode == 4 && Position.IsShort && (TakeShort(EntrySignal41) || TakeShort(EntrySignal42))) ||
                (IsXTrend2EntryEnable && signal == 0 && Mode == 4 && Position.IsLong && (TakeShort2(EntrySignal43) || TakeShort2(EntrySignal44))) ||
                (IsXTrendEntryEnable && signal == 0 && Mode == 8 && Position.IsShort && (TakeShort(EntrySignal81) || TakeShort(EntrySignal82))) ||
                (IsXTrend3EntryEnable && signal == 0 && Mode == 8 && Position.IsLong && (TakeShort(EntrySignal83) || TakeShort(EntrySignal84))) ||
                (IsXTrendEntryEnable && signal == 0 && Mode == 9 && Position.IsLong && (TakeShort(EntrySignal93) || TakeShort(EntrySignal94))) ||
                (signal != 0 && TakeShort(signal))
                )
            {
                if (XTrend.GetPrice6(-1, EntryPriceId, LastSignalActivated, out _currentLimitPrice, out _comment))
                {
                    _comment = "Short Entry:[" + entrySignal + "] " + _comment;
                    LastSignalId = entrySignal;
                    _tryEnterMode = Mode;
                    //  _positionMax = ((float)_currentLimitPrice + StopValue2);
                    //  _positionStop = _positionMax; 

                    // var sig = TakeShort(entrySignal) ? entrySignal : (TakeShort(EntrySignal13) ? EntrySignal13 : entrySignal2);
                    //var sig = signal != 0 ? signal : (TakeShort(EntrySignal11) ? EntrySignal11 : (TakeShort(EntrySignal12) ? EntrySignal12 : EntrySignal13));
                    var sigStr = "ShortEntry(" + LastSignalActivated + ")";
                    TradeContext.Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, StrategyTickerString, "Signal", sigStr,
                        sigStr + " @ " + _currentLimitPrice,
                        $"Mode: {Mode} TrMaxCntr: {TrMaxContracts} Sig: {sigStr}");

                    //if (Mode == 1 && signal == EntrySignal12)
                    //    sig = EntrySignal12;
                    //else if (Mode == 1 && signal == EntrySignal13)
                    //    sig = EntrySignal13;
                    //else if (Mode == 1 && signal == EntrySignal11)
                    //    sig = EntrySignal11;

                    return true;
                }
                if (CancelOrderMode > 0)
                {
                    LastSignalId = 0;
                    // KillOrders2(OrderTypeEnum.Limit, OrderOperationEnum.Sell);
                }
            }
            return false;
        }
        private bool LongEntry(int signal)
        {
            int entrySignal = 0;
            int entrySignal2 = 0;
            int entryMode = 0;
            if (EntryMode == 1 || EntryMode == 0)
            {
                entrySignal = EntrySignal11;
                entrySignal2 = EntrySignal12;
                entryMode = 1;
            }

            if (entrySignal <= 0 && entrySignal2 <= 0)
                throw new NullReferenceException("EntrySignal==0");

            var swCntEntry = SwingCountEntry;
            if (EntryMode == 1 && SwingCountEntry > 2)
            {
                if (Position.IsOpened)
                {
                    if (Position.Quantity >= Contracts)
                        swCntEntry = SwingCountEntry == 3 ? 1 : 2;
                    else
                        swCntEntry = SwingCountEntry == 3 ? 2 : 1;
                }
                else
                {
                    swCntEntry = 2;
                }
            }

            int temp;
            if (signal < 0)
                temp = 1;
            // Mode 2 and 8 Positive
            // Mode 3 and 4 Negative
            //if (!TrEntryEnabled.Value && Mode != 8)
            //    return false;
            if (
                (IsXTrendEntryEnable && signal == 0 && Mode == 1 && (TakeLong(EntrySignal11) || TakeLong(EntrySignal12))) ||
                // Positive Size Up
                (IsXTrendEntryEnable && signal == 0 && Mode == 2 && Position.IsLong && (TakeLong(EntrySignal21) || TakeLong(EntrySignal22))) ||
                // (IsXTrend2EntryEnable && signal == 0 && Mode == 2 && Position.IsLong && (TakeLong2(EntrySignal21) || TakeLong2(EntrySignal22))) ||
                // Positive Exit 
                (IsXTrend3EntryEnable && signal == 0 && Mode == 2 && Position.IsShort && (TakeLong(EntrySignal23) || TakeLong(EntrySignal24))) ||
                // Negative Size Up
                (IsXTrendEntryEnable && signal == 0 && Mode == 3 && Position.IsLong && (TakeLong(EntrySignal31) || TakeLong(EntrySignal32))) ||
                // Negative Exit
                (IsXTrend2EntryEnable && signal == 0 && Mode == 3 && Position.IsShort && (TakeLong2(EntrySignal33) || TakeLong2(EntrySignal34))) ||
                (IsXTrendEntryEnable && signal == 0 && Mode == 5 && Position.IsLong && (TakeLong(EntrySignal51) || TakeLong(EntrySignal52))) ||
                (IsXTrend3EntryEnable && signal == 0 && Mode == 5 && Position.IsShort && (TakeLong(EntrySignal53) || TakeLong(EntrySignal54))) ||
                (IsXTrendEntryEnable && signal == 0 && Mode == 4 && Position.IsLong && (TakeLong(EntrySignal41) || TakeLong(EntrySignal42))) ||
                // Negative Exit
                (IsXTrend2EntryEnable && signal == 0 && Mode == 4 && Position.IsShort && (TakeLong2(EntrySignal43) || TakeLong2(EntrySignal44))) ||
                (IsXTrendEntryEnable && signal == 0 && Mode == 8 && Position.IsLong && (TakeLong(EntrySignal81) || TakeLong(EntrySignal82))) ||
                (IsXTrend3EntryEnable && signal == 0 && Mode == 8 && Position.IsShort && (TakeLong(EntrySignal83) || TakeLong(EntrySignal84))) ||
                (IsXTrendEntryEnable && signal == 0 && Mode == 9 && Position.IsShort && (TakeLong(EntrySignal93) || TakeLong(EntrySignal94))) ||
                (signal != 0 && TakeLong(signal))
                )
            {
                if (XTrend.GetPrice6(+1, EntryPriceId, LastSignalActivated, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Entry:[" + entrySignal + "] " + _comment;
                    LastSignalId = entrySignal;
                    _tryEnterMode = Mode;

                    //var sig = signal != 0 ? signal : (TakeLong(EntrySignal11) ? EntrySignal11 : (TakeLong(EntrySignal12) ? EntrySignal12 : EntrySignal13));
                    var sigStr = "LongEntry(" + LastSignalActivated + ")";

                    TradeContext.Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, StrategyTickerString, "Signal", sigStr,
                        sigStr + " @ " + _currentLimitPrice,
                        $"Mode: {Mode} TrMaxCntr: {TrMaxContracts} Sig: {sigStr}");
                    //if (Mode == 1 && signal == EntrySignal12)
                    //    sig = EntrySignal12;
                    //else if (Mode == 1 && signal == EntrySignal13)
                    //    sig = EntrySignal13;
                    //else if (Mode == 1 && signal == EntrySignal11)
                    //    sig = EntrySignal11;

                    return true;
                }
                if (CancelOrderMode > 0)
                {
                    LastSignalId = 0;
                    //KillOrders2(OrderTypeEnum.Limit, OrderOperationEnum.Buy);
                }
            }
            return false;
        }
        protected override bool ShortExit()
        {
            return false;
        }
        protected override bool ShortStopExit()
        {
            return false;
        }

        protected override int ShortExit2()
        {

            if (TrMaxContracts.Value && PositionStop.IsLessThan((float)XTrend.Ma) && TakeLong(ExitSignal1))
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _tryEnterMode = -1;
                    // LastExitMode = _mode;
                    var sig = "ShortExit(" + ExitSignal1 + ")";
                    TradeContext.Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, StrategyTickerString, "Signal", sig,
                        sig + " @ " + _currentLimitPrice, $"Mode: {Mode} TrMaxCntr: {TrMaxContracts} Sig: {sig}");

                    return (int)Contract;
                }
            }
            return 0;
        }
        protected override bool LongExit()
        {
            return false;
        }
        protected override bool LongStopExit()
        {
            return false;
        }
        protected override int LongExit2()
        {

            if (TrMaxContracts.Value && PositionStop.IsGreaterThan((float)XTrend.Ma) && TakeShort(ExitSignal1))
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _tryEnterMode = -1;
                    // LastExitMode = _mode;
                    var sig = "LongExit(" + ExitSignal1 + ")";
                    TradeContext.Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, StrategyTickerString, "Signal", sig,
                        sig + " @ " + _currentLimitPrice,
                        $"Mode: {Mode} TrMaxCntr: {TrMaxContracts} Sig: {sig}");
                    return (int)Contract;
                }
            }
            return 0;
        }
        private bool ChangeMode()
        {
            //if (_swingCountEntry < SwingCountEntry)
            //    return false;

            //if (
            //    (Position.IsLong && TakeLong(19) && _swingCountEntry >= SwingCountEntry)
            //    ||
            //    (Position.IsShort && TakeShort(19) && _swingCountEntry >= SwingCountEntry)
            //    )
            //{
            //    Mode = 1;
            //   // _swingCountEntry = 0;
            //}
            //if
            //    (
            //   (Position.IsLong && (Position.Price1).IsLessThan((decimal) XTrend.Ma))
            //   //(Position.IsLong && PositionMax.IsLessThan((float)XTrend.Ma))
            //    ||
            //   (Position.IsShort && (Position.Price1).IsGreaterThan((decimal) XTrend.Ma))
            //   // (Position.IsShort && PositionMin.IsGreaterThan((float)XTrend.Ma))
            //    )
            //{
            //    Mode = 2;
            //    // _swingCountEntry = 0;
            //}
            //else
            //{
            //    Mode = 1;
            //}
            //Mode = _mode;


            return false;
        }
        private int _previousMode;
        private int _tr;
        private void ChangeMode123()
        {
            if (NeedTrendChangedDelay && _tr != XTrend.Trend55.Value)
            {
                SkipTheTick(0);
                _tr = XTrend.Trend55.Value;
            }
            var previousMode = Mode;
            if (Position.IsOpened )
            {
                if (EveningSessionClosing)
                {
                    if (XTrend.Ma.IsNoEquals(EveningSessionClosingTrendMaValue) &&
                        XTrend.Ma.IsNoEquals(LastTradeMaValue))
                    {
                        Mode = 8;
                        // return;
                    }
                    else
                        Mode = 4;
                }
                else
                {
                    if (Position.Quantity < Contracts)
                    {
                        //Mode = MaxContractsReached && MaxContractsReachedMode == 2 && IsPosAbsMaxReached 
                        //    ? 5 
                        //    : 2;
                        Mode = IsPosPositive2 ? 2 : 3;
                    }
                    else if (Position.Quantity >= Contracts)
                    {
                        MaxContractsReached = true;

                        Mode = IsPosPositive2 ? 8 : 4;
                    }
                    //if (previousMode != Mode)
                    //    _previousMode = previousMode;
                }
            }
            else if (Position.IsNeutral)
            {
                MaxContractsReached = false;
                Mode = 1;
                _previousMode = 0;
            }          
        }
        public override void EveningSessionFinishPrefix()
        {
            if (!IsHedger)
            {
                LongEnabled = false;
                ShortEnabled = false;
            }

            EveningSessionClosing = true;
            EveningSessionClosingTrendMaValue = XTrend.Ma;

            Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString, StrategyTimeIntTickerString,
                MethodBase.GetCurrentMethod().Name + "()", "Entering to The EveningSessionFinishing Mode=8", "Good Bye");
        }

        public override void EveningSessionFinishPostfix()
        {
            EveningSessionClosing = false;
        }
    }
}

