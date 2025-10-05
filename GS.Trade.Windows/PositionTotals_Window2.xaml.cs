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
using GS.Interfaces;
using GS.Trade.Interfaces;
using GS.Trade.Trades;
using GS.Trade.Trades.UI.Model;

namespace GS.Trade.Windows
{
    /// <summary>
    /// Логика взаимодействия для PositionTotals_Window2.xaml
    /// </summary>
    public partial class PositionTotalsWindow2 : Window
    {
        private ITradeContext _tx;
        private IEventLog _evl;
        //private Positions.Totals _positions;

        public ObservableCollection<PositionTotalNpc2> PositionCollection;
      //  public List<Positions.Totals.PositionTotal> TempList;

        private readonly object _locker;
        //private SimpleProcess _observeProcess;

        public PositionTotalsWindow2()
        {
            InitializeComponent();

            _locker = new object();
            PositionCollection = new ObservableCollection<PositionTotalNpc2>();
        }
        private void CheckNullReference()
        {
            if (_evl == null || _tx == null)
            {
                throw new NullReferenceException("PositionTotalsWindow(EventLog = null or TradeContext == null)");
            }
        }
        public void Init(ITradeContext tx, IEventLog eventlog)
        {
            _tx = tx;
            _evl = eventlog;
            //_positions = positions;

            CheckNullReference();

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
                                "PositionTotalsWindow2", "PositionTotalsWindow2", "Initialization", "", "");
            // _tx.UIProcess.RegisterProcess("PositionsOpened Observer Process", 5000, 4000, null, CallbackGetPositionTotalsToObserve, null);
            // _observeProcess = new SimpleProcess("PositionTotals Observer Process", 5, 3, CallbackGetPositionTotalsToObserve, _evl);
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckNullReference();
           // _positions.PositionTotalsEvent += PositionsEventHandler;

            //lstPositionTotals.ItemsSource = _positions.PositionTotals.PositionTotalsObserveCollection;
            lstPositionTotals.ItemsSource = PositionCollection;
            _tx.EventHub.Subscribe("UI.Positions", "Total", PositionsEventHandler);
            // if (_observeProcess != null) _observeProcess.Start();
            GetAllTotals();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            //if (_observeProcess != null) _observeProcess.Stop();
            _tx.EventHub.UnSubscribe("UI.Positions", "Total", PositionsEventHandler);
        }
        //private void CallbackGetPositionTotalsToObserve()
        //{
        //    Dispatcher.BeginInvoke((ThreadStart)(GetPositions));
        //}
        //private void GetPositions()
        //{
        //    //if (WindowState == WindowState.Minimized) return;
        //    _tx.GetPositionTotals();
        //}
        private void PositionsEventHandler(object o, Events.IEventArgs args)
        {
            if (args.Object == null)
                return;

            var ip = args.Object as IPosition2;

            if (ip == null)
                return;

            switch (args.OperationKey)
            {
                case "UI.POSITIONS.TOTAL.INSERT":
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        var p = new PositionTotalNpc2(ip);
                        lock (_locker)
                        {
                            PositionCollection.Insert(0, p);
                        }
                        this.Title = "Totals ( " + PositionCollection.Count + " )";
                    }
                   ));
                    break;
                case "UI.POSITIONS.TOTAL.ADD":
                case "UI.POSITIONS.TOTAL.ADD.TOEND":
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        var p = new PositionTotalNpc2(ip);
                        lock (_locker)
                        {
                            PositionCollection.Add(p);
                        }
                        this.Title = "Totals ( " + PositionCollection.Count + " )";
                    }
                   ));
                   break;
                case "UI.POSITIONS.TOTAL.ADDNEW":
                   Dispatcher.BeginInvoke((ThreadStart)(() =>
                   {
                       lock (_locker)
                       {
                           var po = PositionCollection.FirstOrDefault(p => p.Key == ip.Key);
                           if (po == null)
                               PositionCollection.Add(new PositionTotalNpc2(ip));
                       }
                       this.Title = "Totals ( " + PositionCollection.Count + " )";
                   }
                  ));
                   break;
                case "UI.POSITIONS.TOTAL.DELETE":
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        lock (_locker)
                        {
                            var po = PositionCollection.FirstOrDefault(p => p.Key == ip.Key);
                            if (po != null)
                                PositionCollection.Remove(po);
                        }
                        this.Title = "Totals ( " + PositionCollection.Count + " )";
                    }
                    ));
                    break;
                
                case "UI.POSITIONS.TOTAL.UPDATE":
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        lock (_locker)
                        {
                            var po = PositionCollection.FirstOrDefault(p => p.Key == ip.Key);
                            if (po != null)
                                po.Update(ip);
                        }
                    }
                    ));
                    break;
                case "UI.POSITIONS.TOTAL.ADDORUPDATE":
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        try
                        {
                            lock (_locker)
                            {
                                var po = PositionCollection.FirstOrDefault(p => p.Key == ip.Key);
                                if (po != null)
                                    po.Update(ip);
                                else
                                    PositionCollection.Add(new PositionTotalNpc2(ip));
                            }
                            this.Title = "Totals ( " + PositionCollection.Count + " )";
                        }
                        catch (Exception e)
                        {
                            _tx.SendExceptionMessage3("TotalsWindow", "Position2", "AddOrUpdate", ip.ToString(), e);
                            // throw;
                        }
                    }
                    ));
                    break;
            }
        }
        private void GetAllTotals()
        {
            PositionCollection.Clear();
            var ps = _tx.GetPositionTotals();
            foreach (var ip in ps)
                // PositionCurrentCollection.Add((PositionNpc)p);
                PositionCollection.Add(new PositionTotalNpc2(ip));
            
            Title = "Totals ( " + PositionCollection.Count + " )";
        }
    }
}
