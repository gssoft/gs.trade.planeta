using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;

namespace CA_Test_Dde_01
{
    class Program
    {
        static void Main(string[] args)
        {
            var cntx = new DdeTestContext
            {
                IsNeedEventHub = false,
                IsNeedWorkTasks = false,
                IsNeedEventLog = true
            };
            cntx.Init();
            ConsoleSync.WriteReadLineT("Init is Completed ...");
            cntx.Start();
            ConsoleSync.WriteReadLineT("Start is Completed ...");
            cntx.Stop();
            ConsoleSync.WriteReadLineT("Stop is Completed ...");
        }
    }
}
