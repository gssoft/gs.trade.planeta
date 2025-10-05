using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for PositionTotals_Window.xaml
    /// </summary>
    public partial class PositionTotalsWindow : Window
    {
        private ITradeContext _tx;
        private IEventLog _evl;
        private Positions.Totals _positions;

        public ObservableCollection<Positions.PositionTotalNpc> PositionCollection;
        public List<Positions.Totals.PositionTotal> TempList;

        private object _lockPositions;
        private SimpleProcess _observeProcess;

        public PositionTotalsWindow()
        {
            InitializeComponent();

            _lockPositions = new object();
            PositionCollection = new ObservableCollection<Positions.PositionTotalNpc>();
            TempList = new List<Positions.Totals.PositionTotal>();
        }
        private void CheckNullReference()
        {
            if (_evl == null || _positions == null)
            {
                throw new NullReferenceException("PositionTotalsWindow(EventLog = null or Positions == null)");
            }
        }
        public void Init(ITradeContext tx, IEventLog eventlog, Positions.Totals positions)
        {
            _tx = tx;
            _evl = eventlog;
            _positions = positions;

            CheckNullReference();

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "PositionTotalsWindow", "PositionTotalsWindow", "Initialization", "", "");
            // _tx.UIProcess.RegisterProcess("PositionsOpened Observer Process", 5000, 4000, null, CallbackGetPositionTotalsToObserve, null);
            // _observeProcess = new SimpleProcess("PositionTotals Observer Process", 5, 3, CallbackGetPositionTotalsToObserve, _evl);
        }    
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckNullReference();
            _positions.PositionTotalsEvent += PositionsEventHandler;

            //lstPositionTotals.ItemsSource = _positions.PositionTotals.PositionTotalsObserveCollection;
            lstPositionTotals.ItemsSource = PositionCollection;
           // if (_observeProcess != null) _observeProcess.Start();
            GetAllTotals();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            if (_observeProcess != null) _observeProcess.Stop();
            _positions.PositionTotalsEvent -= PositionsEventHandler;
        }
        private void CallbackGetPositionTotalsToObserve()
        {
            Dispatcher.BeginInvoke((ThreadStart)(GetPositions));
        }
        private void GetPositions()
        {
            if (WindowState == WindowState.Minimized) return;
            _positions.GetPositionTotalsToObserve();
        }
        private void PositionsEventHandler(object o, PositionsEventArgs e)
        {
            if (e.WhatsHappens != "NewPositionTotal")
                return;
            Dispatcher.BeginInvoke((ThreadStart)(() =>
            {
                lock (_lockPositions)
                {
                    PositionCollection.Add((Positions.PositionTotalNpc)e.Position);
                }
            }));
        }
        private void GetAllTotals()
        {
            TempList.Clear();
            PositionCollection.Clear();
            _positions.GetAllTotals(TempList);
            foreach(var p in TempList)
                PositionCollection.Add((Positions.PositionTotalNpc)p);
        }

    }
}
