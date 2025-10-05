using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GS.Buffers;

namespace CurcularBufferTest02
{
    [TestClass]
    public class UnitTest1
    {
        private CircularBuffer<int> _circularBuffer = new CircularBuffer<int>(10);
        private List<int> _testList;

        [TestInitialize]
        public void Init()
        {
            foreach (var i in Enumerable.Range(0, 10))
            {
                _circularBuffer.SafeEnqueue(i);
            }
        }

        [TestMethod]
        public void T1_CountIsEq10()
        {
            Assert.AreEqual(10, _circularBuffer.Count, "Count != 10");
            Assert.AreEqual(10, _circularBuffer.Size, "Size != 10");
            Assert.AreEqual(0, _circularBuffer.SafePeekHead(), "PeekHead != 0");
            Assert.AreEqual(0, _circularBuffer.SafePeekRear(), "PeekRear != 0");
            Assert.AreEqual(0, _circularBuffer.Head, "Head != 0");
            Assert.AreEqual(0, _circularBuffer.Rear, "Rear != 0");

            foreach (var i in _circularBuffer)
                Console.WriteLine(
                    $"Item:{i} Head:{_circularBuffer.Head} Rear:{_circularBuffer.Rear} PeakHead:{_circularBuffer.PeekHead()} PeakRear:{_circularBuffer.PeekRear()} ");

        }

        [TestMethod]
        public void T2_NewItemLast()
        {
            _circularBuffer.SafeEnqueue(10);
            _testList = new List<int>(10) {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            int j = 0;
            Assert.AreEqual(10, _circularBuffer.Count, "Count != 10");
            Assert.AreEqual(1, _circularBuffer.SafePeekHead(), "PeekHead != 1");
            Assert.AreEqual(1, _circularBuffer.SafePeekRear(), "PeekRear != 1");
            Assert.AreEqual(1, _circularBuffer.Head, "Head != 1");
            Assert.AreEqual(1, _circularBuffer.Rear, "Rear != 1");
            foreach (var i in _circularBuffer)
            {
                Console.WriteLine(
                    $"Item:{i} Head:{_circularBuffer.Head} Rear:{_circularBuffer.Rear} PeakHead:{_circularBuffer.PeekHead()} PeakRear:{_circularBuffer.PeekRear()} ");
                Assert.AreEqual(_testList[j], i, $"Item no equal: {_testList[j]} != {i}");
                j++;
            }
        }

        [TestMethod]
        public void T3_SomeItemInsert()
        {
            _circularBuffer.SafeEnqueue(10);
            _circularBuffer.SafeEnqueue(11);
            _circularBuffer.SafeEnqueue(12);
            _circularBuffer.SafeEnqueue(13);
            _circularBuffer.SafeEnqueue(14);
            _circularBuffer.SafeEnqueue(15);
            _testList = new List<int>(10) {6, 7, 8, 9, 10, 11, 12, 13, 14, 15};
            int j = 0;
            Assert.AreEqual(10, _circularBuffer.Count, "Count != 10");
            Assert.AreEqual(6, _circularBuffer.SafePeekHead(), "PeakHead != 6");
            Assert.AreEqual(6, _circularBuffer.SafePeekRear(), "PeekRear != 6");
            Assert.AreEqual(6, _circularBuffer.Head, "Head != 6");
            Assert.AreEqual(6, _circularBuffer.Rear, "Rear != 6");
            foreach (var i in _circularBuffer)
            {
                Console.WriteLine(
                    $"Item:{i} Head:{_circularBuffer.Head} Rear:{_circularBuffer.Rear} PeakHead:{_circularBuffer.PeekHead()} PeakRear:{_circularBuffer.PeekRear()} ");
                Assert.AreEqual(_testList[j], i, $"Item no equal: {_testList[j]} != {i}");
                j++;
            }
        }

        [TestMethod]
        public void T4_ItemInsertAll()
        {
            _circularBuffer.SafeEnqueue(11);
            _circularBuffer.SafeEnqueue(12);
            _circularBuffer.SafeEnqueue(13);
            _circularBuffer.SafeEnqueue(14);
            _circularBuffer.SafeEnqueue(15);
            _circularBuffer.SafeEnqueue(16);
            _circularBuffer.SafeEnqueue(17);
            _circularBuffer.SafeEnqueue(18);
            _circularBuffer.SafeEnqueue(19);
            _circularBuffer.SafeEnqueue(20);

            _testList = new List<int>(10) {11, 12, 13, 14, 15, 16, 17, 18, 19, 20};
            int j = 0;
            Assert.AreEqual(10, _circularBuffer.Count, "Count != 10");
            Assert.AreEqual(11, _circularBuffer.SafePeekHead(), "PeakHead != 11");
            Assert.AreEqual(11, _circularBuffer.SafePeekRear(), "PeekRear != 11");
            Assert.AreEqual(0, _circularBuffer.Head, "Head != 0");
            Assert.AreEqual(0, _circularBuffer.Rear, "Rear != 0");
            foreach (var i in _circularBuffer)
            {
                Console.WriteLine(
                    $"Item:{i} Head:{_circularBuffer.Head} Rear:{_circularBuffer.Rear} PeakHead:{_circularBuffer.PeekHead()} PeakRear:{_circularBuffer.PeekRear()} ");
                Assert.AreEqual(_testList[j], i, $"Item no equal: {_testList[j]} != {i}");
                j++;
            }
        }

        [TestMethod]
        public void T5_Deque()
        {
            Assert.AreEqual(10, _circularBuffer.Count, "Count != 10");
            Assert.AreEqual(10, _circularBuffer.Size, "Size != 10");
            Assert.AreEqual(0, _circularBuffer.SafePeekHead(), "PeakHead != 0");
            Assert.AreEqual(0, _circularBuffer.SafePeekRear(), "PeekRear != 0");
            Assert.AreEqual(0, _circularBuffer.Head, "Head != 0");
            Assert.AreEqual(0, _circularBuffer.Rear, "Rear != 0");
            _circularBuffer.SafeDequeue();
            Assert.AreEqual(9, _circularBuffer.Count, "Count != 9");
            Assert.AreEqual(10, _circularBuffer.Size, "Size != 10");
            Assert.AreEqual(1, _circularBuffer.SafePeekHead(), "Peak != 1");
            Assert.AreEqual(0, _circularBuffer.SafePeekRear(), "PeekRear != 0");
            Assert.AreEqual(1, _circularBuffer.Head, "Head != 1");
            Assert.AreEqual(0, _circularBuffer.Rear, "Rear != 0");
            _circularBuffer.SafeDequeue();
            Assert.AreEqual(8, _circularBuffer.Count, "Count != 8");
            Assert.AreEqual(10, _circularBuffer.Size, "Size != 10");
            Assert.AreEqual(2, _circularBuffer.SafePeekHead(), "Peak !=2");
            Assert.AreEqual(0, _circularBuffer.SafePeekRear(), "PeekRear != 0");
            Assert.AreEqual(2, _circularBuffer.Head, "Head != 2");
            Assert.AreEqual(0, _circularBuffer.Rear, "Rear != 0");
            _testList = new List<int>(10) {2, 3, 4, 5, 6, 7, 8, 9};
            int j = 0;
            foreach (var i in _circularBuffer)
            {
                Console.WriteLine(
                    $"Item:{i} Head:{_circularBuffer.Head} Rear:{_circularBuffer.Rear} PeakHead:{_circularBuffer.PeekHead()} PeakRear:{_circularBuffer.PeekRear()} ");
                Assert.AreEqual(_testList[j], i, $"Item no equal: {_testList[j]} != {i}");
                j++;
            }
        }

        [TestMethod]
        public void T6_EnqueDeque()
        {
            Assert.AreEqual(10, _circularBuffer.Count, "Count != 10");
            Assert.AreEqual(10, _circularBuffer.Size, "Size != 10");
            Assert.AreEqual(0, _circularBuffer.SafePeekHead(), "Count != 0");
            Assert.AreEqual(0, _circularBuffer.SafePeekRear(), "PeekRear != 0");
            Assert.AreEqual(0, _circularBuffer.Head, "Head != 0");
            Assert.AreEqual(0, _circularBuffer.Rear, "Rear != 0");
            _circularBuffer.SafeDequeue();
            Assert.AreEqual(9, _circularBuffer.Count, "Count != 9");
            Assert.AreEqual(10, _circularBuffer.Size, "Size != 10");
            Assert.AreEqual(1, _circularBuffer.SafePeekHead(), "PeekHead != 1");
            Assert.AreEqual(0, _circularBuffer.SafePeekRear(), "PeekRear != 0");
            Assert.AreEqual(1, _circularBuffer.Head, "Head != 1");
            Assert.AreEqual(0, _circularBuffer.Rear, "Rear != 0");
            _circularBuffer.SafeDequeue();
            Assert.AreEqual(8, _circularBuffer.Count, "Count != 8");
            Assert.AreEqual(10, _circularBuffer.Size, "Size != 10");
            Assert.AreEqual(2, _circularBuffer.SafePeekHead(), "PeekHead !=2");
            Assert.AreEqual(0, _circularBuffer.SafePeekRear(), "PeekRear != 0");
            Assert.AreEqual(2, _circularBuffer.Head, "Head != 2");
            Assert.AreEqual(0, _circularBuffer.Rear, "Rear != 0");
            _circularBuffer.SafeEnqueue(10);
            Assert.AreEqual(9, _circularBuffer.Count, "Count != 9");
            Assert.AreEqual(10, _circularBuffer.Size, "Size != 10");
            Assert.AreEqual(2, _circularBuffer.SafePeekHead(), "PeekHead !=2");
            Assert.AreEqual(2, _circularBuffer.SafePeekHead(), "PeekRear !=2");
            Assert.AreEqual(2, _circularBuffer.Head, "Head != 2");
            Assert.AreEqual(1, _circularBuffer.Rear, "Rear != 1");
            _circularBuffer.SafeEnqueue(11);
            Assert.AreEqual(10, _circularBuffer.Count, "Count != 10");
            Assert.AreEqual(10, _circularBuffer.Size, "Size != 10");
            Assert.AreEqual(2, _circularBuffer.SafePeekHead(), "PeekHead !=2");
            Assert.AreEqual(2, _circularBuffer.SafePeekHead(), "PeekRear !=2");
            Assert.AreEqual(2, _circularBuffer.Head, "Head != 2");
            Assert.AreEqual(2, _circularBuffer.Rear, "Rear != 2");

            _testList = new List<int>(10) {2, 3, 4, 5, 6, 7, 8, 9, 10, 11};
            int j = 0;
            foreach (var i in _circularBuffer)
            {
                Console.WriteLine(
                    $"Item:{i} Head:{_circularBuffer.Head} Rear:{_circularBuffer.Rear} PeakHead:{_circularBuffer.PeekHead()} PeakRear:{_circularBuffer.PeekRear()} ");
                Assert.AreEqual(_testList[j], i, $"Item no equal: {_testList[j]} != {i}");
                j++;
            }
        }

        [TestMethod]
        public void T7_Test_ToArray()
        {
            Assert.AreEqual(10, _circularBuffer.Count, "Count != 10");
            Assert.AreEqual(10, _circularBuffer.Size, "Size != 10");
            Assert.AreEqual(0, _circularBuffer.SafePeekHead(), "PeekHead != 0");
            Assert.AreEqual(0, _circularBuffer.SafePeekRear(), "PeekRear != 0");
            Assert.AreEqual(0, _circularBuffer.Head, "Head != 0");
            Assert.AreEqual(0, _circularBuffer.Rear, "Rear != 0");

            foreach (var i in _circularBuffer)
                Console.WriteLine(
                    $"Item:{i} Head:{_circularBuffer.Head} Rear:{_circularBuffer.Rear} PeakHead:{_circularBuffer.PeekHead()} PeakRear:{_circularBuffer.PeekRear()} ");

            var array = _circularBuffer.SafeToArray();
            for (var i = 0; i < array.Length; ++i)
                Console.WriteLine($"Item:{array[i]}");
            Console.WriteLine("================");

            var reverseArray = _circularBuffer.SafeToReverseArray();
            int j = array.Length - 1;
            for (var i = 0; i < reverseArray.Length; ++i, --j)
            {
                Console.WriteLine($"Item:{reverseArray[i]}");
                Assert.AreEqual(array[j], reverseArray[i], "Reverse Error");
            }
        }

        [TestMethod]
        public void T8_Test_ToArray()
        {
            Assert.AreEqual(10, _circularBuffer.Count, "Count != 10");
            while (_circularBuffer.Count > 0) _circularBuffer.Dequeue();
            Assert.AreEqual(0, _circularBuffer.Count, "Count != 0");

            _circularBuffer = new CircularBuffer<int>(3);
            _circularBuffer.Enqueue(1);
            Console.WriteLine("SimpleArr");
            var arr = _circularBuffer.SafeToArray1();
            foreach (var i in arr)
            {
                Console.WriteLine($"{i}");
            }
            //Console.WriteLine("ReverseArr");
            //var arrRev = _circularBuffer.SafeToReverseArray();
            //foreach (var i in arrRev)
            //{
            //    Console.WriteLine($"{i}");
            //}
            _circularBuffer.Enqueue(2);
            Console.WriteLine("SimpleArr");
            arr = _circularBuffer.SafeToArray1();
            foreach (var i in arr)
            {
                Console.WriteLine($"{i}");
            }
            //Console.WriteLine("ReverseArr");
            //arrRev = _circularBuffer.SafeToReverseArray();
            //foreach (var i in arrRev)
            //{
            //    Console.WriteLine($"{i}");
            //}
            _circularBuffer.Enqueue(3);
            Console.WriteLine("SimpleArr");
            arr = _circularBuffer.SafeToArray1();
            foreach (var i in arr)
            {
                Console.WriteLine($"{i}");
            }
            //Console.WriteLine("ReverseArr");
            //arrRev = _circularBuffer.SafeToReverseArray();
            //foreach (var i in arrRev)
            //{
            //    Console.WriteLine($"{i}");
            //}
            _circularBuffer.Enqueue(4);
            Console.WriteLine("SimpleArr");
            arr = _circularBuffer.SafeToArray1();
            foreach (var i in arr)
            {
                Console.WriteLine($"{i}");
            }
            //Console.WriteLine("ReverseArr");
            //arrRev = _circularBuffer.SafeToReverseArray();
            //foreach (var i in arrRev)
            //{
            //    Console.WriteLine($"{i}");
            //}
            _circularBuffer.Enqueue(5);
            Console.WriteLine("SimpleArr");
            arr = _circularBuffer.SafeToArray1();
            foreach (var i in arr)
            {
                Console.WriteLine($"{i}");
            }
        }
    }
}
