using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;

namespace CaWorker01
{
    public class Worker // : BackgroundService
    {
        //private readonly ILogger<Worker> _logger;

        //public Worker(ILogger<Worker> logger)
        //{
        //    _logger = logger;
        //}

        public static async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var thread = Thread.CurrentThread;
            var count = 0;
            ConsoleSync.WriteLineT($"Worker => Thread:{thread.ManagedThreadId} Starting ...");
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    ++count;
                    // thread = Thread.CurrentThread;
                    ConsoleSync.WriteLineT($"Worker {count} => Thread:{Thread.CurrentThread.ManagedThreadId} Running ...");
                    // ConsoleSync.WriteLineT("Worker => Worker running ...");
                    await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
                    // thread = Thread.CurrentThread;
                    var fact = Factorial(15);
                    ConsoleSync.WriteLineT($"Worker {count} Factorial: {fact}");
                    ConsoleSync.WriteLineT($"Worker {count} => Thread:{Thread.CurrentThread.ManagedThreadId} Running After Await...");
                }
            }
            catch (Exception e)
            {
                thread = Thread.CurrentThread;
                ConsoleSync.WriteLineT($"Worker => Thread:{thread.ManagedThreadId} Exception ...");
                ConsoleSync.WriteLineT("Worker =>" + "Exception: " + e.Message);
                ConsoleSync.WriteLineT("Worker =>\r\n\r\n" +  e);
            }
        }

        public static ulong Factorial(int x)
        {
            ulong result = 1;
            while (x > 1)
            {
                result *= (ulong)x--;
            }
            return result;
        }
    }
}
