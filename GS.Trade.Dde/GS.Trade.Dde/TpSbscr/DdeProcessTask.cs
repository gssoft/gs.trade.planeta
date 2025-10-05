using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using DDEInfo;
using GS.Interfaces;
using GS.Interfaces.Dde;
using GS.ProcessTasks;

namespace GS.Trade.Dde.TpSbscr
{
    public partial class Dde
    {
        public bool IsProcessTaskInUse { get; set; }

        [XmlIgnore]
        public ProcessTask<DataPokeddEventArgs> ProcessTaskDataPoked { get; private set; }
        //[XmlIgnore]
        //public ProcessTask<string> ProcessTask1 { get; private set; }
        //[XmlIgnore]
        //public ProcessTask<List<string>> ProcessTask2 { get; private set; }
        private void SetupProcessTaskDataPoked()
        {
            if (!IsProcessTaskInUse)
            {
                Evlm2(EvlResult.WARNING, EvlSubject.INIT, ParentTypeName, TypeName,
                MethodBase.GetCurrentMethod()?.Name, "ProcessTask Will NOT BE USED",
                ToString());
                return;
            }
            ProcessTaskDataPoked = new ProcessTask<DataPokeddEventArgs>();
            // ProcessTask.Init(_evl);
            ProcessTaskDataPoked.Parent = this;
            ProcessTaskDataPoked.ParentObj = this;
            ProcessTaskDataPoked.TimeInterval = 1000;
            ProcessTaskDataPoked.IsEveryItemPushProcessing = true;
            ProcessTaskDataPoked.ItemProcessingAction = DataPoked2;

            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
                MethodBase.GetCurrentMethod()?.Name, "ProcessTask IS USED NOW",
                ProcessTaskDataPoked?.ToString());
        }
        //private void SetupProcessTask1()
        //{
        //    if (!IsProcessTaskInUse)
        //    {
        //        Evlm2(EvlResult.WARNING, EvlSubject.INIT, TypeName, ShortDescription,
        //        MethodBase.GetCurrentMethod()?.Name, "ProcessTask Will NOT BE USED",
        //        ToString());
        //        return;
        //    }
        //    ProcessTask1 = new ProcessTask<string>();
        //    // ProcessTask.Init(_evl);
        //    ProcessTask1.Parent = this;
        //    ProcessTask1.ParentObj = this;
        //    ProcessTask1.TimeInterval = 1000;
        //    ProcessTask1.IsEveryItemPushProcessing = true;
        //    ProcessTask1.ItemProcessingAction = SendToSubScriberItem;

        //    Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, TypeName, ShortDescription,
        //        MethodBase.GetCurrentMethod()?.Name, "ProcessTask IS USED NOW",
        //        ProcessTask1?.ToString());
        //}
        //private void SetupProcessTask2()
        //{
        //    var m = MethodBase.GetCurrentMethod()?.Name + "()";
        //    if (!IsProcessTaskInUse)
        //    {
        //        Evlm2(EvlResult.WARNING, EvlSubject.INIT, ParentAndMyTypeName, TypeName, 
        //        $"{m}", "ProcessTask Will NOT BE USED",  ToString());
        //        return;
        //    }
        //    ProcessTask2 = new ProcessTask<List<string>>();
        //    // ProcessTask.Init(_evl);
        //    ProcessTask2.Parent = this;
        //    ProcessTask2.ParentObj = this;
        //    ProcessTask2.TimeInterval = 1000;
        //    ProcessTask2.IsEveryItemPushProcessing = true;
        //    if (Mode == ChangesSendMode.Table)
        //        ProcessTask2.ItemProcessingAction = SendToSubScriberTable;
        //    else
        //        ProcessTask2.ItemProcessingAction = SendToSubScriberItemFromTable;

        //    Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentAndMyTypeName, TypeName, $"{m}",
        //             "ProcessTask IS USED NOW", ProcessTask2?.ToString());
        //}
        // Mode == Table,  Send whole Table at once
        private void SendToSubScriberTable(List<string> ls)
        {
            try
            {
                TableChangesSendAction?.Invoke(ls);
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        // Mode == Send Item, Mixed Mode
        // Get from Dde Table and push every Line from Table
        private void SendToSubScriberItemFromTable(List<string> ls)
        {
            try
            {
                foreach (var s in ls)
                    LineChangesSendAction?.Invoke(s);
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        // Mode = Line = One Line Every Dde Changes
        private void SendToSubScriberItem(string s)
        {
            try
            {
                LineChangesSendAction?.Invoke(s);
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        public void StartProcessTask()
        {
            if (!IsProcessTaskInUse)
                return;

            ProcessTaskDataPoked.Start();
            StartEventHub();
            //if(Mode == ChangesSendMode.Line)
            //    ProcessTask1?.Start();
            //else
            //    ProcessTask2?.Start();
        }
        public void StopProcessTask()
        {
            if (!IsProcessTaskInUse)
                return;

            ProcessTaskDataPoked.Stop();
            StopEventHub();

            //if (Mode == ChangesSendMode.Line)
            //    ProcessTask1?.Stop();
            //else
            //    ProcessTask2?.Stop();

            // Thread.Sleep(1000);
        }
    }
}
