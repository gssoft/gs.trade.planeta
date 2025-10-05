using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using GS.EventLog.DataBase.Dal;
using GS.EventLog.DataBase.Model;
using EvlContext = GS.EventLog.DataBase.Dal.EvlContext;

namespace WebEventLog_02.Controllers
{
    /*
    Для класса WebApiConfig может понадобиться внесение дополнительных изменений,
     * чтобы добавить маршрут в этот контроллер. 
     * Объедините эти инструкции в методе Register класса WebApiConfig соответствующим образом.
     * Обратите внимание, что в URL-адресах OData учитывается регистр символов.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using GS.EventLog.DataBase.Model;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<DbEventLogItem>("OdataEventLogItems");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class OdataEventLogItemsController : ODataController
    {
        private EvlContext db = new EvlContext();

        // GET: odata/OdataEventLogItems
        [EnableQuery]
        public IQueryable<DbEventLogItem> GetOdataEventLogItems()
        {
            //return db.EventLogItems.Take(100);
            return db.EventLogItems;
        }

        // GET: odata/OdataEventLogItems(5)
        [EnableQuery]
        public SingleResult<DbEventLogItem> GetDbEventLogItem([FromODataUri] long key)
        {
            return SingleResult.Create(db.EventLogItems.Where(dbEventLogItem => dbEventLogItem.EventLogItemID == key));
        }

        // PUT: odata/OdataEventLogItems(5)
        public IHttpActionResult Put([FromODataUri] long key, Delta<DbEventLogItem> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DbEventLogItem dbEventLogItem = db.EventLogItems.Find(key);
            if (dbEventLogItem == null)
            {
                return NotFound();
            }

            patch.Put(dbEventLogItem);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DbEventLogItemExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(dbEventLogItem);
        }

        // POST: odata/OdataEventLogItems
        public IHttpActionResult Post(DbEventLogItem dbEventLogItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.EventLogItems.Add(dbEventLogItem);
            db.SaveChanges();

            return Created(dbEventLogItem);
        }

        // PATCH: odata/OdataEventLogItems(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] long key, Delta<DbEventLogItem> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DbEventLogItem dbEventLogItem = db.EventLogItems.Find(key);
            if (dbEventLogItem == null)
            {
                return NotFound();
            }

            patch.Patch(dbEventLogItem);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DbEventLogItemExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(dbEventLogItem);
        }

        // DELETE: odata/OdataEventLogItems(5)
        public IHttpActionResult Delete([FromODataUri] long key)
        {
            DbEventLogItem dbEventLogItem = db.EventLogItems.Find(key);
            if (dbEventLogItem == null)
            {
                return NotFound();
            }

            db.EventLogItems.Remove(dbEventLogItem);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DbEventLogItemExists(long key)
        {
            return db.EventLogItems.Count(e => e.EventLogItemID == key) > 0;
        }
    }
}
