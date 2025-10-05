using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Elements;
using GS.Events;
using GS.Exceptions;
using GS.Interfaces;

namespace GS.Trade.Storage
{
    //public abstract class TradeStorage : Element
    //{   [XmlIgnore]
    //    public IEventLog EventLog { get; set; }
    //    [XmlIgnore]
    //    public ITradeStorage TradeStorages { get; set; }

    //    public event EventHandler<IEventArgs> StorageChangedEvent;
    //    public virtual void OnStorageChangedEvent(IEventArgs e)
    //    {
    //        EventHandler<IEventArgs> handler = StorageChangedEvent;
    //        if (handler != null) handler(this, e);
    //    }

    //    public event EventHandler<IEventArgs> ExceptionEvent;
    //    protected virtual void OnExceptionEvent(IEventArgs e)
    //    {
    //        EventHandler<IEventArgs> handler = ExceptionEvent;
    //        if (handler != null) handler(this, e);
    //    }
    //    public void Init(IEventLog eventLog)
    //    {
    //        if (eventLog == null)
    //        {
    //            SendExceptionMessage(Code, "TradeStorage.Init(Evl==null" + "Null reference", Name);
    //            throw new NullReferenceException("TradeStorage.Init(Evl==null");
    //        }
    //        EventLog = eventLog;
    //    }
    //    protected void FireEvent(string category, string entity, string operation, object o)
    //    {
    //        var ea = new Events.EventArgs
    //        {
    //            Category = category,
    //            Entity = entity,
    //            Operation = operation,
    //            Object = o
    //        };
    //        if (TradeStorages == null)
    //            OnStorageChangedEvent(ea);
    //        else
    //            TradeStorages.OnStorageChangedEvent(ea);
    //    }
    //    protected void SendExceptionMessage(string source, string message, string excSource)
    //    {
    //        OnExceptionEvent(new Events.EventArgs
    //        {
    //            Category = "UI.Exceptions",
    //            Entity = "Exception",
    //            Operation = "Add",
    //            Object = new GSException { Source = source, Message = message, SourceExc = excSource }
    //        });
    //    }
    //    protected void Evlm( EvlResult res, EvlSubject subj,
    //                            string source, string entity, string operation, string description, string obj)
    //    {
    //        if (EventLog == null)
    //        {
    //            SendExceptionMessage(Code, "TradeStorage.Evlm(EvenLog==null)" + "Null reference", Name);
    //            throw new NullReferenceException("TradeStorage.Evlm(EvenLog==null)");
    //        }
            
    //        EventLog.AddItem(res, subj, source, entity, operation, description, obj);
    //    }
    //}
}
