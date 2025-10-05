using System.Reflection;
using System.Xml.Serialization;
using GS.Interfaces;
using GS.ProcessTasks;

namespace GS.EventHubs.EventHubPrTskT1
{
    public partial class EventHubItem<TContent>
    {
        [XmlIgnore]
        public ProcessTask<TContent> ProcessTask { get; private set; }
        // Setup in .xml file
        public bool IsProcessTaskInUse { get; set; }
        private void SetupProcessTask()
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            if (!IsProcessTaskInUse)
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                            m, "ProcessTask Will NOT BE USED", ToString());
                return;
            }
            ProcessTask = new ProcessTask<TContent>();
            ProcessTask.Init();
            ProcessTask.TimeInterval = 1000;
            ProcessTask.SecondsWaitingCompleting = 15;
            ProcessTask.Parent = this;
            ProcessTask.IsEveryItemPushProcessing = true;
            ProcessTask.ItemProcessingAction = FireEventOper;

            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                m, "ProcessTask IS USED NOW", ProcessTask?.ToString());
        }
        public void Start()
        {
            if(IsProcessTaskInUse)
                ProcessTask?.Start();
        }
        public void Stop()
        {
            if (IsProcessTaskInUse)
                ProcessTask?.Stop();
        }
        public void FireEventOper(TContent content)
        {
            EventHandler?.Invoke(this, content);
        }
    }
}
