using System;
using System.Collections.Generic;
using GS.Elements;
using GS.Interfaces;

namespace GS.EventHubs.EventHubT2
{
    public interface IEventHubType<in TEventArgs, TContent> : IElement1<string>
    {
        void Init(IEventLog evl);
        IEventHubItem<TContent> Register(IEventHubItem<TContent> ehi);
        void EnQueue(TEventArgs args);
        void EnQueue(object sender, TEventArgs args);
        void Subscribe(string key, EventHandler<TContent> callback);
        void UnSubscribe(string key, EventHandler<TContent> callback);
        void UnSubscribe(EventHandler<TContent> callback);
        void Start();
        void Stop();
    }
    public interface IEventHubItem<TContent> : Containers5.IHaveKey<string>
    {
        void FireEvent(TContent content);
        event EventHandler<TContent> EventHandler;
        string Code { get; }
    }
}
