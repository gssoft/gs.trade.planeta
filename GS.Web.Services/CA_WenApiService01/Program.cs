using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.EnhWebApiService01;
using GS.Web.Api.Service01;


namespace CA_WenApiService01
{
    class Program
    {
        static void Main(string[] args)
        {
               RunWebApiService();
            //  RunCustomWebApiService();
            // RunWebApiServiceBase();
        }
        private static void RunWebApiService()
        {
            var m = new MyService {Greatings = "Root MyService", Tries = 10000};
            var webapi = new WebApiService();
            webapi.Start();
            ConsoleSync.WriteReadLine("Starting. Press any key to Stop");
            webapi.Stop();
            ConsoleSync.WriteReadLine("Finished. Press any key...");
        }
        private static void RunCustomWebApiService()
        {
            //var webapi = new EnhWebApiService();
            //webapi.Start();
            //ConsoleSync.WriteReadLine("Starting. Press any key to Stop");
            //webapi.Stop();
            //ConsoleSync.WriteReadLine("Finished. Press any key...");
        }
        private static void RunWebApiServiceBase()
        {
            var webapi = new WebApiServiceBase();
            webapi.Start();
            ConsoleSync.WriteReadLine("Strating. Press any key to Stop");
            webapi.Stop();
            ConsoleSync.WriteReadLine("Finished. Press any key...");
        }
    }
}
