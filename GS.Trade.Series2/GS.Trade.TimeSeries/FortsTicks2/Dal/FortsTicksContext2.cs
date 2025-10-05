using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.TimeSeries.FortsTicks2.Model;
using GS.Trade.TimeSeries.General;

namespace GS.Trade.TimeSeries.FortsTicks2.Dal
{
    public partial class FortsTicksContext2 : DbContext
    {
        public FortsTicksContext2() : base("FortsTicks02")
        //public FortsTicksContext()
        //    : base(@"Data Source=.\MSSQL12;Initial Catalog=FortsTicks01;Integrated Security=True")
        {
            Database.CommandTimeout = 300;
        }
        public FortsTicksContext2(string connetionStringKey)
            : base(connetionStringKey)
        {
            Database.CommandTimeout = 300;
        }

        public DbSet<Ticker> Tickers { get; set; }


        // public DbSet<Model.TimeSeries> TimeSeries { get; set; }
        public DbSet<Model.TickSeries> TickSeries { get; set; }
        public DbSet<Model.BarSeries> BarSeries { get; set; }


        public DbSet<Tick> Ticks { get; set; }
        public DbSet<Bar> Bars { get; set; }

        public DbSet<Stat> Stats { get; set; }

        public DbSet<File> Files { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Tick>()
            //        .HasRequired<SrcFile>(s => s.SrcFile)
            //        .WithMany(s => s.Ticks)
            //        .HasForeignKey(s => s.SrcFileID);

            // modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Add(new DateConvention());
            
            //modelBuilder.Entity<Model.BarSeries>().ToTable("BarSeries");
            //modelBuilder.Entity<Model.TickSeries>().ToTable("TickSeries");

            //modelBuilder.Entity<Model.TimeSeries>()
            //.Map<Model.BarSeries>(m => m.Requires("TimeSeriesType").HasValue((short)2))
            //.Map<Model.TickSeries>(m => m.Requires("TimeSeriesType").HasValue((short)1));



            // Enum does not support --> lead to Exception
            //.Map<Model.BarSeries>(m => m.Requires("TimeSeriesType").HasValue(TimeSeriesTypeEnum.Bars))
            //.Map<Model.TickSeries>(m => m.Requires("TimeSeriesType").HasValue(TimeSeriesTypeEnum.Ticks));


            // Configure TimeSeriesId as PK for Stat
            //modelBuilder.Entity<Stat>()
            //.HasKey(e => e.TimeSeriesID);

            //// Configure TimeSeries as FK for Stat
            //modelBuilder.Entity<TimeSeries>()
            //            .HasOptional(s => s.Stat) // Mark Stat is optional for TimeSeries
            //            .WithRequired(ad => ad.TimeSeries); // Create inverse relationship
        }
    }
}
