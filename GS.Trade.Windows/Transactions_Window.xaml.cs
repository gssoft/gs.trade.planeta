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
using GS.Trade.TradeTerminals64;
using EventArgs = System.EventArgs;

namespace GS.Trade.Windows
{
    /// <summary>
    /// Логика взаимодействия для Transactions_Window.xaml
    /// </summary>
    public partial class TransactionsWindow : Window
    {
        //private const int CapasityVal = 1024;
        //private const int CapasityLimitVal = 256;

        private const int CapasityVal = 256;
        private const int CapasityLimitVal = 64;

        //Window The_MainWindow = null;
        private IEventLog _evl;
        private ITradeContext _trContext;

        protected ObservableListCollection<ulong, IQuikTransaction> Transactions { get; set; }

        public TransactionsWindow()
        {
            InitializeComponent();
        }
        private void CheckNullReference()
        {
            if (_evl == null || _trContext == null)
                throw new NullReferenceException($"{GetType().Name}: EventLog = null or TradeContext == null)");
        }
        public void Init(IEventLog eventlog, ITradeContext tx)
        {
            _evl = eventlog;
            _trContext = tx;

            CheckNullReference();

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, GetType().Name,
                         Title, "Initialization Start", "", "");

            Transactions = new ObservableListCollection<ulong, IQuikTransaction>
            {
                Code = "TransactionWindowCollection",
                Name = "TransactionWindowCollection",
                Category = "Transactions",
                Entity = "Transaction",
                Capasity = CapasityVal,
                CapasityLimit = CapasityLimitVal,
                IsReversed = true,
                EventLog = _trContext.EventLog,
                IsEvlEnabled = false
            };

            IsProcessTaskInUse = true;
            SetupProcessTask();

            ProcessTask?.Start();

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, GetType().Name,
                Title, "Initialization End", "", "");
        }
        public void Clear()
        {
           Transactions?.Clear();
        }
        public void Refresh()
        {
        }
        private void CallbackNewTransaction(object sender, IEventArgs args)
        {
            var t = args.Object as IQuikTransaction;
            if (t == null)
                return;

            if (args.OperationKey != "TRANSACTIONS.TRANSACTION.ADD" &&
                args.OperationKey != "TRANSACTIONS.TRANSACTION.ADDORUPDATE")
                return;

            if (IsProcessTaskInUse)
            {
                ProcessTask?.EnQueue(t);
                return;
            }
            Dispatcher?.BeginInvoke((ThreadStart)(() =>
            {
                var ot = Transactions.Items.FirstOrDefault(to => to.Key == t.Key);
                if (ot != null)
                    Transactions.Remove(ot);
                Transactions.Add(t);
            }
            ));
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckNullReference();

            TransactionListView.ItemsSource = Transactions.Collection;

            // ProcessTask?.Start();

            _trContext?.EventHub.Subscribe("Transactions", "Transaction", CallbackNewTransaction );
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            _trContext?.EventHub.UnSubscribe("Transactions", "Transaction", CallbackNewTransaction );
            ProcessTask?.Stop();
            Clear();
        }
    }
    
}
