using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using GS.EventLog.DataBase1.Model;
using GS.EventLog.DataBase1.Dal;
using GS.Extension;
using GS.Interfaces;
using Microsoft.Ajax.Utilities;
using PagedList;

namespace GS.Web.Mvc.Evl01.Controllers
{
    public class EventLogItemsAsyncController : Controller
    {
        private readonly EvlContext1 _db = new EvlContext1();

        private const int PageSize = 25;

        // GET: EventLogItems
        public async Task<ActionResult> Index(long? eventLogId, 
                                                    string evlCodeList, string resultCodeList, string subCodeList,
                                                    string searchString, 
                                                    bool? lastDayChecked,
                                                    int? page)
        {
            ViewBag.EventLogId = eventLogId;
            ViewBag.EvlCode = evlCodeList;
            ViewBag.ResultCode = resultCodeList;
            ViewBag.SubjectCode = subCodeList;
            ViewBag.SearchString = searchString;
            ViewBag.LastDayChecked = lastDayChecked;

            // isLastDayChecked = lastDayChecked.HasValue && lastDayChecked == true;

            var isEventLogIdValid = eventLogId.HasValue && eventLogId != 0;
            var isEventLogCodeValid = !string.IsNullOrWhiteSpace(evlCodeList);

            var eventLogCode = string.Empty;
            if (isEventLogIdValid)
            {
                var evl = await _db.EventLogs.FindAsync(eventLogId);
                if (evl != null)
                {
                    eventLogCode = evl.Code;
                    ViewBag.EvlCode = eventLogCode;
                }
            }
            else if (isEventLogCodeValid)
            {
                var evl = await _db.EventLogs.FirstOrDefaultAsync(e => e.Code == evlCodeList);
                if (evl != null)
                {
                    eventLogId = evl.EventLogID;
                    ViewBag.EventLogId = eventLogId;
                }
            }

            var evlis = await _db.SelectEventLogItemsAsync(eventLogId, resultCodeList, subCodeList, searchString, lastDayChecked);

            var items = await evlis
                .OrderByDescending(i => i.EventLogItemID)
                .ToListAsync();
            var eisCount = items.Count;

            // CreateFilters();
            await CreateFiltersAsync();

            var evlStr = eventLogCode.HasValue()
                ? "EventLog: " + eventLogCode + ". "
                : "EventLog: All. ";
            ViewBag.FullTitle = evlStr + "EvenLogItems (" + eisCount + ") in " + ((eisCount / PageSize) + 1) + " Pages with PageSize: " + PageSize;
            ViewBag.Title = ViewBag.FullTitle;

            int pageNumber = (page ?? 1);
            var eis = items.ToPagedList(pageNumber, PageSize);

            return View(eis);
            //return View(await db.EventLogItems.OrderByDescending(i => i.EventLogItemID).ToListAsync());
        }
        
       
        private void CreateFilters()
        {
            // Create DropDown SelectList
            //var resultsQry = from ei in db.EventLogItems
            //                select ei.ResultCode;
            //resultCodes.AddRange(resultsQry.Distinct());
            var resultCodes = new List<EvlResult>();
            resultCodes.AddRange(_db.GetResultCodes().ToList());
            ViewBag.resultCodeList = new SelectList(resultCodes);

            //var subsQry = from ei in db.EventLogItems
            //                 select ei.Subject;
            var subCodes = new List<EvlSubject>();
            subCodes.AddRange(_db.GetSubjectCodes().ToList());
            ViewBag.subCodeList = new SelectList(subCodes);

            var evls = new List<string>();
            evls.AddRange(_db.GetEventLogCodes().ToList());
            ViewBag.evlCodeList = new SelectList(evls);
        }
        private async Task  CreateFiltersAsync()
        {
            var resultCodes = EnumHelper.EnumToList<EvlResult>();

            //var resultCodes = new List<EvlResult>();
            //resultCodes.AddRange(_db.GetResultCodes().ToList());
            ViewBag.resultCodeList = new SelectList(resultCodes);

            //var subCodes = new List<EvlSubject>();
            //subCodes.AddRange(_db.GetSubjectCodes().ToList());

            var subCodes = EnumHelper.EnumToList<EvlSubject>();
            ViewBag.subCodeList = new SelectList(subCodes);

            //var evls = new List<string>();
            //await evls.AddRange(_db.GetEventLogCodes().ToListAsync());
            var evls = await _db.GetEventLogDinamicAsync();
            // ViewBag.evlCodeList = new SelectList(evls, "ID", "Code");
            ViewBag.eventLogId = new SelectList(evls, "ID", "Code");
        }

        // GET: EventLogItems/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DbEventLogItem dbEventLogItem = await _db.EventLogItems.FindAsync(id);
            if (dbEventLogItem == null)
            {
                return HttpNotFound();
            }
            return View(dbEventLogItem);
        }

        // GET: EventLogItems/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EventLogItems/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EventLogItemID,DT,ResultCode,Subject,Source,Entity,Operation,Description,Object,Index,EventLogID")] DbEventLogItem dbEventLogItem)
        {
            if (ModelState.IsValid)
            {
                _db.EventLogItems.Add(dbEventLogItem);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(dbEventLogItem);
        }

        // GET: EventLogItems/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DbEventLogItem dbEventLogItem = await _db.EventLogItems.FindAsync(id);
            if (dbEventLogItem == null)
            {
                return HttpNotFound();
            }
            return View(dbEventLogItem);
        }

        // POST: EventLogItems/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "EventLogItemID,DT,ResultCode,Subject,Source,Entity,Operation,Description,Object,Index,EventLogID")] DbEventLogItem dbEventLogItem)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(dbEventLogItem).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(dbEventLogItem);
        }

        // GET: EventLogItems/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DbEventLogItem dbEventLogItem = await _db.EventLogItems.FindAsync(id);
            if (dbEventLogItem == null)
            {
                return HttpNotFound();
            }
            return View(dbEventLogItem);
        }

        // POST: EventLogItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            DbEventLogItem dbEventLogItem = await _db.EventLogItems.FindAsync(id);
            _db.EventLogItems.Remove(dbEventLogItem);
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
