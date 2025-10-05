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
using GS.Elements;
using GS.Events;
using GS.Interfaces;
using GS.Trade;
using GS.Trade.Interfaces;
using GS.Trade.Trades;
using GS.Trade.Trades.UI.Model;
using EventArgs = System.EventArgs;

namespace GS.Trade.Windows
{
    /// <summary>
    /// Логика взаимодействия для Positions_Window2.xaml
    /// </summary>
    public partial class PositionsWindow2 : Window
    {
        private ITradeContext _tx;
        private IEventLog _evl;
        private IPositions _positions;

        public ObservableCollection<PositionNpc2> PositionCurrentCollection;
        public List<IPosition2> TempList;

        //private SimpleProcess _observeProcess;
        private readonly object _locker;

        public PositionsWindow2()
        {
            InitializeComponent();

            _locker = new object();
            PositionCurrentCollection = new ObservableCollection<PositionNpc2>();
            TempList = new List<IPosition2>(); 
        }
        private void CheckNullReference()
        {
            if (_evl == null || _tx == null)
            {
                throw new NullReferenceException("PositionsWindow(EventLog = null or TradeContect == null)");
            }
        }
        public void Init(ITradeContext tx, IEventLog eventlog) //, IPositions positions)
        {
            _tx = tx;
            _evl = eventlog;
           // _positions = positions;

            CheckNullReference();
            //_positions.PositionsEvent += PositionsEventHandler;

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "PositionsOpenedWindow", "PositionsOpenedWindow", "Initialization", "", "");
            //_tx.UIProcess.RegisterProcess("PositionsOpened Observer Process", 5000, 3000, null, CallbackGetPositionsOpenedToObserve, null);
            //_observeProcess = new SimpleProcess("PositionsOpened Observer Process", 5, 3, CallbackGetPositionsOpenedToObserve, _evl);       
        }    
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckNullReference();

            LstPositionsOpened.ItemsSource = PositionCurrentCollection;
            // if (_observeProcess != null) _observeProcess.Start();
            _tx.EventHub.Subscribe("UI.Positions", "Current", CallbackTradeOperation);

            GetAllCurrents();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            //if (_observeProcess != null) _observeProcess.Stop();
            //if( _positions != null)
            //    _positions.PositionsEvent -= PositionsEventHandler;

            _tx.EventHub.UnSubscribe("UI.Positions", "Current", CallbackTradeOperation);
        }
        private void CallbackGetPositionsOpenedToObserve()
        {
            Dispatcher.BeginInvoke((ThreadStart)(() => _positions.GetPositionsOpenedToObserve()));
        }
        //private void PositionsEventHandler(object o, IPositionsEventArgs e)
        //{
        //    if (e.WhatsHappens != "NewCurrent")
        //        return;
        //    Dispatcher.BeginInvoke((ThreadStart)(() =>
        //                                             {
        //                                                 lock (_lockPositions)
        //                                                 {
        //                                                     PositionCurrentCollection.Add((PositionNpc2)e.Position);
        //                                                 }
        //                                             }));
        //}
        private void CallbackTradeOperation(object sender, IEventArgs args)
        {
            if (args.Object == null)
                return;

            var ip = args.Object as IPosition2;

            if (ip == null)
                return;

            switch (args.OperationKey)
            {
                case "UI.POSITIONS.CURRENT.INSERT":
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        var p = new PositionNpc2(ip);
                        lock (_locker)
                        {
                            PositionCurrentCollection.Insert(0, p);
                        }
                        Title = "Positions ( " + PositionCurrentCollection.Count + " )";
                    }
                   ));
                    break;
                case "UI.POSITIONS.CURRENT.ADD":
                case "UI.POSITIONS.CURRENT.ADD.TOEND":
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        var p = new PositionNpc2(ip);
                        lock (_locker)
                        {
                            PositionCurrentCollection.Add(p);
                        }
                        this.Title = "Positions ( " + PositionCurrentCollection.Count + " )";
                    }
                   ));
                   break;
                case "UI.POSITIONS.CURRENT.ADDNEW":
                   Dispatcher.BeginInvoke((ThreadStart)(() =>
                   {
                       //var pn = new PositionNpc2(ip);
                       lock (_locker)
                       {
                           var po = PositionCurrentCollection.FirstOrDefault(p => p.Key == ip.Key);
                           if (po == null)
                               PositionCurrentCollection.Add(new PositionNpc2(ip));
                       }
                       Title = "Positions ( " + PositionCurrentCollection.Count + " )";
                   }
                  ));
                   break;
                case "UI.POSITIONS.CURRENT.DELETE":
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        lock (_locker)
                        {
                            var po = PositionCurrentCollection.FirstOrDefault(p => p.Key == ip.Key);
                            if (po != null)
                                PositionCurrentCollection.Remove(po);
                        }
                        Title = "Positions ( " + PositionCurrentCollection.Count + " )";
                    }
                    ));
                    break;
                case "UI.POSITIONS.CURRENT.ADDORUPDATE":
                    
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        //if (WindowState == WindowState.Minimized) return;
                        try
                        {
                            lock (_locker)
                            {
                                var po = PositionCurrentCollection.FirstOrDefault(p => p.Key == ip.Key);
                                if (po != null)
                                    po.Update(ip);
                                else
                                {
                                    var pn = new PositionNpc2(ip);
                                    PositionCurrentCollection.Add(pn);
                                }
                            }
                            Title = "Positions ( " + PositionCurrentCollection.Count + " )";
                        }
                        catch (Exception e)
                        {
                            _tx.SendExceptionMessage3("PositionsWindow", "Position2", "AddOrUpdate", ip.ToString(), e);
                            // throw;
                        }
                    }
                    ));
                    break;
                case "UI.POSITIONS.CURRENT.UPDATE":
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        //if (WindowState == WindowState.Minimized) return;
                        try
                        {
                            lock (_locker)
                            {
                                var po = PositionCurrentCollection.FirstOrDefault(p => p.Key == ip.Key);
                                if (po != null)
                                    po.Update(ip);
                            }
                        }
                        catch (Exception e)
                        {
                            _tx.SendExceptionMessage3("PositionsWindow", "Position2", "Update", ip.ToString(),e);
                            // throw;
                        }
                    }
                    ));
                    break;
                case "UI.POSITIONS.CURRENT.UPDATE.PRICE2":
                    
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        //if (WindowState == WindowState.Minimized) return;
                        lock (_locker)
                        {
                            var po = PositionCurrentCollection.FirstOrDefault(p => p.Key == ip.Key);
                            if (po != null)
                                // po.Update(ip);
                                po.Price2 = ip.Price2;

                            // PositionCurrentCollection.Remove(ot);
                            // PositionCurrentCollection.Insert(0, t);
                        }
                    }
                    ));
                    break;
            }
        }
        private void GetAllCurrents()
        {
            //TempList.Clear();
            PositionCurrentCollection.Clear();
            //_positions.GetAllCurrents(TempList);
            var ps = _tx.GetPositionCurrents();
            foreach (var ip in ps)
               // PositionCurrentCollection.Add((PositionNpc)p);
                PositionCurrentCollection.Add(new PositionNpc2(ip));

            Title = "Positions ( " + PositionCurrentCollection.Count + " )";
        }

       
    }
}
