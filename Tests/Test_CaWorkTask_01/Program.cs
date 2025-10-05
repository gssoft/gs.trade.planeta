using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Elements;
using GS.EventLog;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using GS.Queues;
using GS.Serialization;
using GS.Works;
using GS.WorkTasks;

namespace Test_CaWorkTask_01
{
    using EventArgs = GS.Events.EventArgs;
    public class Reader : Element1<string>
    {
        public Reader()
        {
            Code = "Reader";
            InputQueue = new QueueFifo2<string>();
        }

        public void EnQueue(string s)
        {
            InputQueue.Push(s);
        }

        public bool ReadFromQueue()
        {
            //var ss = InputQueue.GetItems();
            //foreach(var s in ss)
            //    Console.WriteLine(s);
            string s;
            while (InputQueue.Get(out s))
            {
                ConsoleAsync.WriteLineT("RECEIVED <<--- : {0}", s);
            }
            return true;
        }

        protected QueueFifo2<string> InputQueue;

        public override string Key => Code.HasValue()? Code : GetType().FullName;
    }

    public class ConsoleWriter : Element3<string,IEventArgs>
    {
        public override string Key => FullName;

        public override void DeQueueProcess()
        {
            //ConsoleSync.WriteLineT("DeQueueProcess");
            var ss = Queue.GetItems().ToList();
            if (!ss.Any())
                return;
            foreach (var s in ss)
            {
                ConsoleSync.WriteLineT("WRITER RECEIVED >> {0}", s.Object);    
            }
        }
    }  
    public partial class Program 
    {
        protected static EventHub3 EventHub;
        protected static WorkTasks WorkTasks;
        protected static IEventLog EventLog;

        static void Main(string[] args)
        {
            EventLog = Builder.Build<EventLogs>(@"Init\EventLog.xml", "EventLogs");
            EventLog.Init();

            WorkTasks = Builder.Build2<WorkTasks>(@"Init\WorkTasks.xml", "WorkTasks");
            WorkTasks.Init(EventLog);

            EventHub = Builder.Build<EventHub3>(@"Init\EventHub3.xml", "EventHub3");
            EventHub.Init(EventLog);

            // TestWorkTask();
            // TestTaskProcessor01();
            // TestTaskProcessor02();
           // TestTaskProcessor021();
            TestTaskProcessor023();
        }

        private static void TestWorkTask()
        {
            EventHub.Subscribe("Quotes","Quote", QuotesEnQueue);
            
            var wt1 = WorkTasks.GetByKey("EventHubReceiver");

            var cw = new ConsoleWriter
            {
                Code = "Writer"
            };
            cw.Init(EventLog);

            //cw.Work.WorkTask = wt1;
            wt1.Works.Register(cw.Work);
            var evhWork = EventHub.Work;
            wt1.Works.Register(evhWork);

            //var ws = new Works();
            //var w = cw.Work;
            //w.WorkTask = wt1;
            //ws.Register(w);

            //wt1.Works = ws;
            wt1.Start();

            Thread.Sleep(1000);
            ConsoleSync.WriteReadLine("Press any key");

            foreach (var i in Enumerable.Range(1, 10000))
            {
                ConsoleSync.WriteLineT("SENDED >> {0}", i);
                var ea = new EventArgs
                {
                    Category = "Quotes",
                    Entity = "Quote",
                    Object = i.ToString()
                };
                cw.EnQueue(cw,ea);
                EventHub.EnQueue( cw, ea);
                
                Thread.Sleep(10);
            }
            ConsoleSync.WriteReadLine("Press any key");

            wt1.Stop();

            ConsoleSync.WriteReadLine("Press any key");
            
            var r = new Reader();
            var work = new Work1<string>
            {
                Parent = null,
                Code = "MyWork",
                Description = @"Work that's I work",
                ErrorCountToStop = 3,
                //MainFunc = () => r.ReadFromQueue(),
                InitFunc = () =>
                {
                    ConsoleSync.WriteLineT("Worker: {0}. Work: {1} Init Complete", r.Code, r.Code);
                    return true;
                },
                FinishFunc = () =>
                {
                    ConsoleSync.WriteLineT("Worker: {0}. Work: {1} Finish Complete", r.Code, r.Code);
                    return true;
                },
            };
            work.MainFunc = () =>
            {
                var ss = work.InputQueue.GetItems();
                foreach(var s in ss)
                    ConsoleSync.WriteLineT("RECEIVEDED >> : {0} {1}", s, work.Code);
                return true;
            };
            work.ExceptionEvent += (s, ea) =>
            {
                ConsoleAsync.WriteLine("************* {0} *************\r\n Worker: {0} Catch Exception: {1}",
                    work.Code, ea.ToString());
                Console.ReadLine();
            };
            var work2 = new Work1<string>
            {
                Parent = null,
                Code = "MyWork2",
                Description = @"Work2 that's I work",
                ErrorCountToStop = 3,
                //MainFunc = () => r.ReadFromQueue(),
                InitFunc = () =>
                {
                    ConsoleSync.WriteLineT("Worker: {0}. Work: {1} Init Complete", r.Code, r.Code);
                    return true;
                },
                FinishFunc = () =>
                {
                    ConsoleSync.WriteLineT("Worker: {0}. Work: {1} Finish Complete", r.Code, r.Code);
                    return true;
                },
            };
            work2.MainFunc = () =>
            {
                var ss = work2.InputQueue.GetItems();
                foreach (var s in ss)
                    ConsoleSync.WriteLineT("RECEIVEDED >> : {0} {1}", s, work2.Code);
                throw new NullReferenceException("Null Reference");
            };
            work2.ExceptionEvent += (s, ea) =>
            {
                ConsoleAsync.WriteLine("************* {0} *************\r\n Worker: {0} Catch Exception: {1}",
                    work.Code, ea.ToString());
                // Console.ReadLine();
            };
            // Works
            var ws = new Works();
            //ws.Register(work);
            //ws.Register(work2);

            var receiver = new GS.Works.Receiver<string>
            {
                Code = "Receiver",
                Description = @"Work that's I work",
                PushItem = (s) =>
                {
                    ConsoleSync.WriteLineT("RECEIVED >> : {0} {1}", s, "Programm");
                },
            };
            ws.Register(receiver.Work);

            var workTask = new WorkTask3
            {
                Code = "WorkTask",
                //Work = work,
                //Works = receiver.Work,
                TimeInterval = 5,
                ErrorCountToStop = 1,
                IsEnabled = true
            };
            receiver.Work.WorkTask = workTask;
            receiver.ExceptionEvent += (s, ea) =>
            {
                ConsoleAsync.WriteLine("************* {0} *************\r\n Worker: {0} Catch Exception: {1}",
                    receiver.Code, ea.ToString());
                Console.ReadLine();
            };
            
            work.WorkTask = workTask;
            work2.WorkTask = workTask;
            
            ConsoleSync.WriteLineT("Try to Start");
            Console.ReadLine();


            workTask.Start();
            foreach (var i in Enumerable.Range(1, 10))
            {
                ConsoleSync.WriteLineT("SENDED >> : {0}", i);
                if(i%2==0)
                    receiver.EnQueue(i.ToString());
                else
                    receiver.EnQueue(i.ToString());
                Thread.Sleep(100);
            }
            Console.ReadLine();
            ConsoleAsync.WriteLine("Try to Stop");
            workTask.Stop();
            ConsoleSync.WriteLineT(" ****************** Stop Test *********************");
            Thread.Sleep(5000);
            Console.ReadLine();
        }
        private static void QuotesEnQueue(object sender, IEventArgs ea)
        {
            ConsoleSync.WriteLineT("EVENTHUB RECEIVED >> " + ea.Object.ToString());
        }    
    }

}
