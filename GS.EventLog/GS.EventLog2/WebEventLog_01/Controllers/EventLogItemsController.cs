using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GS.EventLog.DataBase.Dal;
using GS.EventLog.DataBase.Model;
using GS.Extension;
using GS.Interfaces;
using PagedList;
using EvlContext = GS.EventLog.DataBase.Dal.EvlContext;

namespace WebEventLog_01.Controllers
{
    public class EventLogItemsController : Controller
    {
        private EvlContext db = new EvlContext();

        private int PageSize = 25;
        private bool isLastDayChecked;

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

            isLastDayChecked = lastDayChecked.HasValue && lastDayChecked == true;

            var isEventLogIdValid = eventLogId.HasValue && eventLogId != 0;
            var isEventLogCodeValid = !string.IsNullOrWhiteSpace(evlCodeList);

            var eventLogCode = string.Empty;
            if (isEventLogIdValid)
            {
                var evl = db.EventLogs.Find(eventLogId);
                if (evl != null)
                {
                    eventLogCode = evl.Code;
                    ViewBag.EvlCode = eventLogCode;
                }
            }
            else if (isEventLogCodeValid)
            {
                var evl = db.EventLogs.FirstOrDefault(e => e.Code == evlCodeList);
                if (evl != null)
                {
                    eventLogId = evl.EventLogID;
                    ViewBag.EventLogId = eventLogId;
                }
            }

            //var evlis = await db.EventLogItems.OrderByDescending(i => i.EventLogItemID).ToListAsync();

            //var evlis = await SelectViewDeals(eventLogId, resultCodeList, subCodeList, searchString, lastDayChecked);
            //var dbEventLogItems = evlis as List<DbEventLogItem> ?? evlis.ToList();
            //var eisCount = dbEventLogItems.Count();

            var evlis = db.SelectEventLogItems(eventLogId, resultCodeList, subCodeList, searchString, lastDayChecked)
                            .OrderByDescending(ei=>ei.DT);
            var eisCount = evlis.Count();

            CreateFilters();

            var evlStr = eventLogCode.HasValue()
                ? "EventLog: " + eventLogCode + ". "
                : "EventLog: All. ";
            ViewBag.FullTitle = evlStr + "EvenLogItems ( " + eisCount + " ) in " + ((eisCount / PageSize) + 1) + " Pages with PageSize: " + PageSize;
            ViewBag.Title = ViewBag.FullTitle;

            int pageNumber = (page ?? 1);
            var eis = evlis.ToPagedList(pageNumber, PageSize);

            return View(eis);
            //return View(await db.EventLogItems.OrderByDescending(i => i.EventLogItemID).ToListAsync());
        }
        public async Task<ActionResult> Index1(string resultCodeList, string subCodeList, int? page)
        {
            ViewBag.ResultCode = resultCodeList;
            ViewBag.SubjectCode = subCodeList;

            var evlis = await db.EventLogItems.OrderByDescending(i => i.EventLogItemID).ToListAsync();
            var eisCount = evlis.Count;

            CreateFilters();

            ViewBag.FullTitle = "EvenLogItems ( " + eisCount + " in " + ((eisCount / PageSize) + 1) + " Pages )";
            ViewBag.Title = ViewBag.FullTitle;

            int pageNumber = (page ?? 1);
            var eis = evlis.ToPagedList(pageNumber, PageSize);

            return View(eis);
            //return View(await db.EventLogItems.OrderByDescending(i => i.EventLogItemID).ToListAsync());
        }
        public async Task<ActionResult> Index2()
        {
            ViewBag.PageTitle = "EventLogItems (" + db.EventLogItems.Count() + ")";
            return View(await db.EventLogItems.OrderByDescending(i=>i.EventLogItemID) .ToListAsync());
        }

        #region Select EventLog Items Async from db Methods
        //private async Task<IEnumerable<DbEventLogItem>> SelectViewDeals(long? id, string result, string subject, string search,
        //                                                                        bool? lastDayChecked)
        //{
        //    IEnumerable<DbEventLogItem> deals = null;

        //    // LastDayChecked
        //    var lastDate = new DateTime().Date;
        //    isLastDayChecked = lastDayChecked.HasValue && lastDayChecked == true;
        //    if (isLastDayChecked)
        //    {
        //        var firstOrDefault = db.EventLogItems.OrderByDescending(i => i.EventLogItemID).FirstOrDefault();

        //        if (firstOrDefault == null)
        //        {
        //            ViewBag.Title = "EventLogItems List is Empty";
        //            ViewBag.LastDate = lastDate;
        //            return new List<DbEventLogItem>();
        //        }
        //        lastDate = firstOrDefault.DT.Date;
        //        ViewBag.LastDate = lastDate;
        //    }

        //    //var isHaveSearch = search.HasValue();
        //    var isResultValid = !string.IsNullOrWhiteSpace(result);
        //    if (!isResultValid) result = "";
        //    var isSubjectValid = !string.IsNullOrWhiteSpace(subject);
        //    if (!isSubjectValid) subject = "";
        //    var isSearchValid = !string.IsNullOrWhiteSpace(search);
        //    if (!isSearchValid) search = "";
        //    var isIdValid = id != null && id != 0;
        //    if (!isIdValid)
        //        id = 0;


        //    //else if (result.HasValue() && subject.HasValue())
        //    //{
        //        deals = await db.EventLogItems
        //            .Where(ei=> !isIdValid || ei.EventLogID == id)
        //            .Where(p => !isLastDayChecked || DbFunctions.TruncateTime(p.DT) == lastDate)
        //            .Where(ei => (!isResultValid || ei.ResultCode.ToString() == result) &&
        //                         (!isSubjectValid || ei.Subject.ToString() == subject))
        //            .Where(ei => !isSearchValid || ei.Source.Contains(search) || ei.Entity.Contains(search) || ei.Operation.Contains(search))
        //            .OrderByDescending(i => i.EventLogItemID).ToListAsync();
        //        return deals;
        //    //}

        //    return deals;
        //}
        //private async Task<IEnumerable<DbEventLogItem>> SelectViewDeals1(string result, string subject, string search,
        //                                                                        bool? lastDayChecked)
        //{
        //    IEnumerable<DbEventLogItem> deals = null;

        //    // LastDayChecked
        //    var lastDate = new DateTime();
        //    isLastDayChecked = lastDayChecked.HasValue && lastDayChecked == true;
        //    if (isLastDayChecked)
        //    {
        //        var firstOrDefault = db.EventLogItems.OrderByDescending(i => i.EventLogItemID).FirstOrDefault();

        //        if (firstOrDefault == null)
        //        {
        //            ViewBag.Title = "EventLogItems List is Empty";
        //            return new List<DbEventLogItem>();
        //        }
        //        lastDate = firstOrDefault.DT.Date;
        //        ViewBag.LastDate = lastDate;
        //    }

        //    var isHaveSearch = search.HasValue();

        //    if (result.HasNoValue() && subject.HasNoValue())
        //    {
        //        deals = await db.EventLogItems
        //            .Where(p => !isLastDayChecked || DbFunctions.TruncateTime(p.DT) == lastDate)
        //            .Where(ei => !isHaveSearch || ei.Source.Contains(search) || ei.Entity.Contains(search) || ei.Operation.Contains(search))
        //            .OrderByDescending(i => i.EventLogItemID).ToListAsync();
        //        return deals;
        //    }
        //    if (result.HasValue() && subject.HasNoValue())
        //    {
        //        deals = await db.EventLogItems
        //            .Where(p => !isLastDayChecked || DbFunctions.TruncateTime(p.DT) == lastDate)
        //            .Where(ei=>ei.ResultCode.ToString() == result)
        //            .Where(ei => !isHaveSearch || ei.Source.Contains(search) || ei.Entity.Contains(search) || ei.Operation.Contains(search))
        //            .OrderByDescending(i => i.EventLogItemID).ToListAsync();
        //        return deals;
        //    }
        //    else if (result.HasNoValue() && subject.HasValue())
        //    {
        //        deals = await db.EventLogItems
        //            .Where(p => !isLastDayChecked || DbFunctions.TruncateTime(p.DT) == lastDate)
        //            .Where(ei => ei.Subject.ToString() == subject)
        //            .Where(ei => !isHaveSearch || ei.Source.Contains(search) || ei.Entity.Contains(search) || ei.Operation.Contains(search))
        //            .OrderByDescending(i => i.EventLogItemID).ToListAsync();
        //        return deals;
        //    }
        //    else if (result.HasValue() && subject.HasValue())
        //    {
        //        deals = await db.EventLogItems
        //            .Where(p => !isLastDayChecked || DbFunctions.TruncateTime(p.DT) == lastDate)
        //            .Where(ei => ei.ResultCode.ToString() == result && ei.Subject.ToString() == subject)
        //            .Where(ei => !isHaveSearch || ei.Source.Contains(search) || ei.Entity.Contains(search) || ei.Operation.Contains(search))
        //            .OrderByDescending(i => i.EventLogItemID).ToListAsync();
        //        return deals;
        //    }

        //    return deals;
        //}
        #endregion
        private void CreateFilters()
        {
            // Create DropDown SelectList
            //var resultsQry = from ei in db.EventLogItems
            //                select ei.ResultCode;
            //resultCodes.AddRange(resultsQry.Distinct());
            var resultCodes = new List<EvlResult>();
            resultCodes.AddRange(db.GetResultCodes().ToList());
            ViewBag.resultCodeList = new SelectList(resultCodes);

            //var subsQry = from ei in db.EventLogItems
            //                 select ei.Subject;
            var subCodes = new List<EvlSubject>();
            subCodes.AddRange(db.GetSubjectCodes().ToList());
            ViewBag.subCodeList = new SelectList(subCodes);

            var evls = new List<string>();
            evls.AddRange(db.GetEventLogCodes().ToList());
            ViewBag.evlCodeList = new SelectList(evls);
        }

        // GET: EventLogItems/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DbEventLogItem dbEventLogItem = await db.EventLogItems.FindAsync(id);
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
                db.EventLogItems.Add(dbEventLogItem);
                await db.SaveChangesAsync();
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
            DbEventLogItem dbEventLogItem = await db.EventLogItems.FindAsync(id);
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
                db.Entry(dbEventLogItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
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
            DbEventLogItem dbEventLogItem = await db.EventLogItems.FindAsync(id);
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
            DbEventLogItem dbEventLogItem = await db.EventLogItems.FindAsync(id);
            db.EventLogItems.Remove(dbEventLogItem);
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
