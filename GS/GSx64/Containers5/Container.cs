using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using GS.Events;
using EventArgs = GS.Events.EventArgs;

namespace GS.Containers5
{
    public interface IContainer<in TKey,  TValue>  // : ICollection<TValue>
    {
         IEnumerable<TValue> Items { get; }

         TValue GetByKey(TKey key);
         bool Contains(TValue t);
         bool Add(TValue t);
         TValue AddOrGet(TValue t);
         bool Remove(TValue t);
         bool Remove(TKey key);
         bool Update(TKey key, TValue t);

         long Count { get;  }

        //TValue GetByKey(TValue ci);

        //TValue AddOrGet(TValue t);
        //bool AddNew(TValue t);
    }

    public abstract class Container<TList, TValue, TKey>    : IContainer<TKey, TValue> 
        where TValue : IHaveKey<TKey> 
    {
        public event EventHandler<Events.IEventArgs> ContainerEvent;
        //public EventHandler<Events.IEventArgs> ExternalContainerEvent;

        protected virtual void FireContainerEvent(Events.IEventArgs args)
        {
            EventHandler<IEventArgs> handler = ContainerEvent;
            if (handler != null && args != null)
                handler(this, args);
            //if (ExternalContainerEvent != null && args != null)
            //    ExternalContainerEvent(this, args);
        }

        //protected virtual void OnContainerEvent(string operation, IContainerItem<TKey> i )
        //{
        //    EventHandler<EventArgs> handler = ContainerEvent;
        //    if (handler == null)
        //        return;
        //    var eargs = GetEventArgs();
        //    if (eargs == null)
        //        return;
        //    eargs.Operation = operation;
        //    eargs.Object = i;
        //    handler(this, eargs);
        //}

        protected TList ItemCollection { get; set; }
        protected readonly object LockItems = new object();

        public abstract IEnumerable<TValue> Items { get; }
        public abstract TValue GetByKey(TKey key);
        public abstract bool Contains(TValue t);
        public abstract bool Add(TValue t);
        public abstract bool Remove(TValue t);

       // long IContainer<TKey, TValue>.Count { get; set; }
        
        public abstract bool Remove(TKey key);
        public abstract bool Update(TKey key, TValue t);
        public abstract long Count { get; }

        //protected abstract int Count();

        public TValue GetByKey(TValue ci)
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

        public virtual TValue AddOrGet(TValue t)
        {
            return t.Equals(null) ? default(TValue) : (Contains(t) ? t : ( Add(t) ? t : default(TValue)));
        }

        public virtual bool AddNew(TValue t)
        {
            return !t.Equals(null) && (!Contains(t) && Add(t));
        }

        public virtual Events.EventArgs GetEventArgs()
        {
            return null;
        }
    }
   
// ***************************** List *************************************************
    public class ListContainer<TKey, TValue> : Container<List<TValue>, TValue, TKey>, IContainer<TKey, TValue>
        where TValue : class, IHaveKey<TKey>
        //where TKey : struct 
    {
        public ListContainer()
        {
            ItemCollection = new List<TValue>();
        }

        public override long Count => ItemCollection.Count;

        public override IEnumerable<TValue> Items
        {
            get
            {
                lock (((ICollection)ItemCollection).SyncRoot)
                {
                    return ItemCollection.ToList();
                }
            }
        }
        public override bool Add(TValue t)
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                ItemCollection.Add(t);
            }
           //OnContainerEvent("New", t);
            return true;
        }
        public override TValue GetByKey(TKey key)
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                return ItemCollection.FirstOrDefault(i => i.Key.Equals(key));
            }
            //return t;
        }
        public override bool Contains(TValue t)
        {
            lock (((ICollection) ItemCollection).SyncRoot)
            {
                return GetByKey(t) != null;
                // return !GetByKey(t).Equals(null); // != null;
            }
        }
        public override bool Update(TKey key, TValue t)
        {
            var i = GetByKey(key);

            if (i.Equals(null))
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
        public override bool Remove(TValue t)
        {
            //lock (((ICollection) ItemCollection).SyncRoot)
            //{
            //    return t != null && ItemCollection.Remove(t);
            //}
            return Remove(t.Key);
        }
        public bool RemoveNoKey(TValue t)
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                return t != null && ItemCollection.Remove(t);
            }
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

    // List with Two Keys
    public class ListContainerK2<TKey, TKey2, TValue> : Container<List<TValue>, TValue, TKey>, IContainer<TKey, TValue>
        where TValue : class, IHaveKey<TKey>
        //where TKey : struct 
    {
        public ListContainerK2()
        {
            ItemCollection = new List<TValue>();
        }

        public override long Count => ItemCollection.Count;

        public override IEnumerable<TValue> Items
        {
            get
            {
                lock (((ICollection)ItemCollection).SyncRoot)
                {
                    return ItemCollection.ToList();
                }
            }
        }
        public override bool Add(TValue t)
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                ItemCollection.Add(t);
            }
            //OnContainerEvent("New", t);
            return true;
        }
        public override TValue GetByKey(TKey key)
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                return ItemCollection.FirstOrDefault(i => i.Key.Equals(key));
            }
            //return t;
        }
        public TValue GetByKeyK2(TKey2 key)
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                return ItemCollection.FirstOrDefault(i => i.Key.Equals(key));
            }
            //return t;
        }
        public override bool Contains(TValue t)
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                return GetByKey(t) != null;
                // return !GetByKey(t).Equals(null); // != null;
            }
        }
        public override bool Update(TKey key, TValue t)
        {
            var i = GetByKey(key);

            if (i.Equals(null))
                return false;

            return Remove(i) && Add(t);
        }
        public void RemoveAt(int index)
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                ItemCollection.RemoveAt(index);
            }
        }
        public override bool Remove(TValue t)
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
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                return i != null && ItemCollection.Remove(i);
            }
        }
        public bool Remove2(TValue t)
        {
            lock (((ICollection) ItemCollection).SyncRoot)
            {
                return t != null && ItemCollection.Remove(t);
            }
        }
        public bool RemoveK2(TKey key)
        {
            var i = GetByKey(key);
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                return i != null && ItemCollection.Remove(i);
            }
        }

        //public virtual Events.EventArgs GetEventArgs()
        //{
        //    return null;
        //}
    }
    //// Set ********************************** SET **************************
    //public abstract class SetContainer<TKey> : Container<HashSet<IContainerItem<TKey>>, TKey>
    //{
    //    protected SetContainer()
    //    {
    //        ItemCollection = new HashSet<IContainerItem<TKey>>(comparer: new ContainerItemEqualityComparer<TKey>());
    //    }
    //    public long Count
    //    {
    //        get { return ItemCollection.Count; }
    //    }
    //    public override IEnumerable<IContainerItem<TKey>> Items
    //    {
    //        get
    //        {
    //            var il = new List<IContainerItem<TKey>>();
    //            lock (LockItems)
    //            {
    //                il.AddRange(ItemCollection.ToList());
    //            }
    //            return il;
    //        }
    //    }
    //    public override bool Add(IContainerItem<TKey> t)
    //    {
    //        lock (LockItems)
    //        {
    //            ItemCollection.Add(t);
    //        }
    //        return true;
    //    }
    //    public override bool Contains(IContainerItem<TKey> t)
    //    {
    //        bool boo;
    //        lock (LockItems)
    //        {
    //            boo = ItemCollection.Contains(t);
    //        }
    //        return boo;
    //    }
    //    public override bool Update(TKey key, IContainerItem<TKey> t)
    //    {
    //        var i = GetByKey(key);
    //        if (i == null)
    //            return false;

    //        return Remove(i) && Add(t);
    //    }

    //    public override bool Remove(IContainerItem<TKey> t)
    //    {
    //        lock (LockItems)
    //        {
    //            return t != null && ItemCollection.Remove(t);
    //        }
    //    }
    //    public override bool Remove(TKey key)
    //    {
    //        var i = GetByKey(key);
    //        return i != null && Remove(i);
    //    }
    //}
    // ******************************** Dictionary ***********************************
    public  class DictionaryContainer<TKey,TValue> :
        Containers5.Container<Dictionary<TKey, TValue>, TValue, TKey>, IContainer<TKey, TValue>
        where TValue : class, IHaveKey<TKey>
    {
        public DictionaryContainer()
        {
            ItemCollection = new Dictionary<TKey, TValue>();
        }

        public override long Count
        {
            get { return ItemCollection.Count; }
        }

        public override IEnumerable<TValue> Items
        {
            get
            {
                lock (LockItems)
                {
                    return ItemCollection.Values.ToList();
                }
            }
        }

        public override bool Add(TValue t)
        {
            lock (LockItems)
            {
                ItemCollection.Add(t.Key, t);
            }
            return true;
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

        public override bool Contains(TValue t)
        {
            //bool boo;
            lock (LockItems)
            {
                return ItemCollection.ContainsKey(t.Key);
            }
            //return boo;
        }
        public override bool Update(TKey key, TValue t)
        {
            var i = GetByKey(key);
            if (i == null)
                return false;

            return Remove(i) && Add(t);
        }

        public override bool Remove(TValue t)
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
                return i != null  && ItemCollection.Remove(key);
            }
        }

        public override TValue AddOrGet(TValue t)
        {
            var i = GetByKey(t);
            //if (!i.Equals(null))
            if( i != null)
                return i;
            Add(t);
            return t;
        }
        public TValue AddOrUpdate(TValue t)
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

    public class IhaveKeyEqualityComparer<TKey> : IEqualityComparer<IHaveKey<TKey>>
    {
        public bool Equals(IHaveKey<TKey> i1, IHaveKey<TKey> i2)
        {
            return i1 != null && i2 != null && i1.Key.Equals(i2.Key);
        }

        public int GetHashCode(IHaveKey<TKey> t)
        {
            return t.Key.GetHashCode();
        }
    }
}
