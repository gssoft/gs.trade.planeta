using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Extension
{
    public static class DateTimeExt
    {
        public static DateTime MinValueToNow(this DateTime dt)
        {
            return dt.CompareTo(DateTime.MinValue) <= 0
                ? DateTime.Now
                : dt;
        }
        public static DateTime MinValueToSql(this DateTime dt)
        {
            return dt.CompareTo((DateTime) SqlDateTime.MinValue) < 0
                ? (DateTime) SqlDateTime.MinValue
                : dt;
        }

        public static long ToLongInSec(this DateTime dt)
        {
            return dt.Second +
                    dt.Minute * 100 +
                    dt.Hour * 10000 +
                    dt.Day * 1000000 +
                    dt.Month * 100000000 +
                    dt.Year * 10000000000
                   ;
        }

        public static int CompareInSecTo(this DateTime dt1, DateTime dt2)
        {
            var dtl1 = dt1.ToLongInSec();
            var dtl2 = dt2.ToLongInSec();

            return dtl1.CompareTo(dtl2);
        }

        public static bool IsGreaterInSecThan(this DateTime dt1, DateTime dt2)
        {
            return dt1.CompareInSecTo(dt2) > 0;
        }
        public static bool IsLessInSecThan(this DateTime dt1, DateTime dt2)
        {
            return dt1.CompareInSecTo(dt2) < 0;
        }

        public static int DateToInt(this DateTime dt)
        {
            return dt.Day +
                    dt.Month * 100 +
                    dt.Year * 10000
                   ;
        }
        public static DateTime IntToDate(this int i)
        {
            var year = i / 10000;
            var month = (i - year * 10000) / 100;
            var day = (i - year * 10000) % 100;

            return new DateTime(year, month, day);
        }
        public static int TimeToInt(this DateTime dt)
        {
            return dt.Second +
                    dt.Minute * 100 +
                    dt.Hour * 10000
                   ;
        }
        public static DateTime ToDateTime(this long longDt)
        {
            var s = longDt.ToString(CultureInfo.InvariantCulture);
            var dt = DateTime.ParseExact(s, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            return dt;
        }
        public static DateTime Inc(this DateTime dt)
        {
            return dt.AddDays(1);
        }
        public static DateTime Dec(this DateTime dt)
        {
            return dt.AddDays(-1);
        }
        public static DateTime IncDays(this DateTime dt)
        {
            return dt.AddDays(1);
        }
        public static DateTime DecDays(this DateTime dt)
        {
            return dt.AddDays(-1);
        }
        public static DateTime IncSeconds(this DateTime dt)
        {
            return dt.AddSeconds(1);
        }
        public static DateTime DecSeconds(this DateTime dt)
        {
            return dt.AddSeconds(-1);
        }
        public static DateTime IncMinutes(this DateTime dt)
        {
            return dt.AddMinutes(1);
        }
        public static DateTime DecMinutes(this DateTime dt)
        {
            return dt.AddMinutes(-1);
        }
        public static DateTime TruncateTime(this DateTime dt)
        {
            return dt.Add(-dt.TimeOfDay);
        }
        public static DateTime TruncateMilliSeconds(this DateTime dt)
        {
            return dt.AddMilliseconds(-dt.Millisecond);
        }
        public static DateTime TruncateSeconds(this DateTime dt)
        {
            return dt.AddSeconds(-dt.Second);
        }
        public static bool IsNewDay(this DateTime dt, DateTime dt1)
        {
            return dt.Date.CompareTo(dt1.Date) < 0;
        }
        public static bool IsNextDate(this DateTime dt, DateTime dt1)
        {
            return dt.Date.CompareTo(dt1.Date) < 0;
        }
        public static KeyValuePair<DateTime, DateTime> OneDay(this DateTime dt)
        {
            // dt = dt.TruncateTime();
            return new KeyValuePair<DateTime, DateTime>(dt.Date, dt.Date.AddDays(1));
        }
        public static KeyValuePair<DateTime, DateTime> Week(this DateTime dt)
        {
            var dweek = dt.DayOfWeekRus();
            var dt1 = dt.AddDays(-dweek).Date;
            return new KeyValuePair<DateTime, DateTime>(dt1, dt1.AddDays(7));
        }
        public static KeyValuePair<DateTime, DateTime> WeekToDay(this DateTime dt)
        {
            var dweek = dt.DayOfWeekRus();
            var dt1 = dt.AddDays(-dweek).Date;
            return new KeyValuePair<DateTime, DateTime>(dt1, dt.Date.AddDays(1));
        }
        public static KeyValuePair<DateTime, DateTime> WeekDayToDay(this DateTime dt)
        {
            var dt1 = dt.AddDays(-6).Date;
            return new KeyValuePair<DateTime, DateTime>(dt1, dt.Date.AddDays(1));
        }
        public static KeyValuePair<DateTime, DateTime> WeekPrev(this DateTime dt)
        {
            //DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            //Calendar cal = dfi.Calendar;
            //var weekOfYear = cal.GetWeekOfYear(dt, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

            return dt.AddDays(-7).Date.Week();
        }

        public static KeyValuePair<DateTime, DateTime> Month(this DateTime dt)
        {
            var dt1 = new DateTime(dt.Year, dt.Month, 1).Date;
            return new KeyValuePair<DateTime, DateTime>(dt1, dt1.AddMonths(1).Date);
        }
        public static KeyValuePair<DateTime, DateTime> MonthToDay(this DateTime dt)
        {
            var dt1 = new DateTime(dt.Year, dt.Month, 1).Date;
            return new KeyValuePair<DateTime, DateTime>(dt1, dt.AddDays(1).Date);
        }
        public static KeyValuePair<DateTime, DateTime> MonthDayToDay(this DateTime dt)
        {
            var m = dt.AddMonths(-1);
            var dt1 = new DateTime(m.Year, m.Month , m.Day).Date;

            return new KeyValuePair<DateTime, DateTime>(dt1, dt.AddDays(1).Date);
        }
        public static KeyValuePair<DateTime, DateTime> MonthPrev(this DateTime dt)
        {
            var m = dt.AddMonths(-1);
            var dt1 = new DateTime(m.Year, m.Month, 1).Date;
            var m2 = dt1.AddMonths(1);
            var dt2 = new DateTime(m2.Year, m2.Month, 1).Date;
            return new KeyValuePair<DateTime, DateTime>(dt1, dt2);
        }

        public static int DayOfWeekRus(this DateTime dt)
        {
            var dweek = (int)dt.DayOfWeek;
            if (dweek == 0)
                return 6;
            return dweek - 1;
        }


        // BarsTime
        public static DateTime ToBarDateTime(this DateTime dt, int timeIntSeconds)
        {
            var sec = dt.Hour * 3600 + dt.Minute * 60 + dt.Second;
            var hsec = (sec / timeIntSeconds + 1) * timeIntSeconds;
            return (dt.AddSeconds(hsec - sec)).AddMilliseconds(-dt.Millisecond);
        }
        public static DateTime ToBarDateTime2(this DateTime dt, int timeIntSeconds)
        {
            var sec = dt.Hour * 3600 + dt.Minute * 60 + dt.Second;
            var hsec = (int)(Math.Ceiling((double)sec / timeIntSeconds)) * timeIntSeconds;
            return (dt.AddSeconds(hsec - sec)).AddMilliseconds(-dt.Millisecond);
        }
        public static DatesInterval ToDatesInterval(this DateTime dt1, DateTime dt2)
        {
            return new DatesInterval
            {
                Date1 = dt1.Date,
                Date2 = dt2.AddDays(1).Date
            };
        }
        public static DatesInterval ToDatesInterval(this DateTime dt)
        {
            return new DatesInterval
            {
                Date1 = dt.Date,
                Date2 = dt.AddDays(1).Date
            };
        }

       

    }

    public struct DatesInterval
    {
        public DateTime Date1 { get; set; }
        public DateTime Date2 { get; set; }

        public int Days => 1 + (Date2.Date - Date1.Date).Days;

        public void IncDate1()
        {
            Date1 = Date1.AddDays(1);
        }
    }
    public struct DateTimeInterval
    {
        public DateTime DateTime1 { get; set; }
        public DateTime DateTime2 { get; set; }

        public int Days => DateTime1.Equals(DateTime2)
                            ? 0
                            : (DateTime2 > DateTime1 
                                ? 1 + (DateTime2.Date - DateTime1.Date).Days
                                : -1 + (DateTime2.Date - DateTime1.Date).Days);
    }
}
