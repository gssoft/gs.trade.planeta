using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Queues
{
    public interface IQueueFifo2<T>
    {
        bool IsEmpty { get; }
        IEnumerable<T> PeekItems();
        IEnumerable<T> GetItems();
        void Push(T item);
        bool Get(out T item);
        T Get();
        bool Peek(out T item);
    }
    public class QueueFifo2<T>: IQueueFifo2<T>
    {
        protected ConcurrentQueue<T> Queue { get; set; }
        private readonly object _locker; 

        public QueueFifo2()
        {
            _locker = new object();
            Queue = new ConcurrentQueue<T>();
        }

        public int Count => Queue.Count;

        public void Push(T item)
        {
            Queue.Enqueue(item);
        }

        public bool Get(out T item)
        {
           return Queue.TryDequeue(out item);
        }
        public bool Peek(out T item)
        {
            return Queue.TryPeek(out item);
        }

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
        /// <summary>
        /// PeekItems
        /// </summary>
        public IEnumerable<T> Items
        {
            get
            {
                return PeekItems();
            }
        }

        public T Get()
        {
            T item;
            Queue.TryDequeue(out item);
            return item;
        }
        //public bool GetItem(out T i)
        //{
        //    return Queue.TryDequeue(out i);
        //}

        public IEnumerable<T> GetItems()
        {
                T item;
                var l = new List<T>();
                while (Queue.TryDequeue(out item))
                {
                    l.Add(item);
                }
                return l;
        }
        public IEnumerable<T> PeekItems()
        {
                T item;
                var l = new List<T>();
                while (Queue.TryPeek(out item))
                {
                    l.Add(item);
                }
                return l;
        }
        public IEnumerable<T> Take(int cnt)
        {
            if (cnt <= 0)
                return null;
            lock (_locker)
            {
                T item;
                var l = new List<T>();
                while (cnt > 0 && Queue.TryDequeue(out item))
                {
                    l.Add(item);
                    cnt--;
                }
                return l;
            }
        }
        public IEnumerable<T> Take2(int cnt)
        {
            lock (_locker)
            {
                return cnt <= 0 ? null : Queue.Take(cnt).ToList();
            }
        }
    }
}
