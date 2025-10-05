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
using GS.Trade.Interfaces;
using GS.Trade.Trades.UI.Model;
using EventArgs = System.EventArgs;

namespace GS.Trade.Windows
{
    /// <summary>
    /// Логика взаимодействия для Portfolio_Window.xaml
    /// </summary>
    public partial class PortfolioWindow : Window
    {
        private ITradeContext _tx;
        private IEventLog _evl;

        public ObservableCollection<PositionNpc2> PositionCurrentCollection;
        public List<IPosition2> TempList;

        //private SimpleProcess _observeProcess;
        private readonly object _locker;

        public PortfolioWindow()
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

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "PortfoliosWindow", "PositionsOpenedWindow", "Initialization", "", "");
            //_tx.UIProcess.RegisterProcess("PositionsOpened Observer Process", 5000, 3000, null, CallbackGetPositionsOpenedToObserve, null);
            //_observeProcess = new SimpleProcess("PositionsOpened Observer Process", 5, 3, CallbackGetPositionsOpenedToObserve, _evl);       
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckNullReference();

            LstPositionsOpened.ItemsSource = PositionCurrentCollection;
         
            _tx.EventHub.Subscribe("UI.Portfolio", "Position", CallbackTradeOperation);
            _tx.EventHub.Subscribe("UI.Positions", "Current", CallbackTradeOperation);

            // GetAllCurrents();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            _tx.EventHub.UnSubscribe("UI.Portfolio", "Position", CallbackTradeOperation);
            _tx.EventHub.UnSubscribe("UI.Positions", "Current", CallbackTradeOperation);
        }

        private void CallbackTradeOperation(object sender, IEventArgs args)
        {
            var ip = args.Object as IPosition2;
            if (ip == null)
                return;

            switch (args.OperationKey)
            {
                case "UI.PORTFOLIO.POSITION.ADDORUPDATE":
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
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
                    }
                    ));
                    break;
             
                case "UI.POSITIONS.CURRENT.UPDATE.PRICE2":
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        lock (_locker)
                        {
                            var po = PositionCurrentCollection.FirstOrDefault(p => p.TickerCodeEx == ip.Ticker.Code);
                            if (po == null) return;
                            po.Price2 = ip.Price2;
                            po.LastPrice = ip.LastPrice;
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
        }
    }
}
