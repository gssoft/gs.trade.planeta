using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GS.ConsoleAS;
using GS.Extension;

namespace WebClients
{
    public class XDocWebClient : WebClient02<XDocument>
    {
        public XDocument Get(string configuration, string configurationItem, string obj)
        {
            if (string.IsNullOrWhiteSpace(configuration) || configurationItem.HasNoValue() || obj.HasNoValue())
                return null;
            var queryString = "?" +
                              //"Ticker=" + Server.UrlEncode(ticker) +
                              //"&TimeInt=" + Server.UrlEncode(timeInt.ToString());
                              "cnf=" + configuration +
                              "&item=" + configurationItem +
                              "&obj=" + obj
                ;
            return GetItem(queryString);

        }
        public string Get2(string configuration, string configurationItem, string obj)
        {
            if (string.IsNullOrWhiteSpace(configuration) || configurationItem.HasNoValue() || obj.HasNoValue())
                return null;
            var queryString = "?" +
                //"Ticker=" + Server.UrlEncode(ticker) +
                //"&TimeInt=" + Server.UrlEncode(timeInt.ToString());
                              "cnf=" + configuration +
                              "&item=" + configurationItem +
                              "&obj=" + obj
                              ;
            ConsoleSync.WriteLineT("Query:" + queryString);
            return GetItemInString(queryString);
        }
    }
}
