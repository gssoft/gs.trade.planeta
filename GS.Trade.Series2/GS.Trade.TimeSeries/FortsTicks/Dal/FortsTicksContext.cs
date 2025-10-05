using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.TimeSeries.FortsTicks.Model;

namespace GS.Trade.TimeSeries.FortsTicks.Dal
{
    public partial class FortsTicksContext : DbContext
    {
        public FortsTicksContext() : base("FortsTicks01")
        //public FortsTicksContext()
        //    : base(@"Data Source=.\MSSQL12;Initial Catalog=FortsTicks01;Integrated Security=True")
        {
            Database.CommandTimeout = 300;
        }
        public FortsTicksContext(string connetionStringKey)
            : base(connetionStringKey)
        {
            Database.CommandTimeout = 300;
        }

        public DbSet<Ticker> Tickers { get; set; }
        public DbSet<Tick> Ticks { get; set; }
        public DbSet<Bar> Bars { get; set; }

        public DbSet<Stat> Stats { get; set; }

        public DbSet<SrcFile> SrcFiles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Tick>()
            //        .HasRequired<SrcFile>(s => s.SrcFile)
            //        .WithMany(s => s.Ticks)
            //        .HasForeignKey(s => s.SrcFileID);
        }

    }
    


}
