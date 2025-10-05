using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Elements
{
    abstract public class Converter<T1,T2>
    {
        abstract public  T2 Convert(T1 t);
        abstract public  T1 Convert(T2 t);
    }

    public class IntStrConverter : Converter<int, string>
    {
        public override string Convert(int t)
        {
            return t.ToString(CultureInfo.InvariantCulture);
        }

        public override int Convert(string t)
        {
            return int.Parse(t);
        }

        private void A<T1,T2>(T1 t1, T2 t2)
        {
            if (typeof (T1) != typeof (T2))
                return;
            
        }
    }
}

