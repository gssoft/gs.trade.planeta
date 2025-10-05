using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GS.EventLog.DataBase
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        //IEnumerable<T> Find(Expression<Func<T, bool>> query);
        T Find(Expression<Func<T, bool>> query);
        T GetById(int id);
        T GetById(long id);
        void Add(T item);
        void Update(T item);
        void Delete(T item);
        void Delete(int id);
        void Delete(long id);
        int Count();
    }
}
