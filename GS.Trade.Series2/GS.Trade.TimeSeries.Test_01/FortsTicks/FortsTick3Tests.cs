using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.BatchWorks;
using GS.Extension;
using GS.Interfaces;
using GS.Serialization;
using GS.Trade.TimeSeries.FortsTicks3.Model;
using GS.Trade.TimeSeries.FortsTicks3.Dal;
using NUnit.Framework;

namespace GS.Trade.TimeSeries.Test_01.FortsTicks
{
    public class FortsTick3Tests
    {
        [OneTimeSetUp]
        public void CreateDataBaseTest()
        {
            //SourcePath = @"F:\Forts\2016\Txt\";
            //SourceFileFilter = @"FT*.CSV";

            //var appDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../App_Data");
            //AppDomain.CurrentDomain.SetData("DataDirectory", appDataDir);

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var appDataDirectory = Path.Combine(baseDirectory.Replace("\\bin\\Debug", ""), "");
            AppDomain.CurrentDomain.SetData("DataDirectory", appDataDirectory);

            Database.SetInitializer(new TimeSeries.FortsTicks3.Init.Initializer());
        }

        [Test]
        public void GetTickers()
        {
            using (var db = new FortsTicksContext3())
            {
                var ts = db.Tickers.ToList();
                foreach (var t in ts)
                    Console.WriteLine("Ticker: {0}", t);
            }
        }
        [Test]
        public void GetTicksDaily()
        {
            using (var db = new FortsTicksContext3())
            {
                var sw = new Stopwatch();
                var swtotal = new Stopwatch();
                var tms = db.TickSeriesSet.Where(ts=>ts.Ticker.Code.StartsWith("Si")).ToList();

                swtotal.Start();
                foreach (var t in tms.Take(100))
                {
                    for (DateTime d = t.FirstDT.Date; d <= t.LastDT.Date; d = d.Inc())
                    {
                        sw.Start();
                        var ticks = db.GetTicksDaily(t.Ticker.Contract, d.Date).ToList();
                        sw.Stop();
                        var ts = sw.Elapsed;
                        var elapsed = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";
                        Console.WriteLine($"Ticker: {t.Ticker.Contract}, Date: {d.ToString("d")}, Count: {ticks.LongCount()}, Elapsed: {elapsed} ");
                        sw.Reset();
                    }
                }
                swtotal.Stop();
                {
                    var ts = swtotal.Elapsed;
                    var elapsed = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";
                    Console.WriteLine($"Total Elapsed: {elapsed}");
                }
            }
        }
        
        [Test]
        public void AddTicks_Test()
        {
            using (var db = new FortsTicksContext3())
            {
                db.AddTicks2(@"F:\Forts\2016\Txt\FT\FT160826.F.CSV");
            }
        }
        [Test]
        public void AddBars_Test()
        {
            using (var db = new FortsTicksContext3())
            {
                db.AddBarsFromTicks();
            }
        }
        [Test]
        public void UpdateTickSeriesStat_Test()
        {
            using (var db = new FortsTicksContext3())
            {
                db.UpdateTickSeriesStat();
            }
        }
        [Test]
        public void UpdateBarSeriesStat_Test()
        {
            using (var db = new FortsTicksContext3())
            {
                db.UpdateBarSeriesStat();
            }
        }
        [Test]
        public void UpdateTickStatDaily_Test()
        {
            using (var db = new FortsTicksContext3())
            {
                db.UpdateTicksStatDaily();
            }
        }
        [Test]
        public void UpdateTickStatAll_Test()
        {
            using (var db = new FortsTicksContext3())
            {
                db.UpdateTicksStatAll();
            }
        }

        [Test]
        public void AddTicks_BatchWork_Test()
        {
            var evl = Builder.Build2<IEventLog>(
                AppDomain.CurrentDomain.BaseDirectory + @"\Init\EventLog.xml", "EventLogs");
            evl.Init();

            var ws = new BatchWork();
            ws.Init(evl);

            ws.Init(AppDomain.CurrentDomain.BaseDirectory + @"\Init\Forts_2016.xml");

            ws.DoWorks();
        }

        [Test]
        public void RemoveEmptyTickSeries_Test()
        {
            using (var db = new FortsTicksContext3())
            {
                db.RemoveEmptyTickSeries();
            }
        }
        [Test]
        public void VerifyTickSeries_Test()
        {
            using (var db = new FortsTicksContext3())
            {
                db.VerifySeries();
            }
        }
    }
}
