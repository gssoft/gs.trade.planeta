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
    public interface IContext0
    {
        IEventLog EventLog { get; }
        EventHub EventHub { get; }
        WorkTasks4 WorkTasks { get; }

        void Init();
        void Start();
        void Stop();
        void DoWork();
    }
    // Only ForFileComparer in StellaMaris
    public abstract class Context0 : Element3<string, IEventArgs>, IContext0
    {
        public override string Key => Code.HasValue() ? Code : GetType().FullName;

        public EventHub EventHub { get; set; }
        public WorkTasks4 WorkTasks { get; set; }

        public override void Init()
        {
            EventLog = Builder.Build2<IEventLog>(@"Init\EventLog.xml", "EventLogs");
            EventLog.Init();
            //base.Init(EventLog);

            ((IEventLogs)EventLog).Start();

            WorkTasks = Builder.Build2<WorkTasks4>(@"Init\WorkTasks.xml", "WorkTasks4");
            WorkTasks.Init(EventLog);

            EventHub = Builder.Build<EventHub>(@"Init\EventHub.xml", "EventHub");
            EventHub.Init(EventLog);

            Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Context", "Context", "Init()","","");

            // TO DO 2016.12.13
            // ChangedEvent += EventHub.EnQueue;
        }

        public void Close()
        {
            ((IEventLogs)EventLog).Stop();
        }

        public virtual void Start()
        {
            // ChangedEvent += EventHub.EnQueue;
            //
            WorkTasks?.Start();
        }

        public virtual void Stop()
        {
            // ChangedEvent -= EventHub.EnQueue;
            WorkTasks?.Stop();
        }
        public virtual void DoWork()
        {
            // ChangedEvent += EventHub.EnQueue;
            WorkTasks?.DoWork();
        }
    }
}
