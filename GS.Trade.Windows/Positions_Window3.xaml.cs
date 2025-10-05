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
    public partial class PositionsWindow3 : Window
    {
        private ITradeContext _tx;
        private IEventLog _evl;

        public ObservableCollection<PositionNpc2> PositionCurrentCollection;

        private readonly object _locker;

        public PositionsWindow3()
        {
            InitializeComponent();

            _locker = new object();
            PositionCurrentCollection = new ObservableCollection<PositionNpc2>();
        }
        private void CheckNullReference()
        {
            if (_evl == null || _tx == null)
                  throw new NullReferenceException($"{GetType().Name}: EventLog = null or TradeContext == null)");
        }
        public void Init(ITradeContext tx, IEventLog eventlog) //, IPositions positions)
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

            LstPositionsOpened.ItemsSource = PositionCurrentCollection;
            _tx.EventHub.Subscribe("UI.Positions", "Current", PositionsEventHandler);

            GetAllCurrents();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            _tx.EventHub.UnSubscribe("UI.Positions", "Current", PositionsEventHandler);

            ProcessTask?.Stop();
            PositionCurrentCollection?.Clear();
        }
        private void PositionsEventHandler(object sender, IEventArgs args)
        {
            var ip = args?.Object as IPosition2;
            if (ip == null)
                return;

            ProcessTask?.EnQueue(args);
        }
        private void GetAllCurrents()
        {
            PositionCurrentCollection.Clear();
            var ps = _tx.GetPositionCurrents();
            foreach (var ip in ps)
                PositionCurrentCollection.Add(new PositionNpc2(ip));

            Title = "Positions ( " + PositionCurrentCollection.Count + " )";
        }      
    }
}
