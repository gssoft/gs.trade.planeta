using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.EventLog;
using GS.Events;
using GS.Interfaces;
using GS.Interfaces.Dde;
using GS.Process;
using GS.Serialization;
using GS.Trade;
using GS.Trade.Data;
using GS.Trade.Data.Bars;
using GS.Trade.Dde;
using GS.Trade.Storage2;
using GS.Trade.TimeSeries.Model01;


namespace TimeSeriesCa01
{
    public class TickerKey
    {
        public string Key { get; set; }
        public List<Series> SeriesList { get; set; }

        public TickerKey()
        {
            SeriesList = new List<Series>();
        }
    }
    public class Series
    {
        public Series()
        {
        }
        public string Name { get; set; }
        public string Code { get; set; }
        public int TimeInt { get; set; }
        public int TimeShift { get; set; }
        public string SeriesKey {
            get { return Code; }
        }
    }

    class Program
    {
        protected static IEntityRepository<IBarDb, IBar> Bars;
        protected static IEventLog Evl;

        private static BarRandom _barRandom;
        private static ITickers _tickers;

        static void Main(string[] args)
        {
            var ini = new Initializer();
            Database.SetInitializer<TimeSeriesContext>(ini);

            Evl = Builder.Build<EventLogs>(@"Init\EventLog.xml", "EventLogs");
            Evl.Init();

            Evl.AddItem(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TimeSeries", "Main", "Init()","","");

            var tickerKeys = Builder.Build<List<TickerKey>>(@"Init\TimeSeries02.xml", "ArrayOfTickerKey");

            var  tickerRepo = Builder.Build2<IEntityRepository<ITickerDb, ITicker>>(@"Init\TradeRepositories.xml", "TickerRepository32");
            tickerRepo.Init(Evl);

            var barSeriesRepo = Builder.Build2<IEntityRepository<ITimeSeriesBase, ITimeSeries>>(@"Init\TradeRepositories.xml", "BarSeriesRepository33");
            barSeriesRepo.Init(Evl);

            Bars = Builder.Build2<IEntityRepository<IBarDb, IBar>>(@"Init\TradeRepositories.xml", "BarsRepository33");
            Bars.Init(Evl);

            _tickers = new Tickers { Parent = null, Code = "Tickers", Name = "Tickers" };
            _tickers.Init(Evl);

            //foreach (var t in tickerKeys.Select(tickerKey => tickerRepo.GetByKey(tickerKey.Key)))
            foreach(var tk in tickerKeys)
            {
                var t = tickerRepo.GetByKey(tk.Key);
                if (t == null)
                {
                    Evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "TimeSeries", "Ticker", tk.Key + " Is Not Found", "", "");
                    continue;
                }
                Evl.AddItem(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TimeSeries", "Ticker", t.ToString(), "", "");
                var ti = _tickers.CreateInstance(t);
                _tickers.Register(ti);
                foreach (var s in tk.SeriesList)
                {
                    var bs = new Bars001(s.Name, ti, s.TimeInt , s.TimeShift) {Code = s.Code};
                    barSeriesRepo.AddOrGet(bs);
                    ti.RegisterAsyncSeries(bs);
                }
            }
            var randomTicker = _tickers.GetTicker("UNIVERSE@MARS");

            var dde2 = Builder.Build2<IDde>(@"Init\Dde.xml", "Dde2");
            dde2.Init(Evl);
            dde2.RegisterDefaultCallBack(_tickers.PushDdeQuoteStr1);

            var strategyProcess = new ProcessManager2("Strategy_Process", 1000, 0, Evl);

            strategyProcess.RegisterProcess("Tickers.DdeQuotes DeQueue Process1", "Tickers.QuotesDdeQueueProcess1()", (int)(5 * 1000), 0,
                    null, _tickers.DeQueueDdeQuoteStrProcess1, null);

            _tickers.ChangedEvent += NewBarProcess;

            if (randomTicker != null)
            {
                _barRandom = new BarRandom(100000, 1000)
                {
                    Name = randomTicker.Name,
                    Code = randomTicker.Code,
                    TradeBoard = randomTicker.TradeBoard,
                    DT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                                      DateTime.Now.Hour, DateTime.Now.Minute, 0, 0)
                    //DT = DateTime.Now
                };

                RandomTickerProcess();

                strategyProcess.RegisterProcess("Random.TickerProcess", "Random.TickerProcess", (int)(1 * 1000), 0,
                    null, RandomTickerProcess, null);
               
            }

            strategyProcess.Start();
            dde2.Start();

            Console.ReadLine();

            dde2.Stop();
            strategyProcess.Stop();

        }

        private static void RandomTickerProcess()
        {
            var tick = _barRandom.GetNextTick2();
            // t.DT = t.DT.AddSeconds(-1);
            //_tradeContext.ExecuteTick(t.DT, t.Ticker, t.Value, t.Bid, t.Ask);
            _tickers.PutDdeQuote3(tick.ToString());
            //Evl.AddItem(EvlResult.SUCCESS,EvlSubject.TECHNOLOGY, "RandomTickerProces", "Tick", "New", tick.ToString(),"");
        }

        private static void NewBarProcess(object sender, GS.Events.IEventArgs args)
        {
            Evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "NewBar", "NewBar", "NewBarEventHandler",
                                            args.Object.ToString(),"");
            // Console.ReadLine();
            var b = args.Object as IBar;
            if (b != null)
                Bars.Add(b);
        }
    }
}
