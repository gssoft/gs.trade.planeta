using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.ConsoleAS;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;
using GS.Works;



namespace GS.WorkTasks
{
    using Works = GS.Works.Works;

    public class WorkTask4 : Element1<string>, IWorkTask3, IDisposable
    {
        #region Events
        public event EventHandler StartTaskEvent;
        protected virtual void OnStartTaskEvent()
        {
            var handler = StartTaskEvent;
            handler?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler StopTaskEvent;
        protected virtual void OnStopTaskEvent()
        {
            var handler = StopTaskEvent;
            handler?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        [XmlIgnore]
        public Task Task { get; private set; }
        [XmlIgnore]
        public IWorks Works { get; set; }

        // Parent Should be Set
        //public Work Work {
        //    get { return _work; } 
        //    set
        //    {
        //        if (value == null)
        //        {
        //            _work = new Work {Code = "Unknown"};
        //            _work.Parent = this;
        //            OnExceptionEvent(new NullReferenceException(String.Format("WorkProcess: {0} Work is Null", Code)));
        //        }
        //        else
        //        {
        //            _work = value;
        //            _work.Parent = this;
        //        }
        //    }
        //}

        //[XmlIgnore]
        //public Func<bool> TaskFunc { get; set; }
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
        [XmlIgnore]
        protected CancellationToken CancellationToken { get; set; }
        protected AutoResetEvent AutoReset { get; set; }

        public bool IsNoActive => Task == null ||
                                  (Task != null && (Task.IsCanceled || Task.IsCompleted || Task.IsFaulted));

        public bool IsActive => Task != null && (
            Task.Status == TaskStatus.Running ||
            Task.Status == TaskStatus.RanToCompletion ||
            Task.Status == TaskStatus.WaitingToRun ||
            Task.Status == TaskStatus.WaitingForActivation);

        public string TaskStatusStr => Task != null
            ? $"TaskStatus: {Task.Status}, " + $"IsCanceled: {Task.IsCanceled}, " +
              $"IsCompleted: {Task.IsCompleted}, " + $"IsFaulted: {Task.IsFaulted}"
            : "Task is Null";

        public bool IsUpToDate => 
            ((DateTime.Now - LastTryWorkDateTime) < TimeSpan.FromSeconds(2 * TimeInterval));

        public bool IsErrorLimitExceeded => ErrorCount > ErrorCountToStop;

        public WorkTask4()
        {
            IsEnabled = true;
            Works = new Works
            {
                Parent = this,
                Code = FullCode + ".Works",
                WorkTask = this
            };
            AutoReset = new AutoResetEvent(false);
        }
        public void Init()
        {
            IsEnabled = true;
        }
        public void Start()
        {
            // ConsoleAsync.WriteLineT("WorkTask: {0} Task Starting...", Code);

            if (!IsEnabled)
                return;

            //StartInit();

            if (Works == null || !Works.InitWorks())
                    return;

            if (!CreateTask())
                return;

            Task.ContinueWith(TaskContinueProcess);
            Task.Start();
        }

        public void ReStart()
        {
            if (!IsEnabled)
                return;
            if (!CreateTask())
                return;
            Task.ContinueWith(TaskContinueProcess);
            Task.Start();
        }
        public void Stop()
        {
            // ConsoleAsync.WriteLineT("WorkTask: {0} Task Try To Stop ...", Code);

            Cts.Cancel();
            AutoReset.Set();
            //try
            //{
            //    Task.Wait();
            //}
            //catch (AggregateException e)
            //{
            //    Console.WriteLine("StopProcess.Task.Wait");
            //    Console.WriteLine("Exceptions: {0}", e.ExceptionMessageAgg());
            //   // throw;
            //}

            //if (Work != null)
            //    Work.Finish();
        }

        private void StartInit()
        {
            StopReason = string.Empty;
        }

        private bool CreateTask()
        {
            if (Works == null)
                throw new NullReferenceException("Work is Null");

            if (Task != null &&
                (Task.Status == TaskStatus.Running || Task.Status == TaskStatus.RanToCompletion))
                return false;

            Cts = new CancellationTokenSource();
            CancellationToken = Cts.Token;

            Task = new Task(() =>
            {
                // ConsoleAsync.WriteLineT("WorkTask: {0}: Task is Started", Code);

                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeFullName, TypeName,
                    "Start()", TaskStatusStr, ToString());

                LaunchedDateTime = DateTime.Now;
                OnStartTaskEvent();
                var timeinterval = TimeInterval > 0 ? TimeInterval : Timeout.Infinite;

                //if (Work != null)
                //Work.Init();

                while (true)
                {
                    //try
                    //{
                    CancellationTokenVerify();

                        if (IsEnabled)
                        {
                            // Console.WriteLine("Task is Running: Main Cycle");

                            LastWorkDateTime = DateTime.Now;
                            if (!Works.Main())
                            {
                                IsEnabled = false;
                                StopReason =
                                    $"WorkTask: {Code} Task Try To Stop;\r\nReason: Work: {Works.Code}: Error in Work.Main. work.Meassage: {Works.Message}";
                                Stop();
                            }
                           
                           // throw new NullReferenceException("Task Exception Raised");
                        }
                    //}
                    //catch (Exception e)
                    //{
                    //    IsEnabled = false;
                    //    StopReason = String.Format("Exception:\r\n {0}", e.ExceptionMessage());
                    //    OnExceptionEvent(e);
                    //    StopReason = e.Message;
                    //    Stop();
                    //    //return;
                    //}

                    //if (CancellationToken.IsCancellationRequested)
                    //{
                    //    IsEnabled = false;
                    //     StopReason = "CancellationRequest";
                    //    // ConsoleAsync.WriteLineT("WorkTask: {0} Task is Stopped with Pleasure.\r\nStopReason: {1}", Code, StopReason);
                    //    Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullCode, Code, "Stop()", TaskStatusStr, "StopReason: " + StopReason);

                    //    //if (Work != null)
                    //    // Work.Finish();

                    //    AutoReset.Set();
                    //    // AutoReset.Dispose();
                    //    // Task Status Changed
                    //    OnChangedEvent(new Events.EventArgs
                    //    {
                    //        Sender = this,
                    //        Category = "WorkTasks",
                    //        Entity = "WorkTask",
                    //        Operation = "AddOrUpdate",
                    //        IsHighPriority = true,
                    //        Object = this
                    //    });
                    //    OnStopTaskEvent();
                        
                    //    // Cancel
                    //    // throw new OperationCanceledException(CancellationToken);
                    //    CancellationToken.ThrowIfCancellationRequested();
                    //    return;
                    //}
                    CancellationTokenVerify();
                    AutoReset.WaitOne(timeinterval);
                    // AutoReset.WaitOne(TimeSpan.FromSeconds(45));
                }
            }, CancellationToken);

            return true;
        }

        private void CancellationTokenVerify()
        {
            if (!CancellationToken.IsCancellationRequested)
                return;

            IsEnabled = false;
            StopReason = "CancellationRequest";
            // ConsoleAsync.WriteLineT("WorkTask: {0} Task is Stopped with Pleasure.\r\nStopReason: {1}", Code, StopReason);
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeFullName, TypeName, "Stop()", TaskStatusStr, "StopReason: " + StopReason);

            //if (Work != null)
            // Work.Finish();

            AutoReset.Set();
            // AutoReset.Dispose();
            // Task Status Changed
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

            // Cancel
            // throw new OperationCanceledException(CancellationToken);
            CancellationToken.ThrowIfCancellationRequested();

            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeFullName, TypeName, "Stop()", TaskStatusStr, "StopReason: " + StopReason);
        }

        private void TaskContinueProcess(Task t)
        {
             //ConsoleAsync.WriteLineT("Task Continue Process:\r\nTask.Status: {0}," +
             //                       "\r\nIsComplete: {1}," +
             //                       "\r\nIsCanceled: {2}," +
             //                       "\r\nIsFaulted: {3}",
             //               Task.Status, Task.IsCompleted, Task.IsCanceled, Task.IsFaulted);

            Works?.Finish();

            var ae = Task.Exception;
            if (ae != null)
            {
                //ConsoleAsync.WriteLine("WorkTask: {} is Copmleted with Excpetion:\r\n{1}",
                //    Code, Task.Exception.ExceptionMessage());
                //ae.Handle((x) =>
                //{
                //    ConsoleAsync.WriteLineT("Exception: {0}", x.ExceptionMessage());
                //    return false;
                //});
                //var ae1 = ae.Flatten();
                //ae1.Handle((x) =>
                //{
                //    ConsoleAsync.WriteLineT("Exception: {0}", x.ExceptionMessage());
                //    return false;
                //});
                var str = ae.AggExceptionMessage();

                // ConsoleAsync.WriteLineT("WorkTask: {0} >>> Complete with Exception:\r\n{1}", Code, str);
            }
            // ConsoleAsync.WriteLineT("TaskCompleteProcess(): WorkTask: {0} is Completed.\r\nStatus: {1}", Code, t.Status );
        }

        public void DoWork()
        {
            AutoReset.Set();
        }

        public void TryToDoWork()
        {
            AutoReset.Set();
        }
        public override string Key => Code;

        public void Dispose()
        {
            if (AutoReset == null)
                return;
            AutoReset.Set();
            AutoReset.Dispose();
        }
    }
}
