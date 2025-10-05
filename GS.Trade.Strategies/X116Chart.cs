using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GS.ICharts;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Chart;

namespace GS.Trade.Strategies
{
    public partial class X116
    {
        [XmlIgnore]
        private IChartDataContainer _chartDataContainer;
        [XmlIgnore]
        public override IChartDataContainer ChartDataContainer
        {
            //get { return _chartDataContainer ?? CreateChartDataContainer(); }
            get { return CreateChartDataContainer(); }
        }
        private IChartDataContainer CreateChartDataContainer()
        {
            var chdc = new ChartDataContainer();

            var chart = new ChartData { Name = "Main", HeightExp = 80 };

            //  if (Bars != null)
            chart.ChartBars.Add(_bars.ChartBarSeries);

            if (_xma018 != null && _xma018.ChartBands != null)
                chart.ChartBands.AddRange(_xma018.ChartBands);

            AddChartLevels();
            if (_xma018 != null && _xma018.ChartLevels != null)
                chart.ChartLevels.Add(_xma018.ChartLevels);

            if (_xma018 != null && _xma018.ChartTexts != null)
                chart.ChartTexts.Add(_xma018.ChartTexts);

            chart.ChartLevels2.Add(GetActiveOrderLevels);
            chart.ChartLineXYs.Add(GetClosedPositionLines);

            chdc.Add(chart);

            chart = new ChartData { Name = "Atrs", HeightExp = 20 };

            if (_xAtr != null && _xAtr.ChartLines != null)
                chart.ChartLines.AddRange(_xAtr.ChartLines);

            if (_xAtr2 != null && _xAtr2.ChartLines != null)
                chart.ChartLines.AddRange(_xAtr2.ChartLines);

            chdc.Add(chart);

            _chartDataContainer = chdc;

            return _chartDataContainer;
        }
        private void AddChartLevels()
        {
            var cl = _xma018.ChartLevels;
            var l = new Level
                        {
                            Color = 0xff0000,
                            BackGroundColor = 0xffffff,
                            Text = "10 Atr",
                            GetValue = Get10Atr,
                            IsValid = IsManyAtr
                        };
            cl.Add(l);
        }

        private bool IsManyAtr()
        {
            return Position.IsOpened && Math.Abs(_xma018.Ma - (double) Position.Price1) > 10f*_xAtr.GetAtr(0)
                       ? true
                       : false;
        }
        private double Get10Atr()
        {
            return (double)Position.Price1 < _xma018.Ma
                        ? (double) Position.Price1 + 10f*_xAtr.GetAtr(0)
                        : (double) Position.Price1 - 10f*_xAtr.GetAtr(0);
        }
    }
}
