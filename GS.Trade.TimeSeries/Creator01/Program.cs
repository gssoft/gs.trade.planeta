using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GS.Trade.TimeSeries.Model01;

namespace Creator01
{
    class Program
    {
        static void Main(string[] args)
        {

            var ini = new Initializer();
            Database.SetInitializer<TimeSeriesContext>(ini);

            var cntx = new TimeSeriesContext("TimeSeries03");
            //cntx.SaveChanges();

            //var tb = new TradeBoard { Code = "EQBR" };
            //cntx.TradeBoards.Add(tb);

            //var t = new Ticker { Code = "RIM3", TradeBoardId = tb.Id };
            //cntx.Tickers.Add(t);

            //var ti = new TimeInt { Code = "5Min", TimeInterval = 300, TimeShift = 0 };
            //cntx.TimeInts.Add(ti);

            //var qpr = new QuoteProvider { Code = "Quik.Train" };
            //cntx.QuoteProviders.Add(qpr);

            var tms = new BarSeries
            {
                Code = "Bars002",
                TickerId = 1,
                Key = "SPBFUT@RIM4@Bars001"
                //TimeIntId = ti.Id,
                //QuoteProviderId = qpr.Id
            };
            cntx.TimeSeries.Add(tms);

            var tick = new TickSeries
            {
                Code = "Ticks001",
                TickerId = 1,
                Key = "SPBFUT@RIM4@Tick001"
                //TimeIntId = ti.Id,
                //QuoteProviderId = qpr.Id
            };
            cntx.TimeSeries.Add(tick);

            cntx.SaveChanges();
        }
    }
}
