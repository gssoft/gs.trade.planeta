using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Interfaces;
using GS.Queues;

namespace GS.Elements
{
    public abstract class Element1Q<TKey, TInputItem> : Element1<TKey>
    {
        protected IQueueFifo2<TInputItem> InputQueue;

        public override void Init(IEventLog evl)
        {
            base.Init(evl);
            InputQueue = new QueueFifo2<TInputItem>();
        }

        public void ObjectChangedEventHandler(object sender, TInputItem t)
        {
        }
        public void Push(TInputItem t)
        {
            InputQueue.Push(t);
        }
    }
}
