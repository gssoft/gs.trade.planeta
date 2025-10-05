using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.Strategies
{
    public class X11810 : X118
    {
        public int FlatCount { get; set; }
        public int CancelOrderMode { get; set; }
        public int CancelReverseMode { get; set; }

        public int RichTargetMode { get; set; }
        public int RichTargetMode2 { get; set; }

        public int ZMode { get; set; }

        public int TakeExitMode { get; set; }

        private string _comment;

        private bool _reverse2;
        // private int _reverse2Cnt;

        private bool _richTarget1;
        private bool _richTarget2;

        private bool _tryToReverseLoss;
        private bool _reverseLossMode;
        private int _reverseLossCnt;

        private int _mode;
        private int _tryEnterMode;
        private int _incrLoss;

        protected override void PositionChanged(long op, long np)
        {
            _richTarget1 = false;
            _richTarget2 = false;

            RichTarget = 0;

            if (np != 0)
            {
                if (_tryEnterMode != 2)
                {
                    _tryEnterMode = 0;
                    _reverseLossCnt = 0;
                    _incrLoss = 0;
                    _mode = 1;
                }
                else
                {
                    //if (_reverseLossCnt < ReverseLossCnt)
                    //{
                    _tryEnterMode = 0;
                    _reverseLossCnt = +_incrLoss;
                    _incrLoss = 0;
                    _mode = 2;
                    //}
                    /*
                    else
                    {
                        _mode = 0;
                        _tryToReverseLoss = false;
                    }
                     * */
                }
            }
            else
            {
                _mode = 1;
                _reverseLossCnt = 0;
                _incrLoss = 0;
            }
            RealReverseLossCnt = _reverseLossCnt;
            Mode = _mode;
        }

        protected override void Prefix()
        {
            // if (_richTarget1 ) return;

            if (_mode == 1)
            {
                switch (RichTargetMode)
                {
                    case 0:
                        _richTarget1 = true;
                        break;
                    case 3:
                        if (Position.IsLong && PositionMaxValid && XTrend.Ma.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && XTrend.Ma.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget1 = true;
                        }
                        if (Position.IsLong && PositionMaxValid && XTrend.Ma.CompareTo(PositionMax) > 0)
                        {
                            _richTarget2 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && XTrend.Ma.CompareTo(PositionMin) < 0)
                        {
                            _richTarget2 = true;
                        }
                        break;
                    case 5:
                        if (Position.IsLong && PositionMaxValid && XTrend.Ma.CompareTo(PositionMax) > 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && XTrend.Ma.CompareTo(PositionMin) < 0)
                        {
                            _richTarget1 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && PositionMaxValid && XTrend.Ma.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && XTrend.Ma.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget1 = true;
                        }
                        break;
                }
            }
            else if (_mode == 2)
            {
                switch (RichTargetMode2)
                {
                    case 0:
                        _richTarget1 = true;
                        break;
                    case 5:
                        if (Position.IsLong && PositionMaxValid && XTrend.Ma.CompareTo(PositionMax) > 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && XTrend.Ma.CompareTo(PositionMin) < 0)
                        {
                            _richTarget1 = true;
                        }
                        break;
                    case 8:
                        if (Position.IsLong && PositionMaxValid && XTrend.Ma.CompareTo(XTrend.High2) > 0)
                        {
                            _richTarget1 = true;
                        }
                        else if (Position.IsShort && PositionMinValid && XTrend.Ma.CompareTo(XTrend.Low2) < 0)
                        {
                            _richTarget1 = true;
                        }
                        break;
                }
            }
            RichTarget = _richTarget2 ? 2 : _richTarget1 ? 1 : 0;
            Mode = _mode;
        }

        protected override bool ShortEntry()
        {
            if (EntrySignal1 > 0 && XTrend.TakeShort(EntrySignal1) && XTrend.FlatCount >= FlatCount
                    && _swingCountEntry >= SwingCountEntry)
            {
                if (XTrend.GetPrice5(-1, EntryPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Short Entry:[" + EntrySignal1 + "] " + _comment;
                    LastSignalId = EntrySignal1;
                    _tryToReverseLoss = false;
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
            if (EntrySignal1 > 0 && XTrend.TakeLong(EntrySignal1) && XTrend.FlatCount >= FlatCount
                    && _swingCountEntry >= SwingCountEntry)
            {
                if (XTrend.GetPrice5(+1, EntryPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Entry:[" + EntrySignal1 + "] " + _comment;
                    LastSignalId = EntrySignal1;
                    _tryToReverseLoss = false;
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
            //if (_mode == 1 && RealReverseLossCnt < ReverseLossCnt && PositionMaxValid && XTrend.Ma.CompareTo(PositionMax) > 0)
            if (_mode == 1 && IsPositionRiskLow && PositionMaxValid && XTrend.Ma.CompareTo(PositionMax) > 0)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _tryEnterMode = 2;
                    _incrLoss = 1;
                    return true;
                }
            }
            // RichTarget2. Enter Mod2
            if (_mode == 1 && _richTarget2  && ReverseSignal1 > 0 && XTrend.TakeLong(ReverseSignal1) && _swingCountReverse >= SwingCountReverse)
            {
                _mode = 2;
                _richTarget1 = false;
                _richTarget2 = false;
                ClearValue(out PositionMin, +1);
                return false;
            }
            // Reverse After Profit
            if (_mode == 1 && !_richTarget2 && _richTarget1 && ReverseSignal1 > 0 && XTrend.TakeLong(ReverseSignal1) && _swingCountReverse >= SwingCountReverse)
            {
                if (XTrend.GetPrice5(+1, ReversePriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Reverse To Long:[" + ReverseSignal1 + "] " + _comment;
                    LastSignalId = ReverseSignal1;
                    _tryToReverseLoss = false;
                    return true;
                }
                if (CancelOrderMode > 0)
                {
                    LastSignalId = 0;
                    KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Buy);
                }
            }
            // Reverse After Profit Mode2
            if (_mode == 2 && _richTarget1 /* && IsPositionRiskLow */ && ReverseSignal2 > 0 && XTrend.TakeLong(ReverseSignal2) && _swingCountReverse >= SwingCountReverse)
            {
                if (XTrend.GetPrice5(+1, ReversePriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Reverse To Long:[" + ReverseSignal1 + "] " + _comment;
                    LastSignalId = ReverseSignal2;
                    _tryEnterMode = 2;
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
            //if (_mode == 1 && RealReverseLossCnt < ReverseLossCnt && PositionMinValid && XTrend.Ma.CompareTo(PositionMin) < 0)
            if (_mode == 1 && IsPositionRiskLow && PositionMinValid && XTrend.Ma.CompareTo(PositionMin) < 0)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _tryEnterMode = 2;
                    _incrLoss = 1;
                    return true;
                }
            }
            if (_mode == 1 && _richTarget2 && ReverseSignal1 > 0 && XTrend.TakeShort(ReverseSignal1) && _swingCountReverse >= SwingCountReverse)
            {
                _mode = 2;
                _richTarget1 = false;
                _richTarget2 = false;
                ClearValue(out PositionMax, -1);
                return false;
            }
            // Reverse After Profit
            if (_mode == 1 && !_richTarget2 && _richTarget1 && ReverseSignal1 > 0 && XTrend.TakeShort(ReverseSignal1) && _swingCountReverse >= SwingCountReverse)
            {
                if (XTrend.GetPrice5(-1, ReversePriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Reverse To Short:[" + ReverseSignal1 + "] " + _comment;
                    LastSignalId = ReverseSignal1;
                    return true;
                }
                if (CancelOrderMode > 0)
                {
                    LastSignalId = 0;
                    KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Sell);
                }
            }
            // Reverse After Profit
            if (_mode == 2 && _richTarget1 /*&& IsPositionRiskLow */ && ReverseSignal2 > 0 && XTrend.TakeShort(ReverseSignal2)
                && _swingCountReverse >= SwingCountReverse)
            {
                if (XTrend.GetPrice5(-1, ReversePriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Reverse To Short:[" + ReverseSignal2 + "] " + _comment;
                    LastSignalId = ReverseSignal1;
                    _tryEnterMode = 2;
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
                    _tryToReverseLoss = false;
                    return true;
                }
            }
            if (_mode == 2 && _richTarget1 && ExitSignal2 > 0 && XTrend.TakeLong(ExitSignal2) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Short Exit:[" + ExitSignal2 + "] " + _comment;
                    _tryToReverseLoss = false;
                    return true;
                }
            }
            if (PositionMaxValid && XTrend.Ma.CompareTo(PositionMax) > 0)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _tryToReverseLoss = false;
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
                    _tryToReverseLoss = false;
                    return true;
                }
            }
            if (_mode == 2 && _richTarget1 && ExitSignal2 > 0 && XTrend.TakeShort(ExitSignal2) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Exit:[" + ExitSignal2 + "] " + _comment;
                    _tryToReverseLoss = false;
                    return true;
                }
            }
            if (PositionMinValid && XTrend.Ma.CompareTo(PositionMin) < 0)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _tryToReverseLoss = false;
                    return true;
                }
            }
            return false;
        }
    }
}
