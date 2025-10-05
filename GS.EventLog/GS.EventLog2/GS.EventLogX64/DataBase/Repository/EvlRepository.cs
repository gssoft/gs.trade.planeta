using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using GS.Elements;
using GS.EventLog.DataBase.Dal;
using EvlContext = GS.EventLog.DataBase.Dal.EvlContext;


namespace GS.EventLog.DataBase.Repository
{
    public class EvlRepository : Element1<string>
    {
        protected int Count = 1500;
        public string DbName { get; set; }
        public int TimeOut { get; set; }

        //  private EvlContext _db;
        private readonly object _dbLocker = new object();
        public override string Key
        {
            get { return FullName; }
        }
        public IEnumerable<Model.DbEventLogItem> GetItems()
        {
            using (var db = new EvlContext(DbName))
            {
                return db.EventLogItems.ToList();
            }
        }
        public IEnumerable<Model.DbEventLogItem> GetItems(Model.DbEventLog evl)
        {
            //var ll = new List<Model.DbEventLogItem>();
            using (var db = new EvlContext(DbName))
            {
                return db.EventLogItems.Where(i => i.EventLogID == evl.EventLogID).ToList();
            }
        }


        public IEnumerable<Model.DbEventLogItem> GetItems(Func<Model.DbEventLogItem, bool> func)
        {
            //var ll = new List<Model.DbEventLogItem>();
            using (var db = new EvlContext(DbName))
            {
                return db.EventLogItems.Where(func).ToList();
            }
           // return ll;
        }

        public IEnumerable<Model.DbEventLog> GetEventLogs()
        {
            using (var db = new EvlContext(DbName))
            {
                return db.EventLogs.ToList();
            }
        }

        public int Add(Model.DbEventLogItem it)
        {
            try
            {
                int ret;
                lock (_dbLocker)
                {
                    using (var db = new EvlContext(DbName))
                    {
                        //if (db.Database.Connection.State == ConnectionState.Broken)
                        //{
                        //    db.Database.Connection.Close();
                        //    db.Database.Connection.Open();
                        //}
                        // ((System.Data.Entity.Infrastructure.IObjectContextAdapter)db).ObjectContext.CommandTimeout = TimeOut;
                        db.Database.CommandTimeout = TimeOut;

                        db.EventLogItems.Add(it);
                        ret = db.SaveChanges();

                        
                        //db.Dispose();
                        //if (db.Database.Connection.State != ConnectionState.Closed)
                        //db.Database.Connection.Close();
                    }
                    //if (--Count < 0)
                    //{
                    //    Count = 1500;
                    //    throw new NullReferenceException("Something Wrong with Element1 !!!!");
                    //    //throw new System.ComponentModel.Win32Exception("Something Wrong with Win32Exception !!!!");
                    //}
                }
                return ret;
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                SendExceptionMessage3("DbEventLogs", it.GetType().ToString(),
                    "AddItem(evli.AddItem(evi))", it.ToString(), e);
                return -1;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                SendExceptionMessage3("DbEventLogs", it.GetType().ToString(),
                    "AddItem(evli.AddItem(evi))", it.ToString(), e);
                return -1;
            }
            catch (System.Data.Entity.Core.EntityException e)
            {
                SendExceptionMessage3("DbEventLogs", it.GetType().ToString(),
                    "AddItem(evli.AddItem(evi))", it.ToString(), e);
                return -1;
            }
            catch (NullReferenceException e)
            {
                SendExceptionMessage3("DbEventLogs", it.GetType().ToString(),
                    "AddItem(evli.AddItem(evi))", it.ToString(), e);
                return -1;
            }
            catch (Exception e)
            {
                SendExceptionMessage3("DbEventLogs", it.GetType().ToString(),
                    "AddItem(evli.AddItem(evi))", it.ToString(), e);
                throw;
            }     
        }
        public int Add(Model.DbEventLog evl)
        {
            int ret;
            using (var db = new EvlContext(DbName))
            {
                db.EventLogs.Add(evl);
                ret = db.SaveChanges();
            }
            return ret;
        }
        //public int AddNew(Model.DbEventLog evl)
        //{
        //    var ret = 0;
        //    using (var db = new EvlContext())
        //    {
        //        var ev = FindByKey(evl.Key);
        //        if (ev == null)
        //        {
        //            db.EventLogs.Add(evl);
        //            ret = db.SaveChanges();
        //        }
        //    }
        //    return ret;
        //}
        public Model.DbEventLog AddNew(Model.DbEventLog evl)
        {
            //Model.DbEventLog ev;
            lock (_dbLocker)
            {
                using (var db = new EvlContext(DbName))
                {
                    try
                    {
                        var ev = db.EventLogs.FirstOrDefault(i => i.Code == evl.Code);
                        if (ev != null) return ev;

                        db.EventLogs.Add(evl);
                        db.SaveChanges();

                        //ev = db.EventLogs.FirstOrDefault(i => i.Code == evl.Code);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("DbEventLog.Add Error: " + e.Message);
                        throw;
                    }
                    
                }
                return evl;
            }
        }

        public Model.DbEventLog Find(Model.DbEventLog evl)
        {
            using (var db = new EvlContext(DbName))
            {
                return db.EventLogs.FirstOrDefault(i => i.Code == evl.Code);
            }
        }
        public Model.DbEventLog FindByKey(string key)
        {
            using (var db = new EvlContext(DbName))
            {
                return db.EventLogs.FirstOrDefault(i => i.Code == key);
            }
        }

        public long EventLogItemsCount(Model.DbEventLog evl)
        {
            using (var db = new EvlContext(DbName))
            {
                return db.EventLogItems.Count(i => i.EventLogID == evl.EventLogID);
            }
        }

        public long EventLogCount()
        {
            using (var db = new EvlContext(DbName))
            {
                return db.EventLogs.Count();
            }
        }

        public int Commit()
        {
            lock( _dbLocker)
            {
                
            }
            return 1;
        }
    }
}
