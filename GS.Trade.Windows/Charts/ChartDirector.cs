using System;
using System.Collections.Generic;
using System.Linq;
using ChartDirector;
using GS.ICharts;
using GS.Interfaces;

namespace GS.Trade.Windows.Charts
{
    public partial class ChartWindow
    {
        private IEventLog _evl;

        //private FinanceChart c;
        private XYChart _mainChart;
        private XYChart _chart2;

        private ChartContainer _chartContainer;

        //private BarSeries _bars;

        private IBars _bars;

        private long _tickCount;
       
        private const int BarsToObserver = 80;
        private int _barsToObserver = BarsToObserver;
        private double _barWidth = 8.0;
       
        private DateTime[] _timeStamps;
        private double[] _highData;
        private double[] _lowData;
        private double[] _openData;
        private double[] _closeData;
        private double[] _volData;

        private int _extraPoints = 0;
        
        private int _minPlotArea = 50;

        private int _xClientSize;
        private int _yClientSize;

        private int _xChartArea;
        private int _yChartArea;

        private int resolution = 60;

        public bool ChartTimerEnable { get; set; }

        private string _selectKey;

        public List<LineSeries> LineSeriesCollection = new List<LineSeries>();
        public List<BandSeries> BandSeriesCollection = new List<BandSeries>();
        public List<ILevelCollection> LevelSourceCollection = new List<ILevelCollection>();
        public List<ILineXYCollection> LineXYSourceCollection = new List<ILineXYCollection>();

       public void Init( IBars bs, IEventLog evl)
        {
            if (bs == null || evl == null)
                throw new Exception("ChartWindow BarSeries or EventLog is null");
            _bars = bs;
            _evl = evl;
            evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "ChartWindow", "ChartWindow", "Init", bs.ToString(), "");
        }

        private void ProcessSeries(FinanceChart fch, Chart ch)
        {
            try
            {
                foreach (var bs in ch.BarSeriesCollection)
                {
                    addCandleStick2(ch.ChartXY, 0x00ff00, 0xff0000, bs.OpenData, bs.HighData, bs.LowData, bs.CloseData);
                }

                foreach (var ls in ch.LineSeriesCollection)
                {
                    fch.addLineIndicator2(ch.ChartXY, ls.LineChartData, ls.Color, ls.Name);
                }
                foreach (var bs in ch.BandSeriesCollection)
                {
                    // if( bs.NeedDrawLine)
                    fch.addLineIndicator2(ch.ChartXY, bs.LineChartData, bs.LineColor, bs.Name + ".Line");
                    addBand2(ch.ChartXY, bs.HighChartData, bs.LowChartData, bs.BandLineColor, bs.FillColor, bs.Name);
                }
                foreach (var lc in ch.LevelsCollection)
                    foreach (var l in lc) // .Where(le=>le.Valid))
                    {
                        if (!l.Valid) continue;

                        Mark m = ch.ChartXY.yAxis()
                            .addMark(l.LevelValue, l.LevelColor, l.TextValue + ": " + l.LevelValue.ToString("N2"));
                        m.setAlignment(ChartDirector.Chart.Left);
                        m.setBackground(l.LevelBackGroundColor);
                        m.setLineWidth(l.LevelLineWidth);
                    }
                foreach (var f in ch.LevelSourceCollection)
                {
                    //var lc = f();
                    foreach (var l in f()) // .Where(le => le.Valid))
                    {
                        if (!l.Valid) continue;

                        Mark m = ch.ChartXY.yAxis().addMark(l.LevelValue, l.LevelColor,
                            l.TextValue + ": " + l.LevelValue.ToString("N2"));
                        m.setAlignment(ChartDirector.Chart.Left);
                        m.setBackground(l.LevelBackGroundColor);
                        m.setLineWidth(l.LevelLineWidth);
                    }
                }
                var y = 15;
                foreach (var t in ch.TextsCollection.SelectMany(tc => tc.Where(tx => tx.Valid)))
                {
                    ch.ChartXY.addText(0, y, t.HeaderValue + t.TextValue,
                        t.FontName, t.FontSize, t.TextColor, ChartDirector.Chart.TopLeft)
                        .setBackground(t.TextBackGroundColor);
                    y += (int) (t.FontSize + 5);
                }
                foreach (var lxy in ch.LineXYSourceCollection.SelectMany(f => f()))
                {
                    if (ch.BarSeriesCollection.Count == 0) return;

                    int i = 0, j = 0;
                    var timeStamps = ch.BarSeriesCollection[0].TimeStamps;
                    foreach (var t in timeStamps)
                    {
                        if (lxy.LineX1 < t && t != DateTime.MinValue) break;
                        i++;
                    }
                    foreach (var t in timeStamps)
                    {
                        if (lxy.LineX2 < t && t != DateTime.MinValue) break;
                        j++;
                    }

                    if (j <= 0 || i <= 0) continue;

                    var ay = new[] {lxy.LineY1, lxy.LineY2};
                    LineLayer layer1 = ch.ChartXY.addLineLayer(ay, lxy.LineColor);
                    layer1.setXData(i - 1, j - 1);
                    //layer1.setXData(i, j);
                    layer1.setLineWidth(lxy.LineWidth);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Charts Process Series Failure - " + e.Message);
            }
        }

        public void CreateChart(WinChartViewer viewer, string img)
        {

            // Create a FinanceChart object of width 640 pixels
            FinanceChart c = new FinanceChart(_xClientSize);
            // c = new FinanceChart(_xClientSize);

            // Add a title to the chart
            // c.addTitle("Finance Chart Demonstration");

            // Set the data into the finance chart object
            c.setData(_timeStamps, _highData, _lowData, _openData, _closeData, _volData, 0);

            c.setMargins(_leftMargin, _topMargin, _rightMargin, _bottomMargin);
            // Add a slow stochastic chart (75 pixels high) with %K = 14 and %D = 3
            //  c.addSlowStochastic((int) (yClientSize * 0.38), 14, 3, 0x006060, 0x606000);

            // Add the main chart with 240 pixels in height
            //_mainChart = c.addMainChart((int)(_yChartArea * 0.5));

            //   _chart2 = c.addIndicator((int) (_yChartArea*0.2));

            // Add a 10 period simple moving average to the main chart, using brown
            // color
            //    c.addSimpleMovingAvg(10, 0x663300);

            // Add a 20 period simple moving average to the main chart, using purple
            // color
            //  c.addSimpleMovingAvg(20, 0x9900ff);

            // Add an HLOC symbols to the main chart, using green/red for up/down
            // days
            //c.addCandleStick(0x00ff00, 0xff0000);

            //  addCandleStick2(_chart2, 0x00ff00, 0xff0000);

            //c.addLineIndicator2(_mainChart, _xma015Data, 0x9900ff, "Xma015");
            //c.addLineIndicator2(_mainChart, _xBandLine, 0xff0000, "XAvg");
            //c.addBand(_xmaHigh, _xmaLow, 0x9999ff, unchecked((int) 0xc06666ff), "Band");
            //c.addBand(_xBandHigh, _xBandLow, 0x0099ff, unchecked((int)0xc00066ff), "Band");
            /*
            foreach (var ls in LineSeriesCollection)
            {
                c.addLineIndicator2(_chart2, ls.LineChartData, ls.Color, ls.Name);
            }
            foreach (var bs in BandSeriesCollection)
            {
                c.addLineIndicator2(_chart2, bs.LineChartData, bs.LineColor, bs.Name + ".Line");
                addBand2(_chart2, bs.HighChartData, bs.LowChartData, bs.BandLineColor, bs.FillColor, bs.Name);
            }
             */
            /*
            var mainCh = _chartContainer.Charts.Where(ch => ch.Name == "Main").FirstOrDefault();
            if (mainCh == null)
                return;
                //throw new NullReferenceException("MainChart is Null");
            _mainChart = c.addMainChart((int)(_yChartArea * mainCh.Height));
            mainCh.ChartXY = _mainChart;
            //mainCh.ChartXY = c.addMainChart((int)(_yChartArea * mainCh.Height));
            ProcessSeries(c, mainCh);
            */
          //  foreach (var ch in _chartContainer.Charts.Where(ch => ch.Name != "Main"))
            try
            {
                foreach (var ch in _chartContainer.Charts)
                {
                    ch.ChartXY = c.addIndicator((int) (_yChartArea*ch.Height*0.98f));
                    ProcessSeries(c, ch);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Charts Create Failure - " + e.Message);
            }

            /*
            foreach (var lineXYs in LineXYSourceCollection)
            {
                var lxys = lineXYs.GetLineXYCollection(_selectKey);
                foreach (var lxy in lxys)
                {
                    int i = 0, j = 0;
                    foreach (var t in _timeStamps)
                    {
                        if (lxy.LineX1 < t && t != DateTime.MinValue) break;
                        i++;
                    }
                    foreach (var t in _timeStamps)
                    {
                        if (lxy.LineX2 < t && t != DateTime.MinValue) break;
                        j++;
                    }

                    if (j <= 0 || i <= 0) continue;

                    var ay = new[] { lxy.LineY1, lxy.LineY2 };
                    LineLayer layer1 = _mainChart.addLineLayer(ay, lxy.LineColor);
                    layer1.setXData(i, j);
                    layer1.setLineWidth(2);
                }
            }
            */
            // Add a 75 pixels volume bars sub-chart to the bottom of the main chart,
            // using green/red/grey for up/down/flat days
            // c.addVolBars((int)(_yChartArea * 1.0 * 0.15), 0x99ff99, 0xff9999, 0x808080);

            // Append a MACD(26, 12) indicator chart (75 pixels high) after the main
            // chart, using 9 days for computing divergence.
            // c.addMACD(75, 26, 12, 9, 0x0000ff, 0xff00ff, 0x008000);

            // Output the chart
            viewer.Image = c.makeImage();
        }
        public void CreateChartOld(WinChartViewer viewer, string img)
        {
            // Create a FinanceChart object of width 640 pixels
            FinanceChart c = new FinanceChart(_xClientSize);
            // c = new FinanceChart(_xClientSize);

            // Add a title to the chart
           // c.addTitle("Finance Chart Demonstration");

            // Set the data into the finance chart object
            c.setData(_timeStamps, _highData, _lowData, _openData, _closeData, _volData, 0);

            c.setMargins(_leftMargin,_topMargin,_rightMargin,_bottomMargin);
            // Add a slow stochastic chart (75 pixels high) with %K = 14 and %D = 3
          //  c.addSlowStochastic((int) (yClientSize * 0.38), 14, 3, 0x006060, 0x606000);

            // Add the main chart with 240 pixels in height
            _mainChart = c.addMainChart((int) (_yChartArea*0.5));

         //   _chart2 = c.addIndicator((int) (_yChartArea*0.2));

            // Add a 10 period simple moving average to the main chart, using brown
            // color
        //    c.addSimpleMovingAvg(10, 0x663300);

            // Add a 20 period simple moving average to the main chart, using purple
            // color
          //  c.addSimpleMovingAvg(20, 0x9900ff);

            // Add an HLOC symbols to the main chart, using green/red for up/down
            // days
            c.addCandleStick(0x00ff00, 0xff0000);

          //  addCandleStick2(_chart2, 0x00ff00, 0xff0000);
            
            //c.addLineIndicator2(_mainChart, _xma015Data, 0x9900ff, "Xma015");
            //c.addLineIndicator2(_mainChart, _xBandLine, 0xff0000, "XAvg");
            //c.addBand(_xmaHigh, _xmaLow, 0x9999ff, unchecked((int) 0xc06666ff), "Band");
            //c.addBand(_xBandHigh, _xBandLow, 0x0099ff, unchecked((int)0xc00066ff), "Band");
            /*
            foreach (var ls in LineSeriesCollection)
            {
                c.addLineIndicator2(_chart2, ls.LineChartData, ls.Color, ls.Name);
            }
            foreach (var bs in BandSeriesCollection)
            {
                c.addLineIndicator2(_chart2, bs.LineChartData, bs.LineColor, bs.Name + ".Line");
                addBand2(_chart2, bs.HighChartData, bs.LowChartData, bs.BandLineColor, bs.FillColor, bs.Name);
            }
             */
            foreach( var ch in _chartContainer.Charts)
            {
                ch.ChartXY = c.addIndicator((int)(_yChartArea * ch.Height));
                
                foreach(var bs in ch.BarSeriesCollection)
                {
                    addCandleStick2(ch.ChartXY, 0x00ff00, 0xff0000, bs.OpenData, bs.HighData, bs.LowData, bs.CloseData);
                }
                
                foreach (var ls in ch.LineSeriesCollection)
                {
                    c.addLineIndicator2(ch.ChartXY, ls.LineChartData, ls.Color, ls.Name);
                }
                foreach (var bs in ch.BandSeriesCollection)
                {
                    c.addLineIndicator2(ch.ChartXY, bs.LineChartData, bs.LineColor, bs.Name + ".Line");
                    addBand2(ch.ChartXY, bs.HighChartData, bs.LowChartData, bs.BandLineColor, bs.FillColor, bs.Name);
                }
                foreach (var lc in ch.LevelsCollection)
                    foreach(var l in lc)
                    {
                        Mark m = ch.ChartXY.yAxis().addMark(l.LevelValue, l.LevelColor, l.LevelText + ": " + l.LevelValue.ToString("N2"));
                        m.setAlignment(ChartDirector.Chart.Left);
                        m.setBackground(l.LevelBackGroundColor);
                    }

            }

            foreach (var ls in LineSeriesCollection)
            {
                c.addLineIndicator2(_mainChart, ls.LineChartData, ls.Color, ls.Name);
            }
            foreach (var bs in BandSeriesCollection)
            {
                c.addLineIndicator2(_mainChart, bs.LineChartData, bs.LineColor,bs.Name+".Line");
                // c.addBand(bs.HighChartData, bs.LowChartData, 0x9999ff, unchecked((int)0xc06666ff), bs.Name);
                c.addBand(bs.HighChartData, bs.LowChartData, bs.BandLineColor, bs.FillColor, bs.Name);

            }
            foreach ( var levels in LevelSourceCollection)
            {
                var ls = levels.GetLevelCollection(_selectKey);
                if (ls == null) continue;

                foreach( var l in ls)
                {
                    //var lev =  l;
                    Mark m = _mainChart.yAxis().addMark(l.LevelValue, l.LevelColor, l.LevelText + ": " + l.LevelValue.ToString("N2") );
                    m.setAlignment(ChartDirector.Chart.Left);
                    //m.setBackground(0xffcccc);
                    m.setBackground(l.LevelBackGroundColor);
                }
            }
            foreach ( var lineXYs in LineXYSourceCollection)
            {
                var lxys = lineXYs.GetLineXYCollection(_selectKey);
                foreach( var lxy in lxys)
                {
                    int i = 0, j = 0;
                    foreach (var t in _timeStamps)
                    {
                        if (lxy.LineX1 < t && t != DateTime.MinValue) break;
                        i++;
                    }
                    foreach (var t in _timeStamps)
                    {
                        if (lxy.LineX2 < t && t != DateTime.MinValue ) break;
                        j++;
                    }

                    if (j <= 0 || i <= 0) continue;

                    var ay = new[] { lxy.LineY1, lxy.LineY2 };
                    LineLayer layer1 = _mainChart.addLineLayer(ay, lxy.LineColor);
                    layer1.setXData(i, j);
                    layer1.setLineWidth(2);
                }
            }

            // Add a 75 pixels volume bars sub-chart to the bottom of the main chart,
            // using green/red/grey for up/down/flat days
            c.addVolBars((int) (_yChartArea*1.0*0.15), 0x99ff99, 0xff9999, 0x808080);

            // Append a MACD(26, 12) indicator chart (75 pixels high) after the main
            // chart, using 9 days for computing divergence.
            // c.addMACD(75, 26, 12, 9, 0x0000ff, 0xff00ff, 0x008000);

            // Output the chart
            viewer.Image = c.makeImage();
        }
        public void DrawOnly(WinChartViewer viewer)
        {
            //viewer.Image = c.makeImage();
        }

        public void FilRandom()
        {
            var ticker = "AsdZxc";
            var startDate = DateTime.Now.AddDays(-3);
            var endDate = DateTime.Now;

            FinanceSimulator db = new FinanceSimulator(ticker, startDate, endDate, resolution);

            _timeStamps = db.getTimeStamps();
            _highData = db.getHighData();
            _lowData = db.getLowData();
            _openData = db.getOpenData();
            _closeData = db.getCloseData();
            _volData = db.getVolData();
        }
        public void RefreshView()
        {
            _tickCount = 0;

            _barsToObserver = 0;
            SetBarsToObserverNumber();

            FillFromBars();
            _winChart.updateViewPort(true, false);
        }
        public void setKey( string key )
        {
            _selectKey = key;
        }
        public void SetBarSeries( IBars bs )
        {
            if (bs == null) return;

            _bars = bs;
            _tickCount = 0;
            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Chart", "Chart", "BarSeriesChanged", bs.ToString(), "");
            /*
            else
            {
                throw new NullReferenceException("BarSeries is Null");
            }
            */
        }
      
        public void ClearSeries()
        {
            LineSeriesCollection.Clear();
            BandSeriesCollection.Clear();
            LevelSourceCollection.Clear();
            LineXYSourceCollection.Clear();
        }
        public void CreateChartContainer( IChartDataContainer chdc)
        {
            if (chdc == null) return;
            if ( _chartContainer == null) _chartContainer = new ChartContainer();
            _chartContainer.ClearAll();
            foreach(var ch in chdc.Charts)
            {
                var myCh = new Chart {Name = ch.Name, Height = ch.HeightReal};
                foreach(var bs in ch.ChartBars)
                {
                    var cbs = new ChartBarSeries
                                  {
                                      Name = bs.Name,
                                      ColorUp = bs.ColorUp,
                                      ColorDown = bs.ColorDown,
                                      ColorEdge = bs.ColorEdge,

                                      BarSeries = bs
                                  };
                    cbs.SetChartDataLength(_barsToObserver);
                    myCh.BarSeriesCollection.Add(cbs);
                }
                foreach(var ils in ch.ChartLines)
                {
                    var ls = new LineSeries(ils);
                    ls.SetChartDataLength(_barsToObserver);
                    myCh.LineSeriesCollection.Add(ls);
                }
                foreach (var ibs in ch.ChartBands)
                {
                    var bs = new BandSeries(ibs) { Name = ibs.BandName, LineColor = ibs.BandLineColor,
                                                   BandLineColor = ibs.BandColor, FillColor = ibs.BandFillColor };
                    bs.SetChartDataLength(_barsToObserver);
                    myCh.BandSeriesCollection.Add(bs);
                }
                myCh.LevelsCollection = ch.ChartLevels;
                myCh.LevelSourceCollection = ch.ChartLevels2;
                myCh.TextsCollection = ch.ChartTexts;
                myCh.LineXYSourceCollection = ch.ChartLineXYs;

                _chartContainer.Charts.Add(myCh);
            }
        }

        public void AddLineSeries(ILineSeries ils)
        {
            if (ils == null) return;

            var ls = new LineSeries(ils);
            ls.SetChartDataLength(_barsToObserver);
            LineSeriesCollection.Add(ls);
        }
        public void AddLineSeries(IList<ILineSeries> lines)
        {
            if (lines == null) return;
            foreach (var b in lines)
                AddLineSeries(b);
        }

        public void AddBandSeries(IBandSeries ibs)
        {
            if (ibs == null) return;

            var s = new BandSeries(ibs){Name = ibs.BandName, LineColor = ibs.BandLineColor, BandLineColor = ibs.BandColor, FillColor = ibs.BandFillColor};
            s.SetChartDataLength(_barsToObserver);
            BandSeriesCollection.Add(s);
        }
        public void AddBandSeries(IList<IBandSeries> bands)
        {
            if( bands == null ) return;
            foreach(var b in bands)
                AddBandSeries(b);
        }

        public void AddBand2Series(IBars barser, IBandSeries ibandser)
        {
            if (barser == null || ibandser == null) return;

            var s = new Band2Series(barser, ibandser);
            s.SetChartDataLength(_barsToObserver);
            BandSeriesCollection.Add(s);
        }
        public void AddLevelSeries(ILevelCollection iLevelSource)
        {
            if (iLevelSource == null) return;
            LevelSourceCollection.Add(iLevelSource);         
        }
        public void AddLineXYSeries(ILineXYCollection lineXyCollection)
        {
            if (lineXyCollection == null) return;
            LineXYSourceCollection.Add(lineXyCollection);
        }
       
        public void SetBarWidth(double w)
        {
            _barWidth = w;
        }
        /// <summary>
        /// Utility to shift a double value into an array
        /// </summary>
        static private void ShiftData(IList<double> data, double newValue)
        {
            for (int i = 1; i < data.Count; ++i)
                data[i - 1] = data[i];
            data[data.Count - 1] = newValue;
        }

        /// <summary>
        /// Utility to shift a DataTime value into an array
        /// </summary>
        static private void ShiftData(IList<DateTime> data, DateTime newValue)
        {
            for (int i = 1; i < data.Count; ++i)
                data[i - 1] = data[i];
            data[data.Count - 1] = newValue;
        }
        public void Update(DateTime dt)
        {
            FillFromBars();
            //CreateChart(_winChart, "");
            _winChart.updateViewPort(true,false);
        }
        /*
        public void Update(object o, ElapsedEventArgs e)
        {
            FillFromBars();
            //CreateChart(_winChart, "");
            _winChart.updateViewPort(true, false);
        }
        */
        public void Update(object o, EventArgs e)
        {
         //   if (_bars == null) return;
         //   if (_bars.TickCount <= _tickCount) return;
         //   _tickCount = _bars.TickCount;

            TickRefreshEventHandler();

            //FillFromBars();
            //CreateChart(_winChart, "");
            //_winChart.updateViewPort(true, false);
        }
        private void FillFromBars()
        {
            /*
            int d;
            int length;

            if (_bars == null) return;
            if (_timeStamps == null) return;
            if (_bars.Count == 0) return;

            var l = _timeStamps.Length - 1;
            
                length = Math.Min(_bars.Count, _timeStamps.Length);
                for (var i = 0; i < length; i++)
                {
                    _timeStamps[l - i] = _bars.Bar(i).DT;
                    _openData[l - i] = _bars.Bar(i).Open;
                    _highData[l - i] = _bars.Bar(i).High;
                    _lowData[l - i] = _bars.Bar(i).Low;
                    _closeData[l - i] = _bars.Bar(i).Close;
                    _volData[l - i] = _bars.Bar(i).Volume;
                }
                d = _timeStamps.Length - _bars.Count;
                if (d > 0)
                {
                    for (var i = 0; i < d; i++)
                    {
                        _timeStamps[i] = DateTime.MinValue;
                        _openData[i] = ChartDirector.Chart.NoValue;
                        _highData[i] = ChartDirector.Chart.NoValue;
                        _lowData[i] = ChartDirector.Chart.NoValue;
                        _closeData[i] = ChartDirector.Chart.NoValue;
                        _volData[i] = ChartDirector.Chart.NoValue;
                    }
                }

            foreach(var ls in LineSeriesCollection) ls.FillData();
            foreach(var bs in BandSeriesCollection) bs.FillData();
            */
            foreach (var ch in _chartContainer.Charts)
            {
                foreach(var bs in ch.BarSeriesCollection) bs.FillData();
                foreach(var ls in ch.LineSeriesCollection) ls.FillData();
                foreach(var bnd in ch.BandSeriesCollection) bnd.FillData();
            }
        }
        /*
        private void ChartResize(object sender, EventArgs e)
        {   
            StopObserver();
            
            var needToReDraw = false;
            var width = ClientSize.Width;
            if (_xClientSize != width && (width - _leftMargin - _rightMargin) > 0)
            {
                SetBarsToObserverNumber();
                FillFromBars();
                _xClientSize = width;
                needToReDraw = true;
            }
            var y = ClientSize.Height - _topMargin - _bottomMargin;
            if (_yClientSize != y && y > 0)
            {
                _yClientSize = y;
                needToReDraw = true;
            }
            //_winChart.updateViewPort(true, false);
            if( needToReDraw ) CreateChart(_winChart, "");

            StartObserver();
        }
         */ 
        private void ChartResizeBegin(object sender, EventArgs e)
        {
            //this.SuspendLayout();
        }
        private void ChartResizeEnd(object sender, EventArgs e)
        {
            //this.ResumeLayout();
        }
        private void SetBarsToObserverNumber()
        {
            var n = _barsToObserver;
            _barsToObserver = (int)Math.Ceiling(_xChartArea / _barWidth);
            if (_barsToObserver <= 0) _barsToObserver = 1;
            
            if (n == _barsToObserver) return;

            _timeStamps = new DateTime[_barsToObserver];
            _highData = new double[_barsToObserver];
            _lowData = new double[_barsToObserver];
            _openData = new double[_barsToObserver];
            _closeData = new double[_barsToObserver];
            _volData = new double[_barsToObserver];
            
            foreach (var ls in LineSeriesCollection)
            {
                ls.SetChartDataLength(_barsToObserver);
            }
            foreach (var bs in BandSeriesCollection)
            {
                bs.SetChartDataLength(_barsToObserver);
            }
            foreach(var ch in _chartContainer.Charts)
            {
                foreach (var bs in ch.BarSeriesCollection) bs.SetChartDataLength(_barsToObserver);
                foreach (var ls in ch.LineSeriesCollection) ls.SetChartDataLength(_barsToObserver);
                foreach (var bnd in ch.BandSeriesCollection) bnd.SetChartDataLength(_barsToObserver);
            }

        }
        public void StopObserver()
        {   /*
            _previousTimerStatus = _timerToObserve.Enabled ? +1 : -1;
            if( _timerToObserve.Enabled)
                _timerToObserve.Stop();
            */ 
        }
        public void StartObserver()
        {
            /*
            if (_previousTimerStatus < 0) return;

            _previousTimerStatus = 0;
            if (!_timerToObserve.Enabled)
                _timerToObserve.Start();
            */ 
        }

        private void ViewPortChangedEventHandler(object sender, WinViewPortEventArgs e)
        {
            CreateChart(_winChart, "");
        }
        /*
        private void ChartClosedEventHandler(object sender, FormClosedEventArgs e)
        {
            _timerToObserve.Stop();
            _timerToObserve.Tick -= Update;
            //_timerToObserve.Close();
            _timerToObserve.Dispose();
        }
        */

        public InterLineLayer addBand2(XYChart c, double[] upperLine, double[] lowerLine, int lineColor,
            int fillColor, string name)
        {
            LineLayer uLayer = c.addLineLayer(upperLine, lineColor, name);
            LineLayer lLayer = c.addLineLayer(lowerLine, lineColor);
            return c.addInterLineLayer(uLayer.getLine(), lLayer.getLine(), fillColor);
        }
        public CandleStickLayer addCandleStick2(XYChart c, int upColor, int downColor, double [] op, double [] hi, double [] lo, double [] cl)
        {
            CandleStickLayer ret = c.addCandleStickLayer(hi, lo, op, cl, upColor, downColor);
            if (_highData.Length - _extraPoints > 60)
            {
                ret.setDataGap(0);
            }
            /*
            if (_highData.Length > _extraPoints)
            {
                int expectedWidth = (_totalWidth - m_leftMargin - m_rightMargin) / (
                    _highData.Length - _extraPoints);
                if (expectedWidth <= 5)
                {
                    ret.setDataWidth(expectedWidth + 1 - expectedWidth % 2);
                }
            }
            */
            return ret;
        }
    }
}
