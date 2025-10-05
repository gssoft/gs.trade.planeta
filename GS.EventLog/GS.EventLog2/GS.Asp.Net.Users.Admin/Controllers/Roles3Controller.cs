using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GS.Asp.Net.Users.Admin.Controllers
{
    public class Roles3Controller : Controller
    {
        // GET: Roles3
        public ActionResult Index()
        {
            return View();
        }

        // GET: Roles3/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Roles3/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Roles3/Create
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

        // GET: Roles3/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Roles3/Edit/5
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

        // GET: Roles3/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Roles3/Delete/5
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
    }
}
