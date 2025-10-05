using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GS.Interfaces;
using GS.ProcessTasks;

namespace TcpIpSockets.TcpClientHandler03T
{
    public partial class TcpClientHandler<TMessage>
    {
        public bool IsProcessTaskInUse { get; set; }
        protected ProcessTask<object> ProcessTask;
         
        public void SetupProcessTask()
        {
            if (!IsProcessTaskInUse)
            {
                //Evlm2(EvlResult.WARNING, EvlSubject.INIT, ParentTypeName, TypeName,
                //MethodBase.GetCurrentMethod().Name, "ProcessTask Will NOT BE USED",
                //ToString());
                return;
            }
            ProcessTask = new ProcessTask<object>();
            ProcessTask.Init(EventLog);
            ProcessTask.Parent = this;
            ProcessTask.TimeInterval = 5000;
            ProcessTask.IdlingCycleAction = IsPingEnabled ? SendPingProcess : (Action) null;
            // ProcessTask.IsEveryItemPushProcessing = false;
            ProcessTask.IsEveryItemPushProcessing = true;
            ProcessTask.ItemProcessingAction = str =>
            {
                try
                {
                    WriteStringToClient(str);
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
    }
}
