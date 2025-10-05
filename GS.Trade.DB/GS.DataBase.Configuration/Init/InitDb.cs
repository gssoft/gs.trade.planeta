using System.Data.Entity;
using GS.DataBase.Configuration.Dal;
using GS.DataBase.Configuration.Model;

namespace GS.DataBase.Configuration.Init
{
    using Model;
    //public class InitDb : DropCreateDatabaseAlways<ConfigurationContext>
    public class InitDb : DropCreateDatabaseIfModelChanges<ConfigurationContext>
    {
        protected override void Seed(ConfigurationContext db)
        {
             
        }
    }
}
