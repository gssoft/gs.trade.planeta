using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GS.EventLog.DataBase.Dal;
using EvlContext = GS.EventLog.DataBase.Dal.EvlContext;

namespace WebEventLog_01.Controllers
{
    public class AdminWorksController : Controller
    {
        private readonly EvlContext _db = new EvlContext();
         
        // GET: AdminWorks
        public ActionResult Index()
        {
            ViewBag.Title = "Administration Works:";
            return View();
        }

        public ActionResult ClearDataBase()
        {

            _db.Database.ExecuteSqlCommand("exec dbo.SP_Clear_DB_Last2Days");
            // db.Database.ExecuteSqlCommandAsync("exec dbo.SP_Clear_DB_Last2Days");
            // db.SaveChanges();
            // ViewBag.Title = "Clear DataBase";
            // ViewBag.Message = "Clear DataBase";
            // Index();
            return RedirectToAction("Index");
        }

        // GET: AdminWorks/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminWorks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminWorks/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminWorks/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminWorks/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminWorks/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminWorks/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
