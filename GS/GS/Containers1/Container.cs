using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GS.Containers1
{
    public abstract class Container<T1>
    {
        protected T1 ItemCollection { get; set; }
        protected readonly object LockItems = new object();

        public abstract IEnumerable<IContainerItem> Items { get; }
        public abstract IContainerItem GetByKey(string key);
        public abstract bool Contains(IContainerItem t);
        public abstract bool Add(IContainerItem t);
        public abstract bool Remove(IContainerItem t);

        public IContainerItem GetByKey(IContainerItem ci)
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

        public virtual IContainerItem AddNew(IContainerItem t)
        {
            return t == null ? null : (Contains(t) ? t : (Add(t) ? t : null));
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
        public override bool Equals(object o)
        {
            if (ReferenceEquals(null, o)) return false;
            if (ReferenceEquals(this, o)) return true;
            if (o.GetType() != this.GetType()) return false;
            return Equals((IContainerItem)o);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return Key.GetHashCode();
            }
        }

        public IContainer Container { get; private set; }
    }

    public abstract class ListContainer : Container<List<IContainerItem>>
    {
        protected ListContainer()
        {
            ItemCollection = new List<IContainerItem>();
        }

        public long Count {
            get { return ItemCollection.Count; }
        }

        public override IEnumerable<IContainerItem> Items {
            get
            {
                var il = new List<IContainerItem>();
                lock (((ICollection) ItemCollection).SyncRoot)
                {
                    il.AddRange(ItemCollection.ToList());
                }
                return il;
            }
        }

        public override bool Add(IContainerItem t)
        {
            lock (((ICollection) ItemCollection).SyncRoot)
            {
                ItemCollection.Add(t);
            }
            return true;
        }
        public override IContainerItem GetByKey(string key)
        {
            IContainerItem t;
            lock (((ICollection) ItemCollection).SyncRoot)
            {
                t = ItemCollection.FirstOrDefault(i => string.Equals(i.Key, key));
            }
            return t;
        }
        public override bool Contains(IContainerItem t)
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
        public override bool Remove(IContainerItem t)
        {
            return t != null && ItemCollection.Remove(t);
        }
    }
    public abstract class SetContainer : Container<HashSet<IContainerItem>>
    {
        protected SetContainer()
        {
            ItemCollection = new HashSet<IContainerItem>(comparer: new ContainerItemEqualityComparer());
        }
        public long Count
        {
            get { return ItemCollection.Count; }
        }
        public override IEnumerable<IContainerItem> Items
        {
            get
            {
                var il = new List<IContainerItem>();
                lock (LockItems)
                {
                    il.AddRange(ItemCollection.ToList());
                }
                return il;
            }
        }
        public override bool Add(IContainerItem t)
        {
            lock (LockItems)
            {
                ItemCollection.Add(t);
            }
            return true;
        }

        public override IContainerItem GetByKey(string key)
        {
            IContainerItem t;
            lock (LockItems)
            {
                t = ItemCollection.FirstOrDefault(i => string.Equals(i.Key, key));
            }
            return t;
        }
        public override bool Contains(IContainerItem t)
        {
            bool boo;
            lock (LockItems)
            {
                boo = ItemCollection.Contains(t);
            }
            return boo;
        }
        public override bool Remove(IContainerItem t)
        {
            return t != null && ItemCollection.Remove(t);
        }
    }
    public abstract class DictionaryContainer<TKey> : Container<Dictionary<string, IContainerItem>>
    {
        protected DictionaryContainer()
        {
            ItemCollection = new Dictionary<string, IContainerItem>();
        }
        public long Count
        {
            get { return ItemCollection.Count; }
        }
        public override IEnumerable<IContainerItem> Items
        {
            get
            {
                var il = new List<IContainerItem>();
                lock (LockItems)
                {
                    il.AddRange(ItemCollection.Values.ToList());
                }
                return il;
            }
        }

        public override bool Add(IContainerItem t)
        {
            lock (LockItems)
            {
                ItemCollection.Add(t.Key, t);
            }
            return true;
        }

        public override IContainerItem GetByKey(string key)
        {
            IContainerItem t;
            lock (LockItems)
            {
                ItemCollection.TryGetValue(key, out t);
            }
            return t;
        }
        public override bool Contains(IContainerItem t)
        {
            bool boo;
            lock (LockItems)
            {
                boo = ItemCollection.ContainsKey(t.Key);
            }
            return boo;
        }
        public override bool Remove(IContainerItem t)
        {
            lock (LockItems)
            {
                return t != null && ItemCollection.Remove(t.Key);
            }
        }
    }

    public class ContainerItemEqualityComparer : IEqualityComparer<IContainerItem>
    {
        public bool Equals(IContainerItem ci1, IContainerItem ci2)
        {
            return ci1 != null && ci2 != null && ci1.Equals(ci2);
        }

        public int GetHashCode(IContainerItem t)
        {
            return t.Key.GetHashCode();
        }
    }

    //public class Ser
    //{
    //    public const int MyConst;

    //    public Ser()
    //    {
    //        MyConst = 234;
    //    }
    //}
}
