using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace OwinAuthentication.Models
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