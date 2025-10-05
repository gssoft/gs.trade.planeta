using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sgml
{
    public class HtmlToXml
    {
        public static XmlDocument Parse(StreamReader sr)
        {
            using (var sgmlReader = new SgmlReader())
            {
                sgmlReader.DocType = "HTML";
                sgmlReader.WhitespaceHandling = WhitespaceHandling.All;
                sgmlReader.CaseFolding = CaseFolding.ToLower;
                sgmlReader.InputStream = sr; // Html Content
                var doc = new XmlDocument
                {
                    PreserveWhitespace = true,
                    XmlResolver = null
                };
                doc.Load(sgmlReader);
                return doc;
            }
        }
        public static async Task<XmlDocument> ParseAsync(StreamReader sr)
        {
            return await Task<XmlDocument>.Factory.StartNew(() => Parse(sr));
        }
    }
}
