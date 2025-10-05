using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GS.DataBase.Configuration.Dal;
using GS.DataBase.Configuration.Model;

namespace GS.Trade.Web.Mvc.Cfg_02.Controllers
{
    [Authorize(Users = @"gs_trade@mail.ru")]
    //[Authorize]
    public class ConfigurationsController : Controller
    {
        private readonly ConfigurationContext db = new ConfigurationContext();

        //[Authorize]
        // GET: Configurations
        public ActionResult Index()
        {
            return View(db.Configurations.ToList());
        }

        public ActionResult ToEnable(long? cnfId)
        {
            db.UpdateCnfEnables(cnfId, EnDisEnum.Enabled);

            return View("Index", db.Configurations.ToList());
        }
        public ActionResult ToDisable(long? cnfId)
        {
            db.UpdateCnfEnables(cnfId, EnDisEnum.Disabled);

            return View("Index", db.Configurations.ToList());
        }
        public ActionResult UpdateToken(long? cnfId)
        {
            db.UpdateToken(cnfId);

            return View("Index", db.Configurations.ToList());
        }
        public ActionResult UpdateExpireDate(long? cnfId)
        {
            db.UpdateExpireDate(cnfId);

            return View("Index", db.Configurations.ToList());
        }

        // GET: Configurations/Details/5
        public ActionResult Details(long? id)
        {
            ViewBag.ConfigurationId = id;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Configuration configuration = db.Configurations.Find(id);
            if (configuration == null)
            {
                return HttpNotFound();
            }
            return View(configuration);
        }
       // [Authorize]
        // GET: Configurations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Configurations/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Enabled,Code,Name,Description,CreatedDT,ModifiedDT")] Configuration configuration)
        public ActionResult Create([Bind(Include = "Id,Enabled,Code,Catalog")] Configuration configuration)
        {
            if (ModelState.IsValid)
            {
                //db.Configurations.Add(configuration);
                //db.SaveChanges();
                db.Add(configuration);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(configuration);
        }
       // [Authorize]
        // GET: Configurations/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Configuration configuration = db.Configurations.Find(id);
            if (configuration == null)
            {
                return HttpNotFound();
            }
            return View(configuration);
        }

        // POST: Configurations/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Enabled,Code,Name,Description,CreatedDT,ModifiedDT")] Configuration configuration)
        public ActionResult Edit([Bind(Include = "Id,Enabled,Code,Catalog,Token,IncDaysExpire")] Configuration configuration)
        {
            if (ModelState.IsValid)
            {
                db.Entry(configuration).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(configuration);
        }
       // [Authorize]
        // GET: Configurations/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Configuration configuration = db.Configurations.Find(id);
            if (configuration == null)
            {
                return HttpNotFound();
            }
            return View(configuration);
        }

        // POST: Configurations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Configuration configuration = db.Configurations.Find(id);
            db.Configurations.Remove(configuration);
            db.SaveChanges();
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
