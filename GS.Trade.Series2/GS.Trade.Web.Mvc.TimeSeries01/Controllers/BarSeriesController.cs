using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GS.Trade.TimeSeries.Model;
using GS.Trade.TimeSeries.Model.View;
using GS.Trade.Web.Mvc.TimeSeries01.Models.Views;

namespace GS.Trade.Web.Mvc.TimeSeries01.Controllers
{
    public class BarSeriesController : Controller
    {
        private TimeSeriesContext db = new TimeSeriesContext();

        // GET: BarSeries
        public ActionResult Index(long? timeSeriesId)
        {
            if (timeSeriesId == null)
            {
                var timeSeries =
                    db.TimeSeries.Include(b => b.QuoteProvider).Include(b => b.Ticker).Include(b => b.TimeInt);
                return View(timeSeries.ToList());
            }
            var ts =
                db.TimeSeries
                    .Where(tm => tm.Id == timeSeriesId)
                //.Include(b => b.QuoteProvider).Include(b => b.Ticker).Include(b => b.TimeInt)
                ;
            return View(ts.ToList());
        }

        public async Task<ActionResult> Stats()
        {
            db.Database.CommandTimeout = 300;

            var q = db.StatSelect031();

            var v = await q.ToListAsync();
            return View(v);
        }
        public async Task<ActionResult> StatsAsync()
        {
            db.Database.CommandTimeout = 300;

            var q = await db.StatSelect03Async();
            
            return View(q.ToList());
        }

        // GET: BarSeries/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var barSeries = db.TimeSeries.Find(id);
            if (barSeries == null)
            {
                return HttpNotFound();
            }
            return View(barSeries);
        }

        // GET: BarSeries/Create
        public ActionResult Create()
        {
            ViewBag.QuoteProviderId = new SelectList(db.QuoteProviders, "Id", "Code");
            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Code");
            ViewBag.TimeIntId = new SelectList(db.TimeInts, "Id", "Code");
            return View();
        }

        // POST: BarSeries/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TickerId,TimeIntId,QuoteProviderId,Code,Name,Description,CreatedDT,ModifiedDT")] BarSeries barSeries)
        {
            if (ModelState.IsValid)
            {

                barSeries.ModifiedDT = barSeries.CreatedDT = DateTime.Now;

                db.TimeSeries.Add(barSeries);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.QuoteProviderId = new SelectList(db.QuoteProviders, "Id", "Code", barSeries.QuoteProviderId);
            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Code", barSeries.TickerId);
            ViewBag.TimeIntId = new SelectList(db.TimeInts, "Id", "Code", barSeries.TimeIntId);
            return View(barSeries);
        }

        // GET: BarSeries/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var barSeries = db.TimeSeries.Find(id);
            if (barSeries == null)
            {
                return HttpNotFound();
            }
            ViewBag.QuoteProviderId = new SelectList(db.QuoteProviders, "Id", "Code", barSeries.QuoteProviderId);
            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Code", barSeries.TickerId);
            ViewBag.TimeIntId = new SelectList(db.TimeInts, "Id", "Code", barSeries.TimeIntId);
            return View(barSeries);
        }

        // POST: BarSeries/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,TickerId,TimeIntId,QuoteProviderId,Code,Name,Description,CreatedDT,ModifiedDT")] BarSeries barSeries)
        public ActionResult Edit([Bind(Include = "Id,TickerId,TimeIntId,QuoteProviderId,Code,Name,Description")] BarSeries barSeries)
        {
            if (ModelState.IsValid)
            {
                db.Entry(barSeries).State = EntityState.Modified;

                barSeries.ModifiedDT = DateTime.Now;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.QuoteProviderId = new SelectList(db.QuoteProviders, "Id", "Code", barSeries.QuoteProviderId);
            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Code", barSeries.TickerId);
            ViewBag.TimeIntId = new SelectList(db.TimeInts, "Id", "Code", barSeries.TimeIntId);
            return View(barSeries);
        }

        // GET: BarSeries/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var barSeries = db.TimeSeries.Find(id);
            if (barSeries == null)
            {
                return HttpNotFound();
            }
            return View(barSeries);
        }

        // POST: BarSeries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            var barSeries = db.TimeSeries.Find(id);
            db.TimeSeries.Remove(barSeries);
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
