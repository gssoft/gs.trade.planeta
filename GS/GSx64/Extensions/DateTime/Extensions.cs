using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Extensions.DateTime
{
    public static class Ext
    {
        #region DateTime ToString
        public static string TimeTickStr(this TimeSpan tms)
        {
            return tms.ToString(@"hh\:mm\:ss\.fff");
        }
        public static string TimeTickStr(this System.DateTime dt)
        {
            return dt.TimeOfDay.ToString(@"hh\:mm\:ss\.fff");
        }
        public static string DateStr(this System.DateTime dt)
        {
            return $"{dt.Date.ToString("yy.MM.dd")}";
        }
        public static string DateTimeTickStr(this System.DateTime dt)
        {
            return $"{dt.DateStr()} {dt.TimeTickStr()}";
        }
        public static string DateTimeTickStrTodayCnd(this System.DateTime dt)
        {
            return System.DateTime.Now.Date == dt.Date
                ? dt.TimeTickStr()
                : dt.DateTimeTickStr();
        }
        public static string TimeStr(this System.DateTime dt)
        {
            return dt.TimeOfDay.ToString(@"hh\:mm\:ss");
        }
        public static string DateTimeStr(this System.DateTime dt)
        {
            return $"{dt.DateStr()} {dt.TimeStr()}";
        }
        public static string DateTimeStrTodayCnd(this System.DateTime dt)
        {
            return System.DateTime.Now.Date == dt.Date
                ? dt.TimeStr()
                : dt.DateTimeStr();
        }

        #endregion
    }
}
