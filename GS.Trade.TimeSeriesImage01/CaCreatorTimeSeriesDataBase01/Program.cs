using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Trade.TimeSeriesImage01.Models;
using TimeSeriesImageContext = GS.Trade.TimeSeriesImage01.Models.TimeSeriesImageContext;
using TimeSeriesContext02 = GS.Trade.TimeSeries02.Dal.TimeSeriesContext02;
namespace CaCreatorTimeSeriesDataBase01
{
    class Program
    {
        static void Main(string[] args)
        {
            TimeSeries02_Init();
            //TimeSeriesImage_Init();
            ConsoleSync.WriteReadLine("Press any key");
        }

        private static void TimeSeriesImage_Init()
        {
            var ini = new Initializer();
            Database.SetInitializer<TimeSeriesImageContext>(ini);

            var cntx = new TimeSeriesImageContext();
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
                Code = "Bars002",
                TickerId = t2.Id,
                TimeIntId = ti1.Id,
                QuoteProviderId = qpr1.Id
            };
            cntx.TimeSeries.Add(tms);

            tms = new BarSeries
            {
                Code = "Bars003",
                TickerId = t4.Id,
                TimeIntId = ti1.Id,
                QuoteProviderId = qpr2.Id
            };
            cntx.TimeSeries.Add(tms);
            tms = new BarSeries
            {
                Code = "Bars005",
                TickerId = t5.Id,
                TimeIntId = ti1.Id,
                QuoteProviderId = qpr2.Id
            };
            cntx.TimeSeries.Add(tms);
            tms = new BarSeries
            {
                Code = "Bars006",
                TickerId = t4.Id,
                TimeIntId = ti2.Id,
                QuoteProviderId = qpr2.Id
            };
            cntx.TimeSeries.Add(tms);
            tms = new BarSeries
            {
                Code = "Bars007",
                TickerId = t5.Id,
                TimeIntId = ti2.Id,
                QuoteProviderId = qpr2.Id
            };
            cntx.TimeSeries.Add(tms);

            cntx.SaveChanges();

            var tickseries = new TickSeries
            {
                Code = "Ticks003",
                TickerId = t1.Id,
                TimeIntId = ti1.Id,
                QuoteProviderId = qpr1.Id
            };
            cntx.TimeSeries.Add(tickseries);

            var tick = new Tick
            {
                DT = DateTime.Now,
                Last = 1.0
            };
            tickseries.Items.Add(tick);
            cntx.SaveChanges();

            var bytesseries = new BytesSeries
            {
                Code = "BytesSeries002",
                TickerId = t1.Id,
                TimeIntId = ti1.Id,
                QuoteProviderId = qpr1.Id
            };
            cntx.TimeSeries.Add(bytesseries);

            var bytesseriesitem = new BytesSeriesItem
            {
                DT = DateTime.Now,
                Format = "CSV",
                Count = 1000,
                CheckSum = Guid.NewGuid().ToString()
            };
            bytesseries.Items.Add(bytesseriesitem);

            cntx.SaveChanges();

            var barsseries = cntx.TimeSeries.OfType<BarSeries>().ToList();
            var ticksseries = cntx.TimeSeries.OfType<TickSeries>().ToList();
            var bytessseries = cntx.TimeSeries.OfType<BytesSeries>().ToList();

            Console.WriteLine($"BarSeries: {barsseries.Count}");
            Console.WriteLine($"TickSeries: {ticksseries.Count}");
            Console.WriteLine($"BytesSeries: {bytessseries.Count}");

            ConsoleSync.WriteReadLine("Press any key");
        }

        private static void TimeSeries02_Init()
        {
            try
            {
                var init = new GS.Trade.TimeSeries02.Dal.Initializer();
                Database.SetInitializer<TimeSeriesContext02>(init);

                var cntx = new TimeSeriesContext02();
                var t = new GS.Trade.TimeSeries02.Models.Ticker
                {
                    Code = "SiM0",
                    TradeBoard = "SPBFUT"
                };
                cntx.Tickers.Add(t);
                cntx.SaveChanges();
            }
            catch (Exception e)
            {
                ConsoleSync.WriteLineT(e.Message);
            }
        }
    }
}
