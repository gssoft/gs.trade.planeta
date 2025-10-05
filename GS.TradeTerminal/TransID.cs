using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace sg_TradeTerminal02
{
    public class TransID
    {
        private long _dtNumber;
        volatile private int _number;

        private readonly int _numberMax;
        private readonly string _formatString;

        private readonly object _locker; 
        
        public TransID(int maxNumber)
        {
            _numberMax = maxNumber;
            var s = _numberMax.ToString(CultureInfo.InvariantCulture);
            _formatString = s.Aggregate("", (current, t) => current + '0');
            _locker = new object();
        }
        private static long DtNowToNumber()
        {
            var dt = DateTime.Now;
            return (
                       (long)dt.Second +
                       (long)(dt.Minute) * 100 +
                       (long)(dt.Hour) * 10000 
                      // + (long)(dt.Day) * 1000000
                       // + (lomg)(dt.Month)*100000000
                   );
        }

        private void IncNumber()
        {
            var dtNowNumber = DtNowToNumber();
            //if ( _dtNumber < dtNowNumber )
            if (_dtNumber != dtNowNumber)
            {
                _number = 0;
                _dtNumber = dtNowNumber;
            }
            else
            {
                if (_number < _numberMax) _number++;
                else _number = 0;
            }
        }

        public string GetTransID()
        {
            lock (_locker)
            {
                IncNumber();
                return (_dtNumber + _number.ToString(_formatString));
            }
        }
    }
}
