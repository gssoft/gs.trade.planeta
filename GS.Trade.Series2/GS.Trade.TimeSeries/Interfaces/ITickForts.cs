using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.TimeSeries.Interfaces
{
    public interface ITickForts
    {
        int TickerID { get; }
        DateTime DT { get; }
        double Price { get; }
        double Amount { get; }
    }
    public interface ITickForts2
    {
        int SeriesID { get; }
        DateTime DT { get; }
        double Price { get; }
        double Amount { get; }
    }

    public interface ITickBase : ITickForts
    {

    }
}
