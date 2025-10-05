using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
// using GS.Trade.Data;
// using GS.Trade.Strategies;

namespace GS.Trade.Windows.Charts
{
    
    public partial class ChartWindow
    {
        private ITicker _ticker;
        private ITickers _tickers;

        private IBars _barSeries;
        private ITimeSeries _syncSeries;

        // 15.11.11
        // private Strategies.Strategies _allStrategies;
        private IStrategies _allStrategies;

        // private List<Strategy> _strategyCollection = new List<Strategy>();
        private List<IStrategy> _strategyCollection = new List<IStrategy>();
        // private Strategy _strategy;
        private IStrategy _strategy;

        private void TickerCollectionSelectChanged(object sender, SelectionChangedEventArgs e)
        {
            var ticker = (ITicker)(((ComboBox)sender).SelectedItem);
            if (ticker == null) return;

            _ticker = (ITicker)_tickers.GetTicker(ticker.Code);
            if (_ticker == null) return;

         //   ComboBoxBarSeriesCollection.ItemsSource = _ticker.BarSeriesCollection;
         //   ComboBoxBarSeriesCollection.SelectedIndex = 0;
            // 15.11.11
            // ComboBoxSyncSeriesCollection.ItemsSource = _ticker.AsyncSeriesCollection.Values.OrderBy(s=>s.TimeIntSeconds);
            ComboBoxSyncSeriesCollection.ItemsSource = _ticker.BarSeries.OrderBy(s => s.TimeIntSeconds);
            ComboBoxSyncSeriesCollection.SelectedIndex = 0;
        }
        /*
        private void BarSeriesCollectionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //_barSeries = null;
            var bs = (IBars)(((ComboBox)sender).SelectedItem);
            if (bs == null) return;

            _barSeries = bs;
            _bars = bs;

            _strategyCollection.Clear();
            _strategyCollection.AddRange(from Strategy s in _allStrategies.StrategyCollection
                                           where s.TickerKey == _ticker.Code && s.TimeInt == bs.TimeIntSeconds
                                           select s);

            ComboBoxStrategyCollection.ItemsSource = null;
            ComboBoxStrategyCollection.ItemsSource = _strategyCollection;
            ComboBoxStrategyCollection.SelectedIndex = 0;
        }
         */ 
        private void SyncSeriesCollectionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ts = (IBars)(((ComboBox)sender).SelectedItem);
            if (ts == null) return;

            //_syncSeries = ts;
            _barSeries = ts;
            _bars = ts;

            _strategyCollection.Clear();
            _strategyCollection.AddRange(from IStrategy s in _allStrategies.StrategyCollection
                                         where s.TickerKey == _ticker.Code && s.TimeInt == ts.TimeIntSeconds
                                         select s);

            try
            {
                ComboBoxStrategyCollection.ItemsSource = null;
                ComboBoxStrategyCollection.ItemsSource = _strategyCollection;
                ComboBoxStrategyCollection.SelectedIndex = 0;
            }
            catch(Exception ex)
            {
                throw new NullReferenceException(ex.Message);
            }
        }
        private void StrategyCollectionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //_strategy = null;
            var s = (IStrategy)(((ComboBox)sender).SelectedItem);
          //  if (s == null) return;

            _strategy = s;
            SetChartData();
        }

        private void SetChartData()
        {
            _tickCount = 0;
            /*
            if (_strategy == null)
            {
                ClearSeries();
                return;
            }
            */
         //   SetBarSeries(_barSeries);

            if (_strategy == null)
            {
             //   ClearSeries();
             //   FillFromBars();
             //   DrawChartAsync();
                return;
            }

            if (_strategy.Position != null)
            {
                Title = string.Format("Strategy: {0} Ticker: {1} {2}",
                                      _strategy.Code, _strategy.TickerKey, _strategy.Position.PositionInfo);
            }

           // setKey(_strategy.TradeKey);
           // ClearSeries();
            _tickCount = 0;

            CreateChartContainer(_strategy.ChartDataContainer);
            /*
            AddLineSeries(_strategy.LineSeries);
            AddLineSeries(_strategy.ChartLines);
            AddBandSeries(_strategy.Band);
            AddBandSeries(_strategy.Bands);
            AddBand2Series(_barSeries, _strategy.Band2);
            AddLevelSeries(_strategy.Levels);
            // Chart.AddLevelSeries(_myStrategy.Levels2);
            //  Chart.AddLevelSeries(_xBand);
            AddLevelSeries(_tx.Orders);
            AddLevelSeries(_tx.Positions);
            AddLineXYSeries(_tx.Positions);
            */
            FillFromBars();
            DrawChartAsync();
        }

        public void TickRefreshEventHandler()
        {
            //_tx.Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,"ChartWindow", "TickEvent",DateTime.Now.ToString("H:mm:ss.fff"),"");
            
            if (_bars == null) return;
            if (_bars.TickCount <= _tickCount) return;
            _tickCount = _bars.TickCount;
            
            FillFromBars();

            DrawChartAsync();
            //DrawChartOnly();
        }
        private void DrawChartAsync()
        {
            Dispatcher.BeginInvoke((ThreadStart)(() =>
            {
                if (WindowState == WindowState.Minimized) return;

                if (_strategy != null && _strategy.Position != null)
                {
                    Title =
                        $"Strategy: {_strategy.Code} Ticker: {_strategy.TickerKey} {_strategy.Position.PositionInfo} {_strategy.PositionInfo}";
                }
                CreateChart(_winChart, "");
            }));
        }
        private void DrawChartOnly()
        {
            Dispatcher.BeginInvoke((ThreadStart)(() =>
            {
                if (WindowState == WindowState.Minimized) return;
                DrawOnly(_winChart);
            }));
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            var result = MessageBox.Show("Are you sure to Exit?", "GS.Trading Tools", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }

       
    }

}
