using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using GS.Serialization;
using GS.Trade.Data;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Studies.GS.xMa018;
using GS.Trade.Trades;
using GS.Trade.Trades.Orders3;

namespace GS.Trade.Strategies
{
    public class DefaultStrategy1 : Strategy
    {
        public override IBars Bars { get; protected set; }
        public override int MaxBarsBack { get; protected set; }

        //public override string Key
        //{
        //    get
        //    {
        //        return String.Format("[Type={0};Name={1};Code={2};TradeAccountKey={3};TickerKey={4};TimeInt={5}]",
        //              GetType(), Name, Code, RealAccountKey, RealTickerKey, TimeInt);
        //    }
        //}

        private ITimeSeries _xma018;
        public override void Main()
        {
            //throw new NotImplementedException();
        }
        public override void Init()
        {
            try
            {
                Evlm2(EvlResult.INFO, EvlSubject.DIAGNOSTIC, StrategyTimeIntTickerString, "Init()","","");

                TradeTerminal = TradeContext.RegisterTradeTerminal(TradeTerminalType, TradeTerminalKey);
                if(TradeTerminal == null)
                    throw new NullReferenceException("DefaultStrategy.Init(); TradeTerminal == null. " + TradeTerminalType + " " + TradeTerminalKey);

                EventHub = Builder.Build<EventHub>(@"Init\EventHubStrat.xml", "EventHub");
                EventHub.Parent = this;
                EventHub.Init(TradeContext.EventLog);
               
               // TradeTerminal = TradeContext.RegisterTradeTerminal(TradeTerminalType, TradeTerminalKey);
                TradeTerminal.TradeEntityChangedEvent += EventHub.EnQueue;
                // TradeTerminal.ChangedEvent += EventHub.EnQueue;
                // TradeTerminal.ExceptionEvent += EventHub.FireEvent;

                // EventHub.Subscribe(StrategyTimeIntTickerString, "ORDER.REGISTER", OnOrderRegister);
                EventHub.Subscribe(StrategyTimeIntTickerString, "ORDER.TRANSSEND", OnOrderTransactionSend);
                EventHub.Subscribe(StrategyTimeIntTickerString, "ORDER.TRANSREPLY", OnOrderTransactionReply);
                EventHub.Subscribe(StrategyTimeIntTickerString, "ORDER.STATUSCHANGED", OnOrderStatusChanged);
                EventHub.Subscribe(StrategyTimeIntTickerString, "TRADEREPLY", OnTradeReply);

                EventHub.Subscribe("TradeTerminal", "ConnectionStatusChanged", OnTradeTerminalConnectionStatusChanged);

                Position = TradeContext.RegisterPosition(this);
                if (Position == null)
                    throw new NullReferenceException("DefaultStrategy.Init(); Position is Null");
                if (Position.PositionTotal == null)
                    throw new NullReferenceException("DefaultStrategy.Init(); PositionTotal is Null");

                PositionTotal = Position.PositionTotal;
                Deals = TradeContext.BuildDeals();

                if (TimePlanKey.HasValue())
                {
                    TimePlan = TradeContext.RegisterTimePlanEventHandler(TimePlanKey, TimePlanEventHandler);
                    if (TimePlan == null)
                        throw new NullReferenceException("TimePlan is Empty");
                }

                //Orders = TradeTerminal.Type == Trade.TradeTerminalType.Simulator
                //    ? TradeContext.SimulateOrders
                //    : TradeContext.Orders;

                ActiveOrderCollection = new Orders3();
                EntryEnabled = true;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(StrategyTickerString, "DefaultStrategy", "Strategy.Init()", ToString(), e);
                IsWrong = true;
                // throw;
                //throw new NullReferenceException("Strategy.Init() Failure: " + StrategyTickerString);
            }
        }

        public override void CloseAll(int mode)
        {
            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, "Strategies", StrategyTickerString, "Close All: Close only Active Orders", Position !=null ? Position.PositionString3 : "", "");
            //KillAllOrders2();
        }
    }
}
