using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Contexts;
using GS.EventLog;
using GS.Events;
using GS.Interfaces;
using GS.Interfaces.Dde;
using GS.Serialization;
using GS.Trade;
using GS.Trade.Data;
using GS.Trade.Dde;
using GS.Trade.Storage2;
using GS.WorkTasks;

namespace BarCreator
{
    public class BarCreatorContext : Context
    {
        protected  Tickers3 TickersCore;

        protected  IEntityRepository<ITickerDb, ITicker> TickerRepo;
        protected  IEntityRepository<ITimeSeriesBase2, ITimeSeries2> BarSeriesRepo;
        protected  IEntityRepository<IBarDb, IBar> BarsRepo;

        protected IDde Dde2;

        private  BarRandom _barRandom1;
        private  BarRandom _barRandom2;
        public override void Init()
        {
            //base.Init();

            EventLog = Builder.Build<EventLogs>(@"Init\EventLog.xml", "EventLogs");
            EventLog.Init();

            WorkTasks = Builder.Build2<WorkTasks>(@"Init\WorkTasks.xml", "WorkTasks");
            WorkTasks.Init(EventLog);

            EventHub = Builder.Build<EventHub3>(@"Init\EventHub3.xml", "EventHub3");
            EventHub.Init(EventLog);

            Evlm1(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TimeSeries", "Main", "Init()", "", "");

            BarSeriesRepo = Builder.Build2<IEntityRepository<ITimeSeriesBase2, ITimeSeries2>>(@"Init\TradeRepositories.xml", "BarSeriesRepository33");
            BarSeriesRepo.Init(EventLog);

            BarsRepo = Builder.Build2<IEntityRepository<IBarDb, IBar>>(@"Init\TradeRepositories.xml", "BarsRepository33");
            BarsRepo.Init(EventLog);

            TickerRepo = Builder.Build2<IEntityRepository<ITickerDb, ITicker>>(@"Init\TradeRepositories.xml", "TickerRepository32");
            TickerRepo.Init(EventLog);

            TickersCore = new Tickers3 { Parent = null, Code = "Tickers", Name = "Tickers" };
            TickersCore.Init(EventLog);

            Dde2 = Builder.Build2<IDde>(@"Init\Dde.xml", "Dde2");
            Dde2.Init(EventLog);
        //    Dde2.RegisterDefaultCallBack(TickersCore.PushDdeQuoteStr1);

            EventHub.Subscribe("Quotes","Quote", (sender, ea) =>
            {
                TickersCore.EnQueue(sender ,ea);
            });

        }

        public override void DeQueueProcess()
        {
            throw new NotImplementedException();
        }
    }
}
