using System;
using GS.ICharts;

namespace GS.Trade.Windows.Charts
{
    public class LineSeries
    {
        private ILineSeries _lineSeries;

        private int _chartDataLength;

        public double[] LineChartData { get; private set; }

        public LineSeries(ILineSeries ls)
        {
            _lineSeries = ls;
        }
        public void SetLineSeries(ILineSeries ls)
        {
            _lineSeries = ls;
            if (_chartDataLength != 0)
                LineChartData = new double[_chartDataLength];
        }
        public void SetChartDataLength(int len)
        {
            _chartDataLength = len;
            LineChartData = new double[_chartDataLength];
        }
        public void FillData()
        {
            if (_lineSeries.Count == 0 || LineChartData == null) return;

            var l = LineChartData.Length - 1;
            var length = Math.Min(_lineSeries.Count, LineChartData.Length);
            for (var i=0; i < length; i++)
            {
                LineChartData[l - i] = _lineSeries.GetLine(i);
            }
            var d = LineChartData.Length - _lineSeries.Count;
            if (d <= 0) return;
            for (var i=0; i < d; i++)
                LineChartData[i] = ChartDirector.Chart.NoValue;
        }
        public int Color { get { return _lineSeries.Color; } }
        public string Name { get { return _lineSeries.Name; } }
    }

  
 }