using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GS.Extension;
using GS.Trade.DataBase.Model;
using GS.Trade.DataBase.Dal;

namespace GS.Trade.Web.Mvc_03.Controllers
{
    public class TickersController : Controller
    {
        private DbTradeContext db = new DbTradeContext();

        //
        // GET: /Tickers/

        public ActionResult Index()
        {
            return View(db.Tickers.OrderBy(f=>(f.TradeBoard.Trim().ToUpper()+f.Name.Trim().ToUpper())).ToList());
        }

        //
        // GET: /Tickers/Details/5

        public ActionResult Details(int id = 0)
        {
            Ticker ticker = db.Tickers.Find(id);
            if (ticker == null)
            {
                return HttpNotFound();
            }
            return View(ticker);
        }

        //
        // GET: /Tickers/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Tickers/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Ticker ticker)
        {
            if (ModelState.IsValid)
            {
                db.Tickers.Add(ticker);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ticker);
        }

        //
        // GET: /Tickers/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Ticker ticker = db.Tickers.Find(id);
            if (ticker == null)
            {
                return HttpNotFound();
            }
            return View(ticker);
        }

        //
        // POST: /Tickers/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ticker ticker)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticker).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ticker);
        }

        //
        // GET: /Tickers/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Ticker ticker = db.Tickers.Find(id);
            if (ticker == null)
            {
                return HttpNotFound();
            }
            return View(ticker);
        }

        //
        // POST: /Tickers/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticker ticker = db.Tickers.Find(id);
            db.Tickers.Remove(ticker);
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