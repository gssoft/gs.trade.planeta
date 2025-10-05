using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;
using GS.Trade.DataBase.Model;
using GS.Trade.Trades;

namespace GS.Trade.DataBase.Dal
{
    public partial class DbTradeContext
    {
        #region From Deals

        public DateTime? GetDealsLastDateTime()
        {
            return Deals.Any()
                ? Deals.Max(d => d.LastTradeDT).MinValueToSql().Date
                //: DateTime.Now;
                //: (DateTime) SqlDateTime.MinValue;
                : (DateTime?)null;
        }
        public  IQueryable<Deal> SelectDeals(
                    string accountList, string symbolList, string tickerList,
                    string strategyList,
                    int? timeInt, string searchString, bool? lastDayChecked)
        {

            // LastDayChecked
            var lastDate = GetDealsLastDateTime();
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

            var deals = (Deals
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

        public IList<PositionDb2> SelectTotalsGroupedFromDeals(IQueryable<Deal> deals,
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
            else if (!isAccountGroup && isSymbolGroup && isTickerGroup && isStrategyGroup && isTimeIntGroup)
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

        public IQueryable<object> SelectDealsGroupedProba()
        {
            var v = (from p in Deals
                 group p by new
                 {
                     Strategy  = p.Strategy,
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
                     select new
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
                         PnL3 = g.Sum(p => 0).ToString(g.Key.Format),
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
                   .ThenBy(p => p.TimeInt);
                   //.ToList();
            return v;
        }

        public IList<string> GetDealsTickers()
        {
            var tickerQry = from d in Deals
                            //.Include(d => d.Strategy)
                            //orderby d.Strategy.Ticker.Code
                            select d.Strategy.Ticker.Code;
            var tickers = new List<string>();
            tickers.AddRange(tickerQry.Distinct().OrderBy(p => p));
            return tickers;
        }
        public IList<string> GetDealsSymbols()
        {
            var tickers = GetDealsTickers();
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

        public IList<string> GetDealsStrategies()
        {
            var qry = from d in Deals
                      select d.Strategy.Code;
            var lst = new List<string>();
            lst.AddRange(qry.Distinct().OrderBy(p => p));
            return lst;
        }
        public IList<string> GetDealsAccounts()
        {
            var qry = from d in Deals
                      select d.Strategy.Account.Code;
            var lst = new List<string>();
            lst.AddRange(qry.Distinct().OrderBy(p => p));
            return lst;
        }
        public IList<int> GetDealsTimeInts()
        {
            var qry = (from d in Deals
                       select d.Strategy.TimeInt);
            var lst = new List<int>();
            lst.AddRange(qry.Distinct().OrderBy(p => p));
            return lst;
        }

        #endregion
    }
}
