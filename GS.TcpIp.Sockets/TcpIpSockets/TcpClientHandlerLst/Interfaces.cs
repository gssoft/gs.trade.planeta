using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Events;

namespace TcpIpSockets.TcpClientHandlerLst
{
    public interface ITcpServer : IElement1<string>
    {
        string IpAddress { get; }
        int Port { get; }
        bool IsSubscribeAvailable { get; }
        string ServerFullName { get; }
        ITcpClientHandler Register(ITcpClientHandler tcpClientHandler);
        void RegisterTcpClient(ITcpClientHandler tcpClientHandler);
        void UnRegisterTcpClient(string s);
        int GetClientNameAttemptsNumber { get; }
        void Init();
        void Start();
        void Stop();
        void CloseClients();
        void SendMessage(List<string> m);
        void SendMessage(params string[] s);
        void Subscribe(string key, EventHandler<List<string>> callback,
                                                ITcpClientHandler tcpHandler);
        void UnSubscribe(EventHandler<List<string>> callback);
    }
    public interface ITcpClientHandler : IElement1<string>
    {
        event EventHandler<string> SocketClosed;
        event EventHandler<string[]> MessageReceived;

        bool IsClientSocketConnected { get; }

        TcpClient ClientSocket { get; }
        string ClientName { get; }
        string ClientFullName { get; }
        string ServerFullName { get; }

        void Init();
        // void StartNew();
        void Start();
        void Stop();
        bool SendMessage(params string[] ss);
        void SendMessage(List<string> strlst);
        void ClearSocket();
    }
}
