﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Buffers
{
    [Serializable]
    [DebuggerDisplay("Count = {" + nameof(Count) + "}, Limit = {" + nameof(Limit) + "}")]
    public class FixedSizedQueue<T> : IReadOnlyCollection<T>
    {
        private readonly Queue<T> _queue = new Queue<T>();
        private readonly object _lock = new object();

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _queue.Count;
                }
            }
        }

        public int Limit { get; }

        public FixedSizedQueue(int limit)
        {
            if (limit < 1)
                throw new ArgumentOutOfRangeException(nameof(limit));

            Limit = limit;
        }

        public FixedSizedQueue(IEnumerable<T> collection)
        {
            var enumerable = collection as T[] ?? collection.ToArray();
            if (collection != null || !enumerable.Any())
                throw new ArgumentException("Can not initialize the Queue with a null or empty collection",
                    nameof(collection));

            _queue = new Queue<T>(enumerable);
            Limit = _queue.Count;
        }
        //public FixedSizedQueue(IEnumerable<T> collection)
        //{
        //    if (collection != null || !collection.Any())
        //        throw new ArgumentException("Can not initialize the Queue with a null or empty collection",
        //            nameof(collection));

        //    _queue = new Queue<T>(collection);
        //    Limit = _queue.Count;
        //}

        public void Enqueue(T obj)
        {
            lock (_lock)
            {
                _queue.Enqueue(obj);

                while (_queue.Count > Limit)
                    _queue.Dequeue();
            }
        }

        public void Clear()
        {
            lock (_lock)
                _queue.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_lock)
                return new List<T>(_queue).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}


