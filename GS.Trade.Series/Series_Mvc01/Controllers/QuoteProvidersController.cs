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
    public class QuoteProvidersController : Controller
    {
        private SeriesContext db = new SeriesContext();

        //
        // GET: /QuoteProviders/

        public ActionResult Index()
        {
            return View(db.QuoteProviders.ToList());
        }

        //
        // GET: /QuoteProviders/Details/5

        public ActionResult Details(long id = 0)
        {
            QuoteProvider quoteprovider = db.QuoteProviders.Find(id);
            if (quoteprovider == null)
            {
                return HttpNotFound();
            }
            return View(quoteprovider);
        }

        //
        // GET: /QuoteProviders/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /QuoteProviders/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QuoteProvider quoteprovider)
        {
            if (ModelState.IsValid)
            {
                db.QuoteProviders.Add(quoteprovider);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(quoteprovider);
        }

        //
        // GET: /QuoteProviders/Edit/5

        public ActionResult Edit(long id = 0)
        {
            QuoteProvider quoteprovider = db.QuoteProviders.Find(id);
            if (quoteprovider == null)
            {
                return HttpNotFound();
            }
            return View(quoteprovider);
        }

        //
        // POST: /QuoteProviders/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(QuoteProvider quoteprovider)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quoteprovider).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(quoteprovider);
        }

        //
        // GET: /QuoteProviders/Delete/5

        public ActionResult Delete(long id = 0)
        {
            QuoteProvider quoteprovider = db.QuoteProviders.Find(id);
            if (quoteprovider == null)
            {
                return HttpNotFound();
            }
            return View(quoteprovider);
        }

        //
        // POST: /QuoteProviders/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            QuoteProvider quoteprovider = db.QuoteProviders.Find(id);
            db.QuoteProviders.Remove(quoteprovider);
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