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
using GS.Trade.DataBase.Functions;
using GS.Trade.DataBase.Model;
using GS.Trade.Trades;

namespace GS.Trade.Web.Mvc_07.Controllers
{
    public class TotalDealsController : Controller
    {
        private DbTradeContext db = new DbTradeContext();

        // GET: TotalDeals
        public ActionResult Index(string accountList, string symbolList, string tickerList, string strategyList,
                                int? timeIntList, string searchString, bool? lastDayChecked,
                                bool? accountGroup, bool? symbolGroup, bool? tickerGroup, bool? strategyGroup, bool? timeIntGroup,
                                string sortOrder,
                                int? page)
        {
            db.Database.CommandTimeout = 900;
            // var lastDayChecked = true;
            DateTime? lastDate;
            //lastDate = DealsFunctions.GetDealsLastDateTime(db);

            var isSortOrderValid = !string.IsNullOrWhiteSpace(sortOrder);
            if (isSortOrderValid)
            {
                ViewBag.SortPnL = sortOrder == "PnL_Asc" ? "PnL_Desc" : "PnL_Asc";
            }
            else
                ViewBag.SortPnL = "PnL_Desc";

            //ViewBag.SortPnL = sortOrder == "PnL_Asc" ? "PnL_Desc" : "PnLAsc";

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

            //ViewBag.Request = Request.RawUrl;
            //ViewBag.Query = Request.QueryString;
            //ViewBag.Url = Request.Url;
            var isLastDayChecked = lastDayChecked.HasValue && lastDayChecked == true;
            var isAccountGroup = accountGroup != null && accountGroup == true;
            var isSymbolGroup = symbolGroup != null && symbolGroup == true;
            var isTickerGroup = tickerGroup != null && tickerGroup == true;
            var isStrategyGroup = strategyGroup != null && strategyGroup == true;
            var isTimeIntGroup = timeIntGroup != null && timeIntGroup == true;

            //var deals = DealsFunctions.SelectDeals(db,
            //                        accountList, symbolList, tickerList, strategyList,
            //                        timeIntList, searchString, isLastDayChecked);
            var deals = db.SelectDeals(
                                    accountList, symbolList, tickerList, strategyList,
                                    timeIntList, searchString, isLastDayChecked);
            if (deals == null)
            {
                return View(new List<PositionDb2>());
            }
            var total = deals.Any()
                            ? deals.Sum(d => d.Quantity * (int) d.Operation * (d.Price2 - d.Price1))
                            : 0;
            ViewBag.Profit = total.ToString("N4");

            var v = SelectTotalsGroupedFromDeals(deals, accountGroup, symbolGroup, tickerGroup, strategyGroup, timeIntGroup);

            switch (sortOrder)
            {
                case "PnL_Asc":
                    v = v.OrderBy(p => p.PnL).ToList();
                    break;
                case "PnL_Desc":
                    v = v.OrderByDescending(p => p.PnL).ToList();
                    break;
            }

            // SelectDropBox in Filter Forn
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

            ViewBag.Title = "Totals";
            ViewBag.FullTitle = "Totals = ( " + total.ToString("N4") + " )";

            //var positions = db.Totals.Include(t => t.Strategy);
            return View(v);
        }

        private IList<PositionDb2> SelectTotalsGroupedFromDeals(IQueryable<Deal> deals, 
            bool? accountGroup, bool? symbolGroup, bool? tickerGroup, bool? strategyGroup, bool? timeIntGroup)
        {
            var isAccountGroup = accountGroup != null && accountGroup == true;
            var isSymbolGroup = symbolGroup != null && symbolGroup == true;
            var isTickerGroup = tickerGroup != null && tickerGroup == true;
            var isStrategyGroup = strategyGroup != null && strategyGroup == true;
            var isTimeIntGroup = timeIntGroup != null && timeIntGroup == true;

            var v = new List<PositionDb2>();

            //var ss = deals.Select(p => new
            //{
            //    Account = p.Strategy.Account.Code,
            //    Strategy = p.Strategy.Code,
            //    Ticker = p.Strategy.Ticker.Code
            //});                      

            #region Acounts.Symbol,Tickers,Strategies
            
            // Beginning
            if (isAccountGroup && isSymbolGroup && isTickerGroup && isStrategyGroup && isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         Symbol = p.Strategy.Ticker.Code.Substring(0, p.Strategy.Ticker.Code.Length - 2),
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code,
                         TimeIntKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = g.Key.AccCode,
                             Symbol = g.Key.Symbol,
                             TickerCodeEx = g.Key.TickCode,
                             TimeInt = g.Key.TimeIntKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = 0,
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
                    .ToList();
            }
            else if (isAccountGroup && isSymbolGroup && isTickerGroup && isStrategyGroup && !isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         Symbol = p.Strategy.Ticker.Code.Substring(0, p.Strategy.Ticker.Code.Length - 2),
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = g.Key.AccCode,
                             TickerCodeEx = g.Key.TickCode,
                             Symbol = g.Key.Symbol,
                             TimeInt = 0,
                             Quantity = g.Sum(p => p.Quantity),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = g.Key.Format,
                             FormatAvg = g.Key.FormatAvg,
                             Count = g.Count()
                         })
                   .OrderBy(p => p.AccountCodeEx)
                   .ThenBy(p => p.TickerCodeEx)
                   .ThenBy(p => p.StrategyCodeEx)
                    //.ThenBy(p => p.TimeInt)
                   .ToList();
            }
            else if (isAccountGroup && isSymbolGroup && isTickerGroup && !isStrategyGroup && isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         Symbol = p.Strategy.Ticker.Code.Substring(0, p.Strategy.Ticker.Code.Length - 2),
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         //StratKey = p.Strategy.Code,
                         TimeIntKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = g.Key.AccCode,
                             Symbol = g.Key.Symbol,
                             TickerCodeEx = g.Key.TickCode,
                             TimeInt = g.Key.TimeIntKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
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
                    //.ThenBy(p => p.StrategyCodeEx)
                    .ThenBy(p => p.TimeInt)
                    .ToList();
            }
            else if (isAccountGroup && isSymbolGroup && isTickerGroup && !isStrategyGroup && !isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         Symbol = p.Strategy.Ticker.Code.Substring(0, p.Strategy.Ticker.Code.Length - 2),
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         // StratKey = p.Strategy.Code,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = g.Key.AccCode,
                             Symbol = g.Key.Symbol,
                             TickerCodeEx = g.Key.TickCode,
                             TimeInt = 0,
                             Quantity = g.Sum(p => p.Quantity),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = g.Key.Format,
                             FormatAvg = g.Key.FormatAvg,
                             Count = g.Count()

                         })
                   .OrderBy(p => p.AccountCodeEx)
                   .ThenBy(p => p.TickerCodeEx)
                    //.ThenBy(p => p.StrategyCodeEx)
                    //.ThenBy(p => p.TimeInt)
                   .ToList();
            }
            else if (isAccountGroup && isSymbolGroup && !isTickerGroup && isStrategyGroup && isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         Symbol = p.Strategy.Ticker.Code.Substring(0, p.Strategy.Ticker.Code.Length - 2),
                         //TickKey = p.Strategy.Ticker.Key,
                         //TickCode = p.Strategy.Ticker.Code,
                         //Format = p.Strategy.Ticker.Format,
                         //FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code,
                         TimeIntKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = g.Key.AccCode,
                             Symbol = g.Key.Symbol,
                             TickerCodeEx = "All",
                             TimeInt = g.Key.TimeIntKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = "N4",
                             FormatAvg = "N4",
                             Count = g.Count()

                         })
                    //.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList();
                    .OrderBy(p => p.AccountCodeEx)
                    .ThenBy(p => p.TickerCodeEx)
                    .ThenBy(p => p.StrategyCodeEx)
                    .ThenBy(p => p.TimeInt)
                    .ToList();
            }
            else if (isAccountGroup && isSymbolGroup && !isTickerGroup && isStrategyGroup && !isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         Symbol = p.Strategy.Ticker.Code.Substring(0, p.Strategy.Ticker.Code.Length - 2),
                         //TickKey = p.Strategy.Ticker.Key,
                         //TickCode = p.Strategy.Ticker.Code,
                         //Format = p.Strategy.Ticker.Format,
                         //FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = g.Key.AccCode,
                             Symbol = g.Key.Symbol,
                             TickerCodeEx = "All",
                             TimeInt = 0,
                             Quantity = g.Sum(p => p.Quantity),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = "N4",
                             FormatAvg = "N4",
                             Count = g.Count()

                         })
                   .OrderBy(p => p.AccountCodeEx)
                   .ThenBy(p => p.TickerCodeEx)
                   .ThenBy(p => p.StrategyCodeEx)
                    //.ThenBy(p => p.TimeInt)
                   .ToList();
            }
            else if (isAccountGroup && isSymbolGroup && !isTickerGroup && !isStrategyGroup && isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         Symbol = p.Strategy.Ticker.Code.Substring(0, p.Strategy.Ticker.Code.Length - 2),
                         //TickKey = p.Strategy.Ticker.Key,
                         //TickCode = p.Strategy.Ticker.Code,
                         //Format = p.Strategy.Ticker.Format,
                         //FormatAvg = p.Strategy.Ticker.FormatAvg,
                         //StratKey = p.Strategy.Code,
                         TimeIntKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = g.Key.AccCode,
                             Symbol = g.Key.Symbol,
                             TickerCodeEx = "All",
                             TimeInt = g.Key.TimeIntKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = "N4",
                             FormatAvg = "N4",
                             Count = g.Count()

                         })
                    //.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList();
                    .OrderBy(p => p.AccountCodeEx)
                    //.ThenBy(p => p.TickerCodeEx)
                    //.ThenBy(p => p.StrategyCodeEx)
                    .ThenBy(p => p.TimeInt)
                    .ToList();
            }
            else if (isAccountGroup && isSymbolGroup && !isTickerGroup && !isStrategyGroup && !isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         Symbol = p.Strategy.Ticker.Code.Substring(0, p.Strategy.Ticker.Code.Length - 2),
                         //TickKey = p.Strategy.Ticker.Key,
                         //TickCode = p.Strategy.Ticker.Code,
                         //Format = p.Strategy.Ticker.Format,
                         //FormatAvg = p.Strategy.Ticker.FormatAvg,
                         // StratKey = p.Strategy.Code,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = g.Key.AccCode,
                             Symbol = g.Key.Symbol,
                             TickerCodeEx = "All",
                             TimeInt = 0,
                             Quantity = g.Sum(p => p.Quantity),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = "N4",
                             FormatAvg = "N4",
                             Count = g.Count()

                         })
                   .OrderBy(p => p.AccountCodeEx)
                    //.ThenBy(p => p.TickerCodeEx)
                    //.ThenBy(p => p.StrategyCodeEx)
                    //.ThenBy(p => p.TimeInt)
                   .ToList();
            }
            else if (isAccountGroup && !isSymbolGroup && isTickerGroup && isStrategyGroup && isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code,
                         TimeIntKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = g.Key.AccCode,
                             Symbol = "All",
                             TickerCodeEx = g.Key.TickCode,
                             TimeInt = g.Key.TimeIntKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
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
                    .ToList();
            }
            else if (isAccountGroup && !isSymbolGroup && isTickerGroup && isStrategyGroup && !isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = g.Key.AccCode,
                             Symbol = "All",
                             TickerCodeEx = g.Key.TickCode,
                             TimeInt = 0,
                             Quantity = g.Sum(p => p.Quantity),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = g.Key.Format,
                             FormatAvg = g.Key.FormatAvg,
                             Count = g.Count()

                         })
                   .OrderBy(p => p.AccountCodeEx)
                   .ThenBy(p => p.TickerCodeEx)
                   .ThenBy(p => p.StrategyCodeEx)
                    //.ThenBy(p => p.TimeInt)
                   .ToList();
            }
            else if (isAccountGroup && !isSymbolGroup && isTickerGroup && !isStrategyGroup && isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         //StratKey = p.Strategy.Code,
                         TimeIntKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = g.Key.AccCode,
                             Symbol = "All",
                             TickerCodeEx = g.Key.TickCode,
                             TimeInt = g.Key.TimeIntKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
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
                    //.ThenBy(p => p.StrategyCodeEx)
                    .ThenBy(p => p.TimeInt)
                    .ToList();
            }
            else if (isAccountGroup && !isSymbolGroup && isTickerGroup && !isStrategyGroup && !isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         // StratKey = p.Strategy.Code,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = g.Key.AccCode,
                             Symbol = "All",
                             TickerCodeEx = g.Key.TickCode,
                             TimeInt = 0,
                             Quantity = g.Sum(p => p.Quantity),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = g.Key.Format,
                             FormatAvg = g.Key.FormatAvg,
                             Count = g.Count()

                         })
                   .OrderBy(p => p.AccountCodeEx)
                   .ThenBy(p => p.TickerCodeEx)
                    //.ThenBy(p => p.StrategyCodeEx)
                    //.ThenBy(p => p.TimeInt)
                   .ToList();
            }
            else if (isAccountGroup && !isSymbolGroup && !isTickerGroup && isStrategyGroup && isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         //TickKey = p.Strategy.Ticker.Key,
                         //TickCode = p.Strategy.Ticker.Code,
                         //Format = p.Strategy.Ticker.Format,
                         //FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code,
                         TimeIntKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = g.Key.AccCode,
                             Symbol = "All",
                             TickerCodeEx = "All",
                             TimeInt = g.Key.TimeIntKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = "N4",
                             FormatAvg = "N4",
                             Count = g.Count()

                         })
                    //.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList();
                    .OrderBy(p => p.AccountCodeEx)
                    .ThenBy(p => p.TickerCodeEx)
                    .ThenBy(p => p.StrategyCodeEx)
                    .ThenBy(p => p.TimeInt)
                    .ToList();
            }
            else if (isAccountGroup && !isSymbolGroup && !isTickerGroup && isStrategyGroup && !isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         //TickKey = p.Strategy.Ticker.Key,
                         //TickCode = p.Strategy.Ticker.Code,
                         //Format = p.Strategy.Ticker.Format,
                         //FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = g.Key.AccCode,
                             Symbol = "All",
                             TickerCodeEx = "All",
                             TimeInt = 0,
                             Quantity = g.Sum(p => p.Quantity),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = "N4",
                             FormatAvg = "N4",
                             Count = g.Count()

                         })
                   .OrderBy(p => p.AccountCodeEx)
                   .ThenBy(p => p.TickerCodeEx)
                   .ThenBy(p => p.StrategyCodeEx)
                    //.ThenBy(p => p.TimeInt)
                   .ToList();
            }
            else if (isAccountGroup && !isSymbolGroup && !isTickerGroup && !isStrategyGroup && isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         //TickKey = p.Strategy.Ticker.Key,
                         //TickCode = p.Strategy.Ticker.Code,
                         //Format = p.Strategy.Ticker.Format,
                         //FormatAvg = p.Strategy.Ticker.FormatAvg,
                         //StratKey = p.Strategy.Code,
                         TimeIntKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = g.Key.AccCode,
                             Symbol = "All",
                             TickerCodeEx = "All",
                             TimeInt = g.Key.TimeIntKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = "N4",
                             FormatAvg = "N4",
                             Count = g.Count()

                         })
                    //.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList();
                    .OrderBy(p => p.AccountCodeEx)
                    //.ThenBy(p => p.TickerCodeEx)
                    //.ThenBy(p => p.StrategyCodeEx)
                    .ThenBy(p => p.TimeInt)
                    .ToList();
            }
            else if (isAccountGroup && !isSymbolGroup && !isTickerGroup && !isStrategyGroup && !isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         AccKey = p.Strategy.Account.Key,
                         AccCode = p.Strategy.Account.Code,
                         //TickKey = p.Strategy.Ticker.Key,
                         //TickCode = p.Strategy.Ticker.Code,
                         //Format = p.Strategy.Ticker.Format,
                         //FormatAvg = p.Strategy.Ticker.FormatAvg,
                         // StratKey = p.Strategy.Code,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = g.Key.AccCode,
                             Symbol = "All",
                             TickerCodeEx = "All",
                             TimeInt = 0,
                             Quantity = g.Sum(p => p.Quantity),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = "N4",
                             FormatAvg = "N4",
                             Count = g.Count()

                         })
                   .OrderBy(p => p.AccountCodeEx)
                    //.ThenBy(p => p.TickerCodeEx)
                    //.ThenBy(p => p.StrategyCodeEx)
                    //.ThenBy(p => p.TimeInt)
                   .ToList();
            }
            // Accounts = false
            else if ( !isAccountGroup && isSymbolGroup && isTickerGroup && isStrategyGroup && isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         //AccKey = p.Strategy.Account.Key,
                         //AccCode = p.Strategy.Account.Code,
                         Symbol = p.Strategy.Ticker.Code.Substring(0, p.Strategy.Ticker.Code.Length - 2),
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code,
                         TimeIntKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = "All",
                             Symbol = g.Key.Symbol,
                             TickerCodeEx = g.Key.TickCode,
                             TimeInt = g.Key.TimeIntKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
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
                    //.OrderBy(p => p.AccountCodeEx)
                    .OrderBy(p => p.TickerCodeEx)
                    .ThenBy(p => p.StrategyCodeEx)
                    .ThenBy(p => p.TimeInt)
                    .ToList();
            }
            else if (!isAccountGroup && isSymbolGroup && isTickerGroup && isStrategyGroup && !isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         //AccKey = p.Strategy.Account.Key,
                         //AccCode = p.Strategy.Account.Code,
                         Symbol = p.Strategy.Ticker.Code.Substring(0, p.Strategy.Ticker.Code.Length - 2),
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = "All",
                             TickerCodeEx = g.Key.TickCode,
                             Symbol = g.Key.Symbol,
                             TimeInt = 0,
                             Quantity = g.Sum(p => p.Quantity),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = g.Key.Format,
                             FormatAvg = g.Key.FormatAvg,
                             Count = g.Count()

                         })
                   //.OrderBy(p => p.AccountCodeEx)
                   .OrderBy(p => p.TickerCodeEx)
                   .ThenBy(p => p.StrategyCodeEx)
                    //.ThenBy(p => p.TimeInt)
                   .ToList();
            }
            else if (!isAccountGroup && isSymbolGroup && isTickerGroup && !isStrategyGroup && isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         //AccKey = p.Strategy.Account.Key,
                         //AccCode = p.Strategy.Account.Code,
                         Symbol = p.Strategy.Ticker.Code.Substring(0, p.Strategy.Ticker.Code.Length - 2),
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         //StratKey = p.Strategy.Code,
                         TimeIntKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = "All",
                             Symbol = g.Key.Symbol,
                             TickerCodeEx = g.Key.TickCode,
                             TimeInt = g.Key.TimeIntKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
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
                    //.OrderBy(p => p.AccountCodeEx)
                    .OrderBy(p => p.TickerCodeEx)
                    //.ThenBy(p => p.StrategyCodeEx)
                    .ThenBy(p => p.TimeInt)
                    .ToList();
            }
            else if (!isAccountGroup && isSymbolGroup && isTickerGroup && !isStrategyGroup && !isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         //AccKey = p.Strategy.Account.Key,
                         //AccCode = p.Strategy.Account.Code,
                         Symbol = p.Strategy.Ticker.Code.Substring(0, p.Strategy.Ticker.Code.Length - 2),
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         // StratKey = p.Strategy.Code,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = "All",
                             Symbol = g.Key.Symbol,
                             TickerCodeEx = g.Key.TickCode,
                             TimeInt = 0,
                             Quantity = g.Sum(p => p.Quantity),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = g.Key.Format,
                             FormatAvg = g.Key.FormatAvg,
                             Count = g.Count()

                         })
                   //.OrderBy(p => p.AccountCodeEx)
                   .OrderBy(p => p.TickerCodeEx)
                    //.ThenBy(p => p.StrategyCodeEx)
                    //.ThenBy(p => p.TimeInt)
                   .ToList();
            }
            else if (!isAccountGroup && isSymbolGroup && !isTickerGroup && isStrategyGroup && isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         //AccKey = p.Strategy.Account.Key,
                         //AccCode = p.Strategy.Account.Code,
                         Symbol = p.Strategy.Ticker.Code.Substring(0, p.Strategy.Ticker.Code.Length - 2),
                         //TickKey = p.Strategy.Ticker.Key,
                         //TickCode = p.Strategy.Ticker.Code,
                         //Format = p.Strategy.Ticker.Format,
                         //FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code,
                         TimeIntKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = "All",
                             Symbol = g.Key.Symbol,
                             TickerCodeEx = "All",
                             TimeInt = g.Key.TimeIntKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = "N4",
                             FormatAvg = "N4",
                             Count = g.Count()

                         })
                    //.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList();
                    //.OrderBy(p => p.AccountCodeEx)
                    .OrderBy(p => p.TickerCodeEx)
                    .ThenBy(p => p.StrategyCodeEx)
                    .ThenBy(p => p.TimeInt)
                    .ToList();
            }
            else if (!isAccountGroup && isSymbolGroup && !isTickerGroup && isStrategyGroup && !isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         //AccKey = p.Strategy.Account.Key,
                         //AccCode = p.Strategy.Account.Code,
                         Symbol = p.Strategy.Ticker.Code.Substring(0, p.Strategy.Ticker.Code.Length - 2),
                         //TickKey = p.Strategy.Ticker.Key,
                         //TickCode = p.Strategy.Ticker.Code,
                         //Format = p.Strategy.Ticker.Format,
                         //FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = "All",
                             Symbol = g.Key.Symbol,
                             TickerCodeEx = "All",
                             TimeInt = 0,
                             Quantity = g.Sum(p => p.Quantity),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = "N4",
                             FormatAvg = "N4",
                             Count = g.Count()

                         })
                   //.OrderBy(p => p.AccountCodeEx)
                   .OrderBy(p => p.TickerCodeEx)
                   .ThenBy(p => p.StrategyCodeEx)
                    //.ThenBy(p => p.TimeInt)
                   .ToList();
            }
            else if (!isAccountGroup && isSymbolGroup && !isTickerGroup && !isStrategyGroup && isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         //AccKey = p.Strategy.Account.Key,
                         //AccCode = p.Strategy.Account.Code,
                         Symbol = p.Strategy.Ticker.Code.Substring(0, p.Strategy.Ticker.Code.Length - 2),
                         //TickKey = p.Strategy.Ticker.Key,
                         //TickCode = p.Strategy.Ticker.Code,
                         //Format = p.Strategy.Ticker.Format,
                         //FormatAvg = p.Strategy.Ticker.FormatAvg,
                         //StratKey = p.Strategy.Code,
                         TimeIntKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = "All",
                             Symbol = g.Key.Symbol,
                             TickerCodeEx = "All",
                             TimeInt = g.Key.TimeIntKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = "N4",
                             FormatAvg = "N4",
                             Count = g.Count()

                         })
                    //.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList();
                    //.OrderBy(p => p.AccountCodeEx)
                    //.ThenBy(p => p.TickerCodeEx)
                    //.ThenBy(p => p.StrategyCodeEx)
                    .OrderBy(p => p.TimeInt)
                    .ToList();
            }
            else if (!isAccountGroup && isSymbolGroup && !isTickerGroup && !isStrategyGroup && !isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         //AccKey = p.Strategy.Account.Key,
                         //AccCode = p.Strategy.Account.Code,
                         Symbol = p.Strategy.Ticker.Code.Substring(0, p.Strategy.Ticker.Code.Length - 2),
                         //TickKey = p.Strategy.Ticker.Key,
                         //TickCode = p.Strategy.Ticker.Code,
                         //Format = p.Strategy.Ticker.Format,
                         //FormatAvg = p.Strategy.Ticker.FormatAvg,
                         // StratKey = p.Strategy.Code,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = "All",
                             Symbol = g.Key.Symbol,
                             TickerCodeEx = "All",
                             TimeInt = 0,
                             Quantity = g.Sum(p => p.Quantity),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = "N4",
                             FormatAvg = "N4",
                             Count = g.Count()

                         })
                   //.OrderBy(p => p.AccountCodeEx)
                    //.ThenBy(p => p.TickerCodeEx)
                    //.ThenBy(p => p.StrategyCodeEx)
                    //.ThenBy(p => p.TimeInt)
                   .ToList();
            }
            else if (!isAccountGroup && !isSymbolGroup && isTickerGroup && isStrategyGroup && isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         //AccKey = p.Strategy.Account.Key,
                         //AccCode = p.Strategy.Account.Code,
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code,
                         TimeIntKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = "All",
                             Symbol = "All",
                             TickerCodeEx = g.Key.TickCode,
                             TimeInt = g.Key.TimeIntKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
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
                    //.OrderBy(p => p.AccountCodeEx)
                    .OrderBy(p => p.TickerCodeEx)
                    .ThenBy(p => p.StrategyCodeEx)
                    .ThenBy(p => p.TimeInt)
                    .ToList();
            }
            else if (!isAccountGroup && !isSymbolGroup && isTickerGroup && isStrategyGroup && !isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         //AccKey = p.Strategy.Account.Key,
                         //AccCode = p.Strategy.Account.Code,
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = "All",
                             Symbol = "All",
                             TickerCodeEx = g.Key.TickCode,
                             TimeInt = 0,
                             Quantity = g.Sum(p => p.Quantity),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = g.Key.Format,
                             FormatAvg = g.Key.FormatAvg,
                             Count = g.Count()
                         })
                   //.OrderBy(p => p.AccountCodeEx)
                   .OrderBy(p => p.TickerCodeEx)
                   .ThenBy(p => p.StrategyCodeEx)
                    //.ThenBy(p => p.TimeInt)
                   .ToList();
            }
            else if (!isAccountGroup && !isSymbolGroup && isTickerGroup && !isStrategyGroup && isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         //AccKey = p.Strategy.Account.Key,
                         //AccCode = p.Strategy.Account.Code,
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         //StratKey = p.Strategy.Code,
                         TimeIntKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = "All",
                             Symbol = "All",
                             TickerCodeEx = g.Key.TickCode,
                             TimeInt = g.Key.TimeIntKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
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
                    //.OrderBy(p => p.AccountCodeEx)
                    .OrderBy(p => p.TickerCodeEx)
                    //.ThenBy(p => p.StrategyCodeEx)
                    .ThenBy(p => p.TimeInt)
                    .ToList();
            }
            else if (!isAccountGroup && !isSymbolGroup && isTickerGroup && !isStrategyGroup && !isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         //AccKey = p.Strategy.Account.Key,
                         //AccCode = p.Strategy.Account.Code,
                         TickKey = p.Strategy.Ticker.Key,
                         TickCode = p.Strategy.Ticker.Code,
                         Format = p.Strategy.Ticker.Format,
                         FormatAvg = p.Strategy.Ticker.FormatAvg,
                         // StratKey = p.Strategy.Code,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = "All",
                             Symbol = "All",
                             TickerCodeEx = g.Key.TickCode,
                             TimeInt = 0,
                             Quantity = g.Sum(p => p.Quantity),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = g.Key.Format,
                             FormatAvg = g.Key.FormatAvg,
                             Count = g.Count()

                         })
                   //.OrderBy(p => p.AccountCodeEx)
                   .OrderBy(p => p.TickerCodeEx)
                    //.ThenBy(p => p.StrategyCodeEx)
                    //.ThenBy(p => p.TimeInt)
                   .ToList();
            }
            else if (!isAccountGroup && !isSymbolGroup && !isTickerGroup && isStrategyGroup && isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         //AccKey = p.Strategy.Account.Key,
                         //AccCode = p.Strategy.Account.Code,
                         //TickKey = p.Strategy.Ticker.Key,
                         //TickCode = p.Strategy.Ticker.Code,
                         //Format = p.Strategy.Ticker.Format,
                         //FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code,
                         TimeIntKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = "All",
                             Symbol = "All",
                             TickerCodeEx = "All",
                             TimeInt = g.Key.TimeIntKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = "N4",
                             FormatAvg = "N4",
                             Count = g.Count()

                         })
                    //.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList();
                    //.OrderBy(p => p.AccountCodeEx)
                    .OrderBy(p => p.TickerCodeEx)
                    .ThenBy(p => p.StrategyCodeEx)
                    .ThenBy(p => p.TimeInt)
                    .ToList();
            }
            else if (!isAccountGroup && !isSymbolGroup && !isTickerGroup && isStrategyGroup && !isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         //AccKey = p.Strategy.Account.Key,
                         //AccCode = p.Strategy.Account.Code,
                         //TickKey = p.Strategy.Ticker.Key,
                         //TickCode = p.Strategy.Ticker.Code,
                         //Format = p.Strategy.Ticker.Format,
                         //FormatAvg = p.Strategy.Ticker.FormatAvg,
                         StratKey = p.Strategy.Code,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = g.Key.StratKey,
                             AccountCodeEx = "All",
                             Symbol = "All",
                             TickerCodeEx = "All",
                             TimeInt = 0,
                             Quantity = g.Sum(p => p.Quantity),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = "N4",
                             FormatAvg = "N4",
                             Count = g.Count()

                         })
                   //.OrderBy(p => p.AccountCodeEx)
                   .OrderBy(p => p.TickerCodeEx)
                   .ThenBy(p => p.StrategyCodeEx)
                    //.ThenBy(p => p.TimeInt)
                   .ToList();
            }
            else if (!isAccountGroup && !isSymbolGroup && !isTickerGroup && !isStrategyGroup && isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         //AccKey = p.Strategy.Account.Key,
                         //AccCode = p.Strategy.Account.Code,
                         //TickKey = p.Strategy.Ticker.Key,
                         //TickCode = p.Strategy.Ticker.Code,
                         //Format = p.Strategy.Ticker.Format,
                         //FormatAvg = p.Strategy.Ticker.FormatAvg,
                         //StratKey = p.Strategy.Code,
                         TimeIntKey = p.Strategy.TimeInt
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = "All",
                             Symbol = "All",
                             TickerCodeEx = "All",
                             TimeInt = g.Key.TimeIntKey,
                             Quantity = g.Sum(p => p.Quantity),
                             //PnL = g.Sum(p => (short)p.Operation * p.Quantity * (p.Price2 - p.Price1)),
                             //PnL = g.Sum(p => p.Quantity * (p.Price2 - p.Price1)),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = "N4",
                             FormatAvg = "N4",
                             Count = g.Count()

                         })
                    //.OrderBy(p => p.AccountCodeEx + p.TickerCodeEx).ToList();
                    //.OrderBy(p => p.AccountCodeEx)
                    //.ThenBy(p => p.TickerCodeEx)
                    //.ThenBy(p => p.StrategyCodeEx)
                    .OrderBy(p => p.TimeInt)
                    .ToList();
            }
            else if (!isAccountGroup && !isSymbolGroup && !isTickerGroup && !isStrategyGroup && !isTimeIntGroup)
            {
                v = (from p in deals
                     group p by new
                     {
                         //AccKey = p.Strategy.Account.Key,
                         //AccCode = p.Strategy.Account.Code,
                         //TickKey = p.Strategy.Ticker.Key,
                         //TickCode = p.Strategy.Ticker.Code,
                         //Format = p.Strategy.Ticker.Format,
                         //FormatAvg = p.Strategy.Ticker.FormatAvg,
                         // StratKey = p.Strategy.Code,
                     }
                         into g
                         select new PositionDb2
                         {
                             StrategyCodeEx = "All",
                             AccountCodeEx = "All",
                             Symbol = "All",
                             TickerCodeEx = "All",
                             TimeInt = 0,
                             Quantity = g.Sum(p => p.Quantity),
                             PnL = g.Sum(p => (p.Price2 - p.Price1) * (int)p.Operation * p.Quantity),
                             PnL3 = g.Sum(p => 0),
                             FirstTradeNumber = g.Min(p => p.FirstTradeNumber),
                             FirstTradeDT = g.Min(p => p.FirstTradeDT),
                             LastTradeNumber = g.Max(p => p.LastTradeNumber),
                             LastTradeDT = g.Max(p => p.LastTradeDT),
                             Format = "N4",
                             FormatAvg = "N4",
                             Count = g.Count()

                         })
                   //.OrderBy(p => p.AccountCodeEx)
                    //.ThenBy(p => p.TickerCodeEx)
                    //.ThenBy(p => p.StrategyCodeEx)
                    //.ThenBy(p => p.TimeInt)
                   .ToList();
            }

            #endregion

            return v;
        }

        // GET: TotalDeals/Details/5
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

        // GET: TotalDeals/Create
        public ActionResult Create()
        {
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key");
            return View();
        }

        // POST: TotalDeals/Create
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

        // GET: TotalDeals/Edit/5
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

        // POST: TotalDeals/Edit/5
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

        // GET: TotalDeals/Delete/5
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

        // POST: TotalDeals/Delete/5
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
