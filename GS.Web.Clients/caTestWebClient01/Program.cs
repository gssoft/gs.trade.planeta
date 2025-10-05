using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebClients;

namespace caTestWebClient01
{
    class Program
    {
        static void Main(string[] args)
        {
            var wcl = new WebClient02<string>(@"http://localhost/WebApiEventLog_01/", "application/xml", "api/values");
            wcl.Start();
            foreach (var i in Enumerable.Range(1, 100))
            {
                wcl.PostItem("Hello world: " + i);
                Thread.Sleep(1*100);
            }
            Console.WriteLine("\r\nPost is Finished. Press any Key ...\r\n");
            Console.ReadLine();
            wcl.Stop();
            Console.WriteLine("Programm is Stopped Properly. Press any key to Exit ...");
            Console.ReadLine();
        }
    }
}
