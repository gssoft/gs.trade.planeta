using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GS.EventLog.DataBase;
using GS.EventLog.DataBase.Model;
using GS.EventLog.DataBase.Dal;

namespace EvlMvcApp_01.Controllers
{
    public class EventLogsController : Controller
    {
      //  private EvlContext db = new EvlContext();
        private UnitOfWork _unit = new UnitOfWork();

        //
        // GET: /EventLogs/

        public ActionResult Index()
        {
          //  var v = db.EventLogs.ToList().AsEnumerable();
            var v = _unit.EventLogs.GetAll();
            return View(v);
        }

        //
        // GET: /EventLogs/Details/5

        public ActionResult Details(int id = 0)
        {
            // DbEventLog dbeventlog = db.EventLogs.Find(id);

            var dbeventlog = _unit.EventLogs.GetById(id);

            if (dbeventlog == null)
            {
                return HttpNotFound();
            }
            return View(dbeventlog);
        }

        //
        // GET: /EventLogs/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /EventLogs/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DbEventLog dbeventlog)
        {
            if (ModelState.IsValid)
            {
              //  db.EventLogs.Add(dbeventlog);
              //  db.SaveChanges();
                
                _unit.EventLogs.Add(dbeventlog);
                _unit.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(dbeventlog);
        }

        //
        // GET: /EventLogs/Edit/5

        public ActionResult Edit(int id = 0)
        {
           // DbEventLog dbeventlog = db.EventLogs.Find(id);
            var dbeventlog = _unit.EventLogs.GetById(id);
            if (dbeventlog == null)
            {
                return HttpNotFound();
            }
            return View(dbeventlog);
        }

        //
        // POST: /EventLogs/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DbEventLog dbeventlog)
        {
            if (ModelState.IsValid)
            {
            //    db.Entry(dbeventlog).State = EntityState.Modified;
            //    db.SaveChanges();
                _unit.EventLogs.Update(dbeventlog);
                _unit.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(dbeventlog);
        }

        //
        // GET: /EventLogs/Delete/5

        public ActionResult Delete(int id = 0)
        {
            // DbEventLog dbeventlog = db.EventLogs.Find(id);

            var dbeventlog = _unit.EventLogs.GetById(id);

            if (dbeventlog == null)
            {
                return HttpNotFound();
            }
            return View(dbeventlog);
        }

        //
        // POST: /EventLogs/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //DbEventLog dbeventlog = db.EventLogs.Find(id);
            //db.EventLogs.Remove(dbeventlog);
            //db.SaveChanges();
            var dbeventlog = _unit.EventLogs.GetById(id);
            _unit.EventLogs.Delete(dbeventlog);
            _unit.SaveChanges();


            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
          //  db.Dispose();
            _unit.Dispose();
            base.Dispose(disposing);
        }
    }
}