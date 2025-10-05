using System.Reflection;
using System.Xml.Serialization;
using GS.Interfaces;
using GS.ProcessTasks;

namespace GS.EventHubs.EventHubT2
{
    public partial class EventHub<TEventArgs, TContent>
    {
        [XmlIgnore]
        public GS.ProcessTasks.ProcessTask<TEventArgs> ProcessTask { get; private set; }

        // Setup in .xml file
        public bool IsProcessTaskInUse { get; set; }
        private void SetupProcessTask()
        {
            if (!IsProcessTaskInUse)
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                MethodBase.GetCurrentMethod()?.Name, "ProcessTask Will NOT BE USED",
                ToString());
                return;
            }
            ProcessTask = new ProcessTask<TEventArgs>();
            ProcessTask.Init();
            ProcessTask.TimeInterval = 1000;
            ProcessTask.SecondsWaitingCompleting = 15;
            ProcessTask.Parent = this;
            ProcessTask.IsEveryItemPushProcessing = true;
            ProcessTask.ItemProcessingAction = FireEventOperation;

            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                MethodBase.GetCurrentMethod()?.Name, "ProcessTask IS USED NOW",
                ProcessTask?.ToString());
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
    }
}
