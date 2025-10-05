using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace caTestQ
{
    public class SeriesDef
    {
        public string Ticker;
        public string TimeInt;
        public DateTime FirstDateTime;

        public string Key{ get {return Ticker + TimeInt;} }

    }
}
