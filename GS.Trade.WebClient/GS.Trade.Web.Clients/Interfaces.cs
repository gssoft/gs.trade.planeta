using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.DataBase.Model;

namespace GS.Trade.Web.Clients
{
    public interface IWebClient<T>
    {
        T Get(string uri);
        T Add(T t);
        bool Update(int id, T t);
    }

    public interface ITickerWevClient : IWebClient<Ticker>
    {
    }

    public interface IAccountWebClient : IWebClient<Account>
    {
    }
}
