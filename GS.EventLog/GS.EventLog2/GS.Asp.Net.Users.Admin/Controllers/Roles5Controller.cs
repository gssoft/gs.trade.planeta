using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GS.Asp.Net.Users.Admin.Dal;
using GS.Asp.Net.Users.Admin.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GS.Asp.Net.Users.Admin.Controllers
{
    public class Roles5Controller : Controller
    {
        private UsersContext db = new UsersContext();
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        // GET: Roles
        public ActionResult Index()
        {
           // _userManager.
            var r = new IdentityRole();
            var u = new IdentityUser();
            var rs = db.Roles.ToList(); // IdentityRoles.ToList();

            return View(rs);
        }

        // GET: Roles/Details/5
      

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
