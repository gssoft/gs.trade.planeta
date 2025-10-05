using System;
using System.Collections.Generic;

using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.EventLog;
using GS.Interfaces;
using GS.Process;
using GS.Serialization;
using GS.Trade;

using GS.Trade.Storage2;
using GS.Trade.TimeSeries.Model;
using GS.Works;
using GS.WorkTasks;

//namespace CaBarCreator
//{
//    class Program
//    {
//        protected static ITickers TickersCore;

//        protected static IEntityRepository<ITickerDb, ITicker> TickerRepo;
//        protected static IEntityRepository<ITimeSeriesBase2, ITimeSeries2> BarSeriesRepo;
//        protected static IEntityRepository<IBarDb, IBar> BarsRepo;
//        protected static IEventLog Evl;

//        private static BarRandom _barRandom1;
//        private static BarRandom _barRandom2;

//        protected static Receiver<string> TickerReceiver;

//        static void Main2(string[] args)
//        {
//            //var ini = new Initializer();
//            //Database.SetInitializer<TimeSeriesContext>(ini);

//            Evl = Builder.Build<EventLogs>(@"Init\EventLog.xml", "EventLogs");
//            Evl.Init();

//            Evl.AddItem(EvlResult.INFO, EvlSubject.TECHNOLOGY, "TimeSeries", "Main", "Init()", "", "");

//            BarSeriesRepo = Builder.Build2<IEntityRepository<ITimeSeriesBase2, ITimeSeries2>>(@"Init\TradeRepositories.xml", "BarSeriesRepository33");
//            BarSeriesRepo.Init(Evl);

//            BarsRepo = Builder.Build2<IEntityRepository<IBarDb, IBar>>(@"Init\TradeRepositories.xml", "BarsRepository33");
//            BarsRepo.Init(Evl);

//            TickerRepo = Builder.Build2<IEntityRepository<ITickerDb, ITicker>>(@"Init\TradeRepositories.xml", "TickerRepository32");
//            TickerRepo.Init(Evl);

//            TickersCore = new Tickers { Parent = null, Code = "Tickers", Name = "Tickers" };
//            TickersCore.Init(Evl);

//            var db = new TimeSeriesContext();
//            var dbtickers = db.Tickers.ToList();

//            foreach (var t in dbtickers)
//            {
//                var ti = new GS.Trade.Data.Ticker
//                {
//                    Id = (int)t.Id,

//                    Code = t.Code,
//                    Name = t.Name,

//                    ClassCode = t.TradeBoard.Code,
//                    TradeBoard = t.TradeBoard.Code,
//                };
//                TickersCore.Register(ti);
//                foreach (var s in t.TimeSeries)
//                {
//                    var bs = new Bars001(s.Name, ti, s.TimeInt.TimeInterval, s.TimeInt.TimeShift)
//                    {
//                        Id = s.Id,
//                        Code = s.Code
//                    };
//                    ti.RegisterAsyncSeries(bs);
//                }
//            }


//            var dde2 = Builder.Build2<IDde>(@"Init\Dde.xml", "Dde2");
//            dde2.Init(Evl);
//            dde2.RegisterDefaultCallBack(TickersCore.PushDdeQuoteStr1);

//            var strategyProcess = new ProcessManager2("Strategy_Process", 1000, 0, Evl);

//            strategyProcess.RegisterProcess("Tickers.DdeQuotes DeQueue Process1", "Tickers.QuotesDdeQueueProcess1()",
//                    (int)(5 * 1000), 0, null, TickersCore.DeQueueDdeQuoteStrProcess1, null);

//            TickersCore.ChangedEvent += NewBarProcess;

//            var randomTicker = TickersCore.GetTicker("MARS");
//            if (randomTicker != null)
//            {
//                _barRandom1 = new BarRandom(100000, 1000)
//                {
//                    Name = randomTicker.Name,
//                    Code = randomTicker.Code,
//                    TradeBoard = randomTicker.TradeBoard,
//                    DT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
//                                      DateTime.Now.Hour, DateTime.Now.Minute, 0, 0)
//                    //DT = DateTime.Now
//                };

//                //RandomTickerProcess();
//            }
//            randomTicker = TickersCore.GetTicker("URANUS");
//            if (randomTicker != null)
//            {
//                _barRandom2 = new BarRandom(50000, 1000)
//                {
//                    Name = randomTicker.Name,
//                    Code = randomTicker.Code,
//                    TradeBoard = randomTicker.TradeBoard,
//                    DT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
//                                      DateTime.Now.Hour, DateTime.Now.Minute, 0, 0)
//                    //DT = DateTime.Now
//                };

//                //RandomTickerProcess();
//            }

//            //strategyProcess.RegisterProcess("Random.TickerProcess", "Random.TickerProcess", (int)(1 * 1000), 0,
//            //       null, RandomTickerProcess, null);

//            TickerReceiver = new Receiver<string>
//            {
//                Code = "Receiver",
//                Description = @"Work that's I work",
//                PushItem = (s) =>
//                {
//                    ConsoleSync.WriteLineT("RECEIVED >> : {0} {1}", s, "Programm");
//                    TickersCore.PutDdeQuote3(s);
//                },
//            };
//            var barRandomWork = new Work1<string>
//            {
//                Parent = null,
//                Code = "BarRandomTickerGenerator",
//                Description = @"Work that's I work",
//                ErrorCountToStop = 3,
//                //MainFunc = () => r.ReadFromQueue(),
//                InitFunc = () =>
//                {
//                    ConsoleSync.WriteLineT("Work: {0}. Work: {1} Init Complete"
//                        , "BarRandomTickerGenerator", "BarRandomTickerGenerator");
//                    return true;
//                },
//                MainFunc = () =>
//                {
//                    RandomTickerProcess();
//                    return true;
//                },
//                FinishFunc = () =>
//                {
//                    ConsoleSync.WriteLineT("Worker: {0}. Work: {1} Finish Complete",
//                        "BarRandomTickerGenerator",
//                        "BarRandomTickerGenerator");
//                    return true;
//                },
//            };
//            barRandomWork.ExceptionEvent += (s, ea) =>
//            {
//                ConsoleAsync.WriteLine("************* {0} *************\r\n Worker: {0} Catch Exception: {1}",
//                    "RandomTickerProcess", ea.ToString());
//                // Console.ReadLine();
//            };
//            var works = new Works();
//            works.Register(barRandomWork);
//            works.Register(TickerReceiver.Work);
//            var taskTickerReceiver = new WorkTask3
//            {
//                Code = "WorkTask TickerReciever",
//                //Work = work,
//                //Works = TickerReceiver.Work,
//                TimeInterval = 15,
//                ErrorCountToStop = 1,
//                IsEnabled = true
//            };
//            TickerReceiver.Work.WorkTask = taskTickerReceiver;
//            TickerReceiver.ExceptionEvent += (s, ea) =>
//            {
//                ConsoleAsync.WriteLine("************* {0} *************\r\n Worker: {0} Catch Exception: {1}",
//                    TickerReceiver.Code, ea.ToString());
//                // Console.ReadLine();
//            };
//            var taskBarRandom = new WorkTask3
//            {
//                Code = "WorkTask BarRandomRandom",
//                //Work = work,
//                //Work = barRandomWork,
//                TimeInterval = 1,
//                ErrorCountToStop = 1,
//                IsEnabled = true
//            };
//            barRandomWork.WorkTask = taskBarRandom;

//            taskTickerReceiver.Start();
//            taskBarRandom.Start();

//            //strategyProcess.Start();
//            dde2.Start();

//            ConsoleSync.WriteReadLine("Prees any key to Stop...");

//            taskTickerReceiver.Stop();
//            taskBarRandom.Stop();

//            dde2.Stop();
//            //strategyProcess.Stop();
//            ConsoleSync.WriteReadLine("Prees any key ...");
//        }

//        private static void RandomTickerProcess()
//        {
//            if (_barRandom1 != null)
//            {
//                var tick = _barRandom1.GetNextTick2();
//                // t.DT = t.DT.AddSeconds(-1);
//                //_tradeContext.ExecuteTick(t.DT, t.Ticker, t.Value, t.Bid, t.Ask);

//                // TickersCore.PutDdeQuote3(tick.ToString());
//                ConsoleSync.WriteLineT("SENDED >> : {0} {1}", "BarRandom", tick.ToString());
//                if (TickerReceiver != null)
//                    TickerReceiver.EnQueue(tick.ToString());
//            }
//            //Evl.AddItem(EvlResult.SUCCESS,EvlSubject.TECHNOLOGY, "RandomTickerProces", "Tick", "New", tick.ToString(),"");
//            if (_barRandom2 != null)
//            {
//                var tick = _barRandom2.GetNextTick2();
//                //TickersCore.PutDdeQuote3(tick.ToString());
//                ConsoleSync.WriteLineT("SENDED >> : {0} {1}", "BarRandom", tick.ToString());
//                if (TickerReceiver != null)
//                    TickerReceiver.EnQueue(tick.ToString());
//            }
//        }

//        private const int ViewCntInit = 60;
//        private static int _viewCnt = ViewCntInit;
//        private static void NewBarProcess(object sender, GS.Events.IEventArgs args)
//        {
//            if (--_viewCnt <= 0)
//            {
//                Evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "NewBar", "NewBar", "NewBarEventHandler",
//                    args.Object.ToString(), "");
//                _viewCnt = ViewCntInit;
//            }
//            // Console.ReadLine();
//            var b = args.Object as IBar;
//            if (b != null)
//                BarsRepo.Add(b);
//        }
//    }

//}
