using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GS.Queues
{
    public interface IHaveQueue
    {
        bool IsQueueEnabled { get; }
        void DeQueueProcess();
    }

    public interface IHaveQueue<in T> : IHaveQueue
    {
        void Push(T queueItem);   
    }

    public interface IHaveEnQueue<in TQueueITem>
    {
        void EnQueue(object sender, TQueueITem queueItem);
    }

    public interface IHaveQueue3<in T> : IHaveQueue
    {
        //void Push(T queueItem);
        //void EnQueue(T queueItem);
        void EnQueue(object sender, T queueItem);
    }

    public interface IQueueFifo<T>
    {
        bool IsEmpty { get; }
        IEnumerable<T> Items { get; }
        IEnumerable<T> GetItems();
        void Push(T item);
        bool Get(out T item);
    }

    public class QueueFifo<T> : IQueueFifo<T>
    {
        protected ConcurrentQueue<T> Queue { get; set; }
        private readonly object _locker; 

        public QueueFifo()
        {
            _locker = new object();
            Queue = new ConcurrentQueue<T>();
        }
        // 01.05.2018
        public int Count => Queue.Count;

        public void Push(T item)
        {
            Queue.Enqueue(item);
        }

        public bool Get(out T item)
        {
            lock (_locker)
            {
                return Queue.TryDequeue(out item);
            }
        }
        // 01.05.2018
        public bool IsEmpty => Queue.IsEmpty;

        public void Clear()
        {
            if (Queue.IsEmpty)
                return;
            lock (_locker)
            {
                T item;
                while (Queue.TryDequeue(out item))
                {
                }
            }
        }
        public IEnumerable<T> Items => GetItems();
        public IEnumerable<T> GetItems()
        {
            var l = new List<T>();

            lock (_locker)
            {
                T item;
                // var l = new List<T>();
                while (Queue.TryDequeue(out item))
                {
                    l.Add(item);
                }
            }
            return l;
        }
        public IEnumerable<T> Take(int cnt)
        {
            if (cnt <= 0)
                return null;

            var l = new List<T>();

            lock (_locker)
            {
                T item;
                //var l = new List<T>();
                while (cnt > 0 && Queue.TryDequeue(out item))
                {
                    l.Add(item);
                    cnt--;
                }
            }
            return l;
        }
        public IEnumerable<T> Take2(int cnt)
        {
            lock (_locker)
            {
                return cnt <= 0 ? null : Queue.Take(cnt).ToList();
                // return cnt <= 0 ? Enumerable.Empty<T>() : Queue.Take(cnt).ToList();
            }
        }
        //public void DeQueueProcess()
        //{
        //    if (Queue.IsEmpty)
        //        return;

        //    var items = GetItems();
        //    foreach (var i in items)
        //    {
        //        ItemToProcess(i);
        //    }
        //}

        //public abstract void ItemToProcess(T t);
    }
}
