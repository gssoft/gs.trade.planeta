using System;
using System.Xml.Serialization;
using GS.Elements;
using GS.Extension;
using GS.ProcessTasks;

namespace GS.EventHubs.EventHubPrTskT1
{
    
    public partial class EventHubItem<TContent> : Element1<string>, IEventHubItem<TContent> 
    {
        public override string Key => Code.TrimUpper();
        public event EventHandler<TContent> EventHandler;
        public override void Init()
        {
            SetupProcessTask();
        }
        public void FireEvent(TContent content)
        {
            try
            {
                if (IsProcessTaskInUse)
                    ProcessTask?.EnQueue(content);
                else
                    EventHandler?.Invoke(this, content);
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        public override string ToString()
        {
            return $"{GetType()} Code: {Code} Key: {Key} IsProcessTaskInUse: {IsProcessTaskInUse}";
        }
    }
}
