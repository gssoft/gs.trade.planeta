using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GS.Applications;
using GS.EventLog.DataBase1.Dal;
using GS.EventLog.DataBase1.Model;

namespace GS.Web.Mvc.Evl.Server01.Controllers
{
    [Authorize]
    public class EventLogsController : Controller
    {
        private EvlContext1 db = new EvlContext1();

        // GET: EventLogs
        public ActionResult Index()
        {
            return View(db.EventLogs.ToList());
        }

        // GET: EventLogs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DbEventLog dbEventLog = db.EventLogs.Find(id);
            if (dbEventLog == null)
            {
                return HttpNotFound();
            }
            return View(dbEventLog);
        }

        // GET: EventLogs/Create
        public ActionResult Create()
        {
            ViewBag.ApplicationID =
                    new SelectList(GSApplications.GetAppsByUserName(User.Identity.Name), "ID", "Name");
            return View();
        }

        // POST: EventLogs/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventLogID,ApplicationID,Code,Name,Description")] DbEventLog dbEventLog)
        {
            if (ModelState.IsValid)
            {
                db.EventLogs.Add(dbEventLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dbEventLog);
        }

        // GET: EventLogs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DbEventLog dbEventLog = db.EventLogs.Find(id);
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
        public ActionResult Edit([Bind(Include = "EventLogID,ApplicationID,Alias,Code,Name,Description,ModifiedDT")] DbEventLog dbEventLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dbEventLog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dbEventLog);
        }

        // GET: EventLogs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DbEventLog dbEventLog = db.EventLogs.Find(id);
            if (dbEventLog == null)
            {
                return HttpNotFound();
            }
            return View(dbEventLog);
        }

        // POST: EventLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DbEventLog dbEventLog = db.EventLogs.Find(id);
            db.EventLogs.Remove(dbEventLog);
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
