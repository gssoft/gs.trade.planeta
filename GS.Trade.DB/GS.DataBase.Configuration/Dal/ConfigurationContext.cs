using System.Data.Entity;


namespace GS.DataBase.Configuration.Dal
{
    using Model;
    public partial class ConfigurationContext : DbContext
    {
        
        public ConfigurationContext()
            : base(nameOrConnectionString: "AppDbConfig")
        {
            Database.CommandTimeout = 300;
        }

        public ConfigurationContext(string dbname) : base(nameOrConnectionString: dbname)
        {
            Database.CommandTimeout = 300;
        }

        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        
    }
}
