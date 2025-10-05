using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using Microsoft.Owin.Hosting;
using GS.Interfaces.Service;

namespace GS.Web.Api.Service01
{
    public class WebApiService : IService
    {
        protected IDisposable WebApi;

        public void Start()
        {
            //using (var web = WebApp.Start<Startup>("http://localhost:8081"))
            //{
            //    Console.WriteLine("Running ...");
            //    Console.ReadLine();
            //    web.Dispose();
            //}
            try
            {
                WebApi = WebApp.Start<Startup>("http://localhost:8081");
                Console.WriteLine("Running ...");
            }
            catch (Exception e) 
            {
                ConsoleSync.WriteLineT($"Exception: {e.Message}");
            }
            
            // Console.ReadLine();
        }
        public void Stop()
        {
            Console.WriteLine("Stop ...");
            WebApi?.Dispose();
        }
    }
}
