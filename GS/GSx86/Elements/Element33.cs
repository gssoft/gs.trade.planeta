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
    public interface IElement33<TKey, in TQueueInput> : 
        IElement1<TKey>,
        IHaveQueue3<TQueueInput>,
        IHaveWork<TQueueInput>
    {
    }

    //public abstract class Element3<TKey, TItem, TEventArgs> : Element1<TKey>, IHaveQueue<TEventArgs>
    public abstract class Element33<TKey, TInput,  TEventArgs> : Element1<TKey> //, IElement3<TKey,TEventArgs>
        where TEventArgs : IEventArgs
    {
        public event EventHandler<TInput> NewItemEvent;

        protected virtual void OnNewItemEvent(TInput e)
        {
            EventHandler<TInput> handler = NewItemEvent;
            handler?.Invoke(this, e);
        }

        public event EventHandler<IEnumerable<TInput>> NewItemsEvent;

        protected virtual void OnNewItemsEvent(IEnumerable<TInput> e)
        {
            EventHandler<IEnumerable<TInput>> handler = NewItemsEvent;
            handler?.Invoke(this, e);
        }

        public bool IsQueueEnabled { get; set; }

        [XmlIgnore]
        public QueueFifo2<TInput> Queue { get; set; }

        [XmlIgnore]
        public IWork1<TEventArgs> Work { get; set; }

        //public void ObjectChangedEventHandler(object sender, TItem t)
        //{
        //}
        public void ObjectChangedEventHandler(object sender, TEventArgs t)
        {
        }

        protected Element33()
        {
            IsQueueEnabled = true;
            Queue = new QueueFifo2<TInput>();
        }

        public void Init()
        {
            if (IsQueueEnabled)
            {
                Work = new Work1<TEventArgs>
                {
                    Code = FullCode +".Work",
                    TimeInterval = 5000,
                    MainFunc = () =>
                    {
                        DeQueueProcess();
                        return true;
                    }
                };
            }
        }

        public override void Init(IEventLog eventLog)
        {
            base.Init(eventLog);
            //IsQueueEnabled = true;
            Init();
        }

        public virtual void EnQueue(TInput queueItem)
        {
            Queue.Push(queueItem);
            Work?.DoWork();
        }

        public virtual void DeQueueProcess()
        {
            if (Queue.IsEmpty)
                return;
            var items = Queue.GetItems();

            if (items.Any())
                FireEvent(items);       

        }
        public void FireEvent(IEnumerable<TInput> items)
        {
            foreach (var i in items)
            {
                OnNewItemEvent(i);
            }
            OnNewItemsEvent(items);

            if (!IsAnyExceptionEventSubscriber) return;

            foreach (var i in items)
            {
                FireChangedEvent("Add", i);
            }
        }

    }
}
