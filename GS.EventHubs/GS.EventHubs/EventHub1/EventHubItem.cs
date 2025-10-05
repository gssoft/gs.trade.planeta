using System;
using GS.Containers5;
using GS.Events;
using GS.Extension;

namespace GS.EventHubs.EventHub1
{
    public class EventHubItem :  IEventHubItem
    {
        public string Key => Code.TrimUpper();
        public string Code { get; set; }
        public event EventHandler<IEventArgs> EventHandler;
        public void FireEvent(object sender, IEventArgs eventArgs)
        {
            EventHandler?.Invoke(sender, eventArgs);            
        }
        public override string ToString()
        {
            return $"{GetType()} Code: {Code} Key: {Key}";
        }
    }
}
