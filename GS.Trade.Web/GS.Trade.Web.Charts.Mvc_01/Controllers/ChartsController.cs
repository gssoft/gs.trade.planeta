using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChartDirector;

namespace GS.Trade.Web.Charts.Mvc_01.Controllers
{
    public class ChartsController : Controller
    {
        // GET: Charts
        public ActionResult Index()
        {
            return View();
        }

        public FileContentResult ChartImage()
        {
            // The data for the bar chart
            double[] data = { 85, 156, 179.5, 211, 123 };

            // The labels for the bar chart
            string[] labels = { "Mon", "Tue", "Wed", "Thu", "Fri" };

            // Create a XYChart object of size 250 x 250 pixels
            XYChart c = new XYChart(250, 250);

            // Set the plotarea at (30, 20) and of size 200 x 200 pixels
            c.setPlotArea(30, 20, 200, 200);

            // Add a bar chart layer using the given data
            c.addBarLayer(data);

            // Set the labels on the x axis.
            c.xAxis().setLabels(labels);

            // Output the chart
            //WebChartViewer1.Image = c.makeWebImage(Chart.PNG);
            var img = c.makeChart2(ChartDirector.Chart.PNG);
            //var img = c.makeWebImage(ChartDirector.Chart.PNG);
            // Include tool tip for the chart
            //WebChartViewer1.ImageMap = c.getHTMLImageMap("", "",
            //    "title='{xLabel}: US${value}K'");
            FileContentResult byteStream = new FileContentResult(img, "image/png");
            return byteStream;
        }

        // GET: Charts/Details/5
        public ActionResult Details(int id)
        {
            ViewBag.Title = "Details";
            return View();
        }

        // GET: Charts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Charts/Create
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

        // GET: Charts/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Charts/Edit/5
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

        // GET: Charts/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Charts/Delete/5
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
