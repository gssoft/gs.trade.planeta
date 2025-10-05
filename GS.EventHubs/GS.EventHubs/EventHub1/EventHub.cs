using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using GS.Collections;
using GS.Events;
using GS.Interfaces;

namespace GS.EventHubs.EventHub1
{
    public partial class EventHub : DictionaryCollection<string, IEventHubItem>, IEventHub
    {
        public bool IsQueueEnabled { get; set; }
        public List<EventHubItem> EventHubItems { get; set; }
        
        public EventHub()
        {
            EventHubItems = new List<EventHubItem>();
        }
        public override void Init(IEventLog eventLog)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName, $"{m}", "Begin", ToString());

                SetupProcessTask();
                
                base.Init(eventLog);
                foreach (var ehi in EventHubItems)
                {
                    Register(ehi);
                }
                Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName, $"{m}", "Finish", ToString());
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        public override void Init()
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName, $"{m}", "Begin", ToString());

                SetupProcessTask();
                foreach (var ehi in EventHubItems) Register(ehi);
                
                Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName, $"{m}", "Finish", ToString());
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        public IEventHubItem Register(IEventHubItem ehi)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            if (ehi == null) return null;
            try
            {
                AddOrGet(ehi);
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                    $"{m}", ehi.ToString(), ToString());
            }
            catch (Exception e)
            {
               SendException(e);
            }
            return null;
        }
        public void EnQueue(IEventArgs args)
        {
            try
            {
                if (IsProcessTaskInUse && !args.IsHighPriority)
                {
                    ProcessTask?.EnQueue(args);
                }
                else
                    FireEventOperation(args);
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        public void EnQueue(object sender, IEventArgs args)
        {
            EnQueue(args);
        }
        protected void FireEventOperation(IEventArgs args)
        {
            try
            {
                var key = args.Key;
                var evhubitem = GetByKey(key);
                if (evhubitem != null)
                    evhubitem.FireEvent(args.Sender, args);
                else
                {
                    var sender = args.Sender?.GetType().FullName;
                    var ex = new NullReferenceException("EventHub: FireEvent() Failure. EventHandler() is Null for Key=" + key);
                    SendExceptionMessage3(FullName, Code, "FireEvent() " + args.OperationKey,
                    $"Sender: {sender} Args: {args})", ex);
                }
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }       
        public Task FireEventAsync(object sender, Events.EventArgs args)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var key = args.Key;
                    var evhubitem = GetByKey(key);
                    evhubitem?.FireEvent(sender, args);
                }
                catch (Exception e)
                {
                    SendException(e);
                }
            });
        }
            
        //public void DeQueueProcess()
        //{
        //    try
        //    {
        //        while (!Queue.IsEmpty)
        //        {
        //            var items = Queue.GetItems();
        //            foreach (var i in items)
        //                FireEventOperation(i);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        SendException(e);
        //    }
        //}  
    }
}
