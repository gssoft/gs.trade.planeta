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
    public class DbEventLogItemsController : ApiController
    {
        private EvlContext1 db = new EvlContext1();

        // GET: api/DbEventLogItems
        public IQueryable<DbEventLogItem> GetEventLogItems()
        {
            return db.EventLogItems;
        }

        // GET: api/DbEventLogItems/5
        [ResponseType(typeof(DbEventLogItem))]
        public IHttpActionResult GetDbEventLogItem(long id)
        {
            DbEventLogItem dbEventLogItem = db.EventLogItems.Find(id);
            if (dbEventLogItem == null)
            {
                return NotFound();
            }

            return Ok(dbEventLogItem);
        }

        // PUT: api/DbEventLogItems/5
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

            db.Entry(dbEventLogItem).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
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

        // POST: api/DbEventLogItems
        [ResponseType(typeof(DbEventLogItem))]
        public IHttpActionResult PostDbEventLogItem(DbEventLogItem dbEventLogItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.EventLogItems.Add(dbEventLogItem);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = dbEventLogItem.EventLogItemID }, dbEventLogItem);
        }

        // DELETE: api/DbEventLogItems/5
        [ResponseType(typeof(DbEventLogItem))]
        public IHttpActionResult DeleteDbEventLogItem(long id)
        {
            DbEventLogItem dbEventLogItem = db.EventLogItems.Find(id);
            if (dbEventLogItem == null)
            {
                return NotFound();
            }

            db.EventLogItems.Remove(dbEventLogItem);
            db.SaveChanges();

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