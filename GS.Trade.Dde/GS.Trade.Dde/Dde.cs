using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DDEInfo;
using FTFHelper;
using GS.Containers5;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;
using GS.Interfaces.Collections;
using GS.Interfaces.Dde;

using ChangesSendMode = GS.Interfaces.Dde.ChangesSendMode;

namespace GS.Trade.Dde
{

    // 2018.05.13 the new, not ready try rewrite with eventhub
    public partial class Dde : Element21<string, ITopicItem,
                                Containers5.DictionaryContainer<string, ITopicItem>>,
                                IDde, IHaveCollection<string, ITopicItem>
    {
        public bool IsNeedTopicNameInTable { get; set; }
        public bool IsNeedTopicName { get; set; }
        // public bool IsTableMode => Mode == ChangesSendMode.Table;
        public ChangesSendMode Mode { get; set; }
        private InfoServer _ddeServer;
        public string ServerName => Code;
        public override string Key => Code;
        [XmlIgnore]
        public Action<string> LineChangesSendAction { get; set; }
        [XmlIgnore]
        public Action<List<string>> TableChangesSendAction { get; set; }
        public string DdeStatus => _ddeServer?.State.ToString() ?? "Unknown"; // eServerState.Unknown.ToString(); 
        public Dde()
        {
            Collection = new DictionaryContainer<string, ITopicItem>();
        }
        public override void Init()
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentAndMyTypeName, TypeName,
                                                        $"{m} Begin", ShortDescription, ToString());

                _ddeServer = new InfoServer(Code);

                switch (Mode)
                {
                    case ChangesSendMode.Line:
                        _ddeServer.DataPoked += DataPoked1;
                        SetupProcessTask1();
                        break;
                    case ChangesSendMode.Table:
                    case ChangesSendMode.Mixed:
                        _ddeServer.DataPoked += DataPoked2;
                        SetupProcessTask2();
                        break;
                }

                _ddeServer.StateChanged += StateChanged;

                //Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                //            "Init End", $"DdeServer: {Code}, SendMode: {Mode}", ToString());
                
                Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentAndMyTypeName, TypeName,
                                           $"{m} Finish", ShortDescription, ToString());
                WhoAreYou();
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        public override void Init(IEventLog evl)
        {
            base.Init(evl);
            Init();
        }
        // Mode = Line
       
      
        private ITopicItem RegisterTopic(string topicItem)
        {
            if (DefaultCallBack == null)
                throw new NullReferenceException("Topic=" + topicItem + " Is Not Found and DefaultCallBack == Null");
            
            return  Collection.AddOrGet(new TopicItem
            {
                TopicItemKey = topicItem,
                Action = DefaultCallBack
            });
        }

        public void RegisterTopic(string topic, string item, Action<string> cb)
        {
            if (topic.HasNoValue() || cb == null)
                throw new NullReferenceException("Topic is Empty and DefaultCallBack == Null");

            var top = Collection.GetByKey(topic.WithSqBrackets0() + item);
            if (top != null)
                top.Action = cb;
            else
                Collection.AddNew(new TopicItem
                {
                    TopicItemKey = topic.WithSqBrackets0() + item,
                    Action = cb,
                });
        }
        protected Action<string> DefaultCallBack;
        protected Action<IEnumerable<string>> FireListOfStringsAction;

        public void RegisterDefaultCallBack(Action<string> cb)
        {
            if (cb == null)
                throw new NullReferenceException(FullName + ": Dde DefaultCallBack is Null ");
            DefaultCallBack = cb;
        }
        private void StateChanged(object sender, StateChangedEventArgs e)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            var tps = string.Empty;
            // if (e.TopicCount <= 0) return;

            var topics = _ddeServer.Topics;

            tps = topics.Aggregate(tps, (current, item) => current + item + ";");
            // 2018.05.12
            var status = _ddeServer.State;

            Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, 
                $"{m}: {status}",  $"Topics:{tps} Count:{e.TopicCount}", ShortDescription);
        }

        private string TopicsString => _ddeServer != null
            ? string.Join(";", _ddeServer.Topics)
            : string.Empty;

        public void Start()
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            // CheckCallBacks();
            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,ParentTypeName, TypeName,
                                                    $"{m} Begin", ShortDescription, ToString());

            StartProcessTask();

            if (!_ddeServer.IsRegistered)
            {
                _ddeServer.Register();
            }
            else
                Evlm2(EvlResult.WARNING, EvlSubject.INIT, ParentTypeName, TypeName, $"{m}",
                        $"Already Active Dde Server: {ShortDescription}", ToString());

            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
                                                        $"{m} Finish", ShortDescription, ToString());
        }
        //public void Stop()
        //{
        //    Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,  "Stop Begin", ShortDescription, ToString());

        //    if (!_ddeServer.IsRegistered)
        //        return;

        //    _ddeServer.Disconnect();
        //    _ddeServer.Unregister();


            
        //    Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, "Stop Finish", ShortDescription, ToString());

        //    // StopProcessTask();
        //}
        public void Stop()
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
                $"{m} Begin", ShortDescription, ToString());

            if (_ddeServer.IsRegistered)
            {
                _ddeServer.Disconnect();
                _ddeServer.Unregister();
            }

            _ddeServer.DataPoked -= DataPoked1;
            _ddeServer.DataPoked -= DataPoked2;
            
            _ddeServer.StateChanged -= StateChanged;
            //_ddeServer.DataPoked -= DataPoked;

            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
                $"{m} Finish", ShortDescription, ToString());

            StopProcessTask();
        }

        public override string ToString()
        {
            //var state = _ddeServer?.State.ToString() ?? "IS NULL";
            //var cnt = _ddeServer?.Topics.Length ?? 0;
            return $"Type:{GetType().FullName} " +
                   $"ServerName:{ServerName} " +
                   $"State:{_ddeServer?.State.ToString() ?? "IS NULL"} " +
                   $"Mode:{Mode} " + 
                   $"Topics:{TopicsString} " +
                   $"TopicsCnt:{_ddeServer?.Topics.Length ?? 0}";
        }
        private string StateStr => _ddeServer != null ? $"State:{_ddeServer.State}" : "";
        private  string ShortDescription => $"DdeServer:{Code} {StateStr} Mode:{Mode} " +
                                            $"NeedTopic:{IsNeedTopicName} " +
                                            $"IsNeedTopicTable:{IsNeedTopicNameInTable}";
    }
}


