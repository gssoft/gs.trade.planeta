using System;
using System.Windows;
using ChartDirector;
// using GS.Trade.Data;
using GS.Trade.Interfaces;

namespace GS.Trade.Windows.Charts
{
    /// <summary>
    /// Interaction logic for ChartDirector.xaml
    /// </summary>
    public partial class ChartWindow
    {
        private ITradeContext _tx;
        
        private readonly WinChartViewer _winChart;

        private int _x;
        private int _y;

        private int _y1;
        private int _y2;
        private int _y3;
        private int _y4;

        private int _leftMargin = 0;
        private int _topMargin = 0;
        private int _rightMargin = 42;
        private int _bottomMargin = 30;

        private int _xChartSize;
        private int _yChartSize;

        public ChartWindow()
        {
            InitializeComponent();
            _winChart = new WinChartViewer { ChartSizeMode = WinChartSizeMode.AutoSize };
            this.windowsFormsHost1.Child = _winChart;
            _chartContainer = new ChartContainer();
        }
        public void Init(ITradeContext tradecontext)
        {
            _tx = tradecontext;
            _evl = _tx.EventLog;
            _tickers = _tx.GetTickers; // as ITickers;
            // _allStrategies = _tx.GetStrategies as Strategies.Strategies;
            _allStrategies = _tx.GetStrategies as IStrategies;
           // _winChart.ViewPortChanged += new ChartDirector.WinViewPortEventHandler(this.ViewPortChangedEventHandler);
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (_tickers != null) ComboBoxTickersCollection.ItemsSource = _tickers.TickerCollection;

            //CreateFinanceChart(_winChart, string.Empty);

            for (var i = 0; i < _timeStamps.Length; ++i)
                _timeStamps[i] = DateTime.MinValue;

            ChangeChartSize();
        }
        private void ChangeChartSize()
        {
            var w = (int)(ActualWidth - Settings.ActualWidth);
            var h = (int) ActualHeight;

            var needToReDraw = false;

            if( _xClientSize != w && (w - _leftMargin - _rightMargin) > 0 )
            {
                _xChartArea = w - _leftMargin - _rightMargin;
                SetBarsToObserverNumber();
                FillFromBars();

                _xClientSize = w;
                needToReDraw = true;
            }
            if( _yClientSize != h && (h - _topMargin - _bottomMargin) > 0)
            {
                _yClientSize = h;
                _yChartArea = h - _topMargin - _bottomMargin;
                needToReDraw = true;
            }
            if (needToReDraw) CreateChart(_winChart, "");
        }

        private void ChangeFinanceChartSize()
        {
            var h1 = this.ActualHeight;
            var w1 = this.ActualWidth;

            var exh = Settings.ActualHeight;
            var exW = Settings.ActualWidth;

            var h3 = windowsFormsHost1.ActualHeight;

            _x = (int)(w1 - exW);
            _y = (int)(h3 - 30);

            // _y -= h1 - h3;
            _y1 = (int)(_y * 0.20);
            _y2 = (int)(_y * 0.60);
            _y3 = (int)(_y * 0.15);
            _y4 = (int)(_y * 0.20);

            CreateFinanceChart(_winChart, string.Empty);
        }
        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
           // ChangeFinanceChartSize();
            ChangeChartSize();
        }
        private void ExpSizeChanged(object sender, SizeChangedEventArgs e)
        {
          //  ChangeFinanceChartSize();
            ChangeChartSize();
        }
        private void CreateFinanceChart(WinChartViewer viewer, string img)
        {
            // Create a finance chart demo containing 100 days of data
            int noOfDays = 100;

            // To compute moving averages starting from the first day, we need to get
            // extra data points before the first day
            int extraDays = 30;

            // In this exammple, we use a random number generator utility to simulate
            // the data. We set up the random table to create 6 cols x (noOfDays +
            // extraDays) rows, using 9 as the seed.
            RanTable rantable = new RanTable(9, 6, noOfDays + extraDays);

            // Set the 1st col to be the timeStamp, starting from Sep 4, 2002, with
            // each row representing one day, and counting week days only (jump over
            // Sat and Sun)
            rantable.setDateCol(0, new DateTime(2002, 9, 4), 86400, true);

            // Set the 2nd, 3rd, 4th and 5th columns to be high, low, open and close
            // data. The open value starts from 100, and the daily change is random
            // from -5 to 5.
            rantable.setHLOCCols(1, 100, -5, 5);

            // Set the 6th column as the vol data from 5 to 25 million
            rantable.setCol(5, 50000000, 250000000);

            // Now we read the data from the table into arrays
            double[] timeStamps = rantable.getCol(0);
            double[] highData = rantable.getCol(1);
            double[] lowData = rantable.getCol(2);
            double[] openData = rantable.getCol(3);
            double[] closeData = rantable.getCol(4);
            double[] volData = rantable.getCol(5);

            // Create a FinanceChart object of width 640 pixels
            FinanceChart c = new FinanceChart(_x);

            // Add a title to the chart
            //  c.addTitle("Finance Chart Demonstration");

            // Set the data into the finance chart object
            c.setData(timeStamps, highData, lowData, openData, closeData, volData,
                extraDays);

            c.setMargins(_leftMargin, _topMargin, _rightMargin, _bottomMargin);

            // Add a slow stochastic chart (75 pixels high) with %K = 14 and %D = 3
            c.addSlowStochastic(_y1, 14, 3, 0x006060, 0x606000);

            // Add the main chart with 240 pixels in height
            c.addMainChart(_y2);

            // Add a 10 period simple moving average to the main chart, using brown
            // color
            c.addSimpleMovingAvg(10, 0x663300);

            // Add a 20 period simple moving average to the main chart, using purple
            // color
            c.addSimpleMovingAvg(20, 0x9900ff);

            // Add an HLOC symbols to the main chart, using green/red for up/down
            // days
            c.addCandleStick(0x00ff00, 0xff0000);

            // Add 20 days donchian channel to the main chart, using light blue
            // (9999ff) as the border and semi-transparent blue (c06666ff) as the
            // fill color
            c.addDonchianChannel(20, 0x9999ff, unchecked((int)0xc06666ff));

            // Add a 75 pixels volume bars sub-chart to the bottom of the main chart,
            // using green/red/grey for up/down/flat days
            c.addVolBars(_y3, 0x99ff99, 0xff9999, 0x808080);

            // Append a MACD(26, 12) indicator chart (75 pixels high) after the main
            // chart, using 9 days for computing divergence.
            c.addMACD(_y4, 26, 12, 9, 0x0000ff, 0xff00ff, 0x008000);

            // Output the chart
            viewer.Image = c.makeImage();
        }
    }
}

