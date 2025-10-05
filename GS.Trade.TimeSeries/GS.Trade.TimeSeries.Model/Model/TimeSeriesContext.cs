using System.Data.Entity;

namespace GS.Trade.TimeSeries.Model.Model
{
    public class TimeSeriesContext : DbContext
    {
        public TimeSeriesContext()
            : base("TimeSeries03")
        {
        }
        public DbSet<TradeBoard> TradeBoards { get; set; }

        public DbSet<Ticker> Tickers { get; set; }
        public DbSet<TimeInt> TimeInts { get; set; }
        public DbSet<QuoteProvider> QuoteProviders { get; set; }

        public DbSet<Model.TimeSeries> TimeSeries { get; set; }
        //public DbSet<BarSeries> BarSeries { get; set; }
        //public DbSet<TickSeries> TickSeries { get; set; }

        public DbSet<Bar> Bars { get; set; }
        public DbSet<Tick> Ticks { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Model.TimeSeries>()
            .Map<BarSeries>(m => m.Requires("TimeSeriesType").HasValue(1))
            .Map<TickSeries>(m => m.Requires("TimeSeriesType").HasValue(2));
        }
    }
}
