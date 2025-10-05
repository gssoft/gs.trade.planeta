using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Exceptions;
using GS.Extension;
using GS.Interfaces;
using GS.Process;
using GS.Queues;

namespace GS.Trade.Storage2
{
    //public interface ITradeStorage
    //{
    //    IPosition2 RegisterTotal(IPosition2 p);
    //}

    public class TradeStorage : GS.Storages.Storage<string, ITradeStorage>, ITradeStorage //, IHaveQueue
    {
        public TradeStorage()
        {
            //var t = typeof(TList);
            //var o = Activator.CreateInstance(t);
            //Items = o as Containers5.Container<TList, TestItem, string>;

            Items = new Containers5.DictionaryContainer<string, ITradeStorage>();
        }

        public override string Key
        {
            get { return Code; }
        }

        //public bool IsEnabled { get; set; }

        public bool IsPrimary { get; set; }
        public bool IsAsync { get; set; }

        private ITradeStorage _primary;

        public override void Init(GS.Interfaces.IEventLog evl)
        {
            base.Init(evl);
            _primary = Items.Items.FirstOrDefault(trs => trs.IsEnabled && trs.IsPrimary);
            if (_primary == null)
                SendExceptionMessage3(Name, "Init.SetPrimary", "Primary TradeStorage is not Found", Code, new NullReferenceException(""));
            else
                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, _primary.Name, "Set Primary", "Primary=" + _primary.Name, _primary.ToString());
        }

        public IEnumerable<ProcessManager2.ProcessProcedure> DeQueueProcesses
        {
            get
            {
                var l = new List<ProcessManager2.ProcessProcedure>();
                foreach (var trs in Items.Items.Where(t => t.IsEnabled))
                {
                    l.AddRange( trs.DeQueueProcesses );
                }
                return l;
            }
        }
        public IEnumerable<IEntityRepositoryBase> RepoItems
        {
            get
            {
                var l = new List<IEntityRepositoryBase>();
                foreach (var trs in Items.Items.Where(t => t.IsEnabled))
                {
                    l.AddRange(trs.RepoItems);
                }
                return l;
            }
        }
        public ITickerBase GetTickerByKey(string s)
        {
            try
            {
                if (s.HasNoValue())
                    throw new ArgumentNullException("s", "TradeStorage.GetAccountByKey(string s");

                foreach (var trs in Items.Items.Where(t => t.IsEnabled))
                {
                    var t = trs.GetTickerByKey(s);
                    if (t == null)
                        continue;
                    return t;
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, s, "GetTicker(s)", s, e);
                throw;
            }
            return null;
        }
        public ITickerBase GetTicker(string s)
        {
            try
            {
                if (s.HasNoValue())
                    throw new ArgumentNullException("s", "TradeStorage.GetAccountByKey(string s");

                foreach (var trs in Items.Items.Where(t => t.IsEnabled))
                {
                    var t = trs.GetTicker(s);
                    if (t == null)
                        continue;
                    return t;
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, s, "GetTicker(s)", s, e);
                throw;
            }
            return null;
        }

        public void Add(IGSException ex)
        {
            foreach (var trs in Items.Items.Where(t => t.IsEnabled))
            {
                trs.Add(ex);
            }
        }

        public IAccountBase GetAccountByKey(string key)
        {
            try
            {
                if (key.HasNoValue())
                    throw new ArgumentNullException("key","TradeStorage.GetAccountByKey(string key");

                foreach (var trs in Items.Items.Where(t => t.IsEnabled))
                {
                    var a = trs.GetAccountByKey(key);
                    if (a == null)
                        continue;
                    return a;
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, key, "GetAccountByKey(key)", key, e);
                throw;
            }
            return null;
        }
        public IAccountBase GetAccount(string key)
        {
            try
            {
                if (key.HasNoValue())
                    throw new ArgumentNullException("key", "TradeStorage.GetAccount(string str");

                foreach (var trs in Items.Items.Where(t => t.IsEnabled))
                {
                    var a = trs.GetAccount(key);
                    if (a == null)
                        continue;
                    return a;
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, key, "GetAccount(str)", key, e);
                throw;
            }
            return null;
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
                return _primary.GetStrategyKeyFromOrder(orderKey);
        }

        public IStrategy Register(IStrategy s)
        {
            if (s == null)
            {
                SendExceptionMessage3(Name, "TradeStorage.Register(IStrategy s==Null)", "Null Reference", Code, new Exception());
                return default(IStrategy);
                //  throw new NullReferenceException("TradeStorage.Add(ITrade3) Failure: ");
            }
            var ret = default(IStrategy);
            foreach (var trs in Items.Items.Where(t => t.IsEnabled))
            {
                var str = trs.Register(s);
                if (str != null)
                    ret = str;
            }
            return ret;
        }
        public IPosition2 Register(IPosition2 p)
        {
            try
            {
                if (p == null || p.Strategy == null ) //|| p.Strategy.Id == 0)
                    throw new ArgumentNullException("p", "TradeStorage.Register(IPosition); " +
                        "p == null || p.Strategy == null");

                var ret = default(IPosition2);
                foreach (var trs in Items.Items.Where(t => t.IsEnabled))
                {
                    var str = trs.Register(p);
                    if (str != null)
                        ret = str;
                }
                return ret;

            }
            catch(Exception e)
            {
                SendExceptionMessage3(FullName, "IPosition2",
                    "TradeStorage.Register(IPosition2): " + (p == null || p.Strategy == null ? "" : p.Strategy.StrategyTickerString),
                    p == null ? "Position" : p.ToString(), e);
                throw;
            }
        }
        public IPosition2 RegisterTotal(IPosition2 p)
        {
            try
            {
                if (p == null || p.Strategy == null) // || p.Strategy.Id == 0)
                    throw new ArgumentNullException("p",
                        "p == null || p.Strategy == null");

                var ret = default(IPosition2);
                foreach (var trs in Items.Items.Where(t => t.IsEnabled))
                {
                    var str = trs.RegisterTotal(p);
                    if (str != null)
                        ret = str;
                }
                return ret;

            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "IPosition2",
                    "TradeStorage.RegisterTotal(IPosition2): " + (p == null || p.Strategy == null ? "" : p.Strategy.StrategyTickerString),
                    p == null ? "Total" : p.ToString(), e);
                throw;
            }
        }

        public IAccount Register(IAccount a)
        {
            if (a == null)
            {
                SendExceptionMessage3(Name, "TradeStorage.Register(Account s==Null)", "Null Reference", Code, new Exception());
                return default(IAccount);
                //  throw new NullReferenceException("TradeStorage.Add(ITrade3) Failure: ");
            }
            var ret = default(IAccount);
            //_primary.Register(a);
            foreach (var trs in Items.Items.Where(t => t.IsEnabled))
            {
                var acc = trs.Register(a);
                if (acc != null)
                    ret = acc;
            }
            return ret;
        }

        public ITicker Register(ITicker t)
        {
            if (t == null)
            {
                SendExceptionMessage3(Name, "TradeStorage.Register(ITicker s==Null)", "Null Reference", Code, new Exception());
                return default(ITicker);
                //  throw new NullReferenceException("TradeStorage.Add(ITrade3) Failure: ");
            }
            var ret = default(ITicker);
            foreach (var trs in Items.Items.Where(ti => ti.IsEnabled))
            {
                var ti = trs.Register(t);
                if (ti != null)
                    ret = ti;
            }
            return ret;
        }

        public int SaveChanges(StorageOperationEnum operation, IPosition2 p)
        {
            if (p == null)
            {
                SendExceptionMessage3(Name, "TradeStorage.SaveChanges(IPosition2==NULL)", "Null reference", Code, new Exception());
                return -1;
                //throw new NullReferenceException("TradeStorage.SaveChanges(IPosition2) Failure:");
            }
            foreach (var trs in Items.Items.Where(t => t.IsEnabled))
            {
                trs.SaveChanges(operation, p);
            }
            return 1;
        }

        public int SaveChanges(StorageOperationEnum operation, IPositionTotal2 p)
        {
            throw new NotImplementedException();
        }

        public int SaveChanges(StorageOperationEnum operation, IDeal p)
        {
            if (p == null)
            {
                //SendExceptionMessage(Name, "TradeStorage.SaveDealChanges(IPosition2==Null)", "Null reference", Code);
                return -1;
                //throw new NullReferenceException("TradeStorage.SaveDealChanges(IPosition2) Failure: ");
            }
            foreach (var trs in Items.Items.Where(t => t.IsEnabled))
            {
                trs.SaveChanges(operation, p);
            }
            return 1;
        }

        public int SaveTotalChanges(StorageOperationEnum operation, IPosition2 p)
        {
            if (p == null)
            {
                SendExceptionMessage3(Name, "TradeStorage.SaveTotalChanges(IPositionTotal2==Null)", "Null reference", Code, new Exception());
                return -1;
                //throw new NullReferenceException("TradeStorage.SaveChanges(IPositionTotal2) Failure:");
            }
            foreach (var trs in Items.Items.Where(t => t.IsEnabled))
            {
                trs.SaveTotalChanges(operation, p);
            }
            return 1;

        }

        //public int SaveDealChanges(StorageOperationEnum operation, IPosition2 p)
        //{
        //    if (p == null)
        //    {
        //        //SendExceptionMessage(Name, "TradeStorage.SaveDealChanges(IPosition2==Null)", "Null reference", Code);
        //        return -1;
        //        //throw new NullReferenceException("TradeStorage.SaveDealChanges(IPosition2) Failure: ");
        //    }
        //    foreach (var trs in Items.Items.Where(t => t.IsEnabled))
        //    {
        //        trs.SaveDealChanges(operation, p);
        //    }
        //    return 1;
        //}

        public int SaveChanges(StorageOperationEnum operation, IOrder3 ord)
        {
            if (ord == null)
            {
                SendExceptionMessage3(Name, "TradeStorage.SaveChanges(IOrder3==NULL)", "Null reference", Code, new Exception());
                return -1;
                //throw new NullReferenceException("TradeStorage.SaveChanges(IPosition2) Failure:");
            }
            foreach (var trs in Items.Items.Where(t => t.IsEnabled))
            {
                trs.SaveChanges(operation, ord);
            }
            return 1;
        }

        public int SaveChanges(StorageOperationEnum operation, ITrade3 tr)
        {
            if (tr == null)
            {
                SendExceptionMessage3(Name, "TradeStorage.SaveChanges(ITrade3==NULL)", "Null reference", Code, new Exception());
                return -1;
                //throw new NullReferenceException("TradeStorage.SaveChanges(IPosition2) Failure:");
            }
            foreach (var trs in Items.Items.Where(t => t.IsEnabled))
            {
                trs.SaveChanges(operation, tr);
            }
            return 1;
        }

        public bool IsQueueEnabled { get; set; }
        public void DeQueueProcess()
        {
            foreach (var trs in Items.Items.Where(t => t.IsEnabled && t.IsQueueEnabled))
            {
                trs.DeQueueProcess();
            }
        }
    }
}
