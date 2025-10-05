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
    public class QuoteProvidersController : Controller
    {
        private TimeSeriesContext db = new TimeSeriesContext();

        // GET: QuoteProviders
        public ActionResult Index()
        {
            return View(db.QuoteProviders.ToList());
        }

        // GET: QuoteProviders/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuoteProvider quoteProvider = db.QuoteProviders.Find(id);
            if (quoteProvider == null)
            {
                return HttpNotFound();
            }
            return View(quoteProvider);
        }

        // GET: QuoteProviders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QuoteProviders/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Name,Description,CreatedDT,ModifiedDT")] QuoteProvider quoteProvider)
        {
            if (ModelState.IsValid)
            {
                db.QuoteProviders.Add(quoteProvider);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(quoteProvider);
        }

        // GET: QuoteProviders/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuoteProvider quoteProvider = db.QuoteProviders.Find(id);
            if (quoteProvider == null)
            {
                return HttpNotFound();
            }
            return View(quoteProvider);
        }

        // POST: QuoteProviders/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,Description,CreatedDT,ModifiedDT")] QuoteProvider quoteProvider)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quoteProvider).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(quoteProvider);
        }

        // GET: QuoteProviders/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuoteProvider quoteProvider = db.QuoteProviders.Find(id);
            if (quoteProvider == null)
            {
                return HttpNotFound();
            }
            return View(quoteProvider);
        }

        // POST: QuoteProviders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            QuoteProvider quoteProvider = db.QuoteProviders.Find(id);
            db.QuoteProviders.Remove(quoteProvider);
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
