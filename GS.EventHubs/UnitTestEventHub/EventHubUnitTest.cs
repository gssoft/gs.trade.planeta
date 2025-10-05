using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.EventHubs;
using GS.EventHubs.EventHubT1;
using GS.EventHubs.EventHubT2;
using GS.Extension;
using GS.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestEventHub
{
    
    [TestClass]
    public class EventHubTest
    {
        public EventHub<string[]> EventHubStr;
        public List<GS.EventHubs.EventHubT1.EventHubItem<string[]>> EventHubStrItemList;

        public EventHub<List<string>> EventHubLst;
        public List<GS.EventHubs.EventHubT1.EventHubItem<List<string>>> EventHubItemList;

        public EventHub<MessageStr, string[]> EventHubStrT2;
        public EventHub<Message, List<string>> EventHubLstT2;  

        public Message MessageReceived;
        public List<Message> MessagesReceived; 

        private void MessageReceiver(object sender, List<string> message)
        {
            // PrintMessage(message);
            MessageReceived = new Message(message);
            MessagesReceived.Add(MessageReceived);            
        }
        private static void PrintMessage(List<string> message)
        {
            foreach(var i in message)
                Console.WriteLine(i);
        }

        private void MessageCompare(Message m1, Message m2)
        {
            Assert.IsTrue(m1.Key == m2.Key, "Key is Wrong"); 
            Assert.IsTrue(m1.Content.Count == m2.Content.Count, "Content Count is Wrong");
            Assert.IsTrue(m1.Content[0].TrimUpper() == m2.Content[0].TrimUpper(), "First item is Wrong");
            for (var i=1; i < m1.Content.Count; i++)
            {
                Assert.IsTrue(m1.Content[i] == m2.Content[i], $"Content is wrong. Item:{i} M1:{m1.Content[i]} M2:{m2.Content[i]}");
            }
        }
        [TestInitialize]
        public void Initialize()
        {
            EventHubItemList = new List<GS.EventHubs.EventHubT1.EventHubItem<List<string>>>();
            EventHubStrItemList = new List<GS.EventHubs.EventHubT1.EventHubItem<string[]>>();

            MessagesReceived = new List<Message>();
            EventHubLst = new EventHub<List<string>>();
            Assert.IsNotNull(EventHubLst,"EventHubLst is Null");
            EventHubStr = new EventHub<string[]>();
            Assert.IsNotNull(EventHubStr, "EventHubLst is Null");
        }
        [TestMethod]
        public void EvenHubItemsSerializeTest()
        {
            GS.Serialization.Do.Serialize("EventHubLst.xml", EventHubLst);

            var eventHubItem = new GS.EventHubs.EventHubT1.EventHubItem<List<string>> { Code = "DdeSever.Quotes" };
            GS.Serialization.Do.Serialize("EventHubLstItem.xml", eventHubItem);

            EventHubItemList.Clear();
            EventHubItemList.Add(new GS.EventHubs.EventHubT1.EventHubItem<List<string>> { Code = "DdeSever.Quotes" });
            EventHubItemList.Add(new GS.EventHubs.EventHubT1.EventHubItem<List<string>> { Code = "DdeSever.TickerInfo" });
            EventHubItemList.Add(new GS.EventHubs.EventHubT1.EventHubItem<List<string>> { Code = "DdeSever.OptionDesk" });

            GS.Serialization.Do.Serialize("EventHubLstItems.xml", EventHubItemList);

            EventHubLst.Register(new GS.EventHubs.EventHubT1.EventHubItem<List<string>> { Code = "DdeSever.Quotes" });
            EventHubLst.Register(new GS.EventHubs.EventHubT1.EventHubItem<List<string>> { Code = "DdeSever.Quotes" });
            EventHubLst.Register(new GS.EventHubs.EventHubT1.EventHubItem<List<string>> { Code = "DdeSever.Quotes" });

            GS.Serialization.Do.Serialize("EventHubLstLst.xml", EventHubLst);

            GS.Serialization.Do.Serialize("EventHubStr.xml", EventHubStr);

            var ehistr = new GS.EventHubs.EventHubT1.EventHubItem<string[]> { Code = "DdeSever.Quotes" };
            GS.Serialization.Do.Serialize("EventHubStrItem.xml", ehistr);

            EventHubStrItemList.Clear();
            EventHubStrItemList.Add(new GS.EventHubs.EventHubT1.EventHubItem<string[]> { Code = "DdeSever.Quotes" });
            EventHubStrItemList.Add(new GS.EventHubs.EventHubT1.EventHubItem<string[]> { Code = "DdeSever.TickerInfo" });
            EventHubStrItemList.Add(new GS.EventHubs.EventHubT1.EventHubItem<string[]> { Code = "DdeSever.OptionDesk" });

            GS.Serialization.Do.Serialize("EventHubStrItems.xml", EventHubStrItemList);

            EventHubStr.Register(new GS.EventHubs.EventHubT1.EventHubItem<string[]> { Code = "DdeSever.Quotes" });
            EventHubStr.Register(new GS.EventHubs.EventHubT1.EventHubItem<string[]> { Code = "DdeSever.Quotes" });
            EventHubStr.Register(new GS.EventHubs.EventHubT1.EventHubItem<string[]> { Code = "DdeSever.Quotes" });

            GS.Serialization.Do.Serialize("EventHubStrLst.xml", EventHubLst);
        }

        [TestMethod]
        public void EventHubInit_Test()
        {
            EventHubLst = Builder.Build<EventHub<List<string>>>(@"Init\EventHubLstT1.xml", "EventHubOfListOfString");
            Assert.IsNotNull(EventHubLst, "EventHubLst == null");
            EventHubLst.Init();
            Console.WriteLine(
                $"EventHubType:{Environment.NewLine}" +
                $"TypeName: {EventHubLst.GetType().Name}{Environment.NewLine}" +
                $"TypeFullName: {EventHubLst.GetType().FullName}{Environment.NewLine}" +
                $"ToString(): {EventHubLst}");
            foreach(var i in EventHubLst.Items)
                Console.WriteLine($"EventHubItemType:{Environment.NewLine}" +
                                  $"TypeName: {i.GetType().Name}{Environment.NewLine}" +
                                  $"TypeFullName: {i.GetType().FullName}{Environment.NewLine}" +
                                  $"ToString(): {i}");
        }
        [TestMethod]
        public void DeSerializeEventHub_LstStr_Test()
        {
            var xdoc = XDocument.Load(@"Init\EventHubLstT1.xml");
            var serializer = new XmlSerializer(typeof (EventHub<List<string>>));
            var reader = xdoc.CreateReader();
            var result = serializer.Deserialize(reader);
            reader.Close();
            var eventHubList = result as EventHub<List<string>>;
            Assert.IsNotNull(eventHubList, "EventHubLst is null");
            Console.WriteLine($"EventHubTypeName: {eventHubList.GetType().Name}");
            Console.WriteLine($"EventHubTypeFullName: {eventHubList.GetType().FullName}");
            Console.WriteLine($"ToString(): {eventHubList}");

            xdoc = XDocument.Load(@"Init\EventHubStrT1.xml");
            serializer = new XmlSerializer(typeof(EventHub<string[]>));
            reader = xdoc.CreateReader();
            result = serializer.Deserialize(reader);
            reader.Close();
            var eventHubStr = result as EventHub<string[]>;
            Assert.IsNotNull(eventHubStr, "EventHubLst is null");
            Console.WriteLine($"EventHubTypeName: {eventHubStr.GetType().Name}");
            Console.WriteLine($"EventHubTypeFullName: {eventHubStr.GetType().FullName}");
            Console.WriteLine($"ToString(): {eventHubStr}");
        }
        [TestMethod]
        public void EventHub_LstStr_Build_Test()
        {
            EventHubLst = Builder.Build<EventHub<List<string>>>(@"Init\EventHubLstT1.xml", "EventHubOfListOfString");
            Assert.IsNotNull(EventHubLst, "EventHubLst == null");
            EventHubLst.Init();
            Assert.IsTrue(EventHubLst.Items.Count() == 3, "Items != 3");
            foreach (var i in EventHubLst.Items) Console.WriteLine(i.ToString());

            EventHubStr = Builder.Build<EventHub<string[]>>(@"Init\EventHubStrT1.xml", "EventHubOfArrayOfString");
            Assert.IsNotNull(EventHubStr, "EventHubLst == null");
            EventHubStr.Init();
            Assert.IsTrue(EventHubLst.Items.Count() == 3, "Items != 3");
            foreach (var i in EventHubStr.Items) Console.WriteLine(i.ToString());
        }
        [TestMethod]
        public void Subscribe_UnSubscribed_Test()
        {
            EventHubLst = Builder.Build<EventHub<List<string>>>(@"Init\EventHubLstT1.xml", "EventHubOfListOfString");
            Assert.IsNotNull(EventHubLst, "EventHubLst == null");
            EventHubLst.Init();

            var str = Guid.NewGuid().ToString();
            var content = new List<string>
            {
                "QuikDdeSErVeR.tICkerInfo",
                 str
            };
            var msg = new Message(content);
            // Without Subscribe
            MessageReceived = null;
            MessagesReceived.Clear();
            EventHubLst.EnQueue(msg);
            Assert.IsNull(MessageReceived, "MessageReceived is not null");
            Assert.IsTrue(MessagesReceived.Count == 0, "Messages Received Count != 0");
            // Subscribe
            EventHubLst.Subscribe("QuiKddESerVer.tICKERiNFO", MessageReceiver);
            MessageReceived = null;
            MessagesReceived.Clear();
            EventHubLst.EnQueue(msg);
            Assert.IsNotNull(MessageReceived, "MessageReceived is null");
            Assert.IsTrue(MessagesReceived.Count>0, "Messages Received Count = 0");
            MessageCompare(msg, MessageReceived);
            // UnSubscribe
            EventHubLst.UnSubscribe("QuiKddESerVer.tICKERiNFO", MessageReceiver);
            MessageReceived = null;
            MessagesReceived.Clear();
            EventHubLst.EnQueue(msg);
            Assert.IsNull(MessageReceived, "MessageReceived is not null");
            Assert.IsTrue(MessagesReceived.Count == 0, "Messages Received Count != 0");
        }
        [TestMethod]
        public void Send_Receive_Messages_Test()
        {
            EventHubLst = Builder.Build<EventHub<List<string>>>(@"Init\EventHubLstT1.xml", "EventHubOfListOfString");
            Assert.IsNotNull(EventHubLst, "EventHubLst == null");
            EventHubLst.Init();
            EventHubLst.Subscribe("QuiKddESerVer.tICKERiNFO", MessageReceiver);

            var messages = (from i in Enumerable.Range(1, 100)
                select Guid.NewGuid().ToString()
                into str
                select new List<string>
                {
                    "QuikDdeSErVeR.tICkerInfo", str
                }
                into content
                select new Message(content)).ToList();

            MessagesReceived.Clear();
            foreach (var i in messages)
            {
                MessageReceived = null;
                EventHubLst.EnQueue(i);
                Assert.IsNotNull(MessageReceived, "MessageReceived is null");
                Assert.IsTrue(MessagesReceived.Count > 0, "Messages Received Count = 0");
                MessageCompare(i, MessageReceived);
            }
            var k = 0;
            foreach (var i in messages)
            {
                MessageCompare(i, MessagesReceived[k++]);
            }
        }

        [TestMethod]
        public void Send_Receive_Messages_PrTsk_Test()
        {
            EventHubLst = Builder.Build<EventHub<List<string>>>(@"Init\EventHub1_PrTsk.xml", "EventHubOfListOfString");
            Assert.IsNotNull(EventHubLst, "EventHubLst == null");
            Assert.IsTrue(EventHubLst.IsProcessTaskInUse, "IsProcessTaskInUse false");
            EventHubLst.Init();
            EventHubLst.Start();
            EventHubLst.Subscribe("QuiKddESerVer.tICKERiNFO", MessageReceiver);

            var messages = new List<Message>();
            foreach (var i in Enumerable.Range(1, 100))
            {
                var str = Guid.NewGuid().ToString();
                var content = new List<string>
                {
                    "QuikDdeSErVeR.tICkerInfo", str
                };
                messages.Add(new Message(content));
            }

            MessagesReceived.Clear();
            foreach (var i in messages)
            {
                //MessageReceived = null;
                EventHubLst.EnQueue(i);
                //Assert.IsNotNull(MessageReceived, "MessageReceived is null");
                //Assert.IsTrue(MessagesReceived.Count > 0, "Messages Received Count == 0");
                // MessageCompare(i, MessageReceived);
            }
            Thread.Sleep(1000);
            var k = 0;
            foreach (var i in messages)
            {
                MessageCompare(i, MessagesReceived[k++]);
            }
            EventHubLst.Stop();
        }
    }
}