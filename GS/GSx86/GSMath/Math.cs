using System.Collections.Generic;
using System.Linq;

namespace GS.GSMath
{
    public static class Math
    {
        /// <summary>
        /// Find the Greatest Common Divisor
        /// </summary>
        /// <param name="a">Number a</param>
        /// <param name="b">Number b</param>
        /// <returns>The greatest common Divisor</returns>
        public static long GCD(long a, long b)
        {
            while (b != 0)
            {
                long tmp = b;
                b = a % b;
                a = tmp;
            }

            return a;
        }

        /// <summary>
        /// Find the Least Common Multiple
        /// </summary>
        /// <param name="a">Number a</param>
        /// <param name="b">Number b</param>
        /// <returns>The least common multiple</returns>
        public static long LCM(long a, long b)
        {
            return (a * b) / GCD(a, b);
        }
        
        //Usage
        //List<ValveData> list = ...
        //var result = list.Select( v => (double)v.SomeField ).CalculateStdDev();
        public static double StandardDeviation(IEnumerable<double> values)
        {
            double standardDeviation = 0;
            if (values.Count() <= 1 ) return standardDeviation;

            // Compute the average.     
            var avg = values.Average();
            // Perform the Sum of (value-avg)_2_2.      
            var sum = values.Sum(d => System.Math.Pow(d - avg, 2));
            // Put it all together.      
            standardDeviation = System.Math.Sqrt((sum) / (values.Count() - 1));
            return standardDeviation;
        }
        public static double StandardDeviation(IEnumerable<decimal> values)
        {
            double standardDeviation = 0;
            if (values.Count() <= 1) return standardDeviation;

            // Compute the average.     
            var avg = values.Average();
            // Perform the Sum of (value-avg)_2_2.      
            var sum = values.Sum(d => System.Math.Pow((double)(d - avg), 2));
            // Put it all together.      
            standardDeviation = System.Math.Sqrt((sum) / (values.Count() - 1));
            return standardDeviation;
        }
    }
}
