using System;
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
    //public interface IDde
    //{
    //    void Init(IEventLog evl);

    //    void Start();
    //    void Stop();
    //    void Close();

    //    void RegisterTopic(string t, string i, Action<string> a);
    //    void RegisterDefaultCallBack(Action<string> a);

    //}
    // 2018.05.11
    // the Old; new always Dde21
    public class Dde23 : Element21<string, ITopicItem, Containers5.DictionaryContainer<string, ITopicItem>>,
        IDde2, IHaveCollection<string, ITopicItem>
    {
        public int Mode { get; set; }

        private InfoServer _ddeServer;
        private object _ddeLocker;

        public string ServerName {
            get { return Code; }
        }

        public override string Key
        {
            get { return Code; }
        }

        public Dde23()
        {
            Collection = new DictionaryContainer<string, ITopicItem>();
        }

        public override void Init(IEventLog evl)
        {
            base.Init(evl);

            _ddeServer = new InfoServer(Code);
            _ddeLocker = new object();

            if( Mode == 1)
                _ddeServer.DataPoked += DataPoked1;
            else if (Mode == 2)
                _ddeServer.DataPoked += DataPoked2;

            _ddeServer.StateChanged += StateChanged;

            //_ddeServer.DataPoked += DataPoked;
            //_ddeServer.StateChanged += StateChanged;

            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "DdeServer", Code, "Initialization", FullName, ToString());
            WhoAreYou();
        }

        private void DataPoked1(object sender, DataPokeddEventArgs e)
        {
            // Local
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
                    if(strTemp.HasValue())
                        strTemp += ";" + e.Cells[ndxRow][ndxColl];
                    else
                        strTemp = (string)e.Cells[ndxRow][ndxColl];
                    //}
                }
                //if (!string.IsNullOrWhiteSpace(e.Topic) && !string.IsNullOrWhiteSpace(strTemp))
                if(e.Topic.HasValue() && strTemp.HasValue())
                {
                    lock (_ddeLocker)
                        OnPokedTopic2(e.Topic, strTemp);
                }
            }
        }
        private void DataPoked2(object sender, DataPokeddEventArgs e)
        {
            int ndxRow;
            var ndxRowMax = e.Cells.Length;
            var sb = new StringBuilder();
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
                    if(strTemp.HasValue())
                        strTemp += ";" + e.Cells[ndxRow][ndxColl];
                    else
                        strTemp = (string)e.Cells[ndxRow][ndxColl];
                }
                if (strTemp.HasValue())
                    sb.AppendLine(strTemp);
            }
            //if (!string.IsNullOrWhiteSpace(e.Topic) && !string.IsNullOrWhiteSpace(strTemp))
            if (e.Topic.HasValue() && sb.Length>0)
            {
                lock (_ddeLocker)
                    OnPokedTopic2(e.Topic, sb.ToString());
            }
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

        //private void RegisterTopic(string topicItem)
        //{
        //    if (DefaultCallBack == null)
        //        throw new NullReferenceException("Topic=" + topicItem + " Is Not Found and DefaultCallBack == Null");
            
        //    Collection.AddOrGet(new TopicItem
        //    {
        //        TopicItemKey = topicItem,
        //        Action = DefaultCallBack               
        //    });
        //}
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

            Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, "Dde Server", ServerName, "State Changed",
                              String.Format("Dde Server:{0}; Topics:{1}; Count:{2}", ServerName, tps, e.TopicCount), "");
        }
        public void Start()
        {
            CheckCallBacks();

            if (!_ddeServer.IsRegistered)
            {
                _ddeServer.Register();
                
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Dde Server", ServerName, "Start",
                                ServerName, ToString());
            }
            else
                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, "Dde Server", ServerName, "Start",
                                String.Format("Already Active Dde Server: {0}", ServerName), ToString());
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
            
        }
    }

    //public class TopicItem : Element1<string>
    //{
    //    public string TopicItemKey;
    //    [XmlIgnore]
    //    public Action<string> Action;

    //    public override string Key
    //    {
    //        get { return TopicItemKey; }
    //    }
    //}
}


