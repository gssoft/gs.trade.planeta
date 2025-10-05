using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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
using GS.Trade.Trades;
using EventArgs = System.EventArgs;

namespace GS.Trade.Windows
{
    /// <summary>
    /// Логика взаимодействия для Trades_Window2.xaml
    /// </summary>
    public partial class TradesWindow2 : Window
    {
        protected IEventLog EventLog;
        private const int CapasityVal = 1024;
        private const int CapasityLimitVal = 1024;

        private ITradeContext _tx;
        public ObservableCollection<ITrade3> Trades { get; set; }
        protected ObservableListCollection<string, ITrade3> Items { get; set; }

        public TradesWindow2()
        {
            InitializeComponent();
        }
        public void Init(ITradeContext tx)
        {
            if (tx == null)
                throw new NullReferenceException("TradesWindow.Init(Tx == null)");
            _tx = tx;

            if(_tx.EventLog == null)
                throw new NullReferenceException("TradesWindow.Init(EventLog == null)");
            EventLog = _tx.EventLog; 

            EventLog?.Evlm2(EvlResult.INFO, EvlSubject.INIT, GetType().Name, Title, "Init Begin","","");

            Items = new ObservableListCollection<string, ITrade3>
            {
                Code = "TradesWindowCollection",
                Name = "TradesWindowCollection",
                Category = "Trades",
                Entity = "Trade",
                Capasity = CapasityVal,
                CapasityLimit = CapasityLimitVal,
                IsReversed = true,
                EventLog = _tx.EventLog,
                IsEvlEnabled = false
            };

            IsProcessTaskInUse = true;
            SetupProcessTask();

            _tx?.EventHub.Subscribe("UI.Trades", "Trade", CallbackTradeOperation);

            EventLog?.Evlm2(EvlResult.INFO, EvlSubject.INIT, GetType().Name, Title, "Init Finish", "", "");
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            lstTrades.ItemsSource = Items.Collection;
            ProcessTask?.Start();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            Items?.Clear();
            _tx?.EventHub.UnSubscribe("UI.Trades", "Trade", CallbackTradeOperation);

            EventLog?.Evlm2(EvlResult.INFO, EvlSubject.INIT, GetType().Name, Title, "WindowClosed", "", "");
            ProcessTask?.Stop();
        }
        // old before 2018.05.25 and now
        private void CallbackTradeOperation(object sender, IEventArgs args)
        {
            var t = args?.Object as ITrade3;
            if (t == null)
                return;

            if (IsProcessTaskInUse)
            {
                ProcessTask?.EnQueue(args);
                return;
            }

            switch (args.OperationKey)
            {
                case "UI.TRADES.TRADE.ADD":
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        Items.Add(t);
                        this.Title = "Trades ( " + Items.Count + " )";
                    }
                   ));
                    break;
                case "UI.TRADES.TRADE.DELETE":
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        Items.Remove(t);
                        this.Title = "Trades ( " + Items.Count + " )";
                    }
                    ));
                    break;
                case "UI.TRADES.TRADE.UPDATE":
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        Items.Update(t);
                        this.Title = "Trades ( " + Items.Count + " )";
                    }
                    ));
                    break;
            }
        }       
        // does not work uiContext == null
        private void CallbackTradeOperation1(object sender, IEventArgs args)
        {
            var t = args?.Object as ITrade3;
            if (t == null)
                return;

            // Does not work uiContext == null
            var uiContext = SynchronizationContext.Current;
            uiContext?.Send(x => TradeProcessing(t, args.OperationKey), null);
        }

        private void TradeProcessing(ITrade3 t, string operation)
        {
            switch (operation)
            {
                case "UI.TRADES.TRADE.ADD":
                    Items.Add(t);
                    this.Title = "Trades ( " + Items.Count + " )";
                    break;
                case "UI.TRADES.TRADE.DELETE":
                    Items.Remove(t);
                    this.Title = "Trades ( " + Items.Count + " )";
                    break;
                case "UI.TRADES.TRADE.UPDATE":
                    Items.Update(t);
                    this.Title = "Trades ( " + Items.Count + " )";
                    break;
            }
        }
    }
}
