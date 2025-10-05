using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace GS.Web.Service01
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var web = WebApp.Start<Startup>("http://localhost:8081"))
            {
                Console.WriteLine("Running ...");
                Console.ReadLine();
                web.Dispose();
            }
        }
    }
}
