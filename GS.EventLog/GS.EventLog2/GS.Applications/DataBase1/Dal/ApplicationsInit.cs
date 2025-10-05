using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Applications.DataBase1.Model;

namespace GS.Applications.DataBase1.Dal
{
    // public class ApplicationsInit : DropCreateDatabaseAlways<ApplicationsContext1>
    public class ApplicationsInit : DropCreateDatabaseIfModelChanges<ApplicationsContext1>
    {
        protected override void Seed(ApplicationsContext1 db)
        {
            Console.WriteLine("Initialize DataBase");
            var a = new Application()
            {
                Code = "GS.Trade",
                Name = "GS.Trade",
                Description = "GS.Trade",
            };
            Console.WriteLine("App Created");
            var u = new User
            {
                Name = @"gs_trade@mail.ru",
                Guid = "GUID"
            };
            Console.WriteLine("User Created");

            u.Applications.Add(a);

            Console.WriteLine("Application Added to User");

            db.Users.Add(u);
            Console.WriteLine("User Added to DataBase");

            db.SaveChanges();

            Console.WriteLine("Changes Saved");

        }
    }
}
