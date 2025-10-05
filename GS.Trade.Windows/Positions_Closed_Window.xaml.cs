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
    /// <summary>
    /// Interaction logic for PositionClose_Window.xaml
    /// </summary>
    public partial class PositionsClosedWindow : Window
    {
        //Window The_MainWindow = null;
        private IEventLog _evl;
        private IPositions _positions;

        private SimpleProcess _observeProcess;

        public PositionsClosedWindow()
        {
            InitializeComponent();
        }
        private void CheckNullReference()
        {
            if (_evl == null || _positions == null)
            {
                throw new NullReferenceException("PositionsCLosedWindow(EventLog = null or Positions == null)");
            }
        }
        public void Init(IEventLog eventlog, IPositions positions)
        {         
            _evl = eventlog;
            _positions = positions;

            CheckNullReference();

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "PositionsClosedWindow", "PositionsClosedWindow", "Initialization", "", "");
            //_observeProcess = new SimpleProcess("PositionsClosed Observer Process", 5, 3, CallbackGetPositionsClosedToObserve, _evl);
        }   
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckNullReference();

            //lstPositionsClosed.ItemsSource = _positions.PositionClosedObserveCollection;
            //_positions.NeedToObserverEvent += CallbackGetPositionsClosedToObserve;

            //_observeProcess.Start();
            

        }
        private void WindowClosed(object sender, EventArgs e)
        {
            //_observeProcess.Stop();
            //if (_positions != null) _positions.NeedToObserverEvent -= CallbackGetPositionsClosedToObserve;
        }
        private void CallbackGetPositionsClosedToObserve()
        {
            //Dispatcher.BeginInvoke((ThreadStart)(() => _positions.GetPositionsClosedToObserve()));
        }
    }
}
