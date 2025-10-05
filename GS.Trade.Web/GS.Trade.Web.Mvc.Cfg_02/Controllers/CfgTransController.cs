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
    public class CfgTransController : Controller
    {
        private ConfigurationContext db = new ConfigurationContext();

        // GET: CfgTrans
        public ActionResult Index(long? cnfItemId)
        {
            IQueryable<Transaction> ts;
            if (cnfItemId == null)
                ts = db.Transactions
                    .Include(t => t.Item)
                    .OrderByDescending(t=>t.DT);
            else
                ts = db.Transactions
                        .Where(t => t.ItemId == cnfItemId)
                        .Include(t => t.Item)
                        .OrderByDescending(t => t.DT);
            
            return View(ts.ToList());
        }
        public ActionResult Index2(long? cnfId)
        {
            IQueryable<Transaction> ts;
            if (cnfId == null)
                ts = db.Transactions
                    .Include(t => t.Item)
                    .OrderByDescending(t => t.DT);
            else
            
                ts = db.Transactions
                        .Include(t => t.Item)
                        .Where(t => t.Item.ConfigurationId == cnfId)
                        .OrderByDescending(t => t.DT);
            
            return View("Index",ts.ToList());
        }

        // GET: CfgTrans/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: CfgTrans/Create
        public ActionResult Create()
        {
            ViewBag.ItemId = new SelectList(db.Items, "Id", "Code");
            return View();
        }

        // POST: CfgTrans/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ItemId,Result,Source,Operation,Object,DT")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ItemId = new SelectList(db.Items, "Id", "Code", transaction.ItemId);
            return View(transaction);
        }

        // GET: CfgTrans/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemId = new SelectList(db.Items, "Id", "Code", transaction.ItemId);
            return View(transaction);
        }

        // POST: CfgTrans/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ItemId,Result,Source,Operation,Object,DT")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ItemId = new SelectList(db.Items, "Id", "Code", transaction.ItemId);
            return View(transaction);
        }

        // GET: CfgTrans/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: CfgTrans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DeleteAll()
        {
            var ts = db.Transactions.ToList();

            db.Transactions.RemoveRange(ts);
            db.SaveChanges();

            return View("Index", db.Transactions.ToList());
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
