using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using GS.EventLog;
using GS.Extension;
using GS.Interfaces;
using GS.Serialization;
using GS.Trade;
using GS.Trade.BackTest;
using GS.Trade.Data;
using GS.Trade.Data.Bars;
using GS.Trade.Strategies;
using GS.Trade.TradeContext;
using GS.Trade.Windows;
using GS.Trade.Windows.Charts;

namespace BackTest01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TradeContext52 _tx;
        private IEventLogs _evl;

        private Tickers _myTickers;
        private Ticker _myTicker;
        private ITimeSeries _myBarSeries;

        private IEnumerable<IStrategy> _allStrategies;
        private readonly List<Strategy> _myStrategies = new List<Strategy>();
        private Strategy _myStrategy;
        
        private BackTest5 _backTest;
       // public long TradesNumber { get; set; }

        public ChartWindow Chart;

        public int TempChartObserverInterval { get; set; }
        public int ChartObserverInterval { get; set; }
        public int LoopDelayMilliSeconds { get; set; }

        protected EventLogWindow2 EventLogWindow;

        public MainWindow()
        {
            InitializeComponent();
            //_evl = new EventLog();
            //_evl = Builder.Build<EventLogs>(@"Init\EventLog.xml", "EventLogs");
            //_evl.Init();
            //EventLogWindow = new EventLogWindow2();
            //EventLogWindow.Init(_evl);
            //EventLogWindow.Show();

            //_tx = new TradeContext2 { EventLog = _evl };
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {

            _evl = Builder.Build<EventLogs>(@"Init\EventLog.xml", "EventLogs");
            _evl.Init();

            //_evl.SetMode(EvlModeEnum.Init);

            _tx = Builder.Build<TradeContext52>(@"Init\TradeContext.xml", "TradeContext52");

            _tx.EventLog = _evl;
            try
            {
                _tx.Init();
                _tx.Open();
            }
            catch (Exception ex)
            { 
                throw new NullReferenceException(ex.Message);
            }
            

            _tx.TimePlans.Start();
            //_tx.Open();
            //_tx.Start();
            //_tx.OpenChart();
            //_tx.UIProcess.Start();
            Chart = _tx.ChartWindow;
            
            _allStrategies = _tx.StrategyCollection;
            ComboBoxTickersCollection.ItemsSource = _tx.TickerCollection;

            //var modeList = new List<KeyValuePair<TestModeEnum, string>>
            //{
            //    new KeyValuePair<TestModeEnum, string>(TestModeEnum.Random, "Random"),
            //    new KeyValuePair<TestModeEnum, string>(TestModeEnum.Real, "Real")
            //};

            //ComboBoxTestModes.ItemsSource = modeList;

            ComboBoxTestModes.ItemsSource = EnumHelper.EnumToKeyValuePairList<TestModeEnum>();
            ComboBoxExecutionBarModes.ItemsSource = EnumHelper.EnumToKeyValuePairList<ExecutionBarModeEnum>();

            _backTest = new BackTest5{ TradesNumber = 500000, StartValue = 500000 };
            _backTest.Init(_tx );
            _backTest.BarTickMode = BarTickModeEnum.Bar;
            _backTest.TestMode = TestModeEnum.Random;

            _backTest.FinishEvent += (o, time) => FinishButtonFalse(time);

            //var lst = _backTest.ExecutionBarMode.ToKeyValuePairList();

            TradesToTest.DataContext = _backTest;
            RandomStartValue.DataContext = _backTest;
            LoopDelayCtrl.DataContext = _backTest;
            SpreadInMinMovesTextBox.DataContext = _backTest;

            ChartObserverInterval = 1000;
            TempChartObserverInterval = ChartObserverInterval;
            ChartObserverInt.DataContext = this;

            _backTest.NextPass += UpdateChart;          
            
            _tx.ShowWindows();
            Chart.Show();
        }
       
        public void UpdateChart()
        {
            if (ChartObserverInterval == 0)
                return;

          //  if (_myStrategy == null /*|| _myStrategy.Bars == null*/) return;
            //if (_myStrategy.Bars.Count < 100) return;

            //Dispatcher.BeginInvoke((ThreadStart) (() => Chart.Update(null, null)));

            Chart.Update(null,null);
           
            _backTest.UpdatePositionFromtickerInChartUpdate();
            //15.09.24 Portfolio Update
            _tx.Portfolios.Refresh();

            //Thread.Sleep(ChartObserverInterval * 1000);
            Thread.Sleep(ChartObserverInterval);
        }
        private void UpdateChartSettings()
        {
            /*
            var temp = ChartObserverInterval;
            ChartObserverInterval = 0;

            if (Chart == null)
            {
                Chart = new ChartForm();
                Chart.Init(_myStrategy.Bars, _tx.EventLog);
                Chart.AddBandSeries(_myStrategy.Band);
                Chart.setKey(_myStrategy.TradeKey);
                Chart.AddLevelSeries(_myStrategy.Levels);
                // Chart.AddLevelSeries(_myStrategy.Levels2);
                //Chart.AddLevelSeries(_xBand);
                Chart.AddLevelSeries(_tx.Orders);
                Chart.AddLevelSeries(_tx.Positions);
                Chart.AddLineXYSeries(_tx.Positions);
                Chart.Show();
            }
            else
            {
                Chart.SetBarSeries(_myBarSeries);
                Chart.ClearSeries();
                Chart.setKey(_myStrategy.TradeKey);
                Chart.AddBandSeries(_myStrategy.Band);
                Chart.AddBand2Series(_myBarSeries, _myStrategy.Band2);
                Chart.AddLevelSeries(_myStrategy.Levels);
                Chart.AddLevelSeries(_tx.Orders);
                Chart.AddLevelSeries(_tx.Positions);
                Chart.AddLineXYSeries(_tx.Positions);
                Chart.RefreshView();
            }

            ChartObserverInterval = temp;
             */
        }

        //private void ChartObserverInt_OnTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    int i;
        //    var str = ((TextBox)sender).Text;
        //    if (Int32.TryParse(str, out i))
        //        _tempChartObserverInterval = i;
        //    else
        //        return;

        //    //if (ChartObserverInterval == 0)
        //    //{
        //    //    if (_backTest != null)
        //    //        _backTest.RefreshUI = false;
        //    //    return;
        //    //}
        //    //if (_backTest != null)
        //    //    _backTest.RefreshUI = true;

        //}

        private void BtAppLyClick(object sender, RoutedEventArgs e)
        {
            ChartObserverInterval = TempChartObserverInterval;
            if (ChartObserverInterval == 0)
            {
                if (_backTest != null)
                    _backTest.RefreshUI = false;
                return;
            }
            if (_backTest != null)
                _backTest.RefreshUI = true;

            //if (BtStartStop.IsChecked == true)
            //    BtStartStop.IsChecked = false;
            //else if(BtStartStop.IsChecked == false)
            //    BtStartStop.IsChecked = true;
        }
        private void BtSetDefaultClick(object sender, RoutedEventArgs e)
        {
            ChartObserverInterval = 1000;
            TempChartObserverInterval = 1000;
            ChartObserverInt.Text = TempChartObserverInterval.ToString(CultureInfo.InvariantCulture);

            _backTest.LoopDelay = 1000;
            LoopDelayCtrl.Text = _backTest.LoopDelay.ToString(CultureInfo.InvariantCulture);

        }

        private void BtClearClick(object sender, RoutedEventArgs e)
        {
            ChartObserverInterval = 0;
            TempChartObserverInterval = 0;

            ChartObserverInt.Text = TempChartObserverInterval.ToString(CultureInfo.InvariantCulture);

            _backTest.LoopDelay = 0;
            LoopDelayCtrl.Text = _backTest.LoopDelay.ToString(CultureInfo.InvariantCulture);

           
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            if (_backTest == null)
                return;
            _backTest.AllHaveSameTradesNumber = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            _backTest.AllHaveSameTradesNumber = false;
        }

        private void FinishButtonFalse(DateTime time)
        {
            Dispatcher.BeginInvoke((ThreadStart) (() =>
            {
                BtStartStop.IsChecked = false;
                BtStartStop.Content = "Start Tests";
            }));
            _tx.Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Back Test is Finished", time.ToString("s"), "", "", "");
        }
        private void FinishButtonTrue(DateTime time)
        {
            Dispatcher.BeginInvoke((ThreadStart)(() =>
            {
                BtStartStop.IsChecked = true;
                BtStartStop.Content = "Stop Tests";
            }));
            _tx.Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Back Test is Starteded", time.ToString("s"), "", "", "");
        }

        private void BtClearAllClick(object sender, RoutedEventArgs e)
        {
            if (_backTest == null)
                return;
            _backTest.ClearAll();
        }
    }
}