using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using GS.Trade.DataBase.Dal;
using GS.Trade.DataBase.Model;

namespace GS.Trade.Web.Mvc_07.Controllers
{
    public class StrategiesAsyncController : Controller
    {
        private DbTradeContext db = new DbTradeContext();

        // GET: StrategiesAsync
        public async Task<ActionResult> Index()
        {
            var strategies = db.Strategies
                                .Include(s => s.Account)
                                .Include(s => s.Ticker)
                                .OrderBy(s=>s.Account.Code + s.Ticker.Code + s.Code)
                                ;
            var lst = await strategies.ToListAsync();
            var cnt = lst.Count();
            ViewBag.Title = "Strategies";
            ViewBag.FullTitle = "Strategies ( " + cnt + " )";  
            return View(lst);
        }

        // GET: StrategiesAsync/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Strategy strategy = await db.Strategies.FindAsync(id);
            if (strategy == null)
            {
                return HttpNotFound();
            }
            return View(strategy);
        }

        // GET: StrategiesAsync/Create
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Key");
            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Key");
            return View();
        }

        // POST: StrategiesAsync/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Key,Name,Alias,Code,TimeInt,AccountId,TickerId,Created,Modified")] Strategy strategy)
        {
            if (ModelState.IsValid)
            {
                db.Strategies.Add(strategy);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Key", strategy.AccountId);
            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Key", strategy.TickerId);
            return View(strategy);
        }

        // GET: StrategiesAsync/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Strategy strategy = await db.Strategies.FindAsync(id);
            if (strategy == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Key", strategy.AccountId);
            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Key", strategy.TickerId);
            return View(strategy);
        }

        // POST: StrategiesAsync/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Key,Name,Alias,Code,TimeInt,AccountId,TickerId,Created,Modified")] Strategy strategy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(strategy).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Key", strategy.AccountId);
            ViewBag.TickerId = new SelectList(db.Tickers, "Id", "Key", strategy.TickerId);
            return View(strategy);
        }

        // GET: StrategiesAsync/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Strategy strategy = await db.Strategies.FindAsync(id);
            if (strategy == null)
            {
                return HttpNotFound();
            }
            return View(strategy);
        }

        // POST: StrategiesAsync/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Strategy strategy = await db.Strategies.FindAsync(id);
            db.Strategies.Remove(strategy);
            await db.SaveChangesAsync();
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
