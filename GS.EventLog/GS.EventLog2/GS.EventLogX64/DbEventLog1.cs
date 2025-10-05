using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Elements;
using GS.EventLog.DataBase.Repository;
using GS.EventLog.DataBase1.Dal;
using GS.EventLog.Dto;
using GS.Identity;
using GS.Interfaces;
using GS.Extension;
using GS.EventLog.DataBase1.Model;
using GS.Queues;
using DbEventLog = GS.EventLog.DataBase1.Model.DbEventLog;
using DbEventLogItem = GS.EventLog.DataBase1.Model.DbEventLogItem;

namespace GS.EventLog
{
    using DbEventLog = GS.EventLog.DataBase1.Model.DbEventLog;
    using DbEventLogItem = GS.EventLog.DataBase1.Model.DbEventLogItem;
    public class DbEventLog1 : Evl, IEventLog, IHaveQueue<IEventLogItem>
    {
        public string EventLogKey { get; set; }
        public string AppCode { get; set; }
        public string DataBaseName { get; set; }
        public int TimeOut { get; set; }

        public IEventLog Primary { get { return this; } }
        [XmlIgnore]
        public Receiver<EventLogItemDto> ReceiverDto { get; private set; }
 
        protected DataBase1.Model.DbEventLog DbEvl { get; private set; }

        public DbEventLog1()
        {
            
        }

        public DbEventLog1(string databaseName)
        {
            DataBaseName = databaseName;
        }

        public override IEnumerable<IEventLogItem> Items
        {
            get { throw new NotImplementedException(); }
        }

        public override void Init()
        {
            if( DataBaseName.HasNoValue())
                throw new NullReferenceException(
                    "DbEventLog.Init(DataBaseName==null). Use DbEventLog(strind databaseName) Constructor");

            ReceiverDto = new Receiver<EventLogItemDto> {Parent = this};

            using (var db = GetEvlContext())
            {
                DbEvl = CreateDbEventLog(db);
            }
            if (DbEvl == null)
                throw new NullReferenceException("DbEventLog == null");
        }
        public void AddItem(EvlResult result, string operation, string description)
        {
            AddItem(new EventLogItem
            {
                DT = DateTime.Now,
                ResultCode = result,
                Operation = operation,
                Description = description,
                Index = Convert.ToInt64(Identity.Next())
            });
        }

        public void AddItem(EvlResult result, EvlSubject subject,
                                    string source, string operation, string description, string sobject)
        {
            AddItem(new EventLogItem
            {
                DT = DateTime.Now,
                ResultCode = result,
                Subject = subject,
                Source = source,
                Operation = operation,
                Description = description,
                Object = sobject,
                Index = Convert.ToInt64(Identity.Next())
            });
        }

        public void AddItem(EvlResult result, EvlSubject subject, string source, string entity, string operation, string description,
                            string sobject)
        {
            AddItem(new EventLogItem
            {
                DT = DateTime.Now,
                ResultCode = result,
                Subject = subject,
                Source = source,
                Entity = entity,
                Operation = operation,
                Description = description,
                Object = sobject,
                Index = Convert.ToInt64(Identity.Next())
            });
        }
        
        public void AddItem(IEventLogItem i)
        {
            if (!IsEnabled)
                return;
            if (DbEvl == null)
                return;
            if (EventLogs == null || i.Index == 0)
                i.Index = Next;

            var ei = new DbEventLogItem
            {
                DT = i.DT,
                ResultCode = i.ResultCode,
                Subject = i.Subject,
                Source = i.Source,
                Entity = i.Entity,
                Operation = i.Operation,
                Description = i.Description,
                Object = i.Object,
                Index = i.Index,

                EventLogID = DbEvl.EventLogID
            };
            if (IsAsync)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (IsSaveEnabled)
                        {
                            AddItem(ei);
                        }
                        if (IsUIEnabled)
                        {
                            FireUIEvent(i);
                        }
                    }
                    catch (Exception e)
                    {
                        SendExceptionMessage3(Name, "DbEventLog.AddItem()", "", "", e);
                        throw;
                    }
                });
            }
            else
            {
                try
                {
                    if (IsSaveEnabled)
                    {
                        AddItem(ei);
                    }
                    if (IsUIEnabled)
                    {
                        FireUIEvent(i);
                    }
                }
                catch (Exception e)
                {
                    SendExceptionMessage3(Name, ei.GetType().ToString(), "DbEventLog.AddItem()", ei.ToString(), e);
                }
            }
        }

        private void AddItem(EventLogItemDto dto)
        {
            
                AddItem(CreateDbEventLogItem(dto));
        }

        private void AddItem(DbEventLogItem i)
        {
            //IsEnabled = false;
            using (var db = GetEvlContext())
            {
                try
                {
                    if (! db.IsEvlExist(i.EventLogID))
                    {
                        DbEvl = CreateDbEventLog(db);
                        if (DbEvl == null)
                        {
                            SendExceptionMessage3(FullCode, i.GetType().ToString(),
                                System.Reflection.MethodBase.GetCurrentMethod().Name, i.ToString(),
                                new NullReferenceException("Can't to Create DbEventLog"));
                            return;
                        }
                        i.EventLogID = DbEvl.EventLogID;
                    }
                    db.Add(i);
                    // Only if Success Enable this EventLog
                    IsEnabled = true;
                }
                catch (System.ComponentModel.Win32Exception e)
                {
                    IsEnabled = false;
                    SendExceptionMessage3("DbEventLogs", i.GetType().ToString(),
                        "AddItem(evli.AddItem(evi))", i.ToString(), e);
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    IsEnabled = false;
                    SendExceptionMessage3("DbEventLogs", i.GetType().ToString(),
                        "AddItem(evli.AddItem(evi))", i.ToString(), e);
                }
                catch (System.Data.Entity.Core.EntityException e)
                {
                    IsEnabled = false;
                    SendExceptionMessage3("DbEventLogs", i.GetType().ToString(),
                        "AddItem(evli.AddItem(evi))", i.ToString(), e);
                }
                catch (NullReferenceException e)
                {
                    IsEnabled = false;
                    SendExceptionMessage3("DbEventLogs", i.GetType().ToString(),
                        "AddItem(evli.AddItem(evi))", i.ToString(), e);
                }
                catch (Exception e)
                {
                    IsEnabled = false;
                    SendExceptionMessage3("DbEventLogs", i.GetType().ToString(),
                        "AddItem(evli.AddItem(evi))", i.ToString(), e);
                }
            }
        }

        private DataBase1.Model.DbEventLog CreateDbEventLog(EvlContext1 db)
        {
            {
                var dbevl = new DataBase1.Model.DbEventLog
                {
                    Alias = EventLogKey,
                    Code = EventLogKey,
                    Name = Name,
                    Description = Description
                };
                try
                {
                    return db.Register(AppCode, dbevl);
                }
                catch (Exception e)
                {
                    IsEnabled = false;
                    SendExceptionMessage3(FullCode, GetType().ToString(),
                        System.Reflection.MethodBase.GetCurrentMethod().Name, dbevl.ToString(), e);
                }
                return null;
            }
        }

        private EvlContext1 GetEvlContext()
        {
            return DataBaseName.HasValue()
                ? new EvlContext1(DataBaseName)
                : new EvlContext1();
        }

        public void ClearSomeData(int count)
        {

        }

        public long Count()
        {
           // return _repository.EventLogItemsCount(DbEvl);
            return 0;
        }

        public void Push(IEventLogItem queueItem)
        {
            throw new NotImplementedException();
        }

        public void DeQueueProcess()
        {
            throw new NotImplementedException();
        }

        // public EvlModeEnum Mode { get; private set; }
        private bool _isQueueEnabled;

        public override void SetMode(EvlModeEnum m)
        {
            if (m == EvlModeEnum.Init)
            {
                Mode = m;
                _isQueueEnabled = IsQueueEnabled;
                IsQueueEnabled = false;
            }
            else
            {
                Mode = m;
                IsQueueEnabled = _isQueueEnabled;
            }
        }
        public bool IsQueueEnabled { get; set; }

        public DbEventLogItem CreateDbEventLogItem(EventLogItemDto i)
        {
            return new DbEventLogItem
            {
                // EventLogItemID = i.ID,
                EventLogID = i.EventLogID,
                DT = i.DT,
                ResultCode = i.ResultCode,
                Subject = i.Subject,
                Source = i.Source,
                Entity = i.Entity,
                Operation = i.Operation,
                Description = i.Description,
                Object = i.Object,
                Index = i.Index
            };
        }
    }
}
