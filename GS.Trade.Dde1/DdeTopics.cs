using System;
using System.Collections;
using System.Collections.Generic;
using GS.Interfaces;

namespace GS.Trade.Dde
{
    //public delegate void DdeChannelOnPoked(string s);

    public partial class Dde
    {
        public delegate void DdeChannelOnPoked(string s);

        public class Topics
        {
            private readonly Dictionary<string, Topic> _topicCollection = new Dictionary<string, Topic>();
            // private readonly List<DdeChannel> _channelCollection;

            public static int Capasity { get; private set; }

            public  Topics()
            {
                Capasity = 100;

               // _topicCollection = new Dictionary<string, Topic>();
            }
            private static bool ChIndexIsValid(int channelIndex)
            {
                return channelIndex >= 1 && channelIndex <= Capasity;
            }
            private static bool TryParseChannelIndex(string topicStr, string ddeData, out int channelIndex)
            {
                topicStr = topicStr.Replace("[", "");
                topicStr = topicStr.Replace("]", "");

                var bo = Int32.TryParse(topicStr, out channelIndex);
                if (!bo)
                {
                    _eventLog.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Dde", topicStr, "OnPoked",
                                     String.Format("Invalid Channel Index={0} Str={1}",
                                      channelIndex, topicStr), ddeData);
                    return false;
                }
                if (!ChIndexIsValid(channelIndex))
                {
                    _eventLog.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Dde", topicStr, "OnPoked",
                                     String.Format("Invalid Channel Index. MaxChIndex={2}. ChannelIndex={0} Str={1}",
                                      channelIndex, topicStr, Capasity), ddeData);
                    return false;
                }
                return true;
            }
            private static bool TryParseTopic(string topicStr, string ddeData, out string topicKey, out int channelIndex)
            {
                topicStr = topicStr.Replace("[", "");
                var split = topicStr.Split(new[] { ']' });

                topicKey = (split[0]);
                var indexStr = "[" + (split[1]).Trim() + "]";

                return TryParseChannelIndex(indexStr, ddeData, out channelIndex);
            }
            

            public void OnPokedTopic(string topicStr, string ddeData)
            {
                string topicKey;
                int channelIndex;

                // _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Dde", "New Data",
                //                   topicStr, ddeData);

                if ( !TryParseTopic(topicStr, ddeData, out topicKey, out channelIndex)) return;

                var topic = TryGetTopic(topicKey);
                if (topic != null) // Get Topic
                {
                    var channel = (Topic.Channel)topic.Channels[channelIndex];
                    //  Fire Callback
                    if (channel != null) channel.Fire(ddeData);
                    else
                    {

                        _eventLog.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Dde", topicStr, "OnPoked",
                                     String.Format("Unknown Channel={0};TopicStr={1};",
                                      channelIndex, topicStr), ddeData);

                        topic.RegisterChannel(channelIndex, "Ch" + channelIndex, topic.CallBack);
                    }
                }
                else
                {
                    _eventLog.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Dde", topicStr, "OnPoked",
                                     String.Format("Unknown TopicKey={0};TopicStr={1};",
                                     topicKey, topicStr), ddeData);

                    topic = new Topic(topicKey, "Unknown", null);
                    RegisterTopic(topic);
                }
            }
            // ******************** TryGetTopic Trim and ToUpper only in TryGetTopic() and in RegisterTopic()
            private Topic TryGetTopic(string topicKey)
            {
                Topic topic;
                return _topicCollection.TryGetValue(topicKey.Trim().ToUpper(), out topic) ? topic : null;
            }
            //************ Register Topic **********************************************
            private void RegisterTopic(Topic topic)
            {
                lock (_topicCollection)
                {
                    _topicCollection.Add(topic.Key.Trim().ToUpper(), topic);
                }
                _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Dde", "Dde", "Register New Topic",
                                    String.Format("{0}", topic), "");

            }
            public Topic RegisterTopic(string topicKey, string name, DdeChannelOnPoked cb)
            {
                var topic = TryGetTopic(topicKey);
                if (topic != null) // Get Topic
                {
                    return topic;
                }
                topic = new Topic(topicKey, name, cb);
                RegisterTopic(topic);
                return topic;
            }

            // ************************** Dde Topic *********************************
            public class Topic
            {
                public string Key { get; private set; }
                public string Name { get; private set; }
                private readonly DdeChannelOnPoked _callbackDdeChannelOnPoked;
                public DdeChannelOnPoked CallBack { get { return _callbackDdeChannelOnPoked; } } 

                private readonly List<Channel> _channelCollection;

                public IList Channels { get { return _channelCollection; } }

                //private readonly object _locker;

                public Topic(string key, string name, DdeChannelOnPoked cb)
                {
                    Key = key;
                    Name = name;
                    _callbackDdeChannelOnPoked = cb;

                    _channelCollection = new List<Channel>();
                    Capasity = 100;
                    for (var i = 0; i <= Capasity; i++)
                        _channelCollection.Add(null);

                    //_locker = new object();
                }
   
                public void OnPokedChannel(string topicStr, string ddeData)
                {
                    _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Dde", topicStr, "New Data",
                                       topicStr, ddeData);
                    int channelIndex;
                    if (!TryParseChannelIndex(topicStr, ddeData, out channelIndex)) return;

                    var channel = _channelCollection[channelIndex];
                    //  Fire Callback
                    if (channel != null) channel.Fire(ddeData);
                    else
                    {
                        channel = new Channel(channelIndex, "Ch" + channelIndex, null);
                        _eventLog.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Dde", topicStr, "OnPoked",
                                     String.Format("Unknown Channel={0} Str={1}",
                                      channelIndex, topicStr), ddeData);

                        RegisterChannel(channelIndex, channel);
                    }
                    // */    
                }
                //************* Register Channel ******************************
                private void RegisterChannel(int channelIndex, Channel ch)
                {
                    lock (_channelCollection)
                    {
                        _channelCollection[channelIndex] = ch;
                    }
                    _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Dde", "Dde", "Register New Channel",
                                    String.Format("TopicName={0};Index={1};", Name, channelIndex), ch.ToString());

                }
                public void RegisterChannel(int index, string name, DdeChannelOnPoked cb)
                {
                    Channel ch;
                    if ((ch = _channelCollection[index]) == null)
                    {
                        var channel = new Channel(index, name, cb);
                        RegisterChannel(index, channel);
                    }
                    else
                    {
                        _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TECHNOLOGY, "Dde", "Dde", "Register New Channel",
                                    String.Format("Index={0} is Busy", ch), "");
                    }
                }
                public Channel TryGetChannel(int index)
                {
                    return ChIndexIsValid(index) ? _channelCollection[index] : null;
                }

                public void ErrorCallback(string s)
                {
                    _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TECHNOLOGY, "Dde", "Dde", "OnPoked",
                                     "Unknown topic or Channel", s);
                }
                public override string ToString()
                {
                    return String.Format("Key={0}, Name={1}", Key, Name);
                }

// ******************************* Dde Channel == ddeItem *******************************************************
                public class Channel
                {
                    public int Index;
                    public string Name;

                    private readonly DdeChannelOnPoked _callbackDdeChannelOnPoked;

                    public Channel(int index, string name, DdeChannelOnPoked cb)
                    {
                        Index = index;
                        Name = name;
                        _callbackDdeChannelOnPoked = cb;
                    }
                    public void Fire(string s)
                    {
                        if (_callbackDdeChannelOnPoked != null) _callbackDdeChannelOnPoked(s);
                    }
                    public override string ToString()
                    {
                        return String.Format("Index={0}, Name={1}", Index, Name);
                    }
                }
            }
        }
        
    }     
}
