using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.Strategies
{
    public class X11828 : X118
    {
        public int CancelOrderMode { get; set; }
        private string _comment;

        private float _posMin;
        private float _posMax;

        private float _posMax2;
        private float _posMin2;

        private bool _canExit;

        protected override void PositionChanged(long op, long np)
        {
            if (!Position.IsOpened) return;

            _posMin = Single.MaxValue;
            _posMax = Single.MinValue;

            _posMin2 = Single.MinValue;
            _posMax2 = Single.MaxValue;

            _canExit = false;

        }
        protected override void Prefix()
        {
            if (Position.IsLong && XTrend.IsUp)
                _posMin = Math.Min(_posMin, (float)XTrend.Low2);
            if (Position.IsLong && XTrend.IsDown)
                _posMax = Math.Max(_posMax, (float)XTrend.High2);

            if (Position.IsShort && XTrend.IsDown)
                _posMax = Math.Max(_posMax, (float)XTrend.High2);
            if (Position.IsShort && XTrend.IsUp)
                _posMin = Math.Min(_posMin, (float)XTrend.Low2);

            if( Position.IsLong)
            {
                if (!_canExit && _posMax != Single.MinValue)
                {
                    if (XTrend.TakeShort(ExitSignal1) && _swingCountExit >= SwingCountExit)
                        _posMax2 = _posMax;
                    if(XTrend.Ma.CompareTo(_posMax2) > 0)
                        _canExit = true;
                }
            }
            if (Position.IsShort)
            {
                if (!_canExit && _posMin != Single.MaxValue)
                {
                    if (XTrend.TakeLong(ExitSignal1) && _swingCountExit >= SwingCountExit)
                        _posMin2 = _posMin;

                    if (XTrend.Ma.CompareTo(_posMin2) < 0)
                        _canExit = true;
                }
            }
        }

        protected override bool ShortEntry()
        {
            if (XTrend.TakeShort(EntrySignal1) && _swingCountEntry >= SwingCountEntry)
            {
                _currentLimitPrice = XTrend.GetPrice3(-1, EntryPriceId, out _comment);
                _comment = "Short Entry:[" + EntrySignal1 + "] " + _comment;

                LastSignalId = EntrySignal1;

                return true;
            }
            if (CancelOrderMode > 0 && LastSignalId != 0 && LastSignalId == EntrySignal1)
            {
                LastSignalId = 0;
                KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Sell);
            }
            return false;
        }
        protected override bool LongEntry()
        {
            if (XTrend.TakeLong(EntrySignal1) && _swingCountEntry >= SwingCountEntry)
            {
                _currentLimitPrice = XTrend.GetPrice3(+1, EntryPriceId, out _comment);
                _comment = "Long Entry:[" + EntrySignal1 + "] " + _comment;

                LastSignalId = EntrySignal1;

                return true;
            }
            if (CancelOrderMode > 0 && LastSignalId != 0 && LastSignalId == EntrySignal1)
            {
                LastSignalId = 0;
                KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Buy);
            }
            return false;
        }
        protected override bool ShortReverse()
        {
            if ( _canExit && XTrend.TakeLong(ReverseSignal1) && _swingCountReverse >= SwingCountReverse)
            {
                _currentLimitPrice = XTrend.GetPrice3(+1, ReversePriceId, out _comment);
                _comment = "Reverse to Long:[" + ReverseSignal1 + "] " + _comment;

                LastSignalId = ReverseSignal1;

                return true;
            }
            if (CancelOrderMode > 0 && LastSignalId != 0 && LastSignalId == ReverseSignal1)
            {
                LastSignalId = 0;
                KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Buy);
            }
            return false;
        }
        protected override bool LongReverse()
        {
            if (_canExit &&  XTrend.TakeShort(ReverseSignal1) && _swingCountReverse >= SwingCountReverse)
            {
                _currentLimitPrice = XTrend.GetPrice3(-1, ReversePriceId, out _comment);
                _comment = "Reverse To Short:[" + ReverseSignal1 + "] " + _comment;

                LastSignalId = ReverseSignal1;

                return true;
            }
            if (CancelOrderMode > 0 && LastSignalId != 0 && LastSignalId == ReverseSignal1)
            {
                LastSignalId = 0;
                KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Sell);
            }
            return false;
        }
        protected override bool ShortExit()
        {
            if (_posMax.CompareTo(Single.MinValue) != 0 && XTrend.Ma.CompareTo(_posMax) > 0)
            {
                _currentLimitPrice = XTrend.GetPrice3(+1, ExitPriceId, out _comment);
                return true;
            }
            if (_canExit && XTrend.TakeLong(ExitSignal1) && _swingCountExit >= SwingCountExit)
            {
                    _currentLimitPrice = XTrend.GetPrice3(+1, ExitPriceId, out _comment);
                    _comment = "Short Exit:[" + ExitSignal1 + "] " + _comment;

                    return true;
            }
            return false;
        }
        protected override bool LongExit()
        {
            if (_posMin.CompareTo(Single.MaxValue) != 0 && XTrend.Ma.CompareTo(_posMin) < 0)
            {
                _currentLimitPrice = XTrend.GetPrice3(-1, ExitPriceId, out _comment);
                return true;
            }
            if (_canExit && XTrend.TakeShort(ExitSignal1) && _swingCountExit >= SwingCountExit)
            {
                    _currentLimitPrice = XTrend.GetPrice3(-1, ExitPriceId, out _comment);
                    _comment = "Long Exit:[" + ExitSignal1 + "] " + _comment;

                    return true;
            }
            return false;
        }
    }
}
