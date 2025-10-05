using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.TimeSeries.FortsTicks2
{
    public class FileSeries
    {
        public string Contract { get; set; }
        public string File { get; set; }
        public long Count { get; set; }
        public DateTime FirstDT { get; set; }
        public DateTime LastDT { get; set; }
        public DateTime Date => LastDT.Date;
    }
}
