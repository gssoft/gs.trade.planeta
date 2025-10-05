using System;
using GS.Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoubleUnitTest01
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void T1_DoubleEquals()
        {
            double d1 = 5.253;
            double d2 = 5.25301;
            double d3 = 5.2531;
            
            Assert.IsTrue(d1.IsEquals(d2, 0.001), "1");
            Assert.IsTrue(d1.IsEquals(d3, 0.001), "2");
            Assert.IsTrue(d1.IsEquals(d2, 0.0001), "3");
            Assert.IsTrue(d1.IsEquals(d3, 0.0001), "4");
            Assert.IsTrue(d1.IsEquals(d2, 0.00001), "5");
            Assert.IsFalse(d1.IsEquals(d3, 0.00001), "6");

            Console.WriteLine((d2 - d1).ToString("N20"));
            Console.WriteLine((d3 - d1).ToString("N20"));
        }
        [TestMethod]
        public void T11_DoubleNoEquals()
        {
            double d1 = 5.253;
            double d2 = 5.25301;
            double d3 = 5.2531;

            Assert.IsFalse(d1.IsNoEquals(d2, 0.001), "1");
            Assert.IsFalse(d1.IsNoEquals(d3, 0.001), "2");
            Assert.IsFalse(d1.IsNoEquals(d2, 0.0001), "3");
            Assert.IsFalse(d1.IsNoEquals(d3, 0.0001), "4");
            Assert.IsFalse(d1.IsNoEquals(d2, 0.00001), "5");
            Assert.IsTrue(d1.IsNoEquals(d3, 0.00001), "6");

            Console.WriteLine((d2 - d1).ToString("N20"));
            Console.WriteLine((d3 - d1).ToString("N20"));
        }
        [TestMethod]
        public void T2_DoubleEqualsSoft()
        {
            double d1 = 5.253;
            double d2 = 5.25301;
            double d3 = 5.2531;

            Assert.IsTrue(d1.IsEqualsSoft(d2, 0.001), "1");
            Assert.IsTrue(d1.IsEqualsSoft(d3, 0.001), "2");
            Assert.IsTrue(d1.IsEqualsSoft(d2, 0.0001), "3");
            Assert.IsTrue(d1.IsEqualsSoft(d3, 0.0001), "4");
            Assert.IsTrue(d1.IsEqualsSoft(d2, 0.00001), "5");
            Assert.IsFalse(d1.IsEqualsSoft(d3, 0.00001), "6");

            Console.WriteLine((d2 - d1).ToString("N20"));
            Console.WriteLine((d3 - d1).ToString("N20"));
        }
        [TestMethod]
        public void T3_DoubleGreater()
        {
            double d1 = 5.253;
            double d2 = 5.25301;
            double d3 = 5.2531;

            Assert.IsFalse(d2.IsGreaterThan(d1, 0.001), "1");
            Assert.IsFalse(d3.IsGreaterThan(d1, 0.001), "2");
            Assert.IsFalse(d2.IsGreaterThan(d1, 0.0001), "3");
            Assert.IsFalse(d3.IsGreaterThan(d1, 0.0001), "4");
            Assert.IsFalse(d2.IsGreaterThan(d1, 0.00001), "5");
            Assert.IsTrue(d3.IsGreaterThan(d1, 0.00001), "6");

            Console.WriteLine((d1 - d2).ToString("N20"));
            Console.WriteLine((d1 - d3).ToString("N20"));
        }
        [TestMethod]
        public void T31_DoubleGreater()
        {
            double d1 = 5.253;
            double d2 = 5.25301;
            double d3 = 5.2531;

            Assert.IsFalse(d2.IsGreaterThan(d1, 0.0001), "1");
            Assert.IsTrue(d3.IsGreaterThan(d1, 0.00001), $"2 {d2 - d1:N20}");
            Assert.IsFalse(d3.IsLessThan(d1, 0.00001), $"21 {d2 - d1:N20}");
            Assert.IsFalse(d2.IsGreaterThan(d1, 0.0001), "3");
            Assert.IsFalse(d2.IsLessThan(d1, 0.0001), $"31 {d2 - d1:N20}");
            Assert.IsTrue(d2.IsEquals(d1, 0.0001), $"32 {d2 - d1:N20}");
            Assert.IsFalse(d3.IsGreaterThan(d1, 0.0001), "4");
            Assert.IsFalse(d3.IsLessThan(d1, 0.0001), "41");
            Assert.IsTrue(d3.IsEquals(d1, 0.0001), "42");
            Assert.IsFalse(d2.IsGreaterThan(d1, 0.00001), "5");
            Assert.IsTrue(d3.IsGreaterThan(d1, 0.00001), $"6 {d2 - d1:N20}");

            Console.WriteLine((d1 - d2).ToString("N20"));
            Console.WriteLine((d1 - d3).ToString("N20"));
        }
        [TestMethod]
        public void T4_DoubleLess()
        {
            double d1 = 5.253;
            double d2 = 5.25301;
            double d3 = 5.2531;

            Assert.IsFalse(d2.IsLessThan(d1, 0.001), "1");
            Assert.IsFalse(d3.IsLessThan(d1, 0.001), "2");
            Assert.IsFalse(d2.IsLessThan(d1, 0.0001), "3");
            Assert.IsFalse(d3.IsLessThan(d1, 0.0001), "4");
            Assert.IsFalse(d2.IsLessThan(d1, 0.00001), "5");
            Assert.IsFalse(d2.IsLessThan(d1, 0.00001), "6");

            Console.WriteLine((d1 - d2).ToString("N20"));
            Console.WriteLine((d1 - d3).ToString("N20"));
        }
        [TestMethod]
        public void T5_DoubleGreaterOrEqual()
        {
            double d1 = 5.253;
            double d2 = 5.25301;
            double d3 = 5.2531;

            Assert.IsFalse(d2.IsGreaterThan(d1, 0.001), "1");
            Assert.IsFalse(d3.IsGreaterThan(d1, 0.001), "2");
            Assert.IsFalse(d2.IsGreaterThan(d1, 0.0001), "3");
            Assert.IsFalse(d3.IsGreaterThan(d1, 0.0001), "4");
            Assert.IsFalse(d2.IsGreaterThan(d1, 0.00001), "5");
            Assert.IsTrue(d3.IsGreaterThan(d1, 0.00001), "6");

            Console.WriteLine((d1 - d2).ToString("N20"));
            Console.WriteLine((d1 - d3).ToString("N20"));
        }
        [TestMethod]
        public void T6_DoubleLessOrEqual()
        {
            double d1 = 5.253;
            double d2 = 5.25301;
            double d3 = 5.2531;

            Assert.IsFalse(d2.IsLessThan(d1, 0.001), $"1 {d2-d1:N20}");
            Assert.IsFalse(d3.IsLessThan(d1, 0.001), $"1 {d3-d1:N20}");
            Assert.IsFalse(d2.IsLessThan(d1, 0.0001), $"1 {d2-d1:N20}");
            Assert.IsFalse(d3.IsLessThan(d1, 0.0001), $"1 {d3-d1:N20}");
            Assert.IsFalse(d2.IsLessThan(d1, 0.00001), $"1 {d2-d1:N20}");
            Assert.IsFalse(d3.IsLessThan(d1, 0.00001), $"1 {d3-d1:N20}");

            Console.WriteLine((d2 - d1).ToString("N20"));
            Console.WriteLine((d3 - d1).ToString("N20"));
        }
    }
}
