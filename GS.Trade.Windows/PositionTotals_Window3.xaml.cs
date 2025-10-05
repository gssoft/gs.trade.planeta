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
    public partial class PositionTotalsWindow3 : Window
    {
        private ITradeContext _tx;
        private IEventLog _evl;
        //private Positions.Totals _positions;

        public ObservableCollection<PositionTotalNpc2> PositionCollection;
      //  public List<Positions.Totals.PositionTotal> TempList;

        private readonly object _locker;
        //private SimpleProcess _observeProcess;

        public PositionTotalsWindow3()
        {
            InitializeComponent();

            _locker = new object();
            PositionCollection = new ObservableCollection<PositionTotalNpc2>();
        }
        private void CheckNullReference()
        {
            if (_evl == null || _tx == null)
                throw new NullReferenceException($"{GetType().Name}: EventLog = null or TradeContext == null)");
        }
        public void Init(ITradeContext tx, IEventLog eventlog)
        {
            _tx = tx;
            _evl = eventlog;

            CheckNullReference();

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.INIT,
                    GetType().Name, Title, "Initialization Start", "", "");

            IsProcessTaskInUse = true;
            SetupProcessTask();

            ProcessTask?.Start();

            _evl?.AddItem(EvlResult.SUCCESS, EvlSubject.INIT,
                                GetType().Name, Title, "Initialization Finish", "", "");
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

            // ProcessTask?.Start();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            //if (_observeProcess != null) _observeProcess.Stop();
            _tx.EventHub.UnSubscribe("UI.Positions", "Total", PositionsEventHandler);
            PositionCollection?.Clear();

            _evl?.Evlm2(EvlResult.INFO, EvlSubject.INIT, GetType().Name, Title, "WindowClosed", "", "");
            ProcessTask?.Stop();
                     
        }

        private void PositionsEventHandler(object o, Events.IEventArgs args)
        {
            var ip = args?.Object as IPosition2;
            if (ip == null)
                return;

            ProcessTask?.EnQueue(args);
        }
        
        private void GetAllTotals()
        {
            PositionCollection?.Clear();
            var ps = _tx.GetPositionTotals();
            foreach (var ip in ps)
                // PositionCurrentCollection.Add((PositionNpc)p);
                PositionCollection?.Add(new PositionTotalNpc2(ip));
            
            Title = "Totals ( " + PositionCollection?.Count + " )";
        }
    }
}
