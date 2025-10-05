using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.ConsoleAS;
using GS.Extension;
using GS.Tasks;

namespace Ca_Task_Finish_01
{
    public class TaskCancelTest
    {
        private enum CancelationType { WithException, WithoutException }

        public static Task Task;
        private static bool _working;

        //private static string TaskStatus =>
        //    Task != null
        //        ? $"{Task.Status} Complete:{Task.IsCompleted} Fault:{Task.IsFaulted} Cancel:{Task.IsCanceled}"
        //        : "Taks is null;";
        private static string TaskStatus =>
            Task != null
                ? $"{Task.StatusStr()}"
                : "Taks is null;";

        private static string WorkingMessage => $"WorkAction(): Working Task:{TaskStatus}";
        private static string WorkingCompleteMessage => $"WorkAction(): Working Complete. Task:{TaskStatus}";
        // private static string CancelMessage => $"WorkingTask: {TaskStatus}";

        static void Main(string[] args)
        {
            TaskBase02Test();
            TaskCancelWithTimeoutWithCancelTokenNoThrowExs();
            ConsoleSync.WriteReadLineT("Press any key to Next");
            TaskCancelWithTimeoutWithCancelTokenThrowExs();
            ConsoleSync.WriteReadLineT("Press any key to Next");

            TaskCancelWithoutToken();
            TaskCancelWithToken(CancelationType.WithException); // return = Task.Status = RanToCompletion
            TaskCancelWithToken(CancelationType.WithoutException); // return = Task.Satus = Fault
        }
        private static void WorkAction()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"{m}");
            if (Task == null) return;
            _working = true;
            while (_working)
            {
                ConsoleSync.WriteLineT(WorkingMessage);
                Thread.Sleep(1 * 1000);
            }
            ConsoleSync.WriteLineT(WorkingCompleteMessage);
        }
        private static void WorkActionWithTokenWithoutException(CancellationToken canceltoken)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"{m}");
            if (Task == null) return;

            _working = true;
            
            while (_working)
            {
                ConsoleSync.WriteLineT(WorkingMessage);
                Thread.Sleep(1 * 1000);

                if (canceltoken.IsCancellationRequested)
                {
                    ConsoleSync.WriteLineT("Cancel Token Received ...");
                    canceltoken.ThrowIfCancellationRequested();
                }
            }
            ConsoleSync.WriteLineT(WorkingMessage);
        }
        private static void WorkActionWithTokenWithException(CancellationToken canceltoken)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"{m}");

           // if (Task == null) return;

            _working = true;
            try
            {
                while (_working)
                {
                    ConsoleSync.WriteLineT(WorkingMessage);
                    Thread.Sleep(1*1000);

                    if (canceltoken.IsCancellationRequested)
                    {
                        ConsoleSync.WriteLineT($"{m}: Cancel Token Received ...");
                        canceltoken.ThrowIfCancellationRequested();
                    }
                }
                ConsoleSync.WriteLineT(WorkingCompleteMessage);
            }
            catch (OperationCanceledException e)
            {
                // ConsoleSync.WriteLineT($"Exc:{e.GetType()} {e.Message}");
                // PrintExceptions(e);
                e.PrintExceptions(m);
            }
            catch (Exception e)
            {
                // ConsoleSync.WriteLineT($"Exc:{e.GetType()} {e.Message}");
                e.PrintExceptions(m);
            }
        }
        private static void TaskCancelWithoutToken()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"{m}");

            Task = new Task(WorkAction);
            Task.Start();
            Task.Wait(TimeSpan.FromSeconds(5));

            _working = false;

            while (true)
            {
                if (!Task.IsCompleted)
                {
                    ConsoleSync.WriteLineT($"{m} TaskStatus:{TaskStatus}");
                    Thread.Sleep(1000);
                    continue;
                }
                break;
            }
            ConsoleSync.WriteLineT($"{m} TaskStatus: {TaskStatus}");
            ConsoleSync.WriteReadLineT($"{m}: Press any key ...{Environment.NewLine}");
        }
        private static void TaskCancelWithToken(CancelationType canceltype)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"{m}");

            var tsrc = new CancellationTokenSource();
            var token = tsrc.Token;
            Action<CancellationToken> action;

            if (canceltype == CancelationType.WithException)
                action = WorkActionWithTokenWithException;
            else
                action = WorkActionWithTokenWithoutException;

            Task = new Task(p=>action(token), token);
            Task.Start();
            Task.Wait(TimeSpan.FromSeconds(5));
            tsrc.Cancel();

            while (true)
            {              
                if (!Task.IsCompleted)
                {
                    ConsoleSync.WriteLineT($"{m} TaskStatus:{TaskStatus}");
                    Thread.Sleep(1000);
                    continue;
                }
                break;
            }
            ConsoleSync.WriteLineT($"{m} TaskStatus: {TaskStatus}");
            ConsoleSync.WriteReadLineT($"{m}: Press any key ...{Environment.NewLine}");
        }
        private static async void TaskCancelWithTimeoutWithCancelTokenThrowExs()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";

            var cts = new CancellationTokenSource();
            var token = cts.Token;
            try
            {
                await GetClientAsync(token).WithTimeout(TimeSpan.FromSeconds(5));
            }
            catch (TimeoutException e)
            {
                e.PrintExceptions(m);
                cts.Cancel();
            }
            catch (OperationCanceledException e)
            {
                e.PrintExceptions(m);
            }
            catch (Exception e)
            {
                e.PrintExceptions(m);
                cts.Cancel();
            }
            while (!Task.IsCanceled && !Task.IsCompleted && !Task.IsFaulted)
            {
                ConsoleSync.WriteLineT($"Finishing... {Task.StatusStr()}");
                Thread.Sleep(1000);
            }
            ConsoleSync.WriteLineT($"Finished {Task.StatusStr()}");
        }
        public static Task GetClientAsync(CancellationToken canceltoken)
        {
            Task =  Task.Factory.StartNew(p => WorkActionWithTokenWithoutException(canceltoken), canceltoken);
            return Task;
        }
        private static async void TaskCancelWithTimeoutWithCancelTokenNoThrowExs()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";

            var cts = new CancellationTokenSource();
            var token = cts.Token;
            try
            {
                await WorkActionWithExcAsync(token).WithTimeout(TimeSpan.FromSeconds(5));
            }
            catch (TimeoutException e)
            {
                e.PrintExceptions(m);
                cts.Cancel();
            }
            catch (OperationCanceledException e)
            {
                e.PrintExceptions(m);
            }
            catch (Exception e)
            {
                e.PrintExceptions(m);
                cts.Cancel();
            }
            while (!Task.IsCanceled && !Task.IsCompleted && !Task.IsFaulted)
            {
                ConsoleSync.WriteLineT($"Finishing... {Task.StatusStr()}");
                Thread.Sleep(1000);
            }
            ConsoleSync.WriteLineT($"Finished {Task.StatusStr()}");
        }
        public static Task WorkActionWithExcAsync(CancellationToken canceltoken)
        {
            Task = Task.Factory.StartNew(p => WorkActionWithTokenWithException(canceltoken), canceltoken);
            return Task;
        }

        private static async void TaskCancelWithTmeout02()
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            var task = Task.Run(() => WorkAction()).WithCancellation(cts.Token);
        }

        private static void TaskBase02Test()
        {
            var t = new TaskBase02 {TimeIntervalForMainTaskCompletingSeconds = 60};
            t.Start(WorkActionWithTokenWithException);
            Thread.Sleep(TimeSpan.FromSeconds(15));
            t.Stop();
            ConsoleSync.WriteReadLineT("Press any key to Finish");
        }
    }
}
