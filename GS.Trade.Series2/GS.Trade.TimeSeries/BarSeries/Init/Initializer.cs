using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.TimeSeries.BarSeries.Dal;
using GS.Trade.TimeSeries.BarSeries.Models;

namespace GS.Trade.TimeSeries.BarSeries.Init
{
    //public class Initializer : DropCreateDatabaseAlways<BarSeriesContext01>
    public class Initializer : DropCreateDatabaseIfModelChanges<BarSeriesContext01>
    {
        protected override void Seed(BarSeriesContext01 context)
        {
            Console.WriteLine("BarSereisContext01 Initializer Seed()");
            //try
            //{
            //    var ticker = new Ticker
            //    {
            //        Code = "GS",
            //        Contract = "GS.Trade"
            //    };
            //    //context.Tickers.Add(ticker);

            //    var ti = new Tick
            //    {
            //        TradeID = 1,
            //        Contract = "GS-3.16",
            //        DateTime = DateTime.Now,
            //        Amount = 5,
            //        Price = 77777.77777
            //    };
            //    ticker.Ticks.Add(ti);

            //    context.Tickers.Add(ticker);
            //    var saveChangesAsync = context.SaveChanges();

            //    //.AsNoTracking(); !!!!!!!!!!
            //    foreach (var t in context.Ticks)
            //        context.Ticks.Remove(t);

            //    foreach (var t in context.Tickers)
            //        context.Tickers.Remove(t);

            //    context.SaveChanges();

            //    var tcnt = context.Ticks.Count();
            //}
            //catch (Exception e)
            //{
                
            //    throw;
            //}
            
        }
    }
}
