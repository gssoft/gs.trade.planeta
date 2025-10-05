using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
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
    public class TaskBase02 : Element1<string>
    {
        public const int Infinite = -1;

        [XmlIgnore]
        public Task MainTask { get; private set; }
        public Task MainContinueTask { get; private set; }

        public Task WaitForMainTaskCompletingTask { get; private set; }
        public int TimeIntervalForMainTaskCompletingSeconds { get; set; }

        protected CancellationToken CancellationToken;
        protected CancellationTokenSource CancellationTokenSource;

        protected CancellationToken WaitTaskCancellationToken;
        protected CancellationTokenSource WaitTaskCancellationTokenSource;

        [XmlIgnore]
        public string ClientName { get; set; }

        public string MainActionName { get; private set; }

        [XmlIgnore]
        public AutoResetEvent AutoReset { get; set; }
        [XmlIgnore]
        public Action<CancellationToken> MainAction { get; set; }
        public Action<CancellationToken> ContinueAction { get; set; }
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
        public bool IsActive => MainTask != null && MainTask.IsActive();
        public bool IsCompleted => MainTask != null && MainTask.IsCompleted;
        public bool IsFaulted => MainTask != null && MainTask.IsFaulted;
        public bool IsCanceled => MainTask != null && MainTask.IsCanceled;
        public string TaskStatusStr => MainTask != null
                          ? $"{MainTask.StatusStr()}" : "TaskStatus: is Null";
        public TaskStatus TaskStatus => MainTask?.Status ?? TaskStatus.Faulted;
        public bool IsTaskFinished => MainTask != null && MainTask.IsFinished();

        public TaskBase02()
        {
            IsEnabled = true;
            AutoReset = new AutoResetEvent(false);
            TimeInterval = Infinite;
            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;
        }
        public void Start(Action<CancellationToken> action)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";

            MainAction = action;
            MainActionName = MainAction.Method.ToString();

            IsEnabled = true;

            if (TimeInterval == 0)
                TimeInterval = 15*1000;

            TaskStopReason = TaskStopReasonEnum.Unknown;
            WorkTaskStatus = WorkTaskStatus.TryToStart;

            Working = false;

            //CancellationTokenSource = new CancellationTokenSource();
            //CancellationToken = CancellationTokenSource.Token;

            if (ContinueAction == null)
            {
                MainTask = Task.Run(() => WorkAction(CancellationToken), CancellationToken);
                MainContinueTask = MainTask.ContinueWith(MainTaskCompleteProcess);
            }
            else
            {
                MainTask = Task.Run(() => WorkAction(CancellationToken), CancellationToken);
                MainContinueTask = MainTask.ContinueWith(MainTaskCompleteProcess);
            }

            Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentAndMyTypeName, TypeName, MainActionName,
                           $"{m} Starting {TaskStatusStr}", ToString());

            Working = true;

            //Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
            //    MethodBase.GetCurrentMethod().Name + " Finish", TaskStatusStr, ToString());
        }
        public void Stop()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            if (MainTask == null)
                return;

            IsEnabled = false;
            IsStopByCancelToken = true;

            WorkTaskStatus = WorkTaskStatus.TryToStop;

            // Evlm2(EvlResult.INFO, EvlSubject.INIT,  TypeName, Name, m, TaskStatusStr, ToString());

            //WaitTaskCancellationTokenSource = new CancellationTokenSource();
            //WaitTaskCancellationToken = WaitTaskCancellationTokenSource.Token;

            //WaitForMainTaskCompletingTask = Task.Factory
            //    .StartNew(p => WaitingForMainTaskCompletingAction(WaitTaskCancellationToken),
            //                    WaitTaskCancellationToken);

            //WaitForMainTaskCompletingTask.ContinueWith(WaitTaskCompletingProcess);

            //MainTask.ContinueWith(MainTaskCompletingProcess);

            //Thread.Sleep(1000);
            
            AutoReset.Set();
            if (IsStopByCancelToken)
            {
                TaskStopReason = TaskStopReasonEnum.CancelRequest;
                CancellationTokenSource.Cancel();
                // CancellationTokenSource.Dispose();
            }
            else
            {
                TaskStopReason = TaskStopReasonEnum.CompleteRequest;
                Working = false;
            }
            AutoReset.Set();
            // Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, GetType().Name, "Task", "Stop()", TaskStatusStr, "");

            Evlm2(EvlResult.INFO, EvlSubject.INIT, $"{ParentTypeName}.{TypeName}", MainActionName,
                           $"{m} Try to Stop", TaskStatusStr, ToString());
        }
        private void WorkAction(CancellationToken token)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            WorkTaskStatus = WorkTaskStatus.Working;

            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName, 
                $"{MainActionName}", $"{m} Begin {TaskStatusStr}" , ToString());

            // LaunchedDateTime = DateTime.Now;
            OnTaskStatusChangedEvent();

            // var timeinterval = TimeInterval > 0 ? TimeInterval : Timeout.Infinite;

            //while (Working)
            //{
                //if (IsStopByCancelToken && CancellationToken.IsCancellationRequested)
                //            CancellationToken.ThrowIfCancellationRequested();

                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                $"{MainActionName}", $"{m} Working {TaskStatusStr}", ToString());

                if (IsEnabled)
                {
                    LastWorkDateTime = DateTime.Now;

                    try
                    {
                        MainAction?.Invoke(token);
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
                ConsoleSync.WriteLineT($"{Key} {MainActionName} {m} Task Try to Finish {TaskStatusStr}");
                //if (IsStopByCancelToken && CancellationToken.IsCancellationRequested)
                //    CancellationToken.ThrowIfCancellationRequested();

                // AutoReset.WaitOne(TimeInterval);
            //}
            LastWorkDateTime = DateTime.Now;
            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
                $"{MainActionName}", $"{m} Finish {TaskStatusStr}", ToString() );
        }
        private void WaitingForMainTaskCompletingAction(CancellationToken token)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            if (MainTask == null) return;

            var seconds = TimeIntervalForMainTaskCompletingSeconds > 0
                ? TimeIntervalForMainTaskCompletingSeconds
                : 60;

            while (!MainTask.IsFinished() && seconds-- > 0)
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                    MethodBase.GetCurrentMethod().Name + ": Waiting for Completing ...",
                    $"{TaskStatusStr}, {TaskStopReason}", ToString());

                ConsoleSync.WriteLineT($"{ClientName} {m} MainTask Wait for Completing:{MainTask.IsFinished()} {MainTask.StatusStr()}");

                if (token.IsCancellationRequested)
                {
                    ConsoleSync.WriteLineT($"{ClientName} {m}:Cancel Request Received: Finished:{MainTask.IsFinished()} {MainTask.StatusStr()}");
                    // token.ThrowIfCancellationRequested();
                    break;
                }
                Thread.Sleep(1 * 1000);
            }
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                    MethodBase.GetCurrentMethod().Name + $": MainTask Finished:{MainTask?.IsFinished()}",
                    $"{TaskStatusStr}, {TaskStopReason}", ToString());

            ConsoleSync.WriteLineT($"{ClientName} {m} MainTask Finished:{MainTask.IsFinished()} {MainTask.StatusStr()}");
            MainTask?.Dispose();
            // ConsoleSync.WriteLineT($"{m} MainTask After Dispose Finished:{MainTask.IsFinished()} {MainTask.StatusStr()}");
        }
        // MainTask 
        private void MainTaskCompletingProcess(Task t)
        {
            // Try to Cancel WaitingForMainTaskCompleting Task
            var m = MethodBase.GetCurrentMethod().Name + "()";
            WaitTaskCancellationTokenSource.Cancel();

            var ae = MainTask?.Exception;
            if (ae != null)
            {
                var str = ae.AggExceptionMessage();
                ConsoleSync.WriteLineT($"MainTask: {Code} >>> Complete with Exception:{str}");
            }
            ConsoleSync.WriteLineT($"{ClientName} {m} MainTask Finished:{t?.IsFinished()} {t?.StatusStr()}");
            t?.Dispose();
        }
        private void MainTaskCompleteProcess(Task t)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";

            var ae = MainTask?.Exception;
            if (ae != null)
            {
                var str = ae.AggExceptionMessage();
               // ConsoleSync.WriteLineT($"MainTask: {Code} >>> Complete with Exception:{str}");

                Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, $"{ParentTypeName}.{TypeName}", 
                    m, MainActionName,"Exception", str );
            }
            Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, $"{ParentTypeName}.{TypeName}",
                    m, MainActionName,
                $"Task Finished:{t?.IsFinished()} {t?.StatusStr()}", ToString());

            ConsoleSync.WriteLineT($"{ClientName} {m} MainTask Finished:{t?.IsFinished()} {t?.StatusStr()}");
            t?.Dispose();
        }
        // WaitForMainTaskCompleting Task is Completed
        private void WaitTaskCompletingProcess(Task t)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"{m} WaitTask is Completed. {t.StatusStr()}");
        }       
        public DateTime LastWorkDateTime { get; set; }
        public WorkTaskStatus WorkTaskStatus { get; private set; }
        public TaskStopReasonEnum TaskStopReason { get; private set; }
        public int SecondsToWaitCompleting { get; set; }
        // public override string Key => Code.HasValue() ? Code : GetType().FullName;
        public override string Key => Code.HasValue() ? Code : GetType().Name;
    }
}
