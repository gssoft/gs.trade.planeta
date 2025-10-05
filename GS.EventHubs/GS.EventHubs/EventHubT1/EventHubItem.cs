using System;
using GS.Extension;

namespace GS.EventHubs.EventHubT1
{
    
    public class EventHubItem <TContent>  :  IEventHubItem<TContent>
    {
        public string Key => Code.TrimUpper();
        public string Code { get; set; }

        public event EventHandler<TContent> EventHandler;
        public void FireEvent(TContent content)
        {
            EventHandler?.Invoke(this, content);
        }
        public override string ToString()
        {
            return $"{GetType()} Code: {Code} Key: {Key}";
        }

        //protected virtual void OnEventHandler(TContent e)
        //{
        //    EventHandler?.Invoke(this, e);
        //}
    }
}
