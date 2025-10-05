using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Events;
using GS.Interfaces;
using GS.ProcessTasks;
using GS.Trade.Trades.UI.Model;

namespace GS.Trade.Windows
{
    public partial class PositionTotalsWindow4
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
            ProcessTask = new ProcessTask<IEventArgs>();
            ProcessTask.Init(_evl);
            // ProcessTask.Parent = this;
            ProcessTask.ParentObj = this;
            ProcessTask.TimeInterval = 1000;
            ProcessTask.IsEveryItemPushProcessing = false;
            ProcessTask.ItemsProcessingAction = InsertItemsIntoObserveCollection;
            ProcessTask.IdlingCycleAction = CreateStat;
            // ProcessTask.IdlingCycleAction = TitleUpdate;

            _evl.Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, GetType().Name, Title,
                MethodBase.GetCurrentMethod()?.Name, "ProcessTask IS USED NOW",
                ProcessTask?.ToString());
        }
        private void InsertItemsIntoObserveCollection(IEnumerable<IEventArgs> args)
        {
            Dispatcher.BeginInvoke((ThreadStart)(() =>
            {
                foreach (var arg in args)
                  ProcessPosition(arg);

                _isNeedRefreshStat = true;
            }
           ));
        }
        private void ProcessPosition(IEventArgs arg)
        {
            var ip = arg?.Object as IPosition2;
            if (ip == null)
                return;
            PositionTotalNpc2 p;
            switch (arg.OperationKey)
            {
                case "UI.POSITIONS.TOTAL.INSERT":
                        p = new PositionTotalNpc2(ip);
                        PositionCollection.Insert(0, p);
                        
                        this.Title = "Totals ( " + PositionCollection.Count + " )";
                    break;
                case "UI.POSITIONS.TOTAL.ADD":
                case "UI.POSITIONS.TOTAL.ADD.TOEND":
                        p = new PositionTotalNpc2(ip);
                        PositionCollection.Add(p);
                        
                        this.Title = "Totals ( " + PositionCollection.Count + " )";
                    break;
                case "UI.POSITIONS.TOTAL.ADDNEW":
                        p = PositionCollection.FirstOrDefault(po => po.Key == ip.Key);
                        if (p == null)
                            PositionCollection.Add(new PositionTotalNpc2(ip));
                        
                        this.Title = "Totals ( " + PositionCollection.Count + " )";
                    break;
                case "UI.POSITIONS.TOTAL.DELETE":
                        p = PositionCollection.FirstOrDefault(po => po.Key == ip.Key);
                        if (p != null)
                            PositionCollection.Remove(p);
                        this.Title = "Totals ( " + PositionCollection.Count + " )";
                    break;
                case "UI.POSITIONS.TOTAL.UPDATE":
                        p = PositionCollection.FirstOrDefault(po => po.Key == ip.Key);
                        p?.Update(ip);
                    break;
                case "UI.POSITIONS.TOTAL.MAXMINPROFIT.UPDATE":
                    p = PositionCollection.FirstOrDefault(po => po.Key == ip.Key);
                    if (p == null) break;
                    // p?.Update(ip);
                    p.TotalDailyMaxProfit = ip.TotalDailyMaxProfit;
                    p.TotalDailyMaxLoss = ip.TotalDailyMaxLoss;
                    p.TotalDailyMaxProfitDT = ip.TotalDailyMaxProfitDT;
                    p.TotalDailyMaxLossDT = ip.TotalDailyMaxLossDT;
                    break;
                case "UI.POSITIONS.TOTAL.ADDORUPDATE":
                        try
                        {
                            p = PositionCollection.FirstOrDefault(po => po.Key == ip.Key);
                            if (p != null)
                                p.Update(ip);
                            else
                                PositionCollection.Add(new PositionTotalNpc2(ip));
                            
                            this.Title = "Totals ( " + PositionCollection.Count + " )";
                        }
                        catch (Exception e)
                        {
                            _tx.SendExceptionMessage3("TotalsWindow", "Position2", "AddOrUpdate", ip.ToString(), e);
                        }                 
                    break;
            }
        }

        [XmlIgnore]
        public GS.ProcessTasks.ProcessTask<IEventArgs> ProcessTask { get; private set; }
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
