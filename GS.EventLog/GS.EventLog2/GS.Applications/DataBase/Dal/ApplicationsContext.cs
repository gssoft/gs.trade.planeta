using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Applications.DataBase.Model;

namespace GS.Applications.DataBase.Dal
{
    // Many To Many Model
    public partial class ApplicationsContext : DbContext
    {
        public ApplicationsContext(): base("Applications")
        {
            Database.CommandTimeout = 300;
        }

        public ApplicationsContext(string dbName) : base(dbName)
        {
            Database.CommandTimeout = 300;
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
             modelBuilder.Entity<User>()
                .HasMany<Application>(s => s.Applications)
                .WithMany(c => c.Users)
                .Map(cs =>
                        {
                            cs.MapLeftKey("UserRefId");
                            cs.MapRightKey("AppRefId");
                            cs.ToTable("UserApp");
                        });

        }


    }
}
