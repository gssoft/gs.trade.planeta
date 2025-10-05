using System.Data.Entity;

namespace GS.Trade.TimeSeries.Model01
{
    //public class Initializer : DropCreateDatabaseIfModelChanges<TimeSeriesContext>
    public class Initializer : DropCreateDatabaseAlways <TimeSeriesContext>
    {
        protected override void Seed(TimeSeriesContext cntx)
        {
            //var tb = new TradeBoard { Code = "SPBFUT" };
            //cntx.TradeBoards.Add(tb);

            //var t = new Ticker { Code = "RIZ3", TradeBoardId = tb.Id };
            //cntx.Tickers.Add(t);

            //var ti = new TimeInt { Code = "1Min", TimeInterval = 60, TimeShift = 0 };
            //cntx.TimeInts.Add(ti);

            //var qpr = new QuoteProvider { Code = "Quik.Real" };
            //cntx.QuoteProviders.Add(qpr);

            var tms = new BarSeries
            {
                Code = "Bars001",
                TickerId = 1,
                Key = "SPBFUT@RIM4@Bars001",
                TimeInterval = 5
                //TimeIntId = ti.Id,
                //QuoteProviderId = qpr.Id
            };
            cntx.TimeSeries.Add(tms);

            var tick = new TickSeries
            {
                Code = "Ticks001",
                TickerId = 1,
                Key = "SPBFUT@RIM4@Tick001",
                TimeInterval = 0
                //TimeIntId = ti.Id,
                //QuoteProviderId = qpr.Id
            };
            cntx.TimeSeries.Add(tick);

            cntx.SaveChanges();
        }
    }
}
