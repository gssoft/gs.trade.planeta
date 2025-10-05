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
using GS.EventLog.DataBase.Dal;
using GS.EventLog.DataBase.Model;
using EvlContext = GS.EventLog.DataBase.Dal.EvlContext;

namespace WebEventLog_02.Controllers
{
    public class EventLogItemsAsyncController : ApiController
    {
        private EvlContext db = new EvlContext();

        // GET: api/EventLogItemsAsync
        public async Task<IEnumerable<DbEventLogItem>> GetEventLogItems()
        {
            var v = await db.EventLogItems.Take(10000).ToListAsync();
            return v;
        }

        // GET: api/EventLogItemsAsync/5
        [ResponseType(typeof(DbEventLogItem))]
        public async Task<IHttpActionResult> GetDbEventLogItem(long id)
        {
            DbEventLogItem dbEventLogItem = await db.EventLogItems.FindAsync(id);
            if (dbEventLogItem == null)
            {
                return NotFound();
            }

            return Ok(dbEventLogItem);
        }

        // PUT: api/EventLogItemsAsync/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDbEventLogItem(long id, DbEventLogItem dbEventLogItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dbEventLogItem.EventLogItemID)
            {
                return BadRequest();
            }

            db.Entry(dbEventLogItem).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DbEventLogItemExists(id))
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

        // POST: api/EventLogItemsAsync
        [ResponseType(typeof(DbEventLogItem))]
        public async Task<IHttpActionResult> PostDbEventLogItem(DbEventLogItem dbEventLogItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.EventLogItems.Add(dbEventLogItem);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = dbEventLogItem.EventLogItemID }, dbEventLogItem);
        }

        // DELETE: api/EventLogItemsAsync/5
        [ResponseType(typeof(DbEventLogItem))]
        public async Task<IHttpActionResult> DeleteDbEventLogItem(long id)
        {
            DbEventLogItem dbEventLogItem = await db.EventLogItems.FindAsync(id);
            if (dbEventLogItem == null)
            {
                return NotFound();
            }

            db.EventLogItems.Remove(dbEventLogItem);
            await db.SaveChangesAsync();

            return Ok(dbEventLogItem);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DbEventLogItemExists(long id)
        {
            return db.EventLogItems.Count(e => e.EventLogItemID == id) > 0;
        }
    }
}