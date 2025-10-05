using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Web_Client_2._1
{
    public interface IHaveId
    {
    }

    public interface IAbc
    {
        T Put<T>(int id, T t);
    }

    public class Abc : IAbc
    {
        public T Put<T>(int id, T t)
        {
            return default(T);
        }
    }

    public class B<T> : Abc
    {
    }

    public class List
    {
        public List<IAbc> Lst = new List<IAbc>();

        public List()
        {
            var a = new Abc();
            var b = new Abc();
            Lst.Add(a);
            Lst.Add(b);
        }
    }
}
