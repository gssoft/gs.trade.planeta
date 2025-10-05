using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Buffers
{
    // https://stackoverflow.com/questions/5852863/fixed-size-queue-which-automatically-dequeues-old-values-upon-new-enques

    /*
    I like to use the Foo()/SafeFoo()/UnsafeFoo() convention:
    Foo methods call UnsafeFoo as a default.
    UnsafeFoo methods modify state freely without a lock, they should only call other unsafe methods.
    SafeFoo methods call UnsafeFoo methods inside a lock.
    Its a little verbose, but it makes obvious errors,
    like calling unsafe methods outside a lock in a method which is supposed to be thread-safe,
    more apparent.
    */
    public class CircularBuffer<T> : IEnumerable<T>
    {
        readonly int _size;
        readonly object _locker;

        int _count;
        int _head;
        int _rear;
        readonly T[] _values;

        public CircularBuffer(int max)
        {
            this._size = max;
            _locker = new object();
            _count = 0;
            _head = 0;
            _rear = 0;
            _values = new T[_size];
        }

        static int Incr(int index, int size)
        {
            return (index + 1) % size;
        }
        private void UnsafeEnsureQueueNotEmpty()
        {
            if (_count == 0)
                throw new Exception("Empty queue");
        }

        public int Size => _size;
        public object SyncRoot => _locker;

        #region Count
        public int Count => UnSafeCount;
        public int SafeCount { get { lock (_locker) { return UnSafeCount; } } }
        private int UnSafeCount => _count;
        #endregion
        
        #region Head
        public int Head => UnSafeHead;
        private int UnSafeHead => _head;
        public int SafeHead { get { lock (_locker) { return UnSafeHead; } } }
        #endregion
        
        #region Rear
        public int Rear => UnSafeRear;
        private int UnSafeRear => _rear;
        public int SafeRear { get { lock (_locker) { return UnSafeRear; } } }
        #endregion

        #region Enqueue

        public void Enqueue(T obj)
        {
            UnsafeEnqueue(obj);
        }
        public void SafeEnqueue(T obj)
        {
            lock (_locker) { UnsafeEnqueue(obj); }
        }
        public void UnsafeEnqueue(T obj)
        {
            _values[_rear] = obj;

            if (Count == Size)
                _head = Incr(_head, Size);
            _rear = Incr(_rear, Size);
            _count = Math.Min(_count + 1, Size);
        }

        #endregion

        #region Dequeue

        public T Dequeue()
        {
            return UnsafeDequeue();
        }

        public T SafeDequeue()
        {
            lock (_locker) { return UnsafeDequeue(); }
        }

        public T UnsafeDequeue()
        {
            UnsafeEnsureQueueNotEmpty();

            T res = _values[_head];
            _values[_head] = default(T);
            _head = Incr(_head, Size);
            _count--;

            return res;
        }

        #endregion

        #region PeekHead
        public T PeekHead()
        {
            return UnSafePeekHead();
        }
        public T SafePeekHead()
        {
            lock (_locker) { return UnSafePeekHead(); }
        }
        public T UnSafePeekHead()
        {
            UnsafeEnsureQueueNotEmpty();

            return _values[_head];
        }
        #endregion
        #region PeekRear
        public T PeekRear()
        {
            return UnSafePeekRear();
        }
        public T SafePeekRear()
        {
            lock (_locker) { return UnSafePeekRear(); }
        }
        public T UnSafePeekRear()
        {
            UnsafeEnsureQueueNotEmpty();

            return _values[_rear];
        }
        #endregion


        #region GetEnumerator

        public IEnumerator<T> GetEnumerator()
        {
            return UnsafeGetEnumerator();
        }

        public IEnumerator<T> SafeGetEnumerator()
        {
            lock (_locker)
            {
                var res = new List<T>(_count);
                var enumerator = UnsafeGetEnumerator();
                while (enumerator.MoveNext())
                    res.Add(enumerator.Current);
                return res.GetEnumerator();
            }
        }
        public T[] SafeToArray()
        {
            lock (_locker)
            {
                var res = new T[_count];
                var enumerator = UnsafeGetEnumerator();
                var i = 0;
                while (enumerator.MoveNext())
                {
                    res[i++] = enumerator.Current;
                }
                return res;
            }
        }
        public T[] SafeToArray1()
        {
            lock (_locker)
            {
                var res = new T[_size];
                var enumerator = UnsafeGetEnumerator();
                var i = _size - _count;
                while (enumerator.MoveNext())
                {
                    res[i++] = enumerator.Current;
                }
                return res;
            }
        }
        public T[] SafeToReverseArray()
        {
            lock (_locker)
            {
                var res = new T[_count];
                if (_count <= 0)
                    return res;
                var enumerator = UnsafeGetEnumerator();
                var i = _count - 1;
                while (enumerator.MoveNext())
                {
                    res[i--] = enumerator.Current;
                }
                return res;
            }
        }
        public IEnumerator<T> UnsafeGetEnumerator()
        {
            var index = _head;
            for (var i = 0; i < _count; i++)
            {
                yield return _values[index];
                index = Incr(index, _size);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }

}
