using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.ConsoleAS;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using GS.Queues;
using GS.Works;

namespace GS.Elements
{
    // Element3 = Element1 + Queue + Work
    public interface IElement3<TKey, in TQueueInput> : 
        IElement1<TKey>,
        IHaveQueue3<TQueueInput>,
        IHaveWork<TQueueInput>
    {
    }

    //public abstract class Element3<TKey, TItem, TEventArgs> : Element1<TKey>, IHaveQueue<TEventArgs>
    public abstract class Element3<TKey,  TEventArgs> : Element1<TKey>, IElement3<TKey,TEventArgs>
        where TEventArgs : IEventArgs
    {
        public bool IsQueueEnabled { get; set; }

        [XmlIgnore]
        public QueueFifo2<TEventArgs> Queue { get; set; }

        [XmlIgnore]
        public IWork1<TEventArgs> Work { get; set; }

        //public void ObjectChangedEventHandler(object sender, TItem t)
        //{
        //}
        public void ObjectChangedEventHandler(object sender, TEventArgs t)
        {
        }

        protected Element3()
        {
            Queue = new QueueFifo2<TEventArgs>();
        }

        public override void Init(IEventLog eventLog)
        {
            base.Init(eventLog);

            IsQueueEnabled = true;
            if (IsQueueEnabled)
            {
                Work = new Work1<TEventArgs>
                {
                    Code = "Work." + Code,
                    TimeInterval = 5000,
                    MainFunc = () =>
                    {
                        DeQueueProcess();
                        return true;
                    }
                };
            }
        }
        //public void Push(TEventArgs queueItem)
        //{
        //    //ConsoleSync.WriteLineT("PUSHED >> " + queueItem.Object  );
        //    Queue.Push(queueItem);
        //    if (Work != null)
        //        Work.DoWork();
        //}

        //public void EnQueue(TEventArgs queueItem)
        //{
        //    Queue.Push(queueItem);
        //    if (Work != null)
        //        Work.DoWork();
        //}

        public virtual void EnQueue(object sender, TEventArgs queueItem)
        {
            Queue.Push(queueItem);
            Work?.DoWork();
        }

        //public void DeQueueProcess()
        //{
        //    //ConsoleSync.WriteLineT("DeQueueProcess");
        //    var ss = Queue.GetItems().ToList();
        //    if (!ss.Any())
        //        return;
        //    foreach (var s in ss)
        //    {
        //        ConsoleSync.WriteLineT("RECEIVED >> {0}", s.Object);    
        //    }
        //}
        public abstract void DeQueueProcess();
        
    }
}
