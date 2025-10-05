using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.TimeSeriesImage01.Models
{
    public class Initializer : DropCreateDatabaseIfModelChanges<TimeSeriesImageContext>
    //public class Initializer : DropCreateDatabaseAlways<TimeSeriesContext>
    {
        protected override void Seed(TimeSeriesImageContext cntx)
        {
            var tb = new TradeBoard { Code = "SPBFUT" };
            cntx.TradeBoards.Add(tb);

            var t = new Ticker { Code = "RIZ3", TradeBoardId = tb.Id };
            cntx.Tickers.Add(t);

            var ti = new TimeInt { Code = "1Min", TimeInterval = 60, TimeShift = 0 };
            cntx.TimeInts.Add(ti);

            var qpr = new QuoteProvider { Code = "Quik.Real" };
            cntx.QuoteProviders.Add(qpr);

            var tms = new BarSeries
            {
                Code = "Bars001",
                TickerId = t.Id,
                TimeIntId = ti.Id,
                QuoteProviderId = qpr.Id
            };
            cntx.TimeSeries.Add(tms);

            var tick = new TickSeries
            {
                Code = "Ticks001",
                TickerId = t.Id,
                TimeIntId = ti.Id,
                QuoteProviderId = qpr.Id
            };
            cntx.TimeSeries.Add(tick);

            var bars = new BarSeries
            {
                Code = "Bars001",
                TickerId = t.Id,
                TimeIntId = ti.Id,
                QuoteProviderId = qpr.Id
            };
            cntx.TimeSeries.Add(bars);

            var ticks = new TickSeries
            {
                Code = "Ticks001",
                TickerId = t.Id,
                TimeIntId = ti.Id,
                QuoteProviderId = qpr.Id
            };
            cntx.TimeSeries.Add(ticks);

            var bytes = new BytesSeries
            {
                Code = "Bytes001",
                TickerId = t.Id,
                TimeIntId = ti.Id,
                QuoteProviderId = qpr.Id
            };
            cntx.TimeSeries.Add(bytes);

            cntx.SaveChanges();
        }
    }
}
