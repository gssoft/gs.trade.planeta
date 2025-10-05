using System;
using System.Collections.Generic;
using GS.Events;
using GS.Interfaces;

namespace GS.EventLog
{
    public class ConsoleEventLog : Evl, IEventLog
    {
        public void AddItem(EvlResult result, string operation, string description)
        {
            Console.WriteLine("{0:G} {1} {2} {3}", DateTime.Now, result, operation, description);
        }

        public void AddItem(EvlResult result, EvlSubject subject, 
                               string source, string operation, string description, string objects)
        {
            Console.WriteLine("{0:G} {1} {2} {3} {4} {5} {6}",
                DateTime.Now, result, subject, source, operation, description, objects);
        }
        public void AddItem(EvlResult result, EvlSubject subject, string source, string entity, string operation, string description,
                           string objects)
        {
            Console.WriteLine("{0:G} {1} {2} {3} {4} {5} {6} {7}",
                DateTime.Now, result, subject, source, entity, operation, description, objects);
        }

        public event EventHandler<IEventArgs> EventLogItemsChanged;

        protected virtual void OnEventLogItemsChanged(IEventArgs e)
        {
            EventHandler<IEventArgs> handler = EventLogItemsChanged;
            if (handler != null) handler(this, e);
        }

        public override IEnumerable<IEventLogItem> Items {
            get { return null; }
        }

        public void AddItem(IEventLogItem i)
        {
            Console.WriteLine("{0:G} {1} {2} {3} {4} {5} {6} {7}",
                i.DT, i.ResultCode, i.Subject, i.Source, i.Entity, i.Operation, i.Description, i.Object);
        }

        public IEventLog Primary { get { return this; } }

        public void ClearSomeData(int count)
        {
        }
        public override string ToString()
        {
            return string.Format("[Type:{0}, Name:{1}, ASync:{2}, Enable:{3}]",
                                    GetType(), Name,  IsAsync, IsEnabled);
        }
    }
}
