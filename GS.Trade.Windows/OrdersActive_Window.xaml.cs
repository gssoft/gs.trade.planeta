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
    /// Interaction logic for Orders_Window.xaml
    /// </summary>
    public partial class OrdersActiveWindow : Window
    {
        //private Window _theMainWindow = null;
        private ITradeContext _tx;
        private IEventLog _evl;
        private IOrders _orders;

        private long _index;
        public ObservableCollection<IOrder> OrderObserveCollection { get; set; }
        private List<IOrder> _ordersTemp;

        private SimpleProcess _observeProcess;

        public OrdersActiveWindow()
        {
            InitializeComponent();
            OrderObserveCollection = new ObservableCollection<IOrder>();
            _ordersTemp = new List<IOrder>();
        }
        private void CheckNullReference()
        {
            if (_evl == null || _orders == null)
            {
                throw new NullReferenceException("OrdersActiveWindow(EventLog = null or Orders == null)");
            }
        }
        public void Init(ITradeContext tx, IEventLog eventlog, IOrders orders)
        {
            _tx = tx;
            _evl = eventlog;
            _orders = orders;

            CheckNullReference();

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "OrdersActiveWindow", "OrdersActiveWindow", "Initialization", "", "");
            _tx.UIProcess.RegisterProcess("ActiveOrder Observer Process", 5000, 1000, null, CallbackGetActiveOrdersToObserve, null);
            //_observeProcess = new SimpleProcess("ActiveOrder Observer Process", 5, 2, CallbackGetActiveOrdersToObserve, _evl);
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckNullReference();

            //lstOrdersActive.ItemsSource = _orders.OrderActiveObserveCollection;
            lstOrdersActive.ItemsSource = OrderObserveCollection;
            if (_observeProcess != null) _observeProcess.Start();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            OrderObserveCollection.Clear();
            if (_observeProcess != null) _observeProcess.Stop();
        }
        private void CallbackGetActiveOrdersToObserve()
        {
            //Dispatcher.BeginInvoke((ThreadStart)(() => _orders.GetOrdersToObserve()));
            var m = _orders.MaxIndex;
            if (_index >= m) return;

            //_tx.Evlm(EvlResult.WARNING, EvlSubject.TECHNOLOGY,
            //         "AciveOrdersWindow", "Need To Refresh", _index.ToString(), m.ToString());

            _index = m;
            Dispatcher.BeginInvoke((ThreadStart)(
                                                    () =>
                                                        {
                                                            _ordersTemp.Clear();
                                                            OrderObserveCollection.Clear();
                                                            /*
                                                            var oo = from o in _orders.OrderCollection
                                                                     where
                                                                         o.Status == OrderStatusEnum.Active ||
                                                                         o.Status == OrderStatusEnum.Registered ||
                                                                         o.Status == OrderStatusEnum.PartlyFilled ||
                                                                         o.Status == OrderStatusEnum.Unknown
                                                                     select o;
                                                            */
                                                            _orders.GetActiveOrders(_index, _ordersTemp);
                                                            foreach (var order in _ordersTemp.OrderBy(ord=>ord.MyIndex))
                                                            {
                                                                if (_index < order.MyIndex) _index = order.MyIndex;
                                                                OrderObserveCollection.Insert(0, order);
                                                            }
                                                        }));
        }    
    }
}
