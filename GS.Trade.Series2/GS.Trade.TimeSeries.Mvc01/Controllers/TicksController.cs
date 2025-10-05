using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using GS.Trade.TimeSeries.Model;

namespace GS.Trade.TimeSeries.Mvc01.Controllers
{
    public class TicksController : Controller
    {
        private TimeSeriesContext db = new TimeSeriesContext();

        //
        // GET: /Ticks/

        public ActionResult Index()
        {
            var ticks = db.Ticks.Take(100).Include(t => t.TimeSeries);
            return View(ticks.ToList());
        }

        //
        // GET: /Ticks/Details/5

        public ActionResult Details(long id = 0)
        {
            Tick tick = db.Ticks.Find(id);
            if (tick == null)
            {
                return HttpNotFound();
            }
            return View(tick);
        }

        //
        // GET: /Ticks/Create

        public ActionResult Create()
        {
            ViewBag.TickSeriesId = new SelectList(db.TimeSeries, "Id", "Code");
            return View();
        }

        //
        // POST: /Ticks/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tick tick)
        {
            if (ModelState.IsValid)
            {
                db.Ticks.Add(tick);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TickSeriesId = new SelectList(db.TimeSeries, "Id", "Code", tick.TickSeriesId);
            return View(tick);
        }

        //
        // GET: /Ticks/Edit/5

        public ActionResult Edit(long id = 0)
        {
            Tick tick = db.Ticks.Find(id);
            if (tick == null)
            {
                return HttpNotFound();
            }
            ViewBag.TickSeriesId = new SelectList(db.TimeSeries, "Id", "Code", tick.TickSeriesId);
            return View(tick);
        }

        //
        // POST: /Ticks/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tick tick)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tick).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TickSeriesId = new SelectList(db.TimeSeries, "Id", "Code", tick.TickSeriesId);
            return View(tick);
        }

        //
        // GET: /Ticks/Delete/5

        public ActionResult Delete(long id = 0)
        {
            Tick tick = db.Ticks.Find(id);
            if (tick == null)
            {
                return HttpNotFound();
            }
            return View(tick);
        }

        //
        // POST: /Ticks/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Tick tick = db.Ticks.Find(id);
            db.Ticks.Remove(tick);
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