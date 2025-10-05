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
    /// Interaction logic for PositionClose_Window.xaml
    /// </summary>
    public partial class PositionsClosedWindow2 : Window
    {
        //Window The_MainWindow = null;
        private IEventLog _evl;
        private IPositions _positions;

        public ObservableCollection<IPosition> PositionList;
        private List<IPosition> ItemList;

        private SimpleProcess _observeProcess;

        public PositionsClosedWindow2()
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

            PositionList = new ObservableCollection<IPosition>();
            ItemList = new List<IPosition>(); 

            CheckNullReference();

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "PositionsClosedWindow2", "PositionsClosedWindow2", "Initialization", "", "");
            //_observeProcess = new SimpleProcess("PositionsClosed Observer Process", 5, 3, CallbackGetPositionsClosedToObserve, _evl);
            
            Refresh();
        }
        public void Clear()
        {
            lock (PositionList)
            {
                PositionList.Clear();
            }
        }
        public void Refresh()
        {
            ItemList.Clear();
            _positions.GetPositionClosed(0, ItemList);
            lock (PositionList)
            {
                foreach (var p in ItemList)
                {
                    PositionList.Insert(0, p);
                }
            }
        }
        private void CallbackNewPositionClosed(object sender, IPosition p)
        {
            Dispatcher.BeginInvoke((ThreadStart)(() =>
            {
                lock (PositionList)
                {
                    PositionList.Insert(0, p);
                }
            }
                ));
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckNullReference();

            LstPositionsClosed.ItemsSource = PositionList;
            //_observeProcess.Start();
            // _positions.NeedToObserverEvent += CallbackGetPositionsClosedToObserve;
            if (_positions != null)
                _positions.NewPositionClosed += CallbackNewPositionClosed;
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            //_observeProcess.Stop();
            if (_positions != null) 
                _positions.NewPositionClosed -= CallbackNewPositionClosed;
        }
        //private void CallbackGetPositionsClosedToObserve()
        //{
        //    Dispatcher.BeginInvoke((ThreadStart)(() => _positions.GetPositionsClosedToObserve()));
        //}
    }
}
