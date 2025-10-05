using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Events;
using GS.Extension;

namespace GS.EventLog
{
    public class EvliReceiver : Receiver<EventLogItem>
    {
        public EvliReceiver()
        {
            IsQueueEnabled = true;
        }

        public override string Key
        {
            get { return FullCode.HasValue() ? FullCode : GetType().FullName; }
        }
    }
}
