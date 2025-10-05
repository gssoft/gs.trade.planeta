using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Containers3;
using GS.Extension;
using GS.Interfaces;
using GS.Process;

namespace GS.Events
{
    //public class EventHub
    //{
    //    private Dictionary<string, EventHandler<Events.EventArgs>> _eventHandlers;

    //    public EventHub()
    //    {
    //        _eventHandlers = new Dictionary<string, EventHandler<EventArgs>>();
    //    }

    //}
    public interface IEventHub1
    {
        string Name { get; }
        string Code { get; }

        void Init(IEventLog evl);

        IEventHubItem Register(IEventHubItem ehi);
        void FireEvent(object sender, Events.IEventArgs eventArgs);
        void Subscribe(string category, string entity, EventHandler<Events.IEventArgs> callback);
        void UnSubscribe(string category, string entity, EventHandler<Events.IEventArgs> callback);

        void DeQueueProcess();
    }

    public interface IEventHubItem : Containers5.IHaveKey<string>
    {
        void FireEvent(object sender, Events.IEventArgs eventArgs);
        event EventHandler<Events.IEventArgs> EventHandler;
    }

    public class EventHubItem : IEventHubItem
    {
        public string Entity { get; set; }
        public string Category { get; set; }

        public event EventHandler<Events.IEventArgs> EventHandler;

        public string Key
        {
            get { return Category.WithRight("." + Entity).TrimUpper(); }
        }
        public void FireEvent(object sender, Events.IEventArgs eventArgs)
        {
            EventHandler?.Invoke(sender, eventArgs);
        }
    }

    public class EventHub1 : Containers5.DictionaryContainer<string, IEventHubItem>, IEventHub1
    {
        private IEventLog _evl;

        public string Name { get; set; }
        public string Code { get; set; }

        public List<EventHubItem> EventHubItems { get; set; }

        public EventHub1()
        {
            EventHubItems = new List<EventHubItem>();
        }

        public void Init(IEventLog evl)
        {
            _evl = evl;

            //Name = "Name";
            //Code = "Code";
            //var evi = new EventHubItem
            //{
            //    Category = "Transactions",
            //    Entity = "Transaction"
            //};
            //EventHubItems.Add(evi);

            //evi = new Events.EventHubItem
            //{
            //    Category = "Orders",
            //    Entity = "Order"
            //};
            //EventHubItems.Add(evi);

            //evi = new Events.EventHubItem
            //{
            //    Category = "Order",
            //    Entity = "Status"
            //};
            //EventHubItems.Add(evi);

            //evi = new Events.EventHubItem
            //{
            //    Category = "UI.Orders",
            //    Entity = "Order"
            //};
            //EventHubItems.Add(evi);

            //Serialize("EventHubTest.xml", GetType());

            foreach (var ehi in EventHubItems)
            {
                Register(ehi);
            }
        }

        public IEventHubItem Register(IEventHubItem ehi)
        {
            return ehi == null ? null : AddOrGet(ehi);
        }

        public void FireEvent(object sender, Events.IEventArgs args)
        {
            //_evl.AddItem(EvlResult.INFO,EvlSubject.TECHNOLOGY, "EventHub", "EventHub", "FireEvent()",
            //    args.OperationKey, args.Object.GetType().ToString());
            var key = args.Key;
            var evhubitem = GetByKey(key);
            if (evhubitem != null)
                evhubitem.FireEvent(sender, args);
            else
                throw new NullReferenceException("EventHubItem: FireEvent() Failure");
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
            var ev = GetByKey((category.WithRight(".") + entity).TrimUpper());
            if (ev == null)
            {
                var evhi = new EventHubItem
                {
                    Category = category,
                    Entity = entity
                };
                evhi.EventHandler += callback;
                Add(evhi);
            }
            else
            {
                if (callback != null)
                    ev.EventHandler += callback;
            }
        }
        public void UnSubscribe(string category, string entity, EventHandler<Events.IEventArgs> callback)
        {
            var ev = GetByKey((category.WithRight(".") + entity).TrimUpper());
            if (ev == null)
                return;
            if (callback != null)
                ev.EventHandler -= callback;
        }

        public void DeQueueProcess()
        {
            
        }

        public bool Serialize(string xmlfilname, Type t)
        {
  
            TextWriter tr = null;
            try
            {
                tr = new StreamWriter(xmlfilname);
                var sr = new XmlSerializer(typeof(EventHub));
                sr.Serialize(tr, this);
                tr.Close();

                return true;
            }
            catch (Exception e)
            {

                if (tr != null) tr.Close();
                return false;
            }
        }
    }
}
