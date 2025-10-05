// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");
// https://learn.microsoft.com/en-us/dotnet/api/system.threading.thread.currentthread?view=net-7.0

using System.Collections.Generic;
using System.Runtime.Serialization;
using GS.ConsoleAS;

namespace CaThreadProperty01;

public class Example
{
    private static readonly object LockObj = new();
    private static readonly object RndLock = new();

    private const int Iterations = 35;
    public static void Main()
    {
        var rnd = new System.Random();
        List<Task<double>> tasks = new List<Task<double>>();
        ShowThreadInformation("Application");

        var t = Task.Run(() => {
            ShowThreadInformation("Main Task(Task #" + Task.CurrentId.ToString() + ")");
            for (var ctr = 1; ctr <= Iterations; ctr++)
                tasks.Add(Task.Factory.StartNew(
                    () => {
                        ShowThreadInformation("Task #" + Task.CurrentId.ToString());
                        long s = 0;
                        for (var n = 0; n <= 999999; n++)
                        {
                            lock (RndLock)
                            {
                                s += rnd.Next(1, 1000001);
                            }
                        }
                        return s / 1000000.0;
                    }));
            var arr = tasks.ToArray();
            Task.WaitAll(arr);
            double grandTotal = 0;
            ConsoleSync.WriteLineT("Means of each task: ");
            var j = 0;
            foreach (var child in tasks)
            {
                j++;
                ConsoleSync.WriteLineT("   {0} {1}", j, child.Result);
                grandTotal += child.Result;
            }
            ConsoleSync.WriteLineT("");
            return grandTotal / Iterations;
        });
        ConsoleSync.WriteLineT("Mean of Means: {0}", t.Result);
    }

    private static void ShowThreadInformation(string taskName)
    {
        string? msg = null;
        var thread = Thread.CurrentThread;
        SerializationInfo info;
        lock (LockObj)
        {
            msg = $"{taskName} thread information\n" +
                  $"   Background: {thread.IsBackground}\n" +
                  $"   Thread Pool: {thread.IsThreadPoolThread}\n" +
                  $"   Thread ID: {thread.ManagedThreadId}\n" +
                  $"   Thread State: {thread.ThreadState}\n" +
                  $"   Thread ExeContext: {thread.ExecutionContext}\n" +
 //                 $"   Thread ExeContext: {thread.ExecutionContext.GetObjectData(}\n" +
                  $"   Thread ExePriority: {thread.Priority}\n" +
                  $"   Thread Is" + $"Alive: {thread.IsAlive}\n"
                  ;
        }
        ConsoleSync.WriteLineT(msg);
    }
}