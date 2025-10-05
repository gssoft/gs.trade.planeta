using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Events;
using GS.WorkTasks;
using EventArgs = GS.Events.EventArgs;

namespace ca_Worker_Test_01
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleAsync.WriteLineT("Worker_Test");

            var worker = new ConsoleWorker();
            worker.WorkProcess.Start();

            ConsoleAsync.WriteLineT("Worker_Test: Start Pushing");
            Thread.Sleep(3000);
            foreach (var i in Enumerable.Range(1, 100))
            {
                var ea = new EventArgs
                {
                    Category = "Console",
                    Entity = "Message",
                    Operation = "New",
                    Object = i,
                };
                Console.WriteLine("SENDED --->> : {0} {1}", i, ea.ToString());
                worker.GetChangedEvent(null, ea );

                Thread.Sleep(100);
            }
            worker.Stop();
            ConsoleAsync.WriteLineT(" ****************** Stop Test *********************");
            Console.ReadLine();
        }
    }

    public class ConsoleWorker : Worker<IEventArgs>
    {
        public ConsoleWorker()
        {
            Code = "ConsoleWorker";
            WorkProcess.TaskFunc = () =>
            {
                // ConsoleAsync.WriteLineT("WorkTask is Running");
                if (InputQueue.IsEmpty)
                    return true;
                while (!InputQueue.IsEmpty)
                {
                    IEventArgs ea;
                    if(InputQueue.TryDequeue(out ea))
                        Console.WriteLine("RECEIVED <<--- : {0} {1}", ea.Object.ToString(), ea.ToString());
                }
                return true;
            };
            WorkProcess.TimeInterval = 60;
            WorkProcess.ErrorCountToStop = 1000;
            WorkProcess.IsEnabled = true;
        } 
    }
}
