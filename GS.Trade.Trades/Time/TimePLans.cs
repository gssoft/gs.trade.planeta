using System;
using System.Collections.Generic;
using System.Linq;

namespace GS.Trade.Trades.Time
{
    public class TimePlansOld
    {
        private readonly List<TimePlanOld> _timePlanCollection = new List<TimePlanOld>();

        public TimePlansOld()
        {
            var tp = new TimePlanOld { Name = "Forts Standard", Code = "Forts.Standard", Key = "Forts.Standard".Trim().ToUpper() };

            var tpi = new TimePlanOld.Item{Code = "DaySession", Time1 = new TimeSpan(0, 0, 1), Time2 = new TimeSpan(18, 44, 35)};
            tp.AddTimePeriod(tpi);

            tpi = new TimePlanOld.Item { Code = "EveningSession", Time1 = new TimeSpan(19, 00, 00), Time2 = new TimeSpan(23, 49, 35) };
            tp.AddTimePeriod(tpi);

            _timePlanCollection.Add(tp);

            tp = new TimePlanOld { Name = "Micex Standard", Code = "Micex.Standard", Key = "Micex.Standard".Trim().ToUpper() };
            
            tpi = new TimePlanOld.Item { Code = "DaySession", Time1 = new TimeSpan(0, 0, 1), Time2 = new TimeSpan(18, 43, 58) };
            tp.AddTimePeriod(tpi);

            _timePlanCollection.Add(tp);

            tp = new TimePlanOld { Name = "Forts AllDay", Code = "Forts.AllDay", Key = "Forts.AllDay".Trim().ToUpper() };

            tpi = new TimePlanOld.Item { Code = "Day+Evening", Time1 = new TimeSpan(0, 0, 1), Time2 = new TimeSpan(23, 49, 35) };
            tp.AddTimePeriod(tpi);

            _timePlanCollection.Add(tp);
        }
        public TimePlanOld GetTimePlan( string timePlanKey )
        {
            return (from tp in _timePlanCollection where tp.Key == timePlanKey.Trim().ToUpper() select tp).FirstOrDefault();
        }
        public TimePlanOld RegisterTimeStatusChangedEventHandler(string timePlanKey, TimePlanOld.TimeStatusIsChangedEventHandler act)
        {
            var tplan = (from tp in _timePlanCollection where tp.Key == timePlanKey.Trim().ToUpper() select tp).FirstOrDefault();
            if (tplan != null)
                if (act != null) tplan.TimeStatusIsChangedEvent += act;
            return tplan;
        }
        public void NewTickEventHandler(DateTime dt)
        {
            //var ts = dt.Subtract(dt.Date);
            foreach(var tp in _timePlanCollection)
                tp.NewTickEventHandler(dt);
        } 
    }
    public class TimePlanOld
    {
        public delegate void TimeStatusIsChangedEventHandler(DateTime dt, Status timestatus);
        public event TimeStatusIsChangedEventHandler TimeStatusIsChangedEvent;

        public enum Status {  Unknown = 0, TimeToWork = +1, TimeToRest = -1}

        public string Name { get; set; }
        public string Code { get; set; }
        public string Key { get; set; }

        private Status _timeStatus;

        public bool IsTimeToWork{get{return _timeStatus == Status.TimeToWork ? true : false;}}
        public bool IsTimeToRest{get{return _timeStatus == Status.TimeToRest ? true : false;}}

        private readonly List<Item> _timePlanItemCollection = new List<Item>();

        public void AddTimePeriod(Item tp)
        {
            _timePlanItemCollection.Add(tp);
        }
        private Status GetTimeStatus()
        {
            var t = DateTime.Now.TimeOfDay;
            return GetTimeStatus(t);
        }
        private Status GetTimeStatus(TimeSpan t)
        {
            return _timePlanItemCollection.Any(tp => t.CompareTo(tp.Time1) >= 0 && t.CompareTo(tp.Time2) < 0)
                ? Status.TimeToWork
                : Status.TimeToRest;
        }
        public void NewTickEventHandler(DateTime dt)
        {
            var ts = dt.Subtract(dt.Date);
            var newts = GetTimeStatus(ts);
            if (_timeStatus == newts) return;

            _timeStatus = newts;
            if (TimeStatusIsChangedEvent != null) TimeStatusIsChangedEvent(dt, newts);
        }

        public class Item
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public string Key { get; private set; }

            public TimeSpan Time1 { get; set; }
            public TimeSpan Time2 { get; set; }
        }
    }

}
