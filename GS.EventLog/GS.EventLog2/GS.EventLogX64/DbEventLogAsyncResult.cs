using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.EventLog.DataBase.Repository;
using GS.Events;
using GS.Interfaces;
using EventArgs = GS.Events.EventArgs;

//using EventLogItem = GS.EventLog.DataBase.Model.DbEventLogItem;

namespace GS.EventLog
{
    //public class DbEventLogAsyncResult :   Element, IEventLog
    //{
    //    public delegate int AddDb(DataBase.Model.DbEventLogItem evli);
    //    private AddDb _adder;

    //    public bool Async { get; set; }
    //    public bool Debug { get; set; }

    //    public bool IsEnabled { get; set; }
    //    public bool IsAsync { get; set; }
    //    public bool IsPrimary { get; set; }

    //    public bool IsSaveEnabled { get; set; }
    //    public bool IsUIEnabled { get; set; }
    //    public IEventLog EventLogs { get; set; }

    //    public IEventLog Primary { get { return this; } }

    //    private EvlRepository _repositary;
    //    public DataBase.Model.DbEventLog DbEvl { get; private set; }

    //    public DbEventLogAsyncResult()
    //    {}

    //    public IEventLog GetPrimary()
    //    {
    //        return this as IEventLog;
    //    }

    //    public long GetEventLogItemsCount()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public long GetEventLogItems(long fromIndex, List<IEventLogItem> itemList)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Init()
    //    {
    //        _repositary = new EvlRepository();
    //        DbEvl = _repositary.AddNew(evl: new DataBase.Model.DbEventLog
    //        {
    //            Alias = Alias,
    //            Code = Code,
    //            Name = Name,
    //            Description = Description
    //        }
    //        );

    //        if (DbEvl == null)
    //            throw new NullReferenceException("DbEventLog == null");
    //        _adder = AddToDb;
    //    }

    //    public void Init(string uri)
    //    {
    //    }

    //    public void AddItem(EvlResult result, string operation, string description)
    //    {

    //    }

    //    public void AddItem(EvlResult result, EvlSubject subject, string source, string operation, string description, string objects)
    //    {

    //    }

    //    public void AddItem(EvlResult result, EvlSubject subject, string source, string entity, string operation, string description,
    //                        string sobject)
    //    {
    //        var ei = new DataBase.Model.DbEventLogItem
    //            {
    //                DT = DateTime.Now,
    //                ResultCode = result,
    //                Subject = subject,
    //                Source = source,
    //                Entity = entity,
    //                Operation = operation,
    //                Description = description,
    //                Object = sobject,

    //                EventLogID = DbEvl.EventLogID
    //            };
    //        if (Async)
    //        {
    //            if( Debug )
    //                _adder.BeginInvoke(ei, AddComplete, ei);
    //            else
    //                 _adder.BeginInvoke(ei, null, null);
    //        }
    //        else
    //            AddToDb(ei);
    //    }

    //    public event EventHandler<IEventArgs> EventLogItemsChanged;

    //    public void OnEventLogChangedEvent(EventArgs e)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    protected virtual void OnEventLogItemsChanged(IEventArgs e)
    //    {
    //        EventHandler<IEventArgs> handler = EventLogItemsChanged;
    //        if (handler != null) handler(this, e);
    //    }

    //    public event EventHandler<IEventArgs> EventLogChangedEvent;
    //    public void OnEventLogChangedEvent(IEventArgs e)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public event EventHandler<IEventArgs> ExceptionEvent;
    //    public void OnExceptionEvent(IEventArgs e)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IEnumerable<IEventLogItem> Items { get; private set; }

    //    public void AddItem(IEventLogItem i)
    //    {
    //        var ei = new DataBase.Model.DbEventLogItem
    //        {
    //            DT = i.DT,
    //            ResultCode = i.ResultCode,
    //            Subject = i.Subject,
    //            Source = i.Source,
    //            Entity = i.Entity,
    //            Operation = i.Operation,
    //            Description = i.Description,
    //            Object = i.Object,

    //            EventLogID = DbEvl.EventLogID
    //        };
    //        if (Async)
    //        {
    //            if (Debug)
    //                _adder.BeginInvoke(ei, AddComplete, ei);
    //            else
    //                _adder.BeginInvoke(ei, null, null);
    //        }
    //        else
    //            AddToDb(ei);
    //    }

    //    public void ClearSomeData(int count)
    //    {

    //    }
    //    private int AddToDb(DataBase.Model.DbEventLogItem evli)
    //    {
    //        //Console.WriteLine(evli.ToString());
    //        return _repositary.Add(evli);
    //    }

    //    private static void AddComplete(IAsyncResult iar)
    //    {
    //        var ar = (AsyncResult)iar;
    //        var adder = (AddDb)ar.AsyncDelegate;
    //        Console.WriteLine("AsyncResult={0}", adder.EndInvoke(iar));
    //        var ei = (DataBase.Model.DbEventLogItem) iar.AsyncState;
    //        Console.WriteLine(ei.ToString());

    //    }

    //    public long Count()
    //    {
    //        return _repositary.EventLogItemsCount(DbEvl);
    //    }
    //}
}
