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
    public class PositionsController : Controller
    {
        private DbTradeContext db = new DbTradeContext();

        // GET: Positions
        public ActionResult Index()
        {
            var positions = db.Positions
                                .Where(p=>p.Status != 0)
                                .Include(p => p.Strategy)
                                .OrderBy(p=>p.Strategy.Account.Code)
                                .ThenBy(p => p.Strategy.Ticker.Code)
                                .ThenBy(p => p.Strategy.Code)
                                .ThenBy(p => p.Strategy.TimeInt);
            ViewBag.Title = "Positions";
            ViewBag.FullTitle = "Positions ( " + positions.Count() + " )";
            ViewBag.ItemCount = positions.Count();
            return View(positions.ToList());
        }

        // GET: Positions/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Position position = db.Positions.Find(id);
            if (position == null)
            {
                return HttpNotFound();
            }
            return View(position);
        }

        // GET: Positions/Create
        public ActionResult Create()
        {
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key");
            return View();
        }

        // POST: Positions/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Key,StrategyId,Operation,Status,Quantity,Price1,Price2,PnL,PnL3,FirstTradeDT,FirstTradeNumber,LastTradeDT,LastTradeNumber,Created,Modified")] Position position)
        {
            if (ModelState.IsValid)
            {
                db.Positions.Add(position);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key", position.StrategyId);
            return View(position);
        }

        // GET: Positions/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Position position = db.Positions.Find(id);
            if (position == null)
            {
                return HttpNotFound();
            }
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key", position.StrategyId);
            return View(position);
        }

        // POST: Positions/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Key,StrategyId,Operation,Status,Quantity,Price1,Price2,PnL,PnL3,FirstTradeDT,FirstTradeNumber,LastTradeDT,LastTradeNumber,Created,Modified")] Position position)
        {
            if (ModelState.IsValid)
            {
                db.Entry(position).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key", position.StrategyId);
            return View(position);
        }

        // GET: Positions/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Position position = db.Positions.Find(id);
            if (position == null)
            {
                return HttpNotFound();
            }
            return View(position);
        }

        // POST: Positions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Position position = db.Positions.Find(id);
            db.Positions.Remove(position);
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
