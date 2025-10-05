using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using ChartDirector;

namespace GS.Trade.Web.Charts.Mvc_02.Controllers
{
    public class ChartsController : Controller
    {
        private readonly Random _rand =  new Random((int) DateTime.Now.Ticks);
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

        public ActionResult ChartImage1()
        {
            // The data for the bar chart
            // The x and y coordinates of the grid
            double[] dataX = {-10, -9, -8, -7, -6, -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6,
        7, 8, 9, 10};
            double[] dataY = {-10, -9, -8, -7, -6, -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6,
        7, 8, 9, 10};

            // The values at the grid points. In this example, we will compute the values
            // using the formula z = Sin(x * x / 128 - y * y / 256 + 3) * Cos(x / 4 + 1 -
            // Exp(y / 8))
            double[] dataZ = new double[(dataX.Length) * (dataY.Length)];
            for (int yIndex = 0; yIndex < dataY.Length; ++yIndex)
            {
                double y = dataY[yIndex];
                for (int xIndex = 0; xIndex < dataX.Length; ++xIndex)
                {
                    double x = dataX[xIndex];
                    dataZ[yIndex * (dataX.Length) + xIndex] = Math.Sin(x * x / 128.0 - y * y
                         / 256.0 + 3) * Math.Cos(x / 4.0 + 1 - Math.Exp(y / 8.0));
                }
            }

            // Create a SurfaceChart object of size 750 x 600 pixels
            SurfaceChart c = new SurfaceChart(750, 600);

            // Add a title to the chart using 20 points Times New Roman Italic font
            c.addTitle("Surface Energy Density       ", "Times New Roman Italic", 20);

            // Set the center of the plot region at (380, 260), and set width x depth x
            // height to 360 x 360 x 270 pixels
            c.setPlotRegion(380, 260, 360, 360, 270);

            // Set the elevation and rotation angles to 30 and 210 degrees
            c.setViewAngle(30, 210);

            // Set the perspective level to 60
            c.setPerspective(60);

            // Set the data to use to plot the chart
            c.setData(dataX, dataY, dataZ);

            // Spline interpolate data to a 80 x 80 grid for a smooth surface
            c.setInterpolation(80, 80);

            // Use semi-transparent black (c0000000) for x and y major surface grid lines.
            // Use dotted style for x and y minor surface grid lines.
            int majorGridColor = unchecked((int)0xc0000000);
            int minorGridColor = c.dashLineColor(majorGridColor, Chart.DotLine);
            c.setSurfaceAxisGrid(majorGridColor, majorGridColor, minorGridColor,
                minorGridColor);

            // Set contour lines to semi-transparent white (80ffffff)
            c.setContourColor(unchecked((int)0x80ffffff));

            // Add a color axis (the legend) in which the left center is anchored at (665,
            // 280). Set the length to 200 pixels and the labels on the right side.
            c.setColorAxis(665, 280, Chart.Left, 200, Chart.Right);

            // Set the x, y and z axis titles using 12 points Arial Bold font
            c.xAxis().setTitle("X Title\nPlaceholder", "Arial Bold", 12);
            c.yAxis().setTitle("Y Title\nPlaceholder", "Arial Bold", 12);
            c.zAxis().setTitle("Z Title Placeholder", "Arial Bold", 12);

            // Output the chart

            //WebChartViewer1.Image = c.makeWebImage(Chart.JPG);
            
            //var img = c.makeWebImage(ChartDirector.Chart.PNG);
            // Include tool tip for the chart
            //WebChartViewer1.ImageMap = c.getHTMLImageMap("", "",
            //    "title='{xLabel}: US${value}K'");
            /*
            var img = c.makeChart2(ChartDirector.Chart.PNG);
            FileContentResult byteStream = new FileContentResult(img, "image/png");
            return byteStream;
             */
            return Content(GetChartImage(c));
        }

        public ActionResult ChartImage2()
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
            /*
            var imgtag = "<img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap'>";
            var img = c.makeChart2(ChartDirector.Chart.PNG);
            string imgBytesAsStr = Convert.ToBase64String(img);
            string imgStr = string.Format(imgtag, imgBytesAsStr);
            */
            //var img = c.makeWebImage(ChartDirector.Chart.PNG);
            // Include tool tip for the chart
            //WebChartViewer1.ImageMap = c.getHTMLImageMap("", "",
            //    "title='{xLabel}: US${value}K'");
            //FileContentResult byteStream = new FileContentResult(img, "image/png");
            //return Content(imgStr);
            return Content(GetChartImage(c));
        }

        public ActionResult ChartImage3()
        {
            // The data for the bar chart
            var data = new double[5];

            foreach (var i in Enumerable.Range(0, 5))
            {
                data[i] = _rand.Next(50,300);
            }
            // double[] data = { 85, 156, 179.5, 211, 123 };

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
            /*
            var imgtag = "<img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap'>";
            var img = c.makeChart2(ChartDirector.Chart.PNG);
            string imgBytesAsStr = Convert.ToBase64String(img);
            string imgStr = string.Format(imgtag, imgBytesAsStr);
            */
            //var img = c.makeWebImage(ChartDirector.Chart.PNG);
            // Include tool tip for the chart
            //WebChartViewer1.ImageMap = c.getHTMLImageMap("", "",
            //    "title='{xLabel}: US${value}K'");
            //FileContentResult byteStream = new FileContentResult(img, "image/png");
            //return Content(imgStr);
            return Content(GetChartImage(c));
        }

        public ActionResult GetChartImage()
        {
            return View("GetChartImage");
        }

        public ActionResult GetChartImagePartial()
        {
            return View("GetChartImagePartial");
        }

        private static string GetChartImage(BaseChart c)
        {
            var imgtag = "<img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap'>";
            var img = c.makeChart2(ChartDirector.Chart.PNG);
            string imgBytesAsStr = Convert.ToBase64String(img);
            string imgStr = string.Format(imgtag, imgBytesAsStr);
            return imgStr;
        }

        // GET: Charts/Details/5
        public ActionResult Details(int id)
        {
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
