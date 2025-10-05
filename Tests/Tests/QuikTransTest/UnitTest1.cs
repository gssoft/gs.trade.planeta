using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//Test Name:	TestUlongMaxMumber
//Test Outcome:	Passed
//Result StandardOutput:	18446744073709551615


namespace QuikTransTest
{
    [TestClass]
    public class UnitTest1
    {
        //  1999999999999999999
        // 1892945520759240399
        // overflow 18 446 744 074 446 700 000
        public const ulong UlongMax =  (ulong)18446744073709551615;

        [TestMethod]
        public void TestUlongMaxMumber()
        {
            ulong n = ulong.MaxValue;
            Console.WriteLine($"{n.ToString("###################")}");
            Assert.AreEqual(UlongMax,n);
        }

        [TestMethod]
        public void TestQuikTradeNumber()
        {
            var quikOrderNumber = 1892945520759240399d;
            var ulongOrder = Convert.ToUInt64(quikOrderNumber);
            Assert.AreNotEqual((ulong)1892945520759240399, ulongOrder);
            Console.WriteLine($"{quikOrderNumber.ToString("###################")}{Environment.NewLine}" +
                              $"{ulongOrder.ToString("###################")}");
        }

        [TestMethod]
        public void TestQuikOrderNumber()
        {
            // Work properly
            var quikOrderNumber = 8999999999999999d;
            var ulongOrder = Convert.ToUInt64(quikOrderNumber);
            Assert.AreEqual((ulong) 8999999999999999, ulongOrder);

            // Does not work
            quikOrderNumber = 9999999999999999d;
            ulongOrder = Convert.ToUInt64(quikOrderNumber);
            Assert.AreNotEqual((ulong) 9999999999999999, ulongOrder);
        }

        [TestMethod]
        public void TestMaxDoubleNumber()
        {
            // Work properly
            var quikOrderNumber = 8999999999999999d;
            try
            {
                while (true)
                {
                    quikOrderNumber += 1000000000;
                    var ulongOrder = Convert.ToUInt64(quikOrderNumber);
                    Assert.AreEqual((ulong)quikOrderNumber, ulongOrder);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message} dmax:{quikOrderNumber.ToString("C")}");
            }
        }

        [TestMethod]
        public void TestMaxDoubleValue()
        {
            var dmax = double.MaxValue;
            try
            {
                var dmaxlong = Convert.ToUInt64(dmax);
                Console.WriteLine($"{dmaxlong.ToString("###################")}");
            }
            catch (OverflowException e)
            {
                Console.WriteLine($"{e.Message} dmax:{dmax.ToString("C")}");
            }
        }
        [TestMethod]
        public void TestMaxDoubleValue2()
        {
            var dmax = double.MaxValue;
            try
            {
                var hash = dmax.GetHashCode();
                // var dmaxlong = Convert.ToUInt64(dmax);
                Console.WriteLine($"{hash.ToString("###################")}");
            }
            catch (OverflowException e)
            {
                Console.WriteLine($"{e.Message} dmax:{dmax.ToString("C")}");
            }
        }
    }
}
