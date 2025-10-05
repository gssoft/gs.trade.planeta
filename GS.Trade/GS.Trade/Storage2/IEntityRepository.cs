using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.Elements;
using GS.Events;
using GS.Interfaces;
using GS.Queues;
using GS.Trade.Queues;
using GS.Works;

namespace GS.Trade.Storage2
{
    public interface IEntityRepositoryBase
    {
        string Name { get; }
        string Code { get; }

        bool IsEnabled { get; }
        bool IsUIEnabled { get; }
        //bool UIEnabled { get; }
        bool IsEvlEnabled { get; }

        bool IsQueueEnabled { get; }
        void DeQueueProcess();

        IElement1<string> Parent { get; set; }
    }

    public interface IEntityRepository<out TIntValue, TExtValue> : IEntityRepositoryBase
                            //IElement1<string>,
                            //, IHaveQueue<TradeQueueEntity<TExtValue>>   //:
        
    {
        void Init(IEventLog evl);

        TIntValue GetByKey(string key);
        TIntValue Get(string anyString);

        bool Add(TExtValue ord);
        bool AddNew(TExtValue tr);
        
        TExtValue AddOrGet(TExtValue ord);
        bool AddOrUpdate(TExtValue ord);

        bool Update(TExtValue ord);

        void Push(TradeQueueEntity<TExtValue> e);   

    }

    public interface IEntityRepository3<out TIntValue, TExtValue> : IEntityRepository<TIntValue, TExtValue>,
        IHaveWork<IEventArgs>, IHaveEnQueue<IEventArgs>
    {
    }

    public interface IAccountRepository32 : IEntityRepository<IAccountDb, IAccount>
    {

    }
    //public interface IAccountRepository32 : 
    //                    ITradeBaseRepository3<string, IAccount, IAccountDb>
    //{
    //    void Init(IEventLog evl);
    //}

    public interface IEntityRepository
    {
        bool IsEnabled { get; }
        bool IsQueueEnabled { get; }
    }

    public interface ITradeRepository : IEntityRepository
    {
        void Init(IEventLog evl);

        ITradeDb GetByKey(string key);

        bool Add(ITrade3 ord);
        bool AddNew(ITrade3 tr);
        ITrade3 AddOrGet(ITrade3 ord);
        bool AddOrUpdate(ITrade3 ord);

        bool Update(ITrade3 ord);

        void Push(TradeQueueEntity<ITrade3> e);
        void DeQueueProcess();
    }

    public interface IOrderRepository :  IEntityRepository
    {
        void Init(IEventLog evl);

        IOrderDb GetByKey(string key);
        bool Add(IOrder3 ord);
        bool AddNew(IOrder3 ord);

        IOrder3 AddOrGet(IOrder3 ord);
        bool AddOrUpdate(IOrder3 ord);

        bool Update(IOrder3 ord);

        void Push(TradeQueueEntity<IOrder3> e);
        void DeQueueProcess();
        //void EntityFromQueueToProcess();
    }

    public interface IPositionRepository : IEntityRepository
    {
        void Init(IEventLog evl);

        IPositionDb GetByKey(string key);

        bool Add(IPosition2 p);
        bool AddNew(IPosition2 p);

        IPosition2 AddOrGet(IPosition2 p);
        bool AddOrUpdate(IPosition2 p);

        bool Update(IPosition2 p);

        void Push(TradeQueueEntity<IPosition2> e);
        void DeQueueProcess();
        
        //bool Remove(IPosition p);
        //bool Delete(IPosition p);

    }

    public interface IPositionTotalRepository : IEntityRepository
    {
        void Init(IEventLog evl);

        bool Add(IPosition2 p);
        bool AddNew(IPosition2 p);
        IPosition2 AddOrGet(IPosition2 p);
        bool AddOrUpdate(IPosition2 p);

        bool Update(IPosition2 p);

        void Push(TradeQueueEntity<IPosition2> e);
        void DeQueueProcess();

        //bool Remove(IPosition p);
        //bool Delete(IPosition p);
    }

    public interface IDealRepository : IEntityRepository
    {
        void Init(IEventLog evl);

        bool Add(IDeal p);
        bool AddNew(IDeal p);
        IDeal AddOrGet(IDeal p);
        bool AddOrUpdate(IDeal p);

        bool Update(IDeal p);

        void Push(TradeQueueEntity<IDeal> e);
        void DeQueueProcess();

        //bool Remove(IPosition p);
        //bool Delete(IPosition p);
    }

    public interface ITickerRepository
    {
        void Init(IEventLog evl);
        ITicker AddOrGet(ITicker t);
    }

    public interface IAccountRepository
    {
        void Init(IEventLog evl);

        IAccountDb GetByKey(string s);
        IAccount AddOrGet(IAccount a);
    }
    public interface IAccountRepository3
    {
        void Init(IEventLog evl);
        IAccount AddOrGet(IAccount a);
    }

    public interface IStrategyRepository
    {
        void Init(IEventLog evl);
        IStrategy AddOrGet(IStrategy s);
    }
}
