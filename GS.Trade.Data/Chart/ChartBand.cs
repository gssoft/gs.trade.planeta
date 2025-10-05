using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.ICharts;

namespace GS.Trade.Data.Chart
{
    public class ChartBand : IBandSeries
    {
        public string BandName { get; set; }
        public Int32 BandColor { get; set; }
        public Int32 BandLineColor { get; set; }
        public Int32 BandFillColor { get; set; }

        public bool NeedDrawLine { get; set; }

        public Func<int> CallGetCount { get; set; }
        public Func<int, DateTime> CallGetDateTime { get; set; }
        public Func<int, double> CallGetLine { get; set; }
        public Func<int, double> CallGetHigh { get; set; }
        public Func<int, double> CallGetLow { get; set; }

        public int Count
        {
            get { return CallGetCount(); }
        }
        public int TimeIntSeconds { get { return 0; } }

        public DateTime GetDateTime(int index)
        {
            return CallGetDateTime(index);
        }
        public double GetLine(int index)
        {
            return CallGetLine(index);
        }
        public double GetHigh(int index)
        {
            return CallGetHigh(index);
        }
        public double GetLow(int index)
        {
            return CallGetLow(index);
        }
    }
}
