using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Web.Api.Services.Lib;
using GS.WebApiControllers;

namespace Ca_WebApiService02
{
    class Program
    {
        static void Main(string[] args)
        {
            RunWebApiServiceBase();
        }
        private static void RunWebApiServiceBase()
        {
            //var webapi = new WebApiServiceBase();
            var webapi = new WevApiServiceEnhance();
            // webapi.Serialize();
            // var v = webapi.Deserialize();
            webapi.Start();
            ConsoleSync.WriteReadLine("Starting. Press any key to Stop");
            webapi.Stop();
            ConsoleSync.WriteReadLine("Finished. Press any key...");
        }
    }
}
