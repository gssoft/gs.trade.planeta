using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.BatchWorks;
using GS.Elements;
using GS.Extension;
using GS.Trade.TimeSeries.FortsTicks.Dal;
using GS.Trade.TimeSeries.FortsTicks.Model;
using GS.Trade.TimeSeries.General;
using GS.Trade.TimeSeries.Model;

namespace GS.Trade.TimeSeries.BatchWorks
{
    public class BarsFromTicksInDb : Element1<string>, IBatchWorkItem
    {
        //public string SourcePath { get; set; } // = @"F:\Forts\2016\Txt\";
        //public string SourceFileFilter { get; set; } // = @"FT*.CSV";

        public override string Key
        {
            get { return FullCode; }
        }

        public void DoWork()
        {
            try
            {
                PopulateBarsDataBaseFromTicksDataBase();

                UpdateTicksStatDaily();

                UpdateTicksStatAll();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void PopulateBarsDataBaseFromTicksDataBase()
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
                        if(stat==null)
                            throw new Exception("Forts.Ticks.Ticker.Stat is Null");
                        //Assert.IsNotNull(stat);
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
                        //Assert.IsNotNull(stat);
                        if (tm.LastDate.IsLessInSecThan(stat.LastDT))
                        {
                            var di = tm.LastDate.ToDatesInterval();
                            var bs = fortsdb.GetBars(tm.Ticker, tm.LastDate, di.Date2);
                            //Assert.IsNotNull(bs);
                            //Assert.IsNotEmpty(bs);
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

        private void UpdateTicksStatAll()
        {
            using (var db = new FortsTicksContext())
            {
                var ts = (from t in db.Ticks
                    .Select(t => new { t.Ticker, t.DT, t.Price })
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
        private void UpdateTicksStatAll2()
        {
            using (var db = new FortsTicksContext())
            {
                var ts = (from t in db.Stats
                    .Select(t => new { t.Ticker, t.FirstDT, t.LastDT, t.MinValue, t.MaxValue })
                          group t by new
                          {
                              t.Ticker
                          }
                              into g
                          select new // TickStat
                          {
                              TickerID = g.Key.Ticker.ID,
                              // Period = TimeSeriesStatEnum.All,
                              FirstDate = g.Min(t => t.FirstDT),
                              LastDate = g.Max(t => t.LastDT),
                              MaxValue = g.Max(t => t.MaxValue),
                              MinValue = g.Min(t => t.MinValue),
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
            }
        }

        private void UpdateTicksStatDaily()
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
                    Type = TickBarTypeEnum.Ticks,
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
                db.SaveChanges();
            }
        }
    }
}
