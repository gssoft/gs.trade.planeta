using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.ICharts;

namespace GS.Trade.Data.Chart
{
    public class ChartLineXY : ILineXY
    {
        public double LineY1 { get; set; }
        public double LineY2 { get; set; }
        public DateTime LineX1 { get; set; }
        public DateTime LineX2 { get; set; }

        public int Color { get; set; }
        public Func<int> GetLineColor { get; set; }

        public int Width { get; set; }
        public Func<int> GetLineWidth { get; set; }

        public int LineColor
        {
            get { return GetLineColor == null ? Color : GetLineColor(); }
        }

        public int LineWidth
        {
            get { return GetLineWidth == null ? Width : GetLineWidth(); }
        }
    }
}
