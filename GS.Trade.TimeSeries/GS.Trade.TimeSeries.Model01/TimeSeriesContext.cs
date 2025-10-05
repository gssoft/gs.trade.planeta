using System.Data.Entity;

namespace GS.Trade.TimeSeries.Model01
{
    public class TimeSeriesContext : DbContext
    {
        public TimeSeriesContext()
            : base("TimeSeries03")
        {
        }
        public TimeSeriesContext(string dbName)
            : base(dbName)
        {
        }
        //public DbSet<Model2.TradeBoard> TradeBoards { get; set; }

        //public DbSet<Model2.Ticker> Tickers { get; set; }
        //public DbSet<Model2.TimeInt> TimeInts { get; set; }
        //public DbSet<Model2.QuoteProvider> QuoteProviders { get; set; }

        public DbSet<TimeSeries> TimeSeries { get; set; }
        //public DbSet<Model2.BarSeries> BarSeries { get; set; }
        //public DbSet<Model2.TickSeries> TickSeries { get; set; }

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
