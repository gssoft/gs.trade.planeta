using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.TimeSeries.BarSeries.Models;

namespace GS.Trade.TimeSeries.BarSeries.Dal
{
    using TimeSeries = Models.TimeSeries;
    using BarSeries = Models.BarSeries;
    using TickSeries = Models.TickSeries;

    public partial class BarSeriesContext01 : DbContext
    {
        public BarSeriesContext01()
            : base("BarSeries01")
        {
            Database.CommandTimeout = 300;
        }
        public BarSeriesContext01(string dataBase)
            : base(dataBase)
        {
            Database.CommandTimeout = 300;
        }
        public DbSet<TradeBoard> TradeBoards { get; set; }

        public DbSet<Ticker> Tickers { get; set; }
        public DbSet<TimeInt> TimeInts { get; set; }
        public DbSet<QuoteProvider> QuoteProviders { get; set; }

        public DbSet<Models.TimeSeries> TimeSeries { get; set; }
        //public DbSet<BarSeries> BarSeries { get; set; }
        //public DbSet<TickSeries> TickSeries { get; set; }

        public DbSet<Bar> Bars { get; set; }
        public DbSet<Tick> Ticks { get; set; }

        public DbSet<Stat> Stats { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TimeSeries>()
            .Map<BarSeries>(m => m.Requires("TimeSeriesType").HasValue(1))
            .Map<TickSeries>(m => m.Requires("TimeSeriesType").HasValue(2));

            // Configure TimeSeriesId as PK for Stat
            //modelBuilder.Entity<Stat>()
            //.HasKey(e => e.TimeSeriesID);

            //// Configure TimeSeries as FK for Stat
            //modelBuilder.Entity<TimeSeries>()
            //            .HasOptional(s => s.Stat) // Mark Stat is optional for TimeSeries
            //            .WithRequired(ad => ad.TimeSeries); // Create inverse relationship
        }

        //public System.Data.Entity.DbSet<GS.Trade.Web.Mvc.TimeSeries01.Models.Views.TimeSeriesStat> TimeSeriesStats { get; set; }
        
    }
}
