using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.TimeSeriesIm01.Models
{
    public partial class TimeSeriesContext : DbContext
    {
        public TimeSeriesContext()
            : base("TimeSeries01")
        {
            Database.CommandTimeout = 900;
        }
        public TimeSeriesContext(string dataBase)
            : base(dataBase)
        {
            Database.CommandTimeout = 900;
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

        //public System.Data.Entity.DbSet<GS.Trade.Web.Mvc.TimeSeries01.Models.Views.TimeSeriesStat> TimeSeriesStats { get; set; }
    }
}
