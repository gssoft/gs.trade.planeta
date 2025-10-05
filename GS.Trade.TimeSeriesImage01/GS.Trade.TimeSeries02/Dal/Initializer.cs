using System;
using System.Data.Entity;
using System.Linq;
using GS.ConsoleAS;
using GS.Trade.TimeSeries02.Models;

namespace GS.Trade.TimeSeries02.Dal
{
    //public class Initializer : DropCreateDatabaseIfModelChanges<TimeSeriesContext02>
    public class Initializer : DropCreateDatabaseAlways<TimeSeriesContext02>
    {
        protected override void Seed(TimeSeriesContext02 cntx)
        {
            try
            {
                var ti1 = new Ticker { Code = "RIZ3", TradeBoard = "SPBFUT" };
                cntx.Tickers.Add(ti1);
                var ti2 = new Ticker { Code = "RIZ8", TradeBoard = "SPBFUT" };
                cntx.Tickers.Add(ti2);
                cntx.SaveChanges();

                var ti = new TimeInt { Code = "1Min", TimeInterval = 60, TimeShift = 0 };
                cntx.TimeInts.Add(ti);
                cntx.SaveChanges();

                var qp = new QuoteProvider {Code = "Quik"};

                var t1 = new BarSeries
                {
                    Code = "Bars002",
                    Ticker = ti2,
                    TimeInt = ti,
                    QuoteProvider = qp
                };
                cntx.TimeSeries.Add(t1);
                cntx.SaveChanges();

                var t2 = new TickSeries
                {
                    Code = "Tick001",
                    Ticker = ti2,
                    TimeInt = ti,
                    QuoteProvider = qp
                };
                cntx.TimeSeries.Add(t2);
                cntx.SaveChanges();

                var t3 = new BytesBarTimeSeries
                {
                    Code = "BytesBar001",
                    Ticker = ti2,
                    TimeInt = ti,
                    QuoteProvider = qp
                };
                cntx.TimeSeries.Add(t3);
                cntx.SaveChanges();

                var t4 = new BytesTickTimeSeries()
                {
                    Code = "BytesTick001",
                    Ticker = ti2,
                    TimeInt = ti,
                    QuoteProvider = qp
                };
                cntx.TimeSeries.Add(t4);
                cntx.SaveChanges();

                var t5 = new BytesOrderLogTimeSeries()
                {
                    Code = "BytesOrderLog001",
                    Ticker = ti2,
                    TimeInt = ti,
                    QuoteProvider = qp
                };
                cntx.TimeSeries.Add(t5);
                cntx.SaveChanges();

                var t6 = new BytesTickerBarSeries()
                {
                    Code = "BytesTickerBar001",
                    Ticker = ti2,
                    TimeInt = ti,
                    QuoteProvider = qp
                };
                cntx.TimeSeries.Add(t6);
                cntx.SaveChanges();

                var t7 = new BytesTickerTickSeries()
                {
                    Code = "BytesTickerBar001",
                    Ticker = ti2,
                    TimeInt = ti,
                    QuoteProvider = qp
                };
                cntx.TimeSeries.Add(t7);
                cntx.SaveChanges();

                var t8 = new BytesTickerOrderLogSeries()
                {
                    Code = "BytesTickerBar001",
                    Ticker = ti2,
                    TimeInt = ti,
                    QuoteProvider = qp
                };
                cntx.TimeSeries.Add(t8);

                //var bar = new Bar { DT = DateTime.Now, Open = 1, High = 2, Low=3, Close = 4};
                //t1.Items.Add(bar);

                cntx.SaveChanges();

                var bars = cntx.Bars.ToArray();
                foreach (var b in bars)
                {
                    Console.WriteLine(b);
                }
                var s = cntx.TimeSeries.OfType<TickSeries>().Count();
                ConsoleSync.WriteLineT($"TickSeries.Count: {s}");
                
                //var tick = new TickSeries
                //{
                //    Code = "Ticks001",
                //    Ticker = t,
                //    TimeIntId = ti.Id,
                //   // QuoteProvider = "Quik"
                //};
                //cntx.TimeSeries.Add(tick);

                //var bars = new BarSeries
                //{
                //    Code = "Bars001",
                //    Ticker= t,
                //    TimeIntId = ti.Id,
                //   // QuoteProvider = "Quik"
                //};
                //cntx.TimeSeries.Add(bars);

                //var ticks = new TickSeries
                //{
                //    Code = "Ticks001",
                //    Ticker= t,
                //    TimeIntId = ti.Id,
                //   // QuoteProvider = "Quik"
                //};
                //cntx.TimeSeries.Add(ticks);

                //var bytes = new DailyBytesSeriesItem();
                //{

                //    Ticker = t,
                //    TimeIntId = ti.Id,
                //    QuoteProviderId = qpr.Id
                //};
                //cntx.TimeSeries.Add(bytes);

                cntx.SaveChanges();
            }
            catch (Exception e)
            {
                ConsoleSync.WriteLineT(e.Message);
            }
        }
    }
}
