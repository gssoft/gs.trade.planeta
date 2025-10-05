using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.GSMath
{
    public static class Extensions
    {
        public static double StandardDeviation(this IEnumerable<double> values)
        {
            if (values.Count() <= 1) return 0d;
            double avg = values.Average();
            return System.Math.Sqrt(values.Average(v => System.Math.Pow(v - avg, 2)));
        }
    }
}
