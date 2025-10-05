using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using GS.Elements;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using GS.Interfaces.Service;
using GS.Serialization;
using GS.Trade.Windows;
using GS.WorkTasks;

namespace GS.Contexts
{
    using WorkTasks = GS.WorkTasks.WorkTasks;
    public interface IContext1
    {
        IEventLog EventLog { get; }
        IEventHub EventHub { get; }
        WorkTasks4 WorkTasks { get; }

        void Init();
        void Start();
        void Stop();
        void DoWork();
    }

    public abstract class Context1 : Element3<string, IEventArgs>, IContext1
    {
        public override string Key => Code.HasValue() ? Code : GetType().FullName;

        public IEventHub EventHub { get; set; }
        public WorkTasks4 WorkTasks { get; set; }

        public bool IsNeedEventLog { get; set; }
        public bool IsNeedEventHub { get; set; }
        public bool IsNeedWorkTasks { get; set; }

        public override void Init()
        {
            try
            {
                EventLog = Builder.Build2<IEventLog>(@"Init\EventLog.xml", "EventLogs");
                EventLog.Parent = this;
                EventLog.Init();

                StartEventLogWindow();

               ((IServiceSimple)EventLog)?.Start();

                EventLog?.Evlm52(EvlResult.SUCCESS, EvlSubject.INIT, "Init()","","" );

                // base.Init(EventLog);
                if (IsNeedWorkTasks)
                {
                    WorkTasks = Builder.Build2<WorkTasks4>(@"Init\WorkTasks.xml", "WorkTasks4");
                    WorkTasks.Init(EventLog);
                }
                if (IsNeedEventHub)
                {
                    EventHub = Builder.Build2<IEventHub>(@"Init\EventHub.xml", "EventHub");
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
            ((IServiceSimple)EventLog)?.Stop();
        }
        public virtual void DoWork()
        {
            // ChangedEvent -= EventHub.EnQueue;
            WorkTasks?.DoWork();
        }

        protected EventLogWindow3 EventLogWindow;
        protected Thread WinThread;
        private void StartEventLogWindow()
        {
            WinThread = new Thread(() =>
            {
                EventLogWindow = new EventLogWindow3();
                EventLogWindow.Init(EventLog);
                EventLogWindow?.Show();

                Dispatcher.Run();
            });
            WinThread.SetApartmentState(ApartmentState.STA);
            WinThread.Start();
            Thread.Sleep(1000);
        }
    }
}
