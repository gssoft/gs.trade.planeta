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
    public class TickSeriesController : Controller
    {
        private TimeSeriesContext db = new TimeSeriesContext();

        //
        // GET: /TickSeries/

        public ActionResult Index()
        {
            var timeseries = db.TimeSeries.OfType<TickSeries>().Include(t => t.Ticker).Include(t => t.TimeInt).Include(t => t.QuoteProvider);
            return View(timeseries.ToList());
        }

        //
        // GET: /TickSeries/Details/5

        public ActionResult Details(long id = 0)
        {
            var tickseries = db.TimeSeries.Find(id);
            if (tickseries == null)
            {
                return HttpNotFound();
            }
            return View(tickseries);
        }

        //
        // GET: /TickSeries/Create

        public ActionResult Create()
        {
            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Code");
            ViewBag.TimeIntId = new SelectList(db.TimeInts, "Id", "Code");
            ViewBag.QuoteProviderId = new SelectList(db.QuoteProviders, "Id", "Code");
            return View();
        }

        //
        // POST: /TickSeries/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TickSeries tickseries)
        {
            if (ModelState.IsValid)
            {
                db.TimeSeries.Add(tickseries);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Code", tickseries.TickerId);
            ViewBag.TimeIntId = new SelectList(db.TimeInts, "Id", "Code", tickseries.TimeIntId);
            ViewBag.QuoteProviderId = new SelectList(db.QuoteProviders, "Id", "Code", tickseries.QuoteProviderId);
            return View(tickseries);
        }

        //
        // GET: /TickSeries/Edit/5

        public ActionResult Edit(long id = 0)
        {
            var tickseries = db.TimeSeries.Find(id);
            if (tickseries == null)
            {
                return HttpNotFound();
            }
            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Code", tickseries.TickerId);
            ViewBag.TimeIntId = new SelectList(db.TimeInts, "Id", "Code", tickseries.TimeIntId);
            ViewBag.QuoteProviderId = new SelectList(db.QuoteProviders, "Id", "Code", tickseries.QuoteProviderId);
            return View(tickseries);
        }

        //
        // POST: /TickSeries/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TickSeries tickseries)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tickseries).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Code", tickseries.TickerId);
            ViewBag.TimeIntId = new SelectList(db.TimeInts, "Id", "Code", tickseries.TimeIntId);
            ViewBag.QuoteProviderId = new SelectList(db.QuoteProviders, "Id", "Code", tickseries.QuoteProviderId);
            return View(tickseries);
        }

        //
        // GET: /TickSeries/Delete/5

        public ActionResult Delete(long id = 0)
        {
            var tickseries = db.TimeSeries.Find(id);
            if (tickseries == null)
            {
                return HttpNotFound();
            }
            return View(tickseries);
        }

        //
        // POST: /TickSeries/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            var tickseries = db.TimeSeries.Find(id);
            db.TimeSeries.Remove(tickseries);
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