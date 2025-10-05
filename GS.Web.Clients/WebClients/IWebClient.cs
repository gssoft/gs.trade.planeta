using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClients
{

    public interface IClient
    {
        bool Start();
        bool Stop();
    }
    public interface IWebClient : IClient
    {
        WebClientStatus Status { get; }

    }
}
