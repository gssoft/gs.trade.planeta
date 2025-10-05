using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.Elements;
using GS.Events;
using GS.Interfaces;
using GS.Trade.DataBase.Dal;
using GS.Trade.Queues;
using GS.Trade.Storage2;
using GS.Extension;

namespace GS.Trade.DataBase.Storage
{
    // Next Version TradeBaseRepository with Element3 inheritance
    public abstract class TradeBaseRepository35<TContext, TKey, TValueExt, TValueInt, TValueIntOut> :
                         Element3<string, IEventArgs>,
                         ITradeBaseRepository3<TKey, TValueExt, TValueIntOut>, INeedDataBaseName
        where TValueExt : class, IHaveKey<TKey> //, IHaveId<TId>
        where TValueInt : class, IHaveKey<TKey>, TValueIntOut //, IHaveId<TId>
        where TValueIntOut : class, IHaveKey<TKey> //, IHaveId<TId>
        where TContext : IDisposable
    {
        private readonly object _locker;
        private readonly object _locker2;
        protected TradeBaseRepository35()
        {
            _locker = new object();
            _locker2 = new object();

            TradeEntityQueue = new TradeEntityQueue<TValueExt>();
        }

        public string DataBaseName { get; set; }
        public int TimeOut { get; set; }

        public bool IsUIEnabled { get; set; }

        //public bool IsQueueEnabled { get; set; }

        protected TradeEntityQueue<TValueExt> TradeEntityQueue;

        protected abstract TValueInt GetByKey(TContext cntx, TKey key);
        protected abstract TValueInt Get(TContext cntx, TKey anyString);
        protected abstract bool AddVal(TContext cntx, TValueExt v);
        protected abstract TValueExt Update(TContext cntx, TValueExt ve, TValueInt vi); // Update ExternalValue with Internal
        protected abstract bool Update(TContext cntx, TValueInt vi, TValueExt ve); // Update InternalValue with External
        protected abstract TContext GetContext(string dataBaseName);

        public TValueIntOut GetByKey(TKey key)
        {
            lock (_locker2)
            {
                using (var cntx = GetContext(DataBaseName))
                {
                    ((System.Data.Entity.Infrastructure.IObjectContextAdapter)cntx).ObjectContext.CommandTimeout = TimeOut;
                    return GetByKey(cntx, key);
                }
            }
        }
        public TValueIntOut Get(TKey key)
        {
            lock (_locker2)
            {
                using (var cntx = GetContext(DataBaseName))
                {
                    ((System.Data.Entity.Infrastructure.IObjectContextAdapter)cntx).ObjectContext.CommandTimeout = TimeOut;
                    return Get(cntx, key);
                }
            }
        }

        public bool Add(TValueExt ve)
        {
            const string method = "Add()";
            try
            {
                if (ve == null)
                {
                    //throw new ArgumentNullException("ve", FullName + " Add()");
                    var e = new ArgumentNullException("ve", FullName + " Add()");
                    SendException(null, method, e);
                    return false;
                }
                bool ret;
                lock (_locker)
                {
                    using (var cntx = GetContext(DataBaseName))
                    {
                        ((System.Data.Entity.Infrastructure.IObjectContextAdapter)cntx).ObjectContext.CommandTimeout = TimeOut;
                        ret = AddVal(cntx, ve);
                    }
                }
                Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                    FullName, ve.GetType().ToString(),
                    "Add: " + ve.GetType(),
                    "Add: " + ve.GetType() + " " + ve.Key.ToString().WithSqBrackets0(),
                    ve.ToString());
                return ret;
            }
            //catch (System.Data.UpdateException e)
            //{
            //    SendException(ve, method, e);
            //    return false;
            //}
            catch (System.Data.Entity.Infrastructure.DbUpdateException e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (System.Data.Entity.Core.EntityException e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (Exception e)
            {
                SendException(ve, method, e);
                return false;
                // throw;
            }
        }
        public bool Add(IEnumerable<TValueExt> ave)
        {
            const string method = "Add()";
            TValueExt ve = default(TValueExt);
            try
            {
                if (ave == null)
                {
                    //throw new ArgumentNullException("ave", FullName + " Add(IEnumerable<TValueExt>)");
                    var e = new ArgumentNullException("ave", FullName + " Add(IEnumerable<TValueExt>)");
                    SendException(null, method, e);
                    return false;
                }
                lock (_locker)
                {
                    using (var cntx = GetContext(DataBaseName))
                    {
                        ((System.Data.Entity.Infrastructure.IObjectContextAdapter)cntx).ObjectContext.CommandTimeout = TimeOut;
                        foreach (var v in ave)
                        {
                            ve = v;
                            if (ve == null)
                            {
                                //throw new ArgumentNullException(FullName + " AddVal(ve==Null in IEnumerable<TValueExt>)");
                                var e = new ArgumentNullException("ave", FullName + " AddVal(ve==Null in IEnumerable<TValueExt>)");
                                SendException(null, method, e);
                                continue;
                            }
                            AddVal(cntx, v);
                        }
                    }
                }
                //Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                //    FullName, ave.GetType().ToString(),
                //    "Add: " + ave.GetType(),
                //    "Add: " + ave.GetType() + " " + ave.Key.ToString().WithSqBrackets0(),
                //    ave.ToString());
                return true;
            }
            //catch (System.Data.UpdateException e)
            //{
            //    SendException(ve, method, e);
            //    return false;
            //}
            catch (System.Data.Entity.Infrastructure.DbUpdateException e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (System.Data.Entity.Core.EntityException e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (Exception e)
            {
                SendException(ve, method, e);
                return false;
                //throw;
            }
        }

        public bool AddNew(TValueExt ve)
        {
            const string method = "AddNew()";
            try
            {
                if (ve == null)
                {
                    //throw new ArgumentNullException("ve", FullName + " AddNew()");
                    var e = new ArgumentNullException("ve", FullName + " AddNew()");
                    SendException(null, method, e);
                    return false;
                }
                bool ret;
                lock (_locker)
                {
                    using (var cntx = GetContext(DataBaseName))
                    {
                        ((System.Data.Entity.Infrastructure.IObjectContextAdapter)cntx).ObjectContext.CommandTimeout = TimeOut;
                        var val = GetByKey(cntx, ve.Key);
                        ret = val == null && AddVal(cntx, ve);
                    }
                }
                Evlm1(ret ? EvlResult.SUCCESS : EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                            FullName, ve.GetType().ToString(),
                            "AddNew: " + ve.GetType(),
                            "AddNew: " + ve.GetType() + " " + ve.Key.ToString().WithSqBrackets0(),
                            ve.ToString());
                return ret;
            }
            //catch (System.Data.UpdateException e)
            //{
            //    SendException(ve, method, e);
            //    return false;
            //}
            catch (System.Data.Entity.Infrastructure.DbUpdateException e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (System.Data.Entity.Core.EntityException e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (Exception e)
            {
                SendException(ve, method, e);
                return false;
                // throw;
            }
        }
        public bool AddOrUpdate(TValueExt ve)
        {
            const string method = "AddOrUpdate()";
            try
            {
                if (ve == null)
                {
                    //throw new ArgumentNullException("ve", FullName + " AddOrUpdate()");
                    var e = new ArgumentNullException("ve", FullName + " AddOrUpdate()");
                    SendException(null, method, e);
                    return false;
                }
                bool ret;
                lock (_locker)
                {
                    using (var cntx = GetContext(DataBaseName))
                    {
                        ((System.Data.Entity.Infrastructure.IObjectContextAdapter)cntx).ObjectContext.CommandTimeout = TimeOut;
                        var vi = GetByKey(cntx, ve.Key);
                        ret = vi == null ? AddVal(cntx, ve) : Update(cntx, vi, ve);
                    }
                }
                Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                            FullName, ve.GetType().ToString(),
                            "AddOrUpdate: " + ve.GetType(),
                            "AddOrUpdate: " + ve.GetType() + " " + ve.Key.ToString().WithSqBrackets0(),
                            ve.ToString());
                return ret;
            }
            //catch (System.Data.UpdateException e)
            //{
            //    SendException(ve, method, e);
            //    return false;
            //}
            catch (System.Data.Entity.Infrastructure.DbUpdateException e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (System.Data.Entity.Core.EntityException e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (Exception e)
            {
                SendException(ve, method, e);
                return false;
                // throw;
            }          
        }
        public TValueExt AddOrGet(TValueExt ve)
        {
            const string method = "AddOrGet()";
            try
            {
                if (ve == null)
                {
                    //throw new ArgumentNullException("ve", FullName + " AddOrGet()");
                    var e = new ArgumentNullException("ve", FullName + " AddOrGet()");
                    SendException(null, method, e);
                    return null;
                }
                TValueExt ret = ve;
                lock (_locker)
                {
                    using (var cntx = GetContext(DataBaseName))
                    {
                        ((System.Data.Entity.Infrastructure.IObjectContextAdapter)cntx).ObjectContext.CommandTimeout = TimeOut;
                        var val = GetByKey(cntx, ve.Key);

                        if (val != null)
                        {
                            ret = Update(cntx, ve, val); // Update ExternalValue with Internal
                        }
                        else
                            AddVal(cntx, ve);
                    }
                }
                Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                            FullName, ve.GetType().ToString(),
                            "AddOrGet: " + ve.GetType(),
                            //"AddOrGet: " + ve.GetType() + " " + ve.Key.ToString().WithSqBrackets0(),
                            ve.ToString(),
                            "Key:" + ve.Key.ToString().WithSqBrackets0()
                            );
                return ret;
            }
            //catch (System.Data.UpdateException e)
            //{
            //    SendException(ve, method, e);
            //    return null;
            //}
            catch (System.Data.Entity.Infrastructure.DbUpdateException e)
            {
                SendException(ve, method, e);
                return null;
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                SendException(ve, method, e);
                return null;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                SendException(ve, method, e);
                return null;
            }
            catch (System.Data.Entity.Core.EntityException e)
            {
                SendException(ve, method, e);
                return null;
            }
            catch (Exception e)
            {
                SendException(ve, method, e);
                return null;
                //throw;
            }
        }

        public TValueExt Register(TValueExt v)
        {
            return AddOrGet(v);
        }
        public bool Update(TValueExt ve)
        {
            const string method = "Update()";
            try
            {
                if (ve == null)
                {
                    //throw new ArgumentNullException("ve", FullName + " Update()");
                    var e = new ArgumentNullException("ve", FullName + " Update()");
                    SendException(null, method, e);
                    return false;
                }
                bool ret;
                lock (_locker)
                {
                    using (var cntx = GetContext(DataBaseName))
                    {
                        ((System.Data.Entity.Infrastructure.IObjectContextAdapter)cntx).ObjectContext.CommandTimeout = TimeOut;
                        var vi = GetByKey(cntx, ve.Key);
                        ret = vi != null && Update(cntx, vi, ve);
                    }
                }
                Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                                FullName, ve.GetType().ToString(),
                            "Update: " + ve.GetType(),
                            "Update: " + ve.GetType() + " " + ve.Key.ToString().WithSqBrackets0(),
                            ve.ToString());
                return ret;
            }
            //catch (System.Data.UpdateException e)
            //{
            //    SendException(ve, method, e);
            //    return false;
            //}
            catch (System.Data.Entity.Infrastructure.DbUpdateException e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (System.Data.Entity.Core.EntityException e)
            {
                SendException(ve, method, e);
                return false;
            }
            catch (Exception e)
            {
                SendException(ve, method, e);
                return false;
                //throw;
            }          
        }

        public void Push(TradeQueueEntity<TValueExt> tqe)
        {
            lock (_locker)
            {
                TradeEntityQueue.Push(tqe);
            }
            //Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName, tqe.Entity.GetType().ToString(),
            //                        "Push: " + tqe.Operation + " " + tqe.Entity.GetType(),
            //                        "Push: " + tqe.GetType(), tqe.ToString());
        }

        //public void DeQueueProcess()
        //{
        //    if (TradeEntityQueue.IsEmpty)
        //        return;

        //    //Evlm(EvlResult.WARNING, EvlSubject.TECHNOLOGY, FullName, "Repository", "DeQueueProcess", "Try to get Entities to Perform Operation","");

        //    var items = TradeEntityQueue.GetItems();

        //    foreach (var i in items)
        //    {
        //        EntityFromQueueToProcess(i);
        //    }
        //}
        //public override void EnQueue(object sender, IEventArgs queueItem)
        //{
        //    var tqe = new TradeQueueEntity<IBar>(StorageOperationEnum.AddNew, (IBar)(queueItem.Object));
        //    TradeEntityQueue.Push(tqe);

        //}
        public override void DeQueueProcess()
        {
            if (TradeEntityQueue.IsEmpty)
                return;

            //Evlm(EvlResult.WARNING, EvlSubject.TECHNOLOGY, FullName, "Repository", "DeQueueProcess", "Try to get Entities to Perform Operation","");

            var items = TradeEntityQueue.GetItems();

            foreach (var i in items)
            {
                EntityFromQueueToProcess(i);
            }
        }

        public void EntityFromQueueToProcess(TradeQueueEntity<TValueExt> tqe)
        {
            try
            {
                switch (tqe.Operation)
                {
                    case StorageOperationEnum.AddOrUpdate:
                        AddOrUpdate(tqe.Entity);
                        break;
                    case StorageOperationEnum.Update:
                        Update(tqe.Entity);
                        break;
                    case StorageOperationEnum.AddOrGet:
                    case StorageOperationEnum.Register:
                        AddOrGet(tqe.Entity);
                        break;
                    case StorageOperationEnum.AddNew:
                        AddNew(tqe.Entity);
                        break;
                    case StorageOperationEnum.Add:
                        Add(tqe.Entity);
                        break;
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, tqe.GetType().ToString(), "EntityFromQueueToProcess.", tqe.ToString(), e);
                // throw;
            }
        }

        protected void SendException(TValueExt ve, string method, Exception e)
        {
            var type = ve == null ? "Entity" : ve.GetType().ToString();
            var entity = ve == null ? "Entity" : ve.ToString();
            SendExceptionMessage3(FullName, type, method, entity, e);
        }
    }
}
