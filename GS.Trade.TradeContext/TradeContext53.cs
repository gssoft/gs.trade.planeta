using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Events;
using GS.Serialization;
using GS.Trade.TradeTerminals64;
//using GS.Trade.Dde;
using GS.Trade.TradeTerminals64.Simulate;
using GS.Trade.Windows;
using GS.Trade.Windows.Charts;

namespace GS.Trade.TradeContext
{
    public class TradeContext53 : TradeContext51
    {
        //[XmlIgnore]
        //public SimulateTerminal SimulateTerminal;
        public override void Init()
        {
            try
            {
                //((IEventLogs)EventLog).SetMode(EvlModeEnum.Init);

                //EventHub = Builder.Build<EventHub>(@"Init\EventHub.xml", "EventHub");
                //EventHub.Init(EventLog);

                EventHub = Builder.Build<EventHub>(@"Init\EventHub.xml", "EventHub");
                //EventHub.Init(EventLog);
                //EventHub.ExceptionEvent += ExceptionRegister;

                this.ExceptionEvent += EventHub.FireEvent;
                EventLog.ExceptionEvent += EventHub.FireEvent;

                ExceptionsWindow = new ExceptionsWindow();
                ExceptionsWindow.Init(this);
                ExceptionsWindow.Show();

                EventLogWindow = new EventLogWindow3();
                EventLogWindow.Init(EventLog);
                ShowEventLogWindow();

                EventHub.Init(EventLog);
                EventHub.ExceptionEvent += ExceptionRegister;

                TradeStorage = Builder.Build2<ITradeStorage>(@"Init\TradeStorages.xml", "TradeStorage");
                TradeStorage.ExceptionEvent += EventHub.FireEvent;
                TradeStorage.ChangedEvent += EventHub.FireEvent;
                TradeStorage.Init(EventLog);

                EventHub.Subscribe("UI.EXCEPTIONS", "EXCEPTION", ExceptionRegister);

                TradeTerminals = new TradeTerminals64.TradeTerminals
                {
                    Name = "Trade Terminals",
                    Code = "TradeTerminals",
                    //Parent = this,
                    EventLog = EventLog
                };

                Positions.Init(EventLog);
                try
                {
                    TimePlans.Init(EventLog);
                }
                catch (Exception e)
                {
                    SendExceptionMessage3(FullName, "TradeContext", " TimePlan.Init()", "", e);
                    throw;
                }
                Orders.Init(EventLog, Positions, Trades);
                Trades.Init(EventLog, Positions, Orders);

                SimulateOrders.Init(EventLog, Positions, Trades);

                // throw new NullReferenceException("Test Exceptions");
                
                Accounts.Load();
                TickersAll.Init(EventLog);
                TickersAll.Load();

                // Need to ReCreate DataBase
                if (NeedToReCreateDb)
                {
                    foreach (var a in Accounts.GetAccounts)
                    {
                        try
                        {
                            TradeStorage.Register(a);
                        }
                        catch (Exception e)
                        {
                            SendExceptionMessage3("TradeContex", a.GetType().ToString(),
                                "Account.Register:" + a.Key, a.ToString(), e);
                            throw;
                        }
                    }
                    foreach (var t in TickersAll.GetTickers)
                    {
                        try
                        {
                            TradeStorage.Register(t);
                        }
                        catch (Exception e)
                        {
                            SendExceptionMessage3("TradeContex",
                                t.GetType().ToString(), "Ticker.Register:" + t.Code, t.ToString(), e);
                            throw;
                        }
                    }
                }

                Tickers.Init(EventLog);
                Tickers.Parent = this;

                Orders.BackOrderExecMode = BackTestOrderExecutionMode.Optimistic;
                SimulateOrders.BackOrderExecMode = BackTestOrderExecutionMode.Optimistic;

                ActiveOrders.Init(EventLog);

            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.Init()", ToString(), e);
                //e.Message, e.Source, e.GetType().ToString(), e.TargetSite.ToString());
                throw;
            }
        }

        public override void Open()
        {
            Orders.BackOrderExecMode = BackTestOrderExecutionMode.Optimistic;
            TimePlans.Start();

            try
            {
                //Strategies = new Strategies.Strategies("MyStrategies", "xStyle", this);
                Strategies = Builder.Build2<IStrategies>(@"Init\Strategies.xml", "Strategies");
                if (Strategies == null)
                    return;
                Strategies.Parent = this;
                Strategies.TradeContext = this;
                Strategies.Init();

                foreach (var s in Strategies.StrategyCollection)
                {
                    try
                    {
                        TradeStorage.Register(s);
                        TradeStorage.Register(s.Position);
                        TradeStorage.RegisterTotal(s.PositionTotal);
                    }
                    catch (Exception e)
                    {
                        SendExceptionMessage3(FullName,"TradeContex", "Strategy.Register:" + s.StrategyTickerString, "", e);
                        throw;
                    }
                }

                Strategies.StrategyTradeEntityChangedEvent += EventHub.FireEvent;
                Strategies.ExceptionEvent += EventHub.FireEvent;

                TradeTerminals.ChangedEvent += EventHub.FireEvent;

                SimulateTerminal = TradeTerminals.GetSimulateTerminal();

                //foreach (var t in TradeTerminals.TradeTerminalCollection.Values)
                //{
                //    var sim = t as SimulateTerminal;
                //    if (sim == null)
                //        continue;
                //    SimulateTerminal = sim;

                //    //Rand.NewTickEvent += SimulateTerminal.ExecuteTick;
                //    //sim.TradeEntityChangedEvent += EventHub.FireEvent;
                //    break;
                //}

                // Tickers.LoadBarsFromArch();
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.StrategyInit()", ToString(), e);
                throw new Exception(e.Message);
            }

            WindowsInit();
           // ShowWindows();
        }

        protected override void WindowsInit()
        {
            #region Windows Init

            TradesWindow2 = new TradesWindow2();
            OrdersActiveWindow2 = new OrdersActiveWindow2();
            OrdersFilledWindow2 = new OrdersFilledWindow2();

            DealsWindow = new DealsWindow();
            PositionsWindow3 = new PositionsWindow3();
            PositionTotalsWindow3 = new PositionTotalsWindow3();
            PositionTotalsWindow4 = new PositionTotalsWindow4();

            //PortfolioWindow = new PortfolioWindow();

            //OrderPlaneWindow = new OrderPlaneWindow();
            //OrderPlaneWindow.Init(this);

            TradesWindow2.Init(this);
            OrdersActiveWindow2.Init(this, EventLog);
            OrdersFilledWindow2.Init(this);

            DealsWindow.Init(this, EventLog);
            PositionsWindow3.Init(this, EventLog);
            PositionTotalsWindow3.Init(this, EventLog);
            PositionTotalsWindow4.Init(this, EventLog);

            //PortfolioWindow.Init(this, EventLog);

            //TransactionsWindow.Init(EventLog, this);

            ChartWindow = new ChartWindow();
            ChartWindow.Init(this);

            #endregion
        }
        public override void ShowWindows()
        {
           
            TradesWindow2.Show();

           
            OrdersActiveWindow2.Show();
            OrdersFilledWindow2.Show();

            DealsWindow.Show();
            PositionsWindow3.Show();
            PositionTotalsWindow3?.Show();
            PositionTotalsWindow4?.Show();

            // PortfolioWindow.Show();

            //ExceptionsWindow.Show();

            // TransactionsWindow.Show();

            //OrderPlaneWindow.Show();

            // QuotesWindow.Show();

            // ChartWindow.Show();
        }
    }
}
