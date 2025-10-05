using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.TimeSeries.General
{
    public enum TimeSeriesStatEnum : int { All = 1, Daily = 2 }
    public enum TickBarTypeEnum : int { Ticks = 1, Bars = 2 }

    public enum TimeSeriesTypeEnum : short {Ticks = 1, Bars = 2, Files = 3}

    public enum StatTimeIntType : int
        { All = 1, Daily = 2, Weekly = 3, Monthly = 4, Quarterly = 5, Annually = 6, Total = 7}

}
