using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GS.Interfaces;
using GS.ProcessTasks;

namespace GS.Elements
{
    public abstract class ElementPrTsk01<TInputPrTask> : Element1<string>
    {
        public abstract void SetupProcessTask(); 
        public bool IsProcessTaskInUse { get; set; }
        protected ProcessTask<TInputPrTask> ProcessTask; 

        public override void Init()
        {
            base.Init();
            SetupProcessTask();
        }
        public virtual void EnQueue(TInputPrTask item)
        {
            ProcessTask?.EnQueue(item);
        }

        //private void SetupProcessTask()
        //{
        //    if (!IsProcessTaskInUse)
        //    {
        //        //Evlm2(EvlResult.WARNING, EvlSubject.INIT, ParentTypeName, TypeName,
        //        //MethodBase.GetCurrentMethod().Name, "ProcessTask Will NOT BE USED",
        //        //ToString());
        //        return;
        //    }
        //    ProcessTask = new ProcessTask<TInputPrTask>();
        //    ProcessTask.Init(EventLog);
        //    ProcessTask.Parent = this;
        //    ProcessTask.TimeInterval = 5000;
        //    //ProcessTask.IsEveryItemPushProcessing = false;
        //    ProcessTask.IsEveryItemPushProcessing = true;
        //    ProcessTask.ItemProcessingAction = evli =>
        //    {
        //        try
        //        {
        //            AddItem(evli);
        //        }
        //        catch (Exception e)
        //        {
        //            SendException(e);
        //            //Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING,
        //            //    GetType().Name, "ProcessTask", "Exception", e.Message);
        //        }
        //    };
        //    Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
        //        MethodBase.GetCurrentMethod().Name, "ProcessTask IS USED NOW",
        //        ProcessTask?.ToString());
        //}

        public override string Key => Code;
    }

    public class ElementPrTsk02 : ElementPrTsk01<string>
    {
        public override void SetupProcessTask()
        {
            throw new NotImplementedException();
        }
    }
}
