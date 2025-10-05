using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Trade.Bars;

namespace Test_CaBars_01
{
    class Program
    {
        static void Main(string[] args)
        {
            var bcx = new BarCreatorContext();
            
            bcx.Init();
            System.Threading.Thread.Sleep(3 * 1000);
            ConsoleSync.WriteReadLineT("Init is Completed ...");
            bcx.Start();
            System.Threading.Thread.Sleep(3 * 1000);
            ConsoleSync.WriteReadLineT("Start is Completed ...");
            bcx.Stop();
            System.Threading.Thread.Sleep(3 * 1000);
            ConsoleSync.WriteReadLineT("Stop is Completed ...");
        }
    }
}
