// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");
// https://learn.microsoft.com/ru-ru/dotnet/api/system.threading.tasks.task.delay?view=net-7.0
// Example1
// TASK DELAY

using GS.ConsoleAS;
using static System.Threading.Thread;

namespace CaTaskDelay01;

public class Example
{
    public static void Main()
    {
        // var threadId = Thread.CurrentThread.ManagedThreadId;
        ConsoleSync.WriteLineT($"{nameof(Main)}: TreadId:{CurrentThread.ManagedThreadId}");
        CancellationTokenSource source = new CancellationTokenSource();

        var t = Task.Run(async delegate
        {
            // var t = cancellationToken;
                try
                {
                    ConsoleSync.WriteLineT($"Task Before Delay TreadId:{CurrentThread?.ManagedThreadId}");
                    await Task.Delay(TimeSpan.FromSeconds(1.5), source.Token);
                    ConsoleSync.WriteLineT($"Task After  Delay TreadId:{CurrentThread?.ManagedThreadId}");
                    return 42;
                }
                catch (TaskCanceledException e)
                {
                    ConsoleSync.WriteLineT("TaskCanceledException Thread:{0} {1}: {2}", CurrentThread?.ManagedThreadId,
                        e.GetType().Name, e.Message);
                }
                catch (AggregateException ae)
                {
                    foreach (var e in ae.InnerExceptions)
                        ConsoleSync.WriteLineT("Task AggregateException Thread:{0} {1}: {2}",
                            CurrentThread?.ManagedThreadId, e.GetType().Name, e.Message);
                    // return 0;
                }
                catch (Exception e)
                {
                    ConsoleSync.WriteLineT("Task Exception Thread:{0} {1}: {2}", CurrentThread?.ManagedThreadId,
                        e.GetType().Name, e.Message);
                    // return 0;
                }
                return 0;
            }, source.Token);
        // Main
        ConsoleSync.WriteLineT($"{nameof(Main)}: TreadId:{CurrentThread.ManagedThreadId}");
        source.Cancel();
        ConsoleSync.WriteLineT($"{nameof(Main)}: TreadId:{CurrentThread.ManagedThreadId}");
        try
        {
            ConsoleSync.WriteLineT($"{"Main Before Wait"}: TreadId:{CurrentThread.ManagedThreadId}");
            t.Wait();
            var a = t.Result;
            ConsoleSync.WriteLineT($"{"Main After Wait"}: TreadId:{CurrentThread.ManagedThreadId} Result:{a}");
        }
        catch (AggregateException ae)
        {
            foreach (var e in ae.InnerExceptions)
                ConsoleSync.WriteLineT("Exception Thread:{0} Exc:{1}: {2}", CurrentThread.ManagedThreadId, e.GetType().Name, e.Message);
        }

        ConsoleSync.WriteLineT("Main ThreadId:{0} TaskStatus:{1}", CurrentThread.ManagedThreadId, t.Status);

        if (t.Status == TaskStatus.RanToCompletion) 
            ConsoleSync.WriteLineT(", Result: {0}", t.Result);

        source.Dispose();
    }
}
// The example displays output like the following:
//       TaskCanceledException: A task was canceled.
//       Task t Status: Canceled
