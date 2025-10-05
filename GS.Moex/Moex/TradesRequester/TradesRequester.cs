using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.ConsoleAS;
using GS.Extension;
using GS.Moex.Interfaces;

namespace Moex.TradesRequester
{
    public class TradesRequester
    {
        public event EventHandler<string> MessageEvent;
        public MoexTest MoexTest { get; set; }
        public string TickerBoard { get; set; }
        public string TickerCode { get; set; }
        [XmlIgnore]
        public IMoexTicker Ticker { get; set; }
        public int TradesRequestStartValue { get; set; }
        public int TradesRequestLimitValue { get; set; }
        public bool Verbose { get; set; }
        public int GetTradesDelayTimeInterval {get; set;}
        protected const int GetTradesDelayTimeIntervalDefault = 60;
        protected Task GetTradesSrvTask { get; set; }
        protected bool GetTradesSrvWorking { get; set; }
        [XmlIgnore]
        public AutoResetEvent AutoReset { get; set; }
        public int GetTradeDelaySec =>
            GetTradesDelayTimeInterval > 0
                ? GetTradesDelayTimeInterval
                : GetTradesDelayTimeIntervalDefault;
        public TradesRequester()
        {
            TradeList = new List<MoexTrade>();
            AutoReset = new AutoResetEvent(false);
        }
        [XmlIgnore]
        public List<MoexTrade> TradeList;
        private int _start, _limit, _path;
        public void Init()
        {
            var method = MethodBase.GetCurrentMethod().Name + "()";
            if (MoexTest == null) return;
            MoexTest.Init();
            MoexTest.MessageEvent += PrintMessage;
            if (TickerBoard.HasNoValue() || TickerCode.HasNoValue())
            {
                SendMessage(method, "TickerBoard or Ticker has no Value");
                return;
            }
            Ticker = MoexTest.GetTicker(TickerBoard, TickerCode);
            if (Ticker == null)
            {
                SendMessage(method, "Ticker is null");
                return;
            }
            _start = TradesRequestStartValue;
            _limit = TradesRequestLimitValue;
            _path = 0;

            SendMessage(method, "Completed");
            PrintSettings();
        }
        public void Finish()
        {
            var method = MethodBase.GetCurrentMethod().Name + "()";
            MoexTest.MessageEvent -= PrintMessage;
            // MoexTest.Finish()
            GetTradesSrvTask = null;
            SendMessage(method, "Service Finished");
            MessageEvent -= PrintMessage;
        }
        private void PrintMessage(object sender, string s)
        {
            ConsoleSync.WriteLineT(s);
        }
        public void GetTrades()
        {
            var method = MethodBase.GetCurrentMethod().Name + "()";            
            _path++;
            var trades = MoexTest.GetTrades(TickerBoard, TickerCode, _start, _limit);
            if (trades == null)
            {
                SendMessage(method, "Something Wrong with Trade Receiving.Return Trades is null");
                SendMessage(method, $"MainPath:{_path} TradesCnt:{TradeList.Count} StartValue:{_start}");
                return;
            }
            TradeList.AddRange(trades);
            _start += trades.Count;
            var tradelistcnt = TradeList.Count;
            var msg = tradelistcnt > 0
                    ? $"MainPath:{_path} TradesCnt:{tradelistcnt} [{TradeList[0].TradeNumber}]-[{TradeList[tradelistcnt - 1].TradeNumber}]"
                    : $"MainPath:{_path} TradesCnt:{tradelistcnt} StartValue:{TradesRequestStartValue}";
            SendMessage(method, msg);
        }
        public void StartGetTradesService()
        {
            var method = MethodBase.GetCurrentMethod().Name + "()";
            if (IsTaskRunning)
            {
                SendMessage(method, "Service Already Running");
                return;
            }
            GetTradesSrvWorking = true;
            GetTradesSrvTask = new Task(GetTradesService);
            GetTradesSrvTask.Start();
            SendMessage(method, "Create new GetTradesService Task");
            SendMessage(method, $"TaskStatus:{GetTradesSrvTask.Status}");
        }
        public void GetTradesService()
        {
            var method = MethodBase.GetCurrentMethod().Name + "()";

            SendMessage(method, "Try to Start Service");
            try
            {
                while (GetTradesSrvWorking)
                {
                    GetTrades();
                    AutoReset.WaitOne(GetTradeDelaySec * 1000);
                }
            }
            catch (Exception e)
            {
                SendMessage(method, e.ToString());
                StopGetTradesService();
            }
            SendMessage(method, "Service Finishing ...");
        }
        public void StopGetTradesService()
        {
            var method = MethodBase.GetCurrentMethod().Name + "()";
            GetTradesSrvWorking = false;
            AutoReset.Set();
            while (!IsTaskCompleted)
            {
                SendMessage(method, "Waiting for Service Completed");
                Delay(1);
            }
            Delay(1);
            //GetTradesSrvTask = null;
            //SendMessage(method, "Services is Completed. Good Bye");
            Finish();
        }
        private bool IsTaskCompleted => GetTradesSrvTask == null ||
                                       (GetTradesSrvTask != null && (
                                           GetTradesSrvTask.Status == TaskStatus.RanToCompletion ||
                                           GetTradesSrvTask.Status == TaskStatus.Canceled ||
                                           GetTradesSrvTask.Status == TaskStatus.Faulted));

        private bool IsTaskRunning => GetTradesSrvTask != null &&
                                        (GetTradesSrvTask.Status == TaskStatus.Running ||
                                         GetTradesSrvTask.Status == TaskStatus.WaitingForActivation ||
                                         GetTradesSrvTask.Status == TaskStatus.WaitingToRun);
        private static void Delay(int seconds) {Thread.Sleep(seconds * 1000);}
        public void SendMessage(string method, string msg)
        {
            if (Verbose) OnMessageEvent($"{method} {msg}");
        }
        protected virtual void OnMessageEvent(string s)
        {
            MessageEvent?.Invoke(this, s);
        }
        public void PrintSettings()
        {
            SendMessage("Settings", "");
            SendMessage("Ticker:", Ticker?.SecId);
            SendMessage("Board:", Ticker?.BoardId);
            SendMessage("GetTradesStartValue:", TradesRequestStartValue.ToString());
            SendMessage("GetTradesLimitValue:", TradesRequestLimitValue.ToString());
            SendMessage("Verbose:", Verbose.ToString());
            SendMessage("GetTradesDelay:", GetTradeDelaySec.ToString());
            SendMessage("ServicerWorkingStatus:", GetTradesSrvWorking.ToString());
        }
    }
}
