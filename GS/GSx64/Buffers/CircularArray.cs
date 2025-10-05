using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GS.Buffers
{
    public class CircularArray1<T>
    {
        public int First { get; set; }
        public T FirstValue => Count >= 0 ? array[First] : default(T);
        public int Head { get; set; }
        public int Count { get; set; }
        public int Capacity { get; set; }

        private T[] array;

        public CircularArray1(int capacity)
        {
            array = new T[capacity];
            Capacity = capacity;
        }
        public void Add(T v)
        {
            if (Count == 0)
            {
                First = 0;
                // Head = 1;
            }
            else
            {
                First = ++First % Capacity;
            }
            array[First] = v;
            ++Count;
        }
        public T[] ToArray()
        {
            var ar = new T[Count];
            for (int i = 0; i < Count; ++i)
            {
                ar[i] = array[(First + i) % Capacity];
            }
            return ar;
        }
        public T[] ToArray1()
        {
            var ar = new T[Count];
            for (int i = 0; i < Count; ++i)
            {
                ar[i] = array[(First + i) % Capacity];
            }
            return ar;
        }

        public void Clear()
        {
            var v = default(T);
            for (int i = 0; i < Count; i++)
            {
                array[i] = v;
            }
            First = 0;
            Count = 0;
        }
    }

    public class CircularArray<T> : IEnumerable<T>
    {
        public T Current => IsEmpty ? default(T) : _array[_position];
        public T Last => Current;
        public T First => IsEmpty ? default(T) : _array[(_position + Count - 1) % Size];
        public int Position => _position;
        public bool IsEmpty => _position < 0;
        public int Count { get; private set; }
        public int Length => Count;
        public int Size { get; }

        private readonly T[] _array;
        private int _position;

        public CircularArray(int size)
        {
            _array = new T[size];
            Size = size;
            _position = -1;
        }
        public void Add(T v)
        {
            if (Count == 0)
            {
                _position = Size - 1;
            }
            else
            {
                if (--_position < 0) _position = Size - 1;
            }
            _array[_position] = v;
            if (++Count > Size) Count = Size;
        }

        public T[] ToArray()
        {
            if (IsEmpty) return new T[0];
            var ar = new T[Count];
            for (var i = 0; i < Count; ++i)
            {
                ar[i] = _array[(_position + i) % Size];
            }
            return ar;
        }
        public T[] ToArrayReverse()
        {
            if (IsEmpty) return new T[0];
            var ar = new T[Count];
            for (var i = 0; i < Count; ++i)
            {
                ar[i] = _array[(_position + Count - 1 - i) % Size];
            }
            return ar;
        }
        public T[] ToArrayRawValues()
        {
            var ar = new T[Size];
            for (var i = 0; i < Size; ++i)
            {
                ar[i] = _array[i];
            }
            return ar;
        }
        public T[] ToArrayAll()
        {
            if (IsEmpty) return new T[0];
            var ar = new T[Size];
            for (var i = 0; i < Size; ++i)
            {
                ar[i] = _array[(_position + i) % Size];
            }
            return ar;
        }
        public T[] ToArrayReverseAll()
        {
            if (IsEmpty) return new T[0];
            var ar = new T[Size];
            for (var i = 0; i < Size; ++i)
            {
                ar[i] = _array[(_position + Count - 1 - i) % Size];
            }
            return ar;
        }
        public void Reset()
        {
            _position = -1;
            Count = 0;
        }
        public void Populate(T value)
        {
            for (var i = 0; i < Size; ++i)
                _array[i] = value;
            Count = Size;
            _position = 0;
        }
        //public void Populate()
        //{
        //    for (var i = 0; i < Size; ++i)
        //        _array[i] = default(T);
        //    Count = Size;
        //    _position = 0;
        //}
        public void Clear()
        {
            var v = default(T);
            for (var i = 0; i < Size; i++)
                _array[i] = v;
            Reset();
        }
        public void ClearByNull()
        {
            //array = null;
            //GC.Collect();
            //array = new T[Size];
        }
        public T this[int i]
        {
            get
            {
                try
                {
                    if (i < Count)
                        return _array[(i + _position) % Size];
                    throw new IndexOutOfRangeException(
                        $"{this.GetType().Name} {MethodBase.GetCurrentMethod().Name}: Index out of range. Count:{Count} Idx:{i}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    // throw;
                }
                return default(T);
            }
        }

        // set { _array[i] = value; }
        public IEnumerator<T> GetEnumerator()
        {
            return new CircularArrayEnumerator<T>(_array, _position, Count, Size);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class CircularArrayEnumerator<T> : IEnumerator<T>
    {
        public T[] array;
        private int _position;
        private int _length;
        private int _size;

        public CircularArrayEnumerator(T[] list, int position, int count, int size)
        {
            array = list;
            _size = size;
            _position = --position % _size;
            _length = count;
            _size = size;
        }

        public bool MoveNext()
        {
            //_position = _position++ % _length;
            //return _position < array.Length;
            // _array[(Position + i) % Size];
            _position = ++_position % _size;
            var b = --_length >= 0;
            return b;
        }
        public void Reset()
        {
            // _position = -1;
        }
        object IEnumerator.Current => Current;

        public T Current
        {
            get
            {
                try
                {
                    return array[_position];
                }
                catch (IndexOutOfRangeException e)
                {
                    // throw new InvalidOperationException();
                    Console.WriteLine($"{GetType().Name} {MethodBase.GetCurrentMethod().Name}: Index out of range. Size:{_size} Position:{_position}");
                    return default(T);
                }
            }
        }
        public void Dispose()
        {
            // throw new NotImplementedException();
        }
    }


}
