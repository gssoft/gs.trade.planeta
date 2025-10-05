using System;
using GS.Containers5;

namespace GS.Trade.TimeSeries.Model.View
{
    public abstract class Entity : IHaveKey<string>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Description { get; set; }
        public abstract string Key { get; }
    }

    public abstract class TimedEntity : Entity
    {
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

    public class TimeSeriesStat : TimedEntity
    {
        public long ProviderId { get; set; }
        public long TickerId { get; set; }
        public long TimeIntId { get; set; }
        public string Provider { get; set; }
        public string Ticker { get; set; }
        public string TimeInt { get; set; }

        public double MinValue { get; set; }
        public double MaxValue { get; set; }

        public override string Key
        {
            get
            {
                return
                    Ticker + "@" +
                    TimeInt + "@" +
                    Provider;
            }
        }
    }
}