using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.ICharts;

namespace GS.Trade.Windows.Charts
{
    public class ChartBarSeries
    {
        public string Name { get; set; }

        public int ColorUp { get; set; }
        public int ColorDown { get; set; }
        public int ColorEdge { get; set; }

        private int _chartDataLength;

        public IChartBarSeries BarSeries;

        public DateTime[] TimeStamps { get; private set;}
        public double[] OpenData { get; private set; }
        public double[] HighData { get; private set; }
        public double[] LowData { get; private set; }
        public double[] CloseData { get; private set; }
        public double[] VolData { get; private set; }

        public void SetChartDataLength(int len)
        {
            _chartDataLength = len;

            TimeStamps = new DateTime[_chartDataLength];
            OpenData = new double[_chartDataLength];
            HighData = new double[_chartDataLength];
            LowData = new double[_chartDataLength];
            CloseData = new double[_chartDataLength];
            VolData = new double[_chartDataLength];
        }
        public void FillData()
        {
            int d;
            int length;

            if (BarSeries == null || BarSeries.Count < 1) return;
            if (TimeStamps == null) return;

            var l = TimeStamps.Length - 1;

            length = Math.Min(BarSeries.Count, TimeStamps.Length);
            for (var i = 0; i < length; i++)
            {
                TimeStamps[l - i] = BarSeries.GetDateTime(i);
                OpenData[l - i] = BarSeries.GetOpen(i);
                HighData[l - i] = BarSeries.GetHigh(i);
                LowData[l - i] = BarSeries.GetLow(i);
                CloseData[l - i] = BarSeries.GetClose(i);
                VolData[l - i] = BarSeries.GetVolume(i);
            }

            d = TimeStamps.Length - BarSeries.Count;

            if (d <= 0) return;

            for (var i = 0; i < d; i++)
            {
                TimeStamps[i] = DateTime.MinValue;
                OpenData[i] = ChartDirector.Chart.NoValue;
                HighData[i] = ChartDirector.Chart.NoValue;
                LowData[i] = ChartDirector.Chart.NoValue;
                CloseData[i] = ChartDirector.Chart.NoValue;
                VolData[i] = ChartDirector.Chart.NoValue;
            }
        }
    }
}
