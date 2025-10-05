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
    public interface IContext1
    {
        IEventLog EventLog { get; }
        EventHub3 EventHub { get; }
        WorkTasks4 WorkTasks { get; }

        void Init();
        void Start();
        void Stop();
        void DoWork();
    }

    public abstract class Context1 : Element3<string, IEventArgs>, IContext1
    {
        public override string Key => Code.HasValue() ? Code : GetType().FullName;

        public EventHub3 EventHub { get; set; }
        public WorkTasks4 WorkTasks { get; set; }

        public bool IsNeedEventLog { get; set; }
        public bool IsNeedEventHub { get; set; }
        public bool IsNeedWorkTasks { get; set; }

        public virtual void Init()
        {
            try
            {
                EventLog = Builder.Build2<IEventLog>(@"Init\EventLog.xml", "EventLogs");
                EventLog.Init();

                //base.Init(EventLog);

                WorkTasks = Builder.Build2<WorkTasks4>(@"Init\WorkTasks.xml", "WorkTasks4");
                WorkTasks.Init(EventLog);

                if (IsNeedEventHub)
                {
                    EventHub = Builder.Build<EventHub3>(@"Init\EventHub3.xml", "EventHub3");
                    EventHub?.Init(EventLog);
                }

                //Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, 
                //    System.Reflection.MethodBase.GetCurrentMethod().Name, "Complete", Key);
                
            }
            catch (Exception ex)
            {
                SendException(ex);
            }
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

        public virtual void DoWork()
        {
            // ChangedEvent -= EventHub.EnQueue;
            WorkTasks?.DoWork();
        }

    }
}
