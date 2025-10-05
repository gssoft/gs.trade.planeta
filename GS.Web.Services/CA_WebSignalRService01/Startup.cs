using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Cors;
using Owin;

namespace CA_WebSignalRService01
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Cross Domain
            app.UseCors(CorsOptions.AllowAll);
            // Map SignalR
            app.MapSignalR();
        }
    }
}
