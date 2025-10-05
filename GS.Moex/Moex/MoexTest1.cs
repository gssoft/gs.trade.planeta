using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using GS.ConsoleAS;
using GS.Moex.Interfaces;
// using GS.Trade.Moex;

namespace Moex
{
    public partial class MoexTest
    {
        public IMoexTicker GetTickerFromFile(string filename)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                var xml = XDocument.Load(filename);
                return GetTickerFromXDoc(xml, SecurityRowXPath);
            }
            catch (Exception e)
            {
                SendErrMessage(method, e.ToString());
            }
            return null;
        }
        public MoexTicker GetTickerFromFile(string filename, string xpath)
        {
            // Get XmlRow
            var xml = GetXmlElementInXmlReply(filename, xpath);
            // DeSerializer
            return xml != null ? TickerDeserialize(xml) : null;
        }
        public XElement GetXmlElementInXmlReply(string filename, string xmlpath)
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                var xml = XDocument.Load(filename);
                return xml.XPathSelectElement(xmlpath);
            }
            catch (Exception e)
            {
                ConsoleSync.WriteLineT(methodname, e.ToString());
            }
            return null;
        }
       
        private static void PrintMessage(string e)
        {
            ConsoleSync.WriteLineT(e);
        }
    }
}
