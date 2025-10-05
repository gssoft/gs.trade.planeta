using System;
using GS.Contexts;
using GS.Interfaces;

namespace TcpIpSockets.TcpClient03
{
    public partial class TcpClient03 : Context3
    {
        public override void Init(IEventLog evl)
        {
            base.Init(evl);
        }
        public override void DeQueueProcess()
        {
            throw new NotImplementedException();
        }
    }
}
