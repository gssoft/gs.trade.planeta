using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using EventArgs = GS.Events.EventArgs;


namespace GS.Containers3
{
    public abstract class Container<T1, TKey>
    {
        public event EventHandler<Events.EventArgs> ContainerEvent;

        protected virtual void OnContainerEvent(Events.EventArgs args)
        {
            EventHandler<EventArgs> handler = ContainerEvent;
            if (handler != null && args != null)
                handler(this, args);
        }

        protected virtual void OnContainerEvent(string operation, IContainerItem<TKey> i )
        {
            EventHandler<EventArgs> handler = ContainerEvent;
            if (handler == null)
                return;
            var eargs = GetEventArgs();
            if (eargs == null)
                return;
            eargs.Operation = operation;
            eargs.Object = i;
            handler(this, eargs);
        }

        protected T1 ItemCollection { get; set; }
        protected readonly object LockItems = new object();

        public abstract IEnumerable<IContainerItem<TKey>> Items { get; }
        public abstract IContainerItem<TKey> GetByKey(TKey key);
        public abstract bool Contains(IContainerItem<TKey> t);
        public abstract bool Add(IContainerItem<TKey> t);
        public abstract bool Remove(IContainerItem<TKey> t);
        public abstract bool Remove(TKey key);
        public abstract bool Update(TKey key, IContainerItem<TKey> t);

        public IContainerItem<TKey> GetByKey(IContainerItem<TKey> ci)
        {
            return GetByKey(ci.Key);
        }

        //public virtual bool AddNew(T2 t)
    //{
    //    if (t == null || Contains(t))
    //        return false;
    //    Add(t);
    //    return true;
    //}

        public virtual IContainerItem<TKey> AddNew(IContainerItem<TKey> t)
        {
            return t == null ? null : (Contains(t) ? t : (Add(t) ? t : null));
        }
        public virtual Events.EventArgs GetEventArgs()
        {
            return null;
        }
    }
    public abstract class ContainerItem<TKey> : IContainerItem<TKey>
    {
        public abstract TKey Key { get; }

        public bool Equals(ContainerItem<TKey> other)
        {
            return string.Equals(Key, other.Key);
        }
        // IEqualityComparer
        public override bool Equals(object o)
        {
            if (ReferenceEquals(null, o)) return false;
            if (o.GetType() != this.GetType()) return false;
            if (ReferenceEquals(this, o)) return true;
            return Equals(this, o);
        }
        //public bool Equals(IContainerItem<TKey> o)
        //{
        //    return o != null && Key.Equals(o.Key);
        //}
        //public bool Equals(TKey key)
        //{
        //    return Key.Equals(key);
        //}
        
        public override int GetHashCode()
        {
            unchecked
            {
                return Key.GetHashCode();
            }
        }

        public IContainer Container { get;  set; }
    }
// ***************************** List *************************************************
    public  class ListContainer<TKey> : Container<List<IContainerItem<TKey>>, TKey>
    {
        public ListContainer()
        {
            ItemCollection = new List<IContainerItem<TKey>>();
        }

        public long Count
        {
            get { return ItemCollection.Count; }
        }

        public override IEnumerable<IContainerItem<TKey>> Items
        {
            get
            {
                //var il = new List<IContainerItem<TKey>>();
                //lock (((ICollection)ItemCollection).SyncRoot)
                //{
                //    il.AddRange(ItemCollection.ToList());
                //}
                lock (((ICollection)ItemCollection).SyncRoot)
                {
                return ItemCollection.ToList();
                }
            }
        }

        public override bool Add(IContainerItem<TKey> t)
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                ItemCollection.Add(t);
            }
           //OnContainerEvent("New", t);
            return true;
        }
        public override IContainerItem<TKey> GetByKey(TKey key)
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                return ItemCollection.FirstOrDefault(i => i.Key.Equals(key));
            }
            //return t;
        }
        public override bool Contains(IContainerItem<TKey> t)
        {
            bool boo;
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                // boo = ItemCollection.Contains(t);
                return GetByKey(t) != null;
            }
            //return boo;
        }
        public override bool Update(TKey key, IContainerItem<TKey> t)
        {
            var i = GetByKey(key);
            if (i == null)
                return false;

            return Remove(i) && Add(t);
        }
        public void RemoveAt(int index)
        {
            lock (((ICollection) ItemCollection).SyncRoot)
            {
                ItemCollection.RemoveAt(index);
            }
        }
        public override bool Remove(IContainerItem<TKey> t)
        {
            //lock (((ICollection) ItemCollection).SyncRoot)
            //{
            //    return t != null && ItemCollection.Remove(t);
            //}
            return Remove(t.Key);
        }

        public override bool Remove(TKey key)
        {
            var i = GetByKey(key);
            lock (((ICollection) ItemCollection).SyncRoot)
            {
                return i != null && ItemCollection.Remove(i);
            }
        }

        //public virtual Events.EventArgs GetEventArgs()
        //{
        //    return null;
        //}
    }

    // Set ********************************** SET **************************
    public abstract class SetContainer<TKey> : Container<HashSet<IContainerItem<TKey>>, TKey>
    {
        protected SetContainer()
        {
            ItemCollection = new HashSet<IContainerItem<TKey>>(comparer: new ContainerItemEqualityComparer<TKey>());
        }
        public long Count
        {
            get { return ItemCollection.Count; }
        }
        public override IEnumerable<IContainerItem<TKey>> Items
        {
            get
            {
                var il = new List<IContainerItem<TKey>>();
                lock (LockItems)
                {
                    il.AddRange(ItemCollection.ToList());
                }
                return il;
            }
        }
        public override bool Add(IContainerItem<TKey> t)
        {
            lock (LockItems)
            {
                ItemCollection.Add(t);
            }
            return true;
        }
        public override bool Contains(IContainerItem<TKey> t)
        {
            bool boo;
            lock (LockItems)
            {
                boo = ItemCollection.Contains(t);
            }
            return boo;
        }
        public override bool Update(TKey key, IContainerItem<TKey> t)
        {
            var i = GetByKey(key);
            if (i == null)
                return false;

            return Remove(i) && Add(t);
        }

        public override bool Remove(IContainerItem<TKey> t)
        {
            lock (LockItems)
            {
                return t != null && ItemCollection.Remove(t);
            }
        }
        public override bool Remove(TKey key)
        {
            var i = GetByKey(key);
            return i != null && Remove(i);
        }
    }
    // ******************************** Dictionary ***********************************
    public abstract class DictionaryContainer<TKey> :
        Containers3.Container<Dictionary<TKey, IContainerItem<TKey>>, TKey>
    {
        protected DictionaryContainer()
        {
            ItemCollection = new Dictionary<TKey, IContainerItem<TKey>>();
        }

        public long Count
        {
            get { return ItemCollection.Count; }
        }

        public override IEnumerable<IContainerItem<TKey>> Items
        {
            get
            {
                var il = new List<IContainerItem<TKey>>();
                lock (LockItems)
                {
                    il.AddRange(ItemCollection.Values.ToList());
                }
                return il;
            }
        }

        public override bool Add(IContainerItem<TKey> t)
        {
            lock (LockItems)
            {
                ItemCollection.Add(t.Key, t);
            }
            return true;
        }

        public override IContainerItem<TKey> GetByKey(TKey key)
        {
            IContainerItem<TKey> t;
            lock (LockItems)
            {
                ItemCollection.TryGetValue(key, out t);
            }
            return t;
        }

        public override bool Contains(IContainerItem<TKey> t)
        {
            bool boo;
            lock (LockItems)
            {
                boo = ItemCollection.ContainsKey(t.Key);
            }
            return boo;
        }
        public override bool Update(TKey key, IContainerItem<TKey> t)
        {
            var i = GetByKey(key);
            if (i == null)
                return false;
           
            return Remove(i) && Add(t);
        }

        public override bool Remove(IContainerItem<TKey> t)
        {
            lock (LockItems)
            {
                return t != null && ItemCollection.Remove(t.Key);
            }
        }
        public override bool Remove(TKey key)
        {
            var i = GetByKey(key);
            lock (LockItems)
            {
                return i != null && ItemCollection.Remove(key);
            }
        }

        public IContainerItem<TKey> AddOrGet(IContainerItem<TKey> t)
        {
            var i = GetByKey(t);
            if (i != null) return i;
            Add(t);
            return t;
        }
        public IContainerItem<TKey> AddOrUpdate(IContainerItem<TKey> t)
        {
            var i = GetByKey(t);
            if (i == null)
            {
                Add(t);
            }
            else
            {
                Remove(i);
                Add(t);
            }
            return t;
        }
    }

    public class ContainerItemEqualityComparer<TKey> : IEqualityComparer<Containers3.IContainerItem<TKey>>
    {
        public bool Equals(IContainerItem<TKey> ci1, IContainerItem<TKey> ci2)
        {
            return ci1 != null && ci2 != null && ci1.Key.Equals(ci2.Key);
        }

        public int GetHashCode(IContainerItem<TKey> t)
        {
            return t.Key.GetHashCode();
        }
    }
}
