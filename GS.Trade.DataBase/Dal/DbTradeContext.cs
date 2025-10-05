using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.DataBase.Model;

namespace GS.Trade.DataBase.Dal
{
    public partial class DbTradeContext : DbContext
    {
        public DbTradeContext()
            : base(nameOrConnectionString: "DbTrade2")
        {
           
        }
        public DbTradeContext(string dbname) : base(nameOrConnectionString: dbname)
        {

        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Ticker> Tickers { get; set; }
        public DbSet<Strategy> Strategies { get; set; }

        public DbSet<Model.Trade> Trades { get; set; }
        public DbSet<Order> Orders { get; set; }

        public DbSet<Deal> Deals { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Total> Totals { get; set; }

        public DbSet<GSException> GSExceptions { get; set; }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Account>().Property(p => p.Balance).HasPrecision(28, 5);

            modelBuilder.Entity<Model.Trade>().Property(p => p.Number).HasPrecision(20, 0);
            modelBuilder.Entity<Model.Trade>().Property(p => p.OrderNumber).HasPrecision(20, 0);
            modelBuilder.Entity<Model.Trade>().Property(p => p.Price).HasPrecision(28, 8);

            modelBuilder.Entity<Model.Order>().Property(p => p.Number).HasPrecision(20, 0);


            modelBuilder.Entity<Deal>().Property(p => p.Number).HasPrecision(20, 0);
            modelBuilder.Entity<Deal>().Property(p => p.Price1).HasPrecision(28, 8);
            modelBuilder.Entity<Deal>().Property(p => p.Price2).HasPrecision(28, 8);
            modelBuilder.Entity<Deal>().Property(p => p.FirstTradeNumber).HasPrecision(20, 0);
            modelBuilder.Entity<Deal>().Property(p => p.LastTradeNumber).HasPrecision(20, 0);

            modelBuilder.Entity<Position>().Property(p => p.Price1).HasPrecision(28, 8);
            modelBuilder.Entity<Position>().Property(p => p.Price2).HasPrecision(28, 8);
            modelBuilder.Entity<Position>().Property(p => p.PnL).HasPrecision(28, 8);
            modelBuilder.Entity<Position>().Property(p => p.PnL3).HasPrecision(28, 8);

            modelBuilder.Entity<Position>().Property(p => p.FirstTradeNumber).HasPrecision(20, 0);
            modelBuilder.Entity<Position>().Property(p => p.LastTradeNumber).HasPrecision(20, 0);


            base.OnModelCreating(modelBuilder);
        }
    }
}
