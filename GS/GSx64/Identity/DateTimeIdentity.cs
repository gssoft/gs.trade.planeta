using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;

namespace GS.Identity
{
    public class DateTimeIdentity
    {
        public DateTimeIdentity()
        {
            _locker = new object();
        }

        private readonly object _locker ;
        private long _id;

        public long GetCurrent()
        {
            lock (_locker)
            {
                return _id;
            }
        }

        public long GetNext()
        {
            return Next();
        }
        private long Next()
        {
            lock (_locker)
            {
                var dt = DateTime.Now;
                _id = (long) dt.Second +
                      (long) dt.Minute*100 +
                      (long) dt.Hour*10000 +
                      (long) dt.Day*1000000 +
                      (long) dt.Month*100000000 +
                      (long) (dt.Year >= 2000 ? dt.Year - 2000 : dt.Year - 1900) * 10000000000;
                        
                return _id;
            }
        }
    }
    public class DateTimeIdentity2
    {
        public DateTimeIdentity2()
        {
            _locker = new object();
        }

        private readonly object _locker;
        private long _id;

        public long Current()
        {
            lock (_locker)
            {
                return _id;
            }
        }
        public long GetNext()
        {
            return Next();
        }
        private long Next()
        {
            lock (_locker)
            {
                return _id = Int64.Parse(string.Format("{0:yyMMddhhmmss}",DateTime.Now));
            }
        }
    }
    public class DateNumberIdentity
    {
        public DateNumberIdentity()
        {
            _locker = new object();
        }

        private readonly object _locker;
        private uint _number;
        private long _id;

        public long GetCurrent()
        {
            lock (_locker)
            {
                return _id;
            }
        }
        public long GetNext()
        {
            return Next();
        }
        private long Next()
        {
            lock (_locker)
            {
                var dt = DateTime.Now;
                _number++;
                var n = 
                      dt.Day +
                      (long)dt.Month * 100 +
                      (long)(dt.Year >= 2000 ? dt.Year - 2000 : dt.Year - 1900) * 10000;

                return _id = (n << 32) + _number; 
            }
        }
    }
    public class DateTimeNumberIdentity
    {
        public DateTimeNumberIdentity(uint maxInt)
        {
            _locker = new object();
            _maxNumber = maxInt;
        }

        private readonly object _locker;
        private ulong _id;
        //private int _number;
        private uint _number;
        private readonly uint _maxNumber;
        private ulong _dateTimeNumber;

        public ulong Current()
        {
            lock (_locker)
            {
                return _id;
            }
        }
        public ulong Next()
        {
            lock (_locker)
            {
                var dts = ulong.Parse(string.Format("{0:yyMMddhhmmss}", DateTime.Now));
                if (_dateTimeNumber == dts)
                {
                    _number = _number < _maxNumber-1 ? ++_number : 0;
                    _id = _dateTimeNumber * _maxNumber + _number;
                }
                else
                {
                    _dateTimeNumber = dts;
                    _number = 0;
                    _id = _dateTimeNumber * _maxNumber;
                }
            }
            return _id;
        }
    }
    public class DateTimeSecondsNumberIdentity
    {
        public DateTimeSecondsNumberIdentity()
        {
            _locker = new object();
            _ndt = DateTimeToSeconds() << 32;
        }

        public void Clear()
        {
            _ndt = DateTimeToSeconds() << 32;
            _number = 0;
        }

        private readonly object _locker;
        private uint _number;
        private ulong _ndt;

        public ulong GetCurrent()
        {
            lock (_locker)
            {
                return _ndt  + _number;
            }
            
        }
        public ulong GetNext()
        {
            return Next();
        }
        private ulong Next()
        {
            lock (_locker)
            {
                return _ndt  + ++_number;
            }
        }

        private static uint DateTimeToSeconds()
        {
            var dt = DateTime.Now;
            int ny = (dt.Year/100)*100;
            var dt0 = new DateTime(ny, 1, 1, 0, 0, 0);

            return (uint)((dt - dt0).TotalSeconds);

            //return      (uint)dt.Second +
            //            (uint)dt.Minute * 60 +
            //            (uint)dt.Hour * 3600 +
            //            (uint)dt.DayOfYear * 3600 * 24 +
            //            (uint)(dt.Year >= 2000 ? dt.Year - 2000 : dt.Year - 1900) * 3600 * 24 * 366;
        }

    }
}
