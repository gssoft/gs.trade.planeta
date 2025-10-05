using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using GS.ConsoleAS;
using GS.Extension;
using GS.Moex.Interfaces;
// using GS.Trade.Moex;

namespace Moex
{
    public partial class MoexTest
    {
        public List<string> ClassCodes;
        public MoexTickerTypeEnum TickerTypeEnum;
        
        public IMoexTicker GetTicker(MoexTickerTypeEnum tickerType, string tickerСode)
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            if (tickerСode.HasNoValue()) return null;
            try
            {
                var tickerlink = CreateTickerLink(tickerType, tickerСode);
                if (tickerlink.HasNoValue()) return null;

                var xmlsecstrm = WebClient02.GetItemInStream(tickerlink);
                var xdoc = XDocument.Load(xmlsecstrm);
                //var ret = GetTickerFromXDoc(xdoc, "/document/data[@id='securities']/rows/row");
                var ret = GetTickerFromXDoc(xdoc, SecurityRowXPath);
                return ret;
            }
            catch (Exception ex)
            {
                ConsoleSync.WriteLineT(methodname, ex.ToString());
            }
            return null;
        }
        public IMoexTicker GetTicker(string classСode, string tickerСode)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";
            if (string.IsNullOrWhiteSpace(classСode))
            {
                SendErrMessage(method, new ArgumentNullException(nameof(classСode)).ToString());
                return null;
            }
            if (string.IsNullOrWhiteSpace(tickerСode))
            {
                SendErrMessage(method, new ArgumentNullException(nameof(tickerСode)).ToString());
                return null;
            }
            try
            {
                var tickerLink = CreateTickerLink(classСode, tickerСode);
                if(tickerLink.HasNoValue()) return null;

                var xmlSecStream = WebClient02.GetItemInStream(tickerLink);
                var xDoc = XDocument.Load(xmlSecStream);
                //var ret = GetTickerFromXDoc(xdoc, "/document/data[@id='securities']/rows/row");
                var ret = GetTickerFromXDoc(xDoc, SecurityRowXPath);
                return ret;
            }
            catch (Exception ex)
            {
                SendErrMessage(method, ex.ToString());
            }
            return null;
        }
        public IMoexTicker GetTicker(string tickerCode)
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            if (tickerCode.HasNoValue()) return null;
            try
            {
                var list = EnumToList<MoexTickerTypeEnum>();
                foreach (MoexTickerTypeEnum classcode in list)
                {
                    if(classcode == MoexTickerTypeEnum.Unknown) continue;
                    var ret = GetTicker(classcode, tickerCode);
                    if(ret != null) return ret;
                }
                //var list = TickerTypeEnum.EnumToList<MoexTickerTypeEnum>();
                
            }
            catch (Exception ex)
            {
                ConsoleSync.WriteLineT(methodname, ex.ToString());
            }
            return null;
        }
        public IMoexTicker GetTickerFromXDoc(XDocument xdoc, string xpath)
        {
            var method = MethodBase.GetCurrentMethod()?.Name + "()";
            if (xdoc == null)
            {
                SendErrMessage(method, new ArgumentNullException(nameof(xdoc)).ToString());
                return null;
            }
            if (xpath.HasNoValue())
            {
                SendErrMessage(method, new ArgumentNullException(nameof(xpath)).ToString());
                return null;
            }
            var xml = xdoc.XPathSelectElement(xpath);
            return xml != null ? TickerDeserialize(xml) : null;
        }
        private string GetInstrApiPrefix(MoexTickerTypeEnum tickerType)
        {
            switch (tickerType)
            {
                case MoexTickerTypeEnum.Futures: return FuturesIssWebApiPrefix;
                case MoexTickerTypeEnum.Option:  return OptionsIssWebApiPrefix;
                default: return string.Empty;
            }
        }
        private string GetInstrApiPrefix(string classCode)
        {
            switch (classCode.TrimUpper())
            {
                case "RFUD":                                    // Moex Futures
                case "SPBFUT": return FuturesIssWebApiPrefix;   // Quik Futures
                case "ROPD":                                    // Moex Options
                case "SPBOPT": return OptionsIssWebApiPrefix;   // Quik Options
                default: return string.Empty;
            }
        }
        private static string GetTickerCode(string tickerCode)
        {
            // "BR000059BV9" or "BR59BV9"
            if (tickerCode.Length < 5) return tickerCode;
            switch (tickerCode.Left(2))
            {
                case "BR": return tickerCode.Replace("0000","");
                default: return tickerCode;
            }
        }
        private string CreateTickerLink(MoexTickerTypeEnum tickerType, string tickerCode)
        {
            var instrPrefix = GetInstrApiPrefix(tickerType);
            if (instrPrefix.HasNoValue()) return string.Empty;
            var tcode = GetTickerCode(tickerCode);
            if (tcode.HasNoValue()) return string.Empty;
            return instrPrefix + tickerCode + ApiPostfix; // instr = option/futures, tcode = SiZ9, post = .xm
        }
        private string CreateTickerLink(string board, string tickerCode)
        {
            var instrPrefix = GetInstrApiPrefix(board);
            if (instrPrefix.HasNoValue()) return string.Empty;
            var tcode = GetTickerCode(tickerCode);
            if (tcode.HasNoValue()) return string.Empty;
            return instrPrefix + tickerCode + ApiPostfix; // instr = option/futures, tcode = SiZ9, post = .xm
        }
    }
}
