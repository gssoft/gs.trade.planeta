using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;
using GS.Trade.DataBase.Dal;
using GS.Trade.DataBase.Model;

namespace GS.Trade.DataBase.Functions
{
    public class DealsFunctions
    {
        public static DateTime? GetDealsLastDateTime(DbTradeContext db)
        {
            return db.Deals.Any()
                ? db.Deals.Max(d => d.LastTradeDT).MinValueToSql().Date
                //: DateTime.Now;
                //: (DateTime) SqlDateTime.MinValue;
                : (DateTime?)null;
        }
        public static IQueryable<Deal> SelectDeals(DbTradeContext db,
                    string accountList, string symbolList, string tickerList,
                    string strategyList,
                    int? timeInt, string searchString, bool? lastDayChecked)
        {

            // LastDayChecked
            var lastDate = GetDealsLastDateTime(db);
            if (lastDate == null)
            {
                return default(IQueryable<Deal>);
            }
            var isLastDayChecked = lastDayChecked.HasValue && lastDayChecked == true;
            var isAccountValid = !string.IsNullOrWhiteSpace(accountList);
            var isSymbolValid = !string.IsNullOrWhiteSpace(symbolList);
            var isTickerValid = !string.IsNullOrWhiteSpace(tickerList);
            var isStrategyValid = !string.IsNullOrWhiteSpace(strategyList);
            var isSearchStrValid = !string.IsNullOrWhiteSpace(searchString);

            var deals = (db.Deals
                //.Where(p => p.Strategy.TimeInt != 5)
                //.Where(p => p.Strategy.TimeInt != 5 && p.Strategy.TimeInt != 10)
                .Where(p => !isLastDayChecked || DbFunctions.TruncateTime(p.LastTradeDT) == lastDate)
                .Where(d =>
                    (!isAccountValid || d.Strategy.Account.Code == accountList) &&
                    (!isSymbolValid || d.Strategy.Ticker.Code.Contains(symbolList)) &&
                    (!isTickerValid || d.Strategy.Ticker.Code == tickerList) &&
                    (!isStrategyValid || d.Strategy.Code == strategyList))
                .Where(p => !timeInt.HasValue || p.Strategy.TimeInt == timeInt)
                .Where(p => !isSearchStrValid ||
                            (p.Strategy.Account.Code.Contains(searchString) ||
                             p.Strategy.Ticker.Code.Contains(searchString) ||
                             p.Strategy.Code.Contains(searchString)
                                )))
                //.OrderByDescending(d => d.LastTradeDT)
                //.ToList()
                ;
            return deals;
        }

        public static IList<string> GetDealsTickers(DbTradeContext db)
        {
            var tickerQry = from d in db.Deals
                            //.Include(d => d.Strategy)
                            //orderby d.Strategy.Ticker.Code
                            select d.Strategy.Ticker.Code;
            var tickers = new List<string>();
            tickers.AddRange(tickerQry.Distinct().OrderBy(p => p));
            return tickers;
        }
        public static IList<string> GetDealsSymbols(DbTradeContext db)
        {
            var tickers = GetDealsTickers(db);
            return tickers.Select(t => t.Replace(t.Right(2), ""))
                .Distinct()
                .ToList();
        }

        public static IList<string> GetSymbols(IList<string> tickers)
        {
            return tickers.Select(t => t.Replace(t.Right(2), ""))
                .Distinct()
                .ToList();
        }

        public static IList<string> GetDealsStrategies(DbTradeContext db)
        {
            var qry = from d in db.Deals
                      select d.Strategy.Code;
            var lst = new List<string>();
            lst.AddRange(qry.Distinct().OrderBy(p => p));
            return lst;
        }
        public static IList<string> GetDealsAccounts(DbTradeContext db)
        {
            var qry = from d in db.Deals
                      select d.Strategy.Account.Code;
            var lst = new List<string>();
            lst.AddRange(qry.Distinct().OrderBy(p => p));
            return lst;
        }
        public static IList<int> GetDealsTimeInts(DbTradeContext db)
        {
            var qry = (from d in db.Deals
                       select d.Strategy.TimeInt);
            var lst = new List<int>();
            lst.AddRange(qry.Distinct().OrderBy(p => p));
            return lst;
        }
    }
}
