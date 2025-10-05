using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using GS.Extension;
using GS.Trade.TimeSeries.Model;
using GS.Trade.TimeSeries.Model.View;

namespace GS.Trade.Web.Api.TimeSeries01.Controllers
{
    using BarDto = GS.Trade.Dto.Bar;
    using GS.Trade.TimeSeries.Model;
    public class BarsController : ApiController
    {
        //2018.05.14
        // private const int MaxBarGetValue = 30000;
        private const int MaxBarGetValue = 5000;
        private const int MaxBarsGetDaysNumber = 30;

        private readonly TimeSeriesContext _db = new TimeSeriesContext();

        // GET: api/Bars
       
        public IQueryable<BarDto> GetBars()
        {
            // return db.Bars;
            var bs = (_db.Bars
                        .AsNoTracking()
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
        /// <summary>
        /// with MaxBarGetValue
        /// </summary>
        /// <returns></returns>
        public IQueryable<BarDto> GetBars(long? seriesId)
        {
            // return db.Bars;
            if (!seriesId.HasValue || seriesId == 0)
                return null;
            var bs = (_db.Bars
                      .Where(b=>b.BarSeriesId == seriesId)
                      .AsNoTracking()
                      .OrderByDescending(b=>b.Id)
                      .Take(MaxBarGetValue)
                      .OrderBy(b=>b.Id)
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
        public IQueryable<BarDto> GetBars(long? seriesId, int date)
        {
            var bs = _db.GetBarsDto(seriesId, date);
            return bs;
        }
        public IQueryable<BarDto> GetBars(long? seriesId, int dt1, int dt2)
        {
            // return db.Bars;
            var date1 = dt1.IntToDate();
            var date2 = dt2.IntToDate();

            if (!seriesId.HasValue || seriesId == 0)
                return null;
            var bs = (_db.Bars
                      .Where(b => b.BarSeriesId == seriesId)
                      .Where(b => b.DT > date1 && b.DT < date2)
                //.OrderByDescending(b => b.Id)
                //.Take(MaxBarGetValue)
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

        /// <summary>
        /// With MAxMarsDaysNumber
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="timeInt"></param>
        /// <returns></returns>
        public IQueryable<BarDto> GetBars(string ticker, int? timeInt)
        {
            var tms = _db.GetBarSeries(ticker, timeInt);
            if (tms == null)
                return null;
            var dts = _db.GetBarsLastFirstDates(tms.Id, MaxBarsGetDaysNumber);
            var dt1 = dts[0];
            var dt2 = dts[1];
            var bs = (_db.Bars
                      .Where(b => b.BarSeriesId == tms.Id && b.DT > dt1 && b.DT < dt2)
                      //.Where(b => b.DT > dts[0] && b.DT < dts[1])
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
                      }));

            //var bs = (db.Bars
            //          .Where(b => b.BarSeriesId == tms.Id)
            //          .OrderByDescending(b => b.Id)
            //          .Take(MaxBarGetValue)
            //          .OrderBy(b => b.Id)
            //          .Select(b => new BarDto
            //          {
            //              SeriesId = b.BarSeriesId,
            //              DT = b.DT,
            //              Open = b.Open,
            //              High = b.High,
            //              Low = b.Low,
            //              Close = b.Close,
            //              Volume = b.Volume,
            //          }))
            //;
            return bs;
        }

        public IQueryable<BarDto> GetBars(string ticker, int? timeInt, int date)
        {
            var dt1 = date.IntToDate();
            var dt2 = dt1.AddDays(1);

            var tms = _db.GetBarSeries(ticker, timeInt);
            if (tms == null)
                return null;
            // var dts = db.GetBarsLastFirstDates(tms.Id, MaxBarsGetDaysNumber);
            // var dt1 = dts[0];
            // var dt2 = dts[1];
            var bs = (_db.Bars
                      .Where(b => b.BarSeriesId == tms.Id && b.DT > dt1 && b.DT < dt2)
                //.Where(b => b.DT > dts[0] && b.DT < dts[1])
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
                      }));

            //var bs = (db.Bars
            //          .Where(b => b.BarSeriesId == tms.Id)
            //          .OrderByDescending(b => b.Id)
            //          .Take(MaxBarGetValue)
            //          .OrderBy(b => b.Id)
            //          .Select(b => new BarDto
            //          {
            //              SeriesId = b.BarSeriesId,
            //              DT = b.DT,
            //              Open = b.Open,
            //              High = b.High,
            //              Low = b.Low,
            //              Close = b.Close,
            //              Volume = b.Volume,
            //          }))
            //;
            return bs;
        }

        public IQueryable<BarDto> GetBars(string ticker, int? timeInt, int dt1, int dt2)
        {
            var date1 = dt1.IntToDate();
            var date2 = dt2.IntToDate();

            var tms = _db.GetBarSeries(ticker, timeInt);
            if (tms == null)
                return null;
            // var dts = db.GetBarsLastFirstDates(tms.Id, MaxBarsGetDaysNumber);
            // var dt1 = dts[0];
            // var dt2 = dts[1];
            var bs = (_db.Bars
                      .Where(b => b.BarSeriesId == tms.Id && b.DT > date1 && b.DT < date2)
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
                      }));
            return bs;
        }       


        // GET: api/Bars/5
        [ResponseType(typeof(Bar))]
        public IHttpActionResult GetBar(long id)
        {
            Bar bar = _db.Bars.Find(id);
            if (bar == null)
            {
                return NotFound();
            }

            return Ok(bar);
        }

        // PUT: api/Bars/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBar(long id, Bar bar)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bar.Id)
            {
                return BadRequest();
            }

            _db.Entry(bar).State = EntityState.Modified;

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BarExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Bars
        [ResponseType(typeof(Bar))]
        public IHttpActionResult PostBar(Bar bar)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Bars.Add(bar);
            _db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = bar.Id }, bar);
        }

        // DELETE: api/Bars/5
        [ResponseType(typeof(Bar))]
        public IHttpActionResult DeleteBar(long id)
        {
            Bar bar = _db.Bars.Find(id);
            if (bar == null)
            {
                return NotFound();
            }

            _db.Bars.Remove(bar);
            _db.SaveChanges();

            return Ok(bar);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BarExists(long id)
        {
            return _db.Bars.Count(e => e.Id == id) > 0;
        }
    }
}