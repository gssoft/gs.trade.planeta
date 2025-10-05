using System;
using System.Linq;
using System.Threading;
using GS.ConsoleAS;
using GS.EventLog.Dto;
using GS.EventLog.WebClients;
using GS.Serialization;

namespace caWebEventLogClient01
{
    class Program
    {
        static void Main(string[] args)
        {
            var wcl = Builder.Build<WebEventLogClient01>(@"Init\WebClients.xml", "WebEventLogClient01");
            wcl.Init();
            wcl.Start();
            var dt1 = DateTime.Now;
            foreach (var i in Enumerable.Range(21, 50))
            {
                var evl = new EventLogDto
                {
                    Code = "EventLog" + i,
                    Name = "EventLog" + i,
                    Description = "EventLog" + i, 
                };
                wcl.PostItem(evl);
                Thread.Sleep(1 * 100);
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
