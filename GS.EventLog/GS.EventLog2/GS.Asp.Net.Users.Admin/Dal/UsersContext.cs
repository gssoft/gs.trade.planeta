using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Asp.Net.Users.Admin.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GS.Asp.Net.Users.Admin.Dal
{
    public class UsersContext : IdentityDbContext<User>
    {
       // public System.Data.Entity.DbSet<GS.Asp.Net.Users.Admin.Models.Role> IdentityUserRoles { get; set; }
        // public System.Data.Entity.DbSet<GS.Asp.Net.Users.Admin.Models.Role> IdentityRoles { get; set; }
    }

    public class IdentityUsersContext : IdentityDbContext<IdentityUser>
    {
        public IdentityUsersContext() : base ()
        {
        }
    }

}