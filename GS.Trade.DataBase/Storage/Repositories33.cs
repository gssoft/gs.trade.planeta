using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using GS.Extension;
using GS.Interfaces;
using GS.Trade.DataBase.Dal;
using GS.Trade.DataBase.Model;
using GS.Trade.Storage2;

namespace GS.Trade.DataBase.Storage
{
    public class AccountRepository33 :
                    TradeBaseRepository33<DbTradeContext, string, IAccount, Model.Account, IAccountDb>,
                    IAccountRepository32
    {
        public override string Key { get { return Code; } }

        protected override Model.Account GetByKey(DbTradeContext cntx, string key)
        {
            var a = cntx.Accounts.FirstOrDefault(e => e.Key == key);
            //?? cntx.Accounts.FirstOrDefault(e => e.Code == key)
            //    ?? cntx.Accounts.FirstOrDefault(e => e.Alias == key);
            return a;
        }
        protected override Model.Account Get(DbTradeContext cntx, string key)
        {
            var a = cntx.Accounts.FirstOrDefault(e => e.Key == key)
                            ?? cntx.Accounts.FirstOrDefault(e => e.Code == key)
                                ?? cntx.Accounts.FirstOrDefault(e => e.Alias == key)
                                    ?? cntx.Accounts.FirstOrDefault(e => e.Name == key);
            return a;
        }
        protected override bool AddVal(DbTradeContext cntx, IAccount a)
        {
            var dt = DateTime.Now;
            //try
            //{
                var ac = new Account
                {
                    Name = a.Name,
                    Alias = a.Name,
                    Code = a.Code,
                    TradePlace = a.TradePlace,
                    Key = a.Key,
                    Created = dt,
                    Modified = dt,
                    //Key = a.TradePlace.TrimUpper() + "@" + a.Code.TrimUpper()
                };
                // ac.TradePlace = a.TradePlace ?? "";
                cntx.Accounts.Add(ac);
                cntx.SaveChanges();

                a.Id = ac.Id;
                return true;
            //}
            //catch (Exception e)
            //{
            //    SendExceptionMessage3(FullName, a.GetType().ToString(), "Add(Account):", a.ToString(), e);
            //    throw;
            //}
        }
        protected override IAccount Update(DbTradeContext cntx, IAccount ve, Account vi) // AddOrGet
        {
            ve.Id = vi.Id;
            ve.Code = vi.Code;
            ve.Name = vi.Name;
            ve.Alias = vi.Alias;
            ve.TradePlace = vi.TradePlace;

            ve.Balance = vi.Balance;
            return ve;
        }
        protected override bool Update(DbTradeContext cntx, Account vi, IAccount ve)
        {
            //try
            //{
            //if (ve == null)
            //{
            //    //throw new ArgumentNullException("ve", "AccountRepository32.Update(Account); " + "Account==Null");
            //    var e = new ArgumentNullException("ve", "AccountRepository32.Update(Account); " + "Account==Null");
            //    SendExceptionMessage3(FullName, "Account", "AccountRepository33.Update(Account)", "Account", e);
            //    return false;
            //}
            
                //vi.Id = ve.Id;
                //vi.Code = ve.Code;
                vi.Name = ve.Name;
                vi.Alias = ve.Alias;

                vi.Balance = ve.Balance;
                vi.Modified = DateTime.Now;

                cntx.SaveChanges();

                //if (IsUIEnabled)
                //    FireStorageChangedEvent("UI.Accounts", "Account", "Update", ve);

                //Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                //        FullName, Code, "Update(Account)", ve.Code, ve.ToString());
            //}
            //catch (Exception e)
            //{
            //    SendExceptionMessage3(FullName,
            //        ve == null ? "Account" : ve.GetType().ToString(), "AccountRepository32.Update(Account)",
            //        ve == null ? "Account" : ve.ToString(), e);
            //    throw;
            //}
            return true;
        }

        protected override DbTradeContext GetContext(string dataBaseName)
        {
            return new DbTradeContext(dataBaseName);
        }
    }

    public class TickerRepository33 :
                    TradeBaseRepository33<DbTradeContext, string, ITicker, Model.Ticker, ITickerDb>,
                    IEntityRepository<ITickerDb, ITicker>           //ITickerRepository
    {

        public override string Key { get { return Code; } }
        protected override DbTradeContext GetContext(string dataBaseName)
        {
            return new DbTradeContext(dataBaseName);
        }

        protected override Model.Ticker GetByKey(DbTradeContext cntx, string key)
        {
            return cntx.Tickers.FirstOrDefault(e => e.Key == key);
        }
        protected override Model.Ticker Get(DbTradeContext cntx, string key)
        {
            var a = cntx.Tickers.FirstOrDefault(e => e.Key == key)
                            ?? cntx.Tickers.FirstOrDefault(e => e.Code == key)
                                ?? cntx.Tickers.FirstOrDefault(e => e.Alias == key)
                                    ?? cntx.Tickers.FirstOrDefault(e => e.Name == key);
            return a;
        }
        protected override bool AddVal(DbTradeContext cntx, ITicker t)
        {
            var dt = DateTime.Now;
            //try
            //{
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

                cntx.Tickers.Add(ti);
                cntx.SaveChanges();

                t.Id = ti.Id;
                return true;
            //}
            //catch (Exception e)
            //{
            //    SendExceptionMessage3(FullName, t.GetType().ToString(), "Add(Ticker):", t.ToString(), e);
            //    throw;
            //}
        }

        protected override ITicker Update(DbTradeContext cntx, ITicker ve, Ticker vi) // AddOrGet - Register
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

        protected override bool Update(DbTradeContext cntx, Ticker vi, ITicker ve)
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

            vi.Modified = DateTime.Now;

            cntx.SaveChanges();

            return true;
        }
    }

    public class StrategyRepository33 :
                    TradeBaseRepository33<DbTradeContext, string, IStrategy, Model.Strategy, IStrategyDb>,
                    IEntityRepository<IStrategyDb, IStrategy>  // IStrategyRepository
    {
        public override string Key { get { return Code; } }
        protected override DbTradeContext GetContext(string dataBaseName)
        {
            return new DbTradeContext(dataBaseName);
        }

        protected override Model.Strategy GetByKey(DbTradeContext cntx, string key)
        {
            return cntx.Strategies.FirstOrDefault(e => e.Key == key);
        }
        protected override Model.Strategy Get(DbTradeContext cntx, string key)
        {
            var a = cntx.Strategies.FirstOrDefault(e => e.Key == key)
                            ?? cntx.Strategies.FirstOrDefault(e => e.Code == key)
                                ?? cntx.Strategies.FirstOrDefault(e => e.Alias == key)
                                    ?? cntx.Strategies.FirstOrDefault(e => e.Name == key);
            return a;
        }

        protected override bool AddVal(DbTradeContext cntx, IStrategy ve)
        {
            var dt = DateTime.Now;
            //try
            //{
                var s = new Strategy
                {
                    Key = ve.Key,

                    Name = ve.Name,
                    Alias = ve.Name,
                    Code = ve.Code,

                    TimeInt = ve.TimeInt,

                    Created = dt.MinValueToSql(),
                    Modified = dt.MinValueToSql(),

                    TickerId = ve.Ticker.Id,
                    AccountId = ve.Account.Id
                };

                cntx.Strategies.Add(s);
                cntx.SaveChanges();

                ve.Id = s.Id;
                return true;
            //}
            //catch (System.Data.EntityException)
            //{
            //    return false;
            //}
            //catch (Exception e)
            //{
            //    SendExceptionMessage3(FullName, ve.GetType().ToString(), "Add(Strategy):", ve.ToString(), e);
            //    throw;
            //}
        }

        protected override IStrategy Update(DbTradeContext cntx, IStrategy ve, Strategy vi) // AddOrGet - Register
        {
            ve.Id = vi.Id;
            ve.Name = vi.Name;
            ve.Code = vi.Code;
            ve.Alias = vi.Alias;
            ve.TimeInt = vi.TimeInt;
            return ve;
        }

        protected override bool Update(DbTradeContext cntx, Strategy vi, IStrategy ve)
        {
            var dt = DateTime.Now;
            //try
            //{
                vi.Key = ve.Key;
                vi.Name = ve.Name;
                vi.Code = ve.Code;
                vi.Alias = ve.Alias;

                vi.TimeInt = ve.TimeInt;

                vi.Modified = dt.MinValueToSql();
                cntx.SaveChanges();
                return true;
            //}
            //catch (System.Data.EntityException e)
            //{
            //    Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, FullName, ve.GetType().ToString(), "Update(vi,ve)",
            //            e.Message, ve.ToString());
            //    return false;
            //}
            //catch (Exception e)
            //{
            //    SendExceptionMessage3(FullName, ve == null ? "Strategy" : ve.GetType().ToString(), "Update(Strategy)",
            //        ve == null ? "Strategy" : ve.ToString(), e);
            //    throw;
            //}
        }
    }

    public class OrderRepository33 :
                    TradeBaseRepository33<DbTradeContext, string, IOrder3, Model.Order, IOrderDb>,
                    IEntityRepository<IOrderDb, IOrder3>    // IOrderRepository
    {
        //private object _locker;
        public override string Key { get { return Code; } }

        protected override DbTradeContext GetContext(string dataBaseName)
        {
            return new DbTradeContext(dataBaseName);
        }

        protected override Order GetByKey(DbTradeContext cntx, string key)
        {
            return cntx.Orders.FirstOrDefault(e => e.Key == key);
        }
        protected override Model.Order Get(DbTradeContext cntx, string key)
        {
            return cntx.Orders.FirstOrDefault(e => e.Key == key);
        }
        protected override bool AddVal(DbTradeContext cntx, IOrder3 ve)
        {
            var dt = DateTime.Now;
            //try
            //{
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

                    Created = dt,
                    Modified = dt,

                    OrderKey = ve.Key,
                    StrategyKey = ve.Strategy.Key,
                };
                cntx.Orders.Add(o);
                cntx.SaveChanges();

                ve.Id = o.Id;

                if (IsUIEnabled)
                    FireChangedEvent("UI.Orders", "Order", "AddNew", ve);

                //Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                //            FullName, ve.StratTicker, ve.Key ,"Add" + ve.GetType(), ve.ToString());

                return true;
            //}
            //catch (System.Data.EntityException e)
            //{
            //    Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, FullName, ve.GetType().ToString(), "AddVal(ve)",
            //            e.Message, ve.ToString());
            //    return false;
            //}
            //catch (Exception e)
            //{
            //    SendExceptionMessage3(FullName, ve.GetType().ToString(), "Add(Order):", ve.ToString(), e);
            //    throw;
            //}
        }

        protected override IOrder3 Update(DbTradeContext cntx, IOrder3 ve, Model.Order vi)
        {
            throw new NotImplementedException();
        }

        protected override bool Update(DbTradeContext cntx, Model.Order vi, IOrder3 ve)
        {
            const string method = "Order.Update()";
            //try
            //{
                if (ve == null || ve.Strategy == null)
                {
                    // throw new ArgumentNullException("ve", "OrderCore or OrderCore.Srategy==null");
                    var e = new ArgumentNullException("ve", FullName + "OrderCore or OrderCore.Srategy==null");
                    SendException(ve, method, e);
                    return false;
                }
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

                cntx.SaveChanges();

                if (IsUIEnabled)
                    FireChangedEvent("UI.Orders", "Order", "Update", ve);

                Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName,
                            ve.GetType() + ":" + ve.Key.WithSqBrackets0(),
                            "Update: " + ve.GetType() + ": " + ve.Key.WithSqBrackets0(),
                            ve.GetType() + ": " + ve.Key.WithSqBrackets0() + " " + ve.StratTicker, ve.ToString());

                return true;
            }
            //catch (System.Data.EntityException e)
            //{
            //    Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, FullName,
            //            ve == null ? "Order" : ve.GetType().ToString(), "Update(vi,ve)",
            //            e.Message,
            //            ve == null ? "Order" : ve.ToString());
            //    return false;
            //}
            //catch (ArgumentNullException e)
            //{
            //    SendExceptionMessage3(FullName, ve == null ? "Order" : ve.GetType().ToString(), "Update(Order):",
            //        ve == null ? "Order" : ve.ToString(), e);
            //    throw;
            //}
        //}
    }

    public class TradeRepository33 :
                    TradeBaseRepository33<DbTradeContext, string, ITrade3, Model.Trade, ITradeDb>,
                    IEntityRepository<ITradeDb, ITrade3>
    {
        public override string Key { get { return Code; } }
        protected override DbTradeContext GetContext(string dataBaseName)
        {
            return new DbTradeContext(dataBaseName);
        }

        protected override Model.Trade GetByKey(DbTradeContext cntx, string key)
        {
            return cntx.Trades.FirstOrDefault(e => e.Key == key);
        }
        protected override Model.Trade Get(DbTradeContext cntx, string key)
        {
            return cntx.Trades.FirstOrDefault(e => e.Key == key);
        }

        protected override bool AddVal(DbTradeContext cntx, ITrade3 ve)
        {
            var dt = DateTime.Now;
            //try
            //{
                var tra = new Model.Trade
                {
                    DT = ve.DT,

                    Number = ve.Number,
                    OrderNumber = ve.OrderNumber,

                    Operation = ve.Operation,
                    Quantity = ve.Quantity,
                    Price = ve.Price,

                    Key = ve.Key,
                    StrategyId = ve.Strategy.Id,

                    Created = dt,
                    Modified = dt,
                };
                cntx.Trades.Add(tra);
                cntx.SaveChanges();

                ve.Id = tra.Id;

                if (IsUIEnabled)
                    FireChangedEvent("UI.Trades", "Trade", "Add", ve);

                return true;
            //}
            //catch (System.Data.EntityException e)
            //{
            //    Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, FullName, ve.GetType().ToString(), "AddVal(ve)",
            //            e.Message, ve.ToString());
            //    return false;
            //}
            //catch (Exception e)
            //{
            //    SendExceptionMessage3(FullName, ve.GetType().ToString(), "Add(Trade):", ve.ToString(), e);
            //    throw;
            //}
        }

        protected override ITrade3 Update(DbTradeContext cntx, ITrade3 ve, Model.Trade vi)
        {
            throw new NotImplementedException();
        }

        protected override bool Update(DbTradeContext cntx, Model.Trade vi, ITrade3 ve)
        {
            throw new NotImplementedException();
        }
    }

    public class PositionRepository33 :
                    TradeBaseRepository33<DbTradeContext, string, IPosition2, Model.Position, IPositionDb>,
                    IEntityRepository<IPositionDb, IPosition2> //IPositionRepository
    {
        public override string Key { get { return Code; } }
        protected override DbTradeContext GetContext(string dataBaseName)
        {
            return new DbTradeContext(dataBaseName);
        }

        protected override Model.Position GetByKey(DbTradeContext cntx, string key)
        {
            return cntx.Positions.FirstOrDefault(e => e.Key == key);
        }
        protected override Model.Position Get(DbTradeContext cntx, string key)
        {
            return cntx.Positions.FirstOrDefault(e => e.Key == key);
        }

        protected override bool AddVal(DbTradeContext cntx, IPosition2 p)
        {
            const string method = "Positions.AddVal()";
            //try
            //{
                if (p.Strategy == null || p.Strategy.Id == 0)
                {
                    //throw new ArgumentNullException("p", "PositionRepository32.AddVal(cntx, Position)");
                    var e = new ArgumentNullException("p", "PositionRepository33.AddVal(cntx, Position)");
                    SendException(p, method, e);
                    return false;
                }
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
                cntx.Positions.Add(po);
                cntx.SaveChanges();

                p.Id = po.Id;

                if (IsUIEnabled)
                    FireChangedEvent("UI.Positions", "Current", "AddOrUpdate", p);

                return true;
            //}
            //catch (System.Data.EntityException e)
            //{
            //    Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, FullName, p.GetType().ToString(), "AddVal(ve)",
            //            e.Message, p.ToString());
            //    return false;
            //}
            //catch (ArgumentNullException e)
            //{
            //    SendExceptionMessage3(FullName,
            //        p.GetType().ToString(), "Add(PositionCurrent)", p.ToString(), e);
            //    throw;
            //}
        }

        protected override IPosition2 Update(DbTradeContext cntx, IPosition2 ve, Position vi)
        {
            const string method = "Position.Update(CorePos, DbPos)";
            //try
            //{
                //if (vi == null || vi.Strategy == null || vi.Strategy.Id == 0)
                //{
                //    // throw new ArgumentNullException("vi", "PositionDb is Invalid");
                //    var e = new ArgumentNullException("vi", "PositionDb is Invalid");
                //    SendException(ve, method, e);
                //    return null;
                //}
                if (ve == null || ve.Strategy == null || ve.Strategy.Id == 0)
                {
                    //throw new ArgumentNullException("ve", "PositionCore is Invalid");
                    var e = new ArgumentNullException("ve", "PositionCore is Invalid");
                    SendException(ve, method, e);
                    return null;
                }

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

                if (IsUIEnabled)
                    FireChangedEvent("UI.Positions", "Current", "AddOrUpdate", ve);

                Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                        FullName, Code, "Update(PositionCurrentCore)", ve.Strategy.StrategyTickerString, ve.ToString());
            //}
            //catch (System.Data.EntityException e)
            //{
            //    Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, FullName,
            //            ve == null ? "Position" : ve.GetType().ToString(), "Update(ve,vi)",
            //            e.Message,
            //            ve == null ? "Position" : ve.ToString());
            //    return ve;
            //}
            //catch (ArgumentNullException e)
            //{
            //    SendExceptionMessage3(FullName,
            //        ve == null ? "PositionCore" : ve.GetType().ToString(), "Update(PositionCurrent)",
            //        ve == null ? "PositionCore" : ve.ToString(), e);
            //    throw;
            //}
            return ve;
        }

        protected override bool Update(DbTradeContext cntx, Position vi, IPosition2 ve)
        {
            const string method = "Position.Update(DbPos, CorePos)";
            //try
            //{
                //if (ve == null || ve.Strategy == null || ve.Strategy.Id == 0)
                //    throw new ArgumentNullException("ve", "Repository32.Update(Position vi,ve);");
                if (ve == null || ve.Strategy == null || ve.Strategy.Id == 0)
                {
                    //throw new ArgumentNullException("ve", "ve", "Repository32.Update(Position vi,ve); "+"PositionCore is Invalid");
                    var e = new ArgumentNullException("ve", "Repository33.Update(Position vi,ve); " + "PositionCore is Invalid");
                    SendException(ve, method, e);
                    return false;
                }

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

                cntx.SaveChanges();

                if (IsUIEnabled)
                    FireChangedEvent("UI.Positions", "Current", "AddOrUpdate", ve);

                Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                        FullName, Code, "Update(PositionCurrentDb)", ve.Strategy.StrategyTickerString, ve.ToString());
            //}
            //catch (System.Data.EntityException e)
            //{
            //    Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, FullName,
            //            ve == null ? "Position" : ve.GetType().ToString(), "Update(vi,ve)",
            //            e.Message,
            //            ve == null ? "Position" : ve.ToString());
            //    return false;
            //}
            //catch (ArgumentNullException e)
            //{
            //    SendExceptionMessage3(FullName,
            //        ve == null ? "PositionCore" : ve.GetType().ToString(), "Update(PositionCurrent)",
            //        ve == null ? "PositionCore" : ve.ToString(), e);
            //    throw;
            //}
            return true;
        }
    }

    public class PositionTotalRepository33 :
                    TradeBaseRepository33<DbTradeContext, string, IPosition2, Model.Total, IPositionDb>,
                    IEntityRepository<IPositionDb, IPosition2> // IPositionTotalRepository
    {
        public override string Key { get { return Code; } }
        protected override DbTradeContext GetContext(string dataBaseName)
        {
            return new DbTradeContext(dataBaseName);
        }

        protected override Model.Total GetByKey(DbTradeContext cntx, string key)
        {
            return cntx.Totals.FirstOrDefault(e => e.Key == key);
        }
        protected override Model.Total Get(DbTradeContext cntx, string key)
        {
            return cntx.Totals.FirstOrDefault(e => e.Key == key);
        }

        protected override bool AddVal(DbTradeContext cntx, IPosition2 p)
        {
            const string method = "Totals.AddVal";
            //try
            //{
                if (p == null || p.Strategy == null || p.Strategy.Id == 0)
                {
                    //throw new ArgumentNullException("p", FullName + " AddVal(Total)");
                    var e = new ArgumentNullException("p", FullName + " AddVal(Total)");
                    SendException(p, method, e);
                    return false;
                }
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
                    PnL = p.PnL,
                    PnL3 = p.PnL3,

                    Key = p.Strategy.Key,

                    Created = dtnow,
                    Modified = dtnow
                };
                cntx.Totals.Add(po);
                cntx.SaveChanges();

                p.Id = po.Id;

                if (IsUIEnabled)
                    FireChangedEvent("UI.Positions", "Total", "AddOrUpdate", p);

                return true;
            //}
            //catch (System.Data.EntityException e)
            //{
            //    Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, FullName, p.GetType().ToString(), "AddVal(ve)",
            //            e.Message, p.ToString());
            //    return false;
            //}
            //catch (ArgumentNullException e)
            //{
            //    SendExceptionMessage3(FullName,
            //        p.GetType().ToString(), "Add(Total)", p.ToString(), e);
            //    throw;
            //}
        }

        protected override IPosition2 Update(DbTradeContext cntx, IPosition2 ve, Total vi)
        {
            const string method = "Update(CoreTot, DbTot)";
            //try
            //{
                //if (vi == null || vi.Strategy == null || vi.Strategy.Id == 0)
                //    throw new ArgumentNullException("vi", "TotalDb is Invalid");
                //if (ve == null || ve.Strategy == null || ve.Strategy.Id == 0)
                //    throw new ArgumentNullException("vi", "TotalCore is Invalid");

                if (ve == null || ve.Strategy == null || ve.Strategy.Id == 0)
                {
                    //throw new ArgumentNullException("ve", "ve", "Update(Position vi,ve); "+"PositionCore is Invalid");
                    var e = new ArgumentNullException("ve", FullName + " Update(Position ve,vi); " + "TotalCore is Invalid");
                    SendException(ve, method, e);
                    return null;
                }

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

                ve.PnL = vi.PnL;
                ve.PnL3 = vi.PnL3;

                if (IsUIEnabled)
                    FireChangedEvent("UI.Positions", "Total", "AddOrUpdate", ve);

                Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                        FullName, Code, "Update(Total)", ve.Strategy.StrategyTickerString, ve.ToString());
            //}
            //catch (System.Data.EntityException e)
            //{
            //    return ve;
            //}
            //catch (ArgumentNullException e)
            //{
            //    SendExceptionMessage3(FullName,
            //        ve == null ? "TotalCore" : ve.GetType().ToString(), "Update(Total)",
            //        ve == null ? "TotalCore" : ve.ToString(), e);
            //    throw;
            //}
            return ve;
        }

        protected override bool Update(DbTradeContext cntx, Total vi, IPosition2 ve)
        {
            const string method = "Update(DbTot, CoreTot)";
            try
            {
                //if (ve == null || ve.Strategy == null || ve.Strategy.Id == 0)
                //    throw new ArgumentNullException("ve");

                if (ve == null || ve.Strategy == null || ve.Strategy.Id == 0)
                {
                    //throw new ArgumentNullException("ve", "ve", "Update(Position vi,ve); "+"PositionCore is Invalid");
                    var e = new ArgumentNullException("ve", FullName + " Update(Position vi,ve); " + "TotalCore is Invalid");
                    SendException(ve, method, e);
                    return false;
                }

                vi.FirstTradeDT = ve.FirstTradeDT.MinValueToSql();
                vi.FirstTradeNumber = ve.FirstTradeNumber;

                vi.LastTradeDT = ve.LastTradeDT.MinValueToSql();
                vi.LastTradeNumber = ve.LastTradeNumber;

                vi.Operation = ve.Operation;
                vi.Quantity = ve.Quantity;
                vi.Status = ve.PosStatus;

                vi.Price1 = ve.Price1;
                vi.Price2 = ve.Price2;
                vi.PnL = ve.PnL;
                vi.PnL3 = ve.PnL3;

                vi.Modified = DateTime.Now;

                cntx.SaveChanges();

                if (IsUIEnabled)
                    FireChangedEvent("UI.Positions", "Total", "AddOrUpdate", ve);

                Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                        FullName, Code, "Update(Total)", ve.Strategy.StrategyTickerString, ve.ToString());
            }
            //catch (System.Data.EntityException e)
            //{
            //    Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, FullName,
            //            ve == null ? "Total" : ve.GetType().ToString(), "Update(vi,ve)",
            //            e.Message,
            //            ve == null ? "Total" : ve.ToString());
            //    return false;
            //}
            catch (ArgumentNullException e)
            {
                SendExceptionMessage3(FullName,
                    ve == null ? "TotalCore" : ve.GetType().ToString(), "Update(TotalDb)",
                    ve == null ? "TotalCore" : ve.ToString(), e);
                throw;
            }
            return true;
        }
    }

    public class DealRepository33 :
                    TradeBaseRepository33<DbTradeContext, string, IDeal, Model.Deal, IDealDb>,
                    IEntityRepository<IDealDb, IDeal>     //IDealRepository
    {
        public override string Key { get { return Code; } }
        protected override DbTradeContext GetContext(string dataBaseName)
        {
            return new DbTradeContext(dataBaseName);
        }

        protected override Model.Deal GetByKey(DbTradeContext cntx, string key)
        {
            return cntx.Deals.FirstOrDefault(e => e.Key == key);
        }
        protected override Model.Deal Get(DbTradeContext cntx, string key)
        {
            return cntx.Deals.FirstOrDefault(e => e.Key == key);
        }

        protected override bool AddVal(DbTradeContext cntx, IDeal p)
        {
            const string method = "AddVal(IDeal)";
            //try
            //{
                if (p == null || p.Strategy == null || p.Strategy.Id == 0)
                {
                    //throw new ArgumentNullException("p");
                    var e = new ArgumentNullException("p", FullName + " Deal is Inavalid");
                    SendException(p, method, e);
                    return false;
                }
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
                cntx.Deals.Add(po);
                cntx.SaveChanges();

                p.Id = po.Id;

                if (IsUIEnabled)
                    FireChangedEvent("UI.Deals", "Deal", "Add", p);

                Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName,
                    "Deal: LastTrade #" + p.LastTradeNumber.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                    "Add Deal #" + p.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                     p.Strategy.StrategyTickerString, p.ToString());

                return true;
            //}
            //catch (System.Data.EntityException e)
            //{
            //    Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, FullName, p.GetType().ToString(), "AddVal(ve)",
            //            e.Message, p.ToString());
            //    return false;
            //}
            //catch (ArgumentNullException e)
            //{
            //    SendExceptionMessage3(FullName,
            //        p.GetType().ToString(), "Add(Deal)", p.ToString(), e);
            //    throw;
            //}
        }

        protected override IDeal Update(DbTradeContext cntx, IDeal ve, Deal vi)
        {
            const string method = "Deal.Update(Core from Db)";
            //try
            //{
                //if (vi == null || vi.Strategy == null || vi.Strategy.Id == 0)
                //    throw new ArgumentNullException("vi", "DealDb is Invalid");
                if (ve == null || ve.Strategy == null || ve.Strategy.Id == 0)
                {
                    // throw new ArgumentNullException("vi", "DealCore is Invalid");
                    var e = new ArgumentNullException("ve", FullName + " DealCore is Invalid");
                    SendException(ve, method, e);
                    return ve;
                }
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

                if (IsUIEnabled)
                    FireChangedEvent("UI.Deals", "Deal", "Update", ve);

                Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                        FullName, Code, "Update(Deal)", ve.Strategy.StrategyTickerString, ve.ToString());
            //}
            //catch (System.Data.EntityException e)
            //{
            //    return ve;
            //}
            //catch (ArgumentNullException e)
            //{
            //    SendExceptionMessage3(FullName,
            //        ve == null ? "DealCore" : ve.GetType().ToString(), "Update(Deal)",
            //        ve == null ? "DealCore" : ve.ToString(), e);
            //    throw;
            //}
            return ve;
        }

        protected override bool Update(DbTradeContext cntx, Deal vi, IDeal ve)
        {
            const string method = "Deals.Update(DbDeal from CoreDeal)";
            //try
            //{
                //if (ve == null || ve.Strategy == null || ve.Strategy.Id == 0)
                //    throw new ArgumentNullException("ve");
                if (ve == null || ve.Strategy == null || ve.Strategy.Id == 0)
                {
                    // throw new ArgumentNullException("vi", "DealCore is Invalid");
                    var e = new ArgumentNullException("ve", FullName + " DealCore is Invalid");
                    SendException(ve, method, e);
                    return false;
                }

                vi.FirstTradeDT = ve.FirstTradeDT.MinValueToSql();
                vi.FirstTradeNumber = ve.FirstTradeNumber;

                vi.LastTradeDT = ve.LastTradeDT.MinValueToSql();
                vi.LastTradeNumber = ve.LastTradeNumber;

                vi.Operation = ve.Operation;
                vi.Quantity = ve.Quantity;
                vi.Status = ve.Status;

                vi.Price1 = ve.Price1;
                vi.Price2 = ve.Price2;

                cntx.SaveChanges();

                if (IsUIEnabled)
                    FireChangedEvent("UI.Deals", "Deal", "Update", ve);

                Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                        FullName, Code, "Update(Deal)", ve.Strategy.StrategyTickerString, ve.ToString());
            //}
            //catch (System.Data.EntityException e)
            //{
            //    Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, FullName,
            //            ve == null ? "Deal" : ve.GetType().ToString(), "Update(vi,ve)",
            //            e.Message,
            //            ve == null ? "Deal" : ve.ToString());
            //    return false;
            //}
            //catch (ArgumentNullException e)
            //{
            //    SendExceptionMessage3(FullName,
            //        ve == null ? "DealCore" : ve.GetType().ToString(), "Update(Deal)",
            //        ve == null ? "DealCore" : ve.ToString(), e);
            //    throw;
            //}
            return true;
        }
    }
}
