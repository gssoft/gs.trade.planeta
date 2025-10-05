using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.Elements;
using GS.Events;
using GS.Interfaces;
using System.Collections.ObjectModel;

namespace GS.Collections
{
    public interface IGSCollection<in TKey, TValue>  // : ICollection<TValue>
    {
        IEnumerable<TValue> Items { get; }

        TValue GetByKey(TKey key);
        bool Contains(TValue t);

        bool Add(TValue t);
        bool AddNew(TValue t);
        TValue AddOrGet(TValue t);
        TValue AddOrUpdate(TValue t);
        
        bool Remove(TValue t);
        bool Remove(TKey key);
        void RemoveAt(int index);

        bool Update(TValue t);
        bool Update(TKey key, TValue t);

        void Clear();

        long Count { get; }

        //TValue GetByKey(TValue ci);

        //TValue AddOrGet(TValue t);
        //bool AddNew(TValue t);
    }
    public abstract class GSCollection<TElemenTItemKey, TList, TItem, TItemKey> :
                            Element1<TElemenTItemKey>, IGSCollection<TItemKey, TItem>
                            where TItem : class, IHaveKey<TItemKey>
    {
        public string Category { get; set; }
        public string Entity { get; set; }

        public int Capasity { get; set; }
        public int CapasityLimit { get; set; }

        public bool IsReversed { get; set; }

        public string EntityKey 
        {
            get { return Category + "@" + Entity; }
        }

        protected TList ItemCollection { get; set; }
        protected readonly object LockItems = new object();

        public abstract IEnumerable<TItem> Items { get; }
        public abstract TItem GetByKey(TItemKey key);
        public abstract bool Contains(TItem t);
        //public abstract bool Remove(TItem t);

        protected abstract bool AddVal(TItem t);
        public abstract bool Remove(TItemKey key);
        public abstract void RemoveAt(int index);
        //public abstract bool Update(TItemKey key, TItem t);
        public abstract void Clear();
        public abstract long Count { get; }

        //protected abstract int Count();

        public TItem GetByKey(TItem ci)
        {
            return GetByKey(ci.Key);
        }
              
        public virtual bool Add(TItem t)
        {
            try
            {
                if (t == null)
                    throw new NullReferenceException(FullName + "; Add(); " + EntityKey + "; TItem==Null");
                
                if (Capasity != 0 && (CapasityLimit + Capasity) <= Count)
                    Clear(Capasity);
                
                return AddVal(t);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, 
                        t == null ? EntityKey : t.GetType().ToString(), "Add()",
                        t == null ? EntityKey : t.ToString(), e);
                throw;
            }
        }
        public virtual bool AddNew(TItem t)
        {
            try
            {
                if (t == null)
                    throw new NullReferenceException(FullName + "; AddNewt(); " + EntityKey + "; TItem==Null");

                return !Contains(t) && Add(t);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName,
                            t == null ? EntityKey : t.GetType().ToString(), "AddNew",
                            t == null ? EntityKey : t.ToString(), e);
                throw;
            }
        }

        //public virtual TItem AddOrGet(TItem t)
        //{
        //    try
        //    {
        //        if( t== null)
        //            throw new NullReferenceException(FullName + "; AddOrGet(); " + EntityKey + "; TItem==Null");

        //        return Contains(t) ? t : (Add(t) ? t : default(TItem));
        //    }
        //    catch (Exception e)
        //    {
        //        SendExceptionMessage3(FullName,
        //                    t == null ? EntityKey : t.GetType().ToString(), "AddOrGet()",
        //                    t == null ? EntityKey : t.ToString(), e);
        //        throw;
        //    }
        //}
        public TItem AddOrGet(TItem t)
        {
            try
            {
                if( t== null)
                    throw new NullReferenceException(FullName + "; AddOrGet(); " + EntityKey + "; TItem==Null");

                    var i = GetByKey(t);
                    if (i != null)
                        return i;
                    Add(t);
                    return t;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName,
                            t == null ? EntityKey : t.GetType().ToString(), "AddOrGet()",
                            t == null ? EntityKey : t.ToString(), e);
                throw;
            }
        }
        public TItem AddOrUpdate(TItem t)
        {
            var i = GetByKey(t);
            if (i == null)
            {
                Add(t);
            }
            else
            {
                //Update(i.Key, t);
                Remove(i.Key);
                Add(t);
            }
            return t;
        }

        public bool Update(TItem t)
        {
            var i = GetByKey(t.Key);
            if (i == null)
                return false;

            return Remove(i.Key) && Add(t);
        }
        public bool Update(TItemKey key, TItem t)
        {
            var i = GetByKey(key);
            if (i == null)
                return false;

            return Remove(i.Key) && Add(t);
        }

        public bool Remove(TItem t)
        {
            return t != null && Count > 0 && Remove(t.Key);
        }

        private void Clear(int count)
        {
            while (Count > count)
            {
                RemoveAt((int)Count - 1);
            }
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName, EntityKey, "ClearSomeData()",
                $"Capasity={Capasity}; Limit={CapasityLimit}; ItemsCount={Count}", "");
        }
    }

    // ***************************** List *************************************************
    public class ListCollection<TItemKey, TItem> :
                    GSCollection<string, List<TItem>, TItem, TItemKey> // ,IGSCollection<TItemKey, TItem>
                    where TItem : class, IHaveKey<TItemKey>
    {
        public ListCollection()
        {
            ItemCollection = new List<TItem>();
        }

        public override void Clear()
        {
            lock (((ICollection) ItemCollection).SyncRoot)
            {
                ItemCollection.Clear();
            }
        }

        public override long Count
        {
            get { return ItemCollection.Count; }
        }

        public override IEnumerable<TItem> Items
        {
            get
            {
                lock (((ICollection)ItemCollection).SyncRoot)
                {
                    return ItemCollection.ToList();
                }
            }
        }

        protected override bool AddVal(TItem t)
        {
            try
            {
                lock (((ICollection)ItemCollection).SyncRoot)
                {
                    if (IsReversed)
                        ItemCollection.Insert(0, t);
                    else
                        ItemCollection.Add(t);
                }
                OnChangedEvent(new Events.EventArgs
                {
                    Category = Category,
                    Entity = Entity,
                    Operation = "Add",
                    Object = t
                });
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName, t.GetType().ToString(), "AddVal: " + EntityKey,
                            "Key=" + t.Key, t.ToString());
                return true;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, t.GetType().ToString(), "AddVal", t.ToString(), e);
                throw;
            }
        }
        public override TItem GetByKey(TItemKey key)
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                return ItemCollection.FirstOrDefault(i => i.Key.Equals(key));
            }
            //return t;
        }
        public override bool Contains(TItem t)
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                return GetByKey(t) != null;
                // return !GetByKey(t).Equals(null); // != null;
            }
        }

        public override void RemoveAt(int index)
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                ItemCollection.RemoveAt(index);
            }
        }
        //public override bool Remove(TItem t)
        //{
        //    //lock (((ICollection) ItemCollection).SyncRoot)
        //    //{
        //    //    return t != null && ItemCollection.Remove(t);
        //    //}
        //    return Remove(t.Key);
        //}
        public override bool Remove(TItemKey key)
        {
            var i = GetByKey(key);
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                return i != null && ItemCollection.Remove(i);
            }
        }

        public override string Key
        {
            get { return Code; }
        }
    }
    public class ObservableListCollection<TItemKey, TItem> :
                    GSCollection<string, ObservableCollection<TItem>, TItem, TItemKey> // ,IGSCollection<TItemKey, TItem>
                    where TItem : class, IHaveKey<TItemKey>
    {
        public ObservableListCollection()
        {
            ItemCollection = new ObservableCollection<TItem>();
        }

        public override void Clear()
        {
            lock (((ICollection) ItemCollection).SyncRoot)
            {
                ItemCollection = new ObservableCollection<TItem>();
            }
        }

        public override long Count
        {
            get { return ItemCollection.Count; }
        }

        public ObservableCollection<TItem> Collection {
            get { return ItemCollection; }
        }

        public override IEnumerable<TItem> Items
        {
            get
            {
                lock (((ICollection)ItemCollection).SyncRoot)
                {
                    return ItemCollection.ToList();
                }
            }
        }

        protected override bool AddVal(TItem t)
        {
            try
            {
                lock (((ICollection)ItemCollection).SyncRoot)
                {
                    if (IsReversed)
                        ItemCollection.Insert(0, t);
                    else
                        ItemCollection.Add(t);
                }
                OnChangedEvent(new Events.EventArgs
                {
                    Category = Category,
                    Entity = Entity,
                    Operation = "Add",
                    Object = t
                });
                Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName, t.GetType().ToString(), "AddVal: " + EntityKey,
                            "Key=" + t.Key, t.ToString());
                return true;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, t.GetType().ToString(), "AddVal", t.ToString(), e);
                throw;
            }
        }
        public override TItem GetByKey(TItemKey key)
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                return ItemCollection.FirstOrDefault(i => i.Key.Equals(key));
            }
            //return t;
        }
        public override bool Contains(TItem t)
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                return GetByKey(t) != null;
                // return !GetByKey(t).Equals(null); // != null;
            }
        }

        public override void RemoveAt(int index)
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                ItemCollection.RemoveAt(index);
            }
        }
        //public override bool Remove(TItem t)
        //{
        //    //lock (((ICollection) ItemCollection).SyncRoot)
        //    //{
        //    //    return t != null && ItemCollection.Remove(t);
        //    //}
        //    return Remove(t.Key);
        //}
        public override bool Remove(TItemKey key)
        {
            var i = GetByKey(key);
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                return i != null && ItemCollection.Remove(i);
            }
        }

        public override string Key => Code;
    }
    public class DictionaryCollection<TItemKey, TItem> :
                    GSCollection<string, Dictionary<TItemKey, TItem>, TItem, TItemKey> //, IGSCollection<TItemKey, TItem>
                    where TItem : class, IHaveKey<TItemKey>
    {
        public DictionaryCollection()
        {
            ItemCollection = new Dictionary<TItemKey, TItem>();
        }

        public override void Clear()
        {
            lock (LockItems)
                ItemCollection.Clear();
        }
        public bool IsEmpty => Count <= 0;
        public override long Count => ItemCollection.Count;

        public override IEnumerable<TItem> Items
        {
            get
            {
                lock (LockItems)
                {
                    return ItemCollection.Values.ToList();
                }
            }
        }
        protected override bool AddVal(TItem t)
        {
            try
            {
                lock (LockItems)
                {
                    ItemCollection.Add(t.Key, t);
                }
                OnChangedEvent(new Events.EventArgs
                {
                    Category = Category,
                    Entity = Entity,
                    Operation = "Add",
                    Object = t
                });
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName, t.GetType().ToString(), "AddVal: " + EntityKey,
                            "Key=" + t.Key, t.ToString());
                return true;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName,
                            t == null ? "Item" : t.GetType().ToString(), "AddNew",
                            t == null ? "Item" : t.ToString(), e);
                throw;
            }
        }

        public override TItem GetByKey(TItemKey key)
        {
            TItem t;
            lock (LockItems)
            {
                ItemCollection.TryGetValue(key, out t);
            }
            return t;
        }

        public override bool Contains(TItem t)
        {
            //bool boo;
            lock (LockItems)
            {
                return ItemCollection.ContainsKey(t.Key);
            }
            //return boo;
        }

        //public override bool Remove(TItem t)
        //{
        //    lock (LockItems)
        //    {
        //        return t != null && ItemCollection.Remove(t.Key);
        //    }
        //}
        public override bool Remove(TItemKey key)
        {
            //var i = GetByKey(key);
            //lock (LockItems)
            //{
            //    return i != null && ItemCollection.Remove(key);
            //}
            lock (LockItems)
            {
                return ItemCollection.Remove(key);
            }
        }
        public override void RemoveAt(int index)
        {
            if (Count <= index)
                return;
            //var t = Items.ToArray()[index];
            Remove(Items.ToArray()[index]);
        }

        //public override TItem AddOrGet(TItem t)
        //{
        //    var i = GetByKey(t);
        //    if (i != null)
        //        return i;
        //    Add(t);
        //    return t;
        //}
        

        //public override TElementKey Key
        //{
        //    get { throw new NotImplementedException(); }
        //}
        public override string Key
        {
            get { return Code; }
        }
    }
}
