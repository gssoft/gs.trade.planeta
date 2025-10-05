using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpIpSockets
{
    public enum ConnectionEnum
        { Unknown, Connected, NotConnected, CloseRequest, ConnectionRequest,
                                                CloseRequestAccepted, NeedToReconnect }

    public enum StopRequestEnum { Unknown, Local, Remote }
}
