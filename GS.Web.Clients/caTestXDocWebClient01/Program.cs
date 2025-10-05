using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GS.Configurations;
using GS.ConsoleAS;
using GS.Serialization;
using WebClients;

namespace caTestXDocWebClient01
{
    class Program
    {
        static void Main(string[] args)
        {
            var wcl = Builder.Build<XmlWebClient>(@"Init\WebClients.xml", "XmlWebClient");
            if (wcl == null)
            {
                ConsoleSync.WriteLineDT("SerializeFailure:");
                return;
            }
            wcl.Init();
            ConsoleSync.WriteLineT("Start Get");
            //var dt11 = DateTime.Now;
            //var xdoc1 = wcl.GetInString("gs.trade.open", "Strategies", @"strategies/Real/Z706/ROpen_Z735P_151021.xml");
            //var dt12 = DateTime.Now;
            var dt21 = DateTime.Now;
            var xdoc2 = wcl.GetInStream("gs.trade.open", "Strategies", @"ROpen_Z735P_151021.xml");
            var dt22 = DateTime.Now;
            var dt11 = DateTime.Now;
            var xdoc1 = wcl.GetInString("gs.trade.open", "Strategies", @"ROpen_Z735P_151021.xml");
            var dt12 = DateTime.Now;
            ConsoleSync.WriteLineT("Finish Get");
            if (xdoc1 == null || xdoc2 == null)
                ConsoleSync.WriteLineDT("GetFailure: XDoc = null " + wcl.ErrorMessage);
            else
            {
                //Console.WriteLine(xdoc.ToString());
                //var xml = XDocument.Parse(xdoc);
                Console.WriteLine(xdoc1.ToString());
                Console.ReadLine();
                Console.WriteLine(xdoc2.ToString());
            }
            //wcl.Stop();
            ConsoleSync.WriteReadLineT("Xdoc1: " + (dt12 - dt11) + "; XDoc2: " + (dt22 - dt21));

            // Get Cohfig Config + Item
            ConsoleSync.WriteReadLineT("Try to Load Xdoc3");
            var xdoc3 = wcl.GetInStream("GS.Trade.Open", "Strategies");
            if (xdoc3 == null)
                    ConsoleSync.WriteLineDT("GetFailure: XDoc3 = null " + wcl.ErrorMessage);
                else
                {
                    //Console.WriteLine(xdoc.ToString());
                    //var xml = XDocument.Parse(xdoc);
                    Console.WriteLine(xdoc3.ToString());
                }

            ConsoleSync.WriteReadLineT("Programm is Stopped Properly. Press any key to Exit ...");

            ConsoleSync.WriteReadLineT("CheckWebConfigurator. Press any key to Exit ...");

            //var wbconf = new WebConfigurator();
            //wbconf.Init();
            //var strats = wbconf
            
            var wbcnf = Builder.Build2<IConfigurationResourse>(@"Init\WebConfigurator.xml", "WebConfigurator");
            wbcnf.Init();

            ConsoleSync.WriteReadLineT("Get Strategies. Press any key to Exit ...");

            var ss = wbcnf.Get("GS.Trade.Open", "Strategies");
            if (ss == null)
                ConsoleSync.WriteLineDT("GetFailure: ss = null "); // + wcl.ErrorMessage);
            else
            {
                Console.WriteLine(ss.ToString());

                ConsoleSync.WriteReadLineT("Get StrategyFile. Press any key to Exit ...");

                var lst = (from s in ss.Element("ArrayOfString").Elements("string") select s.Value)
                    .ToList();
                foreach (var s in lst)
                {
                    var xd = wbcnf.Get("GS.Trade.Open", "Strategies", s);
                    if (xd == null)
                        ConsoleSync.WriteLineDT("GetFailure: xd "); // + wcl.ErrorMessage);
                    else
                        Console.WriteLine(xd.ToString());

                }
            }
            ConsoleSync.WriteReadLineT("Programm is Stopped Properly. Press any key to Exit ...");
        }
    }
}
