using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Collections;
using GS.Extension;
using GS.Interfaces;
using GS.Elements;


namespace GS.Events
{
    public interface IEventHub : IElement1<string>
    {
        void Init(IEventLog evl);
        IEventHubItem Register(IEventHubItem ehi);
        void FireEvent(object sender, Events.IEventArgs eventArgs);
        void EnQueue(object sender, Events.IEventArgs eventArgs);
        void Subscribe(string category, string entity, EventHandler<Events.IEventArgs> callback);
        void UnSubscribe(string category, string entity, EventHandler<Events.IEventArgs> callback);
        void UnSubscribe(EventHandler<Events.IEventArgs> callback);

        void DeQueueProcess();
        void Start();
        void Stop();
    }
    public partial class EventHub : DictionaryCollection< string, IEventHubItem>, IEventHub
    {
        public bool IsQueueEnabled { get; set; }
        public List<EventHubItem> EventHubItems { get; set; }

        protected EventArgsQueue Queue;
        private readonly object _locker;

        public EventHub()
        {
            EventHubItems = new List<EventHubItem>();
            Queue = new EventArgsQueue();
            _locker = new object();
        }
        public override void Init(IEventLog eventLog)
        {
            SetupProcessTask();

            Evlm52(EvlResult.INFO, EvlSubject.INIT, MethodBase.GetCurrentMethod()?.Name + "Begin", "", "");

            base.Init(eventLog);
            foreach (var ehi in EventHubItems)
            {
                Register(ehi);
            }
            // SetupProcessTask();

            Evlm52(EvlResult.INFO, EvlSubject.INIT, MethodBase.GetCurrentMethod()?.Name + "Finish", "", "");
        }
        public override void Init()
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";

            Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentAndMyTypeName, TypeName, $"{m}","Begin", ToString());

            SetupProcessTask();
            
            foreach (var ehi in EventHubItems)
            {
                Register(ehi);
            }

            Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentAndMyTypeName, TypeName, $"{m}", "Finish", ToString());
        }

        public IEventHubItem Register(IEventHubItem ehi)
        {
            try
            {
                return ehi == null ? null : AddOrGet(ehi);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(Code, "EventHubItem", "Register()", ehi?.ToString() ?? "EventHubItem",e);
                // throw;
                return null;
            }
        }

        public void FireEvent(object sender, Events.IEventArgs args)
        {
            try
            {
                if(args == null)
                    throw new NullReferenceException("FireEvent(Args==Null");

                if (IsQueueEnabled && !args.IsHighPriority)
                {
                    Push(args);
                }
                else
                    FireEventOperation1(sender, args);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, Code, "FireEvent()", args?.ToString() ?? "EvenArgs", e);
                // throw;
            }
        }
        public void EnQueue(object sender, IEventArgs args)
        {
            try
            {
                if (args == null)
                {
                    //throw new NullReferenceException("FireEvent(Args==Null");
                    var ex = new ArgumentNullException(nameof(args));
                    SendExceptionMessage3(FullName, GetType().FullName, Code, 
                            MethodBase.GetCurrentMethod()?.Name + ": EvenArgs=null" , ex);
                    return;
                }
                if (IsQueueEnabled && !args.IsHighPriority)
                {
                    Push(args);
                }
                else
                    FireEventOperation1(sender, args);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, Code, MethodBase.GetCurrentMethod()?.Name,
                                             args?.ToString() ?? "EvenArgs", e);
                // throw;
            }
        }
        // for Process Task Implementation for FireEvent
        protected void FireEventOperation(Events.IEventArgs args)
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
                    $"Sender: {sender?.ToString()} Args: {args})", ex);
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, Code, "FireEvent() " + args.OperationKey, args.ToString(), e);
                // throw;
            }
        }
        protected void FireEventOperation1(object sender, Events.IEventArgs args)
        {
            try
            {
                var key = args.Key;
                var evhubitem = GetByKey(key);
                if (evhubitem != null)
                    evhubitem.FireEvent(sender, args);
                else
                {
                    var ex = new NullReferenceException("EventHub: FireEvent1() Failure. EventHandler() is Null for Key=" + key);
                    SendExceptionMessage3(FullName, Code, "FireEvent1() " + args.OperationKey, args.ToString(), ex);
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, Code, "FireEvent1() " + args.OperationKey, args.ToString(), e);
                // throw;
            }
        }
        public void FireEventAsync(object sender, Events.EventArgs args)
        {
            //await FireEventPrivateAsync(sender, args);
            //await t;
            //Task task = FireEventPrivateAsync(sender, args);
            //task.Start();
            Task t = Task.Run(() =>
            {
                var key = args.Key;
                var evhubitem = GetByKey(key);
                if (evhubitem != null)
                    evhubitem.FireEvent(sender, args);
                else
                {
                    throw new NullReferenceException("EventHub.FireEventAsync() EventHubItem is Null Key=" + key);
                }
            });
        }

        //public async Task FireEventPrivateAsync(object sender, Events.EventArgs args)
        //{
        //    var key = args.Key;
        //    var evhubitem = GetByKey(key) as IEventHubItem;
        //    if (evhubitem != null)
        //        evhubitem.FireEvent(sender, args);
        //    //return null;
        //}

        public void Subscribe(string category, string entity, EventHandler<Events.IEventArgs> callback)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";

            var key = (category.WithRight(".") + entity).TrimUpper();
            try
            {
                var ev = GetByKey(key);
                if (ev == null)
                {
                    var evhi = new EventHubItem
                    {
                        Category = category.TrimUpper(),
                        Entity = entity.TrimUpper()
                    };
                    evhi.EventHandler += callback;
                    Add(evhi);
                }
                else
                {
                    if (callback != null)
                        ev.EventHandler += callback;
                }
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentAndMyTypeName, Code,
                     $"{m}", $"Key {key}", ToString());
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, Code, "Subscribe", "Key=" + category + "." + entity, e);
               // throw;
            }
        }
        public void UnSubscribe(string category, string entity, EventHandler<Events.IEventArgs> callback)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                if (callback == null)
                    return;

                var ev = GetByKey((category.WithRight(".") + entity).TrimUpper());
                if (ev == null)
                    return;
                ev.EventHandler -= callback;

                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentAndMyTypeName, Code,
                     $"{m}", $"Key {category}.{entity}", ToString());
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, Code, "UnSubscribe", "Key="+ category+"."+entity, e);
               // throw;
            }
        }

        public void UnSubscribe(EventHandler<Events.IEventArgs> callback)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                if (callback == null)
                    return;

                foreach (var ev in Items)
                {
                    ev.EventHandler -= callback;
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentAndMyTypeName, Code,
                        "EventHubItem", "UnSubscribe()", $"Key {ev.Key}");
                }
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        protected void Push(IEventArgs ea)
        {
            //lock (_locker)
            //{
            //    Queue.Push(ea);
            //}
            //Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName, "EventArgs",
            //                        "Push: " + ea.OperationKey,
            //                        "", ea.ToString());
            if(IsProcessTaskInUse)
                ProcessTask?.EnQueue(ea);
            else
                Queue.Push(ea);
        }
        public void DeQueueProcess1()
        {
            if (Queue.IsEmpty)
                return;

            Evlm1(EvlResult.WARNING, EvlSubject.TECHNOLOGY, FullName, Code, "DeQueueProcess",
                                    "Count="+ Queue.Count + "; Try to get Argss to Fire Event","");

            var items = Queue.GetItems();

            foreach (var i in items)
            {
                //try
                //{
                //    FireEventOperation(i.Sender, i);
                //}
                //catch (Exception e)
                //{
                //    SendExceptionMessage3(FullName, i.OperationKey, "DeQueueProcess()", i.ToString(), e);
                //    //throw;
                //}

                FireEventOperation1(i.Sender, i);
            }
            //try
            //{
            //    throw new NullReferenceException("Test for Catching Exceptions");
            //}
            //catch (Exception e)
            //{
            //    SendExceptionMessage3(FullName, EntityKey, "DeQueueProcess()", ToString(),e);
            //}
       }
        public void DeQueueProcess()
        {
            while (!Queue.IsEmpty)       
            {
                //Evlm1(EvlResult.WARNING, EvlSubject.TECHNOLOGY, FullName, "EventHub", "DeQueueProcess",
                //                    "Count=" + Queue.Count + "; Try to get Argss to Fire Event", "");

                var items = Queue.GetItems();
                foreach (var i in items)
                    FireEventOperation(i);
            }
            //try
            //{
            //    throw new NullReferenceException("Test for Catching Exceptions");
            //}
            //catch (Exception e)
            //{
            //    SendExceptionMessage3(FullName, EntityKey, "DeQueueProcess()", ToString(),e);
            //}
        }
    }
}
