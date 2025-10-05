using System;
using System.Linq;
using GS.Contexts;
using GS.Interfaces;
using GS.Interfaces.Dde;
using GS.Serialization;
using GS.Trade.Data;
using GS.Trade.Data.Bars;

using GS.Trade.Storage2;
using GS.Trade.TimeSeries.Model;
using GS.Trade.TimeSeries.Repositories;

namespace GS.Trade.Bars
{
    public class BarCreatorContext2 : Context
    {
        protected Tickers3 TickersCore;

        protected IEntityRepository<ITickerDb, ITicker> TickerRepo;
        protected IEntityRepository<ITimeSeriesBase2, ITimeSeries2> BarSeriesRepo;
        //protected  IEntityRepository3<IBarDb, IBar> BarsRepo;
        protected IBarsRepository3 BarsRepo;

        protected BarRand BarRand1;
        protected BarRand BarRand2;

        protected IDde Dde2;
        public override void Init()
        {
            base.Init();

            Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TimeSeries", "Main", "Init()", "", "");

            BarSeriesRepo = Builder.Build2<IEntityRepository<ITimeSeriesBase2, ITimeSeries2>>(@"Init\TradeRepositories.xml", "BarSeriesRepository33");
            BarSeriesRepo.Init(EventLog);

            //BarsRepo = Builder.Build2<IEntityRepository3<IBarDb, IBar>>(@"Init\TradeRepositories.xml", "BarsRepository35");
            BarsRepo = Builder.Build2<IBarsRepository3>(@"Init\TradeRepositories.xml", "BarsRepository35");
            BarsRepo.Init(EventLog);

            TickerRepo = Builder.Build2<IEntityRepository<ITickerDb, ITicker>>(@"Init\TradeRepositories.xml", "TickerRepository32");
            TickerRepo.Init(EventLog);

            TickersCore = new Tickers3 { Parent = null, Code = "Tickers", Name = "Tickers" };
            TickersCore.Init(EventLog);

            //Dde2 = Builder.Build2<IDde>(@"Init\Dde.xml", "Dde2");
            //Dde2.Init(EventLog);
            ////Dde2.RegisterDefaultCallBack(TickersCore.PushDdeQuoteStr1);
            //Dde2.RegisterDefaultCallBack((s) =>
            //{
            //    var ea = new GS.Events.EventArgs
            //    {
            //        Category = "Quotes",
            //        Entity = "Quote",
            //        Operation = "Add",
            //        Object = s
            //    };
            //    EventHub.EnQueue(Dde2, ea);
            //});

            // EventHub Setting
            var eventHubTask = WorkTasks.GetByKey("EventHubReceiver");
            if (eventHubTask != null)
                eventHubTask.Works.Register(EventHub.Work);

            // Put Quotes from EHub to Ticker
            EventHub.Subscribe("Quotes", "Quote", TickersCore.EnQueue);

            // Main output from EventHub to Bars
            EventHub.Subscribe("Bars", "Bar", (sender, ea) =>
            {
                //ConsoleSync.WriteLineT("Tickers.Bar:" + ea.Object.ToString());

                var b = ea.Object as IBar;
                if (b != null)
                    //BarsRepo.Add(b);
                    BarsRepo.EnQueue(sender, ea);
            });

            var tickerRecTask = WorkTasks.GetByKey("TickersReceiver");
            if (tickerRecTask != null)
                tickerRecTask.Works.Register(TickersCore.Work);

            // Bars Generated to EventHub
            TickersCore.ChangedEvent += EventHub.EnQueue;

            // Tickers
            var db = new TimeSeriesContext();
            var dbtickers = db.Tickers.ToList();

            foreach (var t in dbtickers)
            {
                var ti = new GS.Trade.Data.Ticker
                {
                    Id = (int)t.Id,

                    Code = t.Code,
                    Name = t.Name,

                    ClassCode = t.TradeBoard.Code,
                    TradeBoard = t.TradeBoard.Code,
                };
                TickersCore.Register(ti);
                foreach (var s in t.TimeSeries)
                {
                    var bs = new Bars001(s.Name, ti, s.TimeInt.TimeInterval, s.TimeInt.TimeShift)
                    {
                        Id = s.Id,
                        Code = s.Code
                    };
                    ti.RegisterAsyncSeries(bs);
                }
            }

            // Universe
            //BarRand1 = null, barRand2 = null;
            #region BarRandom
            //var randomTicker = TickersCore.GetTicker("MARS");
            //if (randomTicker != null)
            //{
            //    BarRand1 = new BarRand(randomTicker.Code, randomTicker.Name, randomTicker.TradeBoard);
            //    BarRand1.Init(EventLog);
            //    // Tick to EventHub
            //    BarRand1.ChangedEvent += EventHub.EnQueue;
            //}
            //randomTicker = TickersCore.GetTicker("URANUS");
            //if (randomTicker != null)
            //{
            //    BarRand2 = new BarRand(randomTicker.Code, randomTicker.Name, randomTicker.TradeBoard);
            //    BarRand2.Init(EventLog);
            //    // Tick to EventHub
            //    BarRand2.ChangedEvent += EventHub.EnQueue;
            //}
            //var barRandomTask = WorkTasks.GetByKey("RandomTickPusher");
            //if (barRandomTask != null)
            //{
            //    if (BarRand1 != null)
            //        barRandomTask.Works.Register(BarRand1.Work);
            //    if (BarRand2 != null)
            //        barRandomTask.Works.Register(BarRand2.Work);
            //}
            #endregion
            var barsRepoTask = WorkTasks.GetByKey("BarsRepoTask");
            if (barsRepoTask != null)
                barsRepoTask.Works.Register(BarsRepo.Work);
            
        }

        public override void Start()
        {
            base.Start();
            //if (Dde2 != null)
            //    Dde2.Start();
        }
        public override void Stop()
        {
            base.Stop();
            //if (Dde2 != null)
            //    Dde2.Stop();
        }
        public override void DeQueueProcess()
        {
            // throw new NotImplementedException();
        }
    }
}
