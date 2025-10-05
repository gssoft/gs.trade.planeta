using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.Elements;
using GS.Events;
using GS.Interfaces;

namespace GS.EventHubs.EventHub1
{
    public interface IEventHub : IElement1<string>
    {
        void Init(IEventLog evl);
        IEventHubItem Register(IEventHubItem ehi);
        void EnQueue(IEventArgs eventArgs);
        void EnQueue(object sender, IEventArgs eventArgs);
        void Subscribe(string key, EventHandler<IEventArgs> callback);
        void UnSubscribe(string key, EventHandler<IEventArgs> callback);
        void UnSubscribe(EventHandler<IEventArgs> callback);
        void Start();
        void Stop();
    }
    public interface IEventHubItem : IHaveKey<string>
    {
        void FireEvent(object sender, IEventArgs eventArgs);
        event EventHandler<IEventArgs> EventHandler;
        string Code { get; }
    }
}
