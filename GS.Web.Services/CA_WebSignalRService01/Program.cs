using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace CA_WebSignalRService01
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "http://localhost:8081";

            Log("Try to start Server", url);

            using (WebApp.Start<Startup>(url))
            {
                // Console.WriteLine("Server running on {0}", url);
                Log("Server running on", url);

                Console.ReadLine();
            }
        }
        private static void Log(string subject, string message)
        {
            Console.WriteLine("{0:T} {1} {2}", DateTime.Now.TimeOfDay, subject, message);
        }
    }
}
