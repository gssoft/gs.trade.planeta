using System;
using System.Collections.Generic;
using GS.Containers5;
using GS.Elements;
using GS.Interfaces.Service;
using GS.Queues;

namespace GS.Interfaces
{
    public enum EvlResult { SUCCESS = 1, WARNING = -1, SOS = -2, FATAL = -3, ATTENTION = 2, INFO = 3 }
    public enum EvlSubject { PROGRAMMING = 1, TECHNOLOGY = 2, TRADING = 3, TESTING = 4, DIAGNOSTIC = 5, INIT = 6 }

    public enum EvlModeEnum : short { Init = 1, Nominal = 2 } ;

    public interface IEventLog : IElement1<string>
    {
        event EventHandler<Events.IEventArgs> EventLogChangedEvent;
        void OnEventLogChangedEvent(Events.IEventArgs e);

        //event EventHandler<GS.Events.IEventArgs> ExceptionEvent;
        //void OnExceptionEvent(GS.Events.IEventArgs e);

        //string Name { get; }
        //IElement1<string> Parent { get; set; }

        IEnumerable<IEventLogItem> Items { get; }

        void AddItem(IEventLogItem evli);
        void AddItem(EvlResult result, string operation, string description);

        //void AddItem(EvlResult result, EvlSubject subject,
        //                     string source, string operation, string description, string objects);
        
        void AddItem(EvlResult result, EvlSubject subject,
                             string source, string entity, string operation, string description, string objects);

        //bool IsEnabled { get; }
        bool IsAsync { get; }
        bool IsPrimary { get; }
        bool IsSaveEnabled { get; }
        bool IsUIEnabled { get; }     

        IEventLog EventLogs { get; set; }

        IEventLog Primary { get; }

        void Init();
        void Init(string uri);

        //IEventLog GetPrimary();

        long GetEventLogItemsCount();
        long GetEventLogItems(long fromIndex, List<IEventLogItem> itemList);

        void SetMode(EvlModeEnum m);

        void ClearSomeData(int count);
    }

    public interface IEventLogs : IEventLog, IServiceSimple, IHaveQueue<IEventLogItem>
    {
    }

    public interface IEventLogItem : IHaveKey<long>
    {
        DateTime DT { get; }
        EvlResult ResultCode { get; }
        EvlSubject Subject { get; }
        string Source { get; }
        string Entity { get; }
        string Operation { get; }
        string Description { get; }
        string Object { get; }
        long Index { get; set; }
    }
    public interface IEventLogEvent
    {
        string Operation { get; }
        IEventLogItem EventLogItem { get; }
    }
}
