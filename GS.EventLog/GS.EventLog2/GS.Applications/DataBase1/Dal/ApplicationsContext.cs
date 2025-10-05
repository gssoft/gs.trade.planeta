using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Applications.DataBase1.Model;

namespace GS.Applications.DataBase1.Dal
{
    public partial class ApplicationsContext1 : DbContext
    {
        public ApplicationsContext1(): base("Applications1")
        {
            Database.CommandTimeout = 300;
        }

        public ApplicationsContext1(string dbName) : base(dbName)
        {
            Database.CommandTimeout = 300;
        }

        public static ApplicationsContext1 Create()
        {
            return new ApplicationsContext1();
        }
        public static ApplicationsContext1 Create(string dbname)
        {
            return new ApplicationsContext1(dbname);
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<User> Users { get; set; }

       

        /*
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // ManyToMany see AplicationContext
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
        */


    }
}
