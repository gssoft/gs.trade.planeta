using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.DataBase.Series.Model;

namespace GS.Trade.DataBase.Series.Dal
{
    public class Initializer : DropCreateDatabaseIfModelChanges <SeriesContext>
    {
        protected override void Seed(SeriesContext cntx)
        {
            var tb = new TradeBoard {Code = "SPBFUT"};
            cntx.TradeBoards.Add(tb);

            var t = new Ticker {Code = "RIZ3", TradeBoardId = tb.Id};
            cntx.Tickers.Add(t);

            var ti = new TimeInt { Code = "1Min", TimeInterval = 60, TimeShift = 0};
            cntx.TimeInts.Add(ti);

            var qpr = new QuoteProvider { Code = "Quik.Real" };
            cntx.QuoteProviders.Add(qpr);

            var tms = new BarSeries {  Code = "Bars001",
                                        TickerId  = t.Id, TimeIntId = ti.Id, QuoteProviderId = qpr.Id};
            cntx.TimeSeries.Add(tms);

            var tick = new TickSeries
            {
                Code = "Ticks001",
                TickerId = t.Id,
                TimeIntId = ti.Id,
                QuoteProviderId = qpr.Id
            };
            cntx.TimeSeries.Add(tick);

            cntx.SaveChanges();
        }
    }
}
