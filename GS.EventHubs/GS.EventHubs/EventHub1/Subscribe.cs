using System;
using System.Reflection;
using GS.Events;
using GS.Extension;
using GS.Interfaces;

namespace GS.EventHubs.EventHub1
{
    public partial class EventHub
    {
        public void Subscribe(string key, EventHandler<IEventArgs> callback)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                key = key.TrimUpper();
                var ev = GetByKey(key);
                if (ev == null)
                {
                    var evhi = new EventHubItem { Code = key };
                    evhi.EventHandler += callback;
                    Add(evhi);
                }
                else
                {
                    if (callback != null)
                        ev.EventHandler += callback;
                }
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                     $"{m}", $"Key: {key}", ToString());
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        public void UnSubscribe(string key, EventHandler<Events.IEventArgs> callback)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                if (callback == null)
                    return;
                key = key.TrimUpper();
                var ev = GetByKey(key);
                if (ev == null)
                    return;
                ev.EventHandler -= callback;

                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                     $"{m}", $"Key: {key}", ToString());
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        public void UnSubscribe(EventHandler<IEventArgs> callback)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                if (callback == null)
                    return;
                foreach (var ev in Items)
                {
                    ev.EventHandler -= callback;
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                        $"{m}", $"Key: {ev.Key}", ToString());
                }
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
    }
}
