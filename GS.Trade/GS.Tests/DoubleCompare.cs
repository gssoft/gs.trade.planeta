using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace GS.Tests
{
    [TestFixture]
    public class DoubleCompare
    {
        private Random _rand;
        private int _precision;
        [OneTimeSetUp]
        public void Init()
        {
            var r = new Random();
            _rand = new Random(r.Next());

            _precision = 15;
        }

        [Test]
        public void DoubleIsEqualTo()
        {
            // const int precision = 15;
            double tolerance = 1.0 / Math.Pow(10, _precision);

            foreach (var i in Enumerable.Range(1, 500000))
            {
                var d = _rand.NextDouble();
                var sd = d.ToString();
                var dap = double.Parse(sd);

                // Assert.That(d, Is.EqualTo(dap).Within(tolerance));

                Assert.IsTrue(d.IsEquals(dap, _precision),"Iteration: {0}", i);
                //Assert.IsFalse(d.IsNoEquals(dap, _precision), "Iteration: {0}", i);
            }
        }
        [Test]
        public void DoubleIsEqualTo1()
        {
            // const int precision = 15;
            double tolerance = 1.0 / Math.Pow(10, _precision);

            foreach (var i in Enumerable.Range(1, 500000))
            {
                var d = _rand.NextDouble();
                var sd = d.ToString();
                var dap = double.Parse(sd);

                // Assert.That(d, Is.EqualTo(dap).Within(tolerance));

                Assert.IsTrue(d.IsEquals1(dap, _precision), "Iteration: {0}", i);
                //Assert.IsFalse(d.IsNoEquals(dap, _precision), "Iteration: {0}", i);
            }
        }
        [Test]
        public void DoubleIsNoEqualTo()
        {
            // const int precision = 15;
            double tolerance = 1.0 / Math.Pow(10, _precision);

            foreach (var i in Enumerable.Range(1, 500000))
            {
                var d = _rand.NextDouble();
                var sd = d.ToString();
                var dap = double.Parse(sd);

                Assert.IsFalse(d.IsNoEquals(dap, _precision), "Iteration: {0}", i);
                //Assert.IsTrue(d.IsEquals(dap, _precision), "Iteration: {0}", i);
            }
        }

        [Test]
        public void DoubleIsEqualInteger()
        {
            var precision = 2;
            var d1 = 10.0d;
            var d2 = 9.9d;

            //d1 = d1 - 0.01;

            var b = d1.CompareTo(d2);

            // Math.Abs(dbl - value) < (precision > 0 ? Math.Pow(10, -precision) : 1.0d);
            var diff = Math.Abs(d1 - d2);
            var pow = Math.Pow(10, -precision);
            var bo = diff - pow;

            Assert.IsFalse(d1.IsEquals(d2, precision));
        }
        [Test]
        public void DoubleIsGreaterOrLess()
        {
            var precision = 3;
            var d1 = 10.00d;
            var d2 = 9.99d;

            Assert.IsFalse(d1.IsEquals(d2, precision));
            Assert.IsTrue(d1.IsNoEquals(d2, precision));

            Assert.IsFalse(d2.IsEquals(d1, precision));
            Assert.IsTrue(d2.IsNoEquals(d1, precision));

            Assert.IsTrue(d1.IsGreaterThan(d2, precision),
                                "d1: {0} > d2: {1}", d1.ToString("N5"), d2.ToString("N5"));
            Assert.IsTrue(d1.IsGreaterOrEqualsThan(d2, precision),
                                "d1: {0} >= d2: {1}", d1.ToString("N5"), d2.ToString("N5"));

            Assert.IsFalse(d1.IsLessThan(d2, precision),
                                "d1: {0} < d2: {1}", d1.ToString("N5"), d2.ToString("N5"));
            Assert.IsFalse(d1.IsLessOrEqualsThan(d2, precision),
                                "d1: {0} <= d2: {1}", d1.ToString("N5"), d2.ToString("N5"));
            // ---------

            Assert.IsFalse(d2.IsGreaterThan(d1, precision),
                                "d2: {0} > d1: {1}", d2.ToString("N5"), d1.ToString("N5"));
            Assert.IsFalse(d2.IsGreaterOrEqualsThan(d1, precision),
                                "d2: {0} >= d1: {1}", d2.ToString("N5"), d1.ToString("N5"));

            Assert.IsTrue(d2.IsLessThan(d1, precision),
                                "d2: {0} < d1: {1}", d2.ToString("N5"), d1.ToString("N5"));
            Assert.IsTrue(d2.IsLessOrEqualsThan(d1, precision),
                                "d2: {0} < d1: {1}", d2.ToString("N5"), d1.ToString("N5"));
            

            d1 = 9.99;
            d2 = 9.99;

            Assert.IsTrue(d1.IsEquals(d2, precision));
            Assert.IsFalse(d1.IsNoEquals(d2, precision));

            Assert.IsTrue(d2.IsEquals(d1, precision));
            Assert.IsFalse(d2.IsNoEquals(d1, precision));


            Assert.IsFalse(d1.IsGreaterThan(d2, precision),
                                "d1: {0} > d2: {1}", d1.ToString("N5"), d2.ToString("N5"));

            Assert.IsTrue(d1.IsGreaterOrEqualsThan(d2, precision),
                               "d1: {0} >= d2: {1}", d1.ToString("N5"), d2.ToString("N5"));

            Assert.IsFalse(d1.IsLessThan(d2, precision),
                                "d1: {0} < d2: {1}", d1.ToString("N5"), d2.ToString("N5"));

            Assert.IsTrue(d1.IsLessOrEqualsThan(d2, precision),
                               "d1: {0} <= d2: {1}", d1.ToString("N5"), d2.ToString("N5"));
            // ---

            Assert.IsFalse(d2.IsGreaterThan(d1, precision),
                                "d2: {0} > d1: {1}", d2.ToString("N5"), d1.ToString("N5"));

            Assert.IsTrue(d2.IsGreaterOrEqualsThan(d1, precision),
                               "d2: {0} >= d1: {1}", d2.ToString("N5"), d1.ToString("N5"));
   
            Assert.IsFalse(d2.IsLessThan(d1, precision),
                                    "d2: {0} < d1: {1}", d2.ToString("N5"), d1.ToString("N5"));

            Assert.IsTrue(d2.IsLessOrEqualsThan(d1, precision),
                                   "d2: {0} <= d1: {1}", d2.ToString("N5"), d1.ToString("N5"));
          
        }

    }
}
