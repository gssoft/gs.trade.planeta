using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GS.Interfaces;
using GS.Trade.Interfaces;
using GS.Process;

namespace GS.Trade.Windows
{
    /// <summary>
    /// Interaction logic for EventView_Window.xaml
    /// </summary>
    public partial class EventLogWindow : Window
    {
      //  private Window _theMainWindow;
        private IEventLog _evl;
        private SimpleProcess _observeProcess;
        
        private long _lastGetRequestIndex;
        private long _itemsCount;

        public ObservableCollection<IEventLogItem> EventLogObserveItems;
        private List<IEventLogItem> ItemList;

        private bool _itemListLocker;

        public EventLogWindow()
        {
            InitializeComponent();
        }
        private void CheckNullReference()
        {
            if (_evl == null)
            {
                throw new NullReferenceException("EventLogdWindow(EventLog == null)");
            }
        }
        public void Init(IEventLog eventlog)
        {
            _evl = eventlog;

            EventLogObserveItems= new ObservableCollection<IEventLogItem>();
            ItemList = new List<IEventLogItem>();

            CheckNullReference();

            _itemListLocker = true;

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "TradeContext","EventLogWindow", "Initialization", _evl.ToString(), "");
            _observeProcess = new SimpleProcess("EventLog Observer Process", 5, 1, CallbackGetEventLogToObserve2, _evl);
        }       
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckNullReference();

           // lstEventView.ItemsSource = _evl.EventLogObserveCollection;
            lstEventView.ItemsSource = EventLogObserveItems;
            _observeProcess.Start();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            if( _observeProcess != null) _observeProcess.Stop();
        }
        private void CallbackGetEventLogToObserve()
        {
          //  Dispatcher.BeginInvoke((ThreadStart)(() => _evl.GetEventLogToObserve()));
        }
        private void CallbackGetEventLogToObserve2()
        {
            if (!_itemListLocker)
                return;
            var cntAll = _evl.GetEventLogItemsCount();
            if (_itemsCount >= cntAll)
                return;
            _itemsCount = cntAll;
                
            ItemList.Clear();
            var cnt = _evl.GetEventLogItems(_lastGetRequestIndex, ItemList);
            if (cnt < 1)
                return;
            Dispatcher.BeginInvoke((ThreadStart)(CopyToObserverList));
        }
        private void CopyToObserverList()
        {
            _itemListLocker = false;
            var cnt = ItemList.Count;
            if (cnt < 1)
            {
                _itemListLocker = true;
                return;
            }
            _lastGetRequestIndex = ItemList[cnt-1].Index;
            try
            {
                lock (EventLogObserveItems)
                {
                    foreach (var i in ItemList)
                        EventLogObserveItems.Insert(0, i);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                _itemListLocker = true;
            }
        }
    }
}
