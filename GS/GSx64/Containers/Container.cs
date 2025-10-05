using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GS.Containers
{
    public enum ItemsOperation : short { Insert = +1, Remove = -1, Clear = 0}

    public abstract class Container<TCollection, TKey, TValue>
    {
        public EventHandler<ItemsEventArgs<TValue>> ItemsChangedEvent { get; set; }

        protected TCollection ItemCollection { get; set; }
        protected readonly object LockItems = new object();
        public abstract IEnumerable<TValue> Items { get; }
        
        protected abstract void ClearAll();
        public abstract TValue GetByKey(TKey key);
        public abstract TValue GetByKey(TValue t);
        protected abstract void Add(TValue t);

        protected abstract bool Remove(TValue t);
        public virtual bool RemoveByKey(TKey key)
        {
            TValue v;
            return (v = GetByKey(key)) != null && Remove(v);
        }

        public TValue Contains(TValue t)
        {
            if (t == null)
                return t;
            return GetByKey(t);
        }
        public virtual TValue AddNew(TValue v)
        {
            if(v == null) 
                return v;
            TValue rv;
            if( (rv = Contains(v)) != null )
                return rv;

            Add(v);
            RiseEvent(ItemsOperation.Insert, v);

            //if(ItemsChangedEvent != null)
            //    ItemsChangedEvent( this, new ItemsEventArgs<TValue>{ Operation = ItemsOperation.Insert, ContainerItem = v});
            return v;
        }

        public void Clear()
        {
            ClearAll();
            RiseEvent(ItemsOperation.Clear, default(TValue));
            //if (ItemsChangedEvent != null)
            //    ItemsChangedEvent(this, new ItemsEventArgs<TValue> { Operation = ItemsOperation.Clear, ContainerItem = default(TValue) });

        }

        protected void RiseEvent(ItemsOperation operation, TValue v)
        {
            if (ItemsChangedEvent != null)
                ItemsChangedEvent(this, new ItemsEventArgs<TValue> { Operation = operation, ContainerItem = v });
        }
    }
    public abstract class ContainerItem<TKey> : IContainerItem<TKey>
    {
        public abstract TKey Key { get; }

        public bool Equals(ContainerItem<TKey> other)
        {
            return Equals(Key, other.Key);
        }
        // IEqualityComparer
        //public override bool Equals(object o)
        //{
        //    if (ReferenceEquals(null, o)) return false;
        //    if (ReferenceEquals(this, o)) return true;
        //    if (o.GetType() != this.GetType()) return false;
        //    return Equals(o);
        //}


        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class ListContainer<TKey, TValue> : Container<List<TValue>, TKey, TValue> 
        where TValue : IContainerItem<TKey>
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

        protected override void ClearAll()
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                ItemCollection.Clear();
            }
        }

        protected override void Add(TValue t)
        {
            lock (((ICollection) ItemCollection).SyncRoot)
            {
                ItemCollection.Add(t);
            }
        }
        public override TValue GetByKey(TKey key)
        {
            var t = default(TValue);
            lock (((ICollection) ItemCollection).SyncRoot)
            {
                t = ItemCollection.FirstOrDefault(i => Equals(((IContainerItem<TKey>)t).Key, key));
            }
            return t;
        }
        public override TValue GetByKey(TValue t)
        {
            if (t == null) return default(TValue);

            TValue rt;
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                rt = ItemCollection.FirstOrDefault(i => Equals(i.Key, t.Key));
            }
            return rt;
        }
        public void RemoveAt(int index)
        {
            lock(((ICollection) ItemCollection).SyncRoot)
                ItemCollection.RemoveAt(index);
        }
       
        protected override bool Remove(TValue t)
        {
            if (t == null)
                return false;
            bool boo;
            lock (((ICollection) ItemCollection).SyncRoot)
            {
                boo = ItemCollection.Remove(t);
            }
            if( boo)
                RiseEvent(ItemsOperation.Remove, t);

            return boo;
        }
    }
    public abstract class SetContainer<TKey, TValue> : Container<HashSet<TValue>, TKey, TValue> where TValue : IContainerItem<TKey>
    {
        protected SetContainer()
        {
            ItemCollection = new HashSet<TValue>(comparer: new ContainerItemEqualityComparer<TValue, TKey>());
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
        protected override void Add(TValue t)
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
                t = ItemCollection.FirstOrDefault(i =>  Equals(((IContainerItem<TKey>)i).Key, key));
            }
            return t;
        }
        public override TValue GetByKey(TValue t)
        {
            TValue rt;
            lock (LockItems)
            {
                rt = ItemCollection.FirstOrDefault(i => Equals(i.Key, t.Key));
            }
            return rt;
        }

        protected override void ClearAll()
        {
            lock (LockItems)
            {
                ItemCollection.Clear();
            }
        }

        protected override bool Remove(TValue t)
        {
            if (t == null)
                return false;
            bool scs;
            lock (LockItems)
            {
                scs = ItemCollection.Remove(t);
            }
            if( scs )
                RiseEvent(ItemsOperation.Remove, t);

            return scs;
        }
    }
    public abstract class DictionaryContainer<TKey, TValue> : Container<Dictionary<TKey, TValue>, TKey, TValue> 
        where TValue : IContainerItem<TKey>
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

        protected override void Add(TValue t)
        {
            lock (LockItems)
            {
                ItemCollection.Add(t.Key, t);
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
        public override TValue GetByKey(TValue t)
        {
            TValue rt;
            lock (LockItems)
            {
                ItemCollection.TryGetValue(t.Key, out rt);
            }
            return rt;
        }
       
        protected override bool Remove(TValue t)
        {
            if (t == null)
                return false;
            bool scs;
            lock (LockItems)
            {
                scs = ItemCollection.Remove(t.Key);
            }
            if (scs)
                RiseEvent(ItemsOperation.Remove, t);

            return scs;
        }
    }

    public class ContainerItemEqualityComparer<TValue,TKey> : IEqualityComparer<TValue>  where TValue : IContainerItem<TKey>

{
    //public bool Equals(IContainerItem<TKey> ci1, IContainerItem<TKey> ci2)
    //{
    //    return ci1 != null && ci2 != null && ci1.Equals(ci2);
    //}

    public int GetHashCode(TValue t)
    {
        return t.Key.GetHashCode();
    }

    bool IEqualityComparer<TValue>.Equals(TValue x, TValue y)
    {
            return Equals(x.Key, y.Key);
    }

        //public int GetHashCode(TValue obj)
        //{
        //    return GetHashCode
        //}
   
}

    public class ItemsEventArgs<TValue> : EventArgs
    {
        public ItemsOperation Operation { get; set; }
        public TValue ContainerItem { get; set; }
    }
}
