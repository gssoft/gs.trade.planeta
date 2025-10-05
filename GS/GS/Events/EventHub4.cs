using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Collections;
using GS.Containers5;
using GS.Extension;
using GS.Interfaces;
using GS.Elements;
using GS.ProcessTasks;

namespace GS.Events
{
    using EventArgs = GS.Events.EventArgs;

    public interface IEventHubItem4 : Containers5.IHaveKey<string>
    {
        void FireEvent(object sender, Events.IEventArgs eventArgs);
        event EventHandler<Events.IEventArgs> EventHandler;
    }

    public class EventHubItem4 : Element1<string> , IEventHubItem3
    {
        public event EventHandler<Events.IEventArgs> EventHandler;
        public override string Key => 
                       Category.WithRight("." + Entity).TrimUpper();

        public void FireEvent(object sender, Events.IEventArgs eventArgs)
        {
            EventHandler?.Invoke(sender, eventArgs);
        }
    }
    //public interface IEventHub4 : IElement1<string>
    //{
    //    void Init(IEventLog evl);

    //    IEventHubItem Register(IEventHubItem ehi);
    //    void FireEvent(object sender, Events.IEventArgs eventArgs);
    //    void EnQueue(object sender, Events.IEventArgs eventArgs);
    //    void Subscribe(string category, string entity, EventHandler<Events.IEventArgs> callback);
    //    void UnSubscribe(string category, string entity, EventHandler<Events.IEventArgs> callback);
    //    void DeQueueProcess();
    //    void Start();
    //    void Stop();
    //}
    //public class EventHub3 : DictionaryCollection< string, IEventHubItem>, IEventHub3
    public interface IEventHub4 : IEventHub
    {
           
    }

    public class EventHub4 : Element32<string,  IEventArgs, DictionaryContainer<string, EventHubItem3>,
        EventHubItem3>
    {
        //public bool IsQueueEnabled { get; set; }
        public List<EventHubItem3> EventHubItems { get; set; }

        [XmlIgnore]
        public GS.ProcessTasks.ProcessTask<IEventArgs> ProcessTask { get; private set; }

        // protected EventArgsQueue Queue;
        private readonly object _locker;

        public EventHub4()
        {
            EventHubItems = new List<EventHubItem3>();
            Collection = new DictionaryContainer<string, EventHubItem3>();
            //Queue = new EventArgsQueue();
            _locker = new object();
        }
        public override string Key => FullName;
        public override void Init(IEventLog eventLog)
        {
            base.Init(eventLog);
            SetupProcessTask();
        }
        public override void EnQueue(object sender, IEventArgs args)
        {
            try
            {
                if (args == null)
                {
                    //throw new NullReferenceException("FireEvent(Args==Null");
                    var ex = new ArgumentNullException(nameof(args));
                    SendExceptionMessage3(FullName, GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, "EvenArgs", ex);
                    return;
                }
                if (IsQueueEnabled && !args.IsHighPriority)
                {
                   // base.EnQueue(sender, args);
                   ProcessTask.EnQueue(args);
                }
                else
                    FireEventOperation1(sender, args);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "EventHub", System.Reflection.MethodBase.GetCurrentMethod().Name,
                                             args?.ToString() ?? "EvenArgs", e);
                // throw;
            }
        }
        // 2018.05.11
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
                    var ex = new NullReferenceException("EventHub: FireEvent() Failure. EventHandler() is Null for Key=" + key);
                    SendExceptionMessage3(FullName, "EventHub", "FireEvent() " + args.OperationKey, args.ToString(), ex);
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "EventHub", "FireEvent() " + args.OperationKey, args.ToString(), e);
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
                    var ex = new NullReferenceException("EventHub: FireEvent() Failure. EventHandler() is Null for Key=" + key);
                    SendExceptionMessage3(FullName, "EventHub", "FireEvent() " + args.OperationKey, args.ToString(), ex);
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "EventHub", "FireEvent() " + args.OperationKey, args.ToString(), e);
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

        public void Subscribe(string category, string entity, EventHandler<Events.IEventArgs> callback)
        {
            var key = (category.WithRight(".") + entity).TrimUpper();
            try
            {
                var ev = GetByKey(key);
                if (ev == null)
                {
                    var evhi = new EventHubItem3
                    {
                        Category = category,
                        Entity = entity
                    };
                    evhi.EventHandler += callback;
                    Register(evhi);
                }
                else
                {
                    if (callback != null)
                        ev.EventHandler += callback;
                }
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName,
                    "EventHubItem", "Subscribe()", "Key=" + key, "");
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "EventHub", "Subscribe", "Key=" + category + "." + entity, e);
                throw;
            }
        }
        public void UnSubscribe(string category, string entity, EventHandler<Events.IEventArgs> callback)
        {
            try
            {
                if (callback == null)
                    return;

                var ev = GetByKey((category.WithRight(".") + entity).TrimUpper());
                if (ev == null)
                    return;
                ev.EventHandler -= callback;

                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName,
                    "EventHubItem", "UnSubscribe()", "Key=" + category + "." + entity, "");
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "EventHub", "UnSubscribe", "Key=" + category + "." + entity, e);
                throw;
            }
        }      
        public override void DeQueueProcess()
        {
            if (Queue.IsEmpty)
                return;

            var items = Queue.GetItems();

            //Evlm1(EvlResult.WARNING, EvlSubject.TECHNOLOGY, FullName, "EventHub", "DeQueueProcess",
            //                        "Count=" + items.Count() + " ; Try to get Args to Fire Event", "");

            foreach (var i in items)
            {
                // Exception Handling inside FireEvent()
                // FireEventOperation(i.Sender, i);
                FireEventOperation(i);
            }
        }
        private void SetupProcessTask()
        {
            ProcessTask = new ProcessTask<IEventArgs>();
            ProcessTask.Init();
            ProcessTask.Parent = this;
            ProcessTask.ItemProcessingAction = FireEventOperation;
        }
    }
}
