using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.TimeSeries.Model;
using GS;

namespace Creator01
{
    class Program
    {
        static void Main(string[] args)
        {
            var ini = new Initializer();
            Database.SetInitializer<TimeSeriesContext>(ini);
            
            var cntx = new TimeSeriesContext();
            //var tb = new GS.Trade.DataBase.Series.Model.TradeBoard { Code = "EQBR" };
            //cntx.TradeBoards.Add(tb);
            //cntx.SaveChanges();
            var tb1 = new TradeBoard { Code = "EQBR" };
            cntx.TradeBoards.Add(tb1);
            var tb2 = new TradeBoard { Code = "SPBFUT" };
            cntx.TradeBoards.Add(tb2);
            var tb3 = new TradeBoard { Code = "UNIVERSE" };
            cntx.TradeBoards.Add(tb3);
            cntx.SaveChanges();

            var t1 = new Ticker { Code = "RIM5", TradeBoardId = tb2.Id };
            cntx.Tickers.Add(t1);
            var t2 = new Ticker { Code = "SRM5", TradeBoardId = tb2.Id };
            cntx.Tickers.Add(t2);
            var t3 = new Ticker { Code = "SiM5", TradeBoardId = tb2.Id };
            cntx.Tickers.Add(t3);
            var t4 = new Ticker { Code = "MARS", TradeBoardId = tb3.Id };
            cntx.Tickers.Add(t4);
            var t5 = new Ticker { Code = "URANUS", TradeBoardId = tb3.Id };
            cntx.Tickers.Add(t5);
            cntx.SaveChanges();

            var ti1 = new TimeInt { Code = "5Sec", TimeInterval = 5, TimeShift = 0 };
            cntx.TimeInts.Add(ti1);
            var ti2 = new TimeInt { Code = "15Sec", TimeInterval = 15, TimeShift = 0 };
            cntx.TimeInts.Add(ti2);
            cntx.SaveChanges();

            var qpr1 = new QuoteProvider { Code = "Quik.Real" };
            cntx.QuoteProviders.Add(qpr1);
            var qpr2 = new QuoteProvider { Code = "GS.Trade" };
            cntx.QuoteProviders.Add(qpr2);
            cntx.SaveChanges();

            var tms = new BarSeries
            {
                Code = "Bars001",
                TickerId = t1.Id,
                TimeIntId = ti1.Id,
                QuoteProviderId = qpr1.Id
            };
            cntx.TimeSeries.Add(tms);
            tms = new BarSeries
            {
                Code = "Bars001",
                TickerId = t2.Id,
                TimeIntId = ti1.Id,
                QuoteProviderId = qpr1.Id
            };
            cntx.TimeSeries.Add(tms);
            tms = new BarSeries
            {
                Code = "Bars001",
                TickerId = t3.Id,
                TimeIntId = ti1.Id,
                QuoteProviderId = qpr1.Id
            };
            cntx.SaveChanges();

            //Universe
            cntx.TimeSeries.Add(tms);
            tms = new BarSeries
            {
                Code = "Bars001",
                TickerId = t4.Id,
                TimeIntId = ti1.Id,
                QuoteProviderId = qpr2.Id
            };
            cntx.TimeSeries.Add(tms);
            tms = new BarSeries
            {
                Code = "Bars001",
                TickerId = t5.Id,
                TimeIntId = ti1.Id,
                QuoteProviderId = qpr2.Id
            };
            cntx.TimeSeries.Add(tms);
            tms = new BarSeries
            {
                Code = "Bars001",
                TickerId = t4.Id,
                TimeIntId = ti2.Id,
                QuoteProviderId = qpr2.Id
            };
            cntx.TimeSeries.Add(tms);
            tms = new BarSeries
            {
                Code = "Bars001",
                TickerId = t5.Id,
                TimeIntId = ti2.Id,
                QuoteProviderId = qpr2.Id
            };
            cntx.TimeSeries.Add(tms);

            //var tick = new TickSeries
            //{
            //    Code = "Ticks002",
            //    TickerId = t.Id,
            //    TimeIntId = ti.Id,
            //    QuoteProviderId = qpr.Id
            //};
            //cntx.TimeSeries.Add(tick);

            cntx.SaveChanges();
        }      
    }
}
