using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using GS.Extension;
using GS.ICharts;
using GS.Trade.Data.Chart;

namespace GS.Trade.Strategies
{
    public partial class Z007
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

            chart.ChartLevels.Add(AddChartLevels3());
            //AddChartLevels();
            //if (_xma018 != null && _xma018.ChartLevels != null)
            //    chart.ChartLevels.Add(_xma018.ChartLevels);
            //AddChartText();
            //if (_xma018 != null && _xma018.ChartTexts != null)
            //    chart.ChartTexts.Add(_xma018.ChartTexts);
            chart.ChartTexts.Add(AddChartText2());

            chart.ChartLevels2.Add(GetActiveOrderLevels);
            chart.ChartLineXYs.Add(GetClosedPositionLines);

            chdc.Add(chart);

            chart = new ChartData { Name = "Atrs", HeightExp = 20 };

            //if (_xAtr != null && _xAtr.ChartLines != null)
            //    chart.ChartLines.AddRange(_xAtr.ChartLines);

            //if (_xAtr1 != null && _xAtr1.ChartLines != null)
            //    chart.ChartLines.AddRange(_xAtr1.ChartLines);

            //if (_xAtr2 != null && _xAtr2.ChartLines != null)
            //    chart.ChartLines.AddRange(_xAtr2.ChartLines);

            if (_xAtr != null && _xAtr.ChartLines != null)
                chart.ChartLines.AddRange(_xAtr.ChartLines);

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
                Text = "+5 Atr",
                GetValue = () => (double)Position.Price1 + 5f * XTrend.VolatilityUnit,
                IsValid = () => Position.IsOpened
            };
            cl.Add(l);

            l = new Level
            {
                Color = 0xff0000,
                BackGroundColor = 0xffffff,
                Text = "--5 Atr",
                GetValue = () => (double)Position.Price1 - 5f * XTrend.VolatilityUnit,
                IsValid = () => Position.IsOpened
            };
            cl.Add(l);

            l = new Level
            {
                Color = 0xff0000,
                BackGroundColor = 0xffffff,
                Text = "+10 Atr",
                GetValue = () => (double)Position.Price1 + 10f * XTrend.VolatilityUnit,
                IsValid = () => Position.IsOpened
            };
            cl.Add(l);
            l = new Level
            {
                Color = 0xff0000,
                BackGroundColor = 0xffffff,
                Text = "--10 Atr",
                GetValue = () => (double)Position.Price1 - 10f * XTrend.VolatilityUnit,
                IsValid = () => Position.IsOpened
            };
            cl.Add(l);

            l = new Level
            {
                Color = 0xff0000,
                BackGroundColor = 0xffffff,
                Text = "+15 Atr",
                GetValue = () => (double)Position.Price1 + 15f * XTrend.VolatilityUnit,
                IsValid = () => Position.IsOpened
            };
            cl.Add(l);
            l = new Level
            {
                Color = 0xff0000,
                BackGroundColor = 0xffffff,
                Text = "--15 Atr",
                GetValue = () => (double)Position.Price1 - 15f * XTrend.VolatilityUnit,
                IsValid = () => Position.IsOpened
            };
            cl.Add(l);

            l = new Level
            {
                Color = 0x0000ff,
                BackGroundColor = 0xffffff,
                Text = "PosMax",
                GetValue = () => (double)PositionMax.GetValidValue,
                IsValid = () => PositionMax.IsValid
            };
            cl.Add(l);

            l = new Level
            {
                Color = 0xff0000,
                BackGroundColor = 0xffffff,
                Text = "PosMin",
                GetValue = () => (double)PositionMin.GetValidValue,
                IsValid = () => PositionMin.IsValid
            };
            cl.Add(l);

            l = new Level
            {
                Color = 0xfff000,
                BackGroundColor = 0xffffff,
                Text = "TrailingLow",
                GetValue = () => (double)TrailingTrLow.GetValidValue,
                IsValid = () => TrailingTrLow.IsValid
            };
            cl.Add(l);

            l = new Level
            {
                Color = 0x000fff,
                BackGroundColor = 0xffffff,
                Text = "TrailingHigh",
                GetValue = () => (double)TrailingTrHigh.GetValidValue,
                IsValid = () => TrailingTrHigh.IsValid
            };
            cl.Add(l);

            l = new Level
            {
                Color = 0x00ff00,
                BackGroundColor = 0xffffff,
                Text = "PosStop",
                GetValue = () => (double)PositionStop.GetValidValue,
                IsValid = () => PositionStop.IsValid
            };
            cl.Add(l);

            //l = new Level
            //{
            //    Color = 0xff0000,
            //    BackGroundColor = 0xffffff,
            //    Text = "PosMed",
            //    GetValue = () => (double)PositionMedian,
            //    IsValid = () => PositionMax.IsValid && PositionMin.IsValid
            //};
            //cl.Add(l);
        }
        private IList<ILevel> AddChartLevels3()
        {
            // var cl2 = _xma018.ChartLevels;
            // 04.04.2018
            if (_xma018?.ChartLevels == null)
                return null;

            var cl = new List<ILevel>();
            cl.AddRange(_xma018.ChartLevels);
            Level l;
            //l = new Level
            //{
            //    Color = 0xff0000,
            //    BackGroundColor = 0xffffff,
            //    Text = "+5 Atr",
            //    GetValue = () => (double)Position.Price1 + 5f * XTrend.VolatilityUnit,
            //    IsValid = () => Position.IsOpened
            //};
            //cl.Add(l);

            //l = new Level
            //{
            //    Color = 0xff0000,
            //    BackGroundColor = 0xffffff,
            //    Text = "--5 Atr",
            //    GetValue = () => (double)Position.Price1 - 5f * XTrend.VolatilityUnit,
            //    IsValid = () => Position.IsOpened
            //};
            //cl.Add(l);

            l = new Level
            {
                Color = 0xff0000,
                BackGroundColor = 0xffffff,
                Text = "+10 Atr",
                GetValue = () => (double)Position.Price1 + 10f * XTrend.VolatilityUnit,
                IsValid = () => Position.IsOpened
            };
            cl.Add(l);
            l = new Level
            {
                Color = 0xff0000,
                BackGroundColor = 0xffffff,
                Text = "--10 Atr",
                GetValue = () => (double)Position.Price1 - 10f * XTrend.VolatilityUnit,
                IsValid = () => Position.IsOpened
            };
            cl.Add(l);

            l = new Level
            {
                Color = 0xff0000,
                BackGroundColor = 0xffffff,
                Text = "+15 Atr",
                GetValue = () => (double)Position.Price1 + 15f * XTrend.VolatilityUnit,
                IsValid = () => Position.IsOpened
            };
            cl.Add(l);
            l = new Level
            {
                Color = 0xff0000,
                BackGroundColor = 0xffffff,
                Text = "--15 Atr",
                GetValue = () => (double)Position.Price1 - 15f * XTrend.VolatilityUnit,
                IsValid = () => Position.IsOpened
            };
            cl.Add(l);

            l = new Level
            {
                Color = 0x009999,
                BackGroundColor = 0xffffff,
                Text = "PosMax",
                GetValue = () => (double)PositionMax.GetValidValue,
                IsValid = () => PositionMax.IsValid
            };
            cl.Add(l);

            l = new Level
            {
                Color = 0x990000,
                BackGroundColor = 0xffffff,
                Text = "PosMin",
                GetValue = () => (double)PositionMin.GetValidValue,
                IsValid = () => PositionMin.IsValid
            };
            cl.Add(l);

            // 18.04.2018
            l = new Level
            {
                Color = 0x00ff00,
                BackGroundColor = 0xffffff,
                Text = "PosStop",
                LineWidth = 2,
                GetValue = () => (double)PositionStop.GetValidValue,
                IsValid = () => PositionStop.IsValid
            };
            cl.Add(l);

            l = new Level
            {
                Color = 0x00ffff,
                BackGroundColor = 0xffffff,
                //Text = $"Mode5: {(Position.IsShort ? XTrend.Low2 : XTrend.High2).ToString("N2")}",
                Text = "Mode5",
                LineWidth = 2,
                GetValue = () => Position.IsShort ? XTrend.Low2 : XTrend.High2,
                IsValid = () => XTrend != null && Mode == 5 && Position != null && Position.IsOpened 
                                && (EntrySignal53 != 0 || EntrySignal54 != 0)
            };
            cl.Add(l);

            //l = new Level
            //{
            //    Color = 0xff0000,
            //    BackGroundColor = 0xffffff,
            //    Text = "TrailingLow",
            //    LineWidth = 3,
            //    GetValue = () => (double)TrailingTrLow.GetValidValue,
            //    IsValid = () => TrailingTrLow.IsValid
            //};
            //cl.Add(l);

            //l = new Level
            //{
            //    Color = 0x0000ff,
            //    BackGroundColor = 0xffffff,
            //    Text = "TrailingHigh",
            //    LineWidth = 3,
            //    GetValue = () => (double)TrailingTrHigh.GetValidValue,
            //    IsValid = () => TrailingTrHigh.IsValid
            //};
            //cl.Add(l);

            //l = new Level
            //{
            //    Color = 0xff0000,
            //    BackGroundColor = 0xffffff,
            //    Text = "PosMed",
            //    GetValue = () => (double)PositionMedian,
            //    IsValid = () => PositionMax.IsValid && PositionMin.IsValid
            //};
            //cl.Add(l);

            return cl;
        }

        private void AddChartLevels2()
        {
            var cl = _xma018.ChartLevels;
            var l = new Level
            {
                Color = 0xff0000,
                BackGroundColor = 0xffffff,
                Text = "5 Atr",
                GetValue = () => (double)Position.Price1 < _xma018.Ma
                        ? (double)Position.Price1 + 5f * XTrend.VolatilityUnit
                        : (double)Position.Price1 - 5f * XTrend.VolatilityUnit,

                IsValid = () => ((double)Position.Price1 - _xma018.Ma) * Position.Pos > 0
            };
            cl.Add(l);

            l = new Level
            {
                Color = 0xff0000,
                BackGroundColor = 0xffffff,
                Text = "10 Atr",
                GetValue = () => (double)Position.Price1 < _xma018.Ma
                        ? (double)Position.Price1 + 10f * XTrend.VolatilityUnit
                        : (double)Position.Price1 - 10f * XTrend.VolatilityUnit,

                IsValid = () => ((double)Position.Price1 - _xma018.Ma) * Position.Pos > 0
            };
            cl.Add(l);

            l = new Level
            {
                Color = 0xff0000,
                BackGroundColor = 0xffffff,
                Text = "15 Atr",
                GetValue = () => (double)Position.Price1 < _xma018.Ma
                        ? (double)Position.Price1 + 15f * XTrend.VolatilityUnit
                        : (double)Position.Price1 - 15f * XTrend.VolatilityUnit,

                IsValid = () => ((double)Position.Price1 - _xma018.Ma) * Position.Pos > 0
            };
            cl.Add(l);

        }

        private void AddChartText()
        {
            var ctxl = _xma018.ChartTexts;
            ChartText ctx;
            //var ctx = new ChartText
            //{
            //    Color = 0x0,
            //    GetBackGroundColor =
            //        () => RealReverseLossCnt == 0 ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
            //    Header = "RvrLossCnt:",
            //    GetText = () => string.Format(" {0}", RealReverseLossCnt),
            //    IsValid = () => true
            //};
            //ctxl.Add(ctx);
            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                    () => RichTarget == 0 ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
                Header = "RichTarget:",
                GetText = () => string.Format(" {0}", RichTarget),
                IsValid = () => true
            };
            ctxl.Add(ctx);
            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                    () => Mode == 1 ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
                Header = "Mode:",
                GetText = () => string.Format(" {0}", Mode),
                IsValid = () => true
            };
            ctxl.Add(ctx);
            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                    () => _swingCountEntry != 0 ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
                Header = "SwCntEntry:",
                GetText = () => string.Format(" {0}", _swingCountEntry),
                IsValid = () => true
            };
            ctxl.Add(ctx);
            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                    () => TrEntryEnabled.Value ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
                Header = "TrEntryEnabled:",
                GetText = () => string.Format(" {0}", TrEntryEnabled.Value),
                IsValid = () => true
            };
            ctxl.Add(ctx);
            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                    () => TrMaxContracts.Value ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
                Header = "TrMaxContracts:",
                GetText = () => string.Format(" {0}", TrMaxContracts.Value),
                IsValid = () => true
            };
            ctxl.Add(ctx);
            /*
            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                    () => ModeSafe == 0 ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
                Header = "Safe:",
                GetText = () => string.Format(" {0}", ModeSafe),
                IsValid = () => true
            };
            ctxl.Add(ctx);
            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                    () => IsPositionRiskLow  ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
                Header = "Risk:",
                GetText = () => string.Format(" {0}", IsPositionRiskLow ? 0 : 1),
                IsValid = () => true
            };
            ctxl.Add(ctx);
            */

        }

        private IList<IChartText> AddChartText2()
        {
            if (_xma018 == null || _xma018.ChartTexts == null)
                return null;

            var ctxl = new List<IChartText>();
            ctxl.AddRange(_xma018.ChartTexts);
            //   var ctxl = _xma018.ChartTexts;
            ChartText ctx;
            //var ctx = new ChartText
            //{
            //    Color = 0x0,
            //    GetBackGroundColor =
            //        () => RealReverseLossCnt == 0 ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
            //    Header = "RvrLossCnt:",
            //    GetText = () => string.Format(" {0}", RealReverseLossCnt),
            //    IsValid = () => true
            //};
            //ctxl.Add(ctx);
            // 17.01.15
            //ctx = new ChartText
            //{
            //    Color = 0x0,
            //    GetBackGroundColor =
            //        () => RichTarget == 0 ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
            //    Header = "RichTarget:",
            //    GetText = () => string.Format(" {0}", RichTarget),
            //    IsValid = () => true
            //};
            //ctxl.Add(ctx);

            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                   () => Position.PnL1 > 0 ? unchecked((int)0xa000ff00) : unchecked((int)0xa0ff0000),
                Header = "PnL:",
                GetText = () => $" {Position.PnL1.ToString("N2")}",
                IsValid = () => Position != null
            };
            ctxl.Add(ctx);

            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                   () => Position.DailyCurrentPnL > 0 ? unchecked((int)0xa000ff00) : unchecked((int)0xa0ff0000),
                Header = "DailyPnl:",
                GetText = () => $" {Position.DailyCurrentPnL.ToString("N2")}",
                IsValid = () => Position != null
            };
            ctxl.Add(ctx);

            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                  () => Position.DailyPnLFixed > 0 ? unchecked((int)0xa000ff00) : unchecked((int)0xa0ff0000),
                Header = "DailyPnlFixed:",
                GetText = () => $" {Position.DailyPnLFixed.ToString("N2")}",
                IsValid = () => Position != null
            };
            ctxl.Add(ctx);

            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                  () => Position.PosPnLFixed > 0 ? unchecked((int)0xa000ff00) : unchecked((int)0xa0ff0000),
                Header = "LastPnl:",
                GetText = () => $" {Position.PosPnLFixed.ToString("N2")}",
                IsValid = () => Position != null && Position.IsNeutral
            };
            ctxl.Add(ctx);

            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                   () => PrimTrend?.Trend>0 ? unchecked((int)0xa000ff00) : unchecked((int)0xa0ff0000),
                Header = "PrimTrend:",
                GetText = () => $" {PrimTrend?.Trend}",
                IsValid = () => PrimTrend != null
            };
            ctxl.Add(ctx);
            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                  () => PrimTrend?.Trend2 > 0 
                            ? unchecked((int)0xa000ff00) 
                            : (PrimTrend?.Trend2 < 0
                                ? unchecked((int)0xa0ff0000)
                                : unchecked((int)0xa00000ff)),
                Header = "PrimTrend2:",
                GetText = () => $" {PrimTrend?.Trend2}",
                IsValid = () => PrimTrend != null
            };
            ctxl.Add(ctx);
            
            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                  () => PrimTrend?.Trend50 > 0 ? unchecked((int)0xa000ff00) : unchecked((int)0xa0ff0000),
                Header = "PrimTrend5:",
                GetText = () => $" {PrimTrend?.Trend50}",
                IsValid = () => PrimTrend != null
            };
            ctxl.Add(ctx);

            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                   () => PrimTrend?.BarsDailyCount > 1 ? unchecked((int)0xa000ff00) : unchecked((int)0xa0ff0000),
                Header = "PrimTrend.BarsCount:",
                GetText = () => $" {PrimTrend?.BarsDailyCount}",
                IsValid = () => PrimTrend != null
            };
            ctxl.Add(ctx);

            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                   () => PrimTrend?.ItemsDailyCount > 1 ? unchecked((int)0xa000ff00) : unchecked((int)0xa0ff0000),
                Header = "PrimTrend.ItemsCount:",
                GetText = () => $" {PrimTrend?.ItemsDailyCount}",
                IsValid = () => PrimTrend != null
            };
            ctxl.Add(ctx);

            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                    () => Mode == 1 ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
                Header = "Mode:",
                GetText = () => $" {Mode}",
                IsValid = () => true
            };
            ctxl.Add(ctx);



            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                    () => _swingCountStartEntry <= 0 ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
                Header = "SwCntStartEntry:",
                GetText = () => $" {_swingCountStartEntry}",
                IsValid = () => true
            };
            ctxl.Add(ctx);
            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                    () => _swingCountEntry != 0 ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
                Header = "SwCntEntry:",
                GetText = () => $" {_swingCountEntry}",
                IsValid = () => true
            };
            ctxl.Add(ctx);
            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                    () => TrEntryEnabled.Value ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
                Header = "TrEntryEnabled:",
                GetText = () => $" {TrEntryEnabled.Value}",
                IsValid = () => true
            };
            ctxl.Add(ctx);
            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                    () => TrMaxContracts.Value ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
                Header = "TrMaxContracts:",
                GetText = () => $" {TrMaxContracts.Value}",
                IsValid = () => true
            };
            ctxl.Add(ctx);
            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                    () => Trend55Changed.Value ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
                Header = "Trend55Changed:",
                GetText = () => $" {Trend55Changed.Value}",
                IsValid = () => true
            };
            ctxl.Add(ctx);
            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                   () => 
                   (Position.IsLong && XTrend.Ma.IsGreaterThan((double)Position.Price1)) ||
                   (Position.IsShort && XTrend.Ma.IsLessThan((double)Position.Price1))
                   ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
                Header = "Atrs: ",
                GetText = () =>
                    $"{(double) Position.Operation*(XTrend.Ma - (double) Position.Price1)/XTrend.VolatilityUnit:f2}",
                IsValid = () => Position.IsOpened && XTrend.VolatilityUnit.IsGreaterThan(0f)
            };
            ctxl.Add(ctx);
            /*
            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                    () => ModeSafe == 0 ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
                Header = "Safe:",
                GetText = () => string.Format(" {0}", ModeSafe),
                IsValid = () => true
            };
            ctxl.Add(ctx);
            ctx = new ChartText
            {
                Color = 0x0,
                GetBackGroundColor =
                    () => IsPositionRiskLow  ? unchecked((int)0xa00000ff) : unchecked((int)0xa0ff0000),
                Header = "Risk:",
                GetText = () => string.Format(" {0}", IsPositionRiskLow ? 0 : 1),
                IsValid = () => true
            };
            ctxl.Add(ctx);
            */
            return ctxl;
        }

        private bool IsManyAtr()
        {
            return Position.IsOpened && Math.Abs(_xma018.Ma - (double)Position.Price1) > 10f * _xAtr.GetAtr(1)
                       ? true
                       : false;
        }
        private double Get10Atr()
        {
            return (double)Position.Price1 < _xma018.Ma
                        ? (double)Position.Price1 + 10f * _xAtr.GetAtr(0)
                        : (double)Position.Price1 - 10f * _xAtr.GetAtr(0);
        }
    }
}
