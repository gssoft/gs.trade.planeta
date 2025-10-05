using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using EntityFramework.BulkInsert.Extensions;
using GS.Extension;
using GS.Trade.TimeSeries.Model.View;

namespace GS.Trade.TimeSeriesIm01.Models
{
    using BarDto = GS.Trade.Dto.Bar;

    public partial class TimeSeriesContext
    {
        public IEnumerable<TimeSeriesStat> StatSelect01()
        {
            var q = (from b in Bars
                group b by new
                {
                    TimeSeriesId = b.TimeSeries.Id,
                }
                into g
                select new TimeSeriesStat
                {
                    Id = g.Key.TimeSeriesId,
                    FirstDate = g.Min(b => b.DT),
                    LastDate = g.Max(b => b.DT),
                    Count = g.Count()
                }).ToList();

            var j = q.Join(
                TimeSeries,
                st => st.Id,
                tm => tm.Id,
                (st, tm) => new TimeSeriesStat
                {
                    Id = st.Id,
                    ProviderId = tm.QuoteProviderId,
                    TickerId = tm.TickerId,
                    TimeIntId = tm.TimeIntId,
                    Provider = tm.QuoteProvider.Code,
                    Ticker = tm.Ticker.Code,
                    TimeInt = tm.TimeInt.Code,
                    FirstDate = st.FirstDate,
                    LastDate = st.LastDate,
                    Count = st.Count
                })
                .OrderBy(t => t.Provider)
                .ThenBy(t => t.Ticker)
                .ThenBy(t => t.TimeInt)
                ;
            return j;
        }

        public IEnumerable<TimeSeriesStat> StatSelect02()
        {
            var qry = (from b in Bars
                group b by new
                {
                    //Provider = b.TimeSeries.QuoteProvider.QuoteProviderKey,
                    //Ticker = b.TimeSeries.Ticker.TickerKey,
                    //TimeInt = b.TimeSeries.TimeInt.TimeIntKey
                    TimeSeriesId = b.TimeSeries.Id,
                    Provider = b.TimeSeries.QuoteProvider.Code,
                    Ticker = b.TimeSeries.Ticker.Code,
                    TimeInt = b.TimeSeries.TimeInt.Code
                }
                into g
                select new TimeSeriesStat
                {
                    Id = g.Key.TimeSeriesId,
                    Provider = g.Key.Provider,
                    Ticker = g.Key.Ticker,
                    TimeInt = g.Key.TimeInt,
                    FirstDate = g.Min(b => b.DT),
                    LastDate = g.Max(b => b.DT),
                    Count = g.Count()
                })
                .OrderBy(t => t.Provider)
                .ThenBy(t => t.Ticker)
                .ThenBy(t => t.TimeInt)
                ;
            return qry;
        }

        public IQueryable<TimeSeriesStat> StatSelect03()
        {
            // Database.CommandTimeout = 300;

            var q = (from b in Bars.AsNoTracking()
                group b by new
                {
                    //TimeSeriesId = b.TimeSeries.Id,
                    TimeSeries = b.TimeSeries
                }
                into g
                select new TimeSeriesStat
                {
                    Id = g.Key.TimeSeries.Id,
                    ProviderId = g.Key.TimeSeries.QuoteProviderId,
                    Provider = g.Key.TimeSeries.QuoteProvider.Code,
                    TickerId = g.Key.TimeSeries.TickerId,
                    Ticker = g.Key.TimeSeries.Ticker.Code,
                    TimeIntId = g.Key.TimeSeries.TimeIntId,
                    TimeInt = g.Key.TimeSeries.TimeInt.Code,
                    FirstDate = g.Min(b => b.DT),
                    LastDate = g.Max(b => b.DT),
                    MinValue = g.Min(b => b.Low),
                    MaxValue = g.Max(b => b.High),
                    Count = g.Count()
                })
                .OrderBy(t => t.Provider)
                .ThenBy(t => t.Ticker)
                .ThenBy(t => t.TimeInt)
                ;
            return q;
        }
        public IQueryable<TimeSeriesStat> StatSelect031()
        {
            // Database.CommandTimeout = 300;

            var q = (from b in Bars
                         .AsNoTracking()
                         .Select(b=> new {b.TimeSeries, b.DT})
                         .AsNoTracking()
                     group b by new
                     {
                         //TimeSeriesId = b.TimeSeries.Id,
                         TimeSeries = b.TimeSeries
                     }
                         into g
                         select new TimeSeriesStat
                         {
                             Id = g.Key.TimeSeries.Id,
                             ProviderId = g.Key.TimeSeries.QuoteProviderId,
                             Provider = g.Key.TimeSeries.QuoteProvider.Code,
                             TickerId = g.Key.TimeSeries.TickerId,
                             Ticker = g.Key.TimeSeries.Ticker.Code,
                             TimeIntId = g.Key.TimeSeries.TimeIntId,
                             TimeInt = g.Key.TimeSeries.TimeInt.Code,
                             FirstDate = g.Min(b => b.DT),
                             LastDate = g.Max(b => b.DT),
                             //MinValue = g.Min(b => b.Low),
                             //MaxValue = g.Max(b => b.High),
                             Count = g.Count()
                         })
                .OrderBy(t => t.Provider)
                .ThenBy(t => t.Ticker)
                .ThenBy(t => t.TimeInt)
                ;
            return q;
        }

        public IQueryable<TimeSeriesStat> StatSelect032()
        {
            // Database.CommandTimeout = 300;

            var q = (from b in Bars
                        // .AsNoTracking()
                         .Select(b => new { b.TimeSeries, b.DT })
                        // .AsNoTracking()
                     group b by new
                     {
                         //TimeSeriesId = b.TimeSeries.Id,
                         TimeSeries = b.TimeSeries
                     }
                         into g
                         select new TimeSeriesStat
                         {
                             Id = g.Key.TimeSeries.Id,
                             ProviderId = g.Key.TimeSeries.QuoteProviderId,
                             Provider = g.Key.TimeSeries.QuoteProvider.Code,
                             TickerId = g.Key.TimeSeries.TickerId,
                             Ticker = g.Key.TimeSeries.Ticker.Code,
                             TimeIntId = g.Key.TimeSeries.TimeIntId,
                             TimeInt = g.Key.TimeSeries.TimeInt.Code,
                             FirstDate = g.Min(b => b.DT),
                             LastDate = g.Max(b => b.DT),
                             //MinValue = g.Min(b => b.Low),
                             //MaxValue = g.Max(b => b.High),
                             Count = g.Count()
                         })
                .OrderBy(t => t.Provider)
                .ThenBy(t => t.Ticker)
                .ThenBy(t => t.TimeInt)
                ;
            return q;
        }
        public async Task<IEnumerable<TimeSeriesStat>> StatSelect03Async()
        {
            // Database.CommandTimeout = 300;

            var bs = await Bars.AsNoTracking().ToListAsync();

            var q = (from b in bs
                     group b by new
                     {
                         //TimeSeriesId = b.TimeSeries.Id,
                         TimeSeries = b.TimeSeries
                     }
                         into g
                         select new TimeSeriesStat
                         {
                             Id = g.Key.TimeSeries.Id,
                             ProviderId = g.Key.TimeSeries.QuoteProviderId,
                             Provider = g.Key.TimeSeries.QuoteProvider.Code,
                             TickerId = g.Key.TimeSeries.TickerId,
                             Ticker = g.Key.TimeSeries.Ticker.Code,
                             TimeIntId = g.Key.TimeSeries.TimeIntId,
                             TimeInt = g.Key.TimeSeries.TimeInt.Code,
                             FirstDate = g.Min(b => b.DT),
                             LastDate = g.Max(b => b.DT),
                             MinValue = g.Min(b => b.Low),
                             MaxValue = g.Max(b => b.High),
                             Count = g.Count()
                         })
                .OrderBy(t => t.Provider)
                .ThenBy(t => t.Ticker)
                .ThenBy(t => t.TimeInt)
                ;
            return q;
        }

        public TimeSeriesStat GetSeriesStat(string ticker, int? timeInt)
        {
            // Database.CommandTimeout = 300;

            var tms = GetBarSeries(ticker, timeInt);
            if (tms == null)
                return null;
            var q = (from b in Bars
                         .Where(b => b.BarSeriesId == tms.Id)
                         .AsNoTracking()
                group b by new
                {
                    //TimeSeriesId = b.TimeSeries.Id,
                    TimeSeries = b.TimeSeries
                }
                into g
                select new TimeSeriesStat
                {
                    Id = g.Key.TimeSeries.Id,
                    ProviderId = g.Key.TimeSeries.QuoteProviderId,
                    Provider = g.Key.TimeSeries.QuoteProvider.Code,
                    TickerId = g.Key.TimeSeries.TickerId,
                    Ticker = g.Key.TimeSeries.Ticker.Code,
                    TimeIntId = g.Key.TimeSeries.TimeIntId,
                    TimeInt = g.Key.TimeSeries.TimeInt.Code,
                    FirstDate = g.Min(b => b.DT),
                    LastDate = g.Max(b => b.DT),
                    Count = g.Count()
                })
                //.OrderBy(t => t.Provider)
                //.ThenBy(t => t.Ticker)
                //.ThenBy(t => t.TimeInt)
                .FirstOrDefault()
                ;
            return q;
        }

        public TimeSeries GetBarSeries(string ticker, int? timeInt)
        {
            if (!timeInt.HasValue || timeInt == 0 ||
                string.IsNullOrWhiteSpace(ticker))
                return null;
            ticker = ticker.Trim();
            return TimeSeries
                .FirstOrDefault(tm => tm.Ticker.Code == ticker
                                      && tm.TimeInt.TimeInterval == timeInt)
                ;
        }
        public IEnumerable<TimeSeries> GetBarSeriesByTimeInt(int timeInt)
        {
            return TimeSeries
                .Where(tm => tm.TimeInt.TimeInterval == timeInt).ToList();
        }

        public IQueryable<TimeSeriesStat> GetSeriesStats(int timeInt)
        {
            var q = (from b in Bars
                         .Where(b=>b.TimeSeries.TimeInt.TimeInterval == 1)
                         .AsNoTracking()
                         .Select(b => new { b.TimeSeries, b.DT })
                         .AsNoTracking()
                     group b by new
                     {
                         //TimeSeriesId = b.TimeSeries.Id,
                         TimeSeries = b.TimeSeries
                     }
                         into g
                         select new TimeSeriesStat
                         {
                             Id = g.Key.TimeSeries.Id,
                             ProviderId = g.Key.TimeSeries.QuoteProviderId,
                             Provider = g.Key.TimeSeries.QuoteProvider.Code,
                             TickerId = g.Key.TimeSeries.TickerId,
                             Ticker = g.Key.TimeSeries.Ticker.Code,
                             TimeIntId = g.Key.TimeSeries.TimeIntId,
                             TimeInt = g.Key.TimeSeries.TimeInt.Code,
                             FirstDate = g.Min(b => b.DT),
                             LastDate = g.Max(b => b.DT),
                             //MinValue = g.Min(b => b.Low),
                             //MaxValue = g.Max(b => b.High),
                             Count = g.Count()
                         })
                //.OrderBy(t => t.Provider)
                //.ThenBy(t => t.Ticker)
                //.ThenBy(t => t.TimeInt)
                ;
            return q;
        }

        public IQueryable<TimeSeriesItem> GetTimeSeriesItems(TimeSeries tm)
        {
            if (tm == null)
                return null;
            var bs = Bars
                .Where(b => b.BarSeriesId == tm.Id)
                .AsNoTracking()
                ;
            return bs;
        }

        public DateTime? GetTimeSeriesLastDate(TimeSeries tm)
        {
            if (tm == null)
                return null;
            return Bars
                    .Where(b => b.BarSeriesId == tm.Id)
                    .AsNoTracking()
                    .Max(b => b.DT).Date;
        }

        public DateTime[] GetBarsLastFirstDates(int dayNumber)
        {
            var dts =
                Bars
                .AsNoTracking()
                .Select(b => DbFunctions.TruncateTime(b.DT)).Distinct().OrderByDescending(d => d).Take(dayNumber);

            var r = new DateTime[2];

            var lastOrDefault = dts.FirstOrDefault();
            if (lastOrDefault != null) r[1] = ((DateTime) lastOrDefault).AddDays(1);

            var firstOrDefault = dts.OrderBy(d => d).FirstOrDefault();
            if (firstOrDefault != null) r[0] = ((DateTime) firstOrDefault);
            return r;
        }

        public DateTime[] GetBarsLastFirstDates(long barSeriesId, int dayNumber)
        {
            var dts = Bars
                .Where(b => b.BarSeriesId == barSeriesId)
                .AsNoTracking()
                .Select(b => DbFunctions.TruncateTime(b.DT)).Distinct().OrderByDescending(d => d).Take(dayNumber);

            var r = new DateTime[2];

            var lastOrDefault = dts.FirstOrDefault();
            if (lastOrDefault != null) r[1] = ((DateTime) lastOrDefault).AddDays(1);

            var firstOrDefault = dts.OrderBy(d => d).FirstOrDefault();
            if (firstOrDefault != null) r[0] = ((DateTime) firstOrDefault);
            return r;
        }

        // 15.11.28
        public IQueryable<BarDto> GetBarsDto(long? seriesId, int date)
        {
            if (!seriesId.HasValue || seriesId == 0)
                return null;

            var date1 = date.IntToDate();
            var date2 = date1.AddDays(1);

            var bs = (Bars
                .Where(b => b.BarSeriesId == seriesId)
                .Where(b => b.DT > date1 && b.DT < date2)
                .AsNoTracking()
                .OrderBy(b => b.Id)
                .Select(b => new BarDto
                {
                    SeriesId = b.BarSeriesId,
                    DT = b.DT,
                    Open = b.Open,
                    High = b.High,
                    Low = b.Low,
                    Close = b.Close,
                    Volume = b.Volume,
                }))
                ;
            return bs;
        }

        public async Task<IEnumerable<BarDto>> GetBarsDtoAsync(long? seriesId, int date)
        {
            // var ret = new List<BarDto>();
            if (!seriesId.HasValue || seriesId == 0)
                return null;
            //return ret;

            var date1 = date.IntToDate();
            var date2 = date1.AddDays(1);

            var bs = await GetBarsDtoAsync(seriesId, date1, date2);
            return bs;
        }

        public async Task<IEnumerable<BarDto>> GetBarsDtoAsync(long? seriesId, DateTime dt1, DateTime dt2)
        {
            // var ret = new List<BarDto>();
            if (!seriesId.HasValue || seriesId == 0)
                return null;
                //return ret;

            var date1 = dt1.MinValueToSql();
            var date2 = dt2.MinValueToSql();

            var bs = await (Bars
                .Where(b => b.BarSeriesId == seriesId)
                .Where(b => b.DT > date1 && b.DT < date2)
                .AsNoTracking()
                .OrderBy(b => b.Id)
                .Select(b => new BarDto
                {
                    SeriesId = b.BarSeriesId,
                    DT = b.DT,
                    Open = b.Open,
                    High = b.High,
                    Low = b.Low,
                    Close = b.Close,
                    Volume = b.Volume,
                })).ToListAsync();
            return bs;
        }

        public async Task<IEnumerable<string>> GetBarStrArrAsync(long? seriesId, int date)
        {
            // var ret = new List<BarDto>();
            if (!seriesId.HasValue || seriesId == 0)
                return null;
            //return ret;

            var date1 = date.IntToDate();
            var date2 = date1.AddDays(1);

            var bs = await GetBarsDtoAsync(seriesId, date1, date2);
            return bs.Select(BarToString).ToList();
        }
        //public async Task<IEnumerable<string>> GetBarStrArrAsync(long? seriesId, DateTime dt1, DateTime dt2)
        //{
        //    // var ret = new List<BarDto>();
        //    if (!seriesId.HasValue || seriesId == 0)
        //        return null;
        //    //return ret;

        //    //var date1 = dt1.MinValueToSql();
        //    //var date2 = dt2.MinValueToSql();

        //    //var bs = await (Bars
        //    //    .Where(b => b.BarSeriesId == seriesId)
        //    //    .Where(b => b.DT > date1 && b.DT < date2)
        //    //    .OrderBy(b => b.Id)
        //    //    .Select(b => BarToString(b))).ToListAsync();
        //    //return bs;
        //}
        public void InsertBars(long seriesId, IEnumerable<IBarBase> bars)
        {
            var srs = TimeSeries.Find(seriesId);
            if (srs == null)
                throw new Exception(String.Format("TimeSeriesId: {0} is Not Found", seriesId));

            var bs = bars.Select(b => new Bar
            {
                DT = b.DT,
                BarSeriesId = srs.Id,
                Open = b.Open,
                High = b.High,
                Low = b.Low,
                Close = b.Close,
                Volume = b.Volume
            });

            InsertBars(bs);
        }

        private void InsertBars(IEnumerable<Bar> bars)
        {
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    this.BulkInsert(bars);
                    SaveChanges();
                    transactionScope.Complete();
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Join(" ",
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    e.Message));
            }
        }
        private string BarToString(BarDto b)
        {
            if (b == null) throw new ArgumentNullException("b");
            if (b.Open == b.High && b.Open == b.Low && b.Open == b.Close)
            {
                return String.Format("{0},{1},{2}",
                    ((DateTime) b.DT).ToLongInSec(),
                    b.Open,
                    b.Volume);
            }
            if (b.Open != b.Close &&
                     (b.Open == b.Low || b.Open == b.High) &&
                     (b.Close == b.High || b.Close == b.Low)
                )
            {
                return String.Format("{0},{1},{2},{3}",
                    ((DateTime) b.DT).ToLongInSec(),
                    b.Open,
                    b.Close,
                    b.Volume
                    );
            }
            return String.Format("{0},{1},{2},{3},{4},{5}",
                ((DateTime) b.DT).ToLongInSec(),
                b.Open,
                b.High,
                b.Low,
                b.Close,
                b.Volume
                );
            
        }      
    }
}
