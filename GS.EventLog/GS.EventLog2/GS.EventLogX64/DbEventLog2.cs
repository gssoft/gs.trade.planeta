using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.ConsoleAS;
using GS.Elements;
using GS.EventLog.DataBase.Repository;
using GS.EventLog.DataBase1.Dal;
using GS.EventLog.Dto;
using GS.Identity;
using GS.Interfaces;
using GS.Extension;
using GS.EventLog.DataBase1.Model;
using GS.Queues;
using GS.Serialization;
using GS.Status;
using GS.WorkTasks;
using DbEventLog = GS.EventLog.DataBase1.Model.DbEventLog;
using DbEventLogItem = GS.EventLog.DataBase1.Model.DbEventLogItem;

namespace GS.EventLog
{
    using DbEventLog = GS.EventLog.DataBase1.Model.DbEventLog;
    using DbEventLogItem = GS.EventLog.DataBase1.Model.DbEventLogItem;
    public class DbEventLog2 : Evl, IEventLog, IHaveQueue<IEventLogItem>
    {
        private int _addsMax = 30;
        private int _addsCrnt = 0;
        public int ErrorRecoveryTimeOut { get; set; }
        public int ErrorCntToStop { get; set; }
        private int ErrorCntCurrent = 0;
        public string EventLogKey { get; set; }
        public string AppCode { get; set; }
        public string DataBaseName { get; set; }
        public int TimeOut { get; set; }
        public WorkTask3 WorkTask3 { get; set; }

        public IEventLog Primary { get { return this; } }
        [XmlIgnore]
        public StartStopStatus SSStatus { get; private set; }
        [XmlIgnore]
        public Receiver<EventLogItemDto> ReceiverDto { get; private set; }

        public SSStatusEnum Status { get; set; }

        protected DataBase1.Model.DbEventLog DbEvl { get; private set; }

        private static readonly Lazy<DbEventLog2> Lazy =
           new Lazy<DbEventLog2>(() => CreateInstance());
        public static DbEventLog2 Instance => Lazy.Value;

        private static DbEventLog2 CreateInstance()
        {
            var instance = Builder.Build<DbEventLog2>(@"Init\EventLog.xml", "DbEventLog2");
            if (instance == null)
                throw new NullReferenceException("Build DbEventLog2 Failure");

            instance.Init();
            return instance;
        }

        public DbEventLog2()
        {
            SSStatus = new StartStopStatus();
        }
        public DbEventLog2(string databaseName)
        {
            DataBaseName = databaseName;
        }
        public override IEnumerable<IEventLogItem> Items => null;

        public override void Init()
        {
            if( DataBaseName.HasNoValue())
                throw new NullReferenceException(
                    "DbEventLog.Init(DataBaseName==null). Use DbEventLog(strind databaseName) Constructor");

            EventLog = Builder.Build2<IEventLog>(@"Init\EventLog.xml", "EventLogs");
            EventLog.Init();

            ReceiverDto = new Receiver<EventLogItemDto> {Parent = this};
            ReceiverDto.Init();

            if (WorkTask3 == null)
            {
                WorkTask3 = new WorkTask3
                {
                    Code = FullCode + ".WorkTask",
                    TimeInterval = 60000
                };
            }
            WorkTask3.Init();
            WorkTask3.Parent = this;
            WorkTask3.Works.Register(ReceiverDto.Work);

            //using (var db = GetEvlContext())
            //{
            //    DbEvl = CreateDbEventLog(db);
            //}
            //if (DbEvl == null)
            //    throw new NullReferenceException("DbEventLog == null");
        }

        public void Start()
        {
            ReceiverDto.NewItemEvent -= NewEventLogItemToAdd;
            ReceiverDto.NewItemEvent += NewEventLogItemToAdd;

            // ConsoleAsync.WriteLineT(FullCode + " Start()");

            SSStatus.Status = SSStatusEnum.Starting;

            Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, FullCode, Code,
                                   MethodBase.GetCurrentMethod()?.Name, SSStatus.Status.ToString(), "");

            WorkTask3?.Start();
        }
        public void Stop()
        {
            IsEnabled = false;
            SSStatus.Status = SSStatusEnum.Stopping;

            Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, FullCode, Code,
                                    MethodBase.GetCurrentMethod().Name, SSStatus.Status.ToString(), "");

            ReceiverDto.NewItemEvent -= NewEventLogItemToAdd;
            ReceiverDto.IsEnabled = false;
            
            if (WorkTask3 != null)
            {
                //var t = WorkTask3.Task;
                WorkTask3.Stop();

                //    t.Wait(); // only for active task
                // Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, FullCode, "WorkTask: " + WorkTask3.Code, "Stop()","","");
            }

            // SSStatus.Status = SSStatusEnum.Stopping;
        }

        private void NewEventLogItemToAdd(object sender, EventLogItemDto dto)
        {
            
            //AddItem(dto);
            Console.WriteLine((IsEnabled ? "Item to Add: " : "Item Rejected: ") + dto.ToString());
            if (IsEnabled)
            {
                AddItem1(dto);
            }
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
        /// <summary>
        /// Main Entry after Queue.Dequeue
        /// </summary>
        /// <param name="dto"></param>
        private void AddItem1(EventLogItemDto dto)
        {
            using (var db = GetEvlContext())
            {
                try
                {
                    //if(++_addsCrnt > _addsMax)
                    //    throw new NullReferenceException("Exception in AddItem");

                    DataBase1.Model.DbEventLog evl;
                    if (dto.EventLog.HasNoValue())
                        evl = db.GetUnknown();
                    else
                        evl = db.RegisterEvl("app", dto.EventLog) ?? db.GetUnknown();

                    if (evl != null && evl.EventLogID >= 0)
                    {
                        dto.EventLogID = evl.EventLogID;
                        db.Add(CreateDbEventLogItem(dto));

                        IsEnabled = true;
                        ErrorCntCurrent = 0;
                    }
                    else
                    {
                        IsEnabled = false;

                        SendExceptionMessage3(FullCode, Code,
                            System.Reflection.MethodBase.GetCurrentMethod().Name, dto.ToString(),
                            new NullReferenceException("Can't Find and Create DbEventLog"));

                        StartRepairTask(dto);
                    }
                }
                catch (Exception ex)
                {
                    IsEnabled = false;

                    if(db!=null)
                        db.Dispose();

                    SendExceptionMessage3(FullCode, Code,
                        System.Reflection.MethodBase.GetCurrentMethod().Name, dto.ToString(),
                        ex);

                    StartRepairTask(dto);
                }
                //finally
                //{
                //    if(db!=null)
                //        db.Dispose();
                //}
            }
        }
        private void AddItem2(EventLogItemDto dto)
        {
            try
            {
                using (var db = GetEvlContext())
                {
                    DataBase1.Model.DbEventLog evl;
                    if (dto.EventLog.HasNoValue())
                        evl = db.GetUnknown();
                    else
                        evl = db.RegisterEvl("app", dto.EventLog) ?? db.GetUnknown();

                    if (evl != null && evl.EventLogID >= 0)
                    {
                        dto.EventLogID = evl.EventLogID;
                        db.Add(CreateDbEventLogItem(dto));

                        IsEnabled = true;
                        ErrorCntCurrent = 0;
                    }
                    else
                    {
                        IsEnabled = false;

                        SendExceptionMessage3(FullCode, Code,
                            System.Reflection.MethodBase.GetCurrentMethod().Name, dto.ToString(),
                            new Exception("Can't Find and Create DbEventLog"));

                        StartRepairTask(dto);
                    }
                }
            }
            catch (Exception ex)
            {
                IsEnabled = false;
                SendExceptionMessage3(FullCode, Code,
                    System.Reflection.MethodBase.GetCurrentMethod().Name, dto.ToString(),
                    ex);

                StartRepairTask(dto);
            }
        }

        private void StartRepairTask(EventLogItemDto evli)
        {
            if (++ErrorCntCurrent > ErrorCntToStop)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, FullCode, Code, "Try to Stop",
                                            "Error Limit: " + ErrorCntToStop + " is Reached","");
                IsEnabled = false;
                // Stop();
                return;
            }
            Task.Factory.StartNew((i) =>
            {
                var ei = (EventLogItemDto)i;
                //Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, FullCode, "AddItem1()", "Start Repair Task",
                //                                String.Format("Try to Add Item Again Within {0} Seconds", ErrorTimeOut), 
                //                                "Error Count is : " + ErrorCntCurrent);
                Thread.Sleep(TimeSpan.FromSeconds(ErrorTimeOut));
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, FullCode, "AddItem1()", "Repair Task has been Started. TimeOut is: " + ErrorTimeOut + "Seconds",
                                                "Try to Add Item Again. Error Count is: " + ErrorCntCurrent, ei.ToString());

                AddItem1(ei);

            }, evli);
        }

        public int ErrorTimeOut => ErrorRecoveryTimeOut > 0 ? ErrorRecoveryTimeOut : 60;

        //private DataBase1.Model.DbEventLog GetEventLogByCode(EventLogItemDto dto)
        //{
        //    using (var db = GetEvlContext())
        //    {
        //        if (!dto.EventLog.HasValue()) 
        //                    return db.GetUnknown();
        //        var evl = db.GetEventLog("app", dto.EventLog);
        //        return evl ?? db.GetUnknown();
        //    }
        //}

        private void AddItem(DbEventLogItem i)
        {
            IsEnabled = false;
            using (var db = GetEvlContext())
            {
                try
                {
                    if (i.EventLogID == 0 || ! db.IsEvlExist(i.EventLogID))
                    {
                        var evl  = db.GetUnknown();
                        if (evl == null)
                        {
                            SendExceptionMessage3(FullCode, i.GetType().ToString(),
                                System.Reflection.MethodBase.GetCurrentMethod().Name, i.ToString(),
                                new NullReferenceException("Can't Create Unknown DbEventLog"));
                            return;
                        }
                        i.EventLogID = evl.EventLogID;
                    }
                    db.Add(i);
                    // Only if Success Enable this EventLog
                    IsEnabled = true;
                }
                catch (System.ComponentModel.Win32Exception e)
                {
                    SendExceptionMessage3("DbEventLogs", i.GetType().ToString(),
                        "AddItem(evli.AddItem(evi))", i.ToString(), e);
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    SendExceptionMessage3("DbEventLogs", i.GetType().ToString(),
                        "AddItem(evli.AddItem(evi))", i.ToString(), e);
                }
                catch (System.Data.Entity.Core.EntityException e)
                {
                    SendExceptionMessage3("DbEventLogs", i.GetType().ToString(),
                        "AddItem(evli.AddItem(evi))", i.ToString(), e);
                }
                catch (NullReferenceException e)
                {
                    SendExceptionMessage3("DbEventLogs", i.GetType().ToString(),
                        "AddItem(evli.AddItem(evi))", i.ToString(), e);
                }
                catch (Exception e)
                {
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
                    Alias = Alias,
                    Code = Code,
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
                : new EvlContext1("Dvlp14.EventLog1");
                // : new EvlContext1("Expr14.EventLog1");
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
