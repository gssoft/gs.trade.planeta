using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.EventLog.DataBase.Dal
{
    public class Initializer : DropCreateDatabaseIfModelChanges<EvlContext>
    {
        protected override void Seed(EvlContext context)
        {
          //  context.SaveChanges();
        }
    }
}
