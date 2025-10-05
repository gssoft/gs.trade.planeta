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
    public class TaskBase01 : Element1<string>
    {
        public const int Infinite = -1;

        [XmlIgnore]
        public Task Task { get; private set; }
        [XmlIgnore]
        public AutoResetEvent AutoReset { get; set; }
        [XmlIgnore]
        public Action MainAction { get; set; }
        public int TimeInterval { get; set; }
        public bool IsStopByCancelToken { get; set; }
        public event EventHandler TaskStatusChangedEvent;
        protected virtual void OnTaskStatusChangedEvent()
        {
            var handler = TaskStatusChangedEvent;
            handler?.Invoke(this, EventArgs.Empty);
        }
        protected bool Working { get; private set; }
        public bool IsWorking => Working;
        public bool IsActive => Task != null && (
            Task.Status == TaskStatus.Running ||
            // Task.Status == TaskStatus.RanToCompletion ||
            Task.Status == TaskStatus.WaitingToRun ||
            Task.Status == TaskStatus.WaitingForActivation);

        public bool IsCompleted => Task != null && Task.IsCompleted;
        public bool IsFaulted => Task != null && Task.IsFaulted;
        public bool IsCanceled => Task != null && Task.IsCanceled;
        
        // Created -> WaitingForActivation -> WaitingToRun -> Running 
        // WaitingFor ChildrenCompletion
        // Faulted RanToCompletion Canceled 
        
        //public bool IsCompleted => Task != null && (
        //    Task.Status == TaskStatus.WaitingForActivation ||
        //    Task.Status == TaskStatus.WaitingToRun ||
        //    Task.Status == TaskStatus.Created ||
        //    Task.Status == TaskStatus.Running ||
        //    Task.Status == TaskStatus.WaitingForChildrenToComplete ||
        //    Task.Status == TaskStatus.RanToCompletion ||
        //    Task.Status == TaskStatus.Faulted ||
        //    Task.Status == TaskStatus.Canceled);
        //public bool IsFaulted => Task != null && (
        //    Task.Status == TaskStatus.WaitingForActivation ||
        //    Task.Status == TaskStatus.WaitingToRun ||
        //    Task.Status == TaskStatus.Created ||
        //    Task.Status == TaskStatus.Running ||
        //    Task.Status == TaskStatus.WaitingForChildrenToComplete ||
        //    Task.Status != TaskStatus.RanToCompletion ||
        //    Task.Status == TaskStatus.Faulted ||
        //    Task.Status != TaskStatus.Canceled);
        //public bool IsCanceled => Task != null && (
        //    Task.Status == TaskStatus.WaitingForActivation ||
        //    Task.Status == TaskStatus.WaitingToRun ||
        //    Task.Status == TaskStatus.Created ||
        //    Task.Status == TaskStatus.Running ||
        //    Task.Status == TaskStatus.WaitingForChildrenToComplete ||
        //    Task.Status != TaskStatus.RanToCompletion ||
        //    Task.Status != TaskStatus.Faulted ||
        //    Task.Status == TaskStatus.Canceled);

        public string TaskStatusStr => Task != null
            ?
              // $"{WorkTaskStatusStr}, " +
              $"TaskStatus:{Task.Status}, " +
              $"IsCanceled:{Task.IsCanceled}, " +
              $"IsCompleted:{Task.IsCompleted}, " +
              $"IsFaulted:{Task.IsFaulted}"
            : "TaskStatus: is Null";

        public TaskStatus TaskStatus => Task?.Status ?? TaskStatus.Faulted;
        public bool IsTaskFinished => Task == null || Task.IsCompleted || Task.IsCanceled || Task.IsFaulted;


        public TaskBase01()
        {
            IsEnabled = true;
            AutoReset = new AutoResetEvent(false);
            TimeInterval = Infinite;
        }
        public void Start(Action action)
        {
            MainAction = action;

            if (TimeInterval == 0)
                TimeInterval = 15*1000;

            Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName,
                 MethodBase.GetCurrentMethod().Name + " Begin", TaskStatusStr, ToString());

            TaskStopReason = TaskStopReasonEnum.Unknown;
            WorkTaskStatus = WorkTaskStatus.TryToStart;

            Working = false;

            if (!IsEnabled)
                return;

            if (!CreateTask())
                return;

            Working = true;

            Task?.ContinueWith(TaskContinueProcess);
            Task?.Start();

            //Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
            //    MethodBase.GetCurrentMethod().Name + " Finish", TaskStatusStr, ToString());
        }

        public void Stop()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            if (Task == null)
                return;

            IsEnabled = false;

            WorkTaskStatus = WorkTaskStatus.TryToStop;

            Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName,
                MethodBase.GetCurrentMethod().Name, TaskStatusStr, ToString());

            var cts = new CancellationTokenSource();
            var token = cts.Token;
            Task.Factory.StartNew(p=>WaitingForCompletionAction(CancellationToken), token);

            AutoReset.Set();
            if (IsStopByCancelToken)
            {
                TaskStopReason = TaskStopReasonEnum.CancelRequest;
                CancellationTokenSource.Cancel();
            }
            else
            {
                TaskStopReason = TaskStopReasonEnum.CompleteRequest;
                Working = false;
            }
            AutoReset.Set();

            // Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, GetType().Name, "Task", "Stop()", TaskStatusStr, "");

           // Task.Factory.StartNew(WaitingForCompletionAction);
        }

        private bool CreateTask()
        {

            //if (Task != null &&
            //    (Task.Status == TaskStatus.Running || Task.Status == TaskStatus.RanToCompletion))
            //    return false;

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
            var m = MethodBase.GetCurrentMethod().Name + "()";
            WorkTaskStatus = WorkTaskStatus.Working;

            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
                MethodBase.GetCurrentMethod().Name + " Begin",
                TaskStatusStr, ToString());

            // LaunchedDateTime = DateTime.Now;
            OnTaskStatusChangedEvent();

            // var timeinterval = TimeInterval > 0 ? TimeInterval : Timeout.Infinite;

            //while (Working)
            //{
                //if (IsStopByCancelToken && CancellationToken.IsCancellationRequested)
                //            CancellationToken.ThrowIfCancellationRequested();

                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                "Working...", TaskStatusStr, ToString());

                if (IsEnabled)
                {
                    LastWorkDateTime = DateTime.Now;

                    try
                    {
                       MainAction?.Invoke();
                    }
                    catch (OperationCanceledException e)
                    {
                        e.PrintExceptions(this, m);
                        TaskStopReason = TaskStopReasonEnum.CancelRequest;
                        // break;
                    }
                    catch (Exception e)
                    {
                        e.PrintExceptions(this, m);
                        TaskStopReason = TaskStopReasonEnum.Exception;
                    }
                    LastWorkDateTime = DateTime.Now;
                }
                //if (IsStopByCancelToken && CancellationToken.IsCancellationRequested)
                //    CancellationToken.ThrowIfCancellationRequested();

                // AutoReset.WaitOne(TimeInterval);
            //}
            LastWorkDateTime = DateTime.Now;
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

        public WorkTaskStatus WorkTaskStatus { get; private set; }

        protected CancellationToken CancellationToken;

        protected CancellationTokenSource CancellationTokenSource;

        public TaskStopReasonEnum TaskStopReason { get; private set; }

        private async void WaitForCompletion()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            var cts = new CancellationTokenSource();
            //CancellationToken = cts.Token;
            var token = cts.Token;
            try
            {
                var dt1 = DateTime.Now.TimeOfDay;
                var task = Task.Factory.StartNew(p => WaitingForCompletionAction(CancellationToken), token);
                task.Wait(TimeSpan.FromSeconds(15));

                if (task.IsCompleted || task.IsCanceled || task.IsFaulted)
                {
                    var dt2 = DateTime.Now.TimeOfDay;
                    WorkTaskStatus = WorkTaskStatus.Completed;
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                         $"{m}: Task is {task.Status}", WorkTaskStatus.ToString(),
                        $"Elapsed: {(dt2 - dt1).ToString(@"mm\:ss\.fff")}, {TaskStopReasonStr}");
                    return;
                }
                cts.Cancel();
                await task;
            }
            catch (OperationCanceledException e)
            {
                // Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");

            }
            catch (Exception e)
            {

            }
            finally
            {
                cts.Dispose();
            }
        }

        private void WaitingForCompletionAction(CancellationToken token)
        {
            if (Task == null)
                return;

            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                    MethodBase.GetCurrentMethod().Name + ": Waiting for Completing ...",
                    $"{TaskStatusStr}, {TaskStopReason}", ToString());

            while (!IsTaskFinished)
            {
                Thread.Sleep(1 * 1000);

                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                    MethodBase.GetCurrentMethod().Name + ": Waiting for Completing ...",
                    $"{TaskStatusStr}, {TaskStopReason}", ToString());

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
            }
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                    MethodBase.GetCurrentMethod().Name + ": Task Finished ...",
                    $"{TaskStatusStr}, {TaskStopReason}", ToString());
        }

        public int SecondsToWaitCompleting { get; set; }

        public override string Key => Code.HasValue() ? Code : GetType().FullName;
    }
}
