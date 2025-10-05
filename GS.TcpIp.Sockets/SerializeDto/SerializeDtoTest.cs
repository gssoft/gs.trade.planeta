using System;
using System.Collections;
using System.Collections.Generic;
using GS.ConsoleAS;
using GS.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SerializeDto.Dto;

namespace SerializeDto
{
    [TestClass]
    public class SerializeDto
    {
        TcpDataClass TcpDataClass;
        TcpDataClass TcpDataClassEtalon;
        ArrayList ArrayList;
        string[] StringArray;
        List<string> ListGenericString;

        List<string> ListGenericString1000;
        string[] StringArray1000;

        MessageStr MessageStr;
        MessageLst MessageLst;

        [TestInitialize]
        public void SetUp()
        {
            TcpDataClass = new TcpDataClass
            {
                RouteKey = "Data",
                Data = @"0987654321qwertyuiop[]';lkjhgfdsazxcvbnm,./'"
            };
            TcpDataClassEtalon = new TcpDataClass
            {
                RouteKey = "Data",
                Data = @"0987654321qwertyuiop[]';lkjhgfdsazxcvbnm,./'"
            };
            ArrayList = new ArrayList
            {
                "Data",
                @"0987654321qwertyuiop[]';lkjhgfdsazxcvbnm,./",
                1,
                Guid.NewGuid()
            };
            StringArray = new[]
            {
                "Data",
                @"0987654321qwertyuiop[]';lkjhgfdsazxcvbnm,./"
            };
            ListGenericString = new List<string>
            {
                "Data",
                @"0987654321qwertyuiop[]';lkjhgfdsazxcvbnm,./"
            };

            StringArray1000 = new string[10000];
            for(var i=0; i<StringArray1000.Length;i++)
                StringArray1000[i] = Guid.NewGuid().ToString();

            MessageStr = new MessageStr(StringArray1000);

            ListGenericString1000 = new List<string>(10000);
            for (var i = 0; i < ListGenericString1000.Capacity; i++)
                ListGenericString1000.Add(Guid.NewGuid().ToString());

            MessageLst = new MessageLst(ListGenericString1000);

        }
        [TestMethod]
        public void DtoToBinarySerializationTest()
        {
            var input = TcpDataClass;
            Assert.IsNotNull(input, "TcpDataClass is Null");
            var bytes = BinarySerialization.SerializeToByteArray(input);
            Assert.IsNotNull(bytes, "Bytes is Null");
            ConsoleSync.WriteLineT($"Bytes.Count:{bytes.Length}");
            var answer = BinarySerialization.DeSerialize<TcpDataClass>(bytes);
            Assert.IsNotNull(answer, "tcpDataClass is Null");
            Assert.IsTrue(input.Compare(answer), "TcpDataClass non Equal");
        }
        [TestMethod]
        public void MessageStrToBinarySerializationTest()
        {
            var input = MessageStr;
            Assert.IsNotNull(input, "TcpDataClass is Null");
            var dt1 = DateTime.Now.TimeOfDay;
            var bytes = BinarySerialization.SerializeToByteArray(input);
            Assert.IsNotNull(bytes, "Bytes is Null");
            var answer = BinarySerialization.DeSerialize<MessageStr>(bytes);
            var dt2 = DateTime.Now.TimeOfDay;
            Console.WriteLine($"Elapsed: {(dt2 - dt1).ToString(@"ss\.fff")}");
            Assert.IsNotNull(answer, "tcpDataClass is Null");
            ConsoleSync.WriteLineT($"Bytes.Count:{bytes.Length}");
            // Assert.IsTrue(input.Compare(answer), "TcpDataClass non Equal");
        }
        [TestMethod]
        public void MessageLstToBinarySerializationTest()
        {
            var input = MessageLst;
            Assert.IsNotNull(input, "TcpDataClass is Null");
            var dt1 = DateTime.Now.TimeOfDay;
            var bytes = BinarySerialization.SerializeToByteArray(input);
            Assert.IsNotNull(bytes, "Bytes is Null");
            var answer = BinarySerialization.DeSerialize<MessageLst>(bytes);
            var dt2 = DateTime.Now.TimeOfDay;
            Console.WriteLine($"Elapsed: {(dt2 - dt1).ToString(@"ss\.fff")}");
            Assert.IsNotNull(answer, "tcpDataClass is Null");
            ConsoleSync.WriteLineT($"Bytes.Count:{bytes.Length}");
            // Assert.IsTrue(input.Compare(answer), "TcpDataClass non Equal");
        }

        [TestMethod]
        public void StringArrayToBinarySerializationTest()
        {
            Assert.IsNotNull(StringArray, "TcpDataClass is Null");
            var bytes = BinarySerialization.SerializeToByteArray(StringArray);
            Assert.IsNotNull(bytes, "Bytes is Null");
            ConsoleSync.WriteLineT($"Bytes.Count:{bytes.Length}");
            var answer = BinarySerialization.DeSerialize<string[]>(bytes);
            Assert.IsNotNull(answer, "Answer is Null");
            Assert.IsTrue(ListGenericString[0] == answer[0] && ListGenericString[1] == answer[1], "Non Equals");
        }
        [TestMethod]
        public void StringArray1000ToBinarySerializationTest()
        {
            var input = StringArray1000;
            Assert.IsNotNull(input, "TcpDataClass is Null");
            var dt1 = DateTime.Now.TimeOfDay;
            var bytes = BinarySerialization.SerializeToByteArray(input);
            Assert.IsNotNull(bytes, "Bytes is Null");
            var answer = BinarySerialization.DeSerialize<string[]>(bytes);
            var dt2 = DateTime.Now.TimeOfDay;
            Console.WriteLine($"Elapsed: {(dt2-dt1).ToString(@"ss\.fff")}");
            ConsoleSync.WriteLineT($"Bytes.Count:{bytes.Length}");
            Assert.IsNotNull(answer, "Answer is Null");
            for (var i = 0; i < input.Length; i++)
                Assert.IsTrue(input[i] == answer[i]);
        }
        [TestMethod]
        public void ListGenericStringToBinarySerializationTest()
        {
            var input = ListGenericString;
            Assert.IsNotNull(ListGenericString, "Input is Null");
            var bytes = BinarySerialization.SerializeToByteArray(input);
            Assert.IsNotNull(bytes, "Bytes is Null");
            ConsoleSync.WriteLineT($"Bytes.Count:{bytes.Length}");
            var answer = BinarySerialization.DeSerialize<List<string>>(bytes);
            Assert.IsNotNull(answer, "Answer is Null");
            Assert.IsTrue(input[0] == answer[0] && input[1] == answer[1], "Arrays Non Equals");
        }
        [TestMethod]
        public void ListGenericString1000ToBinarySerializationTest()
        {
            var input = ListGenericString1000;
            Assert.IsNotNull(input, "Input is Null");
            var dt1 = DateTime.Now.TimeOfDay;
            var bytes = BinarySerialization.SerializeToByteArray(input);
            Assert.IsNotNull(bytes, "Bytes is Null");
            var answer = BinarySerialization.DeSerialize<List<string>>(bytes);
            var dt2 = DateTime.Now.TimeOfDay;
            Console.WriteLine($"Elapsed: {(dt2 - dt1).ToString(@"ss\.fff")}");
            ConsoleSync.WriteLineT($"Bytes.Count:{bytes.Length}");
            Assert.IsNotNull(answer, "Answer is Null");
            for (var i=0; i < input.Count; i++)
                Assert.IsTrue(input[i] == answer[i]);
        }
        [TestMethod]
        public void ArrayListToBinarySerializationTest()
        {
            var input = ArrayList;
            Assert.IsNotNull(input, "Input is Null");
            var bytes = BinarySerialization.SerializeToByteArray(input);
            Assert.IsNotNull(bytes, "Bytes is Null");
            ConsoleSync.WriteLineT($"Bytes.Count:{bytes.Length}");
            var answer = BinarySerialization.DeSerialize<ArrayList>(bytes);
            Assert.IsNotNull(answer, "Answer is Null");
            Assert.IsTrue((string)input[0] == (string)answer[0] && (string)input[1] == (string)answer[1], $"Non Equal {answer[0]}, {answer[1]}");           
        }
        [TestMethod]
        public void DtoToJsonSerializationTest()
        {
            Assert.IsNotNull(TcpDataClass, "TcpDataClass is Null");
            var bytes = JsonConvert.SerializeObject(TcpDataClass);
            Assert.IsNotNull(bytes, "Bytes is Null");
            ConsoleSync.WriteLineT($"Bytes.Count:{bytes.Length}");
            var tcpclass = JsonConvert.DeserializeObject<TcpDataClass>(bytes);
            Assert.IsNotNull(tcpclass, "TcpClass is Null");
            Assert.IsTrue(tcpclass.Compare(TcpDataClassEtalon), "TcpClasses Non Equals");
        }
        [TestMethod]
        public void StringArrayToJsonSerializationTest()
        {
            Assert.IsNotNull(StringArray, "TcpDataClass is Null");
            var bytes = JsonConvert.SerializeObject(StringArray);
            Assert.IsNotNull(bytes, "Bytes is Null");
            ConsoleSync.WriteLineT($"Bytes.Count:{bytes.Length}");
            var strarr = JsonConvert.DeserializeObject<string[]>(bytes);
            Assert.IsNotNull(strarr, "StrArr is Null");
            Assert.IsTrue(StringArray[0] == strarr[0] && StringArray[1] == strarr[1], "Arrays Non Equals");
        }
    }
}
