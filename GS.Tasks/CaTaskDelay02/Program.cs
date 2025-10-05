// https://learn.microsoft.com/ru-ru/dotnet/api/system.threading.tasks.task.delay?view=net-7.0

using GS.ConsoleAS;

namespace CaTaskDelay02;

public class Example
{
    public static void Main()
    {
        var t = Task.Run(async delegate
        {
            try
            {
                // origin
                // await Task.Delay(TimeSpan.FromSeconds(1.5));
                // return 42;

                await Task.Delay(TimeSpan.FromSeconds(1.5));
                throw new NullReferenceException("Null reference");
                return 42;
            }
            catch (Exception exp)
            {
                ConsoleSync.WriteLineT($"{exp.GetType().Name}: {exp.GetType().FullName} {exp.Message}");
                return 101;
            }
        });
        t.Wait();
        ConsoleSync.WriteLineT("Task t Status: {0}, Result: {1}", t.Status, t.Result);
    }
}
// The example displays the following output:
//        Task t Status: RanToCompletion, Result: 42
