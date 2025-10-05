using System;

namespace TcpIpSockets.TcpClientHandler03T
{
    public partial class TcpClientHandler<TMessage>
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
