using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Test_WebClient_03
{
    public interface IA<out T>
    {
        T Func();
    }

    public class A<T> : IA<T>
    {
        public string Aaa { get; set; }

        public T Func()
        {
            return default (T);
        }
    }

    public class B
    {
        public string Bbb { get; set; }
    }

    public class C
    {
        public string Ccc { get; set; }
    }


    class Program
    {
        //public List<IA> list;
        static void Main(string[] args)
        {
            
            var b = new A<B>
            {
                Aaa = "Bbb",
            };
            var c = new A<C>
            {Aaa = "Ccc"}; 

             var tr = new StreamWriter("File.xml");
                // var sr = new XmlSerializer(typeof(Dictionary<string,Ticker>));  // !!! Not Support !!!!!
                var sr = new XmlSerializer(typeof(A<B>));
                sr.Serialize(tr, b);
                tr.Close();


        }
    }
}
