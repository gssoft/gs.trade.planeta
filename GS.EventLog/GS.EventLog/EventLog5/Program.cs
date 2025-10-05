using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.EventLog;
using GS.Interfaces;
using GS.Serialization;

namespace EventLog5
{
    class Program
    {
        static void Main(string[] args)
        {
            const int max = 10;
            var evl = Builder.Build<EventLogs>(@"Init\EventLog.xml", "EventLogs");
            evl.Init();
            for (int i = 0; i < max; i++)
                evl.AddItem(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, "Test",
                    i.ToString(), i.ToString(), i.ToString());
                    
            Thread.Sleep(5000);
        }
    }
}
