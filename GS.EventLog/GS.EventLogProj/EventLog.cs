using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using GS.Interfaces;

// using SG.Trade.Process;

namespace GS.EventLog
{
    public delegate void EventLogToObserve();

    public class EventLog : IEventLog
    {
        private DateTime _lastGetRequestDateTime;
        private long _lastGetRequestIndex;
        public ObservableCollection<EventLogItem> EventLogObserveCollection;
        public List<EventLogItem> EventLogCollection;

        public ObservableCollection<EventLogItem> Os { get { return EventLogObserveCollection; } }

        public EventLogToObserve CallbackGetEventLogToObserve;

        //private Thread _observeThread;

        //public SimpleProcess ObserveProcess;

        //private volatile int _myIndex;
        private long _myIndex;
        private readonly object _locker;

        public EventLog()
        {
            _lastGetRequestDateTime = DateTime.Now;
            _lastGetRequestIndex = 0;

            EventLogObserveCollection = new ObservableCollection<EventLogItem>();
            EventLogCollection = new List<EventLogItem>();
            _myIndex = 0;

            _locker = new object();
            //_observeThread = new Thread( ObserveProcess );

            //ObserveProcess = new SimpleProcess("Observe EventLog", 5, 3, ExecuteObserveProcess);

            AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "EventLog", "Initialization", "", "");
        }
        private void Add(EventLogItem eli)
        {
            //if( eli == null ) throw new NullReferenceException("EventLogItem is Null");
            lock (_locker)
            {   
                    //EventLogCollection.Insert(0,eli); //Add(eli);
                    eli.MyIndex = ++_myIndex;
                    EventLogCollection.Add(eli);
            }
        }
        public void AddItem(EvlResult result, string operation, string description)
        {
            Add(new EventLogItem
                    {
                        DT = DateTime.Now,
                        ResultCode = result,
                        Operation = operation,
                        Description = description
                    });
        }

        public void AddItem(EvlResult result, EvlSubject subject, 
                                    string source, string operation, string description, string sobject)
        {
            Add(new EventLogItem
                    {
                        DT = DateTime.Now,
                        ResultCode = result,
                        Subject = subject,
                        Source = source,
                        Operation = operation,
                        Description = description,
                        Object = sobject
                    });
        }
        public void Clear()
        {
            lock (_locker)
            {
                EventLogCollection.Clear();
                _myIndex = 0;
            }
        } 
        public void ExecuteObserveProcess()
        {
            if (CallbackGetEventLogToObserve != null) CallbackGetEventLogToObserve();
        }
        public void GetEventLogToObserve()
        {
            lock (_locker)
            {
                var i = _lastGetRequestIndex; 
                foreach (var eli in EventLogCollection.Where(eli => eli.MyIndex > i))
                {
                    EventLogObserveCollection.Insert(0,eli);
                    _lastGetRequestIndex = eli.MyIndex;
                }
            }
        }
        public List<string> GetEventLogItems(EnumGetLogItem howmany)
        {
            var l = new List<string>();
            lock (this)
            {
                var cnt = EventLogCollection.Count;
                if (cnt > 0)
                {
                    var lastrequestDateTime = EventLogCollection[EventLogCollection.Count - 1].DT;
                    switch (howmany)
                    {
                        case EnumGetLogItem.GET_LAST_ITEM: l.Add(EventLogCollection[EventLogCollection.Count - 1].ToString()); break;
                        case EnumGetLogItem.GET_LAST_ITEMS:
                            l.AddRange(from eli in EventLogCollection
                                       where eli.DT > _lastGetRequestDateTime
                                       select eli.ToString());
                            break;
                        case EnumGetLogItem.GET_LAST_WRONG_ITEMS:
                            l.AddRange(from eli in EventLogCollection
                                       where eli.DT > _lastGetRequestDateTime && eli.ResultCode < 0
                                       select eli.ToString());
                            break;
                        case EnumGetLogItem.GET_ALL_ITEMS:
                            l.AddRange(EventLogCollection.Select(eli => eli.ToString()));
                            break;
                        default:
                            //case EnumGetLogItem.GET_LAST_ITEMS:
                            l.AddRange(from eli in EventLogCollection
                                       where eli.DT > _lastGetRequestDateTime
                                       select eli.ToString());
                            break;
                    }
                    _lastGetRequestDateTime = lastrequestDateTime;
                }
            }
            return l;
        }
        public void ClearSomeData(int count)
        {
            lock (_locker)
            {
                while (EventLogCollection.Count > count)
                {
                    EventLogCollection.RemoveAt(EventLogCollection.Count - 1);
                }
            }
        }
    }
    public class EventLogItem
    {
        public DateTime DT { get; set; }
        public EvlResult ResultCode { get; set; }
        public EvlSubject Subject { get; set; }
        public string Source { get; set; }
        public string Operation { get; set; }
        public string Description { get; set; }
        public string Object { get; set; }
        public long MyIndex { get; set; }

        public string DateTimeString { get { return DT.ToString("G"); } }
        public string TimeDateString
        {
            get { return DT.ToString("T") + ' ' + DT.ToString("d"); }
        }

        //private static long _maxindex = 0;

        public EventLogItem() { }
        
        private string GetResultCodeStr()
        {
            string s;
            switch ( ResultCode )
            {
                case EvlResult.FATAL :       s = "- Fatal    -"; break;
                case EvlResult.SOS:          s = "- SOS      -"; break;
                case EvlResult.WARNING:      s = "- Warning  -"; break;
                case EvlResult.SUCCESS :     s = "- Success  -"; break;
                default :                    s = "- Unknown  -"; break;
            }
            return s;
        }
        
        public override string ToString()
        {
            return String.Format("{0:G} {1} {2} {3} {4} {5} {6} {7}", DT, GetResultCodeStr(), Subject, Source, Operation, Description, Object, MyIndex);
        }
    }
}
