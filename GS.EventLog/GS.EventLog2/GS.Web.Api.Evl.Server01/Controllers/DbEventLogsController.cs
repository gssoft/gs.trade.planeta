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
using GS.EventLog.DataBase1.Dal;
using GS.EventLog.DataBase1.Model;

namespace GS.Web.Api.Evl.Server01.Controllers
{
    public class DbEventLogsController : ApiController
    {
        private EvlContext1 db = new EvlContext1();

        // GET: api/DbEventLogs
        public IQueryable<DbEventLog> GetEventLogs()
        {
            return db.EventLogs;
        }

        // GET: api/DbEventLogs/5
        [ResponseType(typeof(DbEventLog))]
        public IHttpActionResult GetDbEventLog(int id)
        {
            DbEventLog dbEventLog = db.EventLogs.Find(id);
            if (dbEventLog == null)
            {
                return NotFound();
            }

            return Ok(dbEventLog);
        }

        // PUT: api/DbEventLogs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDbEventLog(int id, DbEventLog dbEventLog)
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
                db.SaveChanges();
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
        public IHttpActionResult PostDbEventLog(DbEventLog dbEventLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.EventLogs.Add(dbEventLog);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = dbEventLog.EventLogID }, dbEventLog);
        }

        // DELETE: api/DbEventLogs/5
        [ResponseType(typeof(DbEventLog))]
        public IHttpActionResult DeleteDbEventLog(int id)
        {
            DbEventLog dbEventLog = db.EventLogs.Find(id);
            if (dbEventLog == null)
            {
                return NotFound();
            }

            db.EventLogs.Remove(dbEventLog);
            db.SaveChanges();

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