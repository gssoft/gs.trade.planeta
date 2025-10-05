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
    public class SrcFilesController : Controller
    {
        private FortsTicksContext db = new FortsTicksContext();

        // GET: SrcFiles
        public async Task<ActionResult> Index()
        {
            return View(await db.SrcFiles.ToListAsync());
        }

        // GET: SrcFiles/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SrcFile srcFile = await db.SrcFiles.FindAsync(id);
            if (srcFile == null)
            {
                return HttpNotFound();
            }
            return View(srcFile);
        }

        // GET: SrcFiles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SrcFiles/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Name,Count,FirstDT,LastDT,ModifiedDT")] SrcFile srcFile)
        {
            if (ModelState.IsValid)
            {
                db.SrcFiles.Add(srcFile);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(srcFile);
        }

        // GET: SrcFiles/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SrcFile srcFile = await db.SrcFiles.FindAsync(id);
            if (srcFile == null)
            {
                return HttpNotFound();
            }
            return View(srcFile);
        }

        // POST: SrcFiles/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Name,Count,FirstDT,LastDT,ModifiedDT")] SrcFile srcFile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(srcFile).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(srcFile);
        }

        // GET: SrcFiles/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SrcFile srcFile = await db.SrcFiles.FindAsync(id);
            if (srcFile == null)
            {
                return HttpNotFound();
            }
            return View(srcFile);
        }

        // POST: SrcFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SrcFile srcFile = await db.SrcFiles.FindAsync(id);
            db.SrcFiles.Remove(srcFile);
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
