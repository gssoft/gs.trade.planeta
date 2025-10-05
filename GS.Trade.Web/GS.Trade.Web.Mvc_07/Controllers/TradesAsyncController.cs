using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GS.Extension;
using GS.Trade.DataBase.Dal;
using GS.Trade.DataBase.Model;
using PagedList;

namespace GS.Trade.Web.Mvc_07.Controllers
{
    using DataBase.Model;
    public class TradesAsyncController : Controller
    {
        private DbTradeContext db = new DbTradeContext();
        private const int PageSize = 25;
        private bool isLastDayChecked;

        // GET: TradesAsync
        public ActionResult Index(string accountList, string tickerList, string strategyList, int? timeIntList,
                       bool? lastDayChecked, int? page)
        {
            string accountSelected = accountList;
            string tickerSelected = tickerList;
            string strategySelected = strategyList;

            ViewBag.Account = accountSelected;
            ViewBag.Ticker = tickerSelected;
            ViewBag.Strategy = strategySelected;
            ViewBag.TimeInt = timeIntList;
            ViewBag.LastDayChecked = lastDayChecked;

            isLastDayChecked = lastDayChecked.HasValue && lastDayChecked == true;

            var deals = SelectViewDeals(accountList, tickerList, strategyList, timeIntList, lastDayChecked);
            //var total = deals != null
            //                ? deals.Sum(d => d.PnL)
            //                : 0;

            // Create DropDown SelectList
            // Ticker List
            var tickerQry = from d in db.Deals.Include(d => d.Strategy)
                            //orderby d.Strategy.Ticker.Code
                            select d.Strategy.Ticker.Code;
            var tickers = new List<string>();
            tickers.AddRange(tickerQry.Distinct().OrderBy(p => p));
            //ViewBag.tickerList = new SelectList(tickers, tickerList.HasValue()?tickerList:"All");
            ViewBag.tickerList = new SelectList(tickers);
            // Account List
            var accQry = from d in db.Deals.Include(d => d.Strategy)
                         //orderby d.Strategy.Account.Code
                         select d.Strategy.Account.Code;
            var accs = new List<string>();
            accs.AddRange(accQry.Distinct().OrderBy(p => p));
            ViewBag.accountList = new SelectList(accs);
            // Strategy List
            var sQry = from d in db.Deals.Include(d => d.Strategy)
                       //orderby d.Strategy.Code
                       select d.Strategy.Code;
            var ss = new List<string>();
            ss.AddRange(sQry.Distinct().OrderBy(p => p));
            ViewBag.strategyList = new SelectList(ss);

            var tiQry = from d in db.Deals.Include(d => d.Strategy)
                        select d.Strategy.TimeInt;
            var tis = new List<int>();
            tis.AddRange(tiQry.Distinct().OrderBy(p => p));
            ViewBag.timeIntList = new SelectList(tis);
            //ViewBag.lastDayChecked = new CheckBox();

            var dealsCount = deals.Count();
            ViewBag.Title = "Deals";
            string lastDate= string.Empty;
            if(ViewBag.LastDate != null)
                lastDate = isLastDayChecked ? " LastDate = " + ViewBag.LastDate.ToString("d") : "";
            ViewBag.FullTitle = "Deals ( " + dealsCount + " ) in " + ((dealsCount / PageSize) + 1)
                                + " Pages with PageSize: " + PageSize
                                //+ " Totals = " + total.ToString("N4")
                                + lastDate;
            ViewBag.ItemCount = deals.Count();

            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(deals.ToPagedList(pageNumber, PageSize));

            //return View(deals);
        }
        public async Task<ActionResult> Index1()
        {
            var trades = db.Trades.Include(t => t.Strategy);
            return View(await trades.ToListAsync());
        }
        private IEnumerable<Trade> SelectViewDeals(string accountList, string tickerList, string strategyList, int? timeInt,
            bool? lastDayChecked)
        {
            IEnumerable<Trade> deals = null;

            // LastDayChecked
            var lastDate = new DateTime();
            isLastDayChecked = lastDayChecked.HasValue && lastDayChecked == true;
            if (isLastDayChecked)
            {
                var firstOrDefault = db.Trades.OrderByDescending(p => p.DT).FirstOrDefault();

                if (firstOrDefault == null)
                {
                    ViewBag.Title = "Trades View is Empty";
                    ViewBag.LastDate = lastDate;
                    return new List<Trade>();
                }
                lastDate = firstOrDefault.DT.Date;
                ViewBag.LastDate = lastDate;
            }

            if (accountList.HasNoValue() && tickerList.HasNoValue() && strategyList.HasNoValue())
            {

                deals = db.Trades
                            .Where(p => !isLastDayChecked || DbFunctions.TruncateTime(p.DT) == lastDate)
                            .Where(p => !timeInt.HasValue || p.Strategy.TimeInt == timeInt)
                         //.Include(d => d.Strategy)
                         //select new ViewDeal
                         //{
                         //    Id = d.Id,
                         //    Account = d.Strategy.Account.Code,
                         //    Ticker = d.Strategy.Ticker.Code,
                         //    Strategy = d.Strategy.Code,
                         //    TimeInt = d.Strategy.TimeInt,
                         //    Side = d.Operation,
                         //    Qty = d.Quantity,
                         //    Price1 = d.Price1,
                         //    Price2 = d.Price2,
                         //    FirstTradeDT = d.FirstTradeDT,
                         //    LastTradeDT = d.LastTradeDT,
                         //    Format = d.Strategy.Ticker.Format,
                         //    FormatAvg = d.Strategy.Ticker.FormatAvg,
                         //}
                         //)
                    .OrderByDescending(d => d.DT)
                    .ToList();

                return deals;
            }
            if (accountList.HasValue() && tickerList.HasNoValue() && strategyList.HasNoValue())
            {
                deals = db.Trades
                    .Where(p => !isLastDayChecked || DbFunctions.TruncateTime(p.DT) == lastDate)
                    .Where(d => d.Strategy.Account.Code == accountList)
                    .Where(p => !timeInt.HasValue || p.Strategy.TimeInt == timeInt)
                         //// .Include(d => d.Strategy)
                         //select new ViewDeal
                         //{
                         //    Id = d.Id,
                         //    Account = d.Strategy.Account.Code,
                         //    Ticker = d.Strategy.Ticker.Code,
                         //    Strategy = d.Strategy.Code,
                         //    TimeInt = d.Strategy.TimeInt,
                         //    Side = d.Operation,
                         //    Qty = d.Quantity,
                         //    Price1 = d.Price1,
                         //    Price2 = d.Price2,
                         //    FirstTradeDT = d.FirstTradeDT,
                         //    LastTradeDT = d.LastTradeDT,
                         //    Format = d.Strategy.Ticker.Format,
                         //    FormatAvg = d.Strategy.Ticker.FormatAvg,
                         //})
                    .OrderByDescending(d => d.DT)
                    .ToList();
                //   return deals;
            }
            else if (accountList.HasValue() && tickerList.HasValue() && strategyList.HasNoValue())
            {
                deals = db.Trades
                            .Where(p => !isLastDayChecked || DbFunctions.TruncateTime(p.DT) == lastDate)
                            .Where(d => d.Strategy.Account.Code == accountList &&
                                    d.Strategy.Ticker.Code == tickerList)
                            .Where(p => !timeInt.HasValue || p.Strategy.TimeInt == timeInt)
                         // .Include(d => d.Strategy)
                         //select new ViewDeal
                         //{
                         //    Id = d.Id,
                         //    Account = d.Strategy.Account.Code,
                         //    Ticker = d.Strategy.Ticker.Code,
                         //    Strategy = d.Strategy.Code,
                         //    TimeInt = d.Strategy.TimeInt,
                         //    Side = d.Operation,
                         //    Qty = d.Quantity,
                         //    Price1 = d.Price1,
                         //    Price2 = d.Price2,
                         //    FirstTradeDT = d.FirstTradeDT,
                         //    LastTradeDT = d.LastTradeDT,
                         //    Format = d.Strategy.Ticker.Format,
                         //    FormatAvg = d.Strategy.Ticker.FormatAvg,
                         //})
                    .OrderByDescending(d => d.DT)
                    .ToList();
            }
            else if (accountList.HasValue() && tickerList.HasNoValue() && strategyList.HasValue())
            {
                deals = db.Trades
                            .Where(p => !isLastDayChecked || DbFunctions.TruncateTime(p.DT) == lastDate)
                            .Where(d => d.Strategy.Account.Code == accountList &&
                                d.Strategy.Code == strategyList)
                            .Where(p => !timeInt.HasValue || p.Strategy.TimeInt == timeInt)
                         //.Include(d => d.Strategy)
                         //select new ViewDeal
                         //{
                         //    Id = d.Id,
                         //    Account = d.Strategy.Account.Code,
                         //    Ticker = d.Strategy.Ticker.Code,
                         //    Strategy = d.Strategy.Code,
                         //    TimeInt = d.Strategy.TimeInt,
                         //    Side = d.Operation,
                         //    Qty = d.Quantity,
                         //    Price1 = d.Price1,
                         //    Price2 = d.Price2,
                         //    FirstTradeDT = d.FirstTradeDT,
                         //    LastTradeDT = d.LastTradeDT,
                         //    Format = d.Strategy.Ticker.Format,
                         //    FormatAvg = d.Strategy.Ticker.FormatAvg,
                         //})
                    .OrderByDescending(d => d.DT)
                    .ToList();
            }
            else if (accountList.HasValue() && tickerList.HasValue() && strategyList.HasValue())
            {
                deals = db.Trades
                            .Where(p => !isLastDayChecked || DbFunctions.TruncateTime(p.DT) == lastDate)
                            .Where(d => d.Strategy.Account.Code == accountList &&
                                d.Strategy.Ticker.Code == tickerList &&
                                d.Strategy.Code == strategyList)
                            .Where(p => !timeInt.HasValue || p.Strategy.TimeInt == timeInt)
                         // .Include(d => d.Strategy)
                         //select new ViewDeal
                         //{
                         //    Id = d.Id,
                         //    Account = d.Strategy.Account.Code,
                         //    Ticker = d.Strategy.Ticker.Code,
                         //    Strategy = d.Strategy.Code,
                         //    TimeInt = d.Strategy.TimeInt,
                         //    Side = d.Operation,
                         //    Qty = d.Quantity,
                         //    Price1 = d.Price1,
                         //    Price2 = d.Price2,
                         //    FirstTradeDT = d.FirstTradeDT,
                         //    LastTradeDT = d.LastTradeDT,
                         //    Format = d.Strategy.Ticker.Format,
                         //    FormatAvg = d.Strategy.Ticker.FormatAvg,
                         //})
                    .OrderByDescending(d => d.DT)
                    .ToList();
            }
            else if (accountList.HasNoValue() && tickerList.HasValue() && strategyList.HasNoValue())
            {
                deals = db.Trades
                            .Where(p => !isLastDayChecked || DbFunctions.TruncateTime(p.DT) == lastDate)
                            .Where(d => d.Strategy.Ticker.Code == tickerList)
                            .Where(p => !timeInt.HasValue || p.Strategy.TimeInt == timeInt)
                         //.Include(d => d.Strategy)
                         //select new ViewDeal
                         //{
                         //    Id = d.Id,
                         //    Account = d.Strategy.Account.Code,
                         //    Ticker = d.Strategy.Ticker.Code,
                         //    Strategy = d.Strategy.Code,
                         //    TimeInt = d.Strategy.TimeInt,
                         //    Side = d.Operation,
                         //    Qty = d.Quantity,
                         //    Price1 = d.Price1,
                         //    Price2 = d.Price2,
                         //    FirstTradeDT = d.FirstTradeDT,
                         //    LastTradeDT = d.LastTradeDT,
                         //    Format = d.Strategy.Ticker.Format,
                         //    FormatAvg = d.Strategy.Ticker.FormatAvg,
                         //})
                    .OrderByDescending(d => d.DT)
                    .ToList();
            }
            else if (accountList.HasNoValue() && tickerList.HasNoValue() && strategyList.HasValue())
            {
                deals = db.Trades
                            .Where(p => !isLastDayChecked || DbFunctions.TruncateTime(p.DT) == lastDate)
                            .Where(d => d.Strategy.Code == strategyList)
                            .Where(p => !timeInt.HasValue || p.Strategy.TimeInt == timeInt)
                         //.Include(d => d.Strategy)
                         //select new ViewDeal
                         //{
                         //    Id = d.Id,
                         //    Account = d.Strategy.Account.Code,
                         //    Ticker = d.Strategy.Ticker.Code,
                         //    Strategy = d.Strategy.Code,
                         //    TimeInt = d.Strategy.TimeInt,
                         //    Side = d.Operation,
                         //    Qty = d.Quantity,
                         //    Price1 = d.Price1,
                         //    Price2 = d.Price2,
                         //    FirstTradeDT = d.FirstTradeDT,
                         //    LastTradeDT = d.LastTradeDT,
                         //    Format = d.Strategy.Ticker.Format,
                         //    FormatAvg = d.Strategy.Ticker.FormatAvg,
                         //})
                    .OrderByDescending(d => d.DT)
                    .ToList();
            }
            else if (accountList.HasNoValue() && tickerList.HasValue() && strategyList.HasValue())
            {
                deals = db.Trades
                            .Where(p => !isLastDayChecked || DbFunctions.TruncateTime(p.DT) == lastDate)
                            .Where(d => d.Strategy.Ticker.Code == tickerList &&
                                        d.Strategy.Code == strategyList)
                            .Where(p => !timeInt.HasValue || p.Strategy.TimeInt == timeInt)
                         //.Include(d => d.Strategy)
                         //select new ViewDeal
                         //{
                         //    Id = d.Id,
                         //    Account = d.Strategy.Account.Code,
                         //    Ticker = d.Strategy.Ticker.Code,
                         //    Strategy = d.Strategy.Code,
                         //    TimeInt = d.Strategy.TimeInt,
                         //    Side = d.Operation,
                         //    Qty = d.Quantity,
                         //    Price1 = d.Price1,
                         //    Price2 = d.Price2,
                         //    FirstTradeDT = d.FirstTradeDT,
                         //    LastTradeDT = d.LastTradeDT,
                         //    Format = d.Strategy.Ticker.Format,
                         //    FormatAvg = d.Strategy.Ticker.FormatAvg,
                         //})
                    .OrderByDescending(d => d.DT)
                    .ToList();
            }

            return deals;
        }
        // GET: TradesAsync/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trade trade = await db.Trades.FindAsync(id);
            if (trade == null)
            {
                return HttpNotFound();
            }
            return View(trade);
        }

        // GET: TradesAsync/Create
        public ActionResult Create()
        {
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key");
            return View();
        }

        // POST: TradesAsync/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Key,DT,Number,Operation,Quantity,Price,OrderNumber,StrategyId,Created,Modified")] Trade trade)
        {
            if (ModelState.IsValid)
            {
                db.Trades.Add(trade);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key", trade.StrategyId);
            return View(trade);
        }

        // GET: TradesAsync/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trade trade = await db.Trades.FindAsync(id);
            if (trade == null)
            {
                return HttpNotFound();
            }
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key", trade.StrategyId);
            return View(trade);
        }

        // POST: TradesAsync/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Key,DT,Number,Operation,Quantity,Price,OrderNumber,StrategyId,Created,Modified")] Trade trade)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trade).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key", trade.StrategyId);
            return View(trade);
        }

        // GET: TradesAsync/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trade trade = await db.Trades.FindAsync(id);
            if (trade == null)
            {
                return HttpNotFound();
            }
            return View(trade);
        }

        // POST: TradesAsync/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Trade trade = await db.Trades.FindAsync(id);
            db.Trades.Remove(trade);
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
