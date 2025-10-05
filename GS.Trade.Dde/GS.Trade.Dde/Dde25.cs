using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
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

namespace GS.Trade.Dde
{
    public interface IDde25
    {
        bool IsNeedTopicName { get; }
        ChangesSendMode Mode { get; }
        Action<string> LineChangesSendAction { get; set; }
        Action<List<string>> TableChangesSendAction { get; set; }
    }

    // public enum ChangesSendMode : int { Line = 1, Table = 2 }

    // 2018.05.13 the new, not ready try rewrite with events
    public class Dde25 :    Element21<string, ITopicItem,
                            Containers5.DictionaryContainer<string, ITopicItem>>,
                            IDde25, IHaveCollection<string, ITopicItem>
    {
        public bool IsNeedTopicName { get; set; }
        public ChangesSendMode Mode { get; set; }
        private InfoServer _ddeServer;
        private object _ddeLocker;
        public string ServerName => Code;
        public override string Key => Code;
        [XmlIgnore]
        public Action<string> LineChangesSendAction { get; set; }
        [XmlIgnore]
        public Action<List<string>> TableChangesSendAction { get; set; }
        public Dde25()
        {
            Collection = new DictionaryContainer<string, ITopicItem>();
        }
        public override void Init(IEventLog evl)
        {
            base.Init(evl);

            _ddeServer = new InfoServer(Code);
            _ddeLocker = new object();

            if (Mode == ChangesSendMode.Line)
            {
                _ddeServer.DataPoked += DataPoked1;
            }
            else if (Mode == ChangesSendMode.Table)
            {
                _ddeServer.DataPoked += DataPoked2;
            }

            _ddeServer.StateChanged += StateChanged;

            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, 
                        "DdeServer", Code, "Initialization", FullName, ToString());
            WhoAreYou();
        }
        private void DataPoked1(object sender, DataPokeddEventArgs e)
        {
            try
            {
                int ndxRow;
                var ndxRowMax = e.Cells.Length;
                for (ndxRow = 0; ndxRow < ndxRowMax; ndxRow++)
                {
                    var ndxCollMax = e.Cells[ndxRow].Length;
                    var strTemp = String.Empty;
                    int ndxColl;
                    for (ndxColl = 0; ndxColl < ndxCollMax; ndxColl++)
                    {
                        if (e.Cells[ndxRow][ndxColl] == null)
                        // Если значение ячейки типа "tdtBlank" - неопределенное (пустое) значение
                        {
                            //strTemp += "<null>; ";
                            continue;
                        }
                        //else 
                        if (e.Cells[ndxRow][ndxColl] is PREVIEWVALUE)
                        // Если значение ячейки типа "tdtSkip" - пропущенное значение
                        {
                            //strTemp += "<preview>; ";
                            continue;
                        }
                        // else
                        // Для остальных типов 
                        //{
                        if (strTemp.HasValue())
                            strTemp += ";" + e.Cells[ndxRow][ndxColl];
                        else
                            strTemp = (string)e.Cells[ndxRow][ndxColl];
                        //}
                    }
                    if (!strTemp.HasValue())
                        continue;

                    if (IsNeedTopicName && e.Topic.HasValue())
                        strTemp = string.Concat(e.Topic, ";", strTemp);

                    LineChangesSendAction?.Invoke(strTemp);
                }
            }
            catch (Exception ex)
            {
                SendException(ex);
            }
        }
        private void DataPoked2(object sender, DataPokeddEventArgs e)
        {
            try
            {
                var list = new List<string>();
                int ndxRow;
                var ndxRowMax = e.Cells.Length;
                for (ndxRow = 0; ndxRow < ndxRowMax; ndxRow++)
                {
                    var ndxCollMax = e.Cells[ndxRow].Length;
                    var strTemp = String.Empty;
                    int ndxColl;
                    for (ndxColl = 0; ndxColl < ndxCollMax; ndxColl++)
                    {
                        if (e.Cells[ndxRow][ndxColl] == null)
                        // Если значение ячейки типа "tdtBlank" - неопределенное (пустое) значение
                        {
                            //strTemp += "<null>; ";
                            continue;
                        }
                        //else 
                        if (e.Cells[ndxRow][ndxColl] is PREVIEWVALUE)
                        // Если значение ячейки типа "tdtSkip" - пропущенное значение
                        {
                            //strTemp += "<preview>; ";
                            continue;
                        }
                        if (strTemp.HasValue())
                            strTemp += ";" + e.Cells[ndxRow][ndxColl];
                        else
                            strTemp = (string)e.Cells[ndxRow][ndxColl];
                    }
                    if (!strTemp.HasValue())
                        continue;

                    if (IsNeedTopicName && e.Topic.HasValue())
                        strTemp = string.Concat(e.Topic, ";", strTemp);

                    list.Add(strTemp);
                }
                if (list.Count > 0)
                    TableChangesSendAction?.Invoke(list);
            }
            catch (Exception ex)
            {
                SendException(ex);
            }
        }

        private void DataPoked3(object sender, DataPokeddEventArgs e)
        {
            var list = new List<string>();
            int ndxRow;
            var ndxRowMax = e.Cells.Length;
            for (ndxRow = 0; ndxRow < ndxRowMax; ndxRow++)
            {
                var ndxCollMax = e.Cells[ndxRow].Length;
                var strTemp = String.Empty;
                int ndxColl;
                for (ndxColl = 0; ndxColl < ndxCollMax; ndxColl++)
                {
                    if (e.Cells[ndxRow][ndxColl] == null)
                    // Если значение ячейки типа "tdtBlank" - неопределенное (пустое) значение
                    {
                        //strTemp += "<null>; ";
                        continue;
                    }
                    //else 
                    if (e.Cells[ndxRow][ndxColl] is PREVIEWVALUE)
                    // Если значение ячейки типа "tdtSkip" - пропущенное значение
                    {
                        //strTemp += "<preview>; ";
                        continue;
                    }
                    if (strTemp.HasValue())
                        strTemp += ";" + e.Cells[ndxRow][ndxColl];
                    else
                        strTemp = (string)e.Cells[ndxRow][ndxColl];
                }
                if (!strTemp.HasValue())
                    continue;

                if (Mode == ChangesSendMode.Line)
                    LineChangesSendAction(strTemp);
                else
                    list.Add(strTemp);
            }
            //if (e.Topic.HasValue() && list.Count > 0)
            if (Mode == ChangesSendMode.Table && list.Count > 0)
                    TableChangesSendAction(list);
        }

        private void OnPokedTopic(string topic, string data)
        {
            var top = Collection.GetByKey(topic);
            if (top != null)
            {
                if (top.Action != null)
                    top.Action(data);
                else
                {
                    if( DefaultCallBack != null)
                        top.Action = DefaultCallBack;
                    else
                        throw new NullReferenceException("Dde DefaultCallBack is Null");
                }
            }
            else
                RegisterTopic(topic);
        }
        private void OnPokedTopic2(string topic, string data)
        {
            var top = Collection.GetByKey(topic);
            if (top != null)
                top.Action(data);
            else
            {
                top = RegisterTopic(topic);
                top.Action(data);
            }
        }

        private void CheckCallBacks()
        {
            if (DefaultCallBack == null)
                throw new NullReferenceException("DefaultCallBack == Null");

            foreach (var i in Collection.Items.Where(i => i.Action == null))
            {
                i.Action = DefaultCallBack;
            }
        }
      
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
            // Locals

            var tps = string.Empty;

            if (e.TopicCount <= 0) return;

            var topics = _ddeServer.Topics;

            tps = topics.Aggregate(tps, (current, theItem) => current + (theItem + ";"));

            // 2018.05.12
            var status = _ddeServer.State;

            Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, "Dde Server", ServerName,
                $"State Changed To: {status.ToString()}",
                $"Dde Server:{ServerName}; Topics:{tps}; Count:{e.TopicCount}", "");
        }

        private string TopicsString => _ddeServer != null
            ? string.Join(";", _ddeServer.Topics)
            : string.Empty;

        public void Start()
        {
            // CheckCallBacks();

            if (!_ddeServer.IsRegistered)
            {
                _ddeServer.Register();
                
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Dde Server", ServerName, "Start",
                                ServerName, ToString());
            }
            else
                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, "Dde Server", ServerName, "Start",
                    $"Already Active Dde Server: {ServerName}", ToString());
        }
        public void Stop()
        {
            if (!_ddeServer.IsRegistered)
                return;
            _ddeServer.Disconnect();
            _ddeServer.Unregister();
            
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Dde Server", ServerName, "Stop",
                ServerName, ToString());
        }
        public void Close()
        {
            if (_ddeServer.IsRegistered)
            {
                _ddeServer.Disconnect();
                _ddeServer.Unregister();
            }

            _ddeServer.DataPoked -= DataPoked1;
            _ddeServer.DataPoked -= DataPoked2;
            
            _ddeServer.StateChanged -= StateChanged;
            //_ddeServer.DataPoked -= DataPoked;

            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Dde Server", ServerName, "Close",
                ServerName, ToString());

        }

        public override string ToString()
        {
            //var state = _ddeServer?.State.ToString() ?? "IS NULL";
            //var cnt = _ddeServer?.Topics.Length ?? 0;
            return $"Type: {GetType().FullName}, " +
                   $"ServerName: {ServerName}, " +
                   $"State: {_ddeServer?.State.ToString() ?? "IS NULL"}, " +
                   $"Mode: {Mode.ToString()}, " + 
                   $"Topics: {TopicsString}, " +
                   $"TopicsCnt: {_ddeServer?.Topics.Length ?? 0}";
        }
    }
}


