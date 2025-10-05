using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;

namespace GS.Interfaces
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> query);
        T GetById(int id);
        T GetById(long id);
        void Add(T item);
        void Update(T item);
        void Delete(T item);
        void Delete(int id);
        void Delete(long id);
    }

    public interface IRepository<in TKey, TItem>
        where TItem : IHaveKey<TKey>
    {
        IEnumerable<TItem> GetAll();
        IEnumerable<TItem> Find(Expression<Func<TItem, bool>> query);
        TItem GetById(TKey id);

        bool Add(TItem item);
        bool AddNew(TItem item);
        bool AddOrUpdate(TItem item);

        //int Add(IEnumerable<TItem> items);
        //int AddNew(IEnumerable<TItem> items);
        //int AddOrUpdate(IEnumerable<TItem> items);

        TItem AddOrGet(TItem item);
        TItem Register(TItem item);

        void Update(TItem item);
        //void Update(IEnumerable<TItem> items); 

        void Delete(TItem item);
        //void Delete(IEnumerable<TItem> items);

        void Delete(TKey id);
    }
    public interface IRepository<in TId, in TKey, TItem>
        where TItem : IHaveKey<TKey>
    {
        IEnumerable<TItem> GetAll();
        IEnumerable<TItem> Find(Expression<Func<TItem, bool>> query);
        TItem GetById(TId id);

        bool Add(TItem item);
        bool AddNew(TItem item);
        bool AddOrUpdate(TItem item);

        //int Add(IEnumerable<TItem> items);
        //int AddNew(IEnumerable<TItem> items);
        //int AddOrUpdate(IEnumerable<TItem> items);

        TItem AddOrGet(TItem item);
        TItem Register(TItem item);

        void Update(TItem item);
        //void Update(IEnumerable<TItem> items); 

        void Delete(TItem item);
        //void Delete(IEnumerable<TItem> items);

        void Delete(TKey id);
    }
}
