using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;
using GS.Interfaces;

namespace GS.Trade.Strategies
{
    public  partial class Z007
    {
        public override void TimePlanEventHandler(object sender, ITimePlanEventArgs args)
        {
            // var tpie = (TimePlanItemEvent) sender;

            //TradeContext.Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, StrategyTickerString, tpie.Key,
            //    args.Description, args.ToString());
            switch (args.EventType)
            {
                case TimePlanEventType.TimePlanItem:
                    switch (args.TimePlanItemCode)
                    {
                        case "MORNING":
                            switch (args.Msg)
                            {
                                case "START":
                                    if (!IsMorningSessionEnabled)
                                    {
                                        Evlm2(EvlResult.WARNING, EvlSubject.INIT,
                                        "Strategies", StrategyTickerString,
                                        $"{args.TimePlanItemCode} Session: {args.Msg} DISABLED, Working: {Working}", "", "");
                                        break;
                                    }
                                    MorningSessionStartPrefix();
                                    StartNewDayInit();
                                    //18.09.11
                                    Working = false;

                                    // TrEntryEnabled.Reset();
                                    // _swingCountEntry = SwingCountEntry - SwingCountStartEntry;

                                    SetExitMode(0, args.ToString());
                                    Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                                        "Strategies", StrategyTickerString,
                                        $"{args.TimePlanItemCode} Session: {args.Msg}, Working: {Working}",
                                        $"IsMorningSessionClose: {IsMorningSessionNeedClose}," +
                                        $" IsEveningSessionClose: {IsEveningSessionNeedClose}", "");
                                    break;
                                case "FINISH":
                                    SetExitMode(0, args.ToString());
                                    Working = false;
                                    Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                                        "Strategies", StrategyTickerString, $"{args.TimePlanItemCode} Session: {args.Msg}, Working: {Working}", "", "");
                                    break;
                            }
                            break;
                        case "EVENING":
                            switch (args.Msg)
                            {
                                case "START":
                                    if (!IsEveningSessionEnabled)
                                    {
                                        Evlm2(EvlResult.WARNING, EvlSubject.INIT,
                                        "Strategies", StrategyTickerString,
                                        $"{args.TimePlanItemCode} Session: {args.Msg}, Working: {Working}",
                                        $"IsMorningSessionClose: {IsMorningSessionNeedClose}," +
                                        $" IsEveningSessionClose: {IsEveningSessionNeedClose}", "");
                                        break;
                                    }
                                    if (IsMorningSessionNeedClose)
                                        StartNewDayInit();
                                    Working = false;
                                    SetExitMode(0, args.ToString());
                                    Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                                        "Strategies", StrategyTickerString,
                                         $"{args.TimePlanItemCode} Session: {args.Msg}, Working: {Working}",
                                        $"IsMorningSessionClose: {IsMorningSessionNeedClose}, IsEveningSessionClose: {IsEveningSessionNeedClose}", "");
                                    break;
                                case "FINISH":
                                    SetExitMode(0, args.ToString());
                                    Working = false;
                                    Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                                        "Strategies", StrategyTickerString, $"{args.TimePlanItemCode} Session: {args.Msg}, Working: {Working}", "", "");
                                    break;
                            }
                            break;
                    }
                    break;
                case TimePlanEventType.TimePlanItemEvent:
                    switch (args.TimePlanItemCode)
                    {
                        case "MORNING":
                            switch (args.TimePlanItemEventCode)
                            {
                                case "AFTER":
                                    switch (args.Msg)
                                    {
                                        case "AMERICANSESSIONCLOSE":
                                            IsAmericanSession = false;
                                            Evlm2(EvlResult.WARNING, EvlSubject.INIT,
                                            "Strategies", StrategyTickerString,
                                            $"{args.TimePlanItemCode}, {args.TimePlanItemEventCode}, Time: {args.Msg}," +
                                            $" Working: {Working}",
                                            $"IsAmericanSessionStatus: {IsAmericanSession},", "");
                                            break;

                                        // case "5_MIN":
                                        case "1_MIN":
                                            MorningSessionStartWorkingPrefix();
                                            Working = true;
                                            Evlm2(EvlResult.WARNING, EvlSubject.INIT,
                                            "Strategies", StrategyTickerString,
                                            $"{args.TimePlanItemCode}, {args.TimePlanItemEventCode}, Time: {args.Msg} , Working: {Working}",
                                            $"IsMorningSessionNeedClosePos: {IsMorningSessionNeedClose}," +
                                            $" IsEveningSessionNeedClosePos: {IsEveningSessionNeedClose}", "");

                                            break;
                                    }
                                    break;
                                case "TOEND":
                                    switch (args.Msg)
                                    {
                                        //case "15_SEC":
                                        //    SetExitMode(0, args.ToString());
                                        //    break;
                                        //case "1_MIN":
                                        //    SetExitMode(0, args.ToString());
                                        //    break;
                                        //case "6_MIN":
                                        //    SetExitMode(11, args.ToString());
                                        //    Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                                        //        "Strategies", StrategyTickerString, $"ExitMode: {ExitMode}", $"Session: {args.TimePlanItemCode} Minutes: {args.Msg} {args.TimePlanItemEventCode}", "");
                                        //    break;
                                        //case "9_MIN":
                                        // case "10_Min":
                                         case "1_MIN":
                                            if (!IsMorningSessionNeedClose)
                                                break;
                                            SafeContractsToRest = SafeContractsOvs;
                                            SetExitMode(12, args.ToString());
                                            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                                                "Strategies", StrategyTickerString,
                                                $"ExitMode: {ExitMode}", $"Session: {args.TimePlanItemCode} Minutes: {args.Msg} {args.TimePlanItemEventCode}", "");
                                            break;

                                        case "AMERICANSESSIONOPEN":
                                            IsAmericanSession = true;
                                            Evlm2(EvlResult.WARNING, EvlSubject.INIT,
                                            "Strategies", StrategyTickerString,
                                            $"{args.TimePlanItemCode}, {args.TimePlanItemEventCode}, Time: {args.Msg}," +
                                            $" Working: {Working}",
                                            $"IsAmericanSessionStatus: {IsAmericanSession},", "");
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case "EVENING":
                            switch (args.TimePlanItemEventCode)
                            {
                                case "AFTER":
                                    switch (args.Msg.TrimUpper())
                                    {
                                        // case "10_MIN":
                                        // case "1_MIN":
                                        case "1_SEC":
                                            Working = true;
                                            EveningSessionStartWorkingPrefix();
                                            Evlm2(EvlResult.WARNING, EvlSubject.INIT,
                                            "Strategies", StrategyTickerString,
                                            $"{args.TimePlanItemCode}, {args.TimePlanItemEventCode}, Time: {args.Msg} , Working: {Working}",
                                            $"IsMorningSessionClose: {IsMorningSessionNeedClose}," +
                                            $" IsEveningSessionClose: {IsEveningSessionNeedClose}", "");
                                            break;
                                    }
                                    break;
                                case "TOEND":
                                    switch (args.Msg.TrimUpper())
                                    {
                                        //case "15_SEC": // Market Exit
                                        //    SetExitMode(11, args.ToString());
                                        //    break;
                                        //case "30_SEC": // Market Exit
                                        //    SetExitMode(11, args.ToString());
                                        //    break;
                                        //case "45_SEC": // Market Exit
                                        //    SetExitMode(11, args.ToString());
                                        //    break;
                                        //case "1_MIN": // Market Exit
                                        //    SetExitMode(11, args.ToString());
                                        //    break;
                                        //case "2_MIN": // Volatility/2 Exit
                                        //    //SetExitMode(11, args.ToString());
                                        //    SetExitMode(12, args.ToString());
                                        //    break;
                                        // case "15_MIN":
                                        case "10_MIN":
                                        //case "5_MIN": // Bid/Ask
                                            if (!IsEveningSessionNeedClose)
                                                break;
                                            if (!Working)
                                                break;
                                            EveningSessionFinishPostfix();
                                            SafeContractsToRest = SafeContractsOvn;
                                            SetExitMode(12, args.ToString());
                                            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                                                "Strategies", StrategyTickerString, $"ExitMode: {ExitMode}",
                                                $"Session: {args.TimePlanItemCode} Minutes: {args.Msg} {args.TimePlanItemEventCode}", "");
                                            break;
                                        //case "10_MIN": // Bid/Ask
                                        //    SetExitMode(12, args.ToString());
                                        //    break;
                                        //case "12_MIN": // Volatility Exit
                                        //    //SetExitMode(11, args.ToString());
                                        //    SetExitMode(13, args.ToString());
                                        //    break;
                                        //case "15_MIN": // Volatility Exit
                                        //    //SetExitMode(11, args.ToString());
                                        //    SetExitMode(14, args.ToString());
                                        //    break;
                                        case "50_MIN": // Bid/Ask
                                            //if (!IsEveningSessionClose)
                                            //    break;
                                            //SafeContractsToRest = SafeContractsOvn;
                                            //SetExitMode(12, args.ToString());
                                            //Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,
                                            //    "Strategies", StrategyTickerString, $"ExitMode: {ExitMode}", $"Session: {args.TimePlanItemCode} Minutes: {args.Msg} {args.TimePlanItemEventCode}", "");
                                            EveningSessionFinishPrefix();
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }

        protected bool MorningSessionStarting { get; set; }
        protected double MorningSessionStartWorkingTrendMaValue { get; set; }
        protected double EveningSessionClosingTrendMaValue { get; set; }
        public virtual void MorningSessionStartPrefix()
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";
            Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString,
                        methodname, "", "");

            SetLongShortEnabled(true);

            MorningSessionStarting = true;
            EveningSessionClosing = false;
        }
        public virtual void MorningSessionStartPostfix()
        {
            MorningSessionStarting = false;
        }
 
        public virtual void MorningSessionStartWorkingPrefix()
        {
            EveningSessionClosing = false;
            Working = true;
            MorningSessionStartWorkingTrendMaValue = XTrend.Ma;
        }
        public virtual void EveningSessionStartWorkingPrefix()
        {
            EveningSessionClosing = false;
            Working = true;
            SetLongShortEnabled(true);
        }
        public virtual void EveningSessionFinishPrefix()
        {
            
        }
        public virtual void EveningSessionFinishPostfix()
        {
        }

        protected override void PositionIsChangedEventHandler2(IPosition2 oldposition, IPosition2 newposition,
            PositionChangedEnum changedResult)
        {
            MorningSessionStartPostfix();
            // ClearBuySellRequests();

            LastTradeMaValue = XTrend.Ma;

            CalcLastTradeOperation(oldposition, newposition, changedResult);

            //if (oldposition.IsShort)
            //    TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
            //        OrderTypeEnum.All, OrderOperationEnum.Buy);
            //else if (oldposition.IsLong)
            //    TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
            //        OrderTypeEnum.All, OrderOperationEnum.Sell);

            //     TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.TRADING, StrategyTickerString, "PositionChanged",
            //                                  "New: " + newposition.StatusString + "; Old: " + oldposition.StatusString, "");
            _swingCountEntry = 0;
            _swingCountEntry2 = 0;
            _swingCountEntry3 = 0;
            _swingPrTrendCountEntry = 0;

            TrEntryEnabled.Reset();
            TrEntryEnabled2?.Reset();
            TrEntryEnabled3?.Reset();

            if (changedResult == PositionChangedEnum.Reversed || // reverse
                changedResult == PositionChangedEnum.Opened || // open new from flat
                changedResult == PositionChangedEnum.Registered
                //changedResult == PositionChangedEnum.Closed
                )
            {
                //  TrMaxContracts.Reset();
                TrEntryEnabled.Reset();
                IsPosAbsMaxWasReached.Reset();

                MaxContractsReached = false;

                _swingCountExit = 0;
                _swingCountReverse = 0;
                _swingCountEntry = 0;

                MyReverseSignal1 = ReverseSignal1;
                MyExitSignal1 = ExitSignal1;
                MyEntrySignal1 = EntrySignal1;

                PositionMin.Clear();
                PositionMax.Clear();
                PositionStop.Clear();

                TrailingTrHigh.Clear();
                TrailingTrLow.Clear();
            }
            if (changedResult == PositionChangedEnum.Closed)
            {

                _swingCountEntry = 0;

                LastTradeLongMaValue = 0d;
                LastTradeShortMaValue = 0d;

                MyReverseSignal1 = ReverseSignal1;
                MyExitSignal1 = ExitSignal1;
                MyEntrySignal1 = EntrySignal1;

                MaxContractsReached = false;
                TrMaxContracts.Reset();
                PositionStop.Clear();

                IsPosAbsMaxWasReached.Reset();

            }
            if (newposition.IsLong)
            {
                if (changedResult == PositionChangedEnum.ReSizedUp || changedResult == PositionChangedEnum.Opened)
                {
                    LastTradeLongMaValue = XTrend.Ma;
                    LastTradeShortMaValue = 0d;
                }
                if (changedResult == PositionChangedEnum.ReSizedDown )
                {
                    LastTradeLongMaValue = 0d;
                    LastTradeShortMaValue = 0d;
                }
            }
            if (newposition.IsShort)
            {
                if (changedResult == PositionChangedEnum.ReSizedUp || changedResult == PositionChangedEnum.Opened)
                {
                    LastTradeLongMaValue = 0d;
                    LastTradeShortMaValue = XTrend.Ma;
                }
                if (changedResult == PositionChangedEnum.ReSizedDown)
                {
                    LastTradeLongMaValue = 0d;
                    LastTradeShortMaValue = 0d;
                }
            }

            if (changedResult == PositionChangedEnum.ReSizedUp ||
                changedResult == PositionChangedEnum.ReSizedDown)
            {
                _swingCountReverse = 0;
                _swingCountEntry = 0;
            }
            if (newposition.IsOpened && oldposition.IsNeutral)
            {
                _swingCountEntry = 0;
                TrMaxContracts.Reset();
                TrEntryEnabled.Reset();
                Trend55Changed.Reset();
                IsPosAbsMaxWasReached.Reset();
            }

            SetStopOrderFilledStatus(false);

            ClearBuySellRequests();

            //PositionChanged(oldposition, newposition, changedResult);
        }
    }
}
