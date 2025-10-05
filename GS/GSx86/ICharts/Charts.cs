using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.ICharts
{
    public interface IChartable
    {
        IChartDataContainer ChartDataContainer { get; }
    }
    public interface IChartDataContainer
    {
        List<IChartData> Charts { get; }
        void Add(IChartData chart);
        void ClearAll();
    }
    public interface IChartData
    {
        string Name { get; }
        int HeightExp { get; }
        float HeightReal { get; set; }

        List<IChartBarSeries> ChartBars { get; }
        List<ILineSeries> ChartLines { get; }
        List<IBandSeries> ChartBands { get; }
        List<IList<ILevel>> ChartLevels{get;}
        List<Func<IEnumerable<ILevel>>> ChartLevels2 { get; }
        List<IList<IChartText>> ChartTexts { get; }
        List<Func<IEnumerable<ILineXY>>> ChartLineXYs { get; }

        void ClearAll();
    }
    public interface IChartBarSeriesCollection
    {
        IList<ILineSeries> ChartBars { get; }
    }
    public interface IChartBarSeries
    {
        string Name { get; }
        int Count { get; }

        Int32 ColorUp { get; }
        Int32 ColorDown { get; }
        Int32 ColorEdge { get; }

        DateTime GetDateTime(int index);

        double GetOpen(int index);
        double GetHigh(int index);
        double GetLow(int index);
        double GetClose(int index);
        double GetVolume(int index);

        double GetLine(int index);
    }
    public interface ILineCollection
    {
        IList<ILineSeries> ChartLines { get; }
    }
    public interface ILineSeries
    {
        /*
        string GetName();
        int GetColor();
        */
        string Name { get; }
        Int32 Color { get; }
        int Count { get; }
        DateTime GetDateTime(int index);
        double GetLine(int index);
    }
    public interface IBandCollection
    {
        IList<IBandSeries> Bands { get; }
       // IList<IBandSeries> GetBandCollection();
    }
    public interface IBandSeries
    {
        int Count { get; }
        int TimeIntSeconds { get; }
        DateTime GetDateTime(int index);
        double GetLine(int index);
        double GetHigh(int index);
        double GetLow(int index);

        string BandName { get; }
        int BandLineColor { get; }
        int BandColor { get; }
        int BandFillColor { get; }
    }
    public interface ILevelCollection
    {
        IList<ILevel> GetLevelCollection(string tradeKey);
    }
    public interface ILevel
    {
      //  int Count { get; }
        bool Valid { get; }
        double LevelValue { get; }
        string TextValue { get; }
        int LevelColor { get; }
        int LevelBackGroundColor { get; }
        string LevelText { get; }
        int LevelLineWidth { get; }
    }
    public interface IChartText
    {
        bool Valid { get; }

        string HeaderValue { get; }
        string TextValue { get; }

        int TextColor { get; }
        int TextBackGroundColor { get; }

        string FontName { get; }
        double FontSize { get; }
    }

    public interface ILineXYCollection
    {
        IList<ILineXY> GetLineXYCollection(string tradekey);
    }
    public interface ILineXY
    {
        double LineY1 { get; }
        double LineY2 { get; }

        DateTime LineX1 { get; }
        DateTime LineX2 { get; }

        int LineColor { get; }
        int LineWidth { get; }
    }
}
