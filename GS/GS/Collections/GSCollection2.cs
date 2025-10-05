using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Concurrent;
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
using System.Net.Configuration;
using System.Reflection;
using GS.Extension;

namespace GS.Collections
{
    public interface IGSCollection2<in TKey, TValue>  // : ICollection<TValue>
    {
        IEnumerable<TValue> Items { get; }
        TValue GetByKey(TKey key);
        //bool Contains(TValue t);
        //bool Contain(TKey key);
        bool Add(TValue t);
        bool AddNew(TValue t);
        TValue AddOrGet(TValue t);
        TValue AddOrUpdate(TValue t);
        
        bool Remove(TValue t);
        bool Remove(TKey key);
        void RemoveAt(int index);

        bool Update(TValue t);
        bool Change(TKey key, TValue t);

        void Clear();
        void Clear(int capasity);

        long Count { get; }

        //TValue GetByKey(TValue ci);

        //TValue AddOrGet(TValue t);
        //bool AddNew(TValue t);
    }
    public abstract class GSCollection2<TElemenTItemKey, TList, TItem, TItemKey> :
                            Element1<TElemenTItemKey>, IGSCollection2<TItemKey, TItem>
                            where TItem : class, IHaveKey<TItemKey>
    {
        public int Capasity { get; set; }
        public int CapasityLimit { get; set; }
        public bool IsReversed { get; set; }
        public bool IsNotificationEnabled { get; set; }
        public string EntityKey => Category + "@" + Entity;
        protected TList ItemCollection { get; set; }
        protected readonly object LockItems = new object();
        public abstract IEnumerable<TItem> Items { get; }
        public abstract TItem GetByKey(TItemKey key);
        //public abstract bool Contains(TItem t);
        //protected abstract bool Contains(TItemKey key);
        protected abstract bool AddVal(TItem t);
        public abstract bool Remove(TItemKey key);
        public abstract void RemoveAt(int index);
        //public abstract bool Update(TItemKey key, TItem t);
        public abstract void Clear();
        public abstract long Count { get; }     
        private bool AddIn(TItem t, string method, string typename)
        {
            try
            {
                var result = AddVal(t);
                if (result)
                {
                    OnChangedEvent(new Events.EventArgs
                    {
                        Category = Category,
                        Entity = Entity,
                        Operation = method,
                        Object = t
                    });
                    Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                        method, $"{typename}, Key: {t.Key}", t.ToString());

                    if (Capasity != 0 && CapasityLimit + Capasity <= Count)
                        Clear(Capasity);

                    return true;
                }
                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                    method, $"{typename}, Key: {t.Key}", t.ToString());

                return false;
            }
            catch (Exception e)
            {
                SendException(e);
            }
            return false;
        }
        public virtual bool Add(TItem t)
        {
            var method = MethodBase.GetCurrentMethod().Name;
            var typename = typeof(TItem).Name;
            
            return  VerifyForNullArgument(t, method, typename) &&
                    VerifyForNullArgument(t.Key, method, typename) &&
                    AddIn(t, method, typename);
        }
        public virtual bool AddNew(TItem t)
        {
            var method = MethodBase.GetCurrentMethod().Name;
            var typename = typeof(TItem).Name;

            if (!VerifyForNullArgument(t, method, typename))
                return false;
            if (!VerifyForNullArgument(t, method, typename))
                return false;
            try
            {
                var item = GetByKey(t.Key);
                if (item == null)
                    return AddIn(t, method, typename);

                Evlm1(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                                method, $"{typename}, Key: {t.Key} Already Exist", t.ToString());
            }
            catch (Exception e)
            {
                SendException(e);
                
            }          
            return false;
        }        
        public TItem AddOrGet(TItem t)
        {
            var method = MethodBase.GetCurrentMethod().Name;
            var typename = typeof(TItem).Name;
            try
            {
                if (!VerifyForNullArgument<TItem>(t, method, typename))
                    return null;

                if (!VerifyForNullArgument<TItemKey>(t.Key, method, typename))
                    return null;

                var i = GetByKey(t.Key);
                if (i != null)
                {
                    Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                            $"{method}: Get", $"{typename}, Key: {i.Key}", i.ToString());
                    return i;
                }
                var result = AddIn(t, method, typename);
                return result ? t : null;
            }
            catch (Exception e)
            {
                SendException(e);
            }
            return null;
        }
        public TItem AddOrUpdate(TItem t)
        {
            var method = MethodBase.GetCurrentMethod().Name;
            var typename = typeof(TItem).Name;
            try
            {
                if (!VerifyForNullArgument<TItem>(t, method, typename))
                    return null;

                if (!VerifyForNullArgument<TItemKey>(t.Key, method, typename))
                    return null;

                var i = GetByKey(t.Key);
                if (i == null)
                {
                    return AddIn(t, method, typename) ? t : null;
                }
                Update(i.Key, t, method, typename);
                return t;
            }
            catch (Exception e)
            {
                SendException(e);
            }
            return null;
        }

        public bool Update(TItemKey oldItemKey, TItem newItem, string method, string typename)
        {
            if (oldItemKey == null)
                return false;
            try
            {
                return Remove(oldItemKey) && AddIn(newItem, method, typename);
            }
            catch (Exception e)
            {
                SendException(e);
            }
            return false;
        }
        public bool Update(TItem t, string method, string typename)
        {
            try
            {
                var i = GetByKey(t.Key);
                if (i == null)
                    return false;

                return Remove(i.Key) && AddIn(t, method, typename);
            }
            catch (Exception e)
            {
                SendException(e);
            }
            return false;
        }
        public bool Update(TItem t)
        {
            var method = MethodBase.GetCurrentMethod().Name;
            var typename = typeof(TItem).Name;
            try
            {
                var i = GetByKey(t.Key);
                if (i == null)
                    return false;

                return Remove(i.Key) && AddIn(t, method, typename);
            }
            catch (Exception e)
            {
                SendException(e);
            }
            return false;
        }
        public bool Change(TItemKey oneItemKey, TItem anotherItem)
        {
            var method = MethodBase.GetCurrentMethod().Name;
            var typename = typeof(TItem).Name;
            try
            {
                var i = GetByKey(oneItemKey);
                if (i == null)
                    return false;

                return Remove(i.Key) && AddIn(anotherItem, method, typename);
            }
            catch (Exception e)
            {
                SendException(e);
            }
            return false;
        }
        public bool Remove(TItem t)
        {
            var method = MethodBase.GetCurrentMethod().Name;
            var typename = typeof(TItem).Name;
            try
            {
                if (!VerifyForNullArgument(t, method, typename))
                    return false;
                if (!VerifyForNullArgument(t.Key, method, typename))
                    return false;

                if (Count <= 0)
                {
                    Evlm1(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                        method, $"{typename} Key: {t.Key} Nothing Remove", t.ToString());
                    return false;
                }

                var result = Remove(t.Key);
                if (result)
                {
                    OnChangedEvent(new Events.EventArgs
                    {
                        Category = Category,
                        Entity = Entity,
                        Operation = method,
                        Object = t
                    });
                    Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                            method, $"{typename}, Key: {t.Key}", t.ToString());
                    return true;
                }
                Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                            method, $"{typename}, Key: {t.Key}", t.ToString());
                return false;
            }
            catch (Exception e)
            {
                SendException(e);
            }
            return false;
        }
        public virtual void Clear(int count)
        {
            while (Count > count)
            {
                RemoveAt((int)Count - 1);
            }
            Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                MethodBase.GetCurrentMethod().Name,
                $"Capasity={Capasity}; Limit={CapasityLimit}; ItemsCount={Count}", "");
        }
        protected bool VerifyForNullArgument<T>(T t, string method, string typename)
        {
            if (t != null)
                return true;

            Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                method, $"{typename} is NULL", "");
            return false;
        }
    }
    // ***************************** List *************************************************
    public class ListCollection2<TItemKey, TItem> :
                    GSCollection2<string, List<TItem>, TItem, TItemKey> // ,IGSCollection<TItemKey, TItem>
                    where TItem : class, IHaveKey<TItemKey>
    {
        public ListCollection2()
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
        public override long Count => ItemCollection.Count;
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
                return true;
            }
            catch (Exception e)
            {
                SendException(e);
            }
            return false;
        }
        //public override TItem GetByKey(TItemKey key)
        //{
        //    lock (((ICollection)ItemCollection).SyncRoot)
        //    {
        //        return ItemCollection.FirstOrDefault(i => i.Key.Equals(key));
        //    }
        //}
        public override TItem GetByKey(TItemKey key)
        {
            var method = MethodBase.GetCurrentMethod().Name;
            var typename = typeof(TItem).Name;

            if (!VerifyForNullArgument<TItemKey>(key, method, typename))
                return null;

            return Items.FirstOrDefault(i => i.Key.Equals(key));   
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
        public  bool RemoveNoKey(TItem item)
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                return item != null && ItemCollection.Remove(item);
            }
        }
        public override string Key => Code;
    }
    public class ObservableListCollection2<TItemKey, TItem> :
                    GSCollection2<string, ObservableCollection<TItem>, TItem, TItemKey> // ,IGSCollection<TItemKey, TItem>
                    where TItem : class, IHaveKey<TItemKey>
    {
        public ObservableListCollection2()
        {
            ItemCollection = new ObservableCollection<TItem>();
        }
        public override void Clear()
        {
            lock (((ICollection)ItemCollection).SyncRoot)
            {
                ItemCollection.Clear();
            }
        }
        public override long Count => ItemCollection.Count;
        // public override long Count => Items.Count();
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
                return true;
            }
            catch (Exception e)
            {
                SendException(e);
            }
            return false;
        }
        //public override TItem GetByKey(TItemKey key)
        //{
        //    lock (((ICollection)ItemCollection).SyncRoot)
        //    {
        //        return ItemCollection.FirstOrDefault(i => i.Key.Equals(key));
        //    }
        //}
        public override TItem GetByKey(TItemKey key)
        {
            var method = MethodBase.GetCurrentMethod().Name;
            var typename = typeof(TItem).Name;

            if (!VerifyForNullArgument<TItemKey>(key, method, typename))
                return null;

            return Items.FirstOrDefault(i => i.Key.Equals(key));
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
    public class DictionaryCollection2<TItemKey, TItem> :
                    GSCollection2<string, Dictionary<TItemKey, TItem>, TItem, TItemKey> //, IGSCollection<TItemKey, TItem>
                    where TItem : class, IHaveKey<TItemKey>
    {
        public DictionaryCollection2()
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
                return true;
            }
            catch (Exception e)
            {
                SendException(e);
            }
            return false;
        }

        public override TItem GetByKey(TItemKey key)
        {
            var method = MethodBase.GetCurrentMethod().Name;
            var typename = typeof(TItem).Name;

            if (!VerifyForNullArgument<TItemKey>(key, method, typename))
                return null;

            TItem t;
            lock (LockItems)
            {
                ItemCollection.TryGetValue(key, out t);
            }
            return t;
        }

        //public override bool Contains(TItem t)
        //{
        //    //bool boo;
        //    lock (LockItems)
        //    {
        //        return ItemCollection.ContainsKey(t.Key);
        //    }
        //    //return boo;
        //}

        //public override bool Remove(TItem t)
        //{
        //    lock (LockItems)
        //    {
        //        return t != null && ItemCollection.Remove(t.Key);
        //    }
        //}
        public override bool Remove(TItemKey key)
        {
            var method = MethodBase.GetCurrentMethod().Name;
            var typename = typeof(TItem).Name;

            if (!VerifyForNullArgument<TItemKey>(key, method, typename))
                return false;

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
            var res = Remove(Items.ToArray()[index]);
            // var res = Remove((List<TItem>)Items).RemoveAt(index);
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
        public override string Key => Code;
    }
    public class ConcurrentDictionaryCollection2<TItemKey, TItem> :
                GSCollection2<string, ConcurrentDictionary<TItemKey, TItem>, TItem, TItemKey> //, IGSCollection<TItemKey, TItem>
                where TItem : class, IHaveKey<TItemKey>
    {
        public ConcurrentDictionaryCollection2()
        {
            ItemCollection = new ConcurrentDictionary<TItemKey, TItem>();
        }

        public override void Clear()
        {
                ItemCollection.Clear();
        }
        public bool IsEmpty => Count <= 0;
        public override long Count => ItemCollection.Count;

        public override IEnumerable<TItem> Items => ItemCollection.Values.ToList();

        protected override bool AddVal(TItem t)
        {
            try
            {
                ItemCollection.TryAdd(t.Key, t); //AddOrUpdate();  //Add(t.Key, t);
                return true;
            }
            catch (Exception e)
            {
                SendException(e);
            }
            return false;
        }

        public override TItem GetByKey(TItemKey key)
        {
            var method = MethodBase.GetCurrentMethod().Name;
            var typename = typeof(TItem).Name;

            if (!VerifyForNullArgument<TItemKey>(key, method, typename))
                return null;

            TItem t;
            ItemCollection.TryGetValue(key, out t);
            return t;
        }

        //public override bool Contains(TItem t)
        //{
        //    //bool boo;
        //    lock (LockItems)
        //    {
        //        return ItemCollection.ContainsKey(t.Key);
        //    }
        //    //return boo;
        //}

        //public override bool Remove(TItem t)
        //{
        //    lock (LockItems)
        //    {
        //        return t != null && ItemCollection.Remove(t.Key);
        //    }
        //}
        public override bool Remove(TItemKey key)
        {
            var method = MethodBase.GetCurrentMethod().Name;
            var typename = typeof(TItem).Name;

            if (!VerifyForNullArgument<TItemKey>(key, method, typename))
                return false;

            TItem ret;
            return ItemCollection.TryRemove(key, out ret); //Remove(key);

        }
        public override void RemoveAt(int index)
        {
            if (Count <= index)
                return;
            //var t = Items.ToArray()[index];
            var res = Remove(Items.ToArray()[index]);
            // var res = Remove((List<TItem>)Items).RemoveAt(index);
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
        public override string Key => Code;
    }
    public class ConcurrentBagCollection2<TItemKey, TItem> :
                    GSCollection2<string, ConcurrentBag<TItem>, TItem, TItemKey> // ,IGSCollection<TItemKey, TItem>
                    where TItem : class, IHaveKey<TItemKey>
    {
        public ConcurrentBagCollection2()
        {
            ItemCollection = new ConcurrentBag<TItem>();
        }
        public override void Clear()
        {
            while (!ItemCollection.IsEmpty)
            {
                TItem item;
                ItemCollection.TryTake(out item);
            }
        }
        public override long Count => ItemCollection.Count;
        public override IEnumerable<TItem> Items => ItemCollection.ToList();

        protected override bool AddVal(TItem t)
        {
            try
            {
                //if (IsReversed)
                //    ItemCollection.Insert(0, t);
                //else
               ItemCollection.Add(t);
               return true;
            }
            catch (Exception e)
            {
                SendException(e);
            }
            return false;
        }
        //public override TItem GetByKey(TItemKey key)
        //{
        //    lock (((ICollection)ItemCollection).SyncRoot)
        //    {
        //        return ItemCollection.FirstOrDefault(i => i.Key.Equals(key));
        //    }
        //}
        public override TItem GetByKey(TItemKey key)
        {
            var method = MethodBase.GetCurrentMethod().Name;
            var typename = typeof(TItem).Name;

            if (!VerifyForNullArgument<TItemKey>(key, method, typename))
                return null;

            return ItemCollection.FirstOrDefault(i => i.Key.Equals(key));
        }
        public override void RemoveAt(int index)
        {
            
               // ItemCollection.RemoveAt(index);
            
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
            TItem item;
            var i = GetByKey(key);
            return i != null && ItemCollection.TryTake(out item); //Remove(i);
        }
        public override string Key => Code;
    }
}
