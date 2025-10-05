using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Contexts;
using Microsoft.Owin.Hosting;
using GS.Interfaces.Service;
using GS.Serialization;

namespace GS.Web.Api.Service01
{
    public class WebApiService : IService
    {
        protected IDisposable WebApi;
        private IMyService Myservice; // = new MyService {Greatings = "Hello World"};
        protected IServContext TestContext;

        //public WebApiService()
        //{
        //    // Myservice = myservice;
        //}

        public void Start()
        {
            //using (var web = WebApp.Start<Startup>("http://localhost:8081"))
            //{
            //    Console.WriteLine("Running ...");
            //    Console.ReadLine();
            //    web.Dispose();
            //}
            var uri = new UriBuilder("http", "localhost", 8082);
           // Myservice.WhoAreYou();
            try
            {
                // UnityConfig
                // Create ServiceContext
                // TestContext = GS.Contexts.TestContext.Instance;

                WebApi?.Dispose();
                Myservice = new MyService {Greatings = "Hello World", Tries = 100};
                WebApi = WebApp.Start<Startup>("http://localhost:8081");
                // WebApi = WebApp.Start<Startup>(uri.ToString());
                Console.WriteLine("Running ...");
            }
            catch (Exception e) 
            {
                ConsoleSync.WriteLineT($"Exception: {e.Message}");
                Stop();
            }
        }
        public void Stop()
        {
            Console.WriteLine("Stop ...");
            WebApi?.Dispose();
        }
    }
}
