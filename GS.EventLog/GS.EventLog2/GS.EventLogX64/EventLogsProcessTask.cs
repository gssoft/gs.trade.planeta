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

namespace GS.EventLog
{
    public partial class EventLogs
    {
        // Setup in .xml file
        public bool IsProcessTaskInUse { get; set; }
        private void SetupProcessTask()
        {
            if (!IsProcessTaskInUse)
            {
                //Evlm2(EvlResult.WARNING, EvlSubject.INIT, ParentTypeName, TypeName,
                //MethodBase.GetCurrentMethod().Name, "ProcessTask Will NOT BE USED",
                //ToString());
                return;
            }
            ProcessTask = new ProcessTask<IEventLogItem>();
            ProcessTask.Init(this);
            ProcessTask.Parent = this;
            ProcessTask.TimeInterval = 1000;
            //ProcessTask.IsEveryItemPushProcessing = false;
            ProcessTask.IsEveryItemPushProcessing = true;
            ProcessTask.ItemProcessingAction = evli =>
            {
                try
                {
                    AddItem(evli);
                }
                catch (Exception e)
                {
                    SendException(e);
                    //Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING,
                    //    GetType().Name, "ProcessTask", "Exception", e.Message);
                }
            };
            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
                MethodBase.GetCurrentMethod().Name, "ProcessTask IS USED NOW",
                ProcessTask?.ToString());
        }
        [XmlIgnore]
        public GS.ProcessTasks.ProcessTask<IEventLogItem> ProcessTask { get; private set; }
        public void Start()
        {
            if(!IsProcessTaskInUse)
                return;
            ProcessTask?.Start();
            System.Threading.Thread.Sleep(1000);
        }
        public void Stop()
        {
            if (!IsProcessTaskInUse) return;

            ProcessTask?.Stop();
            System.Threading.Thread.Sleep(1000);
        }
    }
}
