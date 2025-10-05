using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using GS.Events;
using GS.Interfaces;
using GS.ProcessTasks;
using GS.Trade.Trades.UI.Model;
using GS.Trade.TradeTerminals64;

namespace GS.Trade.Windows
{
    public partial class TransactionsWindow : Window
    {
        public bool IsProcessTaskInUse { get; set; }
        private void SetupProcessTask()
        {
            if (!IsProcessTaskInUse)
            {
                _evl.Evlm2(EvlResult.WARNING, EvlSubject.INIT, GetType().Name, Title,
                MethodBase.GetCurrentMethod()?.Name, "ProcessTask Will NOT BE USED",
                ToString());
                return;
            }
            ProcessTask = new ProcessTask<IQuikTransaction>();
            ProcessTask.Init(_evl);
            // ProcessTask.Parent = this;
            ProcessTask.ParentObj = this;
            ProcessTask.TimeInterval = 1000;
            ProcessTask.SecondsWaitingCompleting = 15;
            ProcessTask.IsEveryItemPushProcessing = false;
            ProcessTask.ItemsProcessingAction = InsertItemsIntoObserveCollection;

            _evl.Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, GetType().Name, Title,
                MethodBase.GetCurrentMethod()?.Name, "ProcessTask IS USED NOW",
                ProcessTask?.ToString());
        }

        private void InsertItemsIntoObserveCollection(IEnumerable<IQuikTransaction> transactions)
        {
            Dispatcher?.BeginInvoke((ThreadStart)(() =>
            {
                foreach (var t in transactions)
                  ProcessTransaction(t);

                Title = "Transactions ( " + Transactions.Count + " )";
            }
           ));
        }
        private void ProcessTransaction(IQuikTransaction t)
        {
            if (t == null)
                return;

            var ot  = Transactions.Items.FirstOrDefault(to => to.Key == t.Key);
            if (ot != null)
                Transactions.Remove(ot);
            Transactions.Add(t);
        }

        [XmlIgnore]
        public GS.ProcessTasks.ProcessTask<IQuikTransaction> ProcessTask { get; private set; }
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
