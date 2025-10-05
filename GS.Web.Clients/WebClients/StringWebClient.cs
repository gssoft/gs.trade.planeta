using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WebClients
{
    public class StringWebClient : WebClient02<string>
    {
    }
    public class StreamWebClient : WebClient02<Stream>
    {
    }
}
