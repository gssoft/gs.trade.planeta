using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.Extension;

namespace GS.Trade.Trades.Equity
{

        #region Equity

        //public interface IHaveKey<out TKey>
        //{
        //    TKey Key { get; }
        //}

        public class EquityItem : IHaveKey<DateTime>
        {
            public DateTime DateTime { get; set; }
            public double Change { get; set; }
            public double Value { get; set; }
            public double DrawDown { get; set; }
            public DateTime Key
            {
                get { return DateTime; }
            }

            public override string ToString()
            {
                return string.Format("DT:{0} Change:{1} Value:{2} DD:{3}",
                    DateTime.ToString("yyMMdd"), Change.Round(2), Value.Round(2), DrawDown.Round(2));
            }
        }
        public abstract class TimeInterval
        {
            public int TimeValue { get; set; }

            public abstract DateTime GetEquityDate(DateTime dt);
        }
        public class IntraDayTimeInterval : TimeInterval
        {
            public override DateTime GetEquityDate(DateTime dt)
            {
                var minutes = (dt.Minute/5)*5;
                return new DateTime(dt.Year,dt.Month,dt.Day,dt.Hour,minutes,0);
                //return dt;
            }
        }
        public class DailyTimeInterval : TimeInterval
        {
            public override DateTime GetEquityDate(DateTime dt)
            {
                return dt.Date;
            }
        }
        public class WeeklyTimeInterval : TimeInterval
        {
            public override DateTime GetEquityDate(DateTime dt)
            {
                var dweek = (int)dt.DayOfWeek;
                return dt.AddDays(-(dweek - 1)).Date;
            }
        }
        public class MonthlyTimeInterval : TimeInterval
        {
            public override DateTime GetEquityDate(DateTime dt)
            {
                return new DateTime(dt.Year, dt.Month, 1).Date;
            }
        }
        public class QuartelyTimeInterval : TimeInterval
        {
            public override DateTime GetEquityDate(DateTime dt)
            {
                var m = 0;
                if (dt.Month >= 1 && dt.Month <= 3)
                    m = 1;
                else if (dt.Month >= 4 && dt.Month <= 6)
                    m = 4;
                else if (dt.Month >= 7 && dt.Month <= 9)
                    m = 7;
                else
                    m = 10;
                // m = dt.Month%3 > 0 ? 1 + 3*(dt.Month/3) : dt.Month/3;
                return new DateTime(dt.Year, m, 1).Date;
            }
        }
        public class AnnualyTimeInterval : TimeInterval
        {
            public override DateTime GetEquityDate(DateTime dt)
            {
                return new DateTime(dt.Year, 1, 1).Date;
            }
        }

        public class Equity
        {
            public TimeInterval TimeInterval { get; set; }
            private readonly ConcurrentDictionary<DateTime, EquityItem> _equityCollection;
            private EquityItem _currentItem;

            public IEnumerable<EquityItem> EquityItems
            {
                get
                {
                    ReCalc();
                    return _equityCollection.Values.OrderBy(i => i.DateTime);
                }
            }

            public Equity(int timeint)
            {
                _equityCollection = new ConcurrentDictionary<DateTime, EquityItem>();
                TimeInterval = GetTimeInterval(timeint);
            }

            public void Update(DateTime dt, double value)
            {
                var edt = TimeInterval.GetEquityDate(dt);
                EquityItem ei;
                if (_equityCollection.TryGetValue(edt, out ei))
                {
                    ei.Change += value;
                }
                else
                    _equityCollection.TryAdd(edt, new EquityItem
                    {
                        DateTime = edt,
                        Change = value,
                        Value = value
                    });
            }
            private void ReCalc()
            {
                var tot = 000000d;
                var curEqMax = 0d;
                foreach (var ei in _equityCollection.Values.OrderBy(i => i.DateTime))
                {
                    tot += ei.Change;
                    ei.Value = tot;
                    if (tot > curEqMax)
                        curEqMax = tot;
                    ei.DrawDown = curEqMax - tot;
                }
            }

            public double GetMaxDrawDown()
            {
                return EquityItems.Max(ei => ei.DrawDown);
            }
            public double GetAvgDrawDown()
            {
                return EquityItems.Average(ei => ei.DrawDown);
            }
            public double GetStdDevDrawDown()
            {
                return EquityItems.Select(eq => eq.DrawDown).StdDev().Round(2);
            }

            private TimeInterval GetTimeInterval(string timeInt)
            {
                switch (timeInt)
                {
                    case "IntraDay":
                        return new IntraDayTimeInterval();
                    case "Daily":
                        return new DailyTimeInterval();
                    case "Weekly":
                        return new WeeklyTimeInterval();
                    case "Monthly":
                        return new MonthlyTimeInterval();
                    case "Quarterly":
                        return new QuartelyTimeInterval();
                    case "Annually":
                        return new AnnualyTimeInterval();
                    default:
                        return new WeeklyTimeInterval();
                }
            }
            private TimeInterval GetTimeInterval(int timeInt)
            {
                switch (timeInt)
                {
                    case 10:
                        return new IntraDayTimeInterval();
                    case 1:
                        return new DailyTimeInterval();
                    case 2:
                        return new WeeklyTimeInterval();
                    case 3:
                        return new MonthlyTimeInterval();
                    case 4:
                        return new QuartelyTimeInterval();
                    case 5:
                        return new AnnualyTimeInterval();
                    default:
                        return new WeeklyTimeInterval();
                }
            }

            public static string GetTimeIntTitle(int timeInt )
            {
                switch (timeInt)
                {
                    case 10:
                        return "IntraDay";
                    case 1:
                        return "Daily";
                    case 2:
                        return "Weekly";
                    case 3:
                        return "Monthly";
                    case 4:
                        return "Quartely";
                    case 5:
                        return "Annualy";
                    default:
                        return "Unknown";
                }
            }

        }

        #endregion
    
}
