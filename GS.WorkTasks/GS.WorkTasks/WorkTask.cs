using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Containers5;
using GS.Elements;
using GS.Extension;

namespace GS.WorkTasks
{
    public class WorkTask : Element1<string>, IWorkTask, IDisposable
    {
        #region Events
        public event EventHandler StartTaskEvent;
        protected virtual void OnStartTaskEvent()
        {
            var handler = StartTaskEvent;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        public event EventHandler StopTaskEvent;
        protected virtual void OnStopTaskEvent()
        {
            var handler = StopTaskEvent;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        #endregion

        [XmlIgnore]
        public Task Task { get; private set; }
        [XmlIgnore]
        public Func<bool> TaskFunc { get; set; }
        [XmlIgnore]
        public int ErrorCount { get; private set; }
        public int ErrorCountToStop { get; set; }
        [XmlIgnore]
        public DateTime LaunchedDateTime { get; set; }
        [XmlIgnore]
        public DateTime LastWorkDateTime { get; private set; }
        [XmlIgnore]
        public DateTime LastTryWorkDateTime { get; private set; }
        [XmlIgnore]
        public string StopReason { get; private set; }

        public int TimeInterval { get; set; }

        protected CancellationTokenSource Cts { get; set; }
        protected CancellationToken CancellationToken{ get; set; }
        protected AutoResetEvent AutoReset{ get; set; }

        public bool IsNoActive {
            get
            {
                return
                    Task == null ||
                    (Task != null && (Task.IsCanceled || Task.IsCompleted || Task.IsFaulted));
            }
        }

        public bool IsActive {
            get
            {
                return Task != null && (
                    Task.Status == TaskStatus.Running ||
                    Task.Status == TaskStatus.RanToCompletion ||
                    Task.Status == TaskStatus.WaitingToRun ||
                    Task.Status == TaskStatus.WaitingForActivation);
            }
        }

        public bool IsUpToDate {
            get{ return ((DateTime.Now - LastTryWorkDateTime) < TimeSpan.FromSeconds(2*TimeInterval));}
        }

        public bool IsErrorLimitExceeded {
            get { return ErrorCount > ErrorCountToStop; }
        }
        public WorkTask()
        {
            IsEnabled = true;
            AutoReset = new AutoResetEvent(false);
        }
        public void Init()
        {
            IsEnabled = true;
        }

        public void Start()
        {
            if (!IsEnabled)
                return;

            StartInit();
            if( CreateTask() )
                Task.Start();
        }

        public void ReStart()
        {
            if (!IsEnabled)
                return;
            if (CreateTask())
                Task.Start();
        }

        private bool CreateTask()
        {
            if(TaskFunc == null)
                throw new NullReferenceException("TaskFunc is Null");

            if (Task != null &&
                (Task.Status == TaskStatus.Running || Task.Status == TaskStatus.RanToCompletion))
                return false;

            Cts = new CancellationTokenSource();
            CancellationToken = Cts.Token;

            Task = new Task(() =>
            {
                //ConsoleAsync.ConsoleAsync.WriteLine("Task is Started");
                LaunchedDateTime = DateTime.Now;
                OnStartTaskEvent();
                var timeinterval = TimeInterval > 0 ? 1000*TimeInterval : Timeout.Infinite;
                while (true)
                {
                    try
                    {
                        if (IsEnabled)
                        {
                            // ConsoleAsync.ConsoleAsync.WriteLine("Task: Main Cycle");
                            LastWorkDateTime = DateTime.Now;
                            var ret = TaskFunc();
                            if (!ret)
                            {
                                if (++ErrorCount > ErrorCountToStop)
                                {
                                    IsEnabled = false;
                                    StopReason = String.Format("Max Errors Limit: {0} is Exceeded. ErrorCount: {1}",
                                        ErrorCountToStop, ErrorCount);
                                    Stop();
                                    // return;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        IsEnabled = false;
                        StopReason = String.Format("Exception:\r\n {0}", e.ExceptionMessage());
                        OnExceptionEvent(new Events.EventArgs
                        {
                            Category = "Exceptions",
                            Entity = "Exception",
                            Operation = "New",
                            Sender = this,
                            Object = e,
                            IsHighPriority = true
                        });
                        StopReason = e.Message;
                        Stop();
                        //return;
                    }

                    if (CancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("WorkTask: {0} is Stopped with Pleasure. StopReason: {1}", Code, StopReason );
                        
                        AutoReset.Set();
                        // AutoReset.Dispose();

                        OnChangedEvent(new Events.EventArgs
                        {
                            Sender = this,
                            Category = "WorkTasks",
                            Entity = "WorkTask",
                            Operation = "AddOrUpdate",
                            IsHighPriority = true,
                            Object = this
                        });
                        OnStopTaskEvent();
                        return;
                    }
                    AutoReset.WaitOne(timeinterval);
                }
            }, CancellationToken);

            return true;
        }

        public void Stop()
        {
            Cts.Cancel();
            AutoReset.Set();
        }

        private void StartInit()
        {
            StopReason = String.Empty;
        }

        public void DoWork()
        {
            AutoReset.Set();
        }

        public void TryToDoWork()
        {
            AutoReset.Set();
        }

        public override string Key
        {
            get { return Code; }
        }

        public void Dispose()
        {
            if (AutoReset != null)
            {
                AutoReset.Set();
                AutoReset.Dispose();
            }
        }
    }
}
