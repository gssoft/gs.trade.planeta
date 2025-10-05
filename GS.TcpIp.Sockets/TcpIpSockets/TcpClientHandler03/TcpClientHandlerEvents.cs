using System;

namespace TcpIpSockets.TcpClientHandler03
{
    public partial class TcpClientHandler
    {
        public event EventHandler<string> SocketClosed;
        public event EventHandler<string[]> MessageReceived;

        protected virtual void OnSocketClosed(string e)
        {
            SocketClosed?.Invoke(this, e);
        }
        protected virtual void OnMessageReceived(string[] e)
        {
            MessageReceived?.Invoke(this, e);
        }
    }
    public partial class TcpClientHandler01
    {
        public event EventHandler<string> SocketClosed;
        public event EventHandler<string[]> MessageReceived;

        protected virtual void OnSocketClosed(string e)
        {
            SocketClosed?.Invoke(this, e);
        }
        protected virtual void OnMessageReceived(string[] e)
        {
            MessageReceived?.Invoke(this, e);
        }
    }
}
