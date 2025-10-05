using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using GS.ConsoleAS;
using GS.Elements;
using GS.Extension;
using GS.Serialization;
using GS.Status;
using Moex;
using Moex.TradesRequester;
using WebClients;
using GS.Moex.Interfaces;

namespace Ca_Get_Bars
{
    class Program
    {
        public static MoexTest Moex;
        public static List<MoexTrade> TradeList;
        public static TradesRequester TradesRequester; 

        static void Main(string[] args)
        {
            MoexInterface<MoexTicker>();
           // RunTradeRequesterService();
            //GetTradesCycle();
            // GetTradesPack();
            // GetBars();
            // GetInstrument();
            // GetInstrumentFromFile();
            // GetInstrumentFromWeb();
        }
        private static void RunTradeRequesterService()
        {
            TradesRequester = Builder.Build2<TradesRequester>(@"Init\TradesRequester.xml", "TradesRequester");
            TradesRequester.MessageEvent += PrintMessage;
            TradesRequester.Init();
            ConsoleSync.WriteReadLineT("Press Any Key to Start");
            TradesRequester.StartGetTradesService();
            Thread.Sleep(1000);
            ConsoleSync.WriteReadLineT("Press Any Key to Stop");
            TradesRequester.StopGetTradesService();
            Thread.Sleep(1000);
            ConsoleSync.WriteReadLineT("Press Any Key to Terminate");
        }


        static void GetTrades()
        {
            ConsoleSync.WriteLineT($"Start Method: {System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            string link =
                "https://iss.moex.com/iss/engines/futures/markets/forts/securities/SiZ3/trades.json?start=0&limit=10";
            string dataLine;
            int count = 0;
            using (WebClient wc = new WebClient())
            {
                using (Stream stream = wc.OpenRead(link))
                {
                    if (stream != null)
                    {
                        StreamReader sr = new StreamReader(stream);
                        while ((dataLine = sr.ReadLine()) != null)
                        {
                            if (count >= 14 && count <= 23)
                                ConsoleSync.WriteLineT(dataLine);
                            count += 1;
                        }
                    }
                    else ConsoleSync.WriteLineT($"Stream is Empty");
                    ConsoleSync.WriteLineT($" Count: {count}");
                    stream?.Close();
                }
            }
            ConsoleSync.WriteLineT($"Finish Method: {System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            Console.ReadLine();
        }
        static void GetBars()
        {
            var limit = 500;

            ConsoleSync.WriteLineT($"Method: {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            //string link1 =
            //   "http://iss.moex.com/iss/engines/futures/markets/forts/boards/RFUD/securities/SiZ7/candles.csv?from=2017-11-08&till=2017-11-08&interval=1&start=StartValue&limit=LimitValue";
            //string link2 =
            //    "http://iss.moex.com/iss/engines/futures/markets/forts/boards/RFUD/securities/SiZ7/candles.csv?from=2017-11-01&till=2017-11-10&interval=1&start=StartValue";
            //string link =
            //    "http://iss.moex.com/iss/engines/futures/markets/forts/boards/RFUD/securities/SiZ7/candles.csv?from=2017-11-08&till=2017-11-08&interval=1&start=0&limit=100";

            string link2 =
                "http://iss.moex.com/iss/engines/futures/markets/forts/boards/RFUD/securities/SiZ9/candles.csv?from=2019-09-01&till=2019-09-30&interval=1&start=StartValue";
            //string link2 =
            //    "http://iss.moex.com/iss/engines/futures/markets/forts/boards/RFUD/securities/SiZ7/candles.csv?from=2017-11-01&till=2017-11-10&interval=1&start=StartValue";
            // http://iss.moex.com/iss/engines/futures/markets/forts/boards/RFUD/securities/SiZ9/candleborders.xml
            int lineCount = 0;
            var tryes = 0;

            var portionCount = 0;
            var portionDataCount = 1;
            var allDataCount = 0;

            var firstData = string.Empty;
            var lastData = string.Empty;

            using (WebClient wc = new WebClient())
            {
                while (portionDataCount != 0)
                {
                    var lnk = link2
                        // .Replace("LimitValue", limit.ToString())
                        //.Replace("StartValue", (tryes*limit).ToString());
                        .Replace("StartValue", allDataCount.ToString());

                    //lnk = link.Replace("StartValue", tryes.ToString());
                    tryes++;
                    using (Stream stream = wc.OpenRead(lnk))
                    {
                        portionDataCount = 0;
                        lineCount = 0;

                        if (stream != null)
                        {
                            portionCount++;
                            var sr = new StreamReader(stream);

                            string dataLine;
                            while ((dataLine = sr.ReadLine()) != null)
                            {
                                lineCount += 1;
                                if (lineCount < 4 || dataLine == Environment.NewLine || dataLine == "")
                                    continue;
                                if (dataLine != Environment.NewLine && dataLine != "")
                                    //if (count >= 14 && count <= 23)
                                {
                                    Console.WriteLine(dataLine);
                                    //ConsoleSync.WriteReadLineT(count.ToString());
                                    if (firstData.HasNoValue()) firstData = dataLine;
                                    lastData = dataLine;
                                    portionDataCount++;
                                    allDataCount++;
                                }
                            }
                            ConsoleSync.WriteLineT(
                                $"TryesCnt:{tryes} PortionCnt:{portionCount} String Count:{lineCount};Portion Bars Count:{portionDataCount}; All Bars Count: {allDataCount}");
                            stream?.Close();
                        }
                        else
                            ConsoleSync.WriteLineT("Responce stream is Null");
                    }
                }
            }
            ConsoleSync.WriteLineT(
                $"TryesCnt:{tryes} PortionCnt:{portionCount} LineCount:{lineCount} PortionBars Count:{portionDataCount} AllBars Count:{allDataCount}");
            ConsoleSync.WriteLineT($"FirstBar:{firstData}");
            ConsoleSync.WriteLineT($"LastBar:{lastData}");
            ConsoleSync.WriteReadLineT($"Finish Method: {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }
        static void GetLastTrade()
        {
            string newLine;
            string[] lastLine;
            string link =
                "https://iss.moex.com/iss/engines/futures/markets/forts/securities/SiZ7/trades.json?reversed=1&limit=1";
            int count = 0;
            for (;;)
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(link);
                request.ContentType = "text/plain; charset=utf-8";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(responseStream, Encoding.UTF8);
                    while ((newLine = sr.ReadLine()) != null)
                    {
                        if (count == 14)
                        {
                            if (newLine == "") break;
                            else
                            {
                                lastLine = newLine.Split(',');
                                Console.WriteLine("Volume is " + lastLine[6] + " at Price " + lastLine[5]);
                            }
                        }
                        count++;
                    }
                }
                count = 0;
                response.Close();
            }
        }
        private static void GetInstrument()
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"Method: {methodname}");

            var link = "https://iss.moex.com/iss/engines/futures/markets/forts/securities/SiZ9.xml";

            using (WebClient wc = new WebClient())
            {
                // using (var data2 = wc.DownloadData(link))
                //{
                    // var data = wc.DownloadData(link);

                    var data = wc.DownloadString(link);
                    XDocument xml = XDocument.Parse(data);
                    // "/ipb/profile[id='1253']/name"
                    var xPathSelectElement = xml.XPathSelectElement("/document/data[@id='securities']/rows/row");
                    if (xPathSelectElement != null)
                    {
                        string name = xPathSelectElement.Value;
                    }
                    ConsoleSync.WriteLineST($"Data: data: {data}");
                    ConsoleSync.WriteReadLineT($"Good Buy method {methodname}");
                //}
            }
        }
        private static void GetInstrumentFromWeb()
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"Method: {methodname}");

            //var web = new WebClient02<string>();
            //var name = web.GetType().FullName;

            var webclient = Builder.Build2<StreamWebClient>(@"Init\WebClient.xml", "StreamWebClient");
            var wcstr = webclient.ToString();
            webclient.Init();
            // var str = webclient.GetItemInStream(@"forts/securities/SiZ0.xml");
            var str = webclient.GetItemInStreamAsync(@"forts/securities/SiZ0.xml").Result;

            var xdoc = XDocument.Load(str);
            ConsoleSync.WriteLineT($"{xdoc}");

            var moex = new MoexTest();
            var sec = moex.GetTickerFromXDoc(xdoc, "/document/data[@id='securities']/rows/row");

            ConsoleSync.WriteReadLineT(sec.ShortInfo);
            ConsoleSync.WriteReadLineT(sec.ToString());

            ConsoleSync.WriteReadLineT($"Method:{methodname} Good Bye");

        }
        private static void GetInstrumentFromFile()
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"Method: {methodname}");

            // var xmlFile = @"Init\SiZ9.xml";
            // var xmlFile = @"Init\BRZ9.xml";
            //var xmlFile = @"Init\Si64500BU9D.xml";
            var xmlFile = @"Init\Si64500BI9D.xml";

            var xml = XDocument.Load(xmlFile);
            ConsoleSync.WriteLineT($"{xml}");

            // "/ipb/profile[id='1253']/name"

            // var xPathSelectElement = xml.XPathSelectElement("/document/data[id='securities']/rows/row");
            // var elmnt = xml.XPathSelectElement("/document/data/rows/row");
            var elmnt = xml.XPathSelectElement("/document/data[@id='securities']/rows/row");

            if (elmnt == null)
            {
                ConsoleSync.WriteReadLineT($"Method:{methodname} Element Not Found. Good Bye");
                return;
            }
            var reader = new StringReader(elmnt.ToString());
            var serializer = new XmlSerializer(typeof(MoexSecurityElement));
            var moexSec = (MoexSecurityElement) serializer.Deserialize(reader);

            ConsoleSync.WriteReadLineT(moexSec.ShortInfo);
            ConsoleSync.WriteReadLineT(moexSec.ToString());

            ConsoleSync.WriteReadLineT($"Method:{methodname} Good Bye");
        }
        public static void InitMoex()
        {
            TradeList = new List<MoexTrade>();
            Moex = Builder.Build3<MoexTest>(@"Init\Moex.xml", "MoexTest");
            Moex.Init();
        }
        public static void GetTradesCycle()
        {
            var methodbase = MethodBase.GetCurrentMethod().Name + "()";

            InitMoex();
            if (Moex == null)
            {
                ConsoleSync.WriteReadLineDT("Moex is Null");
                return;
            }
            var working = true;
            var t = Task.Factory.StartNew(() =>
            {
                var start = 0;
                //var tradeCount = 0;
                var path = 0;
                ConsoleSync.WriteLineT("Task Starting");
                while (working)
                {
                    var trades = Moex.GetTrades("SPBFUT", "SiZ9", start, 1000);
                    if (trades == null)
                    {
                        ConsoleSync.WriteLineT($"{methodbase} Something Wrong with Trade Receiving ...");
                        ConsoleSync.WriteLineT(
                            $"MainPath:{++path} Start:{start} TradesCnt:{TradeList.Count}");
                        Thread.Sleep(1*60*1000);
                        continue;
                    }
                    TradeList.AddRange(trades);
                    // tradeCount += trades.Count;
                    var tradesCnt = trades.Count;
                    start += tradesCnt;
                    var tradelistcnt = TradeList.Count;
                    ConsoleSync.WriteLineT(
                        tradelistcnt > 0
                            ? $"MainPath:{++path} TradesCnt:{tradelistcnt} [{TradeList[0].TradeNumber}]-[{TradeList[tradelistcnt - 1].TradeNumber}]"
                            : $"MainPath:{++path} TradesCnt:{tradelistcnt} Start:{start}");
                    Thread.Sleep(1 * 60 * 1000);
                }
                ConsoleSync.WriteLineT("Task Finishing");
            });
            ConsoleSync.WriteReadLineT("Press Any Key to Stop");
            working = false;
            ConsoleSync.WriteLineT($"Final Totals: TradeList.Cnt:{TradeList.Count}");
            ConsoleSync.WriteReadLineT("Press Any Key to Terminate");
        }
        private static void MoexInterface<T>()  
        {
            var t = typeof (T);
            if(t.IsInterface)
                ConsoleSync.WriteLineT($"Interface: {t.FullName} {t.Name}");
            else
                ConsoleSync.WriteLineT($"Type: {t.FullName} {t.Name}");

            var i = t.GetInterface("Moex.IMoexTicker", true);

            ConsoleSync.WriteLineT($"Interface: {i.FullName} {i.Name}");
            ConsoleSync.WriteReadLineT("Press any key to exit ...");            
        }
        public static void PrintMessage(object sender, string s)
        {
            ConsoleSync.WriteLineT(s);
        }
    }
}
