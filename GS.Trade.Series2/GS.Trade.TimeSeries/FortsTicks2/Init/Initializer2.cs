using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.TimeSeries.FortsTicks.Dal;
using GS.Trade.TimeSeries.FortsTicks.Model;
using GS.Trade.TimeSeries.FortsTicks2.Dal;

namespace GS.Trade.TimeSeries.FortsTicks2.Init
{
   //  public class Initializer : DropCreateDatabaseAlways<FortsTicksContext2>
    public class Initializer : DropCreateDatabaseIfModelChanges<FortsTicksContext2>
    {
        protected override void Seed(FortsTicksContext2 db)
        {
            db.InitTickers();
        }
    }
}
