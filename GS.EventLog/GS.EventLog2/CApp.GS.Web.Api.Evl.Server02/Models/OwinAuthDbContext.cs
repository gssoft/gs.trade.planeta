using Microsoft.AspNet.Identity.EntityFramework;

namespace CApp.GS.Web.Api.Evl.Server02.Models
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