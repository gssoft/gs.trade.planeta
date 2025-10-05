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
using GS.Collections;
using GS.Events;
using GS.Interfaces;
using GS.Trade.Interfaces;
using GS.Trade.Trades;
using EventArgs = System.EventArgs;

namespace GS.Trade.Windows
{
    /// <summary>
    /// Логика взаимодействия для OrdersFilled_Window2.xaml
    /// </summary>
    public partial class OrdersFilledWindow2 : Window
    {
        //private const int CapasityVal = 1024;
        //private const int CapasityLimitVal = 256;

        private const int CapasityVal = 256;
        private const int CapasityLimitVal = 64;

        private ITradeContext _tx;

        public ObservableCollection<IOrder3> Orders { get; set; }
        protected ObservableListCollection<string, IOrder3> Items { get; set; }

        private List<IOrder3> _ordersTemp;

        public int Capasity { get; set; }
        public int CapasityLimit { get; set; }

        public int Count => Orders.Count;

        public OrdersFilledWindow2()
        {
            InitializeComponent();

            Orders = new ObservableCollection<IOrder3>();
            _ordersTemp = new List<IOrder3>();
        }
        private void CheckNullReference()
        {
            if (_tx == null)
            {
                throw new NullReferenceException("OrdersFilledWindow(Tx == null)");
            }
        }
        public void Init(ITradeContext tx)
        {
            _tx = tx;

            Capasity = CapasityVal;
            CapasityLimit = CapasityLimit;

            CheckNullReference();

            Items = new ObservableListCollection<string, IOrder3>
            {
                Code = "OrderFilledWindowCollection",
                Name = "OrderFilledWindowCollection",
                Category = "Orders",
                Entity = "Order.Completed",
                Capasity = CapasityVal,
                CapasityLimit = CapasityLimitVal,
                IsReversed = true,
                EventLog = _tx.EventLog,
                IsEvlEnabled = false
            };

            _tx.Evlm(EvlResult.SUCCESS, EvlSubject.TRADING, "OrdersFilledWindow", "OrdersFilledWindow", "Initialization", "", "");
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckNullReference();
            //LstOrdersCompleted.ItemsSource = Orders;
            LstOrdersCompleted.ItemsSource = Items.Collection;

            _tx?.EventHub.Subscribe("UI.Orders", "Order.Completed", CallbackOrderOperation);
            //if (_observeProcess != null) _observeProcess.Start();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            Orders.Clear();
            _tx?.EventHub.UnSubscribe("UI.Orders", "Order.Completed", CallbackOrderOperation);
            //if (_observeProcess != null) _observeProcess.Stop();
        }
        private void CallbackOrderOperation(object sender, IEventArgs args)
        {
            var t = args.Object as IOrder3;
            if (t == null)
                return;
            switch (args.OperationKey)
            {
                case "UI.ORDERS.ORDER.COMPLETED.ADD":
                    Dispatcher?.BeginInvoke((ThreadStart)(() =>
                    {
                        try
                        {
                            //lock (Orders)
                            //{
                                //if (Capasity != 0 && (CapasityLimit + Capasity) <= Count)
                                //    ClearSomeData(Capasity);

                                //Orders.Insert(0, t);
                                Items.Add(t);
                            //}
                        }
                        catch (Exception e)
                        {
                            _tx.SendExceptionMessage3("FilledOrdersWindow", "Order", "Add", t.ToString(), e);
                            // throw;
                        }
                    }
                   ));
                    break;
                case "UI.ORDERS.ORDER.COMPLETED.DELETE":
                    Dispatcher?.BeginInvoke((ThreadStart)(() =>
                    {
                        //lock (Orders)
                        //{
                        //    var ot = Orders.FirstOrDefault(it => it.Key == t.Key);
                        //    if (ot != null)
                        //        Orders.Remove(ot);
                        //}
                        Items.Remove(t);
                    }
                    ));
                    break;
                case "UI.ORDERS.ORDER.COMPLETED.ADDORUPDATE":
                case "UI.ORDERS.ORDER.COMPLETED.UPDATE":
                    Dispatcher?.BeginInvoke((ThreadStart)(() =>
                    {
                        Items.AddOrUpdate(t);
                    }
                    ));
                    break;
            }
        }
        private void ClearSomeData(int count)
        {
            //lock (_locker)
            //{
            while (Count > count)
            {
                Orders.RemoveAt(Count - 1);
            }
            //}
            _tx.Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "OrderFilledWindow", "OrderFilledWindow", "ClearSomeData()",
                $"Capasity={Capasity}; Limit={CapasityLimit}; ItemsCount={Count}", "");
        }
    }
}
