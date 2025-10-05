using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;
using GS.Trade.TimeSeries.FortsTicks3.Model;
using GS.Trade.TimeSeries.General;

namespace GS.Trade.TimeSeries.FortsTicks3.Dal
{
    public partial class FortsTicksContext3
    {
        public void UpdateTickSeriesStat()
        {
            using (var db = new FortsTicksContext3())
            {
                var ts = (from t in db.Ticks.AsNoTracking()
                    .Select(t => new { t.Series, t.DT })
                          group t by new
                          {
                              t.Series
                          }
                          into g
                          select new
                          {
                              Series = g.Key.Series,
                              FirstDT = g.Min(t => t.DT),
                              LastDT = g.Max(t => t.DT),
                              Count = g.Count()
                          }).ToList();

                foreach (var t in ts)
                {
                    //t.Series.FirstDT = t.FirstDT;
                    //t.Series.LastDT = t.LastDT;
                    //t.Series.Count = t.Count;

                    var srs = FindTickSeries(t.Series.ID);
                    srs.FirstDT = t.FirstDT;
                    srs.LastDT = t.LastDT;
                    srs.Count = t.Count;
                    SaveChanges();
                }

                //TickSeries.RemoveRange(TickSeries.Where(t => t.Count == 0));
                //SaveChanges();

                //foreach (var t in tss)
                //{
                //    var s = db.Stats.FirstOrDefault(
                //            i =>
                //            i.TickerID == t.TickerID &&
                //            i.Type == t.Type &&
                //            i.Period == t.Period &&
                //            i.LastDate == t.LastDate);
                //    if (s == null)
                //        db.Stats.Add(t);
                //    else
                //    {
                //        s.Count = t.Count;

                //        s.LastDT = t.LastDT;
                //        s.FirstDT = t.FirstDT;

                //        s.MaxValue = t.MaxValue;
                //        s.MinValue = t.MinValue;

                //        s.ModifiedDT = DateTime.Now;
                //    }
                //    db.SaveChanges();
                //}
            }

        }
        public void UpdateBarSeriesStat()
        {
            using (var db = new FortsTicksContext3())
            {
                var ts = (from t in db.Bars.AsNoTracking()
                    .Select(t => new { t.Series, t.DT })
                          group t by new
                          {
                              t.Series
                          }
                          into g
                          select new
                          {
                              Series = g.Key.Series,
                              FirstDT = g.Min(t => t.DT),
                              LastDT = g.Max(t => t.DT),
                              Count = g.Count()
                          }).ToList();

                foreach (var t in ts)
                {
                    var srs = FindBarSeries(t.Series.ID);
                    srs.FirstDT = t.FirstDT;
                    srs.LastDT = t.LastDT;
                    srs.Count = t.Count;
                    SaveChanges();
                }
            }
        }



        //public void UpdateTicksStatAll()
        //{
        //    var ts = (from t in Ticks
        //        .Select(t => new { t.Ticker, t.DT, t.Price })
        //              group t by new
        //              {
        //                  t.Ticker
        //              }
        //                  into g
        //              select new // TickStat
        //              {
        //                  TickerID = g.Key.Ticker.ID,
        //                  // Period = TimeSeriesStatEnum.All,
        //                  FirstDate = g.Min(t => t.DT),
        //                  LastDate = g.Max(t => t.DT),
        //                  MaxValue = g.Max(t => t.Price),
        //                  MinValue = g.Min(t => t.Price),
        //                  Count = g.Count()
        //              }).ToList();

        //    var tss = ts.Select(t => new Stat()
        //    {
        //        TickerID = t.TickerID,
        //        Type = TickBarTypeEnum.Ticks,
        //        Period = StatTimeIntType.All,
        //        LastDate = t.LastDate.DateToInt(),
        //        LastDT = t.LastDate,
        //        FirstDT = t.FirstDate,
        //        MaxValue = t.MaxValue,
        //        MinValue = t.MinValue,
        //        Count = t.Count
        //    }).ToList();

        //    foreach (var t in tss)
        //    {
        //        Stat t1 = t;
        //        var s = Stats.FirstOrDefault(
        //                i => i.TickerID == t1.TickerID &&
        //                i.Type == t1.Type &&
        //                i.Period == StatTimeIntType.All);
        //        if (s == null)
        //            Stats.Add(t);
        //        else
        //        {
        //            s.Count = t1.Count;

        //            // Exception is Part of Key
        //            s.LastDate = t1.LastDate;

        //            s.LastDT = t1.LastDT;
        //            s.FirstDT = t1.FirstDT;

        //            s.MaxValue = t1.MaxValue;
        //            s.MinValue = t1.MinValue;

        //            s.ModifiedDT = DateTime.Now;
        //        }
        //    }

        //    // db.TickStats.AddRange(tss);
        //     SaveChanges();
        //  }
        public void UpdateTicksStatAll()
        {
            var ts = (from st in Stats
                .Where(st=>st.TimeSeriesType == TimeSeriesTypeEnum.Ticks && st.Period == StatTimeIntType.Daily)
                .Select(st => new { st.TickerID, st.Count, st.FirstDT, st.LastDT, st.MaxValue, st.MinValue, st.LastDate, st.Avg })
                      group st by new
                      {
                          st.TickerID
                      }
                      into g
                      select new 
                      {
                          TickerID = g.Key.TickerID,
                          TimeSeriesType = TimeSeriesTypeEnum.Ticks,
                          Period = StatTimeIntType.All,
                          LastDate = g.Max(t=>t.LastDate),
                          FirstDT = g.Min(t => t.FirstDT),
                          LastDT = g.Max(t => t.LastDT),
                          MaxValue = g.Max(t => t.MaxValue),
                          MinValue = g.Min(t => t.MinValue),
                          Count = g.Sum(t=>t.Count),
                          Avg = g.Average(t => t.Avg)
                      }).ToList();

            var tss = ts.Select(t => new Stat()
            {
                TickerID = t.TickerID,
                TimeSeriesType = TimeSeriesTypeEnum.Ticks,
                Period = StatTimeIntType.All,
                LastDate = t.LastDate,
                LastDT = t.LastDT,
                FirstDT = t.FirstDT,
                MaxValue = t.MaxValue,
                MinValue = t.MinValue,
                Count = t.Count,
                Avg = t.Avg
            }).ToList();

            foreach (var t in tss)
            {
                var s = Stats.FirstOrDefault(
                    i => i.TickerID == t.TickerID &&
                         i.TimeSeriesType == t.TimeSeriesType &&
                         i.Period == StatTimeIntType.All);
                        
                if (s == null)
                    Stats.Add(t);
                else
                {
                    s.Count = t.Count;

                    // Exception is Part of Key
                    s.LastDate = t.LastDate;

                    s.LastDT = t.LastDT;
                    s.FirstDT = t.FirstDT;

                    s.MaxValue = t.MaxValue;
                    s.MinValue = t.MinValue;

                    s.Avg = t.Avg;

                    s.Modified = DateTime.Now;
                }
                SaveChanges();
            }

            // db.TickStats.AddRange(tss);
            
        }

        public void UpdateTicksStatDaily()
        {
            using (var db = new FortsTicksContext3())
            {
                var ts = (from t in db.Ticks
                    .Select(t => new { t.Series.Ticker, t.Series.Date, t.DT, t.Price })
                          group t by new
                          {
                              t.Ticker,
                              // dt = DbFunctions.TruncateTime(t.DT)
                              dt = t.Date
                          }
                              into g
                              select  new // TickStat
                              {
                                  Ticker = g.Key.Ticker,
                                  // Period = TimeSeriesStatEnum.Daily,
                                  LastDate = g.Key.dt,
                                  FirstDT = g.Min(t => t.DT),
                                  LastDT = g.Max(t => t.DT),
                                  MaxValue = g.Max(t => t.Price),
                                  MinValue = g.Min(t => t.Price),
                                  Avg = g.Average(t=>t.Price),
                                //  StDev = DbFunctions.StandardDeviation(t => t.Price),
                                  Count = g.Count()
                              }).ToList();

                var tss = ts.Select(t => new Stat()
                {
                    TickerID = t.Ticker.ID,
                    TimeSeriesType = TimeSeriesTypeEnum.Ticks,
                    Period = StatTimeIntType.Daily,
                    LastDate = t.LastDT.Date,
                    LastDT = t.LastDT,
                    FirstDT = t.FirstDT,
                    MaxValue = t.MaxValue,
                    MinValue = t.MinValue,
                    Avg = t.Avg,
                 //   StDev = EntityFunctions.StandardDeviation(s.Select(t => t.Minutes))
                    Count = t.Count
                }).ToList();

                foreach (var t in tss)
                {
                    var s = db.Stats.FirstOrDefault(
                            i => 
                            i.TickerID == t.TickerID &&
                            i.TimeSeriesType == t.TimeSeriesType &&
                            i.Period == t.Period &&
                            i.LastDate == t.LastDate);
                    if (s == null)
                        db.Stats.Add(t);
                    else
                    {
                        s.Count = t.Count;

                        s.LastDT = t.LastDT;
                        s.FirstDT = t.FirstDT;

                        s.MaxValue = t.MaxValue;
                        s.MinValue = t.MinValue;

                        s.Avg = t.Avg;

                        s.Modified = DateTime.Now;
                    }
                    db.SaveChanges();
                }
            }
        }

        public void RemoveEmptyTickSeries()
        {
          //  TickSeries.RemoveRange(TickSeries.Where(t => t.Count == 0));

            Stats.RemoveRange(Stats.Where(t => t.Count == 0));

            SaveChanges();
        }

    }
}
