using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Extension;
using GS.Trade.Dto;
using GS.Trade.TimeSeries.FortsTicks.Dal;
using GS.Trade.TimeSeries.FortsTicks.Model;
using GS.Trade.TimeSeries.General;
using GS.Trade.TimeSeries.Model;
using NUnit.Framework;
using Bar = GS.Trade.TimeSeries.FortsTicks.Model.Bar;
using Tick = GS.Trade.TimeSeries.FortsTicks.Model.Tick;
using Ticker = GS.Trade.TimeSeries.FortsTicks.Model.Ticker;

namespace GS.Trade.TimeSeries.Test_01.FortsTicks
{
    [TestFixture]
    public class FortsTicksTest
    {
        public string SourceFileFilter { get; set; }
        public string SourcePath { get; set; }
        [OneTimeSetUp]
        public void CreateDataBaseTest()
        {
            SourcePath = @"F:\Forts\2016\Txt\";
            SourceFileFilter = @"FT*.CSV";

            //var appDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../App_Data");
            //AppDomain.CurrentDomain.SetData("DataDirectory", appDataDir);

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var appDataDirectory = Path.Combine(baseDirectory.Replace("\\bin\\Debug", ""), "App_Data");
            AppDomain.CurrentDomain.SetData("DataDirectory", appDataDirectory);

            //Database.SetInitializer(new TimeSeries.FortsTicks.Init.Initializer());

            //using (var db = new FortsTicksContext())
            //{
            //    if (db.Tickers.Count() != 0)
            //        return;

            //    Ticker t;
            //    t = new Ticker
            //    {
            //        Code = "RIH6",
            //        Contract = "RTS-3.16"
            //    };
            //    db.Tickers.Add(t);
            //    t = new Ticker
            //    {
            //        Code = "SiH6",
            //        Contract = "Si-3.16"
            //    };
            //    db.Tickers.Add(t);
            //    t = new Ticker
            //    {
            //        Code = "SRH6",
            //        Contract = "SBRF-3.16"
            //    };
            //    db.Tickers.Add(t);

            //    db.SaveChanges();

            //}

            //var db = new FortsTicksContext();
            //var t = new Tick
            //{
            //    TradeID = 1,
            //    Code = "GSH6",
            //    Contract = "GS-3.16",
            //    DateTime = DateTime.Now,
            //    Amount = 5,
            //    Price = 77777.77777
            //};
            //db.Ticks.Add(t);
            //var saveChangesAsync = db.SaveChanges();
            //Assert.AreEqual(1, saveChangesAsync);
        }

        [Test]
        public void GetTickers()
        {
            using (var db = new FortsTicksContext())
            {
                var ts = db.Tickers.ToList();
                foreach(var t in ts)
                    Console.WriteLine("Ticker: {0} {1} {2}", t.ID, t.Code, t.Contract);
            }
        }

        [Test]
        public void AddTicks()
        {
            var dir = new DirectoryInfo(SourcePath);
            var files = dir.GetFiles(SourceFileFilter);

            using (var context = new FortsTicksContext())
            {
                var tickers = context.Tickers.ToList();
                Assert.AreEqual(3, tickers.Count());

                var filesInDb = context.GetFiles();
                var filesToProcess = files.Select(f => f.FullName).Except(filesInDb).ToList();

                foreach (var f in filesToProcess)
                {
                    var path = Path.Combine(SourcePath, f);
                    context.AddTicks(path);
                }
                //context.AddTicks(@"F:\Forts\2016\Txt\FT160104.F.CSV");
                //context.AddTicks(@"F:\Forts\2016\Txt\FT160105.F.CSV");
                //context.AddTicks(@"F:\Forts\2016\Txt\FT160106.F.CSV");
            }
        }

        [Test]
        public void GetTicks()
        {
            using (var db = new FortsTicksContext())
            {
                var cnt = 0;
                foreach (var t in db.Tickers.ToList())
                {
                    Ticker t1 = t;
                    var ts = db.Ticks.Where(tt => tt.TickerID == t1.ID).AsNoTracking().ToList();
                    Assert.IsNotEmpty(ts);
                    cnt += ts.Count;
                    Console.WriteLine("Ticker: {0} {1} {2}", t1.Code, t1.Contract, ts.Count );
                }
                var tss = db.Ticks.AsNoTracking().ToList();
                Assert.AreEqual(cnt, tss.Count);
                Console.WriteLine("Count: {0}", tss.Count);

                //var ts2 = db.Ticks.Where(t => t.TickerID == 2).AsNoTracking().ToList();
                //var ts3 = db.Ticks.Where(t => t.TickerID == 3).AsNoTracking().ToList();

                //Assert.IsNotEmpty(ts1);
                //Assert.IsNotEmpty(ts2);
                //Assert.IsNotEmpty(ts3);

            }
        }
        [Test]
        public void UpdateBarStatAll()
        {
            using (var db = new FortsTicksContext())
            {
                var ts = (from t in db.Bars
                    .Select(t => new { t.Ticker, t.DT, t.High, t.Low })
                          group t by new
                          {
                              t.Ticker
                          }
                              into g
                              select new // TickStat
                              {
                                  TickerID = g.Key.Ticker.ID,
                                  FirstDT = g.Min(t => t.DT),
                                  LastDT = g.Max(t => t.DT),
                                  MaxValue = g.Max(t => t.High),
                                  MinValue = g.Min(t => t.Low),
                                  Count = g.Count()
                              }).ToList();

                var tss = ts.Select(t => new Stat()
                {
                    TickerID = t.TickerID,
                    Type = TickBarTypeEnum.Bars,
                    Period = TimeSeriesStatEnum.All,
                    LastDate = t.LastDT.DateToInt(),
                    LastDT = t.LastDT,
                    FirstDT = t.FirstDT,
                    MaxValue = t.MaxValue,
                    MinValue = t.MinValue,
                    Count = t.Count
                }).ToList();

                foreach (var t in tss)
                {
                    var s = db.Stats.FirstOrDefault(
                        i => i.TickerID == t.TickerID &&
                             i.Type == t.Type &&
                             i.Period == TimeSeriesStatEnum.All);
                    if (s == null)
                        db.Stats.Add(t);
                    else
                    {
                        s.Count = t.Count;

                        s.LastDT = t.LastDT;
                        s.FirstDT = t.FirstDT;

                        s.MaxValue = t.MaxValue;
                        s.MinValue = t.MinValue;

                        s.ModifiedDT = DateTime.Now;
                    }
                    db.SaveChanges();
                }

            }
        }
        [Test]
        public void UpdateBarStatDaily()
        {
            using (var db = new FortsTicksContext())
            {
                var ts = (from t in db.Bars
                    .Select(t => new { t.Ticker, t.DT, t.High, t.Low })
                          group t by new
                          {
                              t.Ticker,
                              dt = DbFunctions.TruncateTime(t.DT)
                          }
                              into g
                              select new // TickStat
                              {
                                  TickerID = g.Key.Ticker.ID,
                                  LastDate = g.Key.dt,
                                  FirstDT = g.Min(t => t.DT),
                                  LastDT = g.Max(t => t.DT),
                                  MaxValue = g.Max(t => t.High),
                                  MinValue = g.Min(t => t.Low),
                                  Count = g.Count()
                              }).ToList();

                var tss = ts.Select(t => new Stat()
                {
                    TickerID = t.TickerID,
                    Type = TickBarTypeEnum.Bars,
                    Period = TimeSeriesStatEnum.Daily,
                    LastDate = t.LastDT.DateToInt(),
                    LastDT = t.LastDT,
                    FirstDT = t.FirstDT,
                    MaxValue = t.MaxValue,
                    MinValue = t.MinValue,
                    Count = t.Count
                }).ToList();

                foreach (var t in tss)
                {
                    Stat t1 = t;
                    var s = db.Stats.FirstOrDefault(
                            i => i.TickerID == t1.TickerID &&
                            i.Type == t1.Type &&
                            i.Period == TimeSeriesStatEnum.Daily &&
                            i.LastDate == t1.LastDate);
                    if (s == null)
                        db.Stats.Add(t);
                    else
                    {
                        s.Count = t1.Count;

                        s.LastDT = t1.LastDT;
                        s.FirstDT = t1.FirstDT;

                        s.MaxValue = t1.MaxValue;
                        s.MinValue = t1.MinValue;

                        s.ModifiedDT = DateTime.Now;
                    }
                }

                // db.TickStats.AddRange(tss);
                db.SaveChanges();
            }
        }
        [Test]
        public void UpdateTickStatAll()
        {
            using (var db = new FortsTicksContext())
            {
                var ts = (from t in db.Ticks
                    .Select(t => new {t.Ticker, t.DT, t.Price})
                    group t by new
                    {
                        t.Ticker
                    }
                    into g
                    select new // TickStat
                    {
                        TickerID = g.Key.Ticker.ID,
                        // Period = TimeSeriesStatEnum.All,
                        FirstDate = g.Min(t => t.DT),
                        LastDate = g.Max(t => t.DT),
                        MaxValue = g.Max(t => t.Price),
                        MinValue = g.Min(t => t.Price),
                        Count = g.Count()
                    }).ToList();

                var tss = ts.Select(t => new Stat()
                {
                    TickerID = t.TickerID,
                    Type = TickBarTypeEnum.Ticks,
                    Period = TimeSeriesStatEnum.All,
                    LastDate = t.LastDate.DateToInt(),
                    LastDT = t.LastDate,
                    FirstDT = t.FirstDate,
                    MaxValue = t.MaxValue,
                    MinValue = t.MinValue,
                    Count = t.Count
                }).ToList();

                foreach (var t in tss)
                {
                    Stat t1 = t;
                    var s = db.Stats.FirstOrDefault(
                            i => i.TickerID == t1.TickerID &&
                            i.Type == t1.Type &&
                            i.Period == TimeSeriesStatEnum.All);
                    if (s == null)
                        db.Stats.Add(t);
                    else
                    {
                        s.Count = t1.Count;

                        s.LastDate = t1.LastDate;

                        s.LastDT = t1.LastDT;
                        s.FirstDT = t1.FirstDT;

                        s.MaxValue = t1.MaxValue;
                        s.MinValue = t1.MinValue;

                        s.ModifiedDT = DateTime.Now;
                    }
                }

                // db.TickStats.AddRange(tss);
                db.SaveChanges();
            }
         }
        [Test]
        public void UpdateTicksStatAll2()
        {
            ConsoleSync.WriteLineST("Start Method: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            using (var db = new FortsTicksContext())
            {
                var ts = (from t in db.Stats
                    .Select(t => new { TickerID = t.Ticker.ID, t.FirstDT, t.LastDT, t.MinValue, t.MaxValue, t.Count })
                          group t by new
                          {
                             t.TickerID
                          }
                              into g
                          select new // TickStat
                          {
                              TickerID = g.Key.TickerID,
                              // Period = TimeSeriesStatEnum.All,
                              FirstDate = g.Min(t => t.FirstDT),
                              LastDate = g.Max(t => t.LastDT),
                              MaxValue = g.Max(t => t.MaxValue),
                              MinValue = g.Min(t => t.MinValue),
                              Count = g.Sum( t => t.Count)
                          }).ToList();

                var tss = ts.Select(t => new Stat()
                {
                    TickerID = t.TickerID,
                    Type = TickBarTypeEnum.Ticks,
                    Period = TimeSeriesStatEnum.All,
                    LastDate = t.LastDate.DateToInt(),
                    LastDT = t.LastDate,
                    FirstDT = t.FirstDate,
                    MaxValue = t.MaxValue,
                    MinValue = t.MinValue,
                    Count = t.Count
                }).ToList();

                foreach (var t in tss)
                {
                    var s = db.Stats.FirstOrDefault(
                            i =>
                            i.TickerID == t.TickerID &&
                            i.Type == t.Type &&
                            i.Period == TimeSeriesStatEnum.All);
                    if (s == null)
                        db.Stats.Add(t);
                    else
                    {
                        s.Count = t.Count;

                        s.LastDT = t.LastDT;
                        s.FirstDT = t.FirstDT;

                        s.MaxValue = t.MaxValue;
                        s.MinValue = t.MinValue;

                        s.ModifiedDT = DateTime.Now;
                    }
                    db.SaveChanges();
                }

                ConsoleSync.WriteLineST("Stats All Updated");
                var stats = db.Stats.Where(st => st.Period == TimeSeriesStatEnum.All);
                foreach (var st in stats)
                {
                    Console.WriteLine(st.ToString());
                }

            }
            ConsoleSync.WriteLineST("Test is Completed");
        }
        [Test]
        public void UpdateTickStatDaily()
        {
            using (var db = new FortsTicksContext())
            {
                var ts = (from t in db.Ticks
                    .Select(t => new { t.Ticker, t.DT, t.Price })
                          group t by new
                          {
                              t.Ticker,
                              dt = DbFunctions.TruncateTime(t.DT)
                          }
                              into g
                              select new // TickStat
                              {
                                  TickerID = g.Key.Ticker.ID,
                                  // Period = TimeSeriesStatEnum.Daily,
                                  LastDate = g.Key.dt,
                                  FirstDT = g.Min(t => t.DT),
                                  LastDT = g.Max(t => t.DT),
                                  MaxValue = g.Max(t => t.Price),
                                  MinValue = g.Min(t => t.Price),
                                  Count = g.Count()
                              }).ToList();

                var tss = ts.Select(t => new Stat()
                {
                    TickerID = t.TickerID,
                    Type =  TickBarTypeEnum.Ticks,
                    Period = TimeSeriesStatEnum.Daily,
                    LastDate = t.LastDT.DateToInt(),
                    LastDT = t.LastDT,
                    FirstDT = t.FirstDT,
                    MaxValue = t.MaxValue,
                    MinValue = t.MinValue,
                    Count = t.Count
                }).ToList();

                foreach (var t in tss)
                {
                    Stat t1 = t;
                    var s = db.Stats.FirstOrDefault(
                            i => i.TickerID == t1.TickerID &&
                            i.Type == t1.Type && 
                            i.Period == t1.Period &&
                            i.LastDate == t1.LastDate);
                    if (s == null)
                        db.Stats.Add(t);
                    else
                    {
                        s.Count = t1.Count;

                        s.LastDT = t1.LastDT;
                        s.FirstDT = t1.FirstDT;

                        s.MaxValue = t1.MaxValue;
                        s.MinValue = t1.MinValue;

                        s.ModifiedDT = DateTime.Now;
                    }
                }

                // db.TickStats.AddRange(tss);
                db.SaveChanges();
            }
        }
        // Create Bar in FortsTicks / Not FullDay Bars Created only Half of Day
        [Test]
        public void CreateBars()
        {
            // const string tickercode = "RIH6";
            using (var db = new FortsTicksContext())
            {
                var tickersdates = db.GetTickerDatesNeedToCreateBars();
                foreach (var p in tickersdates)
                {
                    var tks = db.GetTicks(p.Key, p.Value.IntToDate());
                    Assert.IsNotEmpty(tks, "Ticks id Empty");
                    Console.WriteLine("Ticks Count: {0}", tks.Count());

                    var barLst = new List<Bar>();
                    long tdt = 0;
                    Bar b = null;
                    foreach (var t in tks)
                    {
                        var dts = t.DT.ToLongInSec();
                        if (tdt != dts)
                        {
                            tdt = dts;
                            if (b != null)
                            {
                                VerifyBar(b);
                                barLst.Add(b);
                            }
                            b = NewBar(t);
                            continue;
                        }
                        BarUpdate(b, t);
                    }
                    Console.WriteLine("Bars Count: {0}", barLst.Count());
                    db.AddBars(barLst);

                    // UpdateStatistic
                }
            }
        }

        private Bar NewBar(Tick t)
        {
            return new Bar
            {
                TickerID = t.TickerID,
                DT = t.DT.ToLongInSec().ToDateTime(), 
                Open = t.Price, High = t.Price, Low = t.Price, Close = t.Price,
                Volume = t.Amount
            };
        }

        private void BarUpdate(Bar b, Tick t)
        {
            b.Close = t.Price;
            b.Volume += t.Amount;

            if (t.Price.IsGreaterThan(b.High))
                b.High = t.Price;
            else if (t.Price.IsLessThan(b.Low))
                b.Low = t.Price;
        }
        private void VerifyBar(Bar b)
        {
            Assert.IsTrue(b.High.IsGreaterOrEqualsThan(b.Open));
            Assert.IsTrue(b.High.IsGreaterOrEqualsThan(b.Close));
            Assert.IsTrue(b.High.IsGreaterOrEqualsThan(b.Low));

            Assert.IsTrue(b.Low.IsLessOrEqualsThan(b.Open));
            Assert.IsTrue(b.Low.IsLessOrEqualsThan(b.Close));
            Assert.IsTrue(b.Low.IsLessOrEqualsThan(b.High));
        }

        [Test]
        public void GetBarsFromFortsDbAndWriteItsToTimeSeriesDb()
        {
            string ticker = "RIH6";
            using (var tms = new TimeSeriesContext())
            using (var fortsdb = new FortsTicksContext())
            {
                Model.TimeSeries series;
                var tstat = tms.GetSeriesStat(ticker, 1);
                if (tstat == null)
                {
                    series = tms.GetBarSeries(ticker, 1);
                    Assert.IsNotNull(series);
                }
                series = tms.GetBarSeries(ticker, 1);
                Assert.IsNotNull(series);
                
                //var days = fortsdb.GetDayStats(tickercode)
                var stat = fortsdb.GetTickerTotalStat(ticker);
                Assert.IsNotNull(stat);
                var dates = stat.FirstDT.ToDatesInterval(stat.LastDT);
                 
                while (dates.Date1.IsLessThan(dates.Date2))
                {
                    var bs = fortsdb.GetBarsDaily(ticker, dates.Date1).ToList();
                    Console.WriteLine("Date: {0}, Count: {1}", dates.Date1.ToString("d"), bs.Count());
                    tms.InsertBars(series.Id, bs);
                    dates.Date1 = dates.Date1.AddDays(1);
                    //Assert.IsTrue(bs.Any());
                }
            }
        }

        // Write Bars to TmeSeriesContext
        [Test]
        public void PopulateBarsDataBaseFromTicksDataBase()
        {
            using (var tms = new TimeSeriesContext())
            using (var fortsdb = new FortsTicksContext())
            {
                // var tmstats = tms.GetSeriesStats(1).ToList();
                var srs = tms.GetBarSeriesByTimeInt(1);
                foreach (var sr in srs)
                {
                    var tm = tms.GetSeriesStat(sr.Ticker.Code, sr.TimeInt.TimeInterval);
                    // Assert.IsNotNull(tm);
                    if (tm == null)
                    {
                        var stat = fortsdb.GetTickerTotalBarsStat1(sr.Ticker.Code);
                        Assert.IsNotNull(stat, "Total TickerBarsStat should be not Null");
                        var dates = stat.FirstDT.ToDatesInterval(stat.LastDT);
                        while (dates.Date1.IsLessThan(dates.Date2))
                        {
                            var bs = fortsdb.GetBarsDaily(sr.Ticker.Code, dates.Date1).ToList();
                            Console.WriteLine("Ticker: {0}, SeriesId: {1}, dt1: {2}, dt1: {3}, BarsCount: {4}",
                                sr.Ticker.Code, sr.Id, dates.Date1.ToString("u"), dates.Date1.ToString("u"), bs.Count());
                            tms.InsertBars(sr.Id, bs);
                            dates.IncDate1();
                            //Assert.IsTrue(bs.Any());
                        }
                    }
                    else
                    {
                        var stat = fortsdb.GetTickerTotalBarsStat1(tm.Ticker);
                        Assert.IsNotNull(stat, "Total TickerBarsStat should be not Null");
                        if (tm.LastDate.IsLessInSecThan(stat.LastDT))
                        {
                            var di = tm.LastDate.ToDatesInterval();
                            var bs = fortsdb.GetBars(tm.Ticker, tm.LastDate, di.Date2);
                            Assert.IsNotNull(bs);
                            Assert.IsNotEmpty(bs);
                            Console.WriteLine("Ticker: {0}, SeriesId: {1}, dt1: {2}, dt2: {3}, BarsCount: {4}",
                                tm.Ticker, sr.Id, tm.LastDate.ToString("u"), di.Date2.ToString("u"), bs.Count());

                            tms.InsertBars(sr.Id, bs);

                            var dates = tm.LastDate.ToDatesInterval(stat.LastDT);
                            dates.IncDate1();
                            while (dates.Date1.IsLessThan(dates.Date2))
                            {
                                bs = fortsdb.GetBarsDaily(sr.Ticker.Code, dates.Date1).ToList();
                                Console.WriteLine("Ticker: {0}, SeriesId: {1}, dt1: {2}, dt1: {3}, BarsCount: {4}",
                                    sr.Ticker.Code, sr.Id, dates.Date1.ToString("u"), dates.Date1.ToString("u"), bs.Count());
                                tms.InsertBars(sr.Id, bs);
                                dates.IncDate1();
                                //Assert.IsTrue(bs.Any());
                            }
                        }
                    }
                }
            }
        }

        [Test]
        public void GetTickerDatesNeedToCreateBarsTest()
        {
            using (var db = new FortsTicksContext())
            {
                var tickerdates = db.GetTickerDatesNeedToCreateBars();
                // Assert.AreEqual(11, tickerdates.Count());
                foreach (var kvp in tickerdates)
                    Console.WriteLine("TickerID: {0}, Date: {1}", kvp.Key, kvp.Value);

                var ts = GetTickerDatesNeedToCreateBars1();
                foreach (var t in ts)
                        Console.WriteLine("TickerID: {0}, Date: {1}", t.Key, t.Value);
            }
        }
        //public IEnumerable<KeyValuePair<int, long>> GetTickerDatesNeedToCreateBars()
        //{
        //    using (var db = new FortsTicksContext())
        //    {
        //        var tstats = db.Stats
        //            .Where(s => s.Type == TickBarTypeEnum.Ticks && s.Period == TimeSeriesStatEnum.Daily)
        //            .Select(s => new
        //            {
        //                s.TickerID,
        //                s.LastDate,
        //                // Key = s.TickerID + "@" + s.LastDate
        //            })
        //            .ToList();

        //        var bstats = db.Stats
        //            .Where(s => s.Type == TickBarTypeEnum.Bars && s.Period == TimeSeriesStatEnum.Daily)
        //            .Select(s => new
        //            {
        //                s.TickerID,
        //                s.LastDate,
        //                // Key = s.TickerID + "@" + s.LastDate
        //            })
        //            .ToList();

        //        return tstats.Except(bstats).ToList()
        //                .Select(n=> new KeyValuePair<int,long>(n.TickerID, n.LastDate));
        //    }
        //}

        public IEnumerable<dynamic> GetTickerDatesNeedToCreateBars1()
        {
            using (var db = new FortsTicksContext())
            {
                var tstats = db.Stats
                    .Where(s => s.Type == TickBarTypeEnum.Ticks && s.Period == TimeSeriesStatEnum.Daily)
                    .Select(s => new
                    {
                        s.TickerID,
                        s.LastDate,
                        // Key = s.TickerID + "@" + s.LastDate
                    })
                    .ToList();

                var bstats = db.Stats
                    .Where(s => s.Type == TickBarTypeEnum.Bars && s.Period == TimeSeriesStatEnum.Daily)
                    .Select(s => new
                    {
                        s.TickerID,
                        s.LastDate,
                        // Key = s.TickerID + "@" + s.LastDate
                    })
                    .ToList();

                return tstats.Except(bstats).ToList()
                        .Select(n => new 
                        {
                            TickerID = n.TickerID,
                            LastDate = n.LastDate
                        });
            }
        }
    }
}
