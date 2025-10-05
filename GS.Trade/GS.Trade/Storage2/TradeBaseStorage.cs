using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Exceptions;
using GS.Extension;
using GS.Interfaces;
using GS.Process;
using GS.Queues;
using GS.Trade.Queues;

namespace GS.Trade.Storage2
{
    public class TradeBaseStorage  : Element1<string>, ITradeStorage //, IHaveQueue
    {
        //private readonly DbTradeContext _db;

        public bool IsPrimary { get; set; }
        public bool IsAsync { get; set; }

        private readonly object _locker;

        //public bool UIEnabled { get; set; }

        //protected IExceptionRepository GSExceptions { get; set; }

        //protected IAccountRepository Accounts { get; set; }
        //protected ITickerRepository Tickers { get; set; }
        //protected IStrategyRepository Strategies { get; set; }

       // protected IOrderRepository Orders { get; set; }
       // protected ITradeRepository Trades { get; set; }
        
        //protected IPositionRepository Positions { get; set; }
        //protected IPositionTotalRepository PositionTotals { get; set; }
        //protected IDealRepository Deals { get; set; }

        protected IEntityRepository<IGSExceptionDb, IGSException> GSExceptions { get; set; }

        protected IAccountRepository32 Accounts { get; set; }
        protected IEntityRepository<ITickerDb, ITicker> Tickers { get; set; }
        protected IEntityRepository<IStrategyDb, IStrategy> Strategies { get; set; }

        protected IEntityRepository<IOrderDb, IOrder3> Orders { get; set; }
        protected IEntityRepository<ITradeDb, ITrade3> Trades { get; set; }

        protected IEntityRepository<IPositionDb, IPosition2> Positions { get; set; }
        protected IEntityRepository<IPositionDb, IPosition2> PositionTotals { get; set; }
        protected IEntityRepository<IDealDb, IDeal> Deals { get; set; }

        public IEnumerable<IEntityRepositoryBase> RepoItems {
            get
            {
                return new IEntityRepositoryBase[]
                {
                    GSExceptions,

                    Accounts,
                    Tickers,
                    Strategies,

                    Orders,
                    Trades,

                    Deals,
                    Positions,
                    PositionTotals,

                };
            }
        }

        //private object Acc {
        //    get { return Accounts as IHaveQueue; }
        //}

        public Action OrderDequeueProcess {
            get { return Orders.DeQueueProcess; }
        }

        public IEnumerable<ProcessManager2.ProcessProcedure> DeQueueProcesses
        {
            get
            {
                return new ProcessManager2.ProcessProcedure[]
            {
                Orders.DeQueueProcess,
                Trades.DeQueueProcess,
                GSExceptions.DeQueueProcess,
                //Deals.DeQueueProcess,
                //Positions.DeQueueProcess,
                //PositionTotals.DeQueueProcess
            };
        }
        }

        public TradeBaseStorage()
        {
            _locker = new object();
        }
        public override void Init(IEventLog eventLog)
        {
            base.Init(eventLog);

            GSExceptions.Init(eventLog);

            Accounts.Init(eventLog);
            Tickers.Init(eventLog);
            Orders.Init(eventLog);
            Trades.Init(eventLog);
            Strategies.Init(eventLog);
            Positions.Init(eventLog);
            PositionTotals.Init(eventLog);
            Deals.Init(eventLog);

        }

        //public bool IsEnabled { get; set; }

        

        public void Add(IGSException ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex", "TradeBaseStorage.Add(exception");

            if (!GSExceptions.IsEnabled)
                return;
            lock (_locker)
            {
                if (GSExceptions.IsQueueEnabled)
                    GSExceptions.Push(new TradeQueueEntity<IGSException>(StorageOperationEnum.Add, ex));
                else
                    GSExceptions.Add(ex);
            }
        }

        public void Add(IOrder3 ord)
        {
            if (Orders == null || ord == null || ord.Strategy == null || ord.Strategy.Id == 0)
                throw new ArgumentNullException("ord", "TradeBaseStorage.Add(IOrder3");
            
            if( Orders.IsEnabled)
                Orders.AddNew(ord);
        }
        public void Add(ITrade3 tr)
        {
            if (Trades == null || tr == null || tr.Strategy == null || tr.Strategy.Id == 0)
                throw new ArgumentNullException("tr", "TradeBaseStorage.Add(ITrade3");
            
            if(Trades.IsEnabled)
                Trades.Add(tr);
           
        }
        //public Trade GetTradeByKey
        public IOrder GetOrderByKey(string key)
        {
            throw new NotImplementedException();
            //return _db.Orders.FirstOrDefault(o => o.OrderKey == key);
        }

        public string GetStrategyKeyFromOrder(string orderKey)
        {
            //using (var db = new DbTradeContext())
            //{
            //var ord = db.Orders.FirstOrDefault(o => o.OrderKey == orderKey);
            //var ord = Orders.GetByKey(orderKey);
            //return ord != null ? ord.StrategyKey : null;

            var ord = Orders.GetByKey(orderKey);
            return ord == null ? null : ord.StrategyKey;          
            //}
        }
       
        public IStrategy GetStrategyByKey(string key)
        {
            throw new NotImplementedException();
        }

        public ITicker Register(ITicker t)
        {
            return Tickers.AddOrGet(t);
        }

        private void SaveChangesOperation<TRepo, TIntValue, TExtValue>(StorageOperationEnum operation, TRepo r, TExtValue p)
            where TRepo : IEntityRepository<TIntValue, TExtValue>
        {
            try
            {
                switch (operation)
                {
                    case StorageOperationEnum.AddOrUpdate:
                        r.AddOrUpdate(p);
                        break;
                    case StorageOperationEnum.Update:
                        r.Update(p);
                        break;
                    case StorageOperationEnum.AddOrGet:
                    case StorageOperationEnum.Register:
                        r.AddOrGet(p);
                        break;
                    case StorageOperationEnum.AddNew:
                        r.AddNew(p);
                        break;
                    case StorageOperationEnum.Add:
                        r.Add(p);
                        break;
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(Name, "DbStorage.SaveChanges(IPosition2)", "", "", e); 
                throw;
            }
        }

        public IAccountBase GetAccountByKey(string key)
        {
            try
            {
                if (key.HasNoValue())
                    throw new ArgumentNullException("key", "TradeBaseStorage.GetAccount(Key==null");

                return Accounts.GetByKey(key);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "Key", "GetAccountByKey(key)", "Key", e);
                throw;
            }
        }
        public IAccountBase GetAccount(string key)
        {
            try
            {
                if (key.HasNoValue())
                    throw new ArgumentNullException("key", "TradeBaseStorage.GetAccount(Key==null");

                return Accounts.Get(key);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, key, "GetAccount(str)", key, e);
                throw;
            }
        }

        public ITickerBase GetTickerByKey(string key)
        {
            try
            {
                if (key.HasNoValue())
                    throw new ArgumentNullException("key", "TradeBaseStorage.GeTickerByKey(Key==null");

                return Tickers.GetByKey(key);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, key, "GetTickerByKey(key)", key, e);

                throw;
            }
        }
        public ITickerBase GetTicker(string key)
        {
            try
            {
                if (key.HasNoValue())
                    throw new ArgumentNullException("key", "TradeBaseStorage.GeTicker(Key==null");

                return Tickers.Get(key);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, key, "GetTicker(str)", key, e);

                throw;
            }
        }
 
        public int SaveChanges(StorageOperationEnum operation, IPosition2 p)
        {
            if (p == null)
            {
                SendExceptionMessage3(Name, "DbStorage.SaveChanges(IPosition2==NULL)", "Null reference", Code, new Exception());
                return -1;
                //throw new NullReferenceException("DbStorage.SaveChanges(IPosition2) Failure:");
            }
            if (IsAsync)
            {
                Task.Factory.StartNew(() => SaveChangesOperation(operation, p));
            }
            else
            {
                lock (_locker)
                {
                    SaveChangesOperation(operation, p);
                }
            }
            return 1;
        }
        private void SaveChangesOperation(StorageOperationEnum operation, IPosition2 p)
        {
            try
            {
                if (Positions.IsQueueEnabled)
                {
                    Positions.Push(new TradeQueueEntity<IPosition2>(operation, p));
                    return;
                }
                switch (operation)
                {
                    case StorageOperationEnum.AddOrUpdate:
                        Positions.AddOrUpdate(p);
                        break;
                    case StorageOperationEnum.Update:
                        Positions.Update(p);
                        break;
                    case StorageOperationEnum.AddOrGet:
                    case StorageOperationEnum.Register:
                       Positions.AddOrGet(p);
                        break;
                    case StorageOperationEnum.AddNew:
                        Positions.AddNew(p);
                        break;
                    case StorageOperationEnum.Add:
                        Positions.Add(p);
                        break;
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, p == null ? "IPosition2" : p.GetType().ToString(), "SaveChanges(IPosition2)", 
                                                p == null ? "IPosition2" : p.ToString(), e);
                throw;
            }
        }

        public int SaveChanges(StorageOperationEnum operation, IPositionTotal2 p)
        {
            if (p == null)
            {
                SendExceptionMessage3(Name, "DbStorage.SaveChanges(IPositionTotal2==Null)", "Null reference", Code, new Exception());
                return -1;
                //throw new NullReferenceException("DbStorage.SaveChanges(IPositionTotal2) Failure:");
            }
            Task.Factory.StartNew(() =>
            {
                try
                {
                    switch (operation)
                    {
                        case StorageOperationEnum.Update:
                            OnChangedEvent(new Events.EventArgs
                            {
                                Category = "UI.Positions",
                                Entity = "Total",
                                Operation = "Update",
                                Object = p
                            });
                            break;
                    }
                }
                catch (Exception e)
                {
                    SendExceptionMessage3(Name, "DbStorage.SaveChanges(IPositionTotal2)", "", "", e);
                    throw;
                }

            });
            return 1;
        }
        
        public int SaveChanges(StorageOperationEnum operation, IDeal p)
        {
            if (p == null)
            {
                SendExceptionMessage3(Name, "DbStorage.SaveDealChanges(IPosition2==Null)", "Null reference", Code,new Exception());
                return -1;
                //throw new NullReferenceException("DbStorage.SaveDealChanges(IPosition2) Failure: ");
            }
            if (IsAsync)
            {
                Task.Factory.StartNew(() => SaveDealChangesOperation(operation, p));
            }
            else
            {
                lock (_locker)
                {
                    SaveDealChangesOperation(operation, p);
                }
            }
            return 1;
        }
        private void SaveDealChangesOperation(StorageOperationEnum operation, IDeal p)
        {
            try
            {
                if (Deals.IsQueueEnabled)
                {
                    Deals.Push(new TradeQueueEntity<IDeal>(operation, p));
                    return;
                }
                switch (operation)
                {
                    case StorageOperationEnum.AddOrUpdate:
                        Deals.AddOrUpdate(p);
                        break;
                    case StorageOperationEnum.Update:
                        Deals.Update(p);
                        break;
                    case StorageOperationEnum.AddOrGet:
                    case StorageOperationEnum.Register:
                        Deals.AddOrGet(p);
                        break;
                    case StorageOperationEnum.AddNew:
                        Deals.AddNew(p);
                        break;
                    case StorageOperationEnum.Add:
                        Deals.Add(p);
                        break;
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, p == null ? "IDeal" : p.GetType().ToString(), "SaveChanges(IDeal)",
                                                p == null ? "IDeal" : p.ToString(), e);
                throw;
            }
        }
        
        public int SaveTotalChanges(StorageOperationEnum operation, IPosition2 p)
        {
            if (p == null)
            {
                SendExceptionMessage3(Name, "DbStorage.SaveTotalChanges(IPositionTotal2==Null)", "Null reference", Code, new Exception());
                return -1;
                //throw new NullReferenceException("DbStorage.SaveChanges(IPositionTotal2) Failure:");
            }
            if (IsAsync)
            {
                Task.Factory.StartNew(() => SaveTotalChangesOperation(operation, p));
            }
            else
            {
                lock (_locker)
                {
                    SaveTotalChangesOperation(operation, p);
                }
            }
            return 1;
        }
        private void SaveTotalChangesOperation(StorageOperationEnum operation, IPosition2 p)
        {
            try
            {
                if (PositionTotals.IsQueueEnabled)
                {
                    PositionTotals.Push(new TradeQueueEntity<IPosition2>(operation, p));
                    return;
                }
                switch (operation)
                {
                    case StorageOperationEnum.AddOrUpdate:
                        PositionTotals.AddOrUpdate(p);
                        break;
                    case StorageOperationEnum.Update:
                    //    FireStorageChangedEvent("Positions", "Total", "Update", p);
                        PositionTotals.Update(p);
                        break;
                    case StorageOperationEnum.AddOrGet:
                    case StorageOperationEnum.Register:
                        PositionTotals.AddOrGet(p);
                        break;
                    //case StorageOperationEnum.Add:
                    case StorageOperationEnum.AddNew:
                        PositionTotals.AddNew(p);
                        break;
                    case StorageOperationEnum.Add:
                        PositionTotals.Add(p);
                        break;
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, p == null ? "IPosition2" : p.GetType().ToString(), "SaveTotalChanges(IPosition2)",
                                                p == null ? "IPosition2" : p.ToString(), e);
                throw;
            }
        }
      
        public int SaveChanges(StorageOperationEnum operation, IOrder3 ord)
        {
            try
            {
                if (ord == null || ord.Strategy == null)
                    throw new ArgumentNullException("ord", "TradeBaseStorage.SaveChanges(IOrder3; )" + "Order or Order.Strategy is Null");

                if (IsAsync)
                {
                    Task.Factory.StartNew(() => SaveChangesOperation(operation, ord));
                }
                else
                {
                    lock (_locker)
                    {
                        SaveChangesOperation(operation, ord);
                    }
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(Name, "Order", "TradeBaseStorage.SaveChanges(Order)",
                                            ord == null ? "Order" : ord.ToString(), e);
                throw;
            }
            return 1;
        }
        private void SaveChangesOperation(StorageOperationEnum operation, IOrder3 o)
        {
            try
            {
                if (Orders.IsQueueEnabled)
                {
                    Orders.Push(new TradeQueueEntity<IOrder3>(operation, o));
                    return;
                }
                switch (operation)
                {
                    case StorageOperationEnum.AddOrUpdate:
                        Orders.AddOrUpdate(o);
                        break;
                    case StorageOperationEnum.Update:
                        Orders.Update(o);
                        break;
                    case StorageOperationEnum.AddOrGet:
                    case StorageOperationEnum.Register:
                        Orders.AddOrGet(o);
                        break;
                    case StorageOperationEnum.Add:
                    case StorageOperationEnum.AddNew:
                        Orders.AddNew(o);
                        break;
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(Name, o.GetType().ToString(), "TradeBaseStorage.SaveChanges(IOrder)", o.ToString(), e);
                throw;
            }
        }

        public int SaveChanges(StorageOperationEnum operation, ITrade3 t)
        {
            try
            {
                if (t == null || t.Strategy == null)
                    throw new ArgumentNullException("t", "TradeBaseStorage.SaveChanges(ITrade3; )" + "Trade or Trade.Strategy is Null");

                if (IsAsync)
                {
                    Task.Factory.StartNew(() => SaveChangesOperation(operation, t));
                }
                else
                {
                    lock (_locker)
                    {
                        SaveChangesOperation(operation, t);
                    }
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(Name, "OTrade", "TradeBaseStorage.SaveChanges(Trade)",
                                                                 t == null ? "Trade" : t.ToString(), e);
                throw;
            }
            return 1;
        }
        private void SaveChangesOperation(StorageOperationEnum operation, ITrade3 t)
        {
            try
            {
                if (Trades.IsQueueEnabled)
                {
                    Trades.Push(new TradeQueueEntity<ITrade3>(operation, t));
                    return;
                }
                switch (operation)
                {
                    case StorageOperationEnum.AddOrUpdate:
                        Trades.AddOrUpdate(t);
                        break;
                    case StorageOperationEnum.Update:
                        Trades.Update(t);
                        break;
                    case StorageOperationEnum.AddOrGet:
                    case StorageOperationEnum.Register:
                        Trades.AddOrGet(t);
                        break;
                    case StorageOperationEnum.Add:
                        Trades.Add(t);
                        break;
                    case StorageOperationEnum.AddNew:
                        Trades.AddNew(t);
                        break;
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(Name, t.GetType().ToString(), "TradeBaseStorage.SaveChanges(ITrade)", t.ToString(), e);
                throw;
            }
        }

        public IPosition2 Register(IPosition2 p)
        {
            try
            {
                if (p == null || p.Strategy == null) //  || p.Strategy.Id == 0)
                    throw new ArgumentNullException("p",
                        "p == null || p.Strategy == null");

                if (p.Strategy.Id == 0)
                    Register(p.Strategy);

                return Positions.AddOrGet(p);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "IPosition2",
                    "Register(IPosition2): " + (p == null || p.Strategy == null ? "" : p.Strategy.StrategyTickerString),
                    p == null ? "Position" : p.ToString(), e);
                throw;
            }
        }

        public IStrategy Register(IStrategy s)
        {
            try
            {
                if (s == null || s.Ticker == null || s.Account == null)
                    throw new ArgumentNullException("s", "s == null || s.Ticker == null ||  s.Account == null");

                if (s.Ticker.Id == 0 )
                        Register(s.Ticker);
                if (s.Account.Id == 0)
                        Register(s.Account);

                return Strategies.AddOrGet(s);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "Strategy",
                    "Register(Strategy): " + (s==null ? "" : s.StrategyTickerString),
                    s==null ? "Strategy" : s.ToString(), e);
                throw;
            }
        }

        public IAccount Register(IAccount a)
        {
            return Accounts.AddOrGet(a);
        }

        public IPosition2 RegisterCurrent(IPosition2 p)
        {
            return Register(p);
        }

        public IPosition2 RegisterTotal(IPosition2 p)
        {
            try
            {
                //throw new NullReferenceException("TestExceptions");

                if (p == null || p.Strategy == null)
                    throw new ArgumentNullException("p",
                        "p == null || p.Strategy == null");

                if (p.Strategy.Id == 0)
                    Register(p.Strategy);

                return PositionTotals.AddOrGet(p);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "IPosition2",
                    "RegisterTotal(IPosition2): " + (p == null || p.Strategy == null ? "" : p.Strategy.StrategyTickerString),
                    p == null ? "PositionTotal" : p.ToString(), e);
                throw;
            }
        }
        public void Init()
        {
            throw new NotImplementedException();
        }

        public override string Key
        {
            get { return Code; }
        }

        public bool IsQueueEnabled { get;  set; }
        public void DeQueueProcess()
        {
            if( GSExceptions.IsQueueEnabled)
                GSExceptions.DeQueueProcess();

            if(Accounts.IsQueueEnabled)
                Accounts.DeQueueProcess();
            if (Tickers.IsQueueEnabled)
                Tickers.DeQueueProcess();
            if (Strategies.IsQueueEnabled)
                Strategies.DeQueueProcess();

            if (Orders.IsQueueEnabled)
                Orders.DeQueueProcess();
            if (Trades.IsQueueEnabled)
                Trades.DeQueueProcess();

            if (Positions.IsQueueEnabled)
                Positions.DeQueueProcess();
            if (PositionTotals.IsQueueEnabled)
                PositionTotals.DeQueueProcess();
            if (Deals.IsQueueEnabled)
                Deals.DeQueueProcess();
        }
    }
}
