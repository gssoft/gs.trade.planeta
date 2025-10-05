using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.Strategies
{   
    public class X11823 : X118
    {
        public int FlatCount { get; set; }
        public int CancelOrderMode { get; set; }
        public int CancelReverseMode { get; set; }

        public int RichTargetMode { get; set; }

        public int TakeExitMode { get; set; }

        private string _comment;
        private bool _tryToReverse2;
        private bool _reverse2;
        private int _reverse2Cnt;

        private bool _richTarget;

        protected override void PositionChanged(long op, long np)
        {
            /* Work Properly
            if (np != 0)
            {
                _reverse2 = _tryToReverse2;
                _tryToReverse2 = false;
            }
            else
            {
                _reverse2 = false;
            }
             */
            _richTarget = false;

            if (np != 0)
            {
                if (!_tryToReverse2)
                {
                    _reverse2Cnt = 0;
                    _reverse2 = false;
                }
                else
                {
                    if (_reverse2Cnt == 0)
                    {
                        _reverse2 = true;
                        _tryToReverse2 = false;
                        _reverse2Cnt++;
                    }
                    else
                    {
                        _reverse2 = false;
                        _tryToReverse2 = false;
                    }
                }
            }
            else
            {
                _reverse2 = false;
            }
            ReverseLossCnt = _reverse2Cnt;
        }

        protected override void Prefix()
        {
            if (RichTargetMode== 0) return;

            if (_richTarget) return;

            if (RichTargetMode == 1)
            {
                if (Position.IsLong && PositionMaxValid && XTrend.Ma.CompareTo(PositionMax) > 0)
                    _richTarget = true;
                else if (Position.IsShort && PositionMinValid && XTrend.Ma.CompareTo(PositionMin) < 0)
                    _richTarget = true;
            }
            else if (RichTargetMode == 2)
            {
                if (Position.IsLong && XTrend.Ma.CompareTo(XTrend.High2) > 0)
                    _richTarget = true;
                else if (Position.IsShort && XTrend.Ma.CompareTo(XTrend.Low2) < 0)
                    _richTarget = true;
            }
            else if (RichTargetMode == 3)
            {
                if (_reverse2Cnt > 0)
                {
                    if (Position.IsLong && PositionMaxValid && XTrend.Ma.CompareTo(PositionMax) > 0)
                        _richTarget = true;
                    else if (Position.IsShort && PositionMinValid && XTrend.Ma.CompareTo(PositionMin) < 0)
                        _richTarget = true;
                }
            }

            RichTarget = _richTarget ? 1 : 0;
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
                    _tryToReverse2 = false;
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
                    _tryToReverse2 = false;
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
            if ( _reverse2Cnt == 0 && PositionMax.CompareTo(Single.MinValue) != 0 && XTrend.Ma.CompareTo(PositionMax) > 0)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _tryToReverse2 = true;
                    return true;
                }
            }
            if (_reverse2) 
                return false;

            if (!_richTarget && ReverseSignal1 > 0 && XTrend.TakeLong(ReverseSignal1) && XTrend.FlatCount >= FlatCount
                 && _swingCountReverse >= SwingCountReverse
                // && PositionMin.CompareTo(Single.MaxValue) != 0 && XTrend.Ma.CompareTo(PositionMin) < 0
                )
            {
                if (XTrend.GetPrice5(+1, ReversePriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Reverse To Long:[" + ReverseSignal1 + "] " + _comment;
                    LastSignalId = ReverseSignal1;
                    _tryToReverse2 = false;
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
            if (_reverse2Cnt == 0 && PositionMin.CompareTo(Single.MaxValue) != 0 && XTrend.Ma.CompareTo(PositionMin) < 0)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _tryToReverse2 = true;
                    return true;
                }
            }

            if (_reverse2)
                return false;

            if (!_richTarget && ReverseSignal1 > 0 && XTrend.TakeShort(ReverseSignal1) && XTrend.FlatCount >= FlatCount
                && _swingCountReverse >= SwingCountReverse
                // && PositionMax.CompareTo(Single.MinValue) != 0 && XTrend.Ma.CompareTo(PositionMax) > 0
                )
            {
                if (XTrend.GetPrice5(-1, ReversePriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Reverse To Short:[" + ReverseSignal1 + "] " + _comment;
                    LastSignalId = ReverseSignal1;
                    _tryToReverse2 = false;
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
            if (_reverse2)
            {
                if ( 
                    (TakeExitMode == 0 && ExitSignal1 > 0 && XTrend.TakeLong(ExitSignal1) && _swingCountExit >= SwingCountExit)
                    ||
                    (TakeExitMode == 1 && ExitSignal1 > 0 && XTrend.TakeLong(ExitSignal1) && _swingCountExit >= SwingCountExit
                    && PositionMin.CompareTo(Single.MaxValue) != 0 && XTrend.Ma.CompareTo(PositionMin) < 0)
                    )
                {
                    if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                    {
                        _comment = "Short Exit:[" + ExitSignal1 + "] " + _comment;
                        _tryToReverse2 = false;
                        return true;
                    }
                }
            }
            if ( _richTarget && ExitSignal1 > 0 && XTrend.TakeLong(ExitSignal1) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Short Exit:[" + ExitSignal1 + "] " + _comment;
                    _tryToReverse2 = false;
                    return true;
                }
            }
            if (PositionMax.CompareTo(Single.MinValue) != 0 && XTrend.Ma.CompareTo(PositionMax) > 0)
            {
                if (XTrend.GetPrice5(+1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _tryToReverse2 = false;
                    return true;
                }
            }
            return false;
        }
        protected override bool LongExit()
        {
            if (_reverse2)
            {
                if (
                    (TakeExitMode == 0 && ExitSignal1 > 0 && XTrend.TakeShort(ExitSignal1) && _swingCountExit >= SwingCountExit)
                    ||
                    (TakeExitMode == 1 && ExitSignal1 > 0 && XTrend.TakeShort(ExitSignal1) && _swingCountExit >= SwingCountExit
                    && PositionMax.CompareTo(Single.MinValue) != 0 && XTrend.Ma.CompareTo(PositionMax) > 0)
                    )
                {
                    if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                    {
                        _comment = "Long Exit:[" + ExitSignal1 + "] " + _comment;
                        _tryToReverse2 = false;
                        return true;
                    }
                }
            }
            if ( _richTarget && ExitSignal1 > 0 && XTrend.TakeShort(ExitSignal1) && _swingCountExit >= SwingCountExit)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Long Exit:[" + ExitSignal1 + "] " + _comment;
                    _tryToReverse2 = false;
                    return true;
                }
            }
            if (PositionMin.CompareTo(Single.MaxValue) != 0 && XTrend.Ma.CompareTo(PositionMin) < 0)
            {
                if (XTrend.GetPrice5(-1, ExitPriceId, out _currentLimitPrice, out _comment))
                {
                    _tryToReverse2 = false;
                    return true;
                }
            }
            return false;
        }
    }
}
