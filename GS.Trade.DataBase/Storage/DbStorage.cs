using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Events;
using GS.Exceptions;
using GS.Extension;
using GS.Interfaces;
using GS.Process;
using GS.Trade.DataBase.Dal;
using GS.Trade.DataBase.EFRepositary;
using GS.Trade.DataBase.Init;
using GS.Trade.DataBase.Model;
using GS.Trade.Storage2;


namespace GS.Trade.DataBase.Storage
{
    //public class DbStorage : Element1<string>, ITradeStorage
    //{
    //    //private readonly DbTradeContext _db;

    //    public bool IsPrimary { get; set; }
    //    public bool IsAsync { get; set; }

    //    public string DataBaseName { get; set; }

    //    protected OrderRepository Orders { get; set; }
    //    protected TradeRepository Trades { get; set; }
    //    protected PositionRepository Positions { get; set; }
    //    protected PositionTotalRepository PositionTotals { get; set; }
    //    protected DealRepository Deals { get; set; }

    //    protected TradeEFRepository EFTrades { get; set; }
    //    protected DbTradeContext DbTradeContext { get; set; }

    //    public DbStorage()
    //    {
    //        //_db = new DbTradeContext();
    //        Database.SetInitializer(new InitDb());

    //        DataBaseName = "DbTrade";

    //        DbTradeContext = new DbTradeContext(DataBaseName);

    //        EFTrades = new TradeEFRepository(DbTradeContext);

    //        Orders = new OrderRepository
    //        {
    //            DataBaseName = DataBaseName,
    //            Code = "OrderRepository",
    //            Name = "Orders Repository",
    //            Parent = this,
    //            UIEnabled = true,
    //            IsEnabled = true
    //        }; 
    //        Trades = new TradeRepository
    //        {
    //            DataBaseName = DataBaseName,
    //            Code = "TradeRepository",
    //            Name = "Trades Repository",
    //            Parent = this,
    //            UIEnabled = true,
    //            IsEnabled = true
    //        }; 
    //        Positions = new PositionRepository
    //        {
    //            DataBaseName = DataBaseName,
    //            Code = "PositionRepository",
    //            Name = "Positions Current Repository",
    //            Parent = this,
    //            UIEnabled = true,
    //            IsEnabled = true
    //        }; 

    //        PositionTotals = new PositionTotalRepository
    //        {
    //            DataBaseName = DataBaseName,
    //            Code = "PositionRepository",
    //            Name = "Positions Current Repository",
    //            Parent = this,
    //            UIEnabled = true,
    //            IsEnabled = true
    //        };
    //        Deals = new DealRepository
    //        {
    //            DataBaseName = DataBaseName,
    //            Code = "PositionRepository",
    //            Name = "Positions Current Repository",
    //            Parent = this,
    //            UIEnabled = true,
    //            IsEnabled = true
    //        }; 
    //    }
    //    //public override void Init(IEventLog eventLog)
    //    //{
    //    //     base.Init(eventLog);
    //    //}

    //    //public bool IsEnabled { get; set; }

    //    public IAccountBase GetAccountByKey(string s)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Add(IGSException ex)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Add(IOrder3 ord)
    //    {
    //        if (Orders == null || ord == null || ord.Strategy == null || ord.Strategy.Id == 0)
    //            throw new ArgumentNullException("ord");
            
    //        if( Orders.IsEnabled)
    //            Orders.AddNew(ord);
            
    //        //var o = new Model.Order
    //        //{
    //        //    Created = ord.Created,
    //        //    Modified = ord.Created,

    //        //    Number = ord.Number,

    //        //    OrderKey = ord.Key,
    //        //    StrategyKey = ord.Strategy.Key,
    //        //    StrategyId = ord.Strategy.Id,

    //        //    Status = ord.Status,

    //        //    Operation = (OrderOperationEnum)ord.Operation,
    //        //    OrderType = ord.OrderType,

    //        //    Quantity = ord.Quantity,
    //        //    Rest = ord.Rest,

    //        //    StopPrice = (decimal)ord.StopPrice,
    //        //    LimitPrice = (decimal)ord.LimitPrice

    //        //};
    //        //var db = new DbTradeContext();
    //        //db.Orders.Add(o);
    //        //db.SaveChanges();
    //        //db.Dispose();
    //    }

    //    public void Add(ITrade3 tr)
    //    {
    //        if (Trades == null || tr == null || tr.Strategy == null || tr.Strategy.Id == 0)
    //            throw new ArgumentNullException("tr");

    //        if (Trades.IsEnabled)
    //            Trades.AddNew(tr);

    //        //var tra = new Model.Trade
    //        //{
    //        //    DT = tr.DT,

    //        //    Number = tr.Number,
    //        //    OrderNumber = tr.OrderNumber,

    //        //    Operation = (TradeOperationEnum)tr.Operation,
    //        //    Quantity = tr.Quantity,
    //        //    Price = tr.Price,

    //        //    Key = tr.Key,

    //        //    StrategyId = tr.Strategy.Id
    //        //};

    //        //EFTrades.Add(tra);
    //        //DbTradeContext.SaveChanges();

    //    }
    //    //public Trade GetTradeByKey
    //    public IOrder GetOrderByKey(string key)
    //    {
    //        throw new NotImplementedException();
    //        //return _db.Orders.FirstOrDefault(o => o.OrderKey == key);
    //    }

    //    public string GetStrategyKeyFromOrder(string orderKey)
    //    {
    //        //using (var db = new DbTradeContext())
    //        //{
    //        //var ord = db.Orders.FirstOrDefault(o => o.OrderKey == orderKey);
    //        var ord = Orders.GetByKey(orderKey);
    //        return ord != null ? ord.StrategyKey : null;
    //        //}
    //    }
       
    //    public IStrategy GetStrategyByKey(string key)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IAccount Register(IAccount a)
    //    {
    //        //throw new NotImplementedException();
    //        var acc = new Account
    //        {
    //            Name = a.Name,
    //            Alias = a.Name,
    //            Code = a.Code,
    //            TradePlace = a.TradePlace,
    //            Key = (a.TradePlace == null ? "UnknownTradePlace" : a.TradePlace.TrimUpper()) + "@" + a.Code.TrimUpper()
    //        };
    //        using (var db = new DbTradeContext(DataBaseName))
    //        {
    //            var ac = db.Accounts.FirstOrDefault(aa => aa.Key == acc.Key);
    //            if (ac == null)
    //            {
    //                db.Accounts.Add(acc);
    //                db.SaveChanges();
    //                //db.Dispose();

    //                a.Id = acc.Id;
    //                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Code, "Register(Account)", a.Code, a.ToString());
    //                return a;
    //            }
    //            a.Id = ac.Id;
    //        }
    //        // db.Dispose();
    //        Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Code, "Register(Account)", a.Code, a.ToString());
    //        return a;
    //    }
    //    public ITicker Register(ITicker t)
    //    {
    //        var key = t.ClassCode.TrimUpper() + "@" + t.Code.TrimUpper();

    //        using (var db = new DbTradeContext(DataBaseName))
    //        {
    //            var ti = db.Tickers.FirstOrDefault(tt => tt.Key == key);
    //            if (ti == null)
    //            {
    //                var dt = DateTime.Now;
    //                ti = new Ticker
    //                {
    //                    Name = t.Name,
    //                    TradeBoard = t.ClassCode,
    //                    Code = t.Code,

    //                    BaseContract = t.BaseContract,

    //                    Decimals = t.Decimals,
    //                    MinMove = t.MinMove,

    //                    //Key = t.ClassCode.TrimUpper() + "@" + t.Code.TrimUpper(),

    //                    Created = dt,
    //                    Modified = dt,

    //                    LaunchDate = dt,
    //                    ExpireDate = dt
    //                };

    //                db.Tickers.Add(ti);
    //                db.SaveChanges();
    //                //db.Dispose();

    //                t.Id = ti.Id;
    //                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Code, "Register(Ticker)", t.Code, t.ToString());
    //                return t;
    //            }
    //            //db.Dispose();
    //            t.Id = ti.Id;
    //            Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Code, "Register(Ticker)", t.Code, t.ToString());
    //            return t;
    //        }
    //    }

    //    public IPosition2 Register(IPosition2 p)
    //    {
    //        return RegisterCurrent(p);
    //    }
       

    //    public int SaveChanges(StorageOperationEnum operation, IPosition2 p)
    //    {
    //        if (p == null)
    //        {
    //            SendExceptionMessage(Name, "DbStorage.SaveChanges(IPosition2==NULL)", "Null reference", Code);
    //            return -1;
    //            //throw new NullReferenceException("DbStorage.SaveChanges(IPosition2) Failure:");
    //        }
    //        if (IsAsync)
    //        {
    //            Task.Factory.StartNew(() => SaveChangesOperation(operation, p));    
    //        }
    //        else
    //            SaveChangesOperation(operation, p);
    //        return 1;
    //    }
    //    private void SaveChangesOperation(StorageOperationEnum operation, IPosition2 p)
    //    {
    //        try
    //        {
    //            switch (operation)
    //            {
    //                case StorageOperationEnum.Update:
    //                    Positions.Update(p);
    //                    break;
    //                case StorageOperationEnum.AddOrGet:
    //                case StorageOperationEnum.Register:
    //                   Positions.AddOrGet(p);
    //                    break;
    //                case StorageOperationEnum.AddNew:
    //                    Positions.AddNew(p);
    //                    break;
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            SendExceptionMessage(Name, "DbStorage.SaveChanges(IPosition2)", e.Message, e.Source);
    //            throw;
    //        }
    //    }

    //    public int SaveChanges(StorageOperationEnum operation, IPositionTotal2 p)
    //    {
    //        if (p == null)
    //        {
    //            SendExceptionMessage(Name, "DbStorage.SaveChanges(IPositionTotal2==Null)", "Null reference", Code);
    //            return -1;
    //            //throw new NullReferenceException("DbStorage.SaveChanges(IPositionTotal2) Failure:");
    //        }
    //        Task.Factory.StartNew(() =>
    //        {
    //            try
    //            {
    //                switch (operation)
    //                {
    //                    case StorageOperationEnum.Update:
    //                        OnStorageChangedEvent(new Events.EventArgs
    //                        {
    //                            Category = "UI.Positions",
    //                            Entity = "Total",
    //                            Operation = "Update",
    //                            Object = p
    //                        });
    //                        break;
    //                }
    //            }
    //            catch (Exception e)
    //            {
    //                SendExceptionMessage(Name, "DbStorage.SaveChanges(IPositionTotal2)", e.Message, e.Source);
    //                throw;
    //            }

    //        });
    //        return 1;
    //    }
    //    public int SaveChanges(StorageOperationEnum operation, IDeal p)
    //    {
    //        if (p == null)
    //        {
    //            SendExceptionMessage(Name,"DbStorage.SaveChanges(IDeal==Null)", "Null reference",  Code);
    //            return -1;
    //            //throw new NullReferenceException("DbStorage.SaveChanges(IDeal) Failure: ");
    //        }
    //        Task.Factory.StartNew(() =>
    //        {
    //            try
    //            {
    //                OnStorageChangedEvent(new Events.EventArgs
    //                {
    //                    Category = "Deals",
    //                    Entity = "Deal",
    //                    Operation = "Add",
    //                    Object = p
    //                });
    //            }
    //            catch (Exception e)
    //            {
    //                SendExceptionMessage(Name,"DbStorage.SaveChanges(IDeal)", e.Message, e.Source);
    //                throw;
    //            }
    //        });
    //        return 1;
    //    }

    //    public int SaveTotalChanges(StorageOperationEnum operation, IPosition2 p)
    //    {
    //        if (p == null)
    //        {
    //            SendExceptionMessage(Name, "DbStorage.SaveTotalChanges(IPositionTotal2==Null)", "Null reference", Code);
    //            return -1;
    //            //throw new NullReferenceException("DbStorage.SaveChanges(IPositionTotal2) Failure:");
    //        }
    //        if (IsAsync)
    //        {
    //            Task.Factory.StartNew(() => SaveTotalChangesOperation(operation, p));
    //        }
    //        else
    //            SaveTotalChangesOperation(operation, p);
    //        return 1;
    //    }

    //    public IEnumerable<ProcessManager2.ProcessProcedure> DeQueueProcesses { get; private set; }
    //    public IEnumerable<IEntityRepositoryBase> RepoItems { get; private set; }

    //    private void SaveTotalChangesOperation(StorageOperationEnum operation, IPosition2 p)
    //    {
    //        try
    //        {
    //            switch (operation)
    //            {
    //                case StorageOperationEnum.Update:
    //                    PositionTotals.Update(p);
    //                    //FireStorageChangedEvent("UI.Positions", "Total", "Update", p);
    //                    break;
    //                case StorageOperationEnum.AddOrGet:
    //                case StorageOperationEnum.Register:
    //                    RegisterTotal(p);
    //                    break;
    //            }

    //        }
    //        catch (Exception e)
    //        {
    //            SendExceptionMessage(Name, "DbStorage.SaveChanges(TotalPosition2)", e.Message, e.Source);
    //            throw;
    //        }
    //    }

    //    public int SaveDealChanges(StorageOperationEnum operation, IPosition2 p)
    //    {
    //        if (p == null)
    //        {
    //            SendExceptionMessage(Name, "DbStorage.SaveDealChanges(IPosition2==Null)", "Null reference", Code);
    //            return -1;
    //            //throw new NullReferenceException("DbStorage.SaveDealChanges(IPosition2) Failure: ");
    //        }
    //        if (IsAsync)
    //        {
    //            Task.Factory.StartNew(() => SaveDealChangesOperation(operation, p));
    //        }
    //        else
    //            SaveDealChangesOperation(operation, p);
    //        return 1;
    //    }
    //    private void SaveDealChangesOperation(StorageOperationEnum operation, IPosition2 p)
    //    {
    //        try
    //        {
    //            switch (operation)
    //            {
    //                case StorageOperationEnum.Update:
    //                    FireChangedEvent("Deals", "Deal", "Update", p);
    //                    break;
    //                case StorageOperationEnum.Add:
    //                    AddDeal(p);
    //                    break;
    //                case StorageOperationEnum.AddOrGet:
    //                case StorageOperationEnum.Register:
    //                    RegisterDeal(p);
    //                    break;
    //            }

    //        }
    //        catch (Exception e)
    //        {
    //            SendExceptionMessage(Name, "DbStorage.SaveChanges(TotalPosition2)", e.Message, e.Source);
    //            throw;
    //        }
    //    }

    //    private bool AddDeal(IPosition2 p)
    //    {
    //        try
    //        {
    //            if (p.Strategy == null || p.Strategy.Id == 0)
    //                throw new ArgumentNullException("p");

    //            using (var db = new DbTradeContext(DataBaseName))
    //            {
    //                //var po = db.Positions.FirstOrDefault(t => t.Key == p.Key);
    //                //if (po == null)
    //                //{
    //                    var dtnow = DateTime.Now;
    //                    var po = new Model.Position
    //                    {
    //                        StrategyId = p.Strategy.Id,

    //                        FirstTradeDT = p.FirstTradeDT,
    //                        FirstTradeNumber = p.FirstTradeNumber,

    //                        LastTradeDT = p.LastTradeDT,
    //                        LastTradeNumber = p.LastTradeNumber,

    //                        Operation = p.Operation,
    //                        Quantity = p.Quantity,
    //                        Status = p.PosStatus,

    //                        Price1 = p.Price1,
    //                        Price2 = p.Price2,

    //                        Key = p.Strategy.Key,
    //                    };
    //                    db.Positions.Add(po);
    //                    db.SaveChanges();

    //                    p.Id = po.Id;
    //                //}
    //                //p.Id = po.Id;
    //            }
    //            Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
    //                    Name, Code, "AddDeal()", p.Strategy.StrategyTickerString, p.ToString());
    //        }
    //        catch (Exception e)
    //        {
    //            SendExceptionMessage("Name", "AddDeal(): " + p.Key, e.Message, e.Source);
    //            throw;
    //        }
    //        return true;
    //    }

    //    private IPosition2 RegisterDeal(IPosition2 p)
    //    {
    //        try
    //        {
    //            if (p.Strategy == null || p.Strategy.Id == 0)
    //                throw new ArgumentNullException("p");

    //            using (var db = new DbTradeContext(DataBaseName))
    //            {
    //                var po = db.Positions.FirstOrDefault(t => t.Key == p.Key);
    //                if (po == null)
    //                {
    //                    var dtnow = DateTime.Now;
    //                    po = new Model.Position
    //                    {
    //                        StrategyId = p.Strategy.Id,

    //                        FirstTradeDT = p.FirstTradeDT,
    //                        FirstTradeNumber = p.FirstTradeNumber,

    //                        LastTradeDT = p.LastTradeDT,
    //                        LastTradeNumber = p.LastTradeNumber,

    //                        Operation = p.Operation,
    //                        Quantity = p.Quantity,
    //                        Status = p.PosStatus,

    //                        Price1 = p.Price1,
    //                        Price2 = p.Price2,

    //                        Key = p.Strategy.Key,
    //                    };
    //                    db.Positions.Add(po);
    //                    db.SaveChanges();

    //                    p.Id = po.Id;
    //                }
    //                p.Id = po.Id;
    //            }
    //            Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
    //                    Name, Code, "Register(Deal)", p.Strategy.StrategyTickerString, p.ToString());
    //        }
    //        catch (Exception e)
    //        {
    //            SendExceptionMessage("Name", "Register(Deal): " + p.Key, e.Message, e.Source);
    //            throw;
    //        }
    //        return p;
    //    }

    //    public int SaveChanges(StorageOperationEnum operation, IOrder3 ord)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public int SaveChanges(StorageOperationEnum operation, ITrade3 p)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IStrategy Register(IStrategy s)
    //    {
    //        try
    //        {
    //            if (s.Ticker.Id == 0 || s.Account.Id == 0)
    //                throw new ArgumentNullException("s");

    //            using (var db = new DbTradeContext(DataBaseName))
    //            {
    //                var stra = db.Strategies.FirstOrDefault(st => st.Key == s.Key);
    //                if (stra == null)
    //                {
    //                    var dt = DateTime.Now;
    //                    stra = new Strategy
    //                    {
    //                        Name = s.Name,
    //                        Alias = s.Name,

    //                        Code = s.Code,

    //                        Key = s.Key,

    //                        TickerId = s.Ticker.Id,
    //                        AccountId = s.Account.Id
    //                    };

    //                    db.Strategies.Add(stra);
    //                    db.SaveChanges();
    //                    //db.Dispose();

    //                    //s.Id = stra.Id;
    //                    //Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Code, "Register(Strategy)",
    //                    //    s.StrategyTickerString, s.ToString());
    //                    //return s;
    //                }
    //                //db.Dispose();
    //                s.Id = stra.Id;
    //                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
    //                    Name, Code, "Register(Strategy)", s.StrategyTickerString, s.ToString());
    //                //return null;
    //            }
    //            RegisterCurrent(s.Position);
    //            return s;
    //        }
    //        catch (Exception e)
    //        {
    //            SendExceptionMessage("Name", "Register(Strategy): " + s.StrategyTickerString, e.Message, e.Source);
    //            throw;
    //        }
    //    }

    //    public IPosition2 RegisterCurrent(IPosition2 p)
    //    {
    //        return Positions.AddOrGet(p);
    //    }
    //    public IPosition2 RegisterTotal(IPosition2 p)
    //    {
    //        return PositionTotals.AddOrGet(p);
    //    }

    //    //public IPosition2 RegisterTotal(IPosition2 p)
    //    //{
    //    //    try
    //    //    {
    //    //        if (p.Strategy == null || p.Strategy.Id == 0)
    //    //            throw new ArgumentNullException("p");

    //    //        using (var db = new DbTradeContext(DataBaseName))
    //    //        {
    //    //            var po = db.Positions.FirstOrDefault(t => t.Key == p.Key);
    //    //            if (po == null)
    //    //            {
    //    //                var dtnow = DateTime.Now;
    //    //                po = new Model.Position
    //    //                {
    //    //                    StrategyId = p.Strategy.Id,

    //    //                    FirstTradeDT = dtnow,
    //    //                    FirstTradeNumber = p.FirstTradeNumber,

    //    //                    LastTradeDT = dtnow,
    //    //                    LastTradeNumber = p.LastTradeNumber,

    //    //                    Operation = p.Operation,
    //    //                    Quantity = p.Quantity,
    //    //                    Status = p.PosStatus,

    //    //                    Price1 = p.Price1,
    //    //                    Price2 = p.Price2,

    //    //                    Key = p.Strategy.Key,
    //    //                };
    //    //                db.Positions.Add(po);
    //    //                db.SaveChanges();

    //    //                p.Id = po.Id;
    //    //            }
    //    //            p.Id = po.Id;
    //    //        }
    //    //        Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
    //    //                Name, Code, "Register(PositionCurrent)", p.Strategy.StrategyTickerString, p.ToString());
    //    //    }
    //    //    catch (Exception e)
    //    //    {
    //    //        SendExceptionMessage("Name", "Add(PositionCurrent): " + p.Key, e.Message, e.Source);
    //    //        throw;
    //    //    }
    //    //    return p;
    //    //}
    //    public void Init()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override string Key
    //    {
    //        get { return Code; }
    //    }
    //}
}
