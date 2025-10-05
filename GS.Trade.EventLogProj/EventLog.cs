using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using GS.Trade.Interfaces;

// using SG.Trade.Process;

namespace GS.Trade.EventLog
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
        private object _locker;

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

            AddItem(EnumEventLog.SUCCESS, EnumEventLogSubject.TRADING, "EventLog", "Initialization", "", "");
        }
        public void Add(EventLogItem eli)
        {
            lock (_locker)
            {   
                    //EventLogCollection.Insert(0,eli); //Add(eli);
                    eli.MyIndex = ++_myIndex;
                    EventLogCollection.Add(eli);
            }
        }
        public void AddItem(EnumEventLog result, string operation, string description)
        {
            Add(new EventLogItem(result,  operation, description));
        }
        public void AddItem(EnumEventLog result, EnumEventLogSubject subject, 
                                    string source, string operation, string description, string objects)
        {
            Add(new EventLogItem(result, subject, source, operation, description, objects));
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
        /*
        public void StartObserve()
        {
            AddItem(EnumEventLog.SUCCESS, EnumEventLogSubject.TECHNOLOGY, "EventLog", "Start Observe Process", "", "");
            ObserveProcess.Start();
        }
        public void StopObserve()
        {
            AddItem(EnumEventLog.SUCCESS, EnumEventLogSubject.TECHNOLOGY, "EventLog", "Stop Observe Process", "", "");
            ObserveProcess.Stop();
        }
        */ 
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
    }
    public class EventLogItem
    {
        public DateTime DT { get; set; }
        public EnumEventLog ResultCode { get; set; }
        public EnumEventLogSubject Subject { get; set; }
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
        public EventLogItem(EnumEventLog result, string source, string description)
        {
            DT = DateTime.Now;
            ResultCode = result;
            Source = source;
            Description = description;
          //  lock(this)
          //  {
          //      MyIndex = ++_maxindex;    
          //  }          
        }
        public EventLogItem(EnumEventLog result, EnumEventLogSubject area, string source, string operation, string description)
        {
            DT = DateTime.Now;
            ResultCode = result;
            Subject = area;
            Source = source;
            Operation = operation;
            Description = description;
          //  lock (this)
          //  {
          //      MyIndex = ++_maxindex;
          //  }
        }
        public EventLogItem(EnumEventLog result, EnumEventLogSubject area, string source, string operation, string description, string obj)
        {
            DT = DateTime.Now;
            ResultCode = result;
            Subject = area;
            Source = source;
            Operation = operation;
            Description = description;
            Object = obj;
          //  lock (this)
          //  {
           //     MyIndex = ++_maxindex;
           // }
        }
        private string GetResultCodeStr()
        {
            string s;
            switch ( ResultCode )
            {
                case EnumEventLog.FATAL :       s = "- Fatal    -"; break;
                case EnumEventLog.SOS:          s = "- SOS      -"; break;
                case EnumEventLog.WARNING:      s = "- Warning  -"; break;
                case EnumEventLog.SUCCESS :     s = "- Success  -"; break;
                default :                       s = "- Unknown  -"; break;
            }
            return s;
        }
        
        public override string ToString()
        {
            return String.Format("{0:G} {1} {2} {3} {4} {5} {6} {7}", DT, GetResultCodeStr(), Subject, Source, Operation, Description, Object, MyIndex);
        }
    }
}
