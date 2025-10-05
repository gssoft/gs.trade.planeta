using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GS.Extension;
using GS.Trade.DataBase;
using GS.Trade.DataBase.Dal;
using GS.Trade.Trades;
using Microsoft.SqlServer.Server;
using Position = GS.Trade.DataBase.Model.Position;

namespace GS.Trade.Web.Mvc_07.Controllers
{
    public class PositionsController : Controller
    {
        private DbTradeContext db = new DbTradeContext();

        // GET: Positions
        public ActionResult Index(string accountList, string symbolList, string tickerList, string strategyList,
                                int? timeIntList, string searchString, bool? lastDayChecked, int? page)
        {

            var tickerQry = from d in db.Positions // .Include(d => d.Strategy)
                            //orderby d.Strategy.Ticker.Code
                            select d.Strategy.Ticker.Code;
            var tickers = new List<string>();
            tickers.AddRange(tickerQry.Distinct().OrderBy(p => p));
            //ViewBag.tickerList = new SelectList(tickers, tickerList.HasValue()?tickerList:"All");
            ViewBag.tickerList = new SelectList(tickers);

            // Symbols
            var symbols = tickers.Select(t => t.Replace(t.Right(2), ""))
                .Distinct()
                .ToList();
            ViewBag.symbolList = new SelectList(symbols);

            // Account List
            var accQry = from d in db.Positions // .Include(d => d.Strategy)
                         //orderby d.Strategy.Account.Code
                         select d.Strategy.Account.Code;
            var accs = new List<string>();
            accs.AddRange(accQry.Distinct().OrderBy(p => p));
            ViewBag.accountList = new SelectList(accs);
            // Strategy List
            var sQry = from d in db.Positions //.Include(d => d.Strategy)
                       //orderby d.Strategy.Code
                       select d.Strategy.Code;
            var ss = new List<string>();
            ss.AddRange(sQry.Distinct().OrderBy(p => p));
            ViewBag.strategyList = new SelectList(ss);

            var tiQry = from d in db.Positions //.Include(d => d.Strategy)
                        select d.Strategy.TimeInt;
            var tis = new List<int>();
            tis.AddRange(tiQry.Distinct().OrderBy(p => p));
            ViewBag.timeIntList = new SelectList(tis);

            var positions = db.Positions
                               .Where(p => p.Status != 0)
                               .Include(p => p.Strategy)
                               .OrderBy(p => p.Strategy.Account.Code)
                               .ThenBy(p => p.Strategy.Ticker.Code)
                               .ThenBy(p => p.Strategy.Code)
                               .ThenBy(p => p.Strategy.TimeInt);
            ViewBag.Title = "Positions";
            ViewBag.FullTitle = "Positions ( " + positions.Count() + " )";
            ViewBag.ItemCount = positions.Count();

            var ps = from p in positions
                select new PositionDb2
                {
                    Id = p.Id,
                    DT = p.LastTradeDT,
                    StrategyCodeEx = p.Strategy.Code,
                    AccountCodeEx = p.Strategy.Account.Code,
                    TickerCodeEx = p.Strategy.Ticker.Code,
                    TimeInt = p.Strategy.TimeInt,
                    Operation = p.Operation,
                    Quantity = p.Quantity,
                    //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                    //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                    //PnL = g.Sum(p => p.Quantity * (p.Operation == PosOperationEnum.Long ? 1 : -1) * (p.Price2 - p.Price2)),
                    PnL = p.Quantity * (int)p.Operation * (p.Price2 - p.Price1),
                    PnL3 =  0,
                    Price1 = p.Price1,
                    Price2 = p.Price2,
                    FirstTradeNumber = p.FirstTradeNumber,
                    FirstTradeDT = p.FirstTradeDT,
                    LastTradeNumber = p.LastTradeNumber,
                    LastTradeDT = p.LastTradeDT,
                    Format = p.Strategy.Ticker.Format,
                    FormatAvg = p.Strategy.Ticker.FormatAvg,
                };

            //return View(positions.ToList());
            return View(ps.ToList());
        }
        public ActionResult Index1()
        {
            //var positions = db.Positions.Include(p => p.Strategy);
            //return View(positions.ToList());
            var positions = db.Positions
                               .Where(p => p.Status != 0)
                               .Include(p => p.Strategy)
                               .OrderBy(p => p.Strategy.Account.Code)
                               .ThenBy(p => p.Strategy.Ticker.Code)
                               .ThenBy(p => p.Strategy.Code)
                               .ThenBy(p => p.Strategy.TimeInt);
            ViewBag.Title = "Positions";
            ViewBag.FullTitle = "Positions ( " + positions.Count() + " )";
            ViewBag.ItemCount = positions.Count();
            return View(positions.ToList());
        }

        public ActionResult AccountsTickers()
        {
            var v = (from p in db.Positions.Include(t => t.Strategy)
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code
                     }
                         into g
                         select new Position2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = g.Key.AccCode,
                             TickerCodeEx = g.Key.TickCode,
                             Quantity = g.Sum(p => p.Quantity * (int)p.Operation),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()

                         }).OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList();

            foreach (var p in v)
                p.Operation = p.Quantity > 0
                    ? PosOperationEnum.Long
                    : (p.Quantity < 0
                        ? PosOperationEnum.Short
                        : PosOperationEnum.Neutral);
            var cnt = v.Count;
            ViewBag.FullTitle = "Accounts & Tickers Positions ( " + cnt + " )";
            ViewBag.Title = "Accounts & Tickers Positions";
            ViewBag.Count = cnt;
            return View(v);
        }

        public ActionResult AccountsTickersStrategies()
        {
            var v = (from p in db.Positions.Include(t => t.Strategy)
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         StratKey = p.Strategy.Code
                     }
                         into g
                         select new Position2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = g.Key.AccCode,
                             TickerCodeEx = g.Key.TickCode,
                             //TimeInt = g.Key.TimeKey,
                             Quantity = g.Sum(p => p.Quantity * (int)p.Operation),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()

                         })
                //.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList();
                         .OrderBy(p => p.AccountCodeEx).ThenBy(p => p.TickerCodeEx).ThenBy(p => p.StrategyCodeEx).ToList();


            foreach (var p in v)
                p.Operation = p.Quantity > 0
                    ? PosOperationEnum.Long
                    : (p.Quantity < 0
                        ? PosOperationEnum.Short
                        : PosOperationEnum.Neutral);
            var cnt = v.Count;
            ViewBag.FullTitle = "Accounts & Strategies Positions ( " + cnt + " )";
            ViewBag.Title = "Accounts & Strategies Positions";
            ViewBag.Count = cnt;
            return View(v);
        }

        // GET: Positions/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Position position = db.Positions.Find(id);
            if (position == null)
            {
                return HttpNotFound();
            }
            return View(position);
        }

        // GET: Positions/Create
        public ActionResult Create()
        {
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key");
            return View();
        }

        // POST: Positions/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Key,StrategyId,Operation,Status,Quantity,Price1,Price2,PnL,PnL3,FirstTradeDT,FirstTradeNumber,LastTradeDT,LastTradeNumber,Created,Modified")] Position position)
        {
            if (ModelState.IsValid)
            {
                db.Positions.Add(position);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key", position.StrategyId);
            return View(position);
        }

        // GET: Positions/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Position position = db.Positions.Find(id);
            if (position == null)
            {
                return HttpNotFound();
            }
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key", position.StrategyId);
            return View(position);
        }

        // POST: Positions/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Key,StrategyId,Operation,Status,Quantity,Price1,Price2,PnL,PnL3,FirstTradeDT,FirstTradeNumber,LastTradeDT,LastTradeNumber,Created,Modified")] Position position)
        public ActionResult Edit([Bind(Include = "Id,Key,StrategyId,Operation,Status,Quantity")] Position position)
        {
            if (ModelState.IsValid)
            {
                var dt = DateTime.Now.Date;
                position.FirstTradeDT = dt;
                position.LastTradeDT = dt;
                position.Created = dt;
                position.Modified = dt;

                db.Entry(position).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key", position.StrategyId);
            return View(position);
        }

        // GET: Positions/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Position position = db.Positions.Find(id);
            if (position == null)
            {
                return HttpNotFound();
            }
            return View(position);
        }

        // POST: Positions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Position position = db.Positions.Find(id);
            db.Positions.Remove(position);
            db.SaveChanges();
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
