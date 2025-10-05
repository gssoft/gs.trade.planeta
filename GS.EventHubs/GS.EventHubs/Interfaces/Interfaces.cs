using System;
using GS.Elements;
using GS.EventHubs.EventHub1;
using GS.EventHubs.EventHubT1;
using GS.Interfaces;

namespace GS.EventHubs.Interfaces
{
    public interface IHaveContent<out TContent> : IHaveKey
    {
        TContent Content { get; }
        int Count { get; }
    }
}
