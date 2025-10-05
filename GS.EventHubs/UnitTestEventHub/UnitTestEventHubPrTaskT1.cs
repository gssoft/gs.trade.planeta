using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.EventHubs;
using GS.EventHubs.EventHubPrTskT1;
using GS.Extension;
using GS.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace UnitTestEventHub
{
    using EventHub = GS.EventHubs.EventHubPrTskT1.EventHub<System.Collections.Generic.List<string>>;

    [TestClass]
    public class UnitTestEventHubPrTaskT1
    {
        public EventHub<List<string>> EventHub;
        public List<EventHubItem<List<string>>> EventHubItemList;

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
            EventHubItemList = new List<EventHubItem<List<string>>>();
            MessagesReceived = new List<Message>();
            EventHub = new EventHub<List<string>>();
            Assert.IsNotNull(EventHub,"EventHub is Null"); 
        }

        [TestMethod]
        public void EventHubInit_Test()
        {
            EventHub = Builder.Build<EventHub<List<string>>>(@"Init\EventHubT1_ItemPrTsk01.xml", "EventHubOfListOfString");
            Assert.IsNotNull(EventHub, "EventHub == null");
            EventHub.Init();
            Console.WriteLine(
                $"EventHubType:{Environment.NewLine}" +
                $"TypeName: {EventHub.GetType().Name}{Environment.NewLine}" +
                $"TypeFullName: {EventHub.GetType().FullName}{Environment.NewLine}" +
                $"ToString(): {EventHub}");
            foreach(var i in EventHub.Items)
                Console.WriteLine($"EventHubItemType:{Environment.NewLine}" +
                                  $"TypeName: {i.GetType().Name}{Environment.NewLine}" +
                                  $"TypeFullName: {i.GetType().FullName}{Environment.NewLine}" +
                                  $"ToString(): {i}");
        }
        [TestMethod]
        public void DeSerializeEventHubT1_Test()
        {
            var xdoc = XDocument.Load(@"Init\EventHubT1_ItemPrTsk04.xml");
            var serializer = new XmlSerializer(typeof (EventHub<List<string>>));
            var reader = xdoc.CreateReader();
            var result = serializer.Deserialize(reader);
            reader.Close();
            var evh = result as EventHub<List<string>>;
            Assert.IsNotNull(evh, "EventHub is null");
            Console.WriteLine($"EventHubTypeName: {evh.GetType().Name}");
            Console.WriteLine($"EventHubTypeFullName: {evh.GetType().FullName}");
            Console.WriteLine($"ToString(): {evh}");
        }
        [TestMethod]
        public void Subscribe_UnSubscribed_Test()
        {
            EventHub = Builder
                .Build<EventHub<List<string>>>(@"Init\EventHubT1_ItemPrTsk01.xml", "EventHubOfListOfString");
            Assert.IsNotNull(EventHub, "EventHub == null");
            EventHub.Init();

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
            EventHub.EnQueue(msg);
            Assert.IsNull(MessageReceived, "MessageReceived is not null");
            Assert.IsTrue(MessagesReceived.Count == 0, "Messages Received Count != 0");
            // Subscribe
            EventHub.Subscribe("QuiKddESerVer.tICKERiNFO", MessageReceiver);
            MessageReceived = null;
            MessagesReceived.Clear();
            EventHub.EnQueue(msg);
            Assert.IsNotNull(MessageReceived, "MessageReceived is null");
            Assert.IsTrue(MessagesReceived.Count>0, "Messages Received Count = 0");
            MessageCompare(msg, MessageReceived);
            // UnSubscribe
            EventHub.UnSubscribe("QuiKddESerVer.tICKERiNFO", MessageReceiver);
            MessageReceived = null;
            MessagesReceived.Clear();
            EventHub.EnQueue(msg);
            Assert.IsNull(MessageReceived, "MessageReceived is not null");
            Assert.IsTrue(MessagesReceived.Count == 0, "Messages Received Count != 0");
        }
        [TestMethod]
        public void Send_Receive_Messages_PrTsk_Test01()
        {
            EventHub = Builder.Build<EventHub<List<string>>>(@"Init\EventHubT1_ItemPrTsk01.xml", "EventHubOfListOfString");
            Assert.IsNotNull(EventHub, "EventHub == null");
            EventHub.Init();
            Assert.IsFalse(EventHub.IsProcessTaskInUse, "IsProcessTaskInUse true");
            foreach (var i in EventHub.Items)
                Assert.IsFalse(i.IsProcessTaskInUse, $"ProcessTaskItem {i.Code} {i.IsProcessTaskInUse}");
            EventHub.Start();
            EventHub.Subscribe("QuiKddESerVer.tICKERiNFO", MessageReceiver);

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
                EventHub.EnQueue(i);
                Assert.IsNotNull(MessageReceived, "MessageReceived is null");
                Assert.IsTrue(MessagesReceived.Count > 0, "Messages Received Count = 0");
                MessageCompare(i, MessageReceived);
            }
            var k = 0;
            foreach (var i in messages)
            {
                MessageCompare(i, MessagesReceived[k++]);
            }
            EventHub.Stop();
        }
        [TestMethod]
        public void Send_Receive_Messages_PrTsk_Test02()
        {
            // HubProcessTask = false
            EventHub = Builder.Build<EventHub<List<string>>>(@"Init\EventHubT1_ItemPrTsk02.xml", "EventHubOfListOfString");
            Assert.IsNotNull(EventHub, "EventHub == null");
            EventHub.Init();
            Assert.IsFalse(EventHub.IsProcessTaskInUse, "IsProcessTaskInUse true");
            foreach (var i in EventHub.Items)
                Assert.IsTrue(i.IsProcessTaskInUse, $"ProcessTaskItem {i.Code} {i.IsProcessTaskInUse}");
            EventHub.Start();
            EventHub.Subscribe("QuiKddESerVer.tICKERiNFO", MessageReceiver);

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
                EventHub.EnQueue(i);
            }
            Thread.Sleep(1000);
            var k = 0;
            foreach (var i in messages)
            {
                MessageCompare(i, MessagesReceived[k++]);
            }
            EventHub.Stop();
        }
        [TestMethod]
        public void Send_Receive_Messages_PrTsk_Test03()
        {
            // HubProcessTask = false
            EventHub = Builder.Build<EventHub<List<string>>>(@"Init\EventHubT1_ItemPrTsk03.xml", "EventHubOfListOfString");
            Assert.IsNotNull(EventHub, "EventHub == null");
            EventHub.Init();
            Assert.IsTrue(EventHub.IsProcessTaskInUse, "IsProcessTaskInUse false");
            foreach (var i in EventHub.Items)
                Assert.IsFalse(i.IsProcessTaskInUse, $"ProcessTask is True {i.Code} {i.IsProcessTaskInUse}");
            EventHub.Start();
            EventHub.Subscribe("QuiKddESerVer.tICKERiNFO", MessageReceiver);

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
               EventHub.EnQueue(i);
            }
            Thread.Sleep(1000);
            var k = 0;
            foreach (var i in messages)
            {
                MessageCompare(i, MessagesReceived[k++]);
            }
            EventHub.Stop();
        }
        [TestMethod]
        public void Send_Receive_Messages_PrTsk_Test04()
        {
            // HubProcessTask = false
            EventHub = Builder.Build<EventHub<List<string>>>(@"Init\EventHubT1_ItemPrTsk04.xml", "EventHubOfListOfString");
            Assert.IsNotNull(EventHub, "EventHub == null");
            EventHub.Init();
            Assert.IsTrue(EventHub.IsProcessTaskInUse, "IsProcessTaskInUse false");
            foreach (var i in EventHub.Items)
                Assert.IsTrue(i.IsProcessTaskInUse, $"ProcessTask is false {i.Code} {i.IsProcessTaskInUse}");
            EventHub.Start();
            EventHub.Subscribe("QuiKddESerVer.tICKERiNFO", MessageReceiver);

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
                EventHub.EnQueue(i);
            }
            Thread.Sleep(2000);
            var k = 0;
            foreach (var i in messages)
            {
                MessageCompare(i, MessagesReceived[k++]);
            }
            EventHub.Stop();
        }
    }
}