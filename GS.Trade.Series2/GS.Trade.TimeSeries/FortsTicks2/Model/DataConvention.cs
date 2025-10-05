using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.TimeSeries.FortsTicks2.Model
{
    public class DateConvention : Convention
    {
        public DateConvention()
        {
            //this.Properties<DateTime>()
            //    .Configure(c => c.HasColumnType("datetime2").HasPrecision(3));

            this.Properties<DateTime>()
                .Where(x => x.GetCustomAttributes(false).OfType<DataTypeAttribute>()
                .Any(a => a.DataType == DataType.Date))
                .Configure(c => c.HasColumnType("date"));
        }
    }
}
