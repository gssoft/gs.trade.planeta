using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.Strategies
{
    public class X11825 : X118
    {
        public int CancelOrderMode { get; set; }
        private string _comment;

        protected override bool ShortEntry()
        {
            if ( XTrend.TakeShort(EntrySignal1) && _swingCountEntry >= SwingCountEntry)
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
            if (XTrend.TakeLong(EntrySignal1) && _swingCountEntry >= SwingCountEntry )
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
            if ( XTrend.TakeLong(ReverseSignal1) && _swingCountReverse >= SwingCountReverse)
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
            if (XTrend.TakeShort(ReverseSignal1) && _swingCountReverse >= SwingCountReverse)
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
            if (XTrend.TakeLong(ExitSignal1) && _swingCountExit >= SwingCountExit)
            {
                _currentLimitPrice = XTrend.GetPrice3(+1, ExitPriceId, out _comment);
                _comment = "Short Exit:[" + ExitSignal1 + "] " + _comment;

                return true;
            }
            return false;
        }
        protected override bool LongExit()
        {
            if (XTrend.TakeShort(ExitSignal1) && _swingCountExit >= SwingCountExit)
            {
                _currentLimitPrice = XTrend.GetPrice3(-1, ExitPriceId, out _comment);
                _comment = "Long Exit:[" + ExitSignal1 + "] " + _comment;

                return true;
            }
            return false;
        }
    }
}
