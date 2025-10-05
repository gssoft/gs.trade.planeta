using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GS.Interfaces;
using GS.Trade.DataBase.Dal;
using GS.Trade.DataBase.EFRepositary;
using GS.Trade.DataBase.Model;
using GS.Extension;
using GS.Trade.Queues;
using GS.Trade.Storage2;

namespace GS.Trade.DataBase.Storage
{
    public class AccountRepository3 :   TradeBaseRepository3<string, IAccount, Model.Account, IAccountDb>,
                                        IEntityRepository<IAccountDb, IAccount>
    {
        //public AccountRepository3()
        //{
        //    TradeEntityQueue = new TradeEntityQueue<IAccount>();
        //}
        public override string Key { get { return Code; } }

        public override void Init(IEventLog eventLog)
        {
            base.Init(eventLog);

            DbTradeContext = new DbTradeContext(DataBaseName);
            //DbSet = DbTradeContext.Set<Model.Account>();
            Repository = new AccountEFRepository(DbTradeContext);
        }

       // public IAccountDb Get(string anyString)
       // {
       //     throw new NotImplementedException();
       // }

        protected override bool AddVal(IAccount a)
        {
            try
            {
                var ac = new Account
                {
                    Name = a.Name,
                    Alias = a.Name,
                    Code = a.Code,
                    TradePlace = a.TradePlace, 
                    Key = a.Key
                    //Key = a.TradePlace.TrimUpper() + "@" + a.Code.TrimUpper()
                };
                // ac.TradePlace = a.TradePlace ?? "";
                Repository.Add(ac);
                DbTradeContext.SaveChanges();

                a.Id = ac.Id;
                return true;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, a.GetType().ToString(), "Add(Account):", a.ToString(), e);
                throw;
            }
        }
        protected override IAccount Update(IAccount ve, Account vi) // AddOrGet
        {
            ve.Id = vi.Id;
            ve.Code = vi.Code;
            ve.Name = vi.Name;
            ve.Alias = vi.Alias;
            ve.TradePlace = vi.TradePlace;
            
            ve.Balance = vi.Balance;
            return ve;
        }
        protected override bool Update(Account vi, IAccount ve)
        {
            try
            {
                if (ve == null)
                    throw new ArgumentNullException("ve","Account==Null");
            //vi.Id = ve.Id;
            //vi.Code = ve.Code;
            vi.Name = ve.Name;
            vi.Alias = ve.Alias;

            vi.Balance = ve.Balance;

            Repository.Update(vi);
            DbTradeContext.SaveChanges();

            //if (UIEnabled)
            //    FireStorageChangedEvent("UI.Accounts", "Account", "Update", ve);

            if(EvlEnabled)
            Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                    FullName, Code, "Update(Account)", ve.Code, ve.ToString());
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName,
                    ve?.GetType().ToString() ?? "Account", "Update(Account)",
                    ve?.ToString() ?? "Account", e);                                 
                // throw;
            }
            return true;
        }
    }

    public class TickerRepository3 :    TradeBaseRepository3<string, ITicker, Model.Ticker, ITickerDb>,
                                        IEntityRepository<ITickerDb, ITicker>           //ITickerRepository
    {
        //public TickerRepository3()
        //{
        //    TradeEntityQueue = new TradeEntityQueue<ITicker>();
        //}
        public override string Key { get { return Code; } }

        public override void Init(IEventLog eventLog)
        {
            base.Init(eventLog);

            DbTradeContext = new DbTradeContext(DataBaseName);
            Repository = new TickerEFRepository(DbTradeContext);
        }

        protected override bool AddVal(ITicker t)
        {
            var dt = DateTime.Now;
            try
            {
                var ti = new Ticker
                {
                    Key = t.Key,

                    Name = t.Name,
                    Alias = t.Name,
                    Code = t.Code,
                    TradeBoard = t.ClassCode,
                    BaseContract = t.BaseContract,
                    Decimals = t.Decimals,
                    MinMove = t.MinMove,
                    //Key = t.ClassCode.TrimUpper() + "@" + t.Code.TrimUpper(),

                    Created = dt,
                    Modified = dt,

                    LaunchDate = dt,
                    ExpireDate = dt
                };

                Repository.Add(ti);
                DbTradeContext.SaveChanges();

                t.Id = ti.Id;
                return true;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, t.GetType().ToString(), "Add(Ticker):", t.ToString(), e);
                throw;
            }
        }

        protected override ITicker Update(ITicker ve, Ticker vi) // AddOrGet - Register
        {
            ve.Id = vi.Id; 
            ve.Name = vi.Name;
            ve.Code = vi.Code;
            ve.TradeBoard = vi.TradeBoard;
            ve.ClassCode = vi.TradeBoard;

            ve.BaseContract = vi.BaseContract;

            ve.Decimals = vi.Decimals;
            ve.MinMove = vi.MinMove;
            ve.Margin = vi.Margin;
            ve.PriceLimit = vi.PriceLimit;
            
            return ve;
        }

        protected override bool Update(Ticker vi, ITicker ve)
        {
            vi.Key = ve.Key;
            vi.Name = ve.Name;
            vi.Code = ve.Code;
            vi.TradeBoard = ve.ClassCode;

            vi.BaseContract = ve.BaseContract;

            vi.Decimals = ve.Decimals;
            vi.MinMove = ve.MinMove;
            vi.Margin = ve.Margin;
            vi.PriceLimit = ve.PriceLimit;

            Repository.Update(vi);
            DbTradeContext.SaveChanges();

            return true;
        }

    }

    public class StrategyRepository3 :  TradeBaseRepository3<string, IStrategy, Model.Strategy, IStrategyDb>,
                                        IEntityRepository<IStrategyDb, IStrategy>  // IStrategyRepository
    {
        public override string Key { get { return Code; } }

        public override void Init(IEventLog eventLog)
        {
            base.Init(eventLog);

            DbTradeContext = new DbTradeContext(DataBaseName);
            Repository = new StrategyEFRepository(DbTradeContext);
        }

        protected override bool AddVal(IStrategy ve)
        {
            var dt = DateTime.Now;
            try
            {
                var s = new Strategy
                {
                    Key = ve.Key,

                    Name = ve.Name,
                    Alias = ve.Name,
                    Code = ve.Code,

                    //Created = dt,
                    //Modified = dt,
                    TickerId = ve.Ticker.Id,
                    AccountId = ve.Account.Id
                };

                Repository.Add(s);
                DbTradeContext.SaveChanges();

                ve.Id = s.Id;
                return true;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, ve.GetType().ToString(), "Add(Strategy):", ve.ToString(), e);
                throw;
            }
        }

        protected override IStrategy Update(IStrategy ve, Strategy vi) // AddOrGet - Register
        {
            ve.Id = vi.Id;
            ve.Name = vi.Name;
            ve.Code = vi.Code;
            ve.Alias = vi.Alias;
            return ve;
        }

        protected override bool Update(Strategy vi, IStrategy ve)
        {
            vi.Key = ve.Key;
            vi.Name = ve.Name;
            vi.Code = ve.Code;
            vi.Alias = ve.Alias;

            Repository.Update(vi);
            DbTradeContext.SaveChanges();

            return true;
        }
    }

    public class OrderRepository3 :     TradeBaseRepository3<string, IOrder3, Model.Order, IOrderDb>,
                                        IEntityRepository<IOrderDb, IOrder3>    // IOrderRepository
    {
        //public OrderRepository3()
        //{
        //    TradeEntityQueue = new TradeEntityQueue<IOrder3>();
        //}
        private  object _locker;
        public override string Key { get { return Code; } }

        public override void Init(IEventLog eventLog)
        {
            base.Init(eventLog);

            DbTradeContext = new DbTradeContext(DataBaseName);
            Repository = new OrderEFRepository(DbTradeContext);
            _locker = new object();
        }

        protected override bool AddVal(IOrder3 ve)
        {
            try
            {
                var o = new Model.Order
                {
                    Number = ve.Number,

                    Status = ve.Status,
                    Operation = ve.Operation,
                    OrderType = ve.OrderType,

                    Quantity = ve.Quantity,
                    Rest = ve.Rest,
                    LimitPrice = ve.LimitPrice,
                    StopPrice = ve.StopPrice,

                    TrMessage = ve.TrMessage,

                    Key = ve.Key,
                    StrategyId = ve.Strategy.Id,

                    Created = ve.Created,
                    Modified = ve.Created,
                            
                    OrderKey = ve.Key,
                    StrategyKey = ve.Strategy.Key,
                };
                Repository.Add(o);
                DbTradeContext.SaveChanges();

                ve.Id = o.Id;

                if (UIEnabled)
                    FireChangedEvent("UI.Orders", "Order", "AddNew", ve);

                return true;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, ve.GetType().ToString(), "Add(Order):", ve.ToString(), e);
                                                // e.Message, e.Source, e.GetType().ToString(), e.TargetSite.ToString());
                throw;
            }
        }

        protected override IOrder3 Update(IOrder3 ve, Model.Order vi)
        {
            throw new NotImplementedException();
        }

        protected override bool Update(Model.Order vi, IOrder3 ve)
        {
            try
            {
                if(ve == null || ve.Strategy == null)
                    throw new NullReferenceException("OrderCore or OrderCore.Srategy==null");

                lock (_locker)
                {
                    vi.Status = ve.Status;
                    vi.Operation = ve.Operation;
                    vi.OrderType = ve.OrderType;

                    vi.Quantity = ve.Quantity;
                    vi.Rest = ve.Rest;

                    vi.LimitPrice = ve.LimitPrice;
                    vi.StopPrice = ve.StopPrice;

                    vi.Modified = DateTime.Now;

                    vi.OrderKey = ve.Key;
                    vi.StrategyKey = ve.Strategy.Key;

                    DbTradeContext.SaveChanges();
                }
                if (UIEnabled)
                    FireChangedEvent("UI.Orders", "Order", "Update", ve);

                return true;
            }
            catch (Exception e)
            {
                // if(ve!=null)
                SendExceptionMessage3(FullName, ve == null ? "Order" : ve.GetType().ToString(), "Update(Order):",
                                            ve == null ? "Order" : ve.ToString(), e);
                throw;
            }
        }
    }

    public class TradeRepository3 :     TradeBaseRepository3<string, ITrade3, Model.Trade, ITradeDb>, 
                                        IEntityRepository<ITradeDb, ITrade3>
    {
        //public TradeRepository3()
        //{
        //    TradeEntityQueue = new TradeEntityQueue<ITrade3>();
        //}

        public override string Key { get { return Code; } }

        public override void Init(IEventLog eventLog)
        {
            base.Init(eventLog);

            DbTradeContext = new DbTradeContext(DataBaseName);
            Repository = new TradeEFRepository(DbTradeContext);
        }

        protected override bool AddVal(ITrade3 ve)
        {
            try
            {
                var tra = new Model.Trade
                {
                    DT = ve.DT,

                    Number = ve.Number,
                    OrderNumber = ve.OrderNumber,

                    Operation = ve.Operation,
                    Quantity = ve.Quantity,
                    Price = ve.Price,

                    Key = ve.Key,
                    StrategyId = ve.Strategy.Id
                };
                Repository.Add(tra);
                DbTradeContext.SaveChanges();

                ve.Id = tra.Id;

                if (UIEnabled)
                    FireChangedEvent("UI.Trades", "Trade", "Add", ve);

                return true;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, ve.GetType().ToString(), "Add(Trade):", ve.ToString(), e);
                throw;
            }
        }

        protected override ITrade3 Update(ITrade3 ve, Model.Trade vi)
        {
            throw new NotImplementedException();
        }

        protected override bool Update(Model.Trade vi, ITrade3 ve)
        {
            throw new NotImplementedException();
        }

        //public override ITradeDb GetByKey(string key)
        //{
        //    throw new NotImplementedException();
        //}
    }

    public class PositionRepository3 :  TradeBaseRepository3<string, IPosition2, Model.Position, IPositionDb>,
                                        IEntityRepository<IPositionDb, IPosition2> //IPositionRepository
    {
        //public PositionRepository3()
        //{
        //    TradeEntityQueue = new TradeEntityQueue<IPosition2>();
        //}
        public override string Key { get { return Code; } }

        public override void Init(IEventLog eventLog)
        {
            base.Init(eventLog);

            DbTradeContext = new DbTradeContext(DataBaseName);
            Repository = new PositionEFRepository(DbTradeContext);
        }

        protected override bool AddVal(IPosition2 p)
        {
            try
            {
                if (p.Strategy == null || p.Strategy.Id == 0)
                    throw new ArgumentNullException("p");

                var dtnow = DateTime.Now;
                var po = new Model.Position
                {
                    StrategyId = p.Strategy.Id,

                    FirstTradeDT = dtnow,
                    FirstTradeNumber = p.FirstTradeNumber,

                    LastTradeDT = dtnow,
                    LastTradeNumber = p.LastTradeNumber,

                    Operation = p.Operation,
                    Quantity = p.Quantity,
                    Status = p.PosStatus,

                    Price1 = p.Price1,
                    Price2 = p.Price2,
                    PnL3 = p.PnL3,

                    Key = p.Strategy.Key,

                    Created = dtnow,
                    Modified = dtnow
                };
                Repository.Add(po);
                DbTradeContext.SaveChanges();

                p.Id = po.Id;

                if (UIEnabled)
                    FireChangedEvent("UI.Positions", "Current", "AddOrUpdate", p);
                return true;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, p.GetType().ToString(), "Add(PositionCurrent)", p.ToString(), e);
                throw;
            }
        }

        protected override IPosition2 Update(IPosition2 ve, Position vi)
        {
            try
            {
                if (vi == null || vi.Strategy == null || vi.Strategy.Id == 0)
                    throw new ArgumentNullException("vi", "PositionDb is Invalid");
                if (ve == null || ve.Strategy == null || ve.Strategy.Id == 0)
                    throw new ArgumentNullException("vi","PositionCore is Invalid");

                ve.Id = vi.Id;

                ve.FirstTradeDT = vi.FirstTradeDT;
                ve.FirstTradeNumber = vi.FirstTradeNumber.ToUint64();

                ve.LastTradeDT = vi.LastTradeDT;
                ve.LastTradeNumber = vi.LastTradeNumber.ToUint64();

                ve.Operation = vi.Operation;
                ve.Quantity = vi.Quantity;
                ve.Status = vi.Status;

                ve.Price1 = vi.Price1;
                ve.Price2 = vi.Price2;
                ve.PnL3 = vi.PnL3;

                // DbTradeContext.SaveChanges();

                if (UIEnabled)
                    FireChangedEvent("UI.Positions", "Current", "AddOrUpdate", ve);

                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                        FullName, Code, "Update(PositionCurrentCore)", ve.Strategy.StrategyTickerString, ve.ToString());
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName,
                    ve == null ? "PositionCore" : ve.GetType().ToString(), "Update(PositionCurrent)",
                    ve == null ? "PositionCore" : ve.ToString(), e);
                throw;
            }
            return ve;
        }

        protected override bool Update(Position vi, IPosition2 ve)
        {
            try
            {
                if (ve == null || ve.Strategy == null || ve.Strategy.Id == 0)
                    throw new ArgumentNullException("ve");

                vi.FirstTradeDT = ve.FirstTradeDT.MinValueToSql();
                vi.FirstTradeNumber = ve.FirstTradeNumber;

                vi.LastTradeDT = ve.LastTradeDT.MinValueToSql();
                vi.LastTradeNumber = ve.LastTradeNumber;

                vi.Operation = ve.Operation;
                vi.Quantity = ve.Quantity;
                vi.Status = ve.PosStatus;

                vi.Price1 = ve.Price1;
                vi.Price2 = ve.Price2;
                vi.PnL3 = ve.PnL3;

                vi.Modified = DateTime.Now;

                DbTradeContext.SaveChanges();

                if (UIEnabled)
                    FireChangedEvent("UI.Positions", "Current", "AddOrUpdate", ve);

                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                        FullName, Code, "Update(PositionCurrentDb)", ve.Strategy.StrategyTickerString, ve.ToString());
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName,
                    ve == null ? "PositionCore" : ve.GetType().ToString(), "Update(PositionCurrent)",
                    ve == null ? "PositionCore" : ve.ToString(), e);
                throw;
            }
            return true;
        }   
    }

    public class PositionTotalRepository3 : TradeBaseRepository3<string, IPosition2, Model.Total, IPositionDb>,
                                            IEntityRepository<IPositionDb, IPosition2> // IPositionTotalRepository
    {
        //public PositionTotalRepository3()
        //{
        //    TradeEntityQueue = new TradeEntityQueue<IPosition2>();
        //}
        public override string Key { get { return Code; } }

        public override void Init(IEventLog eventLog)
        {
            base.Init(eventLog);

            DbTradeContext = new DbTradeContext(DataBaseName);
            Repository = new TotalEFRepository(DbTradeContext);
        }

        protected override bool AddVal(IPosition2 p)
        {
            try
            {
                if (p.Strategy == null || p.Strategy.Id == 0)
                    throw new ArgumentNullException("p");

                var dtnow = DateTime.Now;
                var po = new Model.Total
                {
                    StrategyId = p.Strategy.Id,

                    FirstTradeDT = dtnow,
                    FirstTradeNumber = p.FirstTradeNumber,

                    LastTradeDT = dtnow,
                    LastTradeNumber = p.LastTradeNumber,

                    Operation = p.Operation,
                    Quantity = p.Quantity,
                    Status = p.PosStatus,

                    Price1 = p.Price1,
                    Price2 = p.Price2,
                    PnL3 = p.PnL3,

                    Key = p.Strategy.Key,

                    Created = dtnow,
                    Modified = dtnow
                };
                Repository.Add(po);
                DbTradeContext.SaveChanges();
                p.Id = po.Id;

                if (UIEnabled)
                    FireChangedEvent("UI.Positions", "Total", "AddOrUpdate", p);
                
                return true;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, p.GetType().ToString(), "Add(Total)", p.ToString(), e);
                throw;
            }
        }

        protected override IPosition2 Update(IPosition2 ve, Total vi)
        {
            try
            {
                if (vi == null || vi.Strategy == null || vi.Strategy.Id == 0)
                    throw new ArgumentNullException("vi", "TotalDb is Invalid");
                if (ve == null || ve.Strategy == null || ve.Strategy.Id == 0)
                    throw new ArgumentNullException("vi", "TotalCore is Invalid");

                ve.Id = vi.Id;

                ve.FirstTradeDT = vi.FirstTradeDT;
                ve.FirstTradeNumber = vi.FirstTradeNumber.ToUint64();

                ve.LastTradeDT = vi.LastTradeDT;
                ve.LastTradeNumber = vi.LastTradeNumber.ToUint64();

                ve.Operation = vi.Operation;
                ve.Quantity = vi.Quantity;
                ve.Status = vi.Status;

                ve.Price1 = vi.Price1;
                ve.Price2 = vi.Price2;
                ve.PnL3 = vi.PnL3;

                if (UIEnabled)
                    FireChangedEvent("UI.Positions", "Total", "AddOrUpdate", ve);

                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                        FullName, Code, "Update(Total)", ve.Strategy.StrategyTickerString, ve.ToString());
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName,
                    ve == null ? "TotalCore" : ve.GetType().ToString(), "Update(Total)",
                    ve == null ? "TotalCore" : ve.ToString(), e);
                throw;
            }
            return ve;
        }

        protected override bool Update(Total vi, IPosition2 ve)
        {
            try
            {
                if (ve == null || ve.Strategy == null || ve.Strategy.Id == 0)
                    throw new ArgumentNullException("ve");

                vi.FirstTradeDT = ve.FirstTradeDT.MinValueToSql();
                vi.FirstTradeNumber = ve.FirstTradeNumber;

                vi.LastTradeDT = ve.LastTradeDT.MinValueToSql();
                vi.LastTradeNumber = ve.LastTradeNumber;

                vi.Operation = ve.Operation;
                vi.Quantity = ve.Quantity;
                vi.Status = ve.PosStatus;

                vi.Price1 = ve.Price1;
                vi.Price2 = ve.Price2;
                vi.PnL3 = ve.PnL3;

                vi.Modified = DateTime.Now;

                DbTradeContext.SaveChanges();

                if (UIEnabled)
                    FireChangedEvent("UI.Positions", "Total", "AddOrUpdate", ve);

                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                        FullName, Code, "Update(Total)", ve.Strategy.StrategyTickerString, ve.ToString());
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName,
                    ve == null ? "TotalCore" : ve.GetType().ToString(), "Update(TotalDb)",
                    ve == null ? "TotalCore" : ve.ToString(), e);
                throw;
            }
            return true;
        }
    }

    public class DealRepository3 :      TradeBaseRepository3<string, IDeal, Model.Deal, IDealDb>,
                                        IEntityRepository<IDealDb, IDeal>     //IDealRepository
    {
        //public DealRepository3()
        //{
        //    TradeEntityQueue = new TradeEntityQueue<IDeal>();
        //}
        public override string Key { get { return Code; } }

        public override void Init(IEventLog eventLog)
        {
            base.Init(eventLog);

            DbTradeContext = new DbTradeContext(DataBaseName);
            Repository = new DealEFRepository(DbTradeContext);
        }

        protected override bool AddVal(IDeal p)
        {
            try
            {
                if (p.Strategy == null || p.Strategy.Id == 0)
                    throw new ArgumentNullException("p");

                var dtnow = DateTime.Now;
                var po = new Model.Deal
                {
                    StrategyId = p.Strategy.Id,

                    DT = p.DT.MinValueToSql(),
                    Number = p.Number,

                    FirstTradeDT = dtnow,
                    FirstTradeNumber = p.FirstTradeNumber,

                    LastTradeDT = dtnow,
                    LastTradeNumber = p.LastTradeNumber,

                    Operation = p.Operation,
                    Quantity = p.Quantity,
                    Status = p.Status,

                    Price1 = p.Price1,
                    Price2 = p.Price2,

                    Key = p.Strategy.Key,

                };
                Repository.Add(po);
                DbTradeContext.SaveChanges();

                p.Id = po.Id;

                if (UIEnabled)
                    FireChangedEvent("UI.Deals", "Deal", "Add", p);

                if(EvlEnabled)
                    Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                        FullName, Code, "Add(Deal)", p.Strategy.StrategyTickerString, p.ToString());
                return true;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, p.GetType().ToString(), "Add(Deal)", p.ToString(), e);
                throw;
            }
        }

        protected override IDeal Update(IDeal ve, Deal vi)
        {
            try
            {
                if (vi == null || vi.Strategy == null || vi.Strategy.Id == 0)
                    throw new ArgumentNullException("vi", "DealDb is Invalid");
                if (ve == null || ve.Strategy == null || ve.Strategy.Id == 0)
                    throw new ArgumentNullException("vi", "DealCore is Invalid");

                ve.Id = vi.Id;

                ve.FirstTradeDT = vi.FirstTradeDT.MinValueToSql();
                ve.FirstTradeNumber = vi.FirstTradeNumber.ToUint64();

                ve.LastTradeDT = vi.LastTradeDT.MinValueToSql();
                ve.LastTradeNumber = vi.LastTradeNumber.ToUint64();

                ve.Operation = vi.Operation;
                ve.Quantity = vi.Quantity;
                ve.Status = vi.Status;

                ve.Price1 = vi.Price1;
                ve.Price2 = vi.Price2;

                if (UIEnabled)
                    FireChangedEvent("UI.Deals", "Deal", "Update", ve);

                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                        FullName, Code, "Update(Deal)", ve.Strategy.StrategyTickerString, ve.ToString());
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName,
                    ve?.GetType().ToString() ?? "DealCore", "Update(Deal)",
                    ve?.ToString() ?? "DealCore", e);
                throw;
            }
            return ve;
        }

        protected override bool Update(Deal vi, IDeal ve)
        {
            try
            {
                if (ve?.Strategy == null || ve.Strategy.Id == 0)
                    throw new ArgumentNullException(nameof(ve));

                vi.FirstTradeDT = ve.FirstTradeDT.MinValueToSql();
                vi.FirstTradeNumber = ve.FirstTradeNumber;

                vi.LastTradeDT = ve.LastTradeDT.MinValueToSql();
                vi.LastTradeNumber = ve.LastTradeNumber;

                vi.Operation = ve.Operation;
                vi.Quantity = ve.Quantity;
                vi.Status = ve.Status;

                vi.Price1 = ve.Price1;
                vi.Price2 = ve.Price2;

               DbTradeContext.SaveChanges();

                if (UIEnabled)
                    FireChangedEvent("UI.Deals", "Deal", "Update", ve);

                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                        FullName, Code, "Update(Deal)", ve.Strategy.StrategyTickerString, ve.ToString());
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName,
                    ve?.GetType().ToString() ?? "DealCore", "Update(Deal)", ve == null ? "DealCore" : ve.ToString(), e);
                throw;
            }
            return true;
        }


    }
}
    
