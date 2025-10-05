using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using EntityFramework.BulkInsert.Extensions;
using GS.Trade.TimeSeries.BarSeries;
using GS.Trade.TimeSeries.BarSeries.Models;
using GS.Trade.TimeSeries.General;
using GS.Trade.TimeSeries.Interfaces;
using Microsoft.Win32.SafeHandles;

namespace GS.Trade.TimeSeries.BarSeries.Dal
{
    public partial class BarSeriesContext01
    {
        public IEnumerable<Models.TimeSeries> GetBarSeriesByTimeInt(int timeInt)
        {
            return TimeSeries
                    .Where(tm => tm.TimeInt.TimeInterval == timeInt).ToList();
        }
        public Stat GetSeriesStat(string ticker, int? timeInt)
        {
            var qpCode = "Forts.Pub.Stat";
            //var t = GetTickerByCode(ticker);
            //if(t==null)
            //    return null;
            //var qp = GetQuoteProviderByCode("Forts.Pub.Stat");
            //if (qp == null)
            //    return null;
            //var bs = TimeSeries
            //            .FirstOrDefault(s => s.TickerID == t.ID && 
            //                            s.TimeInt.TimeInterval == timeInt &&
            //                            s.QuoteProviderID == qp.ID);
            var bs = TimeSeries
                .FirstOrDefault(s => (s.Ticker.Code == ticker.Trim() || s.Ticker.Name == ticker.Trim()) &&
                                     s.TimeInt.TimeInterval == timeInt &&
                                     (s.QuoteProvider.Code == qpCode || s.QuoteProvider.Name == qpCode));
            return bs == null 
                ? null
                : Stats.FirstOrDefault(s => s.TimeSeriesID == bs.ID);
        }
        public IEntityTimeSeriesStat GetDailySeriesStat(string ticker, int timeint, int date)
        {
            var st = Stats
                    .FirstOrDefault(s => 
                                    s.Period == TimeSeriesStatEnum.Daily &&
                                    s.Type == TickBarTypeEnum.Bars &&
                                    s.LastDate == date &&
                                    s.TimeSeries.Ticker.Code == ticker.Trim() &&
                                    s.TimeSeries.TimeInt.TimeInterval == timeint);

            return st != null
                ? new EntityTimeSeriesStat
                {
                    EntityID = st.TimeSeries.TickerID,
                    EntityCode = st.TimeSeries.Code,
                    EntityName = st.TimeSeries.Code,
                    Count = st.Count,
                    FirstDT = st.FirstDT,
                    LastDT = st.LastDT
                }
                : null;
        }
        public IEnumerable<IEntityTimeSeriesStat> GetDailySeriesStats(string ticker, int timeint)
        {
            var r = Stats
                    .Where(s => s.Period == TimeSeriesStatEnum.Daily &&
                                s.Type == TickBarTypeEnum.Bars &&
                                s.TimeSeries.Ticker.Code == ticker.Trim() &&
                                s.TimeSeries.TimeInt.TimeInterval == timeint)
                .Select(s => new EntityTimeSeriesStat
                {
                    EntityID = s.TimeSeries.TickerID,
                    EntityCode = s.TimeSeries.Code,
                    EntityName = s.TimeSeries.Code,
                    Count = s.Count,
                    FirstDT = s.FirstDT,
                    LastDT = s.LastDT
                })
                .OrderBy(t => t.LastDate)
                .ToList();
            return r;
        }

        public Ticker GetTickerByCode(string code)
        {
            return Tickers.FirstOrDefault(t => t.Code.Equals(code.Trim()))
                    ?? Tickers.FirstOrDefault(t => t.Name.Equals(code.Trim()));
        }
        public QuoteProvider GetQuoteProviderByCode(string code)
        {
            return QuoteProviders.FirstOrDefault(t => t.Code.Equals(code.Trim()))
                    ?? QuoteProviders.FirstOrDefault(t => t.Name.Equals(code.Trim()));
        }

        public void InsertBars(long seriesId, IEnumerable<IBarBase> bars)
        {
            var srs = TimeSeries.Find(seriesId);
            if (srs == null)
                throw new Exception(String.Format("TimeSeriesId: {0} is Not Found", seriesId));

            var bs = bars.Select(b => new Bar
            {
                DT = b.DT,
                BarSeriesID = srs.ID,
                Open = b.Open,
                High = b.High,
                Low = b.Low,
                Close = b.Close,
                Volume = b.Volume
            });

            InsertBars(bs);
        }
        private void InsertBars(IEnumerable<Bar> bars)
        {
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    this.BulkInsert(bars);
                    SaveChanges();
                    transactionScope.Complete();
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Join(" ",
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    e.Message));
            }
        }
        

    }
}
