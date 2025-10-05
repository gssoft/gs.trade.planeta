using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GS.Trade.DataBase;
using GS.Trade.DataBase.Dal;
using GS.Trade.DataBase.Model;
using GS.Trade.Trades;
using PagedList;

namespace GS.Trade.Web.Mvc_07.Controllers
{
    public class TotalsController : Controller
    {
        private const string FormatTimeInt = "00000";
        private const int TimeIntTo = 60;
        //private const int TimeIntTo = 59;
        private DbTradeContext db = new DbTradeContext();

        // GET: Totals
        public ActionResult Index( int? page)
        {
            //var positions = db.Positions.Include(t => t.Strategy);
            //return View(positions.ToList());
            var totals = db.Totals.Include(t => t.Strategy);
            var totordered =
                totals.OrderBy(o => o.Strategy.Account.Code)
                        .ThenBy(o => o.Strategy.Ticker.Code)
                        .ThenBy(o => o.Strategy.Code)
                        .ThenBy(o => o.Strategy.TimeInt).ToList();
            ViewBag.Title = "Totals";
            ViewBag.FullTitle = "Totals ( " + totordered.Count() + " )";
            ViewBag.ItemCount = totordered.Count();

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(totordered.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Accounts()
        {
            //var totals = db.Totals.Include(t => t.Strategy).ToList();
            var v = (from p in db.Totals.Include(t => t.Strategy)
                     .Where(p => p.Strategy.TimeInt <= TimeIntTo)
                     group p by p.Strategy.Account.Code
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = g.Key,
                             TickerCodeEx = "All",
                             Quantity = g.Sum(p => p.Quantity),
                             // PnL = g.Sum(p=>(short)p.Operation*p.Quantity*(p.Price2-p.Price1)),
                             PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             // PnL = g.Sum(p=>p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()

                         }).OrderBy(p => p.AccountCodeEx).ToList();

            //ViewBag.Title = "Accounts Totals";
            return View(v);
        }

        public ActionResult AccountsTickers()
        {
            var v = (from p in db.Totals.Include(t => t.Strategy)
                         .Where(p => p.Strategy.TimeInt <= TimeIntTo)
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = g.Key.AccCode,
                             TickerCodeEx = g.Key.TickCode,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = g.Key.Format,
                             FormatAvg = g.Key.FormatAvg,
                             Count = g.Count()

                         }).OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList();

            ViewBag.Title = "Accounts & Tickers Totals";
            return View(v);
        }
        public ActionResult AccountsTickersStrategies()
        {
            var v = (from p in db.Totals.Include(t => t.Strategy).Where(p => p.Strategy.TimeInt <= TimeIntTo)
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = g.Key.AccCode,
                             TickerCodeEx = g.Key.TickCode,
                             //TimeInt = g.Key.TimeKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = g.Key.Format,
                             FormatAvg = g.Key.FormatAvg,
                             Count = g.Count()

                         })
                //.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList();
                         .OrderBy(p => p.AccountCodeEx).ThenBy(p => p.TickerCodeEx).ThenBy(p => p.StrategyCodeEx).ToList();

            ViewBag.Title = "Accounts & Tickers & Strategies Totals";
            return View(v);
        }
        public ActionResult AccountsTickersStrategiesEq()
        {
            //var v1 = (from p in db.Deals.Include(t => t.Strategy)
            //          select new
            //          {
            //              Dt = p.DT,
            //              AccKey = p.Strategy.Account.Key,
            //              AccCode = p.Strategy.Account.Code,
            //              TickerKey = p.Strategy.Ticker.Key,
            //              TickCode = p.Strategy.Ticker.Code,
            //              StratKey = p.Strategy.Code,
            //              Q = p.Quantity,
            //              PnL = p.PnL1,
            //              dt1 = p.FirstTradeDT,
            //              dt2 = p.LastTradeDT
            //          }).ToList();

            //var v2 = (from p in v1
            //          group p by new
            //          {
            //              DtKey = DT.ToString("d"),
            //              AccKey1 = AccKey,
            //              AccCode1 = AccCode,
            //              TickKey = TickerKey,
            //              TickCode = TickerCode,
            //              StratKey1 = StratKey
            //          }
            //              into g
            //              select new PositionDb2
            //              {
            //                  DT = DateTime.Parse(g.Key.DtKey),
            //                  StrategyCodeEx = g.Key.StratKey,
            //                  AccountCodeEx = g.Key.AccCode,
            //                  TickerCodeEx = g.Key.TickCode,
            //                  //TimeInt = g.Key.TimeKey,
            //                  Quantity = g.Sum(p => p.Quantity),
            //                  //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
            //                  //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
            //                  PnL = g.Sum(p => p.PnL1),
            //                  PnL3 = g.Sum(p => p.PnL1),
            //                  FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
            //                  FirstTradeDT = g.Min(p => p.FirstTradeDT),
            //                  LastTradeNumber = g.Max(p => p.LastTradeNumber),
            //                  LastTradeDT = g.Max(p => p.LastTradeDT),
            //                  Count = g.Count()

            //              });
            ////);
            var v = (from p in db.Deals.Include(t => t.Strategy).Where(p => p.Strategy.TimeInt <= TimeIntTo)
                     group p by new
                     {
                         DtKey = DbFunctions.TruncateTime(p.DT),
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         StratKey = p.Strategy.Code
                     }
                         into g
                         select new PositionDb2
                         {
                             DT = (DateTime)g.Key.DtKey,
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = g.Key.AccCode,
                             TickerCodeEx = g.Key.TickCode,
                             //TimeInt = g.Key.TimeKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Operation == PosOperationEnum.Long ? 1 : -1) * (p.Price2 - p.Price2)),
                             PnL = g.Sum(p => p.Quantity * (int)p.Operation * (p.Price2 - p.Price1)),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()
                         })
                //.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList();
                         .OrderBy(p => p.AccountCodeEx)
                         .ThenBy(p => p.TickerCodeEx)
                         .ThenBy(p => p.StrategyCodeEx)
                         .ThenBy(p => p.DT).ToList();

            ViewBag.Title = "Accounts & Tickers & Strategies Equity";
            return View(v);
        }
        public ActionResult AccountsTickersStrategiesLastDay()
        {
            // var lastDate = DateTime.Now.Date;
            var firstOrDefault = db.Deals.OrderByDescending(p => p.LastTradeDT).FirstOrDefault();
            //var firstOrDefault = db.Deals.Where(p=>p.LastTradeDT != SqlDateTime.MinValue)
            //    .OrderByDescending(p => p.LastTradeDT).FirstOrDefault();
            if (firstOrDefault == null)
            {
                ViewBag.Title = "Accounts & Tickers & Strategies View is Empty";
                return View(new List<PositionDb2>());
            }
            var lastDate = firstOrDefault.LastTradeDT.Date;

            var v = (from p in db.Deals.Include(t => t.Strategy)
                         .Where(p => p.Strategy.TimeInt <= TimeIntTo)
                         .Where(p => DbFunctions.TruncateTime(p.LastTradeDT) == lastDate)
                     group p by new
                     {
                         DtKey = DbFunctions.TruncateTime(p.DT),
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code,
                         //StratID = p.Strategy.Id
                     }
                         into g
                         select new PositionDb2
                         {
                             //Id = g.Key.StratID,
                             DT = (DateTime)g.Key.DtKey,
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = g.Key.AccCode,
                             TickerCodeEx = g.Key.TickCode,
                             //TimeInt = g.Key.TimeKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Operation == PosOperationEnum.Long ? 1 : -1) * (p.Price2 - p.Price2)),
                             PnL = g.Sum(p => p.Quantity * (int)p.Operation * (p.Price2 - p.Price1)),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = g.Key.Format,
                             FormatAvg = g.Key.FormatAvg,
                             Count = g.Count()
                         })
                //.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList();
                         .OrderBy(p => p.AccountCodeEx)
                         .ThenBy(p => p.TickerCodeEx)
                         .ThenBy(p => p.StrategyCodeEx)
                         .ThenBy(p => p.DT).ToList();

            foreach (var i in v)
            {
                PositionDb2 i1 = i;
                var ps = db.Positions
                    .Where(p => p.Strategy.Code == i1.StrategyCodeEx &&
                                p.Strategy.Account.Code == i1.AccountCodeEx &&
                                p.Strategy.Ticker.Code == i1.TickerCodeEx &&
                                p.Status != 0
                                ).ToList();
                foreach (var p in ps)
                    i1.PnL3 += p.PnL;
            }
            ViewBag.Title = "Accounts & Tickers & Strategies Results at Date=" + lastDate.ToString("d");
            return View(v);
        }
        public ActionResult AccountsTickersStrategiesTimeIntLastDay()
        {
            var firstOrDefault = db.Deals.OrderByDescending(p => p.LastTradeDT).FirstOrDefault();
            if (firstOrDefault == null)
                return View(new List<PositionDb2>());
            var lastDate = firstOrDefault.LastTradeDT.Date;

            var v = (from p in db.Deals.Include(t => t.Strategy)
                        .Where(p => p.Strategy.TimeInt <= TimeIntTo)
                        .Where(p => DbFunctions.TruncateTime(p.LastTradeDT) == lastDate)
                     group p by new
                     {
                         DtKey = DbFunctions.TruncateTime(p.DT),
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code,
                         TimeKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             DT = (DateTime)g.Key.DtKey,
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = g.Key.AccCode,
                             TickerCodeEx = g.Key.TickCode,
                             TimeInt = g.Key.TimeKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Operation == PosOperationEnum.Long ? 1 : -1) * (p.Price2 - p.Price2)),
                             PnL = g.Sum(p => p.Quantity * (int)p.Operation * (p.Price2 - p.Price1)),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = g.Key.Format,
                             FormatAvg = g.Key.FormatAvg,
                             Count = g.Count()
                         })
                //.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList();
                         .OrderBy(p => p.AccountCodeEx)
                         .ThenBy(p => p.TickerCodeEx)
                         .ThenBy(p => p.StrategyCodeEx)
                         .ThenBy(p => p.TimeInt)
                         .ThenBy(p => p.DT).ToList();

            ViewBag.Title = "Accounts & Tickers & Strategies & TimeInt Results at Date=" + lastDate.ToString("d");
            return View(v);
        }

        public ActionResult AccountsTickersTimeInts()
        {
            var v = (from p in db.Totals.Include(t => t.Strategy)
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         TickKey = p.Strategy.Ticker.Key,
                         AccCode = p.Strategy.Account.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         TickCode = p.Strategy.Ticker.Code,
                         TimeKey = p.Strategy.TimeInt
                         //TimeKey = p.Strategy.TimeInt.ToString(CultureInfo.InvariantCulture)
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = g.Key.AccCode,
                             TickerCodeEx = g.Key.TickCode,
                             TimeInt = g.Key.TimeKey,
                             //TimeIntKey = ((Int32)(g.Key.TimeKey)).ToString(CultureInfo.InvariantCulture), 
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             // PnL = g.Sum(p=>p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = g.Key.Format,
                             FormatAvg = g.Key.FormatAvg,
                             Count = g.Count()
                         }).ToList();
            //}).OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList(); 
            //.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx /* + p.TimeInt */).ToList();

            var vv = v.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx + p.TimeInt.ToString(FormatTimeInt)).ToList();
            //ViewBag.Title = "Accounts & Tickers & TimeIntervals Totals";
            return View(vv);
        }
        public ActionResult AccountsTimeInts()
        {
            var v = (from p in db.Totals.Include(t => t.Strategy)
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         TimeKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = g.Key.AccCode,
                             TimeInt = g.Key.TimeKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()

                         }).ToList();
            var vv = v.OrderBy(p => p.AccountCodeEx + p.TimeInt.ToString(FormatTimeInt)).ToList();

            ViewBag.Title = "Accounts & TimeIntervals Totals";
            return View(vv);
        }

        public ActionResult Tickers()
        {
            var v = (from p in db.Totals.Include(t => t.Strategy)
                         .Where(p => p.Strategy.TimeInt <= TimeIntTo)
                     group p by new
                     {
                         TickerKey = p.Strategy.Ticker.Key,
                         TickerCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = "All",
                             //TickerCodeEx = g.Key.TickerCode,
                             TickerCodeEx = g.Key.TickerCode,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             // PnL = g.Sum(p=>p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = g.Key.Format,
                             FormatAvg = g.Key.FormatAvg,
                             Count = g.Count()
                         }).OrderBy(p => p.TickerCodeEx).ToList();

            ViewBag.Title = "Tickers Totals";
            return View(v);
        }
        public ActionResult TickersStrategies()
        {
            var v = (from p in db.Totals.Include(t => t.Strategy).Where(p => p.Strategy.TimeInt <= TimeIntTo)
                     group p by new
                     {
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         StratKey = p.Strategy.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = "All",
                             TickerCodeEx = g.Key.TickCode,
                             //TimeInt = g.Key.TimeKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = g.Key.Format,
                             FormatAvg = g.Key.FormatAvg,
                             Count = g.Count()

                         }).OrderBy(p => p.TickerCodeEx).ThenBy(p => p.StrategyCodeEx).ToList();

            ViewBag.Title = "Tickers & Strategies Totals";
            return View(v);
        }
        public ActionResult TickersStrategiesTimeIntervals()
        {
            var v = (from p in db.Totals.Include(t => t.Strategy)
                     group p by new
                     {
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         StratKey = p.Strategy.Code,
                         TimeKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = "All",
                             TickerCodeEx = g.Key.TickCode,
                             TimeInt = g.Key.TimeKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()

                         }).ToList();
            var vv = v.OrderBy(p => p.TickerCodeEx + p.StrategyCodeEx + p.TimeInt.ToString(FormatTimeInt)).ToList();

            ViewBag.Title = "Tickers & Strategies & TimeIntervals Totals";
            return View(vv);
        }

        public ActionResult TickersTimeInts()
        {
            var v = (from p in db.Totals.Include(t => t.Strategy)
                     group p by new
                     {
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         TimeKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = "All",
                             TickerCodeEx = g.Key.TickCode,
                             TimeInt = g.Key.TimeKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()

                         }).ToList();
            var vv = v.OrderBy(p => p.TickerCodeEx + p.TimeInt.ToString(FormatTimeInt)).ToList();

            ViewBag.Title = "Tickers & TimeIntervals Totals";
            return View(vv);
        }

        public ActionResult Strategies()
        {
            var v = (from p in db.Totals.Include(t => t.Strategy).Where(p => p.Strategy.TimeInt <= TimeIntTo)
                     group p by p.Strategy.Code
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key,
                             AccountCodeEx = "All",
                             TickerCodeEx = "All",
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()
                         }).OrderBy(p => p.StrategyCodeEx).ToList();

            ViewBag.Title = "Strategies Totals";
            return View(v);
        }

        public ActionResult TimeIntervals()
        {
            var v = (from p in db.Totals.Include(t => t.Strategy)
                     group p by p.Strategy.TimeInt
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = "All",
                             TickerCodeEx = "All",
                             TimeInt = g.Key,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()
                         }).ToList();
            var vv = v.OrderBy(p => p.TimeInt.ToString(FormatTimeInt)).ToList();

            ViewBag.Title = "TimeIntervals Totals";
            return View(vv);
        }
        // **********************************
        //  CRUD

        // GET: Totals/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Total total = db.Totals.Find(id);
            if (total == null)
            {
                return HttpNotFound();
            }
            return View(total);
        }

        // GET: Totals/Create
        public ActionResult Create()
        {
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key");
            return View();
        }

        // POST: Totals/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Key,StrategyId,Operation,Status,Quantity,Price1,Price2,PnL,PnL3,FirstTradeDT,FirstTradeNumber,LastTradeDT,LastTradeNumber,Created,Modified")] Total total)
        {
            if (ModelState.IsValid)
            {
                db.Positions.Add(total);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key", total.StrategyId);
            return View(total);
        }

        // GET: Totals/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Total total = db.Totals.Find(id);
            if (total == null)
            {
                return HttpNotFound();
            }
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key", total.StrategyId);
            return View(total);
        }

        // POST: Totals/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Key,StrategyId,Operation,Status,Quantity,Price1,Price2,PnL,PnL3,FirstTradeDT,FirstTradeNumber,LastTradeDT,LastTradeNumber,Created,Modified")] Total total)
        {
            if (ModelState.IsValid)
            {
                db.Entry(total).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key", total.StrategyId);
            return View(total);
        }

        // GET: Totals/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Total total = db.Totals.Find(id);
            if (total == null)
            {
                return HttpNotFound();
            }
            return View(total);
        }

        // POST: Totals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Total total = db.Totals.Find(id);
            db.Positions.Remove(total);
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
