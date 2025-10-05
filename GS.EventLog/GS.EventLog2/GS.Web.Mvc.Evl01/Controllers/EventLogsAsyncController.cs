using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using GS.Applications;
using GS.EventLog.DataBase1.Model;
using GS.EventLog.DataBase1.Dal;
using GS.Extension;
using GS.Web.Mvc.Evl01.ViewModels;
using Microsoft.AspNet.Identity;

namespace GS.Web.Mvc.Evl01.Controllers
{
    [Authorize]
    public class EventLogsAsyncController : Controller
    {
        private readonly EvlContext1 _db = new EvlContext1();

        // GET: EventLogs
        public async Task<ActionResult> Index()
        {
            await GSApplications.RegisterUserAsync(User.Identity.Name, User.Identity.GetUserId());

            var evls = await _db.EventLogs.ToListAsync();
            var cnt = evls.Count;
            ViewBag.Title = "EventLogs: " + cnt.ToString(CultureInfo.InvariantCulture).WithBrackets(); 
            return View(evls);
        }
        public async Task<ActionResult> Index1()
        {
            var u = await GSApplications.RegisterUserAsync(User.Identity.Name, User.Identity.GetUserId());
            var apps = await GSApplications.GetAppsByUserNameAsync(u.Name);

            var evls = await _db.EventLogs.ToListAsync();

            var vevls = evls.Join( apps,
               ev => ev.ApplicationID,
               ap => ap.ID,
               (ev, ap) => new AppEventLog
               {
                   AppID = ap.ID,
                   AppName = ap.Name,
                   EventLogID = ev.EventLogID,
                   Code = ev.Code,
                   Name = ev.Name,
                   Description = ev.Description,
                   ModifiedDT = ev.ModifiedDT ?? new DateTime()
               }).ToList();
            
            var cnt = vevls.Count();
            ViewBag.Title = "EventLogs: " + cnt.ToString(CultureInfo.InvariantCulture).WithBrackets();
            return View(vevls);
        }

        public ActionResult StatsSync()
        {
                var v1 = _db.GetEventLogItemsGrouped(true, false, false).ToList();
                var v2 = _db.GetEventLogItemsGrouped(true, false, true); // .ToList();
                var v3 = _db.GetEventLogItemsGrouped(true, true, false); // .ToList();
                var v4 = _db.GetEventLogItemsGrouped(true, true, true); // .ToList();

                v1.AddRange(v2);
                v1.AddRange(v3);
                v1.AddRange(v4);

                var v = v1.OrderBy(e => e.Id)
                    .ThenBy(e => e.Result)
                    .ThenBy(e => e.Subject)
                    .ToList();
            
            return View("Stats", v );
        }
        #region StatsAsync
        public async Task<ActionResult> StatsAsync(int? evlId)
        {
            //if (evlId == null)
            //{
                var v1 = await _db.GetEventLogItemsGroupedQ(evlId, true, false, false).ToListAsync();
                var v2 = await _db.GetEventLogItemsGroupedQ(evlId, true, false, true).ToListAsync();
                var v3 = await _db.GetEventLogItemsGroupedQ(evlId, true, true, false).ToListAsync();
                var v4 = await _db.GetEventLogItemsGroupedQ(evlId, true, true, true).ToListAsync();

                v1.AddRange(v2);
                v1.AddRange(v3);
                v1.AddRange(v4);

                var v = v1.OrderBy(e => e.Id)
                    .ThenBy(e => e.Result)
                    .ThenBy(e => e.Subject)
                    .ToList();

                ViewBag.Title = "Stats: " + v.Count.ToString(CultureInfo.InvariantCulture).WithBrackets();

                return View("Stats", v);
            //}
            //else
            //{

            //}
            //return View();
        }
        #endregion


        // GET: EventLogs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dbEventLog = await _db.EventLogs.FindAsync(id);
            if (dbEventLog == null)
            {
                return HttpNotFound();
            }
            return View(dbEventLog);
        }

        // GET: EventLogs/Create
        public ActionResult Create()
        {
            var u = GSApplications.RegisterUser(User.Identity.Name, User.Identity.GetUserId());
            var apps = GSApplications.GetAppsByUserName(u.Name);

            ViewBag.ApplicationID = new SelectList(apps, "ID", "Name");
            return View();
        }

        // POST: EventLogs/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EventLogID,ApplicationID,Alias,Code,Name,Description,LongCode")] DbEventLog dbEventLog)
        {
            if (ModelState.IsValid)
            {
                _db.EventLogs.Add(dbEventLog);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index1");
            }

            return View(dbEventLog);
        }

        // GET: EventLogs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DbEventLog dbEventLog = await _db.EventLogs.FindAsync(id);
            if (dbEventLog == null)
            {
                return HttpNotFound();
            }
            return View(dbEventLog);
        }

        // POST: EventLogs/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "EventLogID,Alias,Code,Name,Description,LongCode")] DbEventLog dbEventLog)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(dbEventLog).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(dbEventLog);
        }

        // GET: EventLogs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DbEventLog dbEventLog = await _db.EventLogs.FindAsync(id);
            if (dbEventLog == null)
            {
                return HttpNotFound();
            }
            return View(dbEventLog);
        }

        // POST: EventLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DbEventLog dbEventLog = await _db.EventLogs.FindAsync(id);
            _db.EventLogs.Remove(dbEventLog);
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
