using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.EventHubs;
using GS.EventHubs.EventHub1;
// using GS.Events;
using GS.Serialization;

// using EventHub = GS.EventHubs.EventHubT1.EventHub<System.Collections.Generic.List<>string>;
namespace TcpIpSockets.TcpServer04
{
    public partial class TcpServer04
    {
        public GS.EventHubs.EventHubT1.EventHub<List<string>> EventHubT1 { get; private set; }
        public GS.EventHubs.EventHub1.EventHub EventHub { get; private set; }

        public void SetupEventHub()
        {
            try
            {

                EventHub = Builder.Build2<EventHub>(@"Init\EventHub.xml", "EventHub");
                // EventHubT1/EventHub
                // EventHubT1 = Builder.Build<GS.EventHubs.EventHubT1.EventHub<List<string>>>(@"Init\EventHubT1.xml", "EventHubOfListOfString");
                EventHub.Parent = this;
                EventHub.Init();
                // ChangedEvent += EventHub.EnQueue;
                // EventHub?.Start();
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        public void SetupEventHub1()
        {
            try
            {
                EventHub = Builder.Build2<EventHub>(@"Init\EventHub.xml", "EventHub");
                EventHub.Parent = this;
                EventHub.Init();
                ChangedEvent += EventHub.EnQueue;
                // EventHub?.Start();
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
    }
}
