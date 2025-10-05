using System.Data.Entity;
using GS.Trade.TimeSeries02;
using GS.Trade.TimeSeries02.Models;

namespace GS.Trade.TimeSeries02.Dal
{
    public partial class TimeSeriesContext02 : DbContext
    {
        public TimeSeriesContext02()
            : base("TimeSeries02")
        {
            Database.CommandTimeout = 900;
        }
        public TimeSeriesContext02(string dataBase)
            : base(dataBase)
        {
            Database.CommandTimeout = 900;
        }
        
        public DbSet<Ticker> Tickers { get; set; }
        public DbSet<TimeInt> TimeInts { get; set; }

        public DbSet<TimeSeries> TimeSeries { get; set; }
        //public DbSet<BarSeries> BarSeries { get; set; }
        //public DbSet<TickSeries> TickSeries { get; set; }
        //public DbSet<BytesSeries> ByteSeries { get; set; }

        public DbSet<Bar> Bars { get; set; }
        public DbSet<Tick> Ticks { get; set; }
        public DbSet<BytesBarTimeSeriesItem> BytesBarSeries { get; set; }
        public DbSet<BytesTickTimeSeriesItem> BytesTickSeries { get; set; }
        public DbSet<BytesOrderLogTimeSeriesItem> BytesOrderSeries { get; set; }
        public DbSet<BytesTickerBarSeriesItem> BytesTickerBarSeries { get; set; }
        public DbSet<BytesTickerTickSeriesItem> BytesTickerTickSeries { get; set; }
        public DbSet<BytesTickerOrderLogSeriesItem> BytesTickerOrderLogSeries { get; set; }
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<TimeSeries>()
        //    .Map<BarSeries>(m => m.Requires("TimeSeriesType").HasValue(1))
        //    .Map<TickSeries>(m => m.Requires("TimeSeriesType").HasValue(2))
        //    .Map<BytesSeries>(m => m.Requires("TimeSeriesType").HasValue(3));
        //}
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<TimeSeries>().ToTable("TimeSeries");

        //    //modelBuilder.Entity<TimeSeries>()
        //    //.ToTable("TimeSeries")
        //    //.HasDiscriminator<int>("TimeSeriesType")
        //    //.HasValue<BarSeries>(1)
        //    //.HasValue<BarSeries>(2)
        //    //.HasValue<BarSeries>(3)
        //    //;
        //}

        //public System.Data.Entity.DbSet<GS.Trade.Web.Mvc.TimeSeries01.Models.Views.TimeSeriesStat> TimeSeriesStats { get; set; }
    }
}
