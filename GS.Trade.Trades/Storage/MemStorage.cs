using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Events;
using GS.Exceptions;
using GS.Extension;
using GS.Interfaces;
using GS.Process;
using GS.Trade.Storage2;
using EventArgs = GS.Events.EventArgs;
using System.Xml.Serialization;

namespace GS.Trade.Trades.Storage
{
    public class MemStorage : Element1<string>, ITradeStorage
    {
        //public bool IsEnabled { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsAsync { get; set; }

        protected ITrades3 Trades3 { get; set; }
        protected IOrders3 Orders3 { get; set; }

        protected ITickers Tickers;

        public MemStorage()
        {
            Trades3 = new Trades3.Trades3();
            Orders3 = new Orders3.Orders3();
            // Tickers = new Tickers()
        }
        //public override void Init(GS.Interfaces.IEventLog eventLog)
        //{
        //    base.Init(eventLog);

        //}

        public IAccountBase GetAccountByKey(string s)
        {
            throw new NotImplementedException();
        }

        public IAccountBase GetAccount(string s)
        {
            return null;
        }

        public ITickerBase GetTickerByKey(string key)
        {
            return null;
        }

        public ITickerBase GetTicker(string s)
        {
            // throw new NotImplementedException();
            return null;
        }

        public void Add(IGSException ex)
        {
            throw new NotImplementedException();
        }

        public void Add(IOrder3 ord)
        {
            if (ord == null)
            {
                SendExceptionMessage3(Name, "MemStorage.Add(IOrder3==Null)", "Null Reference", Code, new NullReferenceException(""));
                return;
                //  throw new NullReferenceException("MemStorage.Add(ITrade3) Failure: ");
            }
            Task.Factory.StartNew(() => 
            {
                try
                {
                Orders3.Add(ord);
                OnChangedEvent(new Events.EventArgs
                {
                    Category = "UI.ORDERS",
                    Entity = "ORDER.COMPLETED",
                    Operation = "ADD",
                    Object = ord
                });
                }
                catch (Exception e)
                {
                    SendExceptionMessage3(Name, "MemStorage.Add(IOrder3==Null)", "", "", e);
                    throw;
                }
            });
        }

        public void Add(ITrade3 tr)
        {
            if (tr == null)
            {
                SendExceptionMessage3(Name, "MemStorage.Add(ITrade3==Null)", "Null Reference", Code, new NullReferenceException(""));
                return;
                //  throw new NullReferenceException("MemStorage.Add(ITrade3) Failure: ");
            }
            var t = Task.Factory.StartNew(() => 
            {
                try
                {
                Trades3.Add(tr);
                OnChangedEvent( new Events.EventArgs
                {
                    Category = "UI.Trades",
                    Entity = "Trade",
                    Operation = "Add",
                    Object = tr

                });
                }
                catch (Exception e)
                {
                    SendExceptionMessage3(Name, "MemStorage.Add(ITrade3==Null)", "", "", e);
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
            Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Code, "Register(Strategy)", s.StrategyTickerString, s.ToString());
            return null;
        }

        public IAccount Register(IAccount a)
        {
            Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Code, "Register(Account)", a.Code, a.ToString());
            return null;
        }

        public ITicker Register(ITicker t)
        {
            Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Code, "Register(Ticker)", t.Code, t.ToString());
            return null;
        }

        public IPosition2 Register(IPosition2 p)
        {
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Code, "Register(Position)", p.Key, p.ToString());
            return null;
        }

        public IPosition2 RegisterTotal(IPosition2 p)
        {
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Code, "RegisterTotal(Total)", p.Key, p.ToString());
            return null;
        }

        public int SaveChanges(StorageOperationEnum operation, IPosition2 p)
        {
            if (p == null)
            {
                SendExceptionMessage3(Name, "MemStorage.SaveChanges(IPosition2==NULL)", "Null reference", Code, new NullReferenceException(""));
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
                            OnChangedEvent(new Events.EventArgs
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
                    SendExceptionMessage3(Name, "MemStorage.SaveChanges(IPosition2)","","", e);
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
            {   SendExceptionMessage3(Name, "MemStorage.SaveChanges(IPositionTotal2==Null)", "Null reference", Code, new NullReferenceException(""));
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
                            var pcl = p.Clone();
                            OnChangedEvent(new Events.EventArgs
                            {
                                Category = "UI.Positions",
                                Entity = "Total",
                                Operation = "Update",
                                Object = pcl
                            });
                            break;
                    }
                }
                catch (Exception e)
                {
                    SendExceptionMessage3(Name,"MemStorage.SaveChanges(IPositionTotal2)", e.Message, Code, e);
                    throw;
                }
                
            });
            return 1;
        }

        public int SaveChanges(StorageOperationEnum operation, IDeal p)
        {
            if (p == null)
            {
                SendExceptionMessage3(Name, "MemStorage.SaveChanges(IDeal==Null)", "Null reference", Code, new NullReferenceException());
                return -1;
                //throw new NullReferenceException("MemStorage.SaveChanges(IDeal) Failure: ");
            }
            Task.Factory.StartNew(() =>
            {
                try
                {
                    OnChangedEvent(new Events.EventArgs
                    {
                        Category = "Deals",
                        Entity = "Deal",
                        Operation = "Add",
                        Object = p
                    });
                }
                catch (Exception e)
                {
                    SendExceptionMessage3(Name, "MemStorage.SaveChanges(IDeal)", e.Message, Code, new NullReferenceException(""));
                    throw;
                }
            });
            return 1;
        }

        public int SaveTotalChanges(StorageOperationEnum operation, IPosition2 p)
        {
           if (p == null)
           {
               SendExceptionMessage3(Name, "MemStorage.SaveTotalChanges(IPositionTotal2==Null)", "Null reference", Code, new NullReferenceException(""));
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
                            var pcl = p.Clone();
                            OnChangedEvent(new Events.EventArgs
                            {
                                Category = "UI.Positions",
                                Entity = "Total",
                                Operation = "Update",
                                Object = pcl
                            });
                            break;
                    }
                 }
                catch (Exception e)
                {
                    SendExceptionMessage3(Name, "MemStorage.SaveTotalChanges(IPositionTotal2)", "", "", e);
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
        [XmlIgnore]
        public IEnumerable<ProcessManager2.ProcessProcedure> DeQueueProcesses { get; set; }
        [XmlIgnore]
        public IEnumerable<IEntityRepositoryBase> RepoItems { get; set; }

        public int SaveDealChanges(StorageOperationEnum operation, IPosition2 p)
        {
            if (p == null)
            {
                SendExceptionMessage3(Name, "MemStorage.SaveDealChanges(IPosition2==Null)", "Null reference", Code, new NullReferenceException(""));
                return -1;
                //throw new NullReferenceException("MemStorage.SaveDealChanges(IPosition2) Failure: ");
            }
            var t = Task.Factory.StartNew(() =>
            {
                try
                {
                    OnChangedEvent(new Events.EventArgs
                    {
                        Category = "Deals",
                        Entity = "Deal",
                        Operation = "Add",
                        Object = p
                    });
                }
                catch (Exception e)
                {
                    SendExceptionMessage3(Name, "MemStorage.SaveDealChanges(IPosition2)", e.Message, Code, e);
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
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, ord.StratTicker,
                                            "SaveChanges(Order)", ord.ShortInfo, ord.ToString());
            return +1;
        }

        public int SaveChanges(StorageOperationEnum operation, ITrade3 t)
        {
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, t.StratTicker,
                                            "SaveChanges(Trade)", t.ShortInfo, t.ToString());
            return +1;
        }


        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override string Key => Code;

        public bool IsQueueEnabled { get; set; }
        public void DeQueueProcess()
        {
            throw new NotImplementedException();
        }
    }
}
