using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.EventLog;
using GS.Interfaces;
using GS.Serialization;

namespace caWebEventLogTest
{
    class Program
    {
        private static IEventLogs _evl;
        private static Random _rand;
        static void Main(string[] args)
        {
            _evl = Builder.Build<EventLogs>(@"Init\EventLog.xml", "EventLogs");
            _evl.Init();
            _rand = new Random();
            for (var i = 0; i < 10000; i++)
            {
                var randseconds = _rand.Next(5, 50);
                Thread.Sleep(randseconds*100);
                _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Test from caApplication",
                    i.ToString(), i.ToString(), i.ToString(), i.ToString());
            }
        }
    }
}
