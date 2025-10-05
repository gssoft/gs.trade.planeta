using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;

namespace GS.Trade.Dto
{
    //public abstract class Entity : IHaveKey<string>
    //{
    //    public long Id { get; set; }
    //    public string Code { get; set; }
    //    public string Name { get; set; }
    //    public string Alias { get; set; }
    //    public string Description { get; set; }
    //    public abstract string Key { get; }
    //}

    public abstract class TimedEntity // : Entity
    {
        public long Id { get; set; }
        public DateTime FirstDate { get; set; }
        public DateTime LastDate { get; set; }

        public int Days
        {
            get
            {
                return 1 + (LastDate.Date - FirstDate.Date).Days;
            }
        }

        public long Count { get; set; }
    }

    public class TimeSeriesStat : TimedEntity, IHaveKey<string>
    {
        public long ProviderId { get; set; }
        public long TickerId { get; set; }
        public long TimeIntId { get; set; }
        public string Provider { get; set; }
        public string Ticker { get; set; }
        public string TimeInt { get; set; }

        public string Key
        {
            get
            {
                return
                    Ticker + "@" +
                    TimeInt + "@" +
                    Provider;
            }
        }

        public override string ToString()
        {
            return String.Format("Id: {0}; FirstDate: {1}; LastDate: {2}; Days: {3}; Count: {4} " +
                                 "ProviderId: {5} Provider: {6}; TickerId: {7} Ticker: {8}; TimeIntId: {9} TimeInt: {10}; Key: {11}",
                                 Id, FirstDate.ToString("d"), LastDate.ToString("d"), Days, Count,
                                 ProviderId,Provider,TickerId,Ticker,TimeIntId,TimeInt,Key);
        }
    }
}

