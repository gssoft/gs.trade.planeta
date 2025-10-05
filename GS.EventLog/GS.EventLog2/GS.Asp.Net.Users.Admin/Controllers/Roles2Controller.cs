using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GS.Asp.Net.Users.Admin.Dal;
using GS.Asp.Net.Users.Admin.Models;
using GS.Asp.Net.Users.Admin.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GS.Asp.Net.Users.Admin.Controllers
{
    public class Roles2Controller : Controller
    {
        private UsersContext db = new UsersContext();
        private RoleStore<IdentityRole> _roleStore = new RoleStore<IdentityRole>(); 
        //private RoleManager<IdentityRole> _roleManager = new RoleManager<IdentityRole>(db); 

        // GET: Users
        public ActionResult Index()
        {
            var rs = db.Roles.
                Select(r=>new ViewRole{ID = r.Id, Name=r.Name})
                .ToList();
            return View(rs);
        }

        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
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
        public async Task<ActionResult>  Create([Bind(Include = "Id,Name")] ViewRole role)
        {
            if (ModelState.IsValid)
            {
                //db.Users.Add(user);
                //db.SaveChanges();
                //_roleManager.
                var ir = new IdentityRole(role.Name);
                await _roleStore.CreateAsync(ir);
                return RedirectToAction("Index");
            }

            return View(role);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var r = db.Roles.Find(id);
            if (r == null)
            {
                return HttpNotFound();
            }
            return View(r);
        }

        // POST: Users/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] User user)
        {
            // _roleStore.
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
            var r = db.Roles.Find(id);
            if (r == null)
            {
                return HttpNotFound();
            }
            //return View(new ViewRole{ID = r.Id, Name = r.Name});
            return View(r);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            
            var r = db.Roles.Find(id);
            await _roleStore.DeleteAsync(r);
            //db.Roles.Remove(r);
            //db.SaveChanges();
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
