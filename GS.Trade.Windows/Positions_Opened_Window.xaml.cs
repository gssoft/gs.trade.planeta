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
// using GS.Trade.Trades;

namespace GS.Trade.Windows
{
    
    /// <summary>
    /// Interaction logic for Positions_Window.xaml
    /// </summary>
    public partial class PositionsOpenedWindow : Window
    {
        //Window The_MainWindow = null;
        private ITradeContext _tx;
        private IEventLog _evl;
        private IPositions _positions;

        private SimpleProcess _observeProcess;

        public PositionsOpenedWindow()
        {
            InitializeComponent();
        }
        private void CheckNullReference()
        {
            if (_evl == null || _positions == null)
            {
                throw new NullReferenceException("PositionsOpenedWindow(EventLog = null or Positions == null)");
            }
        }
        public void Init(ITradeContext tx, IEventLog eventlog, IPositions positions)
        {
            _tx = tx;
            _evl = eventlog;
            _positions = positions;

            CheckNullReference();

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "PositionsOpenedWindow", "PositionsOpenedWindow", "Initialization", "", "");
            _tx.UIProcess.RegisterProcess("PositionsOpened Observer Process", 5000, 3000, null, CallbackGetPositionsOpenedToObserve, null);
            //_observeProcess = new SimpleProcess("PositionsOpened Observer Process", 5, 3, CallbackGetPositionsOpenedToObserve, _evl);       
        }    
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckNullReference();

            //lstPositionsOpened.ItemsSource = _positions.PositionOpenedObserveCollection;
            if (_observeProcess != null) _observeProcess.Start();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            if (_observeProcess != null) _observeProcess.Stop();
        }
        private void CallbackGetPositionsOpenedToObserve()
        {
            Dispatcher.BeginInvoke((ThreadStart)(() => _positions.GetPositionsOpenedToObserve()));
        }
    }
}
