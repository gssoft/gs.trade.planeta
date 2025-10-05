using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.Strategies
{
    public class X11837 : X118
    {
        public int EntrySignal11 { get; set; }
        public int EntrySignal12 { get; set; }
        public int EntrySignal13 { get; set; }

        public int ExitSignal12 { get; set; }
        public int ReverseSignal12 { get; set; }

        public int FlatCount { get; set; }
        public int CancelOrderMode { get; set; }
        public int CancelReverseMode { get; set; }

        public int RichHL { get; set; }

        public int RichTargetMode11 { get; set; }
        public int RichTargetMode12 { get; set; }
        public int RichTargetMode13 { get; set; }
        public int RichTargetMode14 { get; set; }
        public int RichTargetMode15 { get; set; }

        public int RichTargetMode21 { get; set; }
        public int RichTargetMode22 { get; set; }
        public int RichTargetMode23 { get; set; }
        public int RichTargetMode24 { get; set; }
        public int RichTargetMode25 { get; set; }

        public int RichTargetMode31 { get; set; }
        public int RichTargetMode32 { get; set; }

        public int RichTargetMode41 { get; set; }
        public int RichTargetMode42 { get; set; }

        public int RichTargetMode51 { get; set; }

        public int RichTargetMode61 { get; set; }
        public int RichTargetMode62 { get; set; }

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

        public int ExitSignal6 { get; set; }
        public int ReverseSignal6 { get; set; }

        public int ExitStopSignal { get; set; }

        public int ChangeMode1 { get; set; }
        public int ChangeMode2 { get; set; }
        public int ChangeMode3 { get; set; }
        public int ChangeMode4 { get; set; }

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

        private bool _reverse2;
        // private int _reverse2Cnt;

        private bool _safeModeCalcEnable;
        private bool _richTargetCalcEnable;

        private bool _richTarget1;
        private bool _richTarget2;
        private bool _richTarget3;
        private bool _richTarget4;
        private bool _richTarget5;
        private bool _richTarget6;

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
                    if (XTrend.HaveLower((int)TimeInt * 60 * KMinFarFromHere, XTrend.Low, out low))
                    {
                        var r = XTrend.Low - low;
                        if (r.CompareTo((double)VolatilityUnit2 * KAtrStop1) < 0)
                        {
                            _positionStop = ((float)low - VolatilityUnit2 * KAtrStop2);
                            _positionMin = (float)low;
                            return true;
                        }
                        else
                        {
                            _positionStop = (float)XTrend.Low - VolatilityUnit2 * (KAtrStop1 + KAtrStop2);
                            _positionMin = (float)XTrend.Low - VolatilityUnit2 * KAtrStop1;
                            return true;
                        }
                    }
                    break;
                case -1:
                    if (XTrend.HaveHigher((int)TimeInt * 60 * KMinFarFromHere, XTrend.High, out high))
                    {
                        var r = high - XTrend.High;
                        if (r.CompareTo((double)VolatilityUnit2 * KAtrStop1) < 0)
                        {
                            _positionStop = ((float)high + VolatilityUnit2 * KAtrStop2);
                            _positionMax = (float)high;
                            return true;
                        }
                        else
                        {
                            _positionStop = (float)XTrend.High + VolatilityUnit2 * (KAtrStop1 + KAtrStop2);
                            _positionMax = (float)XTrend.High + VolatilityUnit2 * KAtrStop1;
                            return true;
                        }
                    }
                    break;
            }
            return false;
        }
        private bool IsEntryRiskLow2(int operation)
        {
            switch (operation)
            {
                case +1:
                    double low;
                    if (!XTrend.HaveLower((int)TimeInt * 60 * KMinFarFromHere, XTrend.Ma, out low))
                    {
                        _positionStop = (float)XTrend.Ma - VolatilityUnit2 * (KAtrStop1 + KAtrStop2);
                        _positionMin = (float)XTrend.Ma - VolatilityUnit2 * KAtrStop1;
                        return true;
                    }
                    break;
                case -1:
                    double high;
                    if (!XTrend.HaveHigher((int)TimeInt * 60 * KMinFarFromHere, XTrend.Ma, out high))
                    {
                        _positionStop = (float)XTrend.Ma + VolatilityUnit2 * (KAtrStop1 + KAtrStop2);
                        _positionMax = (float)XTrend.Ma + VolatilityUnit2 * KAtrStop1;
                        return true;
                    }
                    break;
            }
            return false;
        }

        protected override void PositionChanged(long op, long np)
        {
            _richTarget1 = false;
            _richTarget2 = false;
            _richTarget3 = false;
            _richTarget5 = false;

            _richTargetCalcEnable = false;
            _safeModeCalcEnable = false;

            _richTargetSafeCond = false;

            _modeSafe = false;

            RichTarget = 0;

            if (np != 0)
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

                if (np > 0)
                    PositionMin = _positionMin;
                else if (np < 0)
                    PositionMax = _positionMax;

                PositionStop = _positionStop;

                    EntryEnabled = false;
            }
            else
            {
                // _reverseLossCnt = 0;
                // _incrLoss = 0;
                _positionStop = 0;
                _positionMax = 0;
                _positionMin = 0;

                EntryEnabled = false;
            }
            RealReverseLossCnt = _reverseLossCnt;
            Mode = _mode;
            ModeSafe = _modeSafe ? 1 : 0;

        }

        protected override void Prefix()
        {
            if (Position.IsShort)
            {
                if (!PositionMaxValid)
                {
                    double m;
                    if (XTrend.HaveHigher((int)TimeInt * 60 * KMinFarFromHere, XTrend.High, out m))
                        PositionMax = (float)m;
                }
                /*
                if( ! PositionStopValid)
                    PositionStop = PositionMaxValid
                                   ? Math.Max(PositionMax, ((float)Position.Price1) + StopValue1)
                                   : ((float)Position.Price1) + StopValue1;
                */
                
            }
            else if (Position.IsLong)
            {
                if (!PositionMinValid)
                {
                    double m;
                    if (XTrend.HaveLower((int)TimeInt * 60 * KMinFarFromHere, XTrend.Low, out m))
                        PositionMin = (float)m;
                }
                /*
                if( ! PositionStopValid )
                    PositionStop = PositionMinValid
                                    ? Math.Min(PositionMin, ((float)Position.Price1) - StopValue1)
                                    : ((float)Position.Price1) - StopValue1;
                */
            }
            else if (Position.IsNeutral)
            {
                if (!EntryEnabled && _swingCountEntry >= SwingCountEntry)
                {
                    EntryEnabled = TakeLong(25) || TakeShort(25);
                    if (EntryEnabled)
                        _swingCountEntry = SwingCountEntry - 1;
                }
            }

            double high, low;
            if (RichHL == 1)
            {
                high = XTrend.High;
                low = XTrend.Low;
            }
            else
            {
                high = XTrend.Ma;
                low = XTrend.Ma;
            }
            if (!_richTargetCalcEnable)
            {
                if (Position.IsLong && XTrend.IsDown)
                {
                    _richTargetCalcEnable = true;
                }
                else if (Position.IsShort && XTrend.IsUp)
                {
                    _richTargetCalcEnable = true;
                }
            }
            if (!_safeModeCalcEnable)
            {
                if (Position.IsLong && XTrend.IsUp)
                {
                    _safeModeCalcEnable = true;
                }
                else if (Position.IsShort && XTrend.IsDown)
                {
                    _safeModeCalcEnable = true;
                }
            }

            if (_mode == 1 && _richTargetCalcEnable)
            {
                switch (RichTargetMode11)
                {
                    case 0:
                        _richTarget1 = false;
                        break;
                    case 1:
                        _richTarget1 = true;
                        break;
                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTarget1 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget1 = true;
                        }
                        break;
                    case 10:
                        if (Position.IsLong && ((double)PositionMax2).CompareTo(XTrend.Low) < 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && ((double)PositionMin2).CompareTo(XTrend.High) > 0)
                        {
                            _richTarget1 = true;
                        }
                        break;
                }
                switch (RichTargetMode12)
                {
                    case 0:
                        _richTarget2 = false;
                        break;
                    case 1:
                        _richTarget2 = true;
                        break;
                    case 2:
                        if (Position.IsLong && XTrend.IsUp)
                        {
                            _richTarget2 = true;
                        }
                        else if (Position.IsShort && XTrend.IsDown)
                        {
                            _richTarget2 = true;
                        }
                        break;
                    case 3:
                        if (Position.IsLong && XTrend.IsDown)
                        {
                            _richTarget2 = true;
                        }
                        else if (Position.IsShort && XTrend.IsUp)
                        {
                            _richTarget2 = true;
                        }
                        break;
                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTarget2 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTarget2 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget2 = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget2 = true;
                        }
                        break;
                }
                switch (RichTargetMode13)
                {
                    case 0:
                        _richTarget3 = false;
                        break;
                    case 1:
                        _richTarget3 = true;
                        break;
                    case 2:
                        if (Position.IsLong && XTrend.IsUp)
                        {
                            _richTarget3 = true;
                        }
                        else if (Position.IsShort && XTrend.IsDown)
                        {
                            _richTarget3 = true;
                        }
                        break;
                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTarget3 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTarget3 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget3 = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget3 = true;
                        }
                        break;
                }
                switch (RichTargetMode14)
                {
                    case 0:
                        _richTarget4 = false;
                        break;
                    case 1:
                        _richTarget4 = true;
                        break;
                    case 2:
                        if (Position.IsLong && XTrend.IsUp)
                        {
                            _richTarget4 = true;
                        }
                        else if (Position.IsShort && XTrend.IsDown)
                        {
                            _richTarget4 = true;
                        }
                        break;
                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTarget4 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTarget4 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget4 = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget4 = true;
                        }
                        break;
                }
                switch (RichTargetMode15)
                {
                    case 0:
                        _richTarget5 = false;
                        break;
                    case 1:
                        _richTarget5 = true;
                        break;
                    case 2:
                        if (Position.IsLong && XTrend.IsUp)
                        {
                            _richTarget5 = true;
                        }
                        else if (Position.IsShort && XTrend.IsDown)
                        {
                            _richTarget5 = true;
                        }
                        break;
                    case 3:
                        if (Position.IsLong && XTrend.IsDown)
                        {
                            _richTarget5 = true;
                        }
                        else if (Position.IsShort && XTrend.IsUp)
                        {
                            _richTarget5 = true;
                        }
                        break;
                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTarget5 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTarget5 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget5 = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget5 = true;
                        }
                        break;
                }

            }
            else if (_mode == 2 && _richTargetCalcEnable)
            {
                switch (RichTargetMode21)
                {
                    case 0:
                        _richTarget1 = false;
                        break;
                    case 1:
                        _richTarget1 = true;
                        break;
                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTarget1 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget1 = true;
                        }
                        break;
                    case 10:
                        if (Position.IsLong && ((double)PositionMax2).CompareTo(XTrend.Low) < 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && ((double)PositionMin2).CompareTo(XTrend.High) > 0)
                        {
                            _richTarget1 = true;
                        }
                        break;
                }
                switch (RichTargetMode22)
                {
                    case 0:
                        _richTarget2 = false;
                        break;
                    case 1:
                        _richTarget2 = true;
                        break;
                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTarget2 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTarget2 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget2 = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget2 = true;
                        }
                        break;
                }
                switch (RichTargetMode23)
                {
                    case 0:
                        _richTarget3 = false;
                        break;
                    case 1:
                        _richTarget3 = true;
                        break;
                    case 2:
                        if (Position.IsLong && XTrend.IsUp)
                        {
                            _richTarget3 = true;
                        }
                        else if (Position.IsShort && XTrend.IsDown)
                        {
                            _richTarget3 = true;
                        }
                        break;
                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTarget3 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTarget3 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget3 = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget3 = true;
                        }
                        break;
                }
                switch (RichTargetMode24)
                {
                    case 0:
                        _richTarget4 = false;
                        break;
                    case 1:
                        _richTarget4 = true;
                        break;
                    case 2:
                        if (Position.IsLong && XTrend.IsUp)
                        {
                            _richTarget4 = true;
                        }
                        else if (Position.IsShort && XTrend.IsDown)
                        {
                            _richTarget4 = true;
                        }
                        break;
                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTarget4 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTarget4 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget4 = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget4 = true;
                        }
                        break;
                }
                switch (RichTargetMode25)
                {
                    case 0:
                        _richTarget5 = false;
                        break;
                    case 1:
                        _richTarget5 = true;
                        break;
                    case 2:
                        if (Position.IsLong && XTrend.IsUp)
                        {
                            _richTarget5 = true;
                        }
                        else if (Position.IsShort && XTrend.IsDown)
                        {
                            _richTarget5 = true;
                        }
                        break;
                    case 3:
                        if (Position.IsLong && XTrend.IsDown)
                        {
                            _richTarget5 = true;
                        }
                        else if (Position.IsShort && XTrend.IsUp)
                        {
                            _richTarget5 = true;
                        }
                        break;
                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTarget5 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTarget5 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget5 = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget5 = true;
                        }
                        break;
                }

            }
            else if (_mode == 3 && _richTargetCalcEnable)
            {
                switch (RichTargetMode31)
                {
                    case 0:
                        _richTarget1 = false;
                        break;
                    case 1:
                        _richTarget1 = true;
                        break;
                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTarget1 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget1 = true;
                        }
                        break;
                }
                switch (RichTargetMode32)
                {
                    case 0:
                        _richTarget2 = false;
                        break;
                    case 1:
                        _richTarget2 = true;
                        break;
                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTarget2 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTarget2 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget2 = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget2 = true;
                        }
                        break;
                }
            }
            else if (_mode == 4) //&& _richTargetCalcEnable)
            {
                switch (RichTargetMode41)
                {
                    case 0:
                        _richTarget1 = false;
                        break;
                    case 1:
                        _richTarget1 = true;
                        break;
                    case 2:
                        if (Position.IsLong && XTrend.IsUp)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && XTrend.IsDown)
                        {
                            _richTarget1 = true;
                        }
                        break;

                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTarget1 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget1 = true;
                        }
                        break;
                }
                switch (RichTargetMode42)
                {
                    case 0:
                        _richTarget2 = false;
                        break;
                    case 1:
                        _richTarget2 = true;
                        break;
                    case 2:
                        if (Position.IsLong && XTrend.IsUp)
                        {
                            _richTarget2 = true;
                        }
                        else if (Position.IsShort && XTrend.IsDown)
                        {
                            _richTarget2 = true;
                        }
                        break;

                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTarget2 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTarget2 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget2 = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget2 = true;
                        }
                        break;
                }
            }
            else if (_mode == 5 && _richTargetCalcEnable)
            {
                switch (RichTargetMode51)
                {
                    case 0:
                        _richTarget1 = false;
                        break;
                    case 1:
                        _richTarget1 = true;
                        break;
                    case 2:
                        if (Position.IsLong && XTrend.IsUp)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && XTrend.IsDown)
                        {
                            _richTarget1 = true;
                        }
                        break;

                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTarget1 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget1 = true;
                        }
                        break;
                }
            }
            else if (_mode == 6) //&& _richTargetCalcEnable)
            {
                switch (RichTargetMode61)
                {
                    case 0:
                        _richTarget1 = false;
                        break;
                    case 1:
                        _richTarget1 = true;
                        break;
                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTarget1 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget1 = true;
                        }
                        break;
                }
                switch (RichTargetMode62)
                {
                    case 0:
                        _richTarget2 = false;
                        break;
                    case 1:
                        _richTarget2 = true;
                        break;
                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTarget2 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTarget2 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget2 = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget2 = true;
                        }
                        break;
                }
            }

            RichTarget = _richTarget1 ? 1 : (_richTarget2 ? 2 : (_richTarget3 ? 3 : (_richTarget4 ? 4 : (_richTarget5 ? 5 : 0))));

            //Mode = _mode;
            //ModeSafe = _modeSafe ? 1 : 0;

            ChangeMode();

            Mode = _mode;
            ModeSafe = _modeSafe ? 1 : 0;
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
            if (EntryMode == -5)
            {
                var entrySignal = 0;
                var entryShort = 0;
                var entryLong = 0;

                var entryMode = 0;
                var ma = XTrend.Ma;

                float r1 = 0f;
                float r2 = 0f;
                double rs;

                float pStop1 = 0f;
                float pMin1 = 0f;
                float pMax1 = 0f;

                float pStop2 = 0f;
                float pMin2 = 0f;
                float pMax2 = 0f;

                float rShort = 0f;
                float rLong = 0f;

                float pStopShort = 0f;
                float pStopLong = 0f;
                float pMin = 0f;
                float pMax = 0f;

                bool f1 = false, f2 = false, f3 = false, f4 = false;
                bool bShort = false;
                bool bLong = false;

                if (TakeShort(EntrySignal11) && IsEntryRiskLow(-1))
                {
                    r1 = _positionStop - (float)ma;
                    pStop1 = _positionStop;
                    pMax1 = _positionMax;
                    f1 = true;
                }
                if (TakeShort(EntrySignal12) && IsEntryRiskLow(-1))
                {
                    r2 = _positionStop - (float)ma;
                    pStop2 = _positionStop;
                    pMax2 = _positionMax;
                    f2 = true;
                }
                if ((f1 && !f2) ||
                     (f1 && f2 && r1.CompareTo(r2) <= 0))
                {
                    rShort = r1;
                    pStopShort = pStop1;
                    pMax = pMax1;
                    entryShort = EntrySignal11;
                    bShort = true;
                }
                else if ((f2 && !f1) ||
                     (f2 && f1 && r2.CompareTo(r1) <= 0))
                {
                    rShort = r2;
                    pStopShort = pStop2;
                    pMax = pMax2;
                    entryShort = EntrySignal12;
                    bShort = true;
                }
                if (TakeLong(EntrySignal11) && IsEntryRiskLow(+1))
                {
                    r1 = -_positionStop + (float)ma;
                    pStop1 = _positionStop;
                    pMin1 = _positionMin;
                    f3 = true;
                }
                if (TakeLong(EntrySignal12) && IsEntryRiskLow(+1))
                {
                    r2 = -_positionStop + (float)ma;
                    pStop2 = _positionStop;
                    pMin2 = _positionMin;
                    f4 = true;
                }
                if ((f3 && !f4) ||
                      (f3 && f4 && r1.CompareTo(r2) <= 0))
                {
                    rLong = r1;
                    pStopLong = pStop1;
                    pMin = pMin1;
                    entryLong = EntrySignal11;
                    bLong = true;
                }
                else if ((f4 && !f3) ||
                     (f4 && f3 && r2.CompareTo(r1) <= 0))
                {
                    rLong = r2;
                    pStopLong = pStop2;
                    pMin = pMin2;
                    entryLong = EntrySignal12;
                    bLong = true;
                }
                if (!bShort && !bLong) return 0;

                bool bSell = false, bBuy = false;

                if ((bShort && !bLong) ||
                     (bShort && bLong && rShort.CompareTo(rLong) <= 0))
                {
                    _positionMax = pMax;
                    _positionStop = pStopShort;
                    entrySignal = entryShort;
                    bSell = true;
                }
                else if ((bLong && !bShort) ||
                     (bLong && bShort && rLong.CompareTo(rShort) <= 0))
                {
                    _positionMin = pMin;
                    _positionStop = pStopLong;
                    entrySignal = entryLong;
                    bBuy = true;
                }
                else
                    return 0;

                if (entrySignal == 0)
                    throw new NullReferenceException("EntrySignalMode is Null");
                if (_positionStop.CompareTo(0f) <= 0)
                    throw new NullReferenceException("PositionStop is Null");
                if (bSell && _positionMax.CompareTo(0f) <= 0)
                    throw new NullReferenceException("SellPositionMax is Null");
                if (bBuy && _positionMin.CompareTo(0f) <= 0)
                    throw new NullReferenceException("BuyPositionMin is Null");

                if (bSell)
                    if (TakeShort(entrySignal) && _swingCountEntry >= SwingCountEntry)
                    {
                        if (XTrend.GetPrice5(-1, EntryPriceId, out _currentLimitPrice, out _comment))
                        {
                            _comment = "Short Entry:[" + entrySignal + "] " + _comment;
                            LastSignalId = entrySignal;
                            _tryEnterMode = 1;
                            return -1;
                        }
                        if (CancelOrderMode > 0)
                        {
                            LastSignalId = 0;
                            KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Sell);
                        }
                    }
                if (bBuy)
                    if (TakeLong(entrySignal) && _swingCountEntry >= SwingCountEntry)
                    {
                        if (XTrend.GetPrice5(+1, EntryPriceId, out _currentLimitPrice, out _comment))
                        {
                            _comment = "Long Entry:[" + entrySignal + "] " + _comment;
                            LastSignalId = entrySignal;
                            _tryEnterMode = 1;
                            return +1;
                        }
                        if (CancelOrderMode > 0)
                        {
                            LastSignalId = 0;
                            KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Buy);
                        }
                    }
            }
            else if (EntryMode == -15)
            {
                int entrySignal = 0;
                int entryMode = 0;
                int buysell = 0;

                if (TakeShort(EntrySignal11) /* && IsLongTermBreakDown */&& IsEntryRiskLow(-1))
                {
                    entrySignal = EntrySignal11;
                    buysell = -1;
                }
                else if (TakeLong(EntrySignal11) /* && IsLongTermBreakUp */&& IsEntryRiskLow(+1))
                {
                    entrySignal = EntrySignal11;
                    buysell = +1;
                }
                else if (TakeShort(EntrySignal12) /* && IsLongTermBreakDown */&& IsEntryRiskLow(-1))
                {
                    entrySignal = EntrySignal12;
                    buysell = -1;
                }
                else if (TakeLong(EntrySignal12) /* && IsLongTermBreakUp */&& IsEntryRiskLow(+1))
                {
                    entrySignal = EntrySignal12;
                    buysell = +1;
                }
                else
                    return 0;

                if (entrySignal == 0 || buysell ==  0)
                    return 0;

                if (buysell < 0 && TakeShort(entrySignal) && _swingCountEntry >= SwingCountEntry)
                {
                    if (XTrend.GetPrice5(-1, EntryPriceId, out _currentLimitPrice, out _comment))
                    {
                        _comment = "Short Entry:[" + entrySignal + "] " + _comment;
                        LastSignalId = entrySignal;
                        _tryEnterMode = 1;
                        return -1;
                    }
                    if (CancelOrderMode > 0)
                    {
                        LastSignalId = 0;
                        KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Sell);
                    }
                }
                if (buysell > 0 && TakeLong(entrySignal) && _swingCountEntry >= SwingCountEntry)
                {
                    if (XTrend.GetPrice5(+1, EntryPriceId, out _currentLimitPrice, out _comment))
                    {
                        _comment = "Long Entry:[" + entrySignal + "] " + _comment;
                        LastSignalId = entrySignal;
                        _tryEnterMode = 1;
                        return +1;
                    }
                    if (CancelOrderMode > 0)
                    {
                        LastSignalId = 0;
                        KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Buy);
                    }
                }
                return 0;
            }
            return 0;
        }

        protected override bool ShortEntry()
        {
            int entrySignal = 0;
            int entryMode = 0;
            if (EntryMode == 1 || EntryMode == 0)
            {
                entrySignal = EntrySignal1;
                entryMode = 1;
            }
            else if (EntryMode == 2)
            {
                entrySignal = EntrySignal2;
                entryMode = 2;
            }
            else if (EntryMode == -1)
            {
                if (LastExitMode == 1)
                {
                    entrySignal = EntrySignal2;
                    entryMode = 2;
                }
                else // 0 || 2
                {
                    entrySignal = EntrySignal1;
                    entryMode = 1;
                }
            }
            else if (EntryMode == -2)
            {
                // entrySignal = EntrySignal11;
                entryMode = 1;

                if (TakeShort(EntrySignal11) /* && IsLongTermBreakDown */ && IsEntryRiskLow(-1))
                    entrySignal = EntrySignal11;
                else if (TakeShort(EntrySignal12) /* && !IsLongTermBreakUp */ && IsEntryRiskLow(-1))
                    entrySignal = EntrySignal12;
                else if (TakeShort(EntrySignal13))
                    entrySignal = EntrySignal13;
                else

                    return false;
            }
            else if (EntryMode == -3)
            {
                // entrySignal = EntrySignal11;
                entryMode = 1;

                if (TakeShort(EntrySignal11) && IsLongTermBreakUp && IsEntryRiskLow(-1))
                    entrySignal = EntrySignal11;
                else if (TakeShort(EntrySignal12) && !IsLongTermBreakDown && IsEntryRiskLow(-1))
                    entrySignal = EntrySignal12;
                else if (TakeShort(EntrySignal13))
                    entrySignal = EntrySignal13;
                else

                    return false;
            }
            else if (EntryMode == -6)
            {
                // entrySignal = EntrySignal11;
                entryMode = 1;

                if (TakeShort(EntrySignal11) && IsEntryRiskLow2(-1))
                    entrySignal = EntrySignal11;
                else if (TakeShort(EntrySignal12) && IsEntryRiskLow(-1))
                    entrySignal = EntrySignal12;
                else if (TakeShort(EntrySignal13))
                    entrySignal = EntrySignal13;
                else
                    return false;
            }
            else if (EntryMode == -5)
                return false;

            if (entrySignal <= 0)
                // throw new NullReferenceException("EntrySignal==0");
                return false;

            if (TakeShort(entrySignal) && _swingCountEntry >= SwingCountEntry)
            {
                if (XTrend.GetPrice5(-1, EntryPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Short Entry:[" + entrySignal + "] " + _comment;
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
        protected override bool LongEntry()
        {
            int entrySignal = 0;
            int entryMode = 0;
            if (EntryMode == 1 || EntryMode == 0)
            {
                entrySignal = EntrySignal1;
                entryMode = 1;
            }
            else if (EntryMode == 2)
            {
                entrySignal = EntrySignal2;
                entryMode = 2;
            }
            else if (EntryMode == -1)
            {
                if (LastExitMode == 1)
                {
                    entrySignal = EntrySignal2;
                    entryMode = 2;
                }
                else // 0 || 2
                {
                    entrySignal = EntrySignal1;
                    entryMode = 1;
                }
            }
            else if (EntryMode == -2)
            {
                //entrySignal = EntrySignal11;
                entryMode = 1;
                if (TakeLong(EntrySignal11) /* && IsLongTermBreakUp */ && IsEntryRiskLow(+1))
                    entrySignal = EntrySignal11;
                else if (TakeLong(EntrySignal12) /* && !IsLongTermBreakDown */ && IsEntryRiskLow(+1))
                    entrySignal = EntrySignal12;
                else if (TakeLong(EntrySignal13))
                    entrySignal = EntrySignal13;
                else

                    return false;
            }
            else if (EntryMode == -3)
            {
                entryMode = 1;
                if (TakeLong(EntrySignal11) && IsLongTermBreakDown && IsEntryRiskLow(+1))
                    entrySignal = EntrySignal11;
                else if (TakeLong(EntrySignal12) && !IsLongTermBreakUp && IsEntryRiskLow(+1))
                    entrySignal = EntrySignal12;
                else if (TakeLong(EntrySignal13))
                    entrySignal = EntrySignal13;
                else

                    return false;
            }
            else if (EntryMode == -6)
            {
                entryMode = 1;
                if (TakeLong(EntrySignal11) && IsEntryRiskLow2(+1))
                    entrySignal = EntrySignal11;
                else if (TakeLong(EntrySignal12) && !IsLongTermBreakUp && IsEntryRiskLow(+1))
                    entrySignal = EntrySignal12;
                else if (TakeLong(EntrySignal13))
                    entrySignal = EntrySignal13;
                else

                    return false;
            }
            else if (EntryMode == -5)
                return false;

            if (entrySignal <= 0)
                //throw new NullReferenceException("EntrySignal==0");
                return false;

            if (TakeLong(entrySignal) && _swingCountEntry >= SwingCountEntry)
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

        protected override bool ShortReverse()
        {
            // Reverse After Loss
            if (TakeLong(ReverseLossSignal) && RealReverseLossCnt < ReverseLossCnt && XTrend.Ma.CompareTo(PositionStop) > 0)
            // if ((_mode == 1 || _mode == 2) && IsRiskLowMap(1) && PositionMaxValid && XTrend.Ma.CompareTo(PositionMax) > 0)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    // _tryEnterMode = ReverseMode3 < 0 ? (_mode == 1 ? 2 : 1) : (_mode = ReverseMode3);
                    //_tryEnterMode = ReverseMode3 < 0 ? (_mode == 1 ? 2 : 1) : ReverseMode3;
                    _tryEnterMode = 1;
                    _incrLoss = 1;
                    return true;
                }
            }

            // Reverse After Profit Mode1
            if (_mode == 1 && _richTarget1 && IsMyPositionRiskLow && TakeLong(ReverseSignal1) && _swingCountReverse >= SwingCountReverse)
            {
                if (XTrend.GetPrice5(+1, ReversePriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Reverse To Long:[" + ReverseSignal1 + "] " + _comment;
                    LastSignalId = ReverseSignal1;
                    //_tryEnterMode = ReverseMode1 < 0 ? (_mode == 1 ? 2 : 1) : (_mode = ReverseMode1);
                    _tryEnterMode = ReverseMode1 < 0 ? 2 : ReverseMode1;
                    return true;
                }
                if (CancelOrderMode > 0)
                {
                    LastSignalId = 0;
                    KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Buy);
                }
            }
            if (_mode == 1 && !_richTarget1 && TakeLong(ReverseSignal12) && _swingCountReverse >= SwingCountReverse)
            {
                if (XTrend.GetPrice5(+1, ReversePriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Reverse To Long:[" + ReverseSignal12 + "] " + _comment;
                    LastSignalId = ReverseSignal1;
                    //_tryEnterMode = ReverseMode1 < 0 ? (_mode == 1 ? 2 : 1) : (_mode = ReverseMode1);
                    _tryEnterMode = ReverseMode1 < 0 ? 2 : ReverseMode1;
                    return true;
                }
                if (CancelOrderMode > 0)
                {
                    LastSignalId = 0;
                    KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Buy);
                }
            }
            // Reverse After Profit Mode2
            if (_mode == 2 && _richTarget1 && IsMyPositionRiskLow && TakeLong(ReverseSignal2) && _swingCountReverse >= SwingCountReverse)
            {
                if (XTrend.GetPrice5(+1, ReversePriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Reverse To Long:[" + ReverseSignal2 + "] " + _comment;
                    LastSignalId = ReverseSignal2;
                    //_tryEnterMode = ReverseMode2 < 0 ? (_mode == 1 ? 2 : 1) : (_mode = ReverseMode2);
                    _tryEnterMode = ReverseMode2 < 0 ? 1 : ReverseMode2;
                    return true;
                }
                if (CancelOrderMode > 0)
                {
                    LastSignalId = 0;
                    KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Buy);
                }
            }
            if (_mode == 3 && _richTarget1 && TakeLong(ReverseSignal3) && IsEntryRiskLow(+1) && _swingCountReverse >= SwingCountReverse)
            {
                if (XTrend.GetPrice5(+1, ReversePriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Reverse To LongEmergency:[" + ReverseSignal3 + "] " + _comment;
                    LastSignalId = ReverseSignal3;
                    //_tryEnterMode = ReverseMode5 < 0 ? (_mode == 1 ? 2 : 1) : (_mode = ReverseMode5);
                    _tryEnterMode = ReverseMode5 < 0 ? (_mode == 1 ? 2 : 1) : ReverseMode5;
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
        protected override bool LongReverse()
        {
            // Reverse After Loss
            if (TakeShort(ReverseLossSignal) && RealReverseLossCnt < ReverseLossCnt && XTrend.Ma.CompareTo(PositionStop) < 0)
            // if ((_mode == 1 || _mode == 2) && IsRiskLowMap(1) && PositionMinValid && XTrend.Ma.CompareTo(PositionMin) < 0)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    // _tryEnterMode = ReverseMode3 < 0 ? (_mode == 1 ? 2 : 1) : (_mode = ReverseMode3);
                    _tryEnterMode = ReverseMode3 < 0 ? (_mode == 1 ? 2 : 1) : ReverseMode3;
                    _incrLoss = 1;
                    return true;
                }
            }

            // Reverse After Profit
            if (_mode == 1 && _richTarget1 && IsMyPositionRiskLow && TakeShort(ReverseSignal1) && _swingCountReverse >= SwingCountReverse)
            {
                if (XTrend.GetPrice5(-1, ReversePriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Reverse To Short:[" + ReverseSignal1 + "] " + _comment;
                    LastSignalId = ReverseSignal1;
                    //_tryEnterMode = ReverseMode1 < 0 ? (_mode == 1 ? 2 : 1) : (_mode = ReverseMode1);
                    _tryEnterMode = ReverseMode1 < 0 ? 2 : ReverseMode1;
                    return true;
                }
                if (CancelOrderMode > 0)
                {
                    LastSignalId = 0;
                    KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Sell);
                }
            }
            if (_mode == 1 && !_richTarget1 && TakeShort(ReverseSignal12) && _swingCountReverse >= SwingCountReverse)
            {
                if (XTrend.GetPrice5(-1, ReversePriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Reverse To Short:[" + ReverseSignal1 + "] " + _comment;
                    LastSignalId = ReverseSignal1;
                    //_tryEnterMode = ReverseMode1 < 0 ? (_mode == 1 ? 2 : 1) : (_mode = ReverseMode1);
                    _tryEnterMode = ReverseMode1 < 0 ? 2 : ReverseMode1;
                    return true;
                }
                if (CancelOrderMode > 0)
                {
                    LastSignalId = 0;
                    KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Sell);
                }
            }
            // Reverse After Profit
            if (_mode == 2 && _richTarget1 && IsMyPositionRiskLow && TakeShort(ReverseSignal2) && _swingCountReverse >= SwingCountReverse)
            {
                if (XTrend.GetPrice5(-1, ReversePriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Reverse To Short:[" + ReverseSignal2 + "] " + _comment;
                    LastSignalId = ReverseSignal2;
                    //_tryEnterMode = ReverseMode2 < 0 ? (_mode == 1 ? 2 : 1) : (_mode = ReverseMode2);
                    _tryEnterMode = ReverseMode2 < 0 ? 1 : ReverseMode2;
                    return true;
                }
                if (CancelOrderMode > 0)
                {
                    LastSignalId = 0;
                    KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Sell);
                }
            }
            if (_mode == 3 && _richTarget1 && TakeShort(ReverseSignal3) && IsEntryRiskLow(-1) && _swingCountReverse >= SwingCountReverse)
            {
                if (XTrend.GetPrice5(-1, ReversePriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Reverse To Short:[" + ReverseSignal3 + "] " + _comment;
                    LastSignalId = ReverseSignal3;
                    //_tryEnterMode = ReverseMode5 < 0 ? (_mode == 1 ? 2 : 1) : (_mode = ReverseMode5);
                    _tryEnterMode = ReverseMode5 < 0 ? (_mode == 1 ? 2 : 1) : ReverseMode5;
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

        protected override bool ShortExit()
        {
            if (_mode == 1 && _richTarget1 && TakeLong(ExitSignal1) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Short Exit:[" + ExitSignal1 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            if (_mode == 1 && !_richTarget1 && TakeLong(ExitSignal12) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Short Exit:[" + ExitSignal12 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            if (_mode == 2 && _richTarget1 && TakeLong(ExitSignal2) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Short Exit:[" + ExitSignal2 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            if (_mode == 3 && _richTarget1 && TakeLong(ExitSignal3) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Short Exit:[" + ExitSignal3 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            if (_mode == 5 && _richTarget1 && (TakeLong(ExitSignal51) || TakeLong(ExitSignal52)) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Short Exit:[" + ExitSignal51 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            if (_mode == 4 && _richTarget1 && TakeLong(ExitSignal4))
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Short Exit:[" + ExitSignal4 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            /*
            if (PositionMaxValid && XTrend.Ma.CompareTo(PositionMax) > 0)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            */
            if (_mode == 6 && _richTarget1 && TakeLong(ExitSignal6))
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Short Exit:[" + ExitSignal6 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            //if ( ( (Portfolio == null) || (Portfolio != null && Portfolio.IsShort) ) && 
            //    (PositionStopValid && XTrend.Ma.CompareTo((double)PositionStop) > 0) &&
            //    TakeLong(ExitStopSignal)
            //    )
            //{
            //    if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
            //    {
            //        _tryEnterMode = -1;
            //        LastExitMode = _mode;
            //        return true;
            //    }
            //}
            if (_mode == 10 && TakeLong(ExitStopSignal))
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            return false;
        }
        protected override bool LongExit()
        {
            if (_mode == 1 && _richTarget1 && TakeShort(ExitSignal1) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Exit:[" + ExitSignal1 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            if (_mode == 1 && !_richTarget1 && TakeShort(ExitSignal12) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Exit:[" + ExitSignal12 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            if (_mode == 2 && _richTarget1 && TakeShort(ExitSignal2) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Exit:[" + ExitSignal2 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            if (_mode == 3 && _richTarget1 && TakeShort(ExitSignal3) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Exit:[" + ExitSignal3 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            if (_mode == 5 && _richTarget1 && (TakeShort(ExitSignal51) || TakeShort(ExitSignal52)) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Exit:[" + ExitSignal51 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            if (_mode == 4 && _richTarget1 && TakeShort(ExitSignal4))
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Exit:[" + ExitSignal4 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            /*
            if (PositionMinValid && XTrend.Ma.CompareTo(PositionMin) < 0)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            */
            if (_mode == 6 && _richTarget1 && TakeShort(ExitSignal6))
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Exit:[" + ExitSignal6 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            //if ( ( (Portfolio == null) || (Portfolio != null && Portfolio.IsLong) ) && 
            //    PositionStopValid && XTrend.Ma.CompareTo((double)PositionStop) < 0 &&
            //    TakeShort(ExitStopSignal)
            //    )
            //{
            //    if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
            //    {
            //        _tryEnterMode = -1;
            //        LastExitMode = _mode;
            //        return true;
            //    }
            //}
            if (_mode == 10 && TakeShort(ExitStopSignal))
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            return false;
        }

        private bool ChangeMode()
        {
            if (Position.IsLong)
            {
                if (_mode == 1 && _richTarget2 && TakeShort(ChangeModeCondSignal1))
                {
                    _mode = ChangeMode1;
                    _modeLast = 1;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }
                if (_mode == 1 && _richTarget3 &&
                        TakeShort(ChangeModeCondSignal3) && (PositionMinValid && XTrend.Ma.CompareTo(PositionMin) >= 0))
                {
                    _mode = 3;
                    _modeLast = 1;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }
                if (_mode == 1 && _richTarget5 && TakeShort(ChangeModeCondSignal5))
                {
                    _mode = 5;
                    _modeLast = 1;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }
                if (_mode == 2 && _richTarget2 && TakeShort(ChangeModeCondSignal2))
                {
                    _mode = ChangeMode2;
                    _modeLast = 2;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }
                if (_mode == 2 && _richTarget3 && TakeShort(ChangeModeCondSignal3))
                {
                    _mode = 3;
                    _modeLast = 2;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }
                if (_mode == 2 && _richTarget5 && TakeShort(ChangeModeCondSignal5))
                {
                    _mode = 5;
                    _modeLast = 2;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }
                if (_mode == 3 && _richTarget2)
                {
                    _mode = ChangeMode3 != 0 ? ChangeMode3 : _modeLast;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }

                if (_mode == 4 && _richTarget2)
                {
                    _mode = ChangeMode4 != 0 ? ChangeMode4 : _modeLast;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }
                if (_mode != 4 && _mode != 6 && _mode != 10 && PositionMinValid && XTrend.Ma.CompareTo(PositionMin) < 0)
                {
                    _mode = 4;
                    _modeLast = (_mode == 1 || _mode == 2) ? _mode : _modeLast;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }

                if (_mode != 6 && (Portfolio != null && !Portfolio.IsLong) &&
                    PositionStopValid && XTrend.Ma.CompareTo((double)PositionStop) < 0)
                {
                    _mode = 6;
                    _modeLast = (_mode == 1 || _mode == 2) ? _mode : _modeLast;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }

                if (_mode != 10 && ((Portfolio == null) || (Portfolio != null && Portfolio.IsLong)) &&
                    PositionStopValid && XTrend.Ma.CompareTo((double)PositionStop) < 0)
                {
                    _mode = 10;
                    _modeLast = (_mode == 1 || _mode == 2) ? _mode : _modeLast;
                    return true;
                }
            }
            else if (Position.IsShort)
            {
                if (_mode == 1 && _richTarget2 && TakeLong(ChangeModeCondSignal1))
                {
                    _mode = ChangeMode1;
                    _modeLast = 1;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }
                if (_mode == 1 && _richTarget3 &&
                        TakeLong(ChangeModeCondSignal3) && (PositionMaxValid && XTrend.Ma.CompareTo(PositionMax) <= 0))
                {
                    _mode = 3;
                    _modeLast = 1;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }
                if (_mode == 1 && _richTarget5 && TakeLong(ChangeModeCondSignal5))
                {
                    _mode = 5;
                    _modeLast = 1;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }
                if (_mode == 2 && _richTarget2 && TakeLong(ChangeModeCondSignal2))
                {
                    _mode = ChangeMode2;
                    _modeLast = 2;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }
                if (_mode == 2 && _richTarget3 && TakeLong(ChangeModeCondSignal3))
                {
                    _mode = 3;
                    _modeLast = 2;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }
                if (_mode == 2 && _richTarget5 && TakeLong(ChangeModeCondSignal5))
                {
                    _mode = 5;
                    _modeLast = 2;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }
                if (_mode == 3 && _richTarget2)
                {
                    _mode = ChangeMode3 != 0 ? ChangeMode3 : _modeLast;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }
                if (_mode == 4 && _richTarget2)
                {
                    _mode = ChangeMode4 != 0 ? ChangeMode4 : _modeLast;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }

                if (_mode != 4 && _mode != 6 && _mode != 10 && PositionMaxValid && XTrend.Ma.CompareTo(PositionMax) > 0)
                {
                    _mode = 4;
                    _modeLast = (_mode == 1 || _mode == 2) ? _mode : _modeLast;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }

                if (_mode != 6 && (Portfolio != null && !Portfolio.IsShort) &&
                    PositionStopValid && XTrend.Ma.CompareTo((double)PositionStop) > 0)
                {
                    _mode = 6;
                    _modeLast = (_mode == 1 || _mode == 2) ? _mode : _modeLast;

                    ClearTargets();

                    _richTargetCalcEnable = false;
                    _safeModeCalcEnable = false;
                    _richTargetSafeCond = false;
                    _modeSafe = false;
                    return true;
                }

                if (_mode != 10 && ((Portfolio == null) || (Portfolio != null && Portfolio.IsShort)) &&
                    PositionStopValid && XTrend.Ma.CompareTo((double)PositionStop) > 0)
                {
                    _mode = 10;
                    _modeLast = (_mode == 1 || _mode == 2) ? _mode : _modeLast;
                    return true;
                }
            }
            return false;
        }

        private void ClearTargets()
        {
            _richTarget1 = false;
            _richTarget2 = false;
            _richTarget3 = false;
            _richTarget4 = false;
            _richTarget5 = false;
            _richTarget6 = false;

            _richTargetCalcEnable = false;
        }

        private bool IsRiskLowMap(int bit)
        {
            if ((bit & RiskLowMap) > 0)
                return IsMyPositionRiskLow;
            return true;
        }
    }
}
