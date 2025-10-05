using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.ICharts;

namespace GS.Trade.Data.Chart
{
    public class ChartLine : ILineSeries
    {
        public string Name { get; set; }

        private int _color;
        public int Color
        {
            get { return CallGetColor == null ? _color : CallGetColor();}
            set { _color = value; }
        }

        public Func<int> CallGetColor { get; set; }
        public Func<int> CallGetCount { get; set; }
        public Func<int, DateTime> CallGetDateTime { get; set; }
        public Func<int, double> CallGetLine { get; set; }

        public int Count
        {
            get { return CallGetCount(); }
        }
        public DateTime GetDateTime(int index)
        {
            return CallGetDateTime(index);
        }
        public double GetLine(int index)
        {
            return CallGetLine(index);
        }
    }
}
