// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");
// https://learn.microsoft.com/ru-ru/dotnet/api/system.threading.tasks.task.delay?view=net-7.0
// TASK DELAY

using System.Diagnostics;

namespace CaTaskDelay04;
public class Example
{
    // F4
    public static void Main()
    {
        var delay = Task.Run(async () => {
            Stopwatch sw = Stopwatch.StartNew();
            await Task.Delay(2500);
            sw.Stop();
            return sw.ElapsedMilliseconds;
        });

        Console.WriteLine("Elapsed milliseconds: {0}", delay.Result);
        // The example displays output like the following:
        //        Elapsed milliseconds: 2501
    }
}
