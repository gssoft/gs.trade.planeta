using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClients
{
    public enum WebClientStatus
    {
        Unknown = 0,
        Started = 1,
        Starting = 2,
        Stopped = 3,
        Stopping = 4
    }
}
