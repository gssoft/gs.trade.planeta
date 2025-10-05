using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.DataBase.Model;

namespace GS.Trade.Web.Clients
{
   
    public class TickerWebClient : GenWebClient<Ticker>
    {
        public new Ticker Get (string key)
        {
            var ss = key.Split('@');
            if (ss.Length != 2)
                return null;
            var s = @"?board=" + ss[0] + @"&code=" + ss[1];
            return base.Get(s);
        }
       
    }
    public class Ticker2WebClient : WebClient
    {
        public Ticker Get(string key)
        {
            var ss = key.Split('@');
            if (ss.Length != 2)
                return null;
            var s = @"?board=" + ss[0] + @"&code=" + ss[1];
            return base.GetSync<Ticker>(s);
        }

    }
}
