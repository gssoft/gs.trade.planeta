using System;

namespace GS.Trade.Strategies
{
    public class X11832 : X118
    {
        public int EntrySignal11 { get; set; }
        public int EntrySignal12 { get; set; }
        public int EntrySignal13 { get; set; }

        public int FlatCount { get; set; }
        public int CancelOrderMode { get; set; }
        public int CancelReverseMode { get; set; }

        public int RichHL { get; set; }

        public int RichTargetMode11 { get; set; }
        public int RichTargetMode12 { get; set; }
        public int RichTargetMode13 { get; set; }

        public int RichTargetMode21 { get; set; }
        public int RichTargetMode22 { get; set; }
        public int RichTargetMode23 { get; set; }

        public int RichTargetMode31 { get; set; }
        public int RichTargetMode32 { get; set; }

        public int ExitSignal3 { get; set; }
        public int ReverseSignal3 { get; set; }
        /*
        public int ReverseSignal31 { get; set; }
        public int ReverseSignal32 { get; set; }
        */
        public int ChangeMode1 { get; set; }
        public int ChangeMode2 { get; set; }
        public int ChangeMode3 { get; set; }

        public int ChangeModeCondSignal1 { get; set; }
        public int ChangeModeCondSignal2 { get; set; }
        public int ChangeModeCondSignal3 { get; set; }

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

        private bool _richTargetSafeCond;

        private bool _tryToReverseLoss;
        private bool _reverseLossMode;
        private int _reverseLossCnt;

        private int _mode;
        private bool _modeSafe;
        private int _tryEnterMode;
        private int _incrLoss;


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

        protected override void PositionChanged(long op, long np)
        {
            _richTarget1 = false;
            _richTarget2 = false;
            _richTarget3 = false;

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
            }
            else
            {
                // _reverseLossCnt = 0;
                // _incrLoss = 0;
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
            }
            else if (Position.IsLong)
            {
                if (!PositionMinValid)
                {
                    double m;
                    if (XTrend.HaveLower((int)TimeInt * 60 * KMinFarFromHere, XTrend.Low, out m))
                        PositionMin = (float)m;
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
            
            RichTarget = _richTarget1 ? 1 : (_richTarget2 ? 2 : (_richTarget3 ? 3 : 0));
            Mode = _mode;
            ModeSafe = _modeSafe ? 1 : 0;
        }

        protected override bool StartShortEntry()
        {
            var entrySignal = 0;
            var entryMode = 0;
            if (StartEntryMode == 1 || StartEntryMode == 0)
            {
                entrySignal = EntrySignal1;
                entryMode = 1;
            }
            else if (StartEntryMode == 2)
            {
                entrySignal = EntrySignal2;
                entryMode = 2;
            }

            if (entrySignal <= 0)
                throw new NullReferenceException("EntrySignal==0");

            if (TakeShort(entrySignal) && XTrend.FlatCount >= FlatCount
                    && _swingCountEntry >= SwingCountStartEntry)
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
            int entrySignal = 0;
            int entryMode = 0;
            if (StartEntryMode == 1 || StartEntryMode == 0)
            {
                entrySignal = EntrySignal1;
                entryMode = 1;
            }
            else if (StartEntryMode == 2)
            {
                entrySignal = EntrySignal2;
                entryMode = 2;
            }

            if (entrySignal <= 0)
                throw new NullReferenceException("EntrySignal==0");

            if (TakeLong(entrySignal) && XTrend.FlatCount >= FlatCount
                && _swingCountEntry >= SwingCountStartEntry)
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

                if (TakeShort(EntrySignal11) && IsLongTermBreakDown)
                    entrySignal = EntrySignal11;
                else if (TakeShort(EntrySignal12) && !IsLongTermBreakUp)
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

                if (TakeShort(EntrySignal11) && IsLongTermBreakUp)
                    entrySignal = EntrySignal11;
                else if (TakeShort(EntrySignal12) && !IsLongTermBreakDown)
                    entrySignal = EntrySignal12;
                else if (TakeShort(EntrySignal13))
                    entrySignal = EntrySignal13;
                else

                    return false;
            }

            if (entrySignal <= 0)
                throw new NullReferenceException("EntrySignal==0");

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
                if (TakeLong(EntrySignal11) && IsLongTermBreakUp)
                    entrySignal = EntrySignal11;
                else if (TakeLong(EntrySignal12) && !IsLongTermBreakDown)
                    entrySignal = EntrySignal12;
                else if (TakeLong(EntrySignal13))
                    entrySignal = EntrySignal13;
                else

                    return false;
            }
            else if (EntryMode == -3)
            {
                entryMode = 1;
                if (TakeLong(EntrySignal11) && IsLongTermBreakDown)
                    entrySignal = EntrySignal11;
                else if (TakeLong(EntrySignal12) && !IsLongTermBreakUp)
                    entrySignal = EntrySignal12;
                else if (TakeLong(EntrySignal13))
                    entrySignal = EntrySignal13;
                else

                    return false;
            }

            if (entrySignal <= 0)
                throw new NullReferenceException("EntrySignal==0");

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
            if ((_mode == 1 || _mode == 2) && TakeLong(ReverseLossSignal) &&
                    RealReverseLossCnt < ReverseLossCnt && IsMyPositionRiskLow && XTrend.Ma.CompareTo(PositionMax) > 0)
            // if ((_mode == 1 || _mode == 2) && IsRiskLowMap(1) && PositionMaxValid && XTrend.Ma.CompareTo(PositionMax) > 0)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    // _tryEnterMode = ReverseMode3 < 0 ? (_mode == 1 ? 2 : 1) : (_mode = ReverseMode3);
                    _tryEnterMode = ReverseMode3 < 0 ? (_mode == 1 ? 2 : 1) : ReverseMode3;
                    _incrLoss = 1;
                    return true;
                }
            }
            // RichTarget2. Enter To Another Mode
            if (_mode == 1 && _richTarget2 && TakeLong(ChangeModeCondSignal1) && _swingCountReverse >= SwingCountReverse)
            {
                _mode = ChangeMode1;
                _richTarget1 = false;
                _richTarget2 = false;
                _richTarget3 = false;
                _richTargetCalcEnable = false;
                _safeModeCalcEnable = false;
                _richTargetSafeCond = false;
                _modeSafe = false;
                return false;
            }
            if (_mode == 1 && _richTarget3 && TakeLong(ChangeModeCondSignal3) && _swingCountReverse >= SwingCountReverse)
            {
                _mode = 3;
                _richTarget1 = false;
                _richTarget2 = false;
                _richTarget3 = false;
                _richTargetCalcEnable = false;
                _safeModeCalcEnable = false;
                _richTargetSafeCond = false;
                _modeSafe = false;
                return false;
            }
            if (_mode == 2 && _richTarget2 && TakeLong(ChangeModeCondSignal2) && _swingCountReverse >= SwingCountReverse)
            {
                _mode = ChangeMode2;
                _richTarget1 = false;
                _richTarget2 = false;
                _richTarget3 = false;
                _richTargetCalcEnable = false;
                _safeModeCalcEnable = false;
                _richTargetSafeCond = false;
                _modeSafe = false;
                return false;
            }
            if (_mode == 2 && _richTarget3 && TakeLong(ChangeModeCondSignal3) && _swingCountReverse >= SwingCountReverse)
            {
                _mode = 3;
                _richTarget1 = false;
                _richTarget2 = false;
                _richTarget3 = false;
                _richTargetCalcEnable = false;
                _safeModeCalcEnable = false;
                _richTargetSafeCond = false;
                _modeSafe = false;
                return false;
            }
            if (_mode == 3 && _richTarget2  && _swingCountReverse >= SwingCountReverse)
            {
                _mode = ChangeMode3;
                _richTarget1 = false;
                _richTarget2 = false;
                _richTarget3 = false;
                _richTargetCalcEnable = false;
                _safeModeCalcEnable = false;
                _richTargetSafeCond = false;
                _modeSafe = false;
                return false;
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
            if (_mode == 3 && _richTarget1 && TakeLong(ReverseSignal3) && _swingCountReverse >= SwingCountReverse)
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
            if ((_mode == 1 || _mode == 2) && TakeShort(ReverseLossSignal) &&
                RealReverseLossCnt < ReverseLossCnt && IsMyPositionRiskLow && XTrend.Ma.CompareTo(PositionMin) < 0)
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
            if (_mode == 1 && _richTarget2 && TakeShort(ChangeModeCondSignal1) && _swingCountReverse >= SwingCountReverse)
            {
                _mode = ChangeMode1;
                _richTarget1 = false;
                _richTarget2 = false;
                _richTarget3 = false;
                _richTargetCalcEnable = false;
                _safeModeCalcEnable = false;
                _richTargetSafeCond = false;
                _modeSafe = false;
                return false;
            }
            if (_mode == 1 && _richTarget3 && TakeShort(ChangeModeCondSignal3) && _swingCountReverse >= SwingCountReverse)
            {
                _mode = 3;
                _richTarget1 = false;
                _richTarget2 = false;
                _richTarget3 = false;
                _richTargetCalcEnable = false;
                _safeModeCalcEnable = false;
                _richTargetSafeCond = false;
                _modeSafe = false;
                return false;
            }
            if (_mode == 2 && _richTarget2 && TakeShort(ChangeModeCondSignal2) && _swingCountReverse >= SwingCountReverse)
            {
                _mode = ChangeMode2;
                _richTarget1 = false;
                _richTarget2 = false;
                _richTarget3 = false;
                _richTargetCalcEnable = false;
                _safeModeCalcEnable = false;
                _richTargetSafeCond = false;
                _modeSafe = false;
                return false;
            }
            if (_mode == 2 && _richTarget3 && TakeShort(ChangeModeCondSignal3) && _swingCountReverse >= SwingCountReverse)
            {
                _mode = 3;
                _richTarget1 = false;
                _richTarget2 = false;
                _richTarget3 = false;
                _richTargetCalcEnable = false;
                _safeModeCalcEnable = false;
                _richTargetSafeCond = false;
                _modeSafe = false;
                return false;
            }
            if (_mode == 3 && _richTarget2 && _swingCountReverse >= SwingCountReverse)
            {
                _mode = ChangeMode3;
                _richTarget1 = false;
                _richTarget2 = false;
                _richTarget3 = false;
                _richTargetCalcEnable = false;
                _safeModeCalcEnable = false;
                _richTargetSafeCond = false;
                _modeSafe = false;
                return false;
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
            if ( _mode == 3 && _richTarget1 && TakeShort(ReverseSignal3) && _swingCountReverse >= SwingCountReverse)
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
            if (_mode == 1 && _richTarget1  && TakeLong(ExitSignal1) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Short Exit:[" + ExitSignal1 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            if (_mode == 2 && _richTarget1  && TakeLong(ExitSignal2) && _swingCountExit >= SwingCountExit)
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
            if (PositionMaxValid && XTrend.Ma.CompareTo(PositionMax) > 0)
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
            if (_mode == 1 && _richTarget1 && ExitSignal1 > 0 && TakeShort(ExitSignal1) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Exit:[" + ExitSignal1 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            if (_mode == 2 && _richTarget1 && ExitSignal2 > 0 && TakeShort(ExitSignal2) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Exit:[" + ExitSignal2 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            if (_mode == 3 && _richTarget1  && TakeShort(ExitSignal3) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Exit:[" + ExitSignal3 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            if (PositionMinValid && XTrend.Ma.CompareTo(PositionMin) < 0)
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
        private bool IsRiskLowMap(int bit)
        {
            if ((bit & RiskLowMap) > 0)
                return IsMyPositionRiskLow;
            return true;
        }
    }
}
