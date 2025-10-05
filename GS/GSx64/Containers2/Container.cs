using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GS.Containers;

namespace GS.Containers2
{
    public abstract class Container<TCollection, TKey, TValue>
    {
        protected TCollection ItemCollection { get; set; }
        protected readonly object LockItems = new object();

        public abstract IEnumerable<TValue> Items { get; }
        public abstract TValue GetByKey(TKey key);
        public abstract bool Contains(TValue t);
        public abstract void Add(TValue t);
        public abstract bool Remove(TValue t);

        //public IContainerItem GetByKey(TKey key)
        //{
        //    return GetByKey(key);
        //}

    //public virtual bool AddNew(T2 t)
    //{
    //    if (t == null || Contains(t))
    //        return false;
    //    Add(t);
    //    return true;
    //}

        public virtual bool AddNew(TValue v)
        {
            if(v == null) 
                return false;

            if(Contains(v))
                return true; 
            Add(v);
            return true;
        }
    }
    public abstract class ContainerItem : IContainerItem
    {
        public abstract string Key { get; }

        public bool Equals(ContainerItem other)
        {
            return string.Equals(Key, other.Key);
        }
        // IEqualityComparer
        //public override bool Equals(object o)
        //{
        //    if (ReferenceEquals(null, o)) return false;
        //    if (ReferenceEquals(this, o)) return true;
        //    if (o.GetType() != this.GetType()) return false;
        //    return Equals(o);
        //}
        public override int GetHashCode()
        {
            unchecked
            {
                return Key.GetHashCode();
            }
        }
    }

    public abstract class ListContainer<TKey, TValue> : Container<List<TValue>, TKey, TValue> where TValue : IContainerItem
    {
        protected ListContainer()
        {
            ItemCollection = new List<TValue>();
        }

        public long Count {
            get { return ItemCollection.Count; }
        }

        public override IEnumerable<TValue> Items {
            get
            {
                var il = new List<TValue>();
                lock (((ICollection) ItemCollection).SyncRoot)
                {
                    il.AddRange(ItemCollection.ToList());
                }
                return il;
            }
        }

        public override void Add(TValue t)
        {
            lock (((ICollection) ItemCollection).SyncRoot)
            {
                ItemCollection.Add(t);
            }
        }
        public override TValue GetByKey(TKey key)
        {
            TValue t;
            lock (((ICollection) ItemCollection).SyncRoot)
            {
                t = ItemCollection.FirstOrDefault(i => Equals(((IContainerItem)i).Key, key));
            }
            return t;
        }
        public override bool Contains(TValue t)
        {
            bool boo;
            lock (((ICollection) ItemCollection).SyncRoot)
            {
                boo = ItemCollection.Contains(t);
            }
            return boo;
        }
        public void RemoveAt(int index)
        {
            lock(((ICollection) ItemCollection).SyncRoot)
                ItemCollection.RemoveAt(index);
        }
        public override bool Remove(TValue t)
        {
            return t != null && ItemCollection.Remove(t);
        }
    }
    public abstract class SetContainer<TKey, TValue> : Container<HashSet<TValue>, TKey, TValue> where TValue : IContainerItem
    {
        protected SetContainer()
        {
            ItemCollection = new HashSet<TValue>(comparer: new ContainerItemEqualityComparer<TValue>());
        }
        public long Count
        {
            get { return ItemCollection.Count; }
        }
        public override IEnumerable<TValue> Items
        {
            get
            {
                var il = new List<TValue>();
                lock (LockItems)
                {
                    il.AddRange(ItemCollection.ToList());
                }
                return il;
            }
        }
        public override void Add(TValue t)
        {
            lock (LockItems)
            {
                ItemCollection.Add(t);
            }
        }

        public override TValue GetByKey(TKey key)
        {
            TValue t;
            lock (LockItems)
            {
                t = ItemCollection.FirstOrDefault(i => Equals(((IContainerItem)i).Key, key));
            }
            return t;
        }
        public override bool Contains(TValue t)
        {
            bool boo;
            lock (LockItems)
            {
                boo = ItemCollection.Contains(t);
            }
            return boo;
        }
        public override bool Remove(TValue t)
        {
            return t != null && ItemCollection.Remove(t);
        }
    }
    public abstract class DictionaryContainer<TKey, TValue> : Container<Dictionary<TKey, TValue>, TKey, TValue> 
        where TValue : IContainerItem
    {
        protected DictionaryContainer()
        {
            ItemCollection = new Dictionary<TKey, TValue>();
        }
        public long Count
        {
            get { return ItemCollection.Count; }
        }
        public override IEnumerable<TValue> Items
        {
            get
            {
                var il = new List<TValue>();
                lock (LockItems)
                {
                    il.AddRange(ItemCollection.Values.ToList());
                }
                return il;
            }
        }

        public override void Add(TValue t)
        {
            var key = ((IContainerItem) t).Key;
            lock (LockItems)
            {
                ItemCollection.Add(key, t);
            }
           // return true;
        }

        public override TValue GetByKey(TKey key)
        {
            TValue t;
            lock (LockItems)
            {
                ItemCollection.TryGetValue(key, out t);
            }
            return t;
        }
        public override bool Contains(TKey key)
        {
            bool boo;
            lock (LockItems)
            {
                boo = ItemCollection.ContainsKey(key);
            }
            return boo;
        }
        public override bool Remove(TValue t)
        {
            return t != null && ItemCollection.Remove(t.Key);
        }
    }

    public class ContainerItemEqualityComparer<TValue> : IEqualityComparer<TValue>  where TValue : IContainerItem

{
    public bool Equals(IContainerItem ci1, IContainerItem ci2)
    {
        return ci1 != null && ci2 != null && ci1.Equals(ci2);
    }

    public int GetHashCode(TValue t)
    {
        return t.Key.GetHashCode();
    }

    bool IEqualityComparer<TValue>.Equals(TValue x, TValue y)
    {
            return Equals(x, y);
    }

        //public int GetHashCode(TValue obj)
        //{
        //    return GetHashCode
        //}
}
}
