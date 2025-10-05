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
    /// Interaction logic for OrdersFilled_Window.xaml
    /// </summary>
    public partial class OrdersFilledWindow : Window
    {
        //private Window _theMainWindow = null;
        private ITradeContext _tx;
        IEventLog _evl;
        private IOrders _orders;

        private long _index;
        public ObservableCollection<IOrder> OrderFilledObserveCollection { get; set; }
        private readonly List<IOrder> _tempOrders;

        private object _lockOrders;

        private volatile int _busy;

        private SimpleProcess _observeProcess;

        public OrdersFilledWindow()
        {
            InitializeComponent();
            OrderFilledObserveCollection = new ObservableCollection<IOrder>();
            _tempOrders = new List<IOrder>();
            _lockOrders = new object();
        }
        private void CheckNullReference()
        {
            if (_evl == null || _orders == null)
            {
                throw new NullReferenceException("OrdersFilledWindow(EventLog = null or Orders == null)");
            }
        }
        public void Init(ITradeContext tx, IEventLog eventlog, IOrders orders)
        {
            _tx = tx;
            _evl = eventlog;
            _orders = orders;

            _busy = 0;

            CheckNullReference();

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "OrdersFilledWindow", "OrdersFilledWindow", "Initialization", "", "");
            //_tx.UIProcess.RegisterProcess("FilledOrder Observer Process", 5000, 2000, null, CallbackGetFilledOrdersToObserve, null);
            //_observeProcess = new SimpleProcess("FilledOrder Observer Process", 5, 2, CallbackGetFilledOrdersToObserve, _evl);

            
        }    
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckNullReference();
            _orders.NewOrderStatusEvent += NewOrderStatusEventHandler;

           // lstOrdersFilled.ItemsSource = _orders.OrderFilledObserveCollection;
            lstOrdersFilled.ItemsSource = OrderFilledObserveCollection;
           // if (_observeProcess != null) _observeProcess.Start();
            GetAllFilled();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            if (_observeProcess != null)  _observeProcess.Stop();
            OrderFilledObserveCollection.Clear();
        }
        private void GetAllFilled()
        {
            _tempOrders.Clear();
            OrderFilledObserveCollection.Clear();
            _orders.GetAllFilledOrCancelled(_tempOrders);

            foreach (var ord in _tempOrders)
                OrderFilledObserveCollection.Insert(0,ord);
        }
        private void NewOrderStatusEventHandler(object o, INewOrderStatusEventArgs e)
        {
            if (e.Order.Status == OrderStatusEnum.Filled || 
                e.Order.Status == OrderStatusEnum.Canceled)
            {
                Dispatcher.BeginInvoke((ThreadStart) (() =>
                                                          {
                                                              lock (_lockOrders)
                                                              {
                                                                  OrderFilledObserveCollection.Insert(0, e.Order);
                                                              }
                                                          }));
            }
        }

        private void CallbackGetFilledOrdersToObserve()
        {
            //if (_busy > 0) return;
            //_busy = 1;
            if (_index < _orders.MaxIndex)
            {
                //Dispatcher.BeginInvoke((ThreadStart) (() => _orders.GetOrdersToObserve()));
                Dispatcher.BeginInvoke((ThreadStart)(() =>
                                                         {
                                                             _tempOrders.Clear();
                                                             _orders.GetFilledOrCancelOrders(_index, _tempOrders);
                                                             foreach (var o in _tempOrders.OrderBy(ord => ord.MyIndex))
                                                             {
                                                                 if (o.MyIndex > _index) _index = o.MyIndex;
                                                                 OrderFilledObserveCollection.Insert(0, o);
                                                             }
                                                         }
                                                         ));
                

            }
           // _busy = 0;
        }    
    }
}
