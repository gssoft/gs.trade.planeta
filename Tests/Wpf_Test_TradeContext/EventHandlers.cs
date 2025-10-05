using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using GS.Interfaces;

namespace Wpf_Test_TradeContext
{
    public partial class MainWindow : Window
    {
        private void WindowClosing(object sender, CancelEventArgs e)
        {
            var result = MessageBox.Show("Are you sure to Exit?", "GS.Trading Tools", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel)
                e.Cancel = true;
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            _tx.Stop();
            //Thread.Sleep(5000);
            
            _tx.Close();

            ((IEventLogs)_evl)?.Stop();
        }

        private void ToggleButtonClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void BtCloseAppClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CloseStrategies(object sender, RoutedEventArgs e)
        {
            _tx?.CloseStrategies();
        }
        private void CloseSoftStrategies(object sender, RoutedEventArgs e)
        {
            _tx?.GetStrategies?.CloseAllSoft();
        }

        private void BtStartClick(object sender, RoutedEventArgs e)
        {
            _tx?.EnableEntries();
        }

        private void BtStopClick(object sender, RoutedEventArgs e)
        {
            _tx?.DisableEntries();
        }


        private void BtSuspendClick(object sender, RoutedEventArgs e)
        {
            _tx?.SetWorkingStatus(false);
        }

        private void BtResumeClick(object sender, RoutedEventArgs e)
        {
            _tx?.SetWorkingStatus(true);
        }

        private bool _strategiesWorkingStatus = true;

        private void TglBtnStrWorkingStatusClick(object sender, RoutedEventArgs e)
        {
            var b = (ToggleButton)sender;

            if (b.IsChecked == true)
            {
                b.Content = "Strategies Disabled Now";
                _strategiesWorkingStatus = false;
                
            }
            else if (b.IsChecked == false)
            {
                b.Content = "Strategies Enabled Now";
                _strategiesWorkingStatus = true;
            }

            _tx?.SetWorkingStatus(_strategiesWorkingStatus);
            _evl?.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                   "MainWindow", "Strategies", $"SetWorkingStatus to: {_strategiesWorkingStatus}", "", "");
        }
        private void ClearOrders(object sender, RoutedEventArgs e)
        {
            _tx?.ClearOrderCollection();
        }

        //private bool _longEnabled = true, _shortEnabled = true; 
        //private void BtLongEnabledClick(object sender, RoutedEventArgs e)
        //{
        //    var b = (CheckBox) sender;
        //    _longEnabled = b.IsChecked == true;
        //}

        //private void BtShortEnabledClick(object sender, RoutedEventArgs e)
        //{
        //    var b = (CheckBox)sender;
        //    _shortEnabled = b.IsChecked == true;
        //}
        private void BtLongShortEnabledClick(object sender, RoutedEventArgs e)
        {
            _tx?.EventLog?
                .Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, "MainWindow", "PortfolioSettings", "BtLongShortEnbledClick()",
                             _portfolioSettings?.ToString(),"");

            if (_portfolioSettings == null) return;

            _tx?.SetStrategiesLongShortEnabled(_portfolioSettings.LongEnabled, _portfolioSettings.ShortEnabled);
            _tx?.SetPortfolioMaxSideSize(_portfolioSettings.MaxSideSize);
        }
    }
}
