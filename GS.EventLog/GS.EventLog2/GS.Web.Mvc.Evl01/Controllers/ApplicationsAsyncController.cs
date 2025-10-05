using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GS.Applications.DataBase1.Dal;
using GS.Applications.DataBase1.Model;
using GS.Extension;
using Microsoft.AspNet.Identity;

namespace GS.Web.Mvc.Evl01.Controllers
{
    [Authorize]
    public class ApplicationsAsyncController : Controller
    {
        private readonly ApplicationsContext1 _db = new ApplicationsContext1();

        // GET: ApplicationsAsync
        public async Task<ActionResult> Index()
        {
            var u = await _db.RegisterUserAsync(User.Identity.Name, User.Identity.GetUserId());

            var applications = _db.Applications
                .Where(a=>a.UserID == u.ID)
                .Include(a => a.User)
                .AsNoTracking();
            ViewBag.Title = "Applications: " + applications.Count().ToString(CultureInfo.InvariantCulture).WithBrackets();
            return View(await applications.ToListAsync());
        }

        // GET: ApplicationsAsync/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = await _db.Applications.FindAsync(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        // GET: ApplicationsAsync/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(_db.Users, "ID", "Name");
            return View();
        }

        // POST: ApplicationsAsync/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,UserID,Code,Name,Description")] Application application)
        {
            if (ModelState.IsValid)
            {
                _db.Applications.Add(application);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(_db.Users, "ID", "Name", application.UserID);
            return View(application);
        }

        // GET: ApplicationsAsync/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = await _db.Applications.FindAsync(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(_db.Users, "ID", "Name", application.UserID);
            return View(application);
        }

        // POST: ApplicationsAsync/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,UserID,Code,Name,Description")] Application application)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(application).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(_db.Users, "ID", "Name", application.UserID);
            return View(application);
        }

        // GET: ApplicationsAsync/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = await _db.Applications.FindAsync(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        // POST: ApplicationsAsync/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Application application = await _db.Applications.FindAsync(id);
            _db.Applications.Remove(application);
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
