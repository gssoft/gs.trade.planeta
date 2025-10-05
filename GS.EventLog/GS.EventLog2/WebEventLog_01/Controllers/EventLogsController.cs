using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GS.EventLog.DataBase.Dal;
using GS.EventLog.DataBase.Model;
using GS.EventLog.ViewModel;
using EvlContext = GS.EventLog.DataBase.Dal.EvlContext;

namespace WebEventLog_01.Controllers
{
    public class EventLogsController : Controller
    {
        private EvlContext db = new EvlContext();

        // GET: EventLogs
        public async Task<ActionResult> Index()
        {
            return View(await db.EventLogs.ToListAsync());
        }

        public ActionResult StatsSync()
        {
            var v1 = db.GetEventLogItemsGrouped(true, false, false ).ToList();
            var v2 = db.GetEventLogItemsGrouped(true, false, true ); // .ToList();
            var v3 = db.GetEventLogItemsGrouped(true, true, false ); // .ToList();
            var v4 = db.GetEventLogItemsGrouped(true, true, true );  // .ToList();

            v1.AddRange(v2);
            v1.AddRange(v3);
            v1.AddRange(v4);

            var v = v1.OrderBy(e => e.Id)
                .ThenBy(e => e.Result)
                .ThenBy(e => e.Subject)
                .ToList();
            
            return View("Stats", v );
        }
        #region StatsAsync
        //public async Task<ActionResult> StatsAsync()
        //{
        //    var v1 = await db.GetEventLogItemsGrouped(true, false, false).ToListAsync();
        //    var v2 = await db.GetEventLogItemsGrouped(true, false, true).ToListAsync();
        //    var v3 = await db.GetEventLogItemsGrouped(true, true, false).ToListAsync();
        //    var v4 = await db.GetEventLogItemsGrouped(true, true, true).ToListAsync();

        //    v1.AddRange(v2);
        //    v1.AddRange(v3);
        //    v1.AddRange(v4);

        //    var v = v1.OrderBy(e => e.Id)
        //        .ThenBy(e => e.Result)
        //        .ThenBy(e => e.Subject)
        //        .ToList();

        //    return View(v);
        //}
        #endregion


        // GET: EventLogs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DbEventLog dbEventLog = await db.EventLogs.FindAsync(id);
            if (dbEventLog == null)
            {
                return HttpNotFound();
            }
            return View(dbEventLog);
        }

        // GET: EventLogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EventLogs/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EventLogID,Alias,Code,Name,Description,LongCode")] DbEventLog dbEventLog)
        {
            if (ModelState.IsValid)
            {
                db.EventLogs.Add(dbEventLog);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(dbEventLog);
        }

        // GET: EventLogs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DbEventLog dbEventLog = await db.EventLogs.FindAsync(id);
            if (dbEventLog == null)
            {
                return HttpNotFound();
            }
            return View(dbEventLog);
        }

        // POST: EventLogs/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "EventLogID,Alias,Code,Name,Description,LongCode")] DbEventLog dbEventLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dbEventLog).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(dbEventLog);
        }

        // GET: EventLogs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DbEventLog dbEventLog = await db.EventLogs.FindAsync(id);
            if (dbEventLog == null)
            {
                return HttpNotFound();
            }
            return View(dbEventLog);
        }

        // POST: EventLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DbEventLog dbEventLog = await db.EventLogs.FindAsync(id);
            db.EventLogs.Remove(dbEventLog);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
