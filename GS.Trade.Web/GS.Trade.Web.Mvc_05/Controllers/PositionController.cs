using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GS.Trade.DataBase.Model;
using GS.Trade.DataBase.Dal;

namespace GS.Trade.Web.Mvc_05.Controllers
{
    public class PositionController : Controller
    {
        private DbTradeContext db = new DbTradeContext("DbTrade2");

        //
        // GET: /Position/

        public ActionResult Index()
        {
            var positions = db.Positions.Include(p => p.Strategy);
            return View(positions.ToList());
        }

        //
        // GET: /Position/Details/5

        public ActionResult Details(long id = 0)
        {
            Position position = db.Positions.Find(id);
            if (position == null)
            {
                return HttpNotFound();
            }
            return View(position);
        }

        //
        // GET: /Position/Create

        public ActionResult Create()
        {
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Name");
            return View();
        }

        //
        // POST: /Position/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Position position)
        {
            if (ModelState.IsValid)
            {
                db.Positions.Add(position);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Name", position.StrategyId);
            return View(position);
        }

        //
        // GET: /Position/Edit/5

        public ActionResult Edit(long id = 0)
        {
            Position position = db.Positions.Find(id);
            if (position == null)
            {
                return HttpNotFound();
            }
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Name", position.StrategyId);
            return View(position);
        }

        //
        // POST: /Position/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Position position)
        {
            if (ModelState.IsValid)
            {
                db.Entry(position).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Name", position.StrategyId);
            return View(position);
        }

        //
        // GET: /Position/Delete/5

        public ActionResult Delete(long id = 0)
        {
            Position position = db.Positions.Find(id);
            if (position == null)
            {
                return HttpNotFound();
            }
            return View(position);
        }

        //
        // POST: /Position/Delete/5

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
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}