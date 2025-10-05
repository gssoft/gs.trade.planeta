using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GS.EventLog.DataBase1.Dal;
using GS.EventLog.DataBase1.Model;

namespace GS.Web.Mvc.Evl.Server01.Controllers
{
    public class EventLogItemsController : Controller
    {
        private EvlContext1 db = new EvlContext1();

        // GET: EventLogItems
        public ActionResult Index()
        {
            var eventLogItems = db.EventLogItems.Include(d => d.DbEventLog);
            return View(eventLogItems.ToList());
        }

        // GET: EventLogItems/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DbEventLogItem dbEventLogItem = db.EventLogItems.Find(id);
            if (dbEventLogItem == null)
            {
                return HttpNotFound();
            }
            return View(dbEventLogItem);
        }

        // GET: EventLogItems/Create
        public ActionResult Create()
        {
            ViewBag.EventLogID = new SelectList(db.EventLogs, "EventLogID", "Alias");
            return View();
        }

        // POST: EventLogItems/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventLogItemID,DT,ResultCode,Subject,Source,Entity,Operation,Description,Object,Index,EventLogID")] DbEventLogItem dbEventLogItem)
        {
            if (ModelState.IsValid)
            {
                db.EventLogItems.Add(dbEventLogItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EventLogID = new SelectList(db.EventLogs, "EventLogID", "Alias", dbEventLogItem.EventLogID);
            return View(dbEventLogItem);
        }

        // GET: EventLogItems/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DbEventLogItem dbEventLogItem = db.EventLogItems.Find(id);
            if (dbEventLogItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.EventLogID = new SelectList(db.EventLogs, "EventLogID", "Alias", dbEventLogItem.EventLogID);
            return View(dbEventLogItem);
        }

        // POST: EventLogItems/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventLogItemID,DT,ResultCode,Subject,Source,Entity,Operation,Description,Object,Index,EventLogID")] DbEventLogItem dbEventLogItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dbEventLogItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EventLogID = new SelectList(db.EventLogs, "EventLogID", "Alias", dbEventLogItem.EventLogID);
            return View(dbEventLogItem);
        }

        // GET: EventLogItems/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DbEventLogItem dbEventLogItem = db.EventLogItems.Find(id);
            if (dbEventLogItem == null)
            {
                return HttpNotFound();
            }
            return View(dbEventLogItem);
        }

        // POST: EventLogItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            DbEventLogItem dbEventLogItem = db.EventLogItems.Find(id);
            db.EventLogItems.Remove(dbEventLogItem);
            db.SaveChanges();
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
