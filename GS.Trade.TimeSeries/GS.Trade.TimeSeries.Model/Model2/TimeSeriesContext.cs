using System.Data.Entity;

namespace GS.Trade.TimeSeries.Model.Model2
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

        public DbSet<Model2.TimeSeries> TimeSeries { get; set; }
        //public DbSet<Model2.BarSeries> BarSeries { get; set; }
        //public DbSet<Model2.TickSeries> TickSeries { get; set; }

        public DbSet<Model2.Bar> Bars { get; set; }
        public DbSet<Model2.Tick> Ticks { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Model2.TimeSeries>()
            .Map<Model2.BarSeries>(m => m.Requires("TimeSeriesType").HasValue(1))
            .Map<Model2.TickSeries>(m => m.Requires("TimeSeriesType").HasValue(2));
        }
    }
}
