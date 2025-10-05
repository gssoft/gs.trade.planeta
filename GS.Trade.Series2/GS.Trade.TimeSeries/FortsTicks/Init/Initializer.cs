using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.TimeSeries.FortsTicks.Dal;
using GS.Trade.TimeSeries.FortsTicks.Model;

namespace GS.Trade.TimeSeries.FortsTicks.Init
{
    // public class Initializer : DropCreateDatabaseAlways<FortsTicksContext>
    public class Initializer : DropCreateDatabaseIfModelChanges<FortsTicksContext>
    {
        protected override void Seed(FortsTicksContext db)
        {
            db.InitTickers();
        }
    }
}
