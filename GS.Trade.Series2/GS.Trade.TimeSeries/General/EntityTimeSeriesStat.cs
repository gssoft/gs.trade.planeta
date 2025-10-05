using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;
using GS.Trade.TimeSeries.Interfaces;

namespace GS.Trade.TimeSeries.General
{
    internal class EntityTimeSeriesStat : IEntityTimeSeriesStat
    {
        public long EntityID { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        public long Count { get; set; }
        public DateTime FirstDT { get; set; }
        public DateTime LastDT { get; set; }

        public DateTime FirstDate => FirstDT.Date;

        public DateTime LastDate => LastDT.Date;

        public DatesInterval DatesInterval => new DatesInterval
        {
            Date1 = FirstDate,
            Date2 = LastDate.AddDays(1)
        };
    }
}
