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
using GS.ProcessTasks;
using GS.WorkTasks;
using EventArgs = System.EventArgs;

namespace CA_Tcp_Sockets_Serv03
{
    public class GSTask01<TInput> : Element35<string, TInput, IEventArgs1>, IDisposable
    {
        public Action<TInput> ItemProcessingAction { get; set; }
        public Action Action { get; set; }
        public Action<IEnumerable<TInput>> ItemsProcessingAction { get; set; }
        public Action IdlingCycleAction { get; set; }
        public bool  IsEveryItemPushProcessing { get; set; }
        protected bool Working { get; set; }
        public bool IsWorking => Working;
        public bool IsStopByCancelToken { get; set; }
        // public int AutoResetTimeInt { get; set; }
        public long SuccessCount { get; protected set; }
        public long ErrorCount { get; protected set; }

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
        public WorkTaskStatus WorkTaskStatus
        {
            get { return _workTaskStatus; }
            private set
            {
                if (_workTaskStatus == value)
                    return;
                _workTaskStatus = value;
                var dtnow = DateTime.Now;
                var dtold = LastStatusChangedDateTime;
                LastStatusChangedDateTime = dtnow;
                LastStatusChangedElapsed = dtnow - dtold;
            }
        }
        public DateTime LastStatusChangedDateTime { get; private set; }
        public TimeSpan LastStatusChangedElapsed { get; private set; }

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

        private const int TimeIntervalDefault = 5000;
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
            (Task != null && 
            (Task.IsCanceled || Task.IsCompleted || Task.IsFaulted));

        public bool IsActive => Task != null && (
            Task.Status == TaskStatus.Running ||
            Task.Status == TaskStatus.RanToCompletion ||
            Task.Status == TaskStatus.WaitingToRun ||
            Task.Status == TaskStatus.WaitingForActivation);

        public string WorkTaskStatusStr =>
            $"WorkTaskStatus: {WorkTaskStatus}, " +
           // $"Changed: {LastStatusChangedDateTime.ToString("HH:mm:ss yy-MM-dd")}, " +
           $"Changed: {LastStatusChangedDateTime.ToString("HH:mm:ss")}, " +
           $"Elapsed: {LastStatusChangedElapsed.ToString(@"hh\:mm\:ss\.fff")}";

        public string TaskStatusStr => Task != null
            ? 
              // $"{WorkTaskStatusStr}, " +
              $"TaskStatus: {Task.Status}, " +
              $"IsCanceled: {Task.IsCanceled}, " +
              $"IsCompleted: {Task.IsCompleted}, " +
              $"IsFaulted: {Task.IsFaulted}"
            : $"TaskStatus: is Null, {WorkTaskStatusStr}";

        public string AllStatusStr => $"{WorkTaskStatusStr}, {TaskStatusStr}";

        public bool IsUpToDate =>
            DateTime.Now - LastTryWorkDateTime < TimeSpan.FromSeconds(2*TimeInterval);

        public bool IsErrorLimitExceeded => ErrorCount > ErrorCountToStop;

        public override string ToString()
        {
            return $"Type: {GetType().FullName}, " +
                   $"TI: {TimeInterval}, " +
                   $"IsEveryItem: {IsEveryItemPushProcessing}, " +
                   $"{WorkTaskStatusStr}, " +
                   $"{TaskStatusStr}";
        }
        public GSTask01()
        {
            ItemsProcessingAction = ItemsProcessing;

            IsEnabled = true;
            AutoReset = new AutoResetEvent(false);
        }
        public override void Init()
        {
            base.Init();
            IsEnabled = true;

            //if (TimeInterval <= 0)
            //    TimeInterval = TimeIntervalDefault;
            //CreateTask();
        }
        // Default for ItemsProcessingAction
        public void ItemsProcessing(IEnumerable<TInput> items)
        {
            foreach (var i in items)
            {
                try
                {
                    if (ItemProcessingAction == null)
                    {
                        IsEnabled = false;
                        Queue.Clear();

                        Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                            MethodBase.GetCurrentMethod().Name,
                                "ItemProcessingAction IS NULL, Entering Disabled Mode", ToString());
                        break;
                    }
                    ItemProcessingAction(i);
                    SuccessCount++;
                }
                catch (Exception e)
                {
                    ErrorCount++;
                    SendException(e);
                }
            }
        }
        public override void DeQueueProcess()
        {
            // IdleCycle is a good place and time to perform additional works in An IdlingCycleAction
            if (Queue.IsEmpty)
            {
                IdlingCycleAction?.Invoke();
                return;
            }
            while (!Queue.IsEmpty)
            {
                var items = Queue.GetItems();
                if (ItemsProcessingAction == null)
                {
                    IsEnabled = false;
                    Queue.Clear();
                    Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                            MethodBase.GetCurrentMethod().Name,
                            "ItemsProcessingAction IS NULL, Entering Disabled Mode, Good Bye", ToString());
                    break;
                }
                ItemsProcessingAction(items);
            }
        }
        public void DeQueueProcess1()
        {
            while (!Queue.IsEmpty)
            {
                var items = Queue.GetItems();
                foreach (var i in items)
                {
                    try
                    {
                        ItemProcessingAction(i);
                        SuccessCount++;
                    }
                    catch (Exception e)
                    {
                        ErrorCount++;
                        SendException(e);
                    }
                }
            }
        }
        public void Start()
        {
            //if (TimeInterval == 0)
            //    TimeInterval = TimeIntervalDefault;

            if (ItemProcessingAction == null && ItemsProcessingAction == null)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                    MethodBase.GetCurrentMethod().Name,
                    "Failed to Start: (Item or Items).ProcessingAction is NULL", ToString());
                return;
            }

            Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName,
                 MethodBase.GetCurrentMethod().Name + " Begin", AllStatusStr, ToString());

            TaskStopReason = TaskStopReasonEnum.Unknown;
            WorkTaskStatus = WorkTaskStatus.TryToStart;

            Working = false;

            if (!IsEnabled)
                return;

            ClearCounts();

            if (!CreateTask())
                return;

            Working = true;

            Task?.ContinueWith(TaskContinueProcess);

            Task?.Start();

            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
                MethodBase.GetCurrentMethod().Name + " Finish", AllStatusStr, ToString());
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

            IsEnabled = false;

            WorkTaskStatus = WorkTaskStatus.TryToStop;

            Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName,
                MethodBase.GetCurrentMethod().Name, AllStatusStr, ToString());

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

            Task.Factory.StartNew(WaitingForCompletion);

            // Thread.Sleep(1000);
        }
        private void WaitingForCompletion()
        {
            if (Task == null)
                return;

            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                    MethodBase.GetCurrentMethod().Name + ": Waiting for Completing ...",
                    $"{AllStatusStr}, {TaskStopReason}", ToString());

            var dt1 = DateTime.Now;
            var dtexit = dt1.AddSeconds(SecondsToWaitCompleting);
            while (!TaskIsFinised && dtexit.IsGreaterThan(DateTime.Now))
            {
                Thread.Sleep(1*1000);

                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                    MethodBase.GetCurrentMethod().Name + ": Waiting for Completing ...",
                    $"{AllStatusStr}, {TaskStopReason}", ToString());
            }
            var dt2 = DateTime.Now;
            if (!TaskIsFinised)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                    MethodBase.GetCurrentMethod().Name,
                    $"Failed Stop the Task in {SecondsToWaitCompleting} Seconds, Elapsed: {(dt2 - dt1).ToString(@"mm\:ss\.fff")}",  //ToString(@"hh\:mm\:ss\.fff")}",
                    $"{AllStatusStr} {TaskStopReasonStr}");
                return;
            }
            WorkTaskStatus = WorkTaskStatus.Completed;
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                MethodBase.GetCurrentMethod().Name + ": Completed", AllStatusStr,
                $"Elapsed: {(dt2 - dt1).ToString(@"mm\:ss\.fff")}, {TaskStopReasonStr}");

            // Queue.Clear();
        }
        private Task WaitingForCompletionAsync()
        {
            return Task.Factory.StartNew(WaitingForCompletion);
        }
        private bool CreateTask()
        {
            //if (TimeInterval <= 0)
            //    TimeInterval = TimeIntervalDefault;

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

                Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName,
                    MethodBase.GetCurrentMethod().Name, AllStatusStr, ToString());
            }
            return true;
        }
        private void WorkAction()
        {
            if (TimeInterval <= 0)
                TimeInterval = TimeIntervalDefault;

            WorkTaskStatus = WorkTaskStatus.Working;

            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
                MethodBase.GetCurrentMethod().Name + " Begin",
                AllStatusStr, ToString());

            LaunchedDateTime = DateTime.Now;
            OnStartTaskEvent();

            // var timeinterval = TimeInterval > 0 ? TimeInterval : Timeout.Infinite;

            while (Working)
            {
                if (IsStopByCancelToken)
                    CancellationTokenVerify();

                //if(!(Parent is IEventLog))
                //    Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                //    "Working...", TaskStatusStr, ToString());

                if (IsEnabled)
                {
                    LastWorkDateTime = DateTime.Now;

                    try
                    {
                        Action.Invoke();
                        //DeQueueProcess();
                    }
                    catch (Exception e)
                    {
                        SendException(e);
                    }
                }
                if (IsStopByCancelToken)
                    CancellationTokenVerify();

                AutoReset.WaitOne(TimeInterval);
            }
            Queue.Clear();
            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
                MethodBase.GetCurrentMethod().Name + " Finish",
                "Working cycle is Complete, " +  TaskStatusStr, TaskStopReasonStr);
        }
        private void CancellationTokenVerify()
        {
            if (!CancellationToken.IsCancellationRequested)
                return;

            IsEnabled = false;

            // ConsoleAsync.WriteLineT("WorkTask: {0} Task is Stopped with Pleasure.\r\nStopReason: {1}", Code, StopReason);
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
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
            if (!IsEnabled)
                return;
            if(IsEveryItemPushProcessing)
                AutoReset.Set();
        }
        private void ClearCounts()
        {
            SuccessCount = 0;
            ErrorCount = 0;
        }
        public double PrcntSuccessPass => SuccessCount + ErrorCount > 0 ? SuccessCount/(SuccessCount + ErrorCount) : 0;

        public void Dispose()
        {
            AutoReset?.Set();
            AutoReset?.Dispose();
            Working = false;
        }
        public override string Key => Code.HasValue() ? Code : GetType().Name;
    }
}
