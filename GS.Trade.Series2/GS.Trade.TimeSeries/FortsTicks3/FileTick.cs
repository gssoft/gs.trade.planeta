using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.TimeSeries.FortsTicks3
{
    public class FileTick
    {
        public long TradeID { get; set; }
        public string Code { get; set; }
        public string Contract { get; set; }

        public double Price { get; set; }
        public double Amount { get; set; }
        public DateTime DateTime { get; set; }
    }
}
