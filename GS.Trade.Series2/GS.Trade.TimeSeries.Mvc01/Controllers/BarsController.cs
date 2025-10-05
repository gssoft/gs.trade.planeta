using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GS.Trade.TimeSeries.Model;

namespace GS.Trade.TimeSeries.Mvc01.Controllers
{
    public class BarsController : Controller
    {
        private TimeSeriesContext db = new TimeSeriesContext();

        //
        // GET: /Bars/

        public ActionResult Index()
        {
            var bars = db.Bars.Include(b => b.TimeSeries);
            return View(bars.ToList());
        }

        //
        // GET: /Bars/Details/5

        public ActionResult Details(long id = 0)
        {
            Bar bar = db.Bars.Find(id);
            if (bar == null)
            {
                return HttpNotFound();
            }
            return View(bar);
        }

        //
        // GET: /Bars/Create

        public ActionResult Create()
        {
            ViewBag.BarSeriesId = new SelectList(db.TimeSeries, "Id", "Code");
            return View();
        }

        //
        // POST: /Bars/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Bar bar)
        {
            if (ModelState.IsValid)
            {
                db.Bars.Add(bar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BarSeriesId = new SelectList(db.TimeSeries, "Id", "Code", bar.BarSeriesId);
            return View(bar);
        }

        //
        // GET: /Bars/Edit/5

        public ActionResult Edit(long id = 0)
        {
            Bar bar = db.Bars.Find(id);
            if (bar == null)
            {
                return HttpNotFound();
            }
            ViewBag.BarSeriesId = new SelectList(db.TimeSeries, "Id", "Code", bar.BarSeriesId);
            return View(bar);
        }

        //
        // POST: /Bars/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Bar bar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BarSeriesId = new SelectList(db.TimeSeries, "Id", "Code", bar.BarSeriesId);
            return View(bar);
        }

        //
        // GET: /Bars/Delete/5

        public ActionResult Delete(long id = 0)
        {
            Bar bar = db.Bars.Find(id);
            if (bar == null)
            {
                return HttpNotFound();
            }
            return View(bar);
        }

        //
        // POST: /Bars/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Bar bar = db.Bars.Find(id);
            db.Bars.Remove(bar);
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