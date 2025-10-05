using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;
using GS.Trade.DataBase.Dal;
using GS.Trade.Queues;
using GS.Trade.Storage2;

namespace GS.Trade.DataBase.Storage
{
    //********************* 32 ***********************************

    public abstract class TradeBaseRepository32<TKey, TValueExt, TValueInt, TValueIntOut> :
                            Element1<string>,
                            ITradeBaseRepository3<TKey, TValueExt, TValueIntOut>, INeedDataBaseName
        where TValueExt : class, IHaveKey<TKey> //, IHaveId<TId>
        where TValueInt : class, IHaveKey<TKey>, TValueIntOut //, IHaveId<TId>
        where TValueIntOut : class, IHaveKey<TKey> //, IHaveId<TId>
    {
        private readonly object _locker;
        private readonly object _locker2;
        protected TradeBaseRepository32()
        {
            _locker = new object();
            _locker2 = new object();

            TradeEntityQueue = new TradeEntityQueue<TValueExt>();
        }

        public string DataBaseName { get; set; }

        public bool IsUIEnabled { get; set; }

        //public bool UIEnabled { get; set; }
        public bool IsQueueEnabled { get; set; }

        protected TradeEntityQueue<TValueExt> TradeEntityQueue;

        //protected DbTradeContext DbTradeContext { get; set; }
        //protected IEFRepository<TKey, TValueInt> Repository { get; set; }
        //protected DbSet<TValueInt> DbSet { get; set; }

        protected abstract TValueInt GetByKey(DbTradeContext cntx, TKey key);
        protected abstract TValueInt Get(DbTradeContext cntx, TKey anyString);
        protected abstract bool AddVal(DbTradeContext cntx, TValueExt v);
        protected abstract TValueExt Update(DbTradeContext cntx, TValueExt ve, TValueInt vi); // Update ExternalValue with Internal
        protected abstract bool Update(DbTradeContext cntx, TValueInt vi, TValueExt ve); // Update InternalValue with External
     
        public TValueIntOut GetByKey(TKey key)
        {
            lock (_locker2)
            {
                using (var cntx = new DbTradeContext(DataBaseName))
                {
                    return GetByKey(cntx, key);
                }
            }
        }
        public TValueIntOut Get(TKey key)
        {
            lock (_locker2)
            {
                using (var cntx = new DbTradeContext(DataBaseName))
                {
                    return Get(cntx, key);
                }
            }
        }

        public bool Add(TValueExt ve)
        {
            try
            {
                if (ve == null)
                    throw new ArgumentNullException("ve", FullName + " TradeBaseRepository32.Add()");

                try
                {
                    bool ret;
                    lock (_locker)
                    {
                        using (var cntx = new DbTradeContext(DataBaseName))
                        {
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
                catch (System.Data.Entity.Core.EntityException e)
                {
                    SendExceptionMessage3(FullName, ve.GetType().ToString(), "Add: " + ve.GetType(), ve.ToString(), e);
                    return false;
                }               
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, ve == null ? "Entity" : ve.GetType().ToString(), "TradeBaseRepository32.Add()",
                    ve == null ? "Entity" : ve.ToString(), e);
                throw;
            }
        }

        public bool AddNew(TValueExt ve)
        {
            try
            {
                if (ve == null)
                    throw new ArgumentNullException("ve", FullName + " TradeBaseRepository32.AddNew()");
                try
                {
                    bool ret;
                    lock (_locker)
                    {
                        using (var cntx = new DbTradeContext(DataBaseName))
                        {
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
                catch (System.Data.Entity.Core.EntityException e)
                {
                    SendExceptionMessage3(FullName, ve.GetType().ToString(), "TradeBaseRepository32.AddNew: " + ve.GetType(), ve.ToString(), e);
                    return false;
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, ve == null ? "Entity" : ve.GetType().ToString(), "AddNew()",
                    ve == null ? "Entity" : ve.ToString(), e);
                throw;
            }
        }

        public bool AddOrUpdate(TValueExt ve)
        {
            try
            {
                if (ve == null)
                    throw new ArgumentNullException("ve", FullName + " TradeBaseRepository32.AddOrUpdate()");
                try
                {
                    bool ret;
                    lock (_locker)
                    {
                        using (var cntx = new DbTradeContext(DataBaseName))
                        {
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
                catch (System.Data.Entity.Core.EntityException e)
                {
                    SendExceptionMessage3(FullName, ve.GetType().ToString(), "AddOrUpdate: " + ve.GetType(), ve.ToString(), e);
                    return false;
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, ve == null ? "Entity" : ve.GetType().ToString(), "AddOrUpdate()",
                    ve == null ? "Entity" : ve.ToString(), e);
                throw;
            }
        }

        public TValueExt AddOrGet(TValueExt ve)
        {
            try
            {
                if (ve == null)
                    throw new ArgumentNullException("ve", FullName + " TradeBaseRepository32.AddOrGet()");
                try
                {
                    TValueExt ret = ve;
                    lock (_locker)
                    {
                        using (var cntx = new DbTradeContext(DataBaseName))
                        {
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
                                "AddOrGet: " + ve.GetType() + " " + ve.Key.ToString().WithSqBrackets0(),
                                ve.ToString());
                    return ret;
                }
                catch (System.Data.Entity.Core.EntityException e)
                {
                    SendExceptionMessage3(FullName, ve.GetType().ToString(), "AddOrGet: " + ve.GetType(), ve.ToString(), e);
                    return ve;
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, ve == null ? "Entity" : ve.GetType().ToString(), "AddOrGet()",
                    ve == null ? "Entity" : ve.ToString(), e);
                throw;
            }
        }

        public TValueExt Register(TValueExt v)
        {
            return AddOrGet(v);
        }

        public bool Update(TValueExt ve)
        {
            try
            {
                if (ve == null)
                    throw new ArgumentNullException("ve",  FullName + " TradeBaseRepository32.Update()");
                try
                {
                    bool ret;
                    lock (_locker)
                    {
                        using (var cntx = new DbTradeContext(DataBaseName))
                        {
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
                catch (System.Data.Entity.Core.EntityException e)
                {
                    SendExceptionMessage3(FullName, ve.GetType().ToString(), "Update: " + ve.GetType(), ve.ToString(), e);
                    return false;
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, 
                                        ve == null ? "Entity" : ve.GetType().ToString(), "Update()",
                                        ve == null ? "Entity" : ve.ToString(), e);
                throw;
            }
        }

        public void Push(TradeQueueEntity<TValueExt> tqe)
        {
            lock (_locker)
            {
                TradeEntityQueue.Push(tqe);
            }
            Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName, tqe.Entity.GetType().ToString(),
                                    "Push: " + tqe.Operation + " " + tqe.Entity.GetType(),
                                    "Push: " + tqe.GetType(), tqe.ToString());
        }

        public void DeQueueProcess()
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
                throw;
            }
        }
    }
}
