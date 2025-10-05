using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;
using NUnit.Framework;

namespace GS.Tests
{
    [TestFixture]
    public class DateTimeExtensionTest
    {
        [Test]
        public void DateTimeToLongAndBackToDateTime()
        {
            Assert.DoesNotThrow(() =>
            {
                var dt = DateTime.Now;
                var ms = dt.Millisecond;
                // var ts = dt.Ticks;
                var l = dt.ToLongInSec();
                var dt1 = l.ToDateTime();
                dt1 = dt1.AddMilliseconds(ms);

                Console.WriteLine("Dt0: {0}\r\nDt1: {1}", dt.ToString("O"), dt1.ToString("O"));

                // dt1 = dt1.AddTicks(ts);
                Assert.That(dt, Is.EqualTo(dt1).Within(1).Milliseconds);
            });

            Assert.Pass("Passed with Pleasure ...");
        }
        [Test]
        public void DateTimeToLongAndBackToDateTimeManyTimesInLoop()
        {
            Assert.DoesNotThrow(() => { 
            foreach(var y in Enumerable.Range(1998,5))
                foreach (var M in Enumerable.Range(1, 12))
                    foreach (var d in Enumerable.Range(1, DateTime.DaysInMonth(y,M)))
                        foreach (var h in Enumerable.Range(0, 23))
                            foreach (var m in Enumerable.Range(0, 59))
                                foreach (var s in Enumerable.Range(0, 59))
                                    VerifyDateTime(new DateTime(y,M,d,h,m,s));
            });
            Assert.Pass("Passed with Pleasure ...");
        }

        private void VerifyDateTime(DateTime dt)
        {
            Assert.DoesNotThrow(() =>
            {
                var ms = dt.Millisecond;
                var l = dt.ToLongInSec();
                var dt1 = l.ToDateTime();
                dt1 = dt1.AddMilliseconds(ms);

                // Console.WriteLine("Dt0: {0}\r\nDt1: {1}", dt.ToString("O"), dt1.ToString("O"));

                Assert.That(dt, Is.EqualTo(dt1).Within(1).Milliseconds);
            });
        }

    }
}
