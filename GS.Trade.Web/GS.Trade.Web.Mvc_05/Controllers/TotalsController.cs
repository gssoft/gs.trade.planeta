using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.Expressions;
using GS.Trade.DataBase;
using GS.Trade.DataBase.Model;
using GS.Trade.DataBase.Dal;
using GS.Trade.Trades;
using Microsoft.Ajax.Utilities;

namespace GS.Trade.Web.Mvc_05.Controllers
{
    public class TotalsController : Controller
    {
        private const string FormatTimeInt = "00000";
        private const int TimeIntTo = 60;
        //private const int TimeIntTo = 59;
        private DbTradeContext db = new DbTradeContext("DbTrade2");

        //
        // GET: /Totals/

        public ActionResult Index()
        {
            var totals = db.Totals.Include(t => t.Strategy);
            var totordered =
                totals.OrderBy(o => o.Strategy.Account.Code)
                        .ThenBy(o=>o.Strategy.Ticker.Code)
                        .ThenBy(o=>o.Strategy.Code)
                        .ThenBy(o=>o.Strategy.TimeInt);
            ViewBag.Title = "Totals";
            ViewBag.FullTitle = "Totals ( " + totordered.Count() + " )";
            ViewBag.ItemCount = totordered.Count();
            return View(totordered.ToList());
        }
        public ActionResult Index2()
        {
            var totals = db.Totals.Include(t => t.Strategy);
            var tsordered = totals.OrderBy(o => o.Strategy.Account + o.Strategy.Ticker.Name + o.Strategy.Name + o.Strategy.TimeInt);
            return View(tsordered.ToList());
        }

        // GET: /Totals/Accounts
        public ActionResult Accounts()
        {
            //var totals = db.Totals.Include(t => t.Strategy).ToList();
            var v = (from p in db.Totals.Include(t => t.Strategy)
                     .Where(p => p.Strategy.TimeInt <= TimeIntTo)
                     group p by p.Strategy.Account.Code
                         into g
                         select new Position2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = g.Key,
                             TickerCodeEx = "All",
                             Quantity = g.Sum(p=>p.Quantity),
                             // PnL = g.Sum(p=>(short)p.Operation*p.Quantity*(p.Price2-p.Price1)),
                             PnL =  g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                            // PnL = g.Sum(p=>p.PnL),
                             PnL3 =  g.Sum(p=>p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()
                             
                         }).OrderBy(p=>p.AccountCodeEx).ToList();

            //ViewBag.Title = "Accounts Totals";
            return View(v);
        }
        public ActionResult Accounts2()
        {
            var totals = (from p in db.Totals.Include(t => t.Strategy).Where(t => t.Strategy.Code != "Default")
                          group p by new { AccKey = p.Strategy.Account.Key}
                              into g
                              select new Position2
                              {
                                  StrategyCodeEx = g.Key.AccKey,
                                  AccountCodeEx = g.Key.AccKey,
                                  TickerCodeEx = "All",
                                  Quantity = g.Sum(p => p.Quantity),
                                  //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                                  PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                                  // PnL = g.Sum(p=>p.PnL),
                                  PnL3 = g.Sum(p => p.PnL3),
                                  FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                                  FirstTradeDT = g.Min(p => p.FirstTradeDT),
                                  LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                                  LastTradeDT = g.Max(p => p.LastTradeDT),
                                  Count = g.Count()
                              }).ToList();

            var currents = (from p in db.Positions.Include(t => t.Strategy).Where(t => t.Strategy.Code != "Default")
                            group p by new { AccKey = p.Strategy.Account.Key}
                                into g
                                select new Position2
                                {
                                    StrategyCodeEx = g.Key.AccKey,
                                    AccountCodeEx = g.Key.AccKey,
                                    TickerCodeEx = "All",
                                    Quantity = g.Sum(p => (short)p.Operation * p.Quantity),
                                    PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                                    // PnL = g.Sum(p=>p.PnL),
                                    PnL3 = g.Sum(p => p.PnL3),
                                    FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                                    FirstTradeDT = g.Min(p => p.FirstTradeDT),
                                    LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                                    LastTradeDT = g.Max(p => p.LastTradeDT),
                                    Count = g.Count()

                                }).ToList();
            foreach (var p in currents)
            {
                if (p.Quantity < 0)
                {
                    p.Quantity = -1 * p.Quantity;
                    p.Status = PosStatusEnum.Opened;
                    p.Operation = PosOperationEnum.Short;
                }
                else if (p.Quantity > 0)
                {
                    p.Status = PosStatusEnum.Opened;
                    p.Operation = PosOperationEnum.Long;
                }
                else
                {
                    p.Status = PosStatusEnum.Closed;
                    p.Operation = PosOperationEnum.Neutral;
                }
                p.PositionTotal = totals.FirstOrDefault(t => t.StrategyCodeEx == p.StrategyCodeEx);
                if (p.PositionTotal == null)
                    throw new NullReferenceException("Total not Found");
            }

            ViewBag.Title = "Accounts TotalPosition2";
            return View(currents);
        }
        public ActionResult Tickers()
        {
            var v = (from p in db.Totals.Include(t => t.Strategy)
                         .Where(p => p.Strategy.TimeInt <= TimeIntTo)
                     group p by p.Strategy.Ticker.Key
                         into g
                         select new Position2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = "All",
                             TickerCodeEx = g.Key,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             // PnL = g.Sum(p=>p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()

                         }).OrderBy(p => p.TickerCodeEx).ToList();

            ViewBag.Title = "Tickers Totals";
            return View(v);
        }
        public ActionResult Strategies()
        {
            var v = (from p in db.Totals.Include(t => t.Strategy).Where(p => p.Strategy.TimeInt <= TimeIntTo)
                     group p by p.Strategy.Code
                         into g
                         select new Position2
                         {
                             StrategyCodeEx = g.Key,
                             AccountCodeEx = "All",
                             TickerCodeEx = "All",
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p=>p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()
                         }).OrderBy(p=>p.StrategyCodeEx).ToList();

            ViewBag.Title = "Strategies Totals";
            return View(v);
        }
        public ActionResult TimeIntervals()
        {
            decimal d = 100;
            ulong i = Decimal.ToUInt64(d); // Convert.ToUInt64(d);

            var v = (from p in db.Totals.Include(t => t.Strategy)
                     group p by p.Strategy.TimeInt
                         into g
                         select new Position2
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
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()
                         }).ToList();
            var vv = v.OrderBy(p=>p.TimeInt.ToString(FormatTimeInt)).ToList();

            ViewBag.Title = "TimeIntervals Totals";
            return View(vv);
        }
        public ActionResult TickersTimeInts()
        {
            var v = (from p in db.Totals.Include(t => t.Strategy)
                     group p by new
                     {
                         TickKey = p.Strategy.Ticker.Key,
                         TimeKey = p.Strategy.TimeInt
                     }
                         into g
                         select new Position2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = "All",
                             TickerCodeEx = g.Key.TickKey,
                             TimeInt = g.Key.TimeKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()

                         }).ToList();
                         var vv = v.OrderBy(p => p.TickerCodeEx + p.TimeInt.ToString(FormatTimeInt)).ToList();

            ViewBag.Title = "Tickers & TimeIntervals Totals";
            return View(vv);
        }
        public ActionResult AccountsTickersStrategies()
        {
            var v = (from p in db.Totals.Include(t => t.Strategy).Where(p=>p.Strategy.TimeInt <= TimeIntTo)
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
                             Quantity = g.Sum(p => p.Quantity),
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
                         .OrderBy(p => p.AccountCodeEx).ThenBy(p=>p.TickerCodeEx).ThenBy(p=>p.StrategyCodeEx).ToList();

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
            //              select new Position2
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
                         select new Position2
                         {
                             DT = (DateTime) g.Key.DtKey,
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
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
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
                return View(new List<Position2>());
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
                         StratKey = p.Strategy.Code,
                         //StratID = p.Strategy.Id
                     }
                         into g
                         select new Position2
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
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()
                         })
                //.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList();
                         .OrderBy(p => p.AccountCodeEx)
                         .ThenBy(p => p.TickerCodeEx)
                         .ThenBy(p => p.StrategyCodeEx)
                         .ThenBy(p => p.DT).ToList();

            foreach (var i in v)
            {
                Position2 i1 = i;
                var ps = db.Positions
                    .Where(p => p.Strategy.Code == i1.StrategyCodeEx &&
                                p.Strategy.Account.Code == i1.AccountCodeEx &&
                                p.Strategy.Ticker.Code == i1.TickerCodeEx &&
                                p.Status != 0
                                ).ToList();
                foreach(var p in ps)
                    i1.PnL3 += p.PnL;
            }
            ViewBag.Title = "Accounts & Tickers & Strategies Results at Date=" + lastDate.ToString("d");
            return View(v);
        }
        public ActionResult AccountsTickersStrategiesTimeIntLastDay()
        {
            var firstOrDefault = db.Deals.OrderByDescending(p => p.LastTradeDT).FirstOrDefault();
            if (firstOrDefault == null)
                return View(new List<Position2>());
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
                         StratKey = p.Strategy.Code,
                         TimeKey = p.Strategy.TimeInt
                     }
                         into g
                         select new Position2
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
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()
                         })
                //.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList();
                         .OrderBy(p => p.AccountCodeEx)
                         .ThenBy(p => p.TickerCodeEx)
                         .ThenBy(p => p.StrategyCodeEx)
                         .ThenBy(p=>p.TimeInt)
                         .ThenBy(p => p.DT).ToList();

            ViewBag.Title = "Accounts & Tickers & Strategies & TimeInt Results at Date=" + lastDate.ToString("d");
            return View(v);
        }
        public ActionResult TickersStrategies()
        {
            var v = (from p in db.Totals.Include(t => t.Strategy).Where(p => p.Strategy.TimeInt <= TimeIntTo)
                        group p by new
                        {
                            TickKey = p.Strategy.Ticker.Key,
                            StratKey = p.Strategy.Code
                        }
                        into g
                        select new Position2
                        {
                            StrategyCodeEx = g.Key.StratKey,
                            AccountCodeEx = "All",
                            TickerCodeEx = g.Key.TickKey,
                            //TimeInt = g.Key.TimeKey,
                            Quantity = g.Sum(p => p.Quantity),
                            //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                            //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                            PnL = g.Sum(p=>p.PnL),
                            PnL3 = g.Sum(p => p.PnL3),
                            FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                            FirstTradeDT = g.Min(p => p.FirstTradeDT),
                            LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                            LastTradeDT = g.Max(p => p.LastTradeDT),
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
                         StratKey = p.Strategy.Code,
                         TimeKey = p.Strategy.TimeInt
                     }
                         into g
                         select new Position2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = "All",
                             TickerCodeEx = g.Key.TickKey,
                             TimeInt = g.Key.TimeKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()

                         }).ToList();
                       var vv = v.OrderBy(p => p.TickerCodeEx + p.StrategyCodeEx + p.TimeInt.ToString(FormatTimeInt)).ToList();

            ViewBag.Title = "Tickers & Strategies & TimeIntervals Totals";
            return View(vv);
        }
        public ActionResult Tickers2()
        {
            var totals = (from p in db.Totals.Include(t => t.Strategy).Where(t => t.Strategy.Code != "Default")
                          group p by new {TickKey = p.Strategy.Ticker.Key }
                              into g
                              select new Position2
                              {
                                  StrategyCodeEx = g.Key.TickKey,
                                  AccountCodeEx = "All",
                                  TickerCodeEx = g.Key.TickKey,
                                  Quantity = g.Sum(p => p.Quantity),
                                  //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                                  PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                                  // PnL = g.Sum(p=>p.PnL),
                                  PnL3 = g.Sum(p => p.PnL3),
                                  FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                                  FirstTradeDT = g.Min(p => p.FirstTradeDT),
                                  LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                                  LastTradeDT = g.Max(p => p.LastTradeDT),
                                  Count = g.Count()
                              }).ToList();

            var currents = (from p in db.Positions.Include(t => t.Strategy).Where(t => t.Strategy.Code != "Default")
                            group p by new { TickKey = p.Strategy.Ticker.Key }
                                into g
                                select new Position2
                                {
                                    StrategyCodeEx = g.Key.TickKey,
                                    AccountCodeEx = "All",
                                    TickerCodeEx = g.Key.TickKey,
                                    Quantity = g.Sum(p => (short)p.Operation * p.Quantity),
                                    PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                                    // PnL = g.Sum(p=>p.PnL),
                                    PnL3 = g.Sum(p => p.PnL3),
                                    FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                                    FirstTradeDT = g.Min(p => p.FirstTradeDT),
                                    LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                                    LastTradeDT = g.Max(p => p.LastTradeDT),
                                    Count = g.Count()

                                }).ToList();
            foreach (var p in currents)
            {
                if (p.Quantity < 0)
                {
                    p.Quantity = -1 * p.Quantity;
                    p.Status = PosStatusEnum.Opened;
                    p.Operation = PosOperationEnum.Short;
                }
                else if (p.Quantity > 0)
                {
                    p.Status = PosStatusEnum.Opened;
                    p.Operation = PosOperationEnum.Long;
                }
                else
                {
                    p.Status = PosStatusEnum.Closed;
                    p.Operation = PosOperationEnum.Neutral;
                }
                p.PositionTotal = totals.FirstOrDefault(t => t.StrategyCodeEx == p.StrategyCodeEx);
                if (p.PositionTotal == null)
                    throw new NullReferenceException("Total not Found");
            }

            ViewBag.Title = "Tickers TotalPosition2";
            return View(currents);
        }
        
        public ActionResult AccountsTickers()
        {
            var v = (from p in db.Totals.Include(t => t.Strategy)
                         .Where(p => p.Strategy.TimeInt <= TimeIntTo)
                     group p by new {AccKey=p.Strategy.Account.Key, TickKey=p.Strategy.Ticker.Key}
                         into g
                         select new Position2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = g.Key.AccKey,
                             TickerCodeEx = g.Key.TickKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p=>p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()

                         }).OrderBy(p=>p.AccountCodeEx+p.TickerCodeEx).ToList();

            ViewBag.Title = "Accounts & Tickers Totals";
            return View(v);
        }
        public ActionResult AccountsTimeInts()
        {
            var v = (from p in db.Totals.Include(t => t.Strategy)
                group p by new
                {
                    AccKey = p.Strategy.Account.Key,
                    TimeKey = p.Strategy.TimeInt
                }
                into g
                select new Position2
                {
                    StrategyCodeEx = "All",
                    AccountCodeEx = g.Key.AccKey,
                    TimeInt = g.Key.TimeKey,
                    Quantity = g.Sum(p => p.Quantity),
                    //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                    //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                    PnL = g.Sum(p => p.PnL),
                    PnL3 = g.Sum(p => p.PnL3),
                    FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                    FirstTradeDT = g.Min(p => p.FirstTradeDT),
                    LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                    LastTradeDT = g.Max(p => p.LastTradeDT),
                    Count = g.Count()

                }).ToList();
                var vv = v.OrderBy(p => p.AccountCodeEx + p.TimeInt.ToString(FormatTimeInt)).ToList();

            ViewBag.Title = "Accounts & TimeIntervals Totals";
            return View(vv);
        }
        public ActionResult AccountsTickers2()
        {
            var totals = (from p in db.Totals.Include(t => t.Strategy).Where(t=>t.Strategy.Code != "Default")
                     group p by new { AccKey = p.Strategy.Account.Key, TickKey = p.Strategy.Ticker.Key }
                         into g
                         select new Position2
                         {
                             StrategyCodeEx = g.Key.AccKey + "@" + g.Key.TickKey,
                             AccountCodeEx = g.Key.AccKey,
                             TickerCodeEx = g.Key.TickKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             // PnL = g.Sum(p=>p.PnL),
                             PnL3 = g.Sum(p => p.PnL3),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Count = g.Count()
                         }).ToList();

            var currents = (from p in db.Positions.Include(t => t.Strategy).Where(t => t.Strategy.Code != "Default")
                          group p by new { AccKey = p.Strategy.Account.Key, TickKey = p.Strategy.Ticker.Key }
                              into g
                              select new Position2
                              {
                                  StrategyCodeEx = g.Key.AccKey + "@" + g.Key.TickKey,
                                  AccountCodeEx = g.Key.AccKey,
                                  TickerCodeEx = g.Key.TickKey,
                                  Quantity = g.Sum(p => (short)p.Operation * p.Quantity),
                                  PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                                  // PnL = g.Sum(p=>p.PnL),
                                  PnL3 = g.Sum(p => p.PnL3),
                                  FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                                  FirstTradeDT = g.Min(p => p.FirstTradeDT),
                                  LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                                  LastTradeDT = g.Max(p => p.LastTradeDT),
                                  Count = g.Count()

                              }).OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList();
            foreach (var p in currents)
            {
                if (p.Quantity < 0)
                {
                    p.Quantity = -1*p.Quantity;
                    p.Status = PosStatusEnum.Opened;
                    p.Operation = PosOperationEnum.Short;
                }
                else if (p.Quantity > 0)
                {
                    p.Status = PosStatusEnum.Opened;
                    p.Operation = PosOperationEnum.Long;
                }
                else
                {
                    p.Status = PosStatusEnum.Closed;
                    p.Operation = PosOperationEnum.Neutral;
                }
                p.PositionTotal = totals.FirstOrDefault(t => t.StrategyCodeEx == p.StrategyCodeEx);
                if (p.PositionTotal == null)
                    throw new NullReferenceException("Total not Found");
            }

            ViewBag.Title = "Accounts & Tickers TotalPosition";
            return View(currents);
        }
        public ActionResult AccountsTickersTimeInts()
        {
            var v = (from p in db.Totals.Include(t => t.Strategy)
                group p by new
                {
                    AccKey = p.Strategy.Account.Key,
                    TickKey = p.Strategy.Ticker.Key,
                    TimeKey = p.Strategy.TimeInt
                    //TimeKey = p.Strategy.TimeInt.ToString(CultureInfo.InvariantCulture)
                }
                into g
                select new Position2
                {
                    StrategyCodeEx = "All",
                    AccountCodeEx = g.Key.AccKey,
                    TickerCodeEx = g.Key.TickKey,
                    TimeInt = g.Key.TimeKey,
                    //TimeIntKey = ((Int32)(g.Key.TimeKey)).ToString(CultureInfo.InvariantCulture), 
                    Quantity = g.Sum(p => p.Quantity),
                    //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                    PnL = g.Sum(p => p.Quantity*(p.Price2 - p.Price1)),
                    // PnL = g.Sum(p=>p.PnL),
                    PnL3 = g.Sum(p => p.PnL3),
                    FirstTradeNumber = g.Min(p => p.FirstTradeNumber.ToUint64()),
                    FirstTradeDT = g.Min(p => p.FirstTradeDT),
                    LastTradeNumber = g.Max(p => p.LastTradeNumber.ToUint64()),
                    LastTradeDT = g.Max(p => p.LastTradeDT),
                    Count = g.Count()
                }).ToList();
                //}).OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList(); 
                //.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx /* + p.TimeInt */).ToList();

            var vv = v.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx + p.TimeInt.ToString(FormatTimeInt)).ToList();
            //ViewBag.Title = "Accounts & Tickers & TimeIntervals Totals";
            return View(vv);
        }
        //
        // GET: /Totals/Details/5

        public ActionResult Details(long id = 0)
        {
            Total total = db.Totals.Find(id);
            if (total == null)
            {
                return HttpNotFound();
            }
            return View(total);
        }

        //
        // GET: /Totals/Create

        public ActionResult Create()
        {
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Name");
            return View();
        }

        //
        // POST: /Totals/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Total total)
        {
            if (ModelState.IsValid)
            {
                db.Totals.Add(total);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Name", total.StrategyId);
            return View(total);
        }

        //
        // GET: /Totals/Edit/5

        public ActionResult Edit(long id = 0)
        {
            Total total = db.Totals.Find(id);
            if (total == null)
            {
                return HttpNotFound();
            }
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Name", total.StrategyId);
            return View(total);
        }

        //
        // POST: /Totals/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Total total)
        {
            if (ModelState.IsValid)
            {
                db.Entry(total).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Name", total.StrategyId);
            return View(total);
        }

        //
        // GET: /Totals/Delete/5

        public ActionResult Delete(long id = 0)
        {
            Total total = db.Totals.Find(id);
            if (total == null)
            {
                return HttpNotFound();
            }
            return View(total);
        }

        //
        // POST: /Totals/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Total total = db.Totals.Find(id);
            db.Totals.Remove(total);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

    public class Pos
    {
        public string Account { get; set; }
        public decimal PnL { get; set; }
    }
}