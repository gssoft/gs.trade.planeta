using System;
using System.Collections;
using System.Collections.Generic;
using ChartDirector;
using GS.ICharts;


namespace GS.Trade.Windows.Charts
{
    public class ChartContainer
    {
        public List<Chart> Charts { get; set; }
        public ChartContainer()
        {
            Charts = new List<Chart>();
        }
        public void ClearAll()
        {
            foreach (var ch in Charts)
                ch.ClearAll();

            Charts.Clear();
        }
    }
    public class Chart
    {
        public string Name {get;set;}
        public float Height { get; set; }

        public XYChart ChartXY { get; set; }

        public List<ChartBarSeries> BarSeriesCollection { get; set; }
        public List<LineSeries> LineSeriesCollection { get; set; }
        public List<BandSeries> BandSeriesCollection  { get; set; }
        public List<IList<ILevel>> LevelsCollection { get; set; }
        public List<IList<IChartText>> TextsCollection { get; set; }
        public List<Func<IEnumerable<ILevel>>> LevelSourceCollection {get;set;}
        //public List<ILineXYCollection> LineXYSourceCollection { get; set; }
        public List<Func<IEnumerable<ILineXY>>> LineXYSourceCollection { get; set; }

        public Chart()
        {
            BarSeriesCollection = new List<ChartBarSeries>();
            LineSeriesCollection = new List<LineSeries>();
            BandSeriesCollection = new List<BandSeries>();
            LevelsCollection = new List<IList<ILevel>>();
            LevelSourceCollection = new List<Func<IEnumerable<ILevel>>>();
            TextsCollection = new List<IList<IChartText>>();
            //LineXYSourceCollection = new List<ILineXYCollection>();
            LineXYSourceCollection = new List<Func<IEnumerable<ILineXY>>>();
        }
        public void ClearAll()
        {
            BarSeriesCollection.Clear();
            LineSeriesCollection.Clear();
            BandSeriesCollection.Clear();
            LevelsCollection.Clear();
            LevelSourceCollection.Clear();
            TextsCollection.Clear();
            LineXYSourceCollection.Clear();
        }

    }
}
