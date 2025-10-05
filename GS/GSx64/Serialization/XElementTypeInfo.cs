using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GS.Extension;

namespace GS.Serialization
{
    public class XElementTypeInfo
    {
        public XElement XElement { get; set; }
        public string NameSpace { get; set; }
        public string Assembly { get; set; }
        public string TypeName { get; set; }
        public string XElementName => XElement.Name.LocalName;
    }
}
