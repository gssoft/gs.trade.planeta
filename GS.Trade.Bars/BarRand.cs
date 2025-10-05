using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Elements;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using GS.Trade.Data;
using GS.Works;

namespace GS.Trade.Bars
{
    public class BarRand : Element3<string, IEventArgs>
    {
        private readonly BarRandom _barRandom;

        public BarRand(string code, string name, string tradeBoard)
        {
            Code = code;
            Name = name;
            _barRandom = new BarRandom(100000, 1000)
            {
                Code = code,
                Name = name,
                TradeBoard = tradeBoard,
                DT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                    DateTime.Now.Hour, DateTime.Now.Minute, 0, 0)
            };
        }

        public override void Init(IEventLog evl)
        {
            base.Init(evl);
            Work = new Work1<IEventArgs>
            {
                Code = "Work." + Code,
                TimeInterval = 5000,
                MainFunc = () =>
                {
                    var tick = _barRandom.GetNextTick2();
                    //ConsoleSync.WriteLineT("BarRand: {0}; Tick: {1}", Code, tick.ToString());
                    FireChangedEvent("Quotes","Quote","AddNew",tick.ToString());
                    return true;
                }
            };  
        }
        public override string Key
        {
            get { return Code.HasValue() ? Code : GetType().FullName; }
        }

        public override void DeQueueProcess()
        {
            throw new NotImplementedException();
        }
    }
}
