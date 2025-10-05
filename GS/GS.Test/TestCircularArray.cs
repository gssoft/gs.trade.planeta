using System;
using System.Collections.Generic;
using GS.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GS.Test
{
    [TestClass]
    public class TestCircularArray
    {
        private const int _size = 10;
        readonly CircularArray<int> _ca = new CircularArray<int>(_size);

        private void WriteArray<T>(T[] arr)
        {
            foreach (var t in arr)
                Console.Write($"{t} ");
        }

        private void WriteArray<T>(CircularArray<T> arr)
        {
            Console.WriteLine(
                $"Position:{arr.Position} Count:{arr.Count} Size:{arr.Size} First:{arr.First} Last:{arr.Last}");
            var _arCnt = arr.ToArray();
            Console.Write($"Cnt: ");
            foreach (var t in _arCnt)
                Console.Write($"{t} ");
            Console.WriteLine();
            var _arAll = arr.ToArrayAll();
            Console.Write($"All: ");
            foreach (var t in _arAll)
                Console.Write($"{t} ");
            Console.WriteLine();
            var _arRaw = arr.ToArrayRawValues();
            Console.Write($"Raw: ");
            foreach (var t in _arRaw)
                Console.Write($"{t} ");
            Console.WriteLine();

            Console.Write("FoE: ");
            foreach (var i in arr)
                Console.Write($"{i} ");
            Console.WriteLine();

            Console.Write("Idx: ");
            for (var i = 0; i < arr.Count; ++i)
                Console.Write($"{arr[i]} ");
            Console.WriteLine();

            Console.Write("Idx: ");
            for(var i = 0; i < arr.Size; ++i)
                Console.Write($"{arr[i]} ");
            Console.WriteLine();
        }
        private void WriteArray(CircularArray<int> arr, int[] arrDrct, int[] arrRvrs)
        {
            Console.WriteLine(
                $"Position:{arr.Position} Count:{arr.Count} Size:{arr.Size} First:{arr.First} Last:{arr.Last}");
            Console.Write($"Direct: ");
            foreach (var t in arrDrct)
                Console.Write($"{t} ");
            Console.WriteLine();
            Console.Write($"Revrse: ");
            foreach (var t in arrRvrs)
                Console.Write($"{t} ");
            Console.WriteLine();
            for (int i = 0, j = _ca.Count - 1; i < _ca.Count; ++i, --j)
            {
                if (i == _ca.Count - 1)
                    Console.WriteLine($"{arrDrct[i]}={arrRvrs[j]}; ");
                else
                    Console.Write($"{arrDrct[i]}={arrRvrs[j]}; ");
                Assert.IsTrue(arrDrct[i] == arrRvrs[j]);
            }
            Console.Write("FoE: ");
            foreach (var i in arr)
                Console.Write($"{i} ");
            Console.WriteLine();

            Console.Write("Idx: ");
            for (var i = 0; i < arr.Size; ++i)
                Console.Write($"{arr[i]} ");
            Console.WriteLine();
        }

        [TestInitialize]
        public void Init()
        {
            //_ca.Reset();
        }

        [TestMethod]
        public void Test01_Reset()
        {
            _ca.Reset();
            //Console.WriteLine(
            //    $"Position:{_ca.Position} Count:{_ca.Count} Size:{_ca.Size} First:{_ca.First} Last:{_ca.Last}");
            WriteArray(_ca);
            Assert.IsTrue(_ca.Count == 0, "Count not 0");
            Assert.IsTrue(_ca.Position == -1, "Position not -1");
            Assert.IsTrue(_ca.Size == _size, $"Size should be {_size}");
        }

        [TestMethod]
        public void Test02_Add()
        {
            _ca.Reset();
            for (var i = 1; i <= _size / 2; ++i)
                _ca.Add(i);
            WriteArray(_ca);
            for (var i = _size / 2 + 1; i <= _size * 3 / 2; ++i)
                _ca.Add(i);
            WriteArray(_ca);
            for (var i = 16; i <= 19; ++i)
                _ca.Add(i);
            WriteArray(_ca);
            for (var i = 20; i <= 26; ++i)
                _ca.Add(i);
            WriteArray(_ca);
        }

        [TestMethod]
        public void Test03_Add_ReverseCount()
        {
            _ca.Reset();
            for (var i = 1; i <= _size / 2; ++i)
                _ca.Add(i);
            var arrDrct = _ca.ToArray();
            var arrRvrs = _ca.ToArrayReverse();
            WriteArray(_ca, arrDrct, arrRvrs);

            for (var i = _size / 2 + 1; i <= _size * 3 / 2; ++i)
                _ca.Add(i);
            arrDrct = _ca.ToArray();
            arrRvrs = _ca.ToArrayReverse();
            WriteArray(_ca, arrDrct, arrRvrs);

            for (var i = 16; i <= 19; ++i)
                _ca.Add(i);
            arrDrct = _ca.ToArray();
            arrRvrs = _ca.ToArrayReverse();
            WriteArray(_ca, arrDrct, arrRvrs);

            for (var i = 20; i <= 26; ++i)
                _ca.Add(i);
            arrDrct = _ca.ToArray();
            arrRvrs = _ca.ToArrayReverse();
            WriteArray(_ca, arrDrct, arrRvrs);
        }

        [TestMethod]
        public void Test03_Add_ReverseAll()
        {
            _ca.Reset();
            for (var i = 1; i <= _size / 2; ++i)
                _ca.Add(i);
            var arrDrct = _ca.ToArrayAll();
            var arrRvrs = _ca.ToArrayReverseAll();
            WriteArray(_ca, arrDrct, arrRvrs);

            for (var i = _size / 2 + 1; i <= _size * 3 / 2; ++i)
                _ca.Add(i);
            arrDrct = _ca.ToArrayAll();
            arrRvrs = _ca.ToArrayReverseAll();
            WriteArray(_ca, arrDrct, arrRvrs);

            for (var i = 16; i <= 19; ++i)
                _ca.Add(i);
            arrDrct = _ca.ToArrayAll();
            arrRvrs = _ca.ToArrayReverseAll();
            WriteArray(_ca, arrDrct, arrRvrs);

            for (var i = 20; i <= 26; ++i)
                _ca.Add(i);
            arrDrct = _ca.ToArrayAll();
            arrRvrs = _ca.ToArrayReverseAll();
            WriteArray(_ca, arrDrct, arrRvrs);


        }
        [TestMethod]
        public void Test04_ToArrayRawValues()
        {
            var _caDateTime = new CircularArray<DateTime>(10);
            var _arRaw = _caDateTime.ToArrayRawValues();
            Console.Write($"Raw: ");
            foreach (var t in _arRaw)
                Console.Write($"{t} ");
            Console.WriteLine();
            Assert.IsTrue(_arRaw[0] == DateTime.MinValue);
        }
        //[TestMethod]
        //public void Test04_PopulateDefaultValue()
        //{
        //    var _caDateTime = new CircularArray<DateTime>(10);
        //    _caDateTime.Populate();
        //    WriteArray(_caDateTime);
        //    Assert.IsTrue(_caDateTime.ToArray()[0] == DateTime.MinValue);
        //}
    }
}
