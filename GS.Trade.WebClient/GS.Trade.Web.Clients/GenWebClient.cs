using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.Web.Clients
{
    public class GenWebClient<T> : WebClient
    {
            public T Get(string par)
            {
                return GetSync<T>(par);
            }
            public T Add(T t)
            {
                return Post2Sync<T>(t);
            }
            public bool Update(int id, T t)
            {
                return Put2Sync<T>(id, t);
            }
        
    }
}
