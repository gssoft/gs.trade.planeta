using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.Data.Studies
{
    public interface ILineStudy
    {
        DateTime GetDateTime(int index);
        double GetValue(int index);
    }
    public interface IBandStudy
    {
        DateTime GetDateTime(int index);
        double GetValue(int index);
        double GetHigh(int index);
        double GetLow(int index);
    }
}
