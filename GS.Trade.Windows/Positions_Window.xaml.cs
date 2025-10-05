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
using GS.Process;
using GS.Trade.Interfaces;
using GS.Trade.Trades;

namespace GS.Trade.Windows
{
    /// <summary>
    /// Логика взаимодействия для Positions_Window.xaml
    /// </summary>
    public partial class PositionsWindow : Window
    {
        private ITradeContext _tx;
        private IEventLog _evl;
        private IPositions _positions;

        public ObservableCollection<PositionNpc> PositionCurrentCollection;
        public List<IPosition> TempList;

        private SimpleProcess _observeProcess;
        private object _lockPositions;

        public PositionsWindow()
        {
            InitializeComponent();
            _lockPositions = new object();
            PositionCurrentCollection = new ObservableCollection<PositionNpc>();
            TempList = new List<IPosition>(); 
        }
        private void CheckNullReference()
        {
            if (_evl == null || _positions == null)
            {
                throw new NullReferenceException("PositionsWindow(EventLog = null or Positions == null)");
            }
        }
        public void Init(ITradeContext tx, IEventLog eventlog, IPositions positions)
        {
            _tx = tx;
            _evl = eventlog;
            _positions = positions;

            CheckNullReference();
            _positions.PositionsEvent += PositionsEventHandler;

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "PositionsOpenedWindow", "PositionsOpenedWindow", "Initialization", "", "");
            //_tx.UIProcess.RegisterProcess("PositionsOpened Observer Process", 5000, 3000, null, CallbackGetPositionsOpenedToObserve, null);
            //_observeProcess = new SimpleProcess("PositionsOpened Observer Process", 5, 3, CallbackGetPositionsOpenedToObserve, _evl);       
        }    
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckNullReference();

            //lstPositions.ItemsSource = _positions.PositionOpenedObserveCollection;
            lstPositions.ItemsSource = PositionCurrentCollection;
            // if (_observeProcess != null) _observeProcess.Start();
            GetAllCurrents();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            if (_observeProcess != null) _observeProcess.Stop();
            if( _positions != null)
                _positions.PositionsEvent -= PositionsEventHandler;
        }
        private void CallbackGetPositionsOpenedToObserve()
        {
            Dispatcher.BeginInvoke((ThreadStart)(() => _positions.GetPositionsOpenedToObserve()));
        }
        private void PositionsEventHandler(object o, IPositionsEventArgs e)
        {
            if (e.WhatsHappens != "NewCurrent")
                return;
            Dispatcher.BeginInvoke((ThreadStart)(() =>
                                                     {
                                                         lock (_lockPositions)
                                                         {
                                                             PositionCurrentCollection.Add((PositionNpc)e.Position);
                                                         }
                                                     }));
        }
        private void GetAllCurrents()
        {
            TempList.Clear();
            PositionCurrentCollection.Clear();
            _positions.GetAllCurrents(TempList);
            foreach (var p in TempList)
                PositionCurrentCollection.Add((PositionNpc)p);
        }
    }
}
