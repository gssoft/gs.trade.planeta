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

namespace WebEventLog_021.Controllers
{
    public class EventLogsController : ApiController
    {
        private EvlContext db = new EvlContext();

        // GET: api/EventLogs
        public IQueryable<DbEventLog> GetEventLogs()
        {
            return db.EventLogs;
        }

        // GET: api/EventLogs/5
        [ResponseType(typeof(DbEventLog))]
        public async Task<IHttpActionResult> GetEventLog(int id)
        {
            DbEventLog dbEventLog = await db.EventLogs.FindAsync(id);
            if (dbEventLog == null)
            {
                return NotFound();
            }

            return Ok(dbEventLog);
        }

        // PUT: api/EventLogs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutEventLog(int id, DbEventLog dbEventLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dbEventLog.EventLogID)
            {
                return BadRequest();
            }

            db.Entry(dbEventLog).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventLogExists(id))
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

        // POST: api/EventLogs
        [ResponseType(typeof(DbEventLog))]
        public async Task<IHttpActionResult> PostEventLog(DbEventLog dbEventLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.EventLogs.Add(dbEventLog);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = dbEventLog.EventLogID }, dbEventLog);
        }

        // DELETE: api/EventLogs/5
        [ResponseType(typeof(DbEventLog))]
        public async Task<IHttpActionResult> DeleteEventLog(int id)
        {
            DbEventLog dbEventLog = await db.EventLogs.FindAsync(id);
            if (dbEventLog == null)
            {
                return NotFound();
            }

            db.EventLogs.Remove(dbEventLog);
            await db.SaveChangesAsync();

            return Ok(dbEventLog);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EventLogExists(int id)
        {
            return db.EventLogs.Count(e => e.EventLogID == id) > 0;
        }
    }
}