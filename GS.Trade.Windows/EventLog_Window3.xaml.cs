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
using GS.Extension;
using GS.Interfaces;
using GS.Trade.Interfaces;
using EventArgs = System.EventArgs;

namespace GS.Trade.Windows
{
    /// <summary>
    /// Логика взаимодействия для EventLog_Window2.xaml
    /// </summary>
    public partial class EventLogWindow3 : Window
    {
        // 2018.05.20
        //private const int CapasityVal = 10240;
        //private const int CapasityLimitVal = 10240;
        // 2018.05.21
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

        private readonly string _title;
        public EventLogWindow3()
        {
            InitializeComponent();

            _title = Title;
            _locker = new object();
            EventLogItems = new ObservableCollection<IEventLogItem>();
        }

        
        public EventLogWindow3(string title)
        {
            InitializeComponent();

            if (title.HasValue())
                Title = _title = title;
            else
                _title = Title;

            _locker = new object();
            EventLogItems = new ObservableCollection<IEventLogItem>();
        }
        public void Init(IEventLog evl)
        {
            if (evl == null)
                throw new NullReferenceException($"{GetType().Name}: EventLog = null)");

            EventLog = evl;
            EventLog.EventLogChangedEvent += CallbackEventLogOperation1;
            // EventLog Need for Init ProcessTask !!!!

            EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.INIT,
               GetType().Name, Title, "Init Begin", "", "");

            IsProcessTaskInUse = true;
            SetupProcessTask();

            Capasity = CapasityVal;
            CapasityLimit = CapasityLimitVal;

            EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.INIT,
               GetType().Name, Title, "Init Finish", "", "");

            // SetUpProcessTaskDispather();
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            LstEventLog.ItemsSource = EventLogItems;
            ProcessTask?.Start();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            Clear();
            if (EventLog != null)
                EventLog.EventLogChangedEvent -= CallbackEventLogOperation1;

            // StopProcessTaskDispather();
            ProcessTask?.Stop();
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
        private void CallbackEventLogOperation1(object sender, IEventArgs args)
        {
            var evli = args?.Object as IEventLogItem;
            if (evli == null)
                return;

            switch (args.OperationKey)
            {
                case "UI.EVENTLOGITEMS.EVENTLOGITEM.ADD":
                    ProcessTask?.EnQueue(evli);
                break;                 
            }
        }
#region ProcessTask Try Launch From Dispatcher 
        private void SetUpProcessTaskDispather()
        {
            IsProcessTaskInUse = true;
            Dispatcher.BeginInvoke((ThreadStart)SetupProcessTask);
        }
        private void LaunchProcessTaskDispather()
        {
            Dispatcher.BeginInvoke((ThreadStart) (() =>
            {
                ProcessTask?.Start();
            }));
        }
        private void StopProcessTaskDispather()
        {
            Dispatcher.BeginInvoke((ThreadStart)(() =>
            {
                ProcessTask?.Stop();
            }));
        }
        #endregion
        #region ProcessTask ItemsProcessingHandlers
        private void InsertItemIntoObserveCollection(IEventLogItem evli)
        {
            Dispatcher.BeginInvoke((ThreadStart)(() =>
            {
                EventLogItems.Insert(0, evli);

                if (Capasity != 0 && CapasityLimit + Capasity <= EventLogItems.Count)
                    ClearSomeData(Capasity);
            }));
        }
        private void InsertItemsIntoObserveCollection(IEnumerable<IEventLogItem> evlis)
        {
            Dispatcher?.BeginInvoke((ThreadStart)(() =>
            {
                foreach (var evli in evlis)
                    EventLogItems.Insert(0, evli);

                var cnt = EventLogItems.Count;

                if (Capasity != 0 && CapasityLimit + Capasity <= cnt)
                {
                    ClearSomeData1(cnt);
                    Title = $"{_title} ( {Capasity} )";
                }
                else
                    Title = $"{_title} ( {cnt} )";
            }
            ));
        }
        #endregion
        private void ClearSomeData(int count)
        {
                while (EventLogItems.Count > count)
                {
                    EventLogItems.RemoveAt(EventLogItems.Count - 1);
                }
                EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "EventLogWindow", "EventLogWindow", "ClearSomeData()",
                    $"Capasity={Capasity}; Limit={CapasityLimit}; ItemsCount={EventLogItems.Count}", "");
        }
        private void ClearSomeData1(int currentcnt)
        {
            var curcnt = currentcnt;
            var iterations = currentcnt - Capasity;
            while (iterations-- > 0)
            {
                // EventLogItems.RemoveAt(EventLogItems.Count - 1);
                EventLogItems.RemoveAt(currentcnt-- - 1);
            }
            //EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "EventLogWindow", "EventLogWindow", "ClearSomeData()",
            //    $"ItemsCountBefore={curcnt}, ItemsCountAfter={EventLogItems.Count}, Capasity={Capasity}, Limit={CapasityLimit};", "");
        }
    }
}
