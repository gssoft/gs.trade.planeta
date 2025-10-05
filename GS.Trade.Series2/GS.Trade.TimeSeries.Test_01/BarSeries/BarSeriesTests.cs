using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;
using GS.Trade.TimeSeries.BarSeries.Dal;
using GS.Trade.TimeSeries.BarSeries.Models;
using GS.Trade.TimeSeries.FortsTicks.Dal;
using GS.Trade.TimeSeries.General;
using GS.Trade.TimeSeries.Model;
using NUnit.Framework;
using QuoteProvider = GS.Trade.TimeSeries.BarSeries.Models.QuoteProvider;
using Ticker = GS.Trade.TimeSeries.BarSeries.Models.Ticker;
using TimeInt = GS.Trade.TimeSeries.BarSeries.Models.TimeInt;
using TradeBoard = GS.Trade.TimeSeries.BarSeries.Models.TradeBoard;

namespace GS.Trade.TimeSeries.Test_01.BarSeries
{
    [TestFixture]
    public class BarSeriesTests
    {
        [OneTimeSetUp]
        public void Init()
        {
            InitDb();
        }
        [Test]
        public void FirstTest()
        {
            using (var db = new BarSeriesContext01())
            {
                var tks = db.Tickers.ToList();
                foreach(var t in tks)
                    Console.WriteLine("Code: {0}, Name: {1}", t.Code, t.Name);
            }
        }

        //[Test]
        //public void ClearDb()
        //{
        //    //using (var db = new BarSeriesContext01())
        //    //{
        //    //    db.TimeSeries.RemoveRange(db.TimeSeries);
        //    //    db.TimeInts.RemoveRange(db.TimeInts);
        //    //    db.QuoteProviders.RemoveRange(db.QuoteProviders);
        //    //    db.Tickers.RemoveRange(db.Tickers);
        //    //    db.TradeBoards.RemoveRange(db.TradeBoards);
        //    //    db.Bars.RemoveRange(db.Bars);
        //    //    db.Ticks.RemoveRange(db.Ticks);

        //    //    db.SaveChanges();
        //    //}
        //}
        [Test]
        public void CreateTimeStatAll()
        {
            Assert.DoesNotThrow(() => {
            using (var db = new BarSeriesContext01())
            {
                foreach (var sr in db.TimeSeries.Include(sr => sr.Stats))
                {
                    if (!sr.Stats.Any())
                    {
                        var st = new Stat
                        {
                            Period = TimeSeriesStatEnum.All,
                            Type = TickBarTypeEnum.Bars,
                            TimeSeriesID = sr.ID,
                            //FirstDT = DateTime.MinValue.MinValueToSql(),
                            //LastDT = DateTime.MinValue.MinValueToSql(),
                        };
                        db.Stats.Add(st);
                        Console.WriteLine("AddNew Stat: {0}", st);
                    }
                }
                db.SaveChanges();
            }
            });
        }
        // FortsTicks Ticks to Bars in BarSeries
        [Test]
        public void PopulateBarsDataBaseFromTicksDataBase()
        {
            using (var barsdb = new BarSeriesContext01())
            using (var fortsdb = new FortsTicksContext())
            {
                var srs = barsdb.GetBarSeriesByTimeInt(1);
                foreach (var sr in srs)
                {
                    var seriesBarStat = barsdb.GetSeriesStat(sr.Ticker.Code, sr.TimeInt.TimeInterval);
                    // Assert.IsNotNull(tm);
                    if (seriesBarStat == null)
                    {
                        var stat = fortsdb.GetTickerTotalBarsStat1(sr.Ticker.Code);
                        Assert.IsNotNull(stat, "Total TickerBarsStat should be not Null");
                        var dates = stat.FirstDT.ToDatesInterval(stat.LastDT);
                        while (dates.Date1.IsLessThan(dates.Date2))
                        {
                            var bs = fortsdb.GetBarsDaily(sr.Ticker.Code, dates.Date1).ToList();
                            Console.WriteLine("Ticker: {0}, SeriesId: {1}, dt1: {2}, dt1: {3}, BarsCount: {4}",
                                sr.Ticker.Code, sr.ID, dates.Date1.ToString("u"), dates.Date1.ToString("u"), bs.Count());
                            barsdb.InsertBars(sr.ID, bs);
                            dates.IncDate1();
                            //Assert.IsTrue(bs.Any());
                        }
                    }
                    else
                    {
                        var tickerCode = seriesBarStat.TimeSeries.Ticker.Code;
                        var tickStats = fortsdb.GetTickerDailyTicksStats(tickerCode);
                        foreach (var tst in tickStats)
                        {
                            var bst = barsdb.GetDailySeriesStat(tickerCode, 1, tst.LastDate.DateToInt());
                            if (bst == null)
                            {
                                var st = new Stat
                                {
                                    LastDate = tst.LastDate.DateToInt(),
                                    Period = TimeSeriesStatEnum.Daily,
                                    Type = TickBarTypeEnum.Bars,
                                    TimeSeriesID = sr.ID,
                                };
                                barsdb.Stats.Add(st);
                                Console.WriteLine("AddNew Stat: {0}", st);
                                
                                var ts = fortsdb.GetTicks(tst.EntityCode, tst.LastDate);
                            }
                            
                        }

                        var ticksStat = fortsdb.GetTickerTotalTicksStat(tickerCode);
                        Assert.IsNotNull(ticksStat, "Total TickerTicksStat should be not Null");
                        if (seriesBarStat.LastDT.IsLessInSecThan(ticksStat.LastDT))
                        {
                            var di = seriesBarStat.LastDT.ToDatesInterval();
                            var bs = fortsdb.GetBars(tickerCode, seriesBarStat.LastDT, di.Date2);
                            Assert.IsNotNull(bs);
                            Assert.IsNotEmpty(bs);
                            Console.WriteLine("Ticker: {0}, SeriesId: {1}, dt1: {2}, dt2: {3}, BarsCount: {4}",
                                tickerCode, sr.ID, seriesBarStat.LastDate.ToString("u"), di.Date2.ToString("u"), bs.Count());

                            barsdb.InsertBars(sr.ID, bs);

                            var dates = seriesBarStat.LastDT.ToDatesInterval(ticksStat.LastDT);
                            dates.IncDate1();
                            while (dates.Date1.IsLessThan(dates.Date2))
                            {
                                bs = fortsdb.GetBarsDaily(sr.Ticker.Code, dates.Date1).ToList();
                                Console.WriteLine("Ticker: {0}, SeriesId: {1}, dt1: {2}, dt1: {3}, BarsCount: {4}",
                                    sr.Ticker.Code, sr.ID, dates.Date1.ToString("u"), dates.Date1.ToString("u"), bs.Count());
                                barsdb.InsertBars(sr.ID, bs);
                                dates.IncDate1();
                                //Assert.IsTrue(bs.Any());
                            }
                        }
                    }
                }
            }
        }

        // Bars to Bars from FortsTicks to BarSeries
        [Test]
        public void PopulateBarsDataBaseFromTicksDataBase1()
        {
            using (var tms = new BarSeriesContext01())
            using (var fortsdb = new FortsTicksContext())
            {
                var srs = tms.GetBarSeriesByTimeInt(1);
                foreach (var sr in srs)
                {
                    var seriesStat = tms.GetSeriesStat(sr.Ticker.Code, sr.TimeInt.TimeInterval);
                    // Assert.IsNotNull(tm);
                    if (seriesStat == null)
                    {
                        var stat = fortsdb.GetTickerTotalBarsStat1(sr.Ticker.Code);
                        Assert.IsNotNull(stat, "Total TickerBarsStat should be not Null");
                        var dates = stat.FirstDT.ToDatesInterval(stat.LastDT);
                        while (dates.Date1.IsLessThan(dates.Date2))
                        {
                            var bs = fortsdb.GetBarsDaily(sr.Ticker.Code, dates.Date1).ToList();
                            Console.WriteLine("Ticker: {0}, SeriesId: {1}, dt1: {2}, dt1: {3}, BarsCount: {4}",
                                sr.Ticker.Code, sr.ID, dates.Date1.ToString("u"), dates.Date1.ToString("u"), bs.Count());
                            tms.InsertBars(sr.ID, bs);
                            dates.IncDate1();
                            //Assert.IsTrue(bs.Any());
                        }
                    }
                    else
                    {
                        var tickerCode = seriesStat.TimeSeries.Ticker.Code;

                        var stat = fortsdb.GetTickerTotalBarsStat1(tickerCode);
                        Assert.IsNotNull(stat, "Total TickerBarsStat should be not Null");
                        if (seriesStat.LastDT.IsLessInSecThan(stat.LastDT))
                        {
                            var di = seriesStat.LastDate.IntToDate().ToDatesInterval();
                            var bs = fortsdb.GetBars(tickerCode, seriesStat.LastDT, di.Date2);
                            Assert.IsNotNull(bs);
                            Assert.IsNotEmpty(bs);
                            Console.WriteLine("Ticker: {0}, SeriesId: {1}, dt1: {2}, dt2: {3}, BarsCount: {4}",
                                tickerCode, sr.ID, seriesStat.LastDate.ToString("u"), di.Date2.ToString("u"), bs.Count());

                            tms.InsertBars(sr.ID, bs);

                            var dates = seriesStat.LastDT.ToDatesInterval(stat.LastDT);
                            dates.IncDate1();
                            while (dates.Date1.IsLessThan(dates.Date2))
                            {
                                bs = fortsdb.GetBarsDaily(sr.Ticker.Code, dates.Date1).ToList();
                                Console.WriteLine("Ticker: {0}, SeriesId: {1}, dt1: {2}, dt1: {3}, BarsCount: {4}",
                                    sr.Ticker.Code, sr.ID, dates.Date1.ToString("u"), dates.Date1.ToString("u"), bs.Count());
                                tms.InsertBars(sr.ID, bs);
                                dates.IncDate1();
                                //Assert.IsTrue(bs.Any());
                            }
                        }
                    }
                }
            }
        }
        private void InitDb()
        {
            Assert.DoesNotThrow( () =>
            {
            Database.SetInitializer(new TimeSeries.BarSeries.Init.Initializer());
            using (var db = new BarSeriesContext01())
            {
                var tbs = db.TradeBoards.ToList();
                if (!tbs.Any())
                {
                    var tb = new TradeBoard
                    {
                        Code = "Forts",
                        Name = "Moex.Forts"
                    };
                    db.TradeBoards.Add(tb);
                    db.SaveChanges();
                }
                var tks = db.Tickers.ToList();
                if (!tks.Any())
                {
                    var tb =
                        db.TradeBoards.FirstOrDefault(
                            tbr => tbr.Code.
                                Equals("Forts", StringComparison.InvariantCultureIgnoreCase));
                    Assert.IsNotNull(tb);
                    var t = new Ticker
                    {
                        Code = "RIH6",
                        Name = "RTS-3.16",
                        TradeBoardID = tb.ID
                    };
                    db.Tickers.Add(t);
                    t = new Ticker
                    {
                        Code = "SiH6",
                        Name = "Si-3.16",
                        TradeBoardID = tb.ID
                    };
                    db.Tickers.Add(t);
                    t = new Ticker
                    {
                        Code = "SRH6",
                        Name = "SBRF-3.16",
                        TradeBoardID = tb.ID
                    };
                    db.Tickers.Add(t);
                    db.SaveChanges();
                }
                // TimeInts
                var tmints = db.TimeInts.ToList();
                if (!tmints.Any())
                {
                    var tmi = new TimeInt
                    {
                        Code = "1Sec",
                        TimeInterval = 1,
                        Description = "1Sec"
                    };

                    db.TimeInts.Add(tmi);
                    db.SaveChanges();
                }
                // QuoteProviders
                var qps = db.QuoteProviders.ToList();
                if (!qps.Any())
                {
                    var qp = new QuoteProvider
                    {
                        Code = "Forts.Pub.Stat",
                        Name = "Forts.Pub.Stat",
                        Description = @"ftp://ftp.moex.ru/pub/info/stats/history",
                    };

                    db.QuoteProviders.Add(qp);
                    db.SaveChanges();
                }
                if (!db.TimeSeries.ToList().Any())
                {
                    var tmi = db.TimeInts.FirstOrDefault(t => t.TimeInterval == 1);
                    Assert.IsNotNull(tmi);
                    var qp = db.QuoteProviders.FirstOrDefault(q => q.Code == "Forts.Pub.Stat");
                    Assert.IsNotNull(qp);
                    foreach (var t in db.Tickers.ToList())
                    {
                        var sr = new TimeSeries.BarSeries.Models.BarSeries
                        {
                            TickerID = t.ID,
                            TimeIntID = tmi.ID,
                            QuoteProviderID = qp.ID,
                            Code = t.Code + " " + tmi.Code,
                        };
                        db.TimeSeries.Add(sr);
                    }
                    db.SaveChanges();
                }
            }
            });
        }
    }
}
