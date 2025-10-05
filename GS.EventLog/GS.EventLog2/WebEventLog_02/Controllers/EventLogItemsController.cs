using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using GS.EventLog.DataBase.Dal;
using GS.EventLog.DataBase.Model;
using GS.Extension;
using EvlContext = GS.EventLog.DataBase.Dal.EvlContext;

namespace WebEventLog_02.Controllers
{
    public class EventLogItemsController : ApiController
    {
        private readonly EvlContext _db = new EvlContext();

        // GET: api/EventLogItems
        public IQueryable<DbEventLogItem> GetEventLogItems()
        {
            var eis = _db.EventLogItems.Take(10000);
            return eis;
        }
        // GET api/EventLogsItems?par=count
        public long GetDbEventLogItemsByPar(string par)
        {
            if (par.HasNoValue())
                return -1;
            switch (par.Trim().ToUpper())
            {
                case "COUNT":
                    return _db.EventLogItems.Count();
                default:
                    return -1;
            }
        }

        // GET: api/EventLogItems/5
        [ResponseType(typeof(DbEventLogItem))]
        public IHttpActionResult GetDbEventLogItem(long id)
        {
            DbEventLogItem dbEventLogItem = _db.EventLogItems.Find(id);
            if (dbEventLogItem == null)
            {
                return NotFound();
            }

            return Ok(dbEventLogItem);
        }

        // PUT: api/EventLogItems/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDbEventLogItem(long id, DbEventLogItem dbEventLogItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dbEventLogItem.EventLogItemID)
            {
                return BadRequest();
            }

            _db.Entry(dbEventLogItem).State = EntityState.Modified;

            try
            {
                _db.SaveChanges();
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

        // POST: api/EventLogItems
        [ResponseType(typeof(DbEventLogItem))]
        public IHttpActionResult PostDbEventLogItem(DbEventLogItem dbEventLogItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.EventLogItems.Add(dbEventLogItem);
            _db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = dbEventLogItem.EventLogItemID }, dbEventLogItem);
        }

        // DELETE: api/EventLogItems/5
        [ResponseType(typeof(DbEventLogItem))]
        public IHttpActionResult DeleteDbEventLogItem(long id)
        {
            DbEventLogItem dbEventLogItem = _db.EventLogItems.Find(id);
            if (dbEventLogItem == null)
            {
                return NotFound();
            }

            _db.EventLogItems.Remove(dbEventLogItem);
            _db.SaveChanges();

            return Ok(dbEventLogItem);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DbEventLogItemExists(long id)
        {
            return _db.EventLogItems.Count(e => e.EventLogItemID == id) > 0;
        }
    }
}