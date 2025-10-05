using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.DataBase.Series.Dal;
using GS.Trade.DataBase.Series.Model;

namespace Creator01
{
    class Program
    {
        static void Main(string[] args)
        {
            var ini = new Initializer();
            Database.SetInitializer<SeriesContext>(ini);
            
            var cntx = new SeriesContext();
            //var tb = new GS.Trade.DataBase.Series.Model.TradeBoard { Code = "EQBR" };
            //cntx.TradeBoards.Add(tb);
            //cntx.SaveChanges();
            var tb = new TradeBoard { Code = "EQBR" };
            cntx.TradeBoards.Add(tb);

            var t = new Ticker { Code = "RIM3", TradeBoardId = tb.Id };
            cntx.Tickers.Add(t);

            var ti = new TimeInt { Code = "5Min", TimeInterval = 300, TimeShift = 0 };
            cntx.TimeInts.Add(ti);

            var qpr = new QuoteProvider { Code = "Quik.Train" };
            cntx.QuoteProviders.Add(qpr);

            var tms = new BarSeries
            {
                Code = "Bars002",
                TickerId = t.Id,
                TimeIntId = ti.Id,
                QuoteProviderId = qpr.Id
            };
            cntx.TimeSeries.Add(tms);

            var tick = new TickSeries
            {
                Code = "Ticks002",
                TickerId = t.Id,
                TimeIntId = ti.Id,
                QuoteProviderId = qpr.Id
            };
            cntx.TimeSeries.Add(tick);

            cntx.SaveChanges();

         
        }
    }
}
