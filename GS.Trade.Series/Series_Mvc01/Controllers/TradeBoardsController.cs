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
    public class TradeBoardsController : Controller
    {
        private SeriesContext db = new SeriesContext();

        //
        // GET: /TradeBoards/

        public ActionResult Index()
        {
            return View(db.TradeBoards.ToList());
        }

        //
        // GET: /TradeBoards/Details/5

        public ActionResult Details(long id = 0)
        {
            TradeBoard tradeboard = db.TradeBoards.Find(id);
            if (tradeboard == null)
            {
                return HttpNotFound();
            }
            return View(tradeboard);
        }

        //
        // GET: /TradeBoards/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /TradeBoards/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TradeBoard tradeboard)
        {
            if (ModelState.IsValid)
            {
                db.TradeBoards.Add(tradeboard);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tradeboard);
        }

        //
        // GET: /TradeBoards/Edit/5

        public ActionResult Edit(long id = 0)
        {
            TradeBoard tradeboard = db.TradeBoards.Find(id);
            if (tradeboard == null)
            {
                return HttpNotFound();
            }
            return View(tradeboard);
        }

        //
        // POST: /TradeBoards/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TradeBoard tradeboard)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tradeboard).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tradeboard);
        }

        //
        // GET: /TradeBoards/Delete/5

        public ActionResult Delete(long id = 0)
        {
            TradeBoard tradeboard = db.TradeBoards.Find(id);
            if (tradeboard == null)
            {
                return HttpNotFound();
            }
            return View(tradeboard);
        }

        //
        // POST: /TradeBoards/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            TradeBoard tradeboard = db.TradeBoards.Find(id);
            db.TradeBoards.Remove(tradeboard);
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