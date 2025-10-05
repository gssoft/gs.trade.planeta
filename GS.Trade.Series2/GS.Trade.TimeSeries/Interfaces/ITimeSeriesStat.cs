using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;

namespace GS.Trade.TimeSeries.Interfaces
{
    public interface IEntityTimeSeriesStat
    {
        long EntityID { get; }
        string EntityCode { get; }
        string EntityName { get; }
        long Count { get; }
        DateTime FirstDT { get; }
        DateTime LastDT { get; }

        DateTime FirstDate { get; }
        DateTime LastDate { get; }
        DatesInterval DatesInterval { get; }
    }
}
