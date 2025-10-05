using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GS.Trade.DataBase.Series.Model;
using GS.Trade.DataBase.Series.Dal;

namespace Series_Mvc01.Controllers
{
    public class TimeIntsController : Controller
    {
        private SeriesContext db = new SeriesContext();

        //
        // GET: /TimeInts/

        public ActionResult Index()
        {
            return View(db.TimeInts.ToList());
        }

        //
        // GET: /TimeInts/Details/5

        public ActionResult Details(long id = 0)
        {
            TimeInt timeint = db.TimeInts.Find(id);
            if (timeint == null)
            {
                return HttpNotFound();
            }
            return View(timeint);
        }

        //
        // GET: /TimeInts/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /TimeInts/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TimeInt timeint)
        {
            if (ModelState.IsValid)
            {
                db.TimeInts.Add(timeint);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(timeint);
        }

        //
        // GET: /TimeInts/Edit/5

        public ActionResult Edit(long id = 0)
        {
            TimeInt timeint = db.TimeInts.Find(id);
            if (timeint == null)
            {
                return HttpNotFound();
            }
            return View(timeint);
        }

        //
        // POST: /TimeInts/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TimeInt timeint)
        {
            if (ModelState.IsValid)
            {
                db.Entry(timeint).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(timeint);
        }

        //
        // GET: /TimeInts/Delete/5

        public ActionResult Delete(long id = 0)
        {
            TimeInt timeint = db.TimeInts.Find(id);
            if (timeint == null)
            {
                return HttpNotFound();
            }
            return View(timeint);
        }

        //
        // POST: /TimeInts/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            TimeInt timeint = db.TimeInts.Find(id);
            db.TimeInts.Remove(timeint);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}