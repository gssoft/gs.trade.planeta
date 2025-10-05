using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Serialization;

namespace FtpRequestParse_02
{
    class Program
    {
        static void Main(string[] args)
        {
            // var ftpWebClient = new FtpWebClient();

            var ftpWebClient = Builder.Build<FtpWebClient>(@"Init\FtpWebClient.xml", "FtpWebClient");

            ftpWebClient.Init();
            var lst = ftpWebClient.GetListDirectoryDetails("").ToList();

            foreach (var l in lst)
            {
                ConsoleSync.WriteLineT(l.ToString());
            }

            var cnt = lst.Count(i => i.IsDirectory);

            ConsoleSync.WriteReadLineT($"Dirs: {lst.Count(i => i.IsDirectory )-2} Files: {lst.Count(i => !i.IsDirectory)} ");
            ConsoleSync.WriteReadLineT("Press Any Key ...");
        }
    }
}
