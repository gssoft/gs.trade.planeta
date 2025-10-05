using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;

namespace GS.Trade.TradeTerminals64
{
    public class OrderLockedCount
    {
        public int MaxCount { get; set; }

        private volatile bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        private volatile int _count;
        public int Count
        {
            get { return _count; }
            //private set { _count = value; }
        }

        private readonly Object _locker;
        

        public  OrderLockedCount()
        {
            _locker = new Object();
            _isEnabled = true;
        }

        public void Clear()
        {
            lock (_locker)
            {
                _count = 0;
            }
        }

        public bool Inc()
        {
            lock(_locker)
            {
                if (_count < MaxCount)
                {
                    ++_count;
                    return true;
                }
                return false;
            }
        }

        public bool IsValid { get { lock (_locker) { return _count <= MaxCount; } } }
    }

    public class OrderLockedCount2
    {
        public int MaxCount { get; set; }

        private volatile bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        private volatile int _count;
        public int Count
        {
            get { return _count; }
            //private set { _count = value; }
        }
        private volatile int _second;
        public int Second {
            get { return _second; }
            private set { _second = value; }
        }

        private readonly object _locker;

        public OrderLockedCount2()
        {
            _locker = new Object();
            _isEnabled = true;
            SetNewTime();
        }

        public void NewTime(DateTime dt)
        {
                var s = dt.Second;
                if (s == Second)
                    return;
                Clear();
                Second = s;
            
        }
        private void SetNewTime()
        {
            var s = DateTime.Now.Second;
            if (s == Second)
                return;
            _count = 0;
            Second = s;
           
        }
        public void Clear()
        {
            lock (_locker)
            {
                _count = 0;
            }
        }
        public bool Inc()
        {
            lock (_locker)
            {
                if (_count >= MaxCount)
                    return false;
                ++_count;
                return true;
            }
        }
        public bool Inc2()
        {
            lock (_locker)
            {
                SetNewTime();
                if (_count >= MaxCount)
                    return false;
                ++_count;
                return true;
            }
        }

        public bool IsValid { get { lock (_locker) { return _count <= MaxCount; } } }
    }

    public class OrderLockedCount3
    {
        public int MaxCount { get; set; }

        private volatile bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }
        private volatile int _count;
        public int Count => _count;
        private volatile int _second;
        public int Second
        {
            get { return _second; }
            private set { _second = value; }
        }

        private readonly object _locker;

        public OrderLockedCount3()
        {
            _locker = new Object();
            _isEnabled = true;
            SetNewTime();
        }

        public void NewTime(DateTime dt)
        {
            var s = dt.TimeToInt();
            if (s == Second)
                return;

            Clear();
            Second = s;

        }
        private void SetNewTime()
        {
            var s = DateTime.Now.Second;
            if (s == Second)
                return;
            _count = 0;
            Second = s;

        }
        public void Clear()
        {
            lock (_locker)
            {
                _count = 0;
            }
        }
        public bool Inc()
        {
            lock (_locker)
            {
                if (_count >= MaxCount)
                    return false;
                ++_count;
                return true;
            }
        }
        public bool Inc2()
        {
            lock (_locker)
            {
                SetNewTime();
                if (_count >= MaxCount)
                    return false;
                ++_count;
                return true;
            }
        }

        public bool IsValid { get { lock (_locker) { return _count <= MaxCount; } } }
    }
}
