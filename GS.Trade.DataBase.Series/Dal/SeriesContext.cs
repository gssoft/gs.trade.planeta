using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.DataBase.Series.Model;

namespace GS.Trade.DataBase.Series.Dal
{
    public class SeriesContext : DbContext
    {
        public SeriesContext() : base("Series01")
        {
        }
        public DbSet<TradeBoard> TradeBoards { get; set; }

        public DbSet<Ticker> Tickers { get; set; }
        public DbSet<TimeInt> TimeInts { get; set; }
        public DbSet<QuoteProvider> QuoteProviders { get; set; }

        public DbSet<TimeSeries> TimeSeries { get; set; }
        //public DbSet<BarSeries> BarSeries { get; set; }
        //public DbSet<TickSeries> TickSeries { get; set; }

        public DbSet<Bar> Bars { get; set; }
        public DbSet<Tick> Ticks { get; set; }

       
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TimeSeries>()
            .Map<BarSeries>(m => m.Requires("TimeSeriesType").HasValue(1))
            .Map<TickSeries>(m => m.Requires("TimeSeriesType").HasValue(2));
        }
    }
}
