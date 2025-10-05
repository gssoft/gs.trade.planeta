// #define StringQuotes
 #define ListOfStringQuotes

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

namespace GS.Trade.Data
{

    public partial class Tickers
    {
        public bool IsProcessTaskInUse { get; set; }
#if StringQuotes
        [XmlIgnore]
        public GS.ProcessTasks.ProcessTask<string> ProcessTask { get; private set; }
#elif ListOfStringQuotes
        [XmlIgnore]
        public GS.ProcessTasks.ProcessTask<List<string>> ProcessTask { get; private set; }
#endif
        private void SetupProcessTask()
        {
            if (!IsProcessTaskInUse)
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ParentName, Name,
                MethodBase.GetCurrentMethod().Name, "ProcessTask Will NOT BE USED",
                ToString());
                return;
            }
#if StringQuotes

            ProcessTask = new ProcessTask<string>();
            ProcessTask.Init();
            ProcessTask.TimeInterval = 1000;
            ProcessTask.Parent = this;
            ProcessTask.IsEveryItemPushProcessing = false;
            ProcessTask.ItemsProcessingAction = QuoteStringsProcessing;

#elif ListOfStringQuotes

            ProcessTask = new ProcessTask<List<string>>();
            ProcessTask.Init();
            ProcessTask.TimeInterval = 1000;
            ProcessTask.Parent = this;
            ProcessTask.IsEveryItemPushProcessing = false;
            ProcessTask.ItemsProcessingAction = QuoteListsOfStringsProcessing;

#endif
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentName, Name,
                MethodBase.GetCurrentMethod().Name, "ProcessTask IS USED NOW",
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
