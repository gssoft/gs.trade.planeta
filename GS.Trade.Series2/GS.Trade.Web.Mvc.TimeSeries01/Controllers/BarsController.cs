using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GS.Trade.TimeSeries.Model;

namespace GS.Trade.Web.Mvc.TimeSeries01.Controllers
{
    public class BarsController : Controller
    {
        private TimeSeriesContext db = new TimeSeriesContext();

        // GET: Bars
        //public ActionResult Index()
        //{
        //    var bars = db.Bars.Include(b => b.TimeSeries);
        //    return View(bars.ToList());
        //}
        public ActionResult Index(long? barSeriesId )
        {
            IQueryable<Bar> bars;
            if (barSeriesId.HasValue && barSeriesId != 0)
            {
                bars = db.Bars
                    .Where(b=>b.BarSeriesId == barSeriesId)
                    .Include(b => b.TimeSeries);
            }
            else
            {
                bars = db.Bars.Include(b => b.TimeSeries);
                    
            }
            return View(bars.OrderByDescending(b=>b.DT).ToList());
        }

        // GET: Bars/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bar bar = db.Bars.Find(id);
            if (bar == null)
            {
                return HttpNotFound();
            }
            return View(bar);
        }

        // GET: Bars/Create
        public ActionResult Create()
        {
            ViewBag.BarSeriesId = new SelectList(db.TimeSeries, "Id", "Code");
            return View();
        }

        // POST: Bars/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BarSeriesId,Open,High,Low,Close,Volume,DT")] Bar bar)
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

        // GET: Bars/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bar bar = db.Bars.Find(id);
            if (bar == null)
            {
                return HttpNotFound();
            }
            ViewBag.BarSeriesId = new SelectList(db.TimeSeries, "Id", "Code", bar.BarSeriesId);
            return View(bar);
        }

        // POST: Bars/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BarSeriesId,Open,High,Low,Close,Volume,DT")] Bar bar)
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

        // GET: Bars/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bar bar = db.Bars.Find(id);
            if (bar == null)
            {
                return HttpNotFound();
            }
            return View(bar);
        }

        // POST: Bars/Delete/5
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
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
