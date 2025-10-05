using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Extension
{
    public static partial class DoubleExtension
    {
        public static double Round(this double d, int digits)
        {
            return digits >= 0 ? Math.Round(d, digits) : d;
        }
        public static double Round(this double d, string roundStr)
        {
            return roundStr.HasValue() ? double.Parse(d.ToString(roundStr)) : d;
        }
        public static double StdDev(this IEnumerable<double> values)
        {
            double ret = 0;
            var enumerable = values as double[] ?? values.ToArray();
            int count = enumerable.Count();
            if (count > 1)
            {
                //Compute the Average
                double avg = enumerable.Average();

                //Perform the Sum of (value-avg)^2
                double sum = enumerable.Sum(d => (d - avg) * (d - avg));

                //Put it all together
                ret = Math.Sqrt(sum / count);
            }
            return ret;
        }

        public static double DoubleInvariantParse(this string v)
        {
            return double.Parse(v.Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);
        }
    }
}
