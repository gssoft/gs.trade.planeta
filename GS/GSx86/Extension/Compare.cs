using System;

namespace GS.Extension
{
    public static partial class StringExt
    {
        // decimals
        #region decimals
        public static bool IsGreaterOrEqualsThan(this decimal flt, decimal value)
        {
            return flt.CompareTo(value) >= 0;
        }
        public static bool IsGreaterThan(this decimal flt, decimal value)
        {
            return flt.CompareTo(value) > 0;
        }
        public static bool IsLessOrEqualsThan(this decimal flt, decimal value)
        {
            return flt.CompareTo(value) <= 0;
        }
        public static bool IsLessThan(this decimal flt, decimal value)
        {
            return flt.CompareTo(value) < 0;
        }
        public static bool IsEquals(this decimal flt, decimal value)
        {
            return flt.CompareTo(value) == 0;
        }
        #endregion

        // float
        #region float
        public static bool IsGreaterOrEqualsThan(this float flt, float value)
        {
            return flt.CompareTo(value) >= 0;
        }
        public static bool IsGreaterThan(this float flt, float value)
        {
            return flt.CompareTo(value) > 0;
        }
        public static bool IsLessOrEqualsThan(this float flt, float value)
        {
            return flt.CompareTo(value) <= 0;
        }
        public static bool IsLessThan(this float flt, float value)
        {
            return flt.CompareTo(value) < 0;
        }
        public static bool IsEquals(this float flt, float value)
        {
            return flt.CompareTo(value) == 0;
        }
        #endregion

        // float?
        #region float?
        public static bool IsGreaterOrEqualsThan(this float? flt, float value)
        {
            if (!flt.HasValue) return false;
            return flt.Value.CompareTo(value) >= 0;
        }
        public static bool IsGreaterThan(this float? flt, float value)
        {
            if (!flt.HasValue) return false;
            return flt.Value.CompareTo(value) > 0;
        }
        public static bool IsLessOrEqualsThan(this float? flt, float value)
        {
            if (!flt.HasValue) return false;
            return flt.Value.CompareTo(value) <= 0;
        }
        public static bool IsLessThan(this float? flt, float value)
        {
            if (!flt.HasValue) return false;
            return flt.Value.CompareTo(value) < 0;
        }
        public static bool IsEquals(this float? flt, float value)
        {
            if (!flt.HasValue) return false;
            return flt.Value.CompareTo(value) == 0;
        }
        #endregion

        // double
        #region double
        public static bool IsGreaterOrEqualsThan(this double dbl, double value)
        {
            return dbl.CompareTo(value) >= 0;
        }
        public static bool IsGreaterThan(this double dbl, double value)
        {
            return dbl.CompareTo(value) > 0;
        }
        public static bool IsLessOrEqualsThan(this double dbl, double value)
        {
            return dbl.CompareTo(value) <= 0;
        }
        public static bool IsLessThan(this double dbl, double value)
        {
            return dbl.CompareTo(value) < 0;
        }
        public static bool IsEquals(this double dbl, double value)
        {
            return dbl.CompareTo(value) == 0;
        }
        public static bool IsNoEquals(this double dbl, double value)
        {
            return dbl.CompareTo(value) != 0;
        }
        #endregion
        // ******************************

        // double with digits
        #region double with digits
        public static bool IsGreaterOrEqualsThan(this double dbl, double value, int precision)
        {
            // return dbl.CompareTo(value) >= 0;
            // return dbl - value >= (precision > 0 ? Math.Pow(10, -precision) : 1.0d);
            return dbl.IsGreaterThan(value, precision) || dbl.IsEquals(value, precision);
        }
        public static bool IsGreaterThan(this double dbl, double value, int precision)
        {
            // return dbl.CompareTo(value) > 0;
            return dbl - value > (precision > 0 ? Math.Pow(10, -precision) : 1.0d);
        }
        public static bool IsLessOrEqualsThan(this double dbl, double value, int precision)
        {
            // return dbl.CompareTo(value) <= 0;
            // return value - dbl >= (precision > 0 ? Math.Pow(10, -precision) : 1.0d);
            return dbl.IsLessThan(value, precision) || dbl.IsEquals(value, precision);
        }
        public static bool IsLessThan(this double dbl, double value, int precision)
        {
            return value - dbl > (precision > 0 ? Math.Pow(10, -precision) : 1.0d);
        }
        public static bool IsEquals(this double dbl, double value, int precision)
        {
            // return Math.Abs(dbl - value) <= Math.Pow(10, precision >= 0 ? -precision : 0);
            return Math.Abs(dbl - value) < (precision > 0 ? Math.Pow(10, -precision) : 1.0d);
        }
        public static bool IsEquals1(this double dbl, double value, int precision)
        {
            // double tolerance = 1.0 / Math.Pow(10, precision >= 0 ? precision : 0);
            return dbl.CompareTo(value) == 0 ||
                       Math.Abs(dbl - value) < (precision > 0 ? Math.Pow(10, -precision) : 1.0d);
        }
        public static bool IsEquals12(this double dbl, double value, int precision)
        {
            double tolerance = 1.0 / Math.Pow(10, precision >= 0 ? precision : 0);
            return Math.Abs(dbl - value) <= tolerance;
        }
        public static bool IsNoEquals(this double dbl, double value, int precision)
        {
            //return Math.Abs(dbl - value) > Math.Pow(10, precision >= 0 ? -precision : 0);
            return Math.Abs(dbl - value) >= (precision > 0 ? Math.Pow(10, -precision) : 1.0d);
        }
        public static bool IsNoEquals1(this double dbl, double value, int precision)
        {
            return Math.Abs(dbl - value) > 1.0 / Math.Pow(10, precision >= 0 ? precision : 0);
        }
        public static bool IsNoEquals2(this double dbl, double value, int precision)
        {
            double tolerance = 1.0 / Math.Pow(10, precision >= 0 ? precision : 0);
            return Math.Abs(dbl - value) > tolerance;
        }
        #endregion

        // double with tolerance
        #region double with digits
        public static bool IsGreaterOrEqualsThan(this double dbl, double value, double tolerance)
        {
            return dbl.IsGreaterThan(value, tolerance) || dbl.IsEquals(value, tolerance);
        }
        public static bool IsGreaterOrEqualsThanSoft(this double dbl, double value, double tolerance)
        {
            return dbl.IsGreaterThan(value, tolerance) || dbl.IsEqualsSoft(value, tolerance);
        }
        public static bool IsGreaterThan(this double dbl, double value, double tolerance)
        {
            return dbl - value > tolerance;
        }
        public static bool IsLessOrEqualsThan(this double dbl, double value, double tolerance)
        {
          return dbl.IsLessThan(value, tolerance) || dbl.IsEquals(value, tolerance);
        }
        public static bool IsLessOrEqualsThanSoft(this double dbl, double value, double tolerance)
        {
            return dbl.IsLessThan(value, tolerance) || dbl.IsEqualsSoft(value, tolerance);
        }
        public static bool IsLessThan(this double dbl, double value, double tolerance)
        {
            return value - dbl > tolerance;
        }
        public static bool IsEquals(this double dbl, double value, double tolerance)
        {
            return Math.Abs(dbl - value) < tolerance;
        }
        public static bool IsEqualsSoft(this double dbl, double value, double tolerance)
        {
            return Math.Abs(dbl - value) <= tolerance;
        }
        public static bool IsNoEquals(this double dbl, double value, double tolerance)
        {
            return Math.Abs(dbl - value) > tolerance;
        }
        public static bool IsNoEqualsSoft(this double dbl, double value, double tolerance)
        {
            return Math.Abs(dbl - value) >= tolerance;
        }
        #endregion
        // ******************************

        // DateTime

        public static bool IsGreaterOrEqualsThan(this DateTime dt, DateTime value)
        {
            return dt.CompareTo(value) >= 0;
        }
        public static bool IsGreaterThan(this DateTime dt, DateTime value)
        {
            return dt.CompareTo(value) > 0;
        }
        public static bool IsLessOrEqualsThan(this DateTime dt, DateTime value)
        {
            return dt.CompareTo(value) <= 0;
        }
        public static bool IsLessThan(this DateTime dt, DateTime value)
        {
            return dt.CompareTo(value) < 0;
        }
        public static bool IsEquals(this DateTime dt, DateTime value)
        {
            return dt.CompareTo(value) == 0;
        }
    }
}
