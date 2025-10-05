using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GS.Interfaces
{
    public interface IHaveUri
    {
        string Uri { get; set; }
        string XPath { get; set; }

        string UriXPath { get; }
    }
}
