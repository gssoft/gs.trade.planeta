using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using Microsoft.Owin.Hosting;

namespace Ca_OwinSelfHost_Auth_01
{
    class Program
    {
        static void Main(string[] args)
        {
            //var baseUri = @"http://localhost:8082";

            var baseUri = @"http://185.31.161.159:8083";

            var webApiApp = WebApp.Start<Startup>(baseUri);

            ConsoleSync.WriteReadLineT($"WebApiSelfHost is Started");

            webApiApp.Dispose();

            ConsoleSync.WriteReadLineT($"WebApiSelfHost is Finished");
        }
    }
}
