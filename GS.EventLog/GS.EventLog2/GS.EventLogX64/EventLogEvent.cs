using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Interfaces;

namespace GS.EventLog
{
    public class EventLogEvent : EventArgs, IEventLogEvent
    {
        public string Operation { get; set; }
        public IEventLogItem EventLogItem { get; set; }
    }
}
