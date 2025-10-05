using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.ConsoleAS;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;
using GS.ProcessTasks;
using GS.WorkTasks;

namespace GS.Tasks
{
    public class TaskBase : Element1<string>
    {
        public const int Infinite = -1;

        [XmlIgnore]
        public Task Task { get; private set; }
        public AutoResetEvent AutoReset { get; set; }
        public int TimeInterval { get; set; }
        public Action MainAction { get; set; }

        public event EventHandler StartTaskEvent;
        protected virtual void OnStartTaskEvent()
        {
            var handler = StartTaskEvent;
            handler?.Invoke(this, EventArgs.Empty);
        }
        protected bool Working { get; set; }
        public bool IsWorking => Working;

        public bool IsActive => Task != null && (
            Task.Status == TaskStatus.Running ||
            // Task.Status == TaskStatus.RanToCompletion ||
            Task.Status == TaskStatus.WaitingToRun ||
            Task.Status == TaskStatus.WaitingForActivation);

        // Created -> WaitingForActivation -> WaitingToRun -> Running 
        // WaitingFor ChildrenCompletion
        // Faulted RanToCompletion Canceled 

        public bool IsCompleted => Task != null && (
            Task.Status == TaskStatus.WaitingForActivation ||
            Task.Status == TaskStatus.WaitingToRun ||
            Task.Status == TaskStatus.Created ||
            Task.Status == TaskStatus.Running ||
            Task.Status == TaskStatus.WaitingForChildrenToComplete ||
            Task.Status == TaskStatus.RanToCompletion ||
            Task.Status == TaskStatus.Faulted ||
            Task.Status == TaskStatus.Canceled);

        public bool IsFaulted => Task != null && (
            Task.Status == TaskStatus.WaitingForActivation ||
            Task.Status == TaskStatus.WaitingToRun ||
            Task.Status == TaskStatus.Created ||
            Task.Status == TaskStatus.Running ||
            Task.Status == TaskStatus.WaitingForChildrenToComplete ||
            Task.Status != TaskStatus.RanToCompletion ||
            Task.Status == TaskStatus.Faulted ||
            Task.Status != TaskStatus.Canceled);

        public bool IsCanceled => Task != null && (
            Task.Status == TaskStatus.WaitingForActivation ||
            Task.Status == TaskStatus.WaitingToRun ||
            Task.Status == TaskStatus.Created ||
            Task.Status == TaskStatus.Running ||
            Task.Status == TaskStatus.WaitingForChildrenToComplete ||
            Task.Status != TaskStatus.RanToCompletion ||
            Task.Status != TaskStatus.Faulted ||
            Task.Status == TaskStatus.Canceled);

        public string TaskStatusStr => Task != null
            ?
              // $"{WorkTaskStatusStr}, " +
              $"TaskStatus: {Task.Status}, " +
              $"IsCanceled: {Task.IsCanceled}, " +
              $"IsCompleted: {Task.IsCompleted}, " +
              $"IsFaulted: {Task.IsFaulted}"
            : "TaskStatus: is Null";

        public TaskStatus TaskStatus => Task?.Status ?? TaskStatus.Faulted;
        public bool TaskIsFinished => Task == null || Task.IsCompleted || Task.IsCanceled || Task.IsFaulted;

        public TaskBase()
        {
            IsEnabled = true;
            AutoReset = new AutoResetEvent(false);
        }
        public void Start()
        {
            if (TimeInterval == 0)
                TimeInterval = 15*1000;

            Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName,
                 MethodBase.GetCurrentMethod().Name + " Begin", TaskStatusStr, ToString());

            TaskStopReason = TaskStopReasonEnum.Unknown;
            WorkTaskStatus = WorkTaskStatus.TryToStart;

            Working = false;

            if (!IsEnabled)
                return;

            // ClearCounts();

            if (!CreateTask())
                return;

            Working = true;

            Task?.ContinueWith(TaskContinueProcess);

            Task?.Start();

            //Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
            //    MethodBase.GetCurrentMethod().Name + " Finish", TaskStatusStr, ToString());
        }

        private bool CreateTask()
        {
            //if (TimeInterval <= 0)
            //    TimeInterval = TimeIntervalDefault;

            if (Task != null &&
                (Task.Status == TaskStatus.Running || Task.Status == TaskStatus.RanToCompletion))
                return false;

            TaskStopReason = TaskStopReasonEnum.Unknown;
 
            var cts = new CancellationTokenSource();
            CancellationToken = cts.Token;

            Task = new Task(WorkAction, CancellationToken);

            if (Task != null)
            {
                WorkTaskStatus = WorkTaskStatus.Created;

                Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName,
                    MethodBase.GetCurrentMethod().Name, TaskStatusStr, ToString());
            }
            return true;
        }
        private void WorkAction()
        {
            WorkTaskStatus = WorkTaskStatus.Working;

            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
                MethodBase.GetCurrentMethod().Name + " Begin",
                TaskStatusStr, ToString());

            // LaunchedDateTime = DateTime.Now;
            OnStartTaskEvent();

            // var timeinterval = TimeInterval > 0 ? TimeInterval : Timeout.Infinite;

            while (Working)
            {
                //if (IsStopByCancelToken)
                //    CancellationTokenVerify();

                //if(!(Parent is IEventLog))
                //    Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                //    "Working...", TaskStatusStr, ToString());

                if (IsEnabled)
                {
                    LastWorkDateTime = DateTime.Now;

                    try
                    {
                       MainAction?.Invoke();
                    }
                    catch (Exception e)
                    {
                        SendException(e);
                    }
                }
                //if (IsStopByCancelToken)
                //    CancellationTokenVerify();

                AutoReset.WaitOne(TimeInterval);
            }
            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
                MethodBase.GetCurrentMethod().Name + " Finish",
                "Working cycle is Complete, " + TaskStatusStr, TaskStopReasonStr);
        }

        private void TaskContinueProcess(Task t)
        {
           var ae = Task?.Exception;
            if (ae != null)
            {
                var str = ae.AggExceptionMessage();
                ConsoleAsync.WriteLineT("WorkTask: {0} >>> Complete with Exception:\r\n{1}", Code, str);
            }
            ConsoleAsync.WriteLineT("TaskCompleteProcess(): WorkTask: {0} is Completed.\r\nStatus: {1}", Code, t.Status );
        }

        public string TaskStopReasonStr { get; set; }

        public DateTime LastWorkDateTime { get; set; }

        public WorkTaskStatus WorkTaskStatus { get; set; }

        public CancellationToken CancellationToken { get; set; }

        // public CancellationTokenSource Cts { get; set; }

        public TaskStopReasonEnum TaskStopReason { get; set; }

        public override string Key => Code.HasValue() ? Code : GetType().FullName;
    }
}
