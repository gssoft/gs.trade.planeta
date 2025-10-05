// https://learn.microsoft.com/ru-ru/dotnet/api/system.threading.tasks.task.delay?view=net-7.0

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using GS.ConsoleAS;
using GS.Reflection;

namespace CaTaskDelay03;
public class Example
{
    public static void F1(int delayInMilliSeconds)
    {
        var m = MethodBase.GetCurrentMethod()?.Name + "()";

        ConsoleSync.WriteLineT($"{m} Start");
        ConsoleSync.WriteLineT($"{m} ThreadMain:{Thread.CurrentThread.ManagedThreadId}");
        var sw = Stopwatch.StartNew();
        var t = Task.Run(async delegate
        {
            // ConsoleSync.WriteLineT($"{m} ThreadTask:{Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(delayInMilliSeconds).ConfigureAwait(false);
            sw.Stop();
            ConsoleSync.WriteLineT($"{m} ThreadTask:{Thread.CurrentThread.ManagedThreadId}");
            return sw.ElapsedMilliseconds;
        });
        t.Wait();
        ConsoleSync.WriteLineT($"{m} Complete");
        ConsoleSync.WriteLineT($"{m} Elapsed milliseconds: {t.Result}");
        ConsoleSync.WriteLineT($"{m} ThreadMain:{Thread.CurrentThread.ManagedThreadId}");
        ConsoleSync.WriteLineT("Task t Status: {0}, Result: {1}", t.Status, t.Result);
    }
    public static void F2(int delayInMilliSeconds)
    {
        var m = MethodBase.GetCurrentMethod()?.Name + "()";

        ConsoleSync.WriteLineT($"{m} Start");
        ConsoleSync.WriteLineT($"{m} ThreadMain:{Thread.CurrentThread.ManagedThreadId}");
        var sw = Stopwatch.StartNew();
        var t = Task.Run(async delegate
        {
            // ConsoleSync.WriteLineT($"{m} ThreadTask:{Thread.CurrentThread.ManagedThreadId}"); // Remove Later
            await Task.Delay(delayInMilliSeconds).ConfigureAwait(false);
            // sw.Stop();
            return 1;
        });
        t.Wait();
        sw.Stop();
        ConsoleSync.WriteLineT($"{m} Complete");
        ConsoleSync.WriteLineT($"{m} Elapsed milliseconds: {sw.ElapsedMilliseconds}");
        ConsoleSync.WriteLineT($"{m} ThreadMain:{Thread.CurrentThread.ManagedThreadId}");
        ConsoleSync.WriteLineT("Task t Status: {0}, Result: {1}", t.Status, t.Result);

    }
    public static void F3(int delayInMilliSeconds)
    {
        var m = MethodBase.GetCurrentMethod()?.Name + "()";
        ConsoleSync.WriteLineT($"{m} Start");
        ConsoleSync.WriteLineT($"{m} ThreadMain:{Thread.CurrentThread.ManagedThreadId}");
        var sw = Stopwatch.StartNew();
        var delay = Task.Delay(delayInMilliSeconds).ContinueWith(_ =>
        {
            sw.Stop();
            ConsoleSync.WriteLineT($"{m} Delay Complete");
            ConsoleSync.WriteLineT($"{m} ThreadTask:{Thread.CurrentThread.ManagedThreadId}");
            return sw.ElapsedMilliseconds;
        });

        ConsoleSync.WriteLineT($"{m} Complete");
        ConsoleSync.WriteLineT($"{m} Elapsed milliseconds: {delay.Result}");

        // The example displays output like the following:
        //        Elapsed milliseconds: 1013
    }

    public static void F4(int delayInMilliSeconds)
    {
        var m = MethodBase.GetCurrentMethod()?.Name + "()";
        ConsoleSync.WriteLineT($"{m} Start");
        ConsoleSync.WriteLineT($"{m} ThreadMain:{Thread.CurrentThread.ManagedThreadId}");
        var delay = Task.Run(async () =>
        {
            ConsoleSync.WriteLineT($"{m} ThreadTask:{Thread.CurrentThread.ManagedThreadId}");
            var sw = Stopwatch.StartNew();
            // await Task.Delay(delayInMilliSeconds);
            await Task.Delay(delayInMilliSeconds).ConfigureAwait(false);
            sw.Stop();
            ConsoleSync.WriteLineT($"{m} Delay Complete");
            ConsoleSync.WriteLineT($"{m} ThreadTask:{Thread.CurrentThread.ManagedThreadId}");
            return sw.ElapsedMilliseconds;
        });
        ConsoleSync.WriteLineT($"{m} Complete Task: {delay.Status}");
        ConsoleSync.WriteLineT($"{m} Elapsed milliseconds: {delay.Result}");
        ConsoleSync.WriteLineT($"{m} ThreadMain:{Thread.CurrentThread.ManagedThreadId}");

        // The example displays output like the following:
        //        Elapsed milliseconds: 2501
    }
    public static void F12(int delayInMilliSeconds)
    {
        var m = MethodBase.GetCurrentMethod()?.Name + "()";

        ConsoleSync.WriteLineT($"{m} Start");
        ConsoleSync.WriteLineT($"{m} ThreadMain:{Thread.CurrentThread.ManagedThreadId}");

        var sw = Stopwatch.StartNew();
        var t = Task.Run(async delegate
        {
            await Task.Delay(delayInMilliSeconds).ConfigureAwait(false);
            sw.Stop();
            ConsoleSync.WriteLineT($"{m} ThreadTask:{Thread.CurrentThread.ManagedThreadId}");
            return sw.ElapsedMilliseconds;
        });
        t.Wait();
        ConsoleSync.WriteLineT($"{m} Complete");
        ConsoleSync.WriteLineT($"{m} Elapsed milliseconds: {t.Result}");
        ConsoleSync.WriteLineT($"{m} ThreadMain:{Thread.CurrentThread.ManagedThreadId}");
        ConsoleSync.WriteLineT("Task t Status: {0}, Result: {1}", t.Status, t.Result);
    }
}

public static class Program
{
    public static void Main()
    {
        const int delay = 2500;
        Example.F1(delay);
        Example.F2(delay);
        Example.F3(delay);
        Example.F4(delay);
        Example.F12(delay);
    }
}