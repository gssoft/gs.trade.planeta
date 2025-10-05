using System;

namespace GS.Trade.Strategies
{
    public class X11830 : X118
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

        public int RichTargetMode21 { get; set; }
        public int RichTargetMode22 { get; set; }

        public int RichTargetMode31 { get; set; }
        public int RichTargetMode32 { get; set; }

        public int RichTargetSafeCond1 { get; set; }
        public int RichTargetSafeCond2 { get; set; }

        public int ReverseSignal31 { get; set; }
        public int ReverseSignal32 { get; set; }

        public int ChangeModeCondSignal1 { get; set; }
        public int ChangeModeCondSignal2 { get; set; }

        public int SafeModeCondSignal1 { get; set; }
        public int SafeModeCondSignal2 { get; set; }

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
                    if (XTrend.HaveHigher((int)TimeInt * 60 * 2, XTrend.High, out m))
                        PositionMax = (float)m;
                }
            }
            else if (Position.IsLong)
            {
                if (!PositionMinValid)
                {
                    double m;
                    if (XTrend.HaveLower((int)TimeInt * 60 * 2, XTrend.Low, out m))
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
                switch (RichTargetMode31)
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
                switch (RichTargetSafeCond1)
                {
                    case 0:
                        _richTargetSafeCond = false;
                        break;
                    case 1:
                        _richTargetSafeCond = true;
                        break;
                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTargetSafeCond = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTargetSafeCond = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTargetSafeCond = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTargetSafeCond = true;
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
                switch (RichTargetMode32)
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
                switch (RichTargetSafeCond2)
                {
                    case 0:
                        _richTargetSafeCond = false;
                        break;
                    case 1:
                        _richTargetSafeCond = true;
                        break;
                    case 5:
                        if (Position.IsLong && PositionMaxValid && high.CompareTo(PositionMax) > 0)
                        {
                            _richTargetSafeCond = true;
                        }
                        else if (Position.IsShort && PositionMinValid && low.CompareTo(PositionMin) < 0)
                        {
                            _richTargetSafeCond = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && high.CompareTo(XTrend.High2) > 0)
                        {
                            _richTargetSafeCond = true;
                        }
                        else if (Position.IsShort && low.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTargetSafeCond = true;
                        }
                        break;
                }
            }

            if (_richTarget1)
            {
                _modeSafe = false;
                _richTarget3 = false;
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

            if (XTrend.TakeShort(entrySignal) && XTrend.FlatCount >= FlatCount
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

            if (XTrend.TakeLong(entrySignal) && XTrend.FlatCount >= FlatCount
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

                if (XTrend.TakeShort(EntrySignal11))
                        entrySignal = EntrySignal11;
                else if (XTrend.TakeShort(EntrySignal12))
                        entrySignal = EntrySignal12;
                else if (XTrend.TakeShort(EntrySignal13))
                        entrySignal = EntrySignal13;
                else 
                /*
                if (XTrend.TakeShort(entrySignal)  && _swingCountEntry >= SwingCountEntry)
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
                        KillOrders(OrderTypeEnum.Limit, OperationEnum.Sell);
                    }
                }
                entrySignal = EntrySignal12;
                if (XTrend.TakeShort(entrySignal) && _swingCountEntry >= SwingCountEntry)
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
                        KillOrders(OrderTypeEnum.Limit, OperationEnum.Sell);
                    }
                }
                 */
                return false;
            }

            if (entrySignal <= 0)
                throw new NullReferenceException("EntrySignal==0");

            if (XTrend.TakeShort(entrySignal)  && _swingCountEntry >= SwingCountEntry)
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
                if (XTrend.TakeLong(EntrySignal11))
                    entrySignal = EntrySignal11;
                else if (XTrend.TakeLong(EntrySignal12))
                    entrySignal = EntrySignal12;
                else if (XTrend.TakeLong(EntrySignal13))
                    entrySignal = EntrySignal13;
                else 
                /*
                if (XTrend.TakeLong(entrySignal) && _swingCountEntry >= SwingCountEntry)
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
                        KillOrders(OrderTypeEnum.Limit, OperationEnum.Buy);
                    }
                }
                entrySignal = EntrySignal12;
                if (XTrend.TakeLong(entrySignal) && _swingCountEntry >= SwingCountEntry)
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
                        KillOrders(OrderTypeEnum.Limit, OperationEnum.Buy);
                    }
                }
                 */
                return false;
            }

            if (entrySignal <= 0)
                throw new NullReferenceException("EntrySignal==0");

            if (XTrend.TakeLong(entrySignal) && _swingCountEntry >= SwingCountEntry)
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
            if ((_mode == 1 || _mode == 2) && XTrend.TakeLong(ReverseLossSignal) &&
                    RealReverseLossCnt < ReverseLossCnt && IsPositionRiskLow && XTrend.Ma.CompareTo(PositionMax) > 0)
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
            if (_mode == 1 && _richTarget2 && XTrend.TakeLong(ChangeModeCondSignal1) && _swingCountReverse >= SwingCountReverse)
            {
                _mode = 2;
                _richTarget1 = false;
                _richTarget2 = false;
                _richTarget3 = false;
                _richTargetCalcEnable = false;
                _safeModeCalcEnable = false;
                _richTargetSafeCond = false;
                _modeSafe = false;
                return false;
            }
            if (_mode == 2 && _richTarget2 && XTrend.TakeLong(ChangeModeCondSignal2) && _swingCountReverse >= SwingCountReverse)
            {
                _mode = 1;
                _richTarget1 = false;
                _richTarget2 = false;
                _richTarget3 = false;
                _richTargetCalcEnable = false;
                _safeModeCalcEnable = false;
                _richTargetSafeCond = false;
                _modeSafe = false;
                return false;
            }
            // if don't rich Exit Condition and Begin UpSide
            if ((_mode == 1) && _richTargetSafeCond && !_modeSafe && !_richTarget1 && XTrend.TakeLong(SafeModeCondSignal1) && _swingCountReverse >= SwingCountReverse)
            {
                _richTarget1 = false;
                _richTarget2 = false;
                _richTarget3 = false;
                _richTargetSafeCond = false;
                _modeSafe = true;
                return false;
            }
            if ((_mode == 2) && _richTargetSafeCond && !_modeSafe && !_richTarget1 && XTrend.TakeLong(SafeModeCondSignal2) && _swingCountReverse >= SwingCountReverse)
            {
                _richTarget1 = false;
                _richTarget2 = false;
                _richTarget3 = false;
                _richTargetSafeCond = false;
                _modeSafe = true;
                return false;
            }

            // Reverse After Profit Mode1
            if (_mode == 1 && _richTarget1 && IsPositionRiskLow && XTrend.TakeLong(ReverseSignal1) && _swingCountReverse >= SwingCountReverse)
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
            if (_mode == 2 && _richTarget1 && IsPositionRiskLow && XTrend.TakeLong(ReverseSignal2) && _swingCountReverse >= SwingCountReverse)
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
            if (
                (_mode == 1 && _modeSafe && _richTarget3 && XTrend.TakeLong(ReverseSignal31) && _swingCountReverse >= SwingCountReverse)
                ||
                (_mode == 2 && _modeSafe && _richTarget3 && XTrend.TakeLong(ReverseSignal32) && _swingCountReverse >= SwingCountReverse)
                )
            {
                if (XTrend.GetPrice5(+1, ReversePriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Reverse To LongEmergency:[" + ReverseSignal31 + "] " + _comment;
                    LastSignalId = ReverseSignal31;
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
            if ((_mode == 1 || _mode == 2) && XTrend.TakeShort(ReverseLossSignal) &&
                RealReverseLossCnt < ReverseLossCnt && IsPositionRiskLow && XTrend.Ma.CompareTo(PositionMin) < 0)
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
            if (_mode == 1 && _richTarget2 && XTrend.TakeShort(ChangeModeCondSignal1) && _swingCountReverse >= SwingCountReverse)
            {
                _mode = 2;
                _richTarget1 = false;
                _richTarget2 = false;
                _richTarget3 = false;
                _richTargetCalcEnable = false;
                _safeModeCalcEnable = false;
                _richTargetSafeCond = false;
                _modeSafe = false;
                return false;
            }
            if (_mode == 2 && _richTarget2 && XTrend.TakeShort(ChangeModeCondSignal2) && _swingCountReverse >= SwingCountReverse)
            {
                _mode = 1;
                _richTarget1 = false;
                _richTarget2 = false;
                _richTarget3 = false;
                _richTargetCalcEnable = false;
                _safeModeCalcEnable = false;
                _richTargetSafeCond = false;
                _modeSafe = false;
                return false;
            }
            if ((_mode == 1) && _richTargetSafeCond && !_modeSafe && !_richTarget1 && XTrend.TakeShort(SafeModeCondSignal1) && _swingCountReverse >= SwingCountReverse)
            {
                _richTarget1 = false;
                _richTarget2 = false;
                _richTarget3 = false;
                _richTargetSafeCond = false;
                _modeSafe = true;
                return false;
            }
            if ((_mode == 2) && _richTargetSafeCond && !_modeSafe && !_richTarget1 && XTrend.TakeShort(SafeModeCondSignal2) && _swingCountReverse >= SwingCountReverse)
            {
                _richTarget1 = false;
                _richTarget2 = false;
                _richTarget3 = false;
                _richTargetSafeCond = false;
                _modeSafe = true;
                return false;
            }

            // Reverse After Profit
            if (_mode == 1 && _richTarget1 && IsPositionRiskLow && XTrend.TakeShort(ReverseSignal1) && _swingCountReverse >= SwingCountReverse)
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
            if (_mode == 2 && _richTarget1 && IsPositionRiskLow && XTrend.TakeShort(ReverseSignal2) && _swingCountReverse >= SwingCountReverse)
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
            if (
                (_mode == 1 && _modeSafe && _richTarget3 && XTrend.TakeShort(ReverseSignal31) && _swingCountReverse >= SwingCountReverse)
                ||
                (_mode == 2 && _modeSafe && _richTarget3 && XTrend.TakeShort(ReverseSignal32) && _swingCountReverse >= SwingCountReverse)
                )
            {
                if (XTrend.GetPrice5(-1, ReversePriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Reverse To Short:[" + ReverseSignal31 + "] " + _comment;
                    LastSignalId = ReverseSignal31;
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
            if (_mode == 1 && _richTarget1 && ExitSignal1 > 0 && XTrend.TakeLong(ExitSignal1) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Short Exit:[" + ExitSignal1 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            if (_mode == 2 && _richTarget1 && ExitSignal2 > 0 && XTrend.TakeLong(ExitSignal2) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Short Exit:[" + ExitSignal2 + "] " + _comment;
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
            if (_mode == 1 && _richTarget1 && ExitSignal1 > 0 && XTrend.TakeShort(ExitSignal1) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Exit:[" + ExitSignal1 + "] " + _comment;
                    _tryEnterMode = -1;
                    LastExitMode = _mode;
                    return true;
                }
            }
            if (_mode == 2 && _richTarget1 && ExitSignal2 > 0 && XTrend.TakeShort(ExitSignal2) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Exit:[" + ExitSignal2 + "] " + _comment;
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
                return IsPositionRiskLow;
            return true;
        }
    }
}
