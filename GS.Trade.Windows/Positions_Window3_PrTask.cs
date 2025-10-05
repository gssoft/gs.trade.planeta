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
    public partial class PositionsWindow3
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
            }
           ));
        }
        private void ProcessPosition(IEventArgs args)
        {
            var ip = args?.Object as IPosition2;
            if (ip == null)
                return;
            if (ip.IsOpened)
            {
                var m = 1;
                var p = ip;
            }
            switch (args.OperationKey)
            {
                case "UI.POSITIONS.CURRENT.INSERT":
                        var p = new PositionNpc2(ip);
                        PositionCurrentCollection.Insert(0, p);
                        
                        Title = "Positions ( " + PositionCurrentCollection.Count + " )";                  
                    break;

                case "UI.POSITIONS.CURRENT.ADD":
                case "UI.POSITIONS.CURRENT.ADD.TOEND":
                        p = new PositionNpc2(ip);
                        PositionCurrentCollection.Add(p);
                        
                        Title = "Positions ( " + PositionCurrentCollection.Count + " )";                   
                    break;

                case "UI.POSITIONS.CURRENT.ADDNEW":
                        var po = PositionCurrentCollection.FirstOrDefault(i => i.Key == ip.Key);
                        if (po == null)
                            PositionCurrentCollection.Add(new PositionNpc2(ip));
                        
                        Title = "Positions ( " + PositionCurrentCollection.Count + " )";                  
                    break;

                case "UI.POSITIONS.CURRENT.DELETE":
                        po = PositionCurrentCollection.FirstOrDefault(i => i.Key == ip.Key);
                        if (po != null)
                            PositionCurrentCollection.Remove(po);
                        
                        Title = "Positions ( " + PositionCurrentCollection.Count + " )";                    
                    break;

                case "UI.POSITIONS.CURRENT.ADDORUPDATE":
                        //if (WindowState == WindowState.Minimized) return;
                        try
                        {
                            po = PositionCurrentCollection.FirstOrDefault(i => i.Key == ip.Key);
                            if (po != null)
                                po.Update(ip);
                            else
                            {
                                var pn = new PositionNpc2(ip);
                                PositionCurrentCollection.Add(pn);
                            }
                            
                            Title = "Positions ( " + PositionCurrentCollection.Count + " )";
                        }
                        catch (Exception e)
                        {
                            _tx.SendExceptionMessage3("PositionsWindow", "Position2", "AddOrUpdate", ip.ToString(), e);
                        }                  
                    break;

                case "UI.POSITIONS.CURRENT.UPDATE":
                    //if (WindowState == WindowState.Minimized) return;
                        try
                        {
                            po = PositionCurrentCollection.FirstOrDefault(i => i.Key == ip.Key);
                            po?.Update(ip);
                        }
                        catch (Exception e)
                        {
                            _tx.SendExceptionMessage3("PositionsWindow", "Position2", "Update", ip.ToString(), e);
                        }                   
                    break;

                case "UI.POSITIONS.CURRENT.UPDATE.PRICE2":
                        //if (WindowState == WindowState.Minimized) return;
                        
                        po = PositionCurrentCollection.FirstOrDefault(i => i.Key == ip.Key);
                        if (po != null)
                            // po.Update(ip);
                            po.Price2 = ip.Price2;                                           
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
