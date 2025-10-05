using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.Studies
{
    public interface IAtr
    {
        //TimeSeriesItem this[int index];
        double GetValue(int index);
        double GetDateTime(int index);
    }
}
