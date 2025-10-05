using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.TimeSeriesImage01.Models
{
    public partial class TimeSeriesImageContext : DbContext
    {
        public TimeSeriesImageContext()
            : base("TimeSeriesImage01")
        {
            Database.CommandTimeout = 900;
        }
        public TimeSeriesImageContext(string dataBase)
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
        //public DbSet<BytesSeries> ByteSeries { get; set; }

        public DbSet<Bar> Bars { get; set; }
        public DbSet<Tick> Ticks { get; set; }
        public DbSet<BytesSeriesItem> BytesSeries { get; set; }

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
