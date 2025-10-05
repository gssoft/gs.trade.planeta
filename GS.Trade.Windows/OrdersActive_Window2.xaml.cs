using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GS.Events;
using GS.Interfaces;
using GS.Process;
using GS.Trade.Interfaces;
// using GS.Trade.Trades;
using EventArgs = System.EventArgs;

namespace GS.Trade.Windows
{
    /// <summary>
    /// Логика взаимодействия для OrdersActive_Window2.xaml
    /// </summary>
    public partial class OrdersActiveWindow2 : Window
    {
        private ITradeContext _tx;
        private IEventLog _evl;
        // private IOrders _orders;

        // private long _index;
        public ObservableCollection<IOrder3> Orders { get; set; }
        private List<IOrder3> _ordersTemp;

        //private SimpleProcess _observeProcess;


        public OrdersActiveWindow2()
        {
            InitializeComponent();
            Orders = new ObservableCollection<IOrder3>();

            _ordersTemp = new List<IOrder3>();
        }
        private void CheckNullReference()
        {
            if (_evl == null  || _tx == null)
            {
                throw new NullReferenceException("OrdersActiveWindow(EventLog = null or Tx == null)");
            }
        }
        public void Init(ITradeContext tx, IEventLog eventlog)
        {
            _tx = tx;
            _evl = eventlog;
            //_orders = orders;

            CheckNullReference();

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "OrdersActiveWindow", "OrdersActiveWindow", "Initialization", "", "");
            //_tx.UIProcess.RegisterProcess("ActiveOrder Observer Process", 5000, 1000, null, CallbackGetActiveOrdersToObserve, null);
            //_observeProcess = new SimpleProcess("ActiveOrder Observer Process", 5, 2, CallbackGetActiveOrdersToObserve, _evl);
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckNullReference();
            lstOrdersActive.ItemsSource = Orders;
            _tx?.EventHub.Subscribe("UI.Orders", "Order", CallbackOrderOperation);
            //if (_observeProcess != null) _observeProcess.Start();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            Orders.Clear();
            _tx?.EventHub.UnSubscribe("UI.Orders", "Order", CallbackOrderOperation);
            //if (_observeProcess != null) _observeProcess.Stop();
        }
        private void CallbackOrderOperation(object sender, IEventArgs args)
        {
            var t = args.Object as IOrder3;
            if (t == null)
                return;
            switch (args.OperationKey)
            {
                case "UI.ORDERS.ORDER.ADD":
                     Dispatcher?.BeginInvoke((ThreadStart)(() =>
                     {
                         try
                         {
                            lock (Orders)
                            {
                                Orders.Insert(0, t);
                            }
                         }
                         catch (Exception e)
                         {
                             _tx.SendExceptionMessage3("ActiveOrdersWindow", "Order", "Add", t.ToString(), e);
                             // throw;
                         }
                     }
                    ));
                    break;
                case "UI.ORDERS.ORDER.DELETE":
                    Dispatcher?.BeginInvoke((ThreadStart)(() =>
                    {
                        lock (Orders)
                        {
                            var ot = Orders.FirstOrDefault(it => it.Key == t.Key);
                            if(ot!=null)
                                Orders.Remove(ot);
                        }
                    }
                    ));
                    break;
                case "UI.ORDERS.ORDER.UPDATE":
                    Dispatcher?.BeginInvoke((ThreadStart)(() =>
                    {
                        try
                        {
                            lock (Orders)
                            {
                                var ot = Orders.FirstOrDefault(it => it.Key == t.Key);
                                if (ot != null)
                                    Orders.Remove(ot);
                                Orders.Insert(0, t);
                            }
                        }
                        catch (Exception e)
                        {
                            _tx.SendExceptionMessage3("FilledOrdersWindow", "Order", "Update", t.ToString(), e);
                            // throw;
                        }
                    }
                    ));
                    break;
            }
        }
    }
}
