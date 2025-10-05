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
    public class EventLogItemsController : Controller
    {
      // private EvlContext db = new EvlContext();
        private UnitOfWork _unit = new UnitOfWork();
        //
        // GET: /EventLogItems/

        public ActionResult Index()
        {
           // return View(db.EventLogItems.ToList());
            var v = _unit.EventLogItems.GetAll();
            return View(v);
        }

        //
        // GET: /EventLogItems/Details/5

        public ActionResult Details(long id = 0)
        {
          //  DbEventLogItem dbeventlogitem = db.EventLogItems.Find(id);
            var dbeventlogitem = _unit.EventLogItems.GetById(id);
            if (dbeventlogitem == null)
            {
                return HttpNotFound();
            }
            return View(dbeventlogitem);
        }

        //
        // GET: /EventLogItems/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /EventLogItems/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DbEventLogItem dbeventlogitem)
        {
            if (ModelState.IsValid)
            {
                //db.EventLogItems.Add(dbeventlogitem);
                //db.SaveChanges();
                _unit.EventLogItems.Add(dbeventlogitem);
                _unit.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(dbeventlogitem);
        }

        //
        // GET: /EventLogItems/Edit/5

        public ActionResult Edit(long id = 0)
        {
           // DbEventLogItem dbeventlogitem = db.EventLogItems.Find(id);
            var dbeventlogitem = _unit.EventLogItems.GetById(id);
            if (dbeventlogitem == null)
            {
                return HttpNotFound();
            }
            return View(dbeventlogitem);
        }

        //
        // POST: /EventLogItems/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DbEventLogItem dbeventlogitem)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(dbeventlogitem).State = EntityState.Modified;
                //db.SaveChanges();

                _unit.EventLogItems.Update(dbeventlogitem);
                _unit.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(dbeventlogitem);
        }

        //
        // GET: /EventLogItems/Delete/5

        public ActionResult Delete(long id = 0)
        {
           // DbEventLogItem dbeventlogitem = db.EventLogItems.Find(id);

            var dbeventlogitem = _unit.EventLogItems.GetById(id);
            if (dbeventlogitem == null)
            {
                return HttpNotFound();
            }
            return View(dbeventlogitem);
        }

        //
        // POST: /EventLogItems/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            //DbEventLogItem dbeventlogitem = db.EventLogItems.Find(id);
            //db.EventLogItems.Remove(dbeventlogitem);
            //db.SaveChanges();

            var dbeventlog = _unit.EventLogItems.GetById(id);
            _unit.EventLogItems.Delete(dbeventlog);
            _unit.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
           // db.Dispose();
            _unit.Dispose();
            base.Dispose(disposing);
        }
    }
}