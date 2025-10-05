using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.EventLog.ViewModel
{
    public class EventLogView
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Result { get; set; }
        public string Subject { get; set; }
        public long Count { get; set; }
        public DateTime FirstDate { get; set; }
        public DateTime LastDate { get; set; }

        public int Days {
            get { return 1 + (LastDate.Date - FirstDate.Date).Days; }
        }
    }
}
