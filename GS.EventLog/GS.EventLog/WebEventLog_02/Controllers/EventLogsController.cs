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
using GS;
using GS.Extension;

namespace WebEventLog_02.Controllers
{
    using WorkUnit;
    public class EventLogsController : ApiController
    {
       // private EvlContext db = new EvlContext();
        private readonly WorkUnit _unit = new WorkUnit();

        // GET api/EventLogs
        public IEnumerable<DbEventLog> GetDbEventLogs()
        {
            // return db.EventLogs.AsEnumerable();
           // var evls = _unit.EventLogs.Find(x => x.Name == "Gizmo").ToList();

            return _unit.EventLogs.GetAll();
        }
       

        // GET api/EventLogs/5
        public DbEventLog GetDbEventLog(int id)
        {
            if (id != 0)
            {
                // DbEventLog dbeventlog = db.EventLogs.Find(id);
                var dbeventlog = _unit.EventLogs.GetById(id);
                if (dbeventlog == null)
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }
                return dbeventlog;
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                //var evl = _unit.EventLogs.Find(x => x.Name == "Gizmo").First();
                //if (evl == null)
                //{
                //    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                //}
                //return evl;
            }
            
        }

        public long GetDbEventLogsByPar(string par)
        {
            if (par.HasNoValue())
                return -1;
            switch (par.Trim().ToUpper())
            {
                case "COUNT":
                    return _unit.EventLogs.Count();
                default:
                    return -1;
            }
            
            // return _unit.EventLogs.GetAll();
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

           // db.Entry(dbeventlog).State = EntityState.Modified;
            _unit.EventLogs.Update(dbeventlog);
            /*
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
            */
             
            try
            {
                _unit.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
            /*
            Exception ex;
            var r = _unit.SaveChanges(out ex);
            switch (r)
            {
                case -6:
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
            */ 
        }

        // POST api/EventLogs
        public HttpResponseMessage PostDbEventLog(DbEventLog dbeventlog)
        {
            if (ModelState.IsValid)
            {
               // db.EventLogs.Add(dbeventlog);
               // db.SaveChanges();

                var evl = _unit.EventLogs.Find(x => x.Name == dbeventlog.Name);

                if (evl == null)
                {
                    _unit.EventLogs.Add(dbeventlog);
                    _unit.SaveChanges();

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, dbeventlog);
                    response.Headers.Location = new Uri(Url.Link("DefaultApi", new {id = dbeventlog.EventLogID}));
                    return response;
                }
                else
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, evl);
                    //response.Headers.ETag =  evl.EventLogID.ToString();
                    response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = evl.EventLogID }));
                    return response;
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/EventLogs/5
        public HttpResponseMessage DeleteDbEventLog(int id)
        {
           // DbEventLog dbeventlog = db.EventLogs.Find(id);
            var dbeventlog = _unit.EventLogs.GetById(id);
            if (dbeventlog == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            //db.EventLogs.Remove(dbeventlog);
            _unit.EventLogs.Delete(dbeventlog);

            try
            {
                _unit.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, dbeventlog);
        }

        protected override void Dispose(bool disposing)
        {
           // db.Dispose();
            _unit.Dispose();
            base.Dispose(disposing);
        }
    }
}