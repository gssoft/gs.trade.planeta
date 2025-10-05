using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Elements;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using GS.WorkTasks;
using EventArgs = System.EventArgs;

namespace GS.ProcessTasks
{
    public class ProcessTask02 : Element34<string, IEventArgs1, IEventArgs1>, IDisposable
    {
        protected bool Working { get; set; }
        public bool IsWorking => Working;
        public bool IsStopByCancelToken { get; set; }

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
        public System.Threading.Tasks.Task Task { get; private set; }
        public System.Threading.Tasks.TaskStatus TaskStatus => Task?.Status ?? TaskStatus.Faulted;
        public bool TaskIsFinised => Task == null || (Task.IsCompleted || Task.IsCanceled || Task.IsFaulted);

        private WorkTaskStatus _workTaskStatus;
        [XmlIgnore]
        public WorkTaskStatus WorkTaskStatus {
            get { return _workTaskStatus; }
            private set
            {
                if (_workTaskStatus == value)
                    return;
                _workTaskStatus = value;
                var dtnow = DateTime.Now;
                var dtold = LastStatusChangedDateTime;
                LastStatusChangedDateTime = dtnow;
                LastStatusChangedElapsed = LastStatusChangedDateTime - dtold;
            }
        }
        public string WorkTaskStatusStr =>
            $"WorkTaskStatus: {WorkTaskStatus}, " +
            $"Changed: {LastStatusChangedDateTime.ToString("HH:mm:ss yy-MM-dd")}, " +
            $"Elapsed: {LastStatusChangedElapsed.ToString(@"hh\:mm\:ss\.fff")}";
        public DateTime LastStatusChangedDateTime { get; private set; }
        public TimeSpan LastStatusChangedElapsed { get; private set; }

        [XmlIgnore]
        public int SuccessCount { get; private set; }
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
        public TaskStopReasonEnum TaskStopReason { get; private set; }
        public string TaskStopReasonStr => $"TaskStopReason: {TaskStopReason}";

        public int TimeInterval { get; set; }

        public int SecondsToWaitCompleting
            => SecondsWaitingCompleting > 0 
                    ? SecondsWaitingCompleting 
                    : SecondsWaitingCompletingDefault;

        private const int SecondsWaitingCompletingDefault = 15;
        public int SecondsWaitingCompleting { get; set; }
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
            ?   $"{WorkTaskStatusStr}, TaskStatus: {Task.Status}, " + 
                $"IsCanceled: {Task.IsCanceled}, " +
                $"IsCompleted: {Task.IsCompleted}, " + 
                $"IsFaulted: {Task.IsFaulted}"
            :   $"Status: {WorkTaskStatus} Task is Null";

        public bool IsUpToDate => 
            DateTime.Now - LastTryWorkDateTime < TimeSpan.FromSeconds(2 * TimeInterval);

        public bool IsErrorLimitExceeded => ErrorCount > ErrorCountToStop;

        public ProcessTask02()
        {
            IsEnabled = true;
            AutoReset = new AutoResetEvent(false);
        }
        public override void Init()
        {
            base.Init();
            IsEnabled = true;

            //CreateTask();
        }
        public void Start()
        {
            // ConsoleAsync.WriteLineT("WorkTask: {0} Task Starting...", Code);
            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, GetType().FullName, "Task","Starting()", TaskStatusStr, "");

            TaskStopReason = TaskStopReasonEnum.Unknown;

            WorkTaskStatus = WorkTaskStatus.TryToStart;

            Working = false;

            if (!IsEnabled)
                return;

            ClearCounts();

            if (!CreateTask())
                return;

            Working = true;

            Task.ContinueWith(TaskContinueProcess);

            Task.Start();

            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, GetType().FullName, "Task", "Starting()", TaskStatusStr, "");
        }

        public void ReStart()
        {
            Working = false;
            if (!IsEnabled)
                return;
            if (!CreateTask())
                return;
            Working = true;
            Task.ContinueWith(TaskContinueProcess);
            Task.Start();
        }

        public void Stop()
        {
            if (Task == null)
                return;

            WorkTaskStatus = WorkTaskStatus.TryToStop;

            AutoReset.Set();
            if (IsStopByCancelToken)
            {
                TaskStopReason = TaskStopReasonEnum.CancelRequest;
                Cts.Cancel();
            }
            else
            {
                TaskStopReason = TaskStopReasonEnum.CompleteRequest;
                Working = false;
            }
            AutoReset.Set();

            // Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, GetType().Name, "Task", "Stop()", TaskStatusStr, "");

            System.Threading.Tasks.Task.Factory
                            .StartNew(WaitingForCompletion);
        }

        private void WaitingForCompletion()
        {
            if (Task == null)
                return;

            var dt1 = DateTime.Now;
            var dtexit = dt1.AddSeconds(SecondsToWaitCompleting);
            while (!TaskIsFinised && dtexit.IsGreaterThan(DateTime.Now))
            {
                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, GetType().Name,
                    MethodBase.GetCurrentMethod().Name, "TryToStop", TaskStatusStr, TaskStopReasonStr);

                Thread.Sleep(1*1000);
            }
            var dt2 = DateTime.Now;
            if (!TaskIsFinised)
            {
                  Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, GetType().Name,
                        MethodBase.GetCurrentMethod().Name,
                        $"Failed Stop the Task in {SecondsToWaitCompleting} Seconds",
                        TaskStatusStr, $"Elapsed: {(dt2 - dt1).ToString(@"hh\:mm\:ss\.fff")}, {TaskStopReasonStr}");
                return;
            }           
            WorkTaskStatus = WorkTaskStatus.Completed;
            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, GetType().Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name,
                "Task Completed", TaskStatusStr,
                $"Elapsed: {(dt2 - dt1).ToString(@"hh\:mm\:ss\.fff")}, {TaskStopReasonStr}");
        }

        private bool CreateTask()
        {
            if (Task != null &&
                (Task.Status == TaskStatus.Running || Task.Status == TaskStatus.RanToCompletion))
                return false;
            TaskStopReason = TaskStopReasonEnum.Unknown;

            Cts = new CancellationTokenSource();
            CancellationToken = Cts.Token;

            Task = new System.Threading.Tasks.Task(WorkAction, CancellationToken);

            if (Task != null)
            {
                WorkTaskStatus = WorkTaskStatus.Created;
                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, GetType().FullName, "Task", "Created()", TaskStatusStr, "");
            }
            return true;
        }
        private void WorkAction()
        {
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                    "Start()", TaskStatusStr, ToString());

            WorkTaskStatus = WorkTaskStatus.Started;

            LaunchedDateTime = DateTime.Now;
            OnStartTaskEvent();

            var timeinterval = TimeInterval > 0 ? TimeInterval : Timeout.Infinite;

            while (Working)
            {
                if (IsStopByCancelToken)
                    CancellationTokenVerify();

                //Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                //"Working...", TaskStatusStr, ToString());

                if (IsEnabled)
                {
                    WorkTaskStatus = WorkTaskStatus.Working;

                    LastWorkDateTime = DateTime.Now;
                    // ConsoleSync.WriteLineT("ProcessTask working ...");
                    try
                    {
                        DeQueueProcess();
                    }
                    catch (Exception e)
                    {
                        SendException(e);
                    }
                }

                if (IsStopByCancelToken)
                    CancellationTokenVerify();

                AutoReset.WaitOne(timeinterval);
            }
            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, GetType().FullName,
                System.Reflection.MethodBase.GetCurrentMethod().Name,
                "Working cycle is Complete", TaskStatusStr, TaskStopReasonStr);
        }

        private void CancellationTokenVerify()
        {
            if (!CancellationToken.IsCancellationRequested)
                return;

            IsEnabled = false;

            // ConsoleAsync.WriteLineT("WorkTask: {0} Task is Stopped with Pleasure.\r\nStopReason: {1}", Code, StopReason);
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentName, TypeName,
                MethodBase.GetCurrentMethod().Name, TaskStatusStr, TaskStopReasonStr);

            //TaskStopReason = TaskStopReasonEnum.CancelRequest;

            AutoReset.Set();
            // AutoReset.Dispose();
            // Task Status Changed
            OnChangedEvent(new GS.Events.EventArgs
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
        }

        private void TaskContinueProcess(System.Threading.Tasks.Task t)
        {
             //ConsoleAsync.WriteLineT("Task Continue Process:\r\nTask.Status: {0}," +
             //                       "\r\nIsComplete: {1}," +
             //                       "\r\nIsCanceled: {2}," +
             //                       "\r\nIsFaulted: {3}",
             //               Task.Status, Task.IsCompleted, Task.IsCanceled, Task.IsFaulted);

            // Works?.Finish();

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
        public override void DoWork()
        {
            AutoReset.Set();
        }
        public override void ItemsProcessing(IEnumerable<IEventArgs1> items)
        {
            foreach (var i in items)
            {
                try
                {
                    i.ProcessingAction(i);
                    SuccessCount++;
                }
                catch (Exception e)
                {
                    SendException(e);
                    ErrorCount++;
                }               
            }
        }

        private void ClearCounts()
        {
            SuccessCount = 0;
            ErrorCount = 0;
        }

        public double PrcntSuccessPass => SuccessCount + ErrorCount > 0 ? SuccessCount/(SuccessCount + ErrorCount) : 0;

        public override string Key => Code;

        public void Dispose()
        {
            AutoReset?.Set();
            AutoReset?.Dispose();
            Working = false;
        }
    }
}
