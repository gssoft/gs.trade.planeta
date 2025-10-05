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

namespace WebEventLog_01.Controllers
{
    public class ApiEventLogsController : ApiController
    {
        private EvlContext db = new EvlContext();

        // GET: api/ApiEventLogs
        public IQueryable<DbEventLog> GetEventLogs()
        {
            return db.EventLogs;
        }

        // GET: api/ApiEventLogs/5
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
        // GET: api/ApiEventLogs/?code=Code
        [ResponseType(typeof(DbEventLog))]
        public async Task<IHttpActionResult> GetDbEventLogByCode(string code)
        {
            DbEventLog dbEventLog = await db.EventLogs.FirstOrDefaultAsync(evl=>evl.Code == code);
            if (dbEventLog == null)
            {
                return NotFound();
            }

            return Ok(dbEventLog);
        }
        // GET: api/ApiEventLogs/?name=Name
        [ResponseType(typeof(DbEventLog))]
        public async Task<IHttpActionResult> GetDbEventLogByName(string name)
        {
            DbEventLog dbEventLog = await db.EventLogs.FirstOrDefaultAsync(evl => evl.Name == name);
            if (dbEventLog == null)
            {
                return NotFound();
            }

            return Ok(dbEventLog);
        }

        // PUT: api/ApiEventLogs/5
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

        // POST: api/ApiEventLogs
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

        // DELETE: api/ApiEventLogs/5
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