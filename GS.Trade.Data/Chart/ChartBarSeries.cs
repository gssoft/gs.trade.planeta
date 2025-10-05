using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.ICharts;

namespace GS.Trade.Data.Chart
{
    public class ChartBarSeries : IChartBarSeries
    {
        public string Name { get; set; }
        
        public int ColorUp { get; set; }
        public int ColorDown { get; set; }
        public int ColorEdge { get; set; }

        public Func<int> CallGetCount { get; set; }
        public Func<int, DateTime> CallGetDateTime { get; set; }
        public Func<int, double> CallGetLine { get; set; }

        public Func<int, double> CallGetOpen { get; set; }
        public Func<int, double> CallGetHigh { get; set; }
        public Func<int, double> CallGetLow { get; set; }
        public Func<int, double> CallGetClose { get; set; }
        public Func<int, double> CallGetVolume { get; set; }

        public int Count { get { return CallGetCount(); } }
        public DateTime GetDateTime(int index)
        {
            return CallGetDateTime(index);
        }
        public double GetOpen(int index)
        {
            return CallGetOpen(index);
        }
        public double GetHigh(int index)
        {
            return CallGetHigh(index);
        }
        public double GetLow(int index)
        {
            return CallGetLow(index);
        }
        public double GetClose(int index)
        {
            return CallGetClose(index);
        }
        public double GetVolume(int index)
        {
            return CallGetVolume(index);
        }

        public double GetLine(int index)
        {
            return CallGetClose(index);
        }
    }
}
