using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Events;
using GS.Interfaces;
using GS.ProcessTasks;

namespace GS.Events
{
    public partial class EventHub
    {
        // Setup in .xml file
        public bool IsProcessTaskInUse { get; set; }
        private void SetupProcessTask()
        {
            if (!IsProcessTaskInUse)
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ParentName, Name,
                MethodBase.GetCurrentMethod().Name, "ProcessTask Will NOT BE USED",
                ToString());
                return;
            }
            ProcessTask = new ProcessTask<IEventArgs>();
            ProcessTask.Init();
            ProcessTask.TimeInterval = 1000;
            ProcessTask.Parent = this;
            ProcessTask.IsEveryItemPushProcessing = true;
            ProcessTask.ItemProcessingAction = FireEventOperation;

            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentName, Name,
                MethodBase.GetCurrentMethod().Name, "ProcessTask IS USED NOW",
                ProcessTask?.ToString());
        }
        [XmlIgnore]
        public GS.ProcessTasks.ProcessTask<IEventArgs> ProcessTask { get; private set; }
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
