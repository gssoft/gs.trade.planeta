using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GS.Trade.DataBase.Dal;
using GS.Trade.DataBase.Model;

namespace GS.Trade.Web.Mvc_05.Controllers
{
    public class StrategiesController : Controller
    {
        private DbTradeContext db = new DbTradeContext();

        // GET: Strategies
        public ActionResult Index()
        {
            var strategies = db.Strategies.Include(s => s.Account).Include(s => s.Ticker);
            ViewBag.Title = "Strategies";
            ViewBag.FullTitle = "Strategies ( " + strategies.Count() + " )";
            ViewBag.ItemCount = strategies.Count();
            return View(strategies.ToList());
        }

        // GET: Strategies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Strategy strategy = db.Strategies.Find(id);
            if (strategy == null)
            {
                return HttpNotFound();
            }
            return View(strategy);
        }

        // GET: Strategies/Create
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Key");
            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Key");
            return View();
        }

        // POST: Strategies/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Key,Name,Alias,Code,TimeInt,AccountId,TickerId,Created,Modified")] Strategy strategy)
        {
            if (ModelState.IsValid)
            {
                db.Strategies.Add(strategy);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Key", strategy.AccountId);
            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Key", strategy.TickerId);
            return View(strategy);
        }

        // GET: Strategies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Strategy strategy = db.Strategies.Find(id);
            if (strategy == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Key", strategy.AccountId);
            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Key", strategy.TickerId);
            return View(strategy);
        }

        // POST: Strategies/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Key,Name,Alias,Code,TimeInt,AccountId,TickerId,Created,Modified")] Strategy strategy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(strategy).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Key", strategy.AccountId);
            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Key", strategy.TickerId);
            return View(strategy);
        }

        // GET: Strategies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Strategy strategy = db.Strategies.Find(id);
            if (strategy == null)
            {
                return HttpNotFound();
            }
            return View(strategy);
        }

        // POST: Strategies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Strategy strategy = db.Strategies.Find(id);
            db.Strategies.Remove(strategy);
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
