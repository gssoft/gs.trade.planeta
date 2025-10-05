using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GS.Trade.TimeSeries.FortsTicks.Dal;
using GS.Trade.TimeSeries.FortsTicks.Model;

namespace GS.Trade.Web.Mvc.TimeSeries01.Controllers
{
    public class FortsTickStatsController : Controller
    {
        private readonly FortsTicksContext _db = new FortsTicksContext();

        // GET: FortsTickStats
        public async Task<ActionResult> Index()
        {
           var stats = _db.Stats
                            .Include(s => s.Ticker)
                            .OrderBy(s => s.Period)
                            .ThenBy(s=>s.Ticker.Code)
                            .ThenBy(s=>s.Type)
                            //.ThenBy(s => s.Period)
                            .ThenBy(s=>s.LastDate);
           
           return View(await stats.ToListAsync());
        }

        // GET: FortsTickStats/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stat stat = await _db.Stats.FindAsync(id);
            if (stat == null)
            {
                return HttpNotFound();
            }
            return View(stat);
        }

        // GET: FortsTickStats/Create
        public ActionResult Create()
        {
            ViewBag.TickerID = new SelectList(_db.Tickers, "ID", "Code");
            return View();
        }

        // POST: FortsTickStats/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,TickerID,Type,Period,LastDate,Count,FirstDT,LastDT,MinValue,MaxValue,ModifiedDT")] Stat stat)
        {
            if (ModelState.IsValid)
            {
                _db.Stats.Add(stat);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.TickerID = new SelectList(_db.Tickers, "ID", "Code", stat.TickerID);
            return View(stat);
        }

        // GET: FortsTickStats/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stat stat = await _db.Stats.FindAsync(id);
            if (stat == null)
            {
                return HttpNotFound();
            }
            ViewBag.TickerID = new SelectList(_db.Tickers, "ID", "Code", stat.TickerID);
            return View(stat);
        }

        // POST: FortsTickStats/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,TickerID,Type,Period,LastDate,Count,FirstDT,LastDT,MinValue,MaxValue,ModifiedDT")] Stat stat)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(stat).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.TickerID = new SelectList(_db.Tickers, "ID", "Code", stat.TickerID);
            return View(stat);
        }

        // GET: FortsTickStats/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stat stat = await _db.Stats.FindAsync(id);
            if (stat == null)
            {
                return HttpNotFound();
            }
            return View(stat);
        }

        // POST: FortsTickStats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Stat stat = await _db.Stats.FindAsync(id);
            _db.Stats.Remove(stat);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
