using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;
using GS.Trade.TimeSeries.FortsTicks.Model;
using GS.Trade.TimeSeries.General;

namespace GS.Trade.TimeSeries.FortsTicks.Dal
{
    public partial class FortsTicksContext
    {
        public void UpdateTicksStatAll()
        {
                var ts = (from t in Ticks
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
                    var s = Stats.FirstOrDefault(
                            i => i.TickerID == t1.TickerID &&
                            i.Type == t1.Type &&
                            i.Period == TimeSeriesStatEnum.All);
                    if (s == null)
                        Stats.Add(t);
                    else
                    {
                        s.Count = t1.Count;

                        // Exception is Part of Key
                        s.LastDate = t1.LastDate;

                        s.LastDT = t1.LastDT;
                        s.FirstDT = t1.FirstDT;

                        s.MaxValue = t1.MaxValue;
                        s.MinValue = t1.MinValue;

                        s.ModifiedDT = DateTime.Now;
                    }
                }

                // db.TickStats.AddRange(tss);
                SaveChanges();
            
        }
        public void UpdateTicksStatDaily()
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
                    var s = db.Stats.FirstOrDefault(
                            i => 
                            i.TickerID == t.TickerID &&
                            i.Type == t.Type &&
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

                        s.ModifiedDT = DateTime.Now;
                    }
                    db.SaveChanges();
                }
            }
        }
    }
}
