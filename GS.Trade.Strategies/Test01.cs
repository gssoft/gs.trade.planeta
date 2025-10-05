using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GS.ICharts;
using GS.Interfaces;
using GS.Trade.Data;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Chart;
using GS.Trade.Data.Studies;
using GS.Trade.Data.Studies.Averages;
using GS.Trade.Data.Studies.Bands;
using GS.Trade.Data.Studies.GS;
using GS.Trade.Data.Studies.GS.xMa018;
using GS.Trade.Data.Studies.Stochastic;

namespace GS.Trade.Strategies
{
    public class Test01 : Strategy
    {
        [XmlIgnore]
        public override IBars Bars { get; protected set; }
        [XmlIgnore]
        public override int MaxBarsBack { get; protected set; }
        [XmlIgnore]
        public override Atr Atr { get { return _xAtr; } }

        private Xma018 _xma018;
        private Xma018 _xma0182;

        private BarSeries _bars;

        private Atr _xAtr;

        private Atr _xAtr1;
        private Atr _xAtr2;
        private Atr _xAtr3;
        private Atr _xAtr4;
        private Atr _xAtr5;
        private Atr _xAtr6;

        private XAverage _xAvrg21;
        private XAverage _xAvrg22;
        private XAverage _xAvrg23;

        private SAverage _sAvrg1;
        private SAverage _sAvrg2;
        private SAverage _sAvrg3;

        private Stochastic _xStoch1;
        private Stochastic _xStoch2;

        private Cci _xCci;
        private Cci _xCci2;

        private StdDev _stdDev;
        private BollingerBand _bb;

        private BollingerBand2 _bb2;

        private StdDevBand _bb31;
        private StdDevBand _bb32;
        private StdDevBand _bb33;
        private StdDevBand _bb34;
        private StdDevBand _bb35;

        private BollingerBand _bb0;

        // Xma018
        public int MaLength { get; set; }
        public int MaAtrLength { get; set; }
        public int MaAtrLength1 { get; set; }
        public int MaAtrLength2 { get; set; }

        public float MaKAtr { get; set; }
        public int MaMode { get; set; }

        // XAverage
        public int XMaLength { get; set; }

        // CCI
        public int CciMaLength { get; set; }
        public int CciMaSmoothLength { get; set; }
        public float CciKDeviation { get; set; }

        public int CciMaLength2 { get; set; }
        public int CciMaSmoothLength2 { get; set; }
        public float CciKDeviation2 { get; set; }

        public override void Init()
        {
            base.Init(); 
            if (IsWrong) return;

            Bars001 bs;
            /*
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 1, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 2, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 3, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 4, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 5, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 6, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 7, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 8, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 9, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 10, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 11, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 12, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 13, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 14, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 15, 0));

            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 60, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 120, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 180, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 240, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 300, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 600, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 900, 0));
            Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, 3600, 0));
            */

            _xma018 = (Xma018)TradeContext.RegisterTimeSeries(new Xma018("Xma18", Ticker, (int)TimeInt, MaLength, MaAtrLength, MaKAtr, MaMode));
          //  _xma018.Init();

          //  _xma0182 = (Xma018)TradeContext.RegisterTimeSeries(new Xma018("Xma18", Ticker, (int)TimeInt, MaLength, MaAtrLength1, MaAtrLength2, MaKAtr, MaMode));
          //  _xma0182.Init();

            //Bars = _xma018.Bars;

            _xAtr1 = (Atr)Ticker.RegisterTimeSeries(new Atr("MyAtr", Ticker, (int)TimeInt, 50));
            _xAtr2 = (Atr)Ticker.RegisterTimeSeries(new Atr("MyAtr", Ticker, (int)TimeInt, 100));
            _xAtr3 = (Atr)Ticker.RegisterTimeSeries(new Atr("MyAtr", Ticker, (int)TimeInt, 150));
            _xAtr4 = (Atr)Ticker.RegisterTimeSeries(new Atr("MyAtr", Ticker, (int)TimeInt, 200));
            _xAtr5 = (Atr)Ticker.RegisterTimeSeries(new Atr("MyAtr", Ticker, (int)TimeInt, 250));
            _xAtr6 = (Atr)Ticker.RegisterTimeSeries(new Atr("MyAtr", Ticker, (int)TimeInt, 300));


         //   _xAtr.Init();

          //  _xAtr = (Atr2)Ticker.RegisterTimeSeries(new Atr2("MyAtr2", Ticker, (int)TimeInt, MaAtrLength1, MaAtrLength2 ));
          //  _xAtr.Init();

          //  _xAvrg = (XAverage) TradeContext.RegisterTimeSeries(new XAverage("XAverage", Ticker,(int)TimeInt, XMaLength));
          //  _xAvrg1 = (XAverage)TradeContext.RegisterTimeSeries(new XAverage("XAverage1", Ticker, (int)TimeInt, 50));
          //  _xAvrg2 = (XAverage)TradeContext.RegisterTimeSeries(new XAverage("XAverage", Ticker, (int)TimeInt, 100));
          //  _xAvrg3 = (XAverage)TradeContext.RegisterTimeSeries(new XAverage("XAverage", Ticker, (int)TimeInt, 200));

           // _xAvrg.Init();

          //  _bars = Ticker.GetBarSeries("TypeName=Bar;TimeIntSeconds=" + TimeInt) as BarSeries;
         //   Bars = _bars;

         //_xCci = (Cci)TradeContext.RegisterTimeSeries(new Cci("Cci", Ticker, (int)TimeInt, CciMaLength, CciMaSmoothLength, CciKDeviation));
         //   _xCci.Init();

         //   _xCci2 = (Cci)TradeContext.RegisterTimeSeries(new Cci("Cci", Ticker, (int)TimeInt, CciMaLength2, CciMaSmoothLength2, CciKDeviation2));
         //   _xCci2.Init();

          //  MaxBarsBack = _xAtr.Length;

            // _xStoch1 = (Stochastic)TradeContext.RegisterTimeSeries(new Stochastic("St", Ticker, (int)TimeInt,30,3,3,20,80));
           // _stdDev = (StdDev)TradeContext.RegisterTimeSeries(new StdDev("StdDev", Ticker, (int)TimeInt, 30, 50));
           // _bb = (BollingerBand)TradeContext.RegisterTimeSeries(new BollingerBand("BBand", Ticker, (int)TimeInt, 30, 5,2f, BarValue.Close));

       //     _xAvrg21 = (XAverage2)TradeContext.RegisterTimeSeries(new XAverage2("X1", Ticker, BarValue.Close, (int) TimeInt, 15, 5));
       //     _xAvrg22 = (XAverage2)TradeContext.RegisterTimeSeries(new XAverage2("X2", Ticker, BarValue.Median, (int)TimeInt, 30, 5));
       //     _xAvrg23 = (XAverage2)TradeContext.RegisterTimeSeries(new XAverage2("X3", Ticker, BarValue.Typical, (int)TimeInt, 50, 5));

       //     _sAvrg1 = (SAverage)TradeContext.RegisterTimeSeries(new SAverage("S1", Ticker, BarValue.Close, (int)TimeInt, 15, 0));
       //     _sAvrg2 = (SAverage)TradeContext.RegisterTimeSeries(new SAverage("S2", Ticker, BarValue.Median, (int)TimeInt, 30, 0));
       //     _sAvrg3 = (SAverage)TradeContext.RegisterTimeSeries(new SAverage("S3", Ticker, BarValue.Typical, (int)TimeInt, 50, 0));
            
      //      _bb2 =
      //           (BollingerBand2)
      //           TradeContext.RegisterTimeSeries(new BollingerBand2("B2", Ticker, (int) TimeInt, BarValue.Close,  15, 2f, 2f));

            _xStoch1 = (Stochastic)TradeContext.RegisterTimeSeries(
                new StochasticSlow("St", Ticker, (int)TimeInt,BarValue.High,BarValue.Low,BarValue.Close, 30, 20, 80));

            _xStoch2 = (Stochastic)TradeContext.RegisterTimeSeries(
                new StochasticFast("St", Ticker, (int)TimeInt, BarValue.High, BarValue.Low, BarValue.Close, 50, 30, 70));

            _bb31 = (StdDevBand)
                TradeContext.RegisterTimeSeries(new StdDevBand("B31", Ticker, (int)TimeInt, BarValue.Median, MaType.Exponential, 50, 0,
                                                                                           BarValue.Close, 50, 0, 2f, 2f));
            _bb32 = (StdDevBand)
                TradeContext.RegisterTimeSeries(new StdDevBand("B32", Ticker, (int)TimeInt, BarValue.Median, MaType.Simple, 50, 0,
                                                                                           BarValue.Close, 50, 0, 2f, 2f));

             /* 
            _bb33 = (StdDevBand)
                TradeContext.RegisterTimeSeries(new StdDevBand("B3", Ticker, (int)TimeInt, BarValue.Median, MaType.Exponential, 50, 5,
                                                                                           BarValue.Close, 50, 0, 2f, 2f));
            _bb34 = (StdDevBand)
                 TradeContext.RegisterTimeSeries(new StdDevBand("B3", Ticker, (int)TimeInt, BarValue.Median, MaType.Simple, 50, 5,
                                                                                            BarValue.Close, 50, 0, 2f, 2f));
             */
            _bb35 = (StdDevBand)
                TradeContext.RegisterTimeSeries(new StdDevBand("B35", Ticker, (int)TimeInt, BarValue.Median, MaType.Simple, 15, 0,
                                                                                           BarValue.Close, 15, 0, 2f, 2f));

            _bb0 = (BollingerBand)
            TradeContext.RegisterTimeSeries(new BollingerBand("B0", Ticker, (int) TimeInt, BarValue.Close,  100, 2f, 2f));


            TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategy", "Strategy", "Init " + Code, ToString(), "");
        }

        public override string Key
        {
            get
            {
                return String.Format("Type={0};Name={1};Code={2};TradeAccountKey={3};TickerKey={4};TimeInt={5}",
                      GetType(), Name, Code, TradeAccountKey, TickerKey, TimeInt);
            }
        }
        public override string ToString()
        {
            return String.Format("Name={0};Code={1};Account={2};Ticker={3};TimeInt={4}",
                        Name, Code, TradeAccountKey, TickerKey, TimeInt);
        }
        public override void Main()
        {
           
        }
        public override void Finish()
        {
        }
        //public override ILineSeries LineSeries{ get{return _xAvrg;}}
        //public override ILineSeries LineSeries { get { return _xAvrg; } }
        //public override IBandSeries Band { get { return _xCci2; } }
        //public override ILevelCollection Levels { get { return _xma018; } }

       // public override IBandSeries Band { get { return _xma0182; } }
        public override ILevelCollection Levels { get { return _xma0182; } }

        public override IList<ILineSeries> ChartLines
        {
            //get { return new List<ILineSeries> { _xAvrg1, _xAvrg2, _xAvrg3, _xStoch1 }; }
            get { return CreateChartLineList(); }
        }
        public override IList<IBandSeries> Bands
        {
            // get { return new List<IBandSeries>{_xCci, _xCci2, _xma018}; }
            //get { return new List<IBandSeries> { _xCci, _xCci2, _xma0182, _xma018 }; }
            get { return CreateChartBandList(); }
        }
        private IList<ILineSeries> CreateChartLineList()
        {
            var l = new List<ILineSeries>();

            
         //   if (_xStoch1 != null && _xStoch1.ChartLines != null)
         //       l.AddRange(_xStoch1.ChartLines);
            if (_xCci != null && _xCci.ChartLines != null)
                l.AddRange(_xCci.ChartLines);
            if (_bb != null && _bb.ChartLines != null)
                l.AddRange(_bb.ChartLines);
            if (_xAvrg21 != null && _xAvrg21.ChartLines != null)
                l.AddRange(_xAvrg21.ChartLines);
            if (_xAvrg22 != null && _xAvrg22.ChartLines != null)
                l.AddRange(_xAvrg22.ChartLines);
            if (_xAvrg23 != null && _xAvrg23.ChartLines != null)
                l.AddRange(_xAvrg23.ChartLines);

            if (_sAvrg1 != null && _sAvrg1.ChartLines != null)
                l.AddRange(_sAvrg1.ChartLines);
            if (_sAvrg2 != null && _sAvrg2.ChartLines != null)
                l.AddRange(_sAvrg2.ChartLines);
            if (_sAvrg3 != null && _sAvrg3.ChartLines != null)
                l.AddRange(_sAvrg3.ChartLines);

            if (_bb2 != null && _bb2.ChartLines != null)
                l.AddRange(_bb2.ChartLines);

            if (_bb31 != null && _bb31.ChartLines != null)
                l.AddRange(_bb31.ChartLines);
            if (_bb32 != null && _bb32.ChartLines != null)
                l.AddRange(_bb32.ChartLines);
            if (_bb33 != null && _bb33.ChartLines != null)
                l.AddRange(_bb33.ChartLines);
            if (_bb34 != null && _bb34.ChartLines != null)
                l.AddRange(_bb34.ChartLines);
            if (_bb35 != null && _bb35.ChartLines != null)
                l.AddRange(_bb35.ChartLines);

            if (_bb0 != null && _bb0.ChartLines != null)
                l.AddRange(_bb0.ChartLines);


            return l;
        }
        private IList<IBandSeries> CreateChartBandList()
        {
            var l = new List<IBandSeries>();

            if (_xCci != null && _xCci.ChartBands != null)
                        l.AddRange(_xCci.ChartBands);
            if (_xma018 != null && _xma018.ChartBands != null)
                l.AddRange(_xma018.ChartBands);
            if (_bb != null && _bb.ChartBands != null)
                l.AddRange(_bb.ChartBands);
            if (_bb2 != null && _bb2.ChartBands != null)
                l.AddRange(_bb2.ChartBands);

            if (_bb31 != null && _bb31.ChartBands != null)
                l.AddRange(_bb31.ChartBands);
            if (_bb32 != null && _bb32.ChartBands != null)
                l.AddRange(_bb32.ChartBands);
            if (_bb33 != null && _bb33.ChartBands != null)
                l.AddRange(_bb33.ChartBands);
            if (_bb34 != null && _bb34.ChartBands != null)
                l.AddRange(_bb34.ChartBands);
            if (_bb35 != null && _bb35.ChartBands != null)
                l.AddRange(_bb35.ChartBands);

            if (_bb0 != null && _bb0.ChartBands != null)
                l.AddRange(_bb0.ChartBands);

            return l;
        }

        private IChartDataContainer _chartDataContainer;
        [XmlIgnore]
        public override IChartDataContainer ChartDataContainer
        {
            get { return _chartDataContainer ?? CreateChartDataContainer(); }
        }
        private IChartDataContainer CreateChartDataContainer()
        {
            _chartDataContainer = new ChartDataContainer();
         
            var chart = new ChartData { Name = "Main", HeightExp = 50 };

            if (_bb35 != null && _bb35.Bars != null)
                chart.ChartBars.Add(_bb35.Bars.ChartBarSeries);
            
            if (_xma018 != null && _xma018.ChartBands != null)
                chart.ChartBands.AddRange(_xma018.ChartBands);

            if (_xma018 != null && _xma018.ChartLevels != null)
                    chart.ChartLevels.Add(_xma018.ChartLevels);

            chart.ChartLevels2.Add(GetActiveOrderLevels);
            /*
            if (_bb35 != null && _bb35.ChartLines != null)
                chart.ChartLines.AddRange(_bb35.ChartLines);

            if (_bb35 != null && _bb35.ChartBands != null)
                chart.ChartBands.AddRange(_bb35.ChartBands);

            if (_bb0 != null && _bb0.ChartLines != null)
                chart.ChartLines.AddRange(_bb0.ChartLines);

            if (_bb0 != null && _bb0.ChartBands != null)
                chart.ChartBands.AddRange(_bb0.ChartBands);
            */
            _chartDataContainer.Add(chart);

            chart = new ChartData { Name = "Atrs", HeightExp = 20 };
            
        //    if (_xAtr1 != null && _xAtr1.ChartLines != null)
        //        chart.ChartLines.AddRange(_xAtr1.ChartLines);

           if (_xAtr2 != null && _xAtr2.ChartLines != null)
                chart.ChartLines.AddRange(_xAtr2.ChartLines);

            if (_xAtr3 != null && _xAtr3.ChartLines != null)
                chart.ChartLines.AddRange(_xAtr3.ChartLines);

      //      if (_xAtr4 != null && _xAtr4.ChartLines != null)
      //          chart.ChartLines.AddRange(_xAtr4.ChartLines);

      //      if (_xAtr5 != null && _xAtr5.ChartLines != null)
      //          chart.ChartLines.AddRange(_xAtr5.ChartLines);

            if (_xAtr6 != null && _xAtr6.ChartLines != null)
                chart.ChartLines.AddRange(_xAtr6.ChartLines);

            _chartDataContainer.Add(chart);

            chart = new ChartData{Name="Main2", HeightExp = 30};

            if (_bb35 != null && _bb35.Bars != null)
                chart.ChartBars.Add(_bb35.Bars.ChartBarSeries);

            if (_bb35 != null && _bb35.ChartLines != null)
                chart.ChartLines.AddRange(_bb35.ChartLines);

            if (_bb35 != null && _bb35.ChartBands != null)
                chart.ChartBands.AddRange(_bb35.ChartBands);

            if (_bb0 != null && _bb0.ChartLines != null)
                chart.ChartLines.AddRange(_bb0.ChartLines);

            if (_bb0 != null && _bb0.ChartBands != null)
                chart.ChartBands.AddRange(_bb0.ChartBands);

            _chartDataContainer.Add(chart);

            chart = new ChartData {Name = "Indicator", HeightExp = 20};

          //  if (_bb35 != null && _bb35.Bars != null)
          //      chart.ChartBars.Add(_bb35.Bars.ChartBarSeries);

            if (_xStoch1 != null && _xStoch1.ChartLines != null)
                chart.ChartLines.AddRange(_xStoch1.ChartLines);

            if (_xStoch1 != null && _xStoch1.ChartLines != null)
                chart.ChartLevels.Add(_xStoch1.ChartLevels);

            if (_xStoch2 != null && _xStoch2.ChartLines != null)
                chart.ChartLines.AddRange(_xStoch2.ChartLines);

            if (_xStoch2 != null && _xStoch2.ChartLevels != null)
                chart.ChartLevels.Add(_xStoch2.ChartLevels);



         //   if (_bb35 != null && _bb35.ChartLines != null)
         //       chart.ChartLines.AddRange(_bb35.ChartLines);
            
         //   if (_bb35 != null && _bb35.ChartBands != null)
         //       chart.ChartBands.AddRange(_bb35.ChartBands);

            _chartDataContainer.Add(chart);

            return _chartDataContainer;
        }

    }
}
