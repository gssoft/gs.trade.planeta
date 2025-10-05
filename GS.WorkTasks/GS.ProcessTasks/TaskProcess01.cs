using System;
using System.Collections.Generic;
using GS.Containers5;
using GS.Elements;
using GS.EventLog.DataBase;
using GS.Events;
using GS.Extension;
using GS.Interfaces;

namespace GS.ProcessTasks
{
    public interface IEventHubItem3 : IHaveKey<string>
    {
        void FireEvent(object sender, GS.Events.IEventArgs eventArgs);
        event EventHandler<IEventArgs> EventHandler;
    }
    public class EventHubItem3 : Element1<string> , IEventHubItem3
    {
        public event EventHandler<IEventArgs> EventHandler;

        public override string Key => Category.WithRight("." + Entity).TrimUpper();

        public void FireEvent(object sender, GS.Events.IEventArgs eventArgs)
        {
            EventHandler?.Invoke(sender, eventArgs);
        }
    }
    public interface IEventHub3 : IElement1<string>
    {
        void Init(IEventLog evl);

        IEventHubItem Register(IEventHubItem ehi);
        void FireEvent(object sender, GS.Events.IEventArgs eventArgs);
        void EnQueue(object sender, GS.Events.IEventArgs eventArgs);
        void Subscribe(string category, string entity, EventHandler<IEventArgs> callback);
        void UnSubscribe(string category, string entity, EventHandler<IEventArgs> callback);

        void DeQueueProcess();
    }
    //public class EventHub3 : DictionaryCollection< string, IEventHubItem>, IEventHub3
    public class TaskProcess01 : Element32<string,  IEventArgs1,
        DictionaryContainer<string, UnitOfWork>, UnitOfWork>
    {
        //public bool IsQueueEnabled { get; set; }
        public List<EventHubItem3> EventHubItems { get; set; }

        // protected EventArgsQueue Queue;
        private readonly object _locker;

        protected ProcessTask01 Task { get; set; }

        public TaskProcess01()
        {
            EventHubItems = new List<EventHubItem3>();
            Collection = new DictionaryContainer<string, UnitOfWork>();
            Task = new ProcessTask01 { Parent = this, TimeInterval = 5000 };
            //Queue = new EventArgsQueue();
            _locker = new object();
        }

        public override string Key => FullName;

        public override void Init(IEventLog eventLog)
        {
            EventLog = eventLog;
        }

        public void Start()
        {
            Task?.Start();
        }
        public void Stop()
        {
            Task?.Stop();
        }
        private static string CreateKey(string process, string category, string entity)
        {
            return string.Join(".", process.TrimUpper(), category.TrimUpper(), entity.TrimUpper());
        }
      
        public override void EnQueue(object sender, IEventArgs1 args)
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
                    base.EnQueue(sender, args);
                }
                else
                    FireEventOperation(sender, args);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "EventHub", System.Reflection.MethodBase.GetCurrentMethod().Name,
                                             args?.ToString() ?? "EvenArgs", e);
                // throw;
            }
        }

        protected void FireEventOperation(object sender, GS.Events.IEventArgs1 args)
        {
            try
            {
                var key = args.Key;
                var unitofwork = GetByKey(key);
                if (unitofwork?.Action != null && args != null)
                {
                    var clone = unitofwork.Clone();
                    clone.EventArgs = args;
                    Task.EnQueue(clone);
                }
                else
                {
                    var ex =
                        new NullReferenceException("EventHub: FireEvent() Failure. EventHandler() is Null for Key=" +
                                                   key);
                    SendExceptionMessage3(FullName, "EventHub", "FireEvent() " + args.OperationKey, args.ToString(), ex);
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "EventHub", "FireEvent() " + args.OperationKey, args.ToString(), e);
                // throw;
            }
        }

        public void FireEventAsync(object sender, GS.Events.EventArgs1 args)
        {
            //await FireEventPrivateAsync(sender, args);
            //await t;
            //Task task = FireEventPrivateAsync(sender, args);
            //task.Start();
            System.Threading.Tasks.Task t = System.Threading.Tasks.Task.Run(() =>
            {
                var key = args.Key;
                var unitofwork = GetByKey(key);
                if (unitofwork != null)
                {
                    if (unitofwork.Action == null)
                        return;
                    unitofwork.EventArgs = args;
                    Task.EnQueue(unitofwork);
                }
                else
                {
                    throw new NullReferenceException("EventHub.FireEventAsync() EventHubItem is Null Key=" + key);
                }
            });
        }

        //public async Task FireEventPrivateAsync(object sender, Events.EventArgs args)
        //{
        //    var key = args.Key;
        //    var unitofwork = GetByKey(key) as IEventHubItem;
        //    if (unitofwork != null)
        //        unitofwork.FireEvent(sender, args);
        //    //return null;
        //}

        public void Subscribe(string process, string category, string entity, Action<IEventArgs1> action)
        {
            var key = CreateKey(process, category, entity);
            try
            {
                var ev = GetByKey(key);
                if (ev == null)
                {
                    var evhi = new UnitOfWork("Process", "Category", "Entity", action, null);
                    Register(evhi);
                }
                else
                {
                    if (action != null)
                        ev.Action = action;
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
        public void UnSubscribe(string process, string category, string entity)
        {
            try
            {
                var key = CreateKey(process, category, entity);
                var ev = GetByKey(key);
                if (ev == null)
                    return;
                Remove(ev.Key);

                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName,
                    "EventHubItem", "UnSubscribe()", "Key=" + category + "." + entity, "");
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "EventHub", "UnSubscribe", "Key=" + category + "." + entity, e);
                throw;
            }
        }
        //protected void Push(IEventArgs ea)
        //{
        //    lock (_locker)
        //    {
        //        Queue.Push(ea);
        //    }
        //    Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName, "EventArgs",
        //                            "Push: " + ea.OperationKey,
        //                            "", ea.ToString());
        //}
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
                FireEventOperation(i.Sender, i);
            }
        }
    }
}
