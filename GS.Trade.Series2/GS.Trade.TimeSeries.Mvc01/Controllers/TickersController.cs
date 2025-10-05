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
    public class TickersController : Controller
    {
        private TimeSeriesContext db = new TimeSeriesContext();

        //
        // GET: /Tickers/

        public ActionResult Index()
        {
            var tickers = db.Tickers.Include(t => t.TradeBoard);
            return View(tickers.ToList());
        }

        //
        // GET: /Tickers/Details/5

        public ActionResult Details(long id = 0)
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
            ViewBag.TradeBoardId = new SelectList(db.TradeBoards, "Id", "Code");
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

            ViewBag.TradeBoardId = new SelectList(db.TradeBoards, "Id", "Code", ticker.TradeBoardId);
            return View(ticker);
        }

        //
        // GET: /Tickers/Edit/5

        public ActionResult Edit(long id = 0)
        {
            Ticker ticker = db.Tickers.Find(id);
            if (ticker == null)
            {
                return HttpNotFound();
            }
            ViewBag.TradeBoardId = new SelectList(db.TradeBoards, "Id", "Code", ticker.TradeBoardId);
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
            ViewBag.TradeBoardId = new SelectList(db.TradeBoards, "Id", "Code", ticker.TradeBoardId);
            return View(ticker);
        }

        //
        // GET: /Tickers/Delete/5

        public ActionResult Delete(long id = 0)
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
        public ActionResult DeleteConfirmed(long id)
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