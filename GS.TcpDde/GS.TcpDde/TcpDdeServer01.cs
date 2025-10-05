using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using GS.Contexts;
using GS.Elements;
using GS.Interfaces;
using GS.Interfaces.Dde;
using GS.Serialization;
using TcpIpSockets;
using TcpIpSockets.TcpServer03;
using TcpIpSockets.TcpServer04;

namespace GS.TcpDde
{
    public partial class TcpDdeServer01 : Context3
    {
        public string IpAdress { get; set; }
        public int Port { get; set; }
        public int SecondsWaitAfterStop { get; set; }
        public ITcpServer TcpServer { get; private set; }
        public IDde DdeServer { get; set; }
        public bool RealDdeServer;

        // for Serialization
        public TcpDdeServer01() { }

        public TcpDdeServer01(string ipAddress, int port)
        {
            //TcpServer = new TcpServer03(ipAddress, port) { Parent = this };
            TcpServer = new TcpServer04(ipAddress, port)
            {
                Parent = this,
                // IsSubscribeAvailable = false
                IsSubscribeAvailable = true
            };
            SecondsWaitAfterStop = 1;
            // RealDdeServer = false;
            // RealDdeServer = true;
            // IsProcessTaskInUse = true;
            IsProcessTaskInUse = false;
        }
        public override void Init(IEventLog evl)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";

            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m, "Start", ToString());

            // IsNeedEventHub = true;
            IsNeedEventLog = true;
            base.Init(evl);

            IsProcessTaskInUse = true;
            SetupProcessTask();
            if (RealDdeServer)
            {
                var xDoc = XDocument.Load(@"Init\Dde.xml");
                DdeServer = Builder.Build2<IDde, string, ITopicItem>(xDoc, "Dde");
                DdeServer.Parent = this;
                DdeServer.Init();
            }
            else
                DdeServer = new DdeEmulator01();

            DdeServer.TableChangesSendAction = ProcessTask.EnQueue;

            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m, "Finish",  ToString());
        }
        public override void Init()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";

            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m, "Start", ToString());

            // IsNeedEventHub = true;
            IsNeedEventLog = true;
            base.Init();

            TcpServer.Init();

            // IsProcessTaskInUse = true;
            SetupProcessTask();

            if (RealDdeServer)
            {
                var xDoc = XDocument.Load(@"Init\Dde.xml");
                DdeServer = Builder.Build2<IDde, string, ITopicItem>(xDoc, "Dde");
                DdeServer.Parent = this;
                DdeServer.Init();
            }
            else
                DdeServer = new DdeEmulator01 {Parent = this};
            if (IsProcessTaskInUse)
                DdeServer.TableChangesSendAction = ProcessTask.EnQueue;
            else
                DdeServer.TableChangesSendAction = SendMessage;

            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m, "Finish", ToString());
        }
        public override void Start()
        {
            try
            {
                DdeServer?.Start();
                TcpServer?.Start();

                if (IsProcessTaskInUse)
                    ProcessTask?.Start();
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        public void CloseClients()
        {
            try
            {
                TcpServer?.CloseClients();
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        public override void Stop()
        {
            try
            {
                DdeServer?.Stop();
                Thread.Sleep(SecondsWaitAfterStop);
                TcpServer?.Stop();
                Thread.Sleep(SecondsWaitAfterStop);

                if (IsProcessTaskInUse)
                    ProcessTask?.Start();

                base.Stop();
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }

        private void SendMessage(IList<string> lst)
        {
            if (!lst.Any()) return;
            var msg = ToTcpQuotes2(lst);
            SendMessage(msg);
        }
        public void SendMessage(string[] dto)
        {
            TcpServer?.SendMessage(dto);
        }
        public override void DeQueueProcess()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
        }
    }
}
