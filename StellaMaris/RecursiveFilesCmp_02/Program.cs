using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Serialization;

namespace RecursiveFilesCmp_02
{
    class Program
    {
        static void Main(string[] args)
        {
            var work = true;
            try
            {
                var cnf = Builder.Build<Config>(@"Init\Config.xml", "Config");

                var cntxt = Builder.Build<FtpFilesComparer>(cnf.FtpFileComparer, "FtpFilesComparer");
                cntxt.Init();
                ConsoleSync.WriteReadLineT("Init is Completed ...");
                cntxt.Start();
                // ConsoleSync.WriteReadLineT("Start is Completed ...");
                ConsoleSync.WriteReadLineT("Start is Completed ...");
                while (work)
                {

                    Console.Write(">");
                    var answer = Console.ReadLine();
                    var r = DoWork(answer);
                    switch (r)
                    {
                        case 1:
                            cntxt.DoWork();
                           // _work = true;
                            break;
                        case -1:
                            work = false;
                            break;
                    }                      
                }
                cntxt.Stop();
                ConsoleSync.WriteReadLineT("Stop is Completed ...");
            }
            catch (Exception ex)
            {
                ConsoleSync.WriteReadLineT(ex.Message);
            }
        }

        private static int DoWork(string ans)
        {
            switch (ans.ToLower())
            {
                case "go":
                case "start":
                case "do":
                    return 1;
                case "exit":
                case "quit":
                case "end":
                case "buy":
                    return -1;
                default:
                    return 0;
            }
        }

    }
}
