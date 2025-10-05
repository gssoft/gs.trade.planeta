 using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
 using System.Windows.Controls.Primitives;
 using GS.Interfaces;
 using GS.Trade;
 using GS.Trade.BackTest;
 using GS.Trade.Data;
 using GS.Trade.Data.Bars;
 using GS.Trade.Strategies;

namespace BackTest01
{
    public partial class MainWindow : Window
    {
        private void TickerCollectionSelectChanged(object sender, SelectionChangedEventArgs e)
        {
            var ticker = (Ticker)(((ComboBox)sender).SelectedItem);
            if (ticker == null) return;

            _myTicker = ticker;

            // _myTicker = _myTickers.GetTicker(ticker.Code);

            // if (_myTicker == null) return;

            ComboBoxBarSeriesCollection.ItemsSource = null;
            ComboBoxBarSeriesCollection.ItemsSource = ticker.AsyncSeriesCollectionValues;
            ComboBoxBarSeriesCollection.SelectedIndex = 0;
        }
        private void BarSeriesCollectionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var bs = (ITimeSeries)(((ComboBox)sender).SelectedItem);
            var bs = (((ComboBox)sender).SelectedItem);
            if (bs == null) return;

            _myBarSeries = bs as ITimeSeries;
            _myStrategies.Clear();
            _myStrategies.AddRange(from Strategy str in _allStrategies
                                   where str.TickerKey == _myTicker.Code && str.TimeInt == _myBarSeries.TimeIntSeconds
                                           select str);

            ComboBoxStrategyCollection.ItemsSource = null;
            ComboBoxStrategyCollection.ItemsSource = _myStrategies;
            ComboBoxStrategyCollection.SelectedIndex = 0;
        }
        private void StrategyCollectionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var s = (Strategy)(((ComboBox)sender).SelectedItem);
            if (s == null) return;

            _myStrategy = s;
            UpdateChartSettings();
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            var result = MessageBox.Show("Are you sure to Exit?", "GS.Trading Tools", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel)
                e.Cancel = true;
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            _tx.Stop();
            _tx.Close();
        }

        private void ToggleButtonClick(object sender, RoutedEventArgs e)
        {
            var b = (ToggleButton)sender;
            if (b.IsChecked == true)
            {
                _evl.AddItem(EvlResult.SUCCESS, EvlSubject.DIAGNOSTIC, "Button Start", "Button Start", "Click", "Start", "");
                b.Content = "Stop Tests";
                _backTest.Start();
            }
            else if (b.IsChecked == false)
            {
                _evl.AddItem(EvlResult.SUCCESS, EvlSubject.DIAGNOSTIC, "Button Stop",
                    "Button Stop", "Click", "Stop", "");
                b.Content = "Start Tests";
                _backTest.Stop();
            }
        }
        private void BtBarClick(object sender, RoutedEventArgs e)
        {
            var b = (ToggleButton)sender;
            switch (b.IsChecked)
            {
                case true:
                    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.DIAGNOSTIC, "Button BarTickMode", "Button BarTickMode", "Click", "Every Tick", "");
                    b.Content = "Every Tick Now";
                    _backTest.BarTickMode = BarTickModeEnum.Tick;
                    break;
                case false:
                    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.DIAGNOSTIC, "Button BarTickMode", "Button BarTickMode", "Click", "Every Bar", "");
                    b.Content = "Every Bar Now";
                    _backTest.BarTickMode = BarTickModeEnum.Bar;
                    break;
            }
        }

        private void BtExecModeClick(object sender, RoutedEventArgs e)
        {
            var b = (ToggleButton)sender;
            switch (b.IsChecked)
            {
                case true:
                    b.Content = "Pessimistic Now";
                    _backTest.SetOrderExecMode(BackTestOrderExecutionMode.Pessimistic);
                    break;
                case false:
                    b.Content = "Optimistic Now";
                    _backTest.SetOrderExecMode(BackTestOrderExecutionMode.Optimistic);
                    break;
            }
        }

        private void TestModeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var mode = (((ComboBox)sender).SelectedItem);
            var k = ((KeyValuePair<TestModeEnum,string>)mode).Key;

            if (_backTest == null)
                return;

            _backTest.TestMode = k;

        }
        private void ExecutionBarModeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var mode = (((ComboBox)sender).SelectedItem);
            var k = ((KeyValuePair<ExecutionBarModeEnum, string>)mode).Key;

            if (_backTest == null)
                return;

            _backTest.ExecutionBarMode = k;
        }
    }
}