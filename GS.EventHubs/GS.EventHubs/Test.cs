using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.EventHubs.Interfaces;

namespace GS.EventHubs
{
    public interface ITest<TEventArgs, TContent> where TEventArgs : IHaveContent<TContent>
    {
        void Method1(TEventArgs args);
        void Method2(TContent args);
    }
    public class Test<TEventArgs, TContent> : ITest<TEventArgs, TContent>
        where TEventArgs : IHaveContent<TContent>
    {
        public void Method1(TEventArgs args)
        {
            var m = args.Content;
        }

        public void Method2(TContent args)
        {
            var m = args;
        }
    }

    public class Proba
    {
        Test<MessageStr, string[]> Test { get; set; }

        public Proba()
        {
            Test = new Test<MessageStr, string[]>();

        }


    }
}
