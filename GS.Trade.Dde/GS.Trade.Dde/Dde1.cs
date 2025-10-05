using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDEInfo;
using FTFHelper;
using GS.Elements;
using GS.Interfaces;

namespace GS.Trade.Dde
{
    public partial class Dde1 : Element1<string>
    {
        public string ServerName { get; set; }

        private static IEventLog _eventLog;

        private readonly InfoServer _ddeServer;
        private readonly string _name;

        private readonly Bfr.Dde.Topics _ddeTopics = new Bfr.Dde.Topics();

        private readonly object _ddeLocker = new object();

        public Dde1()
        {
            // _ddeTopics = new Topics();
            // _ddeLocker = new object();
        }

        public Dde1(string name)
        {
            _name = name;
        
            _ddeServer = new InfoServer(_name);

            _ddeServer.DataPoked += new DataPokedEventHandler(DataPoked);
            _ddeServer.StateChanged += new StateChangedEventHandler(StateChanged);

           // _ddeTopics = new Topics();
           // _ddeLocker = new object();
        }

        public override string Key
        {
            get { return Code; }
        }

        public override void Init(IEventLog evl)
        {
            if (evl == null)
            {
                throw new NullReferenceException("EventLog = null");
            }
            _eventLog = evl;
            _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "Dde", _name, "Initialization", _name, "");

            //_ddeServer = new InfoServer(ServerName);

            //_ddeServer.DataPoked += DataPoked;
            //_ddeServer.StateChanged += StateChanged;
        }
        public void Start()
        {
            if ( !_ddeServer.IsRegistered )
            {
                _ddeServer.Register();
                _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Dde Server", _name, "Start",
                                _name,"");
            }
            else
                _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TECHNOLOGY, "Dde Server", _name, "Start",
                                String.Format("Already Active Dde Server: {0}", _name), "");
        }
        public void Stop()
        {
            if (_ddeServer.IsRegistered)
            {
                _ddeServer.Disconnect();
                _ddeServer.Unregister();
                _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Dde Server", _name, "Stop",
                                 _name, "");
            }
        }
        public void Close()
        {
            if (_ddeServer.IsRegistered)
            {
                _ddeServer.Disconnect();
                _ddeServer.Unregister();
            }
            _ddeServer.StateChanged -= new StateChangedEventHandler(StateChanged);
            _ddeServer.DataPoked -= new DataPokedEventHandler(DataPoked);
        }
        private void DataPoked(object sender, DataPokeddEventArgs e)
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
                        strTemp += "<null>; ";
                    }
                    else if (e.Cells[ndxRow][ndxColl] is PREVIEWVALUE)
                    // Если значение ячейки типа "tdtSkip" - пропущенное значение
                    {
                        strTemp += "<preview>; ";
                    }
                    else
                    // Для остальных типов 
                    {
                        strTemp += e.Cells[ndxRow][ndxColl] + "; ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(e.Topic) && !string.IsNullOrWhiteSpace(strTemp))
                {
                    lock (_ddeLocker) _ddeTopics.OnPokedTopic(e.Topic, strTemp);
                }
            }
        }
        private void StateChanged(object sender, StateChangedEventArgs e)
        {
            // Locals

            var tps = string.Empty;

            if (e.TopicCount <= 0) return;

            var topics = _ddeServer.Topics;

            tps = topics.Aggregate(tps, (current, theItem) => current + (theItem + ";"));

            _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TECHNOLOGY, "Dde Server", _name, "State Changed",
                              String.Format("Dde Server:{0}; Topics:{1}; Count:{2}", _name, tps, e.TopicCount), "");
        } 

        public Bfr.Dde.Topics.Topic RegisterTopic(string topicKey, string topicName, Bfr.Dde.DdeChannelOnPoked cb)
        {
            return _ddeTopics.RegisterTopic(topicKey, topicName, cb);
        }
    }
}
