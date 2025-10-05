using Microsoft.AspNet.Identity.EntityFramework;

namespace Ca_OwinSelfHost_Auth_01.Models
{
    public class OwinAuthDbContext : IdentityDbContext
    {
        public OwinAuthDbContext()
            : base("OwinAuthDbContext")
        {
        }

        //public DbSet<Client> Clients { get; set; }
    }

}