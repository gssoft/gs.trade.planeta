using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.EventLog.DataBase.Repository;
using GS.Identity;
using GS.Interfaces;
using GS.Extension;

namespace GS.EventLog
{
    public class DbEventLog : Evl, IEventLog
    {
        public string DataBaseName { get; set; }
        public int TimeOut { get; set; }
        public IEventLog Primary { get { return this; } }

        private EvlRepository _repository;      

        protected DataBase.Model.DbEventLog DbEvl { get; private set; }

        public DbEventLog()
        { }

        public DbEventLog(string databaseName)
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

            
            _repository = new EvlRepository {DbName = DataBaseName, TimeOut = TimeOut, Parent = this};

            DbEvl = _repository.AddNew(evl: new DataBase.Model.DbEventLog
            {
                Alias = Code,
                Code = Code,
                Name = Name,
                Description = Description
            }
            );
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
            if (EventLogs == null || i.Index == 0)
                i.Index = Next;

            var ei = new DataBase.Model.DbEventLogItem
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
                            _repository.Add(ei);
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
                        _repository.Add(ei);
                    }
                    if (IsUIEnabled)
                    {
                        FireUIEvent(i);
                    }
                }
                catch (Exception e)
                {
                    SendExceptionMessage3(Name, ei.GetType().ToString(), "DbEventLog.AddItem()", ei.ToString(), e);
                    throw;
                }
            }
        }

        public void ClearSomeData(int count)
        {

        }

        public long Count()
        {
            return _repository.EventLogItemsCount(DbEvl);
        }
    }
}
