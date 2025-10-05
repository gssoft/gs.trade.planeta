namespace GS.Trade.Strategies
{
    public class X11819 : X118
    {
        public int FlatCount { get; set; }
        public int CancelOrderMode { get; set; }
        public int CancelReverseMode { get; set; }

        private string _comment;
        
        protected override bool ShortEntry()
        {
            if ( MyEntrySignal1 > 0 && XTrend.TakeShort(MyEntrySignal1) && XTrend.FlatCount >= FlatCount
                    && _swingCountEntry >= SwingCountEntry
                    && XTrend.SwingCount >= SwingCount)
            {
                _currentLimitPrice = XTrend.GetPrice3(-1, EntryPriceId, out _comment);
                _comment = "Short Entry:[" + MyEntrySignal1 + "] " + _comment;

                LastSignalId = MyEntrySignal1;

                return true;
            }
            if (CancelOrderMode > 0 && LastSignalId == MyEntrySignal1)
            {
                LastSignalId = 0;
                KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Sell);
            }
            return false;
        }
        protected override bool LongEntry()
        {
            if (MyEntrySignal1 > 0 && XTrend.TakeLong(MyEntrySignal1) && XTrend.FlatCount >= FlatCount
                    && _swingCountEntry >= SwingCountEntry
                    && XTrend.SwingCount >= SwingCount)
            {
                _currentLimitPrice = XTrend.GetPrice3(+1, EntryPriceId, out _comment);
                _comment = "Long Entry:[" + MyEntrySignal1 + "] " + _comment;

                LastSignalId = MyEntrySignal1;

                return true;
            }
            if (CancelOrderMode > 0 && LastSignalId == MyEntrySignal1)
            {
                LastSignalId = 0;
                KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Buy);
            }
            return false;
        }
        protected override bool ShortReverse()
        {
            if (CancelReverseMode == 1 && XTrend.IsDown2 && XTrend.IsPrevUp2) 
                MyReverseSignal1 = 0;

            if (MyReverseSignal1 > 0 && XTrend.TakeLong(MyReverseSignal1) && XTrend.FlatCount >= FlatCount
                 && _swingCountReverse >= SwingCountReverse )
            {
                _currentLimitPrice = XTrend.GetPrice3(+1, ReversePriceId, out _comment);
                _comment = "Reverse to Long:[" + MyReverseSignal1 + "] " + _comment;

                LastSignalId = MyReverseSignal1;

                return true;
            }
            if (CancelOrderMode > 0 && LastSignalId == MyReverseSignal1)
            {
                LastSignalId = 0;
                KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Buy);
            }
            return false;
        }
        protected override bool LongReverse()
        {
            if (CancelReverseMode == 1 && XTrend.IsUp2 && XTrend.IsPrevDown2)
                MyReverseSignal1 = 0;

            if (MyReverseSignal1 > 0 && XTrend.TakeShort(MyReverseSignal1) && XTrend.FlatCount >= FlatCount
                && _swingCountReverse >= SwingCountReverse )
            {
                _currentLimitPrice = XTrend.GetPrice3(-1, ReversePriceId, out _comment);
                _comment = "Reverse To Short:[" + MyReverseSignal1 + "] " + _comment;

                LastSignalId = MyReverseSignal1;

                return true;
            }
            if (CancelOrderMode > 0 && LastSignalId == MyReverseSignal1)
            {
                LastSignalId = 0;
                KillOrders(OrderTypeEnum.Limit, OrderOperationEnum.Sell);
            }
            return false;
        }

        protected override bool ShortExit()
        {
            if ( XTrend.TakeLong(MyExitSignal1) && _swingCountExit >= SwingCountExit )
            {
                _currentLimitPrice = XTrend.GetPrice3(+1, ExitPriceId, out _comment);
                _comment = "Short Exit:[" + MyExitSignal1 + "] " + _comment;

                return true;
            }
            return false;
        }
        protected override bool LongExit()
        {
            if ( XTrend.TakeShort(MyExitSignal1)&& _swingCountExit >= SwingCountExit )
            {
                _currentLimitPrice = XTrend.GetPrice3(-1, ExitPriceId, out _comment);
                _comment = "Long Exit:[" + MyExitSignal1 + "] " + _comment;

                return true;
            }
            return false;
        }
    }
}
