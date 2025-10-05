using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Interfaces;
using GS.ProcessTasks;

namespace GS.Trade.Windows
{
    public partial class EventLogWindow3
    {
        public bool IsProcessTaskInUse { get; set; }
        private void SetupProcessTask()
        {
            if (!IsProcessTaskInUse)
            {
                EventLog.Evlm2(EvlResult.WARNING, EvlSubject.INIT, GetType().Name, Title,
                MethodBase.GetCurrentMethod()?.Name, "ProcessTask Will NOT BE USED",
                ToString());
                return;
            }
            ProcessTask = new ProcessTask<IEventLogItem>();
            ProcessTask.Init(EventLog);
            // ProcessTask.Parent = this;
            ProcessTask.ParentObj = this;
            ProcessTask.TimeInterval = 1000;
            ProcessTask.IsEveryItemPushProcessing = false;
            ProcessTask.ItemsProcessingAction = InsertItemsIntoObserveCollection;

            EventLog.Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, GetType().Name, Title,
                MethodBase.GetCurrentMethod()?.Name, "ProcessTask IS USED NOW",
                ProcessTask?.ToString());
        }
        [XmlIgnore]
        public GS.ProcessTasks.ProcessTask<IEventLogItem> ProcessTask { get; private set; }
        public void Start()
        {
            if (IsProcessTaskInUse)
                ProcessTask?.Start();
        }
        public void Stop()
        {
            if (IsProcessTaskInUse)
                ProcessTask?.Stop();
        }
    }
}
