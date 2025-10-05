using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Containers5;
using GS.Elements;
using GS.Events;
using GS.Exceptions;
using GS.Interfaces;

namespace GS.Trade.TradeStorage
{
    public interface IStorages<TItem>
    {
        string Name { get; }
        string Code { get; }

        void Init(IEventLog evl);

       TItem Register(TItem s);
    }
    public interface IStore<TKey, TItem> // : Containers5.IHaveKey<string>
    {
        event EventHandler<GS.Events.IEventArgs> StorageChangedEvent;
        event EventHandler<GS.Events.IEventArgs> ExceptionEvent;

        IStore<TKey, TItem> ParentStore { get; set; }
        void OnExceptionEvent(IEventArgs e);
    }

    public class Store<TKey, TItem> : Element, IHaveUri, IStore<TKey, TItem> where TItem : class, IHaveKey<TKey>, IHaveInit //  Containers5.ListContainer<string, TItem>,  IStorages<TItem> where TItem : class, IHaveKey<string>
    {   [XmlIgnore]
        protected IEventLog EventLog { get; set; }

        [XmlIgnore]
        public IStore<TKey, TItem> ParentStore { get; set; }

        public string Uri { get; set; }
        public string XPath { get; set; }
        public string UriXPath { get { return Uri + "@" + XPath; } }

        public event EventHandler<GS.Events.IEventArgs> StorageChangedEvent;
        public virtual void OnStorageChangedEvent(IEventArgs e)
        {
            EventHandler<GS.Events.IEventArgs> handler = StorageChangedEvent;
            if (handler != null) handler(this, e);
        }
        public event EventHandler<GS.Events.IEventArgs> ExceptionEvent;
        public virtual void OnExceptionEvent(IEventArgs e)
        {
            EventHandler<IEventArgs> handler = ExceptionEvent;
            if (handler != null) handler(this, e);
        }
        [XmlIgnore]
        public List<TItem> StorageList { get; set; }
        [XmlIgnore]
        public Containers5.ListContainer<TKey, TItem> StorageItems; // Where TItem : class, IHaveKey<string>
       

        public Store()
        {
            StorageList = new List<TItem>();
            StorageItems = new ListContainer<TKey, TItem>(); 
        }

        public void Init(IEventLog evl)
        {
            try
            {
                if(evl == null)
                    throw new NullReferenceException("Init(evl == Null)");
                EventLog = evl;

                DeSerializationCollection(Uri,XPath,StorageList);
                foreach (var s in StorageList)
                {
                    s.Init(EventLog);
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage(Name, "Storage.Init()", e.Message, e.Source);
                throw;
            }
        }
        private bool DeSerializationCollection(string uri, string xpath, ICollection<TItem> tempList )
        {
            try
            {
                var xDoc = XDocument.Load(uri);
                var ss = xDoc.Descendants(xpath).FirstOrDefault();
                if (ss == null)
                    throw new NullReferenceException("Bad XPath?! -- nothing to Find");

                var xx = ss.Elements();
                foreach (var x in xx)
                {
                    var firsta = x.FirstAttribute;
                    if (firsta != null && firsta.Name == "enabled")
                    {
                        bool enabled;
                        if (Boolean.TryParse(firsta.Value, out enabled) && !enabled)
                            continue;
                    }

                    var typeName1 = this.GetType().Namespace + '.' + x.Name.ToString().Trim();
                    var typeName = x.Name.ToString().Trim();
                    var t = Type.GetType(typeName1, false, true);
                   // var t2 = typeof(MemStorage);
                   // t = typeof(TradeStore);
                    var s = Serialization.Do.DeSerialize(t, x, null);

                    if (s == null) continue;

                    tempList.Add((TItem) s);
                    Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, "Storage","DeSerialization()", "Add Element", s.ToString());
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage(Name, "Storage.DeserializeCollection()", e.Message, e.Source);
                throw;
            }
            return true;
        }
        protected virtual void SendExceptionMessage(string source, string operation, string message, string sourceExc)
        {
            var ea = new Events.EventArgs
            {
                Category = "UI.Exceptions",
                Entity = "Exception",
                Operation = "Add",
                Object = new GSException
                {
                    Source = source,
                    Operation = operation,
                    Message = message,
                    SourceExc = sourceExc
                }
            };
            if (ParentStore == null)
                OnExceptionEvent(ea);
            else
                ParentStore.OnExceptionEvent(ea);

            Evlm(EvlResult.FATAL, EvlSubject.PROGRAMMING, source, "Exception", operation, message, sourceExc);
        }
        protected void Evlm(EvlResult result, EvlSubject subject,
            string source, string entity, string operation, string description, string obj)
        {
            if (EventLog != null)
                EventLog.AddItem(result, subject, source, entity, operation, description, obj);
        }
    }
}
