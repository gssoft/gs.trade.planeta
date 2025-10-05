using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Extension;

namespace GS.WorkTasks
{
    public abstract class Worker<T> : Element1<string>
    {
        public WorkTask WorkProcess { get; set; }
        protected ConcurrentQueue<T> InputQueue;

        protected Worker()
        {
            InputQueue = new ConcurrentQueue<T>();
            WorkProcess = new WorkTask();
        }
        /// <summary>
        /// Get Entity from Senders on EntityChangedEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="t"></param>
        public void GetChangedEvent(object sender, T t)
        {
            InputQueue.Enqueue(t);
            WorkProcess.DoWork();
        }

        public bool Start()
        {
            return true;
        }

        public bool Stop()
        {
            return true;
        }

        public override string Key
        {
            get { return Code.HasValue() ? Code: "Unknown"; }
        }
    }

    public abstract class Worker2<T> : Element1<string>
    {
        public WorkTask2 WorkProcess { get; set; }
        protected ConcurrentQueue<T> InputQueue;

        protected Worker2()
        {
            InputQueue = new ConcurrentQueue<T>();
            WorkProcess = new WorkTask2 {Parent = this};
        }
        /// <summary>
        /// Get Entity from Senders on EntityChangedEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="t"></param>
        public void GetChangedEvent(object sender, T t)
        {
            InputQueue.Enqueue(t);
            WorkProcess.DoWork();
        }

        public bool Start()
        {
            if(WorkProcess != null)
                WorkProcess.Start();
            return true;
        }

        public bool Stop()
        {
            if (WorkProcess != null)
                WorkProcess.Stop();
            return true;
        }

        public override string Key
        {
            get { return Code.HasValue() ? Code : "Unknown"; }
        }
    }
}
