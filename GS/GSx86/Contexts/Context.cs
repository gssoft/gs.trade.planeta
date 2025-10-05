using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using GS.Serialization;
using GS.WorkTasks;

namespace GS.Contexts
{
    using WorkTasks = GS.WorkTasks.WorkTasks;
    public interface IContext
    {
        IEventLog EventLog { get; }
        EventHub3 EventHub { get; }
        WorkTasks WorkTasks { get; }

        void Init();
        void Start();
        void Stop();
    }

    public abstract class Context : Element3<string, IEventArgs>, IContext
    {
        public override string Key => Code.HasValue() ? Code : GetType().FullName;

        public EventHub3 EventHub { get; set; }
        public WorkTasks WorkTasks { get; set; }

        public virtual void Init()
        {
            EventLog = Builder.Build2<IEventLog>(@"Init\EventLog.xml", "EventLogs");
            EventLog.Init();

            //base.Init(EventLog);

            WorkTasks = Builder.Build2<WorkTasks>(@"Init\WorkTasks.xml", "WorkTasks");
            WorkTasks.Init(EventLog);

            EventHub = Builder.Build<EventHub3>(@"Init\EventHub3.xml", "EventHub3");
            EventHub.Init(EventLog);

            Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Context", "Context", "Init()","","");

            // TO DO 2016.12.13
            // ChangedEvent += EventHub.EnQueue;
        }

        public virtual void Start()
        {
            // ChangedEvent += EventHub.EnQueue;
            WorkTasks?.Start();
        }

        public virtual void Stop()
        {
            // ChangedEvent -= EventHub.EnQueue;
            WorkTasks?.Stop();
        }
    }
}
