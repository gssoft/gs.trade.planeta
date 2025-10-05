using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.EventHubs.Interfaces;
using GS.Events;
using GS.ProcessTasks;

namespace TcpIpSockets
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
        //void SendMessage(string[] m);
        // void SendMessage(string routekey, string entity, string operation);
        void SendMessage(params string[] s);
        void Subscribe(string key, EventHandler<IEventArgs> callback);
        void Subscribe(string key, EventHandler<IEventArgs> callback, ITcpClientHandler tcpHandler);
        void UnSubscribe(EventHandler<IEventArgs> callback);
    }

    public interface ITcpServer<TContent> : IElement1<string>
    {
        string IpAddress { get; }
        int Port { get; }
        bool IsSubscribeAvailable { get; }
        string ServerFullName { get; }
        ITcpClientHandler<TContent> Register(ITcpClientHandler<TContent> tcpClientHandler);
        void RegisterTcpClient(ITcpClientHandler<TContent> tcpClientHandler);
        void UnRegisterTcpClient(string s);
        int GetClientNameAttemptsNumber { get; }
        void Init();
        void Start();
        void Stop();
        void CloseClients();
        void SendMessage(IHaveContent<TContent> m);
        void Subscribe(string key, EventHandler<TContent> callback);
        void Subscribe(string key, EventHandler<TContent> callback, ITcpClientHandler<TContent> tcpHandler);
        void UnSubscribe(EventHandler<TContent> callback);
    }
    public interface ITcpServer<in TEventArgs, TContent> : IElement1<string>
        where TEventArgs : IHaveContent<TContent>
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
        void SendMessage(TEventArgs m);
        void Subscribe(string key, EventHandler<TContent> callback);
        void Subscribe(string key, EventHandler<TContent> callback, ITcpClientHandler tcpHandler);
        void UnSubscribe(EventHandler<TContent> callback);
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
        void SendMessage(string[] m);
        bool SendMessage(string s1, string s2, string s3);
        void ClearSocket();
    }
    public interface ITcpClientHandler<TMessage> : IElement1<string>
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
        void SendMessage(object sender, TMessage message);
        void SendMessage(TMessage message);
        bool SendMessage(params string[] strings);
        void SendMessage(IHaveContent<TMessage> message);
        // void SendMessage(object m);
        // bool SendMessage(string s1, string s2, string s3);
        void ClearSocket();
    }
    public interface ITcpClientHandler<in TMessage, TContent> : IElement1<string>
        where TMessage : IHaveContent<TContent>, IHaveIndex<string>
    {
        event EventHandler<string> SocketClosed;
        event EventHandler<string[]> MessageReceived;

        ProcessTask<IHaveContent<TContent>> ProcessTask { get;}
        bool IsClientSocketConnected { get; }

        TcpClient ClientSocket { get; }

        string ClientName { get; }
        string ClientFullName { get; }
        string ServerFullName { get; }

        void Init();
        // void StartNew();
        void Start();
        void Stop();
        void SendMessage(object sender, TMessage message);
        void SendMessage(TMessage message);
        void SendMessage(IHaveContent<TContent> message);
        bool SendMessage(params string[] strings);
        // void SendMessage(object m);
        // bool SendMessage(string s1, string s2, string s3);
        void ClearSocket();
    }

    public interface IHaveIndex<out T>
    {
        T this[int index] { get; }
    }

    // ************************************************  
}
