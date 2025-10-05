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

namespace WebMvcEventLog_01.Controllers
{
    public class DbEventLogsController : ApiController
    {
        private EvlContext db = new EvlContext();

        // GET: api/DbEventLogs
        public IQueryable<DbEventLog> GetEventLogs()
        {
            return db.EventLogs;
        }

        // GET: api/DbEventLogs/5
        [ResponseType(typeof(DbEventLog))]
        public async Task<IHttpActionResult> GetDbEventLog(int id)
        {
            DbEventLog dbEventLog = await db.EventLogs.FindAsync(id);
            if (dbEventLog == null)
            {
                return NotFound();
            }

            return Ok(dbEventLog);
        }

        // PUT: api/DbEventLogs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDbEventLog(int id, DbEventLog dbEventLog)
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
                if (!DbEventLogExists(id))
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

        // POST: api/DbEventLogs
        [ResponseType(typeof(DbEventLog))]
        public async Task<IHttpActionResult> PostDbEventLog(DbEventLog dbEventLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.EventLogs.Add(dbEventLog);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = dbEventLog.EventLogID }, dbEventLog);
        }

        // DELETE: api/DbEventLogs/5
        [ResponseType(typeof(DbEventLog))]
        public async Task<IHttpActionResult> DeleteDbEventLog(int id)
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

        private bool DbEventLogExists(int id)
        {
            return db.EventLogs.Count(e => e.EventLogID == id) > 0;
        }
    }
}