using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.Elements;
using GS.Exceptions;
using GS.Interfaces;
using GS.Process;
using GS.Queues;
using GS.Trade.Storage2;

namespace GS.Trade
{
    public enum StorageOperationEnum : short
    {
        Nop = 0, Add = 1, AddNew = 11, AddOrGet = 12, AddOrUpdate = 13, Register = 14,
        Update = 21,
        Read = 31,
        Delete = 41,
    };
    public interface IStorage
    {
        event EventHandler<Events.IEventArgs> StorageChangedEvent;      

        void Add(IOrder3 ord);
        void Add(ITrade3 tr);

        IOrder GetOrderByKey(string key);
        IStrategy GetStrategyByKey(string key);

        string GetStrategyKeyFromOrder(string orderKey);

        IStrategy Register(IStrategy s);
        IAccount Register(IAccount a);
        ITicker Register(ITicker t);

        int SaveChanges(StorageOperationEnum operation, IPosition2 p);
        int SaveChanges(StorageOperationEnum operation, IPositionTotal2 p);
        int SaveChanges(StorageOperationEnum operation, IDeal p);

        int SaveTotalChanges(StorageOperationEnum operation, IPosition2 p);
        int SaveDealChanges(StorageOperationEnum operation, IPosition2 p);

        int SaveChanges(StorageOperationEnum operation, IOrder3 ord);
        int SaveChanges(StorageOperationEnum operation, ITrade3 p);
        
    }

    public interface ITradeStorage : IElement1<string>, IHaveKey<string>, IHaveInit, IHaveQueue
    {
        //event EventHandler<Events.IEventArgs> StorageChangedEvent;
        //void OnStorageChangedEvent(Events.IEventArgs ea);

        //event EventHandler<Events.IEventArgs> ExceptionEvent;

        //bool IsEnabled { get; }
        bool IsPrimary { get; }
        bool IsAsync { get; }

        IAccountBase GetAccountByKey(string key);
        IAccountBase GetAccount(string s);

        ITickerBase GetTickerByKey(string key);
        ITickerBase GetTicker(string s);

        void Add(IGSException ex);

        //void Add(IOrder3 ord);
        //void Add(ITrade3 tr);

        IOrder GetOrderByKey(string key);
        IStrategy GetStrategyByKey(string key);

        string GetStrategyKeyFromOrder(string orderKey);

        IStrategy Register(IStrategy s);
        IAccount Register(IAccount a);
        ITicker Register(ITicker t);

        IPosition2 Register(IPosition2 p);
        IPosition2 RegisterTotal(IPosition2 p);

        int SaveChanges(StorageOperationEnum operation, IOrder3 ord);
        int SaveChanges(StorageOperationEnum operation, ITrade3 p);

        int SaveChanges(StorageOperationEnum operation, IPosition2 p);
        int SaveChanges(StorageOperationEnum operation, IPositionTotal2 p);
        int SaveChanges(StorageOperationEnum operation, IDeal p);

        int SaveTotalChanges(StorageOperationEnum operation, IPosition2 p);
        //int SaveDealChanges(StorageOperationEnum operation, IPosition2 p);

        IEnumerable<ProcessManager2.ProcessProcedure> DeQueueProcesses { get; }
        IEnumerable<IEntityRepositoryBase> RepoItems { get; }

    }
}
