using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.EventLog;
using GS.Interfaces;
using GS.Serialization;
using GS.Trade;
using GS.Trade.Data;
using GS.Trade.Storage2;
using GS.Trade.TimeSeries.Model;

namespace CaBarCreator
{
    public class BarCreator
    {
        protected static IEventLog Evl;

        protected static ITickers Tickers;

        protected static IEntityRepository<IBarDb, IBar> Bars;
        protected IEntityRepository<ITimeSeriesBase2, ITimeSeries2> BarSeriesRepo;
        protected IEntityRepository<ITickerDb, ITicker> TickerRepo;

        private static BarRandom _barRandom;

        protected TimeSeriesContext TimeSeriesDb;

        public BarCreator()
        {
            TimeSeriesDb = new TimeSeriesContext();

            Evl = Evl = Builder.Build<EventLogs>(@"Init\EventLog.xml", "EventLogs");
            Evl.Init();

            Evl.AddItem(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TimeSeries", "Main", "Init()", "", "");

            BarSeriesRepo = Builder.Build2<IEntityRepository<ITimeSeriesBase2, ITimeSeries2>>(@"Init\TradeRepositories.xml", "BarSeriesRepository33");
            BarSeriesRepo.Init(Evl);

            Bars = Builder.Build2<IEntityRepository<IBarDb, IBar>>(@"Init\TradeRepositories.xml", "BarsRepository33");
            Bars.Init(Evl);

            TickerRepo = Builder.Build2<IEntityRepository<ITickerDb, ITicker>>(@"Init\TradeRepositories.xml", "TickerRepository32");
            TickerRepo.Init(Evl);
        }
    }
}
