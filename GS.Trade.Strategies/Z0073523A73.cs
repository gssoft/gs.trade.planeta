using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;
using GS.Interfaces;
using GS.Triggers;

namespace GS.Trade.Strategies
{
    public class Z0073523A73 : Z007
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

        private bool _safeModeCalcEnable;
        private bool _richTargetCalcEnable;

        private bool _richTarget1;
        private bool _richTarget2;
        private bool _richTarget3;
        private bool _richTarget5;

        private bool _richTargetSafeCond;

        private bool _tryToReverseLoss;
        private bool _reverseLossMode;
        private int _reverseLossCnt;

        private int _mode;
        private int _modeLast;

        private bool _modeSafe;
        private int _tryEnterMode;
        private int _incrLoss;

        private float _positionStop;
        private float _positionMax;
        private float _positionMin;

        private TradeOperationEnum _lastTradeOperation;
        private float _lastTradeEntryPrice;

        private bool _maxContracts = false;

        private bool IsMyPositionRiskLow
        {
            get
            {
                return true;
                /*
                return (
                           (
                           Position.IsLong && 
                           (
                           (PositionMaxValid && ((double)PositionMax).CompareTo(XTrend.High2) > 0)) || IsLongTermBreakDown
                           )
                           ||
                           (
                           Position.IsShort &&
                           (
                           (PositionMinValid && ((double)PositionMin).CompareTo(XTrend.Low2) < 0)) || IsLongTermBreakUp
                           )
                       );
                 */
            }
        }
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

            _richTarget1 = false;
            _richTarget2 = false;
            _richTarget3 = false;
            _richTarget5 = false;

            _richTargetCalcEnable = false;
            _safeModeCalcEnable = false;

            _richTargetSafeCond = false;

            _modeSafe = false;

            RichTarget = 0;

            // if (np != 0)
            if (np.IsOpened)
            {
                if (_tryEnterMode > 0)
                {
                    _mode = _tryEnterMode;
                    _tryEnterMode = 0;

                    if (_incrLoss > 0)
                        _reverseLossCnt = +_incrLoss;
                    else
                        _reverseLossCnt = 0;

                    _incrLoss = 0;
                }
                else if (_tryEnterMode < 0)
                {
                    _tryEnterMode = 0;
                    _reverseLossCnt = 0;
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
                _maxContracts = false;
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
            RealReverseLossCnt = _reverseLossCnt;
            // Mode = _mode;
            ModeSafe = _modeSafe ? 1 : 0;

        }

        protected override void Prefix()
        {
            CalcPosMinMax();

            ChangeMode123();
        }
        // 2018.05.31
        // Static Init SL (ISL) swCnt = 0, Smart Running SL (RSL) move only if IsLong && IsUp
        // RSL = if(IsLong && RSL < Low2) then SL = RSL esle SL = RSLprev;
        // RSL does not move if(IsLong && RSL >= Low2)
        protected override void CalcPosMinMax()
        {
            //05.04.2018
            if (Position.IsNeutral)
            {
                //PosSwingHighTrailingDown.Clear();
                //PosSwingLowTrailingUp.Clear();
                PositionMax.Clear();
                PositionMin.Clear();
                PositionStop.Clear();
            }
            // 30.05.2018 to Calculate PosMinMax
            // if (_swingCountEntry <= 0) // Var: 4. Running SL where swCntentry == 0 
            // Static Init SL, not Running, Set at once/ only FirstTime swCnt = 0
            // if (_swingCountEntry <= 0 && !PositionStop.HasValue) // Var : 1,2,3: Running SL only one time at first swCntentry == 0
            if (!PositionStop.HasValue)
            {      
                if (IsShort)
                    PositionStop.Value = (float)XTrend.Ma + StopValue2;
                else if (IsLong)
                    PositionStop.Value = (float)XTrend.Ma - StopValue2;
                return;
            }

            //if (_swingCountEntry <= 0)
            //    return;

            if (Position.IsShort)
            {
                if (XTrend.IsDown)
                {
                    //if (PositionMin.HasValue)
                    //{
                    //    var v = PositionMin.GetValidValue + StopValue2;
                    //    if (v.IsGreaterThan((float)XTrend.High2))
                    //        PositionStop.Value = v;

                    //    // var 2: SL stay at previous place
                    //    //else
                    //    //    PositionStop.Value = (float)XTrend.High2 + VolatilityUnit2;
                    //}
                    //if (PositionMax.SetIfLessThan((float)XTrend.High2))
                    //    PositionStop.Value = PositionMax.GetValidValue;

                    PositionMax.SetIfLessThan((float) XTrend.High2);
                }
                else if (XTrend.IsUp)
                {
                    // Set PosMin
                    if (_swingCountEntry > 0)
                        PositionMin.SetIfGreaterThan((float) XTrend.Low2);

                    //if (PositionMin.SetIfGreaterThan((float) XTrend.Low2))
                    //    PositionStop.Value = PositionMin.GetValidValue + StopValue2;

                    //PosSwingLowTrailingUp.SetIfLessThan((float) XTrend.Low2);
                }
                TrailingTrHigh.SetIfGreaterThan((float)XTrend.TrHigh1);
            }
            else if (Position.IsLong)
            {
                if (XTrend.IsUp)
                {
                    //if (PositionMax.HasValue)
                    //{
                    //    var v = PositionMax.GetValidValue - StopValue2;
                    //    if(v.IsLessThan((float)XTrend.Low2))
                    //        PositionStop.Value = v;
                    //    // var 2: SL stay at previous place
                    //    //else
                    //    //    PositionStop.Value = (float)XTrend.Low2 - VolatilityUnit2;
                    //}
                    PositionMin.SetIfGreaterThan((float) XTrend.Low2);
                }
                else if (XTrend.IsDown)
                {
                    // set PosMax
                    if(_swingCountEntry > 0)
                        PositionMax.SetIfLessThan((float)XTrend.High2);

                    //if ( PositionMax.SetIfLessThan((float)XTrend.High2))
                    //    PositionStop.Value = PositionMax.GetValidValue - StopValue2; 

                    //PosSwingHighTrailingDown.SetIfGreaterThan((float) XTrend.High2);
                }
                //09.04.2018
                TrailingTrLow.SetIfLessThan((float)XTrend.TrLow1);
            }
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

            // 18.04.2018

            //if (Position.IsNeutral && LastSignalActivated == 19 && _swingCountEntry < 1)
            //    return 0;
            // if (Position.IsNeutral && (LastSignalActivated == 19 || LastSignalActivated == 1011 || LastSignalActivated == 1010))

            // 18.08.17
            //if (Position.IsNeutral && (LastSignalActivated == 19 || LastSignalActivated == 1011)) // && _swingCountEntry < 1)                                                                                                                    // if (Position.IsNeutral && LastSignalActivated == 19)
            //{
            //    //if (IsNeutral && Math.Abs(LastTradeMaValue - XTrend.Ma).IsLessThan(VolatilityUnit2 * 0.0d))
            //    //    return 0;
            //    if (IsNeutral && Math.Abs(LastTradeMaValue - XTrend.Ma).IsLessThan(VolatilityUnit2 * 0.5d))
            //        return 0;
            //}

            //if (IsNeutral && Math.Abs(LastTradeMaValue - XTrend.Ma).IsLessThan(VolatilityUnit2 * 0.5d))
            //    return 0;

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
                         if (ShortEntry(0))
                            return -1;
                        if (LongEntry(0))
                            return +1;
                        break;
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
                //if(Mode == 5)
                //    System.Diagnostics.Debug.Print($"Mode: {Mode}");

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

                if (_swingCountEntry < SwingCountEntry)
                    return 0;

                if (TrendMode == -1)
                {
                    //if (XTrend.TrendHigh5 > 0)
                    //if ( XTrend.TrendHigh5 > 0 ||  XTrend.TrendLow5 > 0)
                    //if (PrimTrend?.IsDown2 ?? false)
                    if (PrimTrend?.IsDown ?? false)
                    {
                        if (LongEntry(0))
                            return +1;
                    }
                    // if (XTrend.TrendLow5 < 0)
                    //if (XTrend.TrendHigh5 < 0  || XTrend.TrendLow5 < 0)
                    //if (PrimTrend?.IsUp2 ?? false)
                    if (PrimTrend?.IsUp ?? false)
                    {
                        if (ShortEntry(0))
                            return -1;
                    }
                }
                else
                {
                    //if (XTrend.TrendHigh5 > 0)
                    //if ( XTrend.TrendHigh5 > 0 ||  XTrend.TrendLow5 > 0)
                    // if(PrimTrend?.IsUp2 ?? false)
                    if(PrimTrend?.IsUp ?? false)
                    {
                        if (LongEntry(0))
                            return +1;
                    }
                    // if (XTrend.TrendLow5 < 0)
                    //if (XTrend.TrendHigh5 < 0  || XTrend.TrendLow5 < 0)
                    //if (PrimTrend?.IsDown2 ?? false)
                    if (PrimTrend?.IsDown ?? false)
                    {
                        if (ShortEntry(0))
                            return -1;
                    }
                }

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

            //if (LastTradeMaValue.IsEquals(XTrend.Ma,0))
            //    return false;

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
            // 16.11.09 
            // Main Condition. No All Restrictions
            //if (!TrEntryEnabled.Value)
            //    return false;
            if (
                (signal == 0 && Mode == 1 && (TakeShort(EntrySignal11) || TakeShort(EntrySignal12))) ||
                (signal == 0 && Mode == 2 && Position.IsShort && (TakeShort(EntrySignal21) || TakeShort(EntrySignal22))) ||
                (signal == 0 && Mode == 2 && Position.IsLong && (TakeShort(EntrySignal23) || TakeShort(EntrySignal24))) ||
                (signal == 0 && Mode == 3 && Position.IsShort && (TakeShort(EntrySignal31) || TakeShort(EntrySignal32))) ||
                (signal == 0 && Mode == 3 && Position.IsLong && (TakeShort(EntrySignal33) || TakeShort(EntrySignal34))) ||
                (signal == 0 && Mode == 5 && Position.IsShort && (TakeShort(EntrySignal51) || TakeShort(EntrySignal52))) ||
                (signal == 0 && Mode == 5 && Position.IsLong && (TakeShort(EntrySignal53) || TakeShort(EntrySignal54))) ||
                (signal == 0 && Mode == 4 && Position.IsShort && (TakeShort(EntrySignal41) || TakeShort(EntrySignal42))) ||
                (signal == 0 && Mode == 4 && Position.IsLong && (TakeShort(EntrySignal43) || TakeShort(EntrySignal44) || TakeShort(EntrySignal45))) ||
                (signal == 0 && Mode == 8 && Position.IsShort && (TakeShort(EntrySignal81) || TakeShort(EntrySignal82))) ||
                (signal == 0 && Mode == 8 && Position.IsLong && (TakeShort(EntrySignal83) || TakeShort(EntrySignal84) || TakeShort(EntrySignal85))) ||
                (signal == 0 && Mode == 9 && Position.IsShort && (TakeShort(EntrySignal91) || TakeShort(EntrySignal92))) ||
                (signal == 0 && Mode == 9 && Position.IsLong && (TakeShort(EntrySignal93) || TakeShort(EntrySignal94))) ||
                (signal != 0 && TakeShort(signal))
                )
            {
                //if (Mode == 5)
                //{
                //    System.Diagnostics.Debug.Print($"GS.Trade Mode : {Mode}, LastSignal: {LastSignalActivated}");
                //}

                if (XTrend.GetPrice6(-1, EntryPriceId, LastSignalActivated, out _currentLimitPrice, out _comment))
                {
                    _comment = "Short Entry:[" + entrySignal + "] " + _comment;
                    LastSignalId = entrySignal;
                    _tryEnterMode = Mode;
                    //  _positionMax = ((float)_currentLimitPrice + StopValue2);
                    //  _positionStop = _positionMax; 

                    // var sig = TakeShort(entrySignal) ? entrySignal : (TakeShort(EntrySignal13) ? EntrySignal13 : entrySignal2);
                    //var sig = signal != 0 ? signal : (TakeShort(EntrySignal11) ? EntrySignal11 : (TakeShort(EntrySignal12) ? EntrySignal12 : EntrySignal13));
                    var descrStr =
                    $"Ticker: {Ticker.Code},  Signal: {LastSignalActivated}, Mode: {Mode}, Side: {"Sell"}, @ Price: {_currentLimitPrice}, Position: {Position.PositionString3}";

                    var operStr = $"Sell {Ticker.Code} @ {_currentLimitPrice} with Signal {LastSignalActivated} in Mode: {Mode}";

                    TradeContext.Evlm(EvlResult.INFO, EvlSubject.TRADING, StrategyTickerString, "Signal: " + LastSignalActivated,
                        operStr, descrStr, "");

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

            //if (LastTradeMaValue.IsEquals(XTrend.Ma,0))
            //    return false;

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
            // 16.11.09 
            // Main Condition. No All Restrictions
            //if (!TrEntryEnabled.Value)
            //    return false;
            if (
                (signal == 0 && Mode == 1 && (TakeLong(EntrySignal11) || TakeLong(EntrySignal12))) ||
                (signal == 0 && Mode == 2 && Position.IsLong && (TakeLong(EntrySignal21) || TakeLong(EntrySignal22))) ||
                (signal == 0 && Mode == 2 && Position.IsShort && (TakeLong(EntrySignal23) || TakeLong(EntrySignal24))) ||
                (signal == 0 && Mode == 3 && Position.IsLong && (TakeLong(EntrySignal31) || TakeLong(EntrySignal32))) ||
                (signal == 0 && Mode == 3 && Position.IsShort && (TakeLong(EntrySignal33) || TakeLong(EntrySignal34))) ||
                (signal == 0 && Mode == 5 && Position.IsLong && (TakeLong(EntrySignal51) || TakeLong(EntrySignal52))) ||
                (signal == 0 && Mode == 5 && Position.IsShort && (TakeLong(EntrySignal53) || TakeLong(EntrySignal54))) ||
                (signal == 0 && Mode == 4 && Position.IsLong && (TakeLong(EntrySignal41) || TakeLong(EntrySignal42))) ||
                (signal == 0 && Mode == 4 && Position.IsShort && (TakeLong(EntrySignal43) || TakeLong(EntrySignal44) || TakeLong(EntrySignal45))) ||
                (signal == 0 && Mode == 8 && Position.IsLong && (TakeLong(EntrySignal81) || TakeLong(EntrySignal82))) ||
                (signal == 0 && Mode == 8 && Position.IsShort && (TakeLong(EntrySignal83) || TakeLong(EntrySignal84) || TakeLong(EntrySignal85))) ||
                (signal == 0 && Mode == 9 && Position.IsLong && (TakeLong(EntrySignal91) || TakeLong(EntrySignal92))) ||
                (signal == 0 && Mode == 9 && Position.IsShort && (TakeLong(EntrySignal93) || TakeLong(EntrySignal94))) ||
                (signal != 0 && TakeLong(signal))
                )
            {
                //if (Mode == 5)
                //{
                //    System.Diagnostics.Debug.Print($"GS.Trade Mode : {Mode}, LastSignal: {LastSignalActivated}");
                //}

                if (XTrend.GetPrice6(+1, EntryPriceId, LastSignalActivated ,out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Entry:[" + entrySignal + "] " + _comment;
                    LastSignalId = entrySignal;
                    _tryEnterMode = Mode;

                    //var sig = signal != 0 ? signal : (TakeLong(EntrySignal11) ? EntrySignal11 : (TakeLong(EntrySignal12) ? EntrySignal12 : EntrySignal13));
                    //var sigStr = "LongEntry(" + LastSignalActivated + ")";

                    var descrStr =
                        $"Ticker: {Ticker.Code},  Signal: {LastSignalActivated}, Mode: {Mode}, Side: {"Buy"}, Price {_currentLimitPrice}, Position: {Position.PositionString3}";


                    var operStr = $"Buy {Ticker.Code} @ {_currentLimitPrice} with Signal {LastSignalActivated} in Mode: {Mode}";

                    TradeContext.Evlm(EvlResult.INFO, EvlSubject.TRADING, StrategyTickerString, "Signal: " +  LastSignalActivated,
                        operStr,descrStr,"");
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
                    LastExitMode = _mode;
                    var sig = "ShortExit(" + ExitSignal1 + ")";
                    TradeContext.Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, StrategyTickerString, "Signal", sig,
                        sig + " @ " + _currentLimitPrice, String.Format("Mode: {0} TrMaxCntr: {1} Sig: {2}",
                                                                                    Mode, TrMaxContracts, sig));

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
                    LastExitMode = _mode;
                    var sig = "LongExit(" + ExitSignal1 + ")";
                    TradeContext.Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, StrategyTickerString, "Signal", sig,
                        sig + " @ " + _currentLimitPrice, String.Format("Mode: {0} TrMaxCntr: {1} Sig: {2}",
                                                                                    Mode, TrMaxContracts, sig));
                    return (int)Contract;
                }
            }
            return 0;
        }


        //protected override bool IsPosPositive
        //{
        //    get
        //    {
        //        return Position != null &&
        //            (
        //            (Position.IsLong && XTrend.IsUp && ((double)Position.Price1).IsLessThan((XTrend.Ma + kAtrStop * VolatilityUnit2))) ||
        //            (Position.IsLong && XTrend.IsDown && ((double)Position.Price1).IsLessThan((XTrend.Ma))) ||
        //            (Position.IsShort && XTrend.IsDown && ((double)Position.Price1).IsGreaterThan((XTrend.Ma - kAtrStop * VolatilityUnit2))) ||
        //            (Position.IsShort && XTrend.IsUp && ((double)Position.Price1).IsGreaterThan((XTrend.Ma)))
        //            );
        //    }
        //}
        //protected override bool IsPosPositive 
        //    => Position != null &&
        //        (
        //            (Position.IsLong && XTrend.IsUp && ((double)Position.Price1).IsLessThan(XTrend.Ma + kAtrStop19 * VolatilityUnit2)) ||
        //            (Position.IsLong && XTrend.IsDown && ((double)Position.Price1).IsLessThan(XTrend.Ma + kAtrStop25 * VolatilityUnit2)) ||
        //            (Position.IsShort && XTrend.IsDown && ((double)Position.Price1).IsGreaterThan(XTrend.Ma - kAtrStop19 * VolatilityUnit2)) ||
        //            (Position.IsShort && XTrend.IsUp && ((double)Position.Price1).IsGreaterThan(XTrend.Ma - kAtrStop25 * VolatilityUnit2))
        //         );

        protected bool IsProfitLevelReached => !KAtrStop2.IsEquals(0f) && (
            (IsLong && (XTrend.Ma - (double) Position.Price1).IsGreaterOrEqualsThan(TakeProfitValue)) ||
            (IsShort && (-XTrend.Ma + (double) Position.Price1).IsGreaterOrEqualsThan(TakeProfitValue))
            );

        private int _previousMode;
        private int _tr;
        private void ChangeMode123()
        {
            var previousMode = Mode;
            if (Position.IsOpened )
            {
                if (Position.Quantity < Contracts)
                {
                    if (Mode == 1 && _previousMode == 0)
                        Mode = 2;
                    else
                    {
                        Mode = IsPosPositive ? 2 : 8;
                    }
                }
                else if (Position.Quantity >= Contracts)
                {
                    if (IsPosPositive)
                    {
                        if (IsPosAbsMaxReached)
                            // Mode = 5;
                            Mode = Mode == 9 ? 9 : 5;
                        else
                        {
                            if ((Mode == 5 && !IsPosAbsMaxReached) || Mode == 9)
                            {
                                Mode = 9;
                                return;
                            }

                            if (PositionStop.HasValue)
                            {
                                if (
                                    (Position.IsLong && XTrend.Ma.IsLessThan(PositionStop.GetValidValue)) ||
                                    (Position.IsShort && XTrend.Ma.IsGreaterThan(PositionStop.GetValidValue))
                                    )
                                    Mode = 3;
                                else
                                {
                                    //Mode = _swingCountEntry > 0 ? 8 : 7;
                                    if (_swingCountEntry > 0)
                                        Mode = 8;
                                    else
                                    {
                                        Mode = IsProfitLevelReached || IsTakeProfitAbsValueReached ? 2 : 7;
                                    }
                                }
                            }
                            else
                                Mode = 7;
                        }
                    }
                    // Position is Negative
                    else
                    {
                        if (PositionStop.HasValue)
                        {
                            if (
                                (Position.IsLong && XTrend.Ma.IsLessThan(PositionStop.GetValidValue)) ||
                                (Position.IsShort && XTrend.Ma.IsGreaterThan(PositionStop.GetValidValue))
                                )
                                Mode = 4;
                            else
                                Mode = _swingCountEntry > 0 ? 8 : 7;
                        }
                        else
                            Mode = 6;
                    }
                }
                if (previousMode != Mode)
                    _previousMode = previousMode;
            }
            else if (Position.IsNeutral)
            {
                Mode = 1;
                _previousMode = 0;
            }
        }
    }
}

