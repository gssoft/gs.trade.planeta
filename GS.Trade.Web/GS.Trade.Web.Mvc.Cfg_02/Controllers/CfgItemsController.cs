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
using Microsoft.Ajax.Utilities;

namespace GS.Trade.Web.Mvc.Cfg_02.Controllers
{
    [Authorize(Users = @"gs_trade@mail.ru")]
    public class CfgItemsController : Controller
    {
        private ConfigurationContext db = new ConfigurationContext();

        // GET: CfgItems
        public ActionResult Index(long? cnfId)
        {
            ViewBag.ConfigurationId = cnfId;

            var items = db.GetItems(cnfId);
            
            return View(items.ToList());
        }
        public ActionResult ToEnable(long? cnfId, long? itemId)
        {
            ViewBag.ConfigurationId = cnfId;
            ViewBag.CnfItemId = itemId;

            db.UpdateCnfItemEnables(cnfId, itemId, EnDisEnum.Enabled);
            var items = db.GetItems(cnfId);
            
            return View("Index", items.ToList());
        }
        public ActionResult ToDisable(long? cnfId, long? itemId)
        {
            ViewBag.ConfigurationId = cnfId;
            ViewBag.CnfItemId = itemId;

            db.UpdateCnfItemEnables(cnfId, itemId, EnDisEnum.Disabled);
            var items = db.GetItems(cnfId);

            return View("Index", items.ToList());
        }

        // GET: CfgItems/Details/5
        public ActionResult Details(long? cnfId, long? itemId)
        {
            ViewBag.CnfId = cnfId;

            ViewBag.ConfigurationId = cnfId;
            ViewBag.CnfItemId = itemId;

            if (itemId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(itemId);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: CfgItems/Create
        public ActionResult Create()
        {
            ViewBag.ConfigurationId = new SelectList(db.Configurations, "Id", "Code");
            return View();
        }

        // POST: CfgItems/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ConfigurationId,Enabled,TrCount,Code,Catalog,Obj")] Item item)
        {
            if (ModelState.IsValid)
            {
                //db.Items.Add(item);
                //db.SaveChanges();
                //var conf = item.Configuration;
                var conf = db.Configurations.Find(item.ConfigurationId);
                if (conf == null) 
                    return RedirectToAction("Index");
                var items = db.Items.Where(i => i.ConfigurationId == item.ConfigurationId);
                var it = items.FirstOrDefault(i => i.Code == item.Code);
                if (it != null)
                    return RedirectToAction("Index");
                //conf.Add(item);
                db.Items.Add(item);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ConfigurationId = new SelectList(db.Configurations, "Id", "Code", item.ConfigurationId);
            return View(item);
        }

        // GET: CfgItems/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.ConfigurationId = new SelectList(db.Configurations, "Id", "Code", item.ConfigurationId);
            return View(item);
        }

        // POST: CfgItems/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ConfigurationId,Enabled,TrCount,Code,Catalog,Obj")] Item item)
        {
            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ConfigurationId = new SelectList(db.Configurations, "Id", "Code", item.ConfigurationId);
            return View(item);
        }

        // GET: CfgItems/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: CfgItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Item item = db.Items.Find(id);
            db.Items.Remove(item);
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
