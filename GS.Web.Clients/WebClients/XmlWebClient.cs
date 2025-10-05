using System;
using System.Xml;
using System.Xml.Linq;
using GS.Extension;

namespace WebClients
{
    public class XmlWebClient : WebClient02<XmlDocument>
    {
        public bool IsUserPost { get; set; }

        public XmlDocument Get(string configuration, string configurationItem, string obj)
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
        public XDocument GetInString(string configuration, string configurationItem, string obj)
        {
            if (string.IsNullOrWhiteSpace(configuration) || configurationItem.HasNoValue() || obj.HasNoValue())
                return null;
            var queryString = "fxs?" +
                              //"Ticker=" + Server.UrlEncode(ticker) +
                              //"&TimeInt=" + Server.UrlEncode(timeInt.ToString());
                              "cnf=" + configuration +
                              "&item=" + configurationItem +
                              "&obj=" + obj
                ;
            //ConsoleSync.WriteLineT("Query:" + queryString);
            var str = GetItemInString(queryString);
            if (str == null)
                return null;
            return XDocument.Parse(str);
        }
        public XDocument GetInStream(string configuration, string configurationItem, string obj)
        {
            if (string.IsNullOrWhiteSpace(configuration) || configurationItem.HasNoValue() || obj.HasNoValue())
                return null;
            var queryString = "fxs?" +
                              //"Ticker=" + Server.UrlEncode(ticker) +
                              //"&TimeInt=" + Server.UrlEncode(timeInt.ToString());
                              "cnf=" + configuration +
                              "&item=" + configurationItem +
                              "&obj=" + obj
                ;
            //ConsoleSync.WriteLineT("Query:" + queryString);
            var str = GetItemInStream(queryString);
            if (str == null)
                return null;
            return XDocument.Load(str);
        }
        public XDocument GetInStream(string configuration, string configurationItem)
        {
            if (string.IsNullOrWhiteSpace(configuration) || configurationItem.HasNoValue())
                return null;
            var queryString = "fxs?" +
                              //"Ticker=" + Server.UrlEncode(ticker) +
                              //"&TimeInt=" + Server.UrlEncode(timeInt.ToString());
                              "cnf=" + configuration +
                              "&item=" + configurationItem
                ;
            //ConsoleSync.WriteLineT("Query:" + queryString);
            var str = GetItemInStream(queryString);
            if (str == null)
                return null;
            return XDocument.Load(str);
        }
        public byte[] GetInBytes(string configuration, string configurationItem, string obj)
        {
            if (string.IsNullOrWhiteSpace(configuration) || configurationItem.HasNoValue() || obj.HasNoValue())
                return null;
            var queryString = "fab?" +
                              //"Ticker=" + Server.UrlEncode(ticker) +
                              //"&TimeInt=" + Server.UrlEncode(timeInt.ToString());
                              "cnf=" + configuration +
                              "&item=" + configurationItem +
                              "&obj=" + obj
                ;
            //ConsoleSync.WriteLineT("Query:" + queryString);
            var str = GetItemInBytes(queryString);
            return str;
        }

        public XDocument GetInStream(long token, string configuration, string configurationItem, string obj)
        {
            if (string.IsNullOrWhiteSpace(configuration) || configurationItem.HasNoValue() || obj.HasNoValue())
                return null;
            
            var queryString = "fxs?" +
                              //"Ticker=" + Server.UrlEncode(ticker) +
                              //"&TimeInt=" + Server.UrlEncode(timeInt.ToString());
                              "tkn=" + token + 
                              "&cnf=" + configuration +
                              "&item=" + configurationItem +
                              "&obj=" + obj +
                              (IsUserPost ? "&dm=" + GetUserDomain() + "&u=" + GetComputerUserName() : "")
                              ;
            //ConsoleSync.WriteLineT("Query:" + queryString);
            var str = GetItemInStream(queryString);
            if (str == null)
                return null;
            return XDocument.Load(str);
        }
        public XDocument GetInStream(long token, string configuration, string configurationItem)
        {
            if (string.IsNullOrWhiteSpace(configuration) || configurationItem.HasNoValue())
                return null;
            var queryString = "fxs?" +
                              //"Ticker=" + Server.UrlEncode(ticker) +
                              //"&TimeInt=" + Server.UrlEncode(timeInt.ToString());
                              "tkn=" + token +
                              "&cnf=" + configuration +
                              "&item=" + configurationItem +
                              (IsUserPost ? "&dm=" + GetUserDomain() + "&u=" + GetComputerUserName() : "")
                ;
            //ConsoleSync.WriteLineT("Query:" + queryString);
            var str = GetItemInStream(queryString);
            if (str == null)
                return null;
            return XDocument.Load(str);
        }

        public byte[] GetInBytes(long token, string configuration, string configurationItem, string obj)
        {
            if (string.IsNullOrWhiteSpace(configuration) || configurationItem.HasNoValue() || obj.HasNoValue())
                return null;
            var queryString = "fab?" +
                              //"Ticker=" + Server.UrlEncode(ticker) +
                              //"&TimeInt=" + Server.UrlEncode(timeInt.ToString());
                              "tkn=" + token +
                              "&cnf=" + configuration +
                              "&item=" + configurationItem +
                              "&obj=" + obj +
                              (IsUserPost ? "&dm=" + GetUserDomain() + "&u=" + GetComputerUserName() : "")
                ;
            //ConsoleSync.WriteLineT("Query:" + queryString);
            var str = GetItemInBytes(queryString);
            return str;
        }

        private string GetUserDomain()
        {
            return Environment.UserDomainName;
        }
        private string GetComputerName()
        {
            return Environment.MachineName;
        }
        private string GetUserName()
        {
            return Environment.UserName;
        }

        private string GetComputerUserName()
        {
            return Environment.MachineName + @"\" + Environment.UserName;
        }

    }
}