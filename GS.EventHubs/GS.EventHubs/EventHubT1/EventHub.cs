using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GS.Collections;
using GS.EventHubs.Interfaces;
using GS.Extension;
using GS.Interfaces;

//using EventHubItem = GS.EventHubs.EventHubType.EventHubItem; 

namespace GS.EventHubs.EventHubT1
{
    public partial class EventHub<TContent> : 
        DictionaryCollection<string, IEventHubItem<TContent>>, IEventHubT1<TContent>
    {
        public bool IsQueueEnabled { get; set; }
        public List<EventHubItem<TContent>> EventHubItems { get; set; }        
        public EventHub()
        {
            EventHubItems = new List<EventHubItem<TContent>>();
        }
        public override void Init(IEventLog eventLog)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName, $"{m}", "Begin", ToString());

                // SetupProcessTask();
                
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

                if(!Items.Any())
                    foreach (var ehi in EventHubItems) Register(ehi);
                
                Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName, $"{m}", "Finish", ToString());
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        public IEventHubItem<TContent> Register(IEventHubItem<TContent> ehi)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            if (ehi == null) return null;
            try
            {
                AddOrGet(ehi);
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                    $"{m}", ehi.ToString(), ToString());
                return ehi;
            }
            catch (Exception e)
            {
               SendException(e);
            }
            return null;
        }

        public void EnQueue(IHaveContent<TContent> args)
        {
            try
            {
                if (IsProcessTaskInUse)
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
        public void EnQueue(object sender, IHaveContent<TContent> args)
        {
            EnQueue(args);
        }
        protected void FireEventOperation(IHaveContent<TContent> args)
        {
            try
            {
                var key = args.Key.TrimUpper();
                var evhubitem = GetByKey(key);
                if (evhubitem != null)
                    evhubitem.FireEvent(args.Content);
                else
                {
                    //var sender = args.Sender?.GetType().FullName;
                    //var ex = new NullReferenceException("EventHub: FireEvent() Failure. EventHandler() is Null for Key=" + key);
                    //SendExceptionMessage3(FullName, Code, "FireEvent() " + args.OperationKey,
                    //$"Sender: {sender} Args: {args})", ex);
                }
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }       
        public Task FireEventAsync(IHaveContent<TContent> args)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var key = args.Key;
                    var evhubitem = GetByKey(key);
                    evhubitem?.FireEvent(args.Content);
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
