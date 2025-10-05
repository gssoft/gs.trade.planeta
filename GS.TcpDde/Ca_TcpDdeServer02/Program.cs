using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.TcpDde;

namespace Ca_TcpDdeServer02
{
    class TcpDdeServer
    {
        private const string IpAddress = "127.0.0.1";
        private const int Port = 8082;

        static void Main(string[] args)
        {
            var tcpdde = new TcpDdeServer01(IpAddress, Port)
            {
                RealDdeServer = true
            };
            var classname = tcpdde.GetType().Name;
            Console.Title = classname;

            tcpdde.Init();
            tcpdde.Start();
            // Thread.Sleep(15*1000);
            ConsoleSync.WriteReadLineT("Press any key to Stop");
            tcpdde.Stop();
            ConsoleSync.WriteReadLineT("Press any key to Finish");
        }
    }
}
