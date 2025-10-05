using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.Elements;
using GS.EventHubs;
using GS.EventHubs.EventHubT1;
using GS.EventHubs.EventHubT2;
using GS.EventHubs.Interfaces;
using GS.Events;
// using GS.Events;
using GS.Serialization;

// using EventHub = GS.EventHubs.EventHubT1.EventHub<>;
namespace TcpIpSockets.TcpClientHandlerLst
{
    public partial class TcpServer04
    {
        public EventHub<List<string>> EventHub { get; private set; }

        public void SetupEventHub()
        {
            try
            {
                EventHub = Builder.Build<EventHub<List<string>>>(@"Init\EventHubLstT1.xml", "EventHubOfListOfString");
                EventHub.Parent = this;
                EventHub.Init();
                // ChangedEvent += EventHub.EnQueue;
                EventHub.Start();
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
    }
}
