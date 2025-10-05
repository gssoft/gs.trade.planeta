using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Serializaion
{
    [TestClass]
    public class BinarySerializeTest
    {
        TradeDto _trade = null;
        OrderDto _order = null;
        EventArgsDto _eargsTrade = null;
        EventArgsDto _eargsOrder = null;

        [TestInitialize]
        public void Setup()
        {
            _trade = new TradeDto
            {
                Number = ulong.MaxValue,
                OrderNumber = ulong.MaxValue,
                Strategy = "Trade",
                Ticker = "Si"   
            };
            _eargsTrade = new EventArgsDto
            {
                Key = UInt64.MaxValue.ToString(),
                Entity = "Trade",
                DtoObject = _trade
            };
            _order = new OrderDto
            {
                Number = ulong.MaxValue,
                Strategy = "Order",
                Ticker = "Si"
            };
            _eargsOrder = new EventArgsDto
            {
                Key = UInt64.MaxValue.ToString(),
                Entity = "Order",
                DtoObject = _order
            };
        }

        [TestMethod]
        public void TradeSerializationTest()
        {
            System.IO.Stream tradeStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(tradeStream, _eargsTrade);
            Console.WriteLine($"StreamLength:{tradeStream.Length}");
            tradeStream.Seek(0, SeekOrigin.Begin);
            var tradeEargs = formatter.Deserialize(tradeStream);
            var trade = ((EventArgsDto)tradeEargs).DtoObject;
            Console.WriteLine(trade.ToString());
            Console.WriteLine(tradeEargs.ToString());
            Assert.AreEqual(_eargsTrade.Key, ((EventArgsDto)tradeEargs).Key);
            Assert.AreEqual(_eargsTrade.Entity, ((EventArgsDto)tradeEargs).Entity);
            Assert.AreEqual(_trade.ToString(), trade.ToString());

        }
        [TestMethod]
        public void OrderSerializationTest()
        {
            System.IO.Stream orderStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(orderStream, _eargsOrder);
            Console.WriteLine($"StreamLength:{orderStream.Length}");
            orderStream.Seek(0, SeekOrigin.Begin);
            var orderEargs = formatter.Deserialize(orderStream);
            var order = ((EventArgsDto)orderEargs).DtoObject;
            Console.WriteLine(order.ToString());
            Console.WriteLine(orderEargs.ToString());
            Assert.AreEqual(_eargsOrder.Key, ((EventArgsDto)orderEargs).Key);
            Assert.AreEqual(_eargsOrder.Entity, ((EventArgsDto)orderEargs).Entity);
            Assert.AreEqual(_order.ToString(), order.ToString());
        }
    }
}
