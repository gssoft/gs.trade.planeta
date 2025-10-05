using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Serialization;
using WebClients;
using GS.Trade.Dto;
using System.Web;

namespace CaTest_BarWebClient01
{
    
    class Program
    {
        static void Main(string[] args)
        {
            var wcl = Builder.Build<BarWebClient>(@"Init\WebClients.xml", "BarWebClient");
            if(wcl==null)
                throw new NullReferenceException("webClient After Build is null");
            wcl.Init();

            var example = HttpUtility.UrlEncode("");

            ConsoleSync.WriteLineT("Start GetBars");
            var dt1 = DateTime.Now;
            var bars = wcl.GetItems("?ticker=MARS&timeint=5");
            //var bars = wcl.GetItems();
            if (bars == null)
                return;
            var dt2 = DateTime.Now;
            ConsoleSync.WriteLineT("End GetBars. TimeDiff: {0} Count: {1}", dt2 - dt1, bars.Count());
            
            ConsoleSync.WriteReadLine("Press any key ...");

            var bs = bars.ToList();
            
            foreach (var b in bs)
                Console.WriteLine(b.ToString());
            Console.WriteLine("Count: {0}", bs.Count());
            Console.ReadLine();
        }
    }
}
