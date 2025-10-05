using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers;

namespace GS.TcpDde
{
    public class Formatter
    {
        public string[] ToTcpQuotes(string str)
        {
            return new [] {"Data","Quotes",  str};
        }
        public string[] ToTcpQuotes(IList<string> ss)
        {
            var strarr = new [] {"Data", "Quotes"};
            foreach (var i in ss)
                strarr[2] += i + Environment.NewLine;
            return strarr;
        }
    }
    public class QuotesDto : IHaveKey<string>
    {
        private string[] ss;
        public string Key => $"{ss[1]}.{ss[2]}";
    }
    public class DtoObj<T>
    {

    }
}
