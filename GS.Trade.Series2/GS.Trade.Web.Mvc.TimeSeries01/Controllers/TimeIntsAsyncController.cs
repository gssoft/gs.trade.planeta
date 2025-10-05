using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GS.Trade.TimeSeries.Model;

namespace GS.Trade.Web.Mvc.TimeSeries01.Controllers
{
    public class TimeIntsAsyncController : Controller
    {
        private TimeSeriesContext db = new TimeSeriesContext();

        // GET: TimeIntsAsync
        public async Task<ActionResult> Index()
        {
            return View(await db.TimeInts.ToListAsync());
        }

        // GET: TimeIntsAsync/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeInt timeInt = await db.TimeInts.FindAsync(id);
            if (timeInt == null)
            {
                return HttpNotFound();
            }
            return View(timeInt);
        }

        // GET: TimeIntsAsync/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TimeIntsAsync/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,TimeInterval,TimeShift,Code,Name,Description,CreatedDT,ModifiedDT")] TimeInt timeInt)
        {
            if (ModelState.IsValid)
            {
                db.TimeInts.Add(timeInt);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(timeInt);
        }

        // GET: TimeIntsAsync/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeInt timeInt = await db.TimeInts.FindAsync(id);
            if (timeInt == null)
            {
                return HttpNotFound();
            }
            return View(timeInt);
        }

        // POST: TimeIntsAsync/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,TimeInterval,TimeShift,Code,Name,Description,CreatedDT,ModifiedDT")] TimeInt timeInt)
        {
            if (ModelState.IsValid)
            {
                db.Entry(timeInt).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(timeInt);
        }

        // GET: TimeIntsAsync/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeInt timeInt = await db.TimeInts.FindAsync(id);
            if (timeInt == null)
            {
                return HttpNotFound();
            }
            return View(timeInt);
        }

        // POST: TimeIntsAsync/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            TimeInt timeInt = await db.TimeInts.FindAsync(id);
            db.TimeInts.Remove(timeInt);
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
