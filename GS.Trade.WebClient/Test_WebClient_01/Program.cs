using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.DataBase.Model;
using GS.Trade.Web.Clients;

namespace Test_WebClient_01
{
    class Program
    {
        static void Main(string[] args)
        {
            var wc = new WebClient();
            wc.Init();

            var a = new Account
            {
                Alias = "accAlias01",
                Name = "name_01",
                Code = "001",
                TradePlace = "TradePlace01"
            };
            var b = wc.Register(a);

            a = new Account
            {
                Alias = "accAlias02",
                Name = "name_02",
                Code = "002",
                TradePlace = "TradePlace_02"
            };
           b = wc.Register(a);


        }
    }
}
