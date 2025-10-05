using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.DataBase.Model;

namespace GS.Trade.DataBase.Dal
{
    public class DbTradeContext : DbContext
    {
        public DbTradeContext() : base("DbTrade")
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Ticker> Tickers { get; set; }
        public DbSet<Strategy> Strategies { get; set; }

        public DbSet<Model.Trade> Trades { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Deal> Deals { get; set; }
    }
}
