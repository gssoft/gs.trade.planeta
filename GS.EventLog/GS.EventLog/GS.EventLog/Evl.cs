using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Containers;
using GS.Elements;
using GS.Events;
using GS.Exceptions;
using GS.Identity;
using GS.Interfaces;
using EventArgs = GS.Events.EventArgs;

namespace GS.EventLog
{
    public abstract class Evl : Element1<string>
    {
        public event EventHandler<GS.Events.IEventArgs> EventLogChangedEvent;
        public virtual void OnEventLogChangedEvent(IEventArgs e)
        {
            EventHandler<GS.Events.IEventArgs> handler = EventLogChangedEvent;
            if (handler != null) handler(this, e);
        }
        //public event EventHandler<GS.Events.IEventArgs> ExceptionEvent;
        //public virtual void OnExceptionEvent(IEventArgs e)
        //{
        //    EventHandler<IEventArgs> handler = ExceptionEvent;
        //    if (handler != null) handler(this, e);
        //}

        //public string Alias { get; set; }
        //public string Name { get; set; }
        //public string Code { get; set; }
        //public string Description { get; set; }

        public override string Key {
            get { return Code; }
        }

        public string Uri { get; set; }
        public string XPath { get; set; }
        public string UriXPath { get { return Uri + "@" + XPath; } }

        //public bool IsEnabled { get; set; }
        public bool IsAsync { get; set; }
        public bool IsPrimary { get; set; }

        public bool IsSaveEnabled { get; set; }
        public bool IsUIEnabled { get; set; }

        [XmlIgnore]
        public IEventLog EventLogs { get; set; }

        protected Counter Counter;
        protected static DateTimeNumberIdentity Identity;
        
        protected Evl()
        {
            Counter = new Counter();
            Identity = new DateTimeNumberIdentity(1000000);
        }

        protected long Next {
            get { 
                //return Counter.Next();
                return Identity.Next();
            }
        }
        public abstract IEnumerable<IEventLogItem> Items { get; }

        public virtual void Init()
        {
        }

        public virtual void Init(string uri)
        {
        }

        //public virtual IEventLog GetPrimary()
        //{
        //    return this as IEventLog;
        //}

        public virtual long GetEventLogItemsCount()
        {
            return 0;
        }

        public virtual long GetEventLogItems(long fromIndex, List<IEventLogItem> itemList)
        {
            return 0;
        }
        public virtual IEnumerable<IEventLogItem> GetItems()
        {
            return null;
        }
        protected void FireUIEvent(IEventLogItem evli)
        {
            var ea = new Events.EventArgs
            {
                Category = "UI.EVENTLOGITEMS",
                Entity = "EventLogItem",
                Operation = "Add",
                Object = evli
            };
            if (EventLogs == null)
                OnEventLogChangedEvent(ea);
            else
                EventLogs.OnEventLogChangedEvent(ea);
        }
        //protected virtual void SendExceptionMessage(string source, string operation, string message, string sourceExc)
        //{
        //    var ea = new Events.EventArgs
        //    {
        //        Category = "UI.Exceptions",
        //        Entity = "Exception",
        //        Operation = "Add",
        //        Object = new GSException
        //        {
        //            Source = source,
        //            Operation = operation,
        //            Message = message,
        //            SourceExc = sourceExc
        //        }
        //    };
        //    if (EventLogs == null)
        //        OnExceptionEvent(ea);
        //    else
        //        EventLogs.OnExceptionEvent(ea);
        //}
        //public void SendExceptionMessage3(string source,
        //                                    string objtype, string operation, string objstr,
        //                                    Exception e)
                                            
        //{
        //    var ea = new Events.EventArgs
        //    {
        //        Category = "UI.Exceptions",
        //        Entity = "Exception",
        //        Operation = "Add",
        //        Object = new GSException
        //        {
        //            Source = source,
        //            ObjType = objtype,
        //            Operation = operation,
        //            ObjStr = objstr,
        //            Message = e.Message,
        //            SourceExc = e.Source,
        //            ExcType = e.GetType().ToString(),
        //            TargetSite = e.TargetSite.ToString(),
        //            StackTrace = e.StackTrace
        //        }
        //    };
        //    //if(Parent == null)
        //    //    OnExceptionEvent(ea);
        //    //else
        //    //    Parent.OnExceptionEvent(ea);

        //    if (EventLogs == null)
        //        OnExceptionEvent(ea);
        //    else
        //        EventLogs.OnExceptionEvent(ea);

        //    // Evlm(EvlResult.FATAL, EvlSubject.PROGRAMMING, source, objtype, operation, e.Message, objstr);
        //}
    }
}
