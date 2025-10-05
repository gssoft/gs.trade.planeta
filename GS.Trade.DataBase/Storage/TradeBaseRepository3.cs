using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using GS.Containers5;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;
using GS.Queues;
using GS.Trade.DataBase.Dal;
using GS.Trade.DataBase.EFRepositary;
using GS.Trade.Queues;
using GS.Trade.Storage2;

namespace GS.Trade.DataBase.Storage
{
    //public interface ITradeBaseRepository3<in TKey, TValue, out TValue2> : IHaveQueue<TradeQueueEntity<TValue>>
    //    where TValue : class, IHaveKey<TKey>
    //{
    //    TValue2 GetByKey(TKey key);

    //    bool Add(TValue v);
    //    bool AddNew(TValue v);
    //    bool AddOrUpdate(TValue v);

    //    TValue AddOrGet(TValue item);
    //    TValue Register(TValue item);

    //    bool Update(TValue v);

    //}

    public abstract class TradeBaseRepository3<TKey, TValueExt, TValueInt, TValueIntOut> : Element1<string>,
        ITradeBaseRepository3<TKey, TValueExt, TValueIntOut>, INeedDataBaseName
        where TValueExt : class, IHaveKey<TKey> //, IHaveId<TId>
        where TValueInt : class, IHaveKey<TKey>, TValueIntOut //, IHaveId<TId>
        where TValueIntOut : class, IHaveKey<TKey> //, IHaveId<TId>
    {
        private readonly object _locker;
        protected TradeBaseRepository3()
        {
            _locker = new object();
            TradeEntityQueue = new TradeEntityQueue<TValueExt>();
        }

        public string DataBaseName { get; set; }

        public bool IsUIEnabled { get; set; }
        public bool UIEnabled { get; set; }
        public bool EvlEnabled { get; set; }
        public bool IsQueueEnabled { get; set; }

        protected TradeEntityQueue<TValueExt> TradeEntityQueue;

        protected DbTradeContext DbTradeContext { get; set; }
        protected IEFRepository<TKey, TValueInt> Repository { get; set; }

        protected abstract bool AddVal(TValueExt v);
        protected abstract TValueExt Update(TValueExt ve, TValueInt vi); // Update ExternalValue with Internal
        protected abstract bool Update(TValueInt vi, TValueExt ve); // Update InternalValue with External

        public TValueIntOut GetByKey(TKey key)
        {
            return Repository.GetByKey(key);
        }

        public TValueIntOut Get(TKey anyString)
        {
            throw new NotImplementedException();
        }

        //public TValueExt GetByKey(TKey key)
        //{
        //    return Repository.GetByKey(key) as TValueExt;
        //}

        public bool Add(TValueExt ve)
        {
            return AddVal(ve);
        }

        public bool AddNew(TValueExt ve)
        {
            try
            {
                if (ve == null)
                    throw new ArgumentNullException("ve");

                var val = Repository.GetByKey(ve.Key);
                return val == null && AddVal(ve);
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
                    throw new ArgumentNullException("ve");

                var vi = Repository.GetByKey(ve.Key);
                return vi == null ? AddVal(ve) : Update(vi, ve);
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
                    throw new ArgumentNullException("ve");

                var val = Repository.GetByKey(ve.Key);

                if (val != null)
                    return Update(ve, val); // Update ExternalValue with Internal
                AddVal(ve);
                return ve;
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
                    throw new ArgumentNullException("ve");

                var vi = Repository.GetByKey(ve.Key);
                return vi != null && Update(vi, ve);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "Update", "Update()", "", e);
                throw;
            }
        }
        public void Push(TradeQueueEntity<TValueExt> tqe)
        {
            Evlm(EvlResult.WARNING, EvlSubject.TECHNOLOGY, FullName, tqe.Entity.GetType().ToString(),
                                    "Push to Queue: " + tqe.Operation + " " + tqe.Entity.GetType(),
                                    "Push " + tqe.GetType(), tqe.ToString());
            TradeEntityQueue.Push(tqe);
        }

        public void DeQueueProcess()
        {
            if (TradeEntityQueue.IsEmpty)
                return;
            
            //Evlm(EvlResult.WARNING, EvlSubject.TECHNOLOGY, FullName, "Repository", "DeQueueProcess", "Try to get Entities to Perform Operation","");
            
            var items = TradeEntityQueue.GetItems();
            lock (_locker)
            {
                foreach (var i in items)
                {
                    EntityFromQueueToProcess(i);
                }
            }
        }

        public void EntityFromQueueToProcess(TradeQueueEntity<TValueExt> tqe )
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
