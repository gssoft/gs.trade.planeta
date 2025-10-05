using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GS.Extension;
using GS.Trade.DataBase.Dal;

namespace GS.Trade.Web.Mvc_07.Functions
{
    public class Gen
    {
        public static void FilterFormCreate_01(DbTradeContext db)
        {
            // Create DropDown SelectList
            // Ticker List
            var tickerQry = from d in db.Deals.Include(d => d.Strategy)
                            //orderby d.Strategy.Ticker.Code
                            select d.Strategy.Ticker.Code;
            var tickers = new List<string>();
            tickers.AddRange(tickerQry.Distinct().OrderBy(p => p));
            //ViewBag.tickerList = new SelectList(tickers, tickerList.HasValue()?tickerList:"All");
            //ViewBag.tickerList = new SelectList(tickers);

            // Symbols
            var symbols = tickers.Select(t => t.Replace(t.Right(2), ""))
                .Distinct()
                .ToList();
            //ViewBag.symbolList = new SelectList(symbols);

            // Account List
            var accQry = from d in db.Deals.Include(d => d.Strategy)
                         //orderby d.Strategy.Account.Code
                         select d.Strategy.Account.Code;
            var accs = new List<string>();
            accs.AddRange(accQry.Distinct().OrderBy(p => p));
            //ViewBag.accountList = new SelectList(accs);
            // Strategy List
            var sQry = from d in db.Deals.Include(d => d.Strategy)
                       //orderby d.Strategy.Code
                       select d.Strategy.Code;
            var ss = new List<string>();
            ss.AddRange(sQry.Distinct().OrderBy(p => p));
            //ViewBag.strategyList = new SelectList(ss);

            var tiQry = from d in db.Deals.Include(d => d.Strategy)
                        select d.Strategy.TimeInt;
            var tis = new List<int>();
            tis.AddRange(tiQry.Distinct().OrderBy(p => p));
            //ViewBag.timeIntList = new SelectList(tis);
            //ViewBag.lastDayChecked = new CheckBox();
        }
    }
}