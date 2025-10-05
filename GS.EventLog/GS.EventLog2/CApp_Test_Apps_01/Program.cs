using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// using GS.Applications.DataBase1.Dal;
using GS.Applications.DataBase1.Dal;
using GS.Applications.DataBase1.Model;
using GS.ConsoleAS;
using ApplicationsInit = GS.Applications.DataBase.Dal.ApplicationsInit;

namespace CApp_Test_Apps_01
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleSync.WriteLineT("Init DataBase with name Applications");
            Database.SetInitializer(new ApplicationsInit());
            
            var dbApps = new ApplicationsContext1();
            var a = new Application
            {
                Code = "GS.Trade.EventLog",
                Name = "GS.Trade.EventLog"
            };
            var u = new User
            {
                Name = @"gs_order@mail.ru",
                Guid = "Guid"
            };
            //u.Applications.Add(a);
            //a.Users.Add(u);

            dbApps.Applications.Add(a);
            dbApps.Users.Add(u);

            dbApps.SaveChanges();
            
            var apps = dbApps.Applications.ToList();
            foreach (var app in apps)
            {
                Console.WriteLine(app.Code);
            }
            var users = dbApps.Users.ToList();
            foreach (var au in users)
            {
                Console.WriteLine(u.Name);
            }

            ConsoleSync.WriteReadLineT("Program Completed. Press any Key ....");
        }
    }
}
