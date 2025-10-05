using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.Expressions;
using GS.Extension;
using GS.Trade.DataBase.Dal;
using GS.Trade.DataBase.Functions;
using GS.Trade.DataBase.Model;
using GS.Trade.Trades.Equity;
using GS.Trade.Web.Mvc_07.ViewModel;
using PagedList;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;

namespace GS.Trade.Web.Mvc_07.Controllers
{
    public class DealsController : Controller
    {
        private const int DataBaseCommandTimeOut = 300;
        private readonly DbTradeContext db = new DbTradeContext();

        private const int PageSize = 25;
        private bool _isLastDayChecked;
        private DateTime _lastDate;

        // GET: Deals
        //public ActionResult Index(int? page, string accountSelected, string tickerSelected, string strategySelected )
        public ActionResult Index(string accountList, string symbolList, string tickerList, string strategyList,
                                int? timeIntList, string searchString, bool? lastDayChecked, int? page)
        {
            db.Database.CommandTimeout = DataBaseCommandTimeOut;
            // var lastDt = DealsFunctions.GetDealsLastDateTime(db);

            string accountSelected = accountList;
            string tickerSelected = tickerList;
            string strategySelected = strategyList;

            ViewBag.Account = accountSelected;
            ViewBag.Symbol = symbolList;
            ViewBag.Ticker = tickerSelected;
            ViewBag.Strategy = strategySelected;
            ViewBag.TimeInt = timeIntList;
            ViewBag.SearchString = searchString;
            ViewBag.LastDayChecked = lastDayChecked;

            ViewBag.Request = Request.RawUrl;
            ViewBag.Query = Request.QueryString;
            ViewBag.Url = Request.Url;

            _isLastDayChecked = lastDayChecked.HasValue && lastDayChecked == true;

            var deals = SelectViewDeals(accountList, symbolList, tickerList, strategyList, timeIntList, searchString, lastDayChecked);
            var total = deals != null 
                            ? deals.Sum(d => d.PnL)
                            : 0;
            ViewBag.Profit = total;

           // Create DropDown SelectList
           // Ticker List
            var tickerQry = from d in db.Deals.Include(d => d.Strategy)
                //orderby d.Strategy.Ticker.Code
                select d.Strategy.Ticker.Code;
            var tickers = new List<string>();
            tickers.AddRange(tickerQry.Distinct().OrderBy(p=>p));
            //ViewBag.tickerList = new SelectList(tickers, tickerList.HasValue()?tickerList:"All");
            ViewBag.tickerList = new SelectList(tickers);

            // Symbols
            var symbols = tickers.Select(t => t.Replace(t.Right(2), ""))
                .Distinct()
                .ToList();
            ViewBag.symbolList = new SelectList(symbols);

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
            tis.AddRange(tiQry.Distinct().OrderBy(p=>p));
            ViewBag.timeIntList = new SelectList(tis);
            //ViewBag.lastDayChecked = new CheckBox();

            var dealsCount = deals.Count();
            ViewBag.Title = "Deals";
            string lastDate = _isLastDayChecked ? " LastDate = " + ViewBag.LastDate.ToString("d") : "";
            
            ViewBag.FullTitle = "Deals ( " + dealsCount + " ) in " + ((dealsCount/PageSize) + 1)
                                + " Pages with PageSize: " + PageSize
                                + " Totals = " + total.ToString("N4")
                                + lastDate;

            ViewBag.ItemCount = deals.Count();

            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(deals.ToPagedList(pageNumber, PageSize));

            //return View(deals);
        }
        public ActionResult Index1(string accountList, string tickerList, string strategyList, int? page)
        {
            //var deals = db.Deals.Include(d => d.Strategy);
            //return View(deals.ToList());
            //var deals = db.Deals
            //                .Include(p => p.Strategy)
            //                .OrderByDescending(p=>p.LastTradeDT)
            //                //.ThenBy(p => p.Strategy.Account.Code)
            //                //.ThenBy(p => p.Strategy.Ticker.Code)
            //                //.ThenBy(p => p.Strategy.Code)
            //                //.ThenBy(p => p.Strategy.TimeInt)
            //    
            string accountSelected = accountList;
            string tickerSelected = tickerList;
            string strategySelected = strategyList;

            ViewBag.Account = accountSelected;
            ViewBag.Ticker = tickerSelected;
            ViewBag.Strategy = strategySelected;

            var deals = default(List<ViewDeal>);
            if
                //((tickerSelected.HasNoValue() || tickerSelected == "All") &&
                //    (accountSelected.HasNoValue() || accountSelected == "All"))
                ((tickerSelected.HasNoValue()) &&
                (accountSelected.HasNoValue()))
            {
                deals = (from d in db.Deals.Include(d => d.Strategy)
                         select new ViewDeal
                         {
                             Id = d.Id,
                             Account = d.Strategy.Account.Code,
                             Ticker = d.Strategy.Ticker.Code,
                             Strategy = d.Strategy.Code,
                             TimeInt = d.Strategy.TimeInt,
                             Side = d.Operation,
                             Qty = d.Quantity,
                             Price1 = d.Price1,
                             Price2 = d.Price2,
                             FirstTradeDT = d.FirstTradeDT,
                             LastTradeDT = d.LastTradeDT,
                             Format = d.Strategy.Ticker.Format,
                             FormatAvg = d.Strategy.Ticker.FormatAvg,
                         })
                    .OrderByDescending(d => d.LastTradeDT)
                    .ToList();
            }
            //else if ((tickerSelected.HasValue() && tickerSelected != "All") &&
            //        (accountSelected.HasNoValue() || accountSelected == "All"))
            else if ((tickerSelected.HasValue()) &&
                    (accountSelected.HasNoValue()))
            {
                deals = (from d in db.Deals.Include(d => d.Strategy)
                         .Where(d => d.Strategy.Ticker.Code == tickerSelected)
                         select new ViewDeal
                         {
                             Id = d.Id,
                             Account = d.Strategy.Account.Code,
                             Ticker = d.Strategy.Ticker.Code,
                             Strategy = d.Strategy.Code,
                             TimeInt = d.Strategy.TimeInt,
                             Side = d.Operation,
                             Qty = d.Quantity,
                             Price1 = d.Price1,
                             Price2 = d.Price2,
                             FirstTradeDT = d.FirstTradeDT,
                             LastTradeDT = d.LastTradeDT,
                             Format = d.Strategy.Ticker.Format,
                             FormatAvg = d.Strategy.Ticker.FormatAvg,
                         })
                    .OrderByDescending(d => d.LastTradeDT)
                    .ToList();
            }
            //else if ((tickerSelected.HasNoValue() || tickerSelected == "All") &&
            //        (accountSelected.HasValue() && accountSelected != "All"))
            else if ((tickerSelected.HasNoValue()) &&
                (accountSelected.HasValue()))
            {
                deals = (from d in db.Deals.Include(d => d.Strategy)
                         .Where(d => d.Strategy.Account.Code == accountSelected)
                         select new ViewDeal
                         {
                             Id = d.Id,
                             Account = d.Strategy.Account.Code,
                             Ticker = d.Strategy.Ticker.Code,
                             Strategy = d.Strategy.Code,
                             TimeInt = d.Strategy.TimeInt,
                             Side = d.Operation,
                             Qty = d.Quantity,
                             Price1 = d.Price1,
                             Price2 = d.Price2,
                             FirstTradeDT = d.FirstTradeDT,
                             LastTradeDT = d.LastTradeDT,
                             Format = d.Strategy.Ticker.Format,
                             FormatAvg = d.Strategy.Ticker.FormatAvg,
                         })
                    .OrderByDescending(d => d.LastTradeDT)
                    .ToList();
            }
            //else if ((tickerSelected.HasValue() && tickerSelected != "All") &&
            //       (accountSelected.HasValue() && accountSelected != "All"))
            else if ((tickerSelected.HasValue()) &&
              (accountSelected.HasValue()))
            {
                deals = (from d in db.Deals.Include(d => d.Strategy)
                         .Where(d => d.Strategy.Account.Code == accountSelected && d.Strategy.Ticker.Code == tickerSelected)
                         select new ViewDeal
                         {
                             Id = d.Id,
                             Account = d.Strategy.Account.Code,
                             Ticker = d.Strategy.Ticker.Code,
                             Strategy = d.Strategy.Code,
                             TimeInt = d.Strategy.TimeInt,
                             Side = d.Operation,
                             Qty = d.Quantity,
                             Price1 = d.Price1,
                             Price2 = d.Price2,
                             FirstTradeDT = d.FirstTradeDT,
                             LastTradeDT = d.LastTradeDT,
                             Format = d.Strategy.Ticker.Format,
                             FormatAvg = d.Strategy.Ticker.FormatAvg,
                         })
                    .OrderByDescending(d => d.LastTradeDT)
                    .ToList();
            }
            var tickerQry = from d in db.Deals.Include(d => d.Strategy)
                            orderby d.Strategy.Ticker.Code
                            select d.Strategy.Ticker.Code;
            var tickers = new List<string>();
            tickers.AddRange(tickerQry.Distinct());
            //ViewBag.tickerList = new SelectList(tickers, tickerList.HasValue()?tickerList:"All");
            ViewBag.tickerList = new SelectList(tickers);

            var accQry = from d in db.Deals.Include(d => d.Strategy)
                         orderby d.Strategy.Account.Code
                         select d.Strategy.Account.Code;
            var accs = new List<string>();
            accs.AddRange(accQry.Distinct());
            ViewBag.accountList = new SelectList(accs);

            var sQry = from d in db.Deals.Include(d => d.Strategy)
                       orderby d.Strategy.Code
                       select d.Strategy.Code;
            var ss = new List<string>();
            ss.AddRange(sQry.Distinct());
            ViewBag.strategyList = new SelectList(ss);

            ViewBag.Title = "Deals";
            ViewBag.FullTitle = "Deals ( " + deals.Count() + " )";
            ViewBag.ItemCount = deals.Count();

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(deals.ToPagedList(pageNumber, pageSize));

            //return View(deals);
        }

        private IEnumerable<ViewDeal> SelectViewDeals(string accountList, string symbolList, string tickerList, string strategyList,
            int? timeInt, string searchString, bool? lastDayChecked)
        {
            db.Database.CommandTimeout = DataBaseCommandTimeOut;

            IEnumerable<ViewDeal> deals = null;

            // LastDayChecked
            _lastDate = new DateTime().Date;
            _isLastDayChecked = lastDayChecked.HasValue && lastDayChecked == true;
            if (_isLastDayChecked)
            {
                var firstOrDefault = db.Deals.OrderByDescending(p => p.LastTradeDT).FirstOrDefault();

                if (firstOrDefault == null)
                {
                    ViewBag.Title = "Deals View is Empty";
                    ViewBag.LastDate = _lastDate;
                    return new List<ViewDeal>();
                }
                _lastDate = firstOrDefault.LastTradeDT.Date;
                ViewBag.LastDate = _lastDate;
            }
            //var isAccountValid = accountList != null && accountList.HasValue();
            //var isTickerValid = tickerList != null && tickerList.HasValue();
            //var isStrategyValid = strategyList != null && strategyList.HasValue();
            //var isSearchStrValid = searchString != null && searchString.HasValue();

            var isAccountValid = !string.IsNullOrWhiteSpace(accountList);
            var isSymbolValid = !string.IsNullOrWhiteSpace(symbolList);
            var isTickerValid = !string.IsNullOrWhiteSpace(tickerList);
            var isStrategyValid = !string.IsNullOrWhiteSpace(strategyList);
            var isSearchStrValid = !string.IsNullOrWhiteSpace(searchString);
            var isTimeIntValid = timeInt.HasValue && timeInt != 0;

            deals = (from d in db.Deals
                           //.Where(p => p.Strategy.TimeInt != 5)
                           //.Where(p => p.Strategy.TimeInt != 5 && p.Strategy.TimeInt != 10)
                           .Where(p => !_isLastDayChecked || DbFunctions.TruncateTime(p.LastTradeDT) == _lastDate)
                           .Where(d=>
                               (!isAccountValid || d.Strategy.Account.Code == accountList) &&
                               (!isSymbolValid || d.Strategy.Ticker.Code.Contains(symbolList)) &&
                               (!isTickerValid || d.Strategy.Ticker.Code == tickerList) &&
                               (!isStrategyValid || d.Strategy.Code == strategyList))
                           .Where(p => ! isTimeIntValid || p.Strategy.TimeInt == timeInt)
                           .Where(p => !isSearchStrValid ||
                                   (p.Strategy.Account.Code.Contains(searchString) ||
                                    p.Strategy.Ticker.Code.Contains(searchString) ||
                                    p.Strategy.Code.Contains(searchString)
                                   ))
                     select new ViewDeal
                     {
                         Id = d.Id,
                         Account = d.Strategy.Account.Code,
                         Ticker = d.Strategy.Ticker.Code,
                         Strategy = d.Strategy.Code,
                         TimeInt = d.Strategy.TimeInt,
                         Side = d.Operation,
                         Qty = d.Quantity,
                         Price1 = d.Price1,
                         Price2 = d.Price2,
                         FirstTradeDT = d.FirstTradeDT,
                         LastTradeDT = d.LastTradeDT,
                         Format = d.Strategy.Ticker.Format,
                         FormatAvg = d.Strategy.Ticker.FormatAvg,
                     })
                   .OrderByDescending(d => d.LastTradeDT)
                   .ToList();
            return deals;
        }
        private IEnumerable<TEntityOut> SelectViewDealsT<TEntityIn, TEntityOut, TDbSet>(DbSet<TEntityIn> dbSet,
            string accountList, string tickerList, string strategyList,
            int? timeInt, string searchString, bool? lastDayChecked)
            where TEntityIn : class
        {
            IEnumerable<TEntityOut> deals = null;

            // LastDayChecked
            var lastDate = new DateTime().Date;
            _isLastDayChecked = lastDayChecked.HasValue && lastDayChecked == true;
            if (_isLastDayChecked)
            {
                var firstOrDefault = db.Deals.OrderByDescending(p => p.LastTradeDT).FirstOrDefault();

                if (firstOrDefault == null)
                {
                    ViewBag.Title = "Deals View is Empty";
                    ViewBag.LastDate = lastDate;
                    return new List<TEntityOut>();
                }
                lastDate = firstOrDefault.LastTradeDT.Date;
                ViewBag.LastDate = lastDate;
            }
            //var isAccountValid = accountList != null && accountList.HasValue();
            //var isTickerValid = tickerList != null && tickerList.HasValue();
            //var isStrategyValid = strategyList != null && strategyList.HasValue();
            //var isSearchStrValid = searchString != null && searchString.HasValue();

            var isAccountValid = !string.IsNullOrWhiteSpace(accountList);
            var isTickerValid = !string.IsNullOrWhiteSpace(tickerList);
            var isStrategyValid = !string.IsNullOrWhiteSpace(strategyList);
            var isSearchStrValid = !string.IsNullOrWhiteSpace(searchString);

            //deals = (from d in db.Deals
            //               .Where(p => !isLastDayChecked || DbFunctions.TruncateTime(p.LastTradeDT) == lastDate)
            //               .Where(d =>
            //                   (!isAccountValid || d.Strategy.Account.Code == accountList) &&
            //                   (!isTickerValid || d.Strategy.Ticker.Code == tickerList) &&
            //                   (!isStrategyValid || d.Strategy.Code == strategyList))
            //               .Where(p => !timeInt.HasValue || p.Strategy.TimeInt == timeInt)
            //               .Where(p => !isSearchStrValid ||
            //                       (p.Strategy.Account.Code.Contains(searchString) ||
            //                        p.Strategy.Ticker.Code.Contains(searchString) ||
            //                        p.Strategy.Code.Contains(searchString)
            //                       ))
            //         select new ViewDeal
            //         {
            //             Id = d.Id,
            //             Account = d.Strategy.Account.Code,
            //             Ticker = d.Strategy.Ticker.Code,
            //             Strategy = d.Strategy.Code,
            //             TimeInt = d.Strategy.TimeInt,
            //             Side = d.Operation,
            //             Qty = d.Quantity,
            //             Price1 = d.Price1,
            //             Price2 = d.Price2,
            //             FirstTradeDT = d.FirstTradeDT,
            //             LastTradeDT = d.LastTradeDT,
            //             Format = d.Strategy.Ticker.Format,
            //             FormatAvg = d.Strategy.Ticker.FormatAvg,
            //         })
            //       .OrderByDescending(d => d.LastTradeDT)
            //       .ToList();



            return deals;
        }
        private IEnumerable<ViewDeal> SelectViewDeals1(string accountList, string tickerList, string strategyList,
             int? timeInt, string searchString, bool? lastDayChecked)
        {
            IEnumerable<ViewDeal> deals = null;

            // LastDayChecked
            var lastDate = new DateTime().Date;
            _isLastDayChecked = lastDayChecked.HasValue && lastDayChecked == true;
            if (_isLastDayChecked)
            {
                var firstOrDefault = db.Deals.OrderByDescending(p => p.LastTradeDT).FirstOrDefault();
              
                if (firstOrDefault == null)
                {
                    ViewBag.Title = "Deals View is Empty";
                    ViewBag.LastDate = lastDate;
                    return new List<ViewDeal>();
                }
                lastDate = firstOrDefault.LastTradeDT.Date;
                ViewBag.LastDate = lastDate;
            }
            var isSearchStrValid = searchString != null && searchString.HasValue();

            if (accountList.HasNoValue() && tickerList.HasNoValue() && strategyList.HasNoValue())
            {
                
                    deals = (from d in db.Deals
                                .Where(p => !_isLastDayChecked || DbFunctions.TruncateTime(p.LastTradeDT) == lastDate)
                                .Where(p=> !timeInt.HasValue || p.Strategy.TimeInt == timeInt)
                                .Where(p => !isSearchStrValid || 
                                    (p.Strategy.Account.Code.Contains(searchString) ||
                                     p.Strategy.Ticker.Code.Contains(searchString) ||
                                     p.Strategy.Code.Contains(searchString)
                                    ))
                        select new ViewDeal
                        {
                            Id = d.Id,
                            Account = d.Strategy.Account.Code,
                            Ticker = d.Strategy.Ticker.Code,
                            Strategy = d.Strategy.Code,
                            TimeInt = d.Strategy.TimeInt,
                            Side = d.Operation,
                            Qty = d.Quantity,
                            Price1 = d.Price1,
                            Price2 = d.Price2,
                            FirstTradeDT = d.FirstTradeDT,
                            LastTradeDT = d.LastTradeDT,
                            Format = d.Strategy.Ticker.Format,
                            FormatAvg = d.Strategy.Ticker.FormatAvg,
                        })
                        .OrderByDescending(d => d.LastTradeDT)
                        .ToList();
               
                return deals;
            }
            if (accountList.HasValue() && tickerList.HasNoValue() && strategyList.HasNoValue())
            {
                deals = (from d in db.Deals
                    .Where(p => !_isLastDayChecked || DbFunctions.TruncateTime(p.LastTradeDT) == lastDate)
                    .Where(d => d.Strategy.Account.Code == accountList)
                    .Where(p => !timeInt.HasValue || p.Strategy.TimeInt == timeInt)
                    .Where(p => !isSearchStrValid ||
                                    (p.Strategy.Account.Code.Contains(searchString) ||
                                     p.Strategy.Ticker.Code.Contains(searchString) ||
                                     p.Strategy.Code.Contains(searchString)
                                    ))
                    select new ViewDeal
                    {
                        Id = d.Id,
                        Account = d.Strategy.Account.Code,
                        Ticker = d.Strategy.Ticker.Code,
                        Strategy = d.Strategy.Code,
                        TimeInt = d.Strategy.TimeInt,
                        Side = d.Operation,
                        Qty = d.Quantity,
                        Price1 = d.Price1,
                        Price2 = d.Price2,
                        FirstTradeDT = d.FirstTradeDT,
                        LastTradeDT = d.LastTradeDT,
                        Format = d.Strategy.Ticker.Format,
                        FormatAvg = d.Strategy.Ticker.FormatAvg,
                    })
                    .OrderByDescending(d => d.LastTradeDT)
                    .ToList();
             //   return deals;
            }
            else if (accountList.HasValue() && tickerList.HasValue() && strategyList.HasNoValue())
            {
                deals = (from d in db.Deals
                            .Where(p => !_isLastDayChecked || DbFunctions.TruncateTime(p.LastTradeDT) == lastDate)
                            .Where(d => d.Strategy.Account.Code == accountList && 
                                    d.Strategy.Ticker.Code == tickerList)
                            .Where(p => !timeInt.HasValue || p.Strategy.TimeInt == timeInt)
                            .Where(p => !isSearchStrValid ||
                                    (p.Strategy.Account.Code.Contains(searchString) ||
                                     p.Strategy.Ticker.Code.Contains(searchString) ||
                                     p.Strategy.Code.Contains(searchString)
                                    ))
                         select new ViewDeal
                         {
                             Id = d.Id,
                             Account = d.Strategy.Account.Code,
                             Ticker = d.Strategy.Ticker.Code,
                             Strategy = d.Strategy.Code,
                             TimeInt = d.Strategy.TimeInt,
                             Side = d.Operation,
                             Qty = d.Quantity,
                             Price1 = d.Price1,
                             Price2 = d.Price2,
                             FirstTradeDT = d.FirstTradeDT,
                             LastTradeDT = d.LastTradeDT,
                             Format = d.Strategy.Ticker.Format,
                             FormatAvg = d.Strategy.Ticker.FormatAvg,
                         })
                    .OrderByDescending(d => d.LastTradeDT)
                    .ToList();
            }
            else if (accountList.HasValue() && tickerList.HasNoValue() && strategyList.HasValue())
            {
                deals = (from d in db.Deals
                            .Where(p => !_isLastDayChecked || DbFunctions.TruncateTime(p.LastTradeDT) == lastDate)
                            .Where(d => d.Strategy.Account.Code == accountList &&
                                d.Strategy.Code == strategyList)
                            .Where(p => !timeInt.HasValue || p.Strategy.TimeInt == timeInt)
                            .Where(p => !isSearchStrValid ||
                                    (p.Strategy.Account.Code.Contains(searchString) ||
                                     p.Strategy.Ticker.Code.Contains(searchString) ||
                                     p.Strategy.Code.Contains(searchString)
                                    ))
                         select new ViewDeal
                         {
                             Id = d.Id,
                             Account = d.Strategy.Account.Code,
                             Ticker = d.Strategy.Ticker.Code,
                             Strategy = d.Strategy.Code,
                             TimeInt = d.Strategy.TimeInt,
                             Side = d.Operation,
                             Qty = d.Quantity,
                             Price1 = d.Price1,
                             Price2 = d.Price2,
                             FirstTradeDT = d.FirstTradeDT,
                             LastTradeDT = d.LastTradeDT,
                             Format = d.Strategy.Ticker.Format,
                             FormatAvg = d.Strategy.Ticker.FormatAvg,
                         })
                    .OrderByDescending(d => d.LastTradeDT)
                    .ToList();
            }
            else if (accountList.HasValue() && tickerList.HasValue() && strategyList.HasValue())
            {
                deals = (from d in db.Deals
                            .Where(p => !_isLastDayChecked || DbFunctions.TruncateTime(p.LastTradeDT) == lastDate)
                            .Where(d => d.Strategy.Account.Code == accountList &&
                                d.Strategy.Ticker.Code == tickerList &&
                                d.Strategy.Code == strategyList)
                            .Where(p => !timeInt.HasValue || p.Strategy.TimeInt == timeInt)
                            .Where(p => !isSearchStrValid ||
                                    (p.Strategy.Account.Code.Contains(searchString) ||
                                     p.Strategy.Ticker.Code.Contains(searchString) ||
                                     p.Strategy.Code.Contains(searchString)
                                    ))
                         select new ViewDeal
                         {
                             Id = d.Id,
                             Account = d.Strategy.Account.Code,
                             Ticker = d.Strategy.Ticker.Code,
                             Strategy = d.Strategy.Code,
                             TimeInt = d.Strategy.TimeInt,
                             Side = d.Operation,
                             Qty = d.Quantity,
                             Price1 = d.Price1,
                             Price2 = d.Price2,
                             FirstTradeDT = d.FirstTradeDT,
                             LastTradeDT = d.LastTradeDT,
                             Format = d.Strategy.Ticker.Format,
                             FormatAvg = d.Strategy.Ticker.FormatAvg,
                         })
                    .OrderByDescending(d => d.LastTradeDT)
                    .ToList();
            }
            else if (accountList.HasNoValue() && tickerList.HasValue() && strategyList.HasNoValue())
            {
                deals = (from d in db.Deals
                            .Where(p => !_isLastDayChecked || DbFunctions.TruncateTime(p.LastTradeDT) == lastDate)
                            .Where(d => d.Strategy.Ticker.Code == tickerList)
                            .Where(p => !timeInt.HasValue || p.Strategy.TimeInt == timeInt)
                            .Where(p => !isSearchStrValid ||
                                    (p.Strategy.Account.Code.Contains(searchString) ||
                                     p.Strategy.Ticker.Code.Contains(searchString) ||
                                     p.Strategy.Code.Contains(searchString)
                                    ))
                         select new ViewDeal
                         {
                             Id = d.Id,
                             Account = d.Strategy.Account.Code,
                             Ticker = d.Strategy.Ticker.Code,
                             Strategy = d.Strategy.Code,
                             TimeInt = d.Strategy.TimeInt,
                             Side = d.Operation,
                             Qty = d.Quantity,
                             Price1 = d.Price1,
                             Price2 = d.Price2,
                             FirstTradeDT = d.FirstTradeDT,
                             LastTradeDT = d.LastTradeDT,
                             Format = d.Strategy.Ticker.Format,
                             FormatAvg = d.Strategy.Ticker.FormatAvg,
                         })
                    .OrderByDescending(d => d.LastTradeDT)
                    .ToList();
            }
            else if (accountList.HasNoValue() && tickerList.HasNoValue() && strategyList.HasValue())
            {
                deals = (from d in db.Deals
                            .Where(p => !_isLastDayChecked || DbFunctions.TruncateTime(p.LastTradeDT) == lastDate)
                            .Where(d => d.Strategy.Code == strategyList)
                            .Where(p => !timeInt.HasValue || p.Strategy.TimeInt == timeInt)
                            .Where(p => !isSearchStrValid ||
                                    (p.Strategy.Account.Code.Contains(searchString) ||
                                     p.Strategy.Ticker.Code.Contains(searchString) ||
                                     p.Strategy.Code.Contains(searchString)
                                    ))
                         select new ViewDeal
                         {
                             Id = d.Id,
                             Account = d.Strategy.Account.Code,
                             Ticker = d.Strategy.Ticker.Code,
                             Strategy = d.Strategy.Code,
                             TimeInt = d.Strategy.TimeInt,
                             Side = d.Operation,
                             Qty = d.Quantity,
                             Price1 = d.Price1,
                             Price2 = d.Price2,
                             FirstTradeDT = d.FirstTradeDT,
                             LastTradeDT = d.LastTradeDT,
                             Format = d.Strategy.Ticker.Format,
                             FormatAvg = d.Strategy.Ticker.FormatAvg,
                         })
                    .OrderByDescending(d => d.LastTradeDT)
                    .ToList();
            }
            else if (accountList.HasNoValue() && tickerList.HasValue() && strategyList.HasValue())
            {
                deals = (from d in db.Deals
                            .Where(p => !_isLastDayChecked || DbFunctions.TruncateTime(p.LastTradeDT) == lastDate)
                            .Where(d => d.Strategy.Ticker.Code == tickerList &&
                                        d.Strategy.Code == strategyList)
                            .Where(p => !timeInt.HasValue || p.Strategy.TimeInt == timeInt)
                            .Where(p => !isSearchStrValid ||
                                    (p.Strategy.Account.Code.Contains(searchString) ||
                                     p.Strategy.Ticker.Code.Contains(searchString) ||
                                     p.Strategy.Code.Contains(searchString)
                                    ))
                         select new ViewDeal
                         {
                             Id = d.Id,
                             Account = d.Strategy.Account.Code,
                             Ticker = d.Strategy.Ticker.Code,
                             Strategy = d.Strategy.Code,
                             TimeInt = d.Strategy.TimeInt,
                             Side = d.Operation,
                             Qty = d.Quantity,
                             Price1 = d.Price1,
                             Price2 = d.Price2,
                             FirstTradeDT = d.FirstTradeDT,
                             LastTradeDT = d.LastTradeDT,
                             Format = d.Strategy.Ticker.Format,
                             FormatAvg = d.Strategy.Ticker.FormatAvg,
                         })
                    .OrderByDescending(d => d.LastTradeDT)
                    .ToList();
            }

            return deals;
        }

        #region Charts
        public ActionResult Chart(string account, string symbol, string ticker, string strategy, 
                                        int? timeInt, string searchString, bool? lastDayChecked, int? charttype)
        {
            db.Database.CommandTimeout = DataBaseCommandTimeOut;

            ViewBag.Account = account;
            ViewBag.Symbol = symbol;
            ViewBag.Ticker = ticker;
            ViewBag.Strategy = strategy;
            ViewBag.TimeInt = timeInt;
            ViewBag.SearchString = searchString;
            ViewBag.LastDayChecked = lastDayChecked; 

            ViewBag.ChartType = charttype;
            
            // Total Profit
            //
            var deals = SelectViewDeals(account, symbol, ticker, strategy, timeInt, searchString, lastDayChecked);
            var total = deals != null
                            ? deals.Sum(d => d.PnL)
                            : 0;
            ViewBag.Profit = total.ToString("N2");
            
            _isLastDayChecked = lastDayChecked.HasValue && lastDayChecked == true;

            //DateTime lastDate = new DateTime().Date;
            //if (_isLastDayChecked)
            //    lastDate = ViewBag.LastDate;

            //var title =
            //    (!string.IsNullOrWhiteSpace(account) ? "Account: " + account + " " : "" ) +
            //    (!string.IsNullOrWhiteSpace(ticker) ? "Ticker: " + ticker + " " : "") +
            //    (!string.IsNullOrWhiteSpace(strategy) ? "Strategy: " + strategy + " " : "") +
            //    (timeInt.HasValue ? "TimeInt: " + timeInt.ToString() + " " : "") +
            //    (!string.IsNullOrWhiteSpace(search) ? "Search: " + search + " " : "") +
            //    (lastDayChecked.HasValue ? "LastDate Only " : "" )
            //    ;
            var title =
                "Account: " + (!string.IsNullOrWhiteSpace(account) ? account + " | " : "All | ") +
                "Symbol: " + (!string.IsNullOrWhiteSpace(symbol) ? symbol + " | " : "All | ") +
                "Ticker: " + (!string.IsNullOrWhiteSpace(ticker) ? ticker + " | " : "All | ") +
                "Strategy: " + (!string.IsNullOrWhiteSpace(strategy) ? strategy + " | " : "All | ") +
                "TimeInt: " + (timeInt.HasValue ? timeInt + " | " : "All | ") +
                "Search: " + (!string.IsNullOrWhiteSpace(searchString) ? searchString + " | " : "Nothing | ") +
                //(_isLastDayChecked ? "Last Date Only: " + _lastDate.ToString("d") : "All Dates")
                (lastDayChecked.HasValue && lastDayChecked == true ? "Last Date Only " : "All Dates")
                ;
            ViewBag.FullTitle = title;
            ViewBag.Title = "ChartView";
            return View();
        }

        public ActionResult Chart1(string account, string symbol, string ticker, string strategy,
            int? timeInt, string searchString, bool? lastDayChecked, int? chartType)
        {

            ViewBag.Account = account;
            ViewBag.Symbol = symbol;
            ViewBag.Ticker = ticker;
            ViewBag.Strategy = strategy;
            ViewBag.TimeInt = timeInt;
            ViewBag.SearchString = searchString;
            ViewBag.LastDayChecked = lastDayChecked;

            //ViewBag.ChartType = chartType;

            int charttype;
            if (chartType == null)
                charttype = 1;
            else
                charttype = (int)chartType;

            ViewBag.ChartType = charttype;

            var eq = GetEquity(account, symbol, ticker, strategy,
                                  timeInt, searchString, lastDayChecked, charttype);

            var chart = new System.Web.UI.DataVisualization.Charting.Chart { Width = 1200, Height = 750 };

            ChartArea chartArea = new ChartArea("Equity");
            chart.ChartAreas.Add(chartArea);
            chartArea.BackColor = Color.Transparent;

            Title title = new Title();
            title.Text = "Equity Chart. " + Equity.GetTimeIntTitle(charttype);
            title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            title.Font = new Font("Trebuchet MS", 10F, FontStyle.Bold);
            title.ShadowOffset = 3;
            title.ForeColor = Color.FromArgb(26, 59, 105);

            chart.Titles.Add(title);

            Legend legend = new Legend();
            legend.Enabled = true;
            legend.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            legend.Font = new Font("Trebuchet MS", 8F, FontStyle.Bold);
            legend.ShadowOffset = 3;
            legend.ForeColor = Color.FromArgb(26, 59, 105);
            legend.Alignment = StringAlignment.Center;
            legend.Docking = Docking.Bottom;
            //legend.Title = "Legend";

            chart.Legends.Add(legend);

            chart.Series.Add("Changes");
            //chart.Series.Add("NegChanges");
            chart.Series.Add("Values");

            chart.Series["Changes"].ChartArea = "Equity";
            //chart.Series["NegChanges"].ChartArea = "Equity";
            chart.Series["Values"].ChartArea = "Equity";

            chart.Series["Changes"].ChartType = SeriesChartType.Column;
            //chart.Series["Changes"].Color = Color.CornflowerBlue;
            //chart.Series["NegChanges"].ChartType = SeriesChartType.Column;
            //chart.Series["NegChanges"].Color = Color.Crimson;
            chart.Series["Values"].ChartType = SeriesChartType.Spline;
            chart.Series["Values"].Color = Color.Red;
            chart.Series["Values"].BorderWidth = 3;
            chart.Series["Values"].MarkerBorderColor = Color.Black;
            chart.Series["Values"].MarkerBorderWidth = 1;
            chart.Series["Values"].MarkerColor = Color.Red;
            chart.Series["Values"].MarkerSize = 8;
            chart.Series["Values"].MarkerStyle = MarkerStyle.Circle;

            chart.Series["Changes"].Points.Clear();
            //chart.Series["NegChanges"].Points.Clear();
            chart.Series["Values"].Points.Clear();

            foreach (var ei in eq.EquityItems)
            {
                chart.Series["Changes"].Points.Add(ei.Change);
                chart.Series["Values"].Points.Add(ei.Value);
            }
            // Try to Add Negotive Positive Chart
            //foreach (var ei in eq.EquityItems)
            //{
            //    if (ei.Change > 0)
            //    {
            //        //chart.Series["Changes"].Color = Color.CornflowerBlue;
            //        chart.Series["Changes"].Points.Add(ei.Change);
            //        //chart.Series["NegChanges"].Points.Add(0);
            //        chart.Series["NegChanges"].Points.Add(null);
            //    }
            //    else
            //    {
            //        //chart.Series["Changes"].Color = Color.IndianRed;
            //        //chart.Series["Changes"].Points.Add(ei.Change);
            //        //chart.Series["Changes"].Points.Add(0);
            //        chart.Series["Changes"].Points.Add(null);
            //        chart.Series["NegChanges"].Points.Add(ei.Change);
            //    }
            //    chart.Series["Values"].Points.Add(ei.Value);
            //}
            StringBuilder result = new StringBuilder();
            result.Append(getChartImage(chart));
            result.Append(chart.GetHtmlImageMap("ImageMap"));
            return Content(result.ToString());
        }
        private string getChartImage(System.Web.UI.DataVisualization.Charting.Chart chart)
        {
            using (var stream = new MemoryStream())
            {
                string img = "<img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap'>";
                chart.SaveImage(stream, ChartImageFormat.Png);
                string encoded = Convert.ToBase64String(stream.ToArray());
                return String.Format(img, encoded);
            }
        }

        public class EquityItem
        {
            public double Change { get; set; }
            public double Value { get; set; }
        }

        private Equity GetEquity(string account, string symbol, string ticker, string strategy,
            int? timeInt, string search, bool? lastDayChecked, int chartTimeInt)
        {
            var trs = SelectViewDeals(account, symbol, ticker, strategy, timeInt, search, lastDayChecked);
            var eq = new Equity(chartTimeInt);
            foreach (var t in trs)
            {
                eq.Update(t.LastTradeDT, (double)t.PnL);
            }
            return eq;
        }
        public ActionResult Report(string account, string symbol, string ticker, string strategy,
            int? timeInt, string searchString, bool? lastDayChecked,
            int? chartType)
        {

            ViewBag.Account = account;
            ViewBag.Symbol = symbol;
            ViewBag.Ticker = ticker;
            ViewBag.Strategy = strategy;
            ViewBag.TimeInt = timeInt;
            ViewBag.SearchString = searchString;
            ViewBag.LastDayChecked = lastDayChecked;
            ViewBag.ChartType = chartType;


            var trs = SelectViewDeals(account, symbol, ticker, strategy, timeInt, searchString, lastDayChecked);
            var lst = trs.Select(t => new OmegaReport.TradeItem
            {
                Dt1 = t.FirstTradeDT,
                Dt2 = t.LastTradeDT,
                Profit = (double)t.PnL,
                Cost = 1
            }).ToList();
            var r = new OmegaReport();
            r.Update(lst);

            _isLastDayChecked = lastDayChecked.HasValue && lastDayChecked == true;

            //var title =
            //    (!string.IsNullOrWhiteSpace(account) ? "Account: " + account + " " : "") +
            //    (!string.IsNullOrWhiteSpace(ticker) ? "Ticker: " + ticker + " " : "") +
            //    (!string.IsNullOrWhiteSpace(strategy) ? "Strategy: " + strategy + " " : "") +
            //    (timeInt.HasValue ? "TimeInt: " + timeInt.ToString() + " " : "") +
            //    (!string.IsNullOrWhiteSpace(search) ? "Search: " + search + " " : "") +
            //    (lastDayChecked.HasValue ? "LastDate Only " : "")
            //    ;
            var title =
                "Account: " + (!string.IsNullOrWhiteSpace(account) ? account + " | " : "All | ") +
                "Symbol: " + (!string.IsNullOrWhiteSpace(symbol) ? symbol + " | " : "All | ") +
                "Ticker: " + (!string.IsNullOrWhiteSpace(ticker) ? ticker + " | " : "All | ") +
                "Strategy: " + (!string.IsNullOrWhiteSpace(strategy) ? strategy + " | " : "All | ") +
                "TimeInt: " + (timeInt.HasValue ? timeInt + " | " : "All | ") +
                "Search: " + (!string.IsNullOrWhiteSpace(searchString) ? searchString + " | " : "Nothing | ") +
                //(_isLastDayChecked ? "Last Date Only: " + ViewBag.LastDate.ToString("d") : "All Dates")
                (_isLastDayChecked ? "Last Date Only: " + _lastDate.ToString("d") : "All Dates")
                ;
            ViewBag.FullTitle = title;
            ViewBag.Title = "Omega Research Performance Report";

            return View(r);
        }

        #endregion

        // GET: Deals/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deal deal = db.Deals.Find(id);
            if (deal == null)
            {
                return HttpNotFound();
            }
            return View(deal);
        }

        // GET: Deals/Create
        public ActionResult Create()
        {
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key");
            return View();
        }

        // POST: Deals/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Key,DT,Number,Operation,Status,Price1,Price2,Quantity,FirstTradeDT,FirstTradeNumber,LastTradeDT,LastTradeNumber,StrategyId")] Deal deal)
        {
            if (ModelState.IsValid)
            {
                db.Deals.Add(deal);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key", deal.StrategyId);
            return View(deal);
        }

        // GET: Deals/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deal deal = db.Deals.Find(id);
            if (deal == null)
            {
                return HttpNotFound();
            }
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key", deal.StrategyId);
            return View(deal);
        }

        // POST: Deals/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Key,DT,Number,Operation,Status,Price1,Price2,Quantity,FirstTradeDT,FirstTradeNumber,LastTradeDT,LastTradeNumber,StrategyId")] Deal deal)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deal).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StrategyId = new SelectList(db.Strategies, "Id", "Key", deal.StrategyId);
            return View(deal);
        }

        // GET: Deals/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deal deal = db.Deals.Find(id);
            if (deal == null)
            {
                return HttpNotFound();
            }
            return View(deal);
        }

        // POST: Deals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Deal deal = db.Deals.Find(id);
            db.Deals.Remove(deal);
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
