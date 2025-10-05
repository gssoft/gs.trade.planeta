using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GS.Asp.Net.Users.Admin.ViewModels
{
    public class UserRoles
    {
        public IdentityUser User { get; set; }
        public List<IdentityRole> Roles { get; set; }

        public UserRoles() 
        {
            Roles = new List<IdentityRole>();
        }
    }
   
}