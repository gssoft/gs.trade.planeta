using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GS.Trade.DataBase.Dal;
using GS.Trade.Trades;
using GS.Trade.Trades.Equity;

namespace GS.Trade.Web.Mvc_07.Controllers
{
    public class ReportsController : Controller
    {
        private DbTradeContext db = new DbTradeContext();
        // GET: Reports
        public ActionResult Index(string accountList, string symbolList, string tickerList, string strategyList,
                                int? timeIntList, string searchString, bool? lastDayChecked,
                                bool? accountGroup, bool? symbolGroup, bool? tickerGroup, bool? strategyGroup, bool? timeIntGroup,
                                //string condition,
                                string conditionDays, string conditionTradesN, string conditionPf, string conditionNetToDd,
                                string sortOrder,
                                int? page)
        {
            db.Database.CommandTimeout = 180;

            ViewBag.Account = accountList;
            ViewBag.Symbol = symbolList;
            ViewBag.Ticker = tickerList;
            ViewBag.Strategy = strategyList;
            ViewBag.TimeInt = timeIntList;
            ViewBag.SearchString = searchString;
            ViewBag.LastDayChecked = lastDayChecked;

            ViewBag.AccountGr = accountGroup;
            ViewBag.SymbolGr = symbolGroup;
            ViewBag.TickerGr = tickerGroup;
            ViewBag.StrategyGr = strategyGroup;
            ViewBag.TimeIntGr = timeIntGroup;

            ViewBag.ConditionDays = conditionDays;
            ViewBag.ConditionTradesN = conditionTradesN;
            ViewBag.ConditionPf = conditionPf;
            ViewBag.ConditionNetToDd = conditionNetToDd;

            var isAccountGroup = accountGroup.HasValue && accountGroup == true;
            var isSymbolGroup = symbolGroup.HasValue && symbolGroup == true;
            var isTickerGroup = tickerGroup.HasValue && tickerGroup == true;
            var isStrategyGroup = strategyGroup.HasValue && strategyGroup == true;
            var isTimeIntGroup = timeIntGroup.HasValue && timeIntGroup == true;
            var isLastDayChecked = lastDayChecked.HasValue && lastDayChecked == true;
            //var isConditionValid = !string.IsNullOrWhiteSpace(condition);

            var isConditionDays = !string.IsNullOrWhiteSpace(conditionDays);
            var isConditionPf = !string.IsNullOrWhiteSpace(conditionPf);
            var isConditionNetToDd = !string.IsNullOrWhiteSpace(conditionNetToDd);
            var isConditionTradesN = !string.IsNullOrWhiteSpace(conditionTradesN);

            // Sorting

            var isSortOrderValid = !string.IsNullOrWhiteSpace(sortOrder);
            if (isSortOrderValid)
            {
                ViewBag.SortPnL = sortOrder == "PnL_Asc" ? "PnL_Desc" : "PnL_Asc";
            }
            else
                ViewBag.SortPnL = "PnL_Desc";


            if (isSortOrderValid)
            {
                ViewBag.SortPf = sortOrder == "Pf_Asc" ? "Pf_Desc" : "Pf_Asc";
            }
            else
                ViewBag.SortPf = "Pf_Desc";

            if (isSortOrderValid)
            {
                ViewBag.SortNetToDD = sortOrder == "NetToDD_Asc" ? "NetToDD_Desc" : "NetToDD_Asc";
            }
            else
                ViewBag.SortNetToDD = "NetToDD_Asc";

            if (isSortOrderValid)
            {
                ViewBag.SortProfitable = sortOrder == "Profitable_Asc" ? "Profitable_Desc" : "Profitable_Asc";
            }
            else
                ViewBag.SortProfitable = "Profitable_Asc";

            if (isSortOrderValid)
            {
                ViewBag.SortMaxDD = sortOrder == "MaxDD_Asc" ? "MaxDD_Desc" : "MaxDD_Asc";
            }
            else
                ViewBag.SortMaxDD = "MaxDD_Asc";

            if (isSortOrderValid)
            {
                ViewBag.SortAvg = sortOrder == "Avg_Asc" ? "Avg_Desc" : "Avg_Asc";
            }
            else
                ViewBag.SortAvg = "Avg_Asc";

            if (isSortOrderValid)
            {
                ViewBag.SortStDev = sortOrder == "StDev_Asc" ? "StDev_Desc" : "StDev_Asc";
            }
            else
                ViewBag.SortStDev = "StDev_Asc";



            // Account List
            ViewBag.accountList = new SelectList(db.GetDealsAccounts());
            // Tickers
            var tickers = db.GetDealsTickers();
            ViewBag.tickerList = new SelectList(tickers);
            // Symbols
            ViewBag.symbolList = new SelectList(DbTradeContext.GetSymbols(tickers));
            // Strategy List
            ViewBag.strategyList = new SelectList(db.GetDealsStrategies());
            // TimeInt
            ViewBag.timeIntList = new SelectList(db.GetDealsTimeInts());

            ViewBag.condition = new SelectList(new List<string> { "pf>1.2,NetToDD>2", "pf>2,NetToDD>2" });

            var deals = db.SelectDeals(
                                    accountList, symbolList, tickerList, strategyList,
                                    timeIntList, searchString, isLastDayChecked);           

            // if (!deals.Any())
            if(deals == null)
            {
                ViewBag.FullTitle = "No deals in Selection";
                return View(new List<PositionDb2>());
            }

            IList<PositionDb2> vv = null;
            vv = db.SelectTotalsGroupedFromDeals(deals,
                            accountGroup, symbolGroup, tickerGroup, strategyGroup,timeIntGroup);
           
            var rs = new List<OmegaReport>();
            foreach (var v in vv)
            {
                // var trs = db.SelectDeals(v.AccountCodeEx, v.Symbol, null, v.StrategyCodeEx, v.TimeInt, null, null);
                var trs = deals
                    .Where(d => v.AccountCodeEx == "All" || (v.AccountCodeEx != "All" && d.Strategy.Account.Code == v.AccountCodeEx))
                    .Where(d => v.Symbol == "All" || (v.Symbol != "All" && d.Strategy.Ticker.Code.Substring(0, d.Strategy.Ticker.Code.Length - 2) == v.Symbol))
                    .Where(d => v.TickerCodeEx == "All" || (v.TickerCodeEx != "All" && d.Strategy.Ticker.Code == v.TickerCodeEx))
                    .Where(d => v.StrategyCodeEx == "All" || (v.StrategyCodeEx != "All" && d.Strategy.Code == v.StrategyCodeEx))
                    .Where(d => v.TimeInt == 0 || (v.TimeInt != 0  && d.Strategy.TimeInt == v.TimeInt))
                    ;
                            
                var lst = trs.Select(t => new OmegaReport.TradeItem
                {
                    Dt1 = t.FirstTradeDT,
                    Dt2 = t.LastTradeDT,
                    Profit = (double)((t.Price2 - t.Price1) * (int)t.Operation * t.Quantity),
                    Cost = 1
                }).ToList();
                var r = new OmegaReport();
                r.Update(lst);
                v.Report = r;
            }
           
            if (isConditionDays)
            {
                var days = Double.Parse(conditionDays.Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);
                vv = vv.Where(v => v.Report.Days >= days).ToList();
            }
            if (isConditionTradesN)
            {
                var trades = Double.Parse(conditionTradesN.Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);
                vv = vv.Where(v => v.Report.TotalTrades >= trades).ToList();
            }
            if (isConditionPf)
            {
                var pf = Double.Parse(conditionPf.Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);
                vv = vv.Where(v => v.Report.ProfitFactor >= pf).ToList();
            }
            if (isConditionNetToDd)
            {
                var nettodd = Double.Parse(conditionNetToDd.Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);
                vv = vv.Where(v => v.Report.NetPrtfToMaxDDRatio >= nettodd).ToList();
            }

            ViewBag.conditionDays = conditionDays;
            ViewBag.conditionTradesN = conditionTradesN;
            ViewBag.conditionPf = conditionPf;
            ViewBag.conditionNetToDd = conditionNetToDd;

            switch (sortOrder)
            {
                case "PnL_Asc":
                    vv = vv.OrderBy(p => p.PnL).ToList();
                    break;
                case "PnL_Desc":
                    vv = vv.OrderByDescending(p => p.PnL).ToList();
                    break;

                case "Profitable_Asc":
                    vv = vv.OrderBy(p => p.Report.PercentProfitable).ToList();
                    break;
                case "Profitable_Desc":
                    vv = vv.OrderByDescending(p => p.Report.PercentProfitable).ToList();
                    break;

                case "Avg_Asc":
                    vv = vv.OrderBy(p => p.Report.AverageTrade).ToList();
                    break;
                case "Avg_Desc":
                    vv = vv.OrderByDescending(p => p.Report.AverageTrade).ToList();
                    break;

                case "StDev_Asc":
                    vv = vv.OrderBy(p => p.Report.StdDevTrade).ToList();
                    break;
                case "StDev_Desc":
                    vv = vv.OrderByDescending(p => p.Report.StdDevTrade).ToList();
                    break;

                case "MaxDD_Asc":
                    vv = vv.OrderBy(p => p.Report.MaxDrawDown).ToList();
                    break;
                case "MaxDD_Desc":
                    vv = vv.OrderByDescending(p => p.Report.MaxDrawDown).ToList();
                    break;

                case "Pf_Asc":
                    vv = vv.OrderBy(p => p.Report.ProfitFactor).ToList();
                    break;
                case "Pf_Desc":
                    vv = vv.OrderByDescending(p => p.Report.ProfitFactor).ToList();
                    break;

                case "NetToDD_Asc":
                    vv = vv.OrderBy(p => p.Report.NetPrtfToMaxDDRatio).ToList();
                    break;
                case "NetToDD_Desc":
                    vv = vv.OrderByDescending(p => p.Report.NetPrtfToMaxDDRatio).ToList();
                    break;
            }

            var cnt = rs.Count;
            ViewBag.FullTitle = "Performance Reports: ( " + cnt + " )";
            ViewBag.Title = ViewBag.FullTitle;

            return View(vv);
        }

        // GET: Reports/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Reports/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reports/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reports/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Reports/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reports/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Reports/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
