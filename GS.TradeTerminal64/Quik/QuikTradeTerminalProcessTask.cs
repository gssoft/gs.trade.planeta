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

namespace GS.Trade.TradeTerminals64.Quik
{
    public sealed partial class QuikTradeTerminal
    {
        public bool IsProcessTaskInUse { get; set; }
        [XmlIgnore]
        public ProcessTask<IEventArgs1> ProcessTask { get; private set; }
        private void SetupProcessTask()
        {
            if (!IsProcessTaskInUse)
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ParentName, Name,
                MethodBase.GetCurrentMethod()?.Name, "ProcessTask Will NOT BE USED",
                ToString());
                return;
            }
            ProcessTask = new ProcessTask<IEventArgs1>
            {
                Parent = this,
                TimeInterval = 1000,
                // SleepInterval = 500,
                IsEveryItemPushProcessing = true,
                ItemProcessingAction = ItemProcessingAction,
                IdlingCycleAction = ProcessTaskIdleAction
            };
            ProcessTask.Init();

            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentName, Name,
                 MethodBase.GetCurrentMethod()?.Name, "ProcessTask IS USED NOW",
                 ProcessTask?.ToString());
        }
        public void Start()
        {
            SetupProcessTask();

            if(IsProcessTaskInUse)
                ProcessTask?.Start();
        }
        public void Stop()
        {
            if (IsProcessTaskInUse)
                ProcessTask?.Stop();
        }
        // ************************************************
        // ProcessTask Actions
        private void ItemProcessingAction(IEventArgs1 args)
        {
            try
            {
                args.ProcessingAction(args);
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        // AdditionalWorks in IdlingCycleAction
        private void ProcessTaskIdleAction()
        {
            TradeResolveProcess2();
            OrderResolveProcess();
        }
    }
}
