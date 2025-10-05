using System;
using System.CodeDom;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoubleUnitTest01
{
    [TestClass]
    public class BitConverterUnitTest
    {
        private Int32 _i32 = 12345678;
        int[] values = { 0, 15, -15, 0x100000,  -0x100000, 1000000000,
            -1000000000, int.MinValue, int.MaxValue };

        [TestMethod]
        public void Test01_Int32ToBytes()
        {
            Console.WriteLine("Last Byte (4) First in Output");
            Console.WriteLine("====================================");
            byte[] byteArray = BitConverter.GetBytes(_i32);
            var i32 = BitConverter.ToInt32(byteArray, 0);
            Assert.AreEqual(_i32, i32);
            foreach (var value in values)
            {
                for (int i = 0; i < byteArray.Length; i++) byteArray[i] = 0;

                Console.WriteLine(value);
                byteArray = BitConverter.GetBytes(value);
                string str = "";
                for(int i = byteArray.Length-1; i >=0; i--) str += byteArray[i] + " ";
                Console.WriteLine(str);
                i32 = BitConverter.ToInt32(byteArray, 0);
                // Console.WriteLine(i32);
                Assert.AreEqual(value, i32, $"No Equals: {value} {i32}");
                Console.WriteLine("================================");
            }
        }
    }
}
