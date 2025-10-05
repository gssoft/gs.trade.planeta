using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using GS;
using GS.EventLog.DataBase.Model;
using GS.EventLog.DataBase.Dal;
using GS.Extension;

namespace WebEventLog_02.Controllers
{
    public class EventLogItemsController : ApiController
    {
        private EvlContext db = new EvlContext("EventLog");

        // GET api/EventLogItems
        public IEnumerable<DbEventLogItem> GetDbEventLogItems()
        {
            return db.EventLogItems.AsEnumerable();
        }

        // GET api/EventLogItems/5
        public DbEventLogItem GetDbEventLogItem(long id)
        {
            DbEventLogItem dbeventlogitem = db.EventLogItems.Find(id);
            if (dbeventlogitem == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return dbeventlogitem;
        }
        // GET api/EventLogsItems?par=count
        public long GetDbEventLogItemsByPar(string par)
        {
            if (par.HasNoValue())
                return -1;
            switch (par.Trim().ToUpper())
            {
                case "COUNT":
                    return db.EventLogItems.Count();
                default:
                    return -1;
            }
        }

        // PUT api/EventLogItems/5
        public HttpResponseMessage PutDbEventLogItem(long id, DbEventLogItem dbeventlogitem)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != dbeventlogitem.EventLogItemID)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(dbeventlogitem).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/EventLogItems
        public HttpResponseMessage PostDbEventLogItem(DbEventLogItem dbeventlogitem)
        {
            if (ModelState.IsValid)
            {
                db.EventLogItems.Add(dbeventlogitem);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, dbeventlogitem);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = dbeventlogitem.EventLogItemID }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/EventLogItems/5
        public HttpResponseMessage DeleteDbEventLogItem(long id)
        {
            DbEventLogItem dbeventlogitem = db.EventLogItems.Find(id);
            if (dbeventlogitem == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.EventLogItems.Remove(dbeventlogitem);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, dbeventlogitem);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}