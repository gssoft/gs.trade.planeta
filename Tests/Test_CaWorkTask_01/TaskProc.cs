using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Elements;
using GS.EventLog;
using GS.Events;
using GS.Interfaces;
using GS.ProcessTasks;
using GS.Serialization;
using GS.WorkTasks;


namespace Test_CaWorkTask_01
{
    public class WriterB
    {
        private int b = 100;
        private string bs = "BBBBB";
        public void Write(IEventArgs1 eargs)
        {
            var c = 2;
            ConsoleSync.WriteLineT(c + " " + b + " " + bs + " " + eargs.Object.ToString());
        }
        public void StringEventHandler(object sender, IEventArgs1 args)
        {
            ConsoleSync.WriteLineT(args.Object.ToString());
        }
    }
    public class WriterA
    {
        private int a = 150;
        private string sa = "AAAAA";
        public void Write(IEventArgs1 eargs)
        {
            var c = 1;
            ConsoleSync.WriteLineT(c + " " + a + " " + sa + " " + eargs.Object.ToString());
        }

        public void StringEventHandler(object sender, IEventArgs1 args)
        {
            ConsoleSync.WriteLineT(args.Object.ToString());
        }
    }

    public class EventSender
    {
        public event EventHandler<IEventArgs1> StringWriteEvent;

        public virtual void OnStringWriteEvent(IEventArgs1 e)
        {
            StringWriteEvent?.Invoke(this, e);
        }
    }

    public partial class Program 
    {
        private static void TestTaskProcessor023()
        {

            var writerA = new WriterB();
            //var unitofworkA = new UnitOfWork
            //{
            //    ProcessKey = "WriterBBB",
            //    Category = "category",
            //    Entity = "entity",
            //    Action = writerA.Write
            //};
            var writerB = new WriterA();
            //var unitofworkB = new UnitOfWork
            //{
            //    ProcessKey = "WriterAAA",
            //    Category = "category",
            //    Entity = "entity",
            //    Action = writerB.Write
            //};

            //evsender.StringWriteEvent += writerA.StringEventHandler;
            //evsender.StringWriteEvent += writerB.StringEventHandler;

            var processor = new ProcessTask<IEventArgs1>();
            processor.Init(EventLog);
            processor.ItemProcessingAction = args1 =>
            {
                try
                {
                    // ConsoleSync.WriteLineT("*************************");
                    args1.Action(args1);
                }
                catch (Exception e)
                {
                    ConsoleSync.WriteLineT(e.Message);
                }
            };
            
            // processor.IsStopByCancelToken = true;
            processor.Start();

            //var processor = new TaskProcess02();
            //processor.Init(EventLog);

            //processor.Register(unitofworkA);
            //processor.Register(unitofworkB);

            //processor.Register(unitofworkeventsender);

            //  processor.Start();

            var t1 = System.Threading.Tasks.Task.Factory.StartNew(p =>
            {
                // var pr = (TaskProcess02)p;
                var pr = (ProcessTask<IEventArgs1>)p;
                var rnd = new Random(DateTime.Now.Millisecond);

                foreach (var i in Enumerable.Range(1, 100))
                {
                    var r = rnd.Next(1, 1000);
                    var j = i;
                    var ea = new GS.Events.EventArgs1
                    {
                        Process = "WriterAAA",
                        Category = "category",
                        Entity = "entity",
                        Operation = "Add",
                        Object = $"{i} {r} {DateTime.Now.ToString(@"hh\:mm\:ss\.fff")}",
                        Action = writerA.Write
                    };
                    pr.EnQueue(ea);
                    Thread.Sleep(r);
                }
            }, processor);

            var t2 = System.Threading.Tasks.Task.Factory.StartNew(p =>
            {
                var pr = (ProcessTask<IEventArgs1>)p;
                var rnd = new Random(DateTime.Now.Millisecond * 2);

                foreach (var i in Enumerable.Range(1, 100))
                {
                    var r = rnd.Next(1, 1000);
                    var j = i;
                    var ea = new GS.Events.EventArgs1
                    {
                        Process = "WriterBBB",
                        Category = "category",
                        Entity = "entity",
                        Operation = "Add",
                        Object = $"{i} {r} {DateTime.Now.ToString(@"hh\:mm\:ss\.fff")} Welcome to My nigthmare",
                        Action = writerB.Write
                    };
                    pr.EnQueue(ea);
                    Thread.Sleep(r);
                }
            }, processor);

            System.Threading.Tasks.Task.WaitAll(t1, t2);

            ConsoleSync.WriteReadLineT("Press Any Key to Task.Stop()");
            processor.Stop();
            var cnt = 5;
            while (cnt-- > 0)
            {
                // ConsoleSync.WriteLineT("Waiting for Completing Task ");
                Thread.Sleep(1000);
            }
            ConsoleSync.WriteReadLineT("Press Any Key to Finish Program");
        }
        private static void TestTaskProcessor021()
        {

            var writerA = new WriterB();
            //var unitofworkA = new UnitOfWork
            //{
            //    ProcessKey = "WriterBBB",
            //    Category = "category",
            //    Entity = "entity",
            //    Action = writerA.Write
            //};
            var writerB = new WriterA();
            //var unitofworkB = new UnitOfWork
            //{
            //    ProcessKey = "WriterAAA",
            //    Category = "category",
            //    Entity = "entity",
            //    Action = writerB.Write
            //};

            //evsender.StringWriteEvent += writerA.StringEventHandler;
            //evsender.StringWriteEvent += writerB.StringEventHandler;

            var processor = new ProcessTask02();
            processor.Init(EventLog);
            // processor.IsStopByCancelToken = true;
            processor.Start();

            //var processor = new TaskProcess02();
            //processor.Init(EventLog);

            //processor.Register(unitofworkA);
            //processor.Register(unitofworkB);

            //processor.Register(unitofworkeventsender);

            //  processor.Start();

            var t1 = System.Threading.Tasks.Task.Factory.StartNew(p =>
            {
                // var pr = (TaskProcess02)p;
                var pr = (ProcessTask02)p;
                var rnd = new Random(DateTime.Now.Millisecond);

                foreach (var i in Enumerable.Range(1, 100))
                {
                    var r = rnd.Next(1, 1000);
                    var j = i;
                    var ea = new GS.Events.EventArgs1
                    {
                        Process = "WriterAAA",
                        Category = "category",
                        Entity = "entity",
                        Operation = "Add",
                        Object = $"{i} {r} {DateTime.Now.ToString(@"hh\:mm\:ss\.fff")}",
                        Action = writerA.Write
                    };
                    pr.EnQueue(ea);
                    Thread.Sleep(r);
                }
            }, processor);

            var t2 = System.Threading.Tasks.Task.Factory.StartNew(p =>
            {
                var pr = (ProcessTask02)p;
                var rnd = new Random(DateTime.Now.Millisecond * 2);

                foreach (var i in Enumerable.Range(1, 100))
                {
                    var r = rnd.Next(1, 1000);
                    var j = i;
                    var ea = new GS.Events.EventArgs1
                    {
                        Process = "WriterBBB",
                        Category = "category",
                        Entity = "entity",
                        Operation = "Add",
                        Object = $"{i} {r} {DateTime.Now.ToString(@"hh\:mm\:ss\.fff")} Welcome to My nigthmare",
                        Action = writerB.Write
                    };
                    pr.EnQueue(ea);
                    Thread.Sleep(r);
                }
            }, processor);
           
            System.Threading.Tasks.Task.WaitAll(t1,t2);

            ConsoleSync.WriteReadLineT("Press Any Key to Task.Stop()");
            processor.Stop();
            var cnt = 5;
            while (cnt-- > 0)
            {
                // ConsoleSync.WriteLineT("Waiting for Completing Task ");
                Thread.Sleep(1000);
            }
            ConsoleSync.WriteReadLineT("Press Any Key to Finish Program");
        }
        private static void TestTaskProcessor02() {
           
            var writerA = new WriterB();
            var unitofworkA = new UnitOfWork
            {
                ProcessKey = "WriterBBB",
                Category = "category",
                Entity = "entity",
                Action = writerA.Write
            };
            var writerB = new WriterA();
            var unitofworkB = new UnitOfWork
            {
                ProcessKey = "WriterAAA",
                Category = "category",
                Entity = "entity",
                Action = writerB.Write
            };

            //evsender.StringWriteEvent += writerA.StringEventHandler;
            //evsender.StringWriteEvent += writerB.StringEventHandler;

            var processor = new TaskProcess02();
            processor.Init(EventLog);

            processor.Register(unitofworkA);
            processor.Register(unitofworkB);
            //processor.Register(unitofworkeventsender);

            processor.Start();

            System.Threading.Tasks.Task.Factory.StartNew(p =>
            {
                var pr = (TaskProcess02) p;
                var rnd = new Random(DateTime.Now.Millisecond);

                foreach (var i in Enumerable.Range(1, 100))
                {
                    var r = rnd.Next(1, 1000);
                    var j = i;
                    var ea = new GS.Events.EventArgs1
                    {
                        Process = "WriterAAA",
                        Category = "category",
                        Entity = "entity",
                        Operation = "Add",
                        Object = $"{i} {r} {DateTime.Now.ToString(@"hh\:mm\:ss\.fff")}"
                    };
                    pr.EnQueue(null, ea);              
                    Thread.Sleep(r);
                }
            }, processor);

            System.Threading.Tasks.Task.Factory.StartNew(p =>
            {
                var pr = (TaskProcess02)p;
                var rnd = new Random(DateTime.Now.Millisecond*2);

                foreach (var i in Enumerable.Range(1, 100))
                {
                    var r = rnd.Next(1, 1000);
                    var j = i;
                    var ea = new GS.Events.EventArgs1
                    {
                        Process = "WriterBBB",
                        Category = "category",
                        Entity = "entity",
                        Operation = "Add",
                        Object = $"{i} {r} {DateTime.Now.ToString(@"hh\:mm\:ss\.fff")} Welcome to My nigthmare"
                    };
                    pr.EnQueue(null, ea);
                    Thread.Sleep(r);
                }
            }, processor);

            ConsoleSync.WriteReadLineT("Press Any Key to Task.Stop()");
            processor.Stop();
            var cnt = 60;
            while (cnt-- > 0)
            {
                ConsoleSync.WriteLineT("Waiting for Completing Task ");
                Thread.Sleep(1000);    
            }
            ConsoleSync.WriteReadLineT("Press Any Key to Finish Program");
        }

        private static void TestTaskProcessor01()
        {
            var evsender = new EventSender();
            var unitofworkeventsender = new UnitOfWork
            {
                ProcessKey = "EventSender",
                Category = "EventHub",
                Entity = "Event",
                Action = evsender.OnStringWriteEvent
            };
            var writerA = new WriterB();
            var unitofworkA = new UnitOfWork
            {
                ProcessKey = "WriterBBB",
                Category = "category",
                Entity = "entity",
                Action = writerA.Write
            };
            var writerB = new WriterA();
            var unitofworkB = new UnitOfWork
            {
                ProcessKey = "WriterAAA",
                Category = "category",
                Entity = "entity",
                Action = writerB.Write
            };

            //evsender.StringWriteEvent += writerA.StringEventHandler;
            //evsender.StringWriteEvent += writerB.StringEventHandler;

            var processor = new TaskProcess01();
            processor.Init(EventLog);

            processor.Register(unitofworkA);
            processor.Register(unitofworkB);
            //processor.Register(unitofworkeventsender);

            processor.Start();

            System.Threading.Tasks.Task.Factory.StartNew(p =>
            {
                var pr = (TaskProcess01)p;
                var rnd = new Random(DateTime.Now.Millisecond);

                foreach (var i in Enumerable.Range(1, 100))
                {
                    var r = rnd.Next(1, 1000);
                    var j = i;
                    var ea = new GS.Events.EventArgs1
                    {
                        Process = "WriterAAA",
                        Category = "category",
                        Entity = "entity",
                        Operation = "Add",
                        Object = $"{i} {r} {DateTime.Now.ToString(@"hh\:mm\:ss\.fff")}"
                    };
                    pr.EnQueue(null, ea);
                    Thread.Sleep(r);
                }
            }, processor);

            System.Threading.Tasks.Task.Factory.StartNew(p =>
            {
                var pr = (TaskProcess01)p;
                var rnd = new Random(DateTime.Now.Millisecond * 2);

                foreach (var i in Enumerable.Range(1, 100))
                {
                    var r = rnd.Next(1, 1000);
                    var j = i;
                    var ea = new GS.Events.EventArgs1
                    {
                        Process = "WriterBBB",
                        Category = "category",
                        Entity = "entity",
                        Operation = "Add",
                        Object = $"{i} {r} {DateTime.Now.ToString(@"hh\:mm\:ss\.fff")} Welcome to My nigthmare"
                    };
                    pr.EnQueue(null, ea);
                    Thread.Sleep(r);
                }
            }, processor);

            ConsoleSync.WriteReadLineT("Press Any Key to Task.Stop()");
            processor.Stop();
            var cnt = 60;
            while (cnt-- > 0)
            {
                ConsoleSync.WriteLineT("Waiting for Completing Task ");
                Thread.Sleep(1000);
            }

            ConsoleSync.WriteReadLineT("Press Any Key to Finish Program");
        }
    }
}
