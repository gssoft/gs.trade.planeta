using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.ICharts;

namespace GS.Trade.Data.Chart
{
    public class ChartDataContainer : IChartDataContainer
    {
        public List<IChartData> Charts { get; set; }
        public ChartDataContainer()
        {
            Charts = new List<IChartData>();
        }
        public void Add(IChartData ch)
        {
            Charts.Add(ch);
            CalcRealHeight();
        }
        private void CalcRealHeight()
        {
            var s = Charts.Sum(ch => ch.HeightExp);
            if (s <= 0)
                throw new NullReferenceException("HeightReal == 0");
            foreach(var ch in Charts)
                ch.HeightReal = (float)ch.HeightExp / s;
        }
        public void ClearAll()
        {
            foreach(var c in Charts)
                c.ClearAll();

            Charts.Clear();
        }
    }

    public class ChartData : IChartData
    {
        public string Name { get; set; }
        public int HeightExp { get; set; }
        public float HeightReal { get; set; }

        public List<IChartBarSeries> ChartBars { get; set; }
        public List<ILineSeries> ChartLines { get; set; }
        public List<IBandSeries> ChartBands { get; set; }
        public List<IList<ILevel>> ChartLevels { get; set; }
        public List<Func<IEnumerable<ILevel>>> ChartLevels2 { get; set; }
        public List<IList<IChartText>> ChartTexts { get; set; }
        public List<Func<IEnumerable<ILineXY>>> ChartLineXYs { get; set; }

        public ChartData()
        {
            ChartBars = new List<IChartBarSeries>();
            ChartLines = new List<ILineSeries>();
            ChartBands = new List<IBandSeries>();
            ChartLevels = new List<IList<ILevel>>();
            ChartLevels2 = new List<Func<IEnumerable<ILevel>>>();
            ChartTexts = new List<IList<IChartText>>();
            ChartLineXYs = new List<Func<IEnumerable<ILineXY>>>();
        }
        public void ClearAll()
        {
            ChartBars.Clear();
            ChartLines.Clear();
            ChartBands.Clear();
            ChartLevels.Clear();
            ChartLevels2.Clear();
            ChartTexts.Clear();
            ChartLineXYs.Clear();
        }
    }
}
