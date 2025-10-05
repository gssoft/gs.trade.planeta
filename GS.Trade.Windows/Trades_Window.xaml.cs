using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GS.Interfaces;
using GS.Trade.Interfaces;
using GS.Process;
using GS.Trade.Trades;

namespace GS.Trade.Windows
{
    using Trades;
    /// <summary>
    /// Interaction logic for TradesWindow.xaml
    /// </summary>
    public partial class TradesWindow : Window
    {
        private IEventLog _evl;
        private ITrades _trades;

        private SimpleProcess _observeProcess;

        public TradesWindow()
        {
            InitializeComponent();
        }
        private void CheckNullReference()
        {
            if (_evl == null || _trades == null)
            {
                throw new NullReferenceException("TradesWindow(EventLog = null or Trades == null)");
            }
        }
        public void Init(IEventLog eventlog, ITrades trades)
        {
            _evl = eventlog;
            _trades = trades;

            CheckNullReference();

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "TradesWindow", "TradesWindow", "Initialization", "", "");
            //_observeProcess = new SimpleProcess("Trades Observer Process", 5, 1, CallbackGetTradesToObserve, _evl);
            _trades.NeedToObserverEvent += CallbackGetTradesToObserve;
        }           
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckNullReference();

            //lstTrades.ItemsSource = _trades.TradeObserveCollection; // Need to Restore
           // _observeProcess.Start();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
           // _observeProcess.Stop();
            if(_trades != null)
                _trades.NeedToObserverEvent -= CallbackGetTradesToObserve;

        }
        private void CallbackGetTradesToObserve()
        {
            //Dispatcher.BeginInvoke((ThreadStart)(() => _trades.GetTradesToObserve()));
        }

    }
}
