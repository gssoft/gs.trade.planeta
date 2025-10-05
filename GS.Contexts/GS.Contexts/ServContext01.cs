using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using GS.Elements;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using GS.Serialization;
using GS.Trade.Windows;
using GS.WorkTasks;

// The same with Context2 but without WorkTasks

namespace GS.Contexts
{
    using WorkTasks = GS.WorkTasks.WorkTasks;
    public interface IServContext : IElement1<string>
    {
       // IEventLog EventLog { get; }
        IEventHub EventHub { get; }

        void Init();
        void Start();
        void Stop();
    }

    public abstract class ServContext01 : Element3<string, IEventArgs>, IServContext
    {
        private bool _working;
        private Task WorkingTask;
        public override string Key => Code.HasValue() ? Code : GetType().FullName;
        public IEventHub EventHub { get; set; }
        public bool IsNeedEventLog { get; set; }
        public bool IsNeedEventHub { get; set; }
        
        public override void Init()
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                if (IsNeedEventLog)
                {
                    EventLog = Builder.Build2<IEventLog>(
                        AppDomain.CurrentDomain.BaseDirectory + @"Init\EventLog.xml", "EventLogs");
                    // EventLog = Builder.Build2<IEventLog>(@"Init\EventLog.xml", "EventLogs");
                    if (EventLog == null)
                        throw new NullReferenceException("EventLog == null");

                    EventLog.Parent = this;
                    EventLog.Init();

                    StartLogger();

                    ((IEventLogs)EventLog)?.Start();
                }
                if (IsNeedEventHub)
                {
                    EventHub = Builder.Build2<IEventHub>(AppDomain.CurrentDomain.BaseDirectory +  @"Init\EventHub.xml", "EventHub");
                    EventHub.Init(EventLog);
                    ChangedEvent += EventHub.EnQueue;
                    EventHub.Start();
                }
                Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName, m, "Complete", Key);
            }
            catch (Exception ex)
            {
                SendException(ex);
                Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName, m, "Exception", ex.Message);
            }
            // TO DO 2016.12.13
            // ChangedEvent += EventHub.EnQueue;
        }
        public virtual void Start()
        {
            // StartLogger();
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName, m, "Complete", Key);
            _working = true;
            try
            {
                Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName, m, "Begin", Key);
                _working = true;
                WorkingTask = Task.Factory.StartNew(() =>
                {
                    while (_working)
                    {
                        Evlm2(EvlResult.SUCCESS, EvlSubject.TESTING, ParentTypeName, TypeName, "Working",
                            $"{DateTime.Now:T}", Key);
                        Thread.Sleep(5 * 1000);
                    }
                });
                Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName, m, "Finish", Key);
            }
            catch (Exception ex)
            {
                SendException(ex);
                Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName, m, "Exception", ex.Message);
            }


        }

        private void FinishDelegate(Task t)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            Evlm2(EvlResult.SUCCESS, EvlSubject.TESTING, ParentTypeName, TypeName, m,
                 $"Working is Done {DateTime.Now:T}", t.Status.ToString());
        }

        public virtual void Stop()
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            _working = false;
            Thread.Sleep(5000);
            try
            {
                if (IsNeedEventHub)
                {
                    ChangedEvent -= EventHub.EnQueue;
                    EventHub?.Stop();
                }
                ((IEventLogs)EventLog)?.Stop();
                Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName, m, "Complete", Key);
            }
            catch (Exception ex)
            {
                SendException(ex);
                Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName, m, "Exception", ex.Message);
            }
            
            StopLogger();            
        }
        // Standard Loggin Object
        public virtual void StartLogger()
        {
             StartEventLogWindow();
        }
        public virtual void StopLogger()
        {
        //    EventLogWindow?.Close();

            WinThread?.Abort();
            WinThread?.Join();
        }

        protected Thread WinThread;
        protected EventLogWindow3 EventLogWindow;
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
