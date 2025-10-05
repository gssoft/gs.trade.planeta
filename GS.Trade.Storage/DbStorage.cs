using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Events;
using GS.Extension;
using GS.Trade.DataBase;
using GS.Trade.DataBase.Dal;
using GS.Trade.DataBase.Init;
using GS.Trade.DataBase.Model;
using GS.Trade.Storage;

namespace GS.Trade.TradeStorage
{
    public class DbStorage : Storage.TradeStorage, ITradeStorage
    {
        private readonly DbTradeContext _db;
        public DbStorage()
        {
            //_db = new DbTradeContext();
            Database.SetInitializer(new InitDb());
        }

        public bool IsEnabled { get; set; }

        public void Add(IOrder3 ord)
        {
            var o = new GS.Trade.DataBase.Model.Order
            {
                Created = ord.Created,
                Modified = ord.Created,

                Number = ord.Number,

                OrderKey = ord.Key,
                StrategyKey = ord.Strategy.Key,
                StrategyId = ord.Strategy.Id,

                Status = ord.Status,

                Operation = (OrderOperationEnum)ord.Operation,
                OrderType = ord.OrderType,

                Quantity = ord.Quantity,
                Rest = ord.Rest,
                
                StopPrice = (decimal)ord.StopPrice,
                LimitPrice = (decimal)ord.LimitPrice

            };
            var db = new DbTradeContext();
            db.Orders.Add(o);
            db.SaveChanges();
            db.Dispose();
        }

        public void Add(ITrade3 tr)
        {
            if (tr.Strategy == null || tr.Strategy.Id == 0)
                throw new ArgumentNullException("tr");

            var db = new DbTradeContext();
            var tra = db.Trades.FirstOrDefault(t => t.Key == tr.Key);
            if (tra == null)
            {
                tra = new GS.Trade.DataBase.Model.Trade
                {
                    DT = tr.DT,

                    Number = tr.Number,
                    OrderNumber = tr.OrderNumber,

                    Operation = (TradeOperationEnum)tr.Operation,
                    Quantity = tr.Quantity,
                    Price = tr.Price,
                    
                    Key = tr.Key,

                    StrategyId = tr.Strategy.Id
                };
                db.Trades.Add(tra);
                db.SaveChanges();
                db.Dispose();

                tr.Id = tra.Id;
            }
            tra.Id = tra.Id;
            db.Dispose();
        }
        //public Trade GetTradeByKey
        public IOrder GetOrderByKey(string key)
        {
            throw new NotImplementedException();
            //return _db.Orders.FirstOrDefault(o => o.OrderKey == key);
        }

        public string GetStrategyKeyFromOrder(string orderKey)
        {
            var db = new DbTradeContext();
            var ord = db.Orders.FirstOrDefault(o => o.OrderKey == orderKey);
            db.Dispose();
            return ord != null
                ? ord.StrategyKey
                : null;
        }
       
        public IStrategy GetStrategyByKey(string key)
        {
            throw new NotImplementedException();
        }

        public IAccount Register(IAccount a)
        {
            //throw new NotImplementedException();
            var acc = new GS.Trade.DataBase.Model.Account
            {
                Name = a.Name,
                Alias = a.Name,
                Code = a.Code,
                TradePlace = a.TradePlace,
                Key = a.TradePlace.TrimUpper() + "@" + a.Code.TrimUpper()
            };
            var db = new DbTradeContext();
            var ac = db.Accounts.FirstOrDefault(aa => aa.Key == acc.Key);
            if (ac == null)
            {
                db.Accounts.Add(acc);
                db.SaveChanges();
                db.Dispose();

                a.Id = acc.Id;
                return a;
            }
            a.Id = ac.Id;
            db.Dispose();
            return a;
        }
        public ITicker Register(ITicker t)
        {
            var key = t.ClassCode.TrimUpper() + "@" + t.Code.TrimUpper();

            var db = new DbTradeContext();
            var ti = db.Tickers.FirstOrDefault(tt => tt.Key == key);
            if (ti == null)
            {
                var dt = DateTime.Now;
                ti = new Ticker
                {
                    Name = t.Name,
                    TradeBoard = t.ClassCode,
                    Code = t.Code,

                    BaseContract = t.BaseContract,

                    Decimals = t.Decimals,
                    MinMove = t.MinMove,

                    Key = t.ClassCode.TrimUpper() + "@" + t.Code.TrimUpper(),

                    Created = dt,
                    Modified = dt,

                    LaunchDate = dt,
                    ExpireDate = dt
                };

                db.Tickers.Add(ti);
                db.SaveChanges();
                db.Dispose();

                t.ID = ti.Id;
                return t;
            }
            db.Dispose();
            t.ID = ti.Id;
            return t;

        }

        public int SaveChanges(StorageOperationEnum operation, IPosition2 p)
        {
            if (p == null)
            {
                SendExceptionMessage(Name, "MemStorage.SaveChanges(IPosition2==NULL)", "Null reference");
                return -1;
                //throw new NullReferenceException("MemStorage.SaveChanges(IPosition2) Failure:");
            }
            var t = Task.Factory.StartNew(() =>
            {
                try
                {
                    switch (operation)
                    {
                        case StorageOperationEnum.Update:
                            FireEvent("UI.Positions", "Current", "Update", p);
                            break;
                    }
                }
                catch (Exception e)
                {
                    SendExceptionMessage("MemStorage.SaveChanges(IPosition2)", e.Message, e.Source);
                    throw;
                }
            });
            return 1;
        }

        public int SaveChanges(StorageOperationEnum operation, IPositionTotal2 p)
        {
            if (p == null)
            {
                SendExceptionMessage(Name, "MemStorage.SaveChanges(IPositionTotal2==Null)", "Null reference");
                return -1;
                //throw new NullReferenceException("MemStorage.SaveChanges(IPositionTotal2) Failure:");
            }
            Task.Factory.StartNew(() =>
            {
                try
                {
                    switch (operation)
                    {
                        case StorageOperationEnum.Update:
                            OnStorageChangedEvent(new Events.EventArgs
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
                    SendExceptionMessage("MemStorage.SaveChanges(IPositionTotal2)", e.Message, "");
                    throw;
                }

            });
            return 1;
        }

        public int SaveChanges(StorageOperationEnum operation, IDeal p)
        {
            if (p == null)
            {
                SendExceptionMessage("MemStorage.SaveChanges(IDeal==Null)", "Null reference", "");
                return -1;
                //throw new NullReferenceException("MemStorage.SaveChanges(IDeal) Failure: ");
            }
            Task.Factory.StartNew(() =>
            {
                try
                {
                    OnStorageChangedEvent(new Events.EventArgs
                    {
                        Category = "Deals",
                        Entity = "Deal",
                        Operation = "Add",
                        Object = p
                    });
                }
                catch (Exception e)
                {
                    SendExceptionMessage("MemStorage.SaveChanges(IDeal)", e.Message, "");
                    throw;
                }
            });
            return 1;
        }

        public int SaveTotalChanges(StorageOperationEnum operation, IPosition2 p)
        {
            if (p == null)
            {
                SendExceptionMessage("MemStorage.SaveTotalChanges(IPositionTotal2==Null)", "Null reference", "");
                return -1;
                //throw new NullReferenceException("MemStorage.SaveChanges(IPositionTotal2) Failure:");
            }
            var t = Task.Factory.StartNew(() =>
            {
                try
                {
                    switch (operation)
                    {
                        case StorageOperationEnum.Update:
                            OnStorageChangedEvent(new Events.EventArgs
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
                    SendExceptionMessage("MemStorage.SaveTotalChanges(IPositionTotal2)", e.Message, e.Source);
                    throw;
                }
            });
            return 1;
        }

        public int SaveDealChanges(StorageOperationEnum operation, IPosition2 p)
        {
            if (p == null)
            {
                SendExceptionMessage("DbStorage.SaveDealChanges(IPosition2==Null)", "Null reference", "");
                return -1;
                //throw new NullReferenceException("MemStorage.SaveDealChanges(IPosition2) Failure: ");
            }
            var t = Task.Factory.StartNew(() =>
            {
                try
                {
                    OnStorageChangedEvent(new Events.EventArgs
                    {
                        Category = "Deals",
                        Entity = "Deal",
                        Operation = "Add",
                        Object = p
                    });
                }
                catch (Exception e)
                {
                    SendExceptionMessage("MemStorage.SaveDealChanges(IPosition2)", e.Message, "");
                    throw;
                }
            });
            return 1;
        }

        public int SaveChanges(StorageOperationEnum operation, IOrder3 ord)
        {
            throw new NotImplementedException();
        }

        public int SaveChanges(StorageOperationEnum operation, ITrade3 p)
        {
            throw new NotImplementedException();
        }

        public IStrategy Register(IStrategy s)
        {
            if(s.Ticker.ID == 0 || s.Account.Id == 0)
                throw new ArgumentNullException("s");

            var db = new DbTradeContext();
            var stra = db.Strategies.FirstOrDefault(st => st.Key == s.Key);
            if (stra == null)
            {
                var dt = DateTime.Now;
                stra = new Strategy
                {
                    Name = s.Name,
                    Alias = s.Name,
                    
                    Code = s.Code,

                    Key = s.Key,

                    TickerId = s.Ticker.ID,
                    AccountId = s.Account.Id 
                };

                db.Strategies.Add(stra);
                db.SaveChanges();
                db.Dispose();

                s.Id = stra.Id;
                return s;
            }
            db.Dispose();
            s.Id = stra.Id;
            return s;
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public void OnExceptionEvent(IEventArgs e)
        {
            throw new NotImplementedException();
        }

        public IElement1<string> Parent { get; set; }
    }
}
