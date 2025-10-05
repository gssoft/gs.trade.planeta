using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.Elements;
using GS.Interfaces;
using System.Data.Entity.Infrastructure;

namespace GS.Trade.DataBase.EFRepositary
{
    public interface IEFRepository<in TKey,T> 
        where T : class, IHaveKey<TKey>
    {
        IQueryable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> query);
        T Get(Expression<Func<T, bool>> query);
        T GetById(int id);
        T GetByKey(TKey key);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(int id);
        void Delete(TKey key);

        //int SaveChanges
    }
    public abstract class EFRepository<TKey,T> : Element1<TKey>, IEFRepository<TKey,T> 
        where T : class, IHaveKey<TKey>
    {
        protected EFRepository(DbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("dbContext", "EFRepository(DbContext");
            DbContext = dbContext;
            DbSet = DbContext.Set<T>();
        }
        

        protected DbContext DbContext { get; set; }

        protected DbSet<T> DbSet { get; set; }

        public virtual IQueryable<T> GetAll()
        {
            return DbSet;
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> query)
        {
            return DbSet.Where(query).AsEnumerable();
        }

        public T Get(Expression<Func<T, bool>> query)
        {
            return DbSet.FirstOrDefault(query);
        }

        public virtual T GetById(int id)
        {
            //return DbSet.FirstOrDefault(PredicateBuilder.GetByIdPredicate<T>(id));
            return DbSet.Find(id);
        }

        //public virtual T GetByKey(TKey key)
        //{
        //    return DbSet.FirstOrDefault(e => e.Key.Equals(key));
        //}
        public abstract T GetByKey(TKey key);
        

        public virtual void Add(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                DbSet.Add(entity);
            }
        }

        public virtual void Update(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            dbEntityEntry.State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
            }
            else
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            }
        }

        public virtual void Delete(int id)
        {
            var entity = GetById(id);
            if (entity == null) return; // not found; assume already deleted.
            Delete(entity);
        }

        public void Delete(TKey key)
        {
            Delete(GetByKey(key));
        }
    }
}
