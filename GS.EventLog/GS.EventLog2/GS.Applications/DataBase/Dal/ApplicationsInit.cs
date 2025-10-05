using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Applications.DataBase.Model;
using GS.ConsoleAS;

namespace GS.Applications.DataBase.Dal
{
    //public class ApplicationsInit : DropCreateDatabaseAlways<ApplicationsContext>
    public class ApplicationsInit : DropCreateDatabaseIfModelChanges<ApplicationsContext>
    {
        protected override void Seed(ApplicationsContext db)
        {
            ConsoleSync.WriteLineT("DataBase Initialization Start");
            var a = new Application()
            {
                Code = "GS.Trade",
                Name = "GS.Trade",
                Description = "GS.Trade",
            };
            var u = new User
            {
                Name = @"gs_trade@mail.ru",
                Guid = "GUID"
            };
            u.Applications.Add(a);
            a.Users.Add(u);

            db.Applications.Add(a);
            db.Users.Add(u);

            db.SaveChanges();
            ConsoleSync.WriteLineT("DataBase Initialization Complete");
        }
    }
}
