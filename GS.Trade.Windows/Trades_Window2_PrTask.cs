using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Util;
using System.Windows;
using System.Xml.Serialization;
using GS.Events;
using GS.Interfaces;
using GS.ProcessTasks;
using GS.Trade.Trades.UI.Model;
using GS.Trade.TradeTerminals64;

namespace GS.Trade.Windows
{
    public partial class TradesWindow2 : Window
    {
        [XmlIgnore]
        public GS.ProcessTasks.ProcessTask<IEventArgs> ProcessTask { get; private set; }
        public bool IsProcessTaskInUse { get; set; }
        private void SetupProcessTask()
        {
            if (!IsProcessTaskInUse)
            {
                EventLog?.Evlm2(EvlResult.WARNING, EvlSubject.INIT, GetType().Name, Title,
                MethodBase.GetCurrentMethod()?.Name, "ProcessTask Will NOT BE USED",
                ToString());
                return;
            }
            ProcessTask = new ProcessTask<IEventArgs>();
            ProcessTask.Init(_tx?.EventLog);
            // ProcessTask.Parent = this;
            ProcessTask.ParentObj = this;
            ProcessTask.TimeInterval = 1000;
            ProcessTask.SecondsWaitingCompleting = 15;
            ProcessTask.IsEveryItemPushProcessing = false;
            ProcessTask.ItemsProcessingAction = InsertItemsIntoObserveCollection;

            EventLog?.Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, GetType().Name, Title,
                MethodBase.GetCurrentMethod()?.Name, "ProcessTask IS USED NOW",
                ProcessTask?.ToString());
        }

        // private bool IsSyncContextProcess = true;
        
        private void InsertItemsIntoObserveCollection(IEnumerable<IEventArgs> args)
        {
            //if (IsSyncContextProcess)
            //{
            //    var uiContext = SynchronizationContext.Current;
            //    uiContext?.Send(x => ProcessTradesInSyncContext(args), null);
            //    return;
            //}

            Dispatcher.BeginInvoke((ThreadStart)(() =>
            {
                foreach (var arg in args)
                  ProcessTrade(arg);

                Title = "Trades ( " + Items.Count + " )";
            }
           ));
        }

        private void ProcessTradesInSyncContext(IEnumerable<IEventArgs> args)
        {
            foreach (var arg in args)
                ProcessTrade(arg);

            Title = "Trades ( " + Items.Count + " )";
        }

        private void ProcessTrade(IEventArgs arg)
        {
            if (!(arg?.Object is ITrade3 t))
                return;

            switch (arg.OperationKey)
            {
                case "UI.TRADES.TRADE.ADD":
                        Items.Add(t);
                        this.Title = "Trades ( " + Items.Count + " )";
                    break;
                case "UI.TRADES.TRADE.DELETE":
                        Items.Remove(t);
                        this.Title = "Trades ( " + Items.Count + " )";
                    break;
                case "UI.TRADES.TRADE.UPDATE":
                        Items.Update(t);
                        this.Title = "Trades ( " + Items.Count + " )";
                    break;
            }
        }
     
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
