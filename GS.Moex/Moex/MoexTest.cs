using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using GS.ConsoleAS;
using GS.Elements;
using GS.Extension;
using GS.Serialization;
using GS.Moex.Interfaces;
using WebClients;

namespace Moex
{
    public partial class MoexTest : Element1<string>
    {
        public event EventHandler<string> MessageEvent;
        public bool Verbose { get; set; }
        // Tickers ApiPrefix
        public string TickersFuturesApiPrefix { get; set; }
        public string TickersOptionsApiPrefix { get; set; }
        public string TickersApiPostfix { get; set; }
        public string TickersRowsXPath { get; set; }
        public string FuturesIssWebApiPrefix { get; set; }
        public string OptionsIssWebApiPrefix { get; set; }
        public string ApiPostfix { get; set; }
        public string SecurityRowXPath { get; set; }
        public string SecMarketDataRowXPath { get; set; }
        public int TradesLimitForWebRequest { get; set; }
        public string TradeRowsXPath { get; set; }
        public string TradeApiPostfix { get; set; }
        public string TradeApiParams { get; set; }
        public WebClient02 WebClient02;
        public override void Init()
        {
            var method = MethodBase.GetCurrentMethod()?.Name + "()";
            //WebClient02 = Builder.Build2<WebClient02>(@"Init\WebClient02.xml", "WebClient02");
            WebClient02?.Init();
            //WebClient02.Init();
            SendMessage(method, "Ok");
        }
        public void GetInstrumentFromFile(string filename)
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"Method: {methodname}");
            
            var xml = XDocument.Load(filename);
            ConsoleSync.WriteLineT($"{xml}");
           
            var elmnt = xml.XPathSelectElement("/document/data[@id='securities']/rows/row");

            if (elmnt == null)
            {
                ConsoleSync.WriteReadLineT($"Method:{methodname} Element Not Found. Good Bye");
                return;
            }
            var reader = new StringReader(elmnt.ToString());
            var serializer = new XmlSerializer(typeof(MoexTicker));
            var moexSec = (MoexTicker)serializer.Deserialize(reader);

            ConsoleSync.WriteReadLineT(moexSec.ShortInfo);
            ConsoleSync.WriteReadLineT(moexSec.ToString());

            ConsoleSync.WriteReadLineT($"Method:{methodname} Good Bye");
        }
        public override string Key => GetType().FullName + Code;
        protected virtual void OnMessageEvent(string s)
        {
            MessageEvent?.Invoke(this, s);
        }
        protected void SendMessage(string method, string msg)
        {
            if(Verbose) OnMessageEvent($"{method} {msg}");
        }
        protected void SendErrMessage(string method, string msg)
        {
            OnMessageEvent($"{method} {msg}");
        }
        protected void SendErrMessage(string method, string type, string msg)
        {
            OnMessageEvent($"{method} {type} {msg}");
        }
    }
}
