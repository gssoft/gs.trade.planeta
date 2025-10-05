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
using EventArgs = System.EventArgs;

namespace GS.Trade.Windows
{
    /// <summary>
    /// Логика взаимодействия для EventLog_Window2.xaml
    /// </summary>
    public partial class EventLogWindow2 : Window
    {
        private const int CapasityVal = 5120;
        private const int CapasityLimitVal = 5120;

        //private const int CapasityVal = 1024;
        //private const int CapasityLimitVal = 256;

        //private const int CapasityVal = 256;
        //private const int CapasityLimitVal = 64;

        // private ITradeContext _tx;
        protected IEventLog EventLog;

        public int Capasity { get; set; }
        public int CapasityLimit { get; set; }

        private readonly object _locker;
        protected ObservableCollection<IEventLogItem> EventLogItems { get; set; }
        protected ObservableListCollection<long, IEventLogItem> Items { get; set; }

        public EventLogWindow2()
        {
            InitializeComponent();

            _locker = new object();
            EventLogItems = new ObservableCollection<IEventLogItem>();
        }
        public void Init(IEventLog evl)
        {
            if (evl == null)
                throw new NullReferenceException("TradesWindow.Init(EventLog == null)");

            Capasity = CapasityVal;
            CapasityLimit = CapasityLimitVal;

            //Items = new ObservableListCollection<long, IEventLogItem>
            //{
            //    Code = "EventLogWindowCollection",
            //    Name = "EventLogWindowCollection",
            //    Category = "EventLogWindow",
            //    Entity = "EventLogItem",
            //    Capasity = CapasityVal,
            //    CapasityLimit = CapasityLimitVal,
            //    IsReversed = true,
            //    EventLog = evl,
            //    IsEvlEnabled = false
            //};

            EventLog = evl;
            EventLog.EventLogChangedEvent += CallbackEventLogOperation;
            EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                "EventLogWindow", "EventLogWindow","Init","","");
            //_tx = tx;
            //_tx.Evlm(EvlResult.SUCCESS, EvlSubject.TRADING, "Trades3ActiveWindow", "Initialization", "", "");
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            LstEventLog.ItemsSource = EventLogItems;
            //LstEventLog.ItemsSource = Items.Collection;

            //if (EventLog == null)
            //    throw new NullReferenceException("TradesWindow.Init(Tx == null)");
            
            //_tx.EventHub.Subscribe("UI.EventLogItems", "EventLogItem", CallbackEventLogOperation);
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            Clear();
            if (EventLog != null)
                EventLog.EventLogChangedEvent -= CallbackEventLogOperation;
                //_tx.EventHub.UnSubscribe("UI.EventLogItems", "EventLogItem", CallbackEventLogOperation);
        }

        public void Clear()
        {
            lock (_locker)
                EventLogItems.Clear();
        }

        public void Refresh()
        {
            Clear();
            GetEventLogItems();
        }

        public void GetEventLogItems()
        {
            var evls = EventLog.Items;
            lock (_locker)
            {
                foreach (var p in evls)
                {
                    EventLogItems.Insert(0, p);
                }
            }
        }

        private void CallbackEventLogOperation(object sender, IEventArgs args)
        {
            if (args.Object == null)
                return;

            var evli = args.Object as IEventLogItem;
            if (evli == null)
                return;

            switch (args.OperationKey)
            {
                case "UI.EVENTLOGITEMS.EVENTLOGITEM.ADD":
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        lock (_locker)
                        {
                            EventLogItems.Insert(0, evli);
                            if (Capasity != 0 && (CapasityLimit + Capasity) <= EventLogItems.Count)
                                ClearSomeData(Capasity);
                            //Items.Add(evli);
                        }
                    }
                   ));
                    break;
                //case "UI.EVENTLOGITEMS.EVENTLOGITEM.DELETE":
                //    Dispatcher.BeginInvoke((ThreadStart)(() =>
                //    {
                //        lock (_locker)
                //        {
                //            var ot = EventLogItems.FirstOrDefault(it => it.Key == t.Key);
                //            if (ot != null)
                //                EventLogItems.Remove(ot);
                //        }
                //    }
                //    ));
                //    break;
                //case "UI.EVENTLOGITEMS.EVENTLOGITEM.UPDATE":
                //    Dispatcher.BeginInvoke((ThreadStart)(() =>
                //    {
                //        lock (_locker)
                //        {
                //            var ot = EventLogItems.FirstOrDefault(it => it.Key == t.Key);
                //            if (ot != null)
                //                EventLogItems.Remove(ot);
                //            EventLogItems.Insert(0, t);
                //        }
                //    }
                //    ));
                //    break;
            }
        }
        private void ClearSomeData(int count)
        {
            //lock (_locker)
            //{
                while (EventLogItems.Count > count)
                {
                    EventLogItems.RemoveAt(EventLogItems.Count - 1);
                }
            //}
                EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "EventLogWindow", "EventLogWindow", "ClearSomeData()",
                String.Format("Capasity={0}; Limit={1}; ItemsCount={2}",
                Capasity, CapasityLimit, EventLogItems.Count), "");
        }
    }
}
