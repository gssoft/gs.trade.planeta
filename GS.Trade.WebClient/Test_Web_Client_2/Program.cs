using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Web_Client_2
{
    public interface IHaveId
    {
    }

    public class HaveId
    {
    }

    public interface IWc<T>
    {
        T Put(int id, T t);
    }

    public class Wc<T> : IWc<T>
    {
        public T Put(int id, T t)
        {
            return default(T);
        }
    }

    public class Twc : Wc<Tick>
    {
       
    }

    public class Awc : Wc<Acc>
    {
    }


    public class Tick : HaveId
    {
    }

    public class Acc : HaveId
    {
    }

    class Program
    {
        static void Main(string[] args)
        {
            var list = new List<IWc<HaveId>>();

            var a = new Twc();
            var b = new Awc();
            var c = new Abc<Z>();

            list.Add(a);
            list.Add(b);
            list.Add(c);



        }
    }
}
