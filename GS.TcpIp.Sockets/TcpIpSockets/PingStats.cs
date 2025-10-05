using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;

namespace TcpIpSockets
{
    public class MessageStats
    {
        public const int PingTimeIntervalSecondsDefault = 15;
        public DateTime LastInteractionDateTime { get; private set; }
        public int PingTimeIntervalSeconds { get; set; }
        public int LastPingMarker { get; set; }
        public int LastPongMarker { get; set; }

        public int PingIncomeCnt { get; private set; }
        public int PingOutcomeCnt { get; private set; }
        public int PongIncomeCnt { get; private set; }
        public int PongOutcomeCnt { get; private set; }
        public int IncomeMessageCnt { get; private set; }
        public int OutcomeMessageCnt { get; private set; }
        public MessageStats()
        {
            LastInteractionDateTime = DateTime.Now;
        }
        public void Init()
        {
            if (PingTimeIntervalSeconds <= 0)
                PingTimeIntervalSeconds = PingTimeIntervalSecondsDefault;
        }
        public void IncOutcomeMessageCnt()
        {
            OutcomeMessageCnt++;
            LastInteractionDateTime = DateTime.Now;
        }
        public void IncIncomeMessageCnt()
        {
            IncomeMessageCnt++;
            LastInteractionDateTime = DateTime.Now;
        }
        public void SetLastInteractionDateTime(DateTime dt)
        {
            LastInteractionDateTime = dt;
        }
        public void IncIncomePingCnt()
        {
            PingIncomeCnt++;
            LastInteractionDateTime = DateTime.Now;
        }
        public void IncOutcomePingCnt()
        {
            PingOutcomeCnt++;
            LastInteractionDateTime = DateTime.Now;
        }
        public void IncIncomePongCnt()
        {
            PongIncomeCnt++;
            LastInteractionDateTime = DateTime.Now;
        }
        public void IncOutcomePongCnt()
        {
            PongOutcomeCnt++;
            LastInteractionDateTime = DateTime.Now;
        }
        public int GetNextPingMarker()
        {
            return ++LastPingMarker;
        }
        public int GetNextPongMarker()
        {
            return ++LastPongMarker;
        }
        public bool IsNeedToSendPing()
        {
            return DateTime.Now.IsGreaterThan(LastInteractionDateTime.AddSeconds(PingTimeIntervalSeconds));
        }
        public string PingStatStr => $"Pings In:{PingIncomeCnt} Out:{PingOutcomeCnt}";
        public string PongStatStr => $"Pongs In:{PongIncomeCnt} Out:{PongOutcomeCnt}";
        public string MessageStatStr => $"Messages In:{IncomeMessageCnt} Out:{OutcomeMessageCnt}";

        public override string ToString()
        {
            return $"Pings:{LastPingMarker} Pongs:{LastPongMarker} {LastInteractionDateTime.ToString(@"yy-MM-dd hh\:mm\:ss\.fff")}";
        }
    }
}
