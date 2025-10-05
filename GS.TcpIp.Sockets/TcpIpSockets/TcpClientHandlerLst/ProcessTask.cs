using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GS.EventHubs.Interfaces;
using GS.Interfaces;
using GS.ProcessTasks;

namespace TcpIpSockets.TcpClientHandlerLst
{
    public partial class TcpClientHandler 
    {
        public bool IsProcessTaskInUse { get; set; }
        public ProcessTask<List<string>> ProcessTask { get; private set; }
         
        public void SetupProcessTask()
        {
            if (!IsProcessTaskInUse)
            {
                //Evlm2(EvlResult.WARNING, EvlSubject.INIT, ParentTypeName, TypeName,
                //MethodBase.GetCurrentMethod().Name, "ProcessTask Will NOT BE USED",
                //ToString());
                return;
            }
            ProcessTask = new ProcessTask<List<string>>();
            ProcessTask.Init(EventLog);
            ProcessTask.Parent = this;
            ProcessTask.TimeInterval = 5000;
            // ProcessTask.IdlingCycleAction = IsPingEnabled ? SendPingProcess : (Action) null;
            // ProcessTask.IsEveryItemPushProcessing = false;
            ProcessTask.IsEveryItemPushProcessing = true;
            ProcessTask.ItemProcessingAction = message =>
            {
                try
                {
                    WriteStringToClient(message);
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
