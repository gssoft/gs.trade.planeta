using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Events;
using GS.Extension;

namespace GS.Elements
{
    public class StrReceiver<T> : Element33<string, T, IEventArgs>
    {
        public StrReceiver()
        {
            IsQueueEnabled = true;
        }
        public override string Key => Code.HasValue() ? Code : GetType().FullName;
    }

    public class Receiver<T> : Element33<string, T, IEventArgs>
    {
        public Receiver()
        {
            IsQueueEnabled = true;
        }
        public override string Key => Code.HasValue() ? Code : GetType().FullName;
    }
}
