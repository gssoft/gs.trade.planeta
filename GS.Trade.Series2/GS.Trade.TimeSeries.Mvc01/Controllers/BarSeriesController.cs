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
    using BarSeries = GS.Trade.TimeSeries.Model.BarSeries;
    public class BarSeriesController : Controller
    {
        private TimeSeriesContext db = new TimeSeriesContext();

        //
        // GET: /BarSeries/

        public ActionResult Index()
        {
            var timeseries = db.TimeSeries.OfType<BarSeries>()
                .Include(b => b.Ticker).Include(b => b.TimeInt).Include(b => b.QuoteProvider);
            return View(timeseries.ToList());
        }

        //
        // GET: /BarSeries/Details/5

        public ActionResult Details(long id = 0)
        {
            var barseries = db.TimeSeries.Find(id);
            if (barseries == null)
            {
                return HttpNotFound();
            }
            return View(barseries);
        }

        //
        // GET: /BarSeries/Create

        public ActionResult Create()
        {
            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Code");
            ViewBag.TimeIntId = new SelectList(db.TimeInts, "Id", "Code");
            ViewBag.QuoteProviderId = new SelectList(db.QuoteProviders, "Id", "Code");
            return View();
        }

        //
        // POST: /BarSeries/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BarSeries barseries)
        {
            if (ModelState.IsValid)
            {
                db.TimeSeries.Add(barseries);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Code", barseries.TickerId);
            ViewBag.TimeIntId = new SelectList(db.TimeInts, "Id", "Code", barseries.TimeIntId);
            ViewBag.QuoteProviderId = new SelectList(db.QuoteProviders, "Id", "Code", barseries.QuoteProviderId);
            return View(barseries);
        }

        //
        // GET: /BarSeries/Edit/5

        public ActionResult Edit(long id = 0)
        {
            var barseries = db.TimeSeries.Find(id);
            if (barseries == null)
            {
                return HttpNotFound();
            }
            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Code", barseries.TickerId);
            ViewBag.TimeIntId = new SelectList(db.TimeInts, "Id", "Code", barseries.TimeIntId);
            ViewBag.QuoteProviderId = new SelectList(db.QuoteProviders, "Id", "Code", barseries.QuoteProviderId);
            return View(barseries);
        }

        //
        // POST: /BarSeries/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BarSeries barseries)
        {
            if (ModelState.IsValid)
            {
                db.Entry(barseries).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Code", barseries.TickerId);
            ViewBag.TimeIntId = new SelectList(db.TimeInts, "Id", "Code", barseries.TimeIntId);
            ViewBag.QuoteProviderId = new SelectList(db.QuoteProviders, "Id", "Code", barseries.QuoteProviderId);
            return View(barseries);
        }

        //
        // GET: /BarSeries/Delete/5

        public ActionResult Delete(long id = 0)
        {
            var barseries = db.TimeSeries.Find(id);
            if (barseries == null)
            {
                return HttpNotFound();
            }
            return View(barseries);
        }

        //
        // POST: /BarSeries/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            var barseries = db.TimeSeries.Find(id);
            db.TimeSeries.Remove(barseries);
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