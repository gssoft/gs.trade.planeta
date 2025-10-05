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
using GS.Process;
using GS.Trade.Interfaces;
using GS.Trade.Trades.UI.Model;
using EventArgs = System.EventArgs;

namespace GS.Trade.Windows
{
    /// <summary>
    /// Логика взаимодействия для Deals_Window.xaml
    /// </summary>
    public partial class DealsWindow : Window
    {
        //private const int CapasityVal = 1024;
        //private const int CapasityLimitVal = 256;

        //private const int CapasityVal = 256;
        //private const int CapasityLimitVal = 64;

        //private const int CapasityVal = 1024;
        //private const int CapasityLimitVal = 1024;

        private const int CapasityVal = 512;
        private const int CapasityLimitVal = 512;

        private IEventLog _evl;
        private ITradeContext _tx;
        private readonly object _locker;

        //public ObservableCollection<PositionNpc2> PositionList;
        protected ObservableListCollection<string, IDeal> Items { get; set; }

        public DealsWindow()
        {
            InitializeComponent();
            _locker = new object();
        }
        private void CheckNullReference()
        {
            if (_evl == null || _tx == null)
            {
                throw new NullReferenceException("PositionsCLosedWindow(EventLog = null or Positions == null)");
            }
        }
        public void Init(ITradeContext tx, IEventLog eventlog)
        {
            _evl = eventlog;
            _tx = tx;

            CheckNullReference();

            Items = new ObservableListCollection<string, IDeal>
            {
                Code = "DealsWindowCollection",
                Name = "DealsWindowCollection",
                Category = "Deals",
                Entity = "Deal",
                Capasity = CapasityVal,
                CapasityLimit = CapasityLimitVal,
                IsReversed = true,
                EventLog = _tx.EventLog,
                IsEvlEnabled = false
            };

            //PositionList = new ObservableCollection<PositionNpc2>();

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "Deals Window", "Deals Window", "Initialization", "", "");
            //_observeProcess = new SimpleProcess("PositionsClosed Observer Process", 5, 3, CallbackGetPositionsClosedToObserve, _evl);

            //Refresh();
        }
        public void Clear()
        {
            //lock (PositionList)
            //{
            //    PositionList.Clear();
            //}
            //Items.Clear();
        }
        public void Refresh()
        {
            Clear();
            var deals =_tx.GetDeals();
            //lock (_locker)
            //{
            //    foreach (var p in deals.Select(ip => new PositionNpc2(ip)))
            //    {
            //        PositionList.Insert(0, p);
            //    }
            //}
            //foreach (var p in deals)
            //{
            //    PositionList.Add(p);
            //}
        }
        private void CallbackNewPositionClosed(object sender, IEventArgs args)
        {
            if (args.Object == null)
                return;

            //var ip = args.Object as IPosition2;
            var ip = args.Object as IDeal;

            if (ip == null)
                return;

            switch (args.OperationKey)
            {
                case "UI.DEALS.DEAL.ADD":
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        //var p = new PositionNpc2(ip);
                        //lock (_locker)
                        //{
                        //    PositionList.Insert(0, p);
                        //}
                        Items.Add(ip);

                        this.Title = "Deals ( " + Items.Count + " )"; // + PnLTitle();
                    }
                   ));
                    break;
                case "UI.DEALS.DEAL.DELETE":
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        //lock (_locker)
                        //{
                        //    var po = PositionList.FirstOrDefault(p => p.Key == ip.Key);
                        //    if (po != null)
                        //        PositionList.Remove(po);
                        //}
                        Items.Remove(ip);

                        this.Title = "Deals ( " + Items.Count + " )"; // + PnLTitle();
                    }
                    ));
                    break;
                case "UI.DEALS.DEAL.UPDATE":
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        //lock (_locker)
                        //{
                        //    var po = PositionList.FirstOrDefault(p => p.Key == ip.Key);
                        //    if (po != null)
                        //        po.Update(ip);
                        //}
                        Items.Update(ip);

                        this.Title = "Deals ( " + Items.Count + " )"; // + PnLTitle();
                    }
                    ));
                    break;
            }
        }

        private string PnLTitle()
        {
            return  @" P&L: " + Items.Items.Sum(i => i.PnL).ToString("C");
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckNullReference();

            //LstPositionsClosed.ItemsSource = PositionList;
            LstPositionsClosed.ItemsSource = Items.Collection;
            //_observeProcess.Start();
            // _positions.NeedToObserverEvent += CallbackGetPositionsClosedToObserve;
            if (_tx.EventHub != null)
                _tx.EventHub.Subscribe("UI.Deals", "Deal", CallbackNewPositionClosed);
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            //_observeProcess.Stop();
            if (_tx.EventHub != null)
                _tx.EventHub.UnSubscribe("UI.Deals", "Deal", CallbackNewPositionClosed);
        }
    }
}
