using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using GS.ProcessTasks;
using GS.Queues;
using EventArgs = GS.Events.EventArgs;

namespace GS.EventLog
{
    public partial class EventLogs : Evl, IEventLogs, IHaveUri, IHaveXDocument //, IEventLogs
    {
        [XmlIgnore]
        public  List<IEventLog> EventLogList;
  //      public event EventHandler<Events.EventArgs> NewItemEvent;

        protected EvlQueue EvlQueue { get; set; }

        private bool _isQueueEnabled;
        private EvlModeEnum _mode;
        public bool IsQueueEnabled { get;  set; }

        private IEventLog _primary;
        public EventLogs()
        {
            EventLogList = new List<IEventLog>();
            EvlQueue = new EvlQueue();
        }

        public override void Init()
        {
            this.EventLog = this;
            if(Uri.HasValue())
                DeSerializationCollection2(Uri);
            else if(XDocument!=null)
                DeSerializationCollection2(XDocument);
            else
                throw new NullReferenceException("EventLogs: " + GetType().FullName + ": Collection for Items Seriealization Is Null");

            SetupProcessTask();

            AddItem(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeFullName, TypeFullName, "Init Begin", GetType().FullName, "Count: " + EventLogList.Count);

            // SetMode(EvlModeEnum.Init);

            foreach (var e in EventLogList)
            {
                //if (!e.IsEnabled)
                //    continue;
                e.EventLogs = this;
                e.Parent = this;
                e.Init();
                               
               // e.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "EventLogs", Name, "Init",  e.ToString(),"");
            }
            SetPrimary(); 
            foreach (var e in EventLogList)
            {
                //if (!e.IsEnabled)
                //    continue;

              //  e.SetMode(EvlModeEnum.Init);
                if (e.IsEnabled)
                {
                    AddItem(EvlResult.SUCCESS, EvlSubject.INIT, e.ParentTypeFullName, e.TypeFullName, "Init()",
                        e.FullInfo, e.ToString());

                    e.WhoAreYou();
                }
                else
                {
                    AddItem(EvlResult.FATAL, EvlSubject.INIT, e.ParentTypeFullName, e.TypeFullName, "Init() FAILURE",
                        e.FullInfo, e.ToString());
                }
              //  e.SetMode(EvlModeEnum.Nominal);
            }
            AddItem(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeFullName, TypeFullName, "Init Finish", GetType().FullName, "Count: " + EventLogList.Count);
          //  Serialize

            // SetMode(EvlModeEnum.Nominal);

            // SetupProcessTask();

            // Start();
        }
        public override void Init(string uri)
        {
            DeSerializationCollection2(uri);
            foreach (var e in EventLogList)
            {
                e.Init();
            }
            SetPrimary();
            foreach (var e in EventLogList)
            {
                if(e.IsEnabled)
                    AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "EventLogs", e.Name, "Init", "Initialization", e.ToString());
                else
                    AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "EventLogs", e.Name, "Init", "Initialization Failure", e.ToString());
            }

        }
   
        public IEventLog Primary
        {
            get { return _primary ?? SetPrimary(); }
        }
        private IEventLog SetPrimary()
        {
            _primary = EventLogList.FirstOrDefault(e => e.IsEnabled && e.IsPrimary) ??
                       EventLogList.FirstOrDefault(e => e.IsEnabled);
            if(_primary == null)
                throw new NullReferenceException("EventLogs.SetPrimary() Failure: Primary is Not Found");
            return _primary;
        }

        public override void SetMode(EvlModeEnum m)
        {
            if (m == EvlModeEnum.Init)
            {
                _mode = m;
                _isQueueEnabled = IsQueueEnabled;
                IsQueueEnabled = false;
            }
            else
            {
                _mode = m;
                IsQueueEnabled = _isQueueEnabled;
            }
        }

        public void AddItem(EvlResult result, string operation, string description)
        {
           // throw new NotImplementedException();
        }

        public void AddItem(EvlResult result, EvlSubject subject, string source, string operation, string description, string objects)
        {
            if (!IsEnabled) return;

            var evi = new EventLogItem
            {
                DT = DateTime.Now,
                ResultCode = result,
                Subject = subject,
                Source = source,
                Entity = source,
                Operation = operation,
                Description = description,
                Object = objects
            };
            if (IsQueueEnabled)
                Push(evi);
            else
                AddItem(evi);
        }

        public event EventHandler<IEventArgs> EventLogItemsChanged;

        protected virtual void OnEventLogItemsChanged(IEventArgs e)
        {
            EventHandler<IEventArgs> handler = EventLogItemsChanged;
            if (handler != null) handler(this, e);
        }


        public override IEnumerable<IEventLogItem> Items {
            get { return Primary.Items; }
        }
     
        public void AddItem(IEventLogItem evi)
        {
            if (!IsEnabled)
                return;

            evi.Index = Next;

            if (IsUIEnabled)
                FireUIEvent(evi);

            //if (IsAsync /* && IsSaveEnabled */)
            //{
            //    foreach (var evl in EventLogList) // .Where(e => e.IsEnabled))
            //    {
            //        if (!evl.IsEnabled) continue;

            //        IEventLog evl1 = evl;
            //        Task.Factory.StartNew(() =>
            //        {
            //            try
            //            {
            //                evl1.AddItem(evi);
            //            }
            //            catch (Exception e)
            //            {
            //                SendExceptionMessage("EventLogs.AddItem(evl1.AddItem(evi))", e.Message, e.Source);
            //                throw;
            //            }
            //        });
            //    }
            //}
            //else  // else if (!IsAsync && IsSaveEnabled)

            foreach (var evl in EventLogList) //.Where(evl => evl.IsEnabled))
            {
                if (!evl.IsEnabled) continue;

                try
                {
                    evl.AddItem(evi);
                }
                catch ( System.Data.Entity.Core.EntityException e)
                {
                    SendExceptionMessage3("EventLogs", evi.GetType().ToString(),
                        "EventLogs.AddItem(evli.AddItem(evi))", evi.ToString(), e);
                }
                catch (Exception e)
                {
                    SendExceptionMessage3("EventLogs", evi.GetType().ToString(),
                        "EventLogs.AddItem(evli.AddItem(evi))", evi.ToString(), e);
                    throw;
                }
            }
            //if (IsAsync && IsUIEnabled)
            //{
            //    Task.Factory.StartNew(() =>
            //    {
            //        try
            //        {
            //            FireUIEvent(evi);
            //        }
            //        catch (Exception e)
            //        {
            //            SendExceptionMessage("EventLogs.AddItem(FireEvent())", e.Message, e.Source);
            //            throw;
            //        }
            //    });
            //}
            //else if (!IsAsync && IsUIEnabled)
            //{
            //    try
            //    {
            //        FireUIEvent(evi);
            //    }
            //    catch (Exception e)
            //    {
            //        SendExceptionMessage("EventLogs.AddItem(FireEvent())", e.Message, e.Source);
            //        throw;
            //    }
            //}
        }
       

        public void AddItem(EvlResult result, EvlSubject subject, string source, string entity, string operation, string description,
            string objects)
        {
            if (!IsEnabled) return;
            
            var evi = new EventLogItem
            {
                DT = DateTime.Now,
                ResultCode = result,
                Subject = subject,
                Source = source,
                Entity = entity,
                Operation = operation,
                Description = description,
                Object = objects
            };
            if( IsQueueEnabled)
                Push(evi);
            else
                AddItem(evi);
        }

        //private void FireEvent(IEventLogItem evli)
        //{

        //    if (NewItemEvent != null)
        //    {
        //        NewItemEvent(this, new Events.EventArgs{Category = "EventLog", Entity = "EventLogItem", Operation = "New", Object = evli});
        //    }
        //}

        private bool DeSerializationCollection2(string uri)
        {
            try
            {
                var xDoc = XDocument.Load(uri);
                var ss = xDoc.Descendants(XPath).FirstOrDefault();
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

                    var typeName = this.GetType().Namespace + '.' + x.Name.ToString().Trim();
                    var t = Type.GetType(typeName, false, true);
                    var s = Serialization.Do.DeSerialize(t, x, null);

                    if (s != null) EventLogList.Add((IEventLog)s);
                }
                //  if (_evl != null)
                //      _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Startegies", "Finish DeSerialization", "Startegies Count = " + Count, "");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
                //  if (_evl == null) throw new SerializationException("Serialization error");
                //  _evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Startegies", "DeSerialization", "Failure", "");

            }
            return true;
        }
        private bool DeSerializationCollection2(XDocument xDoc)
        {
            try
            {
                
                var ss = xDoc.Descendants(XPath).FirstOrDefault();
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

                    var typeName = this.GetType().Namespace + '.' + x.Name.ToString().Trim();
                    var t = Type.GetType(typeName, false, true);
                    var s = Serialization.Do.DeSerialize(t, x, null);

                    if (s != null) EventLogList.Add((IEventLog)s);
                }
                //  if (_evl != null)
                //      _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Startegies", "Finish DeSerialization", "Startegies Count = " + Count, "");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
                //  if (_evl == null) throw new SerializationException("Serialization error");
                //  _evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Startegies", "DeSerialization", "Failure", "");

            }
            return true;
        }

        private void Serialize()
        {
            var tr = new StreamWriter("Evl.xml");
            Type t = GetType();
            var sr = new XmlSerializer(t);
            sr.Serialize(tr, this);
            tr.Close();
        }

        public void ClearSomeData(int count)
        {
           // throw new NotImplementedException();
        }

        public void Push(IEventLogItem queueItem)
        {
            if (IsProcessTaskInUse)
                ProcessTask?.EnQueue(queueItem);
            else
                EvlQueue.Push(queueItem);
        }
        // Need Thread for this
        public void DeQueueProcess()
        {
            // Evlm(EvlResult.WARNING, EvlSubject.TECHNOLOGY, FullName, "Repository", "DeQueueProcess", "Try to get Entity to Perform Operation","");
            if (EvlQueue.IsEmpty)
                return;
            var items = EvlQueue.GetItems();
            foreach (var i in items)
            {
                AddItem(i);
            }
        }
    }
}
