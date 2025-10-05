using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using GS.Asp.Net.Users.Admin.Dal;
using GS.Asp.Net.Users.Admin.Data;
using GS.Asp.Net.Users.Admin.Models;
using GS.Asp.Net.Users.Admin.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using MvcCms.Data;

namespace GS.Asp.Net.Users.Admin.Controllers
{
    public class UsersController : Controller
    {
        private IdentityUsersContext db = new IdentityUsersContext();
        private readonly IUserRepository _users;
        private readonly IRoleRepository _roles;

        private readonly ApplicationUserManager _userManager;

        public UsersController() : this(new UserRepository(), new RoleRepository())
        {
            //_userManager = _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }
        public UsersController(IUserRepository users, IRoleRepository roles)
        {
            _users = users;
            _roles = roles;
        }

        // GET: Users
        public async Task<ViewResult> Index()
        {
            var users = await _users.GetAllUsersAsync();
            return View(users);
        }

        // GET: Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await _users.GetUserByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        public async Task<ActionResult> Roles(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await _users.GetUserByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var roles = await _users.GetRolesForUserAsync(user);
            var userroles = new UserRoles {User = user};
            //if (roles == null)
            //    return View(userroles);

            foreach (var r in roles)
            {
                var ir = await _roles.GetRoleByNameAsync(r);
                if(ir == null)
                    continue;
                userroles.Roles.Add(ir);
            }
            var rs = await _roles.GetAllRolesAsync();

            ViewBag.RolesSelectList = new SelectList(rs, "Id", "Name");
            ViewBag.Title = "Add Roles to User: " + user.UserName;
            return View(userroles);
        }

        public async Task<ActionResult> AddUserToRole(string userId, string roleName) // string RolesSelectList)
        {
           var u = await _users.GetUserByIdAsync(userId);
           if (u == null)
                return HttpNotFound();

           var r = await _roles.GetRoleByIdAsync(roleName);
           if (r == null)
               return HttpNotFound();

           await _users.AddUserToRoleAsync(u,r.Name);
           
           return RedirectToAction("Roles", new {id = userId});  
        }


        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Email, UserName")] IdentityUser user)
        {
            if (ModelState.IsValid)
            {
                await _users.CreateAsync(user, "aadadadad");
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var user = await _users.GetUserByIdAsync(id);
            await _users.DeleteAsync(user);
            return RedirectToAction("Index");
        }

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
