using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Elements;
using GS.Extension;
using GS.Queues;
using GS.WorkTasks;

namespace GS.Works
{
    public class Receiver<TInput> : Element1<string>
    {
        public QueueFifo2<TInput> Queue { get; private set; }
        public IWork1<TInput> Work { get; private set; }

        public Action<TInput> PushItem { get; set; }
        public Action<IEnumerable<TInput>> PushItems { get; set; }

        public Receiver()
        {
            Queue = new QueueFifo2<TInput>();
            Work = new Work1<TInput>
            {
                Parent = this,
                Code = "MyWork",
                Description = @"Work that's I work",
                ErrorCountToStop = 3,
                //MainFunc = () => r.ReadFromQueue(),
                InitFunc = () =>
                {
                    ConsoleSync.WriteLineT("Work: {0}. Work: {1} Init Complete", Code, Code);
                    return true;
                },
                MainFunc = () =>
                {
                    if (PushItem != null)
                    {
                        TInput t;
                        while (Queue.Get(out t))
                        {
                            //ConsoleSync.WriteLineT("RECEIVED >> : {0} {1}", t, Code);
                            PushItem(t);                      
                        }
                    }
                    else if (PushItems != null)
                    {
                        var ss = Queue.GetItems();
                        foreach (var s in ss)
                            ConsoleSync.WriteLineT("RECEIVEDED >> : {0} {1}", s, Code);
                        PushItems(ss);
                    }
                    return true;
                },
                FinishFunc = () =>
                {
                    ConsoleSync.WriteLineT("Worker: {0}. Work: {1} Finish Complete", Code, Code);
                    return true;
                },
            };
            Work.ExceptionEvent += (s, ea) =>
            {
                ConsoleAsync.WriteLine("************* {0} *************\r\n Worker: {0} Catch Exception: {1}",
                    Code, ea.ToString());
                Console.ReadLine();
            };
        }

        public void EnQueue(TInput t)
        {
            Queue.Push(t);
            Work?.DoWork();
        }
        public override string Key => Code.HasValue() ? Code : GetType().FullName;
    }
}
