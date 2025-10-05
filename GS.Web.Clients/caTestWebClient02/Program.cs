using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Serialization;
using WebClients;

namespace caTestWebClient02
{
    class Program
    {
        static void Main(string[] args)
        {
            var wcl = Builder.Build<StringWebClient>(@"Init\WebClients.xml", "StringWebClient");
            wcl.Init();
            wcl.Start();
            var dt1 = DateTime.Now;
            foreach (var i in Enumerable.Range(1, 1000))
            {
                wcl.PostItem("Hello world: " + i);
                Thread.Sleep(1 * 1);
            }
            var dt2 = DateTime.Now;
            var eltime = dt2 - dt1;
            //ConsoleAsync.WriteLine(String.Format("\r\nPost is Finished.\r\n ElapsedTiem: {0} Press any Key ...\r\n", eltime.ToString("c")));
            ConsoleAsync.WriteLineDT("\r\nPost is Finished.\r\nElapsedTiem: {0} Press any Key ...\r\n", eltime.ToString("c"));
            Console.ReadLine();
            wcl.Stop();
            ConsoleAsync.WriteLineD("Programm is Stopped Properly. Press any key to Exit ...");
            Console.ReadLine();
        }
    }
}
