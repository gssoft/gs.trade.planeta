using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Events;
using GS.Identity;
using GS.Interfaces;
using GS.Queues;
using Microsoft.SqlServer.Server;
using EventArgs = GS.Events.EventArgs;

// using SG.Trade.Process;
// using WebEventLogClient;

namespace GS.EventLog
{
    public delegate void EventLogToObserve();

    public class EventLog : Evl, IEventLog //, IHaveQueue<IEventLogItem>
    {
        [XmlIgnore]
        public List<IEventLogItem> EventLogCollection;

        public int Capasity { get; set; }
        public int CapasityLimit { get; set; }

        private readonly object _locker;
        public EventLog()
        {
            EventLogCollection = new List<IEventLogItem>();

            _locker = new object();

            AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "EventLog", "EventLog", "Initialization", "", "");
        }

        public override IEnumerable<IEventLogItem> Items
        {
            get
            {
                lock ( _locker )
                {
                    return EventLogCollection.ToList();
                }
            }
        }

        public void AddItem(IEventLogItem eli)
        {
            //if( eli == null ) throw new NullReferenceException("EventLogItem is Null");
            if (!IsEnabled)
                return;
            if(EventLogs == null || eli.Index == 0)
                    eli.Index = Next;
            //SendExceptionMessage("EventLog","TestException", "ExceptionSource");
            if (IsAsync)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (IsSaveEnabled)
                        {
                            lock (_locker)
                            {
                                //EventLogCollection.Insert(0,eli); //Add(eli);
                                EventLogCollection.Add(eli);
                            }
                            if(Capasity != 0 && (CapasityLimit + Capasity) <= EventLogCollection.Count )
                                ClearSomeData(Capasity);
                        }
                        if (IsUIEnabled)
                        {
                            FireUIEvent(eli);
                        }
                    }
                    catch (Exception e)
                    {
                        SendExceptionMessage3("EventLog", "EventLog.AddItem()", "", "", e);
                        throw;
                    }
                });
            }
            else
            {
                try
                {
                    if (IsSaveEnabled)
                    {
                        lock (_locker)
                        {
                            //EventLogCollection.Insert(0,eli); //Add(eli);
                            EventLogCollection.Add(eli);
                        }
                        if (Capasity != 0 && (CapasityLimit + Capasity) <= EventLogCollection.Count)
                            ClearSomeData(Capasity);
                    }
                    // 2018.05.28
                    //if (IsUIEnabled)
                    //{
                    //    FireUIEvent(eli);
                    //}
                }
                catch (Exception e)
                {
                    SendExceptionMessage3(Name, "EventLog.AddItem()", "", "", e);
                }

            }
        }
        public void AddItem(EvlResult result, string operation, string description)
        {
            var evli = new EventLogItem
            {
                DT = DateTime.Now,
                ResultCode = result,
                Operation = operation,
                Description = description,
                Index = Convert.ToInt64(Identity.Next())
            };
            //if (IsQueueEnabled)
            //    Push(evli);
            //else
                AddItem(evli);
        }
        public void AddItem(EvlResult result, EvlSubject subject, 
                                    string source, string operation, string description, string sobject)
        {
            var evli = new EventLogItem
            {
                DT = DateTime.Now,
                ResultCode = result,
                Subject = subject,
                Source = source,
                Operation = operation,
                Description = description,
                Object = sobject,
                Index = Convert.ToInt64(Identity.Next())
            };
            //if (IsQueueEnabled)
            //    Push(evli);
            //else
                AddItem(evli);
        }
        public void AddItem(EvlResult result, EvlSubject subject, string source, string entity, string operation, string description,
                            string sobject)
        {
            var evli = new EventLogItem
            {
                DT = DateTime.Now,
                ResultCode = result,
                Subject = subject,
                Source = source,
                Entity = entity,
                Operation = operation,
                Description = description,
                Object = sobject,
                Index = Convert.ToInt64(Identity.Next())
            };
            //if (IsQueueEnabled)
            //    Push(evli);
            //else
                AddItem(evli);
        }
        public void Clear()
        {
            lock (_locker)
            {
                EventLogCollection.Clear();
            }
        }
        public IEventLog Primary { get { return this; } }

        public override long GetEventLogItemsCount()
        {
            lock (_locker)
            {
                return EventLogCollection.Count;
            }
        }
        public override long GetEventLogItems(long fromIndex, List<IEventLogItem> itemList)
        {
            try
            {
                lock (_locker)
                {
                    itemList.AddRange(EventLogCollection.Where(eli => eli.Index > fromIndex));
                    return itemList.Count;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public void ClearSomeData(int count)
        {
            lock (_locker)
            {
                while (EventLogCollection.Count > count)
                {
                    //EventLogCollection.RemoveAt(EventLogCollection.Count - 1);
                    EventLogCollection.RemoveAt(0);
                }
            }
            AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, "MemEventLog", "ClearSomeData()",
                String.Format("Capasity={0}; Limit={1}; ItemsCount={2}",
                Capasity,CapasityLimit,EventLogCollection.Count), "");
        }

        public override string ToString()
        {
            return string.Format("[Type:{0}, Name:{1}, ASync:{2}, Enable:{3}]",
                                    GetType(), Name, IsAsync, IsEnabled);
        }      
    }
    public class EventLogItem : IEventLogItem
    {
        public DateTime DT { get; set; }
        public EvlResult ResultCode { get; set; }
        public EvlSubject Subject { get; set; }
        public string Source { get; set; }
        public string Entity { get; set; }
        public string Operation { get; set; }
        public string Description { get; set; }
        public string Object { get; set; }
        public long Index { get; set; }
      //  public long MyIndex { get; set; }

        public string DateTimeString { get { return DT.ToString("G"); } }
        public string TimeDateString
        {
            get { return DT.ToString("T") + ' ' + DT.ToString("d"); }
        }

        //private static long _maxindex = 0;

        public EventLogItem() { }
        
    
        public override string ToString()
        {
            return
                $"{DT:G} {ResultCode.ToString()} {Subject.ToString()} {Source} {Entity} {Operation} {Description} {Object} {Index}";
        }
        public long Key => Index;
    }
}
