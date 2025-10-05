using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using GS.Extension;
using GS.Trade.TimeSeries.Model;

namespace GS.Trade.Web.Api.TimeSeries01.Controllers
{
    using BarDto = GS.Trade.Dto.Bar;
    public class BarsAsyncController : ApiController
    {

        private readonly TimeSeriesContext _db = new TimeSeriesContext();

        // GET: api/BarsAsync
        public async Task<IEnumerable<Bar>> GetBars()
        {
            return await _db.Bars
                            .AsNoTracking()
                            .ToListAsync();
        }
       
        //public async Task<IEnumerable<BarDto>> GetBars(long? seriesId, int date)
        //{
        //    var bs = await _db.GetBarsDtoAsync(seriesId, date);
        //    return bs;
        //}
        public async Task<IEnumerable<string>> GetBars(long? seriesId, int date)
        {
            var bs = await _db.GetBarStrArrAsync(seriesId, date);
            return bs;
        }

        // GET: api/BarsAsync/5
        [ResponseType(typeof(Bar))]
        public async Task<IHttpActionResult> GetBar(long id)
        {
            Bar bar = await _db.Bars.FindAsync(id);
            if (bar == null)
            {
                return NotFound();
            }
            return Ok(bar);
        }

        // PUT: api/BarsAsync/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBar(long id, Bar bar)
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
                await _db.SaveChangesAsync();
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

        // POST: api/BarsAsync
        [ResponseType(typeof(Bar))]
        public async Task<IHttpActionResult> PostBar(Bar bar)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Bars.Add(bar);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = bar.Id }, bar);
        }

        // DELETE: api/BarsAsync/5
        [ResponseType(typeof(Bar))]
        public async Task<IHttpActionResult> DeleteBar(long id)
        {
            Bar bar = await _db.Bars.FindAsync(id);
            if (bar == null)
            {
                return NotFound();
            }

            _db.Bars.Remove(bar);
            await _db.SaveChangesAsync();

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
            return _db.Bars
                       // .AsNoTracking()
                        .Count(e => e.Id == id) > 0;
        }
    }
}