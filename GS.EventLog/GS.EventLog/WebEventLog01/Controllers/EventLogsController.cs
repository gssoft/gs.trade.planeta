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
using GS.EventLog.DataBase.Model;
using GS.EventLog.DataBase.Dal;

namespace WebEventLog01.Controllers
{
    public class EventLogsController : ApiController
    {
        private EvlContext db = new EvlContext();

        // GET api/EventLogs
        public IEnumerable<DbEventLog> GetDbEventLogs()
        {
           // return db.EventLogs.Include(s=>s.EventLogItems).AsEnumerable();
            return db.EventLogs.AsEnumerable();
        }

        // GET api/EventLogs/5
        public DbEventLog GetDbEventLog(int id)
        {
            DbEventLog dbeventlog = db.EventLogs.Find(id);
            if (dbeventlog == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return dbeventlog;
        }

        // PUT api/EventLogs/5
        public HttpResponseMessage PutDbEventLog(int id, DbEventLog dbeventlog)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != dbeventlog.EventLogID)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(dbeventlog).State = EntityState.Modified;

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

        // POST api/EventLogs
        public HttpResponseMessage PostDbEventLog(DbEventLog dbeventlog)
        {
            if (ModelState.IsValid)
            {
                db.EventLogs.Add(dbeventlog);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, dbeventlog);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = dbeventlog.EventLogID }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/EventLogs/5
        public HttpResponseMessage DeleteDbEventLog(int id)
        {
            DbEventLog dbeventlog = db.EventLogs.Find(id);
            if (dbeventlog == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.EventLogs.Remove(dbeventlog);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, dbeventlog);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}