using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  GS.ConsoleAS;
using GS.EventLog;
using GS.EventLog.DataBase.Dal;
using  GS.Interfaces;
using  GS.Serialization;
using Microsoft.Owin.Hosting;

namespace CApp.GS.Web.Api.Evl.Server01
{
    class Program
    {
        public static IEventLog Evl;
        static void Main(string[] args)
        {
            Database.SetInitializer(new Initializer());

            ConsoleSync.WriteReadLineT("Init Db Complete. Press any key ...");

            Evl = Builder.Build2<IEventLog>(@"Init\EventLog.xml", "EventLogs");
            Evl.Init();

            Evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "EventLog", "Test New EventLog1", "", "", "");

            var dbLazy = DbEventLog2.Instance;

            ConsoleSync.WriteReadLineT("Press any key To Start Web ...");

            dbLazy.Start();

            var serverDef = Builder.Build<ServerDef>(@"Init\WebServer.xml", "ServerDef");
            if(serverDef==null)
                throw new NullReferenceException("Server Definitions is NUll");

            // Specify the URI to use for the local host:
            //string baseUri = "http://localhost:8082";
            string baseUri = serverDef.Url;
            Console.WriteLine("Starting web Server: {0} {1}", serverDef.Code, baseUri);
            WebApp.Start<Startup>(baseUri);

            Console.WriteLine("Server running at {0} - press Enter to Stop. ", baseUri);
            Console.ReadLine();

            dbLazy.Stop();
            ConsoleAsync.WriteLineT("WebEvl State: {0}" , dbLazy.SSStatus.ToString());
            ConsoleSync.WriteReadLineT("WebEvl Stoped. Press any key To Exit ...");
        }
    }
}
