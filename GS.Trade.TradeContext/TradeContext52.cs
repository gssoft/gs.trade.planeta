using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.Interfaces;
using GS.Serialization;
// using GS.Trade.Dde;
// using GS.Trade.Strategies.Portfolio;
using GS.Trade.TradeTerminals64.Simulate;
using GS.Trade.Windows;
using GS.Trade.Windows.Charts;

namespace GS.Trade.TradeContext
{
    public class TradeContext52 : TradeContext5
    {
         public TradeContext52()
        {
            Code = "TradeContext52";
            Name = Code;
            Alias = Code;
        }
        public override void Init()
        {
            try
            {
                base.Init();
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.Init()", ToString(), e);
                //e.Message, e.Source, e.GetType().ToString(), e.TargetSite.ToString());
                throw;
            }
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

            // 15.09.24
            // 

            PortfolioWindow = new PortfolioWindow();
            PortfolioWindow.Init(this, EventLog);

            //OrderPlaneWindow = new OrderPlaneWindow();
            //OrderPlaneWindow.Init(this);

            TradesWindow2.Init(this);
            OrdersActiveWindow2.Init(this, EventLog);
            OrdersFilledWindow2.Init(this);

            DealsWindow.Init(this, EventLog);
            PositionsWindow3.Init(this, EventLog);
            // PositionTotalsWindow2.Init(this, EventLog);
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

            PortfolioWindow.Show();

            Thread.Sleep(1000);

            Portfolios.Refresh();

            //ExceptionsWindow.Show();

            // TransactionsWindow.Show();

            //OrderPlaneWindow.Show();

            // QuotesWindow.Show();

            // ChartWindow.Show();
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
                       // throw;
                    }
                }

                Strategies.StrategyTradeEntityChangedEvent += EventHub.FireEvent;
                Strategies.ExceptionEvent += EventHub.FireEvent;

                // Portfolios = BuildPortfolios();
                // 15.09.24
                // Portfolio with TimeInts
                Portfolios = BuildPortfolios3();
                Portfolios.ChangedEvent += EventHub.FireEvent;

                TradeTerminals.ChangedEvent += EventHub.FireEvent;

                SimulateTerminal = TradeTerminals.GetSimulateTerminal();
             
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.StrategyInit()", ToString(), e);
                // throw new Exception(e.Message);
            }

            WindowsInit();
            // ShowWindows();
        }
        public override void Close()
        {
            try
            {
                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Close Process Started", "", "");

                Strategies.Close();
                
                TradeTerminals.ChangedEvent -= EventHub.FireEvent;

                Strategies.StrategyTradeEntityChangedEvent -= EventHub.FireEvent;
                Strategies.ExceptionEvent -= EventHub.FireEvent;

                EventLog.EventLogChangedEvent -= EventHub.FireEvent;

                // 15.11.11
                //Rand.NewTickEvent -= Orders.ExecuteTick;
                //Rand.NewTickStrEvent -= Tickers.PutDdeQuote3;

                Tickers.NewTickEvent -= Orders.ExecuteTick;
                Tickers.NewTickEvent -= SimulateOrders.ExecuteTick;

                TradeTerminals.ChangedEvent -= EventHub.FireEvent;

                CloseWindows();

                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Close Process Finished", "", "");
               
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.Close()", ToString(), e);
                throw;
            }
        }
        public override void Stop()
        {
            try
            {
                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Stop Process Started", "", "");

                // 15.11.11
                // Rand.Stop();

                TradeTerminals.DisConnect();             

                Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TradeContext", "TradeContext", "Stop Process Finished", "", "");
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "TradeContext.Stop()", ToString(), e);
                throw;
            }
        }
    }
}
