using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using GS.Asp.Net.Users.Admin.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GS.Asp.Net.Users.Admin.Areas.Admin.Controllers
{
   // [RouteArea("admin")]
    public class Users2Controller : Controller
    {
        // [Route("users")]
        // GET: Admin/Users
        public ActionResult Index()
        {
            var cntxt = new IdentityDbContext<IdentityUser>();
            var store = new UserStore<IdentityUser>();
            var users = store.Users.ToList();
            
            return View(users);
        }

        // GET: Admin/Users/Details/5
      
    }
}
