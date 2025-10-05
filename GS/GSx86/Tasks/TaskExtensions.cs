using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;

namespace GS.Tasks
{
    public static class TaskExtensions
    {
        //public static string StatusStr(this Task t)
        //{
        //    return
        //        $"Status:{t.Status} Completed:{t.IsCompleted} Canceled:{t.IsCanceled} Faulted:{t.IsFaulted} Active:{t.IsActive()} NoActive:{t.IsNoActive()}";
        //}
        public static string StatusStr(this Task t)
        {
            return $"TaskStatus:{t.Status} Completed:{t.IsCompleted} Canceled:{t.IsCanceled} Faulted:{t.IsFaulted}";
        }
        public static string StatusStrShort(this Task t)
        {
            return $"TaskStatus:{t.Status}";
        }
        public static bool IsActive(this Task t)
        {
            return  t != null && (
                    t.Status == TaskStatus.Running ||
                    // t.Status == TaskStatus.RanToCompletion ||
                    t.Status == TaskStatus.WaitingToRun ||
                    t.Status == TaskStatus.WaitingForActivation);
        }
        public static bool IsNoActive(this Task t)
        {
            return t != null &&
                   (
                    t.IsCanceled ||
                    t.IsCompleted ||
                    t.IsFaulted
                    );
        }
        public static async Task WithTimeout(this Task task, TimeSpan timeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)))
            {
                await task;
                return;
            }
            throw new TimeoutException();
        }
        public static async Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)))
            {
                return await task;
            }
            throw new TimeoutException();
        }
        public static bool IsFinished(this Task task)
        {
            return task.IsCompleted || task.IsCanceled || task.IsFaulted;
        }
        public static void WaitingForTaskCompleting(this Task task, int timeInteravlaInSeconds)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            while (!task.IsFinished() && timeInteravlaInSeconds-- > 0)
            {
                ConsoleSync.WriteLineT($"{m} Wait for Completing: {task.GetType()} {task.StatusStr()}");
                Thread.Sleep(1000);
            }
            ConsoleSync.WriteLineT($"{m} Task is Finished {task.StatusStr()}");
            task.Dispose();
        }
        public static void WaitingForTaskCompleting(this Task task, CancellationToken canceltoken)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            while (!task.IsFinished())
            // while (true)
            {
                ConsoleSync.WriteLineT($"{m} Wait for Completing: {task.StatusStr()}");
                Thread.Sleep(1000);
                if (canceltoken.IsCancellationRequested)
                {
                    ConsoleSync.WriteLineT($"{m} Cancel Token Received ...");
                    break;
                }
            }
            ConsoleSync.WriteLineT($"{m} Task is Finished {task.StatusStr()}");
            task.Dispose();
        }
    }
}
