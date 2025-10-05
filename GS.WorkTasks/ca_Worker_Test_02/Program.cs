using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Events;
using GS.WorkTasks;
using GS.Works;
using EventArgs = GS.Events.EventArgs;

namespace ca_Worker_Test_02
{
    class Program
    {
        static void Main(string[] args)
        {
            var excCount = 0;
            ConsoleAsync.WriteLineT("Worker_Test");

            var worker = new ConsoleWorker();
            worker.ExceptionEvent += (s, ea) => ConsoleAsync.WriteLine("************* {2} *************\r\n Worker: {0} Catch Exception: {1}",
                                                        worker.Code, ea.ToString(), ++excCount);
            worker.WorkProcess.Code = "MyWorkTask";
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
                worker.GetChangedEvent(null, ea);


                Thread.Sleep(100);
            }
            Console.WriteLine("Worker_Task: Stop Pushing");
            Console.ReadLine();
            worker.Stop();
            ConsoleAsync.WriteLineT(" ****************** Stop Test *********************");
            Console.ReadLine();
            ConsoleAsync.WriteLineT("Work.Status: {0}", worker.WorkProcess.Work.Status);
            ConsoleAsync.WriteLineT("WorkTask: {0} Status: {1}," +
                                    "\r\n IsCanceled: {2}," +
                                    "\r\n IsCompleted: {3}," +
                                    "\r\n IsFaulted: {4}.",
                worker.WorkProcess.Code, worker.WorkProcess.Task.Status,
                worker.WorkProcess.Task.IsCanceled,
                worker.WorkProcess.Task.IsCompleted,
                worker.WorkProcess.Task.IsFaulted
                );
            Console.ReadLine();
        }
    }

    public class ConsoleWorker : Worker2<IEventArgs>
    {
        public ConsoleWorker()
        {
            Code = "ConsoleWorker";
            var work = new Work
            {
                Code = "MyWork",
                Description = @"Work that's I work",
                ErrorCountToStop = 3,
                MainFunc = () =>
                {
                    //throw new NullReferenceException("NullRefrence Exception");
                    //ConsoleAsync.WriteLineT("WorkTask is Running");
                    if (InputQueue.IsEmpty)
                        return true;
                    while (!InputQueue.IsEmpty)
                    {
                        IEventArgs ea;
                        if (InputQueue.TryDequeue(out ea))
                            Console.WriteLine("RECEIVED <<--- : {0} {1}", ea.Object.ToString(), ea.ToString());
                    }
                    return true;
                },
                InitFunc = () =>
                {
                    ConsoleAsync.WriteLineT("Worker: {0}. Work: {1} Init Complete", Code, WorkProcess.Work.Code);
                    return true;
                },
                FinishFunc = () =>
                {
                    ConsoleAsync.WriteLineT("Worker: {0}. Work: {1} Finish Complete", Code, WorkProcess.Work.Code  );
                    return true;
                },
            };
            // ExceptionEvent += (s, ea) => ConsoleAsync.WriteLine("Worker: {0}: Catch Exception: {1}", Code, ea.ToString());
 //           WorkProcess.Work = null;
            WorkProcess.Work = work;
            WorkProcess.TimeInterval = 5;
            WorkProcess.ErrorCountToStop = 1000;
            WorkProcess.IsEnabled = true;
        }
    }
}
