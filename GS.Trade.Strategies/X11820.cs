using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.Strategies
{
    public class X11820 : X118
    {
        public int FlatCount { get; set; }
        public int CancelOrderMode { get; set; }
        public int CancelReverseMode { get; set; }

        private string _comment;

        protected override bool ShortEntry()
        {
            if (EntrySignal1 > 0 && XTrend.TakeShort(EntrySignal1) && XTrend.FlatCount >= FlatCount
                    && _swingCountEntry >= SwingCountEntry)
            {
                if (XTrend.GetPrice5(-1, EntryPriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Short Entry:[" + EntrySignal1 + "] " + _comment;
                    LastSignalId = EntrySignal1;
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
                    return true;
                }
                if( CancelOrderMode > 0 )
                {
                    LastSignalId = 0;
                    KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Buy);
                }
            }
            return false;
        }
        protected override bool ShortReverse()
        {
            if (ReverseSignal1 > 0 && XTrend.TakeLong(ReverseSignal1) && XTrend.FlatCount >= FlatCount
                 && _swingCountReverse >= SwingCountReverse)
            {
                if (XTrend.GetPrice5(+1, ReversePriceId, out _currentLimitPrice, out _comment))
                {
                    _comment = "Reverse To Long:[" + ReverseSignal1 + "] " + _comment;
                    LastSignalId = ReverseSignal1;
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
            if (ReverseSignal1 > 0 && XTrend.TakeShort(ReverseSignal1) && XTrend.FlatCount >= FlatCount
                && _swingCountReverse >= SwingCountReverse)
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
            return false;
        }
    }
  
}
