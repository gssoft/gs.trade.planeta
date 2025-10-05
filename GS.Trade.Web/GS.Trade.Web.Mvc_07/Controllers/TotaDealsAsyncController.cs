using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GS.Trade.DataBase.Dal;
using GS.Trade.DataBase.Functions;
using GS.Trade.DataBase.Model;

namespace GS.Trade.Web.Mvc_07.Controllers
{
    public class TotaDealsAsyncController : Controller
    {
        private DbTradeContext db = new DbTradeContext();
       
        // GET: TotaDealsAsync
        public async Task<ActionResult> Index()
        {
            var lastDayChecked = true;
            DateTime? lastDate;
            lastDate = DealsFunctions.GetDealsLastDateTime(db);
            
            string accountList = "RIM5";
            string symbolList = "RI";
            string tickerList = "RIM5";
            string strategyList = null;
            string searchString = null;
            var timeInt = 15;

            var deals = DealsFunctions.SelectDeals(db,
                accountList, symbolList, tickerList, strategyList, timeInt, searchString, false);

            var positions = db.Positions.Include(t => t.Strategy);
            return View(await positions.ToListAsync());
        }
        public async Task<ActionResult> Index1()
        {
            var positions = db.Positions.Include(t => t.Strategy);
            return View(await positions.ToListAsync());
        }

        // GET: TotaDealsAsync/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Total total = await db.Totals.FindAsync(id);
            if (total == null)
            {
                return HttpNotFound();
            }
            return View(total);
        }

        // GET: TotaDealsAsync/Create
        public ActionResult Create()
        {
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key");
            return View();
        }

        // POST: TotaDealsAsync/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Key,StrategyId,Operation,Status,Quantity,Price1,Price2,PnL,PnL3,FirstTradeDT,FirstTradeNumber,LastTradeDT,LastTradeNumber,Created,Modified")] Total total)
        {
            if (ModelState.IsValid)
            {
                db.Positions.Add(total);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key", total.StrategyId);
            return View(total);
        }

        // GET: TotaDealsAsync/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Total total = await db.Totals.FindAsync(id);
            if (total == null)
            {
                return HttpNotFound();
            }
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key", total.StrategyId);
            return View(total);
        }

        // POST: TotaDealsAsync/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Key,StrategyId,Operation,Status,Quantity,Price1,Price2,PnL,PnL3,FirstTradeDT,FirstTradeNumber,LastTradeDT,LastTradeNumber,Created,Modified")] Total total)
        {
            if (ModelState.IsValid)
            {
                db.Entry(total).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key", total.StrategyId);
            return View(total);
        }

        // GET: TotaDealsAsync/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Total total = await db.Totals.FindAsync(id);
            if (total == null)
            {
                return HttpNotFound();
            }
            return View(total);
        }

        // POST: TotaDealsAsync/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Total total = await db.Totals.FindAsync(id);
            db.Positions.Remove(total);
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
