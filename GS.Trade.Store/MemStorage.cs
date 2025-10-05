using System;
using System.Threading.Tasks;
using GS.Interfaces;
using GS.Trade.Storage;

namespace GS.Trade.TradeStorage
{
    public class MemStorage : GS.Trade.Storage.TradeStorage, ITradeStorage
    {
        protected ITrades3 Trades3 { get; set; }
        protected IOrders3 Orders3 { get; set; }

        public MemStorage()
        {
            Trades3 = new Trades.Trades3.Trades3();
            Orders3 = new Trades.Orders3.Orders3();
        }

        //public void Init(IEventLog evl)
        //{
        //    EventLog = evl;
        //}

        public bool IsEnabled { get; set; }

        public void Add(IOrder3 ord)
        {
            if (ord == null)
            {
                SendExceptionMessage("MemStorage.Add(IOrder3==Null)", "Null Reference", "");
                return;
                //  throw new NullReferenceException("MemStorage.Add(ITrade3) Failure: ");
            }
            Task.Factory.StartNew(() => 
            {
                try
                {
                Orders3.Add(ord);
                OnStorageChangedEvent(new Events.EventArgs
                {
                    Category = "UI.ORDERS",
                    Entity = "ORDER.COMPLETED",
                    Operation = "ADD",
                    Object = ord
                });
                }
                catch (Exception e)
                {
                    SendExceptionMessage("MemStorage.Add(IOrder3==Null)", e.Message, e.Source);
                    throw;
                }
            });
        }

        public void Add(ITrade3 tr)
        {
            if (tr == null)
            {
                SendExceptionMessage("MemStorage.Add(ITrade3==Null)", "Null Reference", "");
                return;
                //  throw new NullReferenceException("MemStorage.Add(ITrade3) Failure: ");
            }
            var t = Task.Factory.StartNew(() => 
            {
                try
                {
                Trades3.Add(tr);
                OnStorageChangedEvent( new Events.EventArgs
                {
                    Category = "UI.Trades",
                    Entity = "Trade",
                    Operation = "Add",
                    Object = tr

                });
                }
                catch (Exception e)
                {
                    SendExceptionMessage("MemStorage.Add(ITrade3==Null)", e.Message, e.Source);
                    throw;
                }
            });
            //try
            //{
            //    t.Wait();
            //}
            //catch (AggregateException ae)
            //{

            //    foreach (var e in ae.InnerExceptions)
            //    {
            //        SendExceptionMessage("MemStorage", e.Message);
            //    }
            //}
        }

        public IOrder GetOrderByKey(string key)
        {
            throw new NotImplementedException();
        }

        public IStrategy GetStrategyByKey(string key)
        {
            throw new NotImplementedException();
        }

        public string GetStrategyKeyFromOrder(string orderKey)
        {
            throw new NotImplementedException();
        }

        public IStrategy Register(IStrategy s)
        {
            throw new NotImplementedException();
        }

        public IAccount Register(IAccount a)
        {
            throw new NotImplementedException();
        }

        public ITicker Register(ITicker t)
        {
            throw new NotImplementedException();
        }

        public int SaveChanges(StorageOperationEnum operation, IPosition2 p)
        {
            if (p == null)
            {
                SendExceptionMessage("MemStorage.SaveChanges(IPosition2==NULL)", "Null reference", "");
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
                            OnStorageChangedEvent(new Events.EventArgs
                            {
                                Category = "UI.Positions",
                                Entity = "Current",
                                Operation = "Update",
                                Object = p
                            });
                            break;
                    }
                }
                catch (Exception e)
                {
                    SendExceptionMessage("MemStorage.SaveChanges(IPosition2)", e.Message, e.Source);
                    throw;
                }
            });
            //try
            //{
            //    t.Wait();
            //}
            //catch (AggregateException ae)
            //{
            //    foreach (var e in ae.InnerExceptions)
            //    {
            //        SendExceptionMessage("MemStorage", e.Message);
            //    }
            //}
            return 1;
        }

        public int SaveChanges(StorageOperationEnum operation, IPositionTotal2 p)
        {
            if (p == null)
            {   SendExceptionMessage("MemStorage.SaveChanges(IPositionTotal2==Null)", "Null reference", "");
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
            {   SendExceptionMessage("MemStorage.SaveTotalChanges(IPositionTotal2==Null)", "Null reference","");
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
            //try
            //{
            //    t.Wait();
            //}
            //catch (AggregateException ae)
            //{
            //    foreach (var e in ae.InnerExceptions)
            //    {
            //        SendExceptionMessage("MemStorage", e.Message);
            //    }
            //}
            return 1;
        }

        public int SaveDealChanges(StorageOperationEnum operation, IPosition2 p)
        {
            if (p == null)
            {
                SendExceptionMessage("MemStorage.SaveDealChanges(IPosition2==Null)", "Null reference", "");
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
                    SendExceptionMessage("MemStorage.SaveDealChanges(IPosition2)", e.Message,"");
                    throw;
                }
            });

            //try
            //{
            //    t.Wait();
            //}
            //catch (AggregateException ae)
            //{
            //    foreach (var e in ae.InnerExceptions)
            //    {
            //        SendExceptionMessage("MemStorage", e.Message);
            //    }
            //}
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
    }
}
